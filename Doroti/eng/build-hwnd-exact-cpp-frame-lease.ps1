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
$projectPath = Join-Path $repoRoot 'Doroti\validation\hwnd-exact-cpp-frame-lease\Doroti.HwndExactCpp.FrameLeaseProbe.vcxproj'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $vswhere)) { throw "vswhere.exe is missing: $vswhere" }
$msbuild = @(& $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe')[0]
if ([string]::IsNullOrWhiteSpace($msbuild)) { throw 'MSBuild.exe was not found.' }

$arguments = @(
    $projectPath,
    "/t:$Target",
    '/m',
    '/nologo',
    '/v:minimal',
    '/p:Configuration=Release',
    '/p:Platform=x64'
)

$start = [Diagnostics.ProcessStartInfo]::new()
$start.FileName = $msbuild
$start.UseShellExecute = $false
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
foreach ($argument in $arguments) { [void] $start.ArgumentList.Add($argument) }
$process = [Diagnostics.Process]::new()
$process.StartInfo = $start
if (-not $process.Start()) { throw 'C3 native frame-lease probe build failed to start.' }
$stdoutTask = $process.StandardOutput.ReadToEndAsync()
$stderrTask = $process.StandardError.ReadToEndAsync()
if (-not $process.WaitForExit($timeoutMilliseconds)) {
    $process.Kill($true)
    throw 'C3 native frame-lease probe build exceeded the 20-minute timeout.'
}
$stdout = $stdoutTask.GetAwaiter().GetResult()
$stderr = $stderrTask.GetAwaiter().GetResult()
if (-not [string]::IsNullOrWhiteSpace($stdout)) { Write-Host $stdout.TrimEnd() }
if (-not [string]::IsNullOrWhiteSpace($stderr)) { Write-Host $stderr.TrimEnd() }
if ($process.ExitCode -ne 0) { throw "C3 native frame-lease probe build failed with exit code $($process.ExitCode)." }

$binary = Join-Path $repoRoot 'Doroti\validation\hwnd-exact-cpp-frame-lease\bin\Release\x64\Doroti.HwndExactCpp.FrameLeaseProbe.dll'
if (-not (Test-Path -LiteralPath $binary -PathType Leaf)) { throw "C3 native frame-lease probe is missing: $binary" }
Write-Host "probeBinary=$binary"
Write-Host "sha256=$((Get-FileHash -LiteralPath $binary -Algorithm SHA256).Hash.ToLowerInvariant())"
