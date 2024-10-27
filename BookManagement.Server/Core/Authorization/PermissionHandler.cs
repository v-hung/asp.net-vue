using System.Security.Claims;
using BookManagement.Server.Core.Models;
using BookManagement.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Server.Core.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _context;

    public PermissionHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext authContext, PermissionRequirement requirement)
    {
        // get userId on claims
        var userIdClaim = authContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return;

        var userId = Guid.Parse(userIdClaim.Value);

        // Get the list of user roles
        var roles = await _context.Roles
            .Where(r => r.Users.Any(u => u.Id == userId))
            .Select(r => new { r.Id, r.IsAdmin })
            .ToListAsync();

        // Check if the user has the admin role
        if (roles.Any(r => r.IsAdmin))
        {
            authContext.Succeed(requirement);
            return;
        }

        // Check permission
        var hasPermission = await _context.Permissions
            .AnyAsync(p => roles.Select(r => r.Id).Contains(p.RoleId)
                           && p.MenuItem != null && p.MenuItem.Name == requirement.Menu
                           && p.PermissionType == requirement.PermissionType);

        if (hasPermission)
        {
            authContext.Succeed(requirement);
        }
    }

}
