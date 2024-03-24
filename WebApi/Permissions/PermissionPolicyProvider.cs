using Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace WebApi.Permissions;

//  Yetkilendirme politikaları, belirli bir isteğin veya kullanıcının erişim yetkisini belirlemek için kullanılır.
//  Örneğin, bir kullanıcının belirli bir kaynağa erişim izni olup olmadığını kontrol etmek için 
//  yetkilendirme politikaları kullanılabilir.
//  Bu sınıf, özel yetkilendirme gereksinimleri olan politikaları dinamik olarak oluşturmak 
//  ve yönetmek için kullanılır.
//  Bu şekilde, uygulama genelinde farklı yetkilendirme kurallarını dinamik olarak tanımlayabilir ve yönetebilirsiniz.
public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(AppClaim.Permission, StringComparison.CurrentCultureIgnoreCase))
        {
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirement(policyName));
            return Task.FromResult(policy.Build());
        }

        return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => FallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => Task.FromResult<AuthorizationPolicy>(null);
}
