#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ContractPath = (Join-Path $PSScriptRoot '../validation/contracts/windowsappsdk-flutter-input.json'),

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

function Get-OptionalContractArray([object] $Owner, [string] $PropertyName) {
    $property = $Owner.PSObject.Properties[$PropertyName]
    if ($null -eq $property -or $null -eq $property.Value) { return @() }
    return @($property.Value)
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

function ConvertTo-Int64([object] $Value, [string] $Path) {
    [long] $integerValue = 0
    Assert-True ([long]::TryParse([string] $Value, [ref] $integerValue)) (
        "F7 child-input evidence integer is missing or invalid: $Path")
    return $integerValue
}

function Assert-NonNegativeInteger([object] $Value, [string] $Path) {
    $integerValue = ConvertTo-Int64 $Value $Path
    Assert-True ($integerValue -ge 0) "F7 child-input evidence integer is negative: $Path"
    return $integerValue
}

function Assert-ExpectedValues([object] $Evidence, [object] $ExpectedValues, [string] $Prefix = '') {
    foreach ($expected in @($ExpectedValues)) {
        $relativePath = [string] $expected.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = Get-JsonPathValue $Evidence $relativePath
        Assert-True ($actual -eq $expected.value) (
            "F7 child-input evidence value drifted: $path = '$actual', expected '$($expected.value)'.")
    }
}

function Assert-MinimumIntegerValues([object] $Evidence, [object] $MinimumValues, [string] $Prefix = '') {
    foreach ($minimum in @($MinimumValues)) {
        $relativePath = [string] $minimum.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = ConvertTo-Int64 (Get-JsonPathValue $Evidence $relativePath) $path
        $minimumValue = ConvertTo-Int64 $minimum.value "$path minimum"
        Assert-True ($actual -ge $minimumValue) (
            "F7 child-input evidence minimum drifted: $path = '$actual', minimum '$minimumValue'.")
    }
}

function Assert-MaximumIntegerValues([object] $Evidence, [object] $MaximumValues, [string] $Prefix = '') {
    foreach ($maximum in @($MaximumValues)) {
        $relativePath = [string] $maximum.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = ConvertTo-Int64 (Get-JsonPathValue $Evidence $relativePath) $path
        $maximumValue = ConvertTo-Int64 $maximum.value "$path maximum"
        Assert-True ($actual -le $maximumValue) (
            "F7 child-input evidence maximum drifted: $path = '$actual', maximum '$maximumValue'.")
    }
}

function Assert-NamedRows([object] $Evidence, [object] $RowsContract, [string] $RowsName) {
    Assert-True ($null -ne $RowsContract) "F7 $RowsName contract is missing."
    $rowsPath = [string] $RowsContract.path
    $value = Get-JsonPathValue $Evidence $rowsPath
    Assert-True ($value -is [Collections.IEnumerable] -and $value -isnot [string]) (
        "F7 $RowsName is missing or not an array: $rowsPath")
    $rows = @($value)
    $expectedNames = @($RowsContract.expectedNames | ForEach-Object { [string] $_ })
    Assert-True ($rows.Count -eq $expectedNames.Count) (
        "F7 $RowsName cardinality drifted: $($rows.Count), expected $($expectedNames.Count).")
    $booleanPaths = @(Get-OptionalContractArray $RowsContract 'requiredBooleanPaths')
    $integerPaths = @(Get-OptionalContractArray $RowsContract 'requiredNonNegativeIntegerPaths')
    $minimumValues = @(Get-OptionalContractArray $RowsContract 'minimumIntegerValues')
    $maximumValues = @(Get-OptionalContractArray $RowsContract 'maximumIntegerValues')
    $expectedValues = @(Get-OptionalContractArray $RowsContract 'expectedValues')
    $expectedEventSequences = @(Get-OptionalContractArray $RowsContract 'expectedEventSequences')
    for ($index = 0; $index -lt $expectedNames.Count; $index++) {
        $row = $rows[$index]
        $prefix = "$rowsPath[$index]"
        $name = Get-JsonPathValue $row 'name'
        Assert-True ($name -eq $expectedNames[$index]) (
            "F7 $RowsName name/order drifted at index ${index}: '$name', expected '$($expectedNames[$index])'.")
        foreach ($relativePath in $booleanPaths) {
            $path = [string] $relativePath
            Assert-True ((Get-JsonPathValue $row $path) -is [bool]) (
                "F7 $RowsName boolean is missing: $prefix.$path")
        }
        foreach ($relativePath in $integerPaths) {
            $path = [string] $relativePath
            [void] (Assert-NonNegativeInteger (Get-JsonPathValue $row $path) "$prefix.$path")
        }
        Assert-MinimumIntegerValues -Evidence $row -MinimumValues $minimumValues -Prefix $prefix
        Assert-MaximumIntegerValues -Evidence $row -MaximumValues $maximumValues -Prefix $prefix
        Assert-ExpectedValues -Evidence $row -ExpectedValues $expectedValues -Prefix $prefix
        if ($expectedEventSequences.Count -gt 0) {
            $eventsValue = Get-JsonPathValue $row 'events'
            Assert-True ($eventsValue -is [Collections.IEnumerable] -and $eventsValue -isnot [string]) (
                "F7 $RowsName event sequence is missing: $prefix.events")
            $actualEvents = @($eventsValue | ForEach-Object { [string] $_ })
            $expectedEvents = @($expectedEventSequences[$index] | ForEach-Object { [string] $_ })
            $sequenceMatches = $actualEvents.Count -eq $expectedEvents.Count
            if ($sequenceMatches) {
                for ($eventIndex = 0; $eventIndex -lt $expectedEvents.Count; $eventIndex++) {
                    if ($actualEvents[$eventIndex] -ne $expectedEvents[$eventIndex]) {
                        $sequenceMatches = $false
                        break
                    }
                }
            }
            Assert-True $sequenceMatches (
                "F7 $RowsName event sequence drifted: $prefix.events")
        }
    }
}

Assert-True (Test-Path -LiteralPath $contractFullPath -PathType Leaf) (
    "Windows App SDK Flutter input contract is missing: $contractFullPath")
$contract = Get-Content -LiteralPath $contractFullPath -Raw | ConvertFrom-Json -Depth 64
Assert-True ([string] $contract.schemaVersion -eq 'doroti.windowsappsdk-flutter-input-contract/v1') (
    'Unexpected Windows App SDK Flutter input contract schema.')

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
        "F7 child-input contract input is missing: $relativePath")
    $text = Get-Content -LiteralPath $fullPath -Raw
    foreach ($literal in (Get-OptionalContractArray $item 'requiredLiterals')) {
        $requiredLiteral = [string] $literal
        Assert-True (-not [string]::IsNullOrWhiteSpace($requiredLiteral)) "Required literal is empty: $id"
        Assert-True ($text.Contains($requiredLiteral, [StringComparison]::Ordinal)) (
            "F7 child-input contract missing '$requiredLiteral' in $relativePath")
    }
    $previousIndex = -1
    foreach ($literal in (Get-OptionalContractArray $item 'orderedLiterals')) {
        $orderedLiteral = [string] $literal
        $literalIndex = $text.IndexOf($orderedLiteral, [StringComparison]::Ordinal)
        Assert-True ($literalIndex -ge 0) "Ordered F7 literal is missing: '$orderedLiteral' in $relativePath"
        Assert-True ($literalIndex -gt $previousIndex) (
            "F7 child-input ordering drifted in ${relativePath}: '$orderedLiteral' appears before its predecessor.")
        $previousIndex = $literalIndex
    }
    foreach ($literal in (Get-OptionalContractArray $item 'forbiddenLiterals')) {
        $forbiddenLiteral = [string] $literal
        Assert-True (-not $text.Contains($forbiddenLiteral, [StringComparison]::Ordinal)) (
            "F7 child-input contract forbids '$forbiddenLiteral' in $relativePath")
    }
    $validated.Add([ordered]@{
            id = $id
            path = $relativePath.Replace('\', '/')
            sha256 = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
        })
    $validatedPaths.Add($relativePath)
}

Assert-True ($validated.Count -gt 0) 'The F7 child-input contract has no inputs.'
$sourceFingerprint = Get-SourceFingerprint $validatedPaths.ToArray()
$evidenceStatus = 'notVerified'
$validatedEvidencePath = $null
if ($RequireEvidence -and [string]::IsNullOrWhiteSpace($EvidencePath)) {
    throw 'F7 child-input evidence is required but -EvidencePath was not supplied.'
}
if (-not [string]::IsNullOrWhiteSpace($EvidencePath)) {
    $evidenceFullPath = [IO.Path]::GetFullPath($EvidencePath)
    Assert-True (Test-Path -LiteralPath $evidenceFullPath -PathType Leaf) (
        "F7 child-input evidence is missing: $evidenceFullPath")
    $evidence = Get-Content -LiteralPath $evidenceFullPath -Raw | ConvertFrom-Json -Depth 64
    Assert-True ([string] $evidence.schemaVersion -eq [string] $contract.evidence.schemaVersion) (
        'Unexpected F7 child-input evidence schema.')
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredStringPaths')) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [string] -and -not [string]::IsNullOrWhiteSpace($value)) (
            "F7 child-input evidence string is missing: $path")
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredBooleanPaths')) {
        Assert-True ((Get-JsonPathValue $evidence ([string] $path)) -is [bool]) (
            "F7 child-input evidence boolean is missing: $path")
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredNonNegativeIntegerPaths')) {
        [void] (Assert-NonNegativeInteger (Get-JsonPathValue $evidence ([string] $path)) ([string] $path))
    }
    Assert-MinimumIntegerValues -Evidence $evidence -MinimumValues @(Get-OptionalContractArray $contract.evidence 'minimumIntegerValues')
    Assert-MaximumIntegerValues -Evidence $evidence -MaximumValues @(Get-OptionalContractArray $contract.evidence 'maximumIntegerValues')
    Assert-ExpectedValues -Evidence $evidence -ExpectedValues @(Get-OptionalContractArray $contract.evidence 'expectedValues')
    Assert-NamedRows $evidence $contract.evidence.pointerSequences 'pointer sequence'

    $evidenceSourceFingerprint = Get-JsonPathValue $evidence ([string] $contract.evidence.sourceFingerprintPath)
    Assert-True ($evidenceSourceFingerprint -is [string] -and $evidenceSourceFingerprint -eq $sourceFingerprint) (
        'F7 child-input evidence was not produced from the current validated source fingerprint.')
    $executablePath = Get-JsonPathValue $evidence ([string] $contract.evidence.executablePath)
    $executableHash = Get-JsonPathValue $evidence ([string] $contract.evidence.executableHashPath)
    Assert-True ($executablePath -is [string] -and -not [string]::IsNullOrWhiteSpace($executablePath) -and
        $executableHash -is [string] -and -not [string]::IsNullOrWhiteSpace($executableHash)) (
        'F7 child-input evidence executable provenance is missing.')
    $executableFullPath = [IO.Path]::GetFullPath($executablePath)
    Assert-True (Test-Path -LiteralPath $executableFullPath -PathType Leaf) (
        "F7 child-input evidence executable is missing: $executableFullPath")
    $actualExecutableHash = (Get-FileHash -LiteralPath $executableFullPath -Algorithm SHA256).Hash.ToLowerInvariant()
    Assert-True ($actualExecutableHash -eq $executableHash.ToLowerInvariant()) (
        'F7 child-input evidence executable hash drifted after the live run.')
    $evidenceStatus = 'PASS'
    $validatedEvidencePath = $evidenceFullPath
}

$notVerified = [Collections.Generic.List[string]]::new()
if ($evidenceStatus -ne 'PASS') {
    $notVerified.Add('automated raw-child-HWND input, IMM32, clipboard, and child-root UIA structural evidence')
}
$notVerified.Add('real-user pointer/keyboard/focus/capture/Alt+Tab/minimize/restore/popup acceptance')
$notVerified.Add('Korean IME candidate/caret/selection/clipboard physical acceptance')
$notVerified.Add('Narrator and Accessibility Insights physical UIA inspection')
$notVerified.Add('auxiliary-island behavior, F8 lifecycle/recovery, F9 runner selection, and FG visible product acceptance')

$summary = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk-flutter-input-validation/v1'
    status = 'PASS'
    scopeBoundary = [string] $contract.scopeBoundary
    sourceFingerprint = $sourceFingerprint
    validatedInputs = $validated
    evidenceStatus = $evidenceStatus
    evidencePath = $validatedEvidencePath
    notVerified = @($notVerified)
}
$summary | ConvertTo-Json -Depth 64
