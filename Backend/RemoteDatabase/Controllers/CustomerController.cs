using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RemoteDatabase.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerDto>>>> GetAllCustomers()
        {
           return Ok(await _customerService.GetAllCustomers());
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerDto>>>> AddMultipleCompleteCustomers(List<AddCompleteCustomerDto> newCustomers)
        {
            return Ok(await _customerService.AddMultipleCompleteCustomers(newCustomers));
        }
    }
}