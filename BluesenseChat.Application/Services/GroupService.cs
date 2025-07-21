using AutoMapper;
using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.RepositoryInterfaces;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using BluesenseChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BluesenseChat.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupService> _logger;

        public GroupService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GroupService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<Guid>> CreateGroupAsync(CreateGroupDto dto, Guid ownerId)
        {
            try
            {
                var owner = await _unitOfWork.Users.GetByIdAsync(ownerId);

                var group = new Group
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    IsPrivate = dto.IsPrivate,
                    OwnerId = ownerId,
                    Owner = owner,
                    InviteLink = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    Members = new List<GroupMember>()
                };

                var ownerMember = new GroupMember
                {
                    Id = Guid.NewGuid(),
                    GroupId = group.Id,
                    Group = group,
                    UserId = ownerId,
                    User = owner,
                    IsAdmin = true,
                    InvitationStatus = "Accepted"
                };

                group.Members.Add(ownerMember);

                await _unitOfWork.Groups.AddAsync(group);
                await _unitOfWork.GroupMembers.AddAsync(ownerMember);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<Guid>.Success(group.Id, "Grup oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grup oluşturma başarısız");
                return ApiResponse<Guid>.Fail("Grup oluşturulurken bir hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> UpdateGroupInfoAsync(UpdateGroupDto dto, Guid requesterId)
        {
            try
            {
                var group = await _unitOfWork.Groups.GetByIdAsync(dto.GroupId);
                if (group == null)
                    return ApiResponse<string>.Fail("Grup bulunamadı");

                var isAdmin = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == dto.GroupId && m.UserId == requesterId && m.IsAdmin);
                if (isAdmin == null)
                    return ApiResponse<string>.Fail("Sadece admin grup bilgilerini güncelleyebilir");

                _mapper.Map(dto, group);

                _unitOfWork.Groups.Update(group);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Grup bilgileri güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grup bilgisi güncellenemedi");
                return ApiResponse<string>.Fail("Grup bilgileri güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> AddMemberAsync(Guid groupId, Guid userIdToAdd, Guid adminUserId)
        {
            try
            {
                var admin = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == adminUserId && m.IsAdmin);
                if (admin == null)
                    return ApiResponse<string>.Fail("Yalnızca admin üye ekleyebilir");

                var exists = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userIdToAdd);
                if (exists != null)
                    return ApiResponse<string>.Fail("Kullanıcı zaten grupta");

                var member = new GroupMember
                {
                    Id = Guid.NewGuid(),
                    GroupId = groupId,
                    UserId = userIdToAdd,
                    InvitationStatus = "Accepted"
                };

                await _unitOfWork.GroupMembers.AddAsync(member);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Üye eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Üye eklenemedi");
                return ApiResponse<string>.Fail("Üye eklenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> RemoveMemberAsync(Guid groupId, Guid userIdToRemove, Guid adminUserId)
        {
            try
            {
                var admin = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == adminUserId && m.IsAdmin);
                if (admin == null)
                    return ApiResponse<string>.Fail("Yalnızca admin üye silebilir");

                var member = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userIdToRemove);
                if (member == null)
                    return ApiResponse<string>.Fail("Üye bulunamadı");

                _unitOfWork.GroupMembers.SoftDelete(member);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Üye silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Üye silinemedi");
                return ApiResponse<string>.Fail("Üye silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> MuteMemberAsync(Guid groupId, Guid userIdToMute, Guid adminUserId)
        {
            try
            {
                var admin = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == adminUserId && m.IsAdmin);
                if (admin == null)
                    return ApiResponse<string>.Fail("Yalnızca admin susturabilir");

                var member = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userIdToMute);
                if (member == null)
                    return ApiResponse<string>.Fail("Üye bulunamadı");

                member.IsMuted = true;
                _unitOfWork.GroupMembers.Update(member);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Üye susturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Üye susturulamadı");
                return ApiResponse<string>.Fail("Susturma işlemi başarısız oldu");
            }
        }

        public async Task<ApiResponse<string>> KickMemberAsync(Guid groupId, Guid userIdToKick, Guid adminUserId)
            => await RemoveMemberAsync(groupId, userIdToKick, adminUserId);

        public async Task<ApiResponse<string>> PromoteToAdminAsync(Guid groupId, Guid userIdToPromote, Guid adminUserId)
        {
            try
            {
                var admin = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == adminUserId && m.IsAdmin);
                if (admin == null)
                    return ApiResponse<string>.Fail("Yalnızca admin yetki verebilir");

                var member = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userIdToPromote);
                if (member == null)
                    return ApiResponse<string>.Fail("Üye bulunamadı");

                member.IsAdmin = true;
                _unitOfWork.GroupMembers.Update(member);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Üye admin yapıldı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin yapma başarısız");
                return ApiResponse<string>.Fail("Admin yapma işlemi başarısız oldu");
            }
        }

        public async Task<ApiResponse<List<GroupSummaryDto>>> GetUserGroupsAsync(Guid userId)
{
    try
    {
        var memberships = await _unitOfWork.GroupMembers.GetAllAsync();
        var userGroupIds = memberships
            .Where(m => m.UserId == userId && m.InvitationStatus == "Accepted")
            .Select(m => m.GroupId);

                var allGroups = await _unitOfWork.Groups.GetGroupsWithMembersAndUsersAsync();


                var groups = allGroups.Where(g => userGroupIds.Contains(g.Id)).ToList();
        var groupDtos = _mapper.Map<List<GroupSummaryDto>>(groups);

        return ApiResponse<List<GroupSummaryDto>>.Success(groupDtos);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Kullanıcının grupları alınamadı");
        return ApiResponse<List<GroupSummaryDto>>.Fail("Grup listesi alınırken hata oluştu");
    }
}


        public async Task<ApiResponse<List<GroupMember>>> GetGroupMembersAsync(Guid groupId)
        {
            try
            {
                var members = await _unitOfWork.GroupMembers.GetAllAsync();
                var result = members.Where(m => m.GroupId == groupId && m.InvitationStatus == "Accepted").ToList();
                return ApiResponse<List<GroupMember>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grup üyeleri alınamadı");
                return ApiResponse<List<GroupMember>>.Fail("Grup üyeleri alınırken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> GetInviteLinkAsync(Guid groupId, Guid requesterId)
        {
            try
            {
                var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
                if (group == null)
                    return ApiResponse<string>.Fail("Grup bulunamadı");

                var isMember = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == requesterId);
                if (isMember == null)
                    return ApiResponse<string>.Fail("Grup üyesi değilsiniz");

                return ApiResponse<string>.Success(group.InviteLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invite link alınamadı");
                return ApiResponse<string>.Fail("Invite link alınamadı");
            }
        }

        public async Task<ApiResponse<string>> JoinGroupByInviteLinkAsync(string inviteLink, Guid userId)
        {
            try
            {
                var group = await _unitOfWork.Groups.FirstOrDefaultAsync(g => g.InviteLink == inviteLink);
                if (group == null)
                    return ApiResponse<string>.Fail("Geçersiz bağlantı");

                var existing = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == group.Id && m.UserId == userId);
                if (existing != null)
                    return ApiResponse<string>.Fail("Zaten bu grubun üyesisiniz veya başvurunuz var");

                var member = new GroupMember
                {
                    Id = Guid.NewGuid(),
                    GroupId = group.Id,
                    UserId = userId,
                    InvitationStatus = group.IsPrivate ? "Pending" : "Accepted",
                    IsAdmin = false
                };

                await _unitOfWork.GroupMembers.AddAsync(member);
                await _unitOfWork.SaveChangesAsync();

                return group.IsPrivate
                    ? ApiResponse<string>.Success("Grup yöneticisinin onayı bekleniyor.")
                    : ApiResponse<string>.Success("Gruba başarıyla katıldınız.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gruba katılma başarısız");
                return ApiResponse<string>.Fail("Gruba katılma sırasında hata oluştu");
            }
        }

        public async Task<ApiResponse<List<GroupMemberDto>>> GetPendingJoinRequestsAsync(Guid groupId, Guid adminId)
        {
            try
            {
                var isAdmin = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == adminId && m.IsAdmin);
                if (isAdmin == null)
                    return ApiResponse<List<GroupMemberDto>>.Fail("Sadece admin başvuruları görüntüleyebilir");

                var all = await _unitOfWork.GroupMembers.GetAllAsync();
                var pending = all.Where(m => m.GroupId == groupId && m.InvitationStatus == "Pending").ToList();
                var dto = _mapper.Map<List<GroupMemberDto>>(pending);

                return ApiResponse<List<GroupMemberDto>>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bekleyen başvurular alınamadı");
                return ApiResponse<List<GroupMemberDto>>.Fail("Başvurular çekilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> ApproveJoinRequestAsync(Guid groupId, Guid userId, Guid adminId)
        {
            try
            {
                var isAdmin = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == adminId && m.IsAdmin);
                if (isAdmin == null)
                    return ApiResponse<string>.Fail("Sadece admin onaylayabilir");

                var member = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);
                if (member == null || member.InvitationStatus != "Pending")
                    return ApiResponse<string>.Fail("Onaylanacak bekleyen başvuru bulunamadı");

                member.InvitationStatus = "Accepted";
                _unitOfWork.GroupMembers.Update(member);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Kullanıcı gruba kabul edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Başvuru onayı başarısız");
                return ApiResponse<string>.Fail("Başvuru onaylanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> RejectJoinRequestAsync(Guid groupId, Guid userId, Guid adminId)
        {
            try
            {
                // Admin mi kontrolü
                var isAdmin = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m =>
                    m.GroupId == groupId && m.UserId == adminId && m.IsAdmin);

                if (isAdmin == null)
                    return ApiResponse<string>.Fail("Sadece admin kullanıcılar bu işlemi gerçekleştirebilir");

                // Başvuru kontrolü
                var request = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m =>
                    m.GroupId == groupId && m.UserId == userId && m.InvitationStatus == "Pending");

                if (request == null)
                    return ApiResponse<string>.Fail("Bekleyen bir başvuru bulunamadı");

                request.InvitationStatus = "Rejected";

                _unitOfWork.GroupMembers.Update(request);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Kullanıcının başvurusu reddedildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Başvuru reddedilirken hata oluştu");
                return ApiResponse<string>.Fail("Bir hata oluştu, işlem tamamlanamadı");
            }
        }
    }
}
