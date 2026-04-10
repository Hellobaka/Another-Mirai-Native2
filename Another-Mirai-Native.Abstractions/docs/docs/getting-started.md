## CSharp Start!
通过本文可以快速了解插件基础，并完成一个骰子插件

## 先决条件
- Windows
- .net framework4.8 / .net9.0
- Visual Studio / 其他 C# 开发环境

## 创建项目
示例均以 Visual Studio 2026 做示例。
1. 打开 Visual Studio，并选择`创建新项目`
![创建项目](/images/CreateProject.png)
2. 根据要使用的目标框架，选择对应的 `类库`模板
    1. 假如选择`.net framework4.8`，则搜索`类库(.Net Framework)`
    2. 假如选择`.net9.0`，则搜索`类库`
    ![选择模板](/images/CreateClassLibrary.png)
3. 下一步选择项目位置，随后选择模板框架。
4. 看见代码页面后即为创建完成。
![创建完成](/images/CreateFinish.png)
5. 启用本地依赖输出，双击解决方案下的项目，在项目中添加`<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>`
![启用本地依赖输出](/images/CopyLocalAssemblies.png)


## Nuget 安装
1. 打开`工具 - Nuget 包管理器 - 管理解决方案的 Nuget 程序包`
![打开Nuget管理器](/images/OpenNugetManager.png)
2. 搜索`Another-Mirai-Native.Abstractions`，并安装到项目
![安装Nuget](/images/InstallNuget.png)

## 编写插件
### 继承插件基类
1. 添加`using Another_Mirai_Native.Abstractions;` `using Another_Mirai_Native.Abstractions.Models;`
2. 在类后添加插件基类继承`PluginBase`

### 声明插件元数据
添加插件描述特性`[PluginInfo]`，填入三个必须参数，此后还有两个可选参数
>  
    1. AppId: 插件应用程序的唯一标识符。该值用于将插件与其他插件区分开。通常建议使用 反向域名 + 插件用途，例如：com.demo.dice
    2. 插件名称，用于在插件管理器上显示的名称
    3. 插件版本，无格式要求
    4. 描述（可选）：用于在插件管理器上显示对于插件用途的描述
    5. 作者（可选）：用于在插件管理器上显示插件作者
```csharp
using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Models;

namespace DemoPlugin
{
    [PluginInfo("com.demo.dice", "R!", "1.0.0")]
    public class Entry : PluginBase
    {

    }
}

```
到此插件可以被框架加载，但是还没有处理逻辑，我们在下面的步骤继续完成。

### 添加消息处理逻辑
实现`IGroupMessageHandler`接口以处理群消息。我们通过接收一个`r`的消息指令，返回一个`[1-6]`的随机数来实现骰子功能。
```csharp
public class Entry : PluginBase, IGroupMessageHandler
{
    public async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
    {
        if (e.Message.Text.ToLower() == "r")
        {
            var random = new Random();
            var dice = random.Next(1, 7);
            e.FromGroup.SendGroupMessage($"你掷出了 {dice} 点！");
            return EventHandleResult.Block;
        }
        return EventHandleResult.Pass;
    }
}
```

### 加载插件并测试功能
1. 按`Ctrl + B`生成项目，或者手动在解决方案的项目处，点击右键，随后点击`生成`。右键项目，选择`在文件资源管理器中打开`
![打开生成目录](/images/OpenOutputDir.png)
2. 复制其中`Native_`开头的 dll 与 json 文件，至框架的`data\plugins`文件夹
![复制dll](/images/CopyPlugin.png)
3. 重启框架或者点击框架的重载插件
![重载插件](/images/ReloadPlugins.png)

4. 查看框架日志是否显示了我们的插件
![检查插件加载](/images/CheckLoadOK.png)
5. 在协议测试框或 Bot 所在群内发送消息`r`
![发送r](/images/SendR.png)
6. 查看是否插件发送了骰子的结果
![检查插件结果](/images/CheckDice.png)

这样我们就实现了一个骰子功能！

### 代码做了什么
#### 插件加载
1. 框架通过反射加载程序集，寻找继承了`PluginBase`插件基类的类，并实例化
2. 优先查看此类是否使用了`[PluginInfo]`特性，如果没有此特性则查看是否重写了基类的`PluginInfo`，以加载插件元数据
3. 调用插件的`OnEnableAsync`事件

#### 消息处理
1. 我们实现了`IGroupMessageHandler`接口，这样框架在收到群消息时会按事件的优先级依次调用插件的群消息处理事件。
2. 我们事件的参数`e`，表示了此事件所携带的上下文，其中有一个属性`Message`表示群消息内容。
3. 当群消息内容**转换为小写**为`r`时，会通过我们的if块判断，此时我们通过`new Random()`生成一个随机数对象，随后生成一个`[1-6]`的数字
4. 我们的事件参数`e`中包含消息来源的群，就是`e.FromGroup`对象，我们的 SDK 将对群的操作封装进了上下文对象，以便省去输入群号的步骤，所以我们输入`e.FromGroup.SendGroupMessage()`就可以向来源群发送消息

#### EventHandleResult.Block 是什么
我们的返回值`EventHandleResult.Block`是指我们插件对这条消息的处理结果，`Block`表示阻塞，意为此消息不会再向后续的插件投递；而`Pass`表示通过，即后续插件仍旧可以处理此消息。
