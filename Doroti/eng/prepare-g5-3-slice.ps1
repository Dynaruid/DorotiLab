#Requires -Version 5.1
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('W0', 'W1', 'W2', 'W3', 'W4', 'W5', 'W6', 'W7')]
    [string] $Slice,
    [string] $OutputPath
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$fullSelectionPath = Join-Path $dorotiRoot 'migration/selections/g5-3-widgets.json'
& (Join-Path $PSScriptRoot 'prepare-g5-3.ps1') -OutputPath $fullSelectionPath | Write-Output

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot "migration/selections/g5-3-$($Slice.ToLowerInvariant()).json"
}
elseif (-not [IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot $OutputPath
}
$OutputPath = [IO.Path]::GetFullPath($OutputPath)

$sliceSymbols = [ordered]@{
    W0 = [ordered]@{
        'package:flutter/src/widgets/framework.dart' = @('Widget', 'BuildContext', 'BuildOwner', 'Element')
        'package:flutter/src/widgets/binding.dart' = @('WidgetsBindingObserver', 'WidgetsBinding', 'runApp', 'runWidget', '_runWidget', 'RootWidget', 'RootElement', 'WidgetsFlutterBinding')
    }
    W1 = [ordered]@{
        'package:flutter/src/widgets/framework.dart' = @('Widget', 'BuildContext', 'BuildOwner', 'Element', 'ComponentElement', 'ProxyWidget', 'ProxyElement', 'StatelessWidget', 'StatelessElement')
    }
    W2 = [ordered]@{
        'package:flutter/src/widgets/framework.dart' = @('StatefulWidget', 'State', 'StatefulElement')
    }
    W3 = [ordered]@{
        # Key and LocalKey are predecessor Foundation owners; the Widgets
        # slice begins at GlobalKey and inherited dependency propagation.
        'package:flutter/src/widgets/framework.dart' = @('GlobalKey', 'InheritedWidget', 'InheritedElement')
    }
    W4 = [ordered]@{
        'package:flutter/src/widgets/focus_manager.dart' = @('FocusManager', 'FocusNode', 'FocusScopeNode')
        'package:flutter/src/widgets/actions.dart' = @('Intent', 'Action', 'Actions')
        'package:flutter/src/widgets/shortcuts.dart' = @('ShortcutManager', 'Shortcuts')
    }
    W5 = [ordered]@{
        'package:flutter/src/widgets/overlay.dart' = @('OverlayEntry', 'Overlay', 'OverlayState')
        'package:flutter/src/widgets/navigator.dart' = @('Navigator', 'NavigatorState', 'Route')
        'package:flutter/src/widgets/routes.dart' = @('OverlayRoute', 'TransitionRoute', 'ModalRoute')
    }
    W6 = [ordered]@{
        'package:flutter/src/widgets/scrollable.dart' = @('Scrollable', 'ScrollableState')
        'package:flutter/src/widgets/scroll_controller.dart' = @('ScrollController', 'TrackingScrollController')
        'package:flutter/src/widgets/scroll_position.dart' = @('ScrollPosition')
        'package:flutter/src/widgets/scroll_physics.dart' = @('ScrollPhysics')
        # ImageInfo is supplied by the predecessor Painting image-stream
        # contract; this slice owns the Widgets image lifecycle and builders.
        'package:flutter/src/widgets/image.dart' = @('Image', 'ImageFrameBuilder', 'ImageLoadingBuilder')
    }
    W7 = [ordered]@{
        'package:flutter/src/widgets/editable_text.dart' = @('EditableText', 'EditableTextState')
        'package:flutter/src/widgets/text_selection.dart' = @('TextSelectionOverlay')
    }
}

$selection = Get-Content -LiteralPath $fullSelectionPath -Raw | ConvertFrom-Json
$cumulative = [ordered]@{}
foreach ($sliceName in @('W0', 'W1', 'W2', 'W3', 'W4', 'W5', 'W6', 'W7')) {
    foreach ($library in $sliceSymbols[$sliceName].Keys) {
        if (-not $cumulative.Contains($library)) { $cumulative[$library] = [Collections.Generic.List[string]]::new() }
        foreach ($symbol in $sliceSymbols[$sliceName][$library]) {
            if (-not $cumulative[$library].Contains($symbol)) { $cumulative[$library].Add($symbol) }
        }
    }
    if ($sliceName -eq $Slice) { break }
}

# The typed semantic compiler intentionally rejects partial declaration
# emission inside a Dart library. Expand the requested diagnostic symbols to
# their complete Widgets library dependency closure, then emit every
# declaration in each included library. Non-Widgets predecessor libraries stay
# graph-only and are consumed from reviewed product projects.
$frameworkClosure = Get-Content -LiteralPath (Join-Path $dorotiRoot 'migration/flutter-framework/f3-closure.json') -Raw | ConvertFrom-Json
$widgetLibraryByUri = @{}
foreach ($libraryRecord in @($frameworkClosure.libraries | Where-Object { $_.path -match '^src/widgets/' })) {
    $widgetLibraryByUri["package:flutter/$($libraryRecord.path)"] = $libraryRecord
}
$generatedLibraries = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$pendingLibraries = [Collections.Generic.Queue[string]]::new()
foreach ($seedLibrary in $cumulative.Keys) {
    $pendingLibraries.Enqueue([string]$seedLibrary)
}
while ($pendingLibraries.Count -ne 0) {
    $libraryUri = $pendingLibraries.Dequeue()
    if (-not $generatedLibraries.Add($libraryUri)) { continue }
    if (-not $widgetLibraryByUri.ContainsKey($libraryUri)) {
        throw "$Slice dependency closure is missing Widgets inventory record: $libraryUri"
    }
    foreach ($dependencyPath in @($widgetLibraryByUri[$libraryUri].dependencies)) {
        if ([string]$dependencyPath -match '^src/widgets/') {
            $pendingLibraries.Enqueue("package:flutter/$dependencyPath")
        }
    }
}

foreach ($input in @($selection.inputs)) {
    if ($input.library -notmatch '^package:flutter/src/widgets/') { continue }
    $available = @($input.symbols)
    $input.emissionMode = 'graph-only'
    $input.symbols = @()
    if ($cumulative.Contains([string]$input.library)) {
        $missing = @($cumulative[[string]$input.library] | Where-Object { $available -notcontains $_ })
        if ($missing.Count -ne 0) { throw "$Slice requests missing symbol(s) from $($input.library): $($missing -join ', ')." }
    }
    if ($generatedLibraries.Contains([string]$input.library)) {
        $input.symbols = $available
        $input.emissionMode = 'generate'
    }
}

$selection.frameworkMilestone = 'G5-3'
$selection.outputAssemblyName = 'Doroti.Generated.Framework.Widgets'
$parent = Split-Path $OutputPath -Parent
New-Item -ItemType Directory -Force -Path $parent | Out-Null
$json = ($selection | ConvertTo-Json -Depth 20) -replace "`r`n", "`n"
[IO.File]::WriteAllText($OutputPath, $json + "`n", [Text.UTF8Encoding]::new($false))
Write-Output "G5-3 $Slice cumulative selection: PASS ($($generatedLibraries.Count) generated dependency-closure libraries)"
Write-Output "Manifest: $OutputPath"
