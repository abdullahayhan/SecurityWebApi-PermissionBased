using Application.Services;
using AutoMapper;
using Common.Responses;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employee.Queries;

public record class GetListEmployeeQuery : IRequest<IResponseWrapper>;

public class GetListEmployeeQueryHandler : IRequestHandler<GetListEmployeeQuery, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;

    public GetListEmployeeQueryHandler(IEmployeeService employeeService, 
        IMapper mapper)
    {
        _employeeService = employeeService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(GetListEmployeeQuery request, 
        CancellationToken cancellationToken)
    {
        var employeeInDbList = await _employeeService.GetEmployeeListAsync();
        if (employeeInDbList.Count > 0)
        {
            var mappedEmployeeList = _mapper.Map<List<EmloyeeResponse>>(employeeInDbList);
            return await ResponseWrapper<List<EmloyeeResponse>>.SuccessAsync(mappedEmployeeList);
        }
        return await ResponseWrapper.FailAsync("No employees were found");
    }
}
