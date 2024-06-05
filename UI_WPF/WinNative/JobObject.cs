using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.WinNative
{
    public class JobObject
    {
        private enum JobObjectInfoType
        {
            AssociateCompletionPortInformation = 7,

            BasicLimitInformation = 2,

            BasicUIRestrictions = 4,

            EndOfJobTimeInformation = 6,

            ExtendedLimitInformation = 9,

            SecurityLimitInformation = 5,

            GroupInformation = 11
        }

        public static void AssignProcessToKillOnCloseJob(Process process)
        {
            var job = CreateJobObject(IntPtr.Zero, null);

            var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION { LimitFlags = 0x2000 };

            var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION { BasicLimitInformation = info };

            int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (!SetInformationJobObject(job, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length))
            {
                throw new System.ComponentModel.Win32Exception();
            }

            AssignProcessToJobObject(job, process.Handle);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateJobObject(IntPtr a, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

        [StructLayout(LayoutKind.Sequential)]
        private struct IO_COUNTERS
        {
            public ulong ReadOperationCount;

            public ulong WriteOperationCount;

            public ulong OtherOperationCount;

            public ulong ReadTransferCount;

            public ulong WriteTransferCount;

            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;

            public long PerJobUserTimeLimit;

            public short LimitFlags;

            public IntPtr MinimumWorkingSetSize;

            public IntPtr MaximumWorkingSetSize;

            public short ActiveProcessLimit;

            public long Affinity;

            public short PriorityClass;

            public short SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;

            public IO_COUNTERS IoInfo;

            public IntPtr ProcessMemoryLimit;

            public IntPtr JobMemoryLimit;

            public IntPtr PeakProcessMemoryUsed;

            public IntPtr PeakJobMemoryUsed;
        }
    }
}