#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$matrixPath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/framework-parity-matrix.json'
$baselinePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/baseline-evidence.json'
$targetEvidencePath = Join-Path $dorotiRoot 'validation/evidence/app-targets-evidence.json'
$allowedDispositions = @('implemented', 'adapted', 'explicitUnsupported', 'notVerified')

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Read-Json([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "JSON artifact exists: $Path"
    return Get-Content -Raw -LiteralPath $Path | ConvertFrom-Json
}

function Resolve-RepositoryPath([string] $RelativePath) {
    return [IO.Path]::GetFullPath((Join-Path $repositoryRoot ($RelativePath.Replace('/', '\'))))
}

function Resolve-FlutterPath([string] $RelativePath) {
    return [IO.Path]::GetFullPath((Join-Path $flutterRoot ($RelativePath.Replace('/', '\'))))
}

function Get-OptionalProperty($Object, [string] $Name) {
    if ($null -eq $Object) { return $null }
    $property = $Object.PSObject.Properties[$Name]
    if ($null -eq $property) { return $null }
    return $property.Value
}

function Get-CodeWithoutComments([string] $Path) {
    $text = Get-Content -Raw -LiteralPath $Path
    $text = [regex]::Replace($text, '(?s)/\*.*?\*/', '')
    return [regex]::Replace($text, '(?m)//.*$', '')
}

function Get-LineNumber([string] $Text, [int] $Index) {
    return (($Text.Substring(0, $Index) -split "`n").Count)
}

function New-Metric($Value, [string] $Source, [string] $Reason) {
    if ($null -ne $Value) {
        return [ordered]@{ status = 'observed'; value = $Value; source = $Source }
    }
    return [ordered]@{ status = 'notVerified'; value = $null; reason = $Reason }
}

Assert-True (Test-Path -LiteralPath $flutterRoot -PathType Container) 'pinned Flutter checkout exists'
$matrix = Read-Json $matrixPath
Assert-True ($matrix.schemaVersion -eq 'doroti.flutter-conformance-parity-matrix/v1') 'FCR-0 matrix schema'
Assert-True (@($matrix.allowedDispositions).Count -gt 0) 'FCR-0 disposition vocabulary'
Assert-True (@($matrix.allowedDispositions | Where-Object { $_ -notin $allowedDispositions }).Count -eq 0) 'FCR-0 disposition vocabulary is closed'

$flutterRevision = (& git -C $flutterRoot rev-parse HEAD).Trim()
Assert-True ($LASTEXITCODE -eq 0) 'pinned Flutter revision lookup'
Assert-True ($flutterRevision -eq [string]$matrix.flutterRevision) "Flutter revision pin: expected $($matrix.flutterRevision), got $flutterRevision"

$sourceSlices = @($matrix.sourceSlices)
$dependencyContracts = @($matrix.dependencyContracts)
$patternPolicies = @($matrix.patternPolicies)
Assert-True ($sourceSlices.Count -gt 0) 'FCR-0 source slice registration'
Assert-True ($dependencyContracts.Count -gt 0) 'FCR-0 dependency contract registration'
Assert-True (($sourceSlices.id | Sort-Object -Unique).Count -eq $sourceSlices.Count) 'source slice ids are unique'
Assert-True (($sourceSlices.flutterSource | Sort-Object -Unique).Count -eq $sourceSlices.Count) 'source slice paths are unique'
Assert-True (($dependencyContracts.id | Sort-Object -Unique).Count -eq $dependencyContracts.Count) 'dependency contract ids are unique'

$dependenciesById = @{}
foreach ($dependency in $dependencyContracts) {
    Assert-True (-not [string]::IsNullOrWhiteSpace([string]$dependency.id)) 'dependency id is present'
    Assert-True (-not [string]::IsNullOrWhiteSpace([string]$dependency.owner)) "dependency owner: $($dependency.id)"
    Assert-True ([string]$dependency.disposition -in $allowedDispositions) "dependency disposition: $($dependency.id)"
    Assert-True (@($dependency.productPaths).Count -gt 0) "dependency product closure: $($dependency.id)"
    Assert-True (@($dependency.hostConsumers).Count -gt 0) "dependency host consumer: $($dependency.id)"
    Assert-True (@($dependency.testEvidence).Count -gt 0) "dependency test/evidence: $($dependency.id)"
    foreach ($path in @($dependency.productPaths) + @($dependency.hostConsumers) + @($dependency.testEvidence)) {
        Assert-True (Test-Path -LiteralPath (Resolve-RepositoryPath ([string]$path)) -PathType Leaf) "dependency path exists: $($dependency.id) -> $path"
    }
    if ([string]$dependency.kind -in @('shader', 'font', 'data')) {
        Assert-True (-not [string]::IsNullOrWhiteSpace([string]$dependency.flutterSource)) "asset source path: $($dependency.id)"
        $assetSourcePath = Resolve-FlutterPath ([string]$dependency.flutterSource)
        Assert-True (Test-Path -LiteralPath $assetSourcePath -PathType Leaf) "asset source exists: $($dependency.id)"
        $assetHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $assetSourcePath).Hash.ToLowerInvariant()
        Assert-True ($assetHash -eq [string]$dependency.sourceSha256) "asset source hash: $($dependency.id)"
        Assert-True (-not [string]::IsNullOrWhiteSpace([string]$dependency.flutterKey)) "asset key: $($dependency.id)"
    }
    $dependenciesById[[string]$dependency.id] = $dependency
}

$sourceInventory = [Collections.Generic.List[object]]::new()
$importInventory = [Collections.Generic.List[object]]::new()
$assetInventory = [Collections.Generic.List[object]]::new()
$productFiles = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
$productOwners = @{}

$assetPattern = @'
(?:(?:fromAsset|loadString|loadStructuredData|loadStructuredBinaryData)\s*(?:<[^>]+>)?\s*\(\s*[''\"]([^''\"]+)[''\"])
'@
$assetLiteralPattern = @'
['"]([^'"]+\.(?:frag|vert|sksl|spv|ttf|otf|json|yaml|yml|arb|bin))['"]
'@

foreach ($source in $sourceSlices) {
    Assert-True (-not [string]::IsNullOrWhiteSpace([string]$source.owner)) "source owner: $($source.id)"
    Assert-True ([string]$source.disposition -in $allowedDispositions) "source disposition: $($source.id)"
    Assert-True (@($source.productSources).Count -gt 0) "source product closure: $($source.id)"
    Assert-True (@($source.runtimeDependencyIds).Count -gt 0) "source runtime closure: $($source.id)"
    Assert-True (@($source.hostConsumers).Count -gt 0) "source host consumer: $($source.id)"
    Assert-True (@($source.testEvidence).Count -gt 0) "source test/evidence: $($source.id)"

    $flutterPath = Resolve-FlutterPath ([string]$source.flutterSource)
    Assert-True (Test-Path -LiteralPath $flutterPath -PathType Leaf) "Flutter source exists: $($source.flutterSource)"
    $actualHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $flutterPath).Hash.ToLowerInvariant()
    Assert-True ($actualHash -eq [string]$source.sourceSha256) "Flutter source hash: $($source.flutterSource)"

    $resolvedProducts = @()
    foreach ($path in @($source.productSources)) {
        $resolved = Resolve-RepositoryPath ([string]$path)
        Assert-True (Test-Path -LiteralPath $resolved -PathType Leaf) "product source exists: $($source.id) -> $path"
        $resolvedProducts += $resolved
        [void]$productFiles.Add($resolved)
        $productOwners[$resolved.ToLowerInvariant()] = [string]$source.owner
    }
    foreach ($dependencyId in @($source.runtimeDependencyIds)) {
        Assert-True $dependenciesById.ContainsKey([string]$dependencyId) "runtime dependency is registered: $($source.id) -> $dependencyId"
    }
    foreach ($path in @($source.hostConsumers) + @($source.testEvidence)) {
        Assert-True (Test-Path -LiteralPath (Resolve-RepositoryPath ([string]$path)) -PathType Leaf) "source path exists: $($source.id) -> $path"
    }

    $code = Get-CodeWithoutComments $flutterPath
    foreach ($match in [regex]::Matches($code, '(?m)^\s*import\s+["'']([^"'']+)["'']')) {
        $import = [string]$match.Groups[1].Value
        $owner = ''
        $disposition = 'notVerified'
        $resolved = $false
        if ($import.StartsWith('dart:', [StringComparison]::Ordinal)) {
            $owner = 'Dart SDK'
            $disposition = 'implemented'
            $resolved = $true
        } elseif ($import.StartsWith('package:flutter/', [StringComparison]::Ordinal)) {
            $owner = 'Flutter framework package'
            $candidate = Join-Path $flutterRoot ('packages/flutter/lib/' + $import.Substring('package:flutter/'.Length).Replace('/', '\'))
            $resolved = Test-Path -LiteralPath $candidate -PathType Leaf
            if ($resolved) { $disposition = 'implemented' }
        } elseif ($import.StartsWith('package:', [StringComparison]::Ordinal)) {
            $owner = "Flutter package dependency: $($import.Split('/')[0])"
        } else {
            $owner = 'Flutter relative source dependency'
            $candidate = Join-Path (Split-Path -Parent $flutterPath) $import.Replace('/', '\')
            $resolved = Test-Path -LiteralPath $candidate -PathType Leaf
            if ($resolved) { $disposition = 'implemented' }
        }
        Assert-True (-not [string]::IsNullOrWhiteSpace($owner)) "import owner: $($source.id) -> $import"
        $importInventory.Add([ordered]@{
            source = [string]$source.flutterSource
            import = $import
            owner = $owner
            disposition = $disposition
            resolved = $resolved
        })
    }

    $assetKeys = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($match in [regex]::Matches($code, $assetPattern)) {
        [void]$assetKeys.Add([string]$match.Groups[1].Value)
    }
    foreach ($match in [regex]::Matches($code, $assetLiteralPattern)) {
        [void]$assetKeys.Add([string]$match.Groups[1].Value)
    }
    foreach ($assetKey in $assetKeys) {
        $contract = @($dependencyContracts | Where-Object { [string]$_.flutterKey -eq $assetKey }) | Select-Object -First 1
        Assert-True ($null -ne $contract) "asset dependency contract: $($source.id) -> $assetKey"
        $assetInventory.Add([ordered]@{
            source = [string]$source.flutterSource
            key = $assetKey
            contractId = [string]$contract.id
            owner = [string]$contract.owner
            disposition = [string]$contract.disposition
        })
    }

    $sourceInventory.Add([ordered]@{
        id = [string]$source.id
        area = [string]$source.area
        flutterSource = [string]$source.flutterSource
        sourceSha256 = $actualHash
        productSources = @($source.productSources)
        runtimeDependencyIds = @($source.runtimeDependencyIds)
        disposition = [string]$source.disposition
        verification = [string]$source.verification
    })
}

$riskInventory = [Collections.Generic.List[object]]::new()
foreach ($policy in $patternPolicies) {
    Assert-True (-not [string]::IsNullOrWhiteSpace([string]$policy.owner)) "pattern owner: $($policy.id)"
    Assert-True ([string]$policy.disposition -in $allowedDispositions) "pattern disposition: $($policy.id)"
    $locations = [Collections.Generic.List[object]]::new()
    foreach ($productFile in $productFiles) {
        $productText = Get-CodeWithoutComments $productFile
        foreach ($match in [regex]::Matches($productText, [string]$policy.regex)) {
            if ($locations.Count -lt 50) {
                $sourceOwner = [string]$productOwners[$productFile.ToLowerInvariant()]
                Assert-True (-not [string]::IsNullOrWhiteSpace($sourceOwner)) "static candidate owner: $productFile"
                $locations.Add([ordered]@{
                    path = [IO.Path]::GetRelativePath($repositoryRoot, $productFile).Replace('\', '/')
                    sourceOwner = $sourceOwner
                    line = Get-LineNumber $productText $match.Index
                })
            }
        }
    }
    $riskInventory.Add([ordered]@{
        id = [string]$policy.id
        owner = [string]$policy.owner
        disposition = [string]$policy.disposition
        candidateCount = $locations.Count
        candidateLocations = @($locations)
        ownerlessCandidateCount = @($locations | Where-Object { [string]::IsNullOrWhiteSpace([string]$_.sourceOwner) }).Count
        reachability = 'notProvenByStaticScan'
        evidence = [string]$policy.evidence
    })
}

$targetEvidence = $null
if (Test-Path -LiteralPath $targetEvidencePath -PathType Leaf) {
    $targetEvidence = Read-Json $targetEvidencePath
}
$windowsLive = Get-OptionalProperty $targetEvidence 'windowsLive'
$androidLive = Get-OptionalProperty $targetEvidence 'androidLive'
$windowsFrame = Get-OptionalProperty $windowsLive 'frame'
$androidAutomatedGpu = Get-OptionalProperty $androidLive 'automatedGpu'
$androidFrame = Get-OptionalProperty $androidAutomatedGpu 'frame'
$windowsSemantics = Get-OptionalProperty $windowsLive 'semantics'
$androidSemantics = Get-OptionalProperty $androidFrame 'semantics'
if ($null -eq $androidSemantics) { $androidSemantics = Get-OptionalProperty $androidAutomatedGpu 'semantics' }

function New-TargetBaseline($Target, $Raw, $Frame, $Semantics, [string]$Scenario) {
    $captured = Get-OptionalProperty $targetEvidence 'capturedAtUtc'
    $status = if ($null -ne $Raw) { 'partial' } else { 'notVerified' }
    return [ordered]@{
        status = $status
        scenario = $Scenario
        sourceEvidence = if ($null -ne $Raw) { 'Doroti/validation/evidence/app-targets-evidence.json' } else { $null }
        sourceEvidenceCapturedAtUtc = $captured
        metrics = [ordered]@{
            frameTimeMs = New-Metric $null $null 'FCR-0 trace does not yet emit build/layout/paint/raster phase durations.'
            invalidationsRequested = New-Metric (Get-OptionalProperty $Raw 'invalidationsRequested') 'app-targets-evidence.json' 'No current target evidence.'
            invalidationsCoalesced = New-Metric (Get-OptionalProperty $Raw 'invalidationsCoalesced') 'app-targets-evidence.json' 'No current target evidence.'
            dirtyRenderCount = New-Metric $null $null 'Dirty RenderObject count is not emitted by the current host trace.'
            dirtyLayerCount = New-Metric $null $null 'Dirty layer count is not emitted by the current host trace.'
            sceneCommandCount = New-Metric $null $null 'Scene command count is not emitted by the current host trace.'
            allocationsAndGc = New-Metric $null $null 'Allocation and GC counters require the FCR-0 benchmark trace.'
            inputToPresentMs = New-Metric $null $null 'Input sequence to present timestamps require the FCR-3 trace.'
            submittedFrames = New-Metric (Get-OptionalProperty $Frame 'submitted') 'app-targets-evidence.json' 'No current frame evidence.'
            presentedFrames = New-Metric (Get-OptionalProperty $Frame 'presented') 'app-targets-evidence.json' 'No current frame evidence.'
            failedFrames = New-Metric (Get-OptionalProperty $Frame 'failed') 'app-targets-evidence.json' 'No current frame evidence.'
            nativePointerEvents = New-Metric (Get-OptionalProperty $Raw 'nativePointerEvents') 'app-targets-evidence.json' 'No current target evidence.'
            semanticsWork = if ($null -ne $Semantics) {
                [ordered]@{
                    status = 'observed'
                    updatesReceived = Get-OptionalProperty $Semantics 'updatesReceived'
                    updatesApplied = Get-OptionalProperty $Semantics 'updatesApplied'
                    updatesCoalesced = Get-OptionalProperty $Semantics 'updatesCoalesced'
                    activeElements = Get-OptionalProperty $Semantics 'activeElements'
                }
            } else {
                [ordered]@{ status = 'notVerified'; reason = 'Semantics workload is not present in current target evidence.' }
            }
        }
    }
}

$baseline = [ordered]@{
    schemaVersion = 'doroti.flutter-conformance-baseline/v1'
    milestone = 'FCR-0'
    capturedAtUtc = [DateTime]::UtcNow.ToString('o')
    repositoryRevision = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    flutterRevision = $flutterRevision
    matrix = 'Doroti/validation/evidence/flutter-conformance/framework-parity-matrix.json'
    inventory = [ordered]@{
        status = 'pass'
        sourceCount = $sourceInventory.Count
        sourceEntries = @($sourceInventory)
        importReferenceCount = $importInventory.Count
        importReferences = @($importInventory)
        assetReferenceCount = $assetInventory.Count
        assetReferences = @($assetInventory)
        dependencyContractCount = $dependencyContracts.Count
        packageManifestComparison = [ordered]@{
            status = 'pass'
            missingContracts = @()
            sourceHashDrift = @()
            ownerlessDependencies = @()
        }
        patternFindings = @($riskInventory)
        patternPolicy = 'Static candidates are classified with an owner and remain notVerified until semantic reachability/differential gates run.'
    }
    targets = [ordered]@{
        windowsLive = New-TargetBaseline 'WindowsLive' $windowsLive $windowsFrame $windowsSemantics 'DorotiDemoApp launch, idle repaint, wheel/drag scroll, Ink/FAB and frame survival'
        androidPhysical = New-TargetBaseline 'AndroidPhysical' $androidAutomatedGpu $androidFrame $androidSemantics 'DorotiDemoApp launch, automated scroll, Ink/FAB and persistent display'
        macCatalystNative = [ordered]@{ status = 'notVerified'; reason = 'Apple Silicon native presentation was not run by FCR-0.' }
        webBrowser = [ordered]@{ status = 'notVerified'; reason = 'Browser live conformance was not run by FCR-0.' }
    }
    acceptance = [ordered]@{
        status = 'notVerified'
        performancePass = $false
        reason = 'FCR-0 records the baseline shape and existing counters; timing, allocation, input-to-present and 60-second acceptance traces are not yet available.'
        notRun = @('frame phase timings', 'dirty RenderObject/layer counts', 'scene command count', 'allocation/GC trace', 'input-to-present trace', 'same-scenario Windows and Android release benchmark')
    }
    boundaries = [ordered]@{
        sourceContract = 'pass'
        assetClosure = 'pass'
        sourceHashPin = 'pass'
        runtimePatternOwnership = 'pass-classified-notVerified'
        windowsNativePresentation = if ($null -ne $windowsLive) { 'partial-existing-evidence' } else { 'notVerified' }
        androidPhysicalPresentation = if ($null -ne $androidLive) { 'partial-existing-evidence' } else { 'notVerified' }
        macCatalystNative = 'notVerified'
        webBrowser = 'notVerified'
        performance = 'notVerified'
    }
}

[IO.Directory]::CreateDirectory((Split-Path $baselinePath -Parent)) | Out-Null
$json = (($baseline | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n"
[IO.File]::WriteAllText($baselinePath, $json, [Text.UTF8Encoding]::new($false))
Write-Output "Flutter conformance FCR-0: PASS (source=$($sourceInventory.Count), imports=$($importInventory.Count), assets=$($assetInventory.Count), baseline=partial/notVerified)"
