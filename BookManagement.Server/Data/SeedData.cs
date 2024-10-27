using System.Threading.Tasks;
using BookManagement.Server.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace BookManagement.Server.Data;
public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        await SeedUser(serviceProvider);
        SeedMenuAndPermissions(serviceProvider);
    }

    public static async Task SeedUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        // Create roles
        string[] roleNames = { "Admin", "Leader", "StandUser" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new Role()
                {
                    NormalizedName = roleName.ToUpper(),
                    Name = roleName,
                    IsAdmin = roleName == "Admin"
                });
            }
        }

        Dictionary<string, string> userRoles = new Dictionary<string, string>
        {
            { "admin@admin.com", "Admin" },
            { "leader@admin.com", "Leader" },
            { "user@admin.com", "StandUser" }
        };

        foreach (var entry in userRoles)
        {
            var userExit = await userManager.FindByNameAsync(entry.Key);
            if (userExit == null)
            {
                // Create admin account
                var adminUser = new User
                {
                    Email = entry.Key,
                    UserName = entry.Key,
                    NormalizedUserName = entry.Key.ToUpper(),
                    NormalizedEmail = entry.Key.ToUpper(),
                    FullName = entry.Value,
                    LockoutEnabled = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, entry.Value);
                }

            }
        }

    }

    public static void SeedMenuAndPermissions(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var reportId = Guid.NewGuid();
        var menuItems = new[]
        {
            new MenuItem { Id = Guid.NewGuid(), Name = "Dashboard", Url = "/dashboard", Order = 1 },
            new MenuItem { Id = Guid.NewGuid(), Name = "Store", Url = "/store", Order = 2 },
            new MenuItem { Id = Guid.NewGuid(), Name = "Book", Url = "/book", Order = 3 },
            new MenuItem { Id = reportId, Name = "Report", Url = "/report", Order = 4 },
            new MenuItem { Id = Guid.NewGuid(), Name = "Report1", Url = "/report1", Order = 5, ParentId = reportId },
            new MenuItem { Id = Guid.NewGuid(), Name = "Report2", Url = "/report2", Order = 6, ParentId = reportId },
            new MenuItem { Id = Guid.NewGuid(), Name = "Report3", Url = "/report3", Order = 7, ParentId = reportId }
        };

        if (!context.MenuItems.Any())
        {
            context.MenuItems.AddRange(menuItems);
        }

        var menuItemIds = menuItems.ToDictionary(mi => mi.Name, mi => mi.Id);

        var roles = context.Roles.ToList();
        var roleIds = roles.ToDictionary(r => r.Name ?? "", r => r.Id);

        // Add permissions to menu items
        if (!context.Permissions.Any())
        {
            context.Permissions.AddRange(
                // Leader
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["Leader"], MenuItemId = menuItemIds["Dashboard"], PermissionType = PermissionType.View },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["Leader"], MenuItemId = menuItemIds["Store"], PermissionType = PermissionType.Create },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["Leader"], MenuItemId = menuItemIds["Store"], PermissionType = PermissionType.Delete },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["Leader"], MenuItemId = menuItemIds["Report"], PermissionType = PermissionType.View },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["Leader"], MenuItemId = menuItemIds["Report1"], PermissionType = PermissionType.View },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["Leader"], MenuItemId = menuItemIds["Report2"], PermissionType = PermissionType.View },

                // StandUser
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["StandUser"], MenuItemId = menuItemIds["Store"], PermissionType = PermissionType.View },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["StandUser"], MenuItemId = menuItemIds["Store"], PermissionType = PermissionType.Create },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["StandUser"], MenuItemId = menuItemIds["Store"], PermissionType = PermissionType.Update },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["StandUser"], MenuItemId = menuItemIds["Store"], PermissionType = PermissionType.Import },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["StandUser"], MenuItemId = menuItemIds["Book"], PermissionType = PermissionType.View },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["StandUser"], MenuItemId = menuItemIds["Book"], PermissionType = PermissionType.Create },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["StandUser"], MenuItemId = menuItemIds["Book"], PermissionType = PermissionType.Update },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["StandUser"], MenuItemId = menuItemIds["Book"], PermissionType = PermissionType.Import },
                new Permission { Id = Guid.NewGuid(), RoleId = roleIds["StandUser"], MenuItemId = menuItemIds["Report3"], PermissionType = PermissionType.View }
            );
            context.SaveChanges();
        }

    }
}
