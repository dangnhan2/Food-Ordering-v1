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
            var result = await _categoryService.AddAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }
        
        [HttpPut("category/{id}")]
        public async Task<IActionResult> UpdateCatetogy(Guid id, CategoryRequest request)
        {
            var result = await _categoryService.UpdateAsync(id, request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });

        }

        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);
  
            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }

        [HttpPost("menu")]
        public async Task<IActionResult> AddMenu([FromBody] MenuRequest request)
        {
            var result = await _menuService.AddAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode
            });
        }

        [HttpPut("menu/{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, [FromBody] MenuRequest request)
        {
            var result = await _menuService.UpdateAsync(id, request);

            return Ok(new
            {
                result.Message,
                result.StatusCode
            });
        }

        [HttpDelete("menu/{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            var result = await _menuService.DeleteAsync(id);

            return Ok(new
            {
                result.Message,
                result.StatusCode
            });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] OrderParams orderParams)
        {
            var result = await _orderService.GetAllAsync(orderParams);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var result = await _userService.GetAllAsync(userParams);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpGet("vouchers")]
        public async Task<IActionResult> GetVoucher([FromQuery] VoucherParams voucherParams)
        {        
            var result = await _voucherService.GetAllByAdminAsync(voucherParams);
            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpPost("voucher")]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherRequest request)
        {
            var result = await _voucherService.AddAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }

        [HttpPut("voucher/{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] VoucherRequest request)
        {
            var result = await _voucherService.UpdateAsync(id, request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }

        [HttpDelete("voucher/{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            var result = await _voucherService.DeleteAsync(id);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
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
