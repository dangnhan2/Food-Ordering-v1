using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
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
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IVoucherService _voucherService;
        private readonly IAddressService _addressService; 
        public CommonController(ICategoryService categoryService, IMenuService menuService, ICartService cartService, IOrderService orderService, IUserService userService, IVoucherService voucherService, IAddressService addressService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
            _cartService = cartService;
            _orderService = orderService;
            _userService = userService;
            _voucherService = voucherService;
            _addressService = addressService;
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllAsync();

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpGet("menu")]
        public async Task<IActionResult> GetMenus([FromQuery] MenuParams menuParams)
        {
            var result = await _menuService.GetAllAsync(menuParams);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpGet("menu/{id}")]
        public async Task<IActionResult> GetMenuById(Guid id)
        {
            var result = await _menuService.GetByIdAsync(id);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpGet("cart")]
        public async Task<IActionResult> GetCartByCustomer(Guid id)
        {          
            var result = await _cartService.GetCartByCustomer(id);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });           
        }

        [HttpPost("cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
        {
            var result = await _cartService.AddToCartAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });

        }

        [HttpPut("cart/{id}")]
        public async Task<IActionResult> UpdateToCart(Guid id, [FromBody] CartRequest request)
        {
            var result = await _cartService.UpdateToCartAsync(id, request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }

        [HttpPost("order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var result = await _orderService.CreateOrderAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpGet("order/{id}")]
        public async Task<IActionResult> GetAllOrderByCustomer(Guid id, [FromQuery] OrderParams orderParams)
        {
            var result = await _orderService.GetAllAsyncByCustomer(id, orderParams);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpPut("user/avatar/{id}")]
        public async Task<IActionResult> UploadAvatar(Guid id, IFormFile file)
        {
            var result = await _userService.UploadAvatarAsync(id, file);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }

        [HttpPut("user/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserRequest request)
        {
            var result = await _userService.UploadProfileAsync(id, request);

            return Ok(new
            {
                result.Message,
                result.StatusCode
            });

        }

        [HttpGet("voucher-of-customer")]
        public async Task<IActionResult> GetAllVoucherByCustomer()
        {
            var result = await _voucherService.GetAllByCustomerAsync();

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpPost("validate-voucher")]
        public async Task<IActionResult> ValidateVoucher(ValidateVoucherRequest request)
        {
            var result = await _voucherService.ValidateVoucherAsync(request);
           
            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpGet("address")]
        public async Task<IActionResult> GetAddresses(Guid id)
        {
            var result = await _addressService.GetAllByUserAsync(id);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpPost("address")]
        public async Task<IActionResult> AddAddress([FromBody] AddressRequest request)
        {
            var result = await _addressService.AddAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }

        [HttpPut("address/{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] AddressRequest request)
        {
            var result = await _addressService.UpdateAsync(id, request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }

        [HttpDelete("address/{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            var result = await _addressService.DeleteAsync(id);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }

    }
}
