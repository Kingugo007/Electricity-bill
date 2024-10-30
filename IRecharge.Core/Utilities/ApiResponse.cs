using System.Net;

namespace IRecharge.Core.Utilities
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public ApiResponse<T> Fail(string errorMessage, int statusCode = (int)HttpStatusCode.NotFound)
        {
            return new ApiResponse<T> { Status = false, Message = errorMessage, StatusCode = statusCode };
        }
        public ApiResponse<T> Success(string successMessage, T data, int statusCode = (int)HttpStatusCode.OK)
        {
            return new ApiResponse<T> { Status = true, Message = successMessage, Data = data, StatusCode = statusCode };
        }
    }
}
