using AutoMapper;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DTO;
using bull_chat_backend.Repository.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace bull_chat_backend.Hubs
{


    /*
     * INFO:    В веб-сокетах и SignalR мы не можем использовать стандартный заголовок Authorization, поэтому токен передается как query-параметр:
     *          Обращатся к хабу так /chat?access_token=ваш_токен_здесь
     *          access_token забит как строковый литера (ЖЁСТКО, КАК И ДЕЛАЮТ БЫЧКИ см. Extensions.JwtAuthenticationExtensions.cs)
     */
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        public const string HUB_URI = "/chat";
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IContentRepository _contentRepository;

        public ChatHub(
            IUserRepository userRepository,
            IMessageRepository messageRepository,
            IContentRepository contentRepository,
            IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _contentRepository = contentRepository;
        }

        public async Task SendMessage(MessageDto messageDto)
        {
            var token = Context.ConnectionAborted;

            var user = await _userRepository.GetByIdAsync(messageDto.UserId, token) ?? throw new HubException("User not found");

            var message = _mapper.Map<MessageDto, Message>(messageDto);
            await _messageRepository.AddAsync(message, token);

            await Clients.All.ReceiveMessage(messageDto);
        }
    }
}
