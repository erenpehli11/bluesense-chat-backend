using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Interfaces.ServiceInterfaces
{
    public interface IUserService
    {

        
        // Kullanıcıyı ID ile getir
        Task<ApiResponse<User?>> GetByIdAsync(Guid id);

        // E-mail ile kullanıcıyı getir
        Task<ApiResponse<User?>> GetByEmailAsync(string email);

        // Kullanıcı adı ile kullanıcıyı getir
        Task<ApiResponse<User?>> GetByUsernameAsync(string username);

        // Tüm kullanıcıları getir (opsiyonel)
        Task<ApiResponse<List<User?>>> GetAllAsync();




        // Parola hash'ini güncelle (şifre resetleme vs.)
        Task<ApiResponse<string>> UpdatePasswordAsync(Guid userId, string newPassword);

        // Profil bilgilerini güncelle (Name, Surname, Username, Email vs.)
        Task<ApiResponse<string>> UpdateProfileAsync(Guid userId, UpdateUserProfileDto updateDto);

        // Soft delete (kullanıcı silme)
        Task<ApiResponse<string>> SoftDeleteAsync(Guid userId);

        // Kullanıcının grup üyeliklerini getir
        Task<ApiResponse<List<Group>>> GetUserGroupsAsync(Guid userId);

        // Kullanıcının birebir sohbetlerini getir
        Task<ApiResponse<List<PrivateChat>>> GetUserPrivateChatsAsync(Guid userId);

         

        Task<ApiResponse<Guid>> CreateUserAsync(CreateUserRequestDto request);
    }
}
