using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public RoleService(RoleManager<ApplicationRole> roleManager,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _mapper = mapper;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRoleRequest)
    {
        var isRoleNameUnique = await _roleManager.FindByNameAsync(createRoleRequest.RoleName!);

        if (isRoleNameUnique is null)
        {
            var newRole = new ApplicationRole
            {
                Name = createRoleRequest.RoleName,
                Description = createRoleRequest.RoleDescription
            };

            var result = await _roleManager.CreateAsync(newRole);

            if (result.Succeeded)
            {
                var mappedResponseRole = _mapper.Map<RoleResponse>(newRole);
                return await ResponseWrapper<RoleResponse>.SuccessAsync(mappedResponseRole, "Rol başarıyla oluşturulmuştur.");
            }
            return await ResponseWrapper.FailAsync("Rol oluşturulken hata!");
        }
        return await ResponseWrapper.FailAsync("Rol ismi alınmıştır, lütfen benzersiz bir rol ismi deneyiniz.");
    }

    public async Task<IResponseWrapper> DeleteRoleAsync(string roleId)
    {
        var roleInDb = await _roleManager.FindByIdAsync(roleId);
        if (roleInDb is not null)
        {
            if (roleInDb.Name != AppRoles.Admin)
            {
                var allUser = await _userManager.Users.ToListAsync();
                foreach (var user in allUser)
                {
                    if (await _userManager.IsInRoleAsync(user, roleInDb.Name!))
                    {
                        return await ResponseWrapper.FailAsync("Rol başka kullanıcılarda bulunduğu için silinemez, " +
                            $"lütfen var olan kullanıcılardan {roleInDb.Name} yetkisini alınız.");
                    }
                }
                var result = await _roleManager.DeleteAsync(roleInDb);
                if (result.Succeeded)
                {
                    return await ResponseWrapper.SuccessAsync("Rol başarıyla silinmiştir.");
                }
                return await ResponseWrapper.FailAsync("Rol silinirken hata!");
            }
            return await ResponseWrapper.FailAsync("Admin Rolü silinemez!");
        }
        return await ResponseWrapper.FailAsync("Rol bulunamadı!");
    }

    public async Task<IResponseWrapper> GetPermissionsAsync(string roleId)
    {
        var roleInDb = await _roleManager.FindByIdAsync(roleId);
        if (roleId is not null)
        {
            var allPermissions = AppPermissions.AllPermissions;
            var roleClaimResponse = new RoleClaimResponse
            {
                Role = new RoleResponse()
                {
                    Id = roleInDb!.Id,
                    Name = roleInDb!.Name,
                    Description = roleInDb!.Description
                },
                RoleClaims = new()
            };

            var currentRoleClaims = await GetAllClaimsForRoleAsync(roleId);
            var allPermissionNames = allPermissions.Select(p => p.Name).ToList();
            var currentRoleClaimsValues = currentRoleClaims.Select(c => c.ClaimValue).ToList();

            var currentlyAssignedRoleClaimsNames = allPermissionNames
                .Intersect(currentRoleClaimsValues) // %100 matchleşme varsa.
                .ToList();

            foreach (var permission in allPermissions)
            {
                if (currentlyAssignedRoleClaimsNames.Any(carc => carc == permission.Name))
                {
                    roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                    {
                        RoleId = roleId,
                        ClaimType = AppClaim.Permission,
                        ClaimValue = permission.Name,
                        Description = permission.Description,
                        Group = permission.Group,
                        IsAssignedToRole = true
                    });
                }
                else
                {
                    roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                    {
                        RoleId = roleId,
                        ClaimType = AppClaim.Permission,
                        ClaimValue = permission.Name,
                        Description = permission.Description,
                        Group = permission.Group,
                        IsAssignedToRole = false
                    });
                }
            }
            return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }
        return await ResponseWrapper.FailAsync("Rol bilgisi bulunamadı.");
    }

    public async Task<IResponseWrapper> GetRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is not null)
        {
            var mappedRole = _mapper.Map<RoleResponse>(role);
            return await ResponseWrapper<RoleResponse>.SuccessAsync(mappedRole);
        }
        return await ResponseWrapper.FailAsync("Rol bulunamadı.");
    }

    public async Task<IResponseWrapper> GetRolesAsync(string? roleName)
    {
        // parametre verilirse.
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var rolesWithFiltering = _roleManager.Roles
                .Where(role => role.Name!.Contains(roleName!)).ToList();
            if (rolesWithFiltering.Count > 0)
            {
                var mappedRoleListByParameter = _mapper.Map<List<RoleResponse>>(rolesWithFiltering);

                return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(mappedRoleListByParameter);
            }
            return await ResponseWrapper.FailAsync("Rol bulunamadı.");
        }
        // parametre verilmez ise
        var roles = await _roleManager.Roles.ToListAsync();

        if (roles.Count > 0)
        {
            var mappedRoleList = _mapper.Map<List<RoleResponse>>(roles);
            return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(mappedRoleList);
        }
        return await ResponseWrapper.FailAsync("Rol bulunamadı.");
    }

    public async Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRoleRequest)
    {
        var roleInDb = await _roleManager.FindByIdAsync(updateRoleRequest.Id);
        if (roleInDb is not null)
        {
            if (roleInDb.Name != AppRoles.Admin)
            {
                roleInDb.Name = updateRoleRequest.Name;
                roleInDb.Description = updateRoleRequest.Description;

                if (await _roleManager.FindByNameAsync(roleInDb.Name!) is null)
                {
                    var resultRoleUpdate = await _roleManager.UpdateAsync(roleInDb);

                    if (resultRoleUpdate.Succeeded)
                    {
                        return await ResponseWrapper.SuccessAsync("Rol başarıyla güncellendi.");
                    }
                    return await ResponseWrapper.FailAsync("Güncelleme sırasında hata.");
                }
                return await ResponseWrapper.FailAsync("Lütfen benzersiz bir rol ismi giriniz.");
            }
            
            return await ResponseWrapper.FailAsync("Admin rolü silinemez.");
        }
        return await ResponseWrapper.FailAsync("İlgili rol bulunamadı.");
    }

    public async Task<IResponseWrapper> UpdateRolePermissionsAsync
        (UpdateRolePermissionsRequest updateRolePermissionsRequest)
    {
        var roleInDb = await _roleManager.FindByIdAsync(updateRolePermissionsRequest.RoleId);
        if (roleInDb is not null)
        {
            if (roleInDb.Name == AppRoles.Admin)
            {
                return await ResponseWrapper<string>.FailAsync("Admin rolü değiştirilemez.");
            }

            var permissionToBeAssigned = updateRolePermissionsRequest.RoleClaims
                .Where(rc => rc.IsAssignedToRole == true)
                .ToList();

            var allCurrentlyAssignedRoleClaims = await _roleManager
                .GetClaimsAsync(roleInDb);

            // var olan rol permissionları sil.
            foreach (var claim in allCurrentlyAssignedRoleClaims)
            {
                await _roleManager.RemoveClaimAsync(roleInDb, claim);
            }

            // Ekle
            foreach (var claim in permissionToBeAssigned)
            {
                var mappedRoleClaim = _mapper.Map<ApplicationRoleClaim>(claim);
                await _context.RoleClaims.AddAsync(mappedRoleClaim);
            }

            await _context.SaveChangesAsync();
            return await ResponseWrapper.SuccessAsync("İşlem başarılı");
        }
        return await ResponseWrapper.FailAsync("Rol bilgisi bulunamadı.");
    }

    private async Task<List<RoleClaimViewModel>> GetAllClaimsForRoleAsync(string roleId)
    {
        var roleClaims = await _context.RoleClaims
            .Where(roleClaim => roleClaim.RoleId == roleId).ToListAsync();

        if (roleClaims.Any())
        {
            var mappedRoleClaims = _mapper.Map<List<RoleClaimViewModel>>(roleClaims);
            return mappedRoleClaims;
        }
        return new List<RoleClaimViewModel>(); // bir permission'a sahip değilse boş bir nesne gönder.
    }
}
