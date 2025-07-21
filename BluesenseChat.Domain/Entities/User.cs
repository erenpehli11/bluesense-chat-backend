using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BluesenseChat.Domain.Entities
{
    public class User : BaseEntity
    {

        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }




        [JsonIgnore]
        public virtual ICollection<Group> OwnedGroups { get; set; }


        [JsonIgnore]
        public virtual ICollection<GroupMember> GroupMemberships { get; set; }

        public virtual ICollection<PrivateChat> PrivateChats { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }

        public virtual ICollection<Message> ReceivedMessages { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
