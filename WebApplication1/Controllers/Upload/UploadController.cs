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

                return Ok(new
                {
                    result.Message,
                    result.StatusCode
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    ex.Message,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadThumbnail(IFormFile file)
        {
            try
            {
                var result = await _cloudinaryService.UploadImage(file, "Thumbnail");

                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                    result.Data
                });
            }catch(FileNotFoundException ex)
            {
                return NotFound(new
                {
                    ex.Message,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            
        }
    }
}
