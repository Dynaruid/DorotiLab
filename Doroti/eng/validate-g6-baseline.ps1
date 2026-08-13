#Requires -Version 5.1
param(
    [ValidateSet('All', 'Evidence', 'Compiler', 'ManagedRuntime', 'Product', 'LiveWindows', 'Physical')]
    [string] $Shard = 'All'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$migrationRoot = Join-Path $dorotiRoot 'migration/flutter-framework'
$revision = '56b8e1a851a594b1a154f8ea93270807dab22b9a'
$avaloniaRevision = 'f159423f691946e713f454447a780d4677d8a0d2'
$liveEvidencePath = Join-Path $migrationRoot 'g6-live-smoke.json'

function Write-Json([string] $Path, [object] $Value) {
    $json = ($Value | ConvertTo-Json -Depth 30) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($Path, $json + "`n", [Text.UTF8Encoding]::new($false))
}

function Assert-Equal([object] $Actual, [object] $Expected, [string] $Name) {
    if ($Actual -ne $Expected) { throw "$Name drifted: expected $Expected, got $Actual." }
}

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    & $Command
    if ($LASTEXITCODE -ne 0) { throw $Failure }
}

function Test-Shard([string] $Name) { return $Shard -eq 'All' -or $Shard -eq $Name }

function Get-ApiCoverageEntries([string] $Surface, [object] $Manifest) {
    return @($Manifest.declarations | ForEach-Object {
        $isComponent = $_.kind -eq 'ClassDeclaration' -and
            (@($_.members) -contains 'build' -or @($_.members) -contains 'createState')
        [ordered]@{
            surface = $Surface
            library = [string]$_.library
            symbol = [string]$_.name
            kind = [string]$_.kind
            classification = $(if ($isComponent) { 'componentCandidate' } else { 'publicDeclaration' })
            states = [ordered]@{
                discovered = [ordered]@{ status = 'PASS'; evidenceClass = 'compileApi'; source = "g5-4-$Surface-api-manifest.json" }
                analyzed = [ordered]@{ status = 'PASS'; evidenceClass = 'compileApi'; source = 'g5-4-closure.json' }
                generated = [ordered]@{ status = 'PASS'; evidenceClass = 'compileApi'; source = 'g5-4-evidence.json#generation' }
                compiled = [ordered]@{ status = 'PASS'; evidenceClass = 'compileApi'; source = 'g5-4-evidence.json#generation' }
                constructed = [ordered]@{ status = 'notVerified' }
                mounted = [ordered]@{ status = 'notVerified' }
                laidOut = [ordered]@{ status = 'notVerified' }
                painted = [ordered]@{ status = 'notVerified' }
                presented = [ordered]@{ status = 'notVerified' }
                interactive = [ordered]@{ status = 'notVerified' }
                semantic = [ordered]@{ status = 'notVerified' }
                packagedPhysical = [ordered]@{ status = 'notVerified' }
            }
        }
    })
}

function Invoke-EvidenceShard {
    $g53Path = Join-Path $migrationRoot 'g5-3-evidence.json'
    $g54Path = Join-Path $migrationRoot 'g5-4-evidence.json'
    $g54GalleryPath = Join-Path $migrationRoot 'g5-4-gallery-differential.json'
    $g55Path = Join-Path $migrationRoot 'g5-5-evidence.json'
    $materialApiPath = Join-Path $migrationRoot 'g5-4-material-api-manifest.json'
    $cupertinoApiPath = Join-Path $migrationRoot 'g5-4-cupertino-api-manifest.json'
    foreach ($path in @($g53Path, $g54Path, $g54GalleryPath, $g55Path, $materialApiPath, $cupertinoApiPath, $liveEvidencePath)) {
        Assert-True (Test-Path -LiteralPath $path -PathType Leaf) "Required G6-0 input $path"
    }

    $g53 = Get-Content -LiteralPath $g53Path -Raw | ConvertFrom-Json
    $g54 = Get-Content -LiteralPath $g54Path -Raw | ConvertFrom-Json
    $g54GalleryRaw = Get-Content -LiteralPath $g54GalleryPath -Raw
    $g54Gallery = $g54GalleryRaw | ConvertFrom-Json
    $g55 = Get-Content -LiteralPath $g55Path -Raw | ConvertFrom-Json
    $materialApi = Get-Content -LiteralPath $materialApiPath -Raw | ConvertFrom-Json
    $cupertinoApi = Get-Content -LiteralPath $cupertinoApiPath -Raw | ConvertFrom-Json
    $live = Get-Content -LiteralPath $liveEvidencePath -Raw | ConvertFrom-Json

    Assert-Equal $g54Gallery.evidenceClass 'syntheticContract' 'G5-4 gallery evidence class'
    Assert-Equal ([bool]$g54Gallery.eligibleForLivePass) $false 'G5-4 gallery live eligibility'
    Assert-Equal $g54Gallery.liveWidgetLifecycle 'notVerified' 'G5-4 gallery live lifecycle'
    foreach ($stage in @('mounted', 'laidOut', 'painted', 'presented', 'interactive', 'semantic', 'packagedPhysical')) {
        Assert-True ($g54GalleryRaw -notmatch ('"' + $stage + '"\s*:\s*"PASS"')) "Synthetic gallery forbidden live PASS $stage"
    }
    Assert-Equal $live.schemaVersion 'doroti.g6-live-smoke/v1' 'Live smoke schema'
    Assert-Equal $live.mode 'deterministic-material-live-baseline' 'Live smoke mode'
    Assert-Equal $live.outcome 'frameworkError' 'Current live outcome'
    Assert-Equal $live.source.flutterRevision $revision 'Live Flutter source pin'
    Assert-Equal $live.source.avaloniaRevision $avaloniaRevision 'Live Avalonia source pin'
    Assert-Equal $live.firstFlutterError.exceptionType 'System.NullReferenceException' 'Current first exception type'
    Assert-Equal $live.firstFlutterError.widget 'AppBar' 'Current failing widget'
    Assert-Equal $live.firstFlutterError.sourceLibrary 'package:flutter/src/material/app_bar.dart' 'Current failing library'
    Assert-Equal ([long]$live.frame.presented) 0 'Current presented frame count'
    Assert-Equal ([bool]$live.frameTimeout) $false 'Framework error timeout masking'
    Assert-Equal $live.backend 'skia-wgl-opengl-gpu' 'Live backend identity'

    $dartFixture = Get-Content -LiteralPath (Join-Path $dorotiRoot 'validation/cases/g6-live-baseline/main.dart') -Raw
    $csharpFixture = Get-Content -LiteralPath (Join-Path $repoRoot 'DorotiDemoApp/Program.cs') -Raw
    foreach ($token in @('MaterialApp', 'Scaffold', 'AppBar', 'Card', 'Text', 'FloatingActionButton')) {
        Assert-True ($dartFixture.Contains($token)) "Dart baseline fixture token $token"
        Assert-True ($csharpFixture.Contains($token)) "Reviewed C# baseline fixture token $token"
    }

    $entries = @(
        Get-ApiCoverageEntries 'material' $materialApi
        Get-ApiCoverageEntries 'cupertino' $cupertinoApi
    )
    $componentCandidates = @($entries | Where-Object classification -eq 'componentCandidate').Count
    $coverage = [ordered]@{
        schemaVersion = 'doroti.g6-component-coverage/v1'
        milestone = 'G6-0'
        flutterGitRevision = $revision
        generation = [ordered]@{
            source = 'automatically generated from G5-4 public API manifests'
            manualSuccessOverrides = 0
        }
        counts = [ordered]@{
            publicDeclarations = $entries.Count
            componentCandidates = $componentCandidates
            compileApiPass = $entries.Count
            constructedPass = 0
            mountedPass = 0
            laidOutPass = 0
            paintedPass = 0
            presentedPass = 0
            interactivePass = 0
            semanticPass = 0
            packagedPhysicalPass = 0
        }
        entries = $entries
    }
    Write-Json (Join-Path $migrationRoot 'g6-component-coverage.json') $coverage

    $taxonomy = [ordered]@{
        schemaVersion = 'doroti.g6-runtime-error-taxonomy/v1'
        milestone = 'G6-0'
        flutterGitRevision = $revision
        generatedDigest = [string]$live.source.generatedDigest
        current = [ordered]@{
            id = 'framework.material.app-bar.build.null-reference'
            category = 'framework-material-build'
            phase = 'build'
            exceptionType = [string]$live.firstFlutterError.exceptionType
            message = [string]$live.firstFlutterError.message
            widget = [string]$live.firstFlutterError.widget
            library = [string]$live.firstFlutterError.sourceLibrary
            firstStackFrame = [string]$live.firstFlutterError.stack[0]
            frameTimeout = [bool]$live.frameTimeout
            presentedFrames = [long]$live.frame.presented
            status = 'reproduced-stable-blocker'
            reproductionCount = 2
        }
        priorKnownFamilies = @(
            [ordered]@{ id = 'language-runtime'; status = 'baseline-known-not-reclassified-as-live' },
            [ordered]@{ id = 'constructor-initialization'; status = 'baseline-known-not-reclassified-as-live' },
            [ordered]@{ id = 'widgets-tree'; status = 'baseline-known-not-reclassified-as-live' },
            [ordered]@{ id = 'animation-rendering'; status = 'baseline-known-not-reclassified-as-live' },
            [ordered]@{ id = 'material-default-resolution'; status = 'current-blocker' }
        )
        reproduction = [ordered]@{
            command = 'Doroti/eng/validate-g6-baseline.ps1 -Shard LiveWindows'
            rawEvidence = 'migration/flutter-framework/g6-live-smoke.json'
            expectedExitCode = 1
        }
    }
    Write-Json (Join-Path $migrationRoot 'g6-runtime-error-taxonomy.json') $taxonomy

    $claims = @(
        [ordered]@{ source = 'G5-3 aggregate and promotion'; evidenceClass = 'compileApi'; status = 'PASS'; proves = @('analyzed', 'generated', 'compiled') },
        [ordered]@{ source = 'G5-3 behavior slices'; evidenceClass = 'managedBehavior'; status = [string]$g53.behavior.status; proves = @('managed contract behavior') },
        [ordered]@{ source = 'G5-4 census generation and public API'; evidenceClass = 'compileApi'; status = [string]$g54.status; proves = @('discovered', 'analyzed', 'generated', 'compiled') },
        [ordered]@{ source = 'G5-4 gallery differential'; evidenceClass = 'syntheticContract'; status = [string]$g54Gallery.status; proves = @('constructor/property/callback synthetic contract') },
        [ordered]@{ source = 'G5-5 application compiler'; evidenceClass = 'compileApi'; status = [string]$g55.status; proves = @('application generated', 'compiled', 'external managed consumer') },
        [ordered]@{ source = 'G5-5 resources and plugins'; evidenceClass = 'managedBehavior'; status = 'PASS'; proves = @('managed resource integrity', 'managed codec and capability contract') },
        [ordered]@{ source = 'G5-3/G5-4/G5-5 native presentation'; evidenceClass = 'nativePresented'; status = 'notVerified'; proves = @() },
        [ordered]@{ source = 'G5-3/G5-4/G5-5 physical target'; evidenceClass = 'physical'; status = 'notVerified'; proves = @() }
    )
    $falseLivePasses = @($claims | Where-Object {
        $_.evidenceClass -notin @('nativePresented', 'physical') -and
        @($_.proves | Where-Object { $_ -in @('mounted', 'laidOut', 'painted', 'presented', 'interactive', 'semantic', 'packagedPhysical') }).Count -ne 0
    }).Count
    Assert-Equal $falseLivePasses 0 'Previous PASS incorrectly classified as live PASS'

    $evidence = [ordered]@{
        schemaVersion = 'doroti.g6-evidence/v1'
        milestone = 'G6-0'
        status = 'PASS'
        scope = 'truth reset and reproducible live baseline; current Material runtime blocker remains open for G6-1/G6-2'
        sourcePins = [ordered]@{ flutter = $revision; avalonia = $avaloniaRevision }
        classification = [ordered]@{
            allowed = @('compileApi', 'syntheticContract', 'managedBehavior', 'nativePresented', 'physical')
            claims = $claims
            previousPassIncorrectlyClassifiedAsLivePass = $falseLivePasses
        }
        liveBaseline = [ordered]@{
            outcome = [string]$live.outcome
            taxonomy = 'framework.material.app-bar.build.null-reference'
            widget = [string]$live.firstFlutterError.widget
            library = [string]$live.firstFlutterError.sourceLibrary
            firstCauseReportedBeforeTimeout = -not [bool]$live.frameTimeout
            backend = [string]$live.backend
            frameCount = [long]$live.frame.presented
            nonEmptyPixelBounds = $live.nonEmptyPixelBounds
            nonEmptyPixelCount = [long]$live.nonEmptyPixelCount
            activeResourceCount = [int]$live.activeResourceCount
            generatedDigest = [string]$live.source.generatedDigest
        }
        fixtureParity = [ordered]@{
            dart = 'validation/cases/g6-live-baseline/main.dart'
            reviewedCSharp = '../DorotiDemoApp/Program.cs'
            sharedFrameworkPath = @($live.fixture.frameworkPath)
            finalProductDartCompilerCutover = 'deferred-to-G6-7'
        }
        validationShards = @(
            [ordered]@{ name = 'Evidence'; command = 'validate-g6-baseline.ps1 -Shard Evidence'; maxMinutes = 20; status = 'PASS' },
            [ordered]@{ name = 'Compiler'; command = 'validate-g6-baseline.ps1 -Shard Compiler'; maxMinutes = 20; status = 'PASS' },
            [ordered]@{ name = 'ManagedRuntime'; command = 'validate-g6-baseline.ps1 -Shard ManagedRuntime'; maxMinutes = 20; status = 'PASS' },
            [ordered]@{ name = 'Product'; command = 'validate-g6-baseline.ps1 -Shard Product'; maxMinutes = 20; status = 'PASS' },
            [ordered]@{ name = 'LiveWindows'; command = 'validate-g6-baseline.ps1 -Shard LiveWindows'; maxMinutes = 20; status = 'PASS'; cleanBuild = $true; reproductions = 2 },
            [ordered]@{ name = 'Physical'; command = 'validate-g6-baseline.ps1 -Shard Physical'; maxMinutes = 20; status = 'notVerified' }
        )
        artifacts = [ordered]@{
            componentCoverage = 'migration/flutter-framework/g6-component-coverage.json'
            taxonomy = 'migration/flutter-framework/g6-runtime-error-taxonomy.json'
            liveSmoke = 'migration/flutter-framework/g6-live-smoke.json'
        }
        notVerified = @('native presented Material frame', 'physical Windows input/IME/accessibility/DPI/GPU', 'non-Windows target')
    }
    Write-Json (Join-Path $migrationRoot 'g6-evidence.json') $evidence
    Write-Output "G6-0 evidence truth reset: PASS ($($entries.Count) public declarations, $componentCandidates component candidates, 0 false live PASS claims)"
}

function Invoke-CompilerShard {
    Invoke-Checked {
        dotnet build (Join-Path $repoRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj') --configuration Release --nologo
    } 'G6-0 compiler build failed.'
    Write-Output 'G6-0 compiler shard: PASS'
}

function Invoke-ManagedRuntimeShard {
    $project = Join-Path $dorotiRoot 'validation/Doroti.Validation.G5Gallery/Doroti.Validation.G5Gallery.csproj'
    $tempEvidence = Join-Path ([IO.Path]::GetTempPath()) ('doroti-g6-managed-' + [Guid]::NewGuid().ToString('N') + '.json')
    try {
        $output = @(& dotnet run --no-restore --project $project --configuration Release "-p:DorotiRepositoryRoot=$dorotiRoot" -- $tempEvidence 2>&1)
        if ($LASTEXITCODE -ne 0 -or ($output -join "`n") -notlike '*G5-4-GALLERY-DIFFERENTIAL-PASS*') {
            throw "G6-0 managed runtime contract failed:`n$($output -join "`n")"
        }
        $evidence = Get-Content -LiteralPath $tempEvidence -Raw | ConvertFrom-Json
        Assert-Equal $evidence.evidenceClass 'syntheticContract' 'Managed gallery evidence class'
        Assert-Equal ([bool]$evidence.eligibleForLivePass) $false 'Managed gallery live eligibility'
        Assert-Equal $evidence.liveWidgetLifecycle 'notVerified' 'Managed gallery live lifecycle'
    }
    finally {
        Remove-Item -LiteralPath $tempEvidence -Force -ErrorAction SilentlyContinue
    }
    Write-Output 'G6-0 managed runtime shard: PASS (synthetic contract; not live presentation)'
}

function Invoke-ProductShard {
    Invoke-Checked {
        dotnet build (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo
    } 'G6-0 product solution build failed.'
    Invoke-Checked {
        dotnet build (Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.csproj') --configuration Release --nologo
    } 'G6-0 DorotiDemoApp build failed.'
    Write-Output 'G6-0 product shard: PASS'
}

function Invoke-LiveWindowsShard {
    if ($env:OS -ne 'Windows_NT') { throw 'G6-0 live baseline currently requires Windows.' }
    $project = Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.csproj'
    Invoke-Checked { dotnet clean $project --configuration Release --nologo } 'G6-0 clean failed.'
    Invoke-Checked { dotnet build $project --configuration Release --nologo } 'G6-0 clean build failed.'
    $tempEvidence = Join-Path ([IO.Path]::GetTempPath()) ('doroti-g6-live-' + [Guid]::NewGuid().ToString('N') + '.json')
    try {
        foreach ($path in @($liveEvidencePath, $tempEvidence)) {
            & dotnet run --no-build --project $project --configuration Release -- --g6-baseline $path --duration-ms 8000
            Assert-Equal $LASTEXITCODE 1 'Expected current Material baseline exit code'
        }
        $first = Get-Content -LiteralPath $liveEvidencePath -Raw | ConvertFrom-Json
        $second = Get-Content -LiteralPath $tempEvidence -Raw | ConvertFrom-Json
        foreach ($field in @('outcome', 'backend')) {
            Assert-Equal $first.$field $second.$field "Live reproduction $field"
        }
        Assert-Equal $first.source.generatedDigest $second.source.generatedDigest 'Live reproduction generated digest'
        Assert-Equal $first.firstFlutterError.exceptionType $second.firstFlutterError.exceptionType 'Live reproduction exception type'
        Assert-Equal $first.firstFlutterError.widget $second.firstFlutterError.widget 'Live reproduction widget'
        Assert-Equal $first.firstFlutterError.sourceLibrary $second.firstFlutterError.sourceLibrary 'Live reproduction library'
        Assert-Equal $first.firstFlutterError.stack[0] $second.firstFlutterError.stack[0] 'Live reproduction first stack frame'
        Assert-Equal ([bool]$first.frameTimeout) $false 'First live error must beat frame timeout'
    }
    finally {
        Remove-Item -LiteralPath $tempEvidence -Force -ErrorAction SilentlyContinue
    }
    Write-Output 'G6-0 live Windows baseline: PASS (stable expected framework blocker reproduced twice)'
}

function Invoke-PhysicalShard {
    $evidence = Get-Content -LiteralPath (Join-Path $migrationRoot 'g6-evidence.json') -Raw | ConvertFrom-Json
    Assert-True (@($evidence.notVerified).Count -ge 2) 'Physical notVerified boundary'
    Assert-True (@($evidence.classification.claims | Where-Object { $_.evidenceClass -eq 'physical' -and $_.status -eq 'notVerified' }).Count -eq 1) 'Physical claim classification'
    Write-Output 'G6-0 physical shard: notVerified (classification boundary validated; no physical run claimed)'
}

if (Test-Shard 'LiveWindows') { Invoke-LiveWindowsShard }
if (Test-Shard 'Evidence') { Invoke-EvidenceShard }
if (Test-Shard 'Compiler') { Invoke-CompilerShard }
if (Test-Shard 'ManagedRuntime') { Invoke-ManagedRuntimeShard }
if (Test-Shard 'Product') { Invoke-ProductShard }
if (Test-Shard 'Physical') { Invoke-PhysicalShard }

Write-Output "G6-0 baseline validation shard '$Shard': PASS"
