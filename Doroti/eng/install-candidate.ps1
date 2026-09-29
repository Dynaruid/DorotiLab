[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $InstallRoot,
    [string] $CandidateRoot,
    [ValidateSet('windows', 'web')] [string] $Platform = 'windows',
    [ValidateSet('Install', 'Remove')] [string] $Action = 'Install'
)
$ErrorActionPreference = 'Stop'
$destination = [IO.Path]::GetFullPath($InstallRoot)
if ($destination -eq [IO.Path]::GetPathRoot($destination)) { throw 'An installation must have a dedicated directory.' }
$marker = Join-Path $destination '.doroti-candidate-install.json'
$markerValue = 'Doroti local candidate installer v1'
if (Test-Path -LiteralPath $destination) {
    if ((Get-Item -LiteralPath $destination).Attributes -band [IO.FileAttributes]::ReparsePoint) { throw 'Installation root cannot be a reparse point.' }
    if (-not (Test-Path -LiteralPath $marker) -or (Get-Content -LiteralPath $marker -Raw).Trim() -ne $markerValue) {
        throw 'Refusing to modify a directory not owned by this installer.'
    }
} elseif ($Action -eq 'Install') {
    New-Item -ItemType Directory -Path $destination | Out-Null
    Set-Content -LiteralPath $marker -Value $markerValue
} else { return }
$versions = Join-Path $destination 'versions'
$prefix = $destination.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
function Assert-OwnedPath([string] $Path) {
    $resolved = [IO.Path]::GetFullPath($Path)
    if (-not $resolved.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) { throw "Path leaves installation root: $Path" }
    $ancestor = $resolved
    while ($ancestor.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) {
        if ((Test-Path -LiteralPath $ancestor) -and ((Get-Item -LiteralPath $ancestor).Attributes -band [IO.FileAttributes]::ReparsePoint)) {
            throw "Reparse points are not installation directories: $ancestor"
        }
        $ancestor = [IO.Path]::GetDirectoryName($ancestor)
    }
    return $resolved
}
if ($Action -eq 'Remove') {
    if (Test-Path -LiteralPath $versions) {
        $ownedVersions = Assert-OwnedPath $versions
        Get-ChildItem -LiteralPath $ownedVersions -Recurse -Force | ForEach-Object {
            $null = Assert-OwnedPath $_.FullName
        }
        Remove-Item -LiteralPath $ownedVersions -Recurse -Force
    }
    $current = Assert-OwnedPath (Join-Path $destination 'current.json')
    if (Test-Path -LiteralPath $current) { Remove-Item -LiteralPath $current -Force }
    Write-Output 'Removed candidate versions; userdata and installation marker retained.'
    return
}
if (-not $CandidateRoot) { throw 'CandidateRoot is required for Install.' }
$candidate = [IO.Path]::GetFullPath($CandidateRoot)
$manifest = Get-Content -LiteralPath (Join-Path $candidate 'candidate.json') -Raw | ConvertFrom-Json
if ($manifest.status -notlike 'PASS:*') { throw 'Only successfully qualified local candidates can be installed.' }
if ($manifest.version -notmatch '^\d+\.\d+\.\d+(?:-[A-Za-z0-9.-]+)?$') { throw 'Invalid candidate version.' }
$hashes = $manifest.artifacts.$Platform
if (-not $hashes) { throw 'Candidate has no qualified payload checksum manifest for the selected platform.' }
$payload = [IO.Path]::GetFullPath((Join-Path $candidate $Platform))
$payloadPrefix = $payload.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
foreach ($file in $hashes.PSObject.Properties) {
    $path = [IO.Path]::GetFullPath((Join-Path $payload $file.Name))
    if (-not $path.StartsWith($payloadPrefix, [StringComparison]::OrdinalIgnoreCase)) { throw 'Invalid payload path.' }
    if ((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $file.Value) { throw "Payload checksum mismatch: $($file.Name)" }
}
$target = Assert-OwnedPath (Join-Path $versions ($manifest.version + '-' + $Platform))
function Assert-InstalledPayload([string] $Directory) {
    $expected = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach ($file in $hashes.PSObject.Properties) {
        $path = Assert-OwnedPath (Join-Path $Directory $file.Name)
        if (-not $expected.Add($path)) { throw "Duplicate installed path: $($file.Name)" }
        if ((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $file.Value) { throw "Installed checksum mismatch: $($file.Name)" }
    }
    if ($expected.Count -eq 0) { throw 'Candidate payload cannot be empty.' }
    Get-ChildItem -LiteralPath $Directory -Recurse -Force | ForEach-Object {
        $path = Assert-OwnedPath $_.FullName
        if (-not $_.PSIsContainer -and -not $expected.Contains($path)) { throw "Unlisted installed file: $path" }
    }
}
if (-not (Test-Path -LiteralPath $target)) {
    # An interrupted copy must not reserve the immutable version directory.
    $staging = Assert-OwnedPath (Join-Path $versions ('.staging-' + [Guid]::NewGuid().ToString('N')))
    try {
        New-Item -ItemType Directory -Path $staging -Force | Out-Null
        foreach ($file in $hashes.PSObject.Properties) {
            $path = Assert-OwnedPath (Join-Path $staging $file.Name)
            New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($path)) -Force | Out-Null
            Copy-Item -LiteralPath (Join-Path $payload $file.Name) -Destination $path
        }
        Assert-InstalledPayload $staging
        [IO.Directory]::Move($staging, $target)
    }
    finally {
        if (Test-Path -LiteralPath $staging) {
            $ownedStaging = Assert-OwnedPath $staging
            Get-ChildItem -LiteralPath $ownedStaging -Recurse -Force | ForEach-Object { $null = Assert-OwnedPath $_.FullName }
            Remove-Item -LiteralPath $ownedStaging -Recurse -Force
        }
    }
}
# Revalidate even an existing version before making it current, including extra files.
Assert-InstalledPayload $target
$userdata = Assert-OwnedPath (Join-Path $destination 'userdata')
New-Item -ItemType Directory -Path $userdata -Force | Out-Null
$current = @{ version = $manifest.version; platform = $Platform; directory = $target; revision = $manifest.revision }
$temporary = Assert-OwnedPath (Join-Path $destination 'current.json.tmp')
$current | ConvertTo-Json | Set-Content -LiteralPath $temporary
[IO.File]::Move($temporary, (Join-Path $destination 'current.json'), $true)
Write-Output ("Installed local candidate: " + $target)
