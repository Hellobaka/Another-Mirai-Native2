using Another_Mirai_Native.DB;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Protocol_NoConnection
{
    public class PicServer
    {
        public PicServer(string baseFolder, string ip, ushort port)
        {
            BaseFolder = baseFolder;
            ListenIP = ip;
            Port = port;
            Instance = this;
        }

        public static PicServer Instance { get; set; }

        public bool Running => ServerRunning != null && !ServerRunning.IsCancellationRequested;

        public string ListenURL => $"http://{ListenIP}:{Port}/";

        private string BaseFolder { get; set; }

        private string ListenIP { get; set; }

        private ushort Port { get; set; }

        private HttpListener Server { get; set; }

        private CancellationTokenSource ServerRunning { get; set; }

        public bool Start()
        {
            try
            {
                if (ServerRunning != null && !ServerRunning.IsCancellationRequested)
                {
                    if (!Stop())
                    {
                        throw new Exception("停止服务失败");
                    }
                }

                Server = new();
                Server.Prefixes.Add(ListenURL);
                Server.Start();
                ServerRunning = new();
                LogHelper.Info("启动图片服务器", "启动成功");
                Task.Run(() =>
                {
                    var token = ServerRunning.Token;
                    while (!token.IsCancellationRequested)
                    {
                        var context = Server.GetContext();
                        ProcessRequest(context);
                    }
                });
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("启动图片服务器", e);
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                ServerRunning?.Cancel();

                Server.Stop();
                Server.Close();

                Server = null;
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("停止图片服务器", e);
                return false;
            }
        }

        private static string GetContentType(string filename)
        {
            string extension = Path.GetExtension(filename).ToLower();
            return extension switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            string filename = Path.Combine(BaseFolder, context.Request.Url.LocalPath.TrimStart('/'));
            if (File.Exists(filename))
            {
                try
                {
                    byte[] content = File.ReadAllBytes(filename);
                    context.Response.ContentType = PicServer.GetContentType(filename);
                    context.Response.ContentLength64 = content.Length;
                    context.Response.OutputStream.Write(content, 0, content.Length);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("图片服务器处理", ex);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
        }
    }
}