#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $OutputDirectory = '.doroti/evidence/material-sample-p0',
    [switch] $FetchImages
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$output = [IO.Path]::GetFullPath($OutputDirectory, $repoRoot)
if (Test-Path -LiteralPath (Join-Path $output 'reference-inputs.json')) {
    throw 'Select a new output directory to preserve the previous reference snapshot.'
}
New-Item -ItemType Directory -Path $output -Force | Out-Null
$sample = Join-Path $repoRoot 'reference/flutter_sample_app'
$configPath = Join-Path $sample '.dart_tool/package_config.json'
$config = Get-Content -LiteralPath $configPath -Raw | ConvertFrom-Json
$flutterPackage = $config.packages | Where-Object name -EQ 'flutter'
$flutterRoot = [IO.Path]::GetFullPath((Join-Path ([Uri]$flutterPackage.rootUri).LocalPath '../..'))
$colorPackage = $config.packages | Where-Object name -EQ 'material_color_utilities'
$colorRoot = ([Uri]$colorPackage.rootUri).LocalPath
function Fingerprint([string] $Path, [string] $Base) {
    $file = Get-Item -LiteralPath $Path
    [ordered]@{
        path = [IO.Path]::GetRelativePath($Base, $file.FullName).Replace('\', '/')
        bytes = $file.Length
        sha256 = (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()
    }
}
$inputDirectories = @('lib', 'test', 'fonts', 'assets') | ForEach-Object { Join-Path $sample $_ } |
    Where-Object { Test-Path -LiteralPath $_ }
$inputs = @(Get-ChildItem -LiteralPath $inputDirectories -File -Recurse |
    Sort-Object FullName | ForEach-Object { Fingerprint $_.FullName $repoRoot })
foreach ($name in @('pubspec.yaml', 'pubspec.lock', 'LICENSE')) {
    $inputs += Fingerprint (Join-Path $sample $name) $repoRoot
}
$sdkInputs = @('pubspec.lock', 'packages/flutter/lib/src/material/search_anchor.dart', 'packages/flutter/lib/src/material/color_scheme.dart',
    'bin/cache/artifacts/material_fonts/MaterialIcons-Regular.otf') | ForEach-Object {
    $path = Join-Path $flutterRoot $_
    if (Test-Path -LiteralPath $path) { Fingerprint $path $flutterRoot }
}
$colorInputs = @(Get-ChildItem -LiteralPath (Join-Path $colorRoot 'lib') -Recurse -File | Sort-Object FullName |
    ForEach-Object { Fingerprint $_.FullName $colorRoot })
$imageInputs = @()
$constants = Get-Content -LiteralPath (Join-Path $sample 'lib/src/constants.dart') -Raw
foreach ($match in [regex]::Matches($constants, 'https://flutter\.github\.io/[^''\s]+\.png')) {
    $url = $match.Value
    $path = Join-Path $output ([Uri]$url).Segments[-1]
    $status = 'notFetched'
    $failure = $null
    if ($FetchImages) {
        try { Invoke-WebRequest -Uri $url -OutFile $path -TimeoutSec 60; $status = 'downloaded-for-validation' }
        catch { $status = 'FAIL'; $failure = $_.Exception.Message }
    }
    $imageInputs += [ordered]@{
        url = $url; status = $status; error = $failure
        file = $(if (Test-Path -LiteralPath $path) { Fingerprint $path $repoRoot } else { $null })
    }
}
$manifest = [ordered]@{
    schemaVersion = 1
    capturedAt = [DateTimeOffset]::Now.ToString('o')
    repositoryRevision = (git -C $repoRoot rev-parse HEAD)
    sampleStatus = 'local source is authoritative; hashes include local adaptations'
    flutterSdkRoot = $flutterRoot
    flutterSdkRevision = (git -C $flutterRoot rev-parse HEAD)
    flutterSdkDirty = @(git -C $flutterRoot status --porcelain --untracked-files=no)
    packageConfigGeneratorVersion = $config.generatorVersion
    materialColorUtilitiesRoot = $colorRoot
    inputs = $inputs; sdkInputs = @($sdkInputs); colorInputs = $colorInputs; images = $imageInputs
    limits = @('Downloaded images are validation evidence, not a product offline bundle.',
        'Font hashes identify inputs; actual per-target selected font and fallback remain notVerified.',
        'Source hashes and network download do not establish runtime parity or browser CORS success.')
}
$manifest | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath (Join-Path $output 'reference-inputs.json') -Encoding utf8
Write-Output (Join-Path $output 'reference-inputs.json')
