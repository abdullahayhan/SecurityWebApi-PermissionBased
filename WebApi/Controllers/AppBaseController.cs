using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace WebApi.Controllers;

[EnableRateLimiting("token")]
[ApiController]
public class AppBaseController<T> : ControllerBase
{
    private ISender _sender;

    public ISender MeaditorSender 
        => _sender ?? HttpContext?.RequestServices.GetService<ISender>();
}
