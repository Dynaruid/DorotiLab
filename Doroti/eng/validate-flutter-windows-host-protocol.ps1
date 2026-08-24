#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ContractPath = (Join-Path $PSScriptRoot '../validation/contracts/flutter-windows-host-protocol.json')
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$contractFullPath = [IO.Path]::GetFullPath($ContractPath)

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Get-ConfinedPath([string] $Root, [string] $RelativePath) {
    $rootWithSeparator = [IO.Path]::GetFullPath($Root).TrimEnd([IO.Path]::DirectorySeparatorChar, [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    $fullPath = [IO.Path]::GetFullPath((Join-Path $Root $RelativePath))
    Assert-True ($fullPath.StartsWith($rootWithSeparator, [StringComparison]::OrdinalIgnoreCase)) "Reference path escapes its root: $RelativePath"
    return $fullPath
}

function Invoke-Git([string] $WorkingDirectory, [string[]] $Arguments) {
    $result = & git -C $WorkingDirectory @Arguments
    if ($LASTEXITCODE -ne 0) { throw "git $($Arguments -join ' ') failed in $WorkingDirectory" }
    return @($result)
}

Assert-True (Test-Path -LiteralPath $contractFullPath -PathType Leaf) "Flutter Windows host protocol contract is missing: $contractFullPath"
$contract = Get-Content -LiteralPath $contractFullPath -Raw | ConvertFrom-Json -Depth 32
Assert-True ([string] $contract.schemaVersion -eq 'doroti.flutter-windows-host-protocol/v1') 'Unexpected Flutter Windows host protocol contract schema.'
Assert-True (-not [string]::IsNullOrWhiteSpace([string] $contract.reference.root)) 'Contract reference root is empty.'
Assert-True (-not [string]::IsNullOrWhiteSpace([string] $contract.reference.commit)) 'Contract reference commit is empty.'

$referenceRoot = Get-ConfinedPath $repoRoot ([string] $contract.reference.root)
Assert-True (Test-Path -LiteralPath $referenceRoot -PathType Container) "Flutter reference root is missing: $referenceRoot"
$actualCommit = (Invoke-Git $referenceRoot @('rev-parse', 'HEAD') | Select-Object -First 1).Trim().ToLowerInvariant()
$expectedCommit = ([string] $contract.reference.commit).ToLowerInvariant()
Assert-True ($actualCommit -eq $expectedCommit) "Flutter reference commit drifted: $actualCommit != $expectedCommit"

if ([bool] $contract.reference.requireCleanTrackedTree) {
    $trackedChanges = @(Invoke-Git $referenceRoot @('status', '--porcelain', '--untracked-files=no'))
    Assert-True ($trackedChanges.Count -eq 0) 'Flutter reference has tracked local changes; source protocol validation refuses implicit promotion.'
}

$validatedFiles = [Collections.Generic.List[object]]::new()
$fingerprintBuilder = [Text.StringBuilder]::new()
foreach ($file in @($contract.reference.files | Sort-Object path)) {
    $relativePath = [string] $file.path
    $expectedHash = ([string] $file.sha256).ToLowerInvariant()
    Assert-True (-not [string]::IsNullOrWhiteSpace($relativePath)) 'A reference file path is empty.'
    Assert-True ($expectedHash -match '^[0-9a-f]{64}$') "Reference file hash is invalid: $relativePath"
    $fullPath = Get-ConfinedPath $referenceRoot $relativePath
    Assert-True (Test-Path -LiteralPath $fullPath -PathType Leaf) "Pinned Flutter source file is missing: $relativePath"
    $actualHash = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
    Assert-True ($actualHash -eq $expectedHash) "Pinned Flutter source hash drifted: $relativePath"
    [void] $fingerprintBuilder.Append($relativePath.Replace('\', '/')).Append('=').Append($actualHash).Append("`n")
    $validatedFiles.Add([ordered]@{ path = $relativePath; sha256 = $actualHash })
}
Assert-True ($validatedFiles.Count -gt 0) 'Contract has no pinned Flutter source files.'

$anchorIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$validatedAnchors = [Collections.Generic.List[object]]::new()
foreach ($anchor in @($contract.anchors)) {
    $id = [string] $anchor.id
    $relativePath = [string] $anchor.path
    $literal = [string] $anchor.literal
    $expectedOccurrences = [int] $anchor.expectedOccurrences
    Assert-True (-not [string]::IsNullOrWhiteSpace($id) -and $anchorIds.Add($id)) "Anchor id is missing or duplicated: $id"
    Assert-True (-not [string]::IsNullOrWhiteSpace($literal)) "Anchor literal is empty: $id"
    Assert-True ($expectedOccurrences -gt 0) "Anchor occurrence count must be positive: $id"
    $fullPath = Get-ConfinedPath $referenceRoot $relativePath
    Assert-True (Test-Path -LiteralPath $fullPath -PathType Leaf) "Anchor source file is missing: $relativePath"
    $actualOccurrences = @(Select-String -LiteralPath $fullPath -SimpleMatch -Pattern $literal).Count
    Assert-True ($actualOccurrences -eq $expectedOccurrences) "Flutter protocol anchor drifted: $id expected $expectedOccurrences occurrence(s), found $actualOccurrences."
    $validatedAnchors.Add([ordered]@{ id = $id; path = $relativePath; occurrences = $actualOccurrences })
}
Assert-True ($validatedAnchors.Count -gt 0) 'Contract has no Flutter protocol anchors.'

$mappedAnchorIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$validatedMappings = [Collections.Generic.List[object]]::new()
foreach ($mapping in @($contract.protocolMappings)) {
    $mappingId = [string] $mapping.id
    $requirement = [string] $mapping.dorotiRequirement
    Assert-True (-not [string]::IsNullOrWhiteSpace($mappingId)) 'Protocol mapping id is empty.'
    Assert-True (-not [string]::IsNullOrWhiteSpace($requirement)) "Protocol mapping requirement is empty: $mappingId"
    $references = @($mapping.referenceAnchors)
    Assert-True ($references.Count -gt 0) "Protocol mapping has no reference anchors: $mappingId"
    foreach ($reference in $references) {
        $anchorId = [string] $reference
        Assert-True ($anchorIds.Contains($anchorId)) "Protocol mapping references an unknown anchor: $mappingId/$anchorId"
        [void] $mappedAnchorIds.Add($anchorId)
    }
    $validatedMappings.Add([ordered]@{ id = $mappingId; referenceAnchors = @($references); dorotiRequirement = $requirement })
}
Assert-True ($validatedMappings.Count -gt 0) 'Contract has no Flutter-to-Doroti protocol mappings.'
Assert-True ($mappedAnchorIds.SetEquals($anchorIds)) 'Every pinned Flutter protocol anchor must be represented in a Doroti mapping.'

$sourceFingerprint = [Convert]::ToHexString(
    [Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($fingerprintBuilder.ToString()))
).ToLowerInvariant()
$summary = [ordered]@{
    schemaVersion = 'doroti.flutter-windows-host-protocol-validation/v1'
    status = 'PASS'
    flutterCommit = $actualCommit
    sourceFingerprint = $sourceFingerprint
    validatedFileCount = $validatedFiles.Count
    validatedAnchorCount = $validatedAnchors.Count
    validatedMappingCount = $validatedMappings.Count
    scopeBoundary = [string] $contract.scopeBoundary
}

$summary | ConvertTo-Json -Depth 16
