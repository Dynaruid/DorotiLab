#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$manifestPath = Join-Path $dorotiRoot 'validation/fcr4-retained-rendering/fixture-manifest.json'
$contractProject = Join-Path $dorotiRoot 'validation/fcr4-retained-rendering/Doroti.Validation.Fcr4RetainedRendering.csproj'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/fcr4-retained-rendering-evidence.json'

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Read-Text([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "source exists: $Path"
    return Get-Content -Raw -LiteralPath $Path
}

function Invoke-Contract([string] $Configuration) {
    Push-Location $repositoryRoot
    try {
        # Contract execution has the repository-wide 20-minute test ceiling.
        $outputPath = Join-Path ([IO.Path]::GetTempPath()) ("doroti-fcr4-$Configuration-$([guid]::NewGuid()).log")
        try {
            $process = Start-Process dotnet -ArgumentList @('run', '--project', $contractProject, '-c', $Configuration, '--nologo') `
                -NoNewWindow -PassThru -RedirectStandardOutput $outputPath -RedirectStandardError "$outputPath.err"
            Assert-True ($process.WaitForExit(1200000)) "FCR-4 runtime contract timeout ($Configuration)"
            $output = ((Get-Content -Raw -LiteralPath $outputPath) + (Get-Content -Raw -LiteralPath "$outputPath.err"))
            Assert-True ($process.ExitCode -eq 0) "FCR-4 runtime contract exit ($Configuration): $output"
            Assert-True ($output.Contains("FCR-4 retained rendering runtime contract: PASS (configuration=$Configuration", [StringComparison]::Ordinal)) "FCR-4 runtime contract result ($Configuration)"
        }
        finally {
            Remove-Item -LiteralPath $outputPath, "$outputPath.err" -Force -ErrorAction SilentlyContinue
        }
    }
    finally { Pop-Location }
}

Assert-True (Test-Path -LiteralPath $flutterRoot -PathType Container) 'pinned Flutter checkout exists'
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
Assert-True ([string]$manifest.schemaVersion -eq 'doroti.flutter-conformance-fcr4-fixture/v1') 'FCR-4 fixture schema'
$flutterRevision = (& git -C $flutterRoot rev-parse HEAD).Trim()
Assert-True ($flutterRevision -eq [string]$manifest.flutterRevision) "Flutter revision pin: expected $($manifest.flutterRevision), got $flutterRevision"
foreach ($source in @($manifest.sources)) {
    $path = Join-Path $flutterRoot ([string]$source.path).Replace('/', '\')
    Assert-True (Test-Path -LiteralPath $path -PathType Leaf) "Flutter source: $($source.path)"
    $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $path).Hash.ToLowerInvariant()
    Assert-True ($hash -eq [string]$source.sha256) "Flutter source hash: $($source.path)"
    $text = Read-Text $path
    foreach ($anchor in @($source.anchors)) {
        Assert-True ($text.Contains([string]$anchor, [StringComparison]::Ordinal)) "Flutter source anchor: $($source.path) -> $anchor"
    }
}

$layers = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Rendering/layer.cs')
Assert-True ($layers.Contains('builder.addRetained(this._engineLayer!)', [StringComparison]::Ordinal)) 'clean layer emits retained node'
Assert-True ($layers.Contains('ReferenceEquals(this._engineLayer, __value)', [StringComparison]::Ordinal)) 'engine-layer in-place reuse does not dispose itself'
Assert-True ($layers.Contains('child__48661._addToSceneWithRetainedRendering(builder)', [StringComparison]::Ordinal)) 'container delegates children through retained decision'

$graphics = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Ui/GraphicsAndSemanticsContracts.cs')
Assert-True ($graphics.Contains('RetainedResourceDiagnostics', [StringComparison]::Ordinal)) 'retained resource diagnostics contract'
Assert-True ($graphics.Contains('EngineLayer.RecordSnapshot()', [StringComparison]::Ordinal)) 'retained snapshot accounting'
Assert-True ($graphics.Contains('EngineLayer.RecordReuse()', [StringComparison]::Ordinal)) 'retained reuse accounting'
Assert-True ($graphics.Contains('new SceneRetainedPayload', [StringComparison]::Ordinal)) 'retained node snapshots immutable command list'

$mauiRaster = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiSkiaCapabilities.cs')
Assert-True ($mauiRaster.Contains('case "retained" when command.HostPayload is SceneRetainedPayload retained', [StringComparison]::Ordinal)) 'MAUI GPU raster replays retained nodes'
Assert-True ($mauiRaster.Contains('canvas.Clear(_backgroundColor)', [StringComparison]::Ordinal)) 'fresh native back buffer is cleared before replay'
Assert-True ($mauiRaster.Contains('"texture"', [StringComparison]::Ordinal) -eq $false) 'unsupported C2 texture cannot be silently rasterized'
Assert-True ($mauiRaster.Contains('"platformView"', [StringComparison]::Ordinal) -eq $false) 'unsupported C2 platform view cannot be silently rasterized'

Invoke-Contract 'Debug'
Invoke-Contract 'Release'
$evidence = [ordered]@{
    schemaVersion = 'doroti.flutter-conformance-fcr4-evidence/v1'
    status = 'partial'
    capturedAt = [DateTime]::UtcNow.ToString('o')
    repositoryRevision = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    flutterRevision = $flutterRevision
    fixtureManifest = 'Doroti/validation/fcr4-retained-rendering/fixture-manifest.json'
    runtimeContract = [ordered]@{
        status = 'pass'
        debug = 'pass'
        release = 'pass'
        checks = @(
            'clean repaint boundary emits one retained node instead of re-recording its picture subtree',
            'dirty scope records only its own subtree while an unchanged sibling remains retained and engine-layer ownership stays live',
            'C1 color-filter scope retained-resource counters return to baseline after release'
        )
    }
    hostContract = [ordered]@{
        status = 'pass'
        checks = @(
            'MAUI GPU host recursively replays immutable retained payloads',
            'fresh native back buffers clear app-owned background before retained replay',
            'unimplemented C2 texture and platform-view scene operations fail with capability diagnostics rather than being ignored'
        )
    }
    acceptance = [ordered]@{
        status = 'notVerified'
        reason = 'This contract does not execute Flutter-reference rendering, native GPU pixel differential, resize/context-recreation stress, or a physical retained-resource soak.'
        notRun = @('Flutter reference differential for C0/C1 payload and pixels', 'Windows native resize/context recreation', 'Android physical scroll and animation repaint soak', 'C2 owner/target implementation matrix')
    }
}
[IO.Directory]::CreateDirectory((Split-Path $evidencePath -Parent)) | Out-Null
[IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 32) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output 'Doroti FCR-4 retained rendering validation: PASS (runtime Debug/Release; native/reference acceptance remains notVerified)'
