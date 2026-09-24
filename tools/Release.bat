@echo off
setlocal
pushd "%~dp0.." || exit /b 1

echo Build C++ Loader...
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
set "MSBUILD_EXE="
if not exist "%VSWHERE%" (
    echo Cannot find vswhere.exe
    goto :release_failed
)
set "MSBUILD_LIST=%TEMP%\amn2-msbuild-%RANDOM%-%RANDOM%.txt"
"%VSWHERE%" -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -find "MSBuild\Current\Bin\MSBuild.exe" > "%MSBUILD_LIST%"
if errorlevel 1 (
    del /q "%MSBUILD_LIST%"
    goto :release_failed
)
for /f "usebackq delims=" %%I in ("%MSBUILD_LIST%") do if not defined MSBUILD_EXE set "MSBUILD_EXE=%%I"
del /q "%MSBUILD_LIST%"
if not defined MSBUILD_EXE (
    echo Cannot find Visual Studio MSBuild with C++ tools
    goto :release_failed
)
"%MSBUILD_EXE%" "Natives\Another-Mirai-Native.Loader.Cpp\Another-Mirai-Native.Loader.Cpp.sln" /m /p:Configuration=Release /p:Platform=Win32
if errorlevel 1 goto :release_failed
if not exist ".\build\loaders\Cpp\Another-Mirai-Native.Loader.Cpp.exe" goto :release_failed
if not exist ".\build\loaders\Cpp\CQP.dll" goto :release_failed

echo Release...
dotnet publish Another-Mirai-Native\Another-Mirai-Native.csproj /p:PublishProfile=net10.pubxml -f net10.0-windows
if errorlevel 1 goto :release_failed
dotnet publish Another-Mirai-Native\Another-Mirai-Native.csproj /p:PublishProfile=net48.pubxml -f net48
if errorlevel 1 goto :release_failed
dotnet publish UI_WPF\UI_WPF.csproj /p:PublishProfile=net48.pubxml -f net48
if errorlevel 1 goto :release_failed
dotnet publish UI_WPF\UI_WPF.csproj /p:PublishProfile=net10.pubxml -f net10.0-windows
if errorlevel 1 goto :release_failed

rem Previous publishes may have left the old loader in package directories.
for %%D in (
    ".\build\loaders\NetFramework48"
    ".\build\Console\net48\loaders\NetFramework48"
    ".\build\Console\net10\loaders\NetFramework48"
    ".\build\WPF\net48\loaders\NetFramework48"
    ".\build\WPF\net10\loaders\NetFramework48"
) do (
    if exist "%%~D" rd /s /q "%%~D"
    if exist "%%~D" goto :release_failed
)

echo Loaders...
mkdir ".\build\loaders\NetCore"
copy ".\build\Console\net10\Another-Mirai-Native.exe" ".\build\loaders\NetCore\Another-Mirai-Native.exe" /Y

echo Generate Minimal Console(.net48)
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net48\protocols" ".\build\Console\net48\protocols" /E /I /H /Y
xcopy ".\build\loaders\Cpp" ".\build\Console\net48\loaders\Cpp" /E /I /H /Y
if errorlevel 1 goto :release_failed
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
xcopy ".\build\loaders\Cpp" ".\build\Console\net10\loaders\Cpp" /E /I /H /Y
if errorlevel 1 goto :release_failed
echo Copy Protocols
xcopy ".\UI_WPF\bin\x86\Debug\net10.0-windows\protocols" ".\build\Console\net10\protocols" /E /I /H /Y
echo Clean Unnecessary Files
del /Q ".\build\Console\net10\*.pdb"
del /Q ".\build\Console\net10\*.xml"

echo Generate WPF(.net48)
echo Copy Loaders
xcopy ".\build\loaders\NetCore" ".\build\WPF\net48\loaders\NetCore" /E /I /H /Y
xcopy ".\build\loaders\Cpp" ".\build\WPF\net48\loaders\Cpp" /E /I /H /Y
if errorlevel 1 goto :release_failed
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
xcopy ".\build\loaders\Cpp" ".\build\WPF\net10\loaders\Cpp" /E /I /H /Y
if errorlevel 1 goto :release_failed
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
    goto :release_failed
)
7z.exe a -tzip ".\build\Minimal_Console.zip" ".\build\Console\net48\*"
7z.exe a -tzip ".\build\Console_net10.zip" ".\build\Console\net10\*"
7z.exe a -tzip ".\build\WPF_net10.zip" ".\build\WPF\net10\*"
7z.exe a -tzip ".\build\WPF_net48.zip" ".\build\WPF\net48\*"

popd
endlocal
exit /b 0

:release_failed
echo Release failed.
popd
endlocal
exit /b 1
