using FoodOrdering.Application.DTOs.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface ICloudinaryService
    {
        public Task<ApiResponse<string>> DeleteImage(string url);
        public Task<ApiResponse<string>> UploadImage(IFormFile file, string folder);
    }
}
