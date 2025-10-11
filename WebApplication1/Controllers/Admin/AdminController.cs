using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio.Http;

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
        private readonly IVoucherService _voucherService;
        private readonly IDashboardService _dashBoardService;
        public AdminController(ICategoryService categoryService, IMenuService menuService, IOrderService orderService, IUserService userService, IVoucherService voucherService, IDashboardService dashboardService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
            _orderService = orderService;
            _userService = userService;
            _voucherService = voucherService;
            _dashBoardService = dashboardService;
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
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
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
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
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
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
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

        [HttpPost("menu")]
        public async Task<IActionResult> AddMenu([FromBody] MenuRequest request)
        {
            try
            {
                var result = await _menuService.AddAsync(request);

                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.StatusCode
                });
            }
            catch (FileNotFoundException ex)
            {
                return BadRequest(new
                {
                    ex.Message,
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

        [HttpPut("menu/{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, [FromBody] MenuRequest request)
        {
            try
            {
                var result = await _menuService.UpdateAsync(id, request);

                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.StatusCode
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
                        result.StatusCode
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.StatusCode
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
                    result.StatusCode,
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
                    result.StatusCode,
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

        [HttpGet("voucher")]
        public async Task<IActionResult> GetVoucher([FromQuery] VoucherParams voucherParams)
        {
            try
            {
                var result = await _voucherService.GetAllByAdminAsync(voucherParams);

                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.StatusCode,
                    });
                }

                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                    result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("voucher")]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherRequest request)
        {
            try
            {
                var result = await _voucherService.AddAsync(request);

                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.StatusCode,
                    });
                }
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPut("voucher/{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] VoucherRequest request)
        {
            try
            {
                var result = await _voucherService.UpdateAsync(id, request);

                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.StatusCode,
                    });
                }
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpDelete("voucher/{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            try
            {
                var result = await _voucherService.DeleteAsync(id);

                if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        result.Message,
                        result.StatusCode,
                    });
                }
                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> DashBoard()
        {
            var result = await _dashBoardService.GetInfoAsync();

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }
    }
}
