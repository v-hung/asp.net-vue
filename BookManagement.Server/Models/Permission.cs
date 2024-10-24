using System;
using System.Collections.Generic;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string Description { get; set; } = "";

    // Navigation properties
    public IList<Role> Roles { get; set; } = [];
    public IList<MenuItem> MenuItems { get; set; } = [];
}
