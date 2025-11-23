using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services;
using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using System.Collections.Generic;

namespace FoodOrdering.Presentation.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IVoucherService _voucherService;
        private readonly IAddressService _addressService; 
        private readonly INotificationService _notificationService;
        private readonly IRatingService _ratingService;
        public CommonController(
            ICategoryService categoryService, 
            IMenuService menuService, 
            ICartService cartService, 
            IOrderService orderService, 
            IUserService userService, 
            IVoucherService voucherService, 
            IAddressService addressService, 
            INotificationService notificationService,
            IRatingService ratingService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
            _cartService = cartService;
            _orderService = orderService;
            _userService = userService;
            _voucherService = voucherService;
            _addressService = addressService;
            _notificationService = notificationService;
            _ratingService = ratingService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllAsync();
            var response = ApiResponse<IEnumerable<CategoryDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("menus")]
        public async Task<IActionResult> GetMenus([FromQuery] MenuParams menuParams)
        {
            var result = await _menuService.GetAllMenusAsync(menuParams);
            var response = ApiResponse<PagingReponse<MenuDto>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("menu/{id}")]
        public async Task<IActionResult> GetMenuById(Guid id)
        {
            var result = await _menuService.GetMenuByIdAsync(id);
            var response = ApiResponse<MenuDto>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("menus/featured")]
        public async Task<IActionResult> GetFeaturedMenus()
        {
            var result = await _menuService.GetFeaturedMenusAsync();
            var response = ApiResponse<dynamic>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);

            return Ok(response);
        }

        [HttpGet("menus/{id}/related")]
        public async Task<IActionResult> GetRelatedMenus(Guid id)
        {
            var result = await _menuService.GetRelatedMenusAsync(id);
            var response = ApiResponse<dynamic>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);

            return Ok(response);
        }

        [HttpGet("cart")]
        public async Task<IActionResult> GetCartByCustomer(Guid id)
        {          
            var result = await _cartService.GetCartByCustomer(id);
            var response = ApiResponse<CartDTO>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
        {
            await _cartService.AddToCartAsync(request);
            var response = ApiResponse<string>.Success("Thêm item thành công", "", StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPost("order/qr")]
        public async Task<IActionResult> CreateOrderWithQR([FromBody] OrderRequest request)
        {
            var result = await _orderService.CreateOrderByQRAsync(request);
            var response = ApiResponse<dynamic>.Success("Tạo đơn thành công", result, StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPost("order/cod")]
        public async Task<IActionResult> CreateOrderWithCOD([FromBody] OrderRequest request)
        {
            var result = await _orderService.CreateOrderByCODAsync(request);
            var response = ApiResponse<int>.Success("Tạo đơn thành công", result, StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpGet("user/{id}/orders")]
        public async Task<IActionResult> GetAllOrderByCustomer(Guid id, [FromQuery] OrderParams orderParams)
        {
            var result = await _orderService.GetAllAsyncByCustomer(id, orderParams);
            var response = ApiResponse<PagingReponse<OrderDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPut("user/account/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id,[FromForm] UserRequest request)
        {
            await _userService.UploadProfileAsync(id, request);
            var response = ApiResponse<string>.Success("Cập nhật thành công", "", StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("user/vouchers")]
        public async Task<IActionResult> GetAllVoucherByCustomer()
        {
            var result = await _voucherService.GetAllByCustomerAsync();
            var response = ApiResponse<IEnumerable<VoucherDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("user/voucher/validation")]
        public async Task<IActionResult> ValidateVoucher(ValidateVoucherRequest request)
        {
            var result = await _voucherService.ValidateVoucherAsync(request);
            var response = ApiResponse<dynamic>.Success("Áp dụng voucher thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            var response = ApiResponse<UserDTO>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("user/{id}/addresses")]
        public async Task<IActionResult> GetAddresses(Guid id)
        {
            var result = await _addressService.GetAllByUserAsync(id);
            var response = ApiResponse<IEnumerable<AddressDto>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);

            return Ok(response);           
        }

        [HttpPost("address")]
        public async Task<IActionResult> AddAddress([FromBody] AddressRequest request)
        {
            await _addressService.AddAsync(request);
            var response = ApiResponse<string>.Success("Thêm mới thành công", null, StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPut("address/{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] AddressRequest request)
        {
            await _addressService.UpdateAsync(id, request);
            var response = ApiResponse<string>.Success("Cập nhật thành công", null, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpDelete("address/{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            await _addressService.DeleteAsync(id);
            var response = ApiResponse<string>.Success("Xóa thành công",null, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPut("notification")]
        public async Task<IActionResult> UpdateNotification(List<Guid> ids)
        {
            await _notificationService.MarkAsReadAsync(ids);

            var response = ApiResponse<dynamic>.Success("Cập nhật thành công", null, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("ratings/menu/{id}")]
        public async Task<IActionResult> GetAllRatingsByMenuId(Guid id,[FromQuery] RatingParams ratingParams)
        {   
            
            var result = await _ratingService.GetAllRatingsByMenuAsync(id, ratingParams);

            var response = ApiResponse<dynamic>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("rating")]
        public async Task<IActionResult> RatingPaidOrder([FromForm] RatingRequest request)
        {
             await _ratingService.RatingPaidOrderAsync(request);

            var response = ApiResponse<dynamic>.Success("Đánh giá thành công", null, StatusCodes.Status201Created);
            return Ok(response);
        }
    }
}
