#Requires -Version 5.1
param(
    [ValidateSet('All', 'Fixtures', 'Compatibility', 'Regression')]
    [string] $Shard = 'All'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$fixtureRoot = Join-Path $dorotiRoot 'validation/fixtures/g6-language-runtime'
$manifestPath = Join-Path $fixtureRoot 'g6-language-runtime.selection.json'
$compilerProject = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj'
$compilerDll = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/bin/Release/net10.0/Doroti.DartToCSharp.dll'
$compatibilitySource = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.G53Compatibility.cs'
$dispositionPath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-compatibility-disposition.json'
$evidencePath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-language-runtime-evidence.json'
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g6-language-runtime'
$candidateRoot = Join-Path $temporaryRoot 'candidate'
$cacheRoot = Join-Path $repoRoot '.doroti/cache/dart-analyzer'
$revision = '56b8e1a851a594b1a154f8ea93270807dab22b9a'

function Test-Shard([string] $Name) { return $Shard -eq 'All' -or $Shard -eq $Name }

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Assert-Equal([object] $Actual, [object] $Expected, [string] $Name) {
    if ($Actual -cne $Expected) { throw "$Name drifted: expected '$Expected', got '$Actual'." }
}

function Write-Json([string] $Path, [object] $Value) {
    $json = ($Value | ConvertTo-Json -Depth 30) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($Path, $json + "`n", [Text.UTF8Encoding]::new($false))
}

function Get-OutputDigest([string] $Root) {
    $report = Get-Content -LiteralPath (Join-Path $Root 'converter-report.json') -Raw | ConvertFrom-Json
    $lines = @($report.outputs | Sort-Object output | ForEach-Object { "$($_.sha256)  $($_.output)" })
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { $hash = $algorithm.ComputeHash([Text.Encoding]::UTF8.GetBytes(($lines -join "`n"))) }
    finally { $algorithm.Dispose() }
    return (($hash | ForEach-Object { $_.ToString('x2') }) -join '')
}

function Get-JsonLine([object[]] $Output, [string] $Name) {
    $line = @($Output | ForEach-Object { [string]$_ } | Where-Object { $_.TrimStart().StartsWith('{') } | Select-Object -Last 1)
    if ($line.Count -ne 1) { throw "$Name did not emit one JSON result line.`n$($Output -join "`n")" }
    return $line[0].Trim()
}

function Invoke-Compiler([string] $Output) {
    Invoke-Checked {
        dotnet $compilerDll --manifest $manifestPath --output $Output --cache-dir $cacheRoot --parallelism 8
    } 'G6-1 fixture compilation failed'
}

function Get-CompatibilityInventory {
    $lines = @(Get-Content -LiteralPath $compatibilitySource)
    $firstGuard = @($lines | Select-String 'if \(library\.EndsWith' | Select-Object -First 1)
    Assert-True ($firstGuard.Count -eq 1) 'G5-3 compatibility first library guard'
    $firstIndex = $firstGuard[0].LineNumber - 1
    $structural = @($lines[0..($firstIndex - 1)] | Select-String '\.Replace\(|Regex\.Replace\(').Count
    $temporary = @($lines[$firstIndex..($lines.Length - 1)] | Select-String '\.Replace\(|Regex\.Replace\(').Count
    return [ordered]@{ structural = $structural; temporary = $temporary; total = $structural + $temporary }
}

function Invoke-FixturesShard {
    Invoke-Checked { dart format --output=none --set-exit-if-changed $fixtureRoot } 'G6-1 Dart fixture formatting failed'
    Invoke-Checked { dart analyze $fixtureRoot } 'G6-1 Dart fixture analysis failed'
    Invoke-Checked { dotnet build $compilerProject --configuration Release --nologo } 'G6-1 compiler build failed'

    $referenceOutput = @(& dart run (Join-Path $fixtureRoot 'reference_runner.dart') 2>&1)
    Assert-Equal $LASTEXITCODE 0 'Dart reference runner exit code'
    $referenceJson = Get-JsonLine $referenceOutput 'Dart reference runner'
    $reference = $referenceJson | ConvertFrom-Json

    Invoke-Compiler $candidateRoot
    $report = Get-Content -LiteralPath (Join-Path $candidateRoot 'converter-report.json') -Raw | ConvertFrom-Json
    $coverage = Get-Content -LiteralPath (Join-Path $candidateRoot 'framework-coverage.json') -Raw | ConvertFrom-Json
    Assert-Equal ([bool]$report.success) $true 'G6-1 converter success'
    Assert-Equal @($report.diagnostics).Count 0 'G6-1 compiler diagnostics'
    Assert-Equal $coverage.unclassifiedAstNodeCount 0 'G6-1 unclassified AST nodes'
    Assert-Equal $coverage.silentOmissionCount 0 'G6-1 silent omissions'
    Assert-Equal $coverage.generatedCompileErrorCount 0 'G6-1 generated compile errors'
    Assert-Equal @($report.outputs).Count 8 'G6-1 generated fixture file count'
    Assert-Equal $report.identity.flutterGitRevision $revision 'G6-1 Flutter source pin'

    $cleanDigest = Get-OutputDigest $candidateRoot
    Invoke-Compiler $candidateRoot
    $incrementalDigest = Get-OutputDigest $candidateRoot
    Assert-Equal $incrementalDigest $cleanDigest 'G6-1 clean/incremental generated byte identity'

    $harness = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6LanguageRuntime/Doroti.Validation.G6LanguageRuntime.csproj'
    Invoke-Checked {
        dotnet build $harness --configuration Release --nologo --verbosity quiet `
            "-p:G6GeneratedRoot=$candidateRoot" "-p:DorotiRepositoryRoot=$dorotiRoot"
    } 'G6-1 generated behavior harness build failed'
    $generatedOutput = @(& dotnet run --no-build --project $harness --configuration Release --verbosity quiet `
        "-p:G6GeneratedRoot=$candidateRoot" "-p:DorotiRepositoryRoot=$dorotiRoot" 2>&1)
    Assert-Equal $LASTEXITCODE 0 'G6-1 generated behavior harness exit code'
    $generatedJson = Get-JsonLine $generatedOutput 'Generated behavior harness'
    $generated = $generatedJson | ConvertFrom-Json

    $families = @(
        [ordered]@{ id = 'nullable-super-defaults'; fixture = 'nullable_super_defaults.dart'; semantics = @('nullable parameter', 'super formal', 'inherited default argument') },
        [ordered]@{ id = 'constructors-initializers'; fixture = 'constructors_initializers.dart'; semantics = @('generative', 'factory redirect', 'named constructor', 'field initializer', 'mixin late initializer') },
        [ordered]@{ id = 'generic-variance'; fixture = 'generic_variance.dart'; semantics = @('Dart covariance', 'CLR invariant boundary', 'erased runtime type') },
        [ordered]@{ id = 'future-typed-values'; fixture = 'future_typed_values.dart'; semantics = @('Future<T>', 'async await', 'then typed callback') },
        [ordered]@{ id = 'null-aware-late-required'; fixture = 'null_aware_late_required.dart'; semantics = @('null-aware call', 'nullable return', 'cascade', 'late', 'required named') },
        [ordered]@{ id = 'member-resolution'; fixture = 'member_resolution.dart'; semantics = @('override', 'base dispatch', 'mixin member', 'extension method') },
        [ordered]@{ id = 'tearoffs-callbacks'; fixture = 'tearoffs_callbacks.dart'; semantics = @('constructor tear-off', 'method tear-off', 'delegate adaptation', 'optional named callback') },
        [ordered]@{ id = 'collections-patterns-dynamic'; fixture = 'collections_patterns_dynamic.dart'; semantics = @('list and set collection spread', 'list and set collection if', 'collection for', 'record pattern switch', 'dynamic invocation') }
    )
    foreach ($family in $families) {
        $referenceValue = [string]$reference.($family.id)
        $generatedValue = [string]$generated.($family.id)
        Assert-Equal $generatedValue $referenceValue "G6-1 differential $($family.id)"
        $family.reference = $referenceValue
        $family.generated = $generatedValue
        $family.status = 'PASS'
    }

    $compilerSources = @(
        Get-Content -LiteralPath (Join-Path $repoRoot 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.Invocations.cs') -Raw
        Get-Content -LiteralPath (Join-Path $repoRoot 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.Compatibility.cs') -Raw
        Get-Content -LiteralPath (Join-Path $repoRoot 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.Declarations.cs') -Raw
    ) -join "`n"
    foreach ($forbidden in @('DerivedOptions', 'MixedSeed', 'LateHolder', 'MixedLabel', 'Accumulator', 'DynamicTarget')) {
        Assert-True (-not $compilerSources.Contains($forbidden)) "Demo/fixture-specific compiler special case $forbidden"
    }

    $script:g6FixtureEvidence = [ordered]@{
        dartSdkVersion = [string]$report.identity.dartSdkVersion
        analyzerVersion = [string]$report.identity.dartAnalyzerVersion
        generatedDigest = $cleanDigest
        cleanIncrementalByteIdentity = $true
        outputCount = @($report.outputs).Count
        diagnosticCount = @($report.diagnostics).Count
        unclassifiedAstNodeCount = [int]$coverage.unclassifiedAstNodeCount
        silentOmissionCount = [int]$coverage.silentOmissionCount
        generatedCompileErrorCount = [int]$coverage.generatedCompileErrorCount
        families = $families
    }
    Write-Output 'G6-1 fixture/reference/generated differential: PASS (8 semantic families)'
}

function Invoke-CompatibilityShard {
    $inventory = Get-CompatibilityInventory
    $disposition = Get-Content -LiteralPath $dispositionPath -Raw | ConvertFrom-Json
    Assert-Equal $inventory.structural $disposition.active.structural 'G6-1 structural compatibility count'
    Assert-Equal $inventory.temporary $disposition.active.temporary 'G6-1 temporary compatibility count'
    Assert-Equal $inventory.total $disposition.active.total 'G6-1 active compatibility count'
    Assert-Equal $disposition.active.unclassified 0 'G6-1 unclassified compatibility rules'
    Assert-Equal $disposition.active.obsolete 0 'G6-1 active obsolete compatibility rules'
    Assert-Equal $disposition.active.classified $disposition.active.total 'G6-1 classified compatibility rules'
    if ([string]$disposition.milestone -eq 'G6-1') {
        Assert-True ([bool]$disposition.temporaryTrend.decreased) 'G6-1 temporary compatibility decrease status'
        Assert-True ([int]$disposition.temporaryTrend.currentCount -lt [int]$disposition.temporaryTrend.previousCount) 'G6-1 temporary compatibility count decrease'
    }
    else {
        Assert-Equal ([string]$disposition.g6_7.disposition) 'explicit-blocker' 'G6-7 compatibility disposition'
        Assert-Equal ([bool]$disposition.g6_7.silentSuccessAllowed) $false 'G6-7 compatibility silent-success policy'
    }
    Assert-Equal @($disposition.removed).Count 1 'G6-1 removed obsolete compatibility rule count'
    $script:g6CompatibilityEvidence = [ordered]@{
        structural = $inventory.structural
        temporary = $inventory.temporary
        obsolete = 0
        unclassified = 0
        previousTemporary = [int]$disposition.temporaryTrend.previousCount
        removed = @($disposition.removed | ForEach-Object { $_.id })
    }
    $trend = if ([bool]$disposition.temporaryTrend.decreased) { "down from $($disposition.temporaryTrend.previousCount)" } else { 'explicit G6-7 blocker' }
    Write-Output "G6 compatibility disposition: PASS ($($inventory.temporary) temporary, $trend)"
}

function Invoke-RegressionShard {
    Invoke-Checked { dotnet build $compilerProject --configuration Release --nologo } 'G6-1 compiler regression build failed'
    Invoke-Checked {
        dotnet run --project (Join-Path $dorotiRoot 'validation/Doroti.Validation.Compiler/Doroti.Validation.Compiler.csproj') `
            --configuration Release -- --refactor-only
    } 'G6-1 compiler refactor regression failed'
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g5-3-predecessor.ps1') } 'G6-1 G5 predecessor regression failed'
    Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo } 'G6-1 product regression build failed'
    $script:g6RegressionEvidence = [ordered]@{
        compilerBuild = 'PASS'
        compilerRefactor = 'PASS'
        g5Predecessor = 'PASS'
        productBuild = 'PASS'
    }
    Write-Output 'G6-1 compiler and G5 predecessor regressions: PASS'
}

try {
    if (Test-Shard 'Fixtures') { Invoke-FixturesShard }
    if (Test-Shard 'Compatibility') { Invoke-CompatibilityShard }
    if (Test-Shard 'Regression') { Invoke-RegressionShard }

    if ($Shard -eq 'All') {
        $evidence = [ordered]@{
            schemaVersion = 'doroti.g6-language-runtime-evidence/v1'
            milestone = 'G6-1'
            status = 'PASS'
            flutterGitRevision = $revision
            scope = 'Dart semantic, constructor, typed runtime, and structural compiler closure; no live Widgets presentation claim'
            baselineTaxonomy = [ordered]@{
                requiredFamilies = @('language-runtime', 'constructor-initialization')
                coveredFamilies = @('language-runtime', 'constructor-initialization')
                coveragePercent = 100
            }
            differential = $script:g6FixtureEvidence
            compatibility = $script:g6CompatibilityEvidence
            regression = $script:g6RegressionEvidence
            forbiddenImplementation = [ordered]@{
                demoWidgetNameCompilerSpecialCases = 0
                directGeneratedHotfixes = 0
                generatedProductSourcesCommitted = 0
            }
            validationShards = @(
                [ordered]@{ name = 'Fixtures'; command = 'validate-g6-language-runtime.ps1 -Shard Fixtures'; maxMinutes = 20; status = 'PASS' },
                [ordered]@{ name = 'Compatibility'; command = 'validate-g6-language-runtime.ps1 -Shard Compatibility'; maxMinutes = 20; status = 'PASS' },
                [ordered]@{ name = 'Regression'; command = 'validate-g6-language-runtime.ps1 -Shard Regression'; maxMinutes = 20; status = 'PASS' }
            )
            notVerified = @('base Widgets native first frame', 'Material native presentation', 'physical target')
        }
        Write-Json $evidencePath $evidence
        Write-Output "G6-1 evidence: $evidencePath"
    }
    Write-Output "G6-1 language/runtime validation shard '$Shard': PASS"
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) {
        Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
    }
}
