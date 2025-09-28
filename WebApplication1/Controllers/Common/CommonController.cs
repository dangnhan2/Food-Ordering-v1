using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Presentation.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CommonController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var result = await _categoryService.GetAllAsync();

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
                    StatusCodes = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
