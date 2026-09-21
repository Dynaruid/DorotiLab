[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$start = [Diagnostics.ProcessStartInfo]::new('dotnet')
$start.WorkingDirectory = $repositoryRoot
$start.UseShellExecute = $false
$start.CreateNoWindow = $true
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
foreach ($argument in @('run', '--project', $PSScriptRoot, '-c', 'Release', '--', $repositoryRoot)) {
    $start.ArgumentList.Add($argument)
}
$process = [Diagnostics.Process]::Start($start)
$stdout = $process.StandardOutput.ReadToEndAsync()
$stderr = $process.StandardError.ReadToEndAsync()
try {
    if (-not $process.WaitForExit(1200000)) {
        $process.Kill($true)
        throw 'Compiler null-semantics validation exceeded the 20-minute timeout.'
    }
    [Console]::Write($stdout.GetAwaiter().GetResult())
    [Console]::Error.Write($stderr.GetAwaiter().GetResult())
    if ($process.ExitCode -ne 0) { throw "Compiler null-semantics validation failed: exit $($process.ExitCode)." }
} finally { $process.Dispose() }
