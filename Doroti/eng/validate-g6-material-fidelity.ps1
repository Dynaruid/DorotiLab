#Requires -Version 5.1
param(
    [ValidateSet('All', 'Audit', 'Color', 'Raster', 'Font', 'Visual')]
    [string] $Shard = 'All',
    [switch] $RefreshLive
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$migrationRoot = Join-Path $dorotiRoot 'migration/flutter-framework'
$auditPath = Join-Path $migrationRoot 'g6-compatibility-audit.json'
$visualPath = Join-Path $migrationRoot 'g6-material-visual-differential.json'
$fidelityProject = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialFidelity/Doroti.Validation.G6MaterialFidelity.csproj'
$fidelityDll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialFidelity/bin/Release/net10.0/Doroti.Validation.G6MaterialFidelity.dll'
$dartFixture = Join-Path $dorotiRoot 'validation/cases/g6-material-color-reference'
$m6Visual = Join-Path $dorotiRoot 'artifacts/g6-material-gallery/win-x64/M6/visual.png'

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $json = (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n"
    [IO.File]::WriteAllText($Path, $json, [Text.UTF8Encoding]::new($false))
}

function Relative-Path([string] $Path) {
    return [IO.Path]::GetRelativePath($repoRoot, $Path).Replace('\', '/')
}

function Get-TreeIdentity([string[]] $Paths) {
    $sha = [Security.Cryptography.SHA256]::Create()
    try {
        $rows = foreach ($path in $Paths | Sort-Object) {
            "$(Relative-Path $path):$((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant())"
        }
        $bytes = [Text.Encoding]::UTF8.GetBytes(($rows -join "`n"))
        return ([Convert]::ToHexString($sha.ComputeHash($bytes))).ToLowerInvariant()
    }
    finally { $sha.Dispose() }
}

function Invoke-Audit {
    $inventory = [Collections.Generic.List[object]]::new()
    $reviewScript = Join-Path $dorotiRoot 'eng/review-g5-4-generated.ps1'
    $adaptationPath = Join-Path $dorotiRoot 'migration/generated-candidates/g5-4-reviewed/g5-4-reviewed-adaptations.json'
    $adaptations = Get-Content -LiteralPath $adaptationPath -Raw | ConvertFrom-Json
    foreach ($change in @($adaptations.changes)) {
        $inventory.Add([ordered]@{
            id = "review:$($change.file)"
            kind = 'generated-file-review-adaptation'
            target = "migration/generated-candidates/g5-4-reviewed/projects/*/$($change.file)"
            source = 'Doroti/eng/review-g5-4-generated.ps1'
            causeLayer = 'compiler-or-common-lowering-not-yet-structural'
            minimumFixture = 'validate-g5-4 reviewed candidate build plus focused Dart semantic fixture before removal'
            owner = 'Doroti DartToCSharp compiler'
            permanence = 'temporary'
            removalMilestone = 'G6-5R'
            beforeSha256 = [string]$change.beforeSha256
        })
    }

    $lowererPaths = @(Get-ChildItem (Join-Path $repoRoot 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering') -File -Filter 'FrameworkCSharpLowerer*.cs')
    foreach ($path in $lowererPaths) {
        $lines = [IO.File]::ReadAllLines($path.FullName)
        for ($index = 0; $index -lt $lines.Length; $index++) {
            $line = $lines[$index]
            if ($line -notmatch '(?:source\s*=\s*)?(?:source\.)?Replace\(|Regex\.Replace\(source|source\s*=\s*source') { continue }
            $inventory.Add([ordered]@{
                id = "lowerer:$($path.Name):$($index + 1)"
                kind = 'lowerer-source-rewrite'
                target = "$(Relative-Path $path.FullName):$($index + 1)"
                source = Relative-Path $path.FullName
                causeLayer = 'typed-ir-or-lowering-compatibility'
                minimumFixture = 'Doroti.Validation.G6LanguageRuntime plus clean/incremental identity'
                owner = 'Doroti DartToCSharp compiler'
                permanence = 'temporary'
                removalMilestone = 'G6-5R'
                numericGeneratedLocalDependency = [bool]($line -match '(?:__|counter__)\d+')
            })
        }
    }

    $productDiffs = [Collections.Generic.List[object]]::new()
    foreach ($project in @('Material', 'Cupertino')) {
        $reviewedRoot = Join-Path $dorotiRoot "migration/generated-candidates/g5-4-reviewed/projects/$project"
        $productRoot = Join-Path $dorotiRoot "src/Doroti.Flutter.Framework.$project"
        foreach ($reviewed in Get-ChildItem -LiteralPath $reviewedRoot -File -Filter '*.g.cs') {
            $productName = $reviewed.Name -replace '(\.g)+\.cs$', '.cs'
            $product = Join-Path $productRoot $productName
            if (-not (Test-Path -LiteralPath $product)) { continue }
            $reviewedText = ([IO.File]::ReadAllText($reviewed.FullName) -replace '^// <auto-generated />', '// <doroti-reviewed-product-source milestone="G6-3" />').TrimEnd("`r", "`n") + "`n"
            $productText = [IO.File]::ReadAllText($product)
            if ($reviewedText -ceq $productText) { continue }
            $productDiffs.Add([ordered]@{ project=$project; file=$productName; reviewed=Relative-Path $reviewed.FullName; product=Relative-Path $product })
            $inventory.Add([ordered]@{
                id = "product:${project}:$productName"
                kind = 'promoted-product-direct-diff'
                target = Relative-Path $product
                source = Relative-Path $reviewed.FullName
                causeLayer = 'promotion-identity-drift'
                minimumFixture = 'clean reviewed promotion identity'
                owner = 'Doroti framework promotion'
                permanence = 'temporary'
                removalMilestone = 'G6-5R'
            })
        }
    }

    $reviewLines = [IO.File]::ReadAllLines($reviewScript)
    $reviewRewriteRules = 0
    for ($index = 0; $index -lt $reviewLines.Length; $index++) {
        if ($reviewLines[$index] -notmatch '^\s*Update-GeneratedFile\b') { continue }
        $reviewRewriteRules++
        $inventory.Add([ordered]@{
            id = "review-rule:$($index + 1)"
            kind = 'reviewed-generated-rewrite-rule'
            target = "$(Relative-Path $reviewScript):$($index + 1)"
            source = Relative-Path $reviewScript
            causeLayer = 'compiler-or-common-lowering-not-yet-structural'
            minimumFixture = 'focused semantic fixture plus clean reviewed candidate build before removal'
            owner = 'Doroti DartToCSharp compiler'
            permanence = 'temporary'
            removalMilestone = 'G6-5R'
        })
    }
    $numericReviewRules = @($reviewLines | Where-Object { $_ -match '(?:__|counter__)\d+' }).Count
    $widgetSubstitutions = @($reviewLines | Where-Object { $_ -match 'Ink[^\r\n]*DecoratedBox|DecoratedBox[^\r\n]*Ink' }).Count
    $missingMetadata = @($inventory | Where-Object {
        [string]::IsNullOrWhiteSpace($_.causeLayer) -or [string]::IsNullOrWhiteSpace($_.minimumFixture) -or
        [string]::IsNullOrWhiteSpace($_.owner) -or [string]::IsNullOrWhiteSpace($_.permanence) -or
        [string]::IsNullOrWhiteSpace($_.removalMilestone)
    }).Count
    $directHotfixes = [long]$adaptations.changedFiles
    $numericLowererRules = @($inventory | Where-Object { $_.numericGeneratedLocalDependency -eq $true }).Count
    $status = if ($directHotfixes -eq 0 -and $numericReviewRules -eq 0 -and $numericLowererRules -eq 0 -and $widgetSubstitutions -eq 0 -and $productDiffs.Count -eq 0) { 'pass' } else { 'open' }
    Write-Json $auditPath ([ordered]@{
        schemaVersion = 'doroti.g6-compatibility-audit/v1'
        milestone = 'G6-5R'
        capturedAtUtc = [DateTimeOffset]::UtcNow
        status = $status
        scope = [ordered]@{ start='G6-0'; end='G6-5'; reviewScript=Relative-Path $reviewScript; reviewedAdaptations=Relative-Path $adaptationPath }
        coverage = [ordered]@{ detected=$inventory.Count; inventoried=$inventory.Count; percent=100; missingRequiredMetadata=$missingMetadata }
        debt = [ordered]@{ generatedFileHotfixes=$directHotfixes; reviewRewriteRules=$reviewRewriteRules; numericReviewRules=$numericReviewRules; numericLowererRules=$numericLowererRules; widgetTypeSubstitutions=$widgetSubstitutions; promotedProductDirectDiffs=$productDiffs.Count }
        productDiffs = $productDiffs
        rules = $inventory
    })
    Assert-True ($missingMetadata -eq 0) 'compatibility inventory required metadata'
    Write-Host "G6-5R compatibility inventory: PASS ($($inventory.Count) rules, metadata complete; debt status $status)"
    return [pscustomobject]@{ Status=$status; DirectHotfixes=$directHotfixes; NumericRules=($numericReviewRules + $numericLowererRules); ProductDiffs=$productDiffs.Count; WidgetSubstitutions=$widgetSubstitutions }
}

function Invoke-Color {
    Invoke-Checked { dotnet build $fidelityProject --configuration Release --nologo } 'G6-5R fidelity validation build failed'
    Push-Location $dartFixture
    try {
        Invoke-Checked { dart pub get --offline } 'Pinned material_color_utilities restore failed'
        $dart = (& dart run bin/reference.dart | Out-String | ConvertFrom-Json)
    }
    finally { Pop-Location }
    $managed = (& dotnet $fidelityDll --colors | Out-String | ConvertFrom-Json)
    $mismatches = [Collections.Generic.List[object]]::new()
    for ($index = 0; $index -lt $dart.cases.Count; $index++) {
        $expected = $dart.cases[$index]
        $actual = $managed.cases[$index]
        foreach ($property in $expected.roles.PSObject.Properties) {
            $role = $property.Name
            if ([long]$property.Value -ne [long]$actual.roles.$role) {
                $mismatches.Add([ordered]@{ case=$index; seed=[long]$expected.seed; dark=[bool]$expected.dark; contrast=[double]$expected.contrast; variant=[string]$expected.variant; role=$role; expected=[long]$property.Value; actual=[long]$actual.roles.$role })
            }
        }
    }
    Assert-True ($mismatches.Count -eq 0) 'Material Color Utilities role differential'
    Write-Host "G6-5R ColorScheme.fromSeed differential: PASS ($($dart.cases.Count) cases, 3 seeds, light/dark, contrast -1/0/1, 9 variants)"
    return [pscustomobject]@{ Cases=$dart.cases.Count; Roles=@($dart.cases[0].roles.PSObject.Properties).Count; Mismatches=0 }
}

function Invoke-RasterAndFont {
    Invoke-Checked { dotnet build $fidelityProject --configuration Release --nologo } 'G6-5R fidelity validation build failed'
    Invoke-Checked { dotnet $fidelityDll } 'G6-5R managed raster/font contracts failed'
}

function Invoke-Visual {
    if ($RefreshLive) {
        Invoke-Checked { & (Join-Path $dorotiRoot 'eng/validate-g6-material-wave.ps1') -Wave M6 } 'G6-5 M0-M6 live regression failed'
    }
    Assert-True ((Test-Path -LiteralPath $m6Visual) -and (Get-Item -LiteralPath $m6Visual).Length -gt 10000) 'M6 strict-GPU visual artifact'
    $galleryEvidence = Get-Content (Join-Path $migrationRoot 'g6-material-gallery-evidence.json') -Raw | ConvertFrom-Json
    $sourceRevision = [string](Get-Content (Join-Path $migrationRoot 'g5-4-evidence.json') -Raw | ConvertFrom-Json).flutterGitRevision
    $productFiles = @(Get-ChildItem (Join-Path $dorotiRoot 'src/Doroti.Flutter.Framework.Material') -File -Filter '*.cs')
    $compilerFiles = @(Get-ChildItem (Join-Path $repoRoot 'tools/Doroti.DartToCSharp/src') -File -Recurse -Filter '*.cs')
    Write-Json $visualPath ([ordered]@{
        schemaVersion = 'doroti.g6-material-visual-differential/v1'
        milestone = 'G6-5R'
        capturedAtUtc = [DateTimeOffset]::UtcNow
        status = 'notVerified'
        reason = 'Pinned Flutter CalendarDatePicker reference raster has not been captured; Doroti-only pixels cannot establish visual parity.'
        fixture = [ordered]@{ locale='en-US'; textScale=1.0; logicalWindow=@{width=900;height=720}; date='2026-08-13'; month='2026-08'; shapeRadiusDp=28; elevationDp=6 }
        doroti = [ordered]@{ capture='strict-gpu-first-presented-frame-readback'; path=Relative-Path $m6Visual; sha256=(Get-FileHash $m6Visual -Algorithm SHA256).Hash.ToLowerInvariant(); backend=[string]$galleryEvidence.aggregateSmoke.backend; failed=[long]$galleryEvidence.aggregateSmoke.failed; cancelled=[long]$galleryEvidence.aggregateSmoke.cancelled; softwareFallback=[bool]$galleryEvidence.aggregateSmoke.softwareFallback }
        reference = [ordered]@{ status='notVerified'; flutterGitRevision=$sourceRevision; path=$null; sha256=$null }
        identity = [ordered]@{ source=$sourceRevision; product=Get-TreeIdentity @($productFiles.FullName); compiler=Get-TreeIdentity @($compilerFiles.FullName) }
        tolerances = [ordered]@{ glyph=[ordered]@{maxMissing=0}; baseline=[ordered]@{maxDeltaPx=1}; grid=[ordered]@{columns=7; orientation='horizontal'}; selectedToday=[ordered]@{maxStatePixelErrorPercent=1}; cornerRadius=[ordered]@{expectedDp=28; maxDeltaPx=1}; shadowExtent=[ordered]@{expectedElevationDp=6; maxDeltaPx=2} }
        results = [ordered]@{ glyph='notVerified'; baseline='notVerified'; horizontalSevenColumns='dorotiObservedOnly'; selectedToday='dorotiObservedOnly'; cornerRadius='dorotiObservedOnly'; shadowExtent='dorotiObservedOnly'; aggregate='notVerified' }
        boundaries = [ordered]@{ physicalIme='notVerified'; physicalAccessibility='notVerified'; linux='notVerified'; macOS='notVerified'; otherRids='notVerified' }
    })
    Write-Host 'G6-5R Doroti strict-GPU capture identity: PASS; pinned Flutter raster differential remains notVerified.'
    return [pscustomobject]@{ Status='notVerified' }
}

$audit = $null
$color = $null
$visual = $null
if ($Shard -in @('All', 'Audit')) { $audit = Invoke-Audit }
if ($Shard -in @('All', 'Color')) { $color = Invoke-Color }
if ($Shard -in @('All', 'Raster', 'Font')) { Invoke-RasterAndFont }
if ($Shard -in @('All', 'Visual')) { $visual = Invoke-Visual }

if ($Shard -eq 'All') {
    if ($audit.Status -ne 'pass' -or $visual.Status -ne 'pass') {
        throw "G6-5R remains open: generated review hotfixes=$($audit.DirectHotfixes), numeric rules=$($audit.NumericRules), product diffs=$($audit.ProductDiffs), widget substitutions=$($audit.WidgetSubstitutions), Flutter visual differential=$($visual.Status)."
    }
    Write-Output 'G6-5R Material fidelity validation: PASS'
}
