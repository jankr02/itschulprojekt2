namespace MesseauftrittDatenerfassung.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<ServiceResponse<List<GetCustomerResponseDto>>> GetAllCustomers();
        Task<ServiceResponse<GetCustomerResponseDto>> GetCustomerById(int id);
        Task<ServiceResponse<List<GetCustomerResponseDto>>> AddCustomer(AddCustomerRequestDto customer);
    }
}
