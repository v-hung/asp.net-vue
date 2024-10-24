using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class Role : IdentityRole<Guid>
{
    public string Description { get; set; } = "";

    // Navigation properties
    public IList<Permission> Permissions { get; set; } = [];
    public IList<UserRole> UserRoles { get; set; } = [];
}
