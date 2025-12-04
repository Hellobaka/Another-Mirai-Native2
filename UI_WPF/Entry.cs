using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.RPC;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.UI
{
    public class Entry
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }
            if (Directory.Exists("wwwroot"))
            {
                Helper.CreateDirectoryLink(@"wwwroot\image", @"data\image");
            }
            AppConfig.Instance.StartTime = DateTime.Now;

#if NET5_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            if (args.Length == 0)
            {
                InitCore();
                App.Main();
            }
            else
            {
                Another_Mirai_Native.Entry.Main(args);
            }
        }

        private static void InitCore()
        {
            AppConfig.Instance.IsCore = true;
            Another_Mirai_Native.Entry.CreateInitFolders();
            if (AppConfig.Instance.UseDatabase && File.Exists(LogHelper.GetLogFilePath()) is false)
            {
                LogHelper.CreateDB();
            }
            ServerManager serverManager = new();
            if (serverManager.Build(AppConfig.Instance.ServerType) is false)
            {
                LogHelper.Error("初始化", "构建服务器失败");
                return;
            }
            LogHelper.Info("初始化", "初始化连接参数成功");
            if (ServerManager.Server.SetConnectionConfig() is false)
            {
                LogHelper.Error("初始化", "初始化连接参数失败，请检查配置内容");
                return;
            }
            LogHelper.Info("初始化", "启动插件服务器成功");
            if (!ServerManager.Server.Start())
            {
                LogHelper.Error("初始化", "构建服务器失败");
                return;
            }
            ServerManager.Server.OnShowErrorDialogCalled += DialogHelper.ShowErrorDialog;
            Another_Mirai_Native.Entry.InitExceptionCapture();
        }
    }
}