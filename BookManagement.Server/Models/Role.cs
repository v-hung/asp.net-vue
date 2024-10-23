using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class Role : IdentityRole<Guid>
{
    public string Description { get; set; } = "";

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
