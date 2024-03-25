using Application.Features.User.Commands;
using Common.Authorization;
using Common.Requests.User;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : AppBaseController<UserController>
    {
        [MustPermission(AppFeature.Users, AppAction.Create)]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createUserRequest)
        {
            var response = await MeaditorSender
                .Send(new CreateUserCommand(createUserRequest));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
