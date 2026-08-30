[CmdletBinding()]
param(
    [ValidateRange(1, 10)]
    [int] $Runs = 3,

    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [switch] $SkipBuild
)

$ErrorActionPreference = 'Stop'
$timeout = [TimeSpan]::FromMinutes(20)
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$wrapper = Join-Path $PSScriptRoot 'run-web-playwright.ps1'
$wrapperArtifacts = Join-Path $root 'Doroti/validation/web-playwright/artifacts/wrapper'
$artifact = Join-Path $root 'Doroti/validation/web-playwright/artifacts/renderer-ab.json'
$project = Join-Path $root 'DorotiDemoApp/web/DorotiDemoApp.Web.csproj'
$processArtifacts = Join-Path $root 'Doroti/validation/web-playwright/artifacts/wrapper/renderer-ab'
[IO.Directory]::CreateDirectory($processArtifacts) | Out-Null

function Invoke-AbProcess {
    param(
        [Parameter(Mandatory)] [string] $FilePath,
        [Parameter(Mandatory)] [string[]] $ArgumentList,
        [Parameter(Mandatory)] [string] $WorkingDirectory,
        [Parameter(Mandatory)] [string] $Name
    )
    $stdout = Join-Path $processArtifacts "$Name.stdout.log"
    $stderr = Join-Path $processArtifacts "$Name.stderr.log"
    $process = Start-Process -FilePath $FilePath -ArgumentList $ArgumentList `
        -WorkingDirectory $WorkingDirectory -WindowStyle Hidden -PassThru `
        -RedirectStandardOutput $stdout -RedirectStandardError $stderr
    try {
        if (-not $process.WaitForExit([int]$timeout.TotalMilliseconds)) {
            try { $process.Kill($true) } catch { }
            throw "$Name exceeded the repository 20-minute timeout."
        }
        if ($process.ExitCode -ne 0) {
            $output = if (Test-Path -LiteralPath $stdout) { Get-Content -LiteralPath $stdout -Raw } else { '' }
            $errors = if (Test-Path -LiteralPath $stderr) { Get-Content -LiteralPath $stderr -Raw } else { '' }
            throw "$Name failed with exit code $($process.ExitCode).`n$output`n$errors"
        }
    }
    finally { $process.Dispose() }
}

if (-not $SkipBuild) {
    Invoke-AbProcess -FilePath (Get-Command dotnet -ErrorAction Stop).Source `
        -ArgumentList @('build', $project, '--configuration', $Configuration, '--nologo') `
        -WorkingDirectory $root -Name 'build'
}

$results = [ordered]@{}
foreach ($mode in @('document-webgl', 'worker-direct-webgl')) {
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

$current = [double]$results['document-webgl'].medianP95Milliseconds
$candidate = [double]$results['worker-direct-webgl'].medianP95Milliseconds
$results['worker-direct-webgl'].comparisonGatePassed =
    $results['worker-direct-webgl'].absoluteGatePassed -and
    $candidate -le ($current * 1.2) -and
    $candidate -le ($current + 5)
$automatedCandidate = if ($results['worker-direct-webgl'].comparisonGatePassed) {
    'worker-direct-webgl'
} else {
    'document-webgl'
}
$report = [ordered]@{
    schemaVersion = 'doroti.web-renderer-ab/v2'
    configuration = $Configuration
    runsPerMode = $Runs
    absoluteGate = [ordered]@{ p95Milliseconds = 33.4; maxMillisecondsExclusive = 100 }
    comparisonGate = [ordered]@{ maximumRegressionPercent = 20; maximumRegressionMilliseconds = 5 }
    results = $results
    automatedQualificationCandidate = $automatedCandidate
    selectedAutoMode = 'document-webgl'
    autoModeDecision = 'unchanged until physical W7 acceptance and W8 burn-in pass'
}
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($artifact)) | Out-Null
$report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $artifact -Encoding utf8
$report | ConvertTo-Json -Depth 8
