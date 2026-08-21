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
    $webHost = Get-Content -Raw (Join-Path $dorotiRoot 'src/Doroti.Host.Web/Web/doroti.web.ts')

    Assert-True ($resizeContract.Contains('public sealed record DorotiResizeEpoch(', [StringComparison]::Ordinal)) 'common resize epoch'
    Assert-True ($resizeContract.Contains('public sealed class DorotiResizeTrace', [StringComparison]::Ordinal)) 'bounded resize trace'
    Assert-True ($windowsGuard.Contains('"swap-boundary-start"', [StringComparison]::Ordinal) -and
        $windowsGuard.Contains('"dwm-flush-end"', [StringComparison]::Ordinal)) 'Windows swap and DWM trace boundaries'
    Assert-True ($windowsGuard.Contains('ExactSwapTimingAvailable = false', [StringComparison]::Ordinal)) 'Windows public SKGLView limitation remains explicit'
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
