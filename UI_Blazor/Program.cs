using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Another_Mirai_Native.BlazorUI.Components;
using Another_Mirai_Native.BlazorUI.Models;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Another_Mirai_Native.BlazorUI
{
    public class Program
    {
        public static event Action OnBlazorServiceStarted;
        public static event Action OnBlazorServiceStoped;

        public static IHost BlazorHost { get; private set; }

        public static void Main(string[] args)
        {
            Blazor_Config.Instance.LoadConfig();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddMudServices();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<Shared>();
            builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<AuthService>());
            builder.Services.AddSingleton<CircuitHandler, AuthCircuitHandler>();

            var app = builder.Build();
            BlazorHost = app;
            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStarted.Register(() =>
            {
                OnBlazorServiceStarted?.Invoke();
            });

            lifetime.ApplicationStopped.Register(() =>
            {
                OnBlazorServiceStoped?.Invoke();
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
