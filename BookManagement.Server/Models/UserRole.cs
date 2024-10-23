using System;
using Microsoft.AspNetCore.Identity;

public class UserRole : IdentityUserRole<Guid>
{
    public required User User { get; set; }
    public required Role Role { get; set; }
}
