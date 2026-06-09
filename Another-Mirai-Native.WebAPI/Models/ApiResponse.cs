using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("API 统一响应体")]
    public class ApiResponse
    {
        [Description("状态码，0 表示成功")]
        public int Code { get; set; }

        [Description("响应数据")]
        public object? Data { get; set; }

        [Description("错误或提示信息")]
        public string? Message { get; set; }

        public static ApiResponse<T> Ok<T>(T data) => new() { Code = 0, Data = data };
        public static ApiResponse Ok() => new() { Code = 0 };
        public static ApiResponse Error(int code, string message) => new() { Code = code, Message = message };
        public static ApiResponse<T> Error<T>(int code, string message) => new() { Code = code, Message = message };
    }

    [Description("API 统一响应体（带具体数据类型）")]
    public class ApiResponse<T> : ApiResponse
    {
        [Description("响应数据")]
        public new T? Data { get; set; }
    }
}
