using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.DTOs
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }

        public Guid SenderId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        public bool IsEdited { get; set; }

        public List<AttachmentDto> Attachments { get; set; } = new();
    }

}
