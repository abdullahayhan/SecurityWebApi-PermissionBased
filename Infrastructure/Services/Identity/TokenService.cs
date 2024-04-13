using Application.AppConfigs;
using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Identity;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppConfiguration _appConfiguration;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public TokenService(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<AppConfiguration> appConfiguration,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appConfiguration = appConfiguration.Value;
        _signInManager = signInManager;
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
            return await ResponseWrapper<TokenResponse>.FailAsync("Kullanıcı aktif değil. Sistem yöneticiniz ile iletişime geçiniz.");
        }
        // Check email if confirmed
        if (!user.EmailConfirmed)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Email onaylı değil.");
        }
        // Check password
        var result = await _signInManager.CheckPasswordSignInAsync(user, 
            tokenRequest.Password, 
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            // generate refresh token (refresh token database üzerinden tutulur.)
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
        else if (result.IsLockedOut)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Hesabınız bloke olmuştur. Lütfen daha sonra tekrar deneyiniz.");
        }
        else
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Geçersiz kimlik bilgileri");
        }
    }

    public async Task<ResponseWrapper<TokenResponse>> GetRefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        if (refreshTokenRequest is null)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Geçersiz bilgi.");
        }
        var userPrincipal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail!);

        if (user is null)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Kullanıcı bulunamadı.");
        }
        if (user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryDate <= DateTime.Now)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Geçersiz bilgi.");
        }

        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);
        await _userManager.UpdateAsync(user);

        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
        };

        return ResponseWrapper<TokenResponse>.Success(response);

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
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfiguration.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
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
            var currentRole = await _roleManager.FindByNameAsync(role);
            var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
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

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
        };
        var tokenHandler = new JwtSecurityTokenHandler(); // . Bu nesne, JWT'leri işlemek için kullanılır.
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken); //securityToken bir güvenlik belirteci fonksiyon tarafından döndürülür.
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Geçersiz bilgi.");
        }

        return principal;
    }
}
