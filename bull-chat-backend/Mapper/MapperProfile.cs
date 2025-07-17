using AutoMapper;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DTO;

namespace bull_chat_backend.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Content, ContentDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Message, MessageDto>().ReverseMap();

            // MessageDto -> Message
            CreateMap<MessageDto, Message>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => new Content
                { 
                    Id = src.ContentId,
                    Text = src.ContentText
                }))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User
                {
                    Id = src.UserId,
                    Name = src.UserName
                }));
        }
    }
}
