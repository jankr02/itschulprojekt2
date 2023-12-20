using Microsoft.AspNetCore.Mvc;

namespace RemoteDatabase.Controllers
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

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerDto>>>> GetAllCustomers()
        {
           return Ok(await _customerService.GetAllCustomers());
        }

        [HttpGet("{id:int}")]
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
        public async Task<ActionResult<ServiceResponse<GetCustomerDto>>> AddCustomer(AddCustomerDto newCustomer)
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ServiceResponse<GetCustomerDto>>> DeleteSingleCustomer(int id)
        {
            var response = await _customerService.DeleteCustomer(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("ProductGroup")]
        public async Task<ActionResult<ServiceResponse<List<GetCustomerDto>>>> AddCustomerProductGroup(List<AddCustomerProductGroupDto> newCustomerProductGroups)
        {
            var response = await _customerService.AddCustomerProductGroup(newCustomerProductGroups);
            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("Business/{customerId:int}")]
        public async Task<ActionResult<ServiceResponse<GetCustomerDto>>> AddBusiness(AddBusinessDto newBusiness, int customerId)
        {
            var response = await _customerService.AddBusiness(newBusiness, customerId);
            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("Picture/{customerId:int}")]
        public async Task<ActionResult<ServiceResponse<GetCustomerDto>>> AddPicture(IFormFile image, int customerId)
        {
            var newPicture = new AddPictureDto()
            { 
                Name = image.FileName,
                Image = (FormFile)image
            };
            var response = await _customerService.AddPicture(newPicture, customerId);
            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}