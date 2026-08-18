#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$manifestPath = Join-Path $dorotiRoot 'validation/fcr2-semantics/fixture-manifest.json'
$fixturePath = Join-Path $dorotiRoot 'validation/fcr2-semantics/flutter-animation-fixture.dart'
$contractProject = Join-Path $dorotiRoot 'validation/fcr2-semantics/Doroti.Validation.Fcr2Semantics.csproj'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/fcr2-semantic-evidence.json'

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Read-Json([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "JSON exists: $Path"
    Get-Content -Raw -LiteralPath $Path | ConvertFrom-Json
}

function Resolve-RepositoryPath([string] $RelativePath) {
    [IO.Path]::GetFullPath((Join-Path $repositoryRoot ($RelativePath.Replace('/', '\'))))
}

function Resolve-FlutterPath([string] $RelativePath) {
    [IO.Path]::GetFullPath((Join-Path $flutterRoot ($RelativePath.Replace('/', '\'))))
}

function Read-Text([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "source exists: $Path"
    Get-Content -Raw -LiteralPath $Path
}

Assert-True (Test-Path -LiteralPath $flutterRoot -PathType Container) 'pinned Flutter checkout exists'
$manifest = Read-Json $manifestPath
Assert-True ([string]$manifest.schemaVersion -eq 'doroti.flutter-conformance-fcr2-fixture/v1') 'FCR-2 fixture schema'
$flutterRevision = (& git -C $flutterRoot rev-parse HEAD).Trim()
Assert-True ($LASTEXITCODE -eq 0) 'Flutter revision lookup'
Assert-True ($flutterRevision -eq [string]$manifest.flutterRevision) "Flutter revision pin: expected $($manifest.flutterRevision), got $flutterRevision"
Assert-True (Test-Path -LiteralPath $fixturePath -PathType Leaf) 'extracted Flutter fixture exists'
$fixtureText = Read-Text $fixturePath
foreach ($anchor in @('class Tween<T extends Object?>', 'interpolateOffset', 'completer.completeError', 'subscription.cancel()', 'collectionAndPatternFixture')) {
    Assert-True ($fixtureText.Contains($anchor, [StringComparison]::Ordinal)) "fixture anchor: $anchor"
}

foreach ($source in @($manifest.sources)) {
    $path = Resolve-FlutterPath ([string]$source.path)
    Assert-True (Test-Path -LiteralPath $path -PathType Leaf) "Flutter source: $($source.path)"
    $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $path).Hash.ToLowerInvariant()
    Assert-True ($hash -eq [string]$source.sha256) "Flutter source hash: $($source.path)"
    $text = Read-Text $path
    foreach ($anchor in @($source.anchors)) {
        Assert-True ($text.Contains([string]$anchor, [StringComparison]::Ordinal)) "Flutter source anchor: $($source.path) -> $anchor"
    }
}

$tweenPath = Resolve-RepositoryPath 'Doroti/src/Doroti.Framework.Animation/tween.cs'
$tweenText = Read-Text $tweenPath
Assert-True (-not [regex]::IsMatch($tweenText, '\bdynamic\b|RuntimeBinderException')) 'Tween typed arithmetic has no dynamic binder'
Assert-True ($tweenText.Contains('DartRuntimePrimitives.LerpTweenValue', [StringComparison]::Ordinal)) 'Tween uses typed runtime arithmetic'
Assert-True ($tweenText.Contains('DartRuntimePrimitives.RequireNonNull', [StringComparison]::Ordinal)) 'Tween endpoint cast is explicit'

$runtimeAsyncPath = Resolve-RepositoryPath 'Doroti/src/Doroti.Runtime/DartAsync.cs'
$runtimeAsyncText = Read-Text $runtimeAsyncPath
Assert-True (-not [regex]::IsMatch($runtimeAsyncText, '\basync\s+void\b')) 'runtime has no async void completion primitive'
Assert-True ($runtimeAsyncText.Contains('InvokeErrorHandlerAsync', [StringComparison]::Ordinal)) 'Future error handlers are awaited'

$lowererStatements = Read-Text (Resolve-RepositoryPath 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.Statements.cs')
$lowererDeclarations = Read-Text (Resolve-RepositoryPath 'tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.Declarations.cs')
$futureDiscardBlock = [regex]::Match($lowererStatements, 'Dart deliberately permits an unawaited Future expression[\s\S]{0,400}DartRuntimePrimitives\.Observe\([^\r\n]+').Value
Assert-True ($futureDiscardBlock.Length -gt 0) 'lowerer observes unawaited Future'
Assert-True (-not $futureDiscardBlock.Contains('DartRuntimePrimitives.Ignore(', [StringComparison]::Ordinal)) 'new lowering does not discard Future errors'
Assert-True ($lowererDeclarations.Contains('DartRuntimePrimitives.Assert(() => ', [StringComparison]::Ordinal)) 'constructor assert uses Dart assert primitive'
Assert-True (-not $lowererDeclarations.Contains('System.Diagnostics.Debug.Assert(', [StringComparison]::Ordinal)) 'constructor assert is not a CLR Debug.Assert'

$matrixPath = Resolve-RepositoryPath 'Doroti/src/Doroti.Ui/DartUiValues.cs'
$matrixText = Read-Text $matrixPath
Assert-True ([regex]::IsMatch($matrixText, '\(float\)')) 'matrix host boundary contains explicit float conversion'
$mouseText = Read-Text (Resolve-RepositoryPath 'Doroti/src/Doroti.Framework.Services/mouse_tracking.cs')
Assert-True (-not [regex]::IsMatch($mouseText, '\bdynamic\b|RuntimeBinderException')) 'mouse annotation contract is typed'

function Invoke-Contract([string] $Configuration) {
    Push-Location $repositoryRoot
    try {
        $output = @(& dotnet run --project $contractProject -c $Configuration --nologo 2>&1 | ForEach-Object { $_.ToString() })
        if ($LASTEXITCODE -ne 0) {
            throw "FCR-2 runtime contract failed for $Configuration (exit code $LASTEXITCODE).`n$($output -join "`n")"
        }
        $joined = $output -join "`n"
        Assert-True ($joined.Contains("FCR-2 semantic runtime contract: PASS (configuration=$Configuration", [StringComparison]::Ordinal)) "FCR-2 $Configuration runtime result"
        return $joined
    }
    finally {
        Pop-Location
    }
}

$debugOutput = Invoke-Contract 'Debug'
$releaseOutput = Invoke-Contract 'Release'
$evidence = [ordered]@{
    schemaVersion = 'doroti.flutter-conformance-fcr2-evidence/v1'
    status = 'partial'
    capturedAt = [DateTime]::UtcNow.ToString('o')
    repositoryRevision = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    flutterRevision = $flutterRevision
    fixtureManifest = 'Doroti/validation/fcr2-semantics/fixture-manifest.json'
    runtimeContract = [ordered]@{
        status = 'pass'
        debug = 'pass'
        release = 'pass'
        checks = @(
            'typed Offset/Size/Rect/Vector2/double Tween interpolation',
            'explicit matrix float narrowing boundary',
            'nullable callback and Future result',
            'Future completion/error/cancellation and timer dispose race',
            'collection mutation snapshot and typed pattern switch',
            'Debug assert active and Release assert elided'
        )
    }
    compilerContract = [ordered]@{
        status = 'pass'
        checks = @('Future expression lowering emits Observe', 'constructor assert lowering emits DartRuntimePrimitives.Assert', 'typed Tween product contains no dynamic binder')
    }
    acceptance = [ordered]@{
        status = 'notVerified'
        reason = 'The 60-second DorotiDemoApp interaction log and full regenerated framework closure were not run by this compact semantic contract.'
        notRun = @('InkSparkle product interaction log', '60-second app interaction log', 'full framework regeneration and generated-source diff review')
    }
}
[IO.Directory]::CreateDirectory((Split-Path $evidencePath -Parent)) | Out-Null
$json = (($evidence | ConvertTo-Json -Depth 32) -replace "`r`n", "`n") + "`n"
[IO.File]::WriteAllText($evidencePath, $json, [Text.UTF8Encoding]::new($false))
Write-Output 'Doroti FCR-2 semantic validation: PASS (runtime Debug/Release; app/physical acceptance remains notVerified)'
