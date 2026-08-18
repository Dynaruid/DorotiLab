#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$manifestPath = Join-Path $dorotiRoot 'validation/fcr5-scroll/fixture-manifest.json'
$expectedFailurePath = Join-Path $dorotiRoot 'validation/fcr5-scroll/expected-failure.json'
$contractProject = Join-Path $dorotiRoot 'validation/fcr5-scroll/Doroti.Validation.Fcr5Scroll.csproj'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/fcr5-scroll-evidence.json'
$lowererProject = Join-Path $repositoryRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj'
$lowererFixtureManifest = Join-Path $dorotiRoot 'validation/fcr5-scroll/lowerer/selection.json'
$scrollNotificationManifest = Join-Path $dorotiRoot 'validation/fcr5-scroll/lowerer/scroll_notification_selection.json'

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Read-Text([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "source exists: $Path"
    return Get-Content -Raw -LiteralPath $Path
}

function Invoke-DotnetProcess([string[]] $Arguments, [string] $Name) {
    $stdout = Join-Path ([IO.Path]::GetTempPath()) ("doroti-fcr5-process-$([guid]::NewGuid()).log")
    try {
        $process = Start-Process dotnet -ArgumentList $Arguments -NoNewWindow -PassThru `
            -RedirectStandardOutput $stdout -RedirectStandardError "$stdout.err"
        Assert-True ($process.WaitForExit(1200000)) "$Name timeout"
        $output = ((Get-Content -Raw -LiteralPath $stdout) + (Get-Content -Raw -LiteralPath "$stdout.err"))
        Assert-True ($process.ExitCode -eq 0) "$Name exit: $output"
        return $output
    }
    finally { Remove-Item -LiteralPath $stdout, "$stdout.err" -Force -ErrorAction SilentlyContinue }
}

function New-Fcr5TempDirectory([string] $Label) {
    $path = Join-Path ([IO.Path]::GetTempPath()) ("doroti-fcr5-$Label-$([guid]::NewGuid())")
    [IO.Directory]::CreateDirectory($path) | Out-Null
    return [IO.Path]::GetFullPath($path)
}

function Remove-Fcr5TempDirectory([string] $Path) {
    $resolved = [IO.Path]::GetFullPath($Path)
    $tempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
    Assert-True ($resolved.StartsWith($tempRoot, [StringComparison]::OrdinalIgnoreCase)) "temporary output is inside system temp: $resolved"
    if (Test-Path -LiteralPath $resolved) { [IO.Directory]::Delete($resolved, $true) }
}

function Invoke-LowererFixture([string] $Configuration) {
    $output = New-Fcr5TempDirectory "lowerer-$Configuration"
    try {
        Invoke-DotnetProcess @('run', '--project', $lowererProject, '-c', $Configuration, '--',
            '--manifest', $lowererFixtureManifest, '--output', $output, '--parallelism', '1') `
            "FCR-5 lowerer fixture ($Configuration)" | Out-Null
        $generatedPath = Join-Path $output 'constructor_depth_fixture.g.cs'
        $generated = Read-Text $generatedPath
        foreach ($anchor in @(
            'public UpdateNotification(string source, long? depth = null) : base(source: source)',
            'if ((depth is not null))',
            '_depth = DartRuntimePrimitives.RequireValue(')) {
            Assert-True ($generated.Contains($anchor, [StringComparison]::Ordinal)) "lowerer constructor-depth anchor ($Configuration): $anchor"
        }
        $generatedProject = Join-Path $output 'Doroti.Validation.Fcr5Scroll.Lowerer.csproj'
        Invoke-DotnetProcess @('build', $generatedProject, '-c', $Configuration, '--nologo',
            "-p:DorotiRepositoryRoot=$dorotiRoot") "FCR-5 generated lowerer fixture build ($Configuration)" | Out-Null
    }
    finally { Remove-Fcr5TempDirectory $output }
}

function Invoke-ScrollNotificationRegeneration([string] $Configuration) {
    $output = New-Fcr5TempDirectory "scroll-notification-$Configuration"
    try {
        Invoke-DotnetProcess @('run', '--project', $lowererProject, '-c', $Configuration, '--',
            '--manifest', $scrollNotificationManifest, '--output', $output, '--parallelism', '1') `
            "FCR-5 scroll_notification regeneration ($Configuration)" | Out-Null
        $generatedPath = Join-Path $output 'projects/Widgets/scroll_notification.g.cs'
        $generated = Read-Text $generatedPath
        $constructor = [regex]::Match(
            $generated,
            'public ScrollUpdateNotification\([\s\S]{0,500}?if \(\(depth is not null\)\)[\s\S]{0,300}?_depth = DartRuntimePrimitives\.RequireValue\(').Value
        Assert-True ($constructor.Length -gt 0) "regenerated ScrollUpdateNotification preserves nullable depth ($Configuration)"
        return (Get-FileHash -Algorithm SHA256 -LiteralPath $generatedPath).Hash.ToLowerInvariant()
    }
    finally { Remove-Fcr5TempDirectory $output }
}

function Invoke-Contract([string] $Configuration) {
    Push-Location $repositoryRoot
    try {
        # Contract execution has the repository-wide 20-minute test ceiling.
        $outputPath = Join-Path ([IO.Path]::GetTempPath()) ("doroti-fcr5-$Configuration-$([guid]::NewGuid()).log")
        try {
            $process = Start-Process dotnet -ArgumentList @('run', '--project', $contractProject, '-c', $Configuration, '--nologo') `
                -NoNewWindow -PassThru -RedirectStandardOutput $outputPath -RedirectStandardError "$outputPath.err"
            Assert-True ($process.WaitForExit(1200000)) "FCR-5 runtime contract timeout ($Configuration)"
            $output = ((Get-Content -Raw -LiteralPath $outputPath) + (Get-Content -Raw -LiteralPath "$outputPath.err"))
            Assert-True ($process.ExitCode -eq 0) "FCR-5 runtime contract exit ($Configuration): $output"
            Assert-True ($output.Contains("FCR-5 scroll runtime contract: PASS (configuration=$Configuration", [StringComparison]::Ordinal)) "FCR-5 runtime contract result ($Configuration)"
        }
        finally { Remove-Item -LiteralPath $outputPath, "$outputPath.err" -Force -ErrorAction SilentlyContinue }
    }
    finally { Pop-Location }
}

Assert-True (Test-Path -LiteralPath $flutterRoot -PathType Container) 'pinned Flutter checkout exists'
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
Assert-True ([string]$manifest.schemaVersion -eq 'doroti.flutter-conformance-fcr5-fixture/v1') 'FCR-5 fixture schema'
$expectedFailure = Get-Content -Raw -LiteralPath $expectedFailurePath | ConvertFrom-Json
Assert-True ([string]$expectedFailure.schemaVersion -eq 'doroti.flutter-conformance-fcr5-expected-failure/v1' -and
    [string]$expectedFailure.status -eq 'expectedFailure' -and $expectedFailure.expectedDepth -eq 1 -and
    $expectedFailure.reviewedGeneratedDepth -eq 0) 'pre-fix depth-loss evidence is retained explicitly'
$flutterRevision = (& git -C $flutterRoot rev-parse HEAD).Trim()
Assert-True ($flutterRevision -eq [string]$manifest.flutterRevision) "Flutter revision pin: expected $($manifest.flutterRevision), got $flutterRevision"
foreach ($source in @($manifest.sources)) {
    $path = Join-Path $flutterRoot ([string]$source.path).Replace('/', '\')
    $text = Read-Text $path
    Assert-True ((Get-FileHash -Algorithm SHA256 -LiteralPath $path).Hash.ToLowerInvariant() -eq [string]$source.sha256) "Flutter source hash: $($source.path)"
    foreach ($anchor in @($source.anchors)) { Assert-True ($text.Contains([string]$anchor, [StringComparison]::Ordinal)) "Flutter source anchor: $($source.path) -> $anchor" }
}

$controller = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scroll_controller.cs')
Assert-True ($controller.Contains('var futures = new List<Future>();', [StringComparison]::Ordinal)) 'controller snapshots per-position animations'
Assert-True ($controller.Contains('foreach (var position in this._positions.ToArray())', [StringComparison]::Ordinal)) 'controller does not enumerate a mutable position list'
Assert-True ($controller.Contains('DartAsyncRuntime.wait<object?>(futures)', [StringComparison]::Ordinal)) 'controller waits for every initial position'

$activity = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scroll_activity.cs')
Assert-True ($activity.Contains('CreateUnbounded(', [StringComparison]::Ordinal) -and $activity.Contains('value: from,', [StringComparison]::Ordinal)) 'driven activity creates its controller at the requested start offset'
Assert-True ($activity.Contains('DartRuntimePrimitives.Observe(', [StringComparison]::Ordinal) -and $activity.Contains('DrivenScrollActivity.animateTo', [StringComparison]::Ordinal)) 'driven animation completion is observed'

$scrollView = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scroll_view.cs')
Assert-True ($scrollView.Contains('PrimaryScrollController.shouldInherit(context, this.scrollDirection)', [StringComparison]::Ordinal)) 'scroll view uses Flutter primary-controller inheritance rule'
Assert-True ($scrollView.Contains('PrimaryScrollController.maybeOf(context)', [StringComparison]::Ordinal)) 'primary scroll view obtains the inherited controller'
$scrollbar = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scrollbar.cs')
Assert-True ($scrollbar.Contains('((RawScrollbar)(object)this.widget).controller ?? (ScrollController)PrimaryScrollController.maybeOf(this.context)', [StringComparison]::Ordinal)) 'scrollbar resolves the same explicit-or-primary controller contract'
Assert-True ($scrollbar.Contains('this.fadeoutOpacityAnimation.addListener(this.notifyListeners);', [StringComparison]::Ordinal)) 'scrollbar fade animation invalidates its painter'
Assert-True ($scrollbar.Contains('this.fadeoutOpacityAnimation.removeListener(this.notifyListeners);', [StringComparison]::Ordinal)) 'scrollbar fade listener has stable removal identity'

$singleChildScrollView = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/single_child_scroll_view.cs')
Assert-True ($singleChildScrollView.Contains('this._offset.addListener(this._hasScrolled);', [StringComparison]::Ordinal)) 'single-child viewport observes scroll offsets with Flutter method-tear-off identity'
Assert-True ($singleChildScrollView.Contains('this._offset.removeListener(this._hasScrolled);', [StringComparison]::Ordinal)) 'single-child viewport removes the same scroll listener'

$scrollable = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scrollable.cs')
Assert-True ($scrollable.Contains('this._position.addListener(this._scheduleLayoutChange);', [StringComparison]::Ordinal)) 'scroll selection installs its initial position listener'
Assert-True ($scrollable.Contains('this._position.addListener(this.markNeedsSemanticsUpdate);', [StringComparison]::Ordinal)) 'scroll semantics installs its initial position listener'

$overscroll = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/overscroll_indicator.cs')
Assert-True ($overscroll.Contains('this._overscrollNotifier.addListener(listener);', [StringComparison]::Ordinal)) 'overscroll forwards listener identity without a wrapper closure'
Assert-True ($overscroll.Contains('this._overscrollNotifier.removeListener(listener);', [StringComparison]::Ordinal)) 'overscroll removes the forwarded listener identity'

$scrollSources = Get-ChildItem -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets') -Filter '*scroll*.cs' -File
$unstableListenerPattern = '\.(?:addListener|removeListener)\(\(\) => this\.[A-Za-z_][A-Za-z0-9_]*\(\)\)'
$unstableListeners = @($scrollSources | Select-String -Pattern $unstableListenerPattern)
Assert-True ($unstableListeners.Count -eq 0) 'scroll sources contain no unstable instance-method listener wrappers'

$lowerer = Read-Text (Join-Path $repositoryRoot 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.G53Compatibility.cs')
Assert-True ($lowerer.Contains('Retain CLR method-group', [StringComparison]::Ordinal)) 'compiler documents Flutter method-tear-off identity'
Assert-True ($lowerer.Contains('this._overscrollNotifier.$1(listener);', [StringComparison]::Ordinal)) 'compiler preserves forwarded overscroll listener identity'
$constructorLowerer = Read-Text (Join-Path $repositoryRoot 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.Declarations.cs')
Assert-True ($constructorLowerer.Contains('var generativeConstructorBody = constructor.Ast.Children', [StringComparison]::Ordinal) -and
    $constructorLowerer.Contains('EmitBlockBody(', [StringComparison]::Ordinal)) 'lowerer emits generative constructor block bodies by AST rule'
Assert-True (-not $constructorLowerer.Contains('ScrollUpdateNotification', [StringComparison]::Ordinal)) 'constructor-body lowering is not a ScrollUpdateNotification string special case'

$trace = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Ui/ScrollLifecycle.cs')
foreach ($phase in @('nativeInput', 'pointerData', 'hitTest', 'gesture', 'activity', 'viewport', 'layout', 'paint', 'retainedLayer', 'raster', 'present', 'scrollbar', 'semantics')) {
    Assert-True ($trace.Contains($phase, [StringComparison]::Ordinal)) "scroll trace phase: $phase"
}
Assert-True ($trace.Contains('Consumers must supply the', [StringComparison]::Ordinal)) 'scroll trace prevents accidental cross-input attribution'

$frameTrace = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Ui/FrameLifecycle.cs')
foreach ($phase in @('scrollStart', 'scrollUpdate', 'scrollEnd', 'animationStart', 'animationEnd', 'rasterEnd', 'semanticsDeferred')) {
    Assert-True ($frameTrace.Contains($phase, [StringComparison]::Ordinal)) "frame trace phase: $phase"
}
Assert-True ($frameTrace.Contains('private const int Capacity = 8192;', [StringComparison]::Ordinal)) 'frame trace retains complete high-refresh gestures'
foreach ($field in @('ScrollPositionId', 'ScrollOffset', 'ScrollDelta', 'ScrollActivity', 'ScrollMinExtent', 'ScrollMaxExtent', 'TickerId', 'TickerLabel')) {
    Assert-True ($frameTrace.Contains($field, [StringComparison]::Ordinal)) "frame trace diagnostic field: $field"
}
$scrollPosition = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scroll_position.cs')
Assert-True ($scrollPosition.Contains('recordScrollTrace(DorotiFramePhase.scrollStart)', [StringComparison]::Ordinal)) 'scroll position records actual start'
Assert-True ($scrollPosition.Contains('recordScrollTrace(DorotiFramePhase.scrollUpdate, delta)', [StringComparison]::Ordinal)) 'scroll position records actual offset delta'
Assert-True ($scrollPosition.Contains('recordScrollTrace(DorotiFramePhase.scrollEnd)', [StringComparison]::Ordinal)) 'scroll position records actual end'
$scrollNotification = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scroll_notification.cs')
Assert-True ($scrollNotification.Contains('if ((depth is not null))', [StringComparison]::Ordinal) -and
    $scrollNotification.Contains('_depth = DartRuntimePrimitives.RequireValue(', [StringComparison]::Ordinal)) 'reviewed ScrollUpdateNotification preserves regenerated depth assignment'
Assert-True ($scrollPosition.Contains('depth: this.depth', [StringComparison]::Ordinal)) 'metrics-to-update conversion forwards current notification depth'

$sceneBuilder = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Ui/GraphicsAndSemanticsContracts.cs')
Assert-True ($sceneBuilder.Contains('Rect? CanvasBounds,', [StringComparison]::Ordinal)) 'scene picture carries raster-cache bounds'
Assert-True ($sceneBuilder.Contains('bool IsComplexHint,', [StringComparison]::Ordinal) -and
    $sceneBuilder.Contains('bool WillChangeHint);', [StringComparison]::Ordinal)) 'scene picture carries Flutter raster-cache hints'
$pictureLayer = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Rendering/layer.cs')
Assert-True ($pictureLayer.Contains('this.canvasBounds,', [StringComparison]::Ordinal)) 'picture layer forwards cache bounds to the host'
Assert-True ($sceneBuilder.Contains('object? CacheKey = null,', [StringComparison]::Ordinal) -and
    $sceneBuilder.Contains('long CacheGeneration = 0);', [StringComparison]::Ordinal)) 'scene image filter carries stable cache identity and subtree generation'
Assert-True ($pictureLayer.Contains('_filterCacheGeneration++;', [StringComparison]::Ordinal) -and
    $pictureLayer.Contains('cacheKey: this, cacheGeneration: this._filterCacheGeneration', [StringComparison]::Ordinal)) 'image filter layer invalidates its retained output when the subtree changes'
$mauiRaster = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiSkiaCapabilities.cs')
Assert-True ($mauiRaster.Contains('PictureRasterWarmupFrames = 2', [StringComparison]::Ordinal)) 'picture raster cache requires retained reuse before promotion'
Assert-True ($mauiRaster.Contains('payload.WillChangeHint', [StringComparison]::Ordinal)) 'picture raster cache rejects changing content'
Assert-True ($mauiRaster.Contains('MaxPictureRasterPixels', [StringComparison]::Ordinal) -and
    $mauiRaster.Contains('TrimPictureRasterCache()', [StringComparison]::Ordinal)) 'picture raster cache is memory bounded'
Assert-True ($mauiRaster.Contains('MaxImageFilterResources = 64', [StringComparison]::Ordinal) -and
    $mauiRaster.Contains('GetImageFilter(backdrop.Filter)', [StringComparison]::Ordinal)) 'retained backdrop filters reuse a bounded native filter resource'
$mauiHost = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiHostAdapter.cs')
Assert-True ($mauiHost.Contains('CompositionTarget.Rendering += HandleCompositionRendering;', [StringComparison]::Ordinal) -and
    $mauiHost.Contains('CompositionTarget.Rendering -= HandleCompositionRendering;', [StringComparison]::Ordinal)) 'Windows host scopes compositor vsync to active framework animations'
Assert-True ($mauiHost.Contains('_view.Dispatcher.Dispatch(UpdateCompositionVsyncSubscription)', [StringComparison]::Ordinal)) 'Windows host mutates the WinUI compositor event only through the MAUI UI dispatcher'
Assert-True ($mauiHost.Contains('_pendingFrameCallback is null && _compositionVsyncRequested', [StringComparison]::Ordinal)) 'Windows host stops its vsync waiter after the terminal animation frame'
Assert-True ($mauiHost.Contains('MinimumCompositionFrameInterval = TimeSpan.FromMilliseconds(10)', [StringComparison]::Ordinal) -and
    $mauiHost.Contains('timestamp - _lastCompositionInvalidateTimestamp < MinimumCompositionFrameInterval', [StringComparison]::Ordinal)) 'Windows applies high-refresh backpressure before the ANGLE swap chain'
Assert-True ($mauiHost.Contains('Android.Views.Choreographer.Instance!.PostFrameCallback(_androidFrameCallback)', [StringComparison]::Ordinal) -and
    $mauiHost.Contains('Android.Views.Choreographer.Instance!.RemoveFrameCallback(_androidFrameCallback)', [StringComparison]::Ordinal)) 'Android host scopes framework animation requests to native display vsync'
Assert-True ($mauiHost.Contains('nativeVsyncTimestamp ?? DorotiFrameClock.Now', [StringComparison]::Ordinal) -and
    $mauiHost.Contains('compositorOwnsNextFrame = _androidFrameCallbackPosted', [StringComparison]::Ordinal)) 'Android frame timestamps and follow-up paints remain owned by Choreographer pacing'
Assert-True ($mauiHost.Contains('_androidActiveTouchPointers.Count > 0', [StringComparison]::Ordinal) -and
    $mauiHost.Contains('_pendingFrameCallback is null && _androidActiveTouchPointers.Count == 0', [StringComparison]::Ordinal)) 'Android keeps the display waiter pre-armed only while native touch is active'
Assert-True ($mauiHost.Contains('_androidVsyncTimestamp = MapAndroidFrameTimestamp(frameTimeNanos)', [StringComparison]::Ordinal)) 'Android refreshes a delayed render request to the newest display-pulse timestamp'
$mauiSurface = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/DorotiMauiSurface.cs')
Assert-True ($mauiSurface.Contains('Dispatcher.DispatchDelayed(TimeSpan.Zero, () => CompleteNativePaint(completed))', [StringComparison]::Ordinal) -and
    $mauiRaster.Contains('"native frame submitted"', [StringComparison]::Ordinal)) 'Windows records presentation after the native SKSwapChainPanel flush boundary'
Assert-True ($mauiSurface.Contains('_ = Task.Run(() =>', [StringComparison]::Ordinal) -and
    $mauiSurface.Contains('ScheduleEvidenceWrite();', [StringComparison]::Ordinal) -and
    $mauiSurface.Contains('EvidenceWriteQuiescence', [StringComparison]::Ordinal)) 'live evidence waits for paint quiescence and stays off the native paint callback'
Assert-True ($mauiRaster.Contains('#if !WINDOWS', [StringComparison]::Ordinal) -and
    $mauiRaster.Contains('canvas.Flush();', [StringComparison]::Ordinal)) 'Windows leaves canvas and context flushing to SKSwapChainPanel'
$imageFilterRenderer = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Skia.RuntimeEffects/DorotiSkiaImageFilterRenderer.cs')
Assert-True ($imageFilterRenderer.Contains('cached.Generation != generation', [StringComparison]::Ordinal)) 'image filter output cache rejects stale subtree generations'
$appTargetGate = Read-Text (Join-Path $dorotiRoot 'eng/validate-app-targets.ps1')
foreach ($measurement in @('inputToOffsetMilliseconds', 'offsetToPresentMilliseconds', 'inputToPresentMilliseconds', 'animationPresentGaps')) {
    Assert-True ($appTargetGate.Contains($measurement, [StringComparison]::Ordinal)) "Windows live scroll measurement: $measurement"
}
Assert-True ($appTargetGate.Contains('offset-to-present max 60 Hz budget', [StringComparison]::Ordinal)) 'Windows live gate rejects a single visible scroll hitch'
Assert-True ($appTargetGate.Contains('input-to-present excellent-frame budget', [StringComparison]::Ordinal)) 'Windows live gate enforces the complete 16.6 ms visible interaction budget'

Invoke-LowererFixture 'Debug'
Invoke-LowererFixture 'Release'
$debugRegenerationHash = Invoke-ScrollNotificationRegeneration 'Debug'
$releaseRegenerationHash = Invoke-ScrollNotificationRegeneration 'Release'
Assert-True ($debugRegenerationHash -eq $releaseRegenerationHash) 'scroll_notification regeneration is configuration-independent and deterministic'
Invoke-Contract 'Debug'
Invoke-Contract 'Release'
$evidence = [ordered]@{
    schemaVersion = 'doroti.flutter-conformance-fcr5-evidence/v1'
    status = 'partial'
    capturedAt = [DateTime]::UtcNow.ToString('o')
    repositoryRevision = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    flutterRevision = $flutterRevision
    fixtureManifest = 'Doroti/validation/fcr5-scroll/fixture-manifest.json'
    beforeFix = [ordered]@{ status = 'expectedFailure'; evidence = 'Doroti/validation/fcr5-scroll/expected-failure.json'; expectedDepth = 1; reviewedGeneratedDepth = 0 }
    runtimeContract = [ordered]@{
        status = 'pass'; debug = 'pass'; release = 'pass'
        checks = @('ScrollController.animateTo waits for the initial attached-position snapshot', 'DrivenScrollActivity initializes and observes its animation', 'Flutter method tear-offs keep stable CLR listener identity', 'scroll listener removal leaves no retained ChangeNotifier callback', 'ScrollMetricsNotification preserves depth 0, 1, and 2 through asScrollUpdate', 'default predicate accepts only depth 0', 'nested inner start/update/end/viewport metrics update only the inner painter while outer metrics/thumb/fade ownership stay unchanged', 'reverse outer update leaves inner ownership unchanged', 'scroll trace preserves one input sequence through its declared causal phases', 'frame trace retains actual scroll offsets and animation ownership for a complete high-refresh gesture', 'trace capacity is bounded without sequence reuse')
    }
    lowererContract = [ordered]@{
        status = 'pass'; debug = 'pass'; release = 'pass'; regenerationSha256 = $releaseRegenerationHash
        checks = @('nullable named depth parameter', 'super-formal forwarding', 'constructor block null check', 'mixin-private depth assignment', 'actual pinned scroll_notification.dart regeneration', 'Debug/Release deterministic output')
    }
    ownershipContract = [ordered]@{
        status = 'pass'
        checks = @('ScrollView applies PrimaryScrollController.shouldInherit', 'RawScrollbar uses explicit controller or the inherited primary controller', 'scroll viewport, semantics, scrollbar, overscroll, nested-scroll, and ticker paths use stable listener identities', 'ScrollbarPainter observes and removes its fade animation listener symmetrically', 'PictureLayer forwards bounds and change hints to a bounded retained raster cache', 'ImageFilterLayer uses a stable cache identity with subtree generation invalidation', 'Windows drives framework animations from WinUI compositor vsync and marshals compositor subscriptions to the UI thread', 'Android keeps Choreographer pre-armed during native drag and advances delayed paints to the newest display pulse', 'Windows records present after the native swap-chain flush and keeps evidence serialization off the paint callback', 'Windows live gate measures input-to-offset, offset-to-present, complete input-to-present, and scroll-animation cadence including maxima')
    }
    acceptance = [ordered]@{
        status = 'notVerified'
        reason = 'This structural contract does not execute Flutter-reference differential, a real native pointer-to-present capture, lazy-child/cache measurement, Android physical 60-second scroll, or Windows live wheel/drag.'
        notRun = @('Flutter drag/hold/ballistic/driven differential', 'lazy child create/dispose and keepAlive/cacheExtent measurement', 'text/shader/paint cache hit-miss-eviction measurement', 'Android physical 60-second alternating drag/ballistic scroll', 'Windows native wheel/drag presentation and process-survival acceptance')
    }
}
[IO.Directory]::CreateDirectory((Split-Path $evidencePath -Parent)) | Out-Null
[IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 24) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output 'Doroti FCR-5 scroll validation: PASS (runtime Debug/Release and ownership contracts; reference/live/physical acceptance remains notVerified)'
