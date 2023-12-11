
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
                var customer = await _context.Customers
                    .Include(c => c.Picture)
                    .FirstOrDefaultAsync(c => c.Id == id) ?? throw new Exception($"Customer with Id '{id}' not found.");

                var picture = await _context.Pictures.FirstOrDefaultAsync(p => p.Id == customer.Picture.Id);

                if (picture != null)
                {
                    _context.Pictures.Remove(picture);
                }

                _context.Customers.Remove(customer);

                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Customers
                    .Include(c => c.Picture)
                    .Include(c => c.ProductGroups)
                    .Include(c => c.Business)
                    .Select(c => _mapper.Map<GetCustomerDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCustomerDto>>> AddCustomerProductGroup(List<AddCustomerProductGroupDto> newCustomerProductGroups)
        {
            var serviceResponse = new ServiceResponse<List<GetCustomerDto>>();

            try
            {
                foreach (var newCustomerProductGroup in newCustomerProductGroups)
                {
                    var customer = await _context.Customers
                        .Include(c => c.Picture)
                        .Include(c => c.ProductGroups)
                        .Include(c => c.Business)
                        .FirstOrDefaultAsync(c => c.Id == newCustomerProductGroup.CustomerId) ?? throw new Exception($"Customer with Id '{newCustomerProductGroup.CustomerId}' not found.");
                    
                    var productGroup = await _context.ProductGroups.FirstOrDefaultAsync(p => p.Id == newCustomerProductGroup.ProductGroupId) ?? throw new Exception($"ProductGroup with Id '{newCustomerProductGroup.ProductGroupId}' not found.");

                    customer.ProductGroups ??= new List<ProductGroup>();

                    if (!customer.ProductGroups.Contains(productGroup))
                    {
                        customer.ProductGroups.Add(productGroup);
                    }
                }

                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Customers
                        .Include(c => c.Picture)
                        .Include(c => c.ProductGroups)
                        .Include(c => c.Business)
                        .Select(c => _mapper.Map<GetCustomerDto>(c)).ToListAsync();

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCustomerDto>> AddBusiness(AddBusinessDto newBusiness, int customerId)
        {
            var serviceResponse = new ServiceResponse<GetCustomerDto>();

            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Picture)
                    .Include(c => c.ProductGroups)
                    .Include(c => c.Business)
                    .FirstOrDefaultAsync(c => c.Id == customerId) ?? throw new Exception($"Customer with Id '{customerId}' not found.");

                Business? existingBusiness;

                if ((existingBusiness = await _context.Businesses.FirstOrDefaultAsync(b => b.Name == newBusiness.Name)) != null)
                {
                    _mapper.Map(newBusiness, existingBusiness);
                    customer.Business = _mapper.Map<Business>(existingBusiness);
                }
                else
                {
                    var businesses = await _context.Businesses.ToListAsync();
                    businesses.Add(_mapper.Map<Business>(newBusiness));
                    customer.Business = _mapper.Map<Business>(newBusiness);
                }

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

        public async Task<ServiceResponse<GetCustomerDto>> AddPicture(AddPictureDto newPicture, int customerId)
        {
            var serviceResponse = new ServiceResponse<GetCustomerDto>();

            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Picture)
                    .Include(c => c.ProductGroups)
                    .Include(c => c.Business)
                    .FirstOrDefaultAsync(c => c.Id == customerId) ?? throw new Exception($"Customer with Id '{customerId}' not found.");

                Picture? existingPicture;
                var imageConverter = new ImageConverter();

                if ((existingPicture = await _context.Pictures.FirstOrDefaultAsync(p => p.Name == newPicture.Name)) != null)
                {
                    existingPicture.Name = newPicture.Name;
                    existingPicture.Data = (byte[]?)imageConverter.ConvertTo(newPicture.Image, typeof(byte[]));
                    customer.Picture = _mapper.Map<Picture>(existingPicture);
                }
                else
                {
                    var newConvertedPicture = new Picture()
                    {
                        Name = newPicture.Name,
                        Data = (byte[]?)imageConverter.ConvertTo(newPicture.Image, typeof(byte[]))
                };

                    var pictures = await _context.Pictures.ToListAsync();
                    pictures.Add(newConvertedPicture);
                    customer.Picture = _mapper.Map<Picture>(newConvertedPicture);
                }

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
    }
}
