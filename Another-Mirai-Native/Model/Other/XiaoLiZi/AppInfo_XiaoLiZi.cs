using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native.Model.Other.XiaoLiZi
{
    public class AppInfo_XiaoLiZi
    {
        public AppInfo ConvertToBase()
        {
            var info = new AppInfo()
            {
                apiver = 9,
                name = this.appname,
                author = this.author,
                description = this.describe,
                version = this.appv,

                auth = [],
                menu = [],
                _event = [],
                status = []
            };
            List<int> auth = [];
            List<AppInfo.Event> _event = [];

            if (data == null)
            {
                if (!string.IsNullOrEmpty(插件菜单处理函数))
                {
                    info.menu = [new AppInfo.Menu
                    {
                        function = 插件菜单处理函数,
                        name = "控制台",
                    }];
                }
                if (!string.IsNullOrEmpty(被启用处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 1,
                        type = 1003,
                        name = "应用启用事件",
                        function = 被启用处理函数,
                        priority = 30000,
                    });
                }
                if (!string.IsNullOrEmpty(被禁用处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 2,
                        type = 1004,
                        name = "应用停用事件",
                        function = 被禁用处理函数,
                        priority = 30000,
                    });
                }
                if (!string.IsNullOrEmpty(将被卸载处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 3,
                        type = 1002,
                        name = "酷 Q 退出事件",
                        function = 将被卸载处理函数,
                        priority = 30000,
                    });
                }
                if (!string.IsNullOrEmpty(私聊消息处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 4,
                        type = 21,
                        name = "私聊消息",
                        function = 私聊消息处理函数,
                        priority = 30000,
                    });
                }
                if (!string.IsNullOrEmpty(群聊消息处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 5,
                        type = 2,
                        name = "群聊消息",
                        function = 群聊消息处理函数,
                        priority = 30000,
                    });
                }
                if (!string.IsNullOrEmpty(事件消息处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 6,
                        type = 114,
                        name = "事件处理",
                        function = 事件消息处理函数,
                        priority = 30000,
                    });
                }
                if (!string.IsNullOrEmpty(滑块识别处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 7,
                        type = 115,
                        name = "滑块识别处理",
                        function = 滑块识别处理函数,
                        priority = 30000,
                    });
                }
                if (!string.IsNullOrEmpty(短信接码处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 8,
                        type = 116,
                        name = "短信接码",
                        function = 短信接码处理函数,
                        priority = 30000,
                    });
                }
                if (!string.IsNullOrEmpty(频道推送统一处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 9,
                        type = 117,
                        name = "频道推送",
                        function = 频道推送统一处理函数,
                        priority = 30000,
                    });
                }
                if (!string.IsNullOrEmpty(插件消息输出替换处理函数))
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 10,
                        type = 118,
                        name = "插件消息输出",
                        function = 插件消息输出替换处理函数,
                        priority = 30000,
                    });
                }
            }
            else
            {
                if (setproaddres > 0)
                {
                    info.menu = [new AppInfo.Menu
                    {
                        address = setproaddres,
                        name = "控制台",
                    }];
                }

                if (groupmsaddres > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 1,
                        type = 2,
                        name = "群聊消息",
                        address = groupmsaddres,
                        priority = 30000,
                    });
                }
                if (friendmsaddres > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 2,
                        type = 21,
                        name = "私聊消息",
                        address = friendmsaddres,
                        priority = 30000,
                    });
                }
                if (eventmsaddres > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 3,
                        type = 114,
                        name = "事件处理",
                        address = eventmsaddres,
                        priority = 30000,
                    });
                }
                if (unitproaddres > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 4,
                        type = 1002,
                        name = "酷 Q 退出事件",
                        address = unitproaddres,
                        priority = 30000,
                    });
                }
                if (banproaddres > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 5,
                        type = 1004,
                        name = "应用停用事件",
                        address = banproaddres,
                        priority = 30000,
                    });
                }
                if (useproaddres > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 6,
                        type = 1003,
                        name = "应用启用事件",
                        address = useproaddres,
                        priority = 30000,
                    });
                }
                if (SMSVerification > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 7,
                        type = 116,
                        name = "短信接码",
                        address = SMSVerification,
                        priority = 30000,
                    });
                }
                if (SliderR > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 8,
                        type = 115,
                        name = "滑块识别处理",
                        address = SliderR,
                        priority = 30000,
                    });
                }
                if (getticketaddres > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 9,
                        type = 119,
                        name = "GetTicket",
                        address = getticketaddres,
                        priority = 30000,
                    });
                }
                if (getvefcodeaddres > 0)
                {
                    _event.Add(new AppInfo.Event
                    {
                        id = 10,
                        type = 120,
                        name = "GetVefCode",
                        address = getvefcodeaddres,
                        priority = 30000,
                    });
                }
            }
            info.auth = [.. auth];
            info._event = [.. _event];
            return info;
        }

        public string sdkv { get; set; }

        public string appname { get; set; }

        public string author { get; set; }

        public string describe { get; set; }

        public string appv { get; set; }

        #region V3

        public class ApiAddresses
        {
            public JObject needapilist { get; set; }
        }

        public ApiAddresses data { get; set; }

        public int groupmsaddres { get; set; }

        public int friendmsaddres { get; set; }

        public int eventmsaddres { get; set; }

        public int unitproaddres { get; set; }

        public int setproaddres { get; set; }

        public int useproaddres { get; set; }

        public int banproaddres { get; set; }

        public int getvefcodeaddres { get; set; }

        public int getticketaddres { get; set; }

        public int SMSVerification { get; set; }

        public int SliderR { get; set; }
        #endregion V3

        #region V4

        public object needapilist { get; set; }

        public string 滑块识别处理函数 { get; set; }

        public string 短信接码处理函数 { get; set; }

        public string 被启用处理函数 { get; set; }

        public string 被禁用处理函数 { get; set; }

        public string 将被卸载处理函数 { get; set; }

        public string 插件菜单处理函数 { get; set; }

        public string 私聊消息处理函数 { get; set; }

        public string 群聊消息处理函数 { get; set; }

        public string 频道推送统一处理函数 { get; set; }

        public string 事件消息处理函数 { get; set; }

        public string 插件消息输出替换处理函数 { get; set; }

        #endregion V4
    }
}