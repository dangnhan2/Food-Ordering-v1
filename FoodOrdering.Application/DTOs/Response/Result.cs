using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class Result<T>
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public T Data { get; set; }

        public Result(string message, bool success, int code, T data) {
            Message = message;
            IsSuccess = success;
            Code = code;
            Data = data;
        }

        public static Result<T> Success(string message, T data, int code) {
            return new Result<T>(message, true, code, data);
        }

        public static Result<T> Fail(string message, int code) {
            return new Result<T>(message, false, code, default(T));
        } 
    }
}
