using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class ApiResponse<T>
    {
        public dynamic Message { get; set; }
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }

        public ApiResponse(dynamic message, bool success, int code, T data) {
            Message = message;
            IsSuccess = success;
            StatusCode = code;
            Data = data;
        }

        public static ApiResponse<T> Success(dynamic message, T data, int code) {
            return new ApiResponse<T>(message, true, code, data);
        }

        public static ApiResponse<T> Fail(dynamic message, int code) {
            return new ApiResponse<T>(message, false, code, default(T));
        } 
    }
}
