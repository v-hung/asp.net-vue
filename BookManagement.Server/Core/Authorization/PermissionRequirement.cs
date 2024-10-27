using BookManagement.Server.Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookManagement.Server.Core.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Menu { get; }
    public PermissionType PermissionType { get; }

    public PermissionRequirement(string menu, PermissionType permissionType)
    {
        Menu = menu;
        PermissionType = permissionType;
    }
}
