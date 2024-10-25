namespace BookManagement.Server.Core.Models;
public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public required string Resource { get; set; }
}
