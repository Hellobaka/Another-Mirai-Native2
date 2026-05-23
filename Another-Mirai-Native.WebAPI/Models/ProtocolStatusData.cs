using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("当前协议连接状态")]
    public class ProtocolStatusData
    {
        [Description("协议名称")]
        public string Name { get; set; } = string.Empty;

        [Description("是否已连接")]
        public bool IsConnected { get; set; }
    }
}
