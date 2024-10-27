using Microsoft.AspNetCore.Identity;

namespace BookManagement.Server.Core.Models;
public class User : IdentityUser<Guid>
{
    public required string FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public IList<Role> Roles { get; set; } = [];
    public IList<RefreshToken> RefreshTokens { get; set; } = [];
}