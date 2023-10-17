using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.WebSocket;
using SqlSugar;
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

        private static long QQ => ProtocolManager.Instance.CurrentProtocol.GetLoginQQ();

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

        /// <summary>
        /// 日志键排序用
        /// </summary>
        /// <param name="arr">调用</param>
        /// <param name="key">需要排序的键</param>
        /// <param name="desc">是否降序</param>
        /// <typeparam name="T">调用的T</typeparam>
        public static ISugarQueryable<T> CustomOrderBy<T>(this ISugarQueryable<T> arr, string key, bool desc)
            => arr.OrderByIF(!string.IsNullOrWhiteSpace(key), $"{key} {(desc ? "desc" : "asc")}");

        public static (List<LogModel>, int) DetailQueryLogs(int priority, int pageSize, int pageIndex, string search, string sortName, bool desc, long dt1, long dt2)
        {
            using var db = GetInstance();
            List<LogModel> r = db.Queryable<LogModel>()
                .Where(x => x.priority >= priority)
                .WhereIF(dt1 != 0, x => x.time >= dt1 && x.time <= dt2 + 86400)
                .OrderByIF(string.IsNullOrEmpty(sortName), "time desc")
                .CustomOrderBy(sortName, desc).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                r = r.Where(x => x.source.Contains(search) || x.detail.Contains(search) ||
                    x.name.Contains(search) || x.status.Contains(search)).ToList();
            }
            return (r.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), r.Count);
        }

        public static List<LogModel> GetDisplayLogs(int priority, int count)
        {
            using var db = GetInstance();
            var c = db.SqlQueryable<LogModel>($"select * from log where priority>= {priority} order by id desc limit {count}").ToList();
            c.Reverse();
            return c;
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
            var fileInfo = new DirectoryInfo($@"logs\{QQ}").GetFiles("*.db");
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
            if (Directory.Exists($@"logs\{QQ}") is false)
            {
                Directory.CreateDirectory($@"logs\{QQ}");
            }

            return Path.Combine(Environment.CurrentDirectory, $@"logs\{QQ}", GetLogFileName());
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
                var result = Client.Instance.Invoke("AddLog", model);
                if (result.Success)
                {
                    return Convert.ToInt32(result.Result);
                }
                else
                {
                    return 0;
                }
            }
        }

        public static int WriteLog(LogModel model)
        {
            if (File.Exists(GetLogFilePath()) is false && AppConfig.UseDatabase)
            {
                CreateDB();
            }
            if (!string.IsNullOrWhiteSpace(model.detail) && string.IsNullOrWhiteSpace(model.name))
            {
                model.name = "";
            }
            int logId = 1;
            if (AppConfig.UseDatabase)
            {
                using var db = GetInstance();
                logId = db.Insertable(model).ExecuteReturnIdentity();
                model.id = logId;
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
            return WriteLog(LogLevel.Info, "AMN框架", "提示", messages, status);
        }

        public static int WriteLog(CQPluginProxy plugin, LogLevel level, string type, string message, string status)
        {
            return WriteLog(level, plugin.AppInfo.name, type, message, status);
        }

        public static int Debug(string type, string message)
        {
            return WriteLog(LogLevel.Debug, type, message);
        }

        public static int Info(string type, string message)
        {
            return WriteLog(LogLevel.Info, type, message);
        }

        public static int Error(string type, string message)
        {
            return WriteLog(LogLevel.Error, type, message);
        }

        public static int Error(string type, Exception e)
        {
            return WriteLog(LogLevel.Error, type, e.Message + "\n" + e.StackTrace);
        }

        private static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new(new ConnectionConfig()
            {
                ConnectionString = $"data source={GetLogFilePath()}",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
            });
            return db;
        }
    }
}