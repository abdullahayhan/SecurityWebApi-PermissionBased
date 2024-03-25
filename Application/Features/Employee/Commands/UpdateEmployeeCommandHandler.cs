using Application.Services;
using AutoMapper;
using Common.Responses;
using Common.Responses.Wrappers;
using Domain;
using MediatR;

namespace Application.Features.Employee.Commands;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;

    public UpdateEmployeeCommandHandler(IEmployeeService employeeService,
        IMapper mapper)
    {
        _employeeService = employeeService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(UpdateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.UpdateEmployeeRequest.Id);

        if (employeeInDb is not null)
        {
            employeeInDb.FirstName = request.UpdateEmployeeRequest.FirstName;
            employeeInDb.LastName = request.UpdateEmployeeRequest.LastName;
            employeeInDb.Salary = request.UpdateEmployeeRequest.Salary;
            employeeInDb.Email = request.UpdateEmployeeRequest.Email;

            var employeeUpdated = await _employeeService.UpdateEmployeeAsync(employeeInDb);
            if (employeeUpdated is not null)
            {
                var responseEmployee = _mapper.Map<EmloyeeResponse>(employeeUpdated);
                return await ResponseWrapper<EmloyeeResponse>
                    .SuccessAsync(responseEmployee, "Employee updated succesfully.");
            }
            return ResponseWrapper.Fail("Employee updated is failed.");
        }
        return ResponseWrapper.Fail("Employee does not found.");
    }
}
