namespace BookManagement.Server.Core.Requests;

public class RegisterRequest
{

    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }

}