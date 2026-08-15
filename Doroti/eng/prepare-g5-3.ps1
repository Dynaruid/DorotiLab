#Requires -Version 5.1
param(
    [string] $OutputPath
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot 'migration/selections/g5-3-widgets.json'
}
elseif (-not [IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot $OutputPath
}
$OutputPath = [IO.Path]::GetFullPath($OutputPath)

$closurePath = Join-Path $dorotiRoot 'migration/flutter-framework/f3-closure.json'
$closure = Get-Content -LiteralPath $closurePath -Raw | ConvertFrom-Json
$libraries = @($closure.libraries |
    Where-Object { $_.path -match '^src/widgets/' } |
    Sort-Object path)
$declarationCount = @($libraries | ForEach-Object { @($_.declarations) }).Count

if ($libraries.Count -ne 186 -or $declarationCount -ne 1715) {
    throw "Pinned G5-3 inventory drifted: expected 186 libraries/1715 declarations, got $($libraries.Count)/$declarationCount."
}
if (@($libraries | Where-Object { $_.analyzerErrors -ne 0 }).Count -ne 0) {
    throw 'Pinned G5-3 inventory contains analyzer errors.'
}

$boundarySymbols = @{
    'src/widgets/_html_element_view_io.dart' = @('HtmlElementViewImpl')
    'src/widgets/_html_element_view_web.dart' = @('HtmlElementViewImpl')
    'src/widgets/_window_linux.dart' = @('_gMalloc0', '_gFree')
    'src/widgets/_window_macos.dart' = @('_posixCalloc', '_posixFree', '_Utf8Pointer', '_StringUtf8Pointer')
    'src/widgets/_window_win32.dart' = @('_Utf16Pointer', '_StringUtf16Pointer')
    'src/widgets/platform_view.dart' = @('<unnamed-extension>')
}

$predecessorSelections = @(
    'migration/selections/g3-1-framework-multilibrary.json'
    'migration/selections/g3-b0-foundation.json'
    'migration/selections/g3-b0-foundation-batch2.json'
    'migration/selections/g4-3-scheduler-services.json'
    'migration/selections/g4-4-physics-animation-gestures.json'
    'migration/selections/g4-5-painting-rendering-semantics.json'
)
$dependencyByLibrary = [ordered]@{}
foreach ($selectionPath in $predecessorSelections) {
    $predecessor = Get-Content -LiteralPath (Join-Path $dorotiRoot $selectionPath) -Raw | ConvertFrom-Json
    foreach ($input in @($predecessor.inputs)) {
        $dependencyByLibrary[[string]$input.library] = [ordered]@{
            path = [string]$input.path
            library = [string]$input.library
            symbols = @($input.symbols | ForEach-Object { [string]$_ })
            emissionMode = 'graph-only'
        }
    }
}
$dependencyInputs = @($dependencyByLibrary.Values)

$widgetInputs = foreach ($library in $libraries) {
    $excluded = @($boundarySymbols[$library.path])
    $symbols = @($library.declarations | ForEach-Object { [string]$_.name })
    [ordered]@{
        path = "../../../reference/flutter-master/packages/flutter/lib/$($library.path)"
        library = "package:flutter/$($library.path)"
        symbols = $symbols
        emissionMode = if ($symbols.Count -eq 0) { 'graph-only' } else { 'generate' }
        boundarySymbols = $excluded
    }
}
$inputs = @($dependencyInputs) + @($widgetInputs)

$selection = [ordered]@{
    schemaVersion = 'doroti.converter-selection/v4'
    converterVersion = '3.0.0'
    migrationIrVersion = '3.0.0'
    generationMode = 'framework-semantic'
    compatibilityProfile = 'flutter-framework'
    frameworkMilestone = 'G5-3'
    outputNamespace = 'Doroti.Generated.Framework'
    outputAssemblyName = 'Doroti.Generated.Framework.Widgets'
    flutterBaseline = '../flutter-compat/baseline.json'
    inputs = $inputs
}

$parent = Split-Path $OutputPath -Parent
New-Item -ItemType Directory -Force -Path $parent | Out-Null
$json = ($selection | ConvertTo-Json -Depth 12) -replace "`r`n", "`n"
[IO.File]::WriteAllText($OutputPath, $json + "`n", [Text.UTF8Encoding]::new($false))
Write-Output "G5-3 selection: PASS (186 libraries, 1715 declarations)"
Write-Output "Manifest: $OutputPath"
