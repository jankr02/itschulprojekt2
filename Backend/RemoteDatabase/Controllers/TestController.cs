using Microsoft.AspNetCore.Mvc;

namespace RemoteDatabase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public TestController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public ActionResult<bool> TestConnection()
        {
            return Ok(true);
        }
    }
}