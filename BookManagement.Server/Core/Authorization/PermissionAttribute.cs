using BookManagement.Server.Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookManagement.Server.Core.Authorization;

public class PermissionAttribute : AuthorizeAttribute
{
    public string Menu { get; }
    public PermissionType PermissionType { get; }

    public PermissionAttribute(string menu, PermissionType permissionType)
    {
        PermissionType = permissionType;
        Menu = menu;
        Policy = $"{Menu}-{PermissionType}";
    }

}