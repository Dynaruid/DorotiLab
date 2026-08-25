#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ContractPath = (Join-Path $PSScriptRoot '../validation/contracts/windowsappsdk-flutter-frame-scheduler.json'),

    [string] $EvidencePath,

    [switch] $RequireEvidence
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$contractFullPath = [IO.Path]::GetFullPath($ContractPath)

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Get-ConfinedPath([string] $Root, [string] $RelativePath) {
    Assert-True (-not [string]::IsNullOrWhiteSpace($RelativePath)) 'A contract path is empty.'
    $rootWithSeparator = [IO.Path]::GetFullPath($Root).TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    $fullPath = [IO.Path]::GetFullPath((Join-Path $Root $RelativePath))
    Assert-True ($fullPath.StartsWith($rootWithSeparator, [StringComparison]::OrdinalIgnoreCase)) (
        "Contract path escapes the repository root: $RelativePath")
    return $fullPath
}

function Get-JsonPathValue([object] $Value, [string] $Path) {
    $current = $Value
    foreach ($segment in $Path.Split('.')) {
        if ($null -eq $current) { return $null }
        $property = $current.PSObject.Properties[$segment]
        if ($null -eq $property) { return $null }
        $current = $property.Value
    }
    return $current
}

function Get-SourceFingerprint([string[]] $Paths) {
    $builder = [Text.StringBuilder]::new()
    foreach ($relativePath in ($Paths | Sort-Object -Unique)) {
        $fullPath = Get-ConfinedPath $repoRoot $relativePath
        $hash = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
        [void] $builder.Append($relativePath.Replace('\', '/')).Append('=').Append($hash).Append("`n")
    }
    return [Convert]::ToHexString(
        [Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($builder.ToString()))
    ).ToLowerInvariant()
}

function Get-OptionalContractArray([object] $Owner, [string] $PropertyName) {
    $property = $Owner.PSObject.Properties[$PropertyName]
    if ($null -eq $property -or $null -eq $property.Value) { return @() }
    return @($property.Value)
}

function ConvertTo-Int64([object] $Value, [string] $Path) {
    [long] $integerValue = 0
    Assert-True ([long]::TryParse([string] $Value, [ref] $integerValue)) (
        "F6 frame scheduler evidence integer is missing or invalid: $Path")
    return $integerValue
}

function Assert-NonNegativeInteger([object] $Value, [string] $Path) {
    $integerValue = ConvertTo-Int64 $Value $Path
    Assert-True ($integerValue -ge 0) "F6 frame scheduler evidence integer is negative: $Path"
    return $integerValue
}

function Assert-ExpectedValues([object] $Evidence, [object] $ExpectedValues, [string] $Prefix = '') {
    foreach ($expected in @($ExpectedValues)) {
        $relativePath = [string] $expected.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = Get-JsonPathValue $Evidence $relativePath
        $expectedValue = $expected.value
        Assert-True ($actual -eq $expectedValue) (
            "F6 frame scheduler evidence value drifted: $path = '$actual', expected '$expectedValue'.")
    }
}

function Assert-MinimumIntegerValues([object] $Evidence, [object] $MinimumValues, [string] $Prefix = '') {
    foreach ($minimum in @($MinimumValues)) {
        $relativePath = [string] $minimum.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = ConvertTo-Int64 (Get-JsonPathValue $Evidence $relativePath) $path
        $expectedMinimum = ConvertTo-Int64 $minimum.value "$path minimum"
        Assert-True ($actual -ge $expectedMinimum) (
            "F6 frame scheduler evidence minimum drifted: $path = '$actual', minimum '$expectedMinimum'.")
    }
}

function Assert-MaximumIntegerValues([object] $Evidence, [object] $MaximumValues, [string] $Prefix = '') {
    foreach ($maximum in @($MaximumValues)) {
        $relativePath = [string] $maximum.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = ConvertTo-Int64 (Get-JsonPathValue $Evidence $relativePath) $path
        $expectedMaximum = ConvertTo-Int64 $maximum.value "$path maximum"
        Assert-True ($actual -le $expectedMaximum) (
            "F6 frame scheduler evidence maximum drifted: $path = '$actual', maximum '$expectedMaximum'.")
    }
}

function Assert-RendererClassification([object] $Evidence, [object] $ClassificationContract) {
    $classificationPath = [string] $ClassificationContract.classificationPath
    $classification = Get-JsonPathValue $Evidence $classificationPath
    Assert-True ($classification -is [string] -and -not [string]::IsNullOrWhiteSpace($classification)) (
        "F6 frame scheduler renderer classification is missing: $classificationPath")
    $allowedValues = @($ClassificationContract.allowedValues | ForEach-Object { [string] $_ })
    Assert-True ($allowedValues -contains $classification) (
        "F6 frame scheduler renderer classification '$classification' is not allowed.")
    $softwareFallbackPath = [string] $ClassificationContract.softwareFallbackPath
    $softwareFallback = Get-JsonPathValue $Evidence $softwareFallbackPath
    Assert-True ($softwareFallback -is [bool]) (
        "F6 frame scheduler renderer software fallback flag is missing: $softwareFallbackPath")
    if ($classification -eq 'software') {
        Assert-True $softwareFallback (
            'F6 frame scheduler renderer is classified as software but did not report softwareFallback=true.')
    }
    else {
        Assert-True (-not $softwareFallback) (
            'F6 frame scheduler renderer is classified as hardware but reported softwareFallback=true.')
    }
    foreach ($path in (Get-OptionalContractArray $ClassificationContract 'visibleClaimPaths')) {
        $claimPath = [string] $path
        $claim = Get-JsonPathValue $Evidence $claimPath
        Assert-True ($claim -is [bool] -and -not $claim) (
            "F6 scheduler timing evidence overclaims visible or hardware-visible acceptance: $claimPath")
    }
}

function Assert-NamedMatrix([object] $Evidence, [object] $MatrixContract, [string] $MatrixName) {
    $matrixPath = [string] $MatrixContract.path
    $matrix = Get-JsonPathValue $Evidence $matrixPath
    Assert-True ($matrix -is [Collections.IEnumerable] -and $matrix -isnot [string]) (
        "F6 $MatrixName matrix is missing or not an array: $matrixPath")
    $actualRows = @($matrix)
    $expectedNames = @($MatrixContract.expectedNames | ForEach-Object { [string] $_ })
    Assert-True ($actualRows.Count -eq $expectedNames.Count) (
        "F6 $MatrixName matrix cardinality drifted: $($actualRows.Count), expected $($expectedNames.Count).")
    for ($index = 0; $index -lt $expectedNames.Count; $index++) {
        $row = $actualRows[$index]
        $prefix = "$matrixPath[$index]"
        $name = Get-JsonPathValue $row 'name'
        Assert-True ($name -eq $expectedNames[$index]) (
            "F6 $MatrixName matrix name/order drifted at index ${index}: '$name', expected '$($expectedNames[$index])'.")
        foreach ($relativePath in (Get-OptionalContractArray $MatrixContract 'requiredBooleanPaths')) {
            $path = [string] $relativePath
            $value = Get-JsonPathValue $row $path
            Assert-True ($value -is [bool]) "F6 $MatrixName evidence boolean is missing: $prefix.$path"
        }
        foreach ($relativePath in (Get-OptionalContractArray $MatrixContract 'requiredNonNegativeIntegerPaths')) {
            $path = [string] $relativePath
            [void] (Assert-NonNegativeInteger (Get-JsonPathValue $row $path) "$prefix.$path")
        }
        # Preserve the prefix when optional arrays are empty; positional array
        # expansion previously made an array row look like the Prefix argument.
        $minimumValues = @(Get-OptionalContractArray $MatrixContract 'minimumIntegerValues')
        $maximumValues = @(Get-OptionalContractArray $MatrixContract 'maximumIntegerValues')
        $expectedValues = @(Get-OptionalContractArray $MatrixContract 'expectedValues')
        Assert-MinimumIntegerValues -Evidence $row -MinimumValues $minimumValues -Prefix $prefix
        Assert-MaximumIntegerValues -Evidence $row -MaximumValues $maximumValues -Prefix $prefix
        Assert-ExpectedValues -Evidence $row -ExpectedValues $expectedValues -Prefix $prefix
    }
}

function Assert-RefreshRateMatrix([object] $Evidence, [object] $MatrixContract) {
    $matrixPath = [string] $MatrixContract.path
    $matrix = Get-JsonPathValue $Evidence $matrixPath
    Assert-True ($matrix -is [Collections.IEnumerable] -and $matrix -isnot [string]) (
        "F6 cadence matrix is missing or not an array: $matrixPath")
    $actualRows = @($matrix)
    $expectedRates = @($MatrixContract.expectedRefreshRates | ForEach-Object { ConvertTo-Int64 $_ 'cadence expected refresh rate' })
    Assert-True ($actualRows.Count -eq $expectedRates.Count) (
        "F6 cadence matrix cardinality drifted: $($actualRows.Count), expected $($expectedRates.Count).")
    for ($index = 0; $index -lt $expectedRates.Count; $index++) {
        $row = $actualRows[$index]
        $prefix = "$matrixPath[$index]"
        $refreshRate = ConvertTo-Int64 (Get-JsonPathValue $row 'refreshHz') "$prefix.refreshHz"
        Assert-True ($refreshRate -eq $expectedRates[$index]) (
            "F6 cadence refresh rate/order drifted at index ${index}: '$refreshRate', expected '$($expectedRates[$index])'.")
        foreach ($relativePath in (Get-OptionalContractArray $MatrixContract 'requiredBooleanPaths')) {
            $path = [string] $relativePath
            $value = Get-JsonPathValue $row $path
            Assert-True ($value -is [bool]) "F6 cadence evidence boolean is missing: $prefix.$path"
        }
        foreach ($relativePath in (Get-OptionalContractArray $MatrixContract 'requiredNonNegativeIntegerPaths')) {
            $path = [string] $relativePath
            [void] (Assert-NonNegativeInteger (Get-JsonPathValue $row $path) "$prefix.$path")
        }
        $scheduled = ConvertTo-Int64 (Get-JsonPathValue $row 'scheduledFrameCount') "$prefix.scheduledFrameCount"
        $presented = ConvertTo-Int64 (Get-JsonPathValue $row 'presentedFrameCount') "$prefix.presentedFrameCount"
        Assert-True ($scheduled -ge $presented) (
            "F6 cadence cannot present more frames than scheduler admitted: $prefix")
        $minimumValues = @(Get-OptionalContractArray $MatrixContract 'minimumIntegerValues')
        $maximumValues = @(Get-OptionalContractArray $MatrixContract 'maximumIntegerValues')
        $expectedValues = @(Get-OptionalContractArray $MatrixContract 'expectedValues')
        Assert-MinimumIntegerValues -Evidence $row -MinimumValues $minimumValues -Prefix $prefix
        Assert-MaximumIntegerValues -Evidence $row -MaximumValues $maximumValues -Prefix $prefix
        Assert-ExpectedValues -Evidence $row -ExpectedValues $expectedValues -Prefix $prefix
    }
}

function Assert-CausalChains([object] $Evidence, [object] $MatrixContract) {
    Assert-NamedMatrix $Evidence $MatrixContract 'causal chain'
    $matrixPath = [string] $MatrixContract.path
    $rows = @(Get-JsonPathValue $Evidence $matrixPath)
    foreach ($row in $rows) {
        $name = [string] (Get-JsonPathValue $row 'name')
        $prefix = "$matrixPath.$name"
        $callback = ConvertTo-Int64 (Get-JsonPathValue $row 'callbackTimestampTicks') "$prefix.callbackTimestampTicks"
        $raster = ConvertTo-Int64 (Get-JsonPathValue $row 'rasterTimestampTicks') "$prefix.rasterTimestampTicks"
        $swap = ConvertTo-Int64 (Get-JsonPathValue $row 'swapTimestampTicks') "$prefix.swapTimestampTicks"
        $presented = ConvertTo-Int64 (Get-JsonPathValue $row 'presentedTimestampTicks') "$prefix.presentedTimestampTicks"
        Assert-True ($callback -le $raster -and $raster -le $swap -and $swap -le $presented) (
            "F6 causal timestamp ordering drifted for $name.")
    }
}

function Assert-AggregateCausalCounts([object] $Evidence) {
    $callback = ConvertTo-Int64 (Get-JsonPathValue $Evidence 'scheduler.callbackCount') 'scheduler.callbackCount'
    $raster = ConvertTo-Int64 (Get-JsonPathValue $Evidence 'scheduler.rasterCount') 'scheduler.rasterCount'
    $swap = ConvertTo-Int64 (Get-JsonPathValue $Evidence 'scheduler.swapCount') 'scheduler.swapCount'
    $presented = ConvertTo-Int64 (Get-JsonPathValue $Evidence 'scheduler.presentedCount') 'scheduler.presentedCount'
    Assert-True ($callback -ge $raster -and $raster -ge $swap -and $swap -ge $presented -and $presented -gt 0) (
        'F6 callback, raster, swap, and presented counters do not form a valid causal chain.')
}

function Assert-DwmTimingDiagnostics([object] $Evidence, [object] $DiagnosticsContract) {
    Assert-True ($null -ne $DiagnosticsContract) (
        'F6 frame scheduler contract is missing DWM timing diagnostics requirements.')
    $callHwndsPath = [string] $DiagnosticsContract.callHwndsPath
    $boundChildHwndsPath = [string] $DiagnosticsContract.boundChildHwndsPath
    $lastHResultsPath = [string] $DiagnosticsContract.lastHResultsPath
    $viewCountPath = [string] $DiagnosticsContract.viewCountPath
    $expectedCallHwnd = [string] $DiagnosticsContract.expectedCallHwnd
    Assert-True (-not [string]::IsNullOrWhiteSpace($callHwndsPath) -and
        -not [string]::IsNullOrWhiteSpace($boundChildHwndsPath) -and
        -not [string]::IsNullOrWhiteSpace($lastHResultsPath) -and
        -not [string]::IsNullOrWhiteSpace($viewCountPath) -and
        -not [string]::IsNullOrWhiteSpace($expectedCallHwnd)) (
        'F6 frame scheduler DWM timing diagnostics contract is incomplete.')

    $callHwndsValue = Get-JsonPathValue $Evidence $callHwndsPath
    $boundChildHwndsValue = Get-JsonPathValue $Evidence $boundChildHwndsPath
    $lastHResultsValue = Get-JsonPathValue $Evidence $lastHResultsPath
    foreach ($entry in @(
            @{ Value = $callHwndsValue; Path = $callHwndsPath },
            @{ Value = $boundChildHwndsValue; Path = $boundChildHwndsPath },
            @{ Value = $lastHResultsValue; Path = $lastHResultsPath })) {
        Assert-True ($entry.Value -is [Collections.IEnumerable] -and $entry.Value -isnot [string]) (
            "F6 DWM timing diagnostic is missing or not an array: $($entry.Path)")
    }

    $callHwnds = @($callHwndsValue)
    $boundChildHwnds = @($boundChildHwndsValue)
    $lastHResults = @($lastHResultsValue)
    $viewCount = Assert-NonNegativeInteger (Get-JsonPathValue $Evidence $viewCountPath) $viewCountPath
    Assert-True ($viewCount -gt 0 -and $callHwnds.Count -eq $viewCount -and
        $boundChildHwnds.Count -eq $viewCount -and $lastHResults.Count -eq $viewCount) (
        'F6 DWM timing diagnostic cardinality does not match the runtime view count.')

    $boundChildHwndSet = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    for ($index = 0; $index -lt $viewCount; $index++) {
        $callHwnd = $callHwnds[$index]
        Assert-True ($callHwnd -is [string] -and $callHwnd -eq $expectedCallHwnd) (
            "F6 DWM timing used '$callHwnd' for view $index; expected '$expectedCallHwnd'.")
        $childHwnd = $boundChildHwnds[$index]
        Assert-True ($childHwnd -is [string] -and $childHwnd -match '^0x[0-9A-F]+$' -and
            $childHwnd -notmatch '^0x0+$') (
            "F6 DWM timing child-HWND provenance is invalid at index ${index}: '$childHwnd'.")
        Assert-True ($boundChildHwndSet.Add($childHwnd)) (
            "F6 DWM timing child-HWND provenance is duplicated at index $index.")
        [void] (Assert-NonNegativeInteger $lastHResults[$index] "$lastHResultsPath[$index]")
    }
}

Assert-True (Test-Path -LiteralPath $contractFullPath -PathType Leaf) (
    "Windows App SDK Flutter frame scheduler contract is missing: $contractFullPath")
$contract = Get-Content -LiteralPath $contractFullPath -Raw | ConvertFrom-Json -Depth 64
Assert-True ([string] $contract.schemaVersion -eq 'doroti.windowsappsdk-flutter-frame-scheduler-contract/v1') (
    'Unexpected Windows App SDK Flutter frame scheduler contract schema.')

$validated = [Collections.Generic.List[object]]::new()
$validatedPaths = [Collections.Generic.List[string]]::new()
$itemIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)

foreach ($item in @($contract.projects) + @($contract.sources)) {
    $id = [string] $item.id
    $relativePath = [string] $item.path
    Assert-True (-not [string]::IsNullOrWhiteSpace($id) -and $itemIds.Add($id)) (
        "Contract item id is missing or duplicated: $id")
    $fullPath = Get-ConfinedPath $repoRoot $relativePath
    Assert-True (Test-Path -LiteralPath $fullPath -PathType Leaf) (
        "F6 frame scheduler contract input is missing: $relativePath")
    $text = Get-Content -LiteralPath $fullPath -Raw
    foreach ($literal in (Get-OptionalContractArray $item 'requiredLiterals')) {
        $requiredLiteral = [string] $literal
        Assert-True (-not [string]::IsNullOrWhiteSpace($requiredLiteral)) "Required literal is empty: $id"
        Assert-True ($text.Contains($requiredLiteral, [StringComparison]::Ordinal)) (
            "F6 frame scheduler contract missing '$requiredLiteral' in $relativePath")
    }
    $previousIndex = -1
    foreach ($literal in (Get-OptionalContractArray $item 'orderedLiterals')) {
        $orderedLiteral = [string] $literal
        $index = $text.IndexOf($orderedLiteral, [StringComparison]::Ordinal)
        Assert-True ($index -ge 0) "Ordered F6 frame scheduler literal is missing: '$orderedLiteral' in $relativePath"
        Assert-True ($index -gt $previousIndex) (
            "F6 frame scheduler ordering drifted in ${relativePath}: '$orderedLiteral' appears before its required predecessor.")
        $previousIndex = $index
    }
    foreach ($literal in (Get-OptionalContractArray $item 'forbiddenLiterals')) {
        $forbiddenLiteral = [string] $literal
        Assert-True (-not $text.Contains($forbiddenLiteral, [StringComparison]::Ordinal)) (
            "F6 frame scheduler contract forbids '$forbiddenLiteral' in $relativePath")
    }
    $validated.Add([ordered]@{
            id = $id
            path = $relativePath.Replace('\', '/')
            sha256 = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
        })
    $validatedPaths.Add($relativePath)
}

Assert-True ($validated.Count -gt 0) 'The F6 frame scheduler contract has no project or source inputs.'
$sourceFingerprint = Get-SourceFingerprint $validatedPaths.ToArray()

$evidenceStatus = 'notVerified'
$validatedEvidencePath = $null
if ($RequireEvidence -and [string]::IsNullOrWhiteSpace($EvidencePath)) {
    throw 'F6 frame scheduler evidence is required but -EvidencePath was not supplied.'
}
if (-not [string]::IsNullOrWhiteSpace($EvidencePath)) {
    $evidenceFullPath = [IO.Path]::GetFullPath($EvidencePath)
    Assert-True (Test-Path -LiteralPath $evidenceFullPath -PathType Leaf) (
        "F6 frame scheduler evidence is missing: $evidenceFullPath")
    $evidence = Get-Content -LiteralPath $evidenceFullPath -Raw | ConvertFrom-Json -Depth 64
    Assert-True ([string] $evidence.schemaVersion -eq [string] $contract.evidence.schemaVersion) (
        'Unexpected F6 frame scheduler evidence schema.')
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredStringPaths')) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [string] -and -not [string]::IsNullOrWhiteSpace($value)) (
            "F6 frame scheduler evidence string is missing: $path")
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredBooleanPaths')) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [bool]) "F6 frame scheduler evidence boolean is missing: $path"
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredNonNegativeIntegerPaths')) {
        [void] (Assert-NonNegativeInteger (Get-JsonPathValue $evidence ([string] $path)) ([string] $path))
    }
    $minimumValues = @(Get-OptionalContractArray $contract.evidence 'minimumIntegerValues')
    $maximumValues = @(Get-OptionalContractArray $contract.evidence 'maximumIntegerValues')
    $expectedValues = @(Get-OptionalContractArray $contract.evidence 'expectedValues')
    Assert-MinimumIntegerValues -Evidence $evidence -MinimumValues $minimumValues
    Assert-MaximumIntegerValues -Evidence $evidence -MaximumValues $maximumValues
    Assert-ExpectedValues -Evidence $evidence -ExpectedValues $expectedValues
    Assert-RendererClassification $evidence $contract.evidence.rendererClassification
    Assert-DwmTimingDiagnostics $evidence $contract.evidence.dwmTimingDiagnostics
    Assert-AggregateCausalCounts $evidence
    Assert-RefreshRateMatrix $evidence $contract.evidence.cadenceMatrix
    Assert-NamedMatrix $evidence $contract.evidence.views 'view'
    Assert-CausalChains $evidence $contract.evidence.causalChains

    $evidenceSourceFingerprint = Get-JsonPathValue $evidence ([string] $contract.evidence.sourceFingerprintPath)
    Assert-True ($evidenceSourceFingerprint -is [string] -and $evidenceSourceFingerprint -eq $sourceFingerprint) (
        'F6 frame scheduler evidence was not produced from the current validated source fingerprint.')
    $executablePath = Get-JsonPathValue $evidence ([string] $contract.evidence.executablePath)
    $executableHash = Get-JsonPathValue $evidence ([string] $contract.evidence.executableHashPath)
    Assert-True ($executablePath -is [string] -and -not [string]::IsNullOrWhiteSpace($executablePath) -and
        $executableHash -is [string] -and -not [string]::IsNullOrWhiteSpace($executableHash)) (
        'F6 frame scheduler evidence executable provenance is missing.')
    $executableFullPath = [IO.Path]::GetFullPath($executablePath)
    Assert-True (Test-Path -LiteralPath $executableFullPath -PathType Leaf) (
        "F6 frame scheduler evidence executable is missing: $executableFullPath")
    $actualExecutableHash = (Get-FileHash -LiteralPath $executableFullPath -Algorithm SHA256).Hash.ToLowerInvariant()
    Assert-True ($actualExecutableHash -eq $executableHash.ToLowerInvariant()) (
        'F6 frame scheduler evidence executable hash drifted after the live run.')
    $evidenceStatus = 'PASS'
    $validatedEvidencePath = $evidenceFullPath
}

$notVerified = [Collections.Generic.List[string]]::new()
if ($evidenceStatus -ne 'PASS') {
    $notVerified.Add('same-STA scheduler, dedicated-MTA raster, and DWM-timing scheduler evidence')
    $notVerified.Add('60/120/144/165Hz deterministic cadence matrix and per-view causal callback-to-present chain')
    $notVerified.Add('hidden/minimized/suspended stop and latest-metrics ordinary-frame resume evidence')
}
$notVerified.Add('F7 input, F8 lifecycle/recovery, and F9 product-runner selection')
$notVerified.Add('scan-out cadence, visible blank/white-frame absence, and compositor-continuity acceptance (FG)')

$summary = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk-flutter-frame-scheduler-validation/v1'
    status = 'PASS'
    scopeBoundary = [string] $contract.scopeBoundary
    sourceFingerprint = $sourceFingerprint
    validatedInputs = $validated
    evidenceStatus = $evidenceStatus
    evidencePath = $validatedEvidencePath
    notVerified = @($notVerified)
}

$summary | ConvertTo-Json -Depth 32
