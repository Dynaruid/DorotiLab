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
        'src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs',
        'src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs',
        'src/Doroti.Host.Maui/MauiHostAdapter.cs',
        'src/Doroti.Host.Web/BrowserHostContracts.cs',
        'src/Doroti.Host.Web/DorotiWebGlSurface.razor',
        'src/Doroti.Host.Web/Web/doroti.web.ts',
        'validation/resize-contract/Program.cs',
        '../DorotiDemoApp/web/DorotiDemoApp.Web.csproj'
    )
    $resizeContract = Read-Source $sources[0]
    $renderer = Read-Source $sources[1]
    $windowsSurface = Read-Source $sources[2]
    $mauiHost = Read-Source $sources[3]
    $browserHost = Read-Source $sources[4]
    $webSurface = Read-Source $sources[5]
    $webHost = Read-Source $sources[6]
    $validation = Read-Source $sources[7]
    $webRunner = Read-Source $sources[8]

    Assert-True ($resizeContract.Contains('public sealed record DorotiFrameDescriptor(', [StringComparison]::Ordinal) -and
        $resizeContract.Contains('public sealed class DorotiLatestFrameMailbox<T>', [StringComparison]::Ordinal) -and
        $resizeContract.Contains('public sealed class DorotiFrameTerminalLedger', [StringComparison]::Ordinal)) 'common descriptor, bounded mailbox, and terminal ledger'
    Assert-True ($renderer.Contains('frame.Descriptor.IsExactFor(desiredTarget)', [StringComparison]::Ordinal) -and
        $renderer.Contains('DorotiFrameTerminal.superseded', [StringComparison]::Ordinal) -and
        $renderer.Contains('_presentedFrame = frame;', [StringComparison]::Ordinal)) 'renderer exact-size reject and retained-scene contract'
    Assert-True ($validation.Contains('maxQueueDepth <= 2', [StringComparison]::Ordinal) -and
        $validation.Contains('stale generation presents remain zero', [StringComparison]::Ordinal) -and
        $validation.Contains('surface/context recreation', [StringComparison]::Ordinal)) 'deterministic state-machine assertions'

    Assert-True ($windowsSurface.Contains('WindowsD3D12Presenter', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('Doroti-owned-SwapChainPanel/DXGI-D3D12-Skia', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('presenter.Present();', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('swapChain2.MatrixTransform = Matrix3x2.CreateScale', [StringComparison]::Ordinal)) 'Windows owned DXGI presenter and DPI mapping'
    Assert-True ($windowsSurface.Contains('_latestTarget?.Generation != target.Generation', [StringComparison]::Ordinal) -and
        $windowsSurface.Contains('terminal: "superseded"', [StringComparison]::Ordinal)) 'Windows latest target pre-present gate'
    Assert-True (-not $mauiHost.Contains('DwmFlush', [StringComparison]::Ordinal) -and
        -not $mauiHost.Contains('UpdateLayout()', [StringComparison]::Ordinal)) 'Windows UI host has no synchronous render wait'
    foreach ($legacy in @(
        'src/Doroti.Host.Maui/DorotiWindowsSkiaViewHandler.cs',
        'src/Doroti.Host.Maui/WindowsEglInterop.cs',
        'src/Doroti.Host.Maui/WindowsResizeContinuityGuard.cs')) {
        Assert-True (-not (Test-Path -LiteralPath (Join-Path $dorotiRoot $legacy))) "legacy Windows path removed: $legacy"
    }

    Assert-True ($webHost.Contains('observer.observe(root)', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('observer.observe(canvas)', [StringComparison]::Ordinal) -and
        -not $webHost.Contains('addEventListener("resize"', [StringComparison]::Ordinal)) 'Web root-only size authority'
    Assert-True ($webHost.Contains('presenter.raf !== 0 || presenter.current', [StringComparison]::Ordinal) -and
        $webHost.Contains('presenter.latest = descriptor;', [StringComparison]::Ordinal) -and
        $browserHost.Contains('Action<TimeSpan>? _pendingFrame;', [StringComparison]::Ordinal)) 'Web single-rAF and latest-only queues'
    Assert-True ($webHost.Contains('const backingStoreChanged = presenter.canvas.width !== descriptor.physicalWidth', [StringComparison]::Ordinal) -and
        $webHost.Contains('if (host && backingStoreChanged)', [StringComparison]::Ordinal)) 'Web backing store changes only for a new physical size'
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
