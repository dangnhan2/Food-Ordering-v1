using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpDelete("delete-image")]
        public async Task<IActionResult> DeleteImage(string url)
        {
            try
            {
                var result = await _cloudinaryService.DeleteImage(url);

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
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadThumbnail(IFormFile file)
        {
            try
            {
                var result = await _cloudinaryService.UploadImage(file, "Thumbnail");

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
                    result.Data
                });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
