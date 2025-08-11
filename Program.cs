using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CineMate.Data;
using CineMate.Data.Seed;      // може да остане
using CineMate.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// Политики
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("OperatorOrAdmin", policy => policy.RequireRole("Administrator", "Operator"));
    options.AddPolicy("ClientOnly", policy => policy.RequireRole("Client"));
});

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
