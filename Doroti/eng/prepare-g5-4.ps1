#Requires -Version 5.1
param(
    [string] $OutputPath
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot 'migration/selections/g5-4-full-framework.json'
}
elseif (-not [IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot $OutputPath
}
$OutputPath = [IO.Path]::GetFullPath($OutputPath)

$closurePath = Join-Path $dorotiRoot 'migration/flutter-framework/g5-4-closure.json'
if (-not (Test-Path -LiteralPath $closurePath -PathType Leaf)) {
    throw "G5-4 closure is missing: $closurePath"
}
$closure = Get-Content -LiteralPath $closurePath -Raw | ConvertFrom-Json
$libraries = @($closure.libraries | Sort-Object path)
$declarationCount = @($libraries | ForEach-Object { @($_.declarations) }).Count

if ($closure.flutterGitRevision -ne '56b8e1a851a594b1a154f8ea93270807dab22b9a' -or
    $libraries.Count -ne 694 -or $declarationCount -ne 5355) {
    throw "Pinned G5-4 inventory drifted: expected revision 56b8e1a... and 694 libraries/5355 declarations, got $($closure.flutterGitRevision) and $($libraries.Count)/$declarationCount."
}
if (@($libraries | Where-Object { $_.analyzerErrors -ne 0 }).Count -ne 0) {
    throw 'Pinned G5-4 inventory contains analyzer errors.'
}

function New-Input([object] $Library, [string] $EmissionMode) {
    return [ordered]@{
        path = "../../../reference/flutter-master/packages/flutter/lib/$($Library.path)"
        library = "package:flutter/$($Library.path)"
        symbols = @($Library.declarations | ForEach-Object { [string]$_.name })
        emissionMode = $EmissionMode
        boundarySymbols = @()
    }
}

function New-Selection([object[]] $SelectionInputs, [string] $AssemblyName) {
    return [ordered]@{
        schemaVersion = 'doroti.converter-selection/v4'
        converterVersion = '3.0.0'
        migrationIrVersion = '3.0.0'
        generationMode = 'framework-semantic'
        compatibilityProfile = 'flutter-framework'
        frameworkMilestone = 'G5-4'
        outputNamespace = 'Doroti.Generated.Framework'
        outputAssemblyName = $AssemblyName
        flutterBaseline = '../flutter-compat/baseline.json'
        inputs = $SelectionInputs
    }
}

function Write-Json([string] $Path, [object] $Value) {
    New-Item -ItemType Directory -Force -Path (Split-Path $Path -Parent) | Out-Null
    $json = ($Value | ConvertTo-Json -Depth 12) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($Path, $json + "`n", [Text.UTF8Encoding]::new($false))
}

$fullInputs = foreach ($library in $libraries) {
    $product = $library.path -match '^src/(material|cupertino|widget_previews)/' -or
        $library.path -eq 'widget_previews.dart'
    New-Input $library $(if ($product -and @($library.declarations).Count -gt 0) { 'generate' } else { 'graph-only' })
}
Write-Json $OutputPath (New-Selection $fullInputs 'Doroti.Generated.Framework.G54')

$predecessor = Get-Content -LiteralPath (Join-Path $dorotiRoot 'migration/selections/g5-3-widgets.json') -Raw | ConvertFrom-Json
$dependencyInputs = @($predecessor.inputs |
    Where-Object { $_.library -notmatch '^package:flutter/src/(material|cupertino|widget_previews)/' } |
    ForEach-Object {
        [ordered]@{
            path = [string]$_.path
            library = [string]$_.library
            symbols = @($_.symbols | ForEach-Object { [string]$_ })
            emissionMode = 'graph-only'
            boundarySymbols = @($_.boundarySymbols | ForEach-Object { $_ })
        }
    })

function Get-MaterialBatch([string] $Path) {
    $name = [IO.Path]::GetFileNameWithoutExtension($Path)
    if ($name -match 'theme|color_scheme|typography|visual_density|button_style') { return 'M1-theme' }
    if ($name -match 'ink|shape|border|material$|elevation|shadow') { return 'M2-shape-ink' }
    if ($name[0] -le 'm') { return 'M3-components-a-m' }
    return 'M4-components-n-z'
}

function Get-CupertinoBatch([string] $Path) {
    $name = [IO.Path]::GetFileNameWithoutExtension($Path)
    if ($name -match 'theme|colors|icons') { return 'C0-theme' }
    if ($name -match 'route|nav|page|tab|app') { return 'C1-navigation' }
    if ($name -match 'text|form|picker|switch|slider|button|search|date') { return 'C2-form-text' }
    return 'C3-dialog-selection'
}

$batchGroups = [ordered]@{}
foreach ($library in $libraries | Where-Object { $_.path -match '^src/material/' }) {
    $batch = Get-MaterialBatch ([string]$library.path)
    if (-not $batchGroups.Contains($batch)) { $batchGroups[$batch] = [Collections.Generic.List[object]]::new() }
    $batchGroups[$batch].Add($library)
}
foreach ($library in $libraries | Where-Object { $_.path -match '^src/cupertino/' }) {
    $batch = Get-CupertinoBatch ([string]$library.path)
    if (-not $batchGroups.Contains($batch)) { $batchGroups[$batch] = [Collections.Generic.List[object]]::new() }
    $batchGroups[$batch].Add($library)
}
$batchGroups['P0-widget-previews'] = [Collections.Generic.List[object]]::new()
foreach ($library in $libraries | Where-Object { $_.path -match '^(widget_previews\.dart|src/widget_previews/)' }) {
    $batchGroups['P0-widget-previews'].Add($library)
}

$batchIndex = [Collections.Generic.List[object]]::new()
$batchRoot = Join-Path $dorotiRoot 'migration/selections'
foreach ($batch in $batchGroups.Keys) {
    $batchLibraries = @($batchGroups[$batch] | Sort-Object path)
    $batchInputs = foreach ($library in $batchLibraries) {
        New-Input $library $(if (@($library.declarations).Count -gt 0) { 'generate' } else { 'graph-only' })
    }
    $manifestPath = Join-Path $batchRoot ('g5-4-' + $batch + '.json')
    Write-Json $manifestPath (New-Selection (@($dependencyInputs) + @($batchInputs)) "Doroti.Generated.Framework.G54.$($batch -replace '-', '')")
    $batchIndex.Add([ordered]@{
        id = $batch
        manifest = "migration/selections/g5-4-$batch.json"
        libraries = $batchLibraries.Count
        declarations = @($batchLibraries | ForEach-Object { @($_.declarations) }).Count
        paths = @($batchLibraries.path)
    })
}

$productLibraryCount = 0
$productDeclarationCount = 0
foreach ($item in $batchIndex) {
    $productLibraryCount += [int]$item.libraries
    $productDeclarationCount += [int]$item.declarations
}
if ($productLibraryCount -ne 252 -or $productDeclarationCount -ne 2091) {
    throw "G5-4 product inventory drifted: expected 252 libraries/2091 declarations, got $productLibraryCount/$productDeclarationCount."
}
$productLibraries = @($libraries | Where-Object {
    $_.path -match '^src/(material|cupertino|widget_previews)/' -or $_.path -eq 'widget_previews.dart'
})
$productInputs = foreach ($library in $productLibraries) {
    New-Input $library $(if (@($library.declarations).Count -gt 0) { 'generate' } else { 'graph-only' })
}
Write-Json (Join-Path $dorotiRoot 'migration/selections/g5-4-product.json') (
    New-Selection $productInputs 'Doroti.Generated.Framework.G54')
Write-Json (Join-Path $dorotiRoot 'migration/flutter-framework/g5-4-batches.json') ([ordered]@{
    schemaVersion = 'doroti.g5-4-batches/v1'
    milestone = 'G5-4'
    batches = @($batchIndex)
    counts = [ordered]@{
        batches = $batchIndex.Count
        productLibraries = $productLibraryCount
        productDeclarations = $productDeclarationCount
    }
})

Write-Output "G5-4 selection: PASS (13 roots, 695 files, 694 resolved libraries, 5355 declarations)"
Write-Output "Batches: $($batchIndex.Count) (252 product libraries, 2091 declarations)"
Write-Output "Manifest: $OutputPath"
