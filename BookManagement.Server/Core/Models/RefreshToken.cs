// Suggested code may be subject to a license. Learn more: ~LicenseLog:2077256233.
namespace BookManagement.Server.Core.Models;

public class RefreshToken
{
    public required string Token { get; set; }
    public required DateTime Expires { get; set; }
    public bool IsExpired => DateTime.Now >= Expires;
    public DateTime Created { get; set; }
}
