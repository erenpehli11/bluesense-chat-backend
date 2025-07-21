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
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var result = await _groupService.CreateGroupAsync(dto, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupDto dto)
        {
            var result = await _groupService.UpdateGroupInfoAsync(dto, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("add-member/{groupId}/{userIdToAdd}")]
        public async Task<IActionResult> AddMember(Guid groupId, Guid userIdToAdd)
        {
            var result = await _groupService.AddMemberAsync(groupId, userIdToAdd, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("remove-member/{groupId}/{userIdToRemove}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, Guid userIdToRemove)
        {
            var result = await _groupService.RemoveMemberAsync(groupId, userIdToRemove, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("mute/{groupId}/{userIdToMute}")]
        public async Task<IActionResult> MuteMember(Guid groupId, Guid userIdToMute)
        {
            var result = await _groupService.MuteMemberAsync(groupId, userIdToMute, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("kick/{groupId}/{userIdToKick}")]
        public async Task<IActionResult> KickMember(Guid groupId, Guid userIdToKick)
        {
            var result = await _groupService.KickMemberAsync(groupId, userIdToKick, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("promote-admin/{groupId}/{userIdToPromote}")]
        public async Task<IActionResult> PromoteToAdmin(Guid groupId, Guid userIdToPromote)
        {
            var result = await _groupService.PromoteToAdminAsync(groupId, userIdToPromote, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("my-groups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var result = await _groupService.GetUserGroupsAsync(GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("members/{groupId}")]
        public async Task<IActionResult> GetGroupMembers(Guid groupId)
        {
            var result = await _groupService.GetGroupMembersAsync(groupId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("invite-link/{groupId}")]
        public async Task<IActionResult> GetInviteLink(Guid groupId)
        {
            var result = await _groupService.GetInviteLinkAsync(groupId, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinGroup([FromBody] JoinGroupRequestDto dto)
        {
            var result = await _groupService.JoinGroupByInviteLinkAsync(dto.InviteLink, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("pending-requests/{groupId}")]
        public async Task<IActionResult> GetPendingRequests(Guid groupId)
        {
            var result = await _groupService.GetPendingJoinRequestsAsync(groupId, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("approve/{groupId}/{userId}")]
        public async Task<IActionResult> ApproveJoinRequest(Guid groupId, Guid userId)
        {
            var result = await _groupService.ApproveJoinRequestAsync(groupId, userId, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reject/{groupId}/{userId}")]
        [Authorize]
        public async Task<IActionResult> RejectJoinRequest(Guid groupId, Guid userId)
        {
            var result = await _groupService.RejectJoinRequestAsync(groupId, userId, GetUserId());
            return Ok(result);
        }
    }
}
