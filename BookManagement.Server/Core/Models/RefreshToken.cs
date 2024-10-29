using System.ComponentModel.DataAnnotations;

namespace BookManagement.Server.Core.Models;

public class RefreshToken
{
    [Key]
    [Required]
    public required string Token { get; set; }
    [Required]
    public required DateTime Expires { get; set; }
    public DateTime Created { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}
