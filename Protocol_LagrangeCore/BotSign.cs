using Another_Mirai_Native.DB;
using Lagrange.Core.Common;
using Lagrange.Core.Utility.Sign;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Text;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public class BotSign : SignProvider
    {
        private const string FallbackUrl = "https://sign.lagrangecore.org/api/sign/25765";

        private string SignUrl { get; set; }

        public BotAppInfo BotAppInfo { get; set; }

        private Protocols Platform { get; set; } = Protocols.Windows;

        public BotSign(string signUrl, Protocols platform = Protocols.Windows)
        {
            SignUrl = string.IsNullOrEmpty(signUrl) ? FallbackUrl : signUrl;
            Platform = platform;
            BotAppInfo = GetBotAppInfo() ?? GetDefaultAppInfo();
        }

        private BotAppInfo GetDefaultAppInfo()
        {
            return Platform switch
            {
                Protocols.Windows => new()
                {
                    Os = "Windows",
                    Kernel = "Windows_NT",
                    VendorOs = "win32",
                    CurrentVersion = "9.9.2-15962",
                    PtVersion = "2.0.0",
                    MiscBitmap = 32764,
                    SsoVersion = 23,
                    PackageName = "com.tencent.qq",
                    WtLoginSdk = "nt.wtlogin.0.0.1",
                    AppId = 1600001604,
                    SubAppId = 537138217,
                    AppIdQrCode = 537138217,
                    AppClientVersion = 13172,

                    MainSigMap = 169742560,
                    SubSigMap = 0,
                    NTLoginType = 5
                },
                Protocols.MacOs => new()
                {
                    Os = "Mac",
                    Kernel = "Darwin",
                    VendorOs = "mac",
                    CurrentVersion = "6.9.23-20139",
                    PtVersion = "2.0.0",
                    MiscBitmap = 32764,
                    SsoVersion = 23,
                    PackageName = "com.tencent.qq",
                    WtLoginSdk = "nt.wtlogin.0.0.1",
                    AppId = 1600001602,
                    SubAppId = 537200848,
                    AppIdQrCode = 537200848,
                    AppClientVersion = 13172,

                    MainSigMap = 169742560,
                    SubSigMap = 0,
                    NTLoginType = 5
                },
                Protocols.Linux => new()
                {
                    Os = "Linux",
                    Kernel = "Linux",
                    VendorOs = "linux",
                    CurrentVersion = "3.2.10-25765",
                    MiscBitmap = 32764,
                    PtVersion = "2.0.0",
                    SsoVersion = 19,
                    PackageName = "com.tencent.qq",
                    WtLoginSdk = "nt.wtlogin.0.0.1",
                    AppId = 1600001615,
                    SubAppId = 537234773,
                    AppIdQrCode = 13697054,
                    AppClientVersion = 25765,

                    MainSigMap = 169742560,
                    SubSigMap = 0,
                    NTLoginType = 1
                },
                _ => throw new NotImplementedException()
            };
        }

        private BotAppInfo? GetBotAppInfo()
        {
            try
            {
                string url = $"{SignUrl.TrimEnd('/')}/appinfo";
                using var client = new HttpClient();
                var getTask = client.GetAsync(url);
                var response = getTask.Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    LogHelper.Error("获取签名服务器版本", $"请求失败，状态码 = {response.StatusCode}");
                    return null;
                }
                var content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<BotAppInfo>(content);
            }
            catch (Exception e)
            {
                LogHelper.Error("获取签名服务器版本", $"请求失败，{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        public override byte[]? Sign(string cmd, uint seq, byte[] body, out byte[]? ver, out string? token)
        {
            ver = null;
            token = null;
            string url = SignUrl;
            if (!WhiteListCommand.Contains(cmd))
            {
                return null;
            }

            if (string.IsNullOrEmpty(SignUrl))
            {
                throw new Exception("Sign server is not configured");
            }
            try
            {
                using var client = new HttpClient();
                var response = client.PostAsync(url, JsonContent.Create(new
                {
                    cmd,
                    seq,
                    src = Convert.ToHexString(body)
                })).Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    LogHelper.Error("签名", $"请求失败，状态码 = {response.StatusCode}");
                    return null;
                }
                var content = response.Content.ReadAsStringAsync().Result;
                var sign = JObject.Parse(content);
                ver = Convert.FromHexString(sign["value"]["extra"].ToString());
                token = Encoding.UTF8.GetString(Convert.FromHexString(sign["value"]["token"].ToString()));
                return Convert.FromHexString(sign["value"]["sign"].ToString());
            }
            catch (Exception e)
            {
                LogHelper.Error("签名", $"请求失败，{e.Message}\n{e.StackTrace}");
                return null;
            }
        }
    }
}
