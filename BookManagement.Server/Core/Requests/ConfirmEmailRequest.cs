namespace BookManagement.Server.Core.Requests;

public class ConfirmEmailRequest {

    public required string UserId { get; set; }
    public required string Code { get; set; }

}