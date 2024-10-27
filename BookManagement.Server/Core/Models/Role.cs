using Microsoft.AspNetCore.Identity;

namespace BookManagement.Server.Core.Models;
public class Role : IdentityRole<Guid>
{
    public string Description { get; set; } = "";
    public bool IsAdmin { get; set; } = false;

    // Navigation properties
    public IList<User> Users { get; set; } = [];
    public IList<Permission> Permissions { get; set; } = [];
}
