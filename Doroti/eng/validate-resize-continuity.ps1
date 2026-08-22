#Requires -Version 7.0
param(
    [ValidateSet('Contract')]
    [string] $Shard = 'Contract'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Read-Source([string] $RelativePath) {
    $path = Join-Path $dorotiRoot $RelativePath
    Assert-True (Test-Path -LiteralPath $path -PathType Leaf) "source $RelativePath"
    return Get-Content -LiteralPath $path -Raw
}

function Get-SourceFingerprint([string[]] $RelativePaths) {
    $builder = [Text.StringBuilder]::new()
    foreach ($relativePath in ($RelativePaths | Sort-Object)) {
        $path = Join-Path $dorotiRoot $relativePath
        $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
        [void] $builder.Append($relativePath.Replace('\', '/')).Append('=').Append($hash).Append("`n")
    }
    $bytes = [Text.Encoding]::UTF8.GetBytes($builder.ToString())
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant()
}

if ($Shard -eq 'Contract') {
    $sources = @(
        'src/Doroti.Ui/ResizeLifecycle.cs',
        'src/Doroti.Ui/PlatformDispatcher.cs',
        'src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs',
        'src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs',
        'src/Doroti.Host.Maui/MauiHostAdapter.cs',
        'src/Doroti.Host.Web/BrowserHostContracts.cs',
        'src/Doroti.Host.Web/DorotiWebGlSurface.razor',
        'src/Doroti.Host.Web/Web/doroti.web.ts',
        'src/Doroti.Host.Web/wwwroot/doroti.web.css',
        'validation/resize-contract/Program.cs',
        'eng/validate-resize-continuity-live.ps1',
        'eng/validate-web-resize-continuity-live.ps1',
        '../DorotiDemoApp/web/DorotiDemoApp.Web.csproj',
        'src/Doroti.Host.Maui/DorotiMauiSurface.cs'
    )
    $resizeContract = Read-Source $sources[0]
    $dispatcher = Read-Source $sources[1]
    $renderer = Read-Source $sources[2]
    $windowsSurface = Read-Source $sources[3]
    $mauiHost = Read-Source $sources[4]
    $browserHost = Read-Source $sources[5]
    $webSurface = Read-Source $sources[6]
    $webHost = Read-Source $sources[7]
    $webCss = Read-Source $sources[8]
    $validation = Read-Source $sources[9]
    $windowsLiveValidation = Read-Source $sources[10]
    $webLiveValidation = Read-Source $sources[11]
    $webRunner = Read-Source $sources[12]
    $mauiSurface = Read-Source $sources[13]

    Assert-True ($resizeContract.Contains('public sealed record DorotiFrameDescriptor(', [StringComparison]::Ordinal) -and
        $resizeContract.Contains('public sealed class DorotiLatestFrameMailbox<T>', [StringComparison]::Ordinal) -and
        $resizeContract.Contains('public sealed class DorotiFrameTerminalLedger', [StringComparison]::Ordinal)) 'common descriptor, bounded mailbox, and terminal ledger'
    Assert-True ($resizeContract.Contains('private const int Capacity = 16384;', [StringComparison]::Ordinal) -and
        $resizeContract.Contains('System.Diagnostics.Stopwatch.GetTimestamp()', [StringComparison]::Ordinal)) 'resize evidence retains a full live window and cross-process QPC timestamps'
    Assert-True ($resizeContract.Contains('public sealed record DorotiViewEpoch(', [StringComparison]::Ordinal) -and
        $resizeContract.Contains('public sealed record DorotiSceneBuildToken(', [StringComparison]::Ordinal) -and
        $dispatcher.Contains('EnterSceneBuildScope(view.CaptureViewEpoch()', [StringComparison]::Ordinal) -and
        $dispatcher.Contains('token.WithRootPhysicalSize(width, height)', [StringComparison]::Ordinal)) 'framework frame captures viewport epoch and root physical size'
    Assert-True ($renderer.Contains('frame?.Descriptor.MatchExact(', [StringComparison]::Ordinal) -and
        $renderer.Contains('DorotiFrameDescriptor.FromBuildToken(buildToken, sceneSequence)', [StringComparison]::Ordinal) -and
        -not $renderer.Contains('var target = _host.ResizeTarget;', [StringComparison]::Ordinal) -and
        $renderer.Contains('DorotiFrameTerminal.superseded', [StringComparison]::Ordinal) -and
        $renderer.Contains('_presentedFrame = frame;', [StringComparison]::Ordinal)) 'renderer exact-size reject and retained-scene contract'
    Assert-True ($renderer.Contains('var commands = payload.Commands;', [StringComparison]::Ordinal) -and
        -not $renderer.Contains('ObjectDisposedException.ThrowIf(picture.debugDisposed, picture);', [StringComparison]::Ordinal)) 'raster owns immutable picture commands beyond Dart handle disposal'
    Assert-True ($validation.Contains('maxQueueDepth <= 2', [StringComparison]::Ordinal) -and
        $validation.Contains('stale generation presents remain zero', [StringComparison]::Ordinal) -and
        $validation.Contains('surface/context recreation', [StringComparison]::Ordinal)) 'deterministic state-machine assertions'

    Assert-True ($windowsSurface.Contains('WindowsD3D12Presenter', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('dual-exact-staging-SwapChainPanel/DXGI-D3D12-Skia', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('TryCommitViewportAndPresent(', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('swapChain2.MatrixTransform = Matrix3x2.CreateScale', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('private IDXGISwapChain3? _renderSwapChain;', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('private IDXGISwapChain3? _presentedSwapChain;', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('AttachSwapChainOnUiThread(panel, next);', [StringComparison]::Ordinal) -and
        -not $windowsSurface.Contains('SetSourceSize(', [StringComparison]::Ordinal)) 'Windows owned dual exact staging presenter and DPI mapping'
    Assert-True ($windowsSurface.Contains('SizeChanged?.Invoke(target);', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('panel.CompositionScaleX', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('panel.CompositionScaleY', [StringComparison]::Ordinal) -and
        $mauiHost.Contains('private void HandleSizeChanged(DorotiResizeEpoch? publishedTarget)', [StringComparison]::Ordinal) -and
        $mauiHost.Contains('target = publishedTarget;', [StringComparison]::Ordinal)) 'Windows panel payload is the single metrics authority'
    Assert-True ($windowsSurface.Contains('_latestTarget?.Generation != target.Generation', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('terminal: "superseded"', [StringComparison]::Ordinal)) 'Windows latest target pre-present gate'
    Assert-True ($windowsSurface.Contains('pre-raster latest target gate', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('paint.SkipRaster', [StringComparison]::Ordinal) -and
        $mauiSurface.Contains('if (!paint.SkipRaster)', [StringComparison]::Ordinal)) 'Windows stale prepared target releases host paint backpressure without Skia raster'
    Assert-True ($windowsSurface.Contains('pre-flush latest target gate', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('Record("paint-end"', [StringComparison]::Ordinal)) 'Windows rejects a target superseded during Skia paint before GPU flush'
    Assert-True ($windowsSurface.Contains('newerTargetKnownAtPrePresent=0', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('UI-thread final target gate', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('uiCommitTargetGeneration', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('previous exact swap chain', [StringComparison]::Ordinal) -and
        -not $windowsSurface.Contains('scheduler latest-work gate', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('targetAdvancedDuringPresent', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('postPresentObservedGeneration', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('ReleasePresentedBuffer()', [StringComparison]::Ordinal)) 'Windows final present race and buffer release are measured separately'
    Assert-True ($windowsLiveValidation.Contains("[ValidateSet('Left', 'Right', 'Top', 'Bottom', 'TopLeft', 'TopRight', 'BottomLeft', 'BottomRight')]", [StringComparison]::Ordinal) -and
        $windowsLiveValidation.Contains('[DorotiResizeLiveNative]::HTLEFT', [StringComparison]::Ordinal) -and
        $windowsLiveValidation.Contains('[DorotiResizeLiveNative]::HTTOPLEFT', [StringComparison]::Ordinal) -and
        $windowsLiveValidation.Contains('[int] $DurationSeconds = 60', [StringComparison]::Ordinal) -and
        $windowsLiveValidation.Contains('[switch] $KeepWindowOpen', [StringComparison]::Ordinal) -and
        $windowsLiveValidation.Contains('outsideWorkAreaSamples -ne 0', [StringComparison]::Ordinal) -and
        $windowsLiveValidation.Contains('uiCommitTargetGeneration=', [StringComparison]::Ordinal) -and
        $windowsLiveValidation.Contains('$summary.latestTargetAtPresentedAck.laggedAckCount -ne 0', [StringComparison]::Ordinal)) 'Windows live validation covers edge directions and final UI commit generation'
    Assert-True ($windowsSurface.Contains('Prepare the detached exact staging chain in parallel', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('serial == processedSerial', [StringComparison]::Ordinal) -and
        -not [regex]::IsMatch($windowsSurface, '_latestTarget = target;\s*_requestSerial\+\+;')) 'Windows prepares resize in parallel without painting before an exact scene'
    Assert-True (-not $mauiHost.Contains('DwmFlush', [StringComparison]::Ordinal) -and
        -not $mauiHost.Contains('UpdateLayout()', [StringComparison]::Ordinal)) 'Windows UI host has no synchronous render wait'
    Assert-True (-not $mauiHost.Contains('MinimumCompositionFrameInterval', [StringComparison]::Ordinal) -and
        $mauiHost.Contains('CompositionTarget already paces callbacks to the active display.', [StringComparison]::Ordinal)) 'Windows DXGI frame requests use native display cadence'
    Assert-True ($mauiHost.Contains('DispatchWindowsResizeFrame();', [StringComparison]::Ordinal) -and
        $mauiHost.Contains('while raster prepares a detached exact staging chain.', [StringComparison]::Ordinal)) 'Windows overlaps resize metrics frame build with detached staging preparation'
    foreach ($legacy in @(
        'src/Doroti.Host.Maui/DorotiWindowsSkiaViewHandler.cs',
        'src/Doroti.Host.Maui/WindowsEglInterop.cs',
        'src/Doroti.Host.Maui/WindowsResizeContinuityGuard.cs')) {
        Assert-True (-not (Test-Path -LiteralPath (Join-Path $dorotiRoot $legacy))) "legacy Windows path removed: $legacy"
    }

    Assert-True ($webHost.Contains('observer.observe(root, { box:', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('observer.observe(canvas)', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('addEventListener("resize"', [StringComparison]::Ordinal)) 'Web root-only size authority'
    Assert-True ($webHost.Contains('presenter.drainScheduled || presenter.current', [StringComparison]::Ordinal) -and
        $webHost.Contains('queueMicrotask(() =>', [StringComparison]::Ordinal) -and
        $webHost.Contains('presenter.latest = descriptor;', [StringComparison]::Ordinal) -and
        $browserHost.Contains('Action<TimeSpan>? _pendingFrame;', [StringComparison]::Ordinal)) 'Web framework-rAF plus same-turn latest-only presenter queue'
    Assert-True ($webHost.Contains('function observeResizeEntry(', [StringComparison]::Ordinal) -and
        $webHost.Contains('entry.devicePixelContentBoxSize', [StringComparison]::Ordinal) -and
        $webHost.Contains('function applyProvisionalEpoch(', [StringComparison]::Ordinal) -and
        $webHost.Contains('function commitCanvasEpoch(', [StringComparison]::Ordinal) -and
        $webHost.Contains('host.canvas.width = target.physicalWidth;', [StringComparison]::Ordinal) -and
        $webHost.Contains('policy: "target-sized-default-top-left-crop"', [StringComparison]::Ordinal) -and
        $webHost.Contains('const copyWidth = Math.min(front.width, target.physicalWidth);', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('scheduleHostSample', [StringComparison]::Ordinal)) 'Web observer synchronously commits a target-sized non-scaling provisional canvas'
    Assert-True ($webLiveValidation.Contains('Get-AppBarLogicalHeight', [StringComparison]::Ordinal) -and
        $webLiveValidation.Contains('Get-CircularControlAspect', [StringComparison]::Ordinal) -and
        $webLiveValidation.Contains('[int] $SampleCount = 300', [StringComparison]::Ordinal) -and
        $webLiveValidation.Contains('[int] $PostResizeObservationSeconds = 10', [StringComparison]::Ordinal) -and
        $webLiveValidation.Contains("Send-Cdp `$socket 'Browser.setWindowBounds'", [StringComparison]::Ordinal) -and
        $webLiveValidation.Contains('$pixelDelta -gt $tolerancePhysicalPixels', [StringComparison]::Ordinal) -and
        $webLiveValidation.Contains('circularControlObservedSamples', [StringComparison]::Ordinal)) 'Web live validation covers fixed-height and raster-tolerant circular-control geometry'
    Assert-True (-not [regex]::IsMatch($webCss, '(?s)\.doroti-root\s*>\s*canvas\s*\{[^}]*\bwidth\s*:\s*100%') -and
        -not [regex]::IsMatch($webCss, '(?s)\.doroti-root\s*>\s*canvas\s*\{[^}]*\bheight\s*:\s*100%') -and
        $webHost.Contains('host.canvas.style.width =', [StringComparison]::Ordinal) -and
        $webHost.Contains('host.canvas.style.height =', [StringComparison]::Ordinal)) 'Web canvas CSS size is owned by the JS epoch'
    Assert-True ($webHost.Contains('runtime.framebuffers[framebufferId] = framebuffer;', [StringComparison]::Ordinal) -and
        $webHost.Contains('new URLSearchParams', [StringComparison]::Ordinal) -and
        $webHost.Contains('gl.blitFramebuffer(', [StringComparison]::Ordinal) -and
        $webHost.Contains('antialias: 0', [StringComparison]::Ordinal) -and
        $webSurface.Contains('_framebuffer != framebuffer', [StringComparison]::Ordinal)) 'Web app-owned single-sample FBO and framebuffer-identity wrapper contract'
    Assert-True ($webHost.Contains('presenter.glStateDirty = true;', [StringComparison]::Ordinal) -and
        $webHost.Contains('consumePresenterGlStateDirty', [StringComparison]::Ordinal) -and
        $webSurface.Contains('ResetContext(GRGlBackendState.All)', [StringComparison]::Ordinal) -and
        $webHost.Contains('gl.disable(gl.SCISSOR_TEST);', [StringComparison]::Ordinal)) 'Web JS and Skia declare GL state ownership'
    Assert-True ($webHost.Contains('presenter.front = staging;', [StringComparison]::Ordinal) -and
        $webHost.Contains('presenter.staging = previousFront;', [StringComparison]::Ordinal) -and
        $webHost.Contains('"CompleteFrame"', [StringComparison]::Ordinal) -and
        $renderer.Contains('public void SupersedePaint(', [StringComparison]::Ordinal)) 'Web retained front/staging swap and managed terminal handoff'
    Assert-True ($webHost.Contains('terminalRecorded: boolean;', [StringComparison]::Ordinal) -and
        $webHost.Contains('if (descriptor.terminalRecorded) return;', [StringComparison]::Ordinal) -and
        $webHost.Contains('presenter.contextGeneration++;', [StringComparison]::Ordinal)) 'Web context recreation and exactly-once presenter terminal guard'
    Assert-True (-not $webHost.Contains('readPixels', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('toDataURL', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('getImageData', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('preserveDrawingBuffer: 1', [StringComparison]::Ordinal)) 'Web product presenter has no CPU full-frame readback or preserved default buffer'
    Assert-True ($webSurface.Contains('[DllImport("libSkiaSharp", EntryPoint = "DorotiInterceptBrowserObjects")]', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('installCanvasResizeContinuity', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('SKHtmlCanvas.requestAnimationFrame', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('renderFrameCallback =', [StringComparison]::Ordinal)) 'Doroti-owned WebGL presenter without runtime monkey patch'
    Assert-True ($webRunner.Contains('<WasmBuildNative>true</WasmBuildNative>', [StringComparison]::Ordinal) -and
        $webRunner.Contains('DorotiSkiaInterop.js', [StringComparison]::Ordinal)) 'Web runner native GL interop link'

    Invoke-Checked {
        dotnet run --project (Join-Path $dorotiRoot 'validation/resize-contract/Doroti.Validation.ResizeContract.csproj') `
            -c Release --no-restore --nologo
    } 'resize state-machine validation failed'
    Invoke-Checked {
        dotnet build (Join-Path $dorotiRoot 'src/Doroti.Host.Web/Doroti.Host.Web.csproj') `
            -c Release --no-restore --nologo
    } 'Web resize contract build failed'
    Invoke-Checked {
        dotnet build (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/Doroti.Host.Maui.csproj') -c Release `
            -f net10.0-windows10.0.19041.0 --no-restore --nologo
    } 'Windows resize contract build failed'

    $result = [ordered]@{
        schemaVersion = 'doroti.resize-continuity-contract/v3'
        status = 'PASS'
        shard = $Shard
        sourceFingerprint = Get-SourceFingerprint $sources
        sourceFiles = $sources.Count
        legacyWindowsFilesPresent = 0
    }
    Write-Output ($result | ConvertTo-Json -Compress)
}

Write-Output "Doroti resize continuity shard '$Shard': PASS"
