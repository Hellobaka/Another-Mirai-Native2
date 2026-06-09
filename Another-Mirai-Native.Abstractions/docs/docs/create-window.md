# 新建窗口

框架只保证在 UI 线程调用窗口处理器的方法，并不限制窗口必须以何种方式创建或显示。除了用于拉起独立窗口，你也可以把菜单处理器作为某些交互功能的入口。

## 📋 使用前提

在插件中使用 WinForms 或 WPF 时，需要先满足以下两个前提：

1. 项目的目标框架必须带有 `-windows` 后缀。
2. 根据你使用的 UI 框架，在项目文件中启用对应配置。

> ⚠️ **注意**：如果没有完成上述配置，项目通常无法正确构建。

## 🪟 WinForms 示例

### 🔧 启用 WinForms

1. 双击插件项目，或使用文本编辑器打开项目文件（.csproj）。
2. 将 `TargetFramework` 修改为带 `-windows` 后缀的目标框架，例如 `net9.0-windows`。
3. 在 `<PropertyGroup>` 中添加 `<UseWindowsForms>true</UseWindowsForms>`。
4. 保存项目文件并重新加载项目。

```xml
<PropertyGroup>
    <TargetFramework>net9.0-windows</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
    <GenerateAmnManifest>true</GenerateAmnManifest>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <EnableDynamicLoading>true</EnableDynamicLoading>
</PropertyGroup>
```

### 🛠️ 创建 WinForms 窗口

1. 在项目上右键，选择“添加” -> “Windows 窗体”。
2. 假设窗口类名为 `WinFormDemo`。

![创建窗口](/images/CreateWinForm.png)

### 🚀 通过菜单拉起 WinForms 窗口

下面的示例演示了如何通过 `IMenuHandler` 拉起窗口：

```csharp
using Another_Mirai_Native.Abstractions.Attributes;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Handlers;
using System.Windows.Forms;

[Menu("示例菜单")]
public class FormEntry : IMenuHandler
{
    private WinFormDemo Form { get; set; }

    public void OnMenu(MenuContext e)
    {
        e.API.Logger.Info("调用菜单", "调用了示例菜单");
        if (Form == null)
        {
            Form = new WinFormDemo();
            Form.FormClosing += Form_FormClosing;
        }
        Form.Show();
    }

    private void Form_FormClosing(object? sender, FormClosingEventArgs e)
    {
        Form.Hide();
        e.Cancel = true;
    }
}
```

> 💡 **提示**：框架的 UI 线程本质上是 WinForms 线程（`Application.Run` + `BeginInvoke`），因此 WinForms 控件的消息处理完全原生，无需额外配置。
>
> 💡 **提示**：`OnMenu` 由框架在 UI 线程调用，因此可以直接操作窗口对象。示例中在窗口关闭时调用 `Hide()` 并取消关闭，这样下次点击菜单时仍可复用同一个窗口实例。
>
> 💡 **提示**：`OnMenu` 既可以设计为阻塞调用，也可以设计为非阻塞调用。上层会在 UI 线程中通过 `BeginInvoke` 调用该方法，因此这里的阻塞不会卡住上层派发线程。

### 👀 WinForms 效果示例

![菜单](/images/SelectWinForm.png)
![WinForm](/images/WinFormDisplay.png)

## 🖼️ WPF 示例

### 🔧 启用 WPF

1. 双击插件项目，或使用文本编辑器打开项目文件（.csproj）。
2. 将 `TargetFramework` 修改为带 `-windows` 后缀的目标框架，例如 `net9.0-windows`。
3. 在 `<PropertyGroup>` 中添加 `<UseWPF>true</UseWPF>`。
4. 保存项目文件并重新加载项目。

```xml
<PropertyGroup>
    <TargetFramework>net9.0-windows</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <GenerateAmnManifest>true</GenerateAmnManifest>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <EnableDynamicLoading>true</EnableDynamicLoading>
</PropertyGroup>
```

### 🛠️ 创建 WPF 窗口

1. 在项目上右键，选择“添加” -> “WPF 窗口”。
2. 假设窗口类名为 `WPFDemo`。

![创建窗口](/images/CreateWPF.png)

### 🚀 通过菜单拉起 WPF 窗口

> ⚠️ **重要**：框架的 UI 线程是 WinForms 线程（`Application.Run` + `BeginInvoke`），其消息泵无法驱动 WPF 的输入管道。直接在该线程上 `Show()` WPF 窗口会导致窗口能渲染但 **无法接收键盘输入**（TextBox 等控件无响应）。WPF 窗口必须运行在独立的 STA 线程上，该线程拥有自己的 `Dispatcher.Run()` 消息循环。

正确的做法是创建一个后台 STA 线程，在其中完成窗口的创建和 `Dispatcher.Run()`。窗口的 `Show()` / `Hide()` 通过 `Dispatcher.Invoke()` 跨线程调用：

```csharp
using Another_Mirai_Native.Abstractions.Attributes;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Handlers;
using System.Threading;
using System.Windows.Threading;

[Menu("WPF 示例")]
public class WPFEntry : IMenuHandler
{
    private WPFDemo? _window;
    private Thread? _uiThread;

    public void OnMenu(MenuContext e)
    {
        if (_window == null)
        {
            // 在独立 STA 线程上创建 WPF 窗口并启动 Dispatcher 消息循环
            using var ready = new ManualResetEventSlim(false);
            _uiThread = new Thread(() =>
            {
                _window = new WPFDemo();
                _window.Closing += (s, ev) => { ev.Cancel = true; _window.Hide(); };
                ready.Set();
                Dispatcher.Run();
            })
            {
                IsBackground = true,
            };
            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.Start();
            ready.Wait(); // 等待窗口创建完成
        }

        // 跨线程唤醒窗口
        _window.Dispatcher.Invoke(() => _window.Show());
    }
}
```

> 💡 **提示**：`ManualResetEventSlim` 确保窗口创建完成后才返回，避免后续 `Dispatcher.Invoke` 时窗口尚未初始化。
>
> 💡 **提示**：窗口的事件处理（Click、Loaded 等）天然运行在 WPF 线程上，无需额外封送。线程设为 `IsBackground = true` 确保进程退出时不会因该线程而挂起。
>
> 💡 **提示**：WinForms 窗口无此问题，因为框架 UI 线程本身就是 WinForms 消息泵，WinForms 控件可在此线程上直接使用（参见上方 WinForms 示例）。
>
> ⚠️ **注意**：WPF 窗口在调用 `Close()` 后默认会被销毁；如果希望下次点击菜单时继续复用同一实例，应在 `Closing` 事件中改为隐藏窗口。

### 🎨 使用第三方 WPF 组件库

第三方 WPF 组件库（如 MaterialDesign、HandyControl 等）通常依赖 `Application.Current.Resources` 来加载全局样式和主题字典。插件项目没有 `App.xaml`，因此需要手动创建 `Application` 实例并合并资源字典：

```csharp
_uiThread = new Thread(() =>
{
    // 创建 Application 实例，使组件库可通过 Application.Current 查找样式
    var app = new Application();

    // 等效于 App.xaml 中的 <ResourceDictionary.MergedDictionaries>
    app.Resources.MergedDictionaries.Add(new ResourceDictionary
    {
        Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml")
    });
    app.Resources.MergedDictionaries.Add(new ResourceDictionary
    {
        Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.DeepPurple.xaml")
    });

    _window = new MyWPFDemo();
    _window.Closing += (s, ev) => { ev.Cancel = true; _window.Hide(); };
    ready.Set();
    Dispatcher.Run();  // 只启动消息循环，不绑定窗口生命周期
});
```

> 💡 **提示**：多数组件库只需要 `new Application()` 来设置 `Application.Current`，无需完整的 `app.Run()` 生命周期。如果 `Dispatcher.Run()` 不够用，可改用 `app.Run()`，但需在插件 `OnDisableAsync` 中调用 `app.Shutdown()` 以干净退出消息循环。
>
> 💡 **提示**：资源 URI 使用 `pack://application:,,,/` 协议，可引用来自 NuGet 引用程序集的资源。

### 👀 WPF 效果示例

![菜单](/images/SelectWPF.png)
![WPF](/images/WPFDisplay.png)

## 💡 实践建议

1. 菜单处理器尽量只负责窗口的创建、显示和激活，不要把大量业务逻辑直接写在 `OnMenu` 中。
2. 如果窗口需要访问插件能力，优先通过构造函数或属性将依赖传入窗口，而不是在窗口内部重新查找全局对象。
3. 如果窗口会被频繁打开和关闭，优先复用实例；如果窗口承载的是一次性流程，再考虑每次重新创建。
