#Requires -Version 7.0
<#
.SYNOPSIS
Preview or remove regenerable project output and optional user caches.
.DESCRIPTION
Preview is the default. Add -Execute to delete the listed targets.
Project cleanup includes every release under Doroti/artifacts, temporary test
runs, .doroti/cache and .doroti/tmp, and discovered build output. Logs, captures
and traces in those disposable directories are also removed.

DeveloperCaches includes NuGet, npm, Python, Dart and VS Code caches, plus
extension directories marked obsolete by VS Code. AppCaches includes selected
Chrome, Figma and CoreDevice caches on macOS. All selects all three scopes.
Installed SDKs, simulator runtimes, Docker data and browser profiles are excluded.
Close builds and the affected applications before executing cache cleanup.
.EXAMPLE
pwsh -NoProfile -File ./cleanup-plan-all.ps1
.EXAMPLE
pwsh -NoProfile -File ./cleanup-plan-all.ps1 -Execute
.EXAMPLE
pwsh -NoProfile -File ./cleanup-plan-all.ps1 -Scope All
.EXAMPLE
pwsh -NoProfile -File ./cleanup-plan-all.ps1 -Scope All -Execute
.EXAMPLE
pwsh -NoProfile -File ./cleanup-plan-all.ps1 -IncludeDependencies -Execute -WhatIf
#>
[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
param(
    [switch] $Execute,
    [ValidateSet('Project', 'DeveloperCaches', 'AppCaches', 'All')]
    [string] $Scope = 'Project',
    [switch] $IncludeDependencies,
    # Alternate profile layout, also useful for isolated cleanup testing.
    [string] $UserCacheRoot
)

$ErrorActionPreference = 'Stop'
$workspaceRoot = [IO.Path]::GetFullPath($PSScriptRoot)
$comparison = if ($IsWindows) { [StringComparison]::OrdinalIgnoreCase } else { [StringComparison]::Ordinal }
$comparer = if ($IsWindows) { [StringComparer]::OrdinalIgnoreCase } else { [StringComparer]::Ordinal }
$targets = [Collections.Generic.List[object]]::new()
$skipped = [Collections.Generic.List[object]]::new()
$trackedPaths = [Collections.Generic.HashSet[string]]::new($comparer)
$projectSelected = $Scope -in @('Project', 'All')
$developerSelected = $Scope -in @('DeveloperCaches', 'All')
$appsSelected = $Scope -in @('AppCaches', 'All')

if (-not (Test-Path -LiteralPath (Join-Path $workspaceRoot 'Doroti/eng/doroti.ps1') -PathType Leaf) -or
    -not (Test-Path -LiteralPath (Join-Path $workspaceRoot '.git'))) {
    throw 'Keep this script in the DorotiLab repository root.'
}

function Get-PathPrefix([string] $Path) {
    return $Path.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
}

function Assert-SafeTarget([string] $Path, [string] $Anchor) {
    if (-not $Path.StartsWith((Get-PathPrefix $Anchor), $comparison)) {
        throw "Target is not inside its allowed root: $Path"
    }
    # Never follow a linked target or ancestor. Links inside an ordinary target
    # are unlinked by PowerShell 7 Remove-Item, without traversing their contents.
    $current = $Path
    while ($current) {
        if (Test-Path -LiteralPath $current) {
            $item = Get-Item -LiteralPath $current -Force
            if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) {
                throw "Linked target or ancestor: $current"
            }
        }
        $current = [IO.Path]::GetDirectoryName($current)
    }
}

function Add-CleanupTarget([string] $Path, [string] $Category, [string] $Anchor) {
    $absolute = [IO.Path]::GetFullPath($Path)
    if (-not (Test-Path -LiteralPath $absolute)) { return }
    try {
        Assert-SafeTarget $absolute $Anchor
        if ($Category -eq 'Project') {
            $relative = [IO.Path]::GetRelativePath($workspaceRoot, $absolute).Replace('\', '/')
            if ($trackedPaths.Contains($relative)) {
                throw 'Contains Git-tracked files; preserve the entire directory.'
            }
        }
    }
    catch {
        $skipped.Add([pscustomobject]@{ Path = $absolute; Reason = $_.Exception.Message })
        return
    }
    foreach ($target in $targets) {
        if ($absolute.Equals($target.Path, $comparison) -or
            $absolute.StartsWith((Get-PathPrefix $target.Path), $comparison)) { return }
    }
    for ($index = $targets.Count - 1; $index -ge 0; $index--) {
        if ($targets[$index].Path.StartsWith((Get-PathPrefix $absolute), $comparison)) {
            $targets.RemoveAt($index)
        }
    }
    $targets.Add([pscustomobject]@{
        Category = $Category
        Path = $absolute
        Anchor = $Anchor
        Bytes = 0L
        Measured = $false
    })
}

function Test-DotNetOutput([IO.DirectoryInfo] $Directory) {
    foreach ($pattern in @('*.csproj', '*.fsproj', '*.vbproj')) {
        if (@($Directory.Parent.EnumerateFiles($pattern)).Count) { return $true }
    }
    # Old standalone build directories may outlive their temporary project.
    # Recognize .NET metadata rather than deleting every directory named bin.
    $pending = [Collections.Generic.Stack[object]]::new()
    $pending.Push(@($Directory, 0))
    while ($pending.Count) {
        $entry = $pending.Pop()
        foreach ($file in $entry[0].EnumerateFiles()) {
            if ($file.Name -eq 'project.assets.json' -or
                $file.Name -like '*.nuget.g.props' -or
                $file.Name -like '*.deps.json' -or
                $file.Name -like '*.runtimeconfig.json') { return $true }
        }
        if ($entry[1] -ge 3) { continue }
        foreach ($child in $entry[0].EnumerateDirectories()) {
            if (-not ($child.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
                $pending.Push(@($child, ($entry[1] + 1)))
            }
        }
    }
    return $false
}

if ($projectSelected) {
    if (-not (Get-Command git -CommandType Application -ErrorAction SilentlyContinue)) {
        throw 'Git is required to protect tracked project files.'
    }
    $repositoryRoot = & git -C $workspaceRoot rev-parse --show-toplevel
    if ($LASTEXITCODE -ne 0 -or
        -not ([IO.Path]::GetFullPath($repositoryRoot)).Equals($workspaceRoot, $comparison)) {
        throw 'The script must be at the Git repository root.'
    }
    $trackedText = (& git -C $workspaceRoot ls-files -z) -join [Environment]::NewLine
    if ($LASTEXITCODE -ne 0) { throw 'Could not read Git-tracked paths.' }
    foreach ($relative in $trackedText.Split([char] 0)) {
        $current = $relative
        while ($current) {
            [void] $trackedPaths.Add($current)
            $separator = $current.LastIndexOf('/')
            if ($separator -lt 0) { break }
            $current = $current.Substring(0, $separator)
        }
    }
    foreach ($relative in @('Doroti/artifacts', 'temp/testing', '.tmp')) {
        Add-CleanupTarget (Join-Path $workspaceRoot $relative) 'Project' $workspaceRoot
    }
    $pending = [Collections.Generic.Stack[IO.DirectoryInfo]]::new()
    $pending.Push([IO.DirectoryInfo]::new($workspaceRoot))
    while ($pending.Count) {
        $directory = $pending.Pop()
        foreach ($child in $directory.EnumerateDirectories()) {
            if ($child.Name -in @('.git', '.svn', '.hg') -or
                ($directory.FullName.Equals($workspaceRoot, $comparison) -and $child.Name -eq 'reference')) {
                continue
            }
            if ($child.Attributes -band [IO.FileAttributes]::ReparsePoint) { continue }
            $covered = $false
            foreach ($target in $targets) {
                if ($child.FullName.Equals($target.Path, $comparison) -or
                    $child.FullName.StartsWith((Get-PathPrefix $target.Path), $comparison)) {
                    $covered = $true
                    break
                }
            }
            if ($covered) { continue }
            if ($child.Name -eq '.doroti') {
                foreach ($name in @('cache', 'tmp')) {
                    Add-CleanupTarget (Join-Path $child.FullName $name) 'Project' $workspaceRoot
                }
                continue
            }
            if ($child.Name -eq 'node_modules') {
                if ($IncludeDependencies) { Add-CleanupTarget $child.FullName 'Project' $workspaceRoot }
                continue
            }
            if ($child.Name -in @('bin', 'obj')) {
                if (Test-DotNetOutput $child) { Add-CleanupTarget $child.FullName 'Project' $workspaceRoot }
                continue
            }
            if ($child.Name -in @('TestResults', 'BenchmarkDotNet.Artifacts', 'DerivedData', '.dart_tool', '.gradle', '.next')) {
                Add-CleanupTarget $child.FullName 'Project' $workspaceRoot
                continue
            }
            $pending.Push($child)
        }
    }
}

$profileRoot = if ([string]::IsNullOrWhiteSpace($UserCacheRoot)) {
    [Environment]::GetFolderPath([Environment+SpecialFolder]::UserProfile)
}
else {
    if (-not [IO.Path]::IsPathRooted($UserCacheRoot)) {
        throw '-UserCacheRoot must be an absolute profile directory.'
    }
    [IO.Path]::GetFullPath($UserCacheRoot)
}
if (($developerSelected -or $appsSelected) -and [string]::IsNullOrWhiteSpace($profileRoot)) {
    throw 'Could not determine the user profile directory.'
}
if ($developerSelected) {
    $relativeCaches = @('.nuget/packages', '.local/share/NuGet/http-cache',
        '.local/share/NuGet/plugins-cache', '.npm/_cacache', '.cache/pip',
        '.cache/uv', '.dartServer/.analysis-driver')
    if ($IsMacOS) {
        $relativeCaches += @('Library/Caches/pip', 'Library/Caches/uv', 'Library/Caches/Homebrew',
            'Library/Application Support/Code/CachedExtensionVSIXs',
            'Library/Application Support/Code/CachedData',
            'Library/Application Support/Code/Cache',
            'Library/Application Support/Code/Code Cache',
            'Library/Application Support/Code/GPUCache',
            'Library/Application Support/Code/Crashpad/completed',
            'Library/Application Support/Code/logs')
    }
    foreach ($relative in $relativeCaches) {
        Add-CleanupTarget (Join-Path $profileRoot $relative) 'DeveloperCaches' $profileRoot
    }
    foreach ($relative in @('.vscode/extensions', '.vscode-server/extensions')) {
        $extensions = Join-Path $profileRoot $relative
        $obsoleteFile = Join-Path $extensions '.obsolete'
        if (-not (Test-Path -LiteralPath $obsoleteFile -PathType Leaf)) { continue }
        Assert-SafeTarget $obsoleteFile $profileRoot
        $obsolete = Get-Content -LiteralPath $obsoleteFile -Raw | ConvertFrom-Json -AsHashtable
        foreach ($name in $obsolete.Keys) {
            if ($obsolete[$name] -ne $true) { continue }
            if ([string]::IsNullOrWhiteSpace($name) -or
                $name -in @('.', '..') -or $name -match '[/\\]') {
                throw "Invalid obsolete extension directory name: $name"
            }
            $path = Join-Path $extensions $name
            if (Test-Path -LiteralPath $path -PathType Container) {
                Add-CleanupTarget $path 'DeveloperCaches' $profileRoot
            }
        }
    }
}
if ($appsSelected) {
    if ($IsMacOS) {
        foreach ($relative in @('Library/Caches/Google/Chrome',
            'Library/Application Support/Google/GoogleUpdater/crx_cache',
            'Library/Containers/com.apple.CoreDevice.CoreDeviceService/Data/Library/Caches/AppInstallationBinaryDeltas')) {
            Add-CleanupTarget (Join-Path $profileRoot $relative) 'AppCaches' $profileRoot
        }
        $figmaProfile = Join-Path $profileRoot 'Library/Application Support/Figma/DesktopProfile'
        if (Test-Path -LiteralPath $figmaProfile -PathType Container) {
            Assert-SafeTarget $figmaProfile $profileRoot
            foreach ($version in ([IO.DirectoryInfo]::new($figmaProfile)).EnumerateDirectories()) {
                if ($version.Name -notmatch '^v\d+$') { continue }
                foreach ($name in @('Cache', 'Code Cache', 'GPUCache')) {
                    Add-CleanupTarget (Join-Path $version.FullName $name) 'AppCaches' $profileRoot
                }
            }
        }
    }
    else { Write-Warning 'AppCaches currently supports macOS only.' }
}

$du = if (-not $IsWindows) { Get-Command du -CommandType Application -ErrorAction SilentlyContinue }
foreach ($target in $targets) {
    try {
        if ($du) {
            $size = & $du.Source -sk $target.Path 2>$null
            if ($LASTEXITCODE -ne 0) { throw 'du could not measure this directory.' }
            $match = [regex]::Match(($size -join [Environment]::NewLine), '^\s*(\d+)\s')
            if (-not $match.Success) { throw 'Unexpected du output.' }
            $target.Bytes = [long] $match.Groups[1].Value * 1024
        }
        else {
            # Fallback reports logical file sizes; skip linked descendants.
            $pending = [Collections.Generic.Stack[IO.DirectoryInfo]]::new()
            $pending.Push([IO.DirectoryInfo]::new($target.Path))
            while ($pending.Count) {
                foreach ($item in $pending.Pop().EnumerateFileSystemInfos()) {
                    if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) { continue }
                    if ($item -is [IO.DirectoryInfo]) { $pending.Push($item) }
                    else { $target.Bytes += $item.Length }
                }
            }
        }
        $target.Measured = $true
    }
    catch { Write-Warning "Size unavailable: $($target.Path): $($_.Exception.Message)" }
}

Write-Host "Workspace: $workspaceRoot"
Write-Host "Scope: $Scope; dependency directories: $IncludeDependencies"
$targets | Sort-Object Bytes -Descending | Select-Object Category,
    @{Name = 'GiB'; Expression = { if ($_.Measured) { '{0:N3}' -f ($_.Bytes / 1GB) } else { '?' } }},
    @{Name = 'Path'; Expression = {
        if ($_.Category -eq 'Project') { [IO.Path]::GetRelativePath($workspaceRoot, $_.Path) }
        else { '~/' + [IO.Path]::GetRelativePath($profileRoot, $_.Path) }
    }} | Format-Table -AutoSize -Wrap
$totalBytes = [long] (($targets | Measure-Object -Property Bytes -Sum).Sum)
$sizeKind = if ($du) { 'allocated' } else { 'logical' }
Write-Host ('Selected: {0} targets; estimated {1} size: {2:N2} GiB. Shared APFS blocks can reduce reclaimed space.' -f
    $targets.Count, $sizeKind, ($totalBytes / 1GB))
if ($skipped.Count) {
    Write-Host 'Skipped targets:'
    $skipped | Format-List Path, Reason
}
if (-not $Execute) {
    Write-Host 'Preview only. Nothing deleted. Add -Execute to remove these targets.'
    return
}

# Validate every selected target before the first deletion, then again per target.
foreach ($target in $targets) { Assert-SafeTarget $target.Path $target.Anchor }
$failures = [Collections.Generic.List[string]]::new()
$removed = 0
foreach ($target in $targets) {
    try {
        Assert-SafeTarget $target.Path $target.Anchor
        if (-not (Test-Path -LiteralPath $target.Path)) { continue }
        if ($PSCmdlet.ShouldProcess($target.Path, 'Delete disposable output or cache')) {
            Remove-Item -LiteralPath $target.Path -Recurse -Force -ErrorAction Stop
            if (Test-Path -LiteralPath $target.Path) { throw 'Target still exists after deletion.' }
            $removed++
            Write-Host "Removed: $($target.Path)"
        }
    }
    catch {
        $failures.Add("$($target.Path): $($_.Exception.Message)")
        Write-Warning $failures[-1]
    }
}
if ($failures.Count) {
    throw "$($failures.Count) target(s) could not be removed. Close processes holding those files and rerun."
}
Write-Host "Cleanup complete. Removed $removed targets."
