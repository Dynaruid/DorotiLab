#Requires -Version 5.1

$script:DorotiFlutterRevision = '56b8e1a851a594b1a154f8ea93270807dab22b9a'
$script:DorotiFlutterVersionAnchor = '3.33.0-0.0.pre'
$script:DorotiFlutterHistoryDepth = 6579

function Repair-DorotiFlutterSdkLineEndings {
    param([Parameter(Mandatory = $true)][string] $SdkRoot)

    if ($IsWindows -or $env:OS -eq 'Windows_NT') { return }
    if (-not (Test-Path -LiteralPath $SdkRoot -PathType Container)) { return }

    $trackedRows = @(& git -C $SdkRoot ls-files --eol -- '*.sh' 'bin/flutter' 'bin/dart' 'packages/flutter/lib/*.dart')
    if ($LASTEXITCODE -ne 0) {
        throw "Repository-local Flutter SDK text files could not be enumerated at $SdkRoot."
    }
    $trackedTextFiles = @($trackedRows | Where-Object { $_ -match '\bw/crlf\b.*\t(?<path>.+)$' } |
        ForEach-Object { [string]$Matches.path })
    foreach ($relative in $trackedTextFiles) {
        $path = Join-Path $SdkRoot $relative
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { continue }
        $bytes = [IO.File]::ReadAllBytes($path)
        if ([Array]::IndexOf($bytes, [byte]13) -lt 0) { continue }
        $normalized = [Collections.Generic.List[byte]]::new($bytes.Length)
        for ($index = 0; $index -lt $bytes.Length; $index++) {
            if ($bytes[$index] -eq 13 -and $index + 1 -lt $bytes.Length -and $bytes[$index + 1] -eq 10) {
                continue
            }
            $normalized.Add($bytes[$index])
        }
        [IO.File]::WriteAllBytes($path, $normalized.ToArray())
    }
}

function Resolve-DorotiFlutterSdk {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string] $RepositoryRoot
    )

    $repositoryRootPath = [IO.Path]::GetFullPath($RepositoryRoot)
    $sdkRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRootPath 'reference/flutter-master'))
    Repair-DorotiFlutterSdkLineEndings -SdkRoot $sdkRoot
    $commandSuffix = if ($IsWindows -or $env:OS -eq 'Windows_NT') { '.bat' } else { '' }
    $flutterCommand = Join-Path $sdkRoot "bin/flutter$commandSuffix"
    $dartCommand = Join-Path $sdkRoot "bin/dart$commandSuffix"

    if (-not (Test-Path -LiteralPath $flutterCommand -PathType Leaf) -or
        -not (Test-Path -LiteralPath $dartCommand -PathType Leaf)) {
        throw "Repository-local Flutter SDK commands for the current host are missing at $sdkRoot. Run Doroti/eng/prepare-flutter-sdk.ps1."
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
