using Another_Mirai_Native.DB;
using Another_Mirai_Native.Native;
using SqlSugar;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Another_Mirai_Native
{
    public class RequestWaiter
    {
        public static ConcurrentDictionary<object, WaiterInfo> CommonWaiter { get; set; } = new();

        /// <summary>
        /// 关联进程的等待，当进程消失时将返回超时
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="timeout">超时时长 (单位ms)</param>
        /// <returns>是否超时</returns>
        public static bool Wait(object key, int processId, int timeout, out object result)
        {
            ManualResetEvent signal = new(false);
            WaiterInfo waiterInfo = new WaiterInfo()
            {
                CurrentProcessId = processId,
                WaitSignal = signal,
            };
            CommonWaiter.TryAdd(key, waiterInfo);
            bool timeoutFlag;
            if (timeout > 0)
            {
                timeoutFlag = signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                timeoutFlag = signal.WaitOne();
            }
            result = waiterInfo.Result;
            return timeoutFlag;
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="timeout">超时时长 (单位ms)</param>
        /// <returns>是否超时</returns>
        public static bool Wait(object key, int timeout, out object result)
        {
            ManualResetEvent signal = new(false);
            WaiterInfo waiterInfo = new()
            {
                WaitSignal = signal,
            };
            CommonWaiter.TryAdd(key, waiterInfo);
            bool timeoutFlag;
            if (timeout > 0)
            {
                timeoutFlag = signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                timeoutFlag = signal.WaitOne();
            }
            result = waiterInfo.Result;
            return timeoutFlag;
        }

        /// <summary>
        /// 关联自定义文本的等待
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="timeout">超时时长 (单位ms)</param>
        /// <returns>是否超时</returns>
        public static bool Wait(object key, string connectionID, int timeout, out object result)
        {
            ManualResetEvent signal = new(false);
            WaiterInfo waiterInfo = new()
            {
                WaitSignal = signal,
                ConnectionID = connectionID,
            };
            CommonWaiter.TryAdd(key, waiterInfo);
            bool timeoutFlag;
            if (timeout > 0)
            {
                timeoutFlag = signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                timeoutFlag = signal.WaitOne();
            }
            result = waiterInfo.Result;
            return timeoutFlag;
        }

        /// <summary>
        /// 关联Proxy的等待，当Proxy失去连接将返回超时
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="timeout">超时时长 (单位ms)</param>
        /// <returns>是否超时</returns>
        public static bool Wait(object key, CQPluginProxy plugin, int timeout, Action callBack, out object result)
        {
            ManualResetEvent signal = new(false);
            WaiterInfo waiterInfo = new()
            {
                CurrentPluginProxy = plugin,
                WaitSignal = signal,
            };
            CommonWaiter.TryAdd(key, waiterInfo);
            callBack?.Invoke();
            bool timeoutFlag;
            if (timeout > 0)
            {
                timeoutFlag = signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                timeoutFlag = signal.WaitOne();
            }
            result = waiterInfo.Result;
            return timeoutFlag;
        }

        /// <summary>
        /// 关联WebSocket的等待，当WebSocket失去连接将返回超时
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="timeout">超时时长 (单位ms)</param>
        /// <returns>是否超时</returns>
        public static bool Wait(object key, object webSocket, int timeout, Action callback, out object result)
        {
            ManualResetEvent signal = new(false);
            WaiterInfo waiterInfo = new()
            {
                Connection = webSocket,
                WaitSignal = signal,
            };
            CommonWaiter.TryAdd(key, waiterInfo);
            callback?.Invoke();
            bool timeoutFlag;
            if (timeout > 0)
            {
                timeoutFlag = signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                timeoutFlag = signal.WaitOne();
            }
            result = waiterInfo.Result;
            return timeoutFlag;
        }

        public static void TriggerByKey(object key, object result = null)
        {
            LogHelper.Debug("TriggerByKey", $"{key}");
            if (CommonWaiter.TryRemove(key, out WaiterInfo waiterInfo))
            {
                waiterInfo.Result = result;
                waiterInfo.WaitSignal.Set();
            }
        }

        public static void ResetSignalByProcess(int proecessId)
        {
            foreach (var key in CommonWaiter.Keys)
            {
                if (CommonWaiter.TryGetValue(key, out var value)
                    && value.CurrentProcessId == proecessId)
                {
                    if (CommonWaiter.TryRemove(key, out var removedValue))
                    {
                        removedValue.WaitSignal.Set();// 由于进程退出，中断所有由此进程等待的请求
                    }
                }
            }
        }

        public static void ResetSignalByPluginProxy(CQPluginProxy plugin)
        {
            foreach (var key in CommonWaiter.Keys)
            {
                if (CommonWaiter.TryGetValue(key, out var value)
                    && value.CurrentPluginProxy == plugin)
                {
                    if (CommonWaiter.TryRemove(key, out var removedValue))
                    {
                        removedValue.WaitSignal.Set();
                    }
                }
            }
        }

        public static void ResetSignalByConnection(object webSocket)
        {
            foreach (var key in CommonWaiter.Keys)
            {
                if (CommonWaiter.TryGetValue(key, out var value)
                    && value.Connection == webSocket)
                {
                    if (CommonWaiter.TryRemove(key, out var removedValue))
                    {
                        removedValue.WaitSignal.Set();
                    }
                }
            }
        }

        public static void ResetSignalByConnectionID(string connectionID)
        {
            foreach (var key in CommonWaiter.Keys)
            {
                if (CommonWaiter.TryGetValue(key, out var value)
                    && value.ConnectionID == connectionID)
                {
                    if (CommonWaiter.TryRemove(key, out var removedValue))
                    {
                        removedValue.WaitSignal.Set();
                    }
                }
            }
        }
    }

    public class WaiterInfo
    {
        public int CurrentProcessId { get; set; }

        public CQPluginProxy CurrentPluginProxy { get; set; }

        public object Connection { get; set; }

        public string ConnectionID { get; set; }

        public ManualResetEvent WaitSignal { get; set; }

        public object Result { get; set; }
    }
}