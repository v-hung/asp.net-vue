using System.ComponentModel.DataAnnotations;

namespace BookManagement.Server.Models;

public class Book
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public required string Title { get; set; }
}