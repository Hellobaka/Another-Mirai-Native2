using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI
{
    public class Entry
    {
        [STAThread]
        public static void Main(string[] args)
        {
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
            AppConfig.LoadConfig();
            AppConfig.IsCore = true;
            Server.OnShowErrorDialogCalled += DialogHelper.ShowErrorDialog;
            Another_Mirai_Native.Entry.CreateInitFolders();
            Another_Mirai_Native.Entry.InitExceptionCapture();
            if (AppConfig.UseDatabase && File.Exists(LogHelper.GetLogFilePath()) is false)
            {
                LogHelper.CreateDB();
            }
            new Server().Start();
        }
    }
}