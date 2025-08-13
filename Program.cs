using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CineMate.Data;
using CineMate.Data.Seed;      // може да остане
using CineMate.Data.Entities;
using CineMate.Services;
using Microsoft.AspNetCore.Builder.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Политики
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("OperatorOrAdmin", policy => policy.RequireRole("Administrator", "Operator"));
    options.AddPolicy("ClientOnly", policy => policy.RequireRole("Client"));
});

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// DbContext
builder.Services.AddDbContext<CineMateDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opts =>
{
    opts.Password.RequireDigit = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequiredLength = 6;
    opts.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<CineMateDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

//
// ===== SEED (еднократни операции при стартиране) =====
//
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // База
    var db = services.GetRequiredService<CineMateDbContext>();
    await db.Database.MigrateAsync(); // ако не ползваш миграции -> EnsureCreatedAsync()

    // ---- ИДЕМПОТЕНТЕН seed за градове и кина ----
    var plan = new Dictionary<string, string[]>
    {
        ["Sofia"] = new[]
        {
            "CineMate Mall of Sofia",
            "CineMate Paradise Center",
            "CineMate Serdika Center",
            "CineMate The Mall",
            "CineMate Bulgaria Mall"
        },
        ["Plovdiv"] = new[]
        {
            "CineMate Plovdiv Mall",
            "CineMate Markovo Tepe"
        },
        ["Varna"] = new[]
        {
            "CineMate Grand Mall",
            "CineMate Varna Mall"
        }
    };

    foreach (var kv in plan)
    {
        var cityName = kv.Key;
        var cinemaNames = kv.Value;

        // 1) намери ВСИЧКИ градове с това име (case-insensitive)
        var cities = await db.Cities
            .Where(c => c.Name.ToLower() == cityName.ToLower())
            .ToListAsync();

        // ако няма нито един -> създай един
        if (cities.Count == 0)
        {
            var newCity = new City { Name = cityName };
            db.Cities.Add(newCity);
            await db.SaveChangesAsync();
            cities.Add(newCity);
        }

        // 2) за ВСЕКИ град с това име гарантираме кината
        foreach (var city in cities)
        {
            foreach (var cinemaName in cinemaNames)
            {
                bool exists = await db.Cinemas.AnyAsync(x => x.Name == cinemaName && x.CityId == city.Id);
                if (!exists)
                {
                    db.Cinemas.Add(new Cinema
                    {
                        Name = cinemaName,
                        CityId = city.Id
                    });
                }
            }
        }
    }
    await db.SaveChangesAsync(); // запис на евентуално добавени кина

    // ===== Week program: 13–19 Oct 2025 =====
    {
        var startDate = new DateTime(2025, 10, 13);
        var endDate = new DateTime(2025, 10, 19);

        // часови слотове за прожекции
        var slots = new[]
        {
            new TimeSpan(12, 0, 0),
            new TimeSpan(15, 0, 0),
            new TimeSpan(18,30, 0),
            new TimeSpan(21,30, 0),
        };

        var movies = await db.Movies.OrderBy(m => m.Id).ToListAsync();
        var cinemas = await db.Cinemas.OrderBy(c => c.Id).ToListAsync();

        if (movies.Count == 0 || cinemas.Count == 0)
        {
            Console.WriteLine(">> Seed week program skipped: no movies or cinemas.");
        }
        else
        {
            int movieIndex = 0;

            foreach (var cinema in cinemas)
            {
                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    foreach (var slot in slots)
                    {
                        var start = date + slot; // точен старт

                        // пропусни, ако вече има такава прожекция за това кино в този час
                        bool exists = await db.Screenings
                            .AnyAsync(s => s.CinemaId == cinema.Id && s.StartTime == start);

                        if (exists) continue;

                        var movie = movies[movieIndex % movies.Count];

                        var screening = new Screening
                        {
                            CinemaId = cinema.Id,
                            MovieId = movie.Id,
                            StartTime = start
                        };

                        db.Screenings.Add(screening);
                        await db.SaveChangesAsync(); // за да получим screening.Id

                        // създай 7 реда × 10 места
                        var seats = new List<Seat>(7 * 10);
                        for (int row = 1; row <= 7; row++)
                            for (int num = 1; num <= 10; num++)
                                seats.Add(new Seat
                                {
                                    ScreeningId = screening.Id,
                                    Row = row,
                                    Number = num,
                                    IsAvailable = true
                                });

                        db.Seats.AddRange(seats);
                        await db.SaveChangesAsync();

                        movieIndex++;
                    }
                }
            }

            Console.WriteLine(">> Week program (13–19.10.2025) seeded.");
        }
    }

    {
        var posterProp = typeof(Movie).GetProperty("PosterUrl");
        if (posterProp != null && posterProp.PropertyType == typeof(string))
        {
            var posterMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Superman (2025)"] = "~/images/posters/superman-2025.jpg",
                ["The Fantastic Four: First Steps"] = "~/images/posters/the-fantastic-four-first-steps.jpg",
                ["28 Years Later"] = "~/images/posters/28-years-later.jpg",
                ["Now You See Me: Now You Don't"] = "~/images/posters/now-you-see-me-now-you-dont.jpg",
                ["Mission: Impossible – Retribution"] = "~/images/posters/mission-impossible-retribution.jpg",
                ["Sinners"] = "~/images/posters/sinners.jpg",
                ["Zootopia 2"] = "~/images/posters/zootopia-2.jpg",
                ["The Smurfs"] = "~/images/posters/the-smurfs.jpg",
                ["Jurassic World: Rebirth"] = "~/images/posters/jurassic-world-rebirth.jpg",
                ["F1"] = "~/images/posters/f1.jpg",
                ["Lilo & Stitch (2025)"] = "~/images/posters/lilo-stitch-2025.jpg"
            };

            var allMovies = await db.Movies.ToListAsync();
            bool changed = false;

            foreach (var m in allMovies)
            {
                if (m == null) continue;
                var title = m.Title ?? string.Empty;

                if (posterMap.TryGetValue(title, out var path))
                {
                    var current = (string?)posterProp.GetValue(m);
                    if (string.IsNullOrWhiteSpace(current))
                    {
                        posterProp.SetValue(m, path);
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                await db.SaveChangesAsync();
                Console.WriteLine(">> Movie posters seeded into PosterUrl.");
            }
        }
        else
        {
            // Нямаш колона PosterUrl – всичко е ок, view-то може да работи с локален мапинг.
            Console.WriteLine(">> Movie.PosterUrl property not found – skipping poster seed.");
        }
    }



    // (по желание) Лог на connection string-а
    var config = services.GetRequiredService<IConfiguration>();
    Console.WriteLine($">>> Using DefaultConnection = {config.GetConnectionString("DefaultConnection")}");

    // Роли за Identity
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Administrator", "Operator", "Client" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

//
// ===== HTTP pipeline =====
//
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages(); // Identity UI
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
