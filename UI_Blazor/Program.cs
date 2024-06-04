using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Another_Mirai_Native.BlazorUI.Components;
using Another_Mirai_Native.BlazorUI.Models;
using System.Text;

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

            Console.SetOut(new ObservableTextWriter(Console.Out));

            app.Run();
        }

        public class ObservableTextWriter : TextWriter
        {
            private static TextWriter _consoleWriter;

            public static event Action<string> OnConsoleOutput;

            public ObservableTextWriter(TextWriter stream)
            {
                _consoleWriter = stream;
            }

            public override Encoding Encoding => _consoleWriter.Encoding;

            public override void Write(char value)
            {
                _consoleWriter.Write(value);
                OnConsoleOutput?.Invoke(value.ToString());
            }

            public override void Write(string value)
            {
                _consoleWriter.Write(value);
                OnConsoleOutput?.Invoke(value);
            }

            public override void WriteLine(string value)
            {
                _consoleWriter.WriteLine(value);
                OnConsoleOutput?.Invoke(value + Environment.NewLine);
            }

            public override void Flush()
            {
                _consoleWriter.Flush();
            }
        }
    }
}
