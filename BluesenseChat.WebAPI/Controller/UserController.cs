using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BluesenseChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT zorunlu
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Helper: JWT'den kullanıcı ID'si al
        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetUserIdFromToken();
            var result = await _userService.GetByIdAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileDto dto)
        {
            var userId = GetUserIdFromToken();
            var result = await _userService.UpdateProfileAsync(userId, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdateMyPassword([FromBody] string newPassword)
        {
            var userId = GetUserIdFromToken();
            var result = await _userService.UpdatePasswordAsync(userId, newPassword);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete")] // Kendi hesabını silme (soft delete)
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userId = GetUserIdFromToken();
            var result = await _userService.SoftDeleteAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("private-chats")]
        public async Task<IActionResult> GetMyPrivateChats()
        {
            var userId = GetUserIdFromToken();
            var result = await _userService.GetUserPrivateChatsAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var userId = GetUserIdFromToken();
            var result = await _userService.GetUserGroupsAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}