using Application.Features.Identity.Role.Commands;
using Application.Features.Identity.Role.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class RoleController : AppBaseController<RoleController>
    {
        [MustPermission(AppFeature.Roles, AppAction.Read)]
        [HttpGet]
        public async Task<IActionResult> GetRoles(string? roleName)
        {
            var response = await MeaditorSender
                .Send(new GetRolesRequest(roleName));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [MustPermission(AppFeature.Roles, AppAction.Read)]
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var response = await MeaditorSender
                .Send(new GetRoleByIdRequest(roleId));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

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

        [MustPermission(AppFeature.Roles, AppAction.Update)]
        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest updateRoleRequest)
        {
            var response = await MeaditorSender
                .Send(new UpdateRoleCommand(updateRoleRequest));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [MustPermission(AppFeature.Roles, AppAction.Delete)]
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var response = await MeaditorSender
                .Send(new DeleteRoleCommand(roleId));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [MustPermission(AppFeature.RoleClaims, AppAction.Read)]
        [HttpGet("permissions/{roleId}")]
        public async Task<IActionResult> GetPermissions(string roleId)
        {
            var response = await MeaditorSender
                .Send(new GetPermissionsRequest(roleId));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [MustPermission(AppFeature.RoleClaims, AppAction.Update)]
        [HttpPut("update-permissions")]
        public async Task<IActionResult> UpdateRolePermission([FromBody] UpdateRolePermissionsRequest updateRolePermissionsRequest)
        {
            var response = await MeaditorSender
                .Send(new UpdateRolePermissionCommand(updateRolePermissionsRequest));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
