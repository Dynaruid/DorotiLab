#Requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string[]] $ImageFiles,
    [string] $DartExecutable = 'dart',
    [string] $OutputDirectory = ".doroti/evidence/image-pipeline-$(Get-Date -Format yyyyMMdd-HHmmss)"
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$output = [IO.Path]::GetFullPath($OutputDirectory, $repoRoot)
if (Test-Path -LiteralPath $output) { throw 'Use a new output directory to preserve evidence.' }
$inputs = @($ImageFiles | ForEach-Object {
    $path = [IO.Path]::GetFullPath($_, $repoRoot)
    if (!(Test-Path -LiteralPath $path -PathType Leaf)) { throw "Image not found: $path" }
    $path
})
New-Item -ItemType Directory -Path $output | Out-Null
$inputs | ForEach-Object { Get-FileHash -LiteralPath $_ -Algorithm SHA256 } |
    Select-Object Path, Hash | ConvertTo-Json -AsArray | Set-Content (Join-Path $output 'inputs.json')
function Invoke-Checked([string] $Executable, [string[]] $Arguments, [string] $Label) {
    $start = [Diagnostics.ProcessStartInfo]::new((Get-Command $Executable).Source)
    $start.WorkingDirectory = $repoRoot
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    foreach ($argument in $Arguments) { $start.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::Start($start)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    try {
        if (!$process.WaitForExit(1200000)) { $process.Kill($true); throw "$Label exceeded 20 minutes." }
        $stdout.Result | Set-Content (Join-Path $output "$Label.stdout.log")
        $stderr.Result | Set-Content (Join-Path $output "$Label.stderr.log")
        Write-Output $stdout.Result
        if ($process.ExitCode -ne 0) { throw "$Label failed ($($process.ExitCode)): $($stderr.Result)" }
    } finally { $process.Dispose() }
}
$native = Join-Path $output 'native'
Invoke-Checked dotnet (@('run','--project','Doroti/validation/fcr7-material-widget','-c','Release','--','--image-files',$native) + $inputs) 'native'
$files = @(Get-ChildItem -LiteralPath $native -Filter *.json | ForEach-Object FullName)
Invoke-Checked $DartExecutable (@('--packages=reference/flutter_sample_app/.dart_tool/package_config.json','Doroti/validation/image-pipeline/color-oracle.dart') + $files) 'oracle'
Write-Output "Image pipeline and pinned Dart differential: PASS. Evidence: $output"
