using Common.Requests.Employee;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employee.Commands;

public record class UpdateEmployeeCommand(UpdateEmployeeRequest UpdateEmployeeRequest) 
    : IRequest<IResponseWrapper>;
