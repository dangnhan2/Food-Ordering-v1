using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Presentation.Controllers.Admin
{
    [Route("api/admin")]
    [ApiController]
    [Authorize (Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IVoucherService _voucherService;
        private readonly IDashboardService _dashBoardService;
        private readonly INotificationService _notificationService;
        public AdminController(
            ICategoryService categoryService, 
            IMenuService menuService, 
            IOrderService orderService, 
            IUserService userService, 
            IVoucherService voucherService, 
            IDashboardService dashboardService, 
            INotificationService notificationService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
            _orderService = orderService;
            _userService = userService;
            _voucherService = voucherService;
            _dashBoardService = dashboardService;
            _notificationService = notificationService;
        }

        // Category EndPoints
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllAsync();
            var response = ApiResponse<IEnumerable<CategoryDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("category")]
        public async Task<IActionResult> CreateCategory(CategoryRequestDto request)
        {
            await _categoryService.AddAsync(request);
            var response = ApiResponse<dynamic>.Success("Thêm thành công", null, StatusCodes.Status201Created);
            return Ok(response);
        }
        
        [HttpPut("category/{id}")]
        public async Task<IActionResult> UpdateCatetogy(Guid id, CategoryRequestDto request)
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

        // Menu EndPoints
        [HttpGet("menus")]
        public async Task<IActionResult> GetMenus([FromQuery] MenuParams menuParams)
        {
            var result = await _menuService.GetAllMenusAsync(menuParams);
            var response = ApiResponse<PagingReponse<MenuDto>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("menu")]
        public async Task<IActionResult> AddMenu([FromForm] MenuRequestDto request)
        {
            await _menuService.AddMenuAsync(request);
            var response = ApiResponse<dynamic>.Success("Thêm mới thành công", "", StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPut("menu/{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, [FromForm] MenuRequestDto request)
        {
            await _menuService.UpdateMenuAsync(id, request);
            var response = ApiResponse<dynamic>.Success("Cập nhật thành công", "", StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpDelete("menu/{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            await _menuService.DeleteMenuAsync(id);
            var response = ApiResponse<dynamic>.Success("Xóa thành công", "", StatusCodes.Status200OK);
            return Ok(response);
        }

        // Order EndPoint
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] OrderParams orderParams)
        {
            var result = await _orderService.GetAllAsync(orderParams);
            var response = ApiResponse<PagingReponse<OrderDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        // User EndPoint
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var result = await _userService.GetAllAsync(userParams);
            var response = ApiResponse<PagingReponse<UserDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPut("user_banning/{id}")]
        public async Task<IActionResult> BanUser(Guid id)
        {
            await _userService.BanUserAsync(id);

            var response = ApiResponse<dynamic>.Success("Cập nhật người dùng thành công", "", StatusCodes.Status200OK);

            return Ok(response);
        }

        [HttpPut("user_unbanning/{id}")]
        public async Task<IActionResult> UnbanUser(Guid id)
        {
            await _userService.UnBanUserAsync(id);
            var response = ApiResponse<dynamic>.Success("Cập nhật người dùng thành công", "", StatusCodes.Status200OK);

            return Ok(response);
        }

        // Voucher EndPoints
        [HttpGet("vouchers")]
        public async Task<IActionResult> GetVoucher([FromQuery] VoucherParams voucherParams)
        {        
            var result = await _voucherService.GetAllByAdminAsync(voucherParams);
            var response = ApiResponse<PagingReponse<VoucherDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("voucher")]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherRequestDto request)
        {
            await _voucherService.AddAsync(request);
            var response = ApiResponse<string>.Success("Thêm mới thành công", "", StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPut("voucher/{id}")]
        public async Task<IActionResult> UpdateVoucher(Guid id, [FromBody] VoucherRequestDto request)
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

        // Dashboard EndPoint
        [HttpGet("dashboard")]
        public async Task<IActionResult> DashBoard()
        {
            var result = await _dashBoardService.GetInfoAsync();
            var response = ApiResponse<DashboardOverviewDTO>.Success("Lấy dữ liệu thành công", result,StatusCodes.Status200OK);
            return Ok(response);
        }

        // Notification Endpoint
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotificationsByAdmin(Guid id)
        {
            var result = await _notificationService.GetNotificationsByAdmin(id);
            var response = ApiResponse<dynamic>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPut("notifications/marking")]
        public async Task<IActionResult> MarkAsRead(MarkNotificationRequestDto dto)
        {
            await _notificationService.MarkAsReadAsync(dto);
            var response = ApiResponse<dynamic>.Success("Đánh dấu thành công", "", StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpDelete("notification")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            await _notificationService.DeleteAsync(id);
            var response = ApiResponse<dynamic>.Success("Xóa thông báo thành công", "", StatusCodes.Status200OK);

            return Ok(response);
        }
    }
}
