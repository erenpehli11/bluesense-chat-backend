using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluesenseChat.Domain.Entities;

namespace BluesenseChat.Application.Interfaces.RepositoryInterfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<List<Group>> GetGroupsWithMembersAndUsersAsync();
    }
}
