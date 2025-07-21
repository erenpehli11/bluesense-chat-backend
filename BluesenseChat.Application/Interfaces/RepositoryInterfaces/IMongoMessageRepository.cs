using BluesenseChat.Domain.Entities;
using MongoDB.Driver;


namespace BluesenseChat.Application.Interfaces.RepositoryInterfaces
{
    public interface IMongoMessageRepository
    {

        IMongoCollection<Message> Collection { get; }

        Task AddAsync(Message message);

        Task<Message?> GetByIdAsync(Guid messageId);
        Task<List<Message>> GetGroupMessages(Guid groupId, int page, int pageSize);
        Task<List<Message>> GetPrivateMessages(Guid privateChatId, int page, int pageSize);
        Task SoftDeleteAsync(Guid messageId, Guid requesterId);
    }
}
