using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using BluesenseChat.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluesenseChat.Application.Interfaces.RepositoryInterfaces;

namespace BluesenseChat.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IMongoMessageRepository _mongoRepo;

        public SearchService(IMongoMessageRepository mongoRepo)
        {
            _mongoRepo = mongoRepo;
        }

        public async Task<PaginatedResultDto<SearchResultDto>> SearchMessagesAsync(SearchQueryDto query)
        {
            var filter = Builders<Message>.Filter.Where(m =>
                !string.IsNullOrEmpty(query.Query) &&
                m.Content.ToLower().Contains(query.Query.ToLower()));

            var totalCount = await _mongoRepo.Collection.CountDocumentsAsync(filter);

            var skip = (query.PageNumber - 1) * query.PageSize;

            var messages = await _mongoRepo.Collection
                .Find(filter)
                .SortByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Limit(query.PageSize)
                .ToListAsync();

            var results = messages.Select(m => new SearchResultDto
            {
                MessageId = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                GroupId = m.GroupId,
                CreatedAt = m.CreatedAt
            }).ToList();

            return new PaginatedResultDto<SearchResultDto>
            {
                Items = results,
                TotalCount = (int)totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }

}
