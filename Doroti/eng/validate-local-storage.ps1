#Requires -Version 5.1

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$workspaceRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')

$forbiddenPatterns = @(
    ('Get' + 'TempPath'),
    ('Get' + 'TempFileName'),
    ('Directory.' + 'systemTemp'),
    ('${TMP' + 'DIR:-/tmp}'),
    ('/tmp/' + 'doroti')
)
$searchRoots = @(
    (Join-Path $dorotiRoot 'eng'),
    (Join-Path $dorotiRoot 'validation'),
    (Join-Path $dorotiRoot 'tools'),
    (Join-Path $workspaceRoot 'tools/Doroti.DartToCSharp'),
    (Join-Path $workspaceRoot 'DorotiDemoApp')
)
$ownedFiles = foreach ($root in $searchRoots) {
    if (-not (Test-Path -LiteralPath $root)) { continue }
    Get-ChildItem -LiteralPath $root -File -Recurse -ErrorAction SilentlyContinue | Where-Object {
        $_.Extension -in @('.ps1', '.sh', '.cs', '.dart') -and
        $_.Name -ne 'validate-local-storage.ps1' -and
        $_.FullName -notmatch '[\\/](bin|obj|artifacts|generated-candidates)[\\/]'
    }
}
$violations = foreach ($file in @($ownedFiles | Sort-Object FullName -Unique)) {
    foreach ($pattern in $forbiddenPatterns) {
        Select-String -LiteralPath $file.FullName -SimpleMatch -Pattern $pattern | ForEach-Object {
            "$($file.FullName):$($_.LineNumber): $($_.Line.Trim())"
        }
    }
}
if (@($violations).Count -gt 0) {
    throw "Doroti-owned code writes to an operating-system temporary directory:`n$($violations -join "`n")"
}

$localRoot = Get-DorotiLocalRoot -DorotiRoot $dorotiRoot
$temporaryRoot = Get-DorotiTemporaryRoot -DorotiRoot $dorotiRoot
$cacheRoot = Get-DorotiCacheRoot -DorotiRoot $dorotiRoot
foreach ($path in @($temporaryRoot, $cacheRoot)) {
    if (-not $path.StartsWith($localRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Doroti local-state path escaped its configured root: $path"
    }
}

$probe = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'local-storage-contract'
try {
    if ((Split-Path $probe -Parent) -cne $temporaryRoot) {
        throw "Temporary directory was not created below .doroti/tmp: $probe"
    }
}
finally {
    Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $probe
}
if (Test-Path -LiteralPath $probe) { throw "Temporary contract probe was not removed: $probe" }

foreach ($scriptName in @('validate-g5-3.ps1', 'validate-g5-3-slices.ps1')) {
    $content = Get-Content -LiteralPath (Join-Path $PSScriptRoot $scriptName) -Raw
    if ($content -notmatch 'finally\s*\{' -or $content -notmatch 'Remove-DorotiTemporaryItem') {
        throw "$scriptName must clean its invocation-owned workspace in a finally block."
    }
}

$ignored = git -C $workspaceRoot check-ignore '.doroti/local-storage-contract' 2>$null
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace(($ignored | Out-String))) {
    throw 'The workspace .doroti directory is not ignored by Git.'
}
$global:LASTEXITCODE = 0
Write-Output "Doroti local storage contract: PASS ($localRoot)"
