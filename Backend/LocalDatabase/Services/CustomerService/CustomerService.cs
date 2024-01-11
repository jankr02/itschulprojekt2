namespace LocalDatabase.Services.CustomerService
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

        public async Task<ServiceResponse<GetCustomerDto>> AddCompleteCustomer(AddCompleteCustomerDto newCompleteCustomer)
        {
            var serviceResponse = new ServiceResponse<GetCustomerDto>();

            var addedCustomer = Task.Run(() => AddCustomer(_mapper.Map<AddCustomerDto>(newCompleteCustomer)).GetAwaiter().GetResult()).Result;
      
            if ((newCompleteCustomer.Picture != null) && CheckIfNotNull(newCompleteCustomer.Picture))
            {
              var customerAfterAdditionOfPicture = await AddPicture(newCompleteCustomer.Picture, addedCustomer.Id);
              addedCustomer = customerAfterAdditionOfPicture;
            }

            if ((newCompleteCustomer.Business != null) && CheckIfNotNull(newCompleteCustomer.Business))
            {
              var customerAfterAdditionOfBusiness = await AddBusiness(newCompleteCustomer.Business, addedCustomer.Id);
              addedCustomer = customerAfterAdditionOfBusiness.Data;
            }

            if ((newCompleteCustomer.ProductGroups != null) && !newCompleteCustomer.ProductGroups.Any())
            {
              var customerProductGroups = newCompleteCustomer.ProductGroups
                                                             .Select(productGroup => new AddCustomerProductGroupDto
                                                             {
                                                               CustomerId = addedCustomer!.Id,
                                                               ProductGroupId = GetProductGroupId(productGroup.Name)
                                                             })
                                                             .ToList();
              var customerAfterAdditionOfProductGroups = await AddCustomerProductGroup(customerProductGroups);
              addedCustomer = customerAfterAdditionOfProductGroups.FirstOrDefault(d => d.Id == addedCustomer!.Id);
            }
            
            if (addedCustomer != null)
            {
              serviceResponse.Data = addedCustomer;
            }
            
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

        private async Task<GetCustomerDto> AddCustomer(AddCustomerDto newCustomer)
        {
            Customer? existingCustomer;
            Customer? customer;

            if ((existingCustomer = await _context.Customers
                .Where(c => (c.FirstName == newCustomer.FirstName)
                    && (c.LastName == newCustomer.LastName)
                    && (c.Street == newCustomer.Street)
                    && (c.HouseNumber == newCustomer.HouseNumber)
                    && (c.PostalCode == newCustomer.PostalCode))
                .FirstOrDefaultAsync(c => c.City == newCustomer.City)) != null)
            {
                customer = existingCustomer;
            }
            else
            {
                customer = _mapper.Map<Customer>(newCustomer);
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<GetCustomerDto>(customer);
        }
        
        private async Task<List<GetCustomerDto>> AddCustomerProductGroup(List<AddCustomerProductGroupDto> newCustomerProductGroups)
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

            return await _context.Customers
                                 .Include(c => c.Picture)
                                 .Include(c => c.ProductGroups)
                                 .Include(c => c.Business)
                                 .Select(c => _mapper.Map<GetCustomerDto>(c))
                                 .ToListAsync();
        }

        private async Task<ServiceResponse<GetCustomerDto>> AddBusiness(AddBusinessDto newBusiness, int customerId)
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

        private async Task<GetCustomerDto> AddPicture(AddPictureDto newPicture, int customerId)
        {
            var pictureAsByteArray = Convert.FromBase64String(newPicture.Data) ?? throw new InvalidDataException("An image must be uploaded.");

            try
            {
              using var ms = new MemoryStream(pictureAsByteArray);
              Image.FromStream(ms);
            }
            catch
            {
              throw new InvalidDataException("The uploaded file must be an image.");
            }

            switch (pictureAsByteArray.Length)
            {
              case <= 0:
                throw new InvalidDataException("The image must contain data.");
              case > 10 * 1024 * 1024:
                throw new InvalidDataException("The image must not exceed 10MB.");
            }

            var customer = await _context.Customers
                                         .Include(c => c.Picture)
                                         .Include(c => c.ProductGroups)
                                         .Include(c => c.Business)
                                         .FirstOrDefaultAsync(c => c.Id == customerId) ?? throw new Exception($"Customer with Id '{customerId}' not found.");

            Picture? existingPicture;

            if ((existingPicture = await _context.Pictures.Where(p => (p.Customer != null) && (p.Customer.Id == customerId))
                                                 .FirstOrDefaultAsync(p => p.Name == newPicture.Name)) != null)
            {
              existingPicture.Image = pictureAsByteArray;
              customer.Picture = _mapper.Map<Picture>(existingPicture);
            }
            else
            {
              var newConvertedPicture = new Picture()
              {
                Name = newPicture.Name,
                Image = pictureAsByteArray
              };

              var pictures = await _context.Pictures.ToListAsync();
              pictures.Add(newConvertedPicture);
              customer.Picture = _mapper.Map<Picture>(newConvertedPicture);
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<GetCustomerDto>(customer);

        }
        
        private int GetProductGroupId(ProductGroupName productGroupName)
        {
            var result = _context.ProductGroups.FirstOrDefaultAsync(p => p.Name == productGroupName).GetAwaiter().GetResult();

            return result?.Id ?? 0;
        }

        private static bool CheckIfNotNull(Object testObject)
        {
          foreach (var pi in testObject.GetType().GetProperties())
          {
            if (pi.GetValue(testObject) == null)
            {
              return false;
            }

            if (pi.PropertyType != typeof(string))
            {
              continue;
            }

            var value = (string)pi.GetValue(testObject)!;
            if (string.IsNullOrEmpty(value))
            {
              return false;
            }
          }
          return true;
        }
  }
}