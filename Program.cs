using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Services;
using TahibTraders.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=HOST;Database=DBNAME;User Id=USER;Password=PASSWORD;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ImageService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 15;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

try
{
    using var scope = app.Services.CreateScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}
catch (Exception ex)
{
    File.WriteAllText(
        Path.Combine(AppContext.BaseDirectory, "startup-error.txt"),
        ex.ToString());
}

app.Run();
