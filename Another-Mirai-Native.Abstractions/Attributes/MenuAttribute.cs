using System;

namespace Another_Mirai_Native.Abstractions.Attributes
{
    /// <summary>
    /// 描述了菜单的特性，需要加在实现了 IMenuHandler 接口的类上，以指定菜单的名称。框架会根据此特性在插件加载时注册菜单，并在用户点击菜单时调用对应的处理器方法。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MenuAttribute : Attribute
    {
        /// <summary>
        /// 特性的默认构造函数，接受一个字符串参数作为菜单的名称。这个名称将用于框架中显示菜单项，并在用户点击菜单时识别对应的处理器。
        /// </summary>
        /// <param name="name">在管理器上显示的菜单名称</param>
        public MenuAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 在管理器上显示的菜单名称
        /// </summary>
        public string Name { get; set; }
    }
}
