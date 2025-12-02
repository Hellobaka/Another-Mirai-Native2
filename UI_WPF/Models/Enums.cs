using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Models
{
    public enum SystemTheme
    {
        [Description("跟随系统")]
        System,
        [Description("浅色")]
        Light,
        [Description("深色")]
        Dark
    }

    public enum WindowMaterial
    {
        [Description("无")]
        None,
        [Description("云母")]
        Mica,
        [Description("亚克力")]
        Acrylic,
        Tabbed
    }

    public enum LagrangeCorePlatform
    {
        Windows = 0,
        MacOs = 1,
        Linux = 2
    }

    public enum ChatType
    {
        QQGroup,

        QQPrivate,

        Fallback
    }

    public enum DetailItemType
    {
        Notice,

        Receive,

        Send
    }

    public enum MessageStatus
    {
        Sending,
        Sent,
        SendFailed
    }
}
