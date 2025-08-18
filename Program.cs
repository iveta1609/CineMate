using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CineMate.Data;
using CineMate.Data.Entities;
using CineMate.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Administrator"));
    options.AddPolicy("OperatorOrAdmin", p => p.RequireRole("Administrator", "Operator"));
    options.AddPolicy("ClientOnly", p => p.RequireRole("Client"));
});

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddDbContext<CineMateDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login";
    o.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<CineMateDbContext>();

    await db.Database.MigrateAsync();

    var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Administrator", "Operator", "Client" })
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));

    await SeedStaticAsync(db);
}

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

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


static async Task SeedStaticAsync(CineMateDbContext db)
{
    if (!await db.Cities.AnyAsync())
    {
        db.Cities.AddRange(
            new City { Name = "Sofia" },
            new City { Name = "Plovdiv" },
            new City { Name = "Varna" }
        );
        await db.SaveChangesAsync();
    }

    var plan = new Dictionary<string, string[]>
    {
        ["Sofia"] = new[] { "CineMate Mall of Sofia", "CineMate Paradise Center", "CineMate Serdika Center", "CineMate The Mall", "CineMate Bulgaria Mall" },
        ["Plovdiv"] = new[] { "CineMate Plovdiv Mall", "CineMate Markovo Tepe" },
        ["Varna"] = new[] { "CineMate Grand Mall", "CineMate Varna Mall" }
    };

    foreach (var (cityName, Cinemas) in plan)
    {
        var city = await db.Cities.FirstOrDefaultAsync(c => c.Name == cityName);
        if (city is null) continue;

        foreach (var cn in Cinemas)
            if (!await db.Cinemas.AnyAsync(x => x.CityId == city.Id && x.Name == cn))
                db.Cinemas.Add(new Cinema { Name = cn, CityId = city.Id });
    }
    await db.SaveChangesAsync();

    if (!await db.Movies.AnyAsync())
    {
        db.Movies.AddRange(
            new Movie { Title = "Superman (2025)", Genre = "Action", DurationMinutes = 130, ReleaseYear = 2025 },
            new Movie { Title = "The Fantastic Four: First Steps", Genre = "Action", DurationMinutes = 125, ReleaseYear = 2025 },
            new Movie { Title = "28 Years Later", Genre = "Thriller", DurationMinutes = 122, ReleaseYear = 2025 },
            new Movie { Title = "Zootopia 2", Genre = "Animation", DurationMinutes = 110, ReleaseYear = 2025 }
        );
        await db.SaveChangesAsync();
    }

    var startDate = new DateTime(2025, 10, 13);
    var endDate = new DateTime(2025, 10, 19);
    var slots = new[] { new TimeSpan(12, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(18, 30, 0), new TimeSpan(21, 30, 0) };

    var movies = await db.Movies.OrderBy(m => m.Id).ToListAsync();
    var cinemas = await db.Cinemas.OrderBy(c => c.Id).ToListAsync();
    if (movies.Count > 0 && cinemas.Count > 0)
    {
        int mi = 0;
        foreach (var cinema in cinemas)
        {
            for (var d = startDate.Date; d <= endDate.Date; d = d.AddDays(1))
                foreach (var slot in slots)
                {
                    var start = d + slot;
                    if (await db.Screenings.AnyAsync(s => s.CinemaId == cinema.Id && s.StartTime == start))
                        continue;

                    var movie = movies[mi++ % movies.Count];
                    var s = new Screening { CinemaId = cinema.Id, MovieId = movie.Id, StartTime = start };
                    db.Screenings.Add(s);
                    await db.SaveChangesAsync();

                    if (!await db.Seats.AnyAsync(x => x.ScreeningId == s.Id))
                    {
                        var seats = new List<Seat>(70);
                        for (int r = 1; r <= 7; r++)
                            for (int n = 1; n <= 10; n++)
                                seats.Add(new Seat { ScreeningId = s.Id, Row = r, Number = n, IsAvailable = true });
                        db.Seats.AddRange(seats);
                        await db.SaveChangesAsync();
                    }
                }
        }
    }

    var posterProp = typeof(Movie).GetProperty("PosterUrl");
    if (posterProp?.PropertyType == typeof(string))
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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

        var all = await db.Movies.ToListAsync();
        bool changed = false;
        foreach (var m in all)
            if (m != null && map.TryGetValue(m.Title ?? "", out var p) &&
                string.IsNullOrWhiteSpace((string?)posterProp.GetValue(m)))
            { posterProp.SetValue(m, p); changed = true; }
        if (changed) await db.SaveChangesAsync();
    }

}
