using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Services;
using TahibTraders.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ImageService>();

var connectionString = "Server=SERVER_ADDRESS;Database=db73030;User Id=db73030;Password=NAYA_PASSWORD;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
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
    var cs = builder.Configuration.GetConnectionString("DefaultConnection") ?? "NULL";
    var safe = System.Text.RegularExpressions.Regex.Replace(cs, "Password=[^;]*", "Password=***");
    File.WriteAllText(
        Path.Combine(AppContext.BaseDirectory, "startup-error.txt"),
        "CONNECTION USED: " + safe + "\n\n" + ex.ToString());
}

app.Run();
