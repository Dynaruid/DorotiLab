#Requires -Version 5.1

$script:DorotiFlutterRevision = '56b8e1a851a594b1a154f8ea93270807dab22b9a'
$script:DorotiFlutterVersionAnchor = '3.33.0-0.0.pre'
$script:DorotiFlutterHistoryDepth = 6579

function Resolve-DorotiFlutterSdk {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string] $RepositoryRoot
    )

    $repositoryRootPath = [IO.Path]::GetFullPath($RepositoryRoot)
    $sdkRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRootPath 'flutter-master'))
    $flutterCommand = Join-Path $sdkRoot 'bin/flutter.bat'
    $dartCommand = Join-Path $sdkRoot 'bin/dart.bat'

    if (-not (Test-Path -LiteralPath $flutterCommand -PathType Leaf) -or
        -not (Test-Path -LiteralPath $dartCommand -PathType Leaf)) {
        throw "Repository-local Flutter SDK is missing at $sdkRoot. Run Doroti/eng/prepare-flutter-sdk.ps1."
    }

    $global:LASTEXITCODE = 0
    $topLevel = (& git -C $sdkRoot rev-parse --show-toplevel 2>$null | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($topLevel) -or
        [IO.Path]::GetFullPath($topLevel) -cne $sdkRoot) {
        throw "Repository-local Flutter SDK must be an independent Git checkout rooted at $sdkRoot. Run Doroti/eng/prepare-flutter-sdk.ps1."
    }

    $commonDirectory = (& git -C $sdkRoot rev-parse --git-common-dir 2>$null | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($commonDirectory)) {
        throw "Repository-local Flutter SDK Git metadata could not be resolved at $sdkRoot."
    }
    $commonDirectoryPath = if ([IO.Path]::IsPathRooted($commonDirectory)) {
        [IO.Path]::GetFullPath($commonDirectory)
    }
    else {
        [IO.Path]::GetFullPath((Join-Path $sdkRoot $commonDirectory))
    }
    $sdkBoundary = $sdkRoot.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if (-not $commonDirectoryPath.StartsWith($sdkBoundary, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Repository-local Flutter SDK must not depend on external Git metadata: $commonDirectoryPath."
    }

    $revision = (& git -C $sdkRoot rev-parse HEAD 2>$null | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or $revision -cne $script:DorotiFlutterRevision) {
        throw "Repository-local Flutter SDK revision drifted: expected $script:DorotiFlutterRevision, got '$revision'."
    }

    return [pscustomobject]@{
        Root = $sdkRoot
        FlutterCommand = $flutterCommand
        DartCommand = $dartCommand
        Revision = $revision
        GitCommonDirectory = $commonDirectoryPath
    }
}
