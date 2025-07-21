using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Domain.Entities;




namespace BluesenseChat.Application.Interfaces.ServiceInterfaces
    {
        public interface IGroupService
        {
            // Grup oluştur
            Task<ApiResponse<Guid>> CreateGroupAsync(CreateGroupDto dto, Guid ownerId);

            // Grup bilgilerini güncelle (ad, private/public)
            Task<ApiResponse<string>> UpdateGroupInfoAsync(UpdateGroupDto dto, Guid requesterId);

            // Üye ekle (admin tarafından)
            Task<ApiResponse<string>> AddMemberAsync(Guid groupId, Guid userIdToAdd, Guid adminUserId);

            // Üye sil (admin tarafından)
            Task<ApiResponse<string>> RemoveMemberAsync(Guid groupId, Guid userIdToRemove, Guid adminUserId);

            // Üyeyi sustur (admin tarafından)
            Task<ApiResponse<string>> MuteMemberAsync(Guid groupId, Guid userIdToMute, Guid adminUserId);

            // Üyeyi gruptan çıkar (admin tarafından)
            Task<ApiResponse<string>> KickMemberAsync(Guid groupId, Guid userIdToKick, Guid adminUserId);

            // Üyeyi admin yap (admin tarafından)
            Task<ApiResponse<string>> PromoteToAdminAsync(Guid groupId, Guid userIdToPromote, Guid adminUserId);

            // Belirli bir kullanıcının üye olduğu grupları getir
            Task<ApiResponse<List<GroupSummaryDto>>> GetUserGroupsAsync(Guid userId);

            // Grubun üyelerini getir
            Task<ApiResponse<List<GroupMember>>> GetGroupMembersAsync(Guid groupId);

        Task<ApiResponse<string>> GetInviteLinkAsync(Guid groupId, Guid requesterId);

        // 🟢 2. Davet linki ile gruba katılma (public ise direkt, private ise pending olur)
        Task<ApiResponse<string>> JoinGroupByInviteLinkAsync(string inviteLink, Guid userId);

        // 🕒 3. Admin: Bekleyen başvuruları getirir
        Task<ApiResponse<List<GroupMemberDto>>> GetPendingJoinRequestsAsync(Guid groupId, Guid adminId);

        // ✅ 4. Admin: Bir kullanıcının başvurusunu onaylar
        Task<ApiResponse<string>> ApproveJoinRequestAsync(Guid groupId, Guid userId, Guid adminId);

        // ❌ 5. Admin: Bir kullanıcının başvurusunu reddeder
        Task<ApiResponse<string>> RejectJoinRequestAsync(Guid groupId, Guid userId, Guid adminId);
    }
    }


