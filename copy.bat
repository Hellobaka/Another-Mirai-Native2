@echo off
setlocal

set ROOT=.
set TARGET=%ROOT%\UI_WPF\bin\x86\Debug\net481\protocols
set TARGET2=%ROOT%\UI_WPF\bin\x86\Debug\net481

:: 创建目标文件夹
if not exist "%TARGET%" (
    mkdir "%TARGET%"
)

:: 复制文件
for %%F in (
    "%ROOT%\Protocol_OneBot\bin\x86\Debug\net481\Protocol_OneBotv11.dll"
    "%ROOT%\Protocol_MiraiAPIHttp\bin\x86\Debug\net481\MiraiAPIHttp.dll"
    "%ROOT%\Protocol_NoConnection\bin\x86\Debug\net481\Protocol_NoConnection.dll"
) do (
    if exist "%%F" (
        copy /Y "%%F" "%TARGET%"
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
