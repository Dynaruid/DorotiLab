[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$report = Join-Path $repositoryRoot '.doroti/framework-virtual-dispatch.json'

function Invoke-DotnetValidation {
    param([string[]] $Arguments)
    $start = [Diagnostics.ProcessStartInfo]::new('dotnet')
    $start.WorkingDirectory = $repositoryRoot
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    foreach ($argument in $Arguments) { $start.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::Start($start)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    try {
        if (-not $process.WaitForExit(1200000)) {
            $process.Kill($true)
            throw 'Virtual dispatch validation exceeded the 20-minute timeout.'
        }
        [Console]::Write($stdout.GetAwaiter().GetResult())
        [Console]::Error.Write($stderr.GetAwaiter().GetResult())
        if ($process.ExitCode -ne 0) {
            throw "dotnet $($Arguments -join ' ') failed with exit code $($process.ExitCode)."
        }
    }
    finally { $process.Dispose() }
}

# Fresh assemblies are required to resolve contracts across framework projects.
Invoke-DotnetValidation @('build', 'Doroti/validation/fcr7-material-widget', '-c', 'Release')
Invoke-DotnetValidation @('run', '--project', 'Doroti/validation/virtual-dispatch', '-c', 'Release', '--', $repositoryRoot, $report)
Invoke-DotnetValidation @('run', '--project', 'Doroti/validation/fcr7-material-widget', '-c', 'Release', '--no-build', '--', '--virtual-dispatch')
Write-Output "Framework virtual dispatch validation: PASS; report=$report"
