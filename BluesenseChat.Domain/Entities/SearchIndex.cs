using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Domain.Entities
{
    public class SearchIndex : BaseEntity
    {
        public Guid MessageId { get; set; }
        public virtual Message Message { get; set; }

        public string NormalizedContent { get; set; }  // Noktalama, büyük/küçük harf temizlenmiş
        public Guid SenderId { get; set; }

        public Guid? ReceiverId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? ChatId { get; set; }

        public DateTime MessageCreatedAt { get; set; }
    }
}
