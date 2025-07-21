using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BluesenseChat.Domain.Entities
{
    public class Group : BaseEntity
    {
        public string Name { get; set; }
        public bool IsPrivate { get; set; }

        public string InviteLink { get; set; }


        [BsonSerializer(typeof(StandardGuidSerializer))]
        public Guid OwnerId { get; set; }
        public virtual User Owner { get; set; }


        [JsonIgnore]
        public virtual ICollection<GroupMember> Members { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<GroupInvitation> Invitations { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }

    }
}
