#Requires -Version 5.1

function Get-DorotiWorkspaceRoot {
    param([Parameter(Mandatory = $true)][string] $DorotiRoot)

    $resolvedDorotiRoot = [IO.Path]::GetFullPath($DorotiRoot).TrimEnd([IO.Path]::DirectorySeparatorChar)
    $parent = [IO.Path]::GetFullPath((Join-Path $resolvedDorotiRoot '..'))
    if (Test-Path -LiteralPath (Join-Path $parent 'tools/Doroti.DartToCSharp')) {
        return $parent
    }
    return $resolvedDorotiRoot
}

function Get-DorotiLocalRoot {
    param([Parameter(Mandatory = $true)][string] $DorotiRoot)

    $workspaceRoot = Get-DorotiWorkspaceRoot -DorotiRoot $DorotiRoot
    $configured = $env:DOROTI_LOCAL_ROOT
    $localRoot = if ([string]::IsNullOrWhiteSpace($configured)) {
        Join-Path $workspaceRoot '.doroti'
    }
    elseif ([IO.Path]::IsPathRooted($configured)) {
        $configured
    }
    else {
        Join-Path $workspaceRoot $configured
    }
    $resolved = [IO.Path]::GetFullPath($localRoot).TrimEnd([IO.Path]::DirectorySeparatorChar)
    [IO.Directory]::CreateDirectory($resolved) | Out-Null
    $env:DOROTI_LOCAL_ROOT = $resolved
    return $resolved
}

function Get-DorotiTemporaryRoot {
    param([Parameter(Mandatory = $true)][string] $DorotiRoot)

    $root = Join-Path (Get-DorotiLocalRoot -DorotiRoot $DorotiRoot) 'tmp'
    [IO.Directory]::CreateDirectory($root) | Out-Null
    return [IO.Path]::GetFullPath($root).TrimEnd([IO.Path]::DirectorySeparatorChar)
}

function Get-DorotiCacheRoot {
    param([Parameter(Mandatory = $true)][string] $DorotiRoot)

    $root = Join-Path (Get-DorotiLocalRoot -DorotiRoot $DorotiRoot) 'cache'
    [IO.Directory]::CreateDirectory($root) | Out-Null
    return [IO.Path]::GetFullPath($root).TrimEnd([IO.Path]::DirectorySeparatorChar)
}

function New-DorotiTemporaryDirectory {
    param(
        [Parameter(Mandatory = $true)][string] $DorotiRoot,
        [Parameter(Mandatory = $true)][string] $Name
    )

    if ($Name -notmatch '^[A-Za-z0-9][A-Za-z0-9._-]*$') {
        throw "Invalid Doroti temporary directory name: $Name"
    }
    $temporaryRoot = Get-DorotiTemporaryRoot -DorotiRoot $DorotiRoot
    $path = Join-Path $temporaryRoot ($Name + '-' + [Guid]::NewGuid().ToString('N'))
    [IO.Directory]::CreateDirectory($path) | Out-Null
    return [IO.Path]::GetFullPath($path)
}

function Remove-DorotiTemporaryItem {
    param(
        [Parameter(Mandatory = $true)][string] $DorotiRoot,
        [Parameter(Mandatory = $true)][string] $Path
    )

    $temporaryRoot = (Get-DorotiTemporaryRoot -DorotiRoot $DorotiRoot) + [IO.Path]::DirectorySeparatorChar
    $resolved = [IO.Path]::GetFullPath($Path)
    if (-not $resolved.StartsWith($temporaryRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to clean a path outside the Doroti temporary root: $resolved"
    }
    for ($attempt = 1; $attempt -le 5; $attempt++) {
        try {
            if ([IO.Directory]::Exists($resolved)) {
                [IO.Directory]::Delete($resolved, $true)
            }
            elseif ([IO.File]::Exists($resolved)) {
                [IO.File]::Delete($resolved)
            }
            return
        }
        catch {
            if ($attempt -eq 5) { throw }
            [Threading.Thread]::Sleep(200)
        }
    }
}
