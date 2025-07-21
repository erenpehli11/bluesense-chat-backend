using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.RepositoryInterfaces;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using BluesenseChat.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtTokenGenerator jwtTokenGenerator,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
                if (user == null)
                    return ApiResponse<TokenResponseDto>.Fail("Email veya şifre hatalı");

                var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
                if (verification != PasswordVerificationResult.Success)
                    return ApiResponse<TokenResponseDto>.Fail("Email veya şifre hatalı");

                var accessToken = _jwtTokenGenerator.GenerateToken(user);
                var refreshToken = GenerateRefreshToken(user.Id);

                await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<TokenResponseDto>.Success(new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpireAt = DateTime.UtcNow.AddHours(2)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login işlemi sırasında hata oluştu.");
                return ApiResponse<TokenResponseDto>.Fail("Bir hata oluştu");
            }
        }

        public async Task<ApiResponse<TokenResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            var tokenInDb = await _unitOfWork.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);

            if (tokenInDb == null || tokenInDb.ExpiresAt <= DateTime.UtcNow)
                return ApiResponse<TokenResponseDto>.Fail("Geçersiz veya süresi dolmuş refresh token.");

            var user = await _unitOfWork.Users.GetByIdAsync(tokenInDb.UserId);
            if (user == null)
                return ApiResponse<TokenResponseDto>.Fail("Kullanıcı bulunamadı.");

            tokenInDb.RevokedAt = DateTime.UtcNow;

            var newAccessToken = _jwtTokenGenerator.GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken(user.Id);

            await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<TokenResponseDto>.Success(new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpireAt = DateTime.UtcNow.AddHours(2)
            });
        }

        private RefreshToken GenerateRefreshToken(Guid userId)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
