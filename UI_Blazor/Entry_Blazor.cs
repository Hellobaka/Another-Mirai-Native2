using Another_Mirai_Native.BlazorUI.Components;
using Another_Mirai_Native.BlazorUI.Models;
using Another_Mirai_Native.DB;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using System.Security.Cryptography.X509Certificates;

namespace Another_Mirai_Native.BlazorUI
{
    public class Entry_Blazor
    {
        public static event Action OnBlazorServiceStarted;
        public static event Action OnBlazorServiceStopped;

        public static IHost BlazorHost { get; private set; }

        public static string WebUIURL { get; private set; } = "";

        private static ManualResetEvent ConsoleStartedSignal { get; set; }

        private static bool ConsoleMode { get; set; }

        /// <summary>
        /// Console Start Entry
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            ConsoleMode = true;

            if (args.Length > 0)
            {
                Entry.Main(args);
            }
            else
            {
                ConsoleStartedSignal = new(false);
                Entry.ServerStarted += Entry_ServerStarted;

                Task.Run(() => Entry.Main(args));

                ConsoleStartedSignal.WaitOne();
                Console.WriteLine("[+]Console Service Started. Starting Blazor Service...");
                StartBlazorService();
            }
        }

        private static void Entry_ServerStarted()
        {
            ConsoleStartedSignal?.Set();
        }

        public static void StartBlazorService()
        {
            try
            {
                Blazor_Config.Instance.LoadConfig();
                var builder = WebApplication.CreateBuilder();

                // Add services to the container.
                builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();

                builder.Services.AddMudServices();

                builder.Services.AddCascadingAuthenticationState();
                builder.Services.AddScoped<AuthService>();
                builder.Services.AddScoped<RouteService>();
                builder.Services.AddScoped<Shared>();
                builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<AuthService>());
                builder.Services.AddSingleton<CircuitHandler, AuthCircuitHandler>();
                if (!ConsoleMode)
                {
                    // 清除所有默认日志提供程序
                    builder.Logging.ClearProviders();
                    builder.Logging.AddProvider(Logging.Instance);
                }
                WebUIURL = $"http://{Blazor_Config.Instance.ListenIP}:{Blazor_Config.Instance.ListenPort}";
                if (Blazor_Config.Instance.EnableHTTPS)
                {
                    WebUIURL = WebUIURL.Replace("http://", "https://");
                }
                LoadHTTPsCertificate(builder.WebHost);
                builder.WebHost.UseUrls(WebUIURL);
                if (Blazor_Config.Instance.ListenIP == "0.0.0.0" || Blazor_Config.Instance.ListenIP == "*")
                {
                    WebUIURL = $"http://127.0.0.1:{Blazor_Config.Instance.ListenPort}";
                }
                else if (Blazor_Config.Instance.ListenIP == "[::]")
                {
                    WebUIURL = $"http://[::1]:{Blazor_Config.Instance.ListenPort}";
                }
                if (Blazor_Config.Instance.EnableHTTPS)
                {
                    // for UI ContextMenu
                    WebUIURL = WebUIURL.Replace("http://", "https://");
                }
                var app = builder.Build();
                BlazorHost = app;
                var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

                lifetime.ApplicationStarted.Register(() =>
                {
                    OnBlazorServiceStarted?.Invoke();
                });

                lifetime.ApplicationStopped.Register(() =>
                {
                    OnBlazorServiceStopped?.Invoke();
                });

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                }

                app.UseStaticFiles();

                // 添加 data\image 目录的静态文件支持
                string imageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "image");
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(imageDirectory),
                    RequestPath = "/data/image"
                });
                
                app.UseAntiforgery();

                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();

                app.Run();
            }
            catch (Exception ex)
            {
                LogHelper.Error("WebUI异常", ex);
                Helper.ShowErrorDialog(ex, true);
            }
        }

        public static void LoadHTTPsCertificate(ConfigureWebHostBuilder webHost)
        {
            if (Blazor_Config.Instance.EnableHTTPS)
            {
                if (File.Exists(Blazor_Config.Instance.CertificatePath) && File.Exists(Blazor_Config.Instance.CertificateKeyPath))
                {
                    webHost.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            if (TryLoadPEM(httpsOptions))
                            {
                                LogHelper.Info("加载 HTTPS", "成功加载了 PEM 证书");
                            }
                            else if (TryLoadPFX(httpsOptions))
                            {
                                LogHelper.Info("加载 HTTPS", "成功加载了 PFX 证书");
                            }
                            else
                            {
                                LogHelper.Error("加载 HTTPS", "启用HTTPS失败，证书文件加载失败");
                            }
                        });
                    });
                }
                else
                {
                    LogHelper.Error("加载 HTTPS", "启用HTTPS失败，证书文件不存在");
                }
            }
        }

        private static bool TryLoadPFX(HttpsConnectionAdapterOptions httpsOptions)
        {
            try
            {
                httpsOptions.ServerCertificate = X509CertificateLoader.LoadPkcs12FromFile(
                    Blazor_Config.Instance.CertificatePath, Blazor_Config.Instance.CertificateKeyPath);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("加载 PFX 证书失败", ex);
                return false;
            }
        }

        private static bool TryLoadPEM(HttpsConnectionAdapterOptions httpsOptions)
        {
            try
            {
                var pemCert = X509Certificate2.CreateFromPemFile(Blazor_Config.Instance.CertificatePath, Blazor_Config.Instance.CertificateKeyPath);
                byte[] pfxBytes = pemCert.Export(X509ContentType.Pfx);
                httpsOptions.ServerCertificate = X509CertificateLoader.LoadPkcs12(pfxBytes, null);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("加载 PEM 证书失败", ex);
                return false;
            }
        }
    }
}
