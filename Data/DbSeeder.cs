using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Models;

namespace TahibTraders.Data
{
    public static class DbSeeder
    {
        public const string AdminRole = "Admin";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<AppDbContext>();
            var config = services.GetRequiredService<IConfiguration>();
            var env = services.GetRequiredService<IHostEnvironment>();

            await db.Database.MigrateAsync();

            // Default categories
            if (!await db.Categories.AnyAsync())
            {
                db.Categories.AddRange(
                    new Category { Name = "Groceries" },
                    new Category { Name = "Beverages" },
                    new Category { Name = "Snacks" },
                    new Category { Name = "Personal Care" },
                    new Category { Name = "Household" });
                await db.SaveChangesAsync();
            }

            // Demo offers and products are created only while developing,
            // so the live website never shows fake data.
            if (env.IsDevelopment() && !await db.Offers.AnyAsync())
            {
                db.Offers.AddRange(
                    new Offer { Tag = "This week", Title = "Atta 10 kg", Text = "Special price: Rs. 1,650" },
                    new Offer { Tag = "Combo", Title = "Cooking Oil 5 L", Text = "Get 1 kg sugar free" });

                var groceries = await db.Categories.FirstAsync(c => c.Name == "Groceries");
                var beverages = await db.Categories.FirstAsync(c => c.Name == "Beverages");
                db.Products.AddRange(
                    new Product { Name = "Atta 10 kg", Price = 1650, CategoryId = groceries.Id },
                    new Product { Name = "Basmati Rice 5 kg", Price = 1500, CategoryId = groceries.Id },
                    new Product { Name = "Cold Drink 1.5 L", Price = 220, CategoryId = beverages.Id });
                await db.SaveChangesAsync();
            }

            await SeedAdminAsync(services, config);
        }

        private static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
        {
            var email = config["Admin:Email"];
            var password = config["Admin:Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

            var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
            var users = services.GetRequiredService<UserManager<IdentityUser>>();

            if (!await roles.RoleExistsAsync(AdminRole))
                await roles.CreateAsync(new IdentityRole(AdminRole));

            var existing = await users.FindByEmailAsync(email);

            if (existing == null)
            {
                // First start: create the admin user
                var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await users.CreateAsync(user, password);
                if (result.Succeeded) await users.AddToRoleAsync(user, AdminRole);
            }
            else if (config.GetValue<bool>("Admin:ResetPassword"))
            {
                // Recovery mode: set the password from configuration and unlock the account
                var token = await users.GeneratePasswordResetTokenAsync(existing);
                await users.ResetPasswordAsync(existing, token, password);
                await users.SetLockoutEndDateAsync(existing, null);
                await users.ResetAccessFailedCountAsync(existing);
                if (!await users.IsInRoleAsync(existing, AdminRole))
                    await users.AddToRoleAsync(existing, AdminRole);
            }
        }
    }
}