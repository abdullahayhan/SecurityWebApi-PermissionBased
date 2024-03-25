using Application.Services;
using AutoMapper;
using Common.Responses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employee.Queries;

public record class GetEmployeeQuery(int id)
    : IRequest<IResponseWrapper>;

public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService; 
    private readonly IMapper _mapper;

    public GetEmployeeQueryHandler(IEmployeeService employeeService, IMapper mapper)
    {
        _employeeService = employeeService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(GetEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.id);
        if (employeeInDb is not null)
        {
            var responseEmployee = _mapper.Map<EmloyeeResponse>(employeeInDb);

            return await ResponseWrapper<EmloyeeResponse>
                .SuccessAsync(responseEmployee);
        }
        return ResponseWrapper.Fail("Employee does not exists.");
    }
}
