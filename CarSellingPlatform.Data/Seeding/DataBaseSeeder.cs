using System.Text.Json;
using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarSellingPlatform.Data;

public static class DataBaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CarSellingPlatformDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed roles
        await SeedRolesAsync(roleManager);

        // Seed admin user
        await SeedAdminUserAsync(userManager);
        
        await ImportBrandsFromJsonAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager)
    {
        const string email = "admin@admin.com";
        const string password = "Test1234@";

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    public static async Task ImportBrandsFromJsonAsync(CarSellingPlatformDbContext context)
    {
        var path = Path.Combine("..", "CarSellingPlatform.Data", "Seeding", "Input", "Brands.json");
        string brandsJson = File.ReadAllText(path);
        var brands = JsonSerializer.Deserialize<List<Brand>>(brandsJson);

        if (brands != null && brands.Count > 0)
        {
            List<int> brandIds = brands.Select(b => b.Id).ToList();
            if (await context.Brands.AnyAsync(b => brandIds.Contains(b.Id)) == false)
            {
                await context.Brands.AddRangeAsync(brands);
                await context.SaveChangesAsync();
            }
        }
    }
}