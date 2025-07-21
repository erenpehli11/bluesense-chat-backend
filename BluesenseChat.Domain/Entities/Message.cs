using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Domain.Entities
{
    public class Message : BaseEntity
    {

        [BsonSerializer(typeof(StandardGuidSerializer))]
        public Guid SenderId { get; set; }
        public virtual User Sender { get; set; }


        [BsonSerializer(typeof(NullableStandardGuidSerializer))]
        public Guid? ReceiverId { get; set; }


        public virtual User? Receiver { get; set; }


        [BsonSerializer(typeof(NullableStandardGuidSerializer))]
        public Guid? GroupId { get; set; }
        public virtual Group? Group { get; set; }



        [BsonSerializer(typeof(NullableStandardGuidSerializer))]
        public Guid? PrivateChatId { get; set; }

        public virtual PrivateChat? PrivateChat { get; set; }
        public string? Content { get; set; }

        public bool IsEdited { get; set; } = false;
        public DateTime? EditedAt { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
