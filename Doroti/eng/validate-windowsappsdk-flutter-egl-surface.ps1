#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ContractPath = (Join-Path $PSScriptRoot '../validation/contracts/windowsappsdk-flutter-egl-surface.json'),

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
        "F4 EGL surface evidence integer is missing or invalid: $Path")
    return $integerValue
}

function Assert-NonNegativeInteger([object] $Value, [string] $Path) {
    $integerValue = ConvertTo-Int64 $Value $Path
    Assert-True ($integerValue -ge 0) "F4 EGL surface evidence integer is negative: $Path"
    return $integerValue
}

function Assert-ExpectedValues([object] $Evidence, [object] $ExpectedValues) {
    foreach ($expected in @($ExpectedValues)) {
        $path = [string] $expected.path
        $actual = Get-JsonPathValue $Evidence $path
        $expectedValue = $expected.value
        Assert-True ($actual -eq $expectedValue) (
            "F4 EGL surface evidence value drifted: $path = '$actual', expected '$expectedValue'.")
    }
}

function Assert-MinimumIntegerValues([object] $Evidence, [object] $MinimumValues) {
    foreach ($minimum in @($MinimumValues)) {
        $path = [string] $minimum.path
        $actual = ConvertTo-Int64 (Get-JsonPathValue $Evidence $path) $path
        $expectedMinimum = ConvertTo-Int64 $minimum.value "$path minimum"
        Assert-True ($actual -ge $expectedMinimum) (
            "F4 EGL surface evidence minimum drifted: $path = '$actual', minimum '$expectedMinimum'.")
    }
}

function Assert-EqualIntegerPathGroups([object] $Evidence, [object] $Groups) {
    foreach ($group in @($Groups)) {
        $paths = @($group)
        Assert-True ($paths.Count -ge 2) 'F4 EGL surface integer equality group is malformed.'
        $baselinePath = [string] $paths[0]
        $baseline = ConvertTo-Int64 (Get-JsonPathValue $Evidence $baselinePath) $baselinePath
        for ($index = 1; $index -lt $paths.Count; $index++) {
            $path = [string] $paths[$index]
            $actual = ConvertTo-Int64 (Get-JsonPathValue $Evidence $path) $path
            Assert-True ($actual -eq $baseline) (
                "F4 EGL surface evidence equality drifted: $path = '$actual', expected '$baseline'.")
        }
    }
}

function Assert-RendererClassification([object] $Evidence, [object] $ClassificationContract) {
    $classificationPath = [string] $ClassificationContract.classificationPath
    $classification = Get-JsonPathValue $Evidence $classificationPath
    Assert-True ($classification -is [string] -and -not [string]::IsNullOrWhiteSpace($classification)) (
        "F4 EGL surface renderer classification is missing: $classificationPath")
    $allowedValues = @($ClassificationContract.allowedValues | ForEach-Object { [string] $_ })
    Assert-True ($allowedValues -contains $classification) (
        "F4 EGL surface renderer classification '$classification' is not allowed.")

    $softwareFallbackPath = [string] $ClassificationContract.softwareFallbackPath
    $softwareFallback = Get-JsonPathValue $Evidence $softwareFallbackPath
    Assert-True ($softwareFallback -is [bool]) (
        "F4 EGL surface renderer software fallback flag is missing: $softwareFallbackPath")
    if ($classification -eq 'software') {
        Assert-True $softwareFallback (
            'F4 EGL surface renderer is classified as software but did not report softwareFallback=true.')
    }
    else {
        Assert-True (-not $softwareFallback) (
            'F4 EGL surface renderer is classified as hardware but reported softwareFallback=true.')
    }

    foreach ($path in (Get-OptionalContractArray $ClassificationContract 'visibleClaimPaths')) {
        $claimPath = [string] $path
        $claim = Get-JsonPathValue $Evidence $claimPath
        Assert-True ($claim -is [bool] -and -not $claim) (
            "F4 EGL surface evidence overclaims visible or hardware-visible acceptance: $claimPath")
    }
}

Assert-True (Test-Path -LiteralPath $contractFullPath -PathType Leaf) (
    "Windows App SDK Flutter EGL surface contract is missing: $contractFullPath")
$contract = Get-Content -LiteralPath $contractFullPath -Raw | ConvertFrom-Json -Depth 64
Assert-True ([string] $contract.schemaVersion -eq 'doroti.windowsappsdk-flutter-egl-surface-contract/v1') (
    'Unexpected Windows App SDK Flutter EGL surface contract schema.')

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
        "F4 EGL surface contract input is missing: $relativePath")
    $text = Get-Content -LiteralPath $fullPath -Raw
    foreach ($literal in (Get-OptionalContractArray $item 'requiredLiterals')) {
        $requiredLiteral = [string] $literal
        Assert-True (-not [string]::IsNullOrWhiteSpace($requiredLiteral)) (
            "Required literal is empty: $id")
        Assert-True ($text.Contains($requiredLiteral, [StringComparison]::Ordinal)) (
            "F4 EGL surface contract missing '$requiredLiteral' in $relativePath")
    }
    $previousIndex = -1
    foreach ($literal in (Get-OptionalContractArray $item 'orderedLiterals')) {
        $orderedLiteral = [string] $literal
        $index = $text.IndexOf($orderedLiteral, [StringComparison]::Ordinal)
        Assert-True ($index -ge 0) "Ordered F4 EGL surface literal is missing: '$orderedLiteral' in $relativePath"
        Assert-True ($index -gt $previousIndex) (
            "F4 EGL surface lifecycle ordering drifted in ${relativePath}: '$orderedLiteral' appears before its required predecessor.")
        $previousIndex = $index
    }
    foreach ($literal in (Get-OptionalContractArray $item 'forbiddenLiterals')) {
        $forbiddenLiteral = [string] $literal
        Assert-True (-not $text.Contains($forbiddenLiteral, [StringComparison]::Ordinal)) (
            "F4 EGL surface contract forbids '$forbiddenLiteral' in $relativePath")
    }
    $validated.Add([ordered]@{
            id = $id
            path = $relativePath.Replace('\', '/')
            sha256 = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
        })
    $validatedPaths.Add($relativePath)
}

Assert-True ($validated.Count -gt 0) 'The F4 EGL surface contract has no project or source inputs.'
$sourceFingerprint = Get-SourceFingerprint $validatedPaths.ToArray()

$evidenceStatus = 'notVerified'
$validatedEvidencePath = $null
if ($RequireEvidence -and [string]::IsNullOrWhiteSpace($EvidencePath)) {
    throw 'F4 EGL surface evidence is required but -EvidencePath was not supplied.'
}
if (-not [string]::IsNullOrWhiteSpace($EvidencePath)) {
    $evidenceFullPath = [IO.Path]::GetFullPath($EvidencePath)
    Assert-True (Test-Path -LiteralPath $evidenceFullPath -PathType Leaf) (
        "F4 EGL surface evidence is missing: $evidenceFullPath")
    $evidence = Get-Content -LiteralPath $evidenceFullPath -Raw | ConvertFrom-Json -Depth 64
    Assert-True ([string] $evidence.schemaVersion -eq [string] $contract.evidence.schemaVersion) (
        'Unexpected F4 EGL surface evidence schema.')
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredStringPaths')) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [string] -and -not [string]::IsNullOrWhiteSpace($value)) (
            "F4 EGL surface evidence string is missing: $path")
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredBooleanPaths')) {
        $value = Get-JsonPathValue $evidence ([string] $path)
        Assert-True ($value -is [bool]) "F4 EGL surface evidence boolean is missing: $path"
    }
    foreach ($path in (Get-OptionalContractArray $contract.evidence 'requiredNonNegativeIntegerPaths')) {
        [void] (Assert-NonNegativeInteger (Get-JsonPathValue $evidence ([string] $path)) ([string] $path))
    }
    Assert-MinimumIntegerValues $evidence (Get-OptionalContractArray $contract.evidence 'minimumIntegerValues')
    Assert-ExpectedValues $evidence (Get-OptionalContractArray $contract.evidence 'expectedValues')
    Assert-EqualIntegerPathGroups $evidence (Get-OptionalContractArray $contract.evidence 'equalIntegerPathGroups')
    Assert-RendererClassification $evidence $contract.evidence.rendererClassification

    $evidenceSourceFingerprint = Get-JsonPathValue $evidence ([string] $contract.evidence.sourceFingerprintPath)
    Assert-True ($evidenceSourceFingerprint -is [string] -and
        $evidenceSourceFingerprint -eq $sourceFingerprint) (
        'F4 EGL surface evidence was not produced from the current validated source fingerprint.')
    $executablePath = Get-JsonPathValue $evidence ([string] $contract.evidence.executablePath)
    $executableHash = Get-JsonPathValue $evidence ([string] $contract.evidence.executableHashPath)
    Assert-True ($executablePath -is [string] -and -not [string]::IsNullOrWhiteSpace($executablePath) -and
        $executableHash -is [string] -and -not [string]::IsNullOrWhiteSpace($executableHash)) (
        'F4 EGL surface evidence executable provenance is missing.')
    $executableFullPath = [IO.Path]::GetFullPath($executablePath)
    Assert-True (Test-Path -LiteralPath $executableFullPath -PathType Leaf) (
        "F4 EGL surface evidence executable is missing: $executableFullPath")
    $actualExecutableHash = (Get-FileHash -LiteralPath $executableFullPath -Algorithm SHA256).Hash.ToLowerInvariant()
    Assert-True ($actualExecutableHash -eq $executableHash.ToLowerInvariant()) (
        'F4 EGL surface evidence executable hash drifted after the live run.')
    $evidenceStatus = 'PASS'
    $validatedEvidencePath = $evidenceFullPath
}

$notVerified = [Collections.Generic.List[string]]::new()
if ($evidenceStatus -ne 'PASS') {
    $notVerified.Add('raster-thread child-HWND EGL surface creation, exact-size recreation, and successful swaps')
    $notVerified.Add('1,000 resize cycles, bounded EGL/Skia resources, and injected context-loss recovery')
    $notVerified.Add('self-contained restricted-PATH native launch and renderer classification without MAUI/XAML startup')
}
$notVerified.Add('F5 bounded resize completion protocol, F6 scheduler/vsync, F7 input, F8 lifecycle, and F9 runner selection')
$notVerified.Add('output-level visible blank/white-frame, compositor-continuity, and hardware-visible acceptance (FG)')

$summary = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk-flutter-egl-surface-validation/v1'
    status = 'PASS'
    scopeBoundary = [string] $contract.scopeBoundary
    sourceFingerprint = $sourceFingerprint
    validatedInputs = $validated
    evidenceStatus = $evidenceStatus
    evidencePath = $validatedEvidencePath
    notVerified = @($notVerified)
}

$summary | ConvertTo-Json -Depth 32
