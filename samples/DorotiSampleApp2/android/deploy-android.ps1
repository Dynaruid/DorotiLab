<#
.SYNOPSIS
Build, install and launch DorotiSampleApp2 Release on an ARM64 Android device.
.EXAMPLE
./deploy-android.ps1 -Mode CoreClrR2R
.EXAMPLE
./deploy-android.ps1 -Mode CoreClrJit -Serial YOUR_DEVICE_SERIAL
.EXAMPLE
./deploy-android.ps1 -DotnetVersion 11 -Mode CoreClrR2R
#>
[CmdletBinding()]
param(
    [ValidateSet(10, 11)]
    [int]$DotnetVersion = 10,
    [ValidateSet('MonoAot', 'CoreClrJit', 'CoreClrR2R')]
    [string]$Mode,
    [string]$Serial
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $Mode) {
    $Mode = if ($DotnetVersion -eq 11) { 'CoreClrR2R' } else { 'MonoAot' }
}
if ($DotnetVersion -eq 11 -and $Mode -eq 'MonoAot') {
    throw '.NET 11 supports CoreClrJit or CoreClrR2R in this script; use -DotnetVersion 10 for MonoAot.'
}

function Invoke-Checked {
    param([string]$Program, [string[]]$Arguments)
    & $Program @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$Program failed (exit $LASTEXITCODE): $($Arguments -join ' ')"
    }
}

$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$previousDotnetHost = $env:DOTNET_HOST_PATH
Push-Location $repoRoot
try {
    $dotnet = (Get-Command dotnet -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
    $python = (Get-Command python -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
    $adb = (Get-Command adb -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
    $env:DOTNET_HOST_PATH = $dotnet

    # Select the CLI SDK without changing the repository's .NET 10 global.json.
    if ($DotnetVersion -eq 11) {
        Set-Location (Join-Path $PSScriptRoot 'sdk/net11')
    }
    $sdkVersion = ((Invoke-Checked $dotnet @('--version')) -join '').Trim()
    if (-not $sdkVersion.StartsWith("$DotnetVersion.")) {
        throw "Expected .NET $DotnetVersion SDK, but selected $sdkVersion."
    }

    $deviceLines = @(Invoke-Checked $adb @('devices', '-l'))
    $devices = @(
        foreach ($line in $deviceLines) {
            if ($line -match '^(\S+)\s+(device|offline|unauthorized)(?:\s|$)') {
                [pscustomobject]@{ Serial = $Matches[1]; State = $Matches[2] }
            }
        }
    )
    if (-not $Serial) {
        if ($devices.Count -ne 1) {
            throw 'Connect one device, or select a device with -Serial. Check adb devices -l.'
        }
        $Serial = $devices[0].Serial
    }
    $selected = @($devices | Where-Object Serial -EQ $Serial)
    if ($selected.Count -ne 1 -or $selected[0].State -ne 'device') {
        throw "Device '$Serial' is not ready. Connect/unlock the device and authorize USB debugging."
    }
    $abi = ((Invoke-Checked $adb @('-s', $Serial, 'shell', 'getprop', 'ro.product.cpu.abi')) -join '').Trim()
    if ($abi -ne 'arm64-v8a') {
        throw "Device '$Serial' uses '$abi'; this script builds android-arm64 only."
    }

    $variant, $runtimeOptions = switch ($Mode) {
        'MonoAot' {
            'sample2-mono-aot'
            ,@('-p:UseMonoRuntime=true', '-p:PublishReadyToRun=false',
                '-p:RunAOTCompilation=true', '-p:AndroidEnableProfiledAot=true')
        }
        'CoreClrJit' {
            'sample2-coreclr-jit'
            ,@('-p:UseMonoRuntime=false', '-p:PublishReadyToRun=false',
                '-p:RunAOTCompilation=false', '-p:AndroidEnableProfiledAot=false')
        }
        'CoreClrR2R' {
            'sample2-coreclr-r2r'
            ,@('-p:UseMonoRuntime=false', '-p:PublishReadyToRun=true',
                '-p:RunAOTCompilation=false', '-p:AndroidEnableProfiledAot=false')
        }
    }
    $versionOptions = @()
    if ($DotnetVersion -eq 11) {
        $variant += '-net11'
        $versionOptions = @(
            '-p:DorotiAndroidTargetFramework=net11.0-android'
            '-p:DorotiAndroidMauiVersion=11.0.0-rc.1.26451.6'
            '-p:MauiVersion=11.0.0-rc.1.26451.6'
        )
    }
    $artifacts = Join-Path $repoRoot "Doroti/artifacts/$variant"
    $project = Join-Path $PSScriptRoot 'DorotiSampleApp2.Android.csproj'
    $timeoutRunner = Join-Path $repoRoot 'Doroti/eng/run-with-timeout.py'
    $buildArguments = @($timeoutRunner, $dotnet, 'build', $project, '-c', 'Release', '-r', 'android-arm64') +
        $runtimeOptions + $versionOptions + @('-p:AndroidPackageFormats=apk', "-p:ArtifactsPath=$artifacts")

    Write-Host "Building .NET $DotnetVersion / $Mode Release with SDK $sdkVersion for $Serial (20-minute limit)..."
    Invoke-Checked $python $buildArguments

    $apk = Join-Path $artifacts 'bin/DorotiSampleApp2.Android/release_android-arm64/dev.doroti.sample2-Signed.apk'
    if (-not (Test-Path -LiteralPath $apk -PathType Leaf)) {
        throw "Signed APK not found: $apk"
    }

    Invoke-Checked $python @($timeoutRunner, $adb, '-s', $Serial, 'install', '-r', $apk)
    Invoke-Checked $adb @('-s', $Serial, 'shell', 'am', 'force-stop', 'dev.doroti.sample2')
    Invoke-Checked $python @($timeoutRunner, $adb, '-s', $Serial, 'shell', 'monkey',
        '-p', 'dev.doroti.sample2', '-c', 'android.intent.category.LAUNCHER', '1')

    $appProcessId = ''
    $deadline = [DateTime]::UtcNow.AddSeconds(10)
    do {
        $pidOutput = & $adb -s $Serial shell pidof dev.doroti.sample2
        if ($LASTEXITCODE -eq 0) {
            $appProcessId = ([string]$pidOutput).Trim()
        }
        if ($appProcessId) { break }
        Start-Sleep -Milliseconds 500
    } while ([DateTime]::UtcNow -lt $deadline)
    if (-not $appProcessId) {
        throw 'App process did not start. Inspect adb logcat for startup errors.'
    }

    Write-Host "Installed and launched: .NET $DotnetVersion / $Mode / $Serial / PID $appProcessId"
    Write-Host "APK: $apk"
    Write-Host "Logs: adb -s $Serial logcat --pid=$appProcessId"
}
finally {
    $env:DOTNET_HOST_PATH = $previousDotnetHost
    Pop-Location
}
