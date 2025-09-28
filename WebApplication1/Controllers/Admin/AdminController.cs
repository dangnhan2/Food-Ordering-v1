using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Presentation.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public AdminController(ICategoryService categoryService) { 
           _categoryService = categoryService;
        }

        [HttpPost("category")]
        public async Task<IActionResult> CreateCategory(CategoryRequest request)
        {
            try
            {
                var result = await _categoryService.AddAsync(request);

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
                });
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                     Message = ex.InnerException.Message ?? ex.Message,
                     StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPut("category/{id}")]
        public async Task<IActionResult> UpdateCatetogy(Guid id, CategoryRequest request)
        {
            try
            {
                var result = await _categoryService.UpdateAsync(id, request);

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

        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteAsync(id);

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
                });
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
