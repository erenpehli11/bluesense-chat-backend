using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Domain.Entities
{
    public class GroupMember : BaseEntity
    {
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public bool IsAdmin { get; set; } = false;
        public bool IsMuted { get; set; } = false;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public string InvitationStatus { get; set; } // "Pending", "Accepted", "Rejected"
    }
}
