using System.ComponentModel.DataAnnotations;

namespace BookManagement.Server.Core.Dto;

public class UserDto
{
    [Required]
    public required Guid Id { get; set; }

    [Required]
    public string? Email { get; set; }

    [Required]
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    [Required]
    public Boolean EmailConfirmed { get; set; }

    public string? Address { get; set; }

    public string? Image { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

}