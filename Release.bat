@echo off
setlocal

echo Release...
dotnet publish Another-Mirai-Native\Another-Mirai-Native.csproj /p:PublishProfile=net9.pubxml -f net9.0-windows
dotnet publish Another-Mirai-Native\Another-Mirai-Native.csproj /p:PublishProfile=net48.pubxml -f net48
dotnet publish UI_Blazor\UI_Blazor.csproj /p:PublishProfile=net9.pubxml -f net9.0-windows
dotnet publish UI_WPF\UI_WPF.csproj /p:PublishProfile=net48.pubxml -f net48
dotnet publish UI_WPF\UI_WPF.csproj /p:PublishProfile=net9.pubxml -f net9.0-windows

echo Loaders...
mkdir ".\build\loaders\NetFramework48"
xcopy ".\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48\x86" ".\build\loaders\NetFramework48\x86" /E /I /H /Y
copy ".\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48\Another-Mirai-Native.exe" ".\build\loaders\NetFramework48" /Y
copy ".\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48\Another-Mirai-Native.exe.config" ".\build\loaders\NetFramework48" /Y
copy ".\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48\CQP.dll" ".\build\loaders\NetFramework48" /Y

echo Generate Minimal Console(.net48)
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net48\protocols" ".\build\Console\net48\protocols" /E /I /H /Y
echo Copy SQLite.Interop.dll
xcopy ".\UI_WPF\bin\x86\Debug\net48\x86" ".\build\Console\net48\x86" /E /I /H /Y
echo Copy CQP.dll
copy /Y ".\CQP\bin\x86\Debug\CQP.dll" ".\build\Console\net48"
echo Clean Unnecessary Files
del /Q ".\build\Console\net48\*.pdb"
for %%f in (".\build\Console\net48\*.dll") do (
    if /I not "%%~nxf"=="CQP.dll" (
        del /Q "%%f"
    )
)

echo Generate Console(net9)
echo Copy Loaders
xcopy ".\build\loaders" ".\build\Console\net9\loaders" /E /I /H /Y
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net9.0-windows\protocols" ".\build\Console\net9\protocols" /E /I /H /Y
echo Clean Unnecessary Files
del /Q ".\build\Console\net9\*.pdb"

echo Generate WebUI(.net9)
echo Copy Loaders
xcopy ".\build\loaders" ".\build\Web\loaders" /E /I /H /Y
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net9.0-windows\protocols" ".\build\Web\protocols" /E /I /H /Y
echo Clean Unnecessary Files
del /Q ".\build\Web\*.pdb"
del /Q ".\build\Web\Another-Mirai-Native.exe"
del /Q ".\build\Web\Another-Mirai-Native.runtimeconfig.json"

echo Generate WPF(.net48)
echo Copy Loaders
xcopy ".\build\loaders" ".\build\WPF\net48\loaders" /E /I /H /Y
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net48\protocols" ".\build\WPF\net48\protocols" /E /I /H /Y
echo Copy SQLite.Interop.dll
xcopy ".\UI_WPF\bin\x86\Debug\net48\x86" ".\build\WPF\net48\x86" /E /I /H /Y
echo Copy CQP.dll
copy /Y ".\CQP\bin\x86\Debug\CQP.dll" ".\build\WPF\net48"
echo Clean Unnecessary Files
del /Q ".\build\WPF\net48\*.pdb"
for %%f in (".\build\WPF\net48\*.dll") do (
    if /I not "%%~nxf"=="CQP.dll" (
        del /Q "%%f"
    )
)

echo Generate WPF(.net9)
echo Copy Loaders
xcopy ".\build\loaders" ".\build\WPF\net9\loaders" /E /I /H /Y
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net9.0-windows\protocols" ".\build\WPF\net9\protocols" /E /I /H /Y
echo Copy wwwroot
xcopy ".\build\Web\wwwroot" ".\build\WPF\net9\wwwroot" /E /I /H /Y
echo Clean Unnecessary Files
del /Q ".\build\WPF\net9\*.pdb"
del /Q ".\build\WPF\net9\Another-Mirai-Native.exe"
del /Q ".\build\WPF\net9\Another-Mirai-Native.runtimeconfig.json"
del /Q ".\build\WPF\net9\Another-Mirai-Native-WebUI.exe"
del /Q ".\build\WPF\net9\Another-Mirai-Native-WebUI.runtimeconfig.json"

echo Create zip Archives
where 7z.exe >nul 2>&1
if %errorlevel% neq 0 (
    echo Cannot Find 7z.exe
    endlocal
    exit /b
)
7z.exe a -tzip ".\build\Minimal_Console.zip" ".\build\Console\net48\*"
7z.exe a -tzip ".\build\Console_net9.zip" ".\build\Console\net9\*"
7z.exe a -tzip ".\build\WebUI.zip" ".\build\Web\*"
7z.exe a -tzip ".\build\WPF_net9.zip" ".\build\WPF\net9\*"
7z.exe a -tzip ".\build\WPF_net48.zip" ".\build\WPF\net48\*"

endlocal