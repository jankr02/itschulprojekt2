namespace RemoteDatabase.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, GetCustomerDto>()
              .ForPath(dest => dest.Picture.Data, opt => opt.MapFrom(src => Convert.ToBase64String(src.Picture.Image)));
            CreateMap<AddCompleteCustomerDto, AddCustomerDto>();
            CreateMap<AddCustomerDto, Customer>();

            CreateMap<Picture, GetPictureDto>()
              .ForPath(dest => dest.Data, opt => opt.MapFrom(src => Convert.ToBase64String(src.Image)));
            CreateMap<AddPictureDto, Picture>()
              .ForPath(dest => dest.Image, opt => opt.MapFrom(src => Convert.FromBase64String(src.Data)));

            CreateMap<Business, GetBusinessDto>();
            CreateMap<AddBusinessDto, Business>();

            CreateMap<ProductGroup, GetProductGroupDto>();
        }
    }
}
