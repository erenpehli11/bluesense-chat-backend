using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using BluesenseChat.Domain.Entities;
using AutoMapper;

namespace BluesenseChat.API.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;

        public MessageHub(IMessageService messageService ,IMapper mapper )
        {
            _messageService = messageService;
           _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"✅ Bağlanan: {userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"❌ Ayrılan: {userId}");
            await base.OnDisconnectedAsync(exception);
        }


        // 🔁 Hem private hem grup mesajlarını iletmek için
        public async Task SendMessage(SendMessageDto dto)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId)) return;

            var result = await _messageService.SendMessageAsync(dto, Guid.Parse(senderId));

            if (result.IsSuccess)
            {
                var message = result.Data; 

                var messageDto = _mapper.Map<MessageDto>(message);

                if (dto.GroupId.HasValue)
                {
                    await Clients.Group(dto.GroupId.ToString()!).SendAsync("ReceiveMessage", messageDto);
                }
                else if (dto.ReceiverId.HasValue)
                {
                    await Clients.User(dto.ReceiverId.ToString()!).SendAsync("ReceiveMessage", messageDto);
                }
            }
        }
    }
}
    

