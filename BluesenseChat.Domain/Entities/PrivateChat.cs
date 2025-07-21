using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Domain.Entities
{
    public class PrivateChat : BaseEntity
    {


        [BsonSerializer(typeof(StandardGuidSerializer))]
        public Guid User1Id { get; set; }
        public virtual User User1 { get; set; }


        [BsonSerializer(typeof(StandardGuidSerializer))]
        public Guid User2Id { get; set; }
        public virtual User User2 { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
