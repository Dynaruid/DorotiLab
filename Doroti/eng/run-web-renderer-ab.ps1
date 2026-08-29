[CmdletBinding()]
param(
    [ValidateRange(1, 10)]
    [int] $Runs = 3,

    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [switch] $SkipBuild
)

$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$wrapper = Join-Path $PSScriptRoot 'run-web-playwright.ps1'
$wrapperArtifacts = Join-Path $root 'Doroti/validation/web-playwright/artifacts/wrapper'
$artifact = Join-Path $root 'Doroti/validation/web-playwright/artifacts/renderer-ab.json'
$project = Join-Path $root 'DorotiDemoApp/web/DorotiDemoApp.Web.csproj'

if (-not $SkipBuild) {
    & dotnet build $project --configuration $Configuration --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Doroti renderer A/B build failed.' }
}

$results = [ordered]@{}
foreach ($mode in @('document-webgl', 'offscreen-bitmap', 'offscreen-worker')) {
    $samples = @()
    for ($run = 1; $run -le $Runs; $run++) {
        $label = "ab/$mode/run-$run"
        & pwsh -NoProfile -File $wrapper -Configuration $Configuration -HeadlessOnly -SkipBuild `
            -RendererMode $mode -ArtifactLabel $label -TestFile 'tests/wheel-continuity.spec.ts'
        if ($LASTEXITCODE -ne 0) { throw "Doroti renderer A/B run failed: $mode/$run" }
        $stdout = Get-Content -Raw -LiteralPath (Join-Path $wrapperArtifacts "$label/playwright.stdout.log")
        $match = [regex]::Match($stdout, 'WHEEL_LATENCY (\{[^\r\n]+\})')
        if (-not $match.Success) { throw "Doroti renderer A/B output is missing latency evidence: $mode/$run" }
        $value = $match.Groups[1].Value | ConvertFrom-Json
        $samples += [ordered]@{
            run = $run
            sampleCount = [int]$value.samples
            p95Milliseconds = [double]$value.p95Milliseconds
            maxMilliseconds = [double]$value.maxMilliseconds
        }
    }
    $orderedP95 = @($samples.p95Milliseconds | Sort-Object)
    $medianP95 = [double]$orderedP95[[int][Math]::Floor($orderedP95.Count / 2)]
    $results[$mode] = [ordered]@{
        runs = $samples
        medianP95Milliseconds = $medianP95
        absoluteGatePassed = $medianP95 -le 33.4 -and (($samples.maxMilliseconds | Measure-Object -Maximum).Maximum -lt 100)
    }
}

$direct = [double]$results['document-webgl'].medianP95Milliseconds
foreach ($mode in @('offscreen-bitmap', 'offscreen-worker')) {
    $median = [double]$results[$mode].medianP95Milliseconds
    $results[$mode].comparisonGatePassed =
        $results[$mode].absoluteGatePassed -and
        $median -le ($direct * 1.2) -and
        $median -le ($direct + 5)
}
$selected = if ($results['offscreen-worker'].comparisonGatePassed) {
    'offscreen-worker'
} elseif ($results['offscreen-bitmap'].comparisonGatePassed) {
    'offscreen-bitmap'
} else {
    'document-webgl'
}
$report = [ordered]@{
    schemaVersion = 'doroti.web-renderer-ab/v1'
    configuration = $Configuration
    runsPerMode = $Runs
    absoluteGate = [ordered]@{ p95Milliseconds = 33.4; maxMillisecondsExclusive = 100 }
    comparisonGate = [ordered]@{ maximumRegressionPercent = 20; maximumRegressionMilliseconds = 5 }
    results = $results
    selectedAutoMode = $selected
}
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($artifact)) | Out-Null
$report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $artifact -Encoding utf8
$report | ConvertTo-Json -Depth 8
