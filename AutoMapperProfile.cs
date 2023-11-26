namespace MesseauftrittDatenerfassung
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, GetCustomerResponseDto>();
            CreateMap<AddCustomerRequestDto, Customer>();
        }
    }
}
