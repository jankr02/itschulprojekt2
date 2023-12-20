namespace LocalDatabase.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<ServiceResponse<List<GetCustomerDto>>> GetAllCustomers();
        Task<ServiceResponse<GetCustomerDto>> GetCustomerById(int id);
        Task<ServiceResponse<GetCustomerDto>> AddCustomer(AddCustomerDto newCustomer);
        Task<ServiceResponse<GetCustomerDto>> UpdateCustomer(UpdateCustomerDto updatedCustomer);
        Task<ServiceResponse<List<GetCustomerDto>>> DeleteCustomer(int id);
        Task<ServiceResponse<List<GetCustomerDto>>> AddCustomerProductGroup(List<AddCustomerProductGroupDto> newCustomerProductGroups);
        Task<ServiceResponse<GetCustomerDto>> AddBusiness(AddBusinessDto newBusiness, int customerId);
        Task<ServiceResponse<GetCustomerDto>> AddPicture(AddPictureDto newPicture, int customerId);
    }
}