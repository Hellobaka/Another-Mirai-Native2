﻿using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Native.Handler.XiaoLiZi
{
    public static class API
    {
        public static MethodInfo? GetMethodByProxyName(string name)
        {
            var method = typeof(API).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                   .Where(m => m.GetCustomAttributes(typeof(ProxyAPINameAttribute), false)
                                                .OfType<ProxyAPINameAttribute>()
                                                .Any(attr => attr.Description == name))
                                   .FirstOrDefault();
            return method;
        }

        public static string GetProxyName(MethodInfo method)
        {
            if (Attribute.GetCustomAttribute(method, typeof(ProxyAPINameAttribute)) is ProxyAPINameAttribute methodAttribute)
            {
                return methodAttribute.Description;
            }
            return "";
        }

        #region Delegates
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_1_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_2_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_3_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_4_Delegate(string arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_5_Delegate(string arg0, int arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_6_Delegate(long arg0, long arg1, string arg2, long arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_7_Delegate(long arg0, long arg1, string arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_8_Delegate(long arg0, long arg1, long arg2, string arg3, long arg4, int arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_9_Delegate(long arg0, long arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_10_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_11_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_12_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_13_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_14_Delegate(long arg0, long arg1, string arg2, long arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_15_Delegate(long arg0, long arg1, string arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_16_Delegate(long arg0, long arg1, bool arg2, byte[] arg3, int arg4, int arg5, bool arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_17_Delegate(long arg0, long arg1, bool arg2, byte[] arg3, int arg4, int arg5, bool arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_18_Delegate(long arg0, long arg1, int arg2, string arg3, byte[] arg4, int arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_19_Delegate(long arg0, long arg1, int arg2, string arg3, byte[] arg4, int arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_20_Delegate(long arg0, byte[] arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate IntPtr Function_21_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate IntPtr Function_22_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate IntPtr Function_23_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_24_Delegate(long arg0, long arg1, long arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_25_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_26_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_27_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_28_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_29_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_30_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_31_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_32_Delegate(long arg0, object arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_33_Delegate(long arg0, object arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_34_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_35_Delegate(long arg0, long arg1, long arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_36_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_37_Delegate(long arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_38_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_39_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_40_Delegate(long arg0, string arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_41_Delegate(long arg0, long arg1, long arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_42_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_43_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_44_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_45_Delegate(long arg0, long arg1, byte[] arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_46_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_47_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_48_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_49_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_50_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_51_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_52_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_53_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_54_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_55_Delegate(long arg0, long arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_56_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_57_Delegate(long arg0, long arg1, long arg2, int arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_58_Delegate(long arg0, long arg1, double arg2, double arg3, bool arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_59_Delegate(long arg0, long arg1, double arg2, double arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate long Function_60_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_61_Delegate(long arg0, long arg1, long arg2, long arg3, int arg4, int arg5, string arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_62_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_63_Delegate(long arg0, string arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_64_Delegate(long arg0, long arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_65_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_66_Delegate(long arg0, long arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_67_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_68_Delegate(long arg0, long arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_69_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_70_Delegate(long arg0, long arg1, string arg2, string arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_71_Delegate(long arg0, long arg1, string arg2, object arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_72_Delegate(long arg0, int arg1, int arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_73_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_74_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_75_Delegate(string arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_76_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_77_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_78_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_79_Delegate(long arg0, long arg1, long arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_80_Delegate(long arg0, long arg1, long arg2, string arg3, string arg4, long arg5, int arg6, long arg7, int arg8);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_81_Delegate(long arg0, long arg1, string arg2, string arg3, long arg4, int arg5, long arg6, int arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_82_Delegate(long arg0, long arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_83_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_84_Delegate(long arg0, object arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_85_Delegate(long arg0, string arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_86_Delegate(long arg0, object arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_87_Delegate(long arg0, long arg1, string arg2, string arg3, string arg4, string arg5, string arg6, int arg7, int arg8);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_88_Delegate(int arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_89_Delegate(int arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_90_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_91_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_92_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_93_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_94_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_95_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_96_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, bool arg6, string arg7, int arg8, object arg9);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_97_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_98_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_99_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_100_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_101_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_102_Delegate(long arg0, long arg1, long arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_103_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_104_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_105_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_106_Delegate(long arg0, long arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_107_Delegate(long arg0, string arg1, int arg2, string arg3, int arg4, string arg5, string arg6, string arg7, string arg8, string arg9);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_108_Delegate(long arg0, long arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_109_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_110_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_111_Delegate(long arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_112_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_113_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_114_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_115_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_116_Delegate(long arg0, long arg1, int arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_117_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_118_Delegate(long arg0, long arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_119_Delegate(long arg0, long arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_120_Delegate(long arg0, long arg1, long arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_121_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_122_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_123_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_124_Delegate(long arg0, bool arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_125_Delegate(long arg0, string arg1, string arg2, string arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_126_Delegate(long arg0, long arg1, string arg2, string arg3, byte[] arg4, string arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_127_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_128_Delegate(long arg0, long arg1, long arg2, object arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_129_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_130_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_131_Delegate(long arg0, int arg1, long arg2, string arg3, int arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_132_Delegate(long arg0, int arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_133_Delegate(long arg0, long arg1, long arg2, string arg3, string arg4, string arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_134_Delegate(long arg0, long arg1, string arg2, string arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_135_Delegate(long arg0, long arg1, string arg2, byte[] arg3, int arg4, int arg5, int arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_136_Delegate(long arg0, long arg1, string arg2, long arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_137_Delegate(long arg0, long arg1, string arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_138_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_139_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_140_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_141_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_142_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_143_Delegate(long arg0, string arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_144_Delegate(long arg0, int arg1, int arg2, byte[] arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_145_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_146_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_147_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_148_Delegate(long arg0, int arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_149_Delegate(long arg0, byte[] arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_150_Delegate(long arg0, string arg1, int arg2, string arg3, object arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_151_Delegate(long arg0, string arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_152_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_153_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_154_Delegate(long arg0, long arg1, string arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_155_Delegate(long arg0, string arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_156_Delegate(long arg0, long arg1, long arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_157_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_158_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_159_Delegate(long arg0, object arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_160_Delegate(long arg0, object arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_161_Delegate(long arg0, string arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_162_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_163_Delegate(byte[] arg0, byte[] arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_164_Delegate(byte[] arg0, byte[] arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_165_Delegate(string arg0, int arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_166_Delegate(string arg0, int arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_167_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_168_Delegate(long arg0, long arg1, int arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_169_Delegate(long arg0, long arg1, int arg2, string arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_170_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_171_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_172_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_173_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_174_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_175_Delegate(long arg0, long arg1, long arg2, long arg3, int arg4, int arg5, string arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_176_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_177_Delegate(long arg0, long arg1, object arg2, long arg3, int arg4, string arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_178_Delegate(long arg0, long arg1, object arg2, bool arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_179_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_180_Delegate(long arg0, long arg1, string arg2, byte[] arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_181_Delegate(string arg0, string arg1, object arg2, string arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_182_Delegate(long arg0, long arg1, string arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_183_Delegate(long arg0, long arg1, long arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_184_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_185_Delegate(string arg0, string arg1, string arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate void Function_186_Delegate(string arg0, string arg1, string arg2, int arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_187_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_188_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_189_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_190_Delegate(long arg0, long arg1, long arg2, string arg3, long arg4, int arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_191_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_192_Delegate(long arg0, long arg1, byte[] arg2, string arg3, long arg4, int arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_193_Delegate(long arg0, long arg1, string arg2, long arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_194_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_195_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_196_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate long Function_197_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_198_Delegate(long arg0, object arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_199_Delegate(long arg0, object arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_200_Delegate(long arg0, long arg1, long arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_201_Delegate(long arg0, long arg1, long arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_202_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_203_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_204_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_205_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_206_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_207_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_208_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_209_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, bool arg6, string arg7, int arg8, object arg9);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_210_Delegate(long arg0, long arg1, long arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_211_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_212_Delegate(long arg0, long arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_213_Delegate(long arg0, long arg1, string arg2, long arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_214_Delegate(long arg0, long arg1, long arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_215_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_216_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_217_Delegate(long arg0, long arg1, long arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_218_Delegate(long arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_219_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_220_Delegate(long arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_221_Delegate(long arg0, long arg1, long arg2, string arg3, string arg4, long arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_222_Delegate(long arg0, long arg1, long arg2, string arg3, string arg4, long arg5, int arg6, long arg7, int arg8);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_223_Delegate(long arg0, bool arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_224_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_225_Delegate(string arg0, int arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_226_Delegate(long arg0, long arg1, object arg2, string arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_227_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_228_Delegate(long arg0, string arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_229_Delegate(long arg0, long arg1, string arg2, int arg3, string arg4, int arg5, object arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_230_Delegate(long arg0, long arg1, object arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_231_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_232_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_233_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_234_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_235_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_236_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_237_Delegate(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_238_Delegate(long arg0, string arg1, int arg2, string arg3, int arg4, object arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_239_Delegate(long arg0, string arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_240_Delegate(long arg0, string arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_241_Delegate(long arg0, long arg1, int arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate IntPtr Function_242_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_243_Delegate(long arg0, string arg1, byte[] arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_244_Delegate(long arg0, string arg1, string arg2, string arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_245_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_246_Delegate(long arg0, string arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_247_Delegate(long arg0, byte[] arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_248_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_249_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_250_Delegate(long arg0, long arg1, string arg2, long arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_251_Delegate(long arg0, long arg1, byte[] arg2, string arg3, long arg4, int arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_252_Delegate(long arg0, long arg1, long arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_253_Delegate(long arg0, long arg1, long arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_254_Delegate(long arg0, int arg1, byte[] arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_255_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_256_Delegate(long arg0, long arg1, string arg2, int arg3, long arg4, int arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_257_Delegate(long arg0, long arg1, string arg2, string arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_258_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_259_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_260_Delegate(long arg0, long arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_261_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_262_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_263_Delegate(long arg0, long arg1, string arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_264_Delegate(long arg0, long arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_265_Delegate(long arg0, long arg1, string arg2, string arg3, string arg4, bool arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate long Function_266_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_267_Delegate(long arg0, long arg1, long arg2, byte[] arg3, int arg4, int arg5, bool arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_268_Delegate(long arg0, long arg1, long arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_269_Delegate(long arg0, long arg1, long arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_270_Delegate(long arg0, long arg1, long arg2, long arg3, long arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_271_Delegate(long arg0, long arg1, long arg2, int arg3, string arg4, bool arg5, bool arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_272_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_273_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_274_Delegate(long arg0, long arg1, long arg2, int arg3, bool arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_275_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_276_Delegate(long arg0, long arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_277_Delegate(long arg0, long arg1, long arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_278_Delegate(long arg0, long arg1, long arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_279_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_280_Delegate(long arg0, long arg1, string arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_281_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_282_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_283_Delegate(long arg0, long arg1, long arg2, int arg3, long arg4, bool arg5, long arg6, bool arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_284_Delegate(long arg0, long arg1, long arg2, int arg3, long arg4, bool arg5, long arg6, bool arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_285_Delegate(long arg0, long arg1, long arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_286_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_287_Delegate(long arg0, long arg1, long arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_288_Delegate(long arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_289_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_290_Delegate(long arg0, int arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_291_Delegate(long arg0, int arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_292_Delegate(long arg0, int arg1, string arg2, int arg3, string arg4, int arg5, string arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_293_Delegate(long arg0, bool arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_294_Delegate(long arg0, long arg1, int arg2, string arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_295_Delegate(long arg0, string arg1, int arg2, object arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_296_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_297_Delegate(long arg0, bool arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_298_Delegate(long arg0, long arg1, object arg2, int arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_299_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_300_Delegate(long arg0, long arg1, int arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_301_Delegate(long arg0, int arg1, long arg2, string arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_302_Delegate(long arg0, long arg1, long arg2, string arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_303_Delegate(long arg0, int arg1, int arg2, long arg3, long arg4, string arg5, int arg6, string arg7, int arg8, object arg9);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_304_Delegate(long arg0, int arg1, int arg2, long arg3, long arg4, string arg5, int arg6, string arg7, int arg8, object arg9);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_305_Delegate(long arg0, int arg1, int arg2, long arg3, long arg4, long arg5, string arg6, string arg7, int arg8, object arg9);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_306_Delegate(long arg0, long arg1, long arg2, long arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_307_Delegate(long arg0, long arg1, long arg2, object arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_308_Delegate(long arg0, long arg1, long arg2, long arg3, bool arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_309_Delegate(long arg0, long arg1, long arg2, string arg3, long arg4, bool arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_310_Delegate(long arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate long Function_311_Delegate(long arg0, long arg1, string arg2, long arg3, bool arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_312_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_313_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_314_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_315_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_316_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_317_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_318_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_319_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_320_Delegate(string arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_321_Delegate(string arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_322_Delegate(long arg0, long arg1, object arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_323_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_324_Delegate(long arg0, long arg1, long arg2, string arg3, int arg4, int arg5, int arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_325_Delegate(int arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_326_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_327_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_328_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_329_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_330_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_331_Delegate(string arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_332_Delegate(long arg0, long arg1, long arg2, object arg3, int arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_333_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_334_Delegate(long arg0, long arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_335_Delegate(long arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_336_Delegate(long arg0, long arg1, string arg2, string arg3, int arg4, int arg5, int arg6, int arg7, int arg8, long arg9, long arg10, int arg11, int arg12);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_337_Delegate(string arg0, int arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_338_Delegate(long arg0, long arg1, long arg2, string arg3, string arg4, string arg5, string arg6, string arg7, int arg8);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_339_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_340_Delegate(long arg0, string arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_341_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_342_Delegate(long arg0, long arg1, long arg2, object arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_343_Delegate(long arg0, long arg1, long arg2, long arg3, bool arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_344_Delegate(long arg0, long arg1, long arg2, long arg3, bool arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_345_Delegate(long arg0, long arg1, long arg2, long arg3, bool arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_346_Delegate(long arg0, long arg1, long arg2, long arg3, bool arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_347_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_348_Delegate(long arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_349_Delegate(long arg0, long arg1, long arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_350_Delegate(long arg0, long arg1, long arg2, string arg3, int arg4, int arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_351_Delegate(long arg0, long arg1, long arg2, object arg3, string arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_352_Delegate(long arg0, long arg1, long arg2, long arg3, object arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_353_Delegate(long arg0, long arg1, long arg2, long arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_354_Delegate(long arg0, long arg1, long arg2, long arg3, object arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_355_Delegate(long arg0, long arg1, long arg2, string arg3, string arg4, long arg5, long arg6, int arg7, long arg8, object arg9);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_356_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_357_Delegate(long arg0, long arg1, long arg2, object arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_358_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_359_Delegate(long arg0, long arg1, long arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_360_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_361_Delegate(long arg0, long arg1, bool arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_362_Delegate(long arg0, string arg1, string arg2, string arg3, int arg4, string arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_363_Delegate(long arg0, bool arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_364_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_365_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_366_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_367_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_368_Delegate(long arg0, long arg1, long arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_369_Delegate(long arg0, long arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_370_Delegate(long arg0, long arg1, double arg2, double arg3, bool arg4, int arg5);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_371_Delegate(long arg0, long arg1, double arg2, double arg3, double arg4);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_372_Delegate(long arg0, long arg1, int arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_373_Delegate(long arg0, int arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_374_Delegate(long arg0, int arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_375_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate int Function_376_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_377_Delegate(long arg0, long arg1, long arg2, string arg3, string arg4, string arg5, long arg6);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_378_Delegate(long arg0, long arg1, string arg2, string arg3, string arg4, string arg5, long arg6, int arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_379_Delegate(long arg0, long arg1, byte[] arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_380_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_381_Delegate(long arg0, string arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_382_Delegate(long arg0, string arg1, string arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_383_Delegate(long arg0, string arg1, int arg2, byte[] arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_384_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_385_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_386_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_387_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_388_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_389_Delegate(long arg0, int arg1, string arg2, int arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_390_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_391_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_392_Delegate(long arg0, long arg1, string arg2, bool arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_393_Delegate(long arg0, long arg1, string arg2, string arg3);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_394_Delegate(long arg0);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_395_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_396_Delegate(long arg0, string arg1);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_397_Delegate(long arg0, double arg1, double arg2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_398_Delegate();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate string Function_399_Delegate(long arg0, long arg1, long arg2, long arg3, string arg4, int arg5, long arg6, int arg7);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] 
        public delegate bool Function_400_Delegate(long arg0, string arg1);

        public static Function_1_Delegate Function_1_Action = new Function_1_Delegate(Function_1);
        public static Function_2_Delegate Function_2_Action = new Function_2_Delegate(Function_2);
        public static Function_3_Delegate Function_3_Action = new Function_3_Delegate(Function_3);
        public static Function_4_Delegate Function_4_Action = new Function_4_Delegate(Function_4);
        public static Function_5_Delegate Function_5_Action = new Function_5_Delegate(Function_5);
        public static Function_6_Delegate Function_6_Action = new Function_6_Delegate(Function_6);
        public static Function_7_Delegate Function_7_Action = new Function_7_Delegate(Function_7);
        public static Function_8_Delegate Function_8_Action = new Function_8_Delegate(Function_8);
        public static Function_9_Delegate Function_9_Action = new Function_9_Delegate(Function_9);
        public static Function_10_Delegate Function_10_Action = new Function_10_Delegate(Function_10);
        public static Function_11_Delegate Function_11_Action = new Function_11_Delegate(Function_11);
        public static Function_12_Delegate Function_12_Action = new Function_12_Delegate(Function_12);
        public static Function_13_Delegate Function_13_Action = new Function_13_Delegate(Function_13);
        public static Function_14_Delegate Function_14_Action = new Function_14_Delegate(Function_14);
        public static Function_15_Delegate Function_15_Action = new Function_15_Delegate(Function_15);
        public static Function_16_Delegate Function_16_Action = new Function_16_Delegate(Function_16);
        public static Function_17_Delegate Function_17_Action = new Function_17_Delegate(Function_17);
        public static Function_18_Delegate Function_18_Action = new Function_18_Delegate(Function_18);
        public static Function_19_Delegate Function_19_Action = new Function_19_Delegate(Function_19);
        public static Function_20_Delegate Function_20_Action = new Function_20_Delegate(Function_20);
        public static Function_21_Delegate Function_21_Action = new Function_21_Delegate(Function_21);
        public static Function_22_Delegate Function_22_Action = new Function_22_Delegate(Function_22);
        public static Function_23_Delegate Function_23_Action = new Function_23_Delegate(Function_23);
        public static Function_24_Delegate Function_24_Action = new Function_24_Delegate(Function_24);
        public static Function_25_Delegate Function_25_Action = new Function_25_Delegate(Function_25);
        public static Function_26_Delegate Function_26_Action = new Function_26_Delegate(Function_26);
        public static Function_27_Delegate Function_27_Action = new Function_27_Delegate(Function_27);
        public static Function_28_Delegate Function_28_Action = new Function_28_Delegate(Function_28);
        public static Function_29_Delegate Function_29_Action = new Function_29_Delegate(Function_29);
        public static Function_30_Delegate Function_30_Action = new Function_30_Delegate(Function_30);
        public static Function_31_Delegate Function_31_Action = new Function_31_Delegate(Function_31);
        public static Function_32_Delegate Function_32_Action = new Function_32_Delegate(Function_32);
        public static Function_33_Delegate Function_33_Action = new Function_33_Delegate(Function_33);
        public static Function_34_Delegate Function_34_Action = new Function_34_Delegate(Function_34);
        public static Function_35_Delegate Function_35_Action = new Function_35_Delegate(Function_35);
        public static Function_36_Delegate Function_36_Action = new Function_36_Delegate(Function_36);
        public static Function_37_Delegate Function_37_Action = new Function_37_Delegate(Function_37);
        public static Function_38_Delegate Function_38_Action = new Function_38_Delegate(Function_38);
        public static Function_39_Delegate Function_39_Action = new Function_39_Delegate(Function_39);
        public static Function_40_Delegate Function_40_Action = new Function_40_Delegate(Function_40);
        public static Function_41_Delegate Function_41_Action = new Function_41_Delegate(Function_41);
        public static Function_42_Delegate Function_42_Action = new Function_42_Delegate(Function_42);
        public static Function_43_Delegate Function_43_Action = new Function_43_Delegate(Function_43);
        public static Function_44_Delegate Function_44_Action = new Function_44_Delegate(Function_44);
        public static Function_45_Delegate Function_45_Action = new Function_45_Delegate(Function_45);
        public static Function_46_Delegate Function_46_Action = new Function_46_Delegate(Function_46);
        public static Function_47_Delegate Function_47_Action = new Function_47_Delegate(Function_47);
        public static Function_48_Delegate Function_48_Action = new Function_48_Delegate(Function_48);
        public static Function_49_Delegate Function_49_Action = new Function_49_Delegate(Function_49);
        public static Function_50_Delegate Function_50_Action = new Function_50_Delegate(Function_50);
        public static Function_51_Delegate Function_51_Action = new Function_51_Delegate(Function_51);
        public static Function_52_Delegate Function_52_Action = new Function_52_Delegate(Function_52);
        public static Function_53_Delegate Function_53_Action = new Function_53_Delegate(Function_53);
        public static Function_54_Delegate Function_54_Action = new Function_54_Delegate(Function_54);
        public static Function_55_Delegate Function_55_Action = new Function_55_Delegate(Function_55);
        public static Function_56_Delegate Function_56_Action = new Function_56_Delegate(Function_56);
        public static Function_57_Delegate Function_57_Action = new Function_57_Delegate(Function_57);
        public static Function_58_Delegate Function_58_Action = new Function_58_Delegate(Function_58);
        public static Function_59_Delegate Function_59_Action = new Function_59_Delegate(Function_59);
        public static Function_60_Delegate Function_60_Action = new Function_60_Delegate(Function_60);
        public static Function_61_Delegate Function_61_Action = new Function_61_Delegate(Function_61);
        public static Function_62_Delegate Function_62_Action = new Function_62_Delegate(Function_62);
        public static Function_63_Delegate Function_63_Action = new Function_63_Delegate(Function_63);
        public static Function_64_Delegate Function_64_Action = new Function_64_Delegate(Function_64);
        public static Function_65_Delegate Function_65_Action = new Function_65_Delegate(Function_65);
        public static Function_66_Delegate Function_66_Action = new Function_66_Delegate(Function_66);
        public static Function_67_Delegate Function_67_Action = new Function_67_Delegate(Function_67);
        public static Function_68_Delegate Function_68_Action = new Function_68_Delegate(Function_68);
        public static Function_69_Delegate Function_69_Action = new Function_69_Delegate(Function_69);
        public static Function_70_Delegate Function_70_Action = new Function_70_Delegate(Function_70);
        public static Function_71_Delegate Function_71_Action = new Function_71_Delegate(Function_71);
        public static Function_72_Delegate Function_72_Action = new Function_72_Delegate(Function_72);
        public static Function_73_Delegate Function_73_Action = new Function_73_Delegate(Function_73);
        public static Function_74_Delegate Function_74_Action = new Function_74_Delegate(Function_74);
        public static Function_75_Delegate Function_75_Action = new Function_75_Delegate(Function_75);
        public static Function_76_Delegate Function_76_Action = new Function_76_Delegate(Function_76);
        public static Function_77_Delegate Function_77_Action = new Function_77_Delegate(Function_77);
        public static Function_78_Delegate Function_78_Action = new Function_78_Delegate(Function_78);
        public static Function_79_Delegate Function_79_Action = new Function_79_Delegate(Function_79);
        public static Function_80_Delegate Function_80_Action = new Function_80_Delegate(Function_80);
        public static Function_81_Delegate Function_81_Action = new Function_81_Delegate(Function_81);
        public static Function_82_Delegate Function_82_Action = new Function_82_Delegate(Function_82);
        public static Function_83_Delegate Function_83_Action = new Function_83_Delegate(Function_83);
        public static Function_84_Delegate Function_84_Action = new Function_84_Delegate(Function_84);
        public static Function_85_Delegate Function_85_Action = new Function_85_Delegate(Function_85);
        public static Function_86_Delegate Function_86_Action = new Function_86_Delegate(Function_86);
        public static Function_87_Delegate Function_87_Action = new Function_87_Delegate(Function_87);
        public static Function_88_Delegate Function_88_Action = new Function_88_Delegate(Function_88);
        public static Function_89_Delegate Function_89_Action = new Function_89_Delegate(Function_89);
        public static Function_90_Delegate Function_90_Action = new Function_90_Delegate(Function_90);
        public static Function_91_Delegate Function_91_Action = new Function_91_Delegate(Function_91);
        public static Function_92_Delegate Function_92_Action = new Function_92_Delegate(Function_92);
        public static Function_93_Delegate Function_93_Action = new Function_93_Delegate(Function_93);
        public static Function_94_Delegate Function_94_Action = new Function_94_Delegate(Function_94);
        public static Function_95_Delegate Function_95_Action = new Function_95_Delegate(Function_95);
        public static Function_96_Delegate Function_96_Action = new Function_96_Delegate(Function_96);
        public static Function_97_Delegate Function_97_Action = new Function_97_Delegate(Function_97);
        public static Function_98_Delegate Function_98_Action = new Function_98_Delegate(Function_98);
        public static Function_99_Delegate Function_99_Action = new Function_99_Delegate(Function_99);
        public static Function_100_Delegate Function_100_Action = new Function_100_Delegate(Function_100);
        public static Function_101_Delegate Function_101_Action = new Function_101_Delegate(Function_101);
        public static Function_102_Delegate Function_102_Action = new Function_102_Delegate(Function_102);
        public static Function_103_Delegate Function_103_Action = new Function_103_Delegate(Function_103);
        public static Function_104_Delegate Function_104_Action = new Function_104_Delegate(Function_104);
        public static Function_105_Delegate Function_105_Action = new Function_105_Delegate(Function_105);
        public static Function_106_Delegate Function_106_Action = new Function_106_Delegate(Function_106);
        public static Function_107_Delegate Function_107_Action = new Function_107_Delegate(Function_107);
        public static Function_108_Delegate Function_108_Action = new Function_108_Delegate(Function_108);
        public static Function_109_Delegate Function_109_Action = new Function_109_Delegate(Function_109);
        public static Function_110_Delegate Function_110_Action = new Function_110_Delegate(Function_110);
        public static Function_111_Delegate Function_111_Action = new Function_111_Delegate(Function_111);
        public static Function_112_Delegate Function_112_Action = new Function_112_Delegate(Function_112);
        public static Function_113_Delegate Function_113_Action = new Function_113_Delegate(Function_113);
        public static Function_114_Delegate Function_114_Action = new Function_114_Delegate(Function_114);
        public static Function_115_Delegate Function_115_Action = new Function_115_Delegate(Function_115);
        public static Function_116_Delegate Function_116_Action = new Function_116_Delegate(Function_116);
        public static Function_117_Delegate Function_117_Action = new Function_117_Delegate(Function_117);
        public static Function_118_Delegate Function_118_Action = new Function_118_Delegate(Function_118);
        public static Function_119_Delegate Function_119_Action = new Function_119_Delegate(Function_119);
        public static Function_120_Delegate Function_120_Action = new Function_120_Delegate(Function_120);
        public static Function_121_Delegate Function_121_Action = new Function_121_Delegate(Function_121);
        public static Function_122_Delegate Function_122_Action = new Function_122_Delegate(Function_122);
        public static Function_123_Delegate Function_123_Action = new Function_123_Delegate(Function_123);
        public static Function_124_Delegate Function_124_Action = new Function_124_Delegate(Function_124);
        public static Function_125_Delegate Function_125_Action = new Function_125_Delegate(Function_125);
        public static Function_126_Delegate Function_126_Action = new Function_126_Delegate(Function_126);
        public static Function_127_Delegate Function_127_Action = new Function_127_Delegate(Function_127);
        public static Function_128_Delegate Function_128_Action = new Function_128_Delegate(Function_128);
        public static Function_129_Delegate Function_129_Action = new Function_129_Delegate(Function_129);
        public static Function_130_Delegate Function_130_Action = new Function_130_Delegate(Function_130);
        public static Function_131_Delegate Function_131_Action = new Function_131_Delegate(Function_131);
        public static Function_132_Delegate Function_132_Action = new Function_132_Delegate(Function_132);
        public static Function_133_Delegate Function_133_Action = new Function_133_Delegate(Function_133);
        public static Function_134_Delegate Function_134_Action = new Function_134_Delegate(Function_134);
        public static Function_135_Delegate Function_135_Action = new Function_135_Delegate(Function_135);
        public static Function_136_Delegate Function_136_Action = new Function_136_Delegate(Function_136);
        public static Function_137_Delegate Function_137_Action = new Function_137_Delegate(Function_137);
        public static Function_138_Delegate Function_138_Action = new Function_138_Delegate(Function_138);
        public static Function_139_Delegate Function_139_Action = new Function_139_Delegate(Function_139);
        public static Function_140_Delegate Function_140_Action = new Function_140_Delegate(Function_140);
        public static Function_141_Delegate Function_141_Action = new Function_141_Delegate(Function_141);
        public static Function_142_Delegate Function_142_Action = new Function_142_Delegate(Function_142);
        public static Function_143_Delegate Function_143_Action = new Function_143_Delegate(Function_143);
        public static Function_144_Delegate Function_144_Action = new Function_144_Delegate(Function_144);
        public static Function_145_Delegate Function_145_Action = new Function_145_Delegate(Function_145);
        public static Function_146_Delegate Function_146_Action = new Function_146_Delegate(Function_146);
        public static Function_147_Delegate Function_147_Action = new Function_147_Delegate(Function_147);
        public static Function_148_Delegate Function_148_Action = new Function_148_Delegate(Function_148);
        public static Function_149_Delegate Function_149_Action = new Function_149_Delegate(Function_149);
        public static Function_150_Delegate Function_150_Action = new Function_150_Delegate(Function_150);
        public static Function_151_Delegate Function_151_Action = new Function_151_Delegate(Function_151);
        public static Function_152_Delegate Function_152_Action = new Function_152_Delegate(Function_152);
        public static Function_153_Delegate Function_153_Action = new Function_153_Delegate(Function_153);
        public static Function_154_Delegate Function_154_Action = new Function_154_Delegate(Function_154);
        public static Function_155_Delegate Function_155_Action = new Function_155_Delegate(Function_155);
        public static Function_156_Delegate Function_156_Action = new Function_156_Delegate(Function_156);
        public static Function_157_Delegate Function_157_Action = new Function_157_Delegate(Function_157);
        public static Function_158_Delegate Function_158_Action = new Function_158_Delegate(Function_158);
        public static Function_159_Delegate Function_159_Action = new Function_159_Delegate(Function_159);
        public static Function_160_Delegate Function_160_Action = new Function_160_Delegate(Function_160);
        public static Function_161_Delegate Function_161_Action = new Function_161_Delegate(Function_161);
        public static Function_162_Delegate Function_162_Action = new Function_162_Delegate(Function_162);
        public static Function_163_Delegate Function_163_Action = new Function_163_Delegate(Function_163);
        public static Function_164_Delegate Function_164_Action = new Function_164_Delegate(Function_164);
        public static Function_165_Delegate Function_165_Action = new Function_165_Delegate(Function_165);
        public static Function_166_Delegate Function_166_Action = new Function_166_Delegate(Function_166);
        public static Function_167_Delegate Function_167_Action = new Function_167_Delegate(Function_167);
        public static Function_168_Delegate Function_168_Action = new Function_168_Delegate(Function_168);
        public static Function_169_Delegate Function_169_Action = new Function_169_Delegate(Function_169);
        public static Function_170_Delegate Function_170_Action = new Function_170_Delegate(Function_170);
        public static Function_171_Delegate Function_171_Action = new Function_171_Delegate(Function_171);
        public static Function_172_Delegate Function_172_Action = new Function_172_Delegate(Function_172);
        public static Function_173_Delegate Function_173_Action = new Function_173_Delegate(Function_173);
        public static Function_174_Delegate Function_174_Action = new Function_174_Delegate(Function_174);
        public static Function_175_Delegate Function_175_Action = new Function_175_Delegate(Function_175);
        public static Function_176_Delegate Function_176_Action = new Function_176_Delegate(Function_176);
        public static Function_177_Delegate Function_177_Action = new Function_177_Delegate(Function_177);
        public static Function_178_Delegate Function_178_Action = new Function_178_Delegate(Function_178);
        public static Function_179_Delegate Function_179_Action = new Function_179_Delegate(Function_179);
        public static Function_180_Delegate Function_180_Action = new Function_180_Delegate(Function_180);
        public static Function_181_Delegate Function_181_Action = new Function_181_Delegate(Function_181);
        public static Function_182_Delegate Function_182_Action = new Function_182_Delegate(Function_182);
        public static Function_183_Delegate Function_183_Action = new Function_183_Delegate(Function_183);
        public static Function_184_Delegate Function_184_Action = new Function_184_Delegate(Function_184);
        public static Function_185_Delegate Function_185_Action = new Function_185_Delegate(Function_185);
        public static Function_186_Delegate Function_186_Action = new Function_186_Delegate(Function_186);
        public static Function_187_Delegate Function_187_Action = new Function_187_Delegate(Function_187);
        public static Function_188_Delegate Function_188_Action = new Function_188_Delegate(Function_188);
        public static Function_189_Delegate Function_189_Action = new Function_189_Delegate(Function_189);
        public static Function_190_Delegate Function_190_Action = new Function_190_Delegate(Function_190);
        public static Function_191_Delegate Function_191_Action = new Function_191_Delegate(Function_191);
        public static Function_192_Delegate Function_192_Action = new Function_192_Delegate(Function_192);
        public static Function_193_Delegate Function_193_Action = new Function_193_Delegate(Function_193);
        public static Function_194_Delegate Function_194_Action = new Function_194_Delegate(Function_194);
        public static Function_195_Delegate Function_195_Action = new Function_195_Delegate(Function_195);
        public static Function_196_Delegate Function_196_Action = new Function_196_Delegate(Function_196);
        public static Function_197_Delegate Function_197_Action = new Function_197_Delegate(Function_197);
        public static Function_198_Delegate Function_198_Action = new Function_198_Delegate(Function_198);
        public static Function_199_Delegate Function_199_Action = new Function_199_Delegate(Function_199);
        public static Function_200_Delegate Function_200_Action = new Function_200_Delegate(Function_200);
        public static Function_201_Delegate Function_201_Action = new Function_201_Delegate(Function_201);
        public static Function_202_Delegate Function_202_Action = new Function_202_Delegate(Function_202);
        public static Function_203_Delegate Function_203_Action = new Function_203_Delegate(Function_203);
        public static Function_204_Delegate Function_204_Action = new Function_204_Delegate(Function_204);
        public static Function_205_Delegate Function_205_Action = new Function_205_Delegate(Function_205);
        public static Function_206_Delegate Function_206_Action = new Function_206_Delegate(Function_206);
        public static Function_207_Delegate Function_207_Action = new Function_207_Delegate(Function_207);
        public static Function_208_Delegate Function_208_Action = new Function_208_Delegate(Function_208);
        public static Function_209_Delegate Function_209_Action = new Function_209_Delegate(Function_209);
        public static Function_210_Delegate Function_210_Action = new Function_210_Delegate(Function_210);
        public static Function_211_Delegate Function_211_Action = new Function_211_Delegate(Function_211);
        public static Function_212_Delegate Function_212_Action = new Function_212_Delegate(Function_212);
        public static Function_213_Delegate Function_213_Action = new Function_213_Delegate(Function_213);
        public static Function_214_Delegate Function_214_Action = new Function_214_Delegate(Function_214);
        public static Function_215_Delegate Function_215_Action = new Function_215_Delegate(Function_215);
        public static Function_216_Delegate Function_216_Action = new Function_216_Delegate(Function_216);
        public static Function_217_Delegate Function_217_Action = new Function_217_Delegate(Function_217);
        public static Function_218_Delegate Function_218_Action = new Function_218_Delegate(Function_218);
        public static Function_219_Delegate Function_219_Action = new Function_219_Delegate(Function_219);
        public static Function_220_Delegate Function_220_Action = new Function_220_Delegate(Function_220);
        public static Function_221_Delegate Function_221_Action = new Function_221_Delegate(Function_221);
        public static Function_222_Delegate Function_222_Action = new Function_222_Delegate(Function_222);
        public static Function_223_Delegate Function_223_Action = new Function_223_Delegate(Function_223);
        public static Function_224_Delegate Function_224_Action = new Function_224_Delegate(Function_224);
        public static Function_225_Delegate Function_225_Action = new Function_225_Delegate(Function_225);
        public static Function_226_Delegate Function_226_Action = new Function_226_Delegate(Function_226);
        public static Function_227_Delegate Function_227_Action = new Function_227_Delegate(Function_227);
        public static Function_228_Delegate Function_228_Action = new Function_228_Delegate(Function_228);
        public static Function_229_Delegate Function_229_Action = new Function_229_Delegate(Function_229);
        public static Function_230_Delegate Function_230_Action = new Function_230_Delegate(Function_230);
        public static Function_231_Delegate Function_231_Action = new Function_231_Delegate(Function_231);
        public static Function_232_Delegate Function_232_Action = new Function_232_Delegate(Function_232);
        public static Function_233_Delegate Function_233_Action = new Function_233_Delegate(Function_233);
        public static Function_234_Delegate Function_234_Action = new Function_234_Delegate(Function_234);
        public static Function_235_Delegate Function_235_Action = new Function_235_Delegate(Function_235);
        public static Function_236_Delegate Function_236_Action = new Function_236_Delegate(Function_236);
        public static Function_237_Delegate Function_237_Action = new Function_237_Delegate(Function_237);
        public static Function_238_Delegate Function_238_Action = new Function_238_Delegate(Function_238);
        public static Function_239_Delegate Function_239_Action = new Function_239_Delegate(Function_239);
        public static Function_240_Delegate Function_240_Action = new Function_240_Delegate(Function_240);
        public static Function_241_Delegate Function_241_Action = new Function_241_Delegate(Function_241);
        public static Function_242_Delegate Function_242_Action = new Function_242_Delegate(Function_242);
        public static Function_243_Delegate Function_243_Action = new Function_243_Delegate(Function_243);
        public static Function_244_Delegate Function_244_Action = new Function_244_Delegate(Function_244);
        public static Function_245_Delegate Function_245_Action = new Function_245_Delegate(Function_245);
        public static Function_246_Delegate Function_246_Action = new Function_246_Delegate(Function_246);
        public static Function_247_Delegate Function_247_Action = new Function_247_Delegate(Function_247);
        public static Function_248_Delegate Function_248_Action = new Function_248_Delegate(Function_248);
        public static Function_249_Delegate Function_249_Action = new Function_249_Delegate(Function_249);
        public static Function_250_Delegate Function_250_Action = new Function_250_Delegate(Function_250);
        public static Function_251_Delegate Function_251_Action = new Function_251_Delegate(Function_251);
        public static Function_252_Delegate Function_252_Action = new Function_252_Delegate(Function_252);
        public static Function_253_Delegate Function_253_Action = new Function_253_Delegate(Function_253);
        public static Function_254_Delegate Function_254_Action = new Function_254_Delegate(Function_254);
        public static Function_255_Delegate Function_255_Action = new Function_255_Delegate(Function_255);
        public static Function_256_Delegate Function_256_Action = new Function_256_Delegate(Function_256);
        public static Function_257_Delegate Function_257_Action = new Function_257_Delegate(Function_257);
        public static Function_258_Delegate Function_258_Action = new Function_258_Delegate(Function_258);
        public static Function_259_Delegate Function_259_Action = new Function_259_Delegate(Function_259);
        public static Function_260_Delegate Function_260_Action = new Function_260_Delegate(Function_260);
        public static Function_261_Delegate Function_261_Action = new Function_261_Delegate(Function_261);
        public static Function_262_Delegate Function_262_Action = new Function_262_Delegate(Function_262);
        public static Function_263_Delegate Function_263_Action = new Function_263_Delegate(Function_263);
        public static Function_264_Delegate Function_264_Action = new Function_264_Delegate(Function_264);
        public static Function_265_Delegate Function_265_Action = new Function_265_Delegate(Function_265);
        public static Function_266_Delegate Function_266_Action = new Function_266_Delegate(Function_266);
        public static Function_267_Delegate Function_267_Action = new Function_267_Delegate(Function_267);
        public static Function_268_Delegate Function_268_Action = new Function_268_Delegate(Function_268);
        public static Function_269_Delegate Function_269_Action = new Function_269_Delegate(Function_269);
        public static Function_270_Delegate Function_270_Action = new Function_270_Delegate(Function_270);
        public static Function_271_Delegate Function_271_Action = new Function_271_Delegate(Function_271);
        public static Function_272_Delegate Function_272_Action = new Function_272_Delegate(Function_272);
        public static Function_273_Delegate Function_273_Action = new Function_273_Delegate(Function_273);
        public static Function_274_Delegate Function_274_Action = new Function_274_Delegate(Function_274);
        public static Function_275_Delegate Function_275_Action = new Function_275_Delegate(Function_275);
        public static Function_276_Delegate Function_276_Action = new Function_276_Delegate(Function_276);
        public static Function_277_Delegate Function_277_Action = new Function_277_Delegate(Function_277);
        public static Function_278_Delegate Function_278_Action = new Function_278_Delegate(Function_278);
        public static Function_279_Delegate Function_279_Action = new Function_279_Delegate(Function_279);
        public static Function_280_Delegate Function_280_Action = new Function_280_Delegate(Function_280);
        public static Function_281_Delegate Function_281_Action = new Function_281_Delegate(Function_281);
        public static Function_282_Delegate Function_282_Action = new Function_282_Delegate(Function_282);
        public static Function_283_Delegate Function_283_Action = new Function_283_Delegate(Function_283);
        public static Function_284_Delegate Function_284_Action = new Function_284_Delegate(Function_284);
        public static Function_285_Delegate Function_285_Action = new Function_285_Delegate(Function_285);
        public static Function_286_Delegate Function_286_Action = new Function_286_Delegate(Function_286);
        public static Function_287_Delegate Function_287_Action = new Function_287_Delegate(Function_287);
        public static Function_288_Delegate Function_288_Action = new Function_288_Delegate(Function_288);
        public static Function_289_Delegate Function_289_Action = new Function_289_Delegate(Function_289);
        public static Function_290_Delegate Function_290_Action = new Function_290_Delegate(Function_290);
        public static Function_291_Delegate Function_291_Action = new Function_291_Delegate(Function_291);
        public static Function_292_Delegate Function_292_Action = new Function_292_Delegate(Function_292);
        public static Function_293_Delegate Function_293_Action = new Function_293_Delegate(Function_293);
        public static Function_294_Delegate Function_294_Action = new Function_294_Delegate(Function_294);
        public static Function_295_Delegate Function_295_Action = new Function_295_Delegate(Function_295);
        public static Function_296_Delegate Function_296_Action = new Function_296_Delegate(Function_296);
        public static Function_297_Delegate Function_297_Action = new Function_297_Delegate(Function_297);
        public static Function_298_Delegate Function_298_Action = new Function_298_Delegate(Function_298);
        public static Function_299_Delegate Function_299_Action = new Function_299_Delegate(Function_299);
        public static Function_300_Delegate Function_300_Action = new Function_300_Delegate(Function_300);
        public static Function_301_Delegate Function_301_Action = new Function_301_Delegate(Function_301);
        public static Function_302_Delegate Function_302_Action = new Function_302_Delegate(Function_302);
        public static Function_303_Delegate Function_303_Action = new Function_303_Delegate(Function_303);
        public static Function_304_Delegate Function_304_Action = new Function_304_Delegate(Function_304);
        public static Function_305_Delegate Function_305_Action = new Function_305_Delegate(Function_305);
        public static Function_306_Delegate Function_306_Action = new Function_306_Delegate(Function_306);
        public static Function_307_Delegate Function_307_Action = new Function_307_Delegate(Function_307);
        public static Function_308_Delegate Function_308_Action = new Function_308_Delegate(Function_308);
        public static Function_309_Delegate Function_309_Action = new Function_309_Delegate(Function_309);
        public static Function_310_Delegate Function_310_Action = new Function_310_Delegate(Function_310);
        public static Function_311_Delegate Function_311_Action = new Function_311_Delegate(Function_311);
        public static Function_312_Delegate Function_312_Action = new Function_312_Delegate(Function_312);
        public static Function_313_Delegate Function_313_Action = new Function_313_Delegate(Function_313);
        public static Function_314_Delegate Function_314_Action = new Function_314_Delegate(Function_314);
        public static Function_315_Delegate Function_315_Action = new Function_315_Delegate(Function_315);
        public static Function_316_Delegate Function_316_Action = new Function_316_Delegate(Function_316);
        public static Function_317_Delegate Function_317_Action = new Function_317_Delegate(Function_317);
        public static Function_318_Delegate Function_318_Action = new Function_318_Delegate(Function_318);
        public static Function_319_Delegate Function_319_Action = new Function_319_Delegate(Function_319);
        public static Function_320_Delegate Function_320_Action = new Function_320_Delegate(Function_320);
        public static Function_321_Delegate Function_321_Action = new Function_321_Delegate(Function_321);
        public static Function_322_Delegate Function_322_Action = new Function_322_Delegate(Function_322);
        public static Function_323_Delegate Function_323_Action = new Function_323_Delegate(Function_323);
        public static Function_324_Delegate Function_324_Action = new Function_324_Delegate(Function_324);
        public static Function_325_Delegate Function_325_Action = new Function_325_Delegate(Function_325);
        public static Function_326_Delegate Function_326_Action = new Function_326_Delegate(Function_326);
        public static Function_327_Delegate Function_327_Action = new Function_327_Delegate(Function_327);
        public static Function_328_Delegate Function_328_Action = new Function_328_Delegate(Function_328);
        public static Function_329_Delegate Function_329_Action = new Function_329_Delegate(Function_329);
        public static Function_330_Delegate Function_330_Action = new Function_330_Delegate(Function_330);
        public static Function_331_Delegate Function_331_Action = new Function_331_Delegate(Function_331);
        public static Function_332_Delegate Function_332_Action = new Function_332_Delegate(Function_332);
        public static Function_333_Delegate Function_333_Action = new Function_333_Delegate(Function_333);
        public static Function_334_Delegate Function_334_Action = new Function_334_Delegate(Function_334);
        public static Function_335_Delegate Function_335_Action = new Function_335_Delegate(Function_335);
        public static Function_336_Delegate Function_336_Action = new Function_336_Delegate(Function_336);
        public static Function_337_Delegate Function_337_Action = new Function_337_Delegate(Function_337);
        public static Function_338_Delegate Function_338_Action = new Function_338_Delegate(Function_338);
        public static Function_339_Delegate Function_339_Action = new Function_339_Delegate(Function_339);
        public static Function_340_Delegate Function_340_Action = new Function_340_Delegate(Function_340);
        public static Function_341_Delegate Function_341_Action = new Function_341_Delegate(Function_341);
        public static Function_342_Delegate Function_342_Action = new Function_342_Delegate(Function_342);
        public static Function_343_Delegate Function_343_Action = new Function_343_Delegate(Function_343);
        public static Function_344_Delegate Function_344_Action = new Function_344_Delegate(Function_344);
        public static Function_345_Delegate Function_345_Action = new Function_345_Delegate(Function_345);
        public static Function_346_Delegate Function_346_Action = new Function_346_Delegate(Function_346);
        public static Function_347_Delegate Function_347_Action = new Function_347_Delegate(Function_347);
        public static Function_348_Delegate Function_348_Action = new Function_348_Delegate(Function_348);
        public static Function_349_Delegate Function_349_Action = new Function_349_Delegate(Function_349);
        public static Function_350_Delegate Function_350_Action = new Function_350_Delegate(Function_350);
        public static Function_351_Delegate Function_351_Action = new Function_351_Delegate(Function_351);
        public static Function_352_Delegate Function_352_Action = new Function_352_Delegate(Function_352);
        public static Function_353_Delegate Function_353_Action = new Function_353_Delegate(Function_353);
        public static Function_354_Delegate Function_354_Action = new Function_354_Delegate(Function_354);
        public static Function_355_Delegate Function_355_Action = new Function_355_Delegate(Function_355);
        public static Function_356_Delegate Function_356_Action = new Function_356_Delegate(Function_356);
        public static Function_357_Delegate Function_357_Action = new Function_357_Delegate(Function_357);
        public static Function_358_Delegate Function_358_Action = new Function_358_Delegate(Function_358);
        public static Function_359_Delegate Function_359_Action = new Function_359_Delegate(Function_359);
        public static Function_360_Delegate Function_360_Action = new Function_360_Delegate(Function_360);
        public static Function_361_Delegate Function_361_Action = new Function_361_Delegate(Function_361);
        public static Function_362_Delegate Function_362_Action = new Function_362_Delegate(Function_362);
        public static Function_363_Delegate Function_363_Action = new Function_363_Delegate(Function_363);
        public static Function_364_Delegate Function_364_Action = new Function_364_Delegate(Function_364);
        public static Function_365_Delegate Function_365_Action = new Function_365_Delegate(Function_365);
        public static Function_366_Delegate Function_366_Action = new Function_366_Delegate(Function_366);
        public static Function_367_Delegate Function_367_Action = new Function_367_Delegate(Function_367);
        public static Function_368_Delegate Function_368_Action = new Function_368_Delegate(Function_368);
        public static Function_369_Delegate Function_369_Action = new Function_369_Delegate(Function_369);
        public static Function_370_Delegate Function_370_Action = new Function_370_Delegate(Function_370);
        public static Function_371_Delegate Function_371_Action = new Function_371_Delegate(Function_371);
        public static Function_372_Delegate Function_372_Action = new Function_372_Delegate(Function_372);
        public static Function_373_Delegate Function_373_Action = new Function_373_Delegate(Function_373);
        public static Function_374_Delegate Function_374_Action = new Function_374_Delegate(Function_374);
        public static Function_375_Delegate Function_375_Action = new Function_375_Delegate(Function_375);
        public static Function_376_Delegate Function_376_Action = new Function_376_Delegate(Function_376);
        public static Function_377_Delegate Function_377_Action = new Function_377_Delegate(Function_377);
        public static Function_378_Delegate Function_378_Action = new Function_378_Delegate(Function_378);
        public static Function_379_Delegate Function_379_Action = new Function_379_Delegate(Function_379);
        public static Function_380_Delegate Function_380_Action = new Function_380_Delegate(Function_380);
        public static Function_381_Delegate Function_381_Action = new Function_381_Delegate(Function_381);
        public static Function_382_Delegate Function_382_Action = new Function_382_Delegate(Function_382);
        public static Function_383_Delegate Function_383_Action = new Function_383_Delegate(Function_383);
        public static Function_384_Delegate Function_384_Action = new Function_384_Delegate(Function_384);
        public static Function_385_Delegate Function_385_Action = new Function_385_Delegate(Function_385);
        public static Function_386_Delegate Function_386_Action = new Function_386_Delegate(Function_386);
        public static Function_387_Delegate Function_387_Action = new Function_387_Delegate(Function_387);
        public static Function_388_Delegate Function_388_Action = new Function_388_Delegate(Function_388);
        public static Function_389_Delegate Function_389_Action = new Function_389_Delegate(Function_389);
        public static Function_390_Delegate Function_390_Action = new Function_390_Delegate(Function_390);
        public static Function_391_Delegate Function_391_Action = new Function_391_Delegate(Function_391);
        public static Function_392_Delegate Function_392_Action = new Function_392_Delegate(Function_392);
        public static Function_393_Delegate Function_393_Action = new Function_393_Delegate(Function_393);
        public static Function_394_Delegate Function_394_Action = new Function_394_Delegate(Function_394);
        public static Function_395_Delegate Function_395_Action = new Function_395_Delegate(Function_395);
        public static Function_396_Delegate Function_396_Action = new Function_396_Delegate(Function_396);
        public static Function_397_Delegate Function_397_Action = new Function_397_Delegate(Function_397);
        public static Function_398_Delegate Function_398_Action = new Function_398_Delegate(Function_398);
        public static Function_399_Delegate Function_399_Action = new Function_399_Delegate(Function_399);
        public static Function_400_Delegate Function_400_Action = new Function_400_Delegate(Function_400);

        #endregion

        /// <summary>
        /// _初始化
        /// </summary>
        [ProxyAPIName("_初始化")]
        public static void Function_1()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API _初始化");
            }
        }

        /// <summary>
        /// _销毁
        /// </summary>
        [ProxyAPIName("_销毁")]
        public static void Function_2()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API _销毁");
            }
        }

        /// <summary>
        /// 取API函数地址
        /// <param name="arg0">函数名</param>
        /// </summary>
        [ProxyAPIName("")]
        public static int Function_3(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取API函数地址");
            }
            return 0;
        }

        /// <summary>
        /// int
        /// <param name="arg0">_pluginkey</param>
        /// <param name="arg1">_apidata</param>
        /// </summary>
        [ProxyAPIName("int")]
        public static void Function_4(string arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API int");
            }
        }

        /// <summary>
        /// 输出日志
        /// <param name="arg0">日志</param>
        /// <param name="arg1">文字颜色</param>
        /// <param name="arg2">背景颜色</param>
        /// </summary>
        [ProxyAPIName("输出日志")]
        public static string Function_5(string arg0, int arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 输出日志");
            }
            return "";
        }

        /// <summary>
        /// 发送好友消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">发送内容</param>
        /// <param name="arg3">Random</param>
        /// <param name="arg4">Req</param>
        /// </summary>
        [ProxyAPIName("发送好友消息")]
        public static string Function_6(long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送好友消息");
            }
            return "";
        }

        /// <summary>
        /// 发送群消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">发送内容</param>
        /// <param name="arg3">匿名发送</param>
        /// </summary>
        [ProxyAPIName("发送群消息")]
        public static string Function_7(long arg0, long arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群消息");
            }
            return "";
        }

        /// <summary>
        /// 发送群临时消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群ID</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">发送内容</param>
        /// <param name="arg4">Random</param>
        /// <param name="arg5">Req</param>
        /// </summary>
        [ProxyAPIName("发送群临时消息")]
        public static string Function_8(long arg0, long arg1, long arg2, string arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群临时消息");
            }
            return "";
        }

        /// <summary>
        /// 添加好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">问题答案</param>
        /// <param name="arg3">备注</param>
        /// </summary>
        [ProxyAPIName("添加好友")]
        public static string Function_9(long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 添加好友");
            }
            return "";
        }

        /// <summary>
        /// 添加群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">验证消息</param>
        /// </summary>
        [ProxyAPIName("添加群")]
        public static string Function_10(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 添加群");
            }
            return "";
        }

        /// <summary>
        /// 删除好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("删除好友")]
        public static string Function_11(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除好友");
            }
            return "";
        }

        /// <summary>
        /// 置屏蔽好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">是否屏蔽</param>
        /// </summary>
        [ProxyAPIName("置屏蔽好友")]
        public static string Function_12(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置屏蔽好友");
            }
            return "";
        }

        /// <summary>
        /// 置特别关心好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">是否关心</param>
        /// </summary>
        [ProxyAPIName("置特别关心好友")]
        public static string Function_13(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置特别关心好友");
            }
            return "";
        }

        /// <summary>
        /// 发送好友json消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">json代码</param>
        /// <param name="arg3">Random</param>
        /// <param name="arg4">Req</param>
        /// </summary>
        [ProxyAPIName("发送好友json消息")]
        public static string Function_14(long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送好友json消息");
            }
            return "";
        }

        /// <summary>
        /// 发送群json消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">json代码</param>
        /// <param name="arg3">匿名发送</param>
        /// </summary>
        [ProxyAPIName("发送群json消息")]
        public static string Function_15(long arg0, long arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群json消息");
            }
            return "";
        }

        /// <summary>
        /// 上传好友图片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">是否闪照</param>
        /// <param name="arg3">pic</param>
        /// <param name="arg4">宽度</param>
        /// <param name="arg5">高度</param>
        /// <param name="arg6">动图</param>
        /// </summary>
        [ProxyAPIName("上传好友图片")]
        public static string Function_16(long arg0, long arg1, bool arg2, byte[] arg3, int arg4, int arg5, bool arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传好友图片");
            }
            return "";
        }

        /// <summary>
        /// 上传群图片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否闪照</param>
        /// <param name="arg3">pic</param>
        /// <param name="arg4">宽度</param>
        /// <param name="arg5">高度</param>
        /// <param name="arg6">动图</param>
        /// </summary>
        [ProxyAPIName("上传群图片")]
        public static string Function_17(long arg0, long arg1, bool arg2, byte[] arg3, int arg4, int arg5, bool arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群图片");
            }
            return "";
        }

        /// <summary>
        /// 上传好友语音
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">语音类型</param>
        /// <param name="arg3">语音文字</param>
        /// <param name="arg4">audio</param>
        /// <param name="arg5">时长</param>
        /// </summary>
        [ProxyAPIName("上传好友语音")]
        public static string Function_18(long arg0, long arg1, int arg2, string arg3, byte[] arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传好友语音");
            }
            return "";
        }

        /// <summary>
        /// 上传群语音
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">语音类型</param>
        /// <param name="arg3">语音文字</param>
        /// <param name="arg4">audio</param>
        /// <param name="arg5">时长</param>
        /// </summary>
        [ProxyAPIName("上传群语音")]
        public static string Function_19(long arg0, long arg1, int arg2, string arg3, byte[] arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群语音");
            }
            return "";
        }

        /// <summary>
        /// 上传头像
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">pic</param>
        /// </summary>
        [ProxyAPIName("上传头像")]
        public static string Function_20(long arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传头像");
            }
            return "";
        }

        /// <summary>
        /// silk解码
        /// <param name="arg0">音频文件路径</param>
        /// </summary>
        [ProxyAPIName("silk解码")]
        public static IntPtr Function_21(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API silk解码");
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// silk编码
        /// <param name="arg0">音频文件路径</param>
        /// </summary>
        [ProxyAPIName("silk编码")]
        public static IntPtr Function_22(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API silk编码");
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// amr编码
        /// <param name="arg0">音频文件路径</param>
        /// </summary>
        [ProxyAPIName("amr编码")]
        public static IntPtr Function_23(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API amr编码");
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 设置群名片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">新名片</param>
        /// </summary>
        [ProxyAPIName("设置群名片")]
        public static string Function_24(long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置群名片");
            }
            return "";
        }

        /// <summary>
        /// 取昵称_从缓存
        /// <param name="arg0">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取昵称_从缓存")]
        public static string Function_25(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取昵称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 强制取昵称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("强制取昵称")]
        public static string Function_26(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 强制取昵称");
            }
            return "";
        }

        /// <summary>
        /// 取群名称_从缓存
        /// <param name="arg0">群号</param>
        /// </summary>
        [ProxyAPIName("取群名称_从缓存")]
        public static string Function_27(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群名称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 获取skey
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("获取skey")]
        public static string Function_28(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取skey");
            }
            return "";
        }

        /// <summary>
        /// 获取pskey
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">域</param>
        /// </summary>
        [ProxyAPIName("获取pskey")]
        public static string Function_29(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取pskey");
            }
            return "";
        }

        /// <summary>
        /// 获取clientkey
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("获取clientkey")]
        public static string Function_30(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取clientkey");
            }
            return "";
        }

        /// <summary>
        /// 取框架QQ
        /// </summary>
        [ProxyAPIName("取框架QQ")]
        public static string Function_31()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架QQ");
            }
            return "";
        }

        /// <summary>
        /// 取好友列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取好友列表")]
        public static int Function_32(long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友列表");
            }
            return 0;
        }

        /// <summary>
        /// 取群列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取群列表")]
        public static int Function_33(long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群列表");
            }
            return 0;
        }

        /// <summary>
        /// 取群成员列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取群成员列表")]
        public static int Function_34(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群成员列表");
            }
            return 0;
        }

        /// <summary>
        /// 设置管理员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">取消管理</param>
        /// </summary>
        [ProxyAPIName("设置管理员")]
        public static bool Function_35(long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置管理员");
            }
            return false;
        }

        /// <summary>
        /// 取管理层列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取管理层列表")]
        public static string Function_36(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取管理层列表");
            }
            return "";
        }

        /// <summary>
        /// 取群名片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// </summary>
        [ProxyAPIName("取群名片")]
        public static string Function_37(long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群名片");
            }
            return "";
        }

        /// <summary>
        /// 取个性签名
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取个性签名")]
        public static string Function_38(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取个性签名");
            }
            return "";
        }

        /// <summary>
        /// 修改昵称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">昵称</param>
        /// </summary>
        [ProxyAPIName("修改昵称")]
        public static bool Function_39(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改昵称");
            }
            return false;
        }

        /// <summary>
        /// 修改个性签名
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">签名</param>
        /// <param name="arg2">签名地点</param>
        /// </summary>
        [ProxyAPIName("修改个性签名")]
        public static bool Function_40(long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改个性签名");
            }
            return false;
        }

        /// <summary>
        /// 删除群成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">拒绝加群申请</param>
        /// </summary>
        [ProxyAPIName("删除群成员")]
        public static bool Function_41(long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群成员");
            }
            return false;
        }

        /// <summary>
        /// 禁言群成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">禁言时长</param>
        /// </summary>
        [ProxyAPIName("禁言群成员")]
        public static bool Function_42(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 禁言群成员");
            }
            return false;
        }

        /// <summary>
        /// 退群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("退群")]
        public static bool Function_43(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 退群");
            }
            return false;
        }

        /// <summary>
        /// 解散群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("解散群")]
        public static bool Function_44(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 解散群");
            }
            return false;
        }

        /// <summary>
        /// 上传群头像
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">pic</param>
        /// </summary>
        [ProxyAPIName("上传群头像")]
        public static bool Function_45(long arg0, long arg1, byte[] arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群头像");
            }
            return false;
        }

        /// <summary>
        /// 全员禁言
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否开启</param>
        /// </summary>
        [ProxyAPIName("全员禁言")]
        public static bool Function_46(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 全员禁言");
            }
            return false;
        }

        /// <summary>
        /// 群权限_发起新的群聊
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_发起新的群聊")]
        public static bool Function_47(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_发起新的群聊");
            }
            return false;
        }

        /// <summary>
        /// 群权限_发起临时会话
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_发起临时会话")]
        public static bool Function_48(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_发起临时会话");
            }
            return false;
        }

        /// <summary>
        /// 群权限_上传文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_上传文件")]
        public static bool Function_49(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_上传文件");
            }
            return false;
        }

        /// <summary>
        /// 群权限_上传相册
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_上传相册")]
        public static bool Function_50(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_上传相册");
            }
            return false;
        }

        /// <summary>
        /// 群权限_邀请好友加群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_邀请好友加群")]
        public static bool Function_51(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_邀请好友加群");
            }
            return false;
        }

        /// <summary>
        /// 群权限_匿名聊天
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_匿名聊天")]
        public static bool Function_52(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_匿名聊天");
            }
            return false;
        }

        /// <summary>
        /// 群权限_坦白说
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_坦白说")]
        public static bool Function_53(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_坦白说");
            }
            return false;
        }

        /// <summary>
        /// 群权限_新成员查看历史消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_新成员查看历史消息")]
        public static bool Function_54(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_新成员查看历史消息");
            }
            return false;
        }

        /// <summary>
        /// 群权限_邀请方式设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">方式</param>
        /// </summary>
        [ProxyAPIName("群权限_邀请方式设置")]
        public static bool Function_55(long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_邀请方式设置");
            }
            return false;
        }

        /// <summary>
        /// 撤回消息_群聊
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">消息Random</param>
        /// <param name="arg3">消息Req</param>
        /// </summary>
        [ProxyAPIName("撤回消息_群聊")]
        public static bool Function_56(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回消息_群聊");
            }
            return false;
        }

        /// <summary>
        /// 撤回消息_私聊本身
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">消息Random</param>
        /// <param name="arg3">消息Req</param>
        /// <param name="arg4">消息接收时间</param>
        /// </summary>
        [ProxyAPIName("撤回消息_私聊本身")]
        public static bool Function_57(long arg0, long arg1, long arg2, int arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回消息_私聊本身");
            }
            return false;
        }

        /// <summary>
        /// 设置位置共享
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">经度</param>
        /// <param name="arg3">纬度</param>
        /// <param name="arg4">是否开启</param>
        /// </summary>
        [ProxyAPIName("设置位置共享")]
        public static bool Function_58(long arg0, long arg1, double arg2, double arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置位置共享");
            }
            return false;
        }

        /// <summary>
        /// 上报当前位置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">经度</param>
        /// <param name="arg3">纬度</param>
        /// </summary>
        [ProxyAPIName("上报当前位置")]
        public static bool Function_59(long arg0, long arg1, double arg2, double arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上报当前位置");
            }
            return false;
        }

        /// <summary>
        /// 是否被禁言
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("是否被禁言")]
        public static long Function_60(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 是否被禁言");
            }
            return 0;
        }

        /// <summary>
        /// 处理群验证事件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">触发QQ</param>
        /// <param name="arg3">消息Seq</param>
        /// <param name="arg4">操作类型</param>
        /// <param name="arg5">事件类型</param>
        /// <param name="arg6">拒绝理由</param>
        /// </summary>
        [ProxyAPIName("处理群验证事件")]
        public static void Function_61(long arg0, long arg1, long arg2, long arg3, int arg4, int arg5, string arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 处理群验证事件");
            }
        }

        /// <summary>
        /// 处理好友验证事件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">触发QQ</param>
        /// <param name="arg2">消息Seq</param>
        /// <param name="arg3">操作类型</param>
        /// </summary>
        [ProxyAPIName("处理好友验证事件")]
        public static void Function_62(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 处理好友验证事件");
            }
        }

        /// <summary>
        /// 查看转发聊天记录内容
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">resId</param>
        /// <param name="arg2">消息内容</param>
        /// </summary>
        [ProxyAPIName("查看转发聊天记录内容")]
        public static void Function_63(long arg0, string arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查看转发聊天记录内容");
            }
        }

        /// <summary>
        /// 上传群文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件路径</param>
        /// <param name="arg3">文件夹名</param>
        /// </summary>
        [ProxyAPIName("上传群文件")]
        public static string Function_64(long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群文件");
            }
            return "";
        }

        /// <summary>
        /// 创建群文件夹
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件夹名</param>
        /// </summary>
        [ProxyAPIName("创建群文件夹")]
        public static string Function_65(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建群文件夹");
            }
            return "";
        }

        /// <summary>
        /// 重命名群文件夹
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">旧文件夹名</param>
        /// <param name="arg3">新文件夹名</param>
        /// </summary>
        [ProxyAPIName("重命名群文件夹")]
        public static string Function_66(long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 重命名群文件夹");
            }
            return "";
        }

        /// <summary>
        /// 删除群文件夹
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件夹名</param>
        /// </summary>
        [ProxyAPIName("删除群文件夹")]
        public static string Function_67(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群文件夹");
            }
            return "";
        }

        /// <summary>
        /// 删除群文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件fileid</param>
        /// <param name="arg3">文件夹名</param>
        /// </summary>
        [ProxyAPIName("删除群文件")]
        public static string Function_68(long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群文件");
            }
            return "";
        }

        /// <summary>
        /// 保存文件到微云
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件fileid</param>
        /// </summary>
        [ProxyAPIName("保存文件到微云")]
        public static string Function_69(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 保存文件到微云");
            }
            return "";
        }

        /// <summary>
        /// 移动群文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件fileid</param>
        /// <param name="arg3">当前文件夹名</param>
        /// <param name="arg4">目标文件夹名</param>
        /// </summary>
        [ProxyAPIName("移动群文件")]
        public static string Function_70(long arg0, long arg1, string arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 移动群文件");
            }
            return "";
        }

        /// <summary>
        /// 取群文件列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件夹名</param>
        /// <param name="arg3">数据</param>
        /// </summary>
        [ProxyAPIName("取群文件列表")]
        public static string Function_71(long arg0, long arg1, string arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群文件列表");
            }
            return "";
        }

        /// <summary>
        /// 设置在线状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">main</param>
        /// <param name="arg2">sun</param>
        /// <param name="arg3">电量</param>
        /// </summary>
        [ProxyAPIName("设置在线状态")]
        public static bool Function_72(long arg0, int arg1, int arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置在线状态");
            }
            return false;
        }

        /// <summary>
        /// 取插件数据目录
        /// </summary>
        [ProxyAPIName("取插件数据目录")]
        public static string Function_73()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取插件数据目录");
            }
            return "";
        }

        /// <summary>
        /// QQ点赞
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("QQ点赞")]
        public static string Function_74(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ点赞");
            }
            return "";
        }

        /// <summary>
        /// 取图片下载地址
        /// <param name="arg0">图片代码</param>
        /// <param name="arg1">框架QQ</param>
        /// <param name="arg2">群号</param>
        /// </summary>
        [ProxyAPIName("取图片下载地址")]
        public static string Function_75(string arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取图片下载地址");
            }
            return "";
        }

        /// <summary>
        /// 查询好友信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("查询好友信息")]
        public static bool Function_76(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询好友信息");
            }
            return false;
        }

        /// <summary>
        /// 查询群信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("查询群信息")]
        public static bool Function_77(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询群信息");
            }
            return false;
        }

        /// <summary>
        /// 框架重启
        /// </summary>
        [ProxyAPIName("框架重启")]
        public static void Function_78()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 框架重启");
            }
        }

        /// <summary>
        /// 群文件转发至群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">目标群号</param>
        /// <param name="arg3">fileId</param>
        /// </summary>
        [ProxyAPIName("群文件转发至群")]
        public static bool Function_79(long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群文件转发至群");
            }
            return false;
        }

        /// <summary>
        /// 群文件转发至好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">目标QQ</param>
        /// <param name="arg3">fileId</param>
        /// <param name="arg4">filename</param>
        /// <param name="arg5">filesize</param>
        /// <param name="arg6">Req</param>
        /// <param name="arg7">Random</param>
        /// <param name="arg8">time</param>
        /// </summary>
        [ProxyAPIName("群文件转发至好友")]
        public static bool Function_80(long arg0, long arg1, long arg2, string arg3, string arg4, long arg5, int arg6, long arg7, int arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群文件转发至好友");
            }
            return false;
        }

        /// <summary>
        /// 好友文件转发至好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标QQ</param>
        /// <param name="arg2">fileId</param>
        /// <param name="arg3">filename</param>
        /// <param name="arg4">filesize</param>
        /// <param name="arg5">Req</param>
        /// <param name="arg6">Random</param>
        /// <param name="arg7">time</param>
        /// </summary>
        [ProxyAPIName("好友文件转发至好友")]
        public static bool Function_81(long arg0, long arg1, string arg2, string arg3, long arg4, int arg5, long arg6, int arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友文件转发至好友");
            }
            return false;
        }

        /// <summary>
        /// 置群消息接收
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">设置类型</param>
        /// </summary>
        [ProxyAPIName("置群消息接收")]
        public static bool Function_82(long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置群消息接收");
            }
            return false;
        }

        /// <summary>
        /// 取好友在线状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取好友在线状态")]
        public static string Function_83(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友在线状态");
            }
            return "";
        }

        /// <summary>
        /// 取QQ钱包个人信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取QQ钱包个人信息")]
        public static string Function_84(long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ钱包个人信息");
            }
            return "";
        }

        /// <summary>
        /// 获取订单详情
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">订单号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("获取订单详情")]
        public static string Function_85(long arg0, string arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取订单详情");
            }
            return "";
        }

        /// <summary>
        /// 提交支付验证码
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">验证码信息</param>
        /// <param name="arg2">验证码</param>
        /// <param name="arg3">支付密码</param>
        /// </summary>
        [ProxyAPIName("提交支付验证码")]
        public static string Function_86(long arg0, object arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 提交支付验证码");
            }
            return "";
        }

        /// <summary>
        /// 分享音乐
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">分享对象</param>
        /// <param name="arg2">歌曲名</param>
        /// <param name="arg3">歌手名</param>
        /// <param name="arg4">跳转地址</param>
        /// <param name="arg5">封面地址</param>
        /// <param name="arg6">文件地址</param>
        /// <param name="arg7">应用类型</param>
        /// <param name="arg8">分享类型</param>
        /// </summary>
        [ProxyAPIName("分享音乐")]
        public static bool Function_87(long arg0, long arg1, string arg2, string arg3, string arg4, string arg5, string arg6, int arg7, int arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 分享音乐");
            }
            return false;
        }

        /// <summary>
        /// 更改群聊消息内容
        /// <param name="arg0">数据指针</param>
        /// <param name="arg1">新消息内容</param>
        /// </summary>
        [ProxyAPIName("更改群聊消息内容")]
        public static bool Function_88(int arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 更改群聊消息内容");
            }
            return false;
        }

        /// <summary>
        /// 更改私聊消息内容
        /// <param name="arg0">数据指针</param>
        /// <param name="arg1">新消息内容</param>
        /// </summary>
        [ProxyAPIName("更改私聊消息内容")]
        public static bool Function_89(int arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 更改私聊消息内容");
            }
            return false;
        }

        /// <summary>
        /// 群聊口令红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊口令红包")]
        public static string Function_90(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊口令红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊拼手气红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊拼手气红包")]
        public static string Function_91(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊拼手气红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊普通红包")]
        public static string Function_92(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊普通红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊画图红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">题目名</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊画图红包")]
        public static string Function_93(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊画图红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊语音红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">语音口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊语音红包")]
        public static string Function_94(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊语音红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊接龙红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">接龙内容</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊接龙红包")]
        public static string Function_95(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊接龙红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">领取人</param>
        /// <param name="arg5">祝福语</param>
        /// <param name="arg6">是否均分</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊专属红包")]
        public static string Function_96(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, bool arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊专属红包");
            }
            return "";
        }

        /// <summary>
        /// 好友口令红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友口令红包")]
        public static string Function_97(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友口令红包");
            }
            return "";
        }

        /// <summary>
        /// 好友普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友普通红包")]
        public static string Function_98(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友普通红包");
            }
            return "";
        }

        /// <summary>
        /// 好友画图红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">题目名</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友画图红包")]
        public static string Function_99(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友画图红包");
            }
            return "";
        }

        /// <summary>
        /// 好友语音红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">语音口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友语音红包")]
        public static string Function_100(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友语音红包");
            }
            return "";
        }

        /// <summary>
        /// 好友接龙红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">接龙内容</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友接龙红包")]
        public static string Function_101(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友接龙红包");
            }
            return "";
        }

        /// <summary>
        /// 设置专属头衔
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">头衔</param>
        /// </summary>
        [ProxyAPIName("设置专属头衔")]
        public static bool Function_102(long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置专属头衔");
            }
            return false;
        }

        /// <summary>
        /// 下线指定QQ
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("下线指定QQ")]
        public static bool Function_103(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 下线指定QQ");
            }
            return false;
        }

        /// <summary>
        /// 登录指定QQ
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("登录指定QQ")]
        public static bool Function_104(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 登录指定QQ");
            }
            return false;
        }

        /// <summary>
        /// 取群未领红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取群未领红包")]
        public static int Function_105(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群未领红包");
            }
            return 0;
        }

        /// <summary>
        /// 发送输入状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">输入状态</param>
        /// </summary>
        [ProxyAPIName("发送输入状态")]
        public static bool Function_106(long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送输入状态");
            }
            return false;
        }

        /// <summary>
        /// 修改资料
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">昵称</param>
        /// <param name="arg2">性别</param>
        /// <param name="arg3">生日</param>
        /// <param name="arg4">职业</param>
        /// <param name="arg5">公司名</param>
        /// <param name="arg6">所在地</param>
        /// <param name="arg7">家乡</param>
        /// <param name="arg8">邮箱</param>
        /// <param name="arg9">个人说明</param>
        /// </summary>
        [ProxyAPIName("修改资料")]
        public static bool Function_107(long arg0, string arg1, int arg2, string arg3, int arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改资料");
            }
            return false;
        }

        /// <summary>
        /// 取群文件下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">文件id</param>
        /// <param name="arg3">文件名</param>
        /// </summary>
        [ProxyAPIName("取群文件下载地址")]
        public static string Function_108(long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群文件下载地址");
            }
            return "";
        }

        /// <summary>
        /// 打好友电话
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("打好友电话")]
        public static void Function_109(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 打好友电话");
            }
        }

        /// <summary>
        /// 头像双击_好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("头像双击_好友")]
        public static bool Function_110(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 头像双击_好友");
            }
            return false;
        }

        /// <summary>
        /// 头像双击_群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">群号</param>
        /// </summary>
        [ProxyAPIName("头像双击_群")]
        public static bool Function_111(long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 头像双击_群");
            }
            return false;
        }

        /// <summary>
        /// 取群成员简略信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取群成员简略信息")]
        public static string Function_112(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群成员简略信息");
            }
            return "";
        }

        /// <summary>
        /// 群聊置顶
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">置顶</param>
        /// </summary>
        [ProxyAPIName("群聊置顶")]
        public static bool Function_113(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊置顶");
            }
            return false;
        }

        /// <summary>
        /// 私聊置顶
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">置顶</param>
        /// </summary>
        [ProxyAPIName("私聊置顶")]
        public static bool Function_114(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 私聊置顶");
            }
            return false;
        }

        /// <summary>
        /// 取加群链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取加群链接")]
        public static string Function_115(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取加群链接");
            }
            return "";
        }

        /// <summary>
        /// 设为精华
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">消息Req</param>
        /// <param name="arg3">消息Random</param>
        /// </summary>
        [ProxyAPIName("设为精华")]
        public static bool Function_116(long arg0, long arg1, int arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设为精华");
            }
            return false;
        }

        /// <summary>
        /// 群权限_设置群昵称规则
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">名片规则</param>
        /// </summary>
        [ProxyAPIName("群权限_设置群昵称规则")]
        public static bool Function_117(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_设置群昵称规则");
            }
            return false;
        }

        /// <summary>
        /// 群权限_设置群发言频率
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">限制条数</param>
        /// </summary>
        [ProxyAPIName("群权限_设置群发言频率")]
        public static bool Function_118(long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_设置群发言频率");
            }
            return false;
        }

        /// <summary>
        /// 群权限_设置群查找方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">查找方式</param>
        /// </summary>
        [ProxyAPIName("群权限_设置群查找方式")]
        public static bool Function_119(long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_设置群查找方式");
            }
            return false;
        }

        /// <summary>
        /// 邀请好友加群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">来源群号</param>
        /// </summary>
        [ProxyAPIName("邀请好友加群")]
        public static bool Function_120(long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 邀请好友加群");
            }
            return false;
        }

        /// <summary>
        /// 置群内消息通知
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">通知类型</param>
        /// </summary>
        [ProxyAPIName("置群内消息通知")]
        public static bool Function_121(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置群内消息通知");
            }
            return false;
        }

        /// <summary>
        /// 修改群名称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">名称</param>
        /// </summary>
        [ProxyAPIName("修改群名称")]
        public static bool Function_122(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改群名称");
            }
            return false;
        }

        /// <summary>
        /// 重载自身
        /// <param name="arg0">新文件路径</param>
        /// </summary>
        [ProxyAPIName("重载自身")]
        public static void Function_123(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 重载自身");
            }
        }

        /// <summary>
        /// 下线其他设备
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">移动设备</param>
        /// <param name="arg2">appid</param>
        /// </summary>
        [ProxyAPIName("下线其他设备")]
        public static void Function_124(long arg0, bool arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 下线其他设备");
            }
        }

        /// <summary>
        /// 登录网页取ck
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">回调跳转地址</param>
        /// <param name="arg2">appid</param>
        /// <param name="arg3">daid</param>
        /// <param name="arg4">cookie</param>
        /// </summary>
        [ProxyAPIName("登录网页取ck")]
        public static bool Function_125(long arg0, string arg1, string arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 登录网页取ck");
            }
            return false;
        }

        /// <summary>
        /// 发送群公告
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">标题</param>
        /// <param name="arg3">内容</param>
        /// <param name="arg4">图片</param>
        /// <param name="arg5">视频</param>
        /// <param name="arg6">弹窗展示</param>
        /// <param name="arg7">需要确认</param>
        /// <param name="arg8">置顶</param>
        /// <param name="arg9">发送给新成员</param>
        /// <param name="arg10">引导修改群昵称</param>
        /// </summary>
        [ProxyAPIName("发送群公告")]
        public static string Function_126(long arg0, long arg1, string arg2, string arg3, byte[] arg4, string arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, arg10={arg10}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群公告");
            }
            return "";
        }

        /// <summary>
        /// 取框架版本
        /// </summary>
        [ProxyAPIName("取框架版本")]
        public static string Function_127()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架版本");
            }
            return "";
        }

        /// <summary>
        /// 取群成员信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">数据</param>
        /// </summary>
        [ProxyAPIName("取群成员信息")]
        public static string Function_128(long arg0, long arg1, long arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群成员信息");
            }
            return "";
        }

        /// <summary>
        /// 取钱包cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取钱包cookie")]
        public static string Function_129(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取钱包cookie");
            }
            return "";
        }

        /// <summary>
        /// 取群网页cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取群网页cookie")]
        public static string Function_130(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群网页cookie");
            }
            return "";
        }

        /// <summary>
        /// 转账
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">转账金额</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">转账留言</param>
        /// <param name="arg4">转账类型</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("转账")]
        public static string Function_131(long arg0, int arg1, long arg2, string arg3, int arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 转账");
            }
            return "";
        }

        /// <summary>
        /// 取收款链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款金额</param>
        /// <param name="arg2">说明文本</param>
        /// </summary>
        [ProxyAPIName("取收款链接")]
        public static string Function_132(long arg0, int arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取收款链接");
            }
            return "";
        }

        /// <summary>
        /// 取群小视频下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">来源QQ</param>
        /// <param name="arg3">param</param>
        /// <param name="arg4">hash1</param>
        /// <param name="arg5">文件名</param>
        /// </summary>
        [ProxyAPIName("取群小视频下载地址")]
        public static string Function_133(long arg0, long arg1, long arg2, string arg3, string arg4, string arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群小视频下载地址");
            }
            return "";
        }

        /// <summary>
        /// 取私聊小视频下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源QQ</param>
        /// <param name="arg2">param</param>
        /// <param name="arg3">hash1</param>
        /// <param name="arg4">文件名</param>
        /// </summary>
        [ProxyAPIName("取私聊小视频下载地址")]
        public static string Function_134(long arg0, long arg1, string arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取私聊小视频下载地址");
            }
            return "";
        }

        /// <summary>
        /// 上传小视频
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">本地小视频路径</param>
        /// <param name="arg3">小视频封面图</param>
        /// <param name="arg4">宽度</param>
        /// <param name="arg5">高度</param>
        /// <param name="arg6">时长</param>
        /// </summary>
        [ProxyAPIName("上传小视频")]
        public static string Function_135(long arg0, long arg1, string arg2, byte[] arg3, int arg4, int arg5, int arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传小视频");
            }
            return "";
        }

        /// <summary>
        /// 发送好友xml消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">xml代码</param>
        /// <param name="arg3">Random</param>
        /// <param name="arg4">Req</param>
        /// </summary>
        [ProxyAPIName("发送好友xml消息")]
        public static string Function_136(long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送好友xml消息");
            }
            return "";
        }

        /// <summary>
        /// 发送群xml消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">xml代码</param>
        /// <param name="arg3">匿名发送</param>
        /// </summary>
        [ProxyAPIName("发送群xml消息")]
        public static string Function_137(long arg0, long arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送群xml消息");
            }
            return "";
        }

        /// <summary>
        /// 取群成员概况
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取群成员概况")]
        public static string Function_138(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群成员概况");
            }
            return "";
        }

        /// <summary>
        /// 添加好友_取验证类型
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("添加好友_取验证类型")]
        public static string Function_139(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 添加好友_取验证类型");
            }
            return "";
        }

        /// <summary>
        /// 群聊打卡
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("群聊打卡")]
        public static string Function_140(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊打卡");
            }
            return "";
        }

        /// <summary>
        /// 群聊签到
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">附加参数</param>
        /// </summary>
        [ProxyAPIName("群聊签到")]
        public static bool Function_141(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊签到");
            }
            return false;
        }

        /// <summary>
        /// 置群聊备注
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">备注</param>
        /// </summary>
        [ProxyAPIName("置群聊备注")]
        public static bool Function_142(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置群聊备注");
            }
            return false;
        }

        /// <summary>
        /// 红包转发
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">红包ID</param>
        /// <param name="arg2">目标对象</param>
        /// <param name="arg3">Type</param>
        /// </summary>
        [ProxyAPIName("红包转发")]
        public static string Function_143(long arg0, string arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 红包转发");
            }
            return "";
        }

        /// <summary>
        /// 发送数据包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">包体序号</param>
        /// <param name="arg2">最大等待时长</param>
        /// <param name="arg3">数据</param>
        /// </summary>
        [ProxyAPIName("发送数据包")]
        public static bool Function_144(long arg0, int arg1, int arg2, byte[] arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送数据包");
            }
            return false;
        }

        /// <summary>
        /// 请求ssoseq
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("请求ssoseq")]
        public static int Function_145(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 请求ssoseq");
            }
            return 0;
        }

        /// <summary>
        /// 取sessionkey
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取sessionkey")]
        public static string Function_146(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取sessionkey");
            }
            return "";
        }

        /// <summary>
        /// 获取bkn_gtk
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">自定义bkn_gtk</param>
        /// </summary>
        [ProxyAPIName("获取bkn_gtk")]
        public static string Function_147(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取bkn_gtk");
            }
            return "";
        }

        /// <summary>
        /// 置好友验证方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">验证方式</param>
        /// <param name="arg2">Q_and_A</param>
        /// </summary>
        [ProxyAPIName("置好友验证方式")]
        public static bool Function_148(long arg0, int arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置好友验证方式");
            }
            return false;
        }

        /// <summary>
        /// 上传照片墙图片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">pic</param>
        /// </summary>
        [ProxyAPIName("上传照片墙图片")]
        public static string Function_149(long arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传照片墙图片");
            }
            return "";
        }

        /// <summary>
        /// 付款
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">QrcodeUrl</param>
        /// <param name="arg2">银行卡序列</param>
        /// <param name="arg3">支付密码</param>
        /// <param name="arg4">验证码信息</param>
        /// </summary>
        [ProxyAPIName("付款")]
        public static string Function_150(long arg0, string arg1, int arg2, string arg3, object arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 付款");
            }
            return "";
        }

        /// <summary>
        /// 修改支付密码
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">原密码</param>
        /// <param name="arg2">新密码</param>
        /// </summary>
        [ProxyAPIName("修改支付密码")]
        public static string Function_151(long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改支付密码");
            }
            return "";
        }

        /// <summary>
        /// 账号搜索
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">关键词</param>
        /// </summary>
        [ProxyAPIName("账号搜索")]
        public static string Function_152(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 账号搜索");
            }
            return "";
        }

        /// <summary>
        /// 添加群_取验证类型
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("添加群_取验证类型")]
        public static string Function_153(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 添加群_取验证类型");
            }
            return "";
        }

        /// <summary>
        /// 获取红包领取详情
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">红包文本代码</param>
        /// <param name="arg3">红包类型</param>
        /// </summary>
        [ProxyAPIName("获取红包领取详情")]
        public static string Function_154(long arg0, long arg1, string arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取红包领取详情");
            }
            return "";
        }

        /// <summary>
        /// 取好友文件下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">FileId</param>
        /// <param name="arg2">FileName</param>
        /// </summary>
        [ProxyAPIName("取好友文件下载地址")]
        public static string Function_155(long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友文件下载地址");
            }
            return "";
        }

        /// <summary>
        /// 删除群成员_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">群成员QQ</param>
        /// <param name="arg3">拒绝加群申请</param>
        /// </summary>
        [ProxyAPIName("删除群成员_批量")]
        public static bool Function_156(long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群成员_批量");
            }
            return false;
        }

        /// <summary>
        /// 取扩列资料
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取扩列资料")]
        public static string Function_157(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取扩列资料");
            }
            return "";
        }

        /// <summary>
        /// 取资料展示设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取资料展示设置")]
        public static string Function_158(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取资料展示设置");
            }
            return "";
        }

        /// <summary>
        /// 设置资料展示
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("设置资料展示")]
        public static string Function_159(long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置资料展示");
            }
            return "";
        }

        /// <summary>
        /// 获取当前登录设备信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">信息</param>
        /// </summary>
        [ProxyAPIName("获取当前登录设备信息")]
        public static string Function_160(long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取当前登录设备信息");
            }
            return "";
        }

        /// <summary>
        /// 提取图片文字
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">图片地址</param>
        /// <param name="arg2">识别结果</param>
        /// </summary>
        [ProxyAPIName("提取图片文字")]
        public static bool Function_161(long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 提取图片文字");
            }
            return false;
        }

        /// <summary>
        /// 取插件文件名
        /// </summary>
        [ProxyAPIName("取插件文件名")]
        public static string Function_162()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取插件文件名");
            }
            return "";
        }

        /// <summary>
        /// TEA加密
        /// <param name="arg0">内容</param>
        /// <param name="arg1">秘钥</param>
        /// </summary>
        [ProxyAPIName("TEA加密")]
        public static void Function_163(byte[] arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API TEA加密");
            }
        }

        /// <summary>
        /// TEA解密
        /// <param name="arg0">内容</param>
        /// <param name="arg1">秘钥</param>
        /// </summary>
        [ProxyAPIName("TEA解密")]
        public static void Function_164(byte[] arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API TEA解密");
            }
        }

        /// <summary>
        /// 红包数据加密
        /// <param name="arg0">str</param>
        /// <param name="arg1">random</param>
        /// </summary>
        [ProxyAPIName("红包数据加密")]
        public static string Function_165(string arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 红包数据加密");
            }
            return "";
        }

        /// <summary>
        /// 红包数据解密
        /// <param name="arg0">str</param>
        /// <param name="arg1">random</param>
        /// </summary>
        [ProxyAPIName("红包数据解密")]
        public static string Function_166(string arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 红包数据解密");
            }
            return "";
        }

        /// <summary>
        /// 红包msgno计算
        /// <param name="arg0">目标QQ</param>
        /// </summary>
        [ProxyAPIName("红包msgno计算")]
        public static string Function_167(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 红包msgno计算");
            }
            return "";
        }

        /// <summary>
        /// 取消精华
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">消息Req</param>
        /// <param name="arg3">消息Random</param>
        /// </summary>
        [ProxyAPIName("取消精华")]
        public static bool Function_168(long arg0, long arg1, int arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取消精华");
            }
            return false;
        }

        /// <summary>
        /// 群权限_设置加群方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">加群方式</param>
        /// <param name="arg3">问题</param>
        /// <param name="arg4">答案</param>
        /// </summary>
        [ProxyAPIName("群权限_设置加群方式")]
        public static bool Function_169(long arg0, long arg1, int arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_设置加群方式");
            }
            return false;
        }

        /// <summary>
        /// 群权限_群幸运字符
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否开启</param>
        /// </summary>
        [ProxyAPIName("群权限_群幸运字符")]
        public static bool Function_170(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_群幸运字符");
            }
            return false;
        }

        /// <summary>
        /// 群权限_一起写
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">是否允许</param>
        /// </summary>
        [ProxyAPIName("群权限_一起写")]
        public static bool Function_171(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群权限_一起写");
            }
            return false;
        }

        /// <summary>
        /// 取QQ空间cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取QQ空间cookie")]
        public static string Function_172(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ空间cookie");
            }
            return "";
        }

        /// <summary>
        /// 框架是否为单Q
        /// </summary>
        [ProxyAPIName("框架是否为单Q")]
        public static bool Function_173()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 框架是否为单Q");
            }
            return false;
        }

        /// <summary>
        /// 修改指定QQ缓存密码
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">新密码</param>
        /// </summary>
        [ProxyAPIName("修改指定QQ缓存密码")]
        public static bool Function_174(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改指定QQ缓存密码");
            }
            return false;
        }

        /// <summary>
        /// 处理群验证事件_风险号
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">触发QQ</param>
        /// <param name="arg3">消息Seq</param>
        /// <param name="arg4">操作类型</param>
        /// <param name="arg5">事件类型</param>
        /// <param name="arg6">拒绝理由</param>
        /// </summary>
        [ProxyAPIName("处理群验证事件_风险号")]
        public static void Function_175(long arg0, long arg1, long arg2, long arg3, int arg4, int arg5, string arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 处理群验证事件_风险号");
            }
        }

        /// <summary>
        /// 查询网址安全性
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">网址</param>
        /// </summary>
        [ProxyAPIName("查询网址安全性")]
        public static int Function_176(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询网址安全性");
            }
            return 0;
        }

        /// <summary>
        /// 消息合并转发至好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">聊天记录</param>
        /// <param name="arg3">Random</param>
        /// <param name="arg4">Req</param>
        /// <param name="arg5">消息记录来源</param>
        /// </summary>
        [ProxyAPIName("消息合并转发至好友")]
        public static string Function_177(long arg0, long arg1, object arg2, long arg3, int arg4, string arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 消息合并转发至好友");
            }
            return "";
        }

        /// <summary>
        /// 消息合并转发至群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">聊天记录</param>
        /// <param name="arg3">匿名发送</param>
        /// <param name="arg4">消息记录来源</param>
        /// </summary>
        [ProxyAPIName("消息合并转发至群")]
        public static string Function_178(long arg0, long arg1, object arg2, bool arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 消息合并转发至群");
            }
            return "";
        }

        /// <summary>
        /// 取卡片消息代码
        /// <param name="arg0">卡片消息文本代码</param>
        /// </summary>
        [ProxyAPIName("取卡片消息代码")]
        public static string Function_179(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取卡片消息代码");
            }
            return "";
        }

        /// <summary>
        /// 禁言群匿名
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">匿名昵称</param>
        /// <param name="arg3">匿名标识</param>
        /// <param name="arg4">禁言时长</param>
        /// </summary>
        [ProxyAPIName("禁言群匿名")]
        public static bool Function_180(long arg0, long arg1, string arg2, byte[] arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 禁言群匿名");
            }
            return false;
        }

        /// <summary>
        /// 置文件下载
        /// <param name="arg0">文件下载地址</param>
        /// <param name="arg1">文件保存路径</param>
        /// <param name="arg2">下载回调函数</param>
        /// <param name="arg3">文件名</param>
        /// <param name="arg4">下载起点</param>
        /// </summary>
        [ProxyAPIName("置文件下载")]
        public static int Function_181(string arg0, string arg1, object arg2, string arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置文件下载");
            }
            return 0;
        }

        /// <summary>
        /// 领取私聊普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源QQ</param>
        /// <param name="arg2">红包文本代码</param>
        /// <param name="arg3">类型</param>
        /// </summary>
        [ProxyAPIName("领取私聊普通红包")]
        public static string Function_182(long arg0, long arg1, string arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 领取私聊普通红包");
            }
            return "";
        }

        /// <summary>
        /// 领取群聊专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源群号</param>
        /// <param name="arg2">来源QQ</param>
        /// <param name="arg3">红包文本代码</param>
        /// </summary>
        [ProxyAPIName("领取群聊专属红包")]
        public static string Function_183(long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 领取群聊专属红包");
            }
            return "";
        }

        /// <summary>
        /// 加载网页
        /// <param name="arg0">网址</param>
        /// </summary>
        [ProxyAPIName("加载网页")]
        public static bool Function_184(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 加载网页");
            }
            return false;
        }

        /// <summary>
        /// 压缩包_7za解压
        /// <param name="arg0">压缩包路径</param>
        /// <param name="arg1">解压保存路径</param>
        /// <param name="arg2">解压密码</param>
        /// <param name="arg3">跳过已存在的文件</param>
        /// </summary>
        [ProxyAPIName("压缩包_7za解压")]
        public static void Function_185(string arg0, string arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 压缩包_7za解压");
            }
        }

        /// <summary>
        /// 压缩包_7za压缩
        /// <param name="arg0">保存路径</param>
        /// <param name="arg1">欲压缩的文件</param>
        /// <param name="arg2">压缩格式</param>
        /// <param name="arg3">压缩等级</param>
        /// <param name="arg4">压缩密码</param>
        /// </summary>
        [ProxyAPIName("压缩包_7za压缩")]
        public static void Function_186(string arg0, string arg1, string arg2, int arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 压缩包_7za压缩");
            }
        }

        /// <summary>
        /// 发送讨论组消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">消息内容</param>
        /// </summary>
        [ProxyAPIName("发送讨论组消息")]
        public static string Function_187(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送讨论组消息");
            }
            return "";
        }

        /// <summary>
        /// 发送讨论组json消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">Json代码</param>
        /// </summary>
        [ProxyAPIName("发送讨论组json消息")]
        public static string Function_188(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送讨论组json消息");
            }
            return "";
        }

        /// <summary>
        /// 发送讨论组xml消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">Xml代码</param>
        /// </summary>
        [ProxyAPIName("发送讨论组xml消息")]
        public static string Function_189(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送讨论组xml消息");
            }
            return "";
        }

        /// <summary>
        /// 发送讨论组临时消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">消息内容</param>
        /// <param name="arg4">消息Random</param>
        /// <param name="arg5">消息Req</param>
        /// </summary>
        [ProxyAPIName("发送讨论组临时消息")]
        public static string Function_190(long arg0, long arg1, long arg2, string arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送讨论组临时消息");
            }
            return "";
        }

        /// <summary>
        /// 撤回消息_讨论组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">消息Random</param>
        /// <param name="arg3">消息Req</param>
        /// </summary>
        [ProxyAPIName("撤回消息_讨论组")]
        public static bool Function_191(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回消息_讨论组");
            }
            return false;
        }

        /// <summary>
        /// 回复QQ咨询会话
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">会话Token</param>
        /// <param name="arg3">消息内容</param>
        /// <param name="arg4">消息Random</param>
        /// <param name="arg5">消息Req</param>
        /// </summary>
        [ProxyAPIName("回复QQ咨询会话")]
        public static string Function_192(long arg0, long arg1, byte[] arg2, string arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 回复QQ咨询会话");
            }
            return "";
        }

        /// <summary>
        /// 发送订阅号私聊消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">订阅号Id</param>
        /// <param name="arg2">消息内容</param>
        /// <param name="arg3">消息Random</param>
        /// <param name="arg4">消息Req</param>
        /// </summary>
        [ProxyAPIName("发送订阅号私聊消息")]
        public static string Function_193(long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送订阅号私聊消息");
            }
            return "";
        }

        /// <summary>
        /// 取讨论组名称_从缓存
        /// <param name="arg0">讨论组Id</param>
        /// </summary>
        [ProxyAPIName("取讨论组名称_从缓存")]
        public static string Function_194(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组名称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 修改讨论组名称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">新名称</param>
        /// </summary>
        [ProxyAPIName("修改讨论组名称")]
        public static bool Function_195(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改讨论组名称");
            }
            return false;
        }

        /// <summary>
        /// 取讨论组成员列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取讨论组成员列表")]
        public static int Function_196(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组成员列表");
            }
            return 0;
        }

        /// <summary>
        /// 强制取自身匿名Id
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("强制取自身匿名Id")]
        public static long Function_197(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 强制取自身匿名Id");
            }
            return 0;
        }

        /// <summary>
        /// 取订阅号列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取订阅号列表")]
        public static int Function_198(long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取订阅号列表");
            }
            return 0;
        }

        /// <summary>
        /// 取讨论组列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">数据</param>
        /// </summary>
        [ProxyAPIName("取讨论组列表")]
        public static int Function_199(long arg0, object arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组列表");
            }
            return 0;
        }

        /// <summary>
        /// 邀请好友加群_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标群号</param>
        /// <param name="arg2">邀请QQ</param>
        /// <param name="arg3">来源群号</param>
        /// </summary>
        [ProxyAPIName("邀请好友加群_批量")]
        public static bool Function_200(long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 邀请好友加群_批量");
            }
            return false;
        }

        /// <summary>
        /// 邀请好友加入讨论组_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标讨论组Id</param>
        /// <param name="arg2">邀请QQ</param>
        /// <param name="arg3">来源群号</param>
        /// </summary>
        [ProxyAPIName("邀请好友加入讨论组_批量")]
        public static bool Function_201(long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 邀请好友加入讨论组_批量");
            }
            return false;
        }

        /// <summary>
        /// 取框架到期时间
        /// </summary>
        [ProxyAPIName("取框架到期时间")]
        public static string Function_202()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架到期时间");
            }
            return "";
        }

        /// <summary>
        /// 讨论组口令红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组口令红包")]
        public static string Function_203(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组口令红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组拼手气红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组拼手气红包")]
        public static string Function_204(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组拼手气红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">祝福语</param>
        /// <param name="arg5">红包皮肤Id</param>
        /// <param name="arg6">支付密码</param>
        /// <param name="arg7">银行卡序列</param>
        /// <param name="arg8">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组普通红包")]
        public static string Function_205(long arg0, int arg1, int arg2, long arg3, string arg4, int arg5, string arg6, int arg7, object arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组普通红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组画图红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">题目名</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组画图红包")]
        public static string Function_206(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组画图红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组语音红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">语音口令</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组语音红包")]
        public static string Function_207(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组语音红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组接龙红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">接龙内容</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组接龙红包")]
        public static string Function_208(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组接龙红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">领取人</param>
        /// <param name="arg5">祝福语</param>
        /// <param name="arg6">是否均分</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组专属红包")]
        public static string Function_209(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, bool arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组专属红包");
            }
            return "";
        }

        /// <summary>
        /// 领取讨论组专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源讨论组Id</param>
        /// <param name="arg2">来源QQ</param>
        /// <param name="arg3">红包文本代码</param>
        /// </summary>
        [ProxyAPIName("领取讨论组专属红包")]
        public static string Function_210(long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 领取讨论组专属红包");
            }
            return "";
        }

        /// <summary>
        /// 取讨论组未领红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取讨论组未领红包")]
        public static int Function_211(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组未领红包");
            }
            return 0;
        }

        /// <summary>
        /// 取讨论组文件下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">文件id</param>
        /// <param name="arg3">文件名</param>
        /// </summary>
        [ProxyAPIName("取讨论组文件下载地址")]
        public static string Function_212(long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取讨论组文件下载地址");
            }
            return "";
        }

        /// <summary>
        /// 发送QQ咨询会话
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">消息内容</param>
        /// <param name="arg3">消息Random</param>
        /// <param name="arg4">消息Req</param>
        /// </summary>
        [ProxyAPIName("发送QQ咨询会话")]
        public static string Function_213(long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送QQ咨询会话");
            }
            return "";
        }

        /// <summary>
        /// 创建群聊
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">邀请QQ</param>
        /// <param name="arg2">来源群号</param>
        /// <param name="arg3">新群群号</param>
        /// </summary>
        [ProxyAPIName("创建群聊")]
        public static string Function_214(long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建群聊");
            }
            return "";
        }

        /// <summary>
        /// 取群应用列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">数据</param>
        /// </summary>
        [ProxyAPIName("取群应用列表")]
        public static int Function_215(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群应用列表");
            }
            return 0;
        }

        /// <summary>
        /// 退出讨论组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// </summary>
        [ProxyAPIName("退出讨论组")]
        public static bool Function_216(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 退出讨论组");
            }
            return false;
        }

        /// <summary>
        /// 群验证消息接收设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">接收验证消息</param>
        /// </summary>
        [ProxyAPIName("群验证消息接收设置")]
        public static bool Function_217(long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群验证消息接收设置");
            }
            return false;
        }

        /// <summary>
        /// 转让群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">对方QQ</param>
        /// </summary>
        [ProxyAPIName("转让群")]
        public static bool Function_218(long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 转让群");
            }
            return false;
        }

        /// <summary>
        /// 修改好友备注
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">备注</param>
        /// </summary>
        [ProxyAPIName("修改好友备注")]
        public static bool Function_219(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改好友备注");
            }
            return false;
        }

        /// <summary>
        /// 删除讨论组成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">对方QQ</param>
        /// </summary>
        [ProxyAPIName("删除讨论组成员")]
        public static bool Function_220(long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除讨论组成员");
            }
            return false;
        }

        /// <summary>
        /// 讨论组文件转发至群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源讨论组Id</param>
        /// <param name="arg2">目标群号</param>
        /// <param name="arg3">fileId</param>
        /// <param name="arg4">filename</param>
        /// <param name="arg5">filesize</param>
        /// </summary>
        [ProxyAPIName("讨论组文件转发至群")]
        public static bool Function_221(long arg0, long arg1, long arg2, string arg3, string arg4, long arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组文件转发至群");
            }
            return false;
        }

        /// <summary>
        /// 讨论组文件转发至好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">来源讨论组Id</param>
        /// <param name="arg2">目标QQ</param>
        /// <param name="arg3">fileId</param>
        /// <param name="arg4">filename</param>
        /// <param name="arg5">filesize</param>
        /// <param name="arg6">Req</param>
        /// <param name="arg7">Random</param>
        /// <param name="arg8">time</param>
        /// </summary>
        [ProxyAPIName("讨论组文件转发至好友")]
        public static bool Function_222(long arg0, long arg1, long arg2, string arg3, string arg4, long arg5, int arg6, long arg7, int arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组文件转发至好友");
            }
            return false;
        }

        /// <summary>
        /// 取QQ头像
        /// <param name="arg0">对方QQ</param>
        /// <param name="arg1">高清原图</param>
        /// </summary>
        [ProxyAPIName("取QQ头像")]
        public static string Function_223(long arg0, bool arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ头像");
            }
            return "";
        }

        /// <summary>
        /// 取群头像
        /// <param name="arg0">目标群号</param>
        /// </summary>
        [ProxyAPIName("取群头像")]
        public static string Function_224(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群头像");
            }
            return "";
        }

        /// <summary>
        /// 取大表情图片下载地址
        /// <param name="arg0">大表情文本代码</param>
        /// <param name="arg1">长</param>
        /// <param name="arg2">宽</param>
        /// </summary>
        [ProxyAPIName("取大表情图片下载地址")]
        public static string Function_225(string arg0, int arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取大表情图片下载地址");
            }
            return "";
        }

        /// <summary>
        /// 拉起群收款
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">待付款成员</param>
        /// <param name="arg3">收款留言</param>
        /// <param name="arg4">收款订单号</param>
        /// </summary>
        [ProxyAPIName("拉起群收款")]
        public static string Function_226(long arg0, long arg1, object arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 拉起群收款");
            }
            return "";
        }

        /// <summary>
        /// 结束群收款
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款订单号</param>
        /// </summary>
        [ProxyAPIName("结束群收款")]
        public static string Function_227(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 结束群收款");
            }
            return "";
        }

        /// <summary>
        /// 查询群收款状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款订单号</param>
        /// <param name="arg2">收款数据</param>
        /// </summary>
        [ProxyAPIName("查询群收款状态")]
        public static string Function_228(long arg0, string arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询群收款状态");
            }
            return "";
        }

        /// <summary>
        /// 支付群收款
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款发起人</param>
        /// <param name="arg2">收款订单号</param>
        /// <param name="arg3">支付金额</param>
        /// <param name="arg4">支付密码</param>
        /// <param name="arg5">银行卡序列</param>
        /// <param name="arg6">验证码信息</param>
        /// </summary>
        [ProxyAPIName("支付群收款")]
        public static string Function_229(long arg0, long arg1, string arg2, int arg3, string arg4, int arg5, object arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 支付群收款");
            }
            return "";
        }

        /// <summary>
        /// 消息合并转发至讨论组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">讨论组Id</param>
        /// <param name="arg2">聊天记录</param>
        /// <param name="arg3">消息记录来源</param>
        /// </summary>
        [ProxyAPIName("消息合并转发至讨论组")]
        public static string Function_230(long arg0, long arg1, object arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 消息合并转发至讨论组");
            }
            return "";
        }

        /// <summary>
        /// 群收款_催单
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">收款订单号</param>
        /// </summary>
        [ProxyAPIName("群收款_催单")]
        public static string Function_231(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群收款_催单");
            }
            return "";
        }

        /// <summary>
        /// 取好友Diy名片数据
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">Diy名片数据</param>
        /// </summary>
        [ProxyAPIName("取好友Diy名片数据")]
        public static bool Function_232(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友Diy名片数据");
            }
            return false;
        }

        /// <summary>
        /// 设置Diy名片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">Diy名片数据</param>
        /// </summary>
        [ProxyAPIName("设置Diy名片")]
        public static string Function_233(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置Diy名片");
            }
            return "";
        }

        /// <summary>
        /// 取框架主窗口句柄
        /// </summary>
        [ProxyAPIName("取框架主窗口句柄")]
        public static int Function_234()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架主窗口句柄");
            }
            return 0;
        }

        /// <summary>
        /// 好友生僻字红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">对方QQ</param>
        /// <param name="arg4">生僻字</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("好友生僻字红包")]
        public static string Function_235(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 好友生僻字红包");
            }
            return "";
        }

        /// <summary>
        /// 群聊生僻字红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">生僻字</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("群聊生僻字红包")]
        public static string Function_236(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 群聊生僻字红包");
            }
            return "";
        }

        /// <summary>
        /// 讨论组生僻字红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">讨论组Id</param>
        /// <param name="arg4">生僻字</param>
        /// <param name="arg5">支付密码</param>
        /// <param name="arg6">银行卡序列</param>
        /// <param name="arg7">验证码信息</param>
        /// </summary>
        [ProxyAPIName("讨论组生僻字红包")]
        public static string Function_237(long arg0, int arg1, int arg2, long arg3, string arg4, string arg5, int arg6, object arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 讨论组生僻字红包");
            }
            return "";
        }

        /// <summary>
        /// 支付代付请求
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">代付订单号</param>
        /// <param name="arg2">支付金额</param>
        /// <param name="arg3">支付密码</param>
        /// <param name="arg4">银行卡序列</param>
        /// <param name="arg5">验证码信息</param>
        /// </summary>
        [ProxyAPIName("支付代付请求")]
        public static string Function_238(long arg0, string arg1, int arg2, string arg3, int arg4, object arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 支付代付请求");
            }
            return "";
        }

        /// <summary>
        /// 查询代付状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">代付订单号</param>
        /// <param name="arg2">代付数据</param>
        /// </summary>
        [ProxyAPIName("查询代付状态")]
        public static string Function_239(long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询代付状态");
            }
            return "";
        }

        /// <summary>
        /// 拉起代付
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">订单号</param>
        /// <param name="arg2">代付QQ列表</param>
        /// </summary>
        [ProxyAPIName("拉起代付")]
        public static string Function_240(long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 拉起代付");
            }
            return "";
        }

        /// <summary>
        /// 取好友能量值与QID
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">能量值</param>
        /// <param name="arg3">QID</param>
        /// </summary>
        [ProxyAPIName("取好友能量值与QID")]
        public static bool Function_241(long arg0, long arg1, int arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友能量值与QID");
            }
            return false;
        }

        /// <summary>
        /// 创建小栗子文本代码解析类对象
        /// </summary>
        [ProxyAPIName("创建小栗子文本代码解析类对象")]
        public static IntPtr Function_242()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 创建小栗子文本代码解析类对象");
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 文字转语音
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">文本内容</param>
        /// <param name="arg2">语音结果</param>
        /// </summary>
        [ProxyAPIName("文字转语音")]
        public static string Function_243(long arg0, string arg1, byte[] arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 文字转语音");
            }
            return "";
        }

        /// <summary>
        /// 翻译
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">源语言语种</param>
        /// <param name="arg2">目标语言语种</param>
        /// <param name="arg3">原文</param>
        /// <param name="arg4">翻译结果</param>
        /// </summary>
        [ProxyAPIName("翻译")]
        public static string Function_244(long arg0, string arg1, string arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 翻译");
            }
            return "";
        }

        /// <summary>
        /// 撤回消息_群聊s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">消息Random</param>
        /// <param name="arg3">消息Req</param>
        /// </summary>
        [ProxyAPIName("撤回消息_群聊s")]
        public static string Function_245(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回消息_群聊s");
            }
            return "";
        }

        /// <summary>
        /// QQ列表_添加手表协议QQ
        /// <param name="arg0">QQ</param>
        /// <param name="arg1">品牌</param>
        /// <param name="arg2">型号</param>
        /// </summary>
        [ProxyAPIName("QQ列表_添加手表协议QQ")]
        public static string Function_246(long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_添加手表协议QQ");
            }
            return "";
        }

        /// <summary>
        /// QQ列表_二维码登录_拉取二维码
        /// <param name="arg0">QQ</param>
        /// <param name="arg1">二维码数据</param>
        /// </summary>
        [ProxyAPIName("QQ列表_二维码登录_拉取二维码")]
        public static string Function_247(long arg0, byte[] arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_二维码登录_拉取二维码");
            }
            return "";
        }

        /// <summary>
        /// QQ列表_二维码登录_查询二维码状态
        /// <param name="arg0">QQ</param>
        /// </summary>
        [ProxyAPIName("QQ列表_二维码登录_查询二维码状态")]
        public static string Function_248(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_二维码登录_查询二维码状态");
            }
            return "";
        }

        /// <summary>
        /// 拍一拍好友在线状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("拍一拍好友在线状态")]
        public static bool Function_249(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 拍一拍好友在线状态");
            }
            return false;
        }

        /// <summary>
        /// 发送验证消息会话消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">消息内容</param>
        /// <param name="arg3">消息Random</param>
        /// <param name="arg4">消息Req</param>
        /// </summary>
        [ProxyAPIName("发送验证消息会话消息")]
        public static string Function_250(long arg0, long arg1, string arg2, long arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送验证消息会话消息");
            }
            return "";
        }

        /// <summary>
        /// 回复验证消息会话消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// <param name="arg2">会话Token</param>
        /// <param name="arg3">消息内容</param>
        /// <param name="arg4">消息Random</param>
        /// <param name="arg5">消息Req</param>
        /// </summary>
        [ProxyAPIName("回复验证消息会话消息")]
        public static string Function_251(long arg0, long arg1, byte[] arg2, string arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 回复验证消息会话消息");
            }
            return "";
        }

        /// <summary>
        /// 取群文件内存利用状态
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">已使用容量</param>
        /// <param name="arg3">总容量</param>
        /// </summary>
        [ProxyAPIName("取群文件内存利用状态")]
        public static string Function_252(long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群文件内存利用状态");
            }
            return "";
        }

        /// <summary>
        /// 取群文件总数
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">已上传文件数</param>
        /// <param name="arg3">文件数量上限</param>
        /// </summary>
        [ProxyAPIName("取群文件总数")]
        public static string Function_253(long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群文件总数");
            }
            return "";
        }

        /// <summary>
        /// 上传涂鸦
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">模型Id</param>
        /// <param name="arg2">涂鸦数据</param>
        /// </summary>
        [ProxyAPIName("上传涂鸦")]
        public static string Function_254(long arg0, int arg1, byte[] arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传涂鸦");
            }
            return "";
        }

        /// <summary>
        /// 删除群成员_批量s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">欲移除群成员列表</param>
        /// </summary>
        [ProxyAPIName("删除群成员_批量s")]
        public static bool Function_255(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群成员_批量s");
            }
            return false;
        }

        /// <summary>
        /// 上传好友文件s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">文件路径</param>
        /// <param name="arg3">上传进度回调函数</param>
        /// <param name="arg4">Random</param>
        /// <param name="arg5">Req</param>
        /// </summary>
        [ProxyAPIName("上传好友文件s")]
        public static string Function_256(long arg0, long arg1, string arg2, int arg3, long arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传好友文件s");
            }
            return "";
        }

        /// <summary>
        /// 上传群文件s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">文件路径</param>
        /// <param name="arg3">文件夹名</param>
        /// <param name="arg4">上传进度回调函数</param>
        /// </summary>
        [ProxyAPIName("上传群文件s")]
        public static string Function_257(long arg0, long arg1, string arg2, string arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群文件s");
            }
            return "";
        }

        /// <summary>
        /// 取群艾特全体剩余次数
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取群艾特全体剩余次数")]
        public static int Function_258(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群艾特全体剩余次数");
            }
            return 0;
        }

        /// <summary>
        /// 是否已开启QQ咨询
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("是否已开启QQ咨询")]
        public static int Function_259(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 是否已开启QQ咨询");
            }
            return 0;
        }

        /// <summary>
        /// 创建群相册
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册名</param>
        /// <param name="arg3">相册描述</param>
        /// </summary>
        [ProxyAPIName("创建群相册")]
        public static string Function_260(long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建群相册");
            }
            return "";
        }

        /// <summary>
        /// 删除群相册
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册Id</param>
        /// </summary>
        [ProxyAPIName("删除群相册")]
        public static string Function_261(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群相册");
            }
            return "";
        }

        /// <summary>
        /// 取群相册列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("取群相册列表")]
        public static string Function_262(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群相册列表");
            }
            return "";
        }

        /// <summary>
        /// 取群相册照片列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册Id</param>
        /// <param name="arg3">获取数量</param>
        /// </summary>
        [ProxyAPIName("取群相册照片列表")]
        public static string Function_263(long arg0, long arg1, string arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群相册照片列表");
            }
            return "";
        }

        /// <summary>
        /// 删除群相册照片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册Id</param>
        /// <param name="arg3">照片Id</param>
        /// </summary>
        [ProxyAPIName("删除群相册照片")]
        public static string Function_264(long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除群相册照片");
            }
            return "";
        }

        /// <summary>
        /// 修改群相册信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// <param name="arg2">相册Id</param>
        /// <param name="arg3">相册名</param>
        /// <param name="arg4">相册描述</param>
        /// <param name="arg5">相册置顶</param>
        /// </summary>
        [ProxyAPIName("修改群相册信息")]
        public static string Function_265(long arg0, long arg1, string arg2, string arg3, string arg4, bool arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改群相册信息");
            }
            return "";
        }

        /// <summary>
        /// 取群Id_从缓存
        /// <param name="arg0">群号</param>
        /// </summary>
        [ProxyAPIName("取群Id_从缓存")]
        public static long Function_266(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取群Id_从缓存");
            }
            return 0;
        }

        /// <summary>
        /// 上传频道图片
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">pic</param>
        /// <param name="arg4">宽度</param>
        /// <param name="arg5">高度</param>
        /// <param name="arg6">动图</param>
        /// </summary>
        [ProxyAPIName("上传频道图片")]
        public static string Function_267(long arg0, long arg1, long arg2, byte[] arg3, int arg4, int arg5, bool arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传频道图片");
            }
            return "";
        }

        /// <summary>
        /// 发送频道消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息内容</param>
        /// </summary>
        [ProxyAPIName("发送频道消息")]
        public static string Function_268(long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送频道消息");
            }
            return "";
        }

        /// <summary>
        /// 发送频道私信消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">私信频道Id</param>
        /// <param name="arg2">私信子频道Id</param>
        /// <param name="arg3">消息内容</param>
        /// </summary>
        [ProxyAPIName("发送频道私信消息")]
        public static string Function_269(long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送频道私信消息");
            }
            return "";
        }

        /// <summary>
        /// 取私信频道Id
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">目标频道用户Id</param>
        /// <param name="arg3">私信频道Id</param>
        /// <param name="arg4">私信子频道Id</param>
        /// </summary>
        [ProxyAPIName("取私信频道Id")]
        public static bool Function_270(long arg0, long arg1, long arg2, long arg3, long arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取私信频道Id");
            }
            return false;
        }

        /// <summary>
        /// 频道消息粘贴表情
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// <param name="arg4">表情Id</param>
        /// <param name="arg5">是否为emoji</param>
        /// <param name="arg6">取消粘贴</param>
        /// </summary>
        [ProxyAPIName("频道消息粘贴表情")]
        public static bool Function_271(long arg0, long arg1, long arg2, int arg3, string arg4, bool arg5, bool arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道消息粘贴表情");
            }
            return false;
        }

        /// <summary>
        /// 撤回频道消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// </summary>
        [ProxyAPIName("撤回频道消息")]
        public static string Function_272(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回频道消息");
            }
            return "";
        }

        /// <summary>
        /// 撤回频道私信消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">私信频道Id</param>
        /// <param name="arg2">私信子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// </summary>
        [ProxyAPIName("撤回频道私信消息")]
        public static string Function_273(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 撤回频道私信消息");
            }
            return "";
        }

        /// <summary>
        /// 设置子频道精华消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// <param name="arg4">移除</param>
        /// </summary>
        [ProxyAPIName("设置子频道精华消息")]
        public static bool Function_274(long arg0, long arg1, long arg2, int arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置子频道精华消息");
            }
            return false;
        }

        /// <summary>
        /// 禁言频道成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id</param>
        /// <param name="arg3">禁言秒数</param>
        /// </summary>
        [ProxyAPIName("禁言频道成员")]
        public static bool Function_275(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 禁言频道成员");
            }
            return false;
        }

        /// <summary>
        /// 设置频道全员禁言
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">禁言秒数</param>
        /// </summary>
        [ProxyAPIName("设置频道全员禁言")]
        public static bool Function_276(long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置频道全员禁言");
            }
            return false;
        }

        /// <summary>
        /// 移除频道成员
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id</param>
        /// <param name="arg3">拉入黑名单</param>
        /// </summary>
        [ProxyAPIName("移除频道成员")]
        public static bool Function_277(long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 移除频道成员");
            }
            return false;
        }

        /// <summary>
        /// 移除频道成员_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id列表</param>
        /// <param name="arg3">拉入黑名单</param>
        /// </summary>
        [ProxyAPIName("移除频道成员_批量")]
        public static bool Function_278(long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 移除频道成员_批量");
            }
            return false;
        }

        /// <summary>
        /// 退出频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// </summary>
        [ProxyAPIName("退出频道")]
        public static bool Function_279(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 退出频道");
            }
            return false;
        }

        /// <summary>
        /// 更改频道名称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">新名称</param>
        /// <param name="arg3">字色</param>
        /// </summary>
        [ProxyAPIName("更改频道名称")]
        public static bool Function_280(long arg0, long arg1, string arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 更改频道名称");
            }
            return false;
        }

        /// <summary>
        /// 修改频道简介
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">新简介</param>
        /// </summary>
        [ProxyAPIName("修改频道简介")]
        public static bool Function_281(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改频道简介");
            }
            return false;
        }

        /// <summary>
        /// 设置我的频道昵称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道昵称</param>
        /// </summary>
        [ProxyAPIName("设置我的频道昵称")]
        public static bool Function_282(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置我的频道昵称");
            }
            return false;
        }

        /// <summary>
        /// 置子频道观看权限
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">类型</param>
        /// <param name="arg4">指定身份组Id</param>
        /// <param name="arg5">是否取消身份组观看权限</param>
        /// <param name="arg6">指定频道成员Id</param>
        /// <param name="arg7">是否取消频道成员观看权限</param>
        /// </summary>
        [ProxyAPIName("置子频道观看权限")]
        public static bool Function_283(long arg0, long arg1, long arg2, int arg3, long arg4, bool arg5, long arg6, bool arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置子频道观看权限");
            }
            return false;
        }

        /// <summary>
        /// 置子频道发言权限
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">类型</param>
        /// <param name="arg4">指定身份组Id</param>
        /// <param name="arg5">是否取消身份组发言权限</param>
        /// <param name="arg6">指定频道成员</param>
        /// <param name="arg7">是否取消频道成员发言权限</param>
        /// </summary>
        [ProxyAPIName("置子频道发言权限")]
        public static bool Function_284(long arg0, long arg1, long arg2, int arg3, long arg4, bool arg5, long arg6, bool arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 置子频道发言权限");
            }
            return false;
        }

        /// <summary>
        /// 子频道消息提醒设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">对成员打开消息提醒</param>
        /// </summary>
        [ProxyAPIName("子频道消息提醒设置")]
        public static bool Function_285(long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 子频道消息提醒设置");
            }
            return false;
        }

        /// <summary>
        /// 子频道慢速模式设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">模式</param>
        /// </summary>
        [ProxyAPIName("子频道慢速模式设置")]
        public static bool Function_286(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 子频道慢速模式设置");
            }
            return false;
        }

        /// <summary>
        /// 修改子频道名称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">新名称</param>
        /// </summary>
        [ProxyAPIName("修改子频道名称")]
        public static bool Function_287(long arg0, long arg1, long arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改子频道名称");
            }
            return false;
        }

        /// <summary>
        /// 删除子频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// </summary>
        [ProxyAPIName("删除子频道")]
        public static bool Function_288(long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除子频道");
            }
            return false;
        }

        /// <summary>
        /// 修改我的频道用户信息_昵称
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">新昵称</param>
        /// </summary>
        [ProxyAPIName("修改我的频道用户信息_昵称")]
        public static bool Function_289(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改我的频道用户信息_昵称");
            }
            return false;
        }

        /// <summary>
        /// 修改我的频道用户信息_性别
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">性别</param>
        /// </summary>
        [ProxyAPIName("修改我的频道用户信息_性别")]
        public static bool Function_290(long arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改我的频道用户信息_性别");
            }
            return false;
        }

        /// <summary>
        /// 修改我的频道用户信息_年龄
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">年龄</param>
        /// </summary>
        [ProxyAPIName("修改我的频道用户信息_年龄")]
        public static bool Function_291(long arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改我的频道用户信息_年龄");
            }
            return false;
        }

        /// <summary>
        /// 修改我的频道用户信息_所在地
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">国家代码</param>
        /// <param name="arg2">国家名称</param>
        /// <param name="arg3">省份代码</param>
        /// <param name="arg4">省份名称</param>
        /// <param name="arg5">市区代码</param>
        /// <param name="arg6">市区名称</param>
        /// </summary>
        [ProxyAPIName("修改我的频道用户信息_所在地")]
        public static bool Function_292(long arg0, int arg1, string arg2, int arg3, string arg4, int arg5, string arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改我的频道用户信息_所在地");
            }
            return false;
        }

        /// <summary>
        /// 设置是否允许别人私信我
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">不允许</param>
        /// </summary>
        [ProxyAPIName("设置是否允许别人私信我")]
        public static bool Function_293(long arg0, bool arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置是否允许别人私信我");
            }
            return false;
        }

        /// <summary>
        /// 设置频道加入验证方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">验证方式</param>
        /// <param name="arg3">问题</param>
        /// <param name="arg4">答案</param>
        /// </summary>
        [ProxyAPIName("设置频道加入验证方式")]
        public static bool Function_294(long arg0, long arg1, int arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置频道加入验证方式");
            }
            return false;
        }

        /// <summary>
        /// 搜索频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">关键词</param>
        /// <param name="arg2">第几页</param>
        /// <param name="arg3">结果</param>
        /// </summary>
        [ProxyAPIName("搜索频道")]
        public static int Function_295(long arg0, string arg1, int arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 搜索频道");
            }
            return 0;
        }

        /// <summary>
        /// 取频道封面
        /// <param name="arg0">频道Id</param>
        /// </summary>
        [ProxyAPIName("取频道封面")]
        public static string Function_296(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道封面");
            }
            return "";
        }

        /// <summary>
        /// 取频道头像
        /// <param name="arg0">频道Id</param>
        /// <param name="arg1">高清大图</param>
        /// </summary>
        [ProxyAPIName("取频道头像")]
        public static string Function_297(long arg0, bool arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道头像");
            }
            return "";
        }

        /// <summary>
        /// 获取频道成员列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// <param name="arg3">翻页数据</param>
        /// <param name="arg4">翻页信息</param>
        /// </summary>
        [ProxyAPIName("获取频道成员列表")]
        public static int Function_298(long arg0, long arg1, object arg2, int arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取频道成员列表");
            }
            return 0;
        }

        /// <summary>
        /// 取频道信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取频道信息")]
        public static bool Function_299(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道信息");
            }
            return false;
        }

        /// <summary>
        /// 取频道加入验证方式
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">验证方式</param>
        /// <param name="arg3">问题</param>
        /// </summary>
        [ProxyAPIName("取频道加入验证方式")]
        public static bool Function_300(long arg0, long arg1, int arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道加入验证方式");
            }
            return false;
        }

        /// <summary>
        /// 申请加入频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">验证方式</param>
        /// <param name="arg2">频道Id</param>
        /// <param name="arg3">频道Token</param>
        /// <param name="arg4">答案</param>
        /// </summary>
        [ProxyAPIName("申请加入频道")]
        public static string Function_301(long arg0, int arg1, long arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 申请加入频道");
            }
            return "";
        }

        /// <summary>
        /// 取频道文件下载地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">fileid</param>
        /// <param name="arg4">filename</param>
        /// </summary>
        [ProxyAPIName("取频道文件下载地址")]
        public static string Function_302(long arg0, long arg1, long arg2, string arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道文件下载地址");
            }
            return "";
        }

        /// <summary>
        /// 频道拼手气红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">频道Id</param>
        /// <param name="arg4">子频道Id</param>
        /// <param name="arg5">祝福语</param>
        /// <param name="arg6">红包皮肤Id</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("频道拼手气红包")]
        public static string Function_303(long arg0, int arg1, int arg2, long arg3, long arg4, string arg5, int arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道拼手气红包");
            }
            return "";
        }

        /// <summary>
        /// 频道普通红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">频道Id</param>
        /// <param name="arg4">子频道Id</param>
        /// <param name="arg5">祝福语</param>
        /// <param name="arg6">红包皮肤Id</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("频道普通红包")]
        public static string Function_304(long arg0, int arg1, int arg2, long arg3, long arg4, string arg5, int arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道普通红包");
            }
            return "";
        }

        /// <summary>
        /// 频道专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">总数量</param>
        /// <param name="arg2">总金额</param>
        /// <param name="arg3">频道Id</param>
        /// <param name="arg4">子频道Id</param>
        /// <param name="arg5">领取人频道用户Id</param>
        /// <param name="arg6">祝福语</param>
        /// <param name="arg7">支付密码</param>
        /// <param name="arg8">银行卡序列</param>
        /// <param name="arg9">验证码信息</param>
        /// </summary>
        [ProxyAPIName("频道专属红包")]
        public static string Function_305(long arg0, int arg1, int arg2, long arg3, long arg4, long arg5, string arg6, string arg7, int arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道专属红包");
            }
            return "";
        }

        /// <summary>
        /// 领取频道专属红包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">发送人频道用户Id</param>
        /// <param name="arg4">红包文本代码</param>
        /// </summary>
        [ProxyAPIName("领取频道专属红包")]
        public static string Function_306(long arg0, long arg1, long arg2, long arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 领取频道专属红包");
            }
            return "";
        }

        /// <summary>
        /// 取频道成员身份组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id</param>
        /// <param name="arg3">结果</param>
        /// </summary>
        [ProxyAPIName("取频道成员身份组")]
        public static int Function_307(long arg0, long arg1, long arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道成员身份组");
            }
            return 0;
        }

        /// <summary>
        /// 设置频道成员身份组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">频道用户Id</param>
        /// <param name="arg3">身份组Id列表</param>
        /// <param name="arg4">是否取消身份组</param>
        /// </summary>
        [ProxyAPIName("设置频道成员身份组")]
        public static bool Function_308(long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置频道成员身份组");
            }
            return false;
        }

        /// <summary>
        /// 修改身份组信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// <param name="arg3">身份组名</param>
        /// <param name="arg4">身份组外显颜色代码</param>
        /// <param name="arg5">是否在成员列表中单独展示</param>
        /// </summary>
        [ProxyAPIName("修改身份组信息")]
        public static bool Function_309(long arg0, long arg1, long arg2, string arg3, long arg4, bool arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改身份组信息");
            }
            return false;
        }

        /// <summary>
        /// 删除身份组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// </summary>
        [ProxyAPIName("删除身份组")]
        public static bool Function_310(long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除身份组");
            }
            return false;
        }

        /// <summary>
        /// 新增身份组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组名</param>
        /// <param name="arg3">身份组外显颜色代码</param>
        /// <param name="arg4">是否在成员列表中单独展示</param>
        /// </summary>
        [ProxyAPIName("新增身份组")]
        public static long Function_311(long arg0, long arg1, string arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 新增身份组");
            }
            return 0;
        }

        /// <summary>
        /// 取频道身份组列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取频道身份组列表")]
        public static int Function_312(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道身份组列表");
            }
            return 0;
        }

        /// <summary>
        /// 取子频道列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取子频道列表")]
        public static int Function_313(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道列表");
            }
            return 0;
        }

        /// <summary>
        /// 取频道用户个性档案
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取频道用户个性档案")]
        public static int Function_314(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道用户个性档案");
            }
            return 0;
        }

        /// <summary>
        /// 取频道用户资料
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取频道用户资料")]
        public static bool Function_315(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道用户资料");
            }
            return false;
        }

        /// <summary>
        /// 刷新频道列表缓存
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("刷新频道列表缓存")]
        public static bool Function_316(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 刷新频道列表缓存");
            }
            return false;
        }

        /// <summary>
        /// 取频道列表_从缓存
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取频道列表_从缓存")]
        public static string Function_317(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道列表_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取频道用户昵称_从缓存
        /// <param name="arg0">频道用户Id</param>
        /// </summary>
        [ProxyAPIName("取频道用户昵称_从缓存")]
        public static string Function_318(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道用户昵称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取频道名称_从缓存
        /// <param name="arg0">频道Id</param>
        /// </summary>
        [ProxyAPIName("取频道名称_从缓存")]
        public static string Function_319(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道名称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取子频道名称_从缓存
        /// <param name="arg0">频道Id</param>
        /// <param name="arg1">子频道Id</param>
        /// </summary>
        [ProxyAPIName("取子频道名称_从缓存")]
        public static string Function_320(string arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道名称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取频道昵称_从缓存
        /// <param name="arg0">频道Id</param>
        /// <param name="arg1">频道用户Id</param>
        /// </summary>
        [ProxyAPIName("取频道昵称_从缓存")]
        public static string Function_321(string arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道昵称_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 取子频道分组列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">结果</param>
        /// </summary>
        [ProxyAPIName("取子频道分组列表")]
        public static int Function_322(long arg0, long arg1, object arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道分组列表");
            }
            return 0;
        }

        /// <summary>
        /// 取私信频道列表_从缓存
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取私信频道列表_从缓存")]
        public static string Function_323(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取私信频道列表_从缓存");
            }
            return "";
        }

        /// <summary>
        /// 上传频道文件
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">文件路径</param>
        /// <param name="arg4">上传进度回调函数</param>
        /// <param name="arg5">图片宽度</param>
        /// <param name="arg6">图片高度</param>
        /// </summary>
        [ProxyAPIName("上传频道文件")]
        public static string Function_324(long arg0, long arg1, long arg2, string arg3, int arg4, int arg5, int arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传频道文件");
            }
            return "";
        }

        /// <summary>
        /// 更改频道消息内容
        /// <param name="arg0">数据指针</param>
        /// <param name="arg1">新消息内容</param>
        /// </summary>
        [ProxyAPIName("更改频道消息内容")]
        public static bool Function_325(int arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 更改频道消息内容");
            }
            return false;
        }

        /// <summary>
        /// Emoji转频道EmojiId
        /// <param name="arg0">Emoji代码</param>
        /// </summary>
        [ProxyAPIName("Emoji转频道EmojiId")]
        public static string Function_326(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API Emoji转频道EmojiId");
            }
            return "";
        }

        /// <summary>
        /// 频道EmojiId转Emoji
        /// <param name="arg0">频道EmojiId</param>
        /// </summary>
        [ProxyAPIName("频道EmojiId转Emoji")]
        public static string Function_327(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道EmojiId转Emoji");
            }
            return "";
        }

        /// <summary>
        /// Emoji转QQ空间EmId
        /// <param name="arg0">Emoji代码</param>
        /// </summary>
        [ProxyAPIName("Emoji转QQ空间EmId")]
        public static string Function_328(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API Emoji转QQ空间EmId");
            }
            return "";
        }

        /// <summary>
        /// QQ空间EmId转Emoji
        /// <param name="arg0">QQ空间EmId</param>
        /// </summary>
        [ProxyAPIName("QQ空间EmId转Emoji")]
        public static string Function_329(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ空间EmId转Emoji");
            }
            return "";
        }

        /// <summary>
        /// 小黄豆Id转QQ空间EmId
        /// <param name="arg0">小黄豆Id</param>
        /// </summary>
        [ProxyAPIName("小黄豆Id转QQ空间EmId")]
        public static string Function_330(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 小黄豆Id转QQ空间EmId");
            }
            return "";
        }

        /// <summary>
        /// QQ空间EmId转小黄豆Id
        /// <param name="arg0">QQ空间EmId</param>
        /// </summary>
        [ProxyAPIName("QQ空间EmId转小黄豆Id")]
        public static string Function_331(string arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ空间EmId转小黄豆Id");
            }
            return "";
        }

        /// <summary>
        /// 取特定身份组成员列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// <param name="arg3">结果</param>
        /// <param name="arg4">开始位置</param>
        /// </summary>
        [ProxyAPIName("取特定身份组成员列表")]
        public static int Function_332(long arg0, long arg1, long arg2, object arg3, int arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取特定身份组成员列表");
            }
            return 0;
        }

        /// <summary>
        /// 取子频道分组结构
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// </summary>
        [ProxyAPIName("取子频道分组结构")]
        public static string Function_333(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道分组结构");
            }
            return "";
        }

        /// <summary>
        /// 设置子频道分组结构
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道分组结构</param>
        /// </summary>
        [ProxyAPIName("设置子频道分组结构")]
        public static string Function_334(long arg0, long arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置子频道分组结构");
            }
            return "";
        }

        /// <summary>
        /// 删除子频道_批量
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id列表</param>
        /// </summary>
        [ProxyAPIName("删除子频道_批量")]
        public static bool Function_335(long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除子频道_批量");
            }
            return false;
        }

        /// <summary>
        /// 创建子频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">原始子频道分组结构</param>
        /// <param name="arg3">子频道名称</param>
        /// <param name="arg4">消息提醒设置</param>
        /// <param name="arg5">子频道类型</param>
        /// <param name="arg6">子频道子类型</param>
        /// <param name="arg7">应用Id</param>
        /// <param name="arg8">可视类型</param>
        /// <param name="arg9">指定成员频道用户Id列表</param>
        /// <param name="arg10">指定身份组Id</param>
        /// <param name="arg11">所属分组index</param>
        /// <param name="arg12">分组内位置</param>
        /// </summary>
        [ProxyAPIName("创建子频道")]
        public static bool Function_336(long arg0, long arg1, string arg2, string arg3, int arg4, int arg5, int arg6, int arg7, int arg8, long arg9, long arg10, int arg11, int arg12)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, arg10={arg10}, arg11={arg11}, arg12={arg12}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建子频道");
            }
            return false;
        }

        /// <summary>
        /// 构造卡片消息文本代码
        /// <param name="arg0">卡片代码</param>
        /// <param name="arg1">类型</param>
        /// <param name="arg2">强制发送</param>
        /// </summary>
        [ProxyAPIName("构造卡片消息文本代码")]
        public static string Function_337(string arg0, int arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 构造卡片消息文本代码");
            }
            return "";
        }

        /// <summary>
        /// 分享音乐_频道
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">歌曲名</param>
        /// <param name="arg4">歌手名</param>
        /// <param name="arg5">跳转地址</param>
        /// <param name="arg6">封面地址</param>
        /// <param name="arg7">文件地址</param>
        /// <param name="arg8">应用类型</param>
        /// </summary>
        [ProxyAPIName("分享音乐_频道")]
        public static bool Function_338(long arg0, long arg1, long arg2, string arg3, string arg4, string arg5, string arg6, string arg7, int arg8)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 分享音乐_频道");
            }
            return false;
        }

        /// <summary>
        /// 修改频道排序
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id列表</param>
        /// </summary>
        [ProxyAPIName("修改频道排序")]
        public static bool Function_339(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改频道排序");
            }
            return false;
        }

        /// <summary>
        /// 处理频道加入申请
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户标识</param>
        /// <param name="arg2">操作</param>
        /// </summary>
        [ProxyAPIName("处理频道加入申请")]
        public static bool Function_340(long arg0, string arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 处理频道加入申请");
            }
            return false;
        }

        /// <summary>
        /// 查询群设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">群号</param>
        /// </summary>
        [ProxyAPIName("查询群设置")]
        public static string Function_341(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 查询群设置");
            }
            return "";
        }

        /// <summary>
        /// 取子频道管理列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">结果</param>
        /// </summary>
        [ProxyAPIName("取子频道管理列表")]
        public static int Function_342(long arg0, long arg1, long arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取子频道管理列表");
            }
            return 0;
        }

        /// <summary>
        /// 设置子频道管理
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">频道用户Id列表</param>
        /// <param name="arg4">是否取消子频道管理</param>
        /// </summary>
        [ProxyAPIName("设置子频道管理")]
        public static bool Function_343(long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置子频道管理");
            }
            return false;
        }

        /// <summary>
        /// 设置指定身份组子频道观看权限
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// <param name="arg3">欲设置的子频道Id列表</param>
        /// <param name="arg4">是否取消观看权限</param>
        /// </summary>
        [ProxyAPIName("设置指定身份组子频道观看权限")]
        public static bool Function_344(long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置指定身份组子频道观看权限");
            }
            return false;
        }

        /// <summary>
        /// 设置指定身份组子频道发言权限
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">身份组Id</param>
        /// <param name="arg3">欲设置的子频道Id列表</param>
        /// <param name="arg4">是否取消发言权限</param>
        /// </summary>
        [ProxyAPIName("设置指定身份组子频道发言权限")]
        public static bool Function_345(long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置指定身份组子频道发言权限");
            }
            return false;
        }

        /// <summary>
        /// 设置直播子频道主播
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">直播子频道Id</param>
        /// <param name="arg3">欲设置的频道用户Id列表</param>
        /// <param name="arg4">是否取消主播身份</param>
        /// </summary>
        [ProxyAPIName("设置直播子频道主播")]
        public static bool Function_346(long arg0, long arg1, long arg2, long arg3, bool arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置直播子频道主播");
            }
            return false;
        }

        /// <summary>
        /// 获取频道分享链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// </summary>
        [ProxyAPIName("获取频道分享链接")]
        public static string Function_347(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取频道分享链接");
            }
            return "";
        }

        /// <summary>
        /// 获取子频道分享链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// </summary>
        [ProxyAPIName("获取子频道分享链接")]
        public static string Function_348(long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取子频道分享链接");
            }
            return "";
        }

        /// <summary>
        /// 子频道消息通知设置
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">开启消息通知</param>
        /// </summary>
        [ProxyAPIName("子频道消息通知设置")]
        public static bool Function_349(long arg0, long arg1, long arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 子频道消息通知设置");
            }
            return false;
        }

        /// <summary>
        /// 获取红包领取详情s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">红包来源</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">红包文本代码</param>
        /// <param name="arg4">红包类型</param>
        /// <param name="arg5">起始位置</param>
        /// </summary>
        [ProxyAPIName("获取红包领取详情s")]
        public static string Function_350(long arg0, long arg1, long arg2, string arg3, int arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取红包领取详情s");
            }
            return "";
        }

        /// <summary>
        /// 取话题子频道帖子列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">话题子频道Id</param>
        /// <param name="arg3">结果</param>
        /// <param name="arg4">翻页信息</param>
        /// </summary>
        [ProxyAPIName("取话题子频道帖子列表")]
        public static int Function_351(long arg0, long arg1, long arg2, object arg3, string arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取话题子频道帖子列表");
            }
            return 0;
        }

        /// <summary>
        /// 获取日程列表
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">时间戳</param>
        /// <param name="arg4">结果</param>
        /// </summary>
        [ProxyAPIName("获取日程列表")]
        public static int Function_352(long arg0, long arg1, long arg2, long arg3, object arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取日程列表");
            }
            return 0;
        }

        /// <summary>
        /// 获取日程链接
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">日程Id</param>
        /// </summary>
        [ProxyAPIName("获取日程链接")]
        public static string Function_353(long arg0, long arg1, long arg2, long arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 获取日程链接");
            }
            return "";
        }

        /// <summary>
        /// 取日程信息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">日程Id</param>
        /// <param name="arg4">信息</param>
        /// </summary>
        [ProxyAPIName("取日程信息")]
        public static bool Function_354(long arg0, long arg1, long arg2, long arg3, object arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取日程信息");
            }
            return false;
        }

        /// <summary>
        /// 创建日程
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">日程名</param>
        /// <param name="arg4">日程描述</param>
        /// <param name="arg5">开始时间戳</param>
        /// <param name="arg6">结束时间戳</param>
        /// <param name="arg7">提醒</param>
        /// <param name="arg8">开始时跳转的子频道Id</param>
        /// <param name="arg9">信息</param>
        /// </summary>
        [ProxyAPIName("创建日程")]
        public static bool Function_355(long arg0, long arg1, long arg2, string arg3, string arg4, long arg5, long arg6, int arg7, long arg8, object arg9)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, arg8={arg8}, arg9={arg9}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 创建日程");
            }
            return false;
        }

        /// <summary>
        /// 取QQ头像K值
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">对方QQ</param>
        /// </summary>
        [ProxyAPIName("取QQ头像K值")]
        public static string Function_356(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ头像K值");
            }
            return "";
        }

        /// <summary>
        /// 删除日程
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">日程子频道Id</param>
        /// <param name="arg3">日程信息</param>
        /// </summary>
        [ProxyAPIName("删除日程")]
        public static bool Function_357(long arg0, long arg1, long arg2, object arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除日程");
            }
            return false;
        }

        /// <summary>
        /// 发送通行证到群
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">群号</param>
        /// <param name="arg3">通行证数量</param>
        /// </summary>
        [ProxyAPIName("发送通行证到群")]
        public static bool Function_358(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送通行证到群");
            }
            return false;
        }

        /// <summary>
        /// 发送通行证到好友
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">对方QQ</param>
        /// </summary>
        [ProxyAPIName("发送通行证到好友")]
        public static bool Function_359(long arg0, long arg1, long arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送通行证到好友");
            }
            return false;
        }

        /// <summary>
        /// 屏蔽频道用户私信
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户Id</param>
        /// <param name="arg2">解除屏蔽</param>
        /// </summary>
        [ProxyAPIName("屏蔽频道用户私信")]
        public static bool Function_360(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 屏蔽频道用户私信");
            }
            return false;
        }

        /// <summary>
        /// 频道用户私信免打扰
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道用户Id</param>
        /// <param name="arg2">关闭免打扰</param>
        /// </summary>
        [ProxyAPIName("频道用户私信免打扰")]
        public static bool Function_361(long arg0, long arg1, bool arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 频道用户私信免打扰");
            }
            return false;
        }

        /// <summary>
        /// QQ列表_添加QQ
        /// <param name="arg0">QQ</param>
        /// <param name="arg1">密码</param>
        /// <param name="arg2">手机品牌</param>
        /// <param name="arg3">手机型号</param>
        /// <param name="arg4">协议</param>
        /// <param name="arg5">guid</param>
        /// </summary>
        [ProxyAPIName("QQ列表_添加QQ")]
        public static string Function_362(long arg0, string arg1, string arg2, string arg3, int arg4, string arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_添加QQ");
            }
            return "";
        }

        /// <summary>
        /// QQ列表_删除QQ
        /// <param name="arg0">QQ</param>
        /// <param name="arg1">彻底删除</param>
        /// </summary>
        [ProxyAPIName("QQ列表_删除QQ")]
        public static string Function_363(long arg0, bool arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API QQ列表_删除QQ");
            }
            return "";
        }

        /// <summary>
        /// 登录指定QQ_二次登录
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("登录指定QQ_二次登录")]
        public static bool Function_364(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 登录指定QQ_二次登录");
            }
            return false;
        }

        /// <summary>
        /// 是否已设置QQ密码
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("是否已设置QQ密码")]
        public static bool Function_365(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 是否已设置QQ密码");
            }
            return false;
        }

        /// <summary>
        /// 取框架插件列表
        /// </summary>
        [ProxyAPIName("取框架插件列表")]
        public static string Function_366()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取框架插件列表");
            }
            return "";
        }

        /// <summary>
        /// 取在线移动设备列表
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取在线移动设备列表")]
        public static string Function_367(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取在线移动设备列表");
            }
            return "";
        }

        /// <summary>
        /// 设置频道全局公告_指定消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">消息req</param>
        /// </summary>
        [ProxyAPIName("设置频道全局公告_指定消息")]
        public static bool Function_368(long arg0, long arg1, long arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置频道全局公告_指定消息");
            }
            return false;
        }

        /// <summary>
        /// 取频道号
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// </summary>
        [ProxyAPIName("取频道号")]
        public static string Function_369(long arg0, long arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道号");
            }
            return "";
        }

        /// <summary>
        /// 设置位置共享s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标</param>
        /// <param name="arg2">经度</param>
        /// <param name="arg3">纬度</param>
        /// <param name="arg4">是否开启</param>
        /// <param name="arg5">类型</param>
        /// </summary>
        [ProxyAPIName("设置位置共享s")]
        public static bool Function_370(long arg0, long arg1, double arg2, double arg3, bool arg4, int arg5)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 设置位置共享s");
            }
            return false;
        }

        /// <summary>
        /// 上报当前位置s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">目标</param>
        /// <param name="arg2">经度</param>
        /// <param name="arg3">纬度</param>
        /// <param name="arg4">指针偏角</param>
        /// </summary>
        [ProxyAPIName("上报当前位置s")]
        public static bool Function_371(long arg0, long arg1, double arg2, double arg3, double arg4)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上报当前位置s");
            }
            return false;
        }

        /// <summary>
        /// 移动好友分组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">分组Id</param>
        /// </summary>
        [ProxyAPIName("移动好友分组")]
        public static bool Function_372(long arg0, long arg1, int arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 移动好友分组");
            }
            return false;
        }

        /// <summary>
        /// 修改好友分组名
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">分组Id</param>
        /// <param name="arg2">分组名</param>
        /// </summary>
        [ProxyAPIName("修改好友分组名")]
        public static bool Function_373(long arg0, int arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 修改好友分组名");
            }
            return false;
        }

        /// <summary>
        /// 删除好友分组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">分组Id</param>
        /// </summary>
        [ProxyAPIName("删除好友分组")]
        public static bool Function_374(long arg0, int arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除好友分组");
            }
            return false;
        }

        /// <summary>
        /// 取好友分组列表
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取好友分组列表")]
        public static string Function_375(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取好友分组列表");
            }
            return "";
        }

        /// <summary>
        /// 新增好友分组
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">分组名</param>
        /// </summary>
        [ProxyAPIName("新增好友分组")]
        public static int Function_376(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 新增好友分组");
            }
            return 0;
        }

        /// <summary>
        /// 取频道红包pre_grap_token
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">频道Id</param>
        /// <param name="arg2">子频道Id</param>
        /// <param name="arg3">红包listid</param>
        /// <param name="arg4">红包authkey</param>
        /// <param name="arg5">红包channel</param>
        /// <param name="arg6">红包发送人频道用户Id</param>
        /// </summary>
        [ProxyAPIName("取频道红包pre_grap_token")]
        public static string Function_377(long arg0, long arg1, long arg2, string arg3, string arg4, string arg5, long arg6)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取频道红包pre_grap_token");
            }
            return "";
        }

        /// <summary>
        /// 语音红包匹配
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">红包接收对象</param>
        /// <param name="arg2">红包标题</param>
        /// <param name="arg3">匹配语音hash</param>
        /// <param name="arg4">红包listid</param>
        /// <param name="arg5">红包authkey</param>
        /// <param name="arg6">红包发送者QQ</param>
        /// <param name="arg7">红包来源类型</param>
        /// </summary>
        [ProxyAPIName("语音红包匹配")]
        public static string Function_378(long arg0, long arg1, string arg2, string arg3, string arg4, string arg5, long arg6, int arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 语音红包匹配");
            }
            return "";
        }

        /// <summary>
        /// 上传群聊语音红包匹配语音
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">红包来源群号</param>
        /// <param name="arg2">audio</param>
        /// </summary>
        [ProxyAPIName("上传群聊语音红包匹配语音")]
        public static string Function_379(long arg0, long arg1, byte[] arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群聊语音红包匹配语音");
            }
            return "";
        }

        /// <summary>
        /// 取合并转发消息内容
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">resId</param>
        /// </summary>
        [ProxyAPIName("取合并转发消息内容")]
        public static string Function_380(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取合并转发消息内容");
            }
            return "";
        }

        /// <summary>
        /// 上传合并转发消息
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">消息来源</param>
        /// <param name="arg2">消息封面</param>
        /// <param name="arg3">合并转发消息内容</param>
        /// </summary>
        [ProxyAPIName("上传合并转发消息")]
        public static string Function_381(long arg0, string arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传合并转发消息");
            }
            return "";
        }

        /// <summary>
        /// 语音转文字
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">语音hash</param>
        /// <param name="arg2">语音token</param>
        /// </summary>
        [ProxyAPIName("语音转文字")]
        public static string Function_382(long arg0, string arg1, string arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 语音转文字");
            }
            return "";
        }

        /// <summary>
        /// 发送功能包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">功能cmd</param>
        /// <param name="arg2">最大等待时长</param>
        /// <param name="arg3">数据</param>
        /// </summary>
        [ProxyAPIName("发送功能包")]
        public static bool Function_383(long arg0, string arg1, int arg2, byte[] arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发送功能包");
            }
            return false;
        }

        /// <summary>
        /// 二维码扫一扫授权登录其他应用
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">k值</param>
        /// </summary>
        [ProxyAPIName("二维码扫一扫授权登录其他应用")]
        public static string Function_384(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 二维码扫一扫授权登录其他应用");
            }
            return "";
        }

        /// <summary>
        /// 取历史登录设备guid列表
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取历史登录设备guid列表")]
        public static string Function_385(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取历史登录设备guid列表");
            }
            return "";
        }

        /// <summary>
        /// 二维码扫一扫授权其他设备资料辅助验证登录
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">str_url</param>
        /// </summary>
        [ProxyAPIName("二维码扫一扫授权其他设备资料辅助验证登录")]
        public static string Function_386(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 二维码扫一扫授权其他设备资料辅助验证登录");
            }
            return "";
        }

        /// <summary>
        /// 关闭设备锁
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("关闭设备锁")]
        public static bool Function_387(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 关闭设备锁");
            }
            return false;
        }

        /// <summary>
        /// 恢复设备锁
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("恢复设备锁")]
        public static bool Function_388(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 恢复设备锁");
            }
            return false;
        }

        /// <summary>
        /// 余额提现
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">提现金额</param>
        /// <param name="arg2">支付密码</param>
        /// <param name="arg3">银行卡序列</param>
        /// </summary>
        [ProxyAPIName("余额提现")]
        public static string Function_389(long arg0, int arg1, string arg2, int arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 余额提现");
            }
            return "";
        }

        /// <summary>
        /// 取h5钱包cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取h5钱包cookie")]
        public static string Function_390(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取h5钱包cookie");
            }
            return "";
        }

        /// <summary>
        /// 取QQ会员中心cookie
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取QQ会员中心cookie")]
        public static string Function_391(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取QQ会员中心cookie");
            }
            return "";
        }

        /// <summary>
        /// 说说点赞
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">说说发布者QQ</param>
        /// <param name="arg2">说说feedskey</param>
        /// <param name="arg3">取消点赞</param>
        /// </summary>
        [ProxyAPIName("说说点赞")]
        public static bool Function_392(long arg0, long arg1, string arg2, bool arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 说说点赞");
            }
            return false;
        }

        /// <summary>
        /// 说说评论
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">说说发布者QQ</param>
        /// <param name="arg2">说说feedskey</param>
        /// <param name="arg3">评论内容</param>
        /// </summary>
        [ProxyAPIName("说说评论")]
        public static bool Function_393(long arg0, long arg1, string arg2, string arg3)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 说说评论");
            }
            return false;
        }

        /// <summary>
        /// 取最新动态列表
        /// <param name="arg0">框架QQ</param>
        /// </summary>
        [ProxyAPIName("取最新动态列表")]
        public static string Function_394(long arg0)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 取最新动态列表");
            }
            return "";
        }

        /// <summary>
        /// 搜索表情包
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">关键词</param>
        /// </summary>
        [ProxyAPIName("搜索表情包")]
        public static string Function_395(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 搜索表情包");
            }
            return "";
        }

        /// <summary>
        /// 发布说说
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">内容</param>
        /// </summary>
        [ProxyAPIName("发布说说")]
        public static string Function_396(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 发布说说");
            }
            return "";
        }

        /// <summary>
        /// 经纬度定位查询详细地址
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">经度</param>
        /// <param name="arg2">纬度</param>
        /// </summary>
        [ProxyAPIName("经纬度定位查询详细地址")]
        public static string Function_397(long arg0, double arg1, double arg2)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 经纬度定位查询详细地址");
            }
            return "";
        }

        /// <summary>
        /// 取插件自身版本号
        /// </summary>
        [ProxyAPIName("取插件自身版本号")]
        public static string Function_398()
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Error("小栗子API", "使用了未实现了API 取插件自身版本号");
            }
            return "";
        }

        /// <summary>
        /// 上传群临时文件s
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">好友QQ</param>
        /// <param name="arg2">对方QQ</param>
        /// <param name="arg3">群号</param>
        /// <param name="arg4">文件路径</param>
        /// <param name="arg5">上传进度回调函数</param>
        /// <param name="arg6">Random</param>
        /// <param name="arg7">Req</param>
        /// </summary>
        [ProxyAPIName("上传群临时文件s")]
        public static string Function_399(long arg0, long arg1, long arg2, long arg3, string arg4, int arg5, long arg6, int arg7)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, arg2={arg2}, arg3={arg3}, arg4={arg4}, arg5={arg5}, arg6={arg6}, arg7={arg7}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 上传群临时文件s");
            }
            return "";
        }

        /// <summary>
        /// 删除说说
        /// <param name="arg0">框架QQ</param>
        /// <param name="arg1">说说feedskey</param>
        /// </summary>
        [ProxyAPIName("删除说说")]
        public static bool Function_400(long arg0, string arg1)
        {
            if (AppConfig.Instance.DebugMode)
            {
                LogHelper.Debug("小栗子API", $"arg0={arg0}, arg1={arg1}, ");
                LogHelper.Error("小栗子API", "使用了未实现了API 删除说说");
            }
            return false;
        }
    }
}