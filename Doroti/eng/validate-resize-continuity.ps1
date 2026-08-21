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

if ($Shard -eq 'Contract') {
    $resizeContract = Get-Content -Raw (Join-Path $dorotiRoot 'src/Doroti.Ui/ResizeLifecycle.cs')
    $windowsGuard = Get-Content -Raw (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/WindowsResizeContinuityGuard.cs')
    $windowsEgl = Get-Content -Raw (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/WindowsEglInterop.cs')
    $windowsPanel = Get-Content -Raw (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/DorotiWindowsSkiaViewHandler.cs')
    $windowsEtw = Get-Content -Raw (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/WindowsResizeEtw.cs')
    $etwCollector = Get-Content -Raw (Join-Path $dorotiRoot 'eng/collect-resize-continuity-etw.ps1')
    $etwAnalyzer = Get-Content -Raw (Join-Path $dorotiRoot 'eng/analyze-resize-continuity-etw.ps1')
    $mauiApplication = Get-Content -Raw (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/DorotiMauiApplication.cs')
    $webHost = Get-Content -Raw (Join-Path $dorotiRoot 'src/Doroti.Host.Web/Web/doroti.web.ts')

    Assert-True ($resizeContract.Contains('public sealed record DorotiResizeEpoch(', [StringComparison]::Ordinal)) 'common resize epoch'
    Assert-True ($resizeContract.Contains('public sealed class DorotiResizeTrace', [StringComparison]::Ordinal)) 'bounded resize trace'
    Assert-True ($windowsGuard.Contains('"swap-boundary-start"', [StringComparison]::Ordinal) -and
        $windowsGuard.Contains('"dwm-flush-end"', [StringComparison]::Ordinal)) 'Windows swap and DWM trace boundaries'
    Assert-True ($windowsGuard.Contains('"pre-swap"', [StringComparison]::Ordinal) -and
        $windowsGuard.Contains('"post-swap"', [StringComparison]::Ordinal) -and
        $windowsGuard.Contains('ExactSwapTimingAvailable = _exactSwapTimingAvailable', [StringComparison]::Ordinal)) 'Windows final swap timing boundary'
    Assert-True ($windowsEgl.Contains('eglGetCurrentDisplay()', [StringComparison]::Ordinal) -and
        $windowsEgl.Contains('eglGetCurrentSurface(int readdraw)', [StringComparison]::Ordinal) -and
        $windowsEgl.Contains('eglSwapInterval(nint display, int interval)', [StringComparison]::Ordinal) -and
        $windowsEgl.Contains('eglGetError()', [StringComparison]::Ordinal)) 'Windows minimal EGL interop'
    Assert-True (-not $windowsEgl.Contains('eglSwapBuffers(', [StringComparison]::Ordinal)) 'Windows never swaps the SkiaSharp EGL surface directly'
    $renderIndex = $windowsPanel.IndexOf('base.OnRenderFrame(rect);', [StringComparison]::Ordinal)
    $preSwapIndex = $windowsPanel.IndexOf('BeforeFinalSwap?.Invoke', [StringComparison]::Ordinal)
    Assert-True ($renderIndex -ge 0 -and $preSwapIndex -gt $renderIndex) 'Windows pre-swap hook follows Skia flush'
    Assert-True ($windowsPanel.Contains('DrawInBackground = false;', [StringComparison]::Ordinal) -and
        $windowsPanel.Contains('EnableRenderLoop = false;', [StringComparison]::Ordinal)) 'Windows synchronous demand-render contract'
    Assert-True ($windowsPanel.Contains('new(SKGLViewMapper)', [StringComparison]::Ordinal) -and
        $windowsPanel.Contains('MapDorotiIgnorePixelScaling', [StringComparison]::Ordinal)) 'Windows stock handler mappings plus owned scaling mapper'
    Assert-True ($windowsEtw.Contains('Name = "Doroti-Windows-Resize"', [StringComparison]::Ordinal) -and
        $windowsGuard.Contains('EventSource.SetCurrentThreadActivityId(activityId', [StringComparison]::Ordinal)) 'Windows ResizeEpoch ETW activity correlation'
    Assert-True ($etwCollector.Contains("Invoke-Native 'logman.exe'", [StringComparison]::Ordinal) -and
        $etwCollector.Contains("'-merge', `$gpuEtlPath", [StringComparison]::Ordinal)) 'Windows GPU and marker ETW merge collector'
    Assert-True ($etwAnalyzer.Contains('leading =', [StringComparison]::Ordinal) -and
        $etwAnalyzer.Contains('final =', [StringComparison]::Ordinal) -and
        $etwAnalyzer.Contains('primarySyncInterval0', [StringComparison]::Ordinal)) 'Windows activity-correlated Present classifier'
    Assert-True ($mauiApplication.Contains('handlers.AddHandler<SKGLView, DorotiWindowsSkiaViewHandler>()', [StringComparison]::Ordinal)) 'Windows custom Skia handler registration'
    Assert-True ($webHost.Contains('observer.observe(root)', [StringComparison]::Ordinal)) 'Web host element size observer'
    Assert-True (-not $webHost.Contains('observer.observe(canvas)', [StringComparison]::Ordinal)) 'Web canvas observer removal'
    Assert-True (-not $webHost.Contains('host.canvas.style.width', [StringComparison]::Ordinal)) 'Web canvas inline-size writer removal'
    Assert-True ($webHost.Contains('"backing-store"', [StringComparison]::Ordinal) -and
        $webHost.Contains('"managed-skia"', [StringComparison]::Ordinal)) 'Web backing and managed surface trace'

    Invoke-Checked {
        dotnet build (Join-Path $dorotiRoot 'src/Doroti.Host.Web/Doroti.Host.Web.csproj') -c Release --no-restore --nologo
    } 'Web resize contract build failed'
    Invoke-Checked {
        dotnet build (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/Doroti.Host.Maui.csproj') -c Release `
            -f net10.0-windows10.0.19041.0 --no-restore --nologo
    } 'Windows resize contract build failed'
}

Write-Output "Doroti resize continuity shard '$Shard': PASS"
