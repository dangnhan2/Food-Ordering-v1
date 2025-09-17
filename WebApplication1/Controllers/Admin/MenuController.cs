using Food_Ordering.DTOs.Request;
using Food_Ordering.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Ordering.Controllers.Admin
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMenu([FromForm] MenuItemRequest request)
        {
            try
            {
                var result = await _menuItemService.Add(request);

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
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id,[FromForm] MenuItemRequest request)
        {
            try
            {
                var result = await _menuItemService.Update(id, request);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            try
            {
                var result = await _menuItemService.Delete(id);

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
    }
}
