#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ContractPath = (Join-Path $PSScriptRoot '../validation/contracts/hwnd-exact-cpp-c0.json')
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$contractFullPath = [IO.Path]::GetFullPath($ContractPath)

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Get-ConfinedPath([string] $Root, [string] $RelativePath) {
    $rootWithSeparator = [IO.Path]::GetFullPath($Root).TrimEnd([IO.Path]::DirectorySeparatorChar, [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    $fullPath = [IO.Path]::GetFullPath((Join-Path $Root $RelativePath))
    Assert-True ($fullPath.StartsWith($rootWithSeparator, [StringComparison]::OrdinalIgnoreCase)) "Path escapes its root: $RelativePath"
    return $fullPath
}

function Invoke-Git([string] $WorkingDirectory, [string[]] $Arguments) {
    $result = & git -C $WorkingDirectory @Arguments
    if ($LASTEXITCODE -ne 0) { throw "git $($Arguments -join ' ') failed in $WorkingDirectory" }
    return @($result)
}

Assert-True (Test-Path -LiteralPath $contractFullPath -PathType Leaf) "C0 contract is missing: $contractFullPath"
$contract = Get-Content -LiteralPath $contractFullPath -Raw | ConvertFrom-Json -Depth 64
Assert-True ([string] $contract.schemaVersion -eq 'doroti.windows.hwnd-exact-cpp-c0/v1') 'Unexpected C0 contract schema.'

$referenceRoot = Get-ConfinedPath $repoRoot ([string] $contract.reference.root)
$actualCommit = (Invoke-Git $referenceRoot @('rev-parse', 'HEAD') | Select-Object -First 1).Trim().ToLowerInvariant()
$expectedCommit = ([string] $contract.reference.commit).ToLowerInvariant()
Assert-True ($actualCommit -eq $expectedCommit) "Flutter reference commit drifted: $actualCommit != $expectedCommit"
if ([bool] $contract.reference.requireCleanTrackedTree) {
    $trackedChanges = @(Invoke-Git $referenceRoot @('status', '--porcelain', '--untracked-files=no'))
    Assert-True ($trackedChanges.Count -eq 0) 'Flutter reference has tracked local changes.'
}

$fileIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$fingerprintBuilder = [Text.StringBuilder]::new()
foreach ($file in @($contract.reference.files | Sort-Object path)) {
    $relativePath = [string] $file.path
    Assert-True ($fileIds.Add($relativePath)) "Duplicate pinned file: $relativePath"
    $fullPath = Get-ConfinedPath $referenceRoot $relativePath
    Assert-True (Test-Path -LiteralPath $fullPath -PathType Leaf) "Pinned Flutter source is missing: $relativePath"
    $actualHash = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $expectedHash = ([string] $file.sha256).ToLowerInvariant()
    Assert-True ($expectedHash -match '^[0-9a-f]{64}$') "Invalid pinned hash: $relativePath"
    Assert-True ($actualHash -eq $expectedHash) "Pinned Flutter source hash drifted: $relativePath"
    [void] $fingerprintBuilder.Append($relativePath.Replace('\', '/')).Append('=').Append($actualHash).Append("`n")
}
Assert-True ($fileIds.Count -eq 5) 'C0 must pin exactly the five reviewed Flutter host/view fixtures.'

$anchorIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($anchor in @($contract.anchors)) {
    $id = [string] $anchor.id
    Assert-True (-not [string]::IsNullOrWhiteSpace($id) -and $anchorIds.Add($id)) "Anchor id is missing or duplicated: $id"
    Assert-True ($fileIds.Contains([string] $anchor.path)) "Anchor is outside the pinned file set: $id"
    $sourcePath = Get-ConfinedPath $referenceRoot ([string] $anchor.path)
    $actualOccurrences = @(Select-String -LiteralPath $sourcePath -SimpleMatch -Pattern ([string] $anchor.literal)).Count
    Assert-True ($actualOccurrences -eq [int] $anchor.expectedOccurrences) "Flutter source anchor drifted: $id expected $($anchor.expectedOccurrences), found $actualOccurrences"
}

$mappedAnchorIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$mappingIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($mapping in @($contract.protocolMappings)) {
    Assert-True ($mappingIds.Add([string] $mapping.id)) "Protocol mapping id is duplicated: $($mapping.id)"
    Assert-True (-not [string]::IsNullOrWhiteSpace([string] $mapping.dorotiRequirement)) "Protocol mapping requirement is empty: $($mapping.id)"
    foreach ($reference in @($mapping.referenceAnchors)) {
        Assert-True ($anchorIds.Contains([string] $reference)) "Unknown mapped anchor: $($mapping.id)/$reference"
        [void] $mappedAnchorIds.Add([string] $reference)
    }
}
Assert-True ($mappedAnchorIds.SetEquals($anchorIds)) 'Every pinned Flutter source anchor must be represented by a Doroti mapping.'

$expectedWindows = @('standard-overlapped-top-level-hwnd', 'single-visible-child-render-hwnd', 'single-message-only-task-hwnd')
Assert-True ([string] $contract.dorotiTopology.adapter -eq 'HwndExactCpp') 'C0 adapter identity must be HwndExactCpp.'
Assert-True (@(Compare-Object $expectedWindows @($contract.dorotiTopology.windows)).Count -eq 0) 'C0 native window topology drifted.'
Assert-True ([string] $contract.dorotiTopology.queue -eq 'running-one-latest-pending-one') 'C0 queue contract drifted.'
$expectedTerminals = @('presented', 'superseded', 'failed')
Assert-True (@(Compare-Object $expectedTerminals @($contract.dorotiTopology.terminals)).Count -eq 0) 'C0 terminal contract drifted.'
$requiredForbidden = @('content-island-primary-renderer', 'composition-swap-chain-primary-renderer', 'two-visible-front-owners', 'managed-swap-chain-ownership', 'private-skiasharp-reflection', 'unbounded-raw-pointer-lifetime', 'capacity-backing', 'clip-only-resize', 'set-source-size', 'full-frame-stretch', 'resize-debounce-primary-fix', 'stale-scene-relabel')
Assert-True (@(Compare-Object $requiredForbidden @($contract.forbidden)).Count -eq 0) 'C0 forbidden-set drifted.'

$boundary = $contract.repositoryBoundary
foreach ($property in @('hostProject', 'runner', 'targetProject', 'targetManifest', 'targetBuildTransitive')) {
    $path = Get-ConfinedPath $repoRoot ([string] $boundary.$property)
    Assert-True (Test-Path -LiteralPath $path -PathType Leaf) "Repository boundary file is missing: $property"
}
foreach ($diagnosticPath in @($boundary.diagnosticHistory)) {
    $path = Get-ConfinedPath $repoRoot ([string] $diagnosticPath)
    Assert-True (Test-Path -LiteralPath $path -PathType Container) "Diagnostic history boundary is missing: $diagnosticPath"
}
$hostProject = Get-Content -LiteralPath (Get-ConfinedPath $repoRoot ([string] $boundary.hostProject)) -Raw
Assert-True ($hostProject.Contains('<PackageReference Include="Microsoft.WindowsAppSDK" VersionOverride="2.4.0" />', [StringComparison]::Ordinal)) 'Windows App SDK host must keep the exact 2.4.0 VersionOverride.'
$runner = Get-Content -LiteralPath (Get-ConfinedPath $repoRoot ([string] $boundary.runner)) -Raw
Assert-True ($runner.Contains('The WinRtComposition backend is planned but not implemented.', [StringComparison]::Ordinal)) 'Pre-C5 runner must remain fail-fast.'
$targetManifest = Get-Content -LiteralPath (Get-ConfinedPath $repoRoot ([string] $boundary.targetManifest)) -Raw | ConvertFrom-Json -Depth 16
Assert-True ([string] $targetManifest.defaultAdapter -eq [string] $boundary.preC5FailFastAdapter) 'Pre-C5 target adapter drifted.'
Assert-True ([string] $targetManifest.rid -eq 'win-x64') 'C0 target boundary must remain win-x64.'

foreach ($linkedPath in @('work3.md', 'idea.md')) {
    Assert-True (Test-Path -LiteralPath (Get-ConfinedPath $repoRoot $linkedPath) -PathType Leaf) "Linked plan file is missing: $linkedPath"
}

$sourceFingerprint = [Convert]::ToHexString(
    [Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($fingerprintBuilder.ToString()))
).ToLowerInvariant()
[ordered]@{
    schemaVersion = 'doroti.windows.hwnd-exact-cpp-c0-validation/v1'
    gate = 'C0'
    status = 'PASS'
    flutterCommit = $actualCommit
    sourceFingerprint = $sourceFingerprint
    validatedFileCount = $fileIds.Count
    validatedAnchorCount = $anchorIds.Count
    validatedMappingCount = $mappingIds.Count
    implementation = 'notVerified'
    runtime = 'notVerified'
    visible = 'notVerified'
    scopeBoundary = [string] $contract.scopeBoundary
} | ConvertTo-Json -Depth 16
