using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Rating, RatingResponseDto>();
        CreateMap<RatingCreateDto, Rating>();
        CreateMap<WashRequest, WashRequestResponseDto>();
        CreateMap<WashRequestResponseDto, WashRequest>();
    }
}