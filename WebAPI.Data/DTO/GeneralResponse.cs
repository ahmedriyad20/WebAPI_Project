using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Data.DTO
{
    public class GeneralResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        // Success response
        public static GeneralResponse<T> SuccessResponse(T data, string message = "Operation successful", int statusCode = 200)
        {
            return new GeneralResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }

        // Error response
        public static GeneralResponse<T> ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new GeneralResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }

        // Not found response
        public static GeneralResponse<T> NotFoundResponse(string message = "Resource not found")
        {
            return new GeneralResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = 404
            };
        }
    }
}

