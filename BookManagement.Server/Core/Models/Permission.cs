using System.ComponentModel.DataAnnotations;

namespace BookManagement.Server.Core.Models;
public class Permission
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    [Required]
    public Guid MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }

    [Required]
    public PermissionType PermissionType { get; set; }
}
