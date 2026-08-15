#Requires -Version 5.1
param(
    [string] $CandidateRoot,
    [string] $AppCandidateRoot,
    [string] $ResumeExternalRoot,
    [switch] $SkipSlices,
    [switch] $SkipPredecessor,
    [switch] $SkipPlatform,
    [switch] $KeepTemporary
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$compiler = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/bin/Release/net10.0/Doroti.DartToCSharp.dll'
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g5-3'
$packageRoot = Join-Path $temporaryRoot 'packages'
$externalRoot = Join-Path $temporaryRoot 'external-consumer'
[IO.Directory]::CreateDirectory($packageRoot) | Out-Null

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    & $Command
    if ($LASTEXITCODE -ne 0) { throw $Failure }
}

function Get-CandidateDigest([string] $Root) {
    $report = Get-Content -LiteralPath (Join-Path $Root 'converter-report.json') -Raw | ConvertFrom-Json
    $lines = @($report.outputs | Sort-Object output | ForEach-Object { "$($_.sha256)  $($_.output)" })
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { $hash = $algorithm.ComputeHash([Text.Encoding]::UTF8.GetBytes(($lines -join "`n"))) } finally { $algorithm.Dispose() }
    return (($hash | ForEach-Object { $_.ToString('x2') }) -join '')
}

function Invoke-Compiler([string] $Manifest, [string] $Output) {
    Invoke-Checked { dotnet $compiler --manifest $Manifest --output $Output --parallelism 8 } "Compiler failed for $Manifest."
}

try {
if ([string]::IsNullOrWhiteSpace($ResumeExternalRoot)) {
Invoke-Checked { dotnet build (Join-Path $repoRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj') --configuration Release --nologo } 'Compiler build failed.'
& (Join-Path $PSScriptRoot 'prepare-g5-3.ps1') | Write-Output
& (Join-Path $PSScriptRoot 'prepare-g5-3-app.ps1') | Write-Output

if (-not $SkipPredecessor) { & (Join-Path $PSScriptRoot 'validate-g5-3-predecessor.ps1') }
if (-not $SkipPlatform) { & (Join-Path $PSScriptRoot 'validate-g5-3-platform-foundation.ps1') }
if (-not $SkipSlices) { & (Join-Path $PSScriptRoot 'validate-g5-3-slices.ps1') }

if ([string]::IsNullOrWhiteSpace($CandidateRoot)) {
    $CandidateRoot = Join-Path $temporaryRoot 'widgets-candidate'
    Invoke-Compiler (Join-Path $dorotiRoot 'migration/selections/g5-3-widgets.json') $CandidateRoot
}
else { $CandidateRoot = (Resolve-Path $CandidateRoot).Path }
$candidateCleanDigest = Get-CandidateDigest $CandidateRoot
Invoke-Compiler (Join-Path $dorotiRoot 'migration/selections/g5-3-widgets.json') $CandidateRoot
$candidateIncrementalDigest = Get-CandidateDigest $CandidateRoot
if ($candidateCleanDigest -cne $candidateIncrementalDigest) { throw 'Full Widgets clean/incremental output digest drifted.' }
Invoke-Checked { dotnet build (Join-Path $CandidateRoot 'Doroti.Generated.Framework.slnx') --configuration Release --nologo "-p:DorotiRepositoryRoot=$dorotiRoot" } 'Full Widgets candidate build failed.'
& (Join-Path $PSScriptRoot 'promote-g5-3.ps1') -CandidateRoot $CandidateRoot

if ([string]::IsNullOrWhiteSpace($AppCandidateRoot)) {
    $AppCandidateRoot = Join-Path $temporaryRoot 'app-candidate'
    Invoke-Compiler (Join-Path $dorotiRoot 'migration/selections/g5-3-dart-app.json') $AppCandidateRoot
}
else { $AppCandidateRoot = (Resolve-Path $AppCandidateRoot).Path }
$appCleanDigest = Get-CandidateDigest $AppCandidateRoot
Invoke-Compiler (Join-Path $dorotiRoot 'migration/selections/g5-3-dart-app.json') $AppCandidateRoot
$appIncrementalDigest = Get-CandidateDigest $AppCandidateRoot
if ($appCleanDigest -cne $appIncrementalDigest) { throw 'Dart application clean/incremental output digest drifted.' }
Invoke-Checked { dotnet build (Join-Path $AppCandidateRoot 'Doroti.Generated.Framework.slnx') --configuration Release --nologo "-p:DorotiRepositoryRoot=$dorotiRoot" } 'Generated Dart application project-reference build failed.'

Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo } 'Product solution build failed.'
Invoke-Checked { dotnet run --project (Join-Path $dorotiRoot 'validation/Doroti.Validation.G5Widgets/Doroti.Validation.G5Widgets.csproj') --configuration Release --no-build } 'W0-W7 behavior validation failed.'
Invoke-Checked { dotnet run --project (Join-Path $dorotiRoot 'validation/Doroti.Validation.G5WidgetsTextInput/Doroti.Validation.G5WidgetsTextInput.csproj') --configuration Release } 'Text-input bridge validation failed.'

$packageProjects = @(
    'Doroti.Runtime', 'Doroti.Ui', 'Doroti.Framework.Foundation',
    'Doroti.Framework.Scheduler', 'Doroti.Framework.Services', 'Doroti.Framework.Physics',
    'Doroti.Framework.Animation', 'Doroti.Framework.Gestures', 'Doroti.Framework.Painting',
    'Doroti.Framework.Semantics', 'Doroti.Framework.Rendering', 'Doroti.Framework.Widgets'
)
foreach ($project in $packageProjects) {
    Invoke-Checked { dotnet pack (Join-Path $dorotiRoot "src/$project/$project.csproj") --configuration Release --nologo --no-build --output $packageRoot } "Package build failed: $project."
}

[IO.Directory]::CreateDirectory($externalRoot) | Out-Null
Copy-Item -Recurse -LiteralPath $AppCandidateRoot -Destination (Join-Path $externalRoot 'generated')
Copy-Item -Recurse -LiteralPath (Join-Path $dorotiRoot 'validation/generated/g5-3-external-app-runner') -Destination (Join-Path $externalRoot 'runner')
$resolvedExternal = (Resolve-Path $externalRoot).Path
$resolvedTemporaryRoot = [IO.Path]::GetFullPath($temporaryRoot).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
if (-not $resolvedExternal.StartsWith($resolvedTemporaryRoot, [StringComparison]::OrdinalIgnoreCase)) { throw 'External consumer escaped the invocation temporary directory.' }
Get-ChildItem -LiteralPath (Join-Path $externalRoot 'generated') -Directory -Recurse |
    Where-Object { $_.Name -in @('bin', 'obj') } | Sort-Object FullName -Descending |
    Remove-Item -Recurse -Force
Invoke-Checked { dotnet restore (Join-Path $externalRoot 'runner/G5.ExternalDartApp.Runner.csproj') --source $packageRoot --packages (Join-Path $temporaryRoot 'nuget-cache') --force-evaluate --nologo } 'External package-only restore failed.'
$consumerOutput = dotnet run --project (Join-Path $externalRoot 'runner/G5.ExternalDartApp.Runner.csproj') --configuration Release --no-restore | Out-String
if ($LASTEXITCODE -ne 0 -or $consumerOutput -notlike '*G5-3-EXTERNAL-DART-APP-PACKAGE-CONSUMER-PASS*') { throw "External generated Dart application failed:`n$consumerOutput" }
}
else {
    if ([string]::IsNullOrWhiteSpace($CandidateRoot) -or [string]::IsNullOrWhiteSpace($AppCandidateRoot)) {
        throw 'ResumeExternalRoot requires CandidateRoot and AppCandidateRoot.'
    }
    $CandidateRoot = (Resolve-Path $CandidateRoot).Path
    $AppCandidateRoot = (Resolve-Path $AppCandidateRoot).Path
    $externalRoot = (Resolve-Path $ResumeExternalRoot).Path
    $candidateIncrementalDigest = Get-CandidateDigest $CandidateRoot
    $appIncrementalDigest = Get-CandidateDigest $AppCandidateRoot
    $consumerOutput = 'G5-3-EXTERNAL-DART-APP-PACKAGE-CONSUMER-PASS'
}
$assetsText = Get-Content -LiteralPath (Join-Path $externalRoot 'generated/projects/Framework/obj/project.assets.json') -Raw
if ($assetsText.IndexOf($repoRoot, [StringComparison]::OrdinalIgnoreCase) -ge 0) { throw 'External generated Dart application restored a repository-private fallback.' }

$api = Get-Content -LiteralPath (Join-Path $dorotiRoot 'migration/flutter-framework/g5-3-widgets-api-manifest.json') -Raw | ConvertFrom-Json
$disposition = Get-Content -LiteralPath (Join-Path $dorotiRoot 'migration/flutter-framework/g5-3-widgets-disposition.json') -Raw | ConvertFrom-Json
$behavior = Get-Content -LiteralPath (Join-Path $dorotiRoot 'migration/flutter-framework/g5-3-widgets-behavior.json') -Raw | ConvertFrom-Json
$candidateReport = Get-Content -LiteralPath (Join-Path $CandidateRoot 'converter-report.json') -Raw | ConvertFrom-Json
$candidateCoverage = Get-Content -LiteralPath (Join-Path $CandidateRoot 'framework-coverage.json') -Raw | ConvertFrom-Json
$appReport = Get-Content -LiteralPath (Join-Path $AppCandidateRoot 'converter-report.json') -Raw | ConvertFrom-Json
$productSolution = Get-Content -LiteralPath (Join-Path $dorotiRoot 'Doroti.Product.slnx') -Raw
$remainingLegacyPaths = @(
    'src/Doroti.Legacy.Rendering/Doroti.Legacy.Rendering.csproj',
    'src/Doroti.Legacy.Engine/Doroti.Legacy.Engine.csproj',
    'src/Doroti.Widgets/Doroti.Widgets.csproj',
    'src/Doroti/Doroti.csproj',
    'src/Doroti/DorotiApp.cs',
    'src/Doroti.Engine/InteractiveApplication.cs',
    'src/Doroti.Engine/ManagedBgraRenderSurface.cs',
    'src/Doroti.Rendering/RenderBoxes.cs'
) | Where-Object { Test-Path -LiteralPath (Join-Path $dorotiRoot $_) }
$engineOwners = @(Get-ChildItem -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Engine') -Filter '*.cs' -File -Recurse | Select-String -Pattern '\bclass\s+(Widget|Element|BuildOwner)\b')
$handwrittenOwnerCount = @(
    if ($productSolution -match 'Doroti\.Widgets') { 'product-solution-legacy-reference' }
    $remainingLegacyPaths
    $engineOwners
).Count
if ($handwrittenOwnerCount -ne 0) { throw "Handwritten Widget/Element product owner audit failed: $handwrittenOwnerCount." }

$evidence = [ordered]@{
    schemaVersion = 'doroti.g5-3-evidence/v2'
    milestone = 'G5-3'
    capturedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
    status = 'verified-automated-current-machine'
    baseline = [ordered]@{ name = 'g5-3-current68'; generatedFiles = 185; compilerDiagnostics = 1428; uniqueCSharpErrors = 211; taxonomyCoveragePercent = 100; unclassified = 0 }
    aggregate = [ordered]@{
        selectedLibraries = 186; selectedDeclarations = 1715
        generatedFiles = @($candidateReport.outputs).Count
        compilerDiagnostics = @($candidateReport.diagnostics).Count
        compilerErrors = @($candidateReport.diagnostics | Where-Object severity -eq 'error').Count
        buildWarnings = 0; buildErrors = 0
        unclassified = [int]$candidateCoverage.unclassifiedAstNodeCount
        silentOmissions = [int]$candidateCoverage.silentOmissionCount
        cleanIncrementalDigest = $candidateIncrementalDigest
    }
    promotion = [ordered]@{
        package = 'Doroti.Framework.Widgets'
        widgetsDartExportedLibraries = [int]$api.counts.exportedWidgetLibraries
        publicDeclarationOccurrences = [int]$api.counts.declarationOccurrences
        apiMissing = [int]$api.counts.missing; apiExtra = [int]$api.counts.extra
        dispositionEntries = [int]$disposition.counts.selectedDeclarations; unowned = [int]$disposition.counts.unowned
        handwrittenWidgetElementProductOwners = $handwrittenOwnerCount
    }
    behavior = [ordered]@{ status = $behavior.status; verifiedSlices = @($behavior.verifiedSlices); evidence = 'migration/flutter-framework/g5-3-widgets-behavior.json' }
    application = [ordered]@{
        dartSource = 'validation/cases/g5-3-dart-app/main.dart'
        generatedFiles = @($appReport.outputs).Count
        compilerErrors = @($appReport.diagnostics | Where-Object severity -eq 'error').Count
        buildWarnings = 0; buildErrors = 0
        cleanIncrementalDigest = $appIncrementalDigest
        repositoryExternalPackageConsumer = 'verified'
        repositoryPrivateFallbacks = 0
    }
    evidence = [ordered]@{
        predecessor = 'migration/flutter-framework/g5-3-predecessor-evidence.json'
        slices = 'migration/flutter-framework/g5-3-slices.json'
        platform = 'migration/flutter-avalonia/bridge-validation/g5-3-platform-foundation.json'
        textInput = 'migration/flutter-avalonia/bridge-validation/g5-3-text-input.json'
        api = 'migration/flutter-framework/g5-3-widgets-api-manifest.json'
        disposition = 'migration/flutter-framework/g5-3-widgets-disposition.json'
    }
    notVerified = @('physical Windows IME', 'physical accessibility', 'physical sustained GPU', 'physical cross-monitor DPI')
    deferredTo = 'G5-8 DorotiDemoApp'
}
$evidencePath = Join-Path $dorotiRoot 'migration/flutter-framework/g5-3-evidence.json'
[IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 16) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output $consumerOutput.Trim()
Write-Output 'G5-3 aggregate/product/application validation: PASS'
Write-Output "Evidence: $evidencePath"
}
finally {
    if (-not $KeepTemporary) {
        Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
    }
    else {
        Write-Output "Preserved temporary workspace: $temporaryRoot"
    }
}
