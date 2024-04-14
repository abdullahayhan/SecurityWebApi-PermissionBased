using Application.Features.Identity.User.Commands;
using Application.Features.Identity.User.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : AppBaseController<UserController>
    {
        [MustPermission(AppFeature.Users, AppAction.Create)]
        [HttpPost("create-user")]
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
        [HttpPut("update-user")]
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
        [HttpGet("get-user/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var response = await MeaditorSender
                .Send(new GetUserByIdRequest(userId));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [MustPermission(AppFeature.Users, AppAction.Read)]
        [HttpGet("get-all-user")]
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

        [MustPermission(AppFeature.Users, AppAction.Update)]
        [HttpPut("change-user-password")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangeUserPasswordRequest changeUserPasswordRequest)
        {
            var response = await MeaditorSender
                .Send(new ChangeUserPasswordCommand(changeUserPasswordRequest));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [MustPermission(AppFeature.Users, AppAction.Read)]
        [HttpGet("roles/{userId}")]
        public async Task<IActionResult> GetRoles(string userId)
        {
            var response = await MeaditorSender
                .Send(new GetRolesRequest(userId));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }


        [MustPermission(AppFeature.Users, AppAction.Update)]
        [HttpPut("update-user-roles")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRolesRequest updateUserRolesRequest)
        {
            var response = await MeaditorSender
                .Send(new UpdateUserRolesCommand(updateUserRolesRequest));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
