using Microsoft.AspNetCore.Mvc;

namespace RemoteDatabase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<bool> TestConnection()
        {
            return Ok(true);
        }
    }
}