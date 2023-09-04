using Microsoft.AspNetCore.Mvc;

namespace AppointmentsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            // TODO: Add dependency checks.
            return Ok(new
            {
                message = "healthy"
            });
        }
    }
}
