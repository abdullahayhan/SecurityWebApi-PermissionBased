using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UserService(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<IResponseWrapper> ChangeUserPassword(ChangeUserPasswordRequest changeUserPasswordRequest)
    {
        var userInDb = await _userManager.FindByIdAsync(changeUserPasswordRequest.UserId);

        if (userInDb is null)
        {
            return await ResponseWrapper.FailAsync("User bulunamadı.");
        }

        if (changeUserPasswordRequest.NewPassword != changeUserPasswordRequest.ConfirmedNewPassword)
        {
            return await ResponseWrapper.FailAsync("Şifreler aynı olmalıdır.");
        }

        var result = await _userManager.ChangePasswordAsync(userInDb,
            changeUserPasswordRequest.CurrentPassword,
            changeUserPasswordRequest.NewPassword);

        if (result.Succeeded)
        {
            return await ResponseWrapper.SuccessAsync("User password updated.");
        }

        return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
    }

    public async Task<IResponseWrapper> CreateUserAsync(CreateUserRequest createUserRequest)
    {
        var userWithSameEmail = await _userManager.FindByEmailAsync(createUserRequest.Email);

        if (userWithSameEmail is not null)
        {
            return await ResponseWrapper.FailAsync("Email already taken.");
        }

        var userWithSameUserName = await _userManager.FindByNameAsync(createUserRequest.UserName!);

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

        //if (createUserRequest.Password == createUserRequest.ConfirmPassword)
        //{
            var passwordHash = new PasswordHasher<ApplicationUser>();
            newUser.PasswordHash = passwordHash.HashPassword(newUser, createUserRequest.Password!);

            var identityResult = await _userManager.CreateAsync(newUser);

            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);
                return await ResponseWrapper<string>.SuccessAsync($"{newUser.FirstName} {newUser.LastName} registered succesfully.", "user registered succesfully");
            }
            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
        //}

        //return await ResponseWrapper.FailAsync("Passwords are not matched.");
    }

    public async Task<IResponseWrapper> GetAllUsersAsync()
    {
        var userListInDb = await _userManager.Users.ToListAsync();
        if (userListInDb.Count > 0)
        {
            var mappedUserList = _mapper.Map<List<UserResponse>>(userListInDb);
            return await ResponseWrapper<List<UserResponse>>.SuccessAsync(mappedUserList);
        }

        return await ResponseWrapper.FailAsync("User not found");
    }

    public async Task<IResponseWrapper> GetRolesAsync(string userId)
    {
        var userInDb = await _userManager.FindByIdAsync(userId);
        if (userInDb is not null)
        {
            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoleVMList = new List<UserRoleViewModel>();
            foreach (var role in allRoles)
            {
                var userRoleVM = new UserRoleViewModel
                {
                    RoleName = role.Name,
                    RoleDescription = role.Description,
                };

                if (await _userManager.IsInRoleAsync(userInDb, role.Name!))
                {
                    userRoleVM.IsAssignedToUser = true;
                }
                userRoleVMList.Add(userRoleVM);
            }
            return await ResponseWrapper<List<UserRoleViewModel>>.SuccessAsync(userRoleVMList);
        }
        return await ResponseWrapper.FailAsync("User is not found");
    }

    public async Task<IResponseWrapper> GetUserByIdAsync(string userId)
    {
        var userInDb = await _userManager.FindByIdAsync(userId);
        if (userInDb is not null)
        {
            var mappedUser = _mapper.Map<UserResponse>(userInDb);
            return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
        }
        return await ResponseWrapper.FailAsync("User does not exist.");
    }
    public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest updateUserRequest)
    {
        var userInDb = await _userManager.FindByIdAsync(updateUserRequest.UserId!);

        if (userInDb is not null)
        {
            userInDb.FirstName = updateUserRequest.FirstName;
            userInDb.LastName = updateUserRequest.LastName;
            userInDb.IsActive = updateUserRequest.IsActive;

            var result = await _userManager.UpdateAsync(userInDb);

            if (result.Succeeded)
            {
                return await ResponseWrapper.SuccessAsync("İşlem başarılı");
            }
            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
        }
        return await ResponseWrapper.FailAsync("User not found.");
    }

    public async Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest updateUserRolesRequest)
    {
        // cannot un-assign admin role
        // you can't change default user role.
        var userInDb = await _userManager.FindByIdAsync(updateUserRolesRequest.UserId);
        if (userInDb is not null)
        {
            if (userInDb.Email == AppCredentials.Email)
            {
                return await ResponseWrapper.FailAsync("User roles update not permitted");
            }
            var currentlyUserRoles = await _userManager.GetRolesAsync(userInDb);
            var newUserRoles = updateUserRolesRequest.Roles
                .Where(role => role.IsAssignedToUser == true)
                .ToList();

            var currentLoggedInUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (currentLoggedInUser is null)
            {
                return await ResponseWrapper.FailAsync("User does not exist.");
            }
            if (await _userManager.IsInRoleAsync(currentLoggedInUser, AppRoles.Admin))
            {
                var resultRemoveAllRoles = await _userManager.RemoveFromRolesAsync(userInDb, currentlyUserRoles);
                if (resultRemoveAllRoles.Succeeded)
                {
                   var newRoleUpdateResult = await _userManager.AddToRolesAsync(userInDb, 
                       newUserRoles.Select(role => role.RoleName)!);

                    if (newRoleUpdateResult.Succeeded)
                    {
                        return await ResponseWrapper.SuccessAsync("user roles updated succesfully.");
                    }
                    return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(newRoleUpdateResult));
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(resultRemoveAllRoles));
            }
        }
        return await ResponseWrapper.FailAsync("User is not found");
    }

    private List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
    {
        var errorDescriptions = new List<string>();
        foreach (var error in identityResult.Errors)
        {
            errorDescriptions.Add(error.Description);
        }
        return errorDescriptions;
    }

    public async Task<IResponseWrapper> GetUserByEmailAsync(string email)
    {
        var userInDb = await _userManager.FindByEmailAsync(email);
        if (userInDb != null)
        {
            var mappedUser = _mapper.Map<UserResponse>(userInDb);
            return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
        }
        return await ResponseWrapper.FailAsync("User bulunamadı.");
    }
}
