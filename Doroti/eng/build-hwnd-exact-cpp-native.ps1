#Requires -Version 7.0
[CmdletBinding()]
param(
    [ValidateSet('Build', 'Rebuild')]
    [string] $Target = 'Build'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$projectPath = Join-Path $repoRoot 'Doroti\src\Doroti.Host.WindowsAppSdk.Native\Doroti.Host.WindowsAppSdk.Native.vcxproj'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $vswhere)) { throw "vswhere.exe is missing: $vswhere" }
$msbuild = @(& $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe')[0]
if ([string]::IsNullOrWhiteSpace($msbuild)) { throw 'MSBuild.exe was not found.' }

$sdkVersion = '10.0.26100.0'
$cppWinRt = Join-Path ${env:ProgramFiles(x86)} "Windows Kits\10\bin\$sdkVersion\x64\cppwinrt.exe"
$nugetRoot = (& dotnet nuget locals global-packages --list).Split(':', 2)[1].Trim()
$interactiveRoot = Join-Path $nugetRoot 'microsoft.windowsappsdk.interactiveexperiences\2.1.6'
$metadataRoot = Join-Path $interactiveRoot 'metadata\10.0.18362.0'
$foundationRoot = Join-Path $nugetRoot 'microsoft.windowsappsdk.foundation\2.3.9'
$runtimeRoot = Join-Path $nugetRoot 'microsoft.windowsappsdk.runtime\2.4.0'
if (-not (Test-Path -LiteralPath $cppWinRt)) { throw "Pinned C++/WinRT tool is missing: $cppWinRt" }
$arguments = @(
    $projectPath,
    "/t:$Target",
    '/m',
    '/nologo',
    '/v:minimal',
    '/p:Configuration=Release',
    '/p:Platform=x64',
    "/p:CppWinRTExe=$cppWinRt",
    "/p:MicrosoftUiWinmd=$(Join-Path $metadataRoot 'Microsoft.UI.winmd')",
    "/p:MicrosoftFoundationWinmd=$(Join-Path $metadataRoot 'Microsoft.Foundation.winmd')",
    "/p:MicrosoftGraphicsWinmd=$(Join-Path $metadataRoot 'Microsoft.Graphics.winmd')",
    "/p:InteractiveExperiencesInclude=$(Join-Path $interactiveRoot 'include')",
    "/p:FoundationInclude=$(Join-Path $foundationRoot 'include')",
    "/p:RuntimeInclude=$(Join-Path $runtimeRoot 'include')",
    "/p:FoundationLib=$(Join-Path $foundationRoot 'lib\native\x64')"
)

$start = [Diagnostics.ProcessStartInfo]::new()
$start.FileName = $msbuild
$start.UseShellExecute = $false
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
foreach ($argument in $arguments) { [void] $start.ArgumentList.Add($argument) }
$process = [Diagnostics.Process]::new()
$process.StartInfo = $start
if (-not $process.Start()) { throw 'Native host build failed to start.' }
$stdoutTask = $process.StandardOutput.ReadToEndAsync()
$stderrTask = $process.StandardError.ReadToEndAsync()
if (-not $process.WaitForExit($timeoutMilliseconds)) {
    $process.Kill($true)
    throw 'Native host build exceeded the 20-minute timeout.'
}
$stdout = $stdoutTask.GetAwaiter().GetResult()
$stderr = $stderrTask.GetAwaiter().GetResult()
if (-not [string]::IsNullOrWhiteSpace($stdout)) { Write-Host $stdout.TrimEnd() }
if (-not [string]::IsNullOrWhiteSpace($stderr)) { Write-Host $stderr.TrimEnd() }
if ($process.ExitCode -ne 0) { throw "Native host build failed with exit code $($process.ExitCode)." }

$binary = Join-Path $repoRoot 'Doroti\src\Doroti.Host.WindowsAppSdk.Native\bin\Release\x64\doroti_windows_appsdk_host_v1.dll'
$bootstrap = Join-Path $foundationRoot 'runtimes\win-x64\native\Microsoft.WindowsAppRuntime.Bootstrap.dll'
if (-not (Test-Path -LiteralPath $binary -PathType Leaf)) { throw "Native host binary is missing: $binary" }
if (-not (Test-Path -LiteralPath $bootstrap -PathType Leaf)) { throw "Windows App Runtime bootstrap DLL is missing: $bootstrap" }
Copy-Item -LiteralPath $bootstrap -Destination (Join-Path (Split-Path -Parent $binary) 'Microsoft.WindowsAppRuntime.Bootstrap.dll') -Force
Write-Host "nativeBinary=$binary"
Write-Host "sha256=$((Get-FileHash -LiteralPath $binary -Algorithm SHA256).Hash.ToLowerInvariant())"
Write-Host "architecture=x64"
Write-Host "windowsSdk=$sdkVersion"
Write-Host "cppWinRT=$cppWinRt"
