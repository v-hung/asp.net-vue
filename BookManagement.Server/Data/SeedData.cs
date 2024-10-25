using System.Threading.Tasks;
using BookManagement.Server.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace BookManagement.Server.Data;
public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        await SeedUser(serviceProvider);
    }

    public static async Task SeedUser(IServiceProvider serviceProvider) {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        // Create roles
        string[] roleNames = { "Admin", "Leader", "StandUser" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new Role(){
                    NormalizedName = "ADMIN",
                    Name = "admin"
                });
            }
        }

        // Create admin account
        var adminUser = new User {
            Email = "admin@admin.com",
            UserName = "admin@admin.com",
            NormalizedUserName = "ADMIN@ADMIN.COM",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            FullName = "Admin",
            LockoutEnabled = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = true
        };
        
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
