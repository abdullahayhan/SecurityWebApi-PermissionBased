using Application.Features.Role.Commands;
using Application.Features.User.Commands;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : AppBaseController<RoleController>
    {
        [MustPermission(AppFeature.Roles, AppAction.Create)]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest createRoleRequest)
        {
            var response = await MeaditorSender
                .Send(new CreateRoleCommand(createRoleRequest));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
