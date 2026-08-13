#Requires -Version 5.1
param(
    [switch] $SkipStage,
    [switch] $SkipCompile
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$migrationRoot = Join-Path $dorotiRoot 'migration/flutter-framework'
$candidateBase = Join-Path $dorotiRoot 'migration/generated-candidates'
$reviewedRoot = Join-Path $candidateBase 'g5-4-reviewed'
$revision = '56b8e1a851a594b1a154f8ea93270807dab22b9a'

function Write-Json([string] $Path, [object] $Value) {
    $json = ($Value | ConvertTo-Json -Depth 20) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($Path, $json + "`n", [Text.UTF8Encoding]::new($false))
}

function Assert-Equal([object] $Actual, [object] $Expected, [string] $Name) {
    if ($Actual -ne $Expected) { throw "$Name drifted: expected $Expected, got $Actual." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    & $Command
    if ($LASTEXITCODE -ne 0) { throw $Failure }
}

& (Join-Path $PSScriptRoot 'prepare-g5-4.ps1') | Write-Output
if (-not $SkipCompile) {
    $compilerProject = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj'
    $compiler = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/bin/Release/net10.0/Doroti.DartToCSharp.dll'
    Invoke-Checked { dotnet build $compilerProject --configuration Release --nologo } 'G5-4 compiler build failed.'
    $compileIndex = Get-Content -LiteralPath (Join-Path $migrationRoot 'g5-4-batches.json') -Raw | ConvertFrom-Json
    $compileBatches = @($compileIndex.batches)
    for ($offset = 0; $offset -lt $compileBatches.Count; $offset += 3) {
        $last = [Math]::Min($offset + 2, $compileBatches.Count - 1)
        $wave = @($compileBatches[$offset..$last])
        Write-Output "G5-4 compiler wave: $(@($wave.id) -join ', ')"
        $jobs = @($wave | ForEach-Object {
            $batch = $_
            $manifest = Join-Path $dorotiRoot ([string]$batch.manifest)
            $candidate = Join-Path $candidateBase ('g5-4-' + ([string]$batch.id).ToLowerInvariant())
            Start-Job -ScriptBlock {
                param($Compiler, $Manifest, $Candidate, $BatchId, $WorkingDirectory)
                Set-Location -LiteralPath $WorkingDirectory
                $output = (& dotnet $Compiler --manifest $Manifest --output $Candidate --parallelism 3 2>&1 | Out-String)
                [pscustomobject]@{ id = $BatchId; exitCode = $LASTEXITCODE; output = $output }
            } -ArgumentList $compiler, $manifest, $candidate, ([string]$batch.id), $repoRoot
        })
        try {
            $jobs | Wait-Job | Out-Null
            foreach ($job in $jobs) {
                $result = Receive-Job $job
                if ([int]$result.exitCode -ne 0) { throw "G5-4 compiler batch failed: $($result.id).`n$($result.output)" }
                Write-Output "G5-4 compiler batch: PASS ($($result.id))"
            }
        }
        finally { $jobs | Remove-Job -Force -ErrorAction SilentlyContinue }
    }
}
if (-not $SkipStage) { & (Join-Path $PSScriptRoot 'stage-g5-4.ps1') | Write-Output }

$closurePath = Join-Path $migrationRoot 'g5-4-closure.json'
$batchIndexPath = Join-Path $migrationRoot 'g5-4-batches.json'
$fullSelectionPath = Join-Path $dorotiRoot 'migration/selections/g5-4-full-framework.json'
$stagePath = Join-Path $reviewedRoot 'g5-4-stage.json'
$reviewPath = Join-Path $reviewedRoot 'g5-4-reviewed-adaptations.json'
foreach ($required in @($closurePath, $batchIndexPath, $fullSelectionPath, $stagePath, $reviewPath)) {
    if (-not (Test-Path -LiteralPath $required -PathType Leaf)) { throw "G5-4 validation input is missing: $required" }
}

$closure = Get-Content -LiteralPath $closurePath -Raw | ConvertFrom-Json
$batchIndex = Get-Content -LiteralPath $batchIndexPath -Raw | ConvertFrom-Json
$fullSelection = Get-Content -LiteralPath $fullSelectionPath -Raw | ConvertFrom-Json
$stage = Get-Content -LiteralPath $stagePath -Raw | ConvertFrom-Json
$review = Get-Content -LiteralPath $reviewPath -Raw | ConvertFrom-Json
$flutterRoot = Join-Path $repoRoot 'flutter-master'
$flutterLibRoot = Join-Path $flutterRoot 'packages/flutter/lib'
$sourceLock = Get-Content -LiteralPath (Join-Path $dorotiRoot 'validation/flutter-source.lock.json') -Raw | ConvertFrom-Json
$sourceClosureFiles = @(Get-ChildItem -LiteralPath $flutterLibRoot -Recurse -File -Filter '*.dart').Count
$sourceHashMismatches = @($closure.libraries | Where-Object {
    $sourceFile = Join-Path $flutterLibRoot ([string]$_.path)
    -not (Test-Path -LiteralPath $sourceFile -PathType Leaf) -or
        (Get-FileHash -LiteralPath $sourceFile -Algorithm SHA256).Hash.ToLowerInvariant() -cne [string]$_.sha256
})

Assert-Equal ([string]$closure.flutterGitRevision) $revision 'Flutter pin'
Assert-Equal ([string]$sourceLock.upstreamRevision) $revision 'Flutter source lock revision'
Assert-Equal @($closure.roots).Count 13 'Public root count'
Assert-Equal $sourceClosureFiles 695 'Pinned Flutter public graph source file count'
Assert-Equal ([int]$sourceLock.dartFileCount) 695 'Flutter source lock file count'
Assert-Equal $sourceHashMismatches.Count 0 'Closure/current-source SHA mismatch count'
Assert-Equal @($closure.libraries).Count 694 'Resolved product library count'
Assert-Equal @($fullSelection.inputs).Count 694 'Full selection resolved input count'
Assert-Equal ([int]$closure.coverage.declarations) 5355 'Closure declaration count'
Assert-Equal ([int]$closure.coverage.analyzerErrors) 0 'Analyzer error count'
Assert-Equal ([int]$closure.coverage.unclassifiedDeclarations) 0 'Unclassified declaration count'
Assert-Equal ([int]$closure.coverage.unclassifiedMembers) 0 'Unclassified member count'
Assert-Equal ([int]$closure.coverage.unsupportedBlockers) 0 'Unsupported blocker count'
Assert-Equal ([int]$closure.coverage.dispositions.'reviewed-predecessor'.declarations) 3264 'Predecessor disposition count'
Assert-Equal ([int]$closure.coverage.dispositions.generated.declarations) 2091 'Generated disposition count'
Assert-Equal @($closure.libraries | Where-Object { [string]::IsNullOrWhiteSpace([string]$_.owner) }).Count 0 'Unowned library count'
Assert-Equal ([int]$batchIndex.counts.batches) 9 'Batch count'
Assert-Equal ([int]$batchIndex.counts.productLibraries) 252 'G5-4 product library count'
Assert-Equal ([int]$batchIndex.counts.productDeclarations) 2091 'G5-4 product declaration count'
Assert-Equal ([int]$stage.generatedFiles) 249 'Staged generated file count'
Assert-Equal ([int]$review.declarationOrFileRemovals) 0 'Reviewed declaration/file removal count'

$outputByPath = @{}
$batchEvidence = [Collections.Generic.List[object]]::new()
foreach ($batch in @($batchIndex.batches)) {
    $candidate = Join-Path $candidateBase ('g5-4-' + ([string]$batch.id).ToLowerInvariant())
    $report = Get-Content -LiteralPath (Join-Path $candidate 'converter-report.json') -Raw | ConvertFrom-Json
    $coverage = Get-Content -LiteralPath (Join-Path $candidate 'framework-coverage.json') -Raw | ConvertFrom-Json
    $errors = @($report.diagnostics | Where-Object severity -eq 'error')
    Assert-Equal ([bool]$report.success) $true "$($batch.id) compiler success"
    Assert-Equal $errors.Count 0 "$($batch.id) compiler errors"
    Assert-Equal ([int]$coverage.unclassifiedAstNodeCount) 0 "$($batch.id) unclassified AST nodes"
    Assert-Equal ([int]$coverage.silentOmissionCount) 0 "$($batch.id) silent omissions"
    foreach ($output in @($report.outputs)) {
        $sourcePath = ([string]$output.input).Replace('\', '/')
        $marker = '/packages/flutter/lib/'
        $markerIndex = $sourcePath.IndexOf($marker, [StringComparison]::Ordinal)
        if ($markerIndex -lt 0) { throw "Unexpected G5-4 compiler input: $sourcePath" }
        $outputByPath[$sourcePath.Substring($markerIndex + $marker.Length)] = $output
    }
    $batchEvidence.Add([ordered]@{
        id = [string]$batch.id
        libraries = [int]$batch.libraries
        declarations = [int]$batch.declarations
        outputs = @($report.outputs).Count
        diagnostics = @($report.diagnostics).Count
        errors = $errors.Count
        unclassified = [int]$coverage.unclassifiedAstNodeCount
        silentOmissions = [int]$coverage.silentOmissionCount
        compilerIdentity = $report.identity
    })
}

$libraryByPath = @{}
foreach ($library in @($closure.libraries)) { $libraryByPath[[string]$library.path] = $library }

function New-ApiManifest([string] $Surface) {
    $rootPath = "$Surface.dart"
    $prefix = "src/$Surface/"
    $rootLibrary = $libraryByPath[$rootPath]
    if ($null -eq $rootLibrary) { throw "G5-4 API root is absent: $rootPath" }
    $exported = @($rootLibrary.dependencies | Where-Object { ([string]$_).StartsWith($prefix, [StringComparison]::Ordinal) } | Sort-Object -Unique)
    $entries = [Collections.Generic.List[object]]::new()
    $missing = [Collections.Generic.List[object]]::new()
    $extra = [Collections.Generic.List[object]]::new()
    foreach ($libraryPath in $exported) {
        $library = $libraryByPath[[string]$libraryPath]
        if ($null -eq $library) { throw "$rootPath export is absent from closure: $libraryPath" }
        if (-not $outputByPath.ContainsKey([string]$libraryPath)) {
            if (@($library.declarations).Count -eq 0) { continue }
            throw "$rootPath export is absent from compiler outputs: $libraryPath"
        }
        $actual = @($outputByPath[[string]$libraryPath].symbols | Where-Object { -not ([string]$_).StartsWith('_', [StringComparison]::Ordinal) })
        $expected = @($library.declarations | Where-Object { -not ([string]$_.name).StartsWith('_', [StringComparison]::Ordinal) })
        foreach ($declaration in $expected) {
            $entry = [ordered]@{
                library = "package:flutter/$libraryPath"
                name = [string]$declaration.name
                kind = [string]$declaration.kind
                elementId = [string]$declaration.canonicalElementId
                members = @($declaration.members | ForEach-Object { [string]$_ })
            }
            $entries.Add($entry)
            if ($actual -notcontains $entry.name) { $missing.Add($entry) }
        }
        foreach ($name in $actual | Where-Object { $_ -notin @($expected.name) }) {
            $extra.Add([ordered]@{ library = "package:flutter/$libraryPath"; name = [string]$name })
        }
    }
    $manifest = [ordered]@{
        schemaVersion = 'doroti.g5-4-api-manifest/v1'
        milestone = 'G5-4'
        flutterGitRevision = $revision
        surface = $Surface
        root = "package:flutter/$rootPath"
        counts = [ordered]@{
            exportedLibraries = $exported.Count
            declarationOccurrences = $entries.Count
            missing = $missing.Count
            extra = $extra.Count
            diff = $missing.Count + $extra.Count
        }
        declarations = @($entries)
        missing = @($missing)
        extra = @($extra)
    }
    Write-Json (Join-Path $migrationRoot "g5-4-$Surface-api-manifest.json") $manifest
    return $manifest
}

$materialApi = New-ApiManifest 'material'
$cupertinoApi = New-ApiManifest 'cupertino'
Assert-Equal ([int]$materialApi.counts.diff) 0 'Material API manifest diff'
Assert-Equal ([int]$cupertinoApi.counts.diff) 0 'Cupertino API manifest diff'

$buildLogPath = Join-Path $dorotiRoot 'artifacts/g5-4-reviewed-build.log'
$buildLines = @(& dotnet build (Join-Path $reviewedRoot 'Doroti.Generated.Framework.slnx') --configuration Release --nologo "-p:DorotiRepositoryRoot=$dorotiRoot" 2>&1)
$buildExitCode = $LASTEXITCODE
[IO.File]::WriteAllLines($buildLogPath, @($buildLines | ForEach-Object { [string]$_ }), [Text.UTF8Encoding]::new($false))
if ($buildExitCode -ne 0) { throw "G5-4 reviewed candidate build failed. See $buildLogPath" }
$buildWarnings = @($buildLines | Where-Object { [string]$_ -match ':\s+warning\s+[A-Z]+\d+' }).Count
$buildErrors = @($buildLines | Where-Object { [string]$_ -match ':\s+error\s+[A-Z]+\d+' }).Count
Assert-Equal $buildWarnings 0 'Reviewed build warning count'
Assert-Equal $buildErrors 0 'Reviewed build error count'

$galleryEvidencePath = Join-Path $migrationRoot 'g5-4-gallery-differential.json'
$galleryOutput = @(& dotnet run --project (Join-Path $dorotiRoot 'validation/Doroti.Validation.G5Gallery/Doroti.Validation.G5Gallery.csproj') --configuration Release "-p:DorotiRepositoryRoot=$dorotiRoot" -- $galleryEvidencePath 2>&1)
if ($LASTEXITCODE -ne 0 -or ($galleryOutput -join "`n") -notlike '*G5-4-GALLERY-DIFFERENTIAL-PASS*') {
    throw "G5-4 gallery differential failed:`n$($galleryOutput -join "`n")"
}
$galleryEvidence = Get-Content -LiteralPath $galleryEvidencePath -Raw | ConvertFrom-Json
if ($galleryEvidence.status -ne 'PASS') { throw 'G5-4 gallery evidence is not PASS.' }
if ($galleryEvidence.evidenceClass -ne 'syntheticContract' -or
    [bool]$galleryEvidence.eligibleForLivePass -or
    $galleryEvidence.liveWidgetLifecycle -ne 'notVerified') {
    throw 'G5-4 gallery evidence must remain syntheticContract and ineligible for live PASS.'
}

$auditRoots = @((Join-Path $reviewedRoot 'projects'), (Join-Path $dorotiRoot 'validation/Doroti.Validation.G5Gallery'))
$auditFiles = @($auditRoots | ForEach-Object {
    Get-ChildItem -LiteralPath $_ -Recurse -File | Where-Object { $_.Extension -in @('.cs', '.csproj', '.props', '.targets', '.xaml') }
})
$xamlFiles = @($auditFiles | Where-Object Extension -eq '.xaml')
$avaloniaMatches = @($auditFiles | Select-String -Pattern 'Avalonia\.Controls|Avalonia\.Themes|Avalonia\.Markup\.Xaml|x:Class' -CaseSensitive:$false)
Assert-Equal $xamlFiles.Count 0 'G5-4 XAML file dependency count'
Assert-Equal $avaloniaMatches.Count 0 'G5-4 Avalonia Controls/theme/XAML reference count'

$evidence = [ordered]@{
    schemaVersion = 'doroti.g5-4-evidence/v1'
    milestone = 'G5-4'
    capturedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
    status = 'verified-automated-current-machine'
    flutterGitRevision = $revision
    census = [ordered]@{
        publicRoots = @($closure.roots).Count
        sourceClosureFiles = $sourceClosureFiles
        resolvedLibraries = @($closure.libraries).Count
        declarations = [int]$closure.coverage.declarations
        members = [int]$closure.coverage.members
        analyzerErrors = [int]$closure.coverage.analyzerErrors
        unclassifiedDeclarations = [int]$closure.coverage.unclassifiedDeclarations
        unclassifiedMembers = [int]$closure.coverage.unclassifiedMembers
        unsupportedBlockers = [int]$closure.coverage.unsupportedBlockers
        currentSourceHashMismatches = $sourceHashMismatches.Count
        unownedLibraries = 0
        censusCoveragePercent = 100
    }
    generation = [ordered]@{
        batches = @($batchEvidence)
        productLibraries = [int]$batchIndex.counts.productLibraries
        productDeclarations = [int]$batchIndex.counts.productDeclarations
        stagedGeneratedFiles = [int]$stage.generatedFiles
        reviewedAdaptedFiles = [int]$review.changedFiles
        declarationOrFileRemovals = [int]$review.declarationOrFileRemovals
        buildWarnings = $buildWarnings
        buildErrors = $buildErrors
    }
    publicApi = [ordered]@{
        material = [ordered]@{ exportedLibraries = [int]$materialApi.counts.exportedLibraries; declarations = [int]$materialApi.counts.declarationOccurrences; diff = [int]$materialApi.counts.diff }
        cupertino = [ordered]@{ exportedLibraries = [int]$cupertinoApi.counts.exportedLibraries; declarations = [int]$cupertinoApi.counts.declarationOccurrences; diff = [int]$cupertinoApi.counts.diff }
    }
    gallery = [ordered]@{
        status = [string]$galleryEvidence.status
        evidenceClass = [string]$galleryEvidence.evidenceClass
        eligibleForLivePass = [bool]$galleryEvidence.eligibleForLivePass
        liveWidgetLifecycle = [string]$galleryEvidence.liveWidgetLifecycle
        shell = [string]$galleryEvidence.shell
        dimensions = @($galleryEvidence.dimensions)
        evidence = 'migration/flutter-framework/g5-4-gallery-differential.json'
    }
    dependencyAudit = [ordered]@{
        scannedFiles = $auditFiles.Count
        avaloniaControlsThemeXamlReferences = $avaloniaMatches.Count
        xamlFiles = $xamlFiles.Count
    }
    evidence = [ordered]@{
        closure = 'migration/flutter-framework/g5-4-closure.json'
        batches = 'migration/flutter-framework/g5-4-batches.json'
        materialApi = 'migration/flutter-framework/g5-4-material-api-manifest.json'
        cupertinoApi = 'migration/flutter-framework/g5-4-cupertino-api-manifest.json'
        gallery = 'migration/flutter-framework/g5-4-gallery-differential.json'
        reviewedBuildLog = 'artifacts/g5-4-reviewed-build.log'
    }
    notVerified = @('physical Windows IME', 'physical accessibility', 'physical sustained GPU', 'physical cross-monitor DPI')
    deferredTo = 'G5-8 DorotiDemoApp'
}
Write-Json (Join-Path $migrationRoot 'g5-4-evidence.json') $evidence

Write-Output ($buildLines | Select-Object -Last 8)
Write-Output $galleryOutput
Write-Output 'G5-4 census/API/gallery/dependency validation: PASS'
Write-Output "Evidence: $(Join-Path $migrationRoot 'g5-4-evidence.json')"
