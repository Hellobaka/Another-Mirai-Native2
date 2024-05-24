using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Another_Mirai_Native.BlazorUI.Components;
using Another_Mirai_Native.BlazorUI.Models;

namespace Another_Mirai_Native.BlazorUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddMudServices();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<Shared>();
            builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<AuthService>());

            var app = builder.Build();

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
