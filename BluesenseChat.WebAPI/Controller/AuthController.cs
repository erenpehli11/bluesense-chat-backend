using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BluesenseChat.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IAuthService authService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequestDto request)
        {
            try
            {
                var response = await _userService.CreateUserAsync(request);
                var userId = response.Data;
                var message = response.Message ?? "Kullanıcı başarıyla oluşturuldu.";
                return Ok(ApiResponse<Guid>.Success(userId, message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register işlemi sırasında hata oluştu.");
                return BadRequest(ApiResponse<string>.Fail("Kayıt sırasında bir hata oluştu."));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var tokenResult = await _authService.LoginAsync(request);
                if (tokenResult.IsSuccess)
                    return Ok(tokenResult);

                return Unauthorized(ApiResponse<string>.Fail(tokenResult.Message ?? "Giriş başarısız."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login işlemi sırasında hata oluştu.");
                return BadRequest(ApiResponse<string>.Fail("Giriş sırasında bir hata oluştu."));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}