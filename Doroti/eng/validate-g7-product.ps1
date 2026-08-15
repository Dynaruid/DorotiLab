#Requires -Version 5.1
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Cupertino', 'Generated')]
    [string] $Gate
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'flutter-sdk.ps1')
$flutter = Resolve-DorotiFlutterSdk -RepositoryRoot $repoRoot
$migrationRoot = Join-Path $dorotiRoot 'migration/flutter-framework'
$tmpRoot = Join-Path $dorotiRoot '.doroti/tmp/g7-product'
[IO.Directory]::CreateDirectory($tmpRoot) | Out-Null

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Invoke-Captured([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    $lines = @(& $Command 2>&1)
    $lines | ForEach-Object { Write-Host $_ }
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
    return ($lines -join "`n")
}

function Read-Json([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "evidence $Path"
    return Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
}

function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $json = (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n"
    [IO.File]::WriteAllText($Path, $json, [Text.UTF8Encoding]::new($false))
}

function Get-MarkerJson([string] $Output, [string] $Marker) {
    $match = [Text.RegularExpressions.Regex]::Match($Output, "(?m)^$([Text.RegularExpressions.Regex]::Escape($Marker))=(?<json>\{.+\})$")
    Assert-True $match.Success "$Marker output marker"
    return $match.Groups['json'].Value | ConvertFrom-Json
}

function Assert-Selection([object] $Actual, [object] $Expected, [string] $Name) {
    foreach ($property in $Expected.PSObject.Properties) {
        Assert-True ($Actual.PSObject.Properties.Name -contains $property.Name) "$Name $($property.Name) presence"
        Assert-True ([string]$Actual.($property.Name) -ceq [string]$property.Value) "$Name $($property.Name) value"
    }
}

function Invoke-CupertinoGate {
    $referenceRoot = Join-Path $dorotiRoot 'validation/cases/g7-cupertino-adaptive-reference'
    Push-Location $referenceRoot
    try {
        Invoke-Checked { & $flutter.FlutterCommand pub get } 'G7-2 pinned adaptive reference pub get failed'
        $referenceOutput = Invoke-Captured {
            & $flutter.FlutterCommand test test/adaptive_reference_test.dart --reporter expanded
        } 'G7-2 pinned adaptive reference failed'
    }
    finally { Pop-Location }
    $reference = Get-MarkerJson $referenceOutput 'G7_ADAPTIVE_TRACE'

    $project = Join-Path $dorotiRoot 'validation/Doroti.Validation.G7Product/Doroti.Validation.G7Product.csproj'
    $dll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G7Product/bin/Release/net10.0/Doroti.Validation.G7Product.dll'
    $managedPath = Join-Path $tmpRoot 'cupertino-managed.json'
    Invoke-Checked { dotnet build $project --configuration Release --nologo --no-restore } 'G7-2 managed product build failed'
    Invoke-Checked { dotnet $dll --evidence $managedPath } 'G7-2 managed Cupertino/adaptive trace failed'
    $managed = Read-Json $managedPath
    Assert-True ($managed.status -eq 'pass') 'managed adaptive trace status'
    $managedWindows = @($managed.trace | Where-Object platform -eq 'windows')
    $managedMac = @($managed.trace | Where-Object platform -eq 'macOS')
    Assert-True ($managedWindows.Count -eq 1 -and $managedMac.Count -eq 1) 'managed adaptive platform traces'
    Assert-Selection $managedWindows[0].selection $reference.windows.selection 'Windows adaptive reference'
    Assert-Selection $managedMac[0].selection $reference.macOS.selection 'macOS adaptive reference'
    Assert-True ([long]$managedWindows[0].callbackCount -ge 2 -and [long]$managedMac[0].callbackCount -ge 2) 'adaptive managed behavior callbacks'
    Assert-True (@($managedWindows[0].semantics | Where-Object hasAction).Count -eq 3 -and
        @($managedMac[0].semantics | Where-Object hasAction).Count -eq 3) 'adaptive managed semantics actions'

    # Recreate the reviewed compiler source so promotion identity is a producer
    # property rather than a stale local-candidate assumption.
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g5-4.ps1') } 'G7-2 Cupertino regenerated candidate failed'
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g7-baseline.ps1') -Shard Compatibility } 'G7-2 Cupertino promotion identity failed'
    $compatibility = Read-Json (Join-Path $migrationRoot 'g7-compatibility-debt.json')
    Assert-True ([long]$compatibility.forbiddenPatterns.promotedProductDirectDiffs.count -eq 0) 'Cupertino promoted product identity'

    $cupertino = Read-Json (Join-Path $migrationRoot 'g6-cupertino-component-matrix.json')
    $native = Read-Json (Join-Path $migrationRoot 'g7-native-interaction-evidence.json')
    Assert-True ([long]$cupertino.counts.presented -eq 55 -and [long]$cupertino.counts.interactiveNativePointer -eq 1) 'Cupertino predecessor presented/interactive path'
    Assert-True ($native.status -eq 'pass' -and @($native.causalTrace | Where-Object capability -eq 'key').Count -eq 1 -and
        @($native.causalTrace | Where-Object capability -eq 'semantics action').Count -eq 1) 'Tier A native key/semantics capability reuse'

    $evidencePath = Join-Path $migrationRoot 'g7-cupertino-adaptive-evidence.json'
    Write-Json $evidencePath ([ordered]@{
        schemaVersion='doroti.g7-cupertino-adaptive-evidence/v1';milestone='G7-2';capturedAtUtc=[DateTimeOffset]::UtcNow;status='pass'
        validationHost=$(if ($IsMacOS) {'macos-arm64'} elseif ($IsWindows) {'windows'} else {'other'})
        flutterRevision=$flutter.Revision
        tierA=[ordered]@{
            presented=[ordered]@{status='pass-predecessor';count=[long]$cupertino.counts.presented;evidence='g6-cupertino-component-matrix.json'}
            pointer=[ordered]@{status='pass-predecessor-cupertino-button';directCallbackNativePasses=0;evidence='g6-cupertino-waves/C0-evidence.json'}
            key=[ordered]@{status='pass-capability-reuse';evidence='g7-native-interaction-evidence.json#g7i-key'}
            semantics=[ordered]@{status='pass-managed-cupertino-and-native-capability-reuse';managedActions=3;evidence='g7-native-interaction-evidence.json#g7i-semantics-action'}
        }
        adaptive=[ordered]@{
            status='pass';reference=$reference;doroti=$managed.trace
            selectionContract='ThemeData.platform; Switch.adaptive retains Material rendering and applies Cupertino colors on macOS'
        }
        promotion=[ordered]@{status='pass';generatedDirectHotfixes=0;productIdentityDiffs=0;candidate='migration/generated-candidates/g5-4-reviewed/projects/Cupertino'}
        boundaries=[ordered]@{windowsLive='verified-predecessor';physical='notVerified-G7-6';macosDesktopTarget='notVerified-nonblocking';validationOnMacOS='supported'}
    })
    Write-Output "G7-2 Cupertino/adaptive product closure: PASS ($evidencePath)"
}

function Invoke-GeneratedGate {
    $dartRoot = Join-Path $repoRoot 'DorotiDemoApp/dart'
    Push-Location $dartRoot
    try {
        Invoke-Checked { & $flutter.FlutterCommand pub get } 'G7-2 generated DemoApp pub get failed'
        Invoke-Checked { & $flutter.DartCommand format --output=none --set-exit-if-changed lib/main.dart test/product_scenario_test.dart } 'G7-2 generated DemoApp format failed'
        $analyzeOutput = Invoke-Captured { & $flutter.FlutterCommand analyze --no-pub } 'G7-2 generated DemoApp Flutter analyze failed'
        Assert-True ($analyzeOutput -match 'No issues found') 'Flutter analyze zero diagnostics'
        $scenarioOutput = Invoke-Captured {
            & $flutter.FlutterCommand test test/product_scenario_test.dart --reporter expanded
        } 'G7-2 generated DemoApp product scenario failed'
    }
    finally { Pop-Location }
    $scenario = Get-MarkerJson $scenarioOutput 'G7_GENERATED_TRACE'
    Assert-True ((@($scenario.navigation) -join '|') -ceq 'home|details|home') 'generated navigation trace'
    Assert-True ((@($scenario.state) -join '|') -ceq 'pressed=0|pressed=1|pressed=1') 'generated state preservation trace'
    Assert-True ((@($scenario.semantics) -join '|') -ceq 'open-details:tap|back-home:tap') 'generated semantics trace'

    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-generated-demo.ps1') -Shard Compiler } 'G7-2 generated compiler gate failed'
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-generated-demo.ps1') -Shard Package } 'G7-2 external package-only gate failed'
    $releaseRoot = Join-Path $dorotiRoot 'artifacts/g6-release/0.2.0-beta'
    $compiler = Read-Json (Join-Path $releaseRoot 'compiler-metrics.json')
    $package = Read-Json (Join-Path $releaseRoot 'package-metrics.json')
    Assert-True ($compiler.cleanDigest -ceq $compiler.incrementalDigest -and [long]$compiler.silentSuccesses -eq 0) 'generated compiler determinism/diagnostics'
    Assert-True ([long]$package.repositoryPrivateFallbacks -eq 0 -and [long]$package.repositoryPrivateProjectReferences -eq 0) 'external package-only boundary'

    $predecessor = Read-Json (Join-Path $migrationRoot 'g6-generated-demo-evidence.json')
    $handwritten = Read-Json (Join-Path $migrationRoot 'g5-3-widgets-behavior.json')
    $handwrittenProduct = Read-Json (Join-Path $migrationRoot 'g6-material-demo-evidence.json')
    Assert-True ($predecessor.liveWindows.outcome -eq 'presented' -and $predecessor.liveWindows.backend -eq 'skia-wgl-opengl-gpu' -and
        -not [bool]$predecessor.liveWindows.softwareFallbackUsed -and [bool]$predecessor.liveWindows.resourceClosure.IsBalanced) 'generated Windows live predecessor'
    Assert-True ((@($handwritten.traces.W5) -join '|') -ceq 'overlay:back|overlay:front|route:install|route:push|route:pop:result|route:complete:result|route:dispose') 'handwritten navigation contract'
    Assert-True ($handwrittenProduct.builder.interaction.beforeState -cne $handwrittenProduct.builder.interaction.afterState -and
        [long]$handwrittenProduct.builder.automation.nodeCount -gt 0) 'handwritten state/semantics contract'

    $evidencePath = Join-Path $migrationRoot 'g7-generated-demo-evidence.json'
    Write-Json $evidencePath ([ordered]@{
        schemaVersion='doroti.g7-generated-demo-evidence/v1';milestone='G7-2';capturedAtUtc=[DateTimeOffset]::UtcNow;status='pass'
        validationHost=$(if ($IsMacOS) {'macos-arm64'} elseif ($IsWindows) {'windows'} else {'other'})
        source=[ordered]@{dartPackage='DorotiDemoApp/dart';entryPoint='package:doroti_demo_app/main.dart';handwrittenFixture='DorotiDemoApp/Program.cs'}
        tooling=[ordered]@{flutterRevision=$flutter.Revision;flutterAnalyzeDiagnostics=0;compilerAnalyzerDiagnostics=0;dartFormat='pass'}
        scenario=[ordered]@{
            status='pass';generated=$scenario
            handwritten=[ordered]@{
                navigation=$handwritten.traces.W5
                state=[ordered]@{before=$handwrittenProduct.builder.interaction.beforeState;after=$handwrittenProduct.builder.interaction.afterState}
                semanticsNodeCount=[long]$handwrittenProduct.builder.automation.nodeCount
                evidence=@('g5-3-widgets-behavior.json','g6-material-demo-evidence.json')
            }
            normalizedContract=[ordered]@{
                navigation='push-pop-return';state='mutation-survives-route-round-trip';semantics='actionable-controls-exposed';result='match'
            }
        }
        compiler=$compiler
        package=[ordered]@{status='pass';externalRoot=$package.externalRoot;repositoryPrivateFallbacks=[long]$package.repositoryPrivateFallbacks;repositoryPrivateProjectReferences=[long]$package.repositoryPrivateProjectReferences;publishedEntry=$package.publishedEntry}
        liveWindows=[ordered]@{status='pass-predecessor';outcome=$predecessor.liveWindows.outcome;backend=$predecessor.liveWindows.backend;softwareFallbackUsed=[bool]$predecessor.liveWindows.softwareFallbackUsed;interactionChangedPixels=[long]$predecessor.liveWindows.interactionChangedPixels;semanticsNodeCount=[long]$predecessor.liveWindows.semanticsNodeCount;resourceClosure=$predecessor.liveWindows.resourceClosure;evidence='g6-generated-demo-evidence.json'}
        closure=[ordered]@{repositoryPrivateProjectFallbacks=0;candidateFallbacks=0;unsupportedSilentSuccesses=0}
        boundaries=[ordered]@{currentHostWindowsLaunch=$(if ($IsWindows) {'available-not-required-predecessor-preserved'} else {'not-runnable-on-macos-predecessor-preserved'});physical='notVerified-G7-6';fullScreenRaster='not-required'}
    })
    Write-Output "G7-2 generated Dart product closure: PASS ($evidencePath)"
}

if ($Gate -eq 'Cupertino') { Invoke-CupertinoGate } else { Invoke-GeneratedGate }
Write-Output "G7-2 product gate '$Gate': PASS"
