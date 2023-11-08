using Another_Mirai_Native.Native;
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
        public static bool Wait(object key, Process process, int timeout)
        {
            ManualResetEvent signal = new(false);
            CommonWaiter.TryAdd(key, new WaiterInfo()
            {
                CurrentProcess = process,
                WaitSignal = signal,
            });
            if (timeout > 0)
            {
                return signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                return signal.WaitOne();
            }
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="timeout">超时时长 (单位ms)</param>
        /// <returns>是否超时</returns>
        public static bool Wait(object key, int timeout)
        {
            ManualResetEvent signal = new(false);
            CommonWaiter.TryAdd(key, new WaiterInfo()
            {
                WaitSignal = signal,
            });
            if (timeout > 0)
            {
                return signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                return signal.WaitOne();
            }
        }

        /// <summary>
        /// 关联自定义文本的等待
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="timeout">超时时长 (单位ms)</param>
        /// <returns>是否超时</returns>
        public static bool Wait(object key, string connectionID, int timeout)
        {
            ManualResetEvent signal = new(false);
            CommonWaiter.TryAdd(key, new WaiterInfo()
            {
                WaitSignal = signal,
                ConnectionID = connectionID,
            });
            if (timeout > 0)
            {
                return signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                return signal.WaitOne();
            }
        }

        /// <summary>
        /// 关联Proxy的等待，当Proxy失去连接将返回超时
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="timeout">超时时长 (单位ms)</param>
        /// <returns>是否超时</returns>
        public static bool Wait(object key, CQPluginProxy plugin, int timeout)
        {
            ManualResetEvent signal = new(false);
            CommonWaiter.TryAdd(key, new WaiterInfo()
            {
                CurrentPluginProxy = plugin,
                WaitSignal = signal,
            });
            if (timeout > 0)
            {
                return signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                return signal.WaitOne();
            }
        }

        /// <summary>
        /// 关联WebSocket的等待，当WebSocket失去连接将返回超时
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="timeout">超时时长 (单位ms)</param>
        /// <returns>是否超时</returns>
        public static bool Wait(object key, WebSocketSharp.WebSocket webSocket, int timeout)
        {
            ManualResetEvent signal = new(false);
            CommonWaiter.TryAdd(key, new WaiterInfo()
            {
                CurrentWebSocket = webSocket,
                WaitSignal = signal,
            });
            if (timeout > 0)
            {
                return signal.WaitOne(TimeSpan.FromMilliseconds(timeout));
            }
            else
            {
                return signal.WaitOne();
            }
        }

        public static void TriggerByKey(object key)
        {
            if (CommonWaiter.TryRemove(key, out WaiterInfo waiterInfo))
            {
                waiterInfo.WaitSignal.Set();
            }
        }

        public static void ResetSignalByProcess(Process process)
        {
            foreach (var key in CommonWaiter.Keys)
            {
                if (CommonWaiter.TryGetValue(key, out var value)
                    && value.CurrentProcess.Id == process.Id)
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

        public static void ResetSignalByWebSocket(WebSocketSharp.WebSocket webSocket)
        {
            foreach (var key in CommonWaiter.Keys)
            {
                if (CommonWaiter.TryGetValue(key, out var value)
                    && value.CurrentWebSocket == webSocket)
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
        public Process CurrentProcess { get; set; }

        public CQPluginProxy CurrentPluginProxy { get; set; }

        public WebSocketSharp.WebSocket CurrentWebSocket { get; set; }

        public string ConnectionID { get; set; }

        public ManualResetEvent WaitSignal { get; set; }
    }
}