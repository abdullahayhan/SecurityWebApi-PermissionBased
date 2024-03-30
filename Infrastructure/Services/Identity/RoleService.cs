using Application.Services.Identity;
using AutoMapper;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMapper _mapper;

    public RoleService(RoleManager<ApplicationRole> roleManager, 
        IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
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
}
