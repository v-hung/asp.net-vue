namespace BookManagement.Server.Core.Models;
public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    public Guid MenuItemId { get; set; } 
    public MenuItem? MenuItem { get; set; }

    public PermissionType PermissionType { get; set; }
}
