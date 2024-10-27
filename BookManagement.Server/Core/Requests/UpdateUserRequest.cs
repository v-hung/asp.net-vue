namespace BookManagement.Server.Core.Requests;

public class UpdateUserRequest
{

    public string? FullName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public IFormFile? File { get; set; }

}