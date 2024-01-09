namespace RemoteDatabase.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<ServiceResponse<List<GetCustomerDto>>> GetAllCustomers();
        Task<ServiceResponse<List<GetCustomerDto>>> AddMultipleCompleteCustomers(List<AddCompleteCustomerDto> newCompleteCustomers);
    }
}