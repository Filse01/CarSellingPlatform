using System.Text.Json;
using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
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
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Seed roles
        await SeedRolesAsync(roleManager);
        
        // Seed admin user
        await SeedAdminUserAsync(userManager);
        
        await ImportBrandsFromJsonAsync(context);
        
        await ImportCategoriesFromJsonAsync(context);
        
        await ImportFuelTypesFromJsonAsync(context);
        
        await ImportTransmissionsFromJsonAsync(context);
        
        await ImportEnginesFromJsonAsync(context);
        
        await ImportCarsFromJsonAsync(context);
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

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string email = "admin@admin.com";
        const string password = "Test1234@";
        const string firstName = "Admin";
        const string lastName = "Admin";
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var adminUser = new ApplicationUser()
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName,
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
            foreach (var brand in brands)
            {
                if (await context.Brands.AnyAsync(e => brand.Name == e.Name) == false)
                {
                    await context.Brands.AddAsync(brand);
                }
            }
            await context.SaveChangesAsync();
        }
    }

    public static async Task ImportCategoriesFromJsonAsync(CarSellingPlatformDbContext context)
    {
        var path = Path.Combine("..", "CarSellingPlatform.Data", "Seeding", "Input", "Categories.json");
        string categoriesJson = File.ReadAllText(path);
        var categories = JsonSerializer.Deserialize<List<Category>>(categoriesJson);
        if (categories != null && categories.Count > 0)
        {
            List<string> categoriesName = categories.Select(c => c.Name).ToList();
            if (await context.Categories.AnyAsync(c => categoriesName.Contains(c.Name)) == false)
            {
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task ImportTransmissionsFromJsonAsync(CarSellingPlatformDbContext context)
    {
        var path = Path.Combine("..", "CarSellingPlatform.Data", "Seeding", "Input","Transmissions.json");
        string transmissionsJson = File.ReadAllText(path);
        var transmissions = JsonSerializer.Deserialize<List<Transmission>>(transmissionsJson);
        if (transmissions != null && transmissions.Count > 0)
        {
            List<string> transmissionTypes = transmissions.Select(t => t.Type).ToList();
            if (await context.Transmissions.AnyAsync(t => transmissionTypes.Contains(t.Type)) == false)
            {
                await context.Transmissions.AddRangeAsync(transmissions);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task ImportFuelTypesFromJsonAsync(CarSellingPlatformDbContext context)
    {
        var path = Path.Combine("..", "CarSellingPlatform.Data", "Seeding", "Input", "FuelTypes.json");
        string fuelTypesJson = File.ReadAllText(path);
        var fuelTypes = JsonSerializer.Deserialize<List<FuelType>>(fuelTypesJson);
        if (fuelTypes != null && fuelTypes.Count > 0)
        {
            List<string> fuelTypesNames = fuelTypes.Select(f => f.Type).ToList();
            if (await context.FuelTypes.AnyAsync(f => fuelTypesNames.Contains(f.Type)) == false)
            {
                await context.FuelTypes.AddRangeAsync(fuelTypes);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task ImportEnginesFromJsonAsync(CarSellingPlatformDbContext context)
    {
        var path = Path.Combine("..", "CarSellingPlatform.Data", "Seeding", "Input", "Engines.json");
        string enginesJson = File.ReadAllText(path);
        var engines = JsonSerializer.Deserialize<List<Engine>>(enginesJson);
        if(engines!=null&& engines.Count>0)
        {
            //List<Guid> engineIds = engines.Select(e => e.Id).ToList();
            foreach (var engine in engines)
            {
                if (await context.Engines.AnyAsync(e => engine.Id == e.Id) == false)
                {
                    await context.Engines.AddAsync(engine);
                }
            }
            await context.SaveChangesAsync();
        }
    }

    public static async Task ImportCarsFromJsonAsync(CarSellingPlatformDbContext context)
    {
        var path = Path.Combine("..", "CarSellingPlatform.Data", "Seeding", "Input", "Cars.json");
        string carJson = File.ReadAllText(path);
        var cars = JsonSerializer.Deserialize<List<Car>>(carJson);
        if (cars != null && cars.Count > 0)
        {
            foreach (var car in cars)
            {
                if (await context.Cars.AnyAsync(e => car.Id == e.Id) == false)
                {
                    await context.Cars.AddAsync(car);
                }
            }
            await context.SaveChangesAsync();
        }
    }
    
}