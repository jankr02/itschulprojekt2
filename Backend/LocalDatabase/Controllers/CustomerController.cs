using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

    //[Authorize]
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
  }
}
