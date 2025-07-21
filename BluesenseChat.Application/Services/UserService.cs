using AutoMapper;
using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.RepositoryInterfaces;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using BluesenseChat.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(
    IUnitOfWork unitOfWork,
    IPasswordHasher<User> passwordHasher,
    ILogger<UserService> logger,
    IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ApiResponse<User?>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                return ApiResponse<User?>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync failed.");
                return ApiResponse<User?>.Fail("Kullanıcı alınamadı.");
            }
        }

        public async Task<ApiResponse<User?>> GetByEmailAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
                return ApiResponse<User?>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByEmailAsync failed.");
                return ApiResponse<User?>.Fail("Kullanıcı alınamadı.");
            }
        }

        public async Task<ApiResponse<User?>> GetByUsernameAsync(string username)
        {
            try
            {
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
                return ApiResponse<User?>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByUsernameAsync failed.");
                return ApiResponse<User?>.Fail("Kullanıcı alınamadı.");
            }
        }

        public async Task<ApiResponse<List<User>>> GetAllAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                var filtered = users.Where(u => !u.IsDeleted).ToList();
                return ApiResponse<List<User>>.Success(filtered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync failed.");
                return ApiResponse<List<User>>.Fail("Kullanıcılar alınamadı.");
            }
        }

        public async Task<ApiResponse<string>> UpdatePasswordAsync(Guid userId, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<string>.Fail("Kullanıcı bulunamadı.");

                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Şifre başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdatePasswordAsync failed.");
                return ApiResponse<string>.Fail("Şifre güncellenemedi.");
            }
        }

        public async Task<ApiResponse<string>> UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<string>.Fail("Kullanıcı bulunamadı.");

                _mapper.Map(dto, user); // 🔁 null olmayan alanlar user'a aktarılır

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Profil güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProfileAsync failed.");
                return ApiResponse<string>.Fail("Profil güncellenemedi.");
            }
        }


        public async Task<ApiResponse<string>> SoftDeleteAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<string>.Fail("Kullanıcı bulunamadı.");

                _unitOfWork.Users.SoftDelete(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<string>.Success("Kullanıcı silindi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SoftDeleteAsync failed.");
                return ApiResponse<string>.Fail("Kullanıcı silinemedi.");
            }
        }

        public async Task<ApiResponse<List<Group>>> GetUserGroupsAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

                if (user == null)
                    return ApiResponse<List<Group>>.Fail("Kullanıcı bulunamadı.");

                var groups = await _unitOfWork.Groups
                    .GetAllAsync();

                var userGroups = groups.Where(g => g.Members.Any(m => m.Id == userId)).ToList();
                return ApiResponse<List<Group>>.Success(userGroups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserGroupsAsync failed.");
                return ApiResponse<List<Group>>.Fail("Kullanıcı grupları alınamadı.");
            }
        }

        public async Task<ApiResponse<List<PrivateChat>>> GetUserPrivateChatsAsync(Guid userId)
        {
            try
            {
                var chats = await _unitOfWork.PrivateChats.GetAllAsync();
                var result = chats.Where(c => c.User1Id == userId || c.User2Id == userId).ToList();
                return ApiResponse<List<PrivateChat>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserPrivateChatsAsync failed.");
                return ApiResponse<List<PrivateChat>>.Fail("Sohbetler alınamadı.");
            }
        }

        public async Task<ApiResponse<Guid>> CreateUserAsync(CreateUserRequestDto request)
        {
            try
            {
                // Aynı kullanıcı var mı kontrolü (varsa uygun response dön)
                var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                    return ApiResponse<Guid>.Fail("Bu e-posta ile kayıtlı bir kullanıcı zaten var.");

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.UserName,
                    Email = request.Email,
                    CreatedAt = DateTime.UtcNow,
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<Guid>.Success(user.Id, "Kullanıcı başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı oluşturulurken bir hata meydana geldi.");
                return ApiResponse<Guid>.Fail("Kullanıcı oluşturulurken bir hata oluştu.");
            }
        }
    }

}
