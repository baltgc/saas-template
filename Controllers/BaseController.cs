using Microsoft.AspNetCore.Mvc;

namespace saas_template.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger Logger;

    protected BaseController(ILogger logger)
    {
        Logger = logger;
    }
}

