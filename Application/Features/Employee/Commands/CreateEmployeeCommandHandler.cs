using Application.Services;
using AutoMapper;
using Common.Responses.Employee;
using Common.Responses.Wrappers;
using Domain;
using MediatR;

namespace Application.Features.Employee.Commands;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;
    
    public CreateEmployeeCommandHandler(IEmployeeService employeeService, 
        IMapper mapper)
    {
        _employeeService = employeeService;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var mappedEmployee = _mapper.Map<Domain.Employee>(request.CreateEmployeeRequest); // controllerdan bizim request geliyor ama servise domain entity vermemiz lazım.
        var newEmployee = await _employeeService.CreateEmployeeAsync(mappedEmployee); // mapledikten sonra servise değerleri gönderiyoruz ve bize domain entity geri dönüyor.
        if (newEmployee.Id > 0)
        {
            var mappedNewEmployee = _mapper.Map<EmloyeeResponse>(newEmployee); // işlem başarılı ise gelen domain entity'i alıp response tipine dönüştürürüz.
            return await ResponseWrapper<EmloyeeResponse>
                .SuccessAsync(mappedNewEmployee, "Employee created succesfully."); // çevirdiğimiz entity'i burada en temel dönüş tipimiz olan ResponseWrapper'a veriyoruz.
        }
        return await ResponseWrapper.FailAsync("Failed to creat employee."); // hata varsa mesaj gönderiyoruz.
    }
    
}
