using Application.Pipelines;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employee.Commands;

public record class DeleteEmployeeCommand(int id)
    : IRequest<IResponseWrapper>, IValidateMe;


public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService;

    public DeleteEmployeeCommandHandler(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<IResponseWrapper> Handle(DeleteEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        //var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.id);
        //if (employeeInDb is not null)
        //{
            var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.id);
            var employeeId = await _employeeService.DeleteEmployeeAsync(employeeInDb);
            return await ResponseWrapper<int>.SuccessAsync(employeeId,
                "Employee entry deleted succesfly");
        //}
        //else
        //{
        //    return ResponseWrapper.Fail("Employee does not exists.");
        //}
    }
}
