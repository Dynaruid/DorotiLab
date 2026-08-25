[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$projectPath = Join-Path $repoRoot 'Doroti\validation\winrt-content-island-native-bridge\Doroti.WinRtContentIsland.NativeBridge.vcxproj'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $vswhere)) { throw "vswhere.exe is missing: $vswhere" }
$msbuild = @(& $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe')[0]
if ([string]::IsNullOrWhiteSpace($msbuild)) { throw 'MSBuild.exe was not found.' }

$sdkVersion = '10.0.26100.0'
$cppWinRt = Join-Path ${env:ProgramFiles(x86)} "Windows Kits\10\bin\$sdkVersion\x64\cppwinrt.exe"
$nugetRoot = (& dotnet nuget locals global-packages --list).Split(':', 2)[1].Trim()
$interactiveRoot = Join-Path $nugetRoot 'microsoft.windowsappsdk.interactiveexperiences\2.1.6'
$metadataRoot = Join-Path $interactiveRoot 'metadata\10.0.18362.0'
$arguments = @(
    $projectPath,
    '/restore',
    '/m',
    '/nologo',
    '/v:minimal',
    '/p:Configuration=Release',
    '/p:Platform=x64',
    "/p:CppWinRTExe=$cppWinRt",
    "/p:MicrosoftUiWinmd=$(Join-Path $metadataRoot 'Microsoft.UI.winmd')",
    "/p:MicrosoftFoundationWinmd=$(Join-Path $metadataRoot 'Microsoft.Foundation.winmd')",
    "/p:MicrosoftGraphicsWinmd=$(Join-Path $metadataRoot 'Microsoft.Graphics.winmd')",
    "/p:InteractiveExperiencesInclude=$(Join-Path $interactiveRoot 'include')"
)

$start = [System.Diagnostics.ProcessStartInfo]::new()
$start.FileName = $msbuild
$start.UseShellExecute = $false
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
foreach ($argument in $arguments) { [void]$start.ArgumentList.Add($argument) }
$process = [System.Diagnostics.Process]::new()
$process.StartInfo = $start
if (-not $process.Start()) { throw 'Native bridge build failed to start.' }
$stdoutTask = $process.StandardOutput.ReadToEndAsync()
$stderrTask = $process.StandardError.ReadToEndAsync()
if (-not $process.WaitForExit($timeoutMilliseconds)) {
    $process.Kill($true)
    throw 'Native bridge build exceeded the 20-minute test timeout.'
}
$stdout = $stdoutTask.GetAwaiter().GetResult()
$stderr = $stderrTask.GetAwaiter().GetResult()
if (-not [string]::IsNullOrWhiteSpace($stdout)) { Write-Host $stdout.TrimEnd() }
if (-not [string]::IsNullOrWhiteSpace($stderr)) { Write-Host $stderr.TrimEnd() }
if ($process.ExitCode -ne 0) { throw "Native bridge build failed with exit code $($process.ExitCode)." }

$binary = Join-Path $repoRoot 'Doroti\validation\winrt-content-island-native-bridge\bin\Release\x64\Doroti.WinRtContentIsland.NativeBridge.dll'
if (-not (Test-Path -LiteralPath $binary)) { throw "Native bridge binary is missing: $binary" }
Write-Host "nativeBridge=$binary"
Write-Host "sha256=$((Get-FileHash -LiteralPath $binary -Algorithm SHA256).Hash.ToLowerInvariant())"
