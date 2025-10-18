using CloudinaryDotNet;
using FoodOrdering.Application.DTOs.Response;
using System.Net;
using System.Text.Json;

namespace FoodOrdering.Presentation.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next) {
           _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex) // lỗi đầu vào
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            }
            catch (KeyNotFoundException ex) // lỗi không tìm thấy dữ liệu
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException ex) // lỗi xác thực/ủy quyền
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.Unauthorized);
            }
            catch (Exception ex) // các lỗi còn lại
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            string userMessage = statusCode switch
            {
                HttpStatusCode.BadRequest => "Yêu cầu không hợp lệ. Vui lòng kiểm tra lại thông tin.",
                HttpStatusCode.NotFound => "Không tìm thấy dữ liệu yêu cầu.",
                HttpStatusCode.Unauthorized => "Bạn không có quyền truy cập. Vui lòng đăng nhập lại.",
                HttpStatusCode.InternalServerError => "Hệ thống đang gặp sự cố. Vui lòng thử lại sau.",
                _ => "Đã có lỗi xảy ra. Vui lòng thử lại."
            };

            var response = new ApiResponse<object>(userMessage, false, (int)statusCode, null);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
           
        }
    }
}
