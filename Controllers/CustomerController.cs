using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<ServiceResponse<List<GetCustomerDto>>>> GetAllCustomers()
        {
           return Ok(await _customerService.GetAllCustomers());
        }

        [HttpGet("{id}")]
        public async Task <ActionResult<ServiceResponse<GetCustomerDto>>> GetSingleCustomer(int id)
        {
            var response = await _customerService.GetCustomerById(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerDto>>>> AddCustomer(AddCustomerDto newCustomer)
        {
            return Ok(await _customerService.AddCustomer(newCustomer));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerDto>>>> UpdateCustomer(UpdateCustomerDto updatedCustomer)
        {
            var response = await _customerService.UpdateCustomer(updatedCustomer);
            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCustomerDto>>> DeleteSingleCustomer(int id)
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
