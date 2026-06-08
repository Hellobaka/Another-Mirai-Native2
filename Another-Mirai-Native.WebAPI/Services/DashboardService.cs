using Another_Mirai_Native.Config;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.WebAPI.Models;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Another_Mirai_Native.WebAPI.Services
{
    public class DashboardService : IHostedService
    {
        public event Action? OnUsageUpdated;
        public event Action? OnPluginUsageUpdated;

        private Lock UsageLock { get; set; } = new();

        private string OSVersion { get; set; }

        private string CPUName { get; set; }

        private float CPUBaseFrequency { get; set; }

        private ulong TotalMemory { get; set; }

        private ulong UsedMemory { get; set; }

        private double MemoryUsage { get; set; }

        private float CPUUsage { get; set; }

        private double TotalProcessMemory { get; set; }

        private double TotalProcessCPU { get; set; }

        private Timer UsageTimer { get; set; }

        private Timer PluginProcessTimer { get; set; }

        private PerformanceCounter CPUUsageCounter { get; set; }

        private PerformanceCounter CPUBaseFrequencyCounter { get; set; }

        private Dictionary<int, (DateTime, TimeSpan)> PluginCPUUsage { get; set; } = [];

        private List<DashboardPluginItem> PluginUsages { get; set; } = [];

        private Process CurrentProcess { get; set; } = Process.GetCurrentProcess();

        private string AssemblyVersion { get; set; }

        private string DotNetRuntimeVersion { get; set; } = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

        private string WorkingDirectory { get; set; } = Environment.CurrentDirectory;

        private double DiskUsedSpaceInGB { get; set; }

        private double DiskTotalSpaceInGB { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            CollectBaseInformation();
            CreateUsageTimer();
            AssemblyVersion = AppConfig.Instance.GetType().Assembly.GetName()?.Version?.ToString() ?? "未知版本";
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            UsageTimer.Stop();
            PluginProcessTimer.Stop();

            return Task.CompletedTask;
        }

        public object GetBaseInformation()
        {
            var startedSpan = DateTime.UtcNow - CurrentProcess.StartTime.ToUniversalTime();
            return new
            {
                OSVersion,
                CPU = $"{CPUName} @ {CPUBaseFrequency / 1000.0:f2} GHz",
                TotalMemory,
                StartedTime = $"{(int)startedSpan.TotalDays:00}:{startedSpan.Hours:00}:{startedSpan.Minutes:00}:{startedSpan.Seconds:00}",
                Version = AssemblyVersion,
                CurrentBotQQ = AppConfig.Instance.CurrentQQ,
                CurrentBotNick = AppConfig.Instance.CurrentNickName,
                LoadedPluginCount = PluginManagerProxy.Proxies.Count(x => x.Enabled),
                DotNetRuntimeVersion,
                WorkingDirectory,
                DiskUsedSpaceInGB,
                DiskTotalSpaceInGB
            };
        }

        public object GetUsages()
        {
            return new
            {
                CPUUsage,
                MemoryUsage,
                UsedMemoryInMB = UsedMemory,
                TotalMemoryInMB = TotalMemory
            };
        }

        public object GetPluginUsages()
        {
            lock (UsageLock)
            {
                return new
                {
                    TotalProcessMemory,
                    TotalProcessCPU,
                    AppConfig.Instance.ProcessedMessageCount,
                    AppConfig.Instance.SentMessageCount,
                    PluginUsages
                };
            }
        }

        private void CreateUsageTimer()
        {
            UsageTimer = new Timer
            {
                Interval = 1000
            };
            UsageTimer.Elapsed += UsageTimer_Ticked;
            UsageTimer.Enabled = true;

            PluginProcessTimer = new Timer
            {
                Interval = 1000
            };
            PluginProcessTimer.Elapsed += PluginProcessTimer_Ticked;
            PluginProcessTimer.Enabled = true;
        }

        private void PluginProcessTimer_Ticked(object? sender, ElapsedEventArgs e)
        {
            lock (UsageLock)
            {
                PluginUsages.Clear();
                foreach (var plugin in PluginManagerProxy.Proxies)
                {
                    var process = plugin.PluginProcess;
                    if (process == null || process.HasExited || plugin.AppInfo == null)
                    {
                        continue;
                    }
                    process.Refresh();
                    double cpu = CalcProcessCPUUsage(process);
                    try
                    {
                        var processItem = new DashboardPluginItem()
                        {
                            Id = plugin.AppInfo.AuthCode,
                            PID = process.Id,
                            MemoryUsage = process.PrivateMemorySize64 / 1024.0 / 1024,
                            PluginName = plugin.PluginName,
                            Running = !process.HasExited,
                            CPUUsage = cpu
                        };
                        PluginUsages.Add(processItem);
                    }
                    catch
                    {
                        // ignore process access exceptions, e.g. process exited between HasExited check and accessing PrivateMemorySize64
                    }
                }

                CurrentProcess.Refresh();
                var selfProcessItem = new DashboardPluginItem()
                {
                    Id = 0, // 0 for framework self
                    PID = CurrentProcess.Id,
                    MemoryUsage = CurrentProcess.PrivateMemorySize64 / 1024.0 / 1024,
                    PluginName = "主框架",
                    Running = !CurrentProcess.HasExited,
                    CPUUsage = CalcProcessCPUUsage(CurrentProcess)
                };
                PluginUsages.Add(selfProcessItem);

                PluginUsages = PluginUsages.OrderByDescending(x => x.MemoryUsage).ToList();
                TotalProcessMemory = PluginUsages.Sum(x => x.MemoryUsage);
                TotalProcessCPU = PluginUsages.Sum(x => x.CPUUsage);
            }
            OnPluginUsageUpdated?.Invoke();
        }

        private void UsageTimer_Ticked(object? sender, ElapsedEventArgs e)
        {
            CPUUsage = CPUUsageCounter.NextValue();
            UsedMemory = GetUsedMemory();

            MemoryUsage = (UsedMemory / (TotalMemory * 1.0)) * 100.0;

            OnUsageUpdated?.Invoke();
        }

        private void CollectBaseInformation()
        {
            OSVersion = GetDetailedOSVersion();
            CPUName = GetCpuName();
            CPUBaseFrequency = GetCpuFrequency();
            TotalMemory = GetTotalPhysicalMemory();
            CPUUsageCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
            CPUBaseFrequencyCounter = new PerformanceCounter("Processor Information", "Processor Frequency", "_Total");

            CPUUsageCounter.NextValue();
            CPUBaseFrequencyCounter.NextValue();

            var currentDrive = DriveInfo.GetDrives().FirstOrDefault(d => d.IsReady && d.RootDirectory.FullName == Path.GetPathRoot(Environment.CurrentDirectory));
            if (currentDrive != null)
            {
                DiskUsedSpaceInGB = (currentDrive.TotalSize - currentDrive.AvailableFreeSpace) / 1024.0 / 1024 / 1024;
                DiskTotalSpaceInGB = currentDrive.TotalSize / 1024.0 / 1024 / 1024;
            }
        }

        private static string GetDetailedOSVersion()
        {
            string osName = "Unknown OS";
            string version = "Unknown Version";
            string edition = "Unknown Edition";

            using (var searcher = new ManagementObjectSearcher("SELECT Caption, Version, OSArchitecture FROM Win32_OperatingSystem"))
            {
                foreach (var os in searcher.Get())
                {
                    osName = os["Caption"]?.ToString() ?? osName;
                    version = os["Version"]?.ToString() ?? version;
                    edition = os["OSArchitecture"]?.ToString() ?? edition;
                }
            }
            string? releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", "")?.ToString();
            if (!string.IsNullOrEmpty(releaseId))
            {
                version = releaseId;
            }
            return $"{osName} {edition} 版本 {version}";
        }

        private static string GetCpuName()
        {
            using var searcher = new ManagementObjectSearcher("select Name from Win32_Processor");
            var cpuName = searcher.Get().Cast<ManagementBaseObject>().FirstOrDefault()?["Name"];
            return cpuName?.ToString() ?? "Unknown";
        }

        private static uint GetCpuFrequency()
        {
            using var searcher = new ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor");
            var maxClockSpeed = searcher.Get().Cast<ManagementBaseObject>().FirstOrDefault()?["MaxClockSpeed"];
            return (uint)(maxClockSpeed ?? 0);
        }
        private static ulong GetTotalPhysicalMemory()
        {
            using var searcher = new ManagementObjectSearcher("select TotalVisibleMemorySize from Win32_OperatingSystem");
            var totalMemory = searcher.Get().Cast<ManagementBaseObject>().FirstOrDefault()?["TotalVisibleMemorySize"];
            return (ulong)(totalMemory ?? 0) / 1024;
        }

        private ulong GetUsedMemory()
        {
            using var searcher = new ManagementObjectSearcher("select FreePhysicalMemory from Win32_OperatingSystem");
            var freeMemory = searcher.Get().Cast<ManagementBaseObject>().FirstOrDefault()?["FreePhysicalMemory"];
            ulong freeMemoryMB = (ulong)(freeMemory ?? 0) / 1024;
            return TotalMemory - freeMemoryMB;
        }

        private double CalcProcessCPUUsage(Process pluginProcess)
        {
            double cpu = 0;
            if (PluginCPUUsage.TryGetValue(pluginProcess.Id, out (DateTime lastUpdateTime, TimeSpan lastCpuUsage) usage))
            {
                var cpuUsage = pluginProcess.TotalProcessorTime;
                var updateTime = DateTime.UtcNow;

                PluginCPUUsage[pluginProcess.Id] = (updateTime, cpuUsage);

                // 计算 CPU 使用率
                var cpuUsedMs = (cpuUsage - usage.lastCpuUsage).TotalMilliseconds;
                var totalMsPassed = (updateTime - usage.lastUpdateTime).TotalMilliseconds;
                cpu = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100;
            }
            else
            {
                var startCpuUsage = pluginProcess.TotalProcessorTime;
                var startTime = DateTime.UtcNow;

                PluginCPUUsage[pluginProcess.Id] = (startTime, startCpuUsage);
            }

            return cpu;
        }
    }
}
