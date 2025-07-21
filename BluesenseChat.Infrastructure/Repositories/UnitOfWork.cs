using BluesenseChat.Application.Interfaces.RepositoryInterfaces;
using BluesenseChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluesenseChat.Infrastructure.Persistance;

namespace BluesenseChat.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Users = new GenericRepository<User>(_context);
            Messages = new GenericRepository<Message>(_context);
            Groups = new GroupRepository(_context);
            GroupMembers = new GenericRepository<GroupMember>(_context);
            PrivateChats = new GenericRepository<PrivateChat>(_context);
            Attachments = new GenericRepository<Attachment>(_context);
            RefreshTokens = new GenericRepository<RefreshToken>(_context);
            GroupInvitations = new GenericRepository<GroupInvitation>(_context);
            SearchIndexes = new GenericRepository<SearchIndex>(_context);
        }

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Message> Messages { get; }
        public IGroupRepository Groups { get; }

        public IGenericRepository<GroupMember> GroupMembers { get; }
        public IGenericRepository<PrivateChat> PrivateChats { get; }
        public IGenericRepository<Attachment> Attachments { get; }
        public IGenericRepository<RefreshToken> RefreshTokens { get; }
        public IGenericRepository<GroupInvitation> GroupInvitations { get; }
        public IGenericRepository<SearchIndex> SearchIndexes { get; }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }

}
