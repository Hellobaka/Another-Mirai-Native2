using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Another_Mirai_Native.BlazorUI.Models
{
    public class RouteService : IDisposable
    {
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;

        public RouteService(NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
            _navigationManager.LocationChanged += OnLocationChanged;
        }

        private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            string newTitle = GetTitleForRoute(e.Location);
            await _jsRuntime.InvokeVoidAsync("setTitle", newTitle);
        }

        private string GetTitleForRoute(string location)
        {
            location = _navigationManager.ToBaseRelativePath(location);
            string title = location switch
            {
                "" => "连接",
                "chat" => "聊天",
                "dashboard" => "主页",
                "logs" => "日志",
                "plugins" => "插件管理",
                "test" => "测试",
                "setting" => "设置",
                _ => ""
            };

            if (string.IsNullOrEmpty(title))
            {
                return "AMN2 WebUI";
            }
            return $"{title} - AMN2 WebUI";
        }

        public void Dispose()
        {
            _navigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
