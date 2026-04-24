using System;
using System.Collections.Generic;
using System.Text;

namespace Another_Mirai_Native.Abstractions.Enums
{
    /// <summary>
    /// 插件对于事件的处理结果枚举。用于指示事件处理程序在处理事件后是否允许事件继续传播给其他处理程序，或者阻止事件传播以独占事件的处理权。
    /// </summary>
    public enum EventHandleResult
    {
        /// <summary>
        /// 表示将事件传播到下一个处理程序，允许其他处理程序继续处理该事件。使用此选项可以实现事件的多层处理，使多个处理程序能够对同一事件进行响应和处理。
        /// </summary>
        Pass,

        /// <summary>
        /// 表示阻塞事件的传播，防止其他处理程序继续处理该事件。使用此选项可以确保当前处理程序独占事件的处理权，适用于需要完全控制事件处理流程的情况。
        /// </summary>
        Block
    }
}
