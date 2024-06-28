﻿using Another_Mirai_Native.Model;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using System;

namespace Another_Mirai_Native.Export
{
    public class XiaoLiZI_API
    {
        /// <summary>
        /// _初始化
        /// </summary>
        [ProxyAPIName("_初始化")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_1(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API _初始化");
            }
        }

        /// <summary>
        /// _销毁
        /// </summary>
        [ProxyAPIName("_销毁")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_2(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API _销毁");
            }
        }

        /// <summary>
        /// 取API函数地址
        /// <param name="arg0">函数名</param>
        /// </summary>
        [ProxyAPIName("")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_3(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取API函数地址");
            }
            return 0;
        }

        /// <summary>
        /// int
        /// <param name="arg0">_pluginkey</param>
        /// <param name="arg1">_apidata</param>
        /// </summary>
        [ProxyAPIName("int")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_4(string authCode, string arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API int");
            }
        }

        /// <summary>
        /// 输出日志
        /// <param name="arg0">日志</param>
        /// <param name="arg1">文字颜色</param>
        /// <param name="arg2">背景颜色</param>
        /// </summary>
        [ProxyAPIName("输出日志")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_5(string authCode, string arg0, int arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 输出日志");
            }
            return "";
        }

        /// <summary>
        /// 发送好友消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">发送内容</param>
        /// <param name="arg3">Random</param>
        /// <param name="arg4">Req</param>
        /// </summary>
        [ProxyAPIName("发送好友消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_6(string authCode, long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送好友消息");
            }
            return "";
        }

        /// <summary>
        /// 发送群消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">发送内容</param>
        /// <param name="arg3">匿名发送</param>
        /// </summary>
        [ProxyAPIName("发送群消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_7(string authCode, long arg0, long arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群消息");
            }
            return "";
        }

        /// <summary>
        /// 发送群临时消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群ID</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">发送内容</param>
        /// <param name="arg4">Random</param>
        /// <param name="arg5">Req</param>
        /// </summary>
        [ProxyAPIName("发送群临时消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_8(string authCode, long arg0, long arg1, long arg2, string arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群临时消息");
            }
            return "";
        }

        /// <summary>
        /// 添加好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">问题答案</param>
        /// <param name="arg3">备注</param>
        /// </summary>
        [ProxyAPIName("添加好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_9(string authCode, long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 添加好友");
            }
            return "";
        }

        /// <summary>
        /// 添加群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">验证消息</param>
        /// </summary>
        [ProxyAPIName("添加群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_10(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 添加群");
            }
            return "";
        }

        /// <summary>
        /// 删除好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("删除好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_11(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除好友");
            }
            return "";
        }

        /// <summary>
        /// 置屏蔽好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">是否屏蔽</param>
        /// </summary>
        [ProxyAPIName("置屏蔽好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_12(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置屏蔽好友");
            }
            return "";
        }

        /// <summary>
        /// 置特别关心好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">是否关心</param>
        /// </summary>
        [ProxyAPIName("置特别关心好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_13(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置特别关心好友");
            }
            return "";
        }

        /// <summary>
        /// 发送好友json消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">json代码</param>
        /// <param name="arg3">Random</param>
        /// <param name="arg4">Req</param>
        /// </summary>
        [ProxyAPIName("发送好友json消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_14(string authCode, long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送好友json消息");
            }
            return "";
        }

        /// <summary>
        /// 发送群json消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">json代码</param>
        /// <param name="arg3">匿名发送</param>
        /// </summary>
        [ProxyAPIName("发送群json消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_15(string authCode, long arg0, long arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群json消息");
            }
            return "";
        }

        /// <summary>
        /// 上传好友图片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">是否闪照</param>
        /// <param name="arg3">pic</param>
        /// <param name="arg4">宽度</param>
        /// <param name="arg5">高度</param>
        /// <param name="arg6">动图</param>
        /// </summary>
        [ProxyAPIName("上传好友图片")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_16(string authCode, long arg0, long arg1, bool arg2, byte[] arg3, int arg4, int arg5, bool arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传好友图片");
            }
            return "";
        }

        /// <summary>
        /// 上传群图片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否闪照</param>
        /// <param name="arg3">pic</param>
        /// <param name="arg4">宽度</param>
        /// <param name="arg5">高度</param>
        /// <param name="arg6">动图</param>
        /// </summary>
        [ProxyAPIName("上传群图片")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_17(string authCode, long arg0, long arg1, bool arg2, byte[] arg3, int arg4, int arg5, bool arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群图片");
            }
            return "";
        }

        /// <summary>
        /// 上传好友语音
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">语音类型</param>
        /// <param name="arg3">语音文字</param>
        /// <param name="arg4">audio</param>
        /// <param name="arg5">时长</param>
        /// </summary>
        [ProxyAPIName("上传好友语音")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_18(string authCode, long arg0, long arg1, int arg2, string arg3, byte[] arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传好友语音");
            }
            return "";
        }

        /// <summary>
        /// 上传群语音
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">语音类型</param>
        /// <param name="arg3">语音文字</param>
        /// <param name="arg4">audio</param>
        /// <param name="arg5">时长</param>
        /// </summary>
        [ProxyAPIName("上传群语音")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_19(string authCode, long arg0, long arg1, int arg2, string arg3, byte[] arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群语音");
            }
            return "";
        }

        /// <summary>
        /// 上传头像
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">pic</param>
        /// </summary>
        [ProxyAPIName("上传头像")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_20(string authCode, long arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传头像");
            }
            return "";
        }

        /// <summary>
        /// silk解码
        /// <param name="arg0">音频文件路径</param>
        /// </summary>
        [ProxyAPIName("silk解码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static IntPtr Function_21(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API silk解码");
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// silk编码
        /// <param name="arg0">音频文件路径</param>
        /// </summary>
        [ProxyAPIName("silk编码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static IntPtr Function_22(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API silk编码");
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// amr编码
        /// <param name="arg0">音频文件路径</param>
        /// </summary>
        [ProxyAPIName("amr编码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static IntPtr Function_23(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API amr编码");
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 设置群名片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">新名片</param>
        /// </summary>
        [ProxyAPIName("设置群名片")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_24(string authCode, long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置群名片");
            }
            return "";
        }

        /// <summary>
        /// 取昵称_从缓存
        /// <param name="arg0">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取昵称_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_25(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取昵称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 强制取昵称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("强制取昵称")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_26(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 强制取昵称");
            }
            return "";
        }

        /// <summary>
        /// 取群名称_从缓存
        /// <param name="arg0">群号</param>
        /// </summary>
        [ProxyAPIName("取群名称_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_27(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群名称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 获取skey
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("获取skey")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_28(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取skey");
            }
            return "";
        }

        /// <summary>
        /// 获取pskey
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">域</param>
        /// </summary>
        [ProxyAPIName("获取pskey")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_29(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取pskey");
            }
            return "";
        }

        /// <summary>
        /// 获取clientkey
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("获取clientkey")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_30(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取clientkey");
            }
            return "";
        }

        /// <summary>
        /// 取框架QQ
        /// </summary>
        [ProxyAPIName("取框架QQ")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_31(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架QQ");
            }
            return "";
        }

        /// <summary>
        /// 取好友列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取好友列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_32(string authCode, long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友列表");
            }
            return 0;
        }

        /// <summary>
        /// 取群列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取群列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_33(string authCode, long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群列表");
            }
            return 0;
        }

        /// <summary>
        /// 取群成员列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取群成员列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_34(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群成员列表");
            }
            return 0;
        }

        /// <summary>
        /// 设置管理员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">取消管理</param>
        /// </summary>
        [ProxyAPIName("设置管理员")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_35(string authCode, long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置管理员");
            }
            return false;
        }

        /// <summary>
        /// 取管理层列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取管理层列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_36(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取管理层列表");
            }
            return "";
        }

        /// <summary>
        /// 取群名片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// </summary>
        [ProxyAPIName("取群名片")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_37(string authCode, long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群名片");
            }
            return "";
        }

        /// <summary>
        /// 取个性签名
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取个性签名")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_38(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取个性签名");
            }
            return "";
        }

        /// <summary>
        /// 修改昵称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">昵称</param>
        /// </summary>
        [ProxyAPIName("修改昵称")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_39(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改昵称");
            }
            return false;
        }

        /// <summary>
        /// 修改个性签名
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">签名</param>
        /// <param name="arg2">签名地点</param>
        /// </summary>
        [ProxyAPIName("修改个性签名")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_40(string authCode, long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改个性签名");
            }
            return false;
        }

        /// <summary>
        /// 删除群成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">拒绝加群申请</param>
        /// </summary>
        [ProxyAPIName("删除群成员")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_41(string authCode, long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群成员");
            }
            return false;
        }

        /// <summary>
        /// 禁言群成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">禁言时长</param>
        /// </summary>
        [ProxyAPIName("禁言群成员")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_42(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 禁言群成员");
            }
            return false;
        }

        /// <summary>
        /// 退群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("退群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_43(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 退群");
            }
            return false;
        }

        /// <summary>
        /// 解散群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("解散群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_44(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 解散群");
            }
            return false;
        }

        /// <summary>
        /// 上传群头像
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">pic</param>
        /// </summary>
        [ProxyAPIName("上传群头像")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_45(string authCode, long arg0, long arg1, byte[] arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群头像");
            }
            return false;
        }

        /// <summary>
        /// 全员禁言
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否开启</param>
        /// </summary>
        [ProxyAPIName("全员禁言")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_46(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 全员禁言");
            }
            return false;
        }

        /// <summary>
        /// 群权限_发起新的群聊
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_发起新的群聊")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_47(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_发起新的群聊");
            }
            return false;
        }

        /// <summary>
        /// 群权限_发起临时会话
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_发起临时会话")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_48(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_发起临时会话");
            }
            return false;
        }

        /// <summary>
        /// 群权限_上传文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_上传文件")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_49(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_上传文件");
            }
            return false;
        }

        /// <summary>
        /// 群权限_上传相册
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_上传相册")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_50(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_上传相册");
            }
            return false;
        }

        /// <summary>
        /// 群权限_邀请好友加群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_邀请好友加群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_51(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_邀请好友加群");
            }
            return false;
        }

        /// <summary>
        /// 群权限_匿名聊天
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_匿名聊天")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_52(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_匿名聊天");
            }
            return false;
        }

        /// <summary>
        /// 群权限_坦白说
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_坦白说")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_53(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_坦白说");
            }
            return false;
        }

        /// <summary>
        /// 群权限_新成员查看历史消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_新成员查看历史消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_54(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_新成员查看历史消息");
            }
            return false;
        }

        /// <summary>
        /// 群权限_邀请方式设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">方式</param>
        /// </summary>
        [ProxyAPIName("群权限_邀请方式设置")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_55(string authCode, long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_邀请方式设置");
            }
            return false;
        }

        /// <summary>
        /// 撤回消息_群聊
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">消息Random</param>
        /// <param name="arg3">消息Req</param>
        /// </summary>
        [ProxyAPIName("撤回消息_群聊")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_56(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回消息_群聊");
            }
            return false;
        }

        /// <summary>
        /// 撤回消息_私聊本身
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">消息Random</param>
        /// <param name="arg3">消息Req</param>
        /// <param name="arg4">消息接收时间</param>
        /// </summary>
        [ProxyAPIName("撤回消息_私聊本身")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_57(string authCode, long arg0, long arg1, long arg2, int arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回消息_私聊本身");
            }
            return false;
        }

        /// <summary>
        /// 设置位置共享
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">经度</param>
        /// <param name="arg3">纬度</param>
        /// <param name="arg4">是否开启</param>
        /// </summary>
        [ProxyAPIName("设置位置共享")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_58(string authCode, long arg0, long arg1, double arg2, double arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置位置共享");
            }
            return false;
        }

        /// <summary>
        /// 上报当前位置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">经度</param>
        /// <param name="arg3">纬度</param>
        /// </summary>
        [ProxyAPIName("上报当前位置")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_59(string authCode, long arg0, long arg1, double arg2, double arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上报当前位置");
            }
            return false;
        }

        /// <summary>
        /// 是否被禁言
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("是否被禁言")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static long Function_60(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 是否被禁言");
            }
            return 0;
        }

        /// <summary>
        /// 处理群验证事件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">触发QQ</param>
        /// <param name="arg3">消息Seq</param>
        /// <param name="arg4">操作类型</param>
        /// <param name="arg5">事件类型</param>
        /// <param name="arg6">拒绝理由</param>
        /// </summary>
        [ProxyAPIName("处理群验证事件")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_61(string authCode, long arg0, long arg1, long arg2, long arg3, int arg4, int arg5, string arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 处理群验证事件");
            }
        }

        /// <summary>
        /// 处理好友验证事件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">触发QQ</param>
        /// <param name="arg2">消息Seq</param>
        /// <param name="arg3">操作类型</param>
        /// </summary>
        [ProxyAPIName("处理好友验证事件")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_62(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 处理好友验证事件");
            }
        }

        /// <summary>
        /// 查看转发聊天记录内容
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">resId</param>
        /// <param name="arg2">消息内容</param>
        /// </summary>
        [ProxyAPIName("查看转发聊天记录内容")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_63(string authCode, long arg0, string arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查看转发聊天记录内容");
            }
        }

        /// <summary>
        /// 上传群文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件路径</param>
        /// <param name="arg3">文件夹名</param>
        /// </summary>
        [ProxyAPIName("上传群文件")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_64(string authCode, long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群文件");
            }
            return "";
        }

        /// <summary>
        /// 创建群文件夹
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件夹名</param>
        /// </summary>
        [ProxyAPIName("创建群文件夹")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_65(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建群文件夹");
            }
            return "";
        }

        /// <summary>
        /// 重命名群文件夹
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">旧文件夹名</param>
        /// <param name="arg3">新文件夹名</param>
        /// </summary>
        [ProxyAPIName("重命名群文件夹")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_66(string authCode, long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 重命名群文件夹");
            }
            return "";
        }

        /// <summary>
        /// 删除群文件夹
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件夹名</param>
        /// </summary>
        [ProxyAPIName("删除群文件夹")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_67(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群文件夹");
            }
            return "";
        }

        /// <summary>
        /// 删除群文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件fileid</param>
        /// <param name="arg3">文件夹名</param>
        /// </summary>
        [ProxyAPIName("删除群文件")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_68(string authCode, long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群文件");
            }
            return "";
        }

        /// <summary>
        /// 保存文件到微云
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件fileid</param>
        /// </summary>
        [ProxyAPIName("保存文件到微云")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_69(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 保存文件到微云");
            }
            return "";
        }

        /// <summary>
        /// 移动群文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件fileid</param>
        /// <param name="arg3">当前文件夹名</param>
        /// <param name="arg4">目标文件夹名</param>
        /// </summary>
        [ProxyAPIName("移动群文件")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_70(string authCode, long arg0, long arg1, string arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 移动群文件");
            }
            return "";
        }

        /// <summary>
        /// 取群文件列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件夹名</param>
        /// <param name="arg3">数据</param>
        /// </summary>
        [ProxyAPIName("取群文件列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_71(string authCode, long arg0, long arg1, string arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群文件列表");
            }
            return "";
        }

        /// <summary>
        /// 设置在线状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">main</param>
        /// <param name="arg2">sun</param>
        /// <param name="arg3">电量</param>
        /// </summary>
        [ProxyAPIName("设置在线状态")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_72(string authCode, long arg0, int arg1, int arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置在线状态");
            }
            return false;
        }

        /// <summary>
        /// 取插件数据目录
        /// </summary>
        [ProxyAPIName("取插件数据目录")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_73(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取插件数据目录");
            }
            return "";
        }

        /// <summary>
        /// QQ点赞
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("QQ点赞")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_74(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ点赞");
            }
            return "";
        }

        /// <summary>
        /// 取图片下载地址
        /// <param name="arg0">图片代码</param>
        /// <param name="arg1">框架QQ</param>
        /// <param name="arg2">群号</param>
        /// </summary>
        [ProxyAPIName("取图片下载地址")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_75(string authCode, string arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取图片下载地址");
            }
            return "";
        }

        /// <summary>
        /// 查询好友信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("查询好友信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_76(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询好友信息");
            }
            return false;
        }

        /// <summary>
        /// 查询群信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("查询群信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_77(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询群信息");
            }
            return false;
        }

        /// <summary>
        /// 框架重启
        /// </summary>
        [ProxyAPIName("框架重启")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_78(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 框架重启");
            }
        }

        /// <summary>
        /// 群文件转发至群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">目标群号</param>
        /// <param name="arg3">fileId</param>
        /// </summary>
        [ProxyAPIName("群文件转发至群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_79(string authCode, long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群文件转发至群");
            }
            return false;
        }

        /// <summary>
        /// 群文件转发至好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">目标QQ</param>
        /// <param name="arg3">fileId</param>
        /// <param name="arg4">filename</param>
        /// <param name="arg5">filesize</param>
        /// <param name="arg6">Req</param>
        /// <param name="arg7">Random</param>
        /// <param name="arg8">time</param>
        /// </summary>
        [ProxyAPIName("群文件转发至好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_80(string authCode, long arg0, long arg1, long arg2, string arg3, string arg4, long arg5, int arg6, long arg7, int arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群文件转发至好友");
            }
            return false;
        }

        /// <summary>
        /// 好友文件转发至好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标QQ</param>
        /// <param name="arg2">fileId</param>
        /// <param name="arg3">filename</param>
        /// <param name="arg4">filesize</param>
        /// <param name="arg5">Req</param>
        /// <param name="arg6">Random</param>
        /// <param name="arg7">time</param>
        /// </summary>
        [ProxyAPIName("好友文件转发至好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_81(string authCode, long arg0, long arg1, string arg2, string arg3, long arg4, int arg5, long arg6, int arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友文件转发至好友");
            }
            return false;
        }

        /// <summary>
        /// 置群消息接收
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">设置类型</param>
        /// </summary>
        [ProxyAPIName("置群消息接收")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_82(string authCode, long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置群消息接收");
            }
            return false;
        }

        /// <summary>
        /// 取好友在线状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取好友在线状态")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_83(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友在线状态");
            }
            return "";
        }

        /// <summary>
        /// 取QQ钱包个人信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取QQ钱包个人信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_84(string authCode, long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ钱包个人信息");
            }
            return "";
        }

        /// <summary>
        /// 获取订单详情
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">订单号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("获取订单详情")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_85(string authCode, long arg0, string arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取订单详情");
            }
            return "";
        }

        /// <summary>
        /// 提交支付验证码
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">验证码信息</param>
        /// <param name="arg2">验证码</param>
        /// <param name="arg3">支付密码</param>
        /// </summary>
        [ProxyAPIName("提交支付验证码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_86(string authCode, long arg0, object arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 提交支付验证码");
            }
            return "";
        }

        /// <summary>
        /// 分享音乐
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">分享对象</param>
        /// <param name="arg2">歌曲名</param>
        /// <param name="arg3">歌手名</param>
        /// <param name="arg4">跳转地址</param>
        /// <param name="arg5">封面地址</param>
        /// <param name="arg6">文件地址</param>
        /// <param name="arg7">应用类型</param>
        /// <param name="arg8">分享类型</param>
        /// </summary>
        [ProxyAPIName("分享音乐")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_87(string authCode, long arg0, long arg1, string arg2, string arg3, string arg4, string arg5, string arg6, int arg7, int arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 分享音乐");
            }
            return false;
        }

        /// <summary>
        /// 更改群聊消息内容
        /// <param name="arg0">数据指针</param>
        /// <param name="arg1">新消息内容</param>
        /// </summary>
        [ProxyAPIName("更改群聊消息内容")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_88(string authCode, int arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 更改群聊消息内容");
            }
            return false;
        }

        /// <summary>
        /// 更改私聊消息内容
        /// <param name="arg0">数据指针</param>
        /// <param name="arg1">新消息内容</param>
        /// </summary>
        [ProxyAPIName("更改私聊消息内容")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_89(string authCode, int arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 更改私聊消息内容");
            }
            return false;
        }

        /// <summary>
        /// 群聊口令红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊口令红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_90(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊口令红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊拼手气红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊拼手气红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_91(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊拼手气红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊普通红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_92(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊普通红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊画图红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">题目名</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊画图红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_93(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊画图红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊语音红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">语音口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊语音红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_94(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊语音红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊接龙红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">接龙内容</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊接龙红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_95(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊接龙红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">领取人</param>
        /// <param name="arg5">祝福语</param>
        /// <param name="arg6">是否均分</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊专属红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_96(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, bool arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊专属红包");
            }
            return "";
        }

        /// <summary>
        /// 好友口令红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友口令红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_97(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友口令红包");
            }
            return "";
        }

        /// <summary>
        /// 好友普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友普通红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_98(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友普通红包");
            }
            return "";
        }

        /// <summary>
        /// 好友画图红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">题目名</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友画图红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_99(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友画图红包");
            }
            return "";
        }

        /// <summary>
        /// 好友语音红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">语音口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友语音红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_100(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友语音红包");
            }
            return "";
        }

        /// <summary>
        /// 好友接龙红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">接龙内容</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友接龙红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_101(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友接龙红包");
            }
            return "";
        }

        /// <summary>
        /// 设置专属头衔
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">头衔</param>
        /// </summary>
        [ProxyAPIName("设置专属头衔")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_102(string authCode, long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置专属头衔");
            }
            return false;
        }

        /// <summary>
        /// 下线指定QQ
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("下线指定QQ")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_103(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 下线指定QQ");
            }
            return false;
        }

        /// <summary>
        /// 登录指定QQ
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("登录指定QQ")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_104(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 登录指定QQ");
            }
            return false;
        }

        /// <summary>
        /// 取群未领红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取群未领红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_105(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群未领红包");
            }
            return 0;
        }

        /// <summary>
        /// 发送输入状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">输入状态</param>
        /// </summary>
        [ProxyAPIName("发送输入状态")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_106(string authCode, long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送输入状态");
            }
            return false;
        }

        /// <summary>
        /// 修改资料
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">昵称</param>
        /// <param name="arg2">性别</param>
        /// <param name="arg3">生日</param>
        /// <param name="arg4">职业</param>
        /// <param name="arg5">公司名</param>
        /// <param name="arg6">所在地</param>
        /// <param name="arg7">家乡</param>
        /// <param name="arg8">邮箱</param>
        /// <param name="arg9">个人说明</param>
        /// </summary>
        [ProxyAPIName("修改资料")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_107(string authCode, long arg0, string arg1, int arg2, string arg3, int arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改资料");
            }
            return false;
        }

        /// <summary>
        /// 取群文件下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">文件id</param>
        /// <param name="arg3">文件名</param>
        /// </summary>
        [ProxyAPIName("取群文件下载地址")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_108(string authCode, long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群文件下载地址");
            }
            return "";
        }

        /// <summary>
        /// 打好友电话
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("打好友电话")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_109(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 打好友电话");
            }
        }

        /// <summary>
        /// 头像双击_好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("头像双击_好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_110(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 头像双击_好友");
            }
            return false;
        }

        /// <summary>
        /// 头像双击_群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">群号</param>
        /// </summary>
        [ProxyAPIName("头像双击_群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_111(string authCode, long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 头像双击_群");
            }
            return false;
        }

        /// <summary>
        /// 取群成员简略信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取群成员简略信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_112(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群成员简略信息");
            }
            return "";
        }

        /// <summary>
        /// 群聊置顶
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">置顶</param>
        /// </summary>
        [ProxyAPIName("群聊置顶")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_113(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊置顶");
            }
            return false;
        }

        /// <summary>
        /// 私聊置顶
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">置顶</param>
        /// </summary>
        [ProxyAPIName("私聊置顶")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_114(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 私聊置顶");
            }
            return false;
        }

        /// <summary>
        /// 取加群链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取加群链接")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_115(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取加群链接");
            }
            return "";
        }

        /// <summary>
        /// 设为精华
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">消息Req</param>
        /// <param name="arg3">消息Random</param>
        /// </summary>
        [ProxyAPIName("设为精华")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_116(string authCode, long arg0, long arg1, int arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设为精华");
            }
            return false;
        }

        /// <summary>
        /// 群权限_设置群昵称规则
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">名片规则</param>
        /// </summary>
        [ProxyAPIName("群权限_设置群昵称规则")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_117(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_设置群昵称规则");
            }
            return false;
        }

        /// <summary>
        /// 群权限_设置群发言频率
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">限制条数</param>
        /// </summary>
        [ProxyAPIName("群权限_设置群发言频率")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_118(string authCode, long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_设置群发言频率");
            }
            return false;
        }

        /// <summary>
        /// 群权限_设置群查找方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">查找方式</param>
        /// </summary>
        [ProxyAPIName("群权限_设置群查找方式")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_119(string authCode, long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_设置群查找方式");
            }
            return false;
        }

        /// <summary>
        /// 邀请好友加群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">来源群号</param>
        /// </summary>
        [ProxyAPIName("邀请好友加群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_120(string authCode, long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 邀请好友加群");
            }
            return false;
        }

        /// <summary>
        /// 置群内消息通知
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">通知类型</param>
        /// </summary>
        [ProxyAPIName("置群内消息通知")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_121(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置群内消息通知");
            }
            return false;
        }

        /// <summary>
        /// 修改群名称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">名称</param>
        /// </summary>
        [ProxyAPIName("修改群名称")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_122(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改群名称");
            }
            return false;
        }

        /// <summary>
        /// 重载自身
        /// <param name="arg0">新文件路径</param>
        /// </summary>
        [ProxyAPIName("重载自身")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_123(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 重载自身");
            }
        }

        /// <summary>
        /// 下线其他设备
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">移动设备</param>
        /// <param name="arg2">appid</param>
        /// </summary>
        [ProxyAPIName("下线其他设备")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_124(string authCode, long arg0, bool arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 下线其他设备");
            }
        }

        /// <summary>
        /// 登录网页取ck
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">回调跳转地址</param>
        /// <param name="arg2">appid</param>
        /// <param name="arg3">daid</param>
        /// <param name="arg4">cookie</param>
        /// </summary>
        [ProxyAPIName("登录网页取ck")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_125(string authCode, long arg0, string arg1, string arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 登录网页取ck");
            }
            return false;
        }

        /// <summary>
        /// 发送群公告
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">标题</param>
        /// <param name="arg3">内容</param>
        /// <param name="arg4">图片</param>
        /// <param name="arg5">视频</param>
        /// <param name="arg6">弹窗展示</param>
        /// <param name="arg7">需要确认</param>
        /// <param name="arg8">置顶</param>
        /// <param name="arg9">发送给新成员</param>
        /// <param name="arg10">引导修改群昵称</param>
        /// </summary>
        [ProxyAPIName("发送群公告")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_126(string authCode, long arg0, long arg1, string arg2, string arg3, byte[] arg4, string arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, arg10={arg10}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群公告");
            }
            return "";
        }

        /// <summary>
        /// 取框架版本
        /// </summary>
        [ProxyAPIName("取框架版本")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_127(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架版本");
            }
            return "";
        }

        /// <summary>
        /// 取群成员信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">数据</param>
        /// </summary>
        [ProxyAPIName("取群成员信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_128(string authCode, long arg0, long arg1, long arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群成员信息");
            }
            return "";
        }

        /// <summary>
        /// 取钱包cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取钱包cookie")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_129(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取钱包cookie");
            }
            return "";
        }

        /// <summary>
        /// 取群网页cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取群网页cookie")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_130(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群网页cookie");
            }
            return "";
        }

        /// <summary>
        /// 转账
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">转账金额</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">转账留言</param>
        /// <param name="arg4">转账类型</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("转账")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_131(string authCode, long arg0, int arg1, long arg2, string arg3, int arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 转账");
            }
            return "";
        }

        /// <summary>
        /// 取收款链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款金额</param>
        /// <param name="arg2">说明文本</param>
        /// </summary>
        [ProxyAPIName("取收款链接")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_132(string authCode, long arg0, int arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取收款链接");
            }
            return "";
        }

        /// <summary>
        /// 取群小视频下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">来源QQ</param>
        /// <param name="arg3">param</param>
        /// <param name="arg4">hash1</param>
        /// <param name="arg5">文件名</param>
        /// </summary>
        [ProxyAPIName("取群小视频下载地址")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_133(string authCode, long arg0, long arg1, long arg2, string arg3, string arg4, string arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群小视频下载地址");
            }
            return "";
        }

        /// <summary>
        /// 取私聊小视频下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源QQ</param>
        /// <param name="arg2">param</param>
        /// <param name="arg3">hash1</param>
        /// <param name="arg4">文件名</param>
        /// </summary>
        [ProxyAPIName("取私聊小视频下载地址")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_134(string authCode, long arg0, long arg1, string arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取私聊小视频下载地址");
            }
            return "";
        }

        /// <summary>
        /// 上传小视频
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">本地小视频路径</param>
        /// <param name="arg3">小视频封面图</param>
        /// <param name="arg4">宽度</param>
        /// <param name="arg5">高度</param>
        /// <param name="arg6">时长</param>
        /// </summary>
        [ProxyAPIName("上传小视频")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_135(string authCode, long arg0, long arg1, string arg2, byte[] arg3, int arg4, int arg5, int arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传小视频");
            }
            return "";
        }

        /// <summary>
        /// 发送好友xml消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">xml代码</param>
        /// <param name="arg3">Random</param>
        /// <param name="arg4">Req</param>
        /// </summary>
        [ProxyAPIName("发送好友xml消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_136(string authCode, long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送好友xml消息");
            }
            return "";
        }

        /// <summary>
        /// 发送群xml消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">xml代码</param>
        /// <param name="arg3">匿名发送</param>
        /// </summary>
        [ProxyAPIName("发送群xml消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_137(string authCode, long arg0, long arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群xml消息");
            }
            return "";
        }

        /// <summary>
        /// 取群成员概况
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取群成员概况")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_138(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群成员概况");
            }
            return "";
        }

        /// <summary>
        /// 添加好友_取验证类型
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("添加好友_取验证类型")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_139(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 添加好友_取验证类型");
            }
            return "";
        }

        /// <summary>
        /// 群聊打卡
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("群聊打卡")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_140(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊打卡");
            }
            return "";
        }

        /// <summary>
        /// 群聊签到
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">附加参数</param>
        /// </summary>
        [ProxyAPIName("群聊签到")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_141(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊签到");
            }
            return false;
        }

        /// <summary>
        /// 置群聊备注
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">备注</param>
        /// </summary>
        [ProxyAPIName("置群聊备注")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_142(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置群聊备注");
            }
            return false;
        }

        /// <summary>
        /// 红包转发
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">红包ID</param>
        /// <param name="arg2">目标对象</param>
        /// <param name="arg3">Type</param>
        /// </summary>
        [ProxyAPIName("红包转发")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_143(string authCode, long arg0, string arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 红包转发");
            }
            return "";
        }

        /// <summary>
        /// 发送数据包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">包体序号</param>
        /// <param name="arg2">最大等待时长</param>
        /// <param name="arg3">数据</param>
        /// </summary>
        [ProxyAPIName("发送数据包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_144(string authCode, long arg0, int arg1, int arg2, byte[] arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送数据包");
            }
            return false;
        }

        /// <summary>
        /// 请求ssoseq
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("请求ssoseq")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_145(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 请求ssoseq");
            }
            return 0;
        }

        /// <summary>
        /// 取sessionkey
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取sessionkey")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_146(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取sessionkey");
            }
            return "";
        }

        /// <summary>
        /// 获取bkn_gtk
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">自定义bkn_gtk</param>
        /// </summary>
        [ProxyAPIName("获取bkn_gtk")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_147(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取bkn_gtk");
            }
            return "";
        }

        /// <summary>
        /// 置好友验证方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">验证方式</param>
        /// <param name="arg2">Q_and_A</param>
        /// </summary>
        [ProxyAPIName("置好友验证方式")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_148(string authCode, long arg0, int arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置好友验证方式");
            }
            return false;
        }

        /// <summary>
        /// 上传照片墙图片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">pic</param>
        /// </summary>
        [ProxyAPIName("上传照片墙图片")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_149(string authCode, long arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传照片墙图片");
            }
            return "";
        }

        /// <summary>
        /// 付款
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">QrcodeUrl</param>
        /// <param name="arg2">银行卡序列</param>
        /// <param name="arg3">支付密码</param>
        /// <param name="arg4">验证码信息</param>
        /// </summary>
        [ProxyAPIName("付款")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_150(string authCode, long arg0, string arg1, int arg2, string arg3, object arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 付款");
            }
            return "";
        }

        /// <summary>
        /// 修改支付密码
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">原密码</param>
        /// <param name="arg2">新密码</param>
        /// </summary>
        [ProxyAPIName("修改支付密码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_151(string authCode, long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改支付密码");
            }
            return "";
        }

        /// <summary>
        /// 账号搜索
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">关键词</param>
        /// </summary>
        [ProxyAPIName("账号搜索")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_152(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 账号搜索");
            }
            return "";
        }

        /// <summary>
        /// 添加群_取验证类型
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("添加群_取验证类型")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_153(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 添加群_取验证类型");
            }
            return "";
        }

        /// <summary>
        /// 获取红包领取详情
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">红包文本代码</param>
        /// <param name="arg3">红包类型</param>
        /// </summary>
        [ProxyAPIName("获取红包领取详情")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_154(string authCode, long arg0, long arg1, string arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取红包领取详情");
            }
            return "";
        }

        /// <summary>
        /// 取好友文件下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">FileId</param>
        /// <param name="arg2">FileName</param>
        /// </summary>
        [ProxyAPIName("取好友文件下载地址")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_155(string authCode, long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友文件下载地址");
            }
            return "";
        }

        /// <summary>
        /// 删除群成员_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">拒绝加群申请</param>
        /// </summary>
        [ProxyAPIName("删除群成员_批量")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_156(string authCode, long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群成员_批量");
            }
            return false;
        }

        /// <summary>
        /// 取扩列资料
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取扩列资料")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_157(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取扩列资料");
            }
            return "";
        }

        /// <summary>
        /// 取资料展示设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取资料展示设置")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_158(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取资料展示设置");
            }
            return "";
        }

        /// <summary>
        /// 设置资料展示
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("设置资料展示")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_159(string authCode, long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置资料展示");
            }
            return "";
        }

        /// <summary>
        /// 获取当前登录设备信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">信息</param>
        /// </summary>
        [ProxyAPIName("获取当前登录设备信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_160(string authCode, long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取当前登录设备信息");
            }
            return "";
        }

        /// <summary>
        /// 提取图片文字
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">图片地址</param>
        /// <param name="arg2">识别结果</param>
        /// </summary>
        [ProxyAPIName("提取图片文字")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_161(string authCode, long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 提取图片文字");
            }
            return false;
        }

        /// <summary>
        /// 取插件文件名
        /// </summary>
        [ProxyAPIName("取插件文件名")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_162(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取插件文件名");
            }
            return "";
        }

        /// <summary>
        /// TEA加密
        /// <param name="arg0">内容</param>
        /// <param name="arg1">秘钥</param>
        /// </summary>
        [ProxyAPIName("TEA加密")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_163(string authCode, byte[] arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API TEA加密");
            }
        }

        /// <summary>
        /// TEA解密
        /// <param name="arg0">内容</param>
        /// <param name="arg1">秘钥</param>
        /// </summary>
        [ProxyAPIName("TEA解密")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_164(string authCode, byte[] arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API TEA解密");
            }
        }

        /// <summary>
        /// 红包数据加密
        /// <param name="arg0">str</param>
        /// <param name="arg1">random</param>
        /// </summary>
        [ProxyAPIName("红包数据加密")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_165(string authCode, string arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 红包数据加密");
            }
            return "";
        }

        /// <summary>
        /// 红包数据解密
        /// <param name="arg0">str</param>
        /// <param name="arg1">random</param>
        /// </summary>
        [ProxyAPIName("红包数据解密")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_166(string authCode, string arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 红包数据解密");
            }
            return "";
        }

        /// <summary>
        /// 红包msgno计算
        /// <param name="arg0">目标QQ</param>
        /// </summary>
        [ProxyAPIName("红包msgno计算")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_167(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 红包msgno计算");
            }
            return "";
        }

        /// <summary>
        /// 取消精华
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">消息Req</param>
        /// <param name="arg3">消息Random</param>
        /// </summary>
        [ProxyAPIName("取消精华")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_168(string authCode, long arg0, long arg1, int arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取消精华");
            }
            return false;
        }

        /// <summary>
        /// 群权限_设置加群方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">加群方式</param>
        /// <param name="arg3">问题</param>
        /// <param name="arg4">答案</param>
        /// </summary>
        [ProxyAPIName("群权限_设置加群方式")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_169(string authCode, long arg0, long arg1, int arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_设置加群方式");
            }
            return false;
        }

        /// <summary>
        /// 群权限_群幸运字符
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否开启</param>
        /// </summary>
        [ProxyAPIName("群权限_群幸运字符")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_170(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_群幸运字符");
            }
            return false;
        }

        /// <summary>
        /// 群权限_一起写
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_一起写")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_171(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_一起写");
            }
            return false;
        }

        /// <summary>
        /// 取QQ空间cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取QQ空间cookie")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_172(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ空间cookie");
            }
            return "";
        }

        /// <summary>
        /// 框架是否为单Q
        /// </summary>
        [ProxyAPIName("框架是否为单Q")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_173(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 框架是否为单Q");
            }
            return false;
        }

        /// <summary>
        /// 修改指定QQ缓存密码
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">新密码</param>
        /// </summary>
        [ProxyAPIName("修改指定QQ缓存密码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_174(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改指定QQ缓存密码");
            }
            return false;
        }

        /// <summary>
        /// 处理群验证事件_风险号
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">触发QQ</param>
        /// <param name="arg3">消息Seq</param>
        /// <param name="arg4">操作类型</param>
        /// <param name="arg5">事件类型</param>
        /// <param name="arg6">拒绝理由</param>
        /// </summary>
        [ProxyAPIName("处理群验证事件_风险号")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_175(string authCode, long arg0, long arg1, long arg2, long arg3, int arg4, int arg5, string arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 处理群验证事件_风险号");
            }
        }

        /// <summary>
        /// 查询网址安全性
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">网址</param>
        /// </summary>
        [ProxyAPIName("查询网址安全性")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_176(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询网址安全性");
            }
            return 0;
        }

        /// <summary>
        /// 消息合并转发至好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">聊天记录</param>
        /// <param name="arg3">Random</param>
        /// <param name="arg4">Req</param>
        /// <param name="arg5">消息记录来源</param>
        /// </summary>
        [ProxyAPIName("消息合并转发至好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_177(string authCode, long arg0, long arg1, object arg2, long arg3, int arg4, string arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 消息合并转发至好友");
            }
            return "";
        }

        /// <summary>
        /// 消息合并转发至群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">聊天记录</param>
        /// <param name="arg3">匿名发送</param>
        /// <param name="arg4">消息记录来源</param>
        /// </summary>
        [ProxyAPIName("消息合并转发至群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_178(string authCode, long arg0, long arg1, object arg2, bool arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 消息合并转发至群");
            }
            return "";
        }

        /// <summary>
        /// 取卡片消息代码
        /// <param name="arg0">卡片消息文本代码</param>
        /// </summary>
        [ProxyAPIName("取卡片消息代码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_179(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取卡片消息代码");
            }
            return "";
        }

        /// <summary>
        /// 禁言群匿名
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">匿名昵称</param>
        /// <param name="arg3">匿名标识</param>
        /// <param name="arg4">禁言时长</param>
        /// </summary>
        [ProxyAPIName("禁言群匿名")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_180(string authCode, long arg0, long arg1, string arg2, byte[] arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 禁言群匿名");
            }
            return false;
        }

        /// <summary>
        /// 置文件下载
        /// <param name="arg0">文件下载地址</param>
        /// <param name="arg1">文件保存路径</param>
        /// <param name="arg2">下载回调函数</param>
        /// <param name="arg3">文件名</param>
        /// <param name="arg4">下载起点</param>
        /// </summary>
        [ProxyAPIName("置文件下载")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_181(string authCode, string arg0, string arg1, object arg2, string arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置文件下载");
            }
            return 0;
        }

        /// <summary>
        /// 领取私聊普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源QQ</param>
        /// <param name="arg2">红包文本代码</param>
        /// <param name="arg3">类型</param>
        /// </summary>
        [ProxyAPIName("领取私聊普通红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_182(string authCode, long arg0, long arg1, string arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 领取私聊普通红包");
            }
            return "";
        }

        /// <summary>
        /// 领取群聊专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">来源QQ</param>
        /// <param name="arg3">红包文本代码</param>
        /// </summary>
        [ProxyAPIName("领取群聊专属红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_183(string authCode, long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 领取群聊专属红包");
            }
            return "";
        }

        /// <summary>
        /// 加载网页
        /// <param name="arg0">网址</param>
        /// </summary>
        [ProxyAPIName("加载网页")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_184(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 加载网页");
            }
            return false;
        }

        /// <summary>
        /// 压缩包_7za解压
        /// <param name="arg0">压缩包路径</param>
        /// <param name="arg1">解压保存路径</param>
        /// <param name="arg2">解压密码</param>
        /// <param name="arg3">跳过已存在的文件</param>
        /// </summary>
        [ProxyAPIName("压缩包_7za解压")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_185(string authCode, string arg0, string arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 压缩包_7za解压");
            }
        }

        /// <summary>
        /// 压缩包_7za压缩
        /// <param name="arg0">保存路径</param>
        /// <param name="arg1">欲压缩的文件</param>
        /// <param name="arg2">压缩格式</param>
        /// <param name="arg3">压缩等级</param>
        /// <param name="arg4">压缩密码</param>
        /// </summary>
        [ProxyAPIName("压缩包_7za压缩")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void Function_186(string authCode, string arg0, string arg1, string arg2, int arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 压缩包_7za压缩");
            }
        }

        /// <summary>
        /// 发送讨论组消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">消息内容</param>
        /// </summary>
        [ProxyAPIName("发送讨论组消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_187(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送讨论组消息");
            }
            return "";
        }

        /// <summary>
        /// 发送讨论组json消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">Json代码</param>
        /// </summary>
        [ProxyAPIName("发送讨论组json消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_188(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送讨论组json消息");
            }
            return "";
        }

        /// <summary>
        /// 发送讨论组xml消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">Xml代码</param>
        /// </summary>
        [ProxyAPIName("发送讨论组xml消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_189(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送讨论组xml消息");
            }
            return "";
        }

        /// <summary>
        /// 发送讨论组临时消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">消息内容</param>
        /// <param name="arg4">消息Random</param>
        /// <param name="arg5">消息Req</param>
        /// </summary>
        [ProxyAPIName("发送讨论组临时消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_190(string authCode, long arg0, long arg1, long arg2, string arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送讨论组临时消息");
            }
            return "";
        }

        /// <summary>
        /// 撤回消息_讨论组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">消息Random</param>
        /// <param name="arg3">消息Req</param>
        /// </summary>
        [ProxyAPIName("撤回消息_讨论组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_191(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回消息_讨论组");
            }
            return false;
        }

        /// <summary>
        /// 回复QQ咨询会话
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">会话Token</param>
        /// <param name="arg3">消息内容</param>
        /// <param name="arg4">消息Random</param>
        /// <param name="arg5">消息Req</param>
        /// </summary>
        [ProxyAPIName("回复QQ咨询会话")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_192(string authCode, long arg0, long arg1, byte[] arg2, string arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 回复QQ咨询会话");
            }
            return "";
        }

        /// <summary>
        /// 发送订阅号私聊消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">订阅号Id</param>
        /// <param name="arg2">消息内容</param>
        /// <param name="arg3">消息Random</param>
        /// <param name="arg4">消息Req</param>
        /// </summary>
        [ProxyAPIName("发送订阅号私聊消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_193(string authCode, long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送订阅号私聊消息");
            }
            return "";
        }

        /// <summary>
        /// 取讨论组名称_从缓存
        /// <param name="arg0">讨论组Id</param>
        /// </summary>
        [ProxyAPIName("取讨论组名称_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_194(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组名称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 修改讨论组名称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">新名称</param>
        /// </summary>
        [ProxyAPIName("修改讨论组名称")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_195(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改讨论组名称");
            }
            return false;
        }

        /// <summary>
        /// 取讨论组成员列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取讨论组成员列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_196(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组成员列表");
            }
            return 0;
        }

        /// <summary>
        /// 强制取自身匿名Id
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("强制取自身匿名Id")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static long Function_197(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 强制取自身匿名Id");
            }
            return 0;
        }

        /// <summary>
        /// 取订阅号列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取订阅号列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_198(string authCode, long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取订阅号列表");
            }
            return 0;
        }

        /// <summary>
        /// 取讨论组列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取讨论组列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_199(string authCode, long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组列表");
            }
            return 0;
        }

        /// <summary>
        /// 邀请好友加群_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标群号</param>
        /// <param name="arg2">邀请QQ</param>
        /// <param name="arg3">来源群号</param>
        /// </summary>
        [ProxyAPIName("邀请好友加群_批量")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_200(string authCode, long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 邀请好友加群_批量");
            }
            return false;
        }

        /// <summary>
        /// 邀请好友加入讨论组_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标讨论组Id</param>
        /// <param name="arg2">邀请QQ</param>
        /// <param name="arg3">来源群号</param>
        /// </summary>
        [ProxyAPIName("邀请好友加入讨论组_批量")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_201(string authCode, long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 邀请好友加入讨论组_批量");
            }
            return false;
        }

        /// <summary>
        /// 取框架到期时间
        /// </summary>
        [ProxyAPIName("取框架到期时间")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_202(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架到期时间");
            }
            return "";
        }

        /// <summary>
        /// 讨论组口令红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组口令红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_203(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组口令红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组拼手气红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组拼手气红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_204(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组拼手气红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组普通红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_205(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组普通红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组画图红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">题目名</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组画图红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_206(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组画图红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组语音红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">语音口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组语音红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_207(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组语音红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组接龙红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">接龙内容</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组接龙红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_208(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组接龙红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">领取人</param>
        /// <param name="arg5">祝福语</param>
        /// <param name="arg6">是否均分</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组专属红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_209(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, bool arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组专属红包");
            }
            return "";
        }

        /// <summary>
        /// 领取讨论组专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源讨论组Id</param>
        /// <param name="arg2">来源QQ</param>
        /// <param name="arg3">红包文本代码</param>
        /// </summary>
        [ProxyAPIName("领取讨论组专属红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_210(string authCode, long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 领取讨论组专属红包");
            }
            return "";
        }

        /// <summary>
        /// 取讨论组未领红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取讨论组未领红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_211(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组未领红包");
            }
            return 0;
        }

        /// <summary>
        /// 取讨论组文件下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">文件id</param>
        /// <param name="arg3">文件名</param>
        /// </summary>
        [ProxyAPIName("取讨论组文件下载地址")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_212(string authCode, long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组文件下载地址");
            }
            return "";
        }

        /// <summary>
        /// 发送QQ咨询会话
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">消息内容</param>
        /// <param name="arg3">消息Random</param>
        /// <param name="arg4">消息Req</param>
        /// </summary>
        [ProxyAPIName("发送QQ咨询会话")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_213(string authCode, long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送QQ咨询会话");
            }
            return "";
        }

        /// <summary>
        /// 创建群聊
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">邀请QQ</param>
        /// <param name="arg2">来源群号</param>
        /// <param name="arg3">新群群号</param>
        /// </summary>
        [ProxyAPIName("创建群聊")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_214(string authCode, long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建群聊");
            }
            return "";
        }

        /// <summary>
        /// 取群应用列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取群应用列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_215(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群应用列表");
            }
            return 0;
        }

        /// <summary>
        /// 退出讨论组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// </summary>
        [ProxyAPIName("退出讨论组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_216(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 退出讨论组");
            }
            return false;
        }

        /// <summary>
        /// 群验证消息接收设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">接收验证消息</param>
        /// </summary>
        [ProxyAPIName("群验证消息接收设置")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_217(string authCode, long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群验证消息接收设置");
            }
            return false;
        }

        /// <summary>
        /// 转让群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// </summary>
        [ProxyAPIName("转让群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_218(string authCode, long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 转让群");
            }
            return false;
        }

        /// <summary>
        /// 修改好友备注
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">备注</param>
        /// </summary>
        [ProxyAPIName("修改好友备注")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_219(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改好友备注");
            }
            return false;
        }

        /// <summary>
        /// 删除讨论组成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">对方QQ</param>
        /// </summary>
        [ProxyAPIName("删除讨论组成员")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_220(string authCode, long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除讨论组成员");
            }
            return false;
        }

        /// <summary>
        /// 讨论组文件转发至群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源讨论组Id</param>
        /// <param name="arg2">目标群号</param>
        /// <param name="arg3">fileId</param>
        /// <param name="arg4">filename</param>
        /// <param name="arg5">filesize</param>
        /// </summary>
        [ProxyAPIName("讨论组文件转发至群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_221(string authCode, long arg0, long arg1, long arg2, string arg3, string arg4, long arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组文件转发至群");
            }
            return false;
        }

        /// <summary>
        /// 讨论组文件转发至好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源讨论组Id</param>
        /// <param name="arg2">目标QQ</param>
        /// <param name="arg3">fileId</param>
        /// <param name="arg4">filename</param>
        /// <param name="arg5">filesize</param>
        /// <param name="arg6">Req</param>
        /// <param name="arg7">Random</param>
        /// <param name="arg8">time</param>
        /// </summary>
        [ProxyAPIName("讨论组文件转发至好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_222(string authCode, long arg0, long arg1, long arg2, string arg3, string arg4, long arg5, int arg6, long arg7, int arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组文件转发至好友");
            }
            return false;
        }

        /// <summary>
        /// 取QQ头像
        /// <param name="arg0">对方QQ</param>
        /// <param name="arg1">高清原图</param>
        /// </summary>
        [ProxyAPIName("取QQ头像")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_223(string authCode, long arg0, bool arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ头像");
            }
            return "";
        }

        /// <summary>
        /// 取群头像
        /// <param name="arg0">目标群号</param>
        /// </summary>
        [ProxyAPIName("取群头像")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_224(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群头像");
            }
            return "";
        }

        /// <summary>
        /// 取大表情图片下载地址
        /// <param name="arg0">大表情文本代码</param>
        /// <param name="arg1">长</param>
        /// <param name="arg2">宽</param>
        /// </summary>
        [ProxyAPIName("取大表情图片下载地址")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_225(string authCode, string arg0, int arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取大表情图片下载地址");
            }
            return "";
        }

        /// <summary>
        /// 拉起群收款
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">待付款成员</param>
        /// <param name="arg3">收款留言</param>
        /// <param name="arg4">收款订单号</param>
        /// </summary>
        [ProxyAPIName("拉起群收款")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_226(string authCode, long arg0, long arg1, object arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 拉起群收款");
            }
            return "";
        }

        /// <summary>
        /// 结束群收款
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款订单号</param>
        /// </summary>
        [ProxyAPIName("结束群收款")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_227(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 结束群收款");
            }
            return "";
        }

        /// <summary>
        /// 查询群收款状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款订单号</param>
        /// <param name="arg2">收款数据</param>
        /// </summary>
        [ProxyAPIName("查询群收款状态")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_228(string authCode, long arg0, string arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询群收款状态");
            }
            return "";
        }

        /// <summary>
        /// 支付群收款
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款发起人</param>
        /// <param name="arg2">收款订单号</param>
        /// <param name="arg3">支付金额</param>
        /// <param name="arg4">支付密码</param>
        /// <param name="arg5">银行卡序列</param>
        /// <param name="arg6">验证码信息</param>
        /// </summary>
        [ProxyAPIName("支付群收款")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_229(string authCode, long arg0, long arg1, string arg2, int arg3, string arg4, int arg5, object arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 支付群收款");
            }
            return "";
        }

        /// <summary>
        /// 消息合并转发至讨论组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">聊天记录</param>
        /// <param name="arg3">消息记录来源</param>
        /// </summary>
        [ProxyAPIName("消息合并转发至讨论组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_230(string authCode, long arg0, long arg1, object arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 消息合并转发至讨论组");
            }
            return "";
        }

        /// <summary>
        /// 群收款_催单
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款订单号</param>
        /// </summary>
        [ProxyAPIName("群收款_催单")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_231(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群收款_催单");
            }
            return "";
        }

        /// <summary>
        /// 取好友Diy名片数据
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">Diy名片数据</param>
        /// </summary>
        [ProxyAPIName("取好友Diy名片数据")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_232(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友Diy名片数据");
            }
            return false;
        }

        /// <summary>
        /// 设置Diy名片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">Diy名片数据</param>
        /// </summary>
        [ProxyAPIName("设置Diy名片")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_233(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置Diy名片");
            }
            return "";
        }

        /// <summary>
        /// 取框架主窗口句柄
        /// </summary>
        [ProxyAPIName("取框架主窗口句柄")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_234(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架主窗口句柄");
            }
            return 0;
        }

        /// <summary>
        /// 好友生僻字红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">生僻字</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友生僻字红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_235(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友生僻字红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊生僻字红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">生僻字</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊生僻字红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_236(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊生僻字红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组生僻字红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">生僻字</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组生僻字红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_237(string authCode, long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组生僻字红包");
            }
            return "";
        }

        /// <summary>
        /// 支付代付请求
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">代付订单号</param>
        /// <param name="arg2">支付金额</param>
        /// <param name="arg3">支付密码</param>
        /// <param name="arg4">银行卡序列</param>
        /// <param name="arg5">验证码信息</param>
        /// </summary>
        [ProxyAPIName("支付代付请求")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_238(string authCode, long arg0, string arg1, int arg2, string arg3, int arg4, object arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 支付代付请求");
            }
            return "";
        }

        /// <summary>
        /// 查询代付状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">代付订单号</param>
        /// <param name="arg2">代付数据</param>
        /// </summary>
        [ProxyAPIName("查询代付状态")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_239(string authCode, long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询代付状态");
            }
            return "";
        }

        /// <summary>
        /// 拉起代付
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">订单号</param>
        /// <param name="arg2">代付QQ列表</param>
        /// </summary>
        [ProxyAPIName("拉起代付")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_240(string authCode, long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 拉起代付");
            }
            return "";
        }

        /// <summary>
        /// 取好友能量值与QID
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">能量值</param>
        /// <param name="arg3">QID</param>
        /// </summary>
        [ProxyAPIName("取好友能量值与QID")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_241(string authCode, long arg0, long arg1, int arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友能量值与QID");
            }
            return false;
        }

        /// <summary>
        /// 创建小栗子文本代码解析类对象
        /// </summary>
        [ProxyAPIName("创建小栗子文本代码解析类对象")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static IntPtr Function_242(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 创建小栗子文本代码解析类对象");
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 文字转语音
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">文本内容</param>
        /// <param name="arg2">语音结果</param>
        /// </summary>
        [ProxyAPIName("文字转语音")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_243(string authCode, long arg0, string arg1, byte[] arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 文字转语音");
            }
            return "";
        }

        /// <summary>
        /// 翻译
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">源语言语种</param>
        /// <param name="arg2">目标语言语种</param>
        /// <param name="arg3">原文</param>
        /// <param name="arg4">翻译结果</param>
        /// </summary>
        [ProxyAPIName("翻译")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_244(string authCode, long arg0, string arg1, string arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 翻译");
            }
            return "";
        }

        /// <summary>
        /// 撤回消息_群聊s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">消息Random</param>
        /// <param name="arg3">消息Req</param>
        /// </summary>
        [ProxyAPIName("撤回消息_群聊s")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_245(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回消息_群聊s");
            }
            return "";
        }

        /// <summary>
        /// QQ列表_添加手表协议QQ
        /// <param name="arg0">QQ</param>
        /// <param name="arg1">品牌</param>
        /// <param name="arg2">型号</param>
        /// </summary>
        [ProxyAPIName("QQ列表_添加手表协议QQ")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_246(string authCode, long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_添加手表协议QQ");
            }
            return "";
        }

        /// <summary>
        /// QQ列表_二维码登录_拉取二维码
        /// <param name="arg0">QQ</param>
        /// <param name="arg1">二维码数据</param>
        /// </summary>
        [ProxyAPIName("QQ列表_二维码登录_拉取二维码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_247(string authCode, long arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_二维码登录_拉取二维码");
            }
            return "";
        }

        /// <summary>
        /// QQ列表_二维码登录_查询二维码状态
        /// <param name="arg0">QQ</param>
        /// </summary>
        [ProxyAPIName("QQ列表_二维码登录_查询二维码状态")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_248(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_二维码登录_查询二维码状态");
            }
            return "";
        }

        /// <summary>
        /// 拍一拍好友在线状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("拍一拍好友在线状态")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_249(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 拍一拍好友在线状态");
            }
            return false;
        }

        /// <summary>
        /// 发送验证消息会话消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">消息内容</param>
        /// <param name="arg3">消息Random</param>
        /// <param name="arg4">消息Req</param>
        /// </summary>
        [ProxyAPIName("发送验证消息会话消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_250(string authCode, long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送验证消息会话消息");
            }
            return "";
        }

        /// <summary>
        /// 回复验证消息会话消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">会话Token</param>
        /// <param name="arg3">消息内容</param>
        /// <param name="arg4">消息Random</param>
        /// <param name="arg5">消息Req</param>
        /// </summary>
        [ProxyAPIName("回复验证消息会话消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_251(string authCode, long arg0, long arg1, byte[] arg2, string arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 回复验证消息会话消息");
            }
            return "";
        }

        /// <summary>
        /// 取群文件内存利用状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">已使用容量</param>
        /// <param name="arg3">总容量</param>
        /// </summary>
        [ProxyAPIName("取群文件内存利用状态")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_252(string authCode, long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群文件内存利用状态");
            }
            return "";
        }

        /// <summary>
        /// 取群文件总数
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">已上传文件数</param>
        /// <param name="arg3">文件数量上限</param>
        /// </summary>
        [ProxyAPIName("取群文件总数")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_253(string authCode, long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群文件总数");
            }
            return "";
        }

        /// <summary>
        /// 上传涂鸦
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">模型Id</param>
        /// <param name="arg2">涂鸦数据</param>
        /// </summary>
        [ProxyAPIName("上传涂鸦")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_254(string authCode, long arg0, int arg1, byte[] arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传涂鸦");
            }
            return "";
        }

        /// <summary>
        /// 删除群成员_批量s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">欲移除群成员列表</param>
        /// </summary>
        [ProxyAPIName("删除群成员_批量s")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_255(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群成员_批量s");
            }
            return false;
        }

        /// <summary>
        /// 上传好友文件s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">文件路径</param>
        /// <param name="arg3">上传进度回调函数</param>
        /// <param name="arg4">Random</param>
        /// <param name="arg5">Req</param>
        /// </summary>
        [ProxyAPIName("上传好友文件s")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_256(string authCode, long arg0, long arg1, string arg2, int arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传好友文件s");
            }
            return "";
        }

        /// <summary>
        /// 上传群文件s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件路径</param>
        /// <param name="arg3">文件夹名</param>
        /// <param name="arg4">上传进度回调函数</param>
        /// </summary>
        [ProxyAPIName("上传群文件s")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_257(string authCode, long arg0, long arg1, string arg2, string arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群文件s");
            }
            return "";
        }

        /// <summary>
        /// 取群艾特全体剩余次数
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取群艾特全体剩余次数")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_258(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群艾特全体剩余次数");
            }
            return 0;
        }

        /// <summary>
        /// 是否已开启QQ咨询
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("是否已开启QQ咨询")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_259(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 是否已开启QQ咨询");
            }
            return 0;
        }

        /// <summary>
        /// 创建群相册
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册名</param>
        /// <param name="arg3">相册描述</param>
        /// </summary>
        [ProxyAPIName("创建群相册")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_260(string authCode, long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建群相册");
            }
            return "";
        }

        /// <summary>
        /// 删除群相册
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册Id</param>
        /// </summary>
        [ProxyAPIName("删除群相册")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_261(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群相册");
            }
            return "";
        }

        /// <summary>
        /// 取群相册列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取群相册列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_262(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群相册列表");
            }
            return "";
        }

        /// <summary>
        /// 取群相册照片列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册Id</param>
        /// <param name="arg3">获取数量</param>
        /// </summary>
        [ProxyAPIName("取群相册照片列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_263(string authCode, long arg0, long arg1, string arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群相册照片列表");
            }
            return "";
        }

        /// <summary>
        /// 删除群相册照片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册Id</param>
        /// <param name="arg3">照片Id</param>
        /// </summary>
        [ProxyAPIName("删除群相册照片")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_264(string authCode, long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群相册照片");
            }
            return "";
        }

        /// <summary>
        /// 修改群相册信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册Id</param>
        /// <param name="arg3">相册名</param>
        /// <param name="arg4">相册描述</param>
        /// <param name="arg5">相册置顶</param>
        /// </summary>
        [ProxyAPIName("修改群相册信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_265(string authCode, long arg0, long arg1, string arg2, string arg3, string arg4, bool arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改群相册信息");
            }
            return "";
        }

        /// <summary>
        /// 取群Id_从缓存
        /// <param name="arg0">群号</param>
        /// </summary>
        [ProxyAPIName("取群Id_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static long Function_266(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群Id_从缓存");
            }
            return 0;
        }

        /// <summary>
        /// 上传频道图片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">pic</param>
        /// <param name="arg4">宽度</param>
        /// <param name="arg5">高度</param>
        /// <param name="arg6">动图</param>
        /// </summary>
        [ProxyAPIName("上传频道图片")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_267(string authCode, long arg0, long arg1, long arg2, byte[] arg3, int arg4, int arg5, bool arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传频道图片");
            }
            return "";
        }

        /// <summary>
        /// 发送频道消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息内容</param>
        /// </summary>
        [ProxyAPIName("发送频道消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_268(string authCode, long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送频道消息");
            }
            return "";
        }

        /// <summary>
        /// 发送频道私信消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">私信频道Id</param>
        /// <param name="arg2">私信子频道Id</param>
        /// <param name="arg3">消息内容</param>
        /// </summary>
        [ProxyAPIName("发送频道私信消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_269(string authCode, long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送频道私信消息");
            }
            return "";
        }

        /// <summary>
        /// 取私信频道Id
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">目标频道用户Id</param>
        /// <param name="arg3">私信频道Id</param>
        /// <param name="arg4">私信子频道Id</param>
        /// </summary>
        [ProxyAPIName("取私信频道Id")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_270(string authCode, long arg0, long arg1, long arg2, long arg3, long arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取私信频道Id");
            }
            return false;
        }

        /// <summary>
        /// 频道消息粘贴表情
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// <param name="arg4">表情Id</param>
        /// <param name="arg5">是否为emoji</param>
        /// <param name="arg6">取消粘贴</param>
        /// </summary>
        [ProxyAPIName("频道消息粘贴表情")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_271(string authCode, long arg0, long arg1, long arg2, int arg3, string arg4, bool arg5, bool arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道消息粘贴表情");
            }
            return false;
        }

        /// <summary>
        /// 撤回频道消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// </summary>
        [ProxyAPIName("撤回频道消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_272(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回频道消息");
            }
            return "";
        }

        /// <summary>
        /// 撤回频道私信消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">私信频道Id</param>
        /// <param name="arg2">私信子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// </summary>
        [ProxyAPIName("撤回频道私信消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_273(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回频道私信消息");
            }
            return "";
        }

        /// <summary>
        /// 设置子频道精华消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// <param name="arg4">移除</param>
        /// </summary>
        [ProxyAPIName("设置子频道精华消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_274(string authCode, long arg0, long arg1, long arg2, int arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置子频道精华消息");
            }
            return false;
        }

        /// <summary>
        /// 禁言频道成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id</param>
        /// <param name="arg3">禁言秒数</param>
        /// </summary>
        [ProxyAPIName("禁言频道成员")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_275(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 禁言频道成员");
            }
            return false;
        }

        /// <summary>
        /// 设置频道全员禁言
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">禁言秒数</param>
        /// </summary>
        [ProxyAPIName("设置频道全员禁言")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_276(string authCode, long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置频道全员禁言");
            }
            return false;
        }

        /// <summary>
        /// 移除频道成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id</param>
        /// <param name="arg3">拉入黑名单</param>
        /// </summary>
        [ProxyAPIName("移除频道成员")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_277(string authCode, long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 移除频道成员");
            }
            return false;
        }

        /// <summary>
        /// 移除频道成员_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id列表</param>
        /// <param name="arg3">拉入黑名单</param>
        /// </summary>
        [ProxyAPIName("移除频道成员_批量")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_278(string authCode, long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 移除频道成员_批量");
            }
            return false;
        }

        /// <summary>
        /// 退出频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// </summary>
        [ProxyAPIName("退出频道")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_279(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 退出频道");
            }
            return false;
        }

        /// <summary>
        /// 更改频道名称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">新名称</param>
        /// <param name="arg3">字色</param>
        /// </summary>
        [ProxyAPIName("更改频道名称")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_280(string authCode, long arg0, long arg1, string arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 更改频道名称");
            }
            return false;
        }

        /// <summary>
        /// 修改频道简介
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">新简介</param>
        /// </summary>
        [ProxyAPIName("修改频道简介")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_281(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改频道简介");
            }
            return false;
        }

        /// <summary>
        /// 设置我的频道昵称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道昵称</param>
        /// </summary>
        [ProxyAPIName("设置我的频道昵称")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_282(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置我的频道昵称");
            }
            return false;
        }

        /// <summary>
        /// 置子频道观看权限
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">类型</param>
        /// <param name="arg4">指定身份组Id</param>
        /// <param name="arg5">是否取消身份组观看权限</param>
        /// <param name="arg6">指定频道成员Id</param>
        /// <param name="arg7">是否取消频道成员观看权限</param>
        /// </summary>
        [ProxyAPIName("置子频道观看权限")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_283(string authCode, long arg0, long arg1, long arg2, int arg3, long arg4, bool arg5, long arg6, bool arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置子频道观看权限");
            }
            return false;
        }

        /// <summary>
        /// 置子频道发言权限
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">类型</param>
        /// <param name="arg4">指定身份组Id</param>
        /// <param name="arg5">是否取消身份组发言权限</param>
        /// <param name="arg6">指定频道成员</param>
        /// <param name="arg7">是否取消频道成员发言权限</param>
        /// </summary>
        [ProxyAPIName("置子频道发言权限")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_284(string authCode, long arg0, long arg1, long arg2, int arg3, long arg4, bool arg5, long arg6, bool arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置子频道发言权限");
            }
            return false;
        }

        /// <summary>
        /// 子频道消息提醒设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">对成员打开消息提醒</param>
        /// </summary>
        [ProxyAPIName("子频道消息提醒设置")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_285(string authCode, long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 子频道消息提醒设置");
            }
            return false;
        }

        /// <summary>
        /// 子频道慢速模式设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">模式</param>
        /// </summary>
        [ProxyAPIName("子频道慢速模式设置")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_286(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 子频道慢速模式设置");
            }
            return false;
        }

        /// <summary>
        /// 修改子频道名称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">新名称</param>
        /// </summary>
        [ProxyAPIName("修改子频道名称")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_287(string authCode, long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改子频道名称");
            }
            return false;
        }

        /// <summary>
        /// 删除子频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// </summary>
        [ProxyAPIName("删除子频道")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_288(string authCode, long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除子频道");
            }
            return false;
        }

        /// <summary>
        /// 修改我的频道用户信息_昵称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">新昵称</param>
        /// </summary>
        [ProxyAPIName("修改我的频道用户信息_昵称")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_289(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改我的频道用户信息_昵称");
            }
            return false;
        }

        /// <summary>
        /// 修改我的频道用户信息_性别
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">性别</param>
        /// </summary>
        [ProxyAPIName("修改我的频道用户信息_性别")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_290(string authCode, long arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改我的频道用户信息_性别");
            }
            return false;
        }

        /// <summary>
        /// 修改我的频道用户信息_年龄
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">年龄</param>
        /// </summary>
        [ProxyAPIName("修改我的频道用户信息_年龄")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_291(string authCode, long arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改我的频道用户信息_年龄");
            }
            return false;
        }

        /// <summary>
        /// 修改我的频道用户信息_所在地
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">国家代码</param>
        /// <param name="arg2">国家名称</param>
        /// <param name="arg3">省份代码</param>
        /// <param name="arg4">省份名称</param>
        /// <param name="arg5">市区代码</param>
        /// <param name="arg6">市区名称</param>
        /// </summary>
        [ProxyAPIName("修改我的频道用户信息_所在地")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_292(string authCode, long arg0, int arg1, string arg2, int arg3, string arg4, int arg5, string arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改我的频道用户信息_所在地");
            }
            return false;
        }

        /// <summary>
        /// 设置是否允许别人私信我
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">不允许</param>
        /// </summary>
        [ProxyAPIName("设置是否允许别人私信我")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_293(string authCode, long arg0, bool arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置是否允许别人私信我");
            }
            return false;
        }

        /// <summary>
        /// 设置频道加入验证方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">验证方式</param>
        /// <param name="arg3">问题</param>
        /// <param name="arg4">答案</param>
        /// </summary>
        [ProxyAPIName("设置频道加入验证方式")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_294(string authCode, long arg0, long arg1, int arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置频道加入验证方式");
            }
            return false;
        }

        /// <summary>
        /// 搜索频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">关键词</param>
        /// <param name="arg2">第几页</param>
        /// <param name="arg3">结果</param>
        /// </summary>
        [ProxyAPIName("搜索频道")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_295(string authCode, long arg0, string arg1, int arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 搜索频道");
            }
            return 0;
        }

        /// <summary>
        /// 取频道封面
        /// <param name="arg0">频道Id</param>
        /// </summary>
        [ProxyAPIName("取频道封面")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_296(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道封面");
            }
            return "";
        }

        /// <summary>
        /// 取频道头像
        /// <param name="arg0">频道Id</param>
        /// <param name="arg1">高清大图</param>
        /// </summary>
        [ProxyAPIName("取频道头像")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_297(string authCode, long arg0, bool arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道头像");
            }
            return "";
        }

        /// <summary>
        /// 获取频道成员列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// <param name="arg3">翻页数据</param>
        /// <param name="arg4">翻页信息</param>
        /// </summary>
        [ProxyAPIName("获取频道成员列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_298(string authCode, long arg0, long arg1, object arg2, int arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取频道成员列表");
            }
            return 0;
        }

        /// <summary>
        /// 取频道信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取频道信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_299(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道信息");
            }
            return false;
        }

        /// <summary>
        /// 取频道加入验证方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">验证方式</param>
        /// <param name="arg3">问题</param>
        /// </summary>
        [ProxyAPIName("取频道加入验证方式")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_300(string authCode, long arg0, long arg1, int arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道加入验证方式");
            }
            return false;
        }

        /// <summary>
        /// 申请加入频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">验证方式</param>
        /// <param name="arg2">频道Id</param>
        /// <param name="arg3">频道Token</param>
        /// <param name="arg4">答案</param>
        /// </summary>
        [ProxyAPIName("申请加入频道")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_301(string authCode, long arg0, int arg1, long arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 申请加入频道");
            }
            return "";
        }

        /// <summary>
        /// 取频道文件下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">fileid</param>
        /// <param name="arg4">filename</param>
        /// </summary>
        [ProxyAPIName("取频道文件下载地址")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_302(string authCode, long arg0, long arg1, long arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道文件下载地址");
            }
            return "";
        }

        /// <summary>
        /// 频道拼手气红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">频道Id</param>
        /// <param name="arg4">子频道Id</param>
        /// <param name="arg5">祝福语</param>
        /// <param name="arg6">红包皮肤Id</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("频道拼手气红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_303(string authCode, long arg0, int arg1, int arg2, long arg3, long arg4, string arg5, int arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道拼手气红包");
            }
            return "";
        }

        /// <summary>
        /// 频道普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">频道Id</param>
        /// <param name="arg4">子频道Id</param>
        /// <param name="arg5">祝福语</param>
        /// <param name="arg6">红包皮肤Id</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("频道普通红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_304(string authCode, long arg0, int arg1, int arg2, long arg3, long arg4, string arg5, int arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道普通红包");
            }
            return "";
        }

        /// <summary>
        /// 频道专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">频道Id</param>
        /// <param name="arg4">子频道Id</param>
        /// <param name="arg5">领取人频道用户Id</param>
        /// <param name="arg6">祝福语</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("频道专属红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_305(string authCode, long arg0, int arg1, int arg2, long arg3, long arg4, long arg5, string arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道专属红包");
            }
            return "";
        }

        /// <summary>
        /// 领取频道专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">发送人频道用户Id</param>
        /// <param name="arg4">红包文本代码</param>
        /// </summary>
        [ProxyAPIName("领取频道专属红包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_306(string authCode, long arg0, long arg1, long arg2, long arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 领取频道专属红包");
            }
            return "";
        }

        /// <summary>
        /// 取频道成员身份组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id</param>
        /// <param name="arg3">结果</param>
        /// </summary>
        [ProxyAPIName("取频道成员身份组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_307(string authCode, long arg0, long arg1, long arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道成员身份组");
            }
            return 0;
        }

        /// <summary>
        /// 设置频道成员身份组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id</param>
        /// <param name="arg3">身份组Id列表</param>
        /// <param name="arg4">是否取消身份组</param>
        /// </summary>
        [ProxyAPIName("设置频道成员身份组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_308(string authCode, long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置频道成员身份组");
            }
            return false;
        }

        /// <summary>
        /// 修改身份组信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// <param name="arg3">身份组名</param>
        /// <param name="arg4">身份组外显颜色代码</param>
        /// <param name="arg5">是否在成员列表中单独展示</param>
        /// </summary>
        [ProxyAPIName("修改身份组信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_309(string authCode, long arg0, long arg1, long arg2, string arg3, long arg4, bool arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改身份组信息");
            }
            return false;
        }

        /// <summary>
        /// 删除身份组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// </summary>
        [ProxyAPIName("删除身份组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_310(string authCode, long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除身份组");
            }
            return false;
        }

        /// <summary>
        /// 新增身份组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组名</param>
        /// <param name="arg3">身份组外显颜色代码</param>
        /// <param name="arg4">是否在成员列表中单独展示</param>
        /// </summary>
        [ProxyAPIName("新增身份组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static long Function_311(string authCode, long arg0, long arg1, string arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 新增身份组");
            }
            return 0;
        }

        /// <summary>
        /// 取频道身份组列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取频道身份组列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_312(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道身份组列表");
            }
            return 0;
        }

        /// <summary>
        /// 取子频道列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取子频道列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_313(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道列表");
            }
            return 0;
        }

        /// <summary>
        /// 取频道用户个性档案
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取频道用户个性档案")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_314(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道用户个性档案");
            }
            return 0;
        }

        /// <summary>
        /// 取频道用户资料
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取频道用户资料")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_315(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道用户资料");
            }
            return false;
        }

        /// <summary>
        /// 刷新频道列表缓存
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("刷新频道列表缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_316(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 刷新频道列表缓存");
            }
            return false;
        }

        /// <summary>
        /// 取频道列表_从缓存
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取频道列表_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_317(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道列表_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取频道用户昵称_从缓存
        /// <param name="arg0">频道用户Id</param>
        /// </summary>
        [ProxyAPIName("取频道用户昵称_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_318(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道用户昵称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取频道名称_从缓存
        /// <param name="arg0">频道Id</param>
        /// </summary>
        [ProxyAPIName("取频道名称_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_319(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道名称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取子频道名称_从缓存
        /// <param name="arg0">频道Id</param>
        /// <param name="arg1">子频道Id</param>
        /// </summary>
        [ProxyAPIName("取子频道名称_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_320(string authCode, string arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道名称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取频道昵称_从缓存
        /// <param name="arg0">频道Id</param>
        /// <param name="arg1">频道用户Id</param>
        /// </summary>
        [ProxyAPIName("取频道昵称_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_321(string authCode, string arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道昵称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取子频道分组列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取子频道分组列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_322(string authCode, long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道分组列表");
            }
            return 0;
        }

        /// <summary>
        /// 取私信频道列表_从缓存
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取私信频道列表_从缓存")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_323(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取私信频道列表_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 上传频道文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">文件路径</param>
        /// <param name="arg4">上传进度回调函数</param>
        /// <param name="arg5">图片宽度</param>
        /// <param name="arg6">图片高度</param>
        /// </summary>
        [ProxyAPIName("上传频道文件")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_324(string authCode, long arg0, long arg1, long arg2, string arg3, int arg4, int arg5, int arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传频道文件");
            }
            return "";
        }

        /// <summary>
        /// 更改频道消息内容
        /// <param name="arg0">数据指针</param>
        /// <param name="arg1">新消息内容</param>
        /// </summary>
        [ProxyAPIName("更改频道消息内容")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_325(string authCode, int arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 更改频道消息内容");
            }
            return false;
        }

        /// <summary>
        /// Emoji转频道EmojiId
        /// <param name="arg0">Emoji代码</param>
        /// </summary>
        [ProxyAPIName("Emoji转频道EmojiId")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_326(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API Emoji转频道EmojiId");
            }
            return "";
        }

        /// <summary>
        /// 频道EmojiId转Emoji
        /// <param name="arg0">频道EmojiId</param>
        /// </summary>
        [ProxyAPIName("频道EmojiId转Emoji")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_327(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道EmojiId转Emoji");
            }
            return "";
        }

        /// <summary>
        /// Emoji转QQ空间EmId
        /// <param name="arg0">Emoji代码</param>
        /// </summary>
        [ProxyAPIName("Emoji转QQ空间EmId")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_328(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API Emoji转QQ空间EmId");
            }
            return "";
        }

        /// <summary>
        /// QQ空间EmId转Emoji
        /// <param name="arg0">QQ空间EmId</param>
        /// </summary>
        [ProxyAPIName("QQ空间EmId转Emoji")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_329(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ空间EmId转Emoji");
            }
            return "";
        }

        /// <summary>
        /// 小黄豆Id转QQ空间EmId
        /// <param name="arg0">小黄豆Id</param>
        /// </summary>
        [ProxyAPIName("小黄豆Id转QQ空间EmId")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_330(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 小黄豆Id转QQ空间EmId");
            }
            return "";
        }

        /// <summary>
        /// QQ空间EmId转小黄豆Id
        /// <param name="arg0">QQ空间EmId</param>
        /// </summary>
        [ProxyAPIName("QQ空间EmId转小黄豆Id")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_331(string authCode, string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ空间EmId转小黄豆Id");
            }
            return "";
        }

        /// <summary>
        /// 取特定身份组成员列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// <param name="arg3">结果</param>
        /// <param name="arg4">开始位置</param>
        /// </summary>
        [ProxyAPIName("取特定身份组成员列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_332(string authCode, long arg0, long arg1, long arg2, object arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取特定身份组成员列表");
            }
            return 0;
        }

        /// <summary>
        /// 取子频道分组结构
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// </summary>
        [ProxyAPIName("取子频道分组结构")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_333(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道分组结构");
            }
            return "";
        }

        /// <summary>
        /// 设置子频道分组结构
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道分组结构</param>
        /// </summary>
        [ProxyAPIName("设置子频道分组结构")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_334(string authCode, long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置子频道分组结构");
            }
            return "";
        }

        /// <summary>
        /// 删除子频道_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id列表</param>
        /// </summary>
        [ProxyAPIName("删除子频道_批量")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_335(string authCode, long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除子频道_批量");
            }
            return false;
        }

        /// <summary>
        /// 创建子频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">原始子频道分组结构</param>
        /// <param name="arg3">子频道名称</param>
        /// <param name="arg4">消息提醒设置</param>
        /// <param name="arg5">子频道类型</param>
        /// <param name="arg6">子频道子类型</param>
        /// <param name="arg7">应用Id</param>
        /// <param name="arg8">可视类型</param>
        /// <param name="arg9">指定成员频道用户Id列表</param>
        /// <param name="arg10">指定身份组Id</param>
        /// <param name="arg11">所属分组index</param>
        /// <param name="arg12">分组内位置</param>
        /// </summary>
        [ProxyAPIName("创建子频道")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_336(string authCode, long arg0, long arg1, string arg2, string arg3, int arg4, int arg5, int arg6, int arg7, int arg8, long arg9, long arg10, int arg11, int arg12)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, arg10={arg10}, arg11={arg11}, arg12={arg12}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建子频道");
            }
            return false;
        }

        /// <summary>
        /// 构造卡片消息文本代码
        /// <param name="arg0">卡片代码</param>
        /// <param name="arg1">类型</param>
        /// <param name="arg2">强制发送</param>
        /// </summary>
        [ProxyAPIName("构造卡片消息文本代码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_337(string authCode, string arg0, int arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 构造卡片消息文本代码");
            }
            return "";
        }

        /// <summary>
        /// 分享音乐_频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">歌曲名</param>
        /// <param name="arg4">歌手名</param>
        /// <param name="arg5">跳转地址</param>
        /// <param name="arg6">封面地址</param>
        /// <param name="arg7">文件地址</param>
        /// <param name="arg8">应用类型</param>
        /// </summary>
        [ProxyAPIName("分享音乐_频道")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_338(string authCode, long arg0, long arg1, long arg2, string arg3, string arg4, string arg5, string arg6, string arg7, int arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 分享音乐_频道");
            }
            return false;
        }

        /// <summary>
        /// 修改频道排序
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id列表</param>
        /// </summary>
        [ProxyAPIName("修改频道排序")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_339(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改频道排序");
            }
            return false;
        }

        /// <summary>
        /// 处理频道加入申请
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户标识</param>
        /// <param name="arg2">操作</param>
        /// </summary>
        [ProxyAPIName("处理频道加入申请")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_340(string authCode, long arg0, string arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 处理频道加入申请");
            }
            return false;
        }

        /// <summary>
        /// 查询群设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("查询群设置")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_341(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询群设置");
            }
            return "";
        }

        /// <summary>
        /// 取子频道管理列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">结果</param>
        /// </summary>
        [ProxyAPIName("取子频道管理列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_342(string authCode, long arg0, long arg1, long arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道管理列表");
            }
            return 0;
        }

        /// <summary>
        /// 设置子频道管理
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">频道用户Id列表</param>
        /// <param name="arg4">是否取消子频道管理</param>
        /// </summary>
        [ProxyAPIName("设置子频道管理")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_343(string authCode, long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置子频道管理");
            }
            return false;
        }

        /// <summary>
        /// 设置指定身份组子频道观看权限
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// <param name="arg3">欲设置的子频道Id列表</param>
        /// <param name="arg4">是否取消观看权限</param>
        /// </summary>
        [ProxyAPIName("设置指定身份组子频道观看权限")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_344(string authCode, long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置指定身份组子频道观看权限");
            }
            return false;
        }

        /// <summary>
        /// 设置指定身份组子频道发言权限
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// <param name="arg3">欲设置的子频道Id列表</param>
        /// <param name="arg4">是否取消发言权限</param>
        /// </summary>
        [ProxyAPIName("设置指定身份组子频道发言权限")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_345(string authCode, long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置指定身份组子频道发言权限");
            }
            return false;
        }

        /// <summary>
        /// 设置直播子频道主播
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">直播子频道Id</param>
        /// <param name="arg3">欲设置的频道用户Id列表</param>
        /// <param name="arg4">是否取消主播身份</param>
        /// </summary>
        [ProxyAPIName("设置直播子频道主播")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_346(string authCode, long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置直播子频道主播");
            }
            return false;
        }

        /// <summary>
        /// 获取频道分享链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// </summary>
        [ProxyAPIName("获取频道分享链接")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_347(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取频道分享链接");
            }
            return "";
        }

        /// <summary>
        /// 获取子频道分享链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// </summary>
        [ProxyAPIName("获取子频道分享链接")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_348(string authCode, long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取子频道分享链接");
            }
            return "";
        }

        /// <summary>
        /// 子频道消息通知设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">开启消息通知</param>
        /// </summary>
        [ProxyAPIName("子频道消息通知设置")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_349(string authCode, long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 子频道消息通知设置");
            }
            return false;
        }

        /// <summary>
        /// 获取红包领取详情s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">红包来源</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">红包文本代码</param>
        /// <param name="arg4">红包类型</param>
        /// <param name="arg5">起始位置</param>
        /// </summary>
        [ProxyAPIName("获取红包领取详情s")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_350(string authCode, long arg0, long arg1, long arg2, string arg3, int arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取红包领取详情s");
            }
            return "";
        }

        /// <summary>
        /// 取话题子频道帖子列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">话题子频道Id</param>
        /// <param name="arg3">结果</param>
        /// <param name="arg4">翻页信息</param>
        /// </summary>
        [ProxyAPIName("取话题子频道帖子列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_351(string authCode, long arg0, long arg1, long arg2, object arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取话题子频道帖子列表");
            }
            return 0;
        }

        /// <summary>
        /// 获取日程列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">时间戳</param>
        /// <param name="arg4">结果</param>
        /// </summary>
        [ProxyAPIName("获取日程列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_352(string authCode, long arg0, long arg1, long arg2, long arg3, object arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取日程列表");
            }
            return 0;
        }

        /// <summary>
        /// 获取日程链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">日程Id</param>
        /// </summary>
        [ProxyAPIName("获取日程链接")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_353(string authCode, long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取日程链接");
            }
            return "";
        }

        /// <summary>
        /// 取日程信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">日程Id</param>
        /// <param name="arg4">信息</param>
        /// </summary>
        [ProxyAPIName("取日程信息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_354(string authCode, long arg0, long arg1, long arg2, long arg3, object arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取日程信息");
            }
            return false;
        }

        /// <summary>
        /// 创建日程
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">日程名</param>
        /// <param name="arg4">日程描述</param>
        /// <param name="arg5">开始时间戳</param>
        /// <param name="arg6">结束时间戳</param>
        /// <param name="arg7">提醒</param>
        /// <param name="arg8">开始时跳转的子频道Id</param>
        /// <param name="arg9">信息</param>
        /// </summary>
        [ProxyAPIName("创建日程")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_355(string authCode, long arg0, long arg1, long arg2, string arg3, string arg4, long arg5, long arg6, int arg7, long arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建日程");
            }
            return false;
        }

        /// <summary>
        /// 取QQ头像K值
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取QQ头像K值")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_356(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ头像K值");
            }
            return "";
        }

        /// <summary>
        /// 删除日程
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">日程信息</param>
        /// </summary>
        [ProxyAPIName("删除日程")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_357(string authCode, long arg0, long arg1, long arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除日程");
            }
            return false;
        }

        /// <summary>
        /// 发送通行证到群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">群号</param>
        /// <param name="arg3">通行证数量</param>
        /// </summary>
        [ProxyAPIName("发送通行证到群")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_358(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送通行证到群");
            }
            return false;
        }

        /// <summary>
        /// 发送通行证到好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">对方QQ</param>
        /// </summary>
        [ProxyAPIName("发送通行证到好友")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_359(string authCode, long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送通行证到好友");
            }
            return false;
        }

        /// <summary>
        /// 屏蔽频道用户私信
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户Id</param>
        /// <param name="arg2">解除屏蔽</param>
        /// </summary>
        [ProxyAPIName("屏蔽频道用户私信")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_360(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 屏蔽频道用户私信");
            }
            return false;
        }

        /// <summary>
        /// 频道用户私信免打扰
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户Id</param>
        /// <param name="arg2">关闭免打扰</param>
        /// </summary>
        [ProxyAPIName("频道用户私信免打扰")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_361(string authCode, long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道用户私信免打扰");
            }
            return false;
        }

        /// <summary>
        /// QQ列表_添加QQ
        /// <param name="arg0">QQ</param>
        /// <param name="arg1">密码</param>
        /// <param name="arg2">手机品牌</param>
        /// <param name="arg3">手机型号</param>
        /// <param name="arg4">协议</param>
        /// <param name="arg5">guid</param>
        /// </summary>
        [ProxyAPIName("QQ列表_添加QQ")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_362(string authCode, long arg0, string arg1, string arg2, string arg3, int arg4, string arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_添加QQ");
            }
            return "";
        }

        /// <summary>
        /// QQ列表_删除QQ
        /// <param name="arg0">QQ</param>
        /// <param name="arg1">彻底删除</param>
        /// </summary>
        [ProxyAPIName("QQ列表_删除QQ")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_363(string authCode, long arg0, bool arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_删除QQ");
            }
            return "";
        }

        /// <summary>
        /// 登录指定QQ_二次登录
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("登录指定QQ_二次登录")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_364(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 登录指定QQ_二次登录");
            }
            return false;
        }

        /// <summary>
        /// 是否已设置QQ密码
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("是否已设置QQ密码")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_365(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 是否已设置QQ密码");
            }
            return false;
        }

        /// <summary>
        /// 取框架插件列表
        /// </summary>
        [ProxyAPIName("取框架插件列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_366(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架插件列表");
            }
            return "";
        }

        /// <summary>
        /// 取在线移动设备列表
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取在线移动设备列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_367(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取在线移动设备列表");
            }
            return "";
        }

        /// <summary>
        /// 设置频道全局公告_指定消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// </summary>
        [ProxyAPIName("设置频道全局公告_指定消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_368(string authCode, long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置频道全局公告_指定消息");
            }
            return false;
        }

        /// <summary>
        /// 取频道号
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// </summary>
        [ProxyAPIName("取频道号")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_369(string authCode, long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道号");
            }
            return "";
        }

        /// <summary>
        /// 设置位置共享s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标</param>
        /// <param name="arg2">经度</param>
        /// <param name="arg3">纬度</param>
        /// <param name="arg4">是否开启</param>
        /// <param name="arg5">类型</param>
        /// </summary>
        [ProxyAPIName("设置位置共享s")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_370(string authCode, long arg0, long arg1, double arg2, double arg3, bool arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置位置共享s");
            }
            return false;
        }

        /// <summary>
        /// 上报当前位置s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标</param>
        /// <param name="arg2">经度</param>
        /// <param name="arg3">纬度</param>
        /// <param name="arg4">指针偏角</param>
        /// </summary>
        [ProxyAPIName("上报当前位置s")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_371(string authCode, long arg0, long arg1, double arg2, double arg3, double arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上报当前位置s");
            }
            return false;
        }

        /// <summary>
        /// 移动好友分组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">分组Id</param>
        /// </summary>
        [ProxyAPIName("移动好友分组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_372(string authCode, long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 移动好友分组");
            }
            return false;
        }

        /// <summary>
        /// 修改好友分组名
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">分组Id</param>
        /// <param name="arg2">分组名</param>
        /// </summary>
        [ProxyAPIName("修改好友分组名")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_373(string authCode, long arg0, int arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改好友分组名");
            }
            return false;
        }

        /// <summary>
        /// 删除好友分组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">分组Id</param>
        /// </summary>
        [ProxyAPIName("删除好友分组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_374(string authCode, long arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除好友分组");
            }
            return false;
        }

        /// <summary>
        /// 取好友分组列表
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取好友分组列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_375(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友分组列表");
            }
            return "";
        }

        /// <summary>
        /// 新增好友分组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">分组名</param>
        /// </summary>
        [ProxyAPIName("新增好友分组")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static int Function_376(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 新增好友分组");
            }
            return 0;
        }

        /// <summary>
        /// 取频道红包pre_grap_token
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">红包listid</param>
        /// <param name="arg4">红包authkey</param>
        /// <param name="arg5">红包channel</param>
        /// <param name="arg6">红包发送人频道用户Id</param>
        /// </summary>
        [ProxyAPIName("取频道红包pre_grap_token")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_377(string authCode, long arg0, long arg1, long arg2, string arg3, string arg4, string arg5, long arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道红包pre_grap_token");
            }
            return "";
        }

        /// <summary>
        /// 语音红包匹配
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">红包接收对象</param>
        /// <param name="arg2">红包标题</param>
        /// <param name="arg3">匹配语音hash</param>
        /// <param name="arg4">红包listid</param>
        /// <param name="arg5">红包authkey</param>
        /// <param name="arg6">红包发送者QQ</param>
        /// <param name="arg7">红包来源类型</param>
        /// </summary>
        [ProxyAPIName("语音红包匹配")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_378(string authCode, long arg0, long arg1, string arg2, string arg3, string arg4, string arg5, long arg6, int arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 语音红包匹配");
            }
            return "";
        }

        /// <summary>
        /// 上传群聊语音红包匹配语音
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">红包来源群号</param>
        /// <param name="arg2">audio</param>
        /// </summary>
        [ProxyAPIName("上传群聊语音红包匹配语音")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_379(string authCode, long arg0, long arg1, byte[] arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群聊语音红包匹配语音");
            }
            return "";
        }

        /// <summary>
        /// 取合并转发消息内容
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">resId</param>
        /// </summary>
        [ProxyAPIName("取合并转发消息内容")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_380(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取合并转发消息内容");
            }
            return "";
        }

        /// <summary>
        /// 上传合并转发消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">消息来源</param>
        /// <param name="arg2">消息封面</param>
        /// <param name="arg3">合并转发消息内容</param>
        /// </summary>
        [ProxyAPIName("上传合并转发消息")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_381(string authCode, long arg0, string arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传合并转发消息");
            }
            return "";
        }

        /// <summary>
        /// 语音转文字
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">语音hash</param>
        /// <param name="arg2">语音token</param>
        /// </summary>
        [ProxyAPIName("语音转文字")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_382(string authCode, long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 语音转文字");
            }
            return "";
        }

        /// <summary>
        /// 发送功能包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">功能cmd</param>
        /// <param name="arg2">最大等待时长</param>
        /// <param name="arg3">数据</param>
        /// </summary>
        [ProxyAPIName("发送功能包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_383(string authCode, long arg0, string arg1, int arg2, byte[] arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送功能包");
            }
            return false;
        }

        /// <summary>
        /// 二维码扫一扫授权登录其他应用
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">k值</param>
        /// </summary>
        [ProxyAPIName("二维码扫一扫授权登录其他应用")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_384(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 二维码扫一扫授权登录其他应用");
            }
            return "";
        }

        /// <summary>
        /// 取历史登录设备guid列表
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取历史登录设备guid列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_385(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取历史登录设备guid列表");
            }
            return "";
        }

        /// <summary>
        /// 二维码扫一扫授权其他设备资料辅助验证登录
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">str_url</param>
        /// </summary>
        [ProxyAPIName("二维码扫一扫授权其他设备资料辅助验证登录")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_386(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 二维码扫一扫授权其他设备资料辅助验证登录");
            }
            return "";
        }

        /// <summary>
        /// 关闭设备锁
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("关闭设备锁")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_387(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 关闭设备锁");
            }
            return false;
        }

        /// <summary>
        /// 恢复设备锁
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("恢复设备锁")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_388(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 恢复设备锁");
            }
            return false;
        }

        /// <summary>
        /// 余额提现
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">提现金额</param>
        /// <param name="arg2">支付密码</param>
        /// <param name="arg3">银行卡序列</param>
        /// </summary>
        [ProxyAPIName("余额提现")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_389(string authCode, long arg0, int arg1, string arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 余额提现");
            }
            return "";
        }

        /// <summary>
        /// 取h5钱包cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取h5钱包cookie")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_390(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取h5钱包cookie");
            }
            return "";
        }

        /// <summary>
        /// 取QQ会员中心cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取QQ会员中心cookie")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_391(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ会员中心cookie");
            }
            return "";
        }

        /// <summary>
        /// 说说点赞
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">说说发布者QQ</param>
        /// <param name="arg2">说说feedskey</param>
        /// <param name="arg3">取消点赞</param>
        /// </summary>
        [ProxyAPIName("说说点赞")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_392(string authCode, long arg0, long arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 说说点赞");
            }
            return false;
        }

        /// <summary>
        /// 说说评论
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">说说发布者QQ</param>
        /// <param name="arg2">说说feedskey</param>
        /// <param name="arg3">评论内容</param>
        /// </summary>
        [ProxyAPIName("说说评论")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_393(string authCode, long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 说说评论");
            }
            return false;
        }

        /// <summary>
        /// 取最新动态列表
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取最新动态列表")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_394(string authCode, long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取最新动态列表");
            }
            return "";
        }

        /// <summary>
        /// 搜索表情包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">关键词</param>
        /// </summary>
        [ProxyAPIName("搜索表情包")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_395(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 搜索表情包");
            }
            return "";
        }

        /// <summary>
        /// 发布说说
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">内容</param>
        /// </summary>
        [ProxyAPIName("发布说说")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_396(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发布说说");
            }
            return "";
        }

        /// <summary>
        /// 经纬度定位查询详细地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">经度</param>
        /// <param name="arg2">纬度</param>
        /// </summary>
        [ProxyAPIName("经纬度定位查询详细地址")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_397(string authCode, long arg0, double arg1, double arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 经纬度定位查询详细地址");
            }
            return "";
        }

        /// <summary>
        /// 取插件自身版本号
        /// </summary>
        [ProxyAPIName("取插件自身版本号")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_398(string authCode)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取插件自身版本号");
            }
            return "";
        }

        /// <summary>
        /// 上传群临时文件s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">文件路径</param>
        /// <param name="arg5">上传进度回调函数</param>
        /// <param name="arg6">Random</param>
        /// <param name="arg7">Req</param>
        /// </summary>
        [ProxyAPIName("上传群临时文件s")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string Function_399(string authCode, long arg0, long arg1, long arg2, long arg3, string arg4, int arg5, long arg6, int arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群临时文件s");
            }
            return "";
        }

        /// <summary>
        /// 删除说说
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">说说feedskey</param>
        /// </summary>
        [ProxyAPIName("删除说说")]
        [DllExport(CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static bool Function_400(string authCode, long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.LocalDebug("小栗子API", $"authCode={authCode}, arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除说说");
            }
            return false;
        }

    }
}