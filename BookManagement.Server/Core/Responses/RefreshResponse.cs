using System.ComponentModel.DataAnnotations;

namespace BookManagement.Server.Core.Responses;

public class RefreshResponse {

    [Required]
    public required string Token { get; set; }

    [Required]
    public required string RefreshToken { get; set; }

}