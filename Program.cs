using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CineMate.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Administrator"));

    options.AddPolicy("OperatorOrAdmin", policy =>
        policy.RequireRole("Administrator", "Operator"));

    options.AddPolicy("ClientOnly", policy =>
        policy.RequireRole("Client"));
});

// 1) Добавяме IdentityDbContext
builder.Services.AddDbContext<CineMateDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Конфигурираме Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opts => {
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
    // (по желание: options.ExpireTimeSpan = TimeSpan.FromDays(14);)
});


// 3) MVC + Razor Pages (за Login/Register UI)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    Console.WriteLine(">>> Using DefaultConnection = "
      + config.GetConnectionString("DefaultConnection"));
}
// При startup ако не си го правил: създай ролите ако липсват
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Administrator", "Operator", "Client" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
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

app.MapRazorPages();    // Identity UI
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
