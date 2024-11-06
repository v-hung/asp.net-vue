using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookManagement.Server.Core.Models;
public class User : IdentityUser<Guid>
{

    [Required]
    public required string FullName { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? Image { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [Required]
    public IList<Role> Roles { get; set; } = [];

    [Required]
    public IList<RefreshToken> RefreshTokens { get; set; } = [];

}