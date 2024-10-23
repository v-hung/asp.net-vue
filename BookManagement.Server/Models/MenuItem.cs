using System;
using System.Collections.Generic;

public class MenuItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Url { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsVisible { get; set; } = true;

    // Navigation properties
    public ICollection<MenuItem> Children { get; set; } = [];
    public ICollection<Permission> Permissions { get; set; } = [];
}
