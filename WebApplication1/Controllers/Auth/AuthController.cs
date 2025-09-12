using Food_Ordering.DTOs.Request;
using Food_Ordering.Models;
using Food_Ordering.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace Food_Ordering.Controllers.Auth
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService) { 
          _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                var result = await _accountService.Login(loginRequest, HttpContext);

                if (!result.Status)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                //Response.Cookies.Append("refreshToken", result.Data.Tokens.RefreshToken,
                //new CookieOptions
                //{
                //    Expires = DateTimeOffset.UtcNow.AddDays(7),
                //    HttpOnly = true,
                //    IsEssential = true,
                //    Secure = true,
                //    SameSite = SameSiteMode.None,
                //});

                return Ok(new
                {
                    Message = "Đăng nhập thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
                });
            }catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            try
            {
                var result = await _accountService.Register(registerRequest);

                if (!result.Status)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = StatusCodes.Status200OK
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                //var refresh = Request.Cookies["refreshToken"];

                var result = await _accountService.Refresh(HttpContext);

                if (!result.Status)
                {
                    return BadRequest(new
                    {
                        Message = result.Error, 
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                //Response.Cookies.Append("refreshToken", result.Data.Tokens.RefreshToken,
                //    new CookieOptions
                //    {
                //        Expires = DateTimeOffset.UtcNow.AddDays(7),
                //        HttpOnly = true,
                //        IsEssential = true,
                //        Secure = true,
                //        SameSite = SameSiteMode.None,
                //    });

                return Ok(new
                {
                    Message = "Refresh successfully",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
                });
            }catch(NullReferenceException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }catch(Exception ex)
            {
                return BadRequest(ex.InnerException.Message ?? ex.Message);
            }
        }

        [HttpPost("user/{id}/password")]
        public async Task<IActionResult> ChangePassword(string id, PasswordRequest passwordRequest)
        {
            try
            {
                var result = await _accountService.ChangePassword(id, passwordRequest);

                if (!result.Status)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = StatusCodes.Status200OK,
                });
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await _accountService.Logout(HttpContext);

                if (!result.Status)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = StatusCodes.Status200OK,
                });
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
