#Requires -Version 5.1
[CmdletBinding()]
param(
    [string] $SourceRepository = 'https://github.com/flutter/flutter.git',
    [string] $SeedRepository
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repositoryRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$sdkRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'reference/flutter-master'))
. (Join-Path $PSScriptRoot 'flutter-sdk.ps1')

if (Test-Path -LiteralPath $sdkRoot) {
    try {
        $sdk = Resolve-DorotiFlutterSdk -RepositoryRoot $repositoryRoot
        Write-Output "Repository-local Flutter SDK already ready: $($sdk.Root) ($($sdk.Revision))"
        return
    }
    catch {
        throw "Cannot prepare Flutter SDK because $sdkRoot already exists but is not the required independent pinned checkout. Move or remove that exact directory, then rerun. $($_.Exception.Message)"
    }
}

[IO.Directory]::CreateDirectory($sdkRoot) | Out-Null
$createdSdkRoot = $true
try {
    & git -C $sdkRoot init
    if ($LASTEXITCODE -ne 0) { throw 'Flutter SDK Git initialization failed.' }
    & git -C $sdkRoot remote add origin $SourceRepository
    if ($LASTEXITCODE -ne 0) { throw 'Flutter SDK origin configuration failed.' }

    $fetchSource = if ([string]::IsNullOrWhiteSpace($SeedRepository)) { $SourceRepository } else { [IO.Path]::GetFullPath($SeedRepository) }
    & git -C $sdkRoot fetch --depth $script:DorotiFlutterHistoryDepth $fetchSource $script:DorotiFlutterRevision "refs/tags/$($script:DorotiFlutterVersionAnchor):refs/tags/$($script:DorotiFlutterVersionAnchor)"
    if ($LASTEXITCODE -ne 0) { throw "Flutter SDK revision fetch failed from $fetchSource." }
    & git -C $sdkRoot checkout --detach FETCH_HEAD
    if ($LASTEXITCODE -ne 0) { throw 'Flutter SDK pinned checkout failed.' }

    $sdk = Resolve-DorotiFlutterSdk -RepositoryRoot $repositoryRoot
    $describedRevision = (& git -C $sdk.Root describe --tags --match '*.*.*' --long HEAD 2>$null | Out-String).Trim()
    if ($LASTEXITCODE -ne 0 -or $describedRevision -notlike "$($script:DorotiFlutterVersionAnchor)-*") {
        throw "Flutter SDK version history is incomplete: expected anchor $script:DorotiFlutterVersionAnchor, got '$describedRevision'."
    }
    & $sdk.FlutterCommand --version
    if ($LASTEXITCODE -ne 0) { throw 'Repository-local Flutter SDK initialization failed.' }
    $sdk = Resolve-DorotiFlutterSdk -RepositoryRoot $repositoryRoot
    Write-Output "Repository-local Flutter SDK prepared: $($sdk.Root) ($($sdk.Revision))"
}
catch {
    if ($createdSdkRoot -and (Test-Path -LiteralPath $sdkRoot)) {
        $resolvedSdkRoot = [IO.Path]::GetFullPath($sdkRoot)
        $expectedSdkRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'reference/flutter-master'))
        if ($resolvedSdkRoot -ceq $expectedSdkRoot) {
            Remove-Item -LiteralPath $resolvedSdkRoot -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
    throw
}
