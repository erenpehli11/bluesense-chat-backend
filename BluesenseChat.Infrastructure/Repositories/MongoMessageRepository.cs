using BluesenseChat.Application.Common.Settings;
using BluesenseChat.Application.Interfaces.RepositoryInterfaces;
using BluesenseChat.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Infrastructure.Repositories
{
    public class MongoMessageRepository : IMongoMessageRepository
    {
        private readonly IMongoCollection<Message> _messages;

        public MongoMessageRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _messages = database.GetCollection<Message>(settings.Value.MessagesCollection);


        }

        public IMongoCollection<Message> Collection => _messages;


        public async Task AddAsync(Message message)
        {
            await _messages.InsertOneAsync(message);
        }

        public async Task<Message?> GetByIdAsync(Guid messageId)
        {
            return await _messages.Find(m => m.Id == messageId).FirstOrDefaultAsync();
        }


        public async Task<List<Message>> GetGroupMessages(Guid groupId, int page, int pageSize)
        {
            return await _messages.Find(m => m.GroupId == groupId)
                .SortByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<List<Message>> GetPrivateMessages(Guid chatId, int page, int pageSize)
        {
            return await _messages.Find(m => m.PrivateChatId == chatId)
                .SortByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task SoftDeleteAsync(Guid messageId, Guid requesterId)
        {
            var update = Builders<Message>.Update.Set(m => m.IsDeleted, true);
            await _messages.UpdateOneAsync(
                m => m.Id == messageId && m.SenderId == requesterId,
                update);
        }
    }
}
