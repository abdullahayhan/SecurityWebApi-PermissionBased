using Application.Features.User.Commands;
using Application.Features.User.Queries;
using Common.Authorization;
using Common.Requests.Identity;
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

        [MustPermission(AppFeature.Users, AppAction.Update)]
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
