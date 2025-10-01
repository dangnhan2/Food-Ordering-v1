using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.Services;
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
        private readonly IMenuService _menuService;
        private readonly ICartService _cartService;
        public CommonController(ICategoryService categoryService, IMenuService menuService, ICartService cartService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
            _cartService = cartService;
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

        [HttpGet("menu")]
        public async Task<IActionResult> GetMenus([FromQuery] MenuParams menuParams)
        {
            try
            {
                var result = await _menuService.GetAllAsync(menuParams);

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

        [HttpGet("menu/{id}")]
        public async Task<IActionResult> GetMenuById(Guid id)
        {
            try
            {
                var result = await _menuService.GetByIdAsync(id);

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
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
        {
            try
            {
                var result = await _cartService.AddToCartAsync(request);

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
            catch (Exception ex) {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPut("cart/{id}")]
        public async Task<IActionResult> UpdateToCart(Guid id, [FromBody] CartRequest request)
        {
            try
            {
                var result = await _cartService.UpdateToCartAsync(id, request);

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
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
