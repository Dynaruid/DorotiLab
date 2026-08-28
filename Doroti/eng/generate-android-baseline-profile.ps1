#Requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $Serial,
    [Parameter(Mandatory)] [string] $Apk,
    [string] $Package = 'dev.doroti.demo',
    [string] $OutputDirectory
)

$ErrorActionPreference = 'Stop'
$Apk = [IO.Path]::GetFullPath($Apk)
if (-not (Test-Path -LiteralPath $Apk -PathType Leaf)) { throw "APK was not found: $Apk" }
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path ([IO.Path]::GetDirectoryName($Apk)) 'profiles'
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($OutputDirectory) | Out-Null

$androidSdk = if ($env:ANDROID_HOME) { $env:ANDROID_HOME } else { $env:ANDROID_SDK_ROOT }
if ([string]::IsNullOrWhiteSpace($androidSdk)) { throw 'ANDROID_HOME or ANDROID_SDK_ROOT is required.' }
$profgen = Join-Path $androidSdk 'cmdline-tools/latest/bin/profgen.bat'
if (-not (Test-Path -LiteralPath $profgen -PathType Leaf)) { throw "profgen was not found: $profgen" }

$device = @(adb devices | Select-String -Pattern "^$([regex]::Escape($Serial))\s+device$")
if ($device.Count -ne 1) { throw "Android device is not connected and authorized: $Serial" }

$remoteProfile = "/data/misc/profman/$Package.prof"
$snapshot = Join-Path $OutputDirectory "$Package-snapshot.prof"
$humanReadable = Join-Path $OutputDirectory 'baseline-prof.txt'
$binaryProfile = Join-Path $OutputDirectory 'baseline.prof'
$binaryMetadata = Join-Path $OutputDirectory 'baseline.profm'

& adb -s $Serial shell cmd package snapshot-profile $Package
if ($LASTEXITCODE -ne 0) { throw 'Android profile snapshot failed. Exercise the startup CUJ first.' }
& adb -s $Serial pull $remoteProfile $snapshot
if ($LASTEXITCODE -ne 0) { throw 'Android profile pull failed.' }
& $profgen dumpProfile -p $snapshot -a $Apk -o $humanReadable
if ($LASTEXITCODE -ne 0) { throw 'profgen could not convert the device snapshot to HRF.' }
& $profgen validate $humanReadable
if ($LASTEXITCODE -ne 0) { throw 'profgen rejected the generated HRF.' }
& $profgen bin $humanReadable -a $Apk -o $binaryProfile -om $binaryMetadata -pf v0_1_0_p
if ($LASTEXITCODE -ne 0) { throw 'profgen could not generate baseline.prof/baseline.profm.' }

[pscustomobject]@{
    package = $Package
    serial = $Serial
    apkSha256 = (Get-FileHash -LiteralPath $Apk -Algorithm SHA256).Hash.ToLowerInvariant()
    hrf = $humanReadable
    profile = $binaryProfile
    metadata = $binaryMetadata
} | ConvertTo-Json
