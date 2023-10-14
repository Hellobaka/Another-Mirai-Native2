using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native
{
    public class LogHelper
    {
        public static void Debug(string type, string message)
        {
            Console.WriteLine($"[+][Debug][{DateTime.Now:G}][{type}] {message}");
        }

        public static void Info(string type, string message)
        {
            Console.WriteLine($"[+][Info][{DateTime.Now:G}][{type}] {message}");
        }

        public static void Error(string type, string message)
        {
            Console.WriteLine($"[-][Error][{DateTime.Now:G}][{type}] {message}");
        }

        public static void Error(string type, Exception e)
        {
            Console.WriteLine($"[-][Error][{DateTime.Now:G}][{type}] {e.Message}\n{e.StackTrace}");
        }

        public static void UpdateLogStatus(int logId, string msg)
        {
        }
    }
}