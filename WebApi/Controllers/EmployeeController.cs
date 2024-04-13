using Application.Features.Employee.Commands;
using Application.Features.Employee.Queries;
using Common.Authorization;
using Common.Requests.Employee;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class EmployeeController : AppBaseController<EmployeeController>
{
    [MustPermission(AppFeature.Employees,AppAction.Create)]
    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest createEmployeeRequest)
    {
        var response = await MeaditorSender.Send(new CreateEmployeeCommand(createEmployeeRequest));
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [MustPermission(AppFeature.Employees, AppAction.Update)]
    [HttpPut]
    public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest updateEmployeeRequest)
    {
        var response = await MeaditorSender.Send(new UpdateEmployeeCommand(updateEmployeeRequest));
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [MustPermission(AppFeature.Employees, AppAction.Delete)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee([FromBody] int id)
    {
        var response = await MeaditorSender.Send(new DeleteEmployeeCommand(id));
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [MustPermission(AppFeature.Employees,AppAction.Read)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById([FromBody] int id)
    {
        var response = await MeaditorSender.Send(new GetEmployeeQuery(id));
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [MustPermission(AppFeature.Employees, AppAction.Read)]
    [HttpGet]
    public async Task<IActionResult> GetEmployeeList()
    {
        var response = await MeaditorSender.Send(new GetListEmployeeQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

}
