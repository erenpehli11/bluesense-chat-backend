using AutoMapper;
using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.RepositoryInterfaces;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using BluesenseChat.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MessageService> _logger;
        private readonly IMapper _mapper;
        private readonly IMongoMessageRepository _mongo;
        private readonly IRedisCacheService _cache;


        public MessageService(IUnitOfWork unitOfWork, ILogger<MessageService> logger, IMapper mapper, IMongoMessageRepository mongoMessageRepository, IRedisCacheService cache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _mongo = mongoMessageRepository;
            _cache = cache;
        }

        public async Task<ApiResponse<Message>> SendMessageAsync(SendMessageDto dto, Guid senderId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Content) && (dto.Attachments == null || !dto.Attachments.Any()))
                    return ApiResponse<Message>.Fail("Mesaj içeriği veya dosya eklenmelidir.");

                var sender = await _unitOfWork.Users.GetByIdAsync(senderId);
                if (sender == null) return ApiResponse<Message>.Fail("Gönderen kullanıcı bulunamadı.");

                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    Content = dto.Content,
                    Attachments = new List<Attachment>(),
                    CreatedAt = DateTime.UtcNow
                };

                // Grup mesajı
                if (dto.GroupId.HasValue)
                {
                    var group = await _unitOfWork.Groups.GetByIdAsync(dto.GroupId.Value);
                    if (group == null) return ApiResponse<Message>.Fail("Grup bulunamadı.");

                    var isMember = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m =>
                        m.GroupId == dto.GroupId.Value && m.UserId == senderId && m.InvitationStatus == "Accepted");

                    if (isMember == null)
                        return ApiResponse<Message>.Fail("Bu gruba mesaj gönderme yetkiniz yok.");

                    message.GroupId = dto.GroupId;
                }

                // Private mesaj
                if (dto.PrivateChatId.HasValue)
                {
                    var chat = await _unitOfWork.PrivateChats.GetByIdAsync(dto.PrivateChatId.Value);
                    if (chat == null) return ApiResponse<Message>.Fail("Özel sohbet bulunamadı.");

                    if (chat.User1Id != senderId && chat.User2Id != senderId)
                        return ApiResponse<Message>.Fail("Bu özel sohbete mesaj gönderme yetkiniz yok.");

                    message.PrivateChatId = dto.PrivateChatId;
                    message.ReceiverId = chat.User1Id == senderId ? chat.User2Id : chat.User1Id;
                }
                else if (dto.ReceiverId.HasValue)
                {
                    var receiver = await _unitOfWork.Users.GetByIdAsync(dto.ReceiverId.Value);
                    if (receiver == null) return ApiResponse<Message>.Fail("Alıcı kullanıcı bulunamadı.");

                    // PrivateChat varsa bul, yoksa oluştur
                    var chat = await _unitOfWork.PrivateChats.FirstOrDefaultAsync(pc =>
                        (pc.User1Id == senderId && pc.User2Id == dto.ReceiverId) ||
                        (pc.User1Id == dto.ReceiverId && pc.User2Id == senderId));

                    if (chat == null)
                    {
                        chat = new PrivateChat
                        {
                            Id = Guid.NewGuid(),
                            User1Id = senderId,
                            User2Id = dto.ReceiverId.Value,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.PrivateChats.AddAsync(chat);
                        await _unitOfWork.SaveChangesAsync();
                    }

                    message.PrivateChatId = chat.Id;
                    message.ReceiverId = dto.ReceiverId;
                }

                // Dosya ekleri varsa yükle
                if (dto.Attachments != null && dto.Attachments.Any())
                {
                    var uploadFolder = Path.Combine("wwwroot", "uploads", "messages", message.Id.ToString());
                    Directory.CreateDirectory(uploadFolder);

                    foreach (var file in dto.Attachments)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var attachment = new Attachment
                        {
                            Id = Guid.NewGuid(),
                            FileName = file.FileName,
                            FileSize = file.Length,
                            FileType = file.ContentType,
                            FileUrl = $"/uploads/messages/{message.Id}/{fileName}",
                            MessageId = message.Id,
                            SenderId = senderId,
                            GroupId = dto.GroupId,
                            PrivateChatId = dto.PrivateChatId,
                            RecieverId = message.ReceiverId,
                            UploadedAt = DateTime.UtcNow
                        };

                        message.Attachments.Add(attachment);
                        await _unitOfWork.Attachments.AddAsync(attachment);
                    }
                }

                await _mongo.AddAsync(message);

                string cacheKey = message.GroupId.HasValue
                    ? $"group:{message.GroupId}:messages"
                    : $"private:{message.PrivateChatId}:messages";

                var cachedMessages = await _cache.GetAsync<List<Message>>(cacheKey) ?? new List<Message>();

                cachedMessages.Insert(0, message); // Yeni mesajı en başa ekle

                // Sadece son 50 mesajı tut
                cachedMessages = cachedMessages.Take(50).ToList();

                await _cache.SetAsync(cacheKey, cachedMessages, TimeSpan.FromMinutes(60));


                return ApiResponse<Message>.Success(message, "Mesaj gönderildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj gönderilirken hata oluştu.");
                return ApiResponse<Message>.Fail("Mesaj gönderilemedi.");
            }
        }

        public async Task<ApiResponse<List<MessageDto>>> GetGroupMessagesAsync(Guid groupId, int page, int pageSize, Guid requesterId)
        {
            try
            {
                var isMember = await _unitOfWork.GroupMembers.FirstOrDefaultAsync(m =>
                    m.GroupId == groupId && m.UserId == requesterId && m.InvitationStatus == "Accepted");

                if (isMember == null)
                    return ApiResponse<List<MessageDto>>.Fail("Bu grubun mesajlarını görmeye yetkiniz yok.");

                string cacheKey = $"group:{groupId}:messages";

                var cached = await _cache.GetAsync<List<Message>>(cacheKey);
                if (cached != null)
                {
                    var paged = cached
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var mapped2 = _mapper.Map<List<MessageDto>>(paged);
                    return ApiResponse<List<MessageDto>>.Success(mapped2);
                }

                var messages = await _mongo.GetGroupMessages(groupId, page, pageSize);


                var mapped = _mapper.Map<List<MessageDto>>(messages);
                return ApiResponse<List<MessageDto>>.Success(mapped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grup mesajları alınamadı.");
                return ApiResponse<List<MessageDto>>.Fail("Mesajlar alınırken hata oluştu.");
            }
        }

        public async Task<ApiResponse<List<MessageDto>>> GetPrivateMessagesAsync(Guid privateChatId, Guid requesterId, int page, int pageSize)
        {
            try
            {
                var chat = await _unitOfWork.PrivateChats.GetByIdAsync(privateChatId);
                if (chat == null)
                    return ApiResponse<List<MessageDto>>.Fail("Sohbet bulunamadı.");

                if (chat.User1Id != requesterId && chat.User2Id != requesterId)
                    return ApiResponse<List<MessageDto>>.Fail("Bu özel sohbeti görmeye yetkiniz yok.");

                string cacheKey = $"private:{privateChatId}:messages";

                var cached = await _cache.GetAsync<List<Message>>(cacheKey);
                if (cached != null)
                {
                    var paged = cached
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var mapped2 = _mapper.Map<List<MessageDto>>(paged);
                    return ApiResponse<List<MessageDto>>.Success(mapped2);
                }

                var messages = await _mongo.GetPrivateMessages(privateChatId, page, pageSize);


                var mapped = _mapper.Map<List<MessageDto>>(messages);
                return ApiResponse<List<MessageDto>>.Success(mapped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Özel mesajlar alınamadı.");
                return ApiResponse<List<MessageDto>>.Fail("Mesajlar alınırken hata oluştu.");
            }
        }

        public async Task<ApiResponse<string>> SoftDeleteMessageAsync(Guid messageId, Guid requesterId)
        {
            try
            {
                await _mongo.SoftDeleteAsync(messageId, requesterId);

                var message = await _mongo.GetByIdAsync(messageId);
                if (message == null)
                    return ApiResponse<string>.Fail("Mesaj bulunamadı.");

                // Redis'ten mesajı çıkar
                var groupKey = $"group:{message.GroupId}:messages";
                var privateKey = $"private:{message.PrivateChatId}:messages";

                var key = message.GroupId != null ? groupKey : privateKey;

                var cached = await _cache.GetAsync<List<Message>>(key);
                if (cached != null)
                {
                    var updated = cached.Where(m => m.Id != message.Id).ToList();
                    await _cache.SetAsync(key, updated, TimeSpan.FromMinutes(60));
                }

                return ApiResponse<string>.Success("Mesaj silindi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj silinirken hata oluştu.");
                return ApiResponse<string>.Fail("Mesaj silinemedi.");
            }
        }

    }
}
