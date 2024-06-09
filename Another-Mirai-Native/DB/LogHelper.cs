using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC;
using SqlSugar;
using System.Text;

namespace Another_Mirai_Native.DB
{
    /// <summary>
    /// 描述日志的静态类
    /// </summary>
    public static class LogHelper
    {
        private static object writeLock = new();

        public delegate void AddLogHandler(int logId, LogModel log);

        public delegate void UpdateLogStatusHandler(int logId, string status);

        /// <summary>
        /// Origin Type Message
        /// </summary>
        public static event Action<LogModel> DebugLogAdded;

        /// <summary>
        /// 日志添加事件
        /// </summary>
        public static event AddLogHandler LogAdded;

        /// <summary>
        /// 日志状态更新事件
        /// </summary>
        public static event UpdateLogStatusHandler LogStatusUpdated;

        public static List<LogModel> DebugLogs { get; set; } = new();

        private static List<LogModel> NoDatabaseLogs { get; set; } = new();

        private static int NoDBLogID { get; set; }

        /// <summary>
        /// 初始化日志数据库
        /// </summary>
        public static void CreateDB()
        {
            using (var db = GetInstance())
            {
                string DBPath = GetLogFilePath();
                db.DbMaintenance.CreateDatabase(DBPath);
                db.CodeFirst.InitTables(typeof(LogModel));
            }
            WriteLog(LogLevel.InfoSuccess, "运行日志", $"日志数据库初始化完毕{DateTime.Now:yyMMdd}。");
        }

        public static void Debug(string type, string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{type}]", $"\t{message}");
            if (AppConfig.Instance.DebugMode is false)
            {
                return;
            }
            var logModel = new LogModel
            {
                detail = message,
                name = type,
                time = Helper.TimeStamp
            };
            DebugLogs.Add(logModel);
            DebugLogAdded?.Invoke(logModel);
        }

        public static List<LogModel> DetailQueryLogs(int priority, int pageSize, string search)
        {
            if (AppConfig.Instance.UseDatabase)
            {
                using var db = GetInstance();
                List<LogModel> r = db.Queryable<LogModel>()
                    .Where(x => x.priority >= priority)
                    .Where(x => x.source.Contains(search) || x.detail.Contains(search) ||
                        x.name.Contains(search) || x.status.Contains(search))
                    .OrderByDescending(x => x.id)
                    .Take(pageSize).ToList();
                r.Reverse();
                return r;
            }
            else
            {
                List<LogModel> r = NoDatabaseLogs
                    .Where(x => x.priority >= priority)
                    .Where(x => x.source.Contains(search) || x.detail.Contains(search) ||
                        x.name.Contains(search) || x.status.Contains(search))
                    .OrderByDescending(x => x.id)
                    .Take(pageSize).ToList();
                r.Reverse();
                return r;
            }
        }

        public static int Error(string type, string message)
        {
            return WriteLog(LogLevel.Error, type, message);
        }

        public static int Error(string type, Exception e)
        {
            if (e.InnerException != null)
            {
                Error("调用插件方法", e.InnerException);
            }
            return WriteLog(LogLevel.Error, type, e.Message + "\n" + e.StackTrace);
        }

        public static List<LogModel> GetDisplayLogs(int priority, int count)
        {
            if (AppConfig.Instance.UseDatabase)
            {
                using var db = GetInstance();
                var c = db.SqlQueryable<LogModel>($"select * from log where priority>= {priority} order by id desc limit {count}").ToList();
                c.Reverse();
                return c;
            }
            else
            {
                return DetailQueryLogs(priority, count, "");
            }
        }

        public static LogModel GetLastLog()
        {
            using var db = GetInstance();
            return db.SqlQueryable<LogModel>("select * from log order by id desc limit 1").First();
        }

        public static LogModel GetLogByID(int id)
        {
            using var db = GetInstance();
            return db.Queryable<LogModel>().First(x => x.id == id);
        }

        /// <summary>
        /// 获取当天日志文件名
        /// </summary>
        /// <returns></returns>
        public static string GetLogFileName()
        {
            var fileInfo = new DirectoryInfo("logs").GetFiles("*.db");
            string filename = "";
            foreach (var item in fileInfo)
            {
                if (item.Name.StartsWith($"logv2_{DateTime.Now:yyMM}"))
                {
                    filename = item.Name;
                    break;
                }
            }
            return string.IsNullOrWhiteSpace(filename) ? $"logv2_{DateTime.Now:yyMMdd}.db" : filename;
        }

        /// <summary>
        /// 获取日志文件路径
        /// </summary>
        public static string GetLogFilePath()
        {
            if (Directory.Exists("logs") is false)
            {
                Directory.CreateDirectory("logs");
            }

            return Path.Combine(Environment.CurrentDirectory, "logs", GetLogFileName());
        }

        /// <summary>
        /// 获取显示日志的时间文本
        /// </summary>
        /// <param name="timestamp">待转换文本</param>
        /// <returns></returns>
        public static string GetTimeStampString(long timestamp)
        {
            DateTime time = Helper.TimeStamp2DateTime(timestamp);
            StringBuilder sb = new();
            sb.Append($"{time:MM/dd HH:mm:ss}");
            return sb.ToString();
        }

        public static int Info(string type, string message, string status = "")
        {
            return WriteLog(LogLevel.InfoSuccess, type, message, status);
        }

        public static void UpdateLogStatus(int id, string status)
        {
            if (AppConfig.Instance.UseDatabase)
            {
                using var db = GetInstance();
                db.Updateable<LogModel>().SetColumns(x => x.status == status).Where(x => x.id == id)
                  .ExecuteCommand();
            }
            else
            {
                var r = NoDatabaseLogs.FirstOrDefault(x => x.id == id);
                if (r != null)
                {
                    r.status = status;
                }
            }
            LogStatusUpdated?.Invoke(id, status);
        }

        public static int WriteLog(LogLevel level, string logOrigin, string type, string messages, string status = "")
        {
            LogModel model = new()
            {
                detail = messages,
                id = 0,
                source = logOrigin,
                priority = (int)level,
                name = type,
                time = Helper.TimeStamp,
                status = status
            };
            if (AppConfig.Instance.IsCore)
            {
                return WriteLog(model);
            }
            else
            {
                ClientManager.Client.AddLog(model);
                return 0;
            }
        }

        public static int WriteLog(LogModel model)
        {
            if (AppConfig.Instance.UseDatabase && File.Exists(GetLogFilePath()) is false)
            {
                CreateDB();
            }
            if (!string.IsNullOrWhiteSpace(model.detail) && string.IsNullOrWhiteSpace(model.name))
            {
                model.name = "";
            }
            int logId = NoDBLogID++;
            if (AppConfig.Instance.UseDatabase)
            {
                lock (writeLock)
                {
                    using var db = GetInstance();
                    logId = db.Insertable(model).ExecuteReturnIdentity();
                }
                model.id = logId;
            }
            else
            {
                model.id = logId;
                NoDatabaseLogs.Add(model);
            }
            if (Console.LargestWindowWidth > 0)
            {
                ChangeConsoleColor(model.priority);
                Console.WriteLine($"[{(model.priority > (int)LogLevel.Warning ? "-" : "+")}][{DateTime.Now:G}][{model.source}] [{model.name}]{model.detail}");
            }
            LogAdded?.Invoke(logId, model);
            return logId;
        }

        public static int WriteLog(int level, string logOrigin, string type, string messages, string status = "")
        {
            LogLevel logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), Enum.GetName(typeof(LogLevel), level));
            return WriteLog(logLevel, logOrigin, type, messages, status);
        }

        public static int WriteLog(LogLevel level, string type, string message, string status = "")
        {
            return WriteLog(level, "AMN框架", type, message, status);
        }

        /// <summary>
        /// 以Info为等级，"AMN框架"为来源，"提示"为类型写出一条日志
        /// </summary>
        /// <param name="messages">日志内容</param>
        public static int WriteLog(string messages, string status = "")
        {
            return WriteLog(LogLevel.InfoSuccess, "AMN框架", "提示", messages, status);
        }

        public static int WriteLog(CQPluginProxy plugin, LogLevel level, string type, string message, string status)
        {
            return WriteLog(level, plugin.AppInfo.name, type, message, status);
        }

        private static void ChangeConsoleColor(int priority)
        {
            ConsoleColor logColor = (LogLevel)priority switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Fatal => ConsoleColor.DarkRed,
                LogLevel.InfoSuccess => ConsoleColor.Magenta,
                LogLevel.InfoSend => ConsoleColor.Green,
                LogLevel.InfoReceive => ConsoleColor.Blue,
                LogLevel.Warning => ConsoleColor.DarkYellow,
                _ => ConsoleColor.White,
            };
            Console.ForegroundColor = logColor;
        }

        private static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new(new ConnectionConfig()
            {
                ConnectionString = $"data source={GetLogFilePath()}",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = false,
                InitKeyType = InitKeyType.Attribute,
            });
            return db;
        }
    }
}