namespace MesseauftrittDatenerfassung.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<ServiceResponse<List<GetCustomerResponseDto>>> GetAllCustomers();
        Task<ServiceResponse<GetCustomerResponseDto>> GetCustomerById(int id);
        Task<ServiceResponse<List<GetCustomerResponseDto>>> AddCustomer(AddCustomerRequestDto newCustomer);
        Task<ServiceResponse<GetCustomerResponseDto>> UpdateCustomer(UpdateCustomerRequestDto updatedCustomer);
        Task<ServiceResponse<List<GetCustomerResponseDto>>> DeleteCustomer(int id);
    }
}
