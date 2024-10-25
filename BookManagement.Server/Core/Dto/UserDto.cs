namespace BookManagement.Server.Core.Dto;

public class UserDto {
  public required Guid Id { get; set; }
  public string? Email { get; set; }
  public string? FullName { get; set; }
  public string? PhoneNumber { get; set; }
  public Boolean EmailConfirmed { get; set; }
  public string? Address { get; set; }
  public string? Image { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}