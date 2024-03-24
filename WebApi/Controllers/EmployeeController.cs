﻿using Application.Features.Employee.Commands;
using Application.Features.Employee.Queries;
using Common.Requests.Employee;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : AppBaseController<EmployeeController>
{
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
