using BluesenseChat.Application.Interfaces.RepositoryInterfaces;
using BluesenseChat.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluesenseChat.Domain.Entities;

namespace BluesenseChat.Infrastructure.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(AppDbContext context) : base(context) { }

        public async Task<List<Group>> GetGroupsWithMembersAndUsersAsync()
        {
            return await _context.Groups
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .ToListAsync();
        }
    }
}
