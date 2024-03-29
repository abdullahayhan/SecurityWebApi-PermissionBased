using AutoMapper;
using Common.Responses;
using Infrastructure.Models;

namespace Infrastructure;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<ApplicationUser, UserResponse>();

	}
}
