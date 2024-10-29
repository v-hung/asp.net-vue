using System.ComponentModel.DataAnnotations;
using BookManagement.Server.Core.Dto;

namespace BookManagement.Server.Core.Responses;

public class LoginResponse {

    [Required]
    public required string Token { get; set; }

    [Required]
    public required string RefreshToken { get; set; }

    [Required]
    public required UserDto User { get; set; }

}