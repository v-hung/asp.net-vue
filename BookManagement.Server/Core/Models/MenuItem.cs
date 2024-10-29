using System.ComponentModel.DataAnnotations;

namespace BookManagement.Server.Core.Models;
public class MenuItem
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Url { get; set; }
    public Guid? ParentId { get; set; }

    [Required]
    public bool IsVisible { get; set; } = true;
    public int Order { get; set; }

    [Required]
    public virtual IList<string> PermissionTypes { get; set; } = [];

    // Navigation properties
    [Required]
    public IList<MenuItem> Children { get; set; } = [];

    [Required]
    public IList<Permission> Permissions { get; set; } = [];
}
