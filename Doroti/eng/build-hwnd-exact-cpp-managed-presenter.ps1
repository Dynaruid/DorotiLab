#Requires -Version 7.0
[CmdletBinding()]
param([ValidateSet('Build', 'Rebuild')][string] $Target = 'Build')

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$project = Join-Path $repoRoot 'Doroti\validation\hwnd-exact-cpp-managed-presenter\Doroti.HwndExactCpp.ManagedPresenterProbe.vcxproj'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path -LiteralPath $vswhere)) { throw "vswhere.exe is missing: $vswhere" }
$msbuild = @(& $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe')[0]
if ([string]::IsNullOrWhiteSpace($msbuild)) { throw 'MSBuild.exe was not found.' }

$start = [Diagnostics.ProcessStartInfo]::new()
$start.FileName = $msbuild
$start.UseShellExecute = $false
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
foreach ($argument in @($project, "/t:$Target", '/m', '/nologo', '/v:minimal', '/p:Configuration=Release', '/p:Platform=x64')) {
    [void] $start.ArgumentList.Add($argument)
}
$process = [Diagnostics.Process]::new()
$process.StartInfo = $start
if (-not $process.Start()) { throw 'Managed-presenter native probe build failed to start.' }
$stdout = $process.StandardOutput.ReadToEndAsync()
$stderr = $process.StandardError.ReadToEndAsync()
if (-not $process.WaitForExit(20 * 60 * 1000)) {
    $process.Kill($true)
    throw 'Managed-presenter native probe build exceeded the 20-minute timeout.'
}
$outText = $stdout.GetAwaiter().GetResult()
$errText = $stderr.GetAwaiter().GetResult()
if ($outText) { Write-Host $outText.TrimEnd() }
if ($errText) { Write-Host $errText.TrimEnd() }
if ($process.ExitCode -ne 0) { throw "Managed-presenter native probe build failed with exit code $($process.ExitCode)." }

$binary = Join-Path $repoRoot 'Doroti\validation\hwnd-exact-cpp-managed-presenter\bin\Release\x64\Doroti.HwndExactCpp.ManagedPresenterProbe.dll'
if (-not (Test-Path -LiteralPath $binary -PathType Leaf)) { throw "Managed-presenter probe is missing: $binary" }
Write-Host "probeBinary=$binary"
Write-Host "sha256=$((Get-FileHash -LiteralPath $binary -Algorithm SHA256).Hash.ToLowerInvariant())"
