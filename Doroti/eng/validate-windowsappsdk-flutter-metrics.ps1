#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ContractPath = (Join-Path $PSScriptRoot '../validation/contracts/windowsappsdk-flutter-metrics.json'),

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
        "F3 metrics evidence integer is missing or invalid: $Path")
    return $integerValue
}

function Assert-NonNegativeInteger([object] $Value, [string] $Path) {
    $integerValue = ConvertTo-Int64 $Value $Path
    Assert-True ($integerValue -ge 0) "F3 metrics evidence integer is negative: $Path"
    return $integerValue
}

function Assert-ExpectedValues([object] $Evidence, [object] $ExpectedValues, [string] $Prefix) {
    foreach ($expected in @($ExpectedValues)) {
        $relativePath = [string] $expected.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = Get-JsonPathValue $Evidence $relativePath
        $expectedValue = $expected.value
        Assert-True ($actual -eq $expectedValue) (
            "F3 metrics evidence value drifted: $path = '$actual', expected '$expectedValue'.")
    }
}

function Assert-MinimumIntegerValues([object] $Evidence, [object] $MinimumValues, [string] $Prefix) {
    foreach ($minimum in @($MinimumValues)) {
        $relativePath = [string] $minimum.path
        $path = if ([string]::IsNullOrWhiteSpace($Prefix)) { $relativePath } else { "$Prefix.$relativePath" }
        $actual = ConvertTo-Int64 (Get-JsonPathValue $Evidence $relativePath) $path
        $expectedMinimum = ConvertTo-Int64 $minimum.value "$path minimum"
        Assert-True ($actual -ge $expectedMinimum) (
            "F3 metrics evidence minimum drifted: $path = '$actual', minimum '$expectedMinimum'.")
    }
}

function Assert-DpiMatrix([object] $Evidence, [object] $MatrixContract) {
    $matrixPath = [string] $MatrixContract.path
    $matrixValue = Get-JsonPathValue $Evidence $matrixPath
    Assert-True ($matrixValue -is [Collections.IEnumerable] -and $matrixValue -isnot [string]) (
        "F3 metrics evidence matrix is missing or not an array: $matrixPath")
    $actualCases = @($matrixValue)
    $expectedCases = @($MatrixContract.expected)
    Assert-True ($actualCases.Count -eq $expectedCases.Count) (
        "F3 metrics evidence matrix cardinality drifted: $($actualCases.Count), expected $($expectedCases.Count).")

    for ($index = 0; $index -lt $expectedCases.Count; $index++) {
        $case = $actualCases[$index]
        $expectedCase = $expectedCases[$index]
        $prefix = "$matrixPath[$index]"
        $scale = ConvertTo-Int64 (Get-JsonPathValue $case 'scalePercent') "$prefix.scalePercent"
        $dpi = ConvertTo-Int64 (Get-JsonPathValue $case 'dpi') "$prefix.dpi"
        Assert-True ($scale -eq (ConvertTo-Int64 $expectedCase.scalePercent "$prefix expected scalePercent")) (
            "F3 metrics DPI matrix scale ordering drifted at index ${index}: '$scale'.")
        Assert-True ($dpi -eq (ConvertTo-Int64 $expectedCase.dpi "$prefix expected dpi")) (
            "F3 metrics DPI matrix DPI drifted at index ${index}: '$dpi'.")

        $dprPath = [string] $MatrixContract.devicePixelRatioPath
        $dprValue = Get-JsonPathValue $case $dprPath
        [double] $dpr = 0
        Assert-True ([double]::TryParse(
                [string] $dprValue,
                [Globalization.NumberStyles]::Float,
                [Globalization.CultureInfo]::InvariantCulture,
                [ref] $dpr)) "F3 metrics evidence decimal is missing or invalid: $prefix.$dprPath"
        $expectedDpr = [double] $scale / 100.0
        Assert-True ([Math]::Abs($dpr - $expectedDpr) -lt 0.000001) (
            "F3 metrics devicePixelRatio drifted at index ${index}: '$dpr', expected '$expectedDpr'.")

        foreach ($relativePath in (Get-OptionalContractArray $MatrixContract 'requiredBooleanPaths')) {
            $path = [string] $relativePath
            $value = Get-JsonPathValue $case $path
            Assert-True ($value -is [bool]) "F3 metrics evidence boolean is missing: $prefix.$path"
        }
        foreach ($relativePath in (Get-OptionalContractArray $MatrixContract 'requiredNonNegativeIntegerPaths')) {
            $path = [string] $relativePath
            [void] (Assert-NonNegativeInteger (Get-JsonPathValue $case $path) "$prefix.$path")
        }
        Assert-MinimumIntegerValues $case (Get-OptionalContractArray $MatrixContract 'minimumIntegerValues') $prefix
        Assert-ExpectedValues $case (Get-OptionalContractArray $MatrixContract 'expectedValues') $prefix
        foreach ($group in (Get-OptionalContractArray $MatrixContract 'equalIntegerPathGroups')) {
            $paths = @($group)
            Assert-True ($paths.Count -ge 2) "F3 metrics matrix equality group is malformed at $prefix."
            $baselinePath = [string] $paths[0]
            $baseline = ConvertTo-Int64 (Get-JsonPathValue $case $baselinePath) "$prefix.$baselinePath"
            for ($pathIndex = 1; $pathIndex -lt $paths.Count; $pathIndex++) {
                $relativePath = [string] $paths[$pathIndex]
                $actual = ConvertTo-Int64 (Get-JsonPathValue $case $relativePath) "$prefix.$relativePath"
                Assert-True ($actual -eq $baseline) (
                    "F3 metrics matrix exact-value drifted: $prefix.$relativePath = '$actual', expected '$baseline'.")
            }
        }
    }
}

Assert-True (Test-Path -LiteralPath $contractFullPath -PathType Leaf) (
    "Windows App SDK Flutter metrics contract is missing: $contractFullPath")
$contract = Get-Content -LiteralPath $contractFullPath -Raw | ConvertFrom-Json -Depth 64
Assert-True ([string] $contract.schemaVersion -eq 'doroti.windowsappsdk-flutter-metrics-contract/v1') (
    'Unexpected Windows App SDK Flutter metrics contract schema.')

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
        "F3 metrics contract input is missing: $relativePath")
    $text = Get-Content -LiteralPath $fullPath -Raw
    foreach ($literal in (Get-OptionalContractArray $item 'requiredLiterals')) {
        $requiredLiteral = [string] $literal
        Assert-True (-not [string]::IsNullOrWhiteSpace($requiredLiteral)) (
            "Required literal is empty: $id")
        Assert-True ($text.Contains($requiredLiteral, [StringComparison]::Ordinal)) (
            "F3 metrics contract missing '$requiredLiteral' in $relativePath")
    }
    $previousIndex = -1
    foreach ($literal in (Get-OptionalContractArray $item 'orderedLiterals')) {
        $orderedLiteral = [string] $literal
        $index = $text.IndexOf($orderedLiteral, [StringComparison]::Ordinal)
        Assert-True ($index -ge 0) "Ordered F3 metrics literal is missing: '$orderedLiteral' in $relativePath"
        Assert-True ($index -gt $previousIndex) (
            "F3 metrics lifecycle ordering drifted in ${relativePath}: '$orderedLiteral' appears before its required predecessor.")
        $previousIndex = $index
    }
    foreach ($literal in (Get-OptionalContractArray $item 'forbiddenLiterals')) {
        $forbiddenLiteral = [string] $literal
        Assert-True (-not $text.Contains($forbiddenLiteral, [StringComparison]::Ordinal)) (
            "F3 metrics contract forbids '$forbiddenLiteral' in $relativePath")
    }
    $validated.Add([ordered]@{
            id = $id
            path = $relativePath.Replace('\', '/')
            sha256 = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
        })
    $validatedPaths.Add($relativePath)
}

Assert-True ($validated.Count -gt 0) 'The F3 metrics contract has no project or source inputs.'
$sourceFingerprint = Get-SourceFingerprint $validatedPaths.ToArray()

$evidenceStatus = 'notVerified'
$validatedEvidencePath = $null
if ($RequireEvidence -and [string]::IsNullOrWhiteSpace($EvidencePath)) {
    throw 'F3 metrics evidence is required but -EvidencePath was not supplied.'
}
if (-not [string]::IsNullOrWhiteSpace($EvidencePath)) {
    $evidenceFullPath = [IO.Path]::GetFullPath($EvidencePath)
    Assert-True (Test-Path -LiteralPath $evidenceFullPath -PathType Leaf) (
        "F3 metrics evidence is missing: $evidenceFullPath")
    $evidence = Get-Content -LiteralPath $evidenceFullPath -Raw | ConvertFrom-Json -Depth 64
    Assert-True ([string] $evidence.schemaVersion -eq [string] $contract.evidence.schemaVersion) (
        'Unexpected F3 metrics evidence schema.')
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredStringPaths')) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [string] -and -not [string]::IsNullOrWhiteSpace($value)) (
            "F3 metrics evidence string is missing: $path")
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredBooleanPaths')) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [bool]) "F3 metrics evidence boolean is missing: $path"
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredNonNegativeIntegerPaths')) {
        [void] (Assert-NonNegativeInteger (Get-JsonPathValue $evidence ([string] $path)) ([string] $path))
    }
    Assert-MinimumIntegerValues $evidence (Get-OptionalContractArray $contract.evidence 'minimumIntegerValues') ''
    Assert-ExpectedValues $evidence (Get-OptionalContractArray $contract.evidence 'expectedValues') ''
    Assert-DpiMatrix $evidence $contract.evidence.matrix

    $evidenceSourceFingerprint = Get-JsonPathValue $evidence ([string] $contract.evidence.sourceFingerprintPath)
    Assert-True ($evidenceSourceFingerprint -is [string] -and
        $evidenceSourceFingerprint -eq $sourceFingerprint) (
        'F3 metrics evidence was not produced from the current validated source fingerprint.')
    $executablePath = Get-JsonPathValue $evidence ([string] $contract.evidence.executablePath)
    $executableHash = Get-JsonPathValue $evidence ([string] $contract.evidence.executableHashPath)
    Assert-True ($executablePath -is [string] -and -not [string]::IsNullOrWhiteSpace($executablePath) -and
        $executableHash -is [string] -and -not [string]::IsNullOrWhiteSpace($executableHash)) (
        'F3 metrics evidence executable provenance is missing.')
    $executableFullPath = [IO.Path]::GetFullPath($executablePath)
    Assert-True (Test-Path -LiteralPath $executableFullPath -PathType Leaf) (
        "F3 metrics evidence executable is missing: $executableFullPath")
    $actualExecutableHash = (Get-FileHash -LiteralPath $executableFullPath -Algorithm SHA256).Hash.ToLowerInvariant()
    Assert-True ($actualExecutableHash -eq $executableHash.ToLowerInvariant()) (
        'F3 metrics evidence executable hash drifted after the live run.')
    $evidenceStatus = 'PASS'
    $validatedEvidencePath = $evidenceFullPath
}

$notVerified = [Collections.Generic.List[string]]::new()
if ($evidenceStatus -ne 'PASS') {
    $notVerified.Add('same-STA raw child HWND physical client metrics and deterministic DPI/display matrix')
    $notVerified.Add('immutable metric/frame generation admission, suspension, and bounded resource evidence')
    $notVerified.Add('self-contained unpackaged launch without MAUI/XAML startup or PATH-native fallback')
}
$notVerified.Add('F4 raster-thread EGL child-window surface creation, recreation, and successful swap/present')
$notVerified.Add('F5 bounded resize handshake, F6 scheduler/vsync, F7 input, F8 lifecycle, and F9 runner selection')
$notVerified.Add('output-level visible resize/compositor acceptance')

$summary = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk-flutter-metrics-validation/v1'
    status = 'PASS'
    scopeBoundary = [string] $contract.scopeBoundary
    sourceFingerprint = $sourceFingerprint
    validatedInputs = $validated
    evidenceStatus = $evidenceStatus
    evidencePath = $validatedEvidencePath
    notVerified = @($notVerified)
}

$summary | ConvertTo-Json -Depth 32
