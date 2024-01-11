namespace LocalDatabase.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<ServiceResponse<List<GetCustomerDto>>> GetAllCustomers();
        Task<ServiceResponse<GetCustomerDto>> AddCompleteCustomer(AddCompleteCustomerDto newCompleteCustomer);
    }
}