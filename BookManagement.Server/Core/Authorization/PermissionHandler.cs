// PermissionHandler.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _context;

    public PermissionHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // Lấy userId từ claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return;
        }

        var userId = userIdClaim.Value;

        // Lấy user từ cơ sở dữ liệu
        var user = await _context.Users
            .Include(u => u.Role) // Giả sử User có Role
            .ThenInclude(r => r.RolePermissions) // Giả sử Role có RolePermissions
            .ThenInclude(rp => rp.Permission) // Và RolePermissions liên kết tới Permission
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return;
        }

        // Kiểm tra xem user có quyền không
        var hasPermission = user.Role.RolePermissions.Any(rp => rp.Permission.Name == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement); // Nếu có quyền, đánh dấu yêu cầu là thành công
        }
    }
}
