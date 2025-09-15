using Food_Ordering.DTOs.Request;
using Food_Ordering.Services;
using Microsoft.AspNetCore.Mvc;

namespace Food_Ordering.Controllers.Admin
{
    [Route("api/v1/admin/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMenuCategoryService _menuCategory;

        public CategoryController(IMenuCategoryService menuCategory)
        {
            _menuCategory = menuCategory;
        }   

        [HttpPut("{id}")]       
        public async Task<IActionResult> Update(Guid id, [FromBody] MenuCategoryRequest request)
        {
            try
            {
                var result = await _menuCategory.Update(id, request);

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
                    StatusCode = StatusCodes.Status400BadRequest
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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] MenuCategoryRequest request)
        {
            try
            {
                var result = await _menuCategory.Add(request);

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
                    StatusCode = StatusCodes.Status400BadRequest
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
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _menuCategory.Delete(id);

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
                    StatusCode = StatusCodes.Status400BadRequest
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
