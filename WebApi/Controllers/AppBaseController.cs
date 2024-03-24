using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public class AppBaseController<T> : ControllerBase
{
    private ISender _sender;

    public ISender MeaditorSender 
        => _sender ?? HttpContext?.RequestServices.GetService<ISender>();
}
