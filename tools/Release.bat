@echo off
setlocal
pushd "%~dp0.." || exit /b 1

echo Release...
dotnet publish Another-Mirai-Native\Another-Mirai-Native.csproj /p:PublishProfile=net10.pubxml -f net10.0-windows
dotnet publish Another-Mirai-Native\Another-Mirai-Native.csproj /p:PublishProfile=net48.pubxml -f net48
dotnet publish UI_WPF\UI_WPF.csproj /p:PublishProfile=net48.pubxml -f net48
dotnet publish UI_WPF\UI_WPF.csproj /p:PublishProfile=net10.pubxml -f net10.0-windows

echo Loaders...
mkdir ".\build\loaders\NetFramework48"
mkdir ".\build\loaders\NetCore"
xcopy ".\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48\x86" ".\build\loaders\NetFramework48\x86" /E /I /H /Y
copy ".\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48\Another-Mirai-Native.exe" ".\build\loaders\NetFramework48" /Y
copy ".\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48\Another-Mirai-Native.exe.config" ".\build\loaders\NetFramework48" /Y
copy ".\Another-Mirai-Native\bin\x86\Debug\net48\loaders\NetFramework48\CQP.dll" ".\build\loaders\NetFramework48" /Y
copy ".\build\Console\net10\Another-Mirai-Native.exe" ".\build\loaders\NetCore\Another-Mirai-Native.exe" /Y

echo Generate Minimal Console(.net48)
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net48\protocols" ".\build\Console\net48\protocols" /E /I /H /Y
if exist ".\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" xcopy ".\build\loaders\Cpp" ".\build\Console\net48\loaders\Cpp" /E /I /H /Y
echo Copy SQLite.Interop.dll
xcopy ".\UI_WPF\bin\x86\Debug\net48\x86" ".\build\Console\net48\x86" /E /I /H /Y
echo Copy CQP.dll
copy /Y ".\Natives\CQP\bin\x86\Debug\CQP.dll" ".\build\Console\net48"
echo Clean Unnecessary Files
del /Q ".\build\Console\net48\*.pdb"
del /Q ".\build\Console\net48\*.xml"
for %%f in (".\build\Console\net48\*.dll") do (
    if /I not "%%~nxf"=="CQP.dll" (
        del /Q "%%f"
    )
)

echo Generate Console(net10)
echo Copy Loaders
xcopy ".\build\loaders\NetFramework48" ".\build\Console\net10\loaders\NetFramework48" /E /I /H /Y
if exist ".\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" xcopy ".\build\loaders\Cpp" ".\build\Console\net10\loaders\Cpp" /E /I /H /Y
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net10.0-windows\protocols" ".\build\Console\net10\protocols" /E /I /H /Y
echo Clean Unnecessary Files
del /Q ".\build\Console\net10\*.pdb"
del /Q ".\build\Console\net10\*.xml"

echo Generate WPF(.net48)
echo Copy Loaders
xcopy ".\build\loaders\NetCore" ".\build\WPF\net48\loaders\NetCore" /E /I /H /Y
if exist ".\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" xcopy ".\build\loaders\Cpp" ".\build\WPF\net48\loaders\Cpp" /E /I /H /Y
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net48\protocols" ".\build\WPF\net48\protocols" /E /I /H /Y
echo Copy SQLite.Interop.dll
xcopy ".\UI_WPF\bin\x86\Debug\net48\x86" ".\build\WPF\net48\x86" /E /I /H /Y
echo Copy CQP.dll
copy /Y ".\Natives\CQP\bin\x86\Debug\CQP.dll" ".\build\WPF\net48"
echo Clean Unnecessary Files
del /Q ".\build\WPF\net48\*.pdb"
del /Q ".\build\WPF\net48\*.xml"
for %%f in (".\build\WPF\net48\*.dll") do (
    if /I not "%%~nxf"=="CQP.dll" (
        del /Q "%%f"
    )
)

echo Generate WPF(.net10)
echo Copy Loaders
xcopy ".\build\loaders\NetFramework48" ".\build\WPF\net10\loaders\NetFramework48" /E /I /H /Y
if exist ".\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" xcopy ".\build\loaders\Cpp" ".\build\WPF\net10\loaders\Cpp" /E /I /H /Y
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net10.0-windows\protocols" ".\build\WPF\net10\protocols" /E /I /H /Y
echo Clean Unnecessary Files
rd /s /q ".\build\WPF\net10\conf"
del /Q ".\build\WPF\net10\*.pdb"
del /Q ".\build\WPF\net10\*.xml"
del /Q ".\build\WPF\net10\Another-Mirai-Native.exe"
del /Q ".\build\WPF\net10\Another-Mirai-Native.runtimeconfig.json"
del /Q ".\build\WPF\net10\appsettings.Development.json"
del /Q ".\build\WPF\net10\appsettings.json"
del /Q ".\build\WPF\net10\Another-Mirai-Native.WebAPI.exe"
del /Q ".\build\WPF\net10\Another-Mirai-Native.WebAPI.runtimeconfig.json"

echo Create zip Archives
where 7z.exe >nul 2>&1
if %errorlevel% neq 0 (
    echo Cannot Find 7z.exe
    popd
    endlocal
    exit /b
)
7z.exe a -tzip ".\build\Minimal_Console.zip" ".\build\Console\net48\*"
7z.exe a -tzip ".\build\Console_net10.zip" ".\build\Console\net10\*"
7z.exe a -tzip ".\build\WPF_net10.zip" ".\build\WPF\net10\*"
7z.exe a -tzip ".\build\WPF_net48.zip" ".\build\WPF\net48\*"

popd
endlocal
