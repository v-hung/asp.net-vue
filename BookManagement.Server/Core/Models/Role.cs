using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookManagement.Server.Core.Models;
public class Role : IdentityRole<Guid>
{
    [Required]
    public string Description { get; set; } = "";

    [Required]
    public bool IsAdmin { get; set; } = false;

    // Navigation properties
    [Required]
    public IList<User> Users { get; set; } = [];
    
    [Required]
    public IList<Permission> Permissions { get; set; } = [];
}
