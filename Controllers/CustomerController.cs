using Microsoft.AspNetCore.Mvc;

namespace MesseauftrittDatenerfassung.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
                _customerService = customerService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerResponseDto>>>> GetAllCustomers()
        {
           return Ok(await _customerService.GetAllCustomers());
        }

        [HttpGet("{id}")]
        public async Task <ActionResult<ServiceResponse<GetCustomerResponseDto>>> GetSingleCustomer(int id)
        {
            return Ok(await _customerService.GetCustomerById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerResponseDto>>>> AddCustomer(AddCustomerRequestDto customer)
        {
            return Ok(await _customerService.AddCustomer(customer));
        }
    }
}
