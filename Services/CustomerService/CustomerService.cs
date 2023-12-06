
namespace MesseauftrittDatenerfassung.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CustomerService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCustomerDto>>> AddCustomer(AddCustomerDto newCustomer)
        {
            var serviceResponse = new ServiceResponse<List<GetCustomerDto>>();
            var customer = _mapper.Map<Customer>(newCustomer);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Customers
                .Include(c => c.Picture)
                .Include(c => c.ProductGroups)
                .Include(c => c.Business)
                .Select(c => _mapper.Map<GetCustomerDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCustomerDto>>> GetAllCustomers()
        {
            var serviceResponse = new ServiceResponse<List<GetCustomerDto>>();
            var dbcustomers = await _context.Customers
                .Include(c => c.Picture)
                .Include(c => c.ProductGroups)
                .Include(c => c.Business)
                .ToListAsync();
            serviceResponse.Data = dbcustomers.Select(c => _mapper.Map<GetCustomerDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCustomerDto>> GetCustomerById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCustomerDto>();
            try
            {
                var dbcustomer = await _context.Customers
                    .Include(c => c.Picture)
                    .Include(c => c.ProductGroups)
                    .Include(c => c.Business)
                    .FirstOrDefaultAsync(c => c.Id == id) ?? throw new Exception($"Customer with Id '{id}' not found.");
                serviceResponse.Data = _mapper.Map<GetCustomerDto>(dbcustomer);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCustomerDto>> UpdateCustomer(UpdateCustomerDto updatedCustomer)
        {
            var serviceResponse = new ServiceResponse<GetCustomerDto>();
            
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == updatedCustomer.Id) ?? throw new Exception($"Customer with Id '{updatedCustomer.Id}' not found.");
                _mapper.Map(updatedCustomer, customer);

                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCustomerDto>(customer);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCustomerDto>>> DeleteCustomer(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCustomerDto>>();

            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id) ?? throw new Exception($"Customer with Id '{id}' not found.");
                _context.Customers.Remove(customer);

                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Customers.Select(c => _mapper.Map<GetCustomerDto>(c)).ToListAsync();
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
