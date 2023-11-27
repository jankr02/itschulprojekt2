
namespace MesseauftrittDatenerfassung.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private static List<Customer> customers = new()
        {
            new Customer (),
            new Customer { Id = 2, FirstName = "Neuer", LastName = "Kunde"}
        };

        private readonly IMapper _autoMapper;

        public CustomerService(IMapper autoMapper)
        {
            _autoMapper = autoMapper;
        }

        public async Task<ServiceResponse<List<GetCustomerResponseDto>>> AddCustomer(AddCustomerRequestDto newCustomer)
        {
            var serviceResponse = new ServiceResponse<List<GetCustomerResponseDto>>();
            var customer = _autoMapper.Map<Customer>(newCustomer);
            customer.Id = customers.Max(c => c.Id) + 1;
            customers.Add(customer);
            serviceResponse.Data = customers.Select(c => _autoMapper.Map<GetCustomerResponseDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCustomerResponseDto>>> GetAllCustomers()
        {
            var serviceResponse = new ServiceResponse<List<GetCustomerResponseDto>>();
            serviceResponse.Data = customers.Select(c => _autoMapper.Map<GetCustomerResponseDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCustomerResponseDto>> GetCustomerById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCustomerResponseDto>();
            try
            {
                var customer = customers.FirstOrDefault(c => c.Id == id) ?? throw new Exception($"Customer with Id '{id}' not found.");
                serviceResponse.Data = _autoMapper.Map<GetCustomerResponseDto>(customer);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCustomerResponseDto>> UpdateCustomer(UpdateCustomerRequestDto updatedCustomer)
        {
            var serviceResponse = new ServiceResponse<GetCustomerResponseDto>();
            
            try
            {
                var customer = customers.FirstOrDefault(c => c.Id == updatedCustomer.Id) ?? throw new Exception($"Customer with Id '{updatedCustomer.Id}' not found.");
                _autoMapper.Map(updatedCustomer, customer);
                serviceResponse.Data = _autoMapper.Map<GetCustomerResponseDto>(customer);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCustomerResponseDto>>> DeleteCustomer(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCustomerResponseDto>>();

            try
            {
                var customer = customers.FirstOrDefault(c => c.Id == id) ?? throw new Exception($"Customer with Id '{id}' not found.");
                customers.Remove(customer);
                serviceResponse.Data = customers.Select(c => _autoMapper.Map<GetCustomerResponseDto>(c)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}
