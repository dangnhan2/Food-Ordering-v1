using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DotNetEnv;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class CLoudinaryService : ICloudinaryService
    {
        private readonly string[] allowedExtensions = { ".jpeg", ".gif", ".png", ".jpg" };
        private readonly Cloudinary _cloudinary;

        public CLoudinaryService()
        {
            Env.Load();
            Account account = new Account
            {
                Cloud = Env.GetString("CLOUD_NAME"),
                ApiKey = Env.GetString("API_KEY"),
                ApiSecret = Env.GetString("API_SECRET")
            };
            _cloudinary = new Cloudinary(account);
        }
        public async Task DeleteImage(string url)
        {
            var publicId = ExtractPublicIdFromUrl(url);
            if (publicId == null)
            {
                throw new InvalidOperationException($"Lỗi khi trích xuất publicId từ URL: {url}");
            }
            if (!string.IsNullOrEmpty(publicId))
            {
                var deleteParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deleteParams);
            }
        }

        public async Task<Result<string>> UploadImage(IFormFile file, string folder)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(fileExtension))
            {
                return Result<string>.Fail($"Hãy upload các file có đuôi {string.Join(" ,", allowedExtensions)}", StatusCodes.Status400BadRequest);
            }

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            Console.WriteLine(result);
            return Result<string>.Success("Up ảnh thành công", result.SecureUrl.ToString(), StatusCodes.Status200OK);

        }

        public string ExtractPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath; // /dvdv4id16/image/upload/v1749660746/pho_hk86qj.jpg

            // Tách phần sau "upload/"
            var parts = path.Split("/upload/");
            if (parts.Length < 2)
                throw new ArgumentException("File không hợp lệ");

            // Lấy phần sau upload/, loại bỏ version
            var pathAfterUpload = parts[1]; // v1749660746/pho_hk86qj.jpg
            var segments = pathAfterUpload.Split('/').ToList();

            if (segments[0].StartsWith("v") && segments[0].Length > 1)
            {
                segments.RemoveAt(0); // bỏ "v1749660746"
            }

            var fullPath = string.Join("/", segments); // "pho_hk86qj.jpg" hoặc "folder/abc.jpg"
            var publicId = Path.ChangeExtension(fullPath, null); // remove .jpg

            return publicId;
        }
    }
}
