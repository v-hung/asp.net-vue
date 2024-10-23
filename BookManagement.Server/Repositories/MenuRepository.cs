public class MenuRepository
{
    private readonly ApplicationDbContext _context;

    public MenuRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Permission>> GetPermissionsForUserAsync(Guid userId)
    {
        // Giả định bạn có một bảng UserPermission để lưu quyền của người dùng
        return await _context.UserPermissions
            .Include(up => up.Permission) // Include Permission nếu cần
            .Where(up => up.UserId == userId)
            .Select(up => up.Permission)
            .ToListAsync();
    }

    public List<MenuItem> GetAllMenuItems()
    {
        return _context.MenuItems
            .Include(m => m.Permissions)
            .ToList();
    }
}
