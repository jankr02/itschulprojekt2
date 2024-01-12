namespace LocalDatabase.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, GetCustomerDto>()
              .ForPath(dest => dest.Picture.Id, opt => opt.MapFrom(src => src.Picture.Id))
              .ForPath(dest => dest.Picture.Name, opt => opt.MapFrom(src => src.Picture.Name))
              .ForPath(dest => dest.Picture.Data, opt => opt.MapFrom(src => Convert.ToBase64String(src.Picture.Image)));
            CreateMap<AddCompleteCustomerDto, AddCustomerDto>();
            CreateMap<AddCustomerDto, Customer>();

            CreateMap<Picture, GetPictureDto>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.Data, opt => opt.MapFrom(src => Convert.ToBase64String(src.Image)));
            CreateMap<AddPictureDto, Picture>()
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.Image, opt => opt.MapFrom(src => Convert.FromBase64String(src.Data)));

            CreateMap<Business, GetBusinessDto>();
            CreateMap<AddBusinessDto, Business>();

            CreateMap<ProductGroup, GetProductGroupDto>();
        }
    }
}
