using FoodOrdering.Application.DTOs.QueryParams;
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
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public AdminController(ICategoryService categoryService, IMenuService menuService, IOrderService orderService, IUserService userService) { 
           _categoryService = categoryService;
           _menuService = menuService;
           _orderService = orderService;
           _userService = userService;
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

        [HttpPost("menu")]
        public async Task<IActionResult> AddMenu([FromForm] MenuRequest request)
        {
            try
            {
                var result = await _menuService.AddAsync(request);

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
            }catch(FileNotFoundException ex)
            {
                return BadRequest(new
                {
                    ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPut("menu/{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, [FromForm] MenuRequest request)
        {
            try
            {
                var result = await _menuService.UpdateAsync(id, request);

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
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpDelete("menu/{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            try
            {
                var result = await _menuService.DeleteAsync(id);

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
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("order")]
        public async Task<IActionResult> GetOrders([FromQuery] OrderParams orderParams)
        {
            try
            {
                var result = await _orderService.GetAllAsync(orderParams);

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
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            try
            {
                var result = await _userService.GetAllAsync(userParams);

                return Ok(new
                {
                    result.Message,
                    result.Code,
                    result.Data
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
