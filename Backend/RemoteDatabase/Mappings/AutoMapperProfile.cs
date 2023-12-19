namespace RemoteDatabase.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, GetCustomerDto>();
            CreateMap<AddCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();

            CreateMap<Picture, GetPictureDto>();
            CreateMap<AddPictureDto, Picture>();

            CreateMap<Business, GetBusinessDto>();
            CreateMap<AddBusinessDto, Business>();

            CreateMap<ProductGroup, GetProductGroupDto>();
        }
    }
}
