using Food_Ordering.DTOs.QueryParams;
using Food_Ordering.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace Food_Ordering.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IDishService _menuItemService;

        public MenuController(IDishService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] MenuItemQuery query)
        {
            try
            {
                var result = await _menuItemService.GetAll(query);

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
            }
            catch (Exception ex) {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
