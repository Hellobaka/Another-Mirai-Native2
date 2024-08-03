@echo off
setlocal

set ROOT=.

echo Net48...
mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders"
mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48"
copy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\Another-Mirai-Native.exe" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48" /Y
copy "%ROOT%\CQP\bin\x86\Debug\CQP.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48" /Y

mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols"
copy "%ROOT%\Protocol_OneBot\bin\x86\Debug\net48\Protocol_OneBotv11.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols" /Y
copy "%ROOT%\Protocol_MiraiAPIHttp\bin\x86\Debug\net48\MiraiAPIHttp.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols" /Y
copy "%ROOT%\Protocol_NoConnection\bin\x86\Debug\net48\Protocol_NoConnection.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols" /Y
copy "%ROOT%\Protocol_Satori_v1\bin\x86\Debug\net48\Protocol_Satori_v1.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols" /Y

mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders"
mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders\NetFramework48"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48" "%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders\NetFramework48" /E /I /H /Y

mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net48\protocols"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols" "%ROOT%\UI_WPF\bin\x86\Debug\net48\protocols" /E /I /H /Y

copy "CleanI18nFolders.exe" "%ROOT%\UI_WPF\bin\x86\Debug\net48"
%ROOT%\UI_WPF\bin\x86\Debug\net48\CleanI18nFolders.exe
echo Delete CleanI18nFolders.exe
del %ROOT%\UI_WPF\bin\x86\Debug\net48\CleanI18nFolders.exe

echo Net8...
mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\loaders"
mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\loaders\NetFramework48"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\loaders\NetFramework48" /E /I /H /Y

mkdir "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\protocols"
copy "%ROOT%\Protocol_OneBot\bin\x86\Debug\net8.0-windows\Protocol_OneBotv11.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\protocols" /Y
copy "%ROOT%\Protocol_MiraiAPIHttp\bin\x86\Debug\net8.0-windows\MiraiAPIHttp.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\protocols" /Y
copy "%ROOT%\Protocol_NoConnection\bin\x86\Debug\net8.0-windows\Protocol_NoConnection.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\protocols" /Y
copy "%ROOT%\Protocol_Satori_v1\bin\x86\Debug\net8.0-windows\Protocol_Satori_v1.dll" "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\protocols" /Y

mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders"
mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders\NetFramework48"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\loaders\NetFramework48" "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders\NetFramework48" /E /I /H /Y

mkdir "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\protocols"
xcopy "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\protocols" "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\protocols" /E /I /H /Y

copy "CleanI18nFolders.exe" "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows"
%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\CleanI18nFolders.exe
echo Delete CleanI18nFolders.exe
del %ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\CleanI18nFolders.exe


endlocal
