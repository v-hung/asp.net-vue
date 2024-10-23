using Microsoft.AspNetCore.Identity;

public class MenuService
{
    private readonly MenuRepository _menuRepository;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MenuService(MenuRepository menuRepository, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _menuRepository = menuRepository;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<MenuItem>> GetMenuForCurrentUser()
    {
        // Lấy người dùng hiện tại
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        if (user == null)
        {
            return new List<MenuItem>(); // Nếu không đăng nhập, không trả về menu nào
        }

        // Lấy tất cả các quyền của người dùng hiện tại
        var userPermissions = await GetUserPermissionsAsync(user);

        // Lấy tất cả các menu từ cơ sở dữ liệu
        var allMenuItems = _menuRepository.GetAllMenuItems();

        // Chỉ trả về các menu mà người dùng có quyền truy cập
        var rootMenuItems = allMenuItems
            .Where(m => m.ParentId == null && m.Permissions.Any(p => userPermissions.Contains(p.Name)))
            .ToList();

        // Ánh xạ các menu con vào menu cha dựa trên quyền
        foreach (var parentMenu in rootMenuItems)
        {
            parentMenu.Children = GetChildrenForUser(parentMenu, allMenuItems, userPermissions);
        }

        return rootMenuItems;
    }

    private List<MenuItem> GetChildrenForUser(MenuItem parentMenuItem, List<MenuItem> allMenuItems, IList<string> userPermissions)
    {
        var children = allMenuItems
            .Where(m => m.ParentId == parentMenuItem.Id && m.Permissions.Any(p => userPermissions.Contains(p.Name)))
            .ToList();

        foreach (var child in children)
        {
            child.Children = GetChildrenForUser(child, allMenuItems, userPermissions);
        }

        return children;
    }

    private async Task<List<string>> GetUserPermissionsAsync(User user)
    {
        // Giả sử bạn có một phương thức để lấy quyền của người dùng từ cơ sở dữ liệu
        // Ví dụ, bạn có thể kiểm tra quyền của người dùng thông qua bảng User-Permission

        // Code ví dụ:
        var permissions = await _menuRepository.GetPermissionsForUserAsync(user.Id);
        return permissions.Select(p => p.Name).ToList();
    }
}
