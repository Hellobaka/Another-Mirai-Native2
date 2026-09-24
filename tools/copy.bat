@echo off
setlocal

set "ROOT=%~dp0.."

echo Net48...
mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders"
mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetCore"
copy "%ROOT%\build\Console\net10\Another-Mirai-Native.exe" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetCore\Another-Mirai-Native.exe" /Y
if exist "%ROOT%\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" xcopy "%ROOT%\build\loaders\Cpp" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders\Cpp" /E /I /H /Y

mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols"
copy "%ROOT%\Protocols\Protocol_OneBot\bin\x86\Debug\net48\Protocol_OneBotv11.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols" /Y
copy "%ROOT%\Protocols\Protocol_MiraiAPIHttp\bin\x86\Debug\net48\MiraiAPIHttp.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols" /Y
copy "%ROOT%\Protocols\Protocol_NoConnection\bin\x86\Debug\net48\Protocol_NoConnection.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols" /Y

mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders"
mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders\NetCore"
if exist "%ROOT%\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" xcopy "%ROOT%\build\loaders\Cpp" "%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders\Cpp" /E /I /H /Y

mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net48\protocols"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols" "%ROOT%\UI_WPF\bin\x86\Debug\net48\protocols" /E /I /H /Y

copy "%~dp0CleanI18nFolders.exe" "%ROOT%\UI_WPF\bin\x86\Debug\net48"
"%ROOT%\UI_WPF\bin\x86\Debug\net48\CleanI18nFolders.exe"
echo Delete CleanI18nFolders.exe
del "%ROOT%\UI_WPF\bin\x86\Debug\net48\CleanI18nFolders.exe"

echo Net10...
mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\loaders"
mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\loaders\NetFramework48"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\loaders\NetFramework48" /E /I /H /Y
if exist "%ROOT%\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" xcopy "%ROOT%\build\loaders\Cpp" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\loaders\Cpp" /E /I /H /Y

mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols"
copy "%ROOT%\Protocols\Protocol_OneBot\bin\x86\Debug\net10.0-windows\Protocol_OneBotv11.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols" /Y
copy "%ROOT%\Protocols\Protocol_MiraiAPIHttp\bin\x86\Debug\net10.0-windows\MiraiAPIHttp.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols" /Y
copy "%ROOT%\Protocols\Protocol_NoConnection\bin\x86\Debug\net10.0-windows\Protocol_NoConnection.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols" /Y
copy "%ROOT%\Protocols\Protocol_LagrangeCore\bin\x86\Debug\net10.0-windows\Protocol_LagrangeCore.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols" /Y
copy "%ROOT%\Protocols\Protocol_LagrangeCore\bin\x86\Debug\net10.0-windows\Lagrange.Core.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols" /Y
copy "%ROOT%\Protocols\Protocol_LagrangeCore\Lagrange.Core\Lagrange.OneBot\bin\Debug\net9.0\protobuf-net.Core.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols" /Y
copy "%ROOT%\Protocols\Protocol_LagrangeCore\Lagrange.Core\Lagrange.OneBot\bin\Debug\net9.0\protobuf-net.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols" /Y

mkdir "%ROOT%\Another-Mirai-Native.WebAPI\bin\x86\Debug\net10.0-windows\loaders"
mkdir "%ROOT%\Another-Mirai-Native.WebAPI\bin\x86\Debug\net10.0-windows\loaders\NetFramework48"
xcopy "%ROOT%\Another-Mirai-Native.WebAPI\bin\x86\Debug\net48\loaders\NetFramework48" "%ROOT%\Another-Mirai-Native.WebAPI\bin\x86\Debug\net10.0-windows\loaders\NetFramework48" /E /I /H /Y
if exist "%ROOT%\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" xcopy "%ROOT%\build\loaders\Cpp" "%ROOT%\Another-Mirai-Native.WebAPI\bin\x86\Debug\net10.0-windows\loaders\Cpp" /E /I /H /Y

mkdir "%ROOT%\Another-Mirai-Native.WebAPI\bin\x86\Debug\net10.0-windows\protocols"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols" "%ROOT%\Another-Mirai-Native.WebAPI\bin\x86\Debug\net10.0-windows\protocols" /E /I /H /Y

mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net10.0-windows\loaders"
mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net10.0-windows\loaders\NetFramework48"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\loaders\NetFramework48" "%ROOT%\UI_WPF\bin\x86\Debug\net10.0-windows\loaders\NetFramework48" /E /I /H /Y
if exist "%ROOT%\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" xcopy "%ROOT%\build\loaders\Cpp" "%ROOT%\UI_WPF\bin\x86\Debug\net10.0-windows\loaders\Cpp" /E /I /H /Y

mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net10.0-windows\protocols"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows\protocols" "%ROOT%\UI_WPF\bin\x86\Debug\net10.0-windows\protocols" /E /I /H /Y

copy "%~dp0CleanI18nFolders.exe" "%ROOT%\UI_WPF\bin\x86\Debug\net10.0-windows"
"%ROOT%\UI_WPF\bin\x86\Debug\net10.0-windows\CleanI18nFolders.exe"
echo Delete CleanI18nFolders.exe
del "%ROOT%\UI_WPF\bin\x86\Debug\net10.0-windows\CleanI18nFolders.exe"

"%~dp0Another-Mirai-Native2-DependencyManifest.exe" -i "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net10.0-windows" -o "%ROOT%\Another-Mirai-Native.Abstractions\tools\DependencyManifest-dotnet10.json"

endlocal
