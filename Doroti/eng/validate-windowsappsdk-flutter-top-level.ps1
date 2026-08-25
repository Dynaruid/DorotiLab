#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ContractPath = (Join-Path $PSScriptRoot '../validation/contracts/windowsappsdk-flutter-top-level.json'),

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

function Assert-ExactArrayValues([object] $Actual, [object] $Expected, [string] $Path) {
    Assert-True ($Actual -is [Collections.IEnumerable] -and $Actual -isnot [string]) (
        "F2 top-level evidence value is not an array: $Path")
    Assert-True ($Expected -is [Collections.IEnumerable] -and $Expected -isnot [string]) (
        "F2 top-level contract value is not an array: $Path")

    $actualValues = @($Actual)
    $expectedValues = @($Expected)
    Assert-True ($actualValues.Count -eq $expectedValues.Count) (
        "F2 top-level evidence array length drifted: $Path = $($actualValues.Count), expected $($expectedValues.Count).")
    for ($index = 0; $index -lt $expectedValues.Count; $index++) {
        Assert-True ($actualValues[$index] -eq $expectedValues[$index]) (
            "F2 top-level evidence array drifted at ${Path}[$index]: '$($actualValues[$index])', expected '$($expectedValues[$index])'.")
    }
}

Assert-True (Test-Path -LiteralPath $contractFullPath -PathType Leaf) (
    "Windows App SDK Flutter top-level contract is missing: $contractFullPath")
$contract = Get-Content -LiteralPath $contractFullPath -Raw | ConvertFrom-Json -Depth 32
Assert-True ([string] $contract.schemaVersion -eq 'doroti.windowsappsdk-flutter-top-level-contract/v1') (
    'Unexpected Windows App SDK Flutter top-level contract schema.')

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
        "F2 top-level contract input is missing: $relativePath")
    $text = Get-Content -LiteralPath $fullPath -Raw
    foreach ($literal in (Get-OptionalContractArray $item 'requiredLiterals')) {
        $requiredLiteral = [string] $literal
        Assert-True (-not [string]::IsNullOrWhiteSpace($requiredLiteral)) (
            "Required literal is empty: $id")
        Assert-True ($text.Contains($requiredLiteral, [StringComparison]::Ordinal)) (
            "F2 top-level contract missing '$requiredLiteral' in $relativePath")
    }
    $previousIndex = -1
    foreach ($literal in (Get-OptionalContractArray $item 'orderedLiterals')) {
        $orderedLiteral = [string] $literal
        $index = $text.IndexOf($orderedLiteral, [StringComparison]::Ordinal)
        Assert-True ($index -ge 0) "Ordered F2 top-level literal is missing: '$orderedLiteral' in $relativePath"
        Assert-True ($index -gt $previousIndex) (
            "F2 top-level lifecycle ordering drifted in ${relativePath}: '$orderedLiteral' appears before its required predecessor.")
        $previousIndex = $index
    }
    foreach ($literal in (Get-OptionalContractArray $item 'forbiddenLiterals')) {
        $forbiddenLiteral = [string] $literal
        Assert-True (-not $text.Contains($forbiddenLiteral, [StringComparison]::Ordinal)) (
            "F2 top-level contract forbids '$forbiddenLiteral' in $relativePath")
    }
    $validated.Add([ordered]@{
            id = $id
            path = $relativePath.Replace('\', '/')
            sha256 = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
        })
    $validatedPaths.Add($relativePath)
}

Assert-True ($validated.Count -gt 0) 'The F2 top-level contract has no project or source inputs.'
$sourceFingerprint = Get-SourceFingerprint $validatedPaths.ToArray()

$evidenceStatus = 'notVerified'
$validatedEvidencePath = $null
if ($RequireEvidence -and [string]::IsNullOrWhiteSpace($EvidencePath)) {
    throw 'F2 top-level evidence is required but -EvidencePath was not supplied.'
}
if (-not [string]::IsNullOrWhiteSpace($EvidencePath)) {
    $evidenceFullPath = [IO.Path]::GetFullPath($EvidencePath)
    Assert-True (Test-Path -LiteralPath $evidenceFullPath -PathType Leaf) (
        "F2 top-level evidence is missing: $evidenceFullPath")
    $evidence = Get-Content -LiteralPath $evidenceFullPath -Raw | ConvertFrom-Json -Depth 32
    Assert-True ([string] $evidence.schemaVersion -eq [string] $contract.evidence.schemaVersion) (
        'Unexpected F2 top-level evidence schema.')
    foreach ($path in @($contract.evidence.requiredStringPaths)) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [string] -and -not [string]::IsNullOrWhiteSpace($value)) (
            "F2 top-level evidence string is missing: $path")
    }
    foreach ($path in @($contract.evidence.requiredBooleanPaths)) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [bool]) "F2 top-level evidence boolean is missing: $path"
    }
    foreach ($path in @($contract.evidence.requiredNonNegativeIntegerPaths)) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        [long] $integerValue = 0
        Assert-True ([long]::TryParse([string] $value, [ref] $integerValue) -and $integerValue -ge 0) (
            "F2 top-level evidence non-negative integer is missing: $path")
    }
    foreach ($minimum in @($contract.evidence.minimumIntegerValues)) {
        $path = [string] $minimum.path
        $actual = Get-JsonPathValue $evidence $path
        $expectedMinimum = [long] $minimum.value
        [long] $actualInteger = 0
        Assert-True ([long]::TryParse([string] $actual, [ref] $actualInteger) -and $actualInteger -ge $expectedMinimum) (
            "F2 top-level evidence minimum drifted: $path = '$actual', minimum '$expectedMinimum'.")
    }
    foreach ($expected in @($contract.evidence.expectedValues)) {
        $path = [string] $expected.path
        $actual = Get-JsonPathValue $evidence $path
        $expectedValue = $expected.value
        Assert-True ($actual -eq $expectedValue) (
            "F2 top-level evidence value drifted: $path = '$actual', expected '$expectedValue'.")
    }
    foreach ($expected in @($contract.evidence.expectedArrayValues)) {
        Assert-ExactArrayValues (Get-JsonPathValue $evidence ([string] $expected.path)) $expected.value ([string] $expected.path)
    }
    $evidenceSourceFingerprint = Get-JsonPathValue $evidence ([string] $contract.evidence.sourceFingerprintPath)
    Assert-True ($evidenceSourceFingerprint -is [string] -and
        $evidenceSourceFingerprint -eq $sourceFingerprint) (
        'F2 top-level evidence was not produced from the current validated source fingerprint.')
    $executablePath = Get-JsonPathValue $evidence ([string] $contract.evidence.executablePath)
    $executableHash = Get-JsonPathValue $evidence ([string] $contract.evidence.executableHashPath)
    Assert-True ($executablePath -is [string] -and -not [string]::IsNullOrWhiteSpace($executablePath) -and
        $executableHash -is [string] -and -not [string]::IsNullOrWhiteSpace($executableHash)) (
        'F2 top-level evidence executable provenance is missing.')
    $executableFullPath = [IO.Path]::GetFullPath($executablePath)
    Assert-True (Test-Path -LiteralPath $executableFullPath -PathType Leaf) (
        "F2 top-level evidence executable is missing: $executableFullPath")
    $actualExecutableHash = (Get-FileHash -LiteralPath $executableFullPath -Algorithm SHA256).Hash.ToLowerInvariant()
    Assert-True ($actualExecutableHash -eq $executableHash.ToLowerInvariant()) (
        'F2 top-level evidence executable hash drifted after the live run.')
    $evidenceStatus = 'PASS'
    $validatedEvidencePath = $evidenceFullPath
}

$notVerified = [Collections.Generic.List[string]]::new()
if ($evidenceStatus -ne 'PASS') {
    $notVerified.Add('live same-STA Windows App Runtime, raw HWND/AppWindow/DispatcherQueue lifecycle')
    $notVerified.Add('one standard top-level HWND plus one child view HWND and exact client layout')
    $notVerified.Add('physical min/max conversion, structural first-show ordering, and ordered teardown')
    $notVerified.Add('post-warmup GDI/USER resource boundedness and MAUI/XAML startup absence')
}
$notVerified.Add('real child-window EGL swap and first actual swap callback')
$notVerified.Add('F3 physical metric/display contract, F4 surface/present, resize, input, DPI, and lifecycle integration')
$notVerified.Add('visible blank/white-frame and compositor acceptance')

$summary = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk-flutter-top-level-validation/v1'
    status = 'PASS'
    scopeBoundary = [string] $contract.scopeBoundary
    sourceFingerprint = $sourceFingerprint
    validatedInputs = $validated
    evidenceStatus = $evidenceStatus
    evidencePath = $validatedEvidencePath
    notVerified = @($notVerified)
}

$summary | ConvertTo-Json -Depth 16
