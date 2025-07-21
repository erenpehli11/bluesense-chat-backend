using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.DTOs
{
    public class GroupMemberDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
