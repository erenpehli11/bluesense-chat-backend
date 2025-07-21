using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.DTOs
{
    public class SearchResultDto
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; }
        public Guid SenderId { get; set; }
        public Guid? ReceiverId { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
