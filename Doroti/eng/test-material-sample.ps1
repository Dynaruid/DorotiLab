#Requires -Version 7.0
[CmdletBinding()]
param(
    [ValidateSet('Search', 'Blockers', 'Regression', 'Windows', 'AppBarRaster', 'Tabs', 'Scroll', 'Selection')]
    [string] $Suite = 'Search',
    [string] $OutputDirectory = ''
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
if (-not $OutputDirectory) {
    $OutputDirectory = ".doroti/evidence/material-sample-$Suite-$(Get-Date -Format yyyyMMdd-HHmmss)-$([Guid]::NewGuid().ToString('N').Substring(0, 8))"
}
$output = [IO.Path]::GetFullPath($OutputDirectory, $repoRoot)
New-Item -ItemType Directory -Path $output -Force | Out-Null
$stdout = Join-Path $output 'contracts.stdout.log'
$stderr = Join-Path $output 'contracts.stderr.log'
if ((Test-Path -LiteralPath $stdout) -or (Test-Path -LiteralPath $stderr)) {
    throw 'Select a new output directory to preserve prior validation evidence.'
}
$arguments = @('run', '--project', 'Doroti/validation/fcr7-material-widget', '-c', 'Release')
if ($Suite -eq 'Search') { $arguments += @('--', '--material-sample-search') }
if ($Suite -eq 'Tabs') { $arguments += @('--', '--tabs') }
if ($Suite -eq 'Scroll') { $arguments += @('--', '--sample-scroll', ('"' + $output + '"')) }
if ($Suite -eq 'Selection') { $arguments += @('--', '--selection-controls', ('"' + $output + '"')) }
if ($Suite -eq 'Blockers') { $arguments += @('--', '--material-sample') }
if ($Suite -eq 'Windows') { $arguments += @('--', '--windows-sample', ('"' + $output + '"')) }
if ($Suite -eq 'AppBarRaster') { $arguments += @('--', '--appbar-raster', ('"' + $output + '"')) }
$process = Start-Process -FilePath (Get-Command dotnet -ErrorAction Stop).Source -ArgumentList $arguments `
    -WorkingDirectory $repoRoot -WindowStyle Hidden -PassThru `
    -RedirectStandardOutput $stdout -RedirectStandardError $stderr
try {
    if (-not $process.WaitForExit(20 * 60 * 1000)) {
        $process.Kill($true)
        throw 'Material sample validation exceeded the repository 20-minute timeout.'
    }
    Get-Content -LiteralPath $stdout
    Get-Content -LiteralPath $stderr
    if ($process.ExitCode -ne 0) {
        throw "Material sample $Suite failed (exit $($process.ExitCode)); evidence: $output"
    }
    Write-Output "Evidence: $output"
}
finally { $process.Dispose() }
