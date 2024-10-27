using BookManagement.Server.Core.Models;
using BookManagement.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Server.Core.Services;

public class MenuService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MenuService(ApplicationDbContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<MenuItem>> GetMenuForCurrentUser(bool? tree = true)
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            return new List<MenuItem>();
        }

        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        if (user == null)
        {
            return new List<MenuItem>();
        }

        // Get the list of user roles
        var roles = _context.Roles
            .Where(r => r.Users.Any(u => u.Id == user.Id))
            .Select(r => new { r.Id, r.IsAdmin, Permissions = r.Permissions.Select(p => new { p.PermissionType, p.MenuItemId }).ToList() })
            .ToList();

        List<MenuItem> menus = await _context.MenuItems
            .Where(m => m.IsVisible)
            .OrderBy(m => m.Order).ToListAsync();


        if (roles.Any(r => r.IsAdmin)) {
            PermissionType[] allPermissions = (PermissionType[])Enum.GetValues(typeof(PermissionType));

            // add permisstions to menu
            menus.ForEach(m => m.PermissionTypes = allPermissions.Select(pt => pt.ToString()).ToList());
        }
        else {
            var permissions = roles.SelectMany(r => r.Permissions);

            menus = menus
                .Where(m => permissions.Any(p => p.MenuItemId == m.Id))
                .ToList();

            // add permisstions to menu
            menus.ForEach(m => m.PermissionTypes = permissions.Where(p => p.MenuItemId == m.Id).Select(p => p.PermissionType.ToString()).ToList());
        }

        return tree == true ? BuildMenuTree(menus, null) : menus;

    }

    private List<MenuItem> BuildMenuTree(List<MenuItem> menus, Guid? parentId)
    {
        return menus
            .Where(m => m.ParentId == parentId)
            .Select(m =>
            {
                m.Children = BuildMenuTree(menus, m.Id);
                return m;
            })
            .ToList();
    }

}
