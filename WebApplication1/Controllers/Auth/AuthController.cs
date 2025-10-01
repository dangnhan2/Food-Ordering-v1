using FoodOrdering.Application.Auth;
using FoodOrdering.Application.DTOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Presentation.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request, HttpContext);

                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.Code
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.Code,
                    result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);

                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.Code
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.Code
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(HttpContext);

                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.Code
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.Code,
                    result.Data
                });
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await _authService.LogoutAsync(HttpContext);
                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.Code,
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.Code,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCodes.Status400BadRequest
                });
            }
            finally
            {
                 BadRequest(new
                {
                    Message = "Đăng kí không thành công",
                    Code = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
