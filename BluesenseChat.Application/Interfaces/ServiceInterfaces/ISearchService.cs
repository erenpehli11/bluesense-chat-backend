using BluesenseChat.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Interfaces.ServiceInterfaces
{
    public interface ISearchService
    {
        Task<PaginatedResultDto<SearchResultDto>> SearchMessagesAsync(SearchQueryDto query);
    }

}
