using Application.Features.User.Commands;
using Application.Features.User.Queries;
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

        [MustPermission(AppFeature.Users, AppAction.Update)]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updateUserRequest)
        {
            var response = await MeaditorSender
                .Send(new UpdateUserCommand(updateUserRequest));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [MustPermission(AppFeature.Users, AppAction.Read)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var response = await MeaditorSender
                .Send(new GetUserByIdRequest(id));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [MustPermission(AppFeature.Users, AppAction.Read)]
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var response = await MeaditorSender
                .Send(new GetUserListRequest());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        //[MustPermission(AppFeature.Users, AppAction.Update)]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordRequest changeUserPasswordRequest)
        {
            var response = await MeaditorSender
                .Send(new ChangeUserPasswordCommand(changeUserPasswordRequest));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

    }
}
