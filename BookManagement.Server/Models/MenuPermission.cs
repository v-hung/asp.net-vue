using System;

public class MenuPermission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MenuItemId { get; set; }
    public Guid PermissionId { get; set; }

    // Navigation properties
    public required MenuItem MenuItem { get; set; }
    public required Permission Permission { get; set; }
}
