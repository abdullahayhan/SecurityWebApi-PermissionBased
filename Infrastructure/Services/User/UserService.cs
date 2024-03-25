using Application.Services;
using Common.Authorization;
using Common.Requests.User;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.User;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UserService(UserManager<ApplicationUser> userManager, 
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IResponseWrapper> CreateUserAsync(CreateUserRequest createUserRequest)
    {
        var userWithSameEmail = await _userManager.FindByEmailAsync(createUserRequest.Email);

        if (userWithSameEmail is not null)
        {
            return await ResponseWrapper.FailAsync("Email already taken.");
        }

        var userWithSameUserName = _userManager.FindByNameAsync(createUserRequest.UserName!);
        
        if (userWithSameUserName is not null)
        {
            return await ResponseWrapper.FailAsync("UserName already taken.");
        }

        var userName = createUserRequest.Email![..createUserRequest.Email!.IndexOf('@')];
        var newUser = new ApplicationUser
        {
            FirstName = createUserRequest.FirstName,
            LastName = createUserRequest.LastName,
            Email = createUserRequest.Email,
            NormalizedEmail = createUserRequest.Email.ToUpper(),
            UserName = userName,
            NormalizedUserName = userName.ToUpper(),
            PhoneNumber = createUserRequest.PhoneNumber,
            EmailConfirmed = createUserRequest.AutoConfirmEmail,
            IsActive = createUserRequest.IsActive,
        };

        if (createUserRequest.Password == createUserRequest.ConfirmPassword)
        {
            var passwordHash = new PasswordHasher<ApplicationUser>();
            newUser.PasswordHash = passwordHash.HashPassword(newUser, createUserRequest.Password);

            var identityResult = await _userManager.CreateAsync(newUser);

            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);
                return await ResponseWrapper<string>.SuccessAsync($"{newUser.FirstName} {newUser.LastName} user created.","user created succesfully");
            }
            return await ResponseWrapper.FailAsync("User created failed.");
        }

        return await ResponseWrapper.FailAsync("Passwords are not matched.");
    }

    public Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest updateUserRequest)
    {
        throw new NotImplementedException();
    }
}
