@echo off
setlocal

echo Net48...
set ROOT=.
set TARGET=%ROOT%\UI_WPF\bin\x86\Debug\net48\protocols
set TARGET_Console=%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols
set TARGET2=%ROOT%\UI_WPF\bin\x86\Debug\net48

:: 创建目标文件夹
if not exist "%TARGET%" (
    mkdir "%TARGET%"
)
if not exist "%TARGET_Console%" (
    mkdir "%TARGET_Console%"
)

echo Copy Net48 Protocol...

:: 复制文件
for %%F in (
    "%ROOT%\Protocol_OneBot\bin\x86\Debug\net48\Protocol_OneBotv11.dll"
    "%ROOT%\Protocol_MiraiAPIHttp\bin\x86\Debug\net48\MiraiAPIHttp.dll"
    "%ROOT%\Protocol_NoConnection\bin\x86\Debug\net48\Protocol_NoConnection.dll"
    "%ROOT%\Protocol_Satori_v1\bin\x86\Debug\net48\Protocol_Satori_v1.dll"
) do (
    if exist "%%F" (
        copy /Y "%%F" "%TARGET%"
        copy /Y "%%F" "%TARGET_Console%"
    ) else (
        echo Warning: File "%%F" not found.
    )
)

echo Copy Net48 Loader...

set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders
if not exist "%DIR%" (
    mkdir "%DIR%"
)
set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders\Net8
if not exist "%DIR%" (
    mkdir "%DIR%"
)
set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders\NetFramework48
if not exist "%DIR%" (
    mkdir "%DIR%"
)

set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders\NetFramework48
set FILE=%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\Another-Mirai-Native.exe
if exist "%FILE%" (
    copy /Y "%FILE%" "%DIR%"
) else (
    echo Warning: File "%FILE%" not found.
)
set FILE=%ROOT%\CQP\bin\x86\Debug\CQP.dll
if exist "%FILE%" (
    copy /Y "%FILE%" "%DIR%"
) else (
    echo Warning: File "%FILE%" not found.
)

set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders\Net8
for %%F in (
    "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\Another-Mirai-Native.deps.json"
    "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\Another-Mirai-Native.dll"
    "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\Another-Mirai-Native.exe"
    "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\Another-Mirai-Native.runtimeconfig.json"
) do (
    if exist "%%F" (
        copy /Y "%%F" "%DIR%"
    ) else (
        echo Warning: File "%%F" not found.
    )
)

set FILE="CleanI18nFolders.exe"
set DIR="%ROOT%\UI_WPF\bin\x86\Debug\net48"
copy "%FILE%" "%DIR%"
%DIR%\%FILE%
echo Delete CleanI18nFolders.exe
del %DIR%\%FILE%

set FILE=%ROOT%\CQP\bin\x86\Debug\CQP.dll
if exist "%FILE%" (
    copy /Y "%FILE%" "%TARGET2%"
) else (
    echo Warning: File "%FILE%" not found.
)
echo net8...

set TARGET=%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\protocols
set TARGET_Console=%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\protocols
set TARGET2=%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows

:: 创建目标文件夹
if not exist "%TARGET%" (
    mkdir "%TARGET%"
)
if not exist "%TARGET_Console%" (
    mkdir "%TARGET_Console%"
)

echo Copy Net8 Protocol...

:: 复制文件
for %%F in (
    "%ROOT%\Protocol_OneBot\bin\x86\Debug\net8\Protocol_OneBotv11.dll"
    "%ROOT%\Protocol_MiraiAPIHttp\bin\x86\Debug\net8\MiraiAPIHttp.dll"
    "%ROOT%\Protocol_NoConnection\bin\x86\Debug\net8.0-windows\Protocol_NoConnection.dll"
    "%ROOT%\Protocol_Satori_v1\bin\x86\Debug\net8\Protocol_Satori_v1.dll"
) do (
    if exist "%%F" (
        copy /Y "%%F" "%TARGET%"
        copy /Y "%%F" "%TARGET_Console%"
    ) else (
        echo Warning: File "%%F" not found.
    )
)

echo Copy Net8 Loader...

set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders
if not exist "%DIR%" (
    mkdir "%DIR%"
)
set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders\Net8
if not exist "%DIR%" (
    mkdir "%DIR%"
)
set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders\NetFramework48
if not exist "%DIR%" (
    mkdir "%DIR%"
)

set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders\NetFramework48
set FILE=%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\Another-Mirai-Native.exe
if exist "%FILE%" (
    copy /Y "%FILE%" "%DIR%"
) else (
    echo Warning: File "%FILE%" not found.
)
set FILE=%ROOT%\CQP\bin\x86\Debug\CQP.dll
if exist "%FILE%" (
    copy /Y "%FILE%" "%DIR%"
) else (
    echo Warning: File "%FILE%" not found.
)

set DIR=%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders\Net8
for %%F in (
    "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\Another-Mirai-Native.deps.json"
    "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\Another-Mirai-Native.dll"
    "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\Another-Mirai-Native.exe"
    "%ROOT%\Another-Mirai-Native\bin\x86\Debug\net8.0-windows\Another-Mirai-Native.runtimeconfig.json"
    "%ROOT%\CQP_DNNE\bin\x86\Debug\net8.0\win-x86\CQP.dll"
    "%ROOT%\CQP_DNNE\bin\x86\Debug\net8.0\win-x86\CQP_DNNE.dll"
    "%ROOT%\CQP_DNNE\bin\x86\Debug\net8.0\win-x86\CQP_DNNE.deps.json"
    "%ROOT%\CQP_DNNE\bin\x86\Debug\net8.0\win-x86\CQP_DNNE.runtimeconfig.json"
) do (
    if exist "%%F" (
        copy /Y "%%F" "%DIR%"
    ) else (
        echo Warning: File "%%F" not found.
    )
)

set FILE="CleanI18nFolders.exe"
set DIR="%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows"
copy "%FILE%" "%DIR%"
%DIR%\%FILE%
echo Delete CleanI18nFolders.exe
del %DIR%\%FILE%

set FILE=%ROOT%\CQP\bin\x86\Debug\CQP.dll
if exist "%FILE%" (
    copy /Y "%FILE%" "%TARGET2%"
) else (
    echo Warning: File "%FILE%" not found.
)

endlocal
