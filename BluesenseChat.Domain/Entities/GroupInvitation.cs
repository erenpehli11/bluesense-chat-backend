using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Domain.Entities
{
    public class GroupInvitation : BaseEntity
    {
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }

        public Guid InvitedByUserId { get; set; }
        public virtual User InvitedByUser { get; set; }

        public Guid InvitedUserId { get; set; }
        public virtual User InvitedUser { get; set; }

        public string Status { get; set; } // "Pending", "Accepted", "Rejected"
        public DateTime ExpiresAt { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}
