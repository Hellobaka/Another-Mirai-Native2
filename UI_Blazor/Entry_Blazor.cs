using Another_Mirai_Native.BlazorUI.Components;
using Another_Mirai_Native.BlazorUI.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using MudBlazor.Services;
using System.Net;

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
            ConsoleStartedSignal = new(false);
            Entry.ServerStarted += Entry_ServerStarted;

            Task.Run(() => Entry.Main(args));

            ConsoleStartedSignal.WaitOne();
            Console.WriteLine("[+]Console Service Started. Starting Blazor Service...");
            StartBlazorService();
        }

        private static void Entry_ServerStarted()
        {
            ConsoleStartedSignal?.Set();
        }

        public static void StartBlazorService()
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
            builder.WebHost.UseUrls(WebUIURL);
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
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
