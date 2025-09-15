using Food_Ordering.DTOs.QueryParams;
using Food_Ordering.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Ordering.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMenuCategoryService _menuCategory;

        public CategoryController(IMenuCategoryService menuCategory)
        {
            _menuCategory = menuCategory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MenuCategoryQuery query)
        {
            try
            {
                var result = await _menuCategory.GetAll(query);

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
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = result.Data
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
