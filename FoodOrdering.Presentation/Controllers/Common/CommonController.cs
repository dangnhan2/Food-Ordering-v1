using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services;
using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using System.Collections.Generic;

namespace FoodOrdering.Presentation.Controllers.Common
{
    [Route("api/common")]
    [ApiController]
    [Authorize (Roles = ("Admin, Customer"))]
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
        private readonly ISearchService _searchService;
        private readonly IAdvertisementService _advertisementService;
        public CommonController(
            ICategoryService categoryService, 
            IMenuService menuService, 
            ICartService cartService, 
            IOrderService orderService, 
            IUserService userService, 
            IVoucherService voucherService, 
            IAddressService addressService, 
            INotificationService notificationService,
            IRatingService ratingService,
            ISearchService searchService,
            IAdvertisementService advertisementService)
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
            _searchService = searchService;
            _advertisementService = advertisementService;
        }

        // Category EndPoint
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllAsync();
            var response = ApiResponse<IEnumerable<CategoryDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        // Menu EndPoints
        [HttpGet("menus")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMenus([FromQuery] MenuParams menuParams)
        {
            var result = await _menuService.GetAllMenusAsync(menuParams);
            var response = ApiResponse<PagingReponse<MenuDto>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("menu/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMenuById(Guid id)
        {
            var result = await _menuService.GetMenuByIdAsync(id);
            var response = ApiResponse<MenuDto>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("menus/featured")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeaturedMenus()
        {
            var result = await _menuService.GetFeaturedMenusAsync();
            var response = ApiResponse<dynamic>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);

            return Ok(response);
        }

        [HttpGet("menus/{id}/related")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRelatedMenus(Guid id)
        {
            var result = await _menuService.GetRelatedMenusAsync(id);
            var response = ApiResponse<dynamic>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);

            return Ok(response);
        }

        [HttpGet("menus/onsale")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMenusOnSale([FromQuery] MenuParams menuParams)
        {
            var result = await _menuService.GetAllMenusOnSaleAsync(menuParams);
            var response = ApiResponse<PagingReponse<MenuDto>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("advertisements")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdvertisement()
        {
            var result = await _advertisementService.GetAdvertisementsAsync();
            var response = ApiResponse<dynamic>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);

            return Ok(response);
        }

        // Cart EndPoints
        [HttpGet("cart")]
        public async Task<IActionResult> GetCartByCustomer(Guid id)
        {          
            var result = await _cartService.GetCartByCustomer(id);
            var response = ApiResponse<CartDTO>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequestDto request)
        {
            await _cartService.AddToCartAsync(request);
            var response = ApiResponse<string>.Success("Thêm item thành công", "", StatusCodes.Status201Created);
            return Ok(response);
        }

        // Order EndPoints
        [HttpPost("order/qr")]
        public async Task<IActionResult> CreateOrderWithQR([FromBody] OrderRequestDto request)
        {
            var result = await _orderService.CreateOrderByQRAsync(request);
            var response = ApiResponse<dynamic>.Success("Tạo đơn thành công", result, StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPost("order/cod")]
        public async Task<IActionResult> CreateOrderWithCOD([FromBody] OrderRequestDto request)
        {
            var result = await _orderService.CreateOrderByCODAsync(request);
            var response = ApiResponse<int>.Success("Tạo đơn thành công", result, StatusCodes.Status201Created);
            return Ok(response);
        }

        // User's Order EndPoint
        [AllowAnonymous]
        [HttpGet("user/{id}/orders")]
        public async Task<IActionResult> GetAllOrderByCustomer(Guid id, [FromQuery] OrderParams orderParams)
        {
            var result = await _orderService.GetAllAsyncByCustomer(id, orderParams);
            var response = ApiResponse<PagingReponse<OrderDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        // User's Account EndPoint
        [HttpPut("user/profile/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id,[FromForm] UserRequestDto request)
        {
            await _userService.UploadProfileAsync(id, request);
            var response = ApiResponse<string>.Success("Cập nhật thành công", "", StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            var response = ApiResponse<UserDTO>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        // User's Voucher Endpoints
        [HttpGet("user/vouchers")]
        public async Task<IActionResult> GetAllVoucherByCustomer()
        {
            var result = await _voucherService.GetAllByCustomerAsync();
            var response = ApiResponse<IEnumerable<VoucherDTO>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("user/voucher/validation")]
        public async Task<IActionResult> ValidateVoucher(ValidateVoucherRequestDto request)
        {
            var result = await _voucherService.ValidateVoucherAsync(request);
            var response = ApiResponse<dynamic>.Success("Áp dụng voucher thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }


        // User's Address EndPoints
        [HttpGet("user/{id}/addresses")]
        public async Task<IActionResult> GetAddresses(Guid id)
        {
            var result = await _addressService.GetAllByUserAsync(id);
            var response = ApiResponse<IEnumerable<AddressDto>>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);

            return Ok(response);           
        }

        [HttpPost("address")]
        public async Task<IActionResult> AddAddress([FromBody] AddressRequestDto request)
        {
            await _addressService.AddAsync(request);
            var response = ApiResponse<string>.Success("Thêm mới thành công", null, StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPut("address/{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] AddressRequestDto request)
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

        // Rating Endpoints
        [AllowAnonymous]
        [HttpGet("ratings/menu/{id}")]
        public async Task<IActionResult> GetAllRatingsByMenuId(Guid id,[FromQuery] RatingParams ratingParams)
        {   
            
            var result = await _ratingService.GetAllRatingsByMenuAsync(id, ratingParams);

            var response = ApiResponse<dynamic>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("rating")]
        public async Task<IActionResult> RatingPaidOrder([FromForm] RatingRequestDto request)
        {
             await _ratingService.RatingPaidOrderAsync(request);

            var response = ApiResponse<dynamic>.Success("Đánh giá thành công", null, StatusCodes.Status201Created);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("searching")]
        public async Task<IActionResult> SearchingMenu([FromQuery] SearchRequestDto searchRequest)
        {
            var result = await _searchService.SearchMenuAsync(searchRequest);

            var response = ApiResponse<dynamic>.Success("Lấy dữ liệu thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }
    }
}
