@echo off
setlocal
set ROOT=.

echo Generate Minimal Console(.net48)
echo Copy Loaders
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders" "%ROOT%\build\Console\net48\loaders" /E /I /H /Y
echo Copy Protocols
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net48\protocols" "%ROOT%\build\Console\net48\protocols" /E /I /H /Y
echo Copy SQLite.Interop.dll
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net48\x86" "%ROOT%\build\Console\net48\x86" /E /I /H /Y
echo Copy CQP.dll
copy /Y "%ROOT%\CQP\bin\x86\Debug\CQP.dll" "%ROOT%\build\Console\net48"
echo Clean Unnecessary Files
del /S /Q "%ROOT%\build\Console\net48\*.pdb"
del /S /Q "%ROOT%\build\Console\net48\*.config"
for %%f in ("%ROOT%\build\Console\net48\*.dll") do (
    if /I not "%%~nxf"=="CQP.dll" (
        del /S /Q "%%f"
    )
)

echo Generate WebUI(.net8)
echo Copy Loaders
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders" "%ROOT%\build\Web\loaders" /E /I /H /Y
echo Copy Protocols
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\protocols" "%ROOT%\build\Web\protocols" /E /I /H /Y
echo Clean Unnecessary Files
del /S /Q "%ROOT%\build\Web\*.pdb"
del /S /Q "%ROOT%\build\Web\*.config"
del /S /Q "%ROOT%\build\Web\Another-Mirai-Native.exe"
del /S /Q "%ROOT%\build\Web\Another-Mirai-Native.runtimeconfig.json"

echo Generate WPF(.net48)
echo Copy Loaders
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net48\loaders" "%ROOT%\build\WPF\net48\loaders" /E /I /H /Y
echo Copy Protocols
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net48\protocols" "%ROOT%\build\WPF\net48\protocols" /E /I /H /Y
echo Copy SQLite.Interop.dll
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net48\x86" "%ROOT%\build\WPF\net48\x86" /E /I /H /Y
echo Copy CQP.dll
copy /Y "%ROOT%\CQP\bin\x86\Debug\CQP.dll" "%ROOT%\build\WPF\net48"
echo Clean Unnecessary Files
del /S /Q "%ROOT%\build\WPF\net48\*.pdb"
del /S /Q "%ROOT%\build\WPF\net48\*.config"
for %%f in ("%ROOT%\build\WPF\net48\*.dll") do (
    if /I not "%%~nxf"=="CQP.dll" (
        del /S /Q "%%f"
    )
)

echo Generate WPF(.net8)
echo Copy Loaders
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\loaders" "%ROOT%\build\WPF\net8\loaders" /E /I /H /Y
echo Copy Protocols
xcopy "%ROOT%\UI_WPF\bin\x86\Debug\net8.0-windows\protocols" "%ROOT%\build\WPF\net8\protocols" /E /I /H /Y
echo Copy wwwroot
xcopy "%ROOT%\build\Web\wwwroot" "%ROOT%\build\WPF\net8\wwwroot" /E /I /H /Y
echo Clean Unnecessary Files
del /S /Q "%ROOT%\build\WPF\net8\*.pdb"
del /S /Q "%ROOT%\build\WPF\net8\*.config"
del /S /Q "%ROOT%\build\WPF\net8\Another-Mirai-Native.exe"
del /S /Q "%ROOT%\build\WPF\net8\Another-Mirai-Native.runtimeconfig.json"
del /S /Q "%ROOT%\build\WPF\net8\Another-Mirai-Native-WebUI.exe"
del /S /Q "%ROOT%\build\WPF\net8\Another-Mirai-Native-WebUI.runtimeconfig.json"

echo Create zip Archives
where 7z.exe >nul 2>&1
if %errorlevel% neq 0 (
    echo Cannot Find 7z.exe
    endlocal
    exit /b
)
7z.exe a -tzip "%ROOT%\build\Minimal_Console.zip" "%ROOT%\build\Console\net48\*"
7z.exe a -tzip "%ROOT%\build\WebUI.zip" "%ROOT%\build\Web\*"
7z.exe a -tzip "%ROOT%\build\WPF_net8.zip" "%ROOT%\build\WPF\net8\*"
7z.exe a -tzip "%ROOT%\build\WPF_net48.zip" "%ROOT%\build\WPF\net48\*"

endlocal
