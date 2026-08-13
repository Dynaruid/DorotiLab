#Requires -Version 5.1
param(
    [Parameter(Mandatory = $true)] [string] $ServicesCandidateRoot,
    [Parameter(Mandatory = $true)] [string] $RenderingCandidateRoot,
    [Parameter(Mandatory = $true)] [string] $MaterialCandidateRoot
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$servicesRoot = (Resolve-Path $ServicesCandidateRoot).Path
$renderingRoot = (Resolve-Path $RenderingCandidateRoot).Path
$materialRoot = (Resolve-Path $MaterialCandidateRoot).Path

function Assert-SuccessfulCandidate([string] $Root, [string] $Name) {
    $reportPath = Join-Path $Root 'converter-report.json'
    if (-not (Test-Path -LiteralPath $reportPath -PathType Leaf)) {
        throw "$Name compiler report is missing: $reportPath"
    }
    $report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json
    if (-not [bool]$report.success -or @($report.diagnostics | Where-Object severity -eq 'error').Count -ne 0) {
        throw "$Name candidate is not a successful clean compiler output."
    }
    return $report
}

function Write-Utf8Lf([string] $Path, [string] $Content) {
    $normalized = $Content -replace "`r`n", "`n"
    if (-not $normalized.EndsWith("`n", [StringComparison]::Ordinal)) { $normalized += "`n" }
    [IO.File]::WriteAllText($Path, $normalized, [Text.UTF8Encoding]::new($false))
}

$servicesReport = Assert-SuccessfulCandidate $servicesRoot 'Services'
$renderingReport = Assert-SuccessfulCandidate $renderingRoot 'Rendering'
$materialReport = Assert-SuccessfulCandidate $materialRoot 'Material'
$candidateAnnotationPath = Join-Path $servicesRoot 'projects/Services/mouse_tracking.g.cs'
$candidateProxyPath = Join-Path $renderingRoot 'projects/Rendering/proxy_box.g.cs'
$candidateTrackerPath = Join-Path $renderingRoot 'projects/Rendering/mouse_tracker.g.cs'
$productAnnotationPath = Join-Path $dorotiRoot 'src/Doroti.Flutter.Framework.Services/mouse_tracking.cs'
$productProxyPath = Join-Path $dorotiRoot 'src/Doroti.Flutter.Framework.Rendering/proxy_box.cs'
$productTrackerPath = Join-Path $dorotiRoot 'src/Doroti.Flutter.Framework.Rendering/mouse_tracker.cs'
$candidateInkWellPath = Join-Path $materialRoot 'projects/Material/ink_well.g.cs'
$productInkWellPath = Join-Path $dorotiRoot 'src/Doroti.Flutter.Framework.Material/ink_well.cs'

$candidateAnnotation = Get-Content -LiteralPath $candidateAnnotationPath -Raw
$annotationMatch = [regex]::Match($candidateAnnotation, '(?s)public interface IMouseTrackerAnnotation\s*\{.*?public class MouseTrackerAnnotation[^\r\n]*IMouseTrackerAnnotation[^\r\n]*\s*\{.*?\}\s*$')
if (-not $annotationMatch.Success) {
    throw 'Clean Services output does not emit the MouseTrackerAnnotation structural companion contract.'
}
$productAnnotation = Get-Content -LiteralPath $productAnnotationPath -Raw
$productAnnotationPattern = '(?s)(?:public interface IMouseTrackerAnnotation\s*\{.*?\}\s*)?public (?:class|interface) MouseTrackerAnnotation(?:\s*:\s*[^\r\n{]+)?\s*\{.*?\}\s*$'
if (-not [regex]::IsMatch($productAnnotation, $productAnnotationPattern)) {
    throw 'Reviewed MouseTrackerAnnotation product declaration was not found.'
}
$productAnnotation = [regex]::Replace(
    $productAnnotation,
    $productAnnotationPattern,
    $annotationMatch.Value.TrimEnd())
Write-Utf8Lf $productAnnotationPath $productAnnotation

$candidateProxy = Get-Content -LiteralPath $candidateProxyPath -Raw
$proxyMatch = [regex]::Match($candidateProxy, '(?m)^public class RenderMouseRegion\s*:\s*RenderProxyBoxWithHitTestBehavior,\s*(?:global::Doroti\.Generated\.Framework\.Services\.)?IMouseTrackerAnnotation\s*$')
if (-not $proxyMatch.Success) {
    throw 'Clean Rendering output does not preserve RenderMouseRegion implements MouseTrackerAnnotation.'
}
$productProxy = Get-Content -LiteralPath $productProxyPath -Raw
$productProxyPattern = '(?m)^public class RenderMouseRegion\s*:\s*RenderProxyBoxWithHitTestBehavior(?:,\s*(?:global::Doroti\.Generated\.Framework\.Services\.)?I?MouseTrackerAnnotation)?\s*$'
if (-not [regex]::IsMatch($productProxy, $productProxyPattern)) {
    throw 'Reviewed RenderMouseRegion declaration was not found.'
}
$productProxy = [regex]::Replace($productProxy, $productProxyPattern, $proxyMatch.Value)
$explicitContract = [regex]::Match($candidateProxy, '(?m)^\s+dynamic global::Doroti\.Generated\.Framework\.Services\.IMouseTrackerAnnotation\.onEnter[^\r\n]*\r?\n\s+dynamic global::Doroti\.Generated\.Framework\.Services\.IMouseTrackerAnnotation\.onExit[^\r\n]*$')
if (-not $explicitContract.Success) {
    throw 'Clean Rendering output does not emit explicit MouseTracker callback contract adapters.'
}
$productProxy = [regex]::Replace(
    $productProxy,
    '(?m)^\s+dynamic global::Doroti\.Generated\.Framework\.Services\.IMouseTrackerAnnotation\.on(?:Enter|Exit)[^\r\n]*\r?\n?',
    '')
$contractAnchor = '(?m)^(\s+internal virtual bool _validForMouseTracker[^\r\n]*\r?\n)'
if (-not [regex]::IsMatch($productProxy, $contractAnchor)) {
    throw 'Reviewed RenderMouseRegion callback contract anchor was not found.'
}
$productProxy = [regex]::Replace(
    $productProxy,
    $contractAnchor,
    '$1' + $explicitContract.Value + "`n",
    1)
Write-Utf8Lf $productProxyPath $productProxy

$candidateTracker = Get-Content -LiteralPath $candidateTrackerPath -Raw
if ($candidateTracker -notmatch 'DartMap<[^>]*IMouseTrackerAnnotation, Matrix4>' -or
    $candidateTracker -notmatch 'target[^\r\n]* is [^\r\n]*IMouseTrackerAnnotation') {
    throw 'Clean Rendering output does not make MouseTracker consume the structural annotation contract.'
}
$productTracker = Get-Content -LiteralPath $productTrackerPath -Raw
$productTracker = [regex]::Replace($productTracker, '(?<!I)\bMouseTrackerAnnotation\b', 'IMouseTrackerAnnotation')
Write-Utf8Lf $productTrackerPath $productTracker

$nullableCallbackPattern = '(?m)^\s*return .*new _InkResponseStateWidget__ink_well\(onTap: this\.onTap,.*onTapCancel: this\.onTapCancel,.*onSecondaryTapCancel: this\.onSecondaryTapCancel,.*$'
$candidateInkWell = Get-Content -LiteralPath $candidateInkWellPath -Raw
$productInkWell = Get-Content -LiteralPath $productInkWellPath -Raw
if (-not [regex]::IsMatch($candidateInkWell, $nullableCallbackPattern) -or
    -not [regex]::IsMatch($productInkWell, $nullableCallbackPattern)) {
    throw 'Clean Material output and reviewed product do not preserve nullable InkResponse callbacks.'
}

$evidence = [ordered]@{
    schemaVersion = 'doroti.g6-pointer-contract-promotion/v1'
    milestone = 'G6-5R-I'
    flutterGitRevision = '56b8e1a851a594b1a154f8ea93270807dab22b9a'
    promotedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
    servicesCompilerIdentity = $servicesReport.identity
    renderingCompilerIdentity = $renderingReport.identity
    materialCompilerIdentity = $materialReport.identity
    contracts = @(
        [ordered]@{ dart = 'RenderMouseRegion implements MouseTrackerAnnotation'; csharp = 'RenderMouseRegion : RenderProxyBoxWithHitTestBehavior, IMouseTrackerAnnotation' },
        [ordered]@{ dart = 'MouseTrackerAnnotation structural surface'; csharp = 'interface IMouseTrackerAnnotation plus class MouseTrackerAnnotation' },
        [ordered]@{ dart = 'nullable InkResponse callback tear-offs'; csharp = 'nullable delegate method groups without non-null forwarding wrappers' }
    )
    candidateSha256 = [ordered]@{
        services = (Get-FileHash -LiteralPath $candidateAnnotationPath -Algorithm SHA256).Hash.ToLowerInvariant()
        rendering = (Get-FileHash -LiteralPath $candidateProxyPath -Algorithm SHA256).Hash.ToLowerInvariant()
        tracker = (Get-FileHash -LiteralPath $candidateTrackerPath -Algorithm SHA256).Hash.ToLowerInvariant()
        materialInkWell = (Get-FileHash -LiteralPath $candidateInkWellPath -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    productSha256 = [ordered]@{
        services = (Get-FileHash -LiteralPath $productAnnotationPath -Algorithm SHA256).Hash.ToLowerInvariant()
        rendering = (Get-FileHash -LiteralPath $productProxyPath -Algorithm SHA256).Hash.ToLowerInvariant()
        tracker = (Get-FileHash -LiteralPath $productTrackerPath -Algorithm SHA256).Hash.ToLowerInvariant()
        materialInkWell = (Get-FileHash -LiteralPath $productInkWellPath -Algorithm SHA256).Hash.ToLowerInvariant()
    }
}
$evidencePath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-pointer-contract-promotion.json'
Write-Utf8Lf $evidencePath ($evidence | ConvertTo-Json -Depth 20)
Write-Output 'G6-5R-I compiler contract promotion: PASS'
