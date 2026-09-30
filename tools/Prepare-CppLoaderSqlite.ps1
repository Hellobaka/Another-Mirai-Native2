<#
.SYNOPSIS
Copies the .NET Framework SQLite dependencies and CLR binding configuration
beside the C++ loader. Called by the loader's MSBuild project.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$OutputDirectory,
    [ValidateSet('Win32', 'x64')]
    [string]$Platform = 'Win32',
    [string]$RepositoryRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($RepositoryRoot)) {
    $RepositoryRoot = Split-Path -Parent $PSScriptRoot
}
$coreProject = Join-Path $RepositoryRoot 'Another-Mirai-Native/Another-Mirai-Native.csproj'
$assetsPath = Join-Path $RepositoryRoot 'Another-Mirai-Native/obj/project.assets.json'

function Find-SqlitePackage {
    if (-not (Test-Path -LiteralPath $assetsPath)) { return $null }
    $assets = Get-Content -LiteralPath $assetsPath -Raw | ConvertFrom-Json
    $target = $assets.targets.PSObject.Properties | Where-Object { $_.Name -eq 'net48' } | Select-Object -First 1
    if ($null -eq $target) { return $null }
    $package = $target.Value.PSObject.Properties |
        Where-Object { $_.Name -like 'Stub.System.Data.SQLite.Core.NetFramework/*' } | Select-Object -First 1
    if ($null -eq $package) { return $null }
    return [pscustomobject]@{ Assets = $assets; Package = $package }
}

$resolved = Find-SqlitePackage
if ($null -eq $resolved) {
    & dotnet restore $coreProject '-p:TargetFramework=net48'
    if ($LASTEXITCODE -ne 0) { throw 'Could not restore the .NET Framework SQLite dependency.' }
    $resolved = Find-SqlitePackage
}
if ($null -eq $resolved) { throw 'The core net48 restore did not resolve System.Data.SQLite.' }

$library = $resolved.Assets.libraries.PSObject.Properties[$resolved.Package.Name].Value
$managedAsset = $resolved.Package.Value.runtime.PSObject.Properties |
    Where-Object { $_.Name -like 'lib/*/System.Data.SQLite.dll' } | Select-Object -First 1
if ($null -eq $managedAsset) { throw 'No .NET Framework System.Data.SQLite.dll runtime asset was found.' }
$framework = ($managedAsset.Name -split '/')[1]
$architecture = if ($Platform -eq 'x64') { 'x64' } else { 'x86' }
$managedPath = $null
$interopPath = $null
foreach ($folder in $resolved.Assets.packageFolders.PSObject.Properties) {
    $packageRoot = Join-Path $folder.Name $library.path
    $managedCandidate = Join-Path $packageRoot $managedAsset.Name
    $interopCandidate = Join-Path $packageRoot "build/$framework/$architecture/SQLite.Interop.dll"
    if ((Test-Path -LiteralPath $managedCandidate) -and (Test-Path -LiteralPath $interopCandidate)) {
        $managedPath = $managedCandidate
        $interopPath = $interopCandidate
        break
    }
}
if ($null -eq $managedPath) { throw 'The matching managed and native SQLite package files were not found.' }

# Read the assembly identity, rather than assuming the NuGet or file version.
$identity = [Reflection.AssemblyName]::GetAssemblyName($managedPath)
$version = $identity.Version.ToString()
$token = ($identity.GetPublicKeyToken() | ForEach-Object { $_.ToString('x2') }) -join ''
$config = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <startup useLegacyV2RuntimeActivationPolicy="true">
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" />
  </startup>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="System.Data.SQLite" publicKeyToken="$token" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-$version" newVersion="$version" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
"@

$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
$nativeDirectory = Join-Path $OutputDirectory $architecture
New-Item -ItemType Directory -Path $nativeDirectory -Force | Out-Null
Copy-Item -LiteralPath $managedPath -Destination (Join-Path $OutputDirectory 'System.Data.SQLite.dll') -Force
Copy-Item -LiteralPath $interopPath -Destination (Join-Path $nativeDirectory 'SQLite.Interop.dll') -Force
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'Another-Mirai-Native.Loader.Cpp.exe.config'), $config, [Text.UTF8Encoding]::new($false))
Write-Host "Prepared C++ loader SQLite $version ($architecture)."
