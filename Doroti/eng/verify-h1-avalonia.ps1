[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$project = Join-Path $dorotiRoot 'samples/AvaloniaHostCounter/AvaloniaHostCounter.csproj'
$artifactRoot = Join-Path $dorotiRoot 'artifacts/h1-avalonia'

if (-not [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)) {
    throw 'H1 Avalonia target verification requires Windows.'
}

foreach ($renderer in @('hardware', 'software')) {
    $output = Join-Path $artifactRoot $renderer
    & dotnet run --project $project --configuration Release -- --renderer $renderer --artifact-dir $output --verify-target
    if ($LASTEXITCODE -ne 0) {
        throw "H1 Avalonia $renderer verification failed with exit code $LASTEXITCODE."
    }
}

Write-Host 'H1 Avalonia hardware/software target verification: PASS'
