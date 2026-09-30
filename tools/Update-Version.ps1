<#
.SYNOPSIS
Updates application version metadata without changing dependency versions.
.DESCRIPTION
Updates the core, WPF, WebAPI, four protocol projects and CQP AssemblyInfo.cs.
Accepts major.minor.patch or major.minor.patch.revision; assembly versions use
four components. All target files are read and validated before writing.
.EXAMPLE
./tools/Update-Version.ps1 -Version 2.15.0 -WhatIf
.EXAMPLE
./tools/Update-Version.ps1 -Version 2.15.0
#>
[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Low')]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Version,
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-AssemblyVersion([string]$Value) {
    if ($Value -notmatch '^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(\.(0|[1-9]\d*))?$') {
        throw "Invalid version '$Value'. Expected major.minor.patch[.revision]."
    }
    $parts = $Value.Split('.')
    foreach ($part in $parts) {
        if ([long]$part -gt 65534) {
            throw "Invalid version '$Value': assembly version components must be between 0 and 65534."
        }
    }
    if ($parts.Count -eq 3) { return "$Value.0" }
    return $Value
}

$assemblyVersion = Get-AssemblyVersion $Version
$RepositoryRoot = (Resolve-Path -LiteralPath $RepositoryRoot).Path
$changes = [Collections.Generic.List[object]]::new()

function Read-Source([string]$RelativePath) {
    $path = Join-Path $RepositoryRoot $RelativePath
    $stream = [IO.File]::OpenRead($path)
    $reader = [IO.StreamReader]::new($stream, [Text.UTF8Encoding]::new($false, $true), $true)
    try {
        $content = $reader.ReadToEnd()
        $encoding = $reader.CurrentEncoding
    } finally {
        $reader.Dispose()
    }
    return [pscustomobject]@{ Path = $path; Content = $content; Encoding = $encoding }
}

function Add-Change($Source, [string]$Updated) {
    if ($Updated -cne $Source.Content) {
        $changes.Add([pscustomobject]@{ Path = $Source.Path; Content = $Updated; Encoding = $Source.Encoding })
    }
}

function Update-Project([string]$RelativePath, [string]$ProjectVersion, [string]$NumericVersion) {
    $source = Read-Source $RelativePath
    $document = [Xml.XmlDocument]::new()
    $document.XmlResolver = $null
    $document.LoadXml($source.Content)
    if ($document.DocumentElement.Name -ne 'Project') { throw "Not a project file: $RelativePath" }
    $hasVersion = $null -ne $document.SelectSingleNode('/Project/PropertyGroup/Version')
    $newline = if ($source.Content.Contains("`r`n")) { "`r`n" } else { "`n" }
    $state = @{ AddedVersion = $false }
    $values = @{
        Version = $ProjectVersion; PackageVersion = $ProjectVersion
        AssemblyVersion = $NumericVersion; FileVersion = $NumericVersion
        InformationalVersion = $ProjectVersion
    }
    $groupPattern = [regex]::new('(?s)(<PropertyGroup\b[^>]*>)(.*?)(</PropertyGroup>)')
    $propertyPattern = [regex]::new('(<(?<Name>Version|PackageVersion|AssemblyVersion|FileVersion|InformationalVersion)>)[^<]*(</\k<Name>>)')
    $updated = $groupPattern.Replace($source.Content, [Text.RegularExpressions.MatchEvaluator]{
        param($group)
        $body = $propertyPattern.Replace($group.Groups[2].Value, [Text.RegularExpressions.MatchEvaluator]{
            param($property)
            return $property.Groups[1].Value + $values[$property.Groups['Name'].Value] + $property.Groups[2].Value
        })
        if (-not $hasVersion -and -not $state.AddedVersion -and $group.Groups[1].Value -notmatch '\bCondition\s*=') {
            $indentMatch = [regex]::Match($body, '(?m)^([\t ]+)<')
            $indent = if ($indentMatch.Success) { $indentMatch.Groups[1].Value } else { '    ' }
            $body = $newline + $indent + '<Version>' + $ProjectVersion + '</Version>' + $body
            $state.AddedVersion = $true
        }
        return $group.Groups[1].Value + $body + $group.Groups[3].Value
    })
    if (-not $hasVersion -and -not $state.AddedVersion) { throw "No unconditional PropertyGroup found in $RelativePath" }
    $document.LoadXml($updated)
    Add-Change $source $updated
}

function Update-CqpAssemblyInfo {
    $source = Read-Source 'Natives/CQP/Properties/AssemblyInfo.cs'
    $updated = $source.Content
    foreach ($name in @('AssemblyVersion', 'AssemblyFileVersion', 'AssemblyInformationalVersion')) {
        $pattern = [regex]::new('(?m)(^[\t ]*\[assembly:\s*' + $name + '\s*\(\s*")[^"\r\n]*("\s*\)\s*\])')
        if ($name -ne 'AssemblyInformationalVersion' -and -not $pattern.IsMatch($updated)) {
            throw "Missing $name in $($source.Path)"
        }
        $value = if ($name -eq 'AssemblyInformationalVersion') { $Version } else { $assemblyVersion }
        $updated = $pattern.Replace($updated, [Text.RegularExpressions.MatchEvaluator]{
            param($attribute)
            return $attribute.Groups[1].Value + $value + $attribute.Groups[2].Value
        })
    }
    Add-Change $source $updated
}

$projects = @(
    'Another-Mirai-Native/Another-Mirai-Native.csproj',
    'UI_WPF/UI_WPF.csproj',
    'Another-Mirai-Native.WebAPI/Another-Mirai-Native.WebAPI.csproj',
    'Protocols/Protocol_OneBot/Protocol_OneBotv11.csproj',
    'Protocols/Protocol_MiraiAPIHttp/Protocol_MiraiAPIHttp.csproj',
    'Protocols/Protocol_LagrangeCore/Protocol_LagrangeCore.csproj',
    'Protocols/Protocol_NoConnection/Protocol_NoConnection.csproj'
)
foreach ($project in $projects) { Update-Project $project $Version $assemblyVersion }
Update-CqpAssemblyInfo

foreach ($change in $changes) {
    if ($PSCmdlet.ShouldProcess($change.Path, "Update version metadata (application $Version)")) {
        [IO.File]::WriteAllText($change.Path, $change.Content, $change.Encoding)
        Write-Host "Updated $($change.Path)"
    }
}
if ($changes.Count -eq 0) { Write-Host 'Version metadata is already up to date.' }
