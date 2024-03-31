using Application.Features.Identity.Token.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class TokenController : AppBaseController<TokenController>
{
    [HttpPost("get-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
    {
        var response = await MeaditorSender.Send(new GetTokenQuery(tokenRequest));
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPost("get-refresh-token")]
    public async Task<IActionResult> GetRefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await MeaditorSender.Send(new GetRefreshTokenQuery(refreshTokenRequest));
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);    
    }

}
