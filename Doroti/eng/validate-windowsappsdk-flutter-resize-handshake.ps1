#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ContractPath = (Join-Path $PSScriptRoot '../validation/contracts/windowsappsdk-flutter-resize-handshake.json'),

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
        "F5 resize handshake evidence integer is missing or invalid: $Path")
    return $integerValue
}

function Assert-NonNegativeInteger([object] $Value, [string] $Path) {
    $integerValue = ConvertTo-Int64 $Value $Path
    Assert-True ($integerValue -ge 0) "F5 resize handshake evidence integer is negative: $Path"
    return $integerValue
}

function Assert-ExpectedValues([object] $Evidence, [object] $ExpectedValues, [string] $Prefix = '') {
    foreach ($expected in @($ExpectedValues)) {
        $relativePath = [string] $expected.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = Get-JsonPathValue $Evidence $relativePath
        $expectedValue = $expected.value
        Assert-True ($actual -eq $expectedValue) (
            "F5 resize handshake evidence value drifted: $path = '$actual', expected '$expectedValue'.")
    }
}

function Assert-MinimumIntegerValues([object] $Evidence, [object] $MinimumValues, [string] $Prefix = '') {
    foreach ($minimum in @($MinimumValues)) {
        $relativePath = [string] $minimum.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = ConvertTo-Int64 (Get-JsonPathValue $Evidence $relativePath) $path
        $expectedMinimum = ConvertTo-Int64 $minimum.value "$path minimum"
        Assert-True ($actual -ge $expectedMinimum) (
            "F5 resize handshake evidence minimum drifted: $path = '$actual', minimum '$expectedMinimum'.")
    }
}

function Assert-MaximumIntegerValues([object] $Evidence, [object] $MaximumValues, [string] $Prefix = '') {
    foreach ($maximum in @($MaximumValues)) {
        $relativePath = [string] $maximum.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = ConvertTo-Int64 (Get-JsonPathValue $Evidence $relativePath) $path
        $expectedMaximum = ConvertTo-Int64 $maximum.value "$path maximum"
        Assert-True ($actual -le $expectedMaximum) (
            "F5 resize handshake evidence maximum drifted: $path = '$actual', maximum '$expectedMaximum'.")
    }
}

function Assert-RendererClassification([object] $Evidence, [object] $ClassificationContract) {
    $classificationPath = [string] $ClassificationContract.classificationPath
    $classification = Get-JsonPathValue $Evidence $classificationPath
    Assert-True ($classification -is [string] -and -not [string]::IsNullOrWhiteSpace($classification)) (
        "F5 resize handshake renderer classification is missing: $classificationPath")
    $allowedValues = @($ClassificationContract.allowedValues | ForEach-Object { [string] $_ })
    Assert-True ($allowedValues -contains $classification) (
        "F5 resize handshake renderer classification '$classification' is not allowed.")
    $softwareFallbackPath = [string] $ClassificationContract.softwareFallbackPath
    $softwareFallback = Get-JsonPathValue $Evidence $softwareFallbackPath
    Assert-True ($softwareFallback -is [bool]) (
        "F5 resize handshake renderer software fallback flag is missing: $softwareFallbackPath")
    if ($classification -eq 'software') {
        Assert-True $softwareFallback (
            'F5 resize handshake renderer is classified as software but did not report softwareFallback=true.')
    }
    else {
        Assert-True (-not $softwareFallback) (
            'F5 resize handshake renderer is classified as hardware but reported softwareFallback=true.')
    }
    foreach ($path in (Get-OptionalContractArray $ClassificationContract 'visibleClaimPaths')) {
        $claimPath = [string] $path
        $claim = Get-JsonPathValue $Evidence $claimPath
        Assert-True ($claim -is [bool] -and -not $claim) (
            "F5 resize handshake evidence overclaims visible or hardware-visible acceptance: $claimPath")
    }
}

function Assert-NamedMatrix([object] $Evidence, [object] $MatrixContract, [string] $MatrixName) {
    $matrixPath = [string] $MatrixContract.path
    $matrix = Get-JsonPathValue $Evidence $matrixPath
    Assert-True ($matrix -is [Collections.IEnumerable] -and $matrix -isnot [string]) (
        "F5 $MatrixName matrix is missing or not an array: $matrixPath")
    $actualRows = @($matrix)
    $expectedNames = @($MatrixContract.expectedNames | ForEach-Object { [string] $_ })
    Assert-True ($actualRows.Count -eq $expectedNames.Count) (
        "F5 $MatrixName matrix cardinality drifted: $($actualRows.Count), expected $($expectedNames.Count).")
    for ($index = 0; $index -lt $expectedNames.Count; $index++) {
        $row = $actualRows[$index]
        $prefix = "$matrixPath[$index]"
        $name = Get-JsonPathValue $row 'name'
        Assert-True ($name -eq $expectedNames[$index]) (
            "F5 $MatrixName matrix name/order drifted at index ${index}: '$name', expected '$($expectedNames[$index])'.")
        foreach ($relativePath in (Get-OptionalContractArray $MatrixContract 'requiredBooleanPaths')) {
            $path = [string] $relativePath
            $value = Get-JsonPathValue $row $path
            Assert-True ($value -is [bool]) "F5 $MatrixName evidence boolean is missing: $prefix.$path"
        }
        foreach ($relativePath in (Get-OptionalContractArray $MatrixContract 'requiredNonNegativeIntegerPaths')) {
            $path = [string] $relativePath
            [void] (Assert-NonNegativeInteger (Get-JsonPathValue $row $path) "$prefix.$path")
        }
        # Keep the row prefix named: an absent optional matrix array would
        # otherwise shift this argument into the preceding positional slot.
        $minimumValues = @(Get-OptionalContractArray $MatrixContract 'minimumIntegerValues')
        $maximumValues = @(Get-OptionalContractArray $MatrixContract 'maximumIntegerValues')
        $expectedValues = @(Get-OptionalContractArray $MatrixContract 'expectedValues')
        Assert-MinimumIntegerValues -Evidence $row -MinimumValues $minimumValues -Prefix $prefix
        Assert-MaximumIntegerValues -Evidence $row -MaximumValues $maximumValues -Prefix $prefix
        Assert-ExpectedValues -Evidence $row -ExpectedValues $expectedValues -Prefix $prefix
    }
}

Assert-True (Test-Path -LiteralPath $contractFullPath -PathType Leaf) (
    "Windows App SDK Flutter resize handshake contract is missing: $contractFullPath")
$contract = Get-Content -LiteralPath $contractFullPath -Raw | ConvertFrom-Json -Depth 64
Assert-True ([string] $contract.schemaVersion -eq 'doroti.windowsappsdk-flutter-resize-handshake-contract/v1') (
    'Unexpected Windows App SDK Flutter resize handshake contract schema.')

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
        "F5 resize handshake contract input is missing: $relativePath")
    $text = Get-Content -LiteralPath $fullPath -Raw
    foreach ($literal in (Get-OptionalContractArray $item 'requiredLiterals')) {
        $requiredLiteral = [string] $literal
        Assert-True (-not [string]::IsNullOrWhiteSpace($requiredLiteral)) (
            "Required literal is empty: $id")
        Assert-True ($text.Contains($requiredLiteral, [StringComparison]::Ordinal)) (
            "F5 resize handshake contract missing '$requiredLiteral' in $relativePath")
    }
    $previousIndex = -1
    foreach ($literal in (Get-OptionalContractArray $item 'orderedLiterals')) {
        $orderedLiteral = [string] $literal
        $index = $text.IndexOf($orderedLiteral, [StringComparison]::Ordinal)
        Assert-True ($index -ge 0) "Ordered F5 resize handshake literal is missing: '$orderedLiteral' in $relativePath"
        Assert-True ($index -gt $previousIndex) (
            "F5 resize handshake lifecycle ordering drifted in ${relativePath}: '$orderedLiteral' appears before its required predecessor.")
        $previousIndex = $index
    }
    foreach ($literal in (Get-OptionalContractArray $item 'forbiddenLiterals')) {
        $forbiddenLiteral = [string] $literal
        Assert-True (-not $text.Contains($forbiddenLiteral, [StringComparison]::Ordinal)) (
            "F5 resize handshake contract forbids '$forbiddenLiteral' in $relativePath")
    }
    $validated.Add([ordered]@{
            id = $id
            path = $relativePath.Replace('\', '/')
            sha256 = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
        })
    $validatedPaths.Add($relativePath)
}

Assert-True ($validated.Count -gt 0) 'The F5 resize handshake contract has no project or source inputs.'
$sourceFingerprint = Get-SourceFingerprint $validatedPaths.ToArray()

$evidenceStatus = 'notVerified'
$validatedEvidencePath = $null
if ($RequireEvidence -and [string]::IsNullOrWhiteSpace($EvidencePath)) {
    throw 'F5 resize handshake evidence is required but -EvidencePath was not supplied.'
}
if (-not [string]::IsNullOrWhiteSpace($EvidencePath)) {
    $evidenceFullPath = [IO.Path]::GetFullPath($EvidencePath)
    Assert-True (Test-Path -LiteralPath $evidenceFullPath -PathType Leaf) (
        "F5 resize handshake evidence is missing: $evidenceFullPath")
    $evidence = Get-Content -LiteralPath $evidenceFullPath -Raw | ConvertFrom-Json -Depth 64
    Assert-True ([string] $evidence.schemaVersion -eq [string] $contract.evidence.schemaVersion) (
        'Unexpected F5 resize handshake evidence schema.')
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredStringPaths')) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [string] -and -not [string]::IsNullOrWhiteSpace($value)) (
            "F5 resize handshake evidence string is missing: $path")
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredBooleanPaths')) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [bool]) "F5 resize handshake evidence boolean is missing: $path"
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredNonNegativeIntegerPaths')) {
        [void] (Assert-NonNegativeInteger (Get-JsonPathValue $evidence ([string] $path)) ([string] $path))
    }
    Assert-MinimumIntegerValues $evidence (Get-OptionalContractArray $contract.evidence 'minimumIntegerValues')
    Assert-MaximumIntegerValues $evidence (Get-OptionalContractArray $contract.evidence 'maximumIntegerValues')
    Assert-ExpectedValues $evidence (Get-OptionalContractArray $contract.evidence 'expectedValues')
    Assert-RendererClassification $evidence $contract.evidence.rendererClassification
    Assert-NamedMatrix $evidence $contract.evidence.directions 'direction'
    Assert-NamedMatrix $evidence $contract.evidence.terminalCases 'terminal'

    $evidenceSourceFingerprint = Get-JsonPathValue $evidence ([string] $contract.evidence.sourceFingerprintPath)
    Assert-True ($evidenceSourceFingerprint -is [string] -and
        $evidenceSourceFingerprint -eq $sourceFingerprint) (
        'F5 resize handshake evidence was not produced from the current validated source fingerprint.')
    $executablePath = Get-JsonPathValue $evidence ([string] $contract.evidence.executablePath)
    $executableHash = Get-JsonPathValue $evidence ([string] $contract.evidence.executableHashPath)
    Assert-True ($executablePath -is [string] -and -not [string]::IsNullOrWhiteSpace($executablePath) -and
        $executableHash -is [string] -and -not [string]::IsNullOrWhiteSpace($executableHash)) (
        'F5 resize handshake evidence executable provenance is missing.')
    $executableFullPath = [IO.Path]::GetFullPath($executablePath)
    Assert-True (Test-Path -LiteralPath $executableFullPath -PathType Leaf) (
        "F5 resize handshake evidence executable is missing: $executableFullPath")
    $actualExecutableHash = (Get-FileHash -LiteralPath $executableFullPath -Algorithm SHA256).Hash.ToLowerInvariant()
    Assert-True ($actualExecutableHash -eq $executableHash.ToLowerInvariant()) (
        'F5 resize handshake evidence executable hash drifted after the live run.')
    $evidenceStatus = 'PASS'
    $validatedEvidencePath = $evidenceFullPath
}

$notVerified = [Collections.Generic.List[string]]::new()
if ($evidenceStatus -ne 'PASS') {
    $notVerified.Add('same-STA platform and dedicated-MTA raster bounded resize handshake evidence')
    $notVerified.Add('exact generation/extent swap admission, exactly-once terminals, and fault-timeout recovery')
    $notVerified.Add('all edge/corner protocol coverage, post-unblock DwmFlush, and restricted-PATH self-contained launch')
}
$notVerified.Add('F6 scheduler/vsync cadence, F7 input, F8 lifecycle, and F9 product-runner selection')
$notVerified.Add('output-level visible blank/white-frame and compositor-continuity acceptance (FG)')

$summary = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk-flutter-resize-handshake-validation/v1'
    status = 'PASS'
    scopeBoundary = [string] $contract.scopeBoundary
    sourceFingerprint = $sourceFingerprint
    validatedInputs = $validated
    evidenceStatus = $evidenceStatus
    evidencePath = $validatedEvidencePath
    notVerified = @($notVerified)
}

$summary | ConvertTo-Json -Depth 32
