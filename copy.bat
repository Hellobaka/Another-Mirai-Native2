@echo off
setlocal

set ROOT=.
set TARGET=%ROOT%\UI_WPF\bin\x86\Debug\net48\protocols
set TARGET_Console=%ROOT%\Another-Mirai-Native\bin\x86\Debug\net48\protocols
set TARGET2=%ROOT%\UI_WPF\bin\x86\Debug\net48

:: 创建目标文件夹
if not exist "%TARGET%" (
    mkdir "%TARGET%"
)

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

:: 复制文件到第二个目标文件夹
set FILE=%ROOT%\CQP\bin\x86\Debug\CQP.dll
if exist "%FILE%" (
    copy /Y "%FILE%" "%TARGET2%"
) else (
    echo Warning: File "%FILE%" not found.
)

endlocal
