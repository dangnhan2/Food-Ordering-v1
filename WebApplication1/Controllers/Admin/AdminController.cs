using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
            await _categoryService.AddAsync(request);
            var response = ApiResponse<dynamic>.Success("Thêm thành công", null, StatusCodes.Status201Created);
            return Ok(response);
        }
        
        [HttpPut("category/{id}")]
        public async Task<IActionResult> UpdateCatetogy(Guid id, CategoryRequest request)
        {
            await _categoryService.UpdateAsync(id, request);
            var response = ApiResponse<dynamic>.Success("Cập nhật thành công", null, StatusCodes.Status200OK);
            return Ok(response);

        }

        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _categoryService.DeleteAsync(id);
            var response = ApiResponse<dynamic>.Success("Xóa thành công", null, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("menu")]
        public async Task<IActionResult> AddMenu([FromBody] MenuRequest request)
        {
            await _menuService.AddAsync(request);
            var response = ApiResponse<dynamic>.Success("Thêm mới thành công", "", StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPut("menu/{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, [FromBody] MenuRequest request)
        {
            await _menuService.UpdateAsync(id, request);
            var response = ApiResponse<dynamic>.Success("Cập nhật thành công", "", StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpDelete("menu/{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            await _menuService.DeleteAsync(id);
            var response = ApiResponse<dynamic>.Success("Xóa thành công", "", StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] OrderParams orderParams)
        {
            var result = await _orderService.GetAllAsync(orderParams);
            var response = ApiResponse<PagingReponse<OrderDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var result = await _userService.GetAllAsync(userParams);
            var response = ApiResponse<PagingReponse<UserDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("vouchers")]
        public async Task<IActionResult> GetVoucher([FromQuery] VoucherParams voucherParams)
        {        
            var result = await _voucherService.GetAllByAdminAsync(voucherParams);
            var response = ApiResponse<PagingReponse<VoucherDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("voucher")]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherRequest request)
        {
            await _voucherService.AddAsync(request);
            var response = ApiResponse<string>.Success("Thêm mới thành công", "", StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPut("voucher/{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] VoucherRequest request)
        {
            await _voucherService.UpdateAsync(id, request);
            var response = ApiResponse<string>.Success("Cập nhật thành công", "", StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpDelete("voucher/{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            await _voucherService.DeleteAsync(id);
            var response = ApiResponse<string>.Success("Xóa thành công", "", StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> DashBoard()
        {
            var result = await _dashBoardService.GetInfoAsync();
            var response = ApiResponse<DashboardOverviewDTO>.Success("Lấy dữ liệu thành công", result,StatusCodes.Status201Created);
            return Ok(response);
        }
    }
}
