using Common.Authorization;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class ApplicationDbSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;

    public ApplicationDbSeeder(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task SeedDataseAsync()
    {
        await CheckAndApplyPendingMigrationAsync();
        await SeedRolesAsync();
        await SeedAdminUserAsync();
        await SeedCreateUserAsync();
    }

    private async Task CheckAndApplyPendingMigrationAsync()
    {
        if (_dbContext.Database.GetPendingMigrations().Any())
        {
            await _dbContext.Database.MigrateAsync();
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in AppRoles.DefaultRoles)
        {
            /* is not : 
                Bu ifade, _roleManager.Roles.FirstOrDefaultAsync() yöntemi tarafından döndürülen 
                rol nesnesinin ApplicationRole türüne ait olmadığını doğrular.

                Rolün ApplicationRole türüne ait olup olmadığını kontrol eder 
                ve eğer değilse, bu durumun işlenmesine izin verir.
            */

            if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                role = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} Role."
                };

                await _roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == AppRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(role, AppPermissions.AdminPermissions);
            }

            else if (roleName == AppRoles.Basic)
            {
                await AssignPermissionsToRoleAsync(role, AppPermissions.BasicPermissions);
            }

            else if (roleName == AppRoles.EmployeeRead)
            {
                await AssignPermissionsToRoleAsync(role, AppPermissions.BasicEmployeePermission);
            }

        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationRole role,
        IReadOnlyList<AppPermission> permissions)
    {
        // Claims ve permissions aynı anlama geliyor, yaptıkları iş aynı.
        var currentClaims = await _roleManager.GetClaimsAsync(role); // hangi rolün claimsleri gelsin?
        foreach (var permission in permissions)
        {
            // var olan claimslerin içinde Type'ı Permission olan ve value değeri de permission ismi ile uyuşmayan bir claim var ise
            if (!currentClaims.Any(claim => claim.Type == AppClaim.Permission && claim.Value == permission.Name))
            {
                await _dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group
                });

                await _dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        string adminUserName = AppCredentials.Email[..AppCredentials.Email.IndexOf('@')]; // başlangıçtan @ karakterine kadar al.
        var adminUser = new ApplicationUser
        {
            FirstName = "Admin",
            LastName = "User",
            Email = AppCredentials.Email,
            UserName = adminUserName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            NormalizedEmail = AppCredentials.Email.ToUpperInvariant(),
            NormalizedUserName = adminUserName.ToUpperInvariant(),
            LockoutEnabled= true,
            IsActive = true
        };

        if (!await _userManager.Users.AnyAsync(u => u.Email == AppCredentials.Email))
        {
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, AppCredentials.DefaultPassword);
            await _userManager.CreateAsync(adminUser);
        }

        if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Basic) 
            || !await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            await _userManager.AddToRolesAsync(adminUser, AppRoles.DefaultRoles);
        }
    }

    private async Task SeedCreateUserAsync()
    {
        string userName = AppCredentials.UserEmail[..AppCredentials.UserEmail.IndexOf('@')];
        var user = new ApplicationUser
        {
            FirstName = "Normal",
            LastName = "User",
            Email = AppCredentials.UserEmail,
            UserName = userName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            NormalizedEmail = AppCredentials.UserEmail.ToUpperInvariant(),
            NormalizedUserName = userName.ToUpperInvariant(),
            IsActive = true
        };
        if (!await _userManager.Users.AnyAsync(u => u.Email == user.Email))
        {
            var password = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = password.HashPassword(user, AppCredentials.DefaultUserPassword);
            await _userManager.CreateAsync(user);
        }

        if(!await _userManager.IsInRoleAsync(user, AppRoles.EmployeeRead))
        {
            await _userManager.AddToRoleAsync(user, AppRoles.EmployeeRead);
        }
    }
}
