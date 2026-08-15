#Requires -Version 5.1
param(
    [string] $OutputPath
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot 'migration/selections/g4-5-painting-rendering-semantics.json'
}
elseif (-not [IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot $OutputPath
}
$OutputPath = [IO.Path]::GetFullPath($OutputPath)

$closurePath = Join-Path $dorotiRoot 'migration/flutter-framework/f3-closure.json'
$closure = Get-Content -LiteralPath $closurePath -Raw | ConvertFrom-Json
$libraries = @($closure.libraries |
    Where-Object { $_.path -match '^src/(painting|rendering|semantics)/' } |
    Sort-Object path)
$declarationCount = @($libraries | ForEach-Object { @($_.declarations) }).Count

if ($libraries.Count -ne 101 -or $declarationCount -ne 663) {
    throw "Pinned G4-5 inventory drifted: expected 101 libraries/663 declarations, got $($libraries.Count)/$declarationCount."
}
if (@($libraries | Where-Object { $_.analyzerErrors -ne 0 }).Count -ne 0) {
    throw 'Pinned G4-5 inventory contains analyzer errors.'
}

$inputs = foreach ($library in $libraries) {
    $symbols = @($library.declarations | ForEach-Object { [string]$_.name })
    if ($symbols.Count -eq 0) {
        throw "G4-5 library has no selected declarations: $($library.path)"
    }
    [ordered]@{
        path = "../../../reference/flutter-master/packages/flutter/lib/$($library.path)"
        library = "package:flutter/$($library.path)"
        symbols = $symbols
    }
}

$selection = [ordered]@{
    schemaVersion = 'doroti.converter-selection/v4'
    converterVersion = '3.0.0'
    migrationIrVersion = '3.0.0'
    generationMode = 'framework-semantic'
    compatibilityProfile = 'flutter-framework'
    frameworkMilestone = 'G4-5'
    outputNamespace = 'Doroti.Generated.Framework'
    outputAssemblyName = 'Doroti.Generated.Framework.PaintingRenderingSemantics'
    flutterBaseline = '../flutter-compat/baseline.json'
    inputs = $inputs
}

$parent = Split-Path $OutputPath -Parent
New-Item -ItemType Directory -Force -Path $parent | Out-Null
$json = ($selection | ConvertTo-Json -Depth 12) -replace "`r`n", "`n"
[IO.File]::WriteAllText($OutputPath, $json + "`n", [Text.UTF8Encoding]::new($false))
Write-Output "G4-5 selection: PASS (101 libraries, 663 declarations)"
Write-Output "Manifest: $OutputPath"
