using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.DTOs
{
    public class SendMessageDto
    {
        public string? Content { get; set; }

        // Grup mesajıysa dolu olur
        public Guid? GroupId { get; set; }

        public Guid? ReceiverId { get; set; }        // Yeni başlayan özel sohbet için

        // Private mesajsa dolu olur
        public Guid? PrivateChatId { get; set; }

        // Opsiyonel olarak dosya yüklenebilir
        public List<IFormFile>? Attachments { get; set; }
    }

}
