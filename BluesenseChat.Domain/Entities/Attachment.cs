using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BluesenseChat.Domain.Entities
{
    public class Attachment : BaseEntity
    {
        // Dosyanın bağlı olduğu mesaj

        [BsonSerializer(typeof(StandardGuidSerializer))]
        public Guid MessageId { get; set; }
        public virtual Message Message { get; set; }

        // Eğer mesaj grup içindeyse: bu alan set edilir (grup içi medya erişimi için)

        [BsonSerializer(typeof(NullableStandardGuidSerializer))]
        public Guid? GroupId { get; set; }


        [BsonIgnore]
        [JsonIgnore]
        public virtual Group? Group { get; set; }


        [BsonSerializer(typeof(NullableStandardGuidSerializer))]
        public Guid? SenderId { get; set; }
        public virtual User? User { get; set; }


        [BsonSerializer(typeof(NullableStandardGuidSerializer))]
        public Guid? PrivateChatId { get; set; }

        public virtual PrivateChat? PrivateChat { get; set; }



        [BsonSerializer(typeof(NullableStandardGuidSerializer))]
        public Guid? RecieverId { get; set; }

        // Dosya bilgileri
        public string FileName { get; set; }       // Orijinal dosya adı
        public string FileUrl { get; set; }        // Sunucuda saklanan yol veya cloud URL
        public string FileType { get; set; }       // MIME türü: image/png, application/pdf vb.
        public long FileSize { get; set; }         // Byte cinsinden

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
