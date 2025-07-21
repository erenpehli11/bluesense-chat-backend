using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Common
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default
            };
        }

        public static ApiResponse<T> Fail(string message, T data)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Data = data
            };
        }
    }
}
