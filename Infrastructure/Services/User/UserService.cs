using Application.Services;
using AutoMapper;
using Common.Authorization;
using Common.Requests.User;
using Common.Responses;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.User;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMapper _mapper;
    public UserService(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
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

        return await ResponseWrapper.FailAsync("User password değiştirelemedi, mevcut şifrenizi kontrol ederek tekrar deneyebilirsiniz."); 
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

        if (createUserRequest.Password == createUserRequest.ConfirmPassword)
        {
            var passwordHash = new PasswordHasher<ApplicationUser>();
            newUser.PasswordHash = passwordHash.HashPassword(newUser, createUserRequest.Password);

            var identityResult = await _userManager.CreateAsync(newUser);

            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);
                return await ResponseWrapper<string>.SuccessAsync($"{newUser.FirstName} {newUser.LastName} registered succesfully.", "user registered succesfully");
            }
            return await ResponseWrapper.FailAsync("User created failed.");
        }

        return await ResponseWrapper.FailAsync("Passwords are not matched.");
    }

    public async Task<IResponseWrapper> GetAllUsersAsync()
    {
        var userListInDb = await _userManager.Users.ToListAsync();
        if (userListInDb.Count > 0)
        {
            var mappedUserList = _mapper.Map<UserResponse>(userListInDb);
            return await ResponseWrapper<List<UserResponse>>.SuccessAsync();
        }

        return await ResponseWrapper.FailAsync("User not found");
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
            return await ResponseWrapper.SuccessAsync("Güncelleme işlemi başarısız oldu.");
        }
        return await ResponseWrapper.SuccessAsync("User not found.");
    }
}
