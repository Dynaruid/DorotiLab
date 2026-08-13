[CmdletBinding()]
param(
    [ValidateSet('all', 'windows', 'linux')]
    [string] $Target = 'all',

    [string] $LinuxDistribution = 'Ubuntu',

    [string] $LinuxDotNet = ''
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$localCacheRoot = Get-DorotiCacheRoot -DorotiRoot $dorotiRoot
$artifactsRoot = Join-Path $dorotiRoot 'artifacts/h4-distribution'
$runRoot = Join-Path $artifactsRoot "run-$([DateTimeOffset]::UtcNow.ToString('yyyyMMddTHHmmssZ'))"
$feed = Join-Path $runRoot 'feed'
$templateHive = Join-Path $runRoot 'template-hive'
$consumerRoot = Join-Path $runRoot 'consumer-source'
$consumerProject = Join-Path $consumerRoot 'H4ExternalConsumer.csproj'
$reportPath = Join-Path $artifactsRoot 'distribution-report.json'
$templateInstalled = $false

function Invoke-Checked {
    param(
        [Parameter(Mandatory)] [string] $File,
        [Parameter(Mandatory)] [AllowEmptyCollection()] [string[]] $Arguments,
        [string] $WorkingDirectory = $dorotiRoot
    )

    Push-Location $WorkingDirectory
    try {
        & $File @Arguments
        if ($LASTEXITCODE -ne 0) {
            throw "Command failed with exit code ${LASTEXITCODE}: $File $($Arguments -join ' ')"
        }
    }
    finally {
        Pop-Location
    }
}

function Invoke-WslChecked {
    param(
        [Parameter(Mandatory)] [string] $File,
        [Parameter(Mandatory)] [AllowEmptyCollection()] [string[]] $Arguments
    )

    & wsl.exe -d $LinuxDistribution -- env "DOTNET_CLI_HOME=$linuxCliHome" 'NUGET_XMLDOC_MODE=skip' $File @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "WSL command failed with exit code ${LASTEXITCODE}: $File $($Arguments -join ' ')"
    }
}

function Convert-ToWslPath {
    param([Parameter(Mandatory)] [string] $Path)

    $resolved = [System.IO.Path]::GetFullPath($Path)
    if ($resolved -notmatch '^([A-Za-z]):\\(.*)$') {
        throw "H4 WSL validation requires an absolute Windows drive path: $resolved"
    }
    $drive = $Matches[1].ToLowerInvariant()
    $relative = $Matches[2].Replace('\', '/')
    return "/mnt/$drive/$relative"
}

$windowsCliHome = Join-Path $localCacheRoot 'dotnet-cli-h4-linux'
[IO.Directory]::CreateDirectory($windowsCliHome) | Out-Null
$linuxCliHome = Convert-ToWslPath $windowsCliHome

function Write-NuGetConfig {
    param(
        [Parameter(Mandatory)] [string] $Path,
        [Parameter(Mandatory)] [string] $LocalFeed
    )

    $escapedFeed = [System.Security.SecurityElement]::Escape($LocalFeed)
    $content = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="doroti-h4-local" value="$escapedFeed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
"@
    [System.IO.File]::WriteAllText($Path, ($content -replace "`r`n", "`n") + "`n", [System.Text.UTF8Encoding]::new($false))
}

New-Item -ItemType Directory -Force -Path $feed, $templateHive, $consumerRoot | Out-Null
Invoke-Checked 'dotnet' @(
    'pack', (Join-Path $dorotiRoot 'Doroti.Product.slnx'),
    '--configuration', 'Release', '--output', $feed, '--nologo')

$templatePackages = @(Get-ChildItem -LiteralPath $feed -Filter 'Doroti.Templates.*.nupkg' -File |
    Where-Object { $_.Name -notlike '*.symbols.nupkg' })
if ($templatePackages.Count -ne 1) {
    throw "Expected one Doroti.Templates package in the H4 feed; found $($templatePackages.Count)."
}
$templatePackage = $templatePackages[0]
try {
    Invoke-Checked 'dotnet' @('new', 'install', $templatePackage.FullName, '--force', '--debug:custom-hive', $templateHive)
    $templateInstalled = $true
    Invoke-Checked 'dotnet' @(
        'new', 'doroti-counter', '--name', 'H4ExternalConsumer', '--output', $consumerRoot,
        '--debug:custom-hive', $templateHive)

    $sourceHashes = [ordered]@{
        project = (Get-FileHash -LiteralPath $consumerProject -Algorithm SHA256).Hash.ToLowerInvariant()
        program = (Get-FileHash -LiteralPath (Join-Path $consumerRoot 'Program.cs') -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    $windows = [ordered]@{ status = 'not-run' }
    $linux = [ordered]@{ status = 'not-run' }

    if ($Target -in @('all', 'windows')) {
        $windowsPackages = Join-Path $runRoot 'windows-packages'
        $windowsPublish = Join-Path $runRoot 'publish-win-x64'
        $windowsConfig = Join-Path $runRoot 'NuGet.windows.config'
        Write-NuGetConfig $windowsConfig $feed
        Invoke-Checked 'dotnet' @(
            'restore', $consumerProject, '--configfile', $windowsConfig,
            '--packages', $windowsPackages, '--runtime', 'win-x64', '--force-evaluate', '--no-cache',
            '-p:PublishReadyToRun=true')
        Invoke-Checked 'dotnet' @('build', $consumerProject, '--configuration', 'Release', '--no-restore', '--nologo')
        Invoke-Checked 'dotnet' @('run', '--project', $consumerProject, '--configuration', 'Release', '--no-build', '--', '--smoke')
        Invoke-Checked 'dotnet' @(
            'publish', $consumerProject, '--configuration', 'Release', '--output', $windowsPublish,
            '--no-restore', '--runtime', 'win-x64', '--self-contained', 'true',
            '-p:PublishSingleFile=true', '-p:PublishReadyToRun=true')
        Invoke-Checked (Join-Path $windowsPublish 'H4ExternalConsumer.exe') @('--smoke')
        $windows = [ordered]@{
            status = 'pass'
            target = 'windows-win-x64'
            restore = 'pass'
            build = 'pass'
            windowRun = 'pass'
            publish = 'pass'
            packagedRun = 'pass'
        }
    }

    if ($Target -in @('all', 'linux')) {
        $resolvedLinuxDotNet = $LinuxDotNet
        if ([string]::IsNullOrWhiteSpace($resolvedLinuxDotNet)) {
            $linuxHomeOutput = @(& wsl.exe -d $LinuxDistribution -- printenv HOME 2>&1)
            $linuxHome = if ($linuxHomeOutput.Count -gt 0) { $linuxHomeOutput[-1].ToString().Trim() } else { '' }
            if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($linuxHome)) {
                throw "Could not determine the home directory for WSL distribution $LinuxDistribution."
            }
            $resolvedLinuxDotNet = "$linuxHome/.doroti-h4-dotnet/dotnet"
        }
        $wslDotNetVersionOutput = @(& wsl.exe -d $LinuxDistribution -- $resolvedLinuxDotNet --version 2>&1)
        $wslDotNetVersion = if ($wslDotNetVersionOutput.Count -gt 0) { $wslDotNetVersionOutput[-1].ToString().Trim() } else { '' }
        if ($LASTEXITCODE -ne 0 -or $wslDotNetVersion -ne '10.0.300') {
            throw "WSL $LinuxDistribution must provide .NET SDK 10.0.300 at $resolvedLinuxDotNet."
        }
        $linuxProject = Convert-ToWslPath $consumerProject
        $linuxFeed = Convert-ToWslPath $feed
        $linuxPackages = Convert-ToWslPath (Join-Path $runRoot 'linux-packages')
        $linuxPublish = Convert-ToWslPath (Join-Path $runRoot 'publish-linux-x64')
        $linuxConfigPath = Join-Path $runRoot 'NuGet.linux.config'
        Write-NuGetConfig $linuxConfigPath $linuxFeed
        $linuxConfig = Convert-ToWslPath $linuxConfigPath
        Invoke-WslChecked $resolvedLinuxDotNet @(
            'restore', $linuxProject, '--configfile', $linuxConfig,
            '--packages', $linuxPackages, '--runtime', 'linux-x64', '--force-evaluate', '--no-cache',
            '-p:PublishReadyToRun=true')
        Invoke-WslChecked $resolvedLinuxDotNet @('build', $linuxProject, '--configuration', 'Release', '--no-restore', '--nologo')
        Invoke-WslChecked 'xvfb-run' @('-a', $resolvedLinuxDotNet, 'run', '--project', $linuxProject, '--configuration', 'Release', '--no-build', '--', '--smoke')
        Invoke-WslChecked $resolvedLinuxDotNet @(
            'publish', $linuxProject, '--configuration', 'Release', '--output', $linuxPublish,
            '--no-restore', '--runtime', 'linux-x64', '--self-contained', 'true',
            '-p:PublishSingleFile=true', '-p:PublishReadyToRun=true')
        Invoke-WslChecked 'xvfb-run' @('-a', "$linuxPublish/H4ExternalConsumer", '--smoke')
        $linux = [ordered]@{
            status = 'pass'
            target = 'ubuntu-26.04-wsl2-linux-x64-xvfb'
            dotnetSdk = $wslDotNetVersion
            restore = 'pass'
            build = 'pass'
            windowRun = 'pass'
            publish = 'pass'
            packagedRun = 'pass'
        }
    }
}
finally {
    if ($templateInstalled) {
        Invoke-Checked 'dotnet' @('new', 'uninstall', 'Doroti.Templates', '--debug:custom-hive', $templateHive)
    }
}

$report = [ordered]@{
    schemaVersion = 'doroti.h4-distribution-report/v1'
    generatedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
    localFeed = [ordered]@{
        status = 'pass'
        packageCount = @(Get-ChildItem -LiteralPath $feed -Filter '*.nupkg' -File).Count
    }
    consumer = [ordered]@{
        template = 'doroti-counter'
        publicSourceHashes = $sourceHashes
        templateInstall = 'pass'
        templateUninstall = 'pass'
        repositoryProjectReferences = 0
    }
    targets = @($windows, $linux)
    cleanWindowsVm = [ordered]@{
        status = 'not-verified'
        reason = 'The Windows run used an isolated package cache, template hive, and external workspace on the development machine; no disposable clean VM was attached.'
    }
}
New-Item -ItemType Directory -Force -Path $artifactsRoot | Out-Null
$json = $report | ConvertTo-Json -Depth 8
[System.IO.File]::WriteAllText($reportPath, ($json -replace "`r`n", "`n") + "`n", [System.Text.UTF8Encoding]::new($false))
Write-Host "H4 distribution: PASS ($reportPath)"
