#Requires -Version 7.0
<#
.SYNOPSIS
Preview or remove only the disposable files from the 2026-09-29 plan execution.
.EXAMPLE
pwsh -NoProfile -File ./cleanup-plan-all.ps1
.EXAMPLE
pwsh -NoProfile -File ./cleanup-plan-all.ps1 -Execute
#>
[CmdletBinding()]
param([switch] $Execute)

$ErrorActionPreference = 'Stop'
$workspaceRoot = [IO.Path]::GetFullPath($PSScriptRoot)
$workspacePrefix = $workspaceRoot.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
$comparison = [StringComparison]::OrdinalIgnoreCase
$protectedRelative = 'Doroti/artifacts/release/0.3.0-beta.rc.20260929010529'
$protectedPath = [IO.Path]::GetFullPath((Join-Path $workspaceRoot $protectedRelative))

# Fixed allowlist: never discover additional deletion targets by age or wildcard.
$relativeTargets = @(
    'temp/testing/plan-all'
    'temp/testing/developer/d410872debb0425c81716f57d527bd4c'
    'temp/testing/developer/b46c2df3346342a087eb0bca556a1b85'
    'temp/testing/release-candidate/1c38349e85f7'
    'temp/testing/release-candidate/db7bd448acbd'
    'temp/navigation-build.log'
    'Doroti/artifacts/release/0.3.0-beta.rc.20260929000428'
    'Doroti/artifacts/release/0.3.0-beta.rc.20260929000744'
    'Doroti/artifacts/release/0.3.0-beta.rc.20260929001602'
    'Doroti/artifacts/release/0.3.0-beta.rc.20260929002556'
    'Doroti/artifacts/release/0.3.0-beta.rc.20260929004147'
    'Doroti/artifacts/release/0.3.0-beta.rc.20260929005641'
)

if (-not (Test-Path -LiteralPath (Join-Path $workspaceRoot 'plan.md') -PathType Leaf) -or
    -not (Test-Path -LiteralPath (Join-Path $workspaceRoot 'Doroti/eng/doroti.ps1') -PathType Leaf)) {
    throw 'Keep this script in the DorotiLab repository root.'
}

function Assert-SafeTarget([string] $Path) {
    if (-not $Path.StartsWith($workspacePrefix, $comparison)) {
        throw "Target escapes the workspace: $Path"
    }
    $targetPrefix = $Path.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if ($Path.Equals($protectedPath, $comparison) -or
        $protectedPath.StartsWith($targetPrefix, $comparison) -or
        $Path.StartsWith($protectedPath + [IO.Path]::DirectorySeparatorChar, $comparison)) {
        throw "Target overlaps the final candidate: $Path"
    }
    # Check every existing ancestor, including the workspace itself.
    $current = $Path
    while ($current) {
        if (Test-Path -LiteralPath $current) {
            $item = Get-Item -LiteralPath $current -Force
            if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) {
                throw "Refusing a linked path or ancestor: $current"
            }
        }
        $current = [IO.Path]::GetDirectoryName($current)
    }
}

$targets = foreach ($relative in $relativeTargets) {
    $absolute = [IO.Path]::GetFullPath((Join-Path $workspaceRoot $relative))
    Assert-SafeTarget $absolute
    [pscustomobject]@{
        Relative = $relative
        Absolute = $absolute
        Exists = Test-Path -LiteralPath $absolute
    }
}

Write-Host "Workspace: $workspaceRoot"
Write-Host "Preserved final candidate: $protectedRelative"
$targets | Select-Object @{Name='Status'; Expression={ if ($_.Exists) { 'DELETE' } else { 'MISSING' } }}, Relative | Format-Table -AutoSize

if (-not $Execute) {
    Write-Host 'Preview only. Nothing deleted. Add -Execute to delete the listed existing targets.'
    exit 0
}

# Preflight every tree before any mutation. PowerShell 7 does not traverse links
# without -FollowSymlink; reject the links themselves, too.
foreach ($target in $targets) {
    Assert-SafeTarget $target.Absolute
    if (Test-Path -LiteralPath $target.Absolute -PathType Container) {
        Get-ChildItem -LiteralPath $target.Absolute -Recurse -Force | ForEach-Object {
            if ($_.Attributes -band [IO.FileAttributes]::ReparsePoint) {
                throw "Refusing a tree containing a link: $($_.FullName)"
            }
        }
    }
}

$protectedExisted = Test-Path -LiteralPath $protectedPath
$failures = [Collections.Generic.List[string]]::new()
foreach ($target in $targets) {
    try {
        Assert-SafeTarget $target.Absolute
        if (-not (Test-Path -LiteralPath $target.Absolute)) { continue }
        Remove-Item -LiteralPath $target.Absolute -Recurse -Force -ErrorAction Stop
        if (Test-Path -LiteralPath $target.Absolute) { throw 'Target still exists after deletion.' }
        Write-Host "Removed: $($target.Relative)"
    }
    catch {
        $failures.Add("$($target.Relative): $($_.Exception.Message)")
        Write-Warning $failures[-1]
    }
}
if ($protectedExisted -and -not (Test-Path -LiteralPath $protectedPath)) {
    throw "Final candidate is missing: $protectedPath"
}
if ($failures.Count) {
    throw "$($failures.Count) target(s) could not be removed. Close processes holding those files and rerun this script. No processes were stopped by the script."
}
Write-Host 'Cleanup complete. Final candidate and product sources preserved.'
