using BluesenseChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Interfaces.RepositoryInterfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Message> Messages { get; }
        IGroupRepository Groups { get; }



        IGenericRepository<GroupMember> GroupMembers { get; }

        IGenericRepository<PrivateChat> PrivateChats { get; }
        IGenericRepository<Attachment> Attachments { get; }
        IGenericRepository<RefreshToken> RefreshTokens { get; }
        IGenericRepository<GroupInvitation> GroupInvitations { get; }
        IGenericRepository<SearchIndex> SearchIndexes { get; }

        Task<int> SaveChangesAsync();
    }
}
