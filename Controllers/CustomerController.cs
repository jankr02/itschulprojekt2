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
            var response = await _customerService.GetCustomerById(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerResponseDto>>>> AddCustomer(AddCustomerRequestDto newCustomer)
        {
            return Ok(await _customerService.AddCustomer(newCustomer));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerResponseDto>>>> UpdateCustomer(UpdateCustomerRequestDto updatedCustomer)
        {
            var response = await _customerService.UpdateCustomer(updatedCustomer);
            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCustomerResponseDto>>> DeleteSingleCustomer(int id)
        {
            var response = await _customerService.DeleteCustomer(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
