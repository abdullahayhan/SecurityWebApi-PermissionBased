using Common.Requests.Employee;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employee.Commands;

public record class CreateEmployeeCommand(CreateEmployeeRequest CreateEmployeeRequest) 
    :IRequest<IResponseWrapper>;
