#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$workspaceRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$candidateRelative = 'migration/generated-candidates/flutter-framework/56b8d5410b5a6adaf2e0542989c20e2a4efadb13/g5-3-current68'
$candidateRoot = Join-Path $dorotiRoot $candidateRelative
$selectionPath = Join-Path $dorotiRoot 'migration/selections/g5-3-widgets.json'
$buildLogPath = Join-Path $dorotiRoot 'migration/g5-3-current68-build.log'
$taxonomyPath = Join-Path $dorotiRoot 'migration/flutter-framework/g5-3-error-taxonomy.json'
$evidencePath = Join-Path $dorotiRoot 'migration/flutter-framework/g5-3-predecessor-evidence.json'
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g5-3-predecessor'
$temporarySelection = Join-Path $temporaryRoot 'selection.json'

function Invoke-Checked {
    param([scriptblock] $Command)
    $global:LASTEXITCODE = 0
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "Command failed with exit code $LASTEXITCODE." }
}

function Get-Sha256Text {
    param([string] $Text)
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try {
        $bytes = [Text.Encoding]::UTF8.GetBytes($Text)
        return ([BitConverter]::ToString($algorithm.ComputeHash($bytes))).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $algorithm.Dispose()
    }
}

function Write-AtomicJson {
    param([string] $Path, [object] $Value)
    $fullPath = [IO.Path]::GetFullPath($Path)
    $temporary = $fullPath + '.tmp-' + [Guid]::NewGuid().ToString('N')
    $backup = $fullPath + '.bak-' + [Guid]::NewGuid().ToString('N')
    $json = ($Value | ConvertTo-Json -Depth 16) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($temporary, $json + "`n", [Text.UTF8Encoding]::new($false))
    try {
        if ([IO.File]::Exists($fullPath)) {
            [IO.File]::Replace($temporary, $fullPath, $backup, $true)
        }
        else {
            [IO.File]::Move($temporary, $fullPath)
        }
    }
    finally {
        if ([IO.File]::Exists($temporary)) { [IO.File]::Delete($temporary) }
        if ([IO.File]::Exists($backup)) { [IO.File]::Delete($backup) }
    }
}

function Get-FirstSlice {
    param([string] $File, [string] $Symbol)
    $value = ($File + ' ' + $Symbol).ToLowerInvariant()
    if ($value -match 'editable|text_selection|text_edit|spell|magnifier|undo_history') { return 'W7' }
    if ($value -match 'scroll|sliver|viewport|image|list_wheel|page_view') { return 'W6' }
    if ($value -match 'navigator|route|overlay|dialog|modal|hero|pages') { return 'W5' }
    if ($value -match 'focus|action|shortcut|keyboard') { return 'W4' }
    if ($value -match 'inherited|globalkey|localkey|keyed|key$') { return 'W3' }
    if ($value -match 'stateful|setstate|lifecycle|keep_alive') { return 'W2' }
    if ($value -match 'binding|app_lifecycle|^view|rootwidget|buildowner') { return 'W0' }
    return 'W1'
}

function Get-Category {
    param([string] $File, [string] $Symbol, [object[]] $Diagnostics)
    $text = ($File + ' ' + $Symbol + ' ' + (($Diagnostics | ForEach-Object { $_.code + ' ' + $_.message }) -join ' '))
    if ($File -eq 'binding.g.cs' -and $text -match 'PlatformDispatcher|DorotiView|ViewFocus|SemanticsAction') {
        return 'host-capability/avalonia-port'
    }
    if ($text -match 'Doroti\.Flutter\.Ui|AppLifecycleState|DisplayFeature|ViewFocus|SemanticsActionEvent|IReadOnlyList<Doroti\.Flutter\.Ui') {
        return 'dart-ui-contract'
    }
    if ($text -match "CS0311|CS0314|CS0305|CS0403|CS8978|CS8716|제네릭|형식 매개 변수") {
        return 'dart-model-representation'
    }
    if ($text -match 'Future<|DartMap|HashSet|IEnumerable|AsyncEnumerable|DartCoreExtensions|clamp|Cast<|FoundationRuntimePorts') {
        return 'dart-runtime'
    }
    if ($text -match 'ConstantsLibrary|Generated\.Framework\.(Foundation|Scheduler|Services|Gestures|Painting|Rendering|Semantics)|RenderObject|ScrollActivityDelegate') {
        return 'predecessor-framework'
    }
    return 'compiler-lowering'
}

function Get-FixOwner {
    param([string] $Category)
    switch ($Category) {
        'compiler-lowering' { 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering' }
        'dart-runtime' { 'src/Doroti.Runtime' }
        'dart-ui-contract' { 'src/Doroti.Ui' }
        'predecessor-framework' { 'src/Doroti.Framework.* predecessor package' }
        'dart-model-representation' { 'tools/Doroti.DartToCSharp typed Core/CSharp IR' }
        'host-capability/avalonia-port' { 'src/Doroti.Host.Desktop.Framework + Doroti.Vendor.Avalonia.*' }
        default { throw "Unclassified G5-3 category: $Category" }
    }
}

try {
    if (-not (Test-Path $candidateRoot)) { throw "Frozen current68 candidate is missing: $candidateRoot" }
    Invoke-Checked { & (Join-Path $PSScriptRoot 'prepare-g5-3.ps1') -OutputPath $temporarySelection }
    if ([IO.File]::ReadAllText($temporarySelection) -cne [IO.File]::ReadAllText($selectionPath)) {
        throw 'The regenerated G5-3 selection differs from the frozen current68 selection.'
    }

    $expectedSelectionHash = '291d6315702a93ef2db0a1297ca26027da28b6f8a42e7f8d55c661b017ff617f'
    $selectionHash = (Get-FileHash -LiteralPath $selectionPath -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($selectionHash -cne $expectedSelectionHash) { throw "G5-3 selection hash drifted: $selectionHash" }

    $report = Get-Content (Join-Path $candidateRoot 'converter-report.json') -Raw | ConvertFrom-Json
    if (-not $report.success -or @($report.outputs).Count -ne 185 -or @($report.diagnostics).Count -ne 1428) {
        throw 'Frozen current68 converter census drifted from 185 files / 1428 diagnostics.'
    }
    foreach ($output in @($report.outputs)) {
        $outputPath = Join-Path $candidateRoot ([string]$output.output)
        $actualHash = (Get-FileHash -LiteralPath $outputPath -Algorithm SHA256).Hash.ToLowerInvariant()
        if ($actualHash -cne [string]$output.sha256) {
            throw "Frozen current68 output hash drifted: $($output.output)"
        }
    }
    $digestLines = @($report.outputs | Sort-Object output | ForEach-Object { "$($_.sha256)  $($_.output)" })
    $candidateDigest = Get-Sha256Text ($digestLines -join "`n")
    if ($candidateDigest -cne '3ae512f3a1389b24e3d4642da9b6497b554c35f8f61ce482fb797addada3eabd') {
        throw "Frozen current68 candidate digest drifted: $candidateDigest"
    }

    $pattern = 'projects[\\/]Widgets[\\/](?<file>[^\\/]+\.g\.cs)\((?<line>\d+),(?<column>\d+)\): error (?<code>CS\d+): (?<message>.*?)(?: \[[^\]]+\.csproj\])?$'
    $parsed = foreach ($line in Get-Content $buildLogPath) {
        $match = [regex]::Match($line, $pattern)
        if ($match.Success) {
            [pscustomobject][ordered]@{
                file = $match.Groups['file'].Value
                line = [int]$match.Groups['line'].Value
                column = [int]$match.Groups['column'].Value
                code = $match.Groups['code'].Value
                message = $match.Groups['message'].Value
            }
        }
    }
    $locations = @($parsed | Sort-Object file, line, column | Group-Object { "$($_.file)|$($_.line)|$($_.column)" })
    if ($locations.Count -ne 211) { throw "Expected 211 frozen C# error locations, found $($locations.Count)." }

    $sourceMap = Get-Content (Join-Path $candidateRoot 'source-map.json') -Raw | ConvertFrom-Json
    $taxonomyEntries = foreach ($location in $locations) {
        $first = $location.Group[0]
        $generatedFile = "projects/Widgets/$($first.file)"
        $fileMappings = @($sourceMap.mappings | Where-Object { ([string]$_.generatedFile -replace '\\', '/') -eq $generatedFile })
        $mapping = $fileMappings | Where-Object {
            $first.line -ge [int]$_.generatedLineStart -and $first.line -le [int]$_.generatedLineEnd
        } | Select-Object -First 1
        if ($null -eq $mapping) {
            $mapping = $fileMappings | Sort-Object { [Math]::Min([Math]::Abs($first.line - [int]$_.generatedLineStart), [Math]::Abs($first.line - [int]$_.generatedLineEnd)) } | Select-Object -First 1
        }
        if ($null -eq $mapping -or [string]::IsNullOrWhiteSpace([string]$mapping.symbol)) {
            throw "No Dart source symbol maps to ${generatedFile}:$($first.line)."
        }
        $diagnostics = @($location.Group | Sort-Object code, message -Unique | ForEach-Object {
            [ordered]@{ code = $_.code; message = $_.message }
        })
        $category = Get-Category -File $first.file -Symbol ([string]$mapping.symbol) -Diagnostics $diagnostics
        $slice = Get-FirstSlice -File $first.file -Symbol ([string]$mapping.symbol)
        [ordered]@{
            id = 'G53E-' + (Get-Sha256Text $location.Name).Substring(0, 12)
            category = $category
            dartSource = [ordered]@{
                path = ([string]$mapping.source -replace '^\.\./\.\./\.\./reference/flutter-master/', 'reference/flutter-master/')
                symbol = [string]$mapping.symbol
                sourceOffset = [int]$mapping.sourceOffset
            }
            generatedCSharp = [ordered]@{
                path = "$candidateRelative/$generatedFile"
                line = $first.line
                column = $first.column
            }
            csharpDiagnostics = $diagnostics
            firstRequiredSlice = $slice
            fixOwner = Get-FixOwner $category
            reproduction = "./eng/validate-g5-3-slice.ps1 -Slice $slice"
            status = 'baseline-open'
        }
    }
    $categoryCounts = [ordered]@{}
    foreach ($category in @('compiler-lowering', 'dart-runtime', 'dart-ui-contract', 'predecessor-framework', 'dart-model-representation', 'host-capability/avalonia-port')) {
        $categoryCounts[$category] = @($taxonomyEntries | Where-Object { $_.category -eq $category }).Count
    }
    if (($categoryCounts.Values | Measure-Object -Sum).Sum -ne 211) { throw 'G5-3 taxonomy contains unclassified entries.' }

    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g4-3.ps1') }
    Invoke-Checked { dotnet run --project (Join-Path $dorotiRoot 'validation/Doroti.Validation.G5WidgetsTextInput/Doroti.Validation.G5WidgetsTextInput.csproj') --configuration Release }

    $g43 = Get-Content (Join-Path $dorotiRoot 'migration/flutter-avalonia/bridge-validation/g4-3.json') -Raw | ConvertFrom-Json
    if ($g43.currentRun.aggregate.status -ne 'verified' -or
        $g43.currentRun.promotion.status -ne 'verified' -or
        $g43.currentRun.productBehavior.status -ne 'verified') {
        throw 'Fresh G4-3 aggregate, promotion, or behavior evidence is not verified.'
    }

    $capturedAt = [DateTimeOffset]::UtcNow.ToString('O')
    $taxonomy = [ordered]@{
        schemaVersion = 'doroti.g5-3-error-taxonomy/v1'
        milestone = 'G5-3A'
        baseline = 'g5-3-current68'
        capturedAtUtc = $capturedAt
        sourceErrorLocations = 211
        compilerDiagnosticOccurrences = 214
        classified = 211
        unclassified = 0
        categoryCounts = $categoryCounts
        entries = @($taxonomyEntries)
    }
    Write-AtomicJson -Path $taxonomyPath -Value $taxonomy

    $evidence = [ordered]@{
        schemaVersion = 'doroti.g5-3-predecessor-evidence/v1'
        milestone = 'G5-3A'
        capturedAtUtc = $capturedAt
        status = 'verified'
        baseline = [ordered]@{
            name = 'g5-3-current68'
            compilerIdentity = $report.identity
            selectionSha256 = $selectionHash
            candidateDigestSha256 = $candidateDigest
            generatedFiles = 185
            compilerDiagnostics = 1428
            csharpDiagnosticOccurrences = 214
            uniqueCsharpErrorLocations = 211
            byteIdentity = 'verified'
            verification = 'regenerated selection byte match plus converter-report output hash replay'
        }
        taxonomy = [ordered]@{
            path = 'migration/flutter-framework/g5-3-error-taxonomy.json'
            classified = 211
            unclassified = 0
            categoryCounts = $categoryCounts
        }
        predecessor = [ordered]@{
            freshAggregate = 'verified'
            warnings = 0
            errors = 0
            apiMissing = 0
            apiExtra = 0
            behavior = 'verified'
            packageOnlyConsumer = 'verified'
            g4Regression = 'verified'
            evidence = 'migration/flutter-avalonia/bridge-validation/g4-3.json'
        }
        widgetsTextInput = [ordered]@{
            status = 'verifiedManaged'
            evidence = 'migration/flutter-avalonia/bridge-validation/g5-3-text-input.json'
            physical = 'notVerified'
        }
        validationCommands = @(
            './eng/validate-g4-3.ps1',
            'dotnet run --project validation/Doroti.Validation.G5WidgetsTextInput --configuration Release'
        )
    }
    Write-AtomicJson -Path $evidencePath -Value $evidence
    Write-Output 'G5-3A baseline, taxonomy, predecessor promotion, package, and managed text-input validation: PASS'
}
finally {
    Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
}
