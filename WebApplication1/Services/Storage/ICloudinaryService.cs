using Food_Ordering.DTOs.Response;

namespace Food_Ordering.Services.Storage
{
    public interface ICloudinaryService
    {
        public Task<Response<string>> UploadImage(IFormFile file, string folder);
        public Task DeleteImage(string url);
    }
}
