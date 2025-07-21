using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.DTOs
{
    public class GroupSummaryDto
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public List<GroupMemberDto> Members { get; set; } = new();
    }

}
