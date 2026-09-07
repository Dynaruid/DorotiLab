#Requires -Version 7.0
[CmdletBinding()]
param([string] $OutputDirectory = '')
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
if (-not $OutputDirectory) {
    $OutputDirectory = ".doroti/evidence/windows-text-input-$(Get-Date -Format yyyyMMdd-HHmmss)"
}
$output = [IO.Path]::GetFullPath($OutputDirectory, $repoRoot)
New-Item -ItemType Directory -Path $output -Force | Out-Null

function Invoke-Stage([string] $Name, [string] $File, [string[]] $Arguments) {
    $stdout = Join-Path $output "$Name.stdout.log"
    $stderr = Join-Path $output "$Name.stderr.log"
    if ((Test-Path -LiteralPath $stdout) -or (Test-Path -LiteralPath $stderr)) {
        throw 'Choose a new output directory to preserve prior evidence.'
    }
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $File
    $start.WorkingDirectory = $repoRoot
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    foreach ($argument in $Arguments) { [void] $start.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::Start($start)
    try {
        $outTask = $process.StandardOutput.ReadToEndAsync()
        $errTask = $process.StandardError.ReadToEndAsync()
        if (-not $process.WaitForExit(20 * 60 * 1000)) {
            $process.Kill($true)
            throw "$Name exceeded the repository 20-minute timeout."
        }
        $outTask.GetAwaiter().GetResult() | Set-Content -LiteralPath $stdout
        $errTask.GetAwaiter().GetResult() | Set-Content -LiteralPath $stderr
        Get-Content -LiteralPath $stdout
        Get-Content -LiteralPath $stderr
        if ($process.ExitCode -ne 0) { throw "$Name failed: exit $($process.ExitCode); evidence: $output" }
    }
    finally { $process.Dispose() }
}

Invoke-Stage 'native-build' 'pwsh' @('-NoProfile', '-File', 'Doroti/eng/build-hwnd-exact-cpp-native.ps1')
Invoke-Stage 'native-text' 'dotnet' @('run', '--project', 'Doroti/validation/windowsappsdk-native-abi', '-c', 'Release', '--', '--text-input')
Invoke-Stage 'mounted-text' 'dotnet' @('run', '--project', 'Doroti/validation/fcr7-material-widget', '-c', 'Release', '--', '--mounted-text')
Write-Output "Evidence: $output"
