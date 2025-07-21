using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Domain.Entities;

namespace BluesenseChat.Application.Interfaces.ServiceInterfaces
{
    public interface IMessageService
    {
        // 1. Grup mesajı gönder
        Task<ApiResponse<Message>> SendMessageAsync(SendMessageDto dto, Guid senderId);

        // 2. Grup mesajlarını getir (sayfalanabilir)
        Task<ApiResponse<List<MessageDto>>> GetGroupMessagesAsync(Guid groupId, int page, int pageSize, Guid requesterId);

        // 3. Özel mesajları getir (sayfalanabilir)
        Task<ApiResponse<List<MessageDto>>> GetPrivateMessagesAsync(Guid privateChatId, Guid requesterId, int page, int pageSize);

        // 4. Mesajı soft sil (sadece gönderen)
        Task<ApiResponse<string>> SoftDeleteMessageAsync(Guid messageId, Guid requesterId);


    }
}
