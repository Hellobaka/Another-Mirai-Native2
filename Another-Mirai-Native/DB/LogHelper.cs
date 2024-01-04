using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC;
using Another_Mirai_Native.WebSocket;
using SqlSugar;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.DB
{
    /// <summary>
    /// 描述日志的静态类
    /// </summary>
    public static class LogHelper
    {
        public delegate void AddLogHandler(int logId, LogModel log);

        public delegate void UpdateLogStatusHandler(int logId, string status);

        /// <summary>
        /// 日志添加事件
        /// </summary>
        public static event AddLogHandler LogAdded;

        /// <summary>
        /// 日志状态更新事件
        /// </summary>
        public static event UpdateLogStatusHandler LogStatusUpdated;

        private static List<LogModel> NoDatabaseLogs { get; set; } = new();

        private static long QQ
        {
            get
            {
                try
                {
                    if (ProtocolManager.Instance.CurrentProtocol != null)
                    {
                        return ProtocolManager.Instance.CurrentProtocol.GetLoginQQ();
                    }
                    return 10001;
                }
                catch
                {
                    return 10001;
                }
            }
        }

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

        public static List<LogModel> DetailQueryLogs(int priority, int pageSize, string search)
        {
            if (AppConfig.UseDatabase)
            {
                using var db = GetInstance();
                List<LogModel> r = db.Queryable<LogModel>()
                    .Where(x => x.priority >= priority)
                    .Where(x => x.source.Contains(search) || x.detail.Contains(search) ||
                        x.name.Contains(search) || x.status.Contains(search))
                    .OrderBy(x => x.time)
                    .Take(pageSize).ToList();
                return r;
            }
            else
            {
                List<LogModel> r = NoDatabaseLogs
                    .Where(x => x.priority >= priority)
                    .Where(x => x.source.Contains(search) || x.detail.Contains(search) ||
                        x.name.Contains(search) || x.status.Contains(search))
                    .OrderBy(x => x.time)
                    .Take(pageSize).ToList();
                return r;

            }
        }

        public static List<LogModel> GetDisplayLogs(int priority, int count)
        {
            if (AppConfig.UseDatabase)
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

        public static void UpdateLogStatus(int id, string status)
        {
            if (AppConfig.UseDatabase)
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
            if (AppConfig.IsCore)
            {
                return WriteLog(model);
            }
            else
            {
                Console.WriteLine($"[{level}][{DateTime.Now:G}]\t[{type}]\t{messages}");
                ClientManager.Client.AddLog(model);
                return 0;
            }
        }

        public static int WriteLog(LogModel model)
        {
            if (AppConfig.UseDatabase && File.Exists(GetLogFilePath()) is false)
            {
                CreateDB();
            }
            if (!string.IsNullOrWhiteSpace(model.detail) && string.IsNullOrWhiteSpace(model.name))
            {
                model.name = "";
            }
            int logId = NoDBLogID++;
            if (AppConfig.UseDatabase)
            {
                using var db = GetInstance();
                logId = db.Insertable(model).ExecuteReturnIdentity();
                model.id = logId;
            }
            else
            {
                model.id = logId;
                NoDatabaseLogs.Add(model);
                Console.WriteLine($"[{(model.priority > (int)LogLevel.Warning ? "-" : "+")}][{DateTime.Now:G}][{model.source}]\t[{model.name}]{model.detail}");
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

        public static int Debug(string type, string message)
        {
            if (AppConfig.DebugMode)
            {
                return WriteLog(LogLevel.Debug, type, message);
            }
            else
            {
                return 0;
            }
        }

        public static int Info(string type, string message, string status = "")
        {
            return WriteLog(LogLevel.InfoSuccess, type, message, status);
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