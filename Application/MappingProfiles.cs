using AutoMapper;
using Common.Requests.Employee;
using Common.Responses;
using Domain;

namespace Application;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<CreateEmployeeRequest, Employee>();
		CreateMap<Employee, EmloyeeResponse>();
	}
}
