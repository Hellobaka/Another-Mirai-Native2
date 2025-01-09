namespace Another_Mirai_Native.Model
{
    public class InvokeResult
    {
        public string Type { get; set; } = "";

        public string GUID { get; set; } = "";

        public bool Success { get; set; }

        public string Message { get; set; }

        public object? Result { get; set; }
    }

    public class InvokeBody
    {
        public string GUID { get; set; } = "";

        public string Function { get; set; } = "";

        public object[] Args { get; set; }
    }
}