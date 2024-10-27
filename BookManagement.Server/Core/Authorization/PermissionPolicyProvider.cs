

using BookManagement.Server.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace BookManagement.Server.Core.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var segments = policyName.Split('-');
        if (segments.Length == 2)
        {
            var menu = segments[0];
            var permissionTypeString = segments[1];
            if (Enum.TryParse<PermissionType>(permissionTypeString, true, out var permissionType))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(menu, permissionType));
                return Task.FromResult(policy?.Build());
            }
        }

        return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}