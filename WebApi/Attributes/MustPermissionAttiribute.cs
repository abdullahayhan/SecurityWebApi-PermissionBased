using Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Attributes;

public class MustPermission : AuthorizeAttribute
{
	public MustPermission(string feature, string action)
		=> Policy = AppPermission.NameFor(feature, action);
}
