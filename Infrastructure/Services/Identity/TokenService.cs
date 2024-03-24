using Application.AppConfigs;
using Application.Services.Identity;
using Common.Requests;
using Common.Responses;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.Identity;

public class TokenService : ITokenService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppConfiguration _appConfiguration;

    public TokenService(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<AppConfiguration> appConfiguration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _appConfiguration = appConfiguration.Value;
    }

    public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest)
    {
        // Validate user
        var user = await _userManager.FindByEmailAsync(tokenRequest.Email);
        // Check user
        if (user is null)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials");
        }
        // Check if Active 
        if (!user.IsActive)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("User not active. Please contact the administrator");
        }
        // Check email if confirmed
        if (!user.EmailConfirmed)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Email not confirmed");
        }
        // Check password
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);
        if (!isPasswordValid)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials");
        }
        // generate refresh token
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);
        // update user
        await _userManager.UpdateAsync(user);
        // GENERATE NEW TOKEN
        var token = await GenerateJWTAsync(user);

        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
        };

        return ResponseWrapper<TokenResponse>.Success(response);
    }

    public Task<ResponseWrapper<TokenResponse>> GetRefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        throw new NotImplementedException();
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateJWTAsync(ApplicationUser user)
    {
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));

        return token;
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes),
            signingCredentials : signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private SigningCredentials GetSigningCredentials() 
    {
        var secret = Encoding.UTF8.GetBytes(_appConfiguration.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret),SecurityAlgorithms.HmacSha256);
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var userClamis = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();


        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            var currentRole = await _roleManager.FindByIdAsync(role);
            var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole!);
            permissionClaims.AddRange(allPermissionsForCurrentRole);
        }

        var claims = new List<Claim> 
        { 
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
        }
        .Union(userClamis)
        .Union(roleClaims)
        .Union(permissionClaims);

        return claims; //finallyyyyy be abi
    }
}
