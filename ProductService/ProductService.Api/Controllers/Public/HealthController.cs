using Microsoft.AspNetCore.Mvc;

namespace ProductService.Api.Controllers.Public;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return "OK";
    }
}