[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $VariantsFile,
    [ValidatePattern('^[a-zA-Z0-9-]+$')] [string] $ArtifactPrefix = 'resize-v2-matrix',
    [string[]] $Edges = @('Right','Bottom','Left','TopLeft'),
    [int[]] $Durations = @(150,600),
    [string[]] $Motions = @('expand','shrink','reverse'),
    [ValidateRange(1,3)] [int] $Runs = 3,
    [int] $Port = 5188
)
$ErrorActionPreference = 'Stop'
$variants = @(Get-Content -LiteralPath $VariantsFile -Raw | ConvertFrom-Json)
if ($variants.Count -eq 0) { throw 'At least one variant is required.' }
$results = [Collections.Generic.List[object]]::new()
$names = @('DOROTI_FAST_RESIZE_EDGE','DOROTI_FAST_RESIZE_MS','DOROTI_FAST_RESIZE_MOTION','DOROTI_FAST_RESIZE_RUNS','DOROTI_RESIZE_EXPERIMENT_QUERY','DOROTI_FAST_RESIZE_TRACE','DOROTI_FAST_RESIZE_CAPTURE')
$prior = @{}
foreach ($name in $names) { $prior[$name] = [Environment]::GetEnvironmentVariable($name) }
try {
    foreach ($edge in $Edges) { foreach ($duration in $Durations) { foreach ($motion in $Motions) {
        for ($run=0; $run -lt $Runs; $run++) { foreach ($variant in $variants) {
            if ($variant.name -notmatch '^[a-zA-Z0-9-]+$') { throw 'Variant name must be alphanumeric/hyphen.' }
            $label = "$ArtifactPrefix/$edge-$duration-$motion-$run-$($variant.name)"
            $env:DOROTI_FAST_RESIZE_EDGE=$edge
            $env:DOROTI_FAST_RESIZE_MS=[string]$duration
            $env:DOROTI_FAST_RESIZE_MOTION=$motion
            $env:DOROTI_FAST_RESIZE_RUNS='1'
            $env:DOROTI_RESIZE_EXPERIMENT_QUERY=[string]$variant.query
            $env:DOROTI_FAST_RESIZE_TRACE=if($variant.trace){'1'}else{'0'}
            $env:DOROTI_FAST_RESIZE_CAPTURE=if($variant.capture){'1'}else{'0'}
            Write-Output "Running $label"
            & pwsh -NoProfile -File (Join-Path $PSScriptRoot 'run-web-playwright.ps1') -SkipBuild -FastResize -Configuration Release -BuildMode $variant.buildMode -Port $Port -ArtifactLabel $label
            # Preserve stimulus/correctness failures and continue the remaining
            # independent conditions. Every child Playwright has the 20m timeout.
            $results.Add(@{ label=$label; processExitCode=$LASTEXITCODE })
        }}
    }}}
}
finally {
    foreach ($name in $names) { [Environment]::SetEnvironmentVariable($name,$prior[$name]) }
    $path=Join-Path $PSScriptRoot "../validation/web-playwright/artifacts/$ArtifactPrefix/process-results.json"
    [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName([IO.Path]::GetFullPath($path))) | Out-Null
    $results | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $path
}
if (@($results | Where-Object { $_.processExitCode -ne 0 }).Count) { exit 1 }
