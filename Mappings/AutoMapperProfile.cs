namespace MesseauftrittDatenerfassung.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, GetCustomerDto>();
                //.ForPath(dest => dest.Picture.Image, opt => opt.MapFrom(src => new CustomImageConverter().ByteArrayToImage(src.Picture.Data)));
            CreateMap<AddCustomerDto, Customer>();
                //.ForPath(dest => dest.Picture.Data, opt => opt.MapFrom(src => new CustomImageConverter().ImageToByteArray(src.Picture.Image)));
            CreateMap<UpdateCustomerDto, Customer>();
                //.ForPath(dest => dest.Picture.Data, opt => opt.MapFrom(src => new CustomImageConverter().ImageToByteArray(src.Picture.Image)));

            CreateMap<Picture, GetPictureDto>();
                //.ForMember(dest => dest.Image, opt => opt.MapFrom<CustomToImageMemberResolver, byte[]?>(src => src.Data));
            CreateMap<AddPictureDto, Picture>();
                //.ForMember(dest => dest.Data, opt => opt.MapFrom<CustomToByteArrayMemberResolver, Image?>(src => src.Image));
            CreateMap<UpdatePictureDto, Picture>();
                //.ForMember(dest => dest.Data, opt => opt.MapFrom<CustomToByteArrayMemberResolver, Image?>(src => src.Image));

            CreateMap<Business, GetBusinessDto>();
            CreateMap<AddBusinessDto, Business>();
            CreateMap<UpdateBusinessDto, Business>();

            CreateMap<ProductGroup, GetProductGroupDto>();
            CreateMap<AddProductGroupDto, ProductGroup>();
            CreateMap<UpdateProductGroupDto, ProductGroup>();
        }
    }
}
