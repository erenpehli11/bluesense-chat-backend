using BluesenseChat.Application.Common;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluesenseChat.Application.DTOs;

namespace BluesenseChat.Application.Interfaces.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<TokenResponseDto>> RefreshTokenAsync(string refreshToken);

    }
}
