namespace Another_Mirai_Native.WebAPI.Models
{
    public class ApiResponse
    {
        public int Code { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }

        public static ApiResponse Ok(object? data = null) => new() { Code = 0, Data = data };

        public static ApiResponse Error(int code, string message) => new() { Code = code, Message = message };
    }
}
