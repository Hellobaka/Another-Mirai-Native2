# 新建窗口

框架只保证在 UI 线程调用窗口处理器的方法，并不限制你必须如何创建或显示窗口。除了用于拉起独立窗口，也可以把菜单处理器作为某些交互功能的入口。

## 📋 使用前提

在插件中使用 WinForms 或 WPF 时，需要满足下面两个前提：

1. 项目的目标框架必须带有 `-windows` 后缀。
2. 根据你使用的 UI 框架，在项目文件中启用对应配置。

> ⚠️ **注意**：如果没有完成这一步，项目通常会出现无法构建的问题。

## 🪟 WinForms 示例

### 🔧 启用 WinForms

1. 双击插件项目，或使用文本编辑器打开项目文件（.csproj）。
2. 将 `TargetFramework` 改为带 `-windows` 的目标框架，例如 `net9.0-windows`。
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

下面的示例演示了如何通过 `IMenuHandler` 打开窗口：

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

> 💡 **提示**：`OnMenu` 由框架在 UI 线程调用，因此可以直接操作窗口对象。示例中在窗口关闭时调用 `Hide()` 并取消关闭，这样下次点击菜单时可以复用同一个窗口实例。
>
> 💡 **提示**：`OnMenu` 设计为阻塞或不阻塞都可以。上层是在 UI 线程中通过 `BeginInvoke` 调用该方法，因此这里的阻塞不会卡住上层派发线程。

### 👀 WinForms 效果示例

![菜单](/images/SelectWinForm.png)
![WinForm](/images/WinFormDisplay.png)

## 🖼️ WPF 示例

### 🔧 启用 WPF

1. 双击插件项目，或使用文本编辑器打开项目文件（.csproj）。
2. 将 `TargetFramework` 改为带 `-windows` 的目标框架，例如 `net9.0-windows`。
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

```csharp
using Another_Mirai_Native.Abstractions.Attributes;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Handlers;
using System.ComponentModel;

[Menu("WPF 示例")]
public class WPFEntry : IMenuHandler
{
    private WPFDemo Form { get; set; }

    public void OnMenu(MenuContext e)
    {
        e.API.Logger.Info("调用菜单", "调用了 WPF 示例菜单");
        if (Form == null)
        {
            Form = new WPFDemo();
            Form.Closing += Form_Closing;
        }
        Form.Show();
    }

    private void Form_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        Form.Hide();
        e.Cancel = true;
    }
}
```

> 💡 **提示**：WPF 示例与 WinForms 的核心思路一致，都是通过菜单处理器复用同一个窗口实例。`OnMenu` 设计为阻塞或不阻塞都可以。上层是在 UI 线程中通过 `BeginInvoke` 调用该方法，因此这里的阻塞不会卡住上层派发线程。
>
> ⚠️ **注意**：WPF 窗口在调用 `Close()` 后默认会被销毁；如果希望下次菜单点击继续复用同一实例，应在 `Closing` 事件中改为隐藏窗口。
>
> 💡 **提示**：如果你需要真正销毁窗口，请去掉 `e.Cancel = true` 和 `Hide()`，并在下次打开时重新创建窗口实例。如果你选择让 `OnMenu` 持续阻塞来维持某种交互流程，建议在窗口关闭或隐藏时结束阻塞，避免这次菜单处理长时间不返回。

### 👀 WPF 效果示例

![菜单](/images/SelectWPF.png)
![WPF](/images/WPFDisplay.png)

## 💡 实践建议

1. 菜单处理器中尽量只负责窗口的创建、显示和激活，不要把大量业务逻辑直接写在 `OnMenu` 中。
2. 如果窗口里需要访问插件能力，优先通过构造函数或属性把依赖传入窗口，而不是在窗口内部重新查找全局对象。
3. 如果窗口可能被频繁打开和关闭，优先复用实例；如果窗口承载的是一次性流程，再考虑每次重新创建。
