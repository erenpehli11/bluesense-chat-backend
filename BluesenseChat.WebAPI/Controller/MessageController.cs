using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BluesenseChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


        // 1. Mesaj gönderme
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageDto dto)
        {
            var result = await _messageService.SendMessageAsync(dto, GetUserId());
            return Ok(result);
        }

        // 2. Grup mesajlarını getir
        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetGroupMessages(Guid groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _messageService.GetGroupMessagesAsync(groupId, page, pageSize, GetUserId());
            return Ok(result);
        }

        // 3. Private mesajları getir
        [HttpGet("private/{privateChatId}")]
        public async Task<IActionResult> GetPrivateMessages(Guid privateChatId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _messageService.GetPrivateMessagesAsync(privateChatId, GetUserId(), page, pageSize);
            return Ok(result);
        }

        // 4. Mesaj sil (soft delete)
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var result = await _messageService.SoftDeleteMessageAsync(messageId, GetUserId());
            return Ok(result);
        }

        
    }
}
