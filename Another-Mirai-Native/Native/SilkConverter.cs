using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using System.Diagnostics;

namespace Another_Mirai_Native.Native
{
    public class SilkConverter
    {
        private static readonly string CMDPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\cmd.exe";

        public static bool SilkEncode(string voicePath, string extension)
        {
            if (!Directory.Exists("tools"))
            {
                LogHelper.WriteLog(LogLevel.Error, "音频格式转换", "工具丢失", "tools目录丢失，无法继续");
                return false;
            }
            if (!File.Exists(@"tools\silk_v3_encoder.exe"))
            {
                LogHelper.WriteLog(LogLevel.Error, "音频格式转换", "工具丢失", "tools\\silk_v3_encoder.exe 文件丢失，无法继续");
                return false;
            }
            if (!File.Exists(@"tools\ffmpeg.exe"))
            {
                LogHelper.WriteLog(LogLevel.Error, "音频格式转换", "工具丢失", "tools\\ffmpeg.exe 文件丢失，无法继续");
                return false;
            }
            string output = RunCMDCommand($"tools\\ffmpeg.exe -y -i \"{voicePath}\" -f s16le -ar 24000 -ac 1 \"{voicePath.Replace(extension, ".pcm")}\"");
            if (!Directory.Exists("logs\\audiologs"))
            {
                Directory.CreateDirectory("logs\\audiologs");
            }

            if (output.Contains("Invalid data found when processing input"))
            {
                LogHelper.WriteLog(LogLevel.Error, "音频格式转换", "格式错误", "接受的音频可能不是FFmpeg可转换的格式");
                return false;
            }
            if (!output.Contains("video:0kB"))
            {
                string logPath = $"{DateTime.Now:yyyyMMddHHmmss}.log";
                Directory.CreateDirectory("logs\\audio");
                File.WriteAllText(Path.Combine("logs\\audio", logPath), output);
                LogHelper.WriteLog(LogLevel.Error, "音频格式转换", "未知错误", $"FFmpeg输出已保存至{logPath}");
                return false;
            }
            string filePath = voicePath.Replace(extension, ".pcm");
            output = RunCMDCommand($"tools\\silk_v3_encoder.exe \"{filePath}\" \"{filePath.Replace(".pcm", ".silk")}\" -tencent -quiet");
            return true;
        }

        private static string RunCMDCommand(string Command)
        {
            using Process pc = new();
            Command = Command.Trim().TrimEnd('&') + "&exit";

            pc.StartInfo.FileName = CMDPath;
            pc.StartInfo.CreateNoWindow = true;
            pc.StartInfo.RedirectStandardError = true;
            pc.StartInfo.RedirectStandardInput = true;
            pc.StartInfo.RedirectStandardOutput = true;
            pc.StartInfo.UseShellExecute = false;
            pc.Start();

            pc.StandardInput.WriteLine(Command);
            pc.StandardInput.AutoFlush = true;
            string a = pc.StandardOutput.ReadToEnd();
            string b = pc.StandardError.ReadToEnd();
            string outPut = a + b;
            int P = outPut.IndexOf(Command) + Command.Length;
            outPut = outPut.Substring(P, outPut.Length - P - 3);
            pc.WaitForExit();
            pc.Close();
            return outPut;
        }
    }
}