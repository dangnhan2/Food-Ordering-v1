using Food_Ordering.DTOs.FormQuery;
using Food_Ordering.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace Food_Ordering.Controllers.Admin
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
           _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserQuery query)
        {
            try
            {
                var result = await _userService.GetUsersAsync(query);

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
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _userService.GetUserAsync(id);

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
                    Message = "Lấy dữ liệu thành công",
                    StatusCode = StatusCodes.Status200OK,
                    Datta = result.Data
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
    }
}
