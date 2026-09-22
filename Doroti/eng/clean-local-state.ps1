#Requires -Version 5.1
param(
    [ValidateSet('status', 'temporary', 'cache', 'artifacts', 'all')]
    [string] $Action = 'status',
    [ValidateRange(0, 87600)]
    [int] $OlderThanHours = 24,
    [switch] $Force
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$localRoot = Get-DorotiLocalRoot -DorotiRoot $dorotiRoot
$temporaryRoot = Get-DorotiTemporaryRoot -DorotiRoot $dorotiRoot
$cacheRoot = Get-DorotiCacheRoot -DorotiRoot $dorotiRoot
$artifactsRoot = Join-Path $dorotiRoot 'artifacts'

function Measure-Directory([string] $Path) {
    $measurement = Get-ChildItem -LiteralPath $Path -File -Recurse -Force -ErrorAction SilentlyContinue |
        Measure-Object -Property Length -Sum
    $bytes = $measurement.Sum
    if ($null -eq $bytes) { $bytes = 0L }
    return [pscustomobject]@{ Files = $measurement.Count; Bytes = [int64]$bytes }
}

function Remove-LocalChildren([string] $Root) {
    if (-not (Test-Path -LiteralPath $Root)) { return 0 }
    $rootItem = Get-Item -LiteralPath $Root -Force
    if ($rootItem.Attributes -band [IO.FileAttributes]::ReparsePoint) {
        throw "Refusing to clean a linked local-state root: $Root"
    }
    $threshold = [DateTime]::UtcNow.AddHours(-$OlderThanHours)
    $children = @(Get-ChildItem -LiteralPath $Root -Force -ErrorAction SilentlyContinue | Where-Object {
        $Force -or $_.LastWriteTimeUtc -lt $threshold
    })
    foreach ($child in $children) {
        $resolved = [IO.Path]::GetFullPath($child.FullName)
        $expectedRoot = [IO.Path]::GetFullPath($Root).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
        if (-not $resolved.StartsWith($expectedRoot, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Refusing to clean a path outside the selected local-state root: $resolved"
        }
        if ($child.Attributes -band [IO.FileAttributes]::ReparsePoint) {
            throw "Refusing to clean a linked local-state entry: $resolved"
        }
        if ($child.PSIsContainer) {
            $linkedDescendants = @(Get-ChildItem -LiteralPath $resolved -Recurse -Force -ErrorAction Stop |
                Where-Object { $_.Attributes -band [IO.FileAttributes]::ReparsePoint })
            if ($linkedDescendants.Count -gt 0) {
                throw "Refusing to clean an entry containing links: $resolved"
            }
        }
        if ($child.PSIsContainer) { [IO.Directory]::Delete($resolved, $true) }
        else { [IO.File]::Delete($resolved) }
    }
    return $children.Count
}

if ($Action -in @('temporary', 'all')) {
    $removedTemporary = Remove-LocalChildren -Root $temporaryRoot
    Write-Output "Removed temporary entries: $removedTemporary"
}
if ($Action -in @('cache', 'all')) {
    $removedCache = Remove-LocalChildren -Root $cacheRoot
    Write-Output "Removed cache entries: $removedCache"
}
if ($Action -in @('artifacts', 'all')) {
    $removedArtifacts = Remove-LocalChildren -Root $artifactsRoot
    Write-Output "Removed artifact entries: $removedArtifacts"
}

$temporary = Measure-Directory -Path $temporaryRoot
$cache = Measure-Directory -Path $cacheRoot
$artifacts = Measure-Directory -Path $artifactsRoot
Write-Output "Doroti local root: $localRoot"
Write-Output ('Temporary: files={0}; bytes={1}; path={2}' -f $temporary.Files, $temporary.Bytes, $temporaryRoot)
Write-Output ('Cache: files={0}; bytes={1}; path={2}' -f $cache.Files, $cache.Bytes, $cacheRoot)
Write-Output ('Artifacts: files={0}; bytes={1}; path={2}' -f $artifacts.Files, $artifacts.Bytes, $artifactsRoot)
if ($Action -eq 'status') {
    Write-Output 'Use -Action temporary, cache, artifacts, or all. Entries newer than -OlderThanHours are preserved unless -Force is specified.'
}
