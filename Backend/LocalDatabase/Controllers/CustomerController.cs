using Microsoft.AspNetCore.Mvc;

namespace LocalDatabase.Controllers
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
    
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<GetCustomerDto>>> AddCompleteCustomer(AddCompleteCustomerDto newCustomer)
    {
      return Ok(await _customerService.AddCompleteCustomer(newCustomer));
    }

    [HttpDelete]
    public async Task<ActionResult<ServiceResponse<List<GetCustomerDto>>>> TruncateAllTables()
    {
        return Ok(await _customerService.TruncateAllTables());
    }
  }
}
