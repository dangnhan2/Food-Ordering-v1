using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace FoodOrdering.Presentation.Controllers.Upload
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public UploadController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpDelete("menu/image")]
        public async Task<IActionResult> DeleteImage(string url)
        {
            await _cloudinaryService.DeleteImage(url);
            var response = ApiResponse<dynamic>.Success("Xóa ảnh thành công", null, StatusCodes.Status200OK);
            return Ok(response);

        }

        [HttpPost("menu/image")]
        public async Task<IActionResult> UploadThumbnail(IFormFile file)
        {
            var result = await _cloudinaryService.UploadImage(file, "Thumbnail");
            var response = ApiResponse<string>.Success("Tải ảnh thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }
    }
}
