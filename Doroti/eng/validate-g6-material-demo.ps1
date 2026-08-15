#Requires -Version 5.1
param(
    [ValidateSet('All', 'LiveWindows', 'ExternalConsumer', 'Compiler', 'Regression', 'Evidence')]
    [string] $Shard = 'All',
    [switch] $KeepTemporary
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g6-material-demo'
$demoProject = Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.csproj'
$demoDll = Join-Path $repoRoot 'DorotiDemoApp/bin/Release/net10.0/DorotiDemoApp.dll'
$referencePath = Join-Path $repoRoot 'DorotiDemoApp/g6-material-reference.json'
$artifactRoot = Join-Path $dorotiRoot 'artifacts/g6-material-demo/win-x64'
$evidencePath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-material-demo-evidence.json'
$builderEvidence = Join-Path $artifactRoot 'builder/run.json'
$homeEvidence = Join-Path $artifactRoot 'home/run.json'
$uiaEvidence = Join-Path $artifactRoot 'builder/uia.json'
$externalBuilderEvidence = Join-Path $artifactRoot 'external-consumer/builder/run.json'
$externalHomeEvidence = Join-Path $artifactRoot 'external-consumer/home/run.json'

function Test-Shard([string] $Name) { return $Shard -eq 'All' -or $Shard -eq $Name }
function Assert-True([bool] $Condition, [string] $Name) { if (-not $Condition) { throw "$Name failed." } }
function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}
function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $temporary = $Path + '.tmp-' + [Guid]::NewGuid().ToString('N')
    [IO.File]::WriteAllText($temporary, (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
    if ([IO.File]::Exists($Path)) {
        [IO.File]::Copy($temporary, $Path, $true)
        [IO.File]::Delete($temporary)
    } else {
        [IO.File]::Move($temporary, $Path)
    }
}
function Assert-Bounds([object] $Actual, [object] $Expected, [double] $Scale, [double] $Tolerance, [string] $Name) {
    foreach ($property in @('x', 'y', 'width', 'height')) {
        $delta = [Math]::Abs(([double]$Actual.$property / $Scale) - [double]$Expected.$property)
        Assert-True ($delta -le $Tolerance) "$Name $property tolerance"
    }
}
function Assert-MaterialRun([string] $Path, [string] $Entry, [bool] $LongCadence, [bool] $External) {
    $actual = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    $reference = Get-Content -LiteralPath $referencePath -Raw | ConvertFrom-Json
    Assert-True ($actual.outcome -eq 'presented') "$Entry presented outcome"
    Assert-True ($actual.source.flutterRevision -eq $reference.flutterRevision) "$Entry Flutter source pin"
    Assert-True ($actual.source.materialProductAssembly -eq 'Doroti.Framework.Material') "$Entry promoted Material assembly"
    Assert-True ($actual.fixture.entryMode -eq $Entry) "$Entry entry mode"
    Assert-True ($actual.frame.backendIdentity -eq 'skia-wgl-opengl-gpu') "$Entry strict GPU backend"
    Assert-True (-not [bool]$actual.frame.softwareFallbackUsed) "$Entry software fallback blocked"
    Assert-True ([long]$actual.frame.failed -eq 0 -and [long]$actual.frame.cancelled -eq 0) "$Entry framework/raster errors"
    $terminal = [long]$actual.frame.presented + [long]$actual.frame.superseded + [long]$actual.frame.stale + [long]$actual.frame.failed + [long]$actual.frame.cancelled
    Assert-True ([long]$actual.frame.submitted -eq $terminal) "$Entry terminal ACK balance"
    Assert-True ([bool]$actual.resourceClosure.isBalanced) "$Entry native resource closure"
    Assert-True ([long]$actual.interaction.count -ge 7) "$Entry native pointer and six direct interactions"
    Assert-True ([long]$actual.interaction.nativePointerCount -eq 1) "$Entry native pointer interaction"
    Assert-True (@($actual.interaction.nativePointerHitTestTargets | Where-Object { $_ -like '*RenderPointerListener' }).Count -gt 0) "$Entry framework pointer hit-test path"
    Assert-True ([long]$actual.interaction.changedPixelCount -ge [long]$reference.layoutPaintTolerance.minimumChangedPixels) "$Entry interaction pixel change"
    Assert-True ([long]$actual.automation.nodeCount -ge 6) "$Entry component semantics"
    foreach ($component in @('MaterialApp', 'Theme', 'Navigator', 'Scaffold', 'AppBar', 'Card', 'ListTile', 'ElevatedButton', 'Checkbox', 'Radio', 'Switch', 'Slider', 'FloatingActionButton', 'Row', 'Column', 'Stack', 'SingleChildScrollView', 'ListView.builder')) {
        Assert-True ($component -in @($actual.fixture.frameworkPath)) "$Entry framework component $component"
    }
    foreach ($color in @('appBarAndFab', 'cardSurface', 'primaryControls', 'initialStack', 'blackInk')) {
        Assert-True ([long]$actual.initialPixels.colors.$color.count -gt 0) "$Entry initial $color paint"
    }
    Assert-True ([long]$actual.changedPixels.colors.changedStackAndSwitch.count -gt 0) "$Entry changed stack/switch paint"
    $scale = [double]$actual.initialPixels.width / [double]$reference.logicalSize.width
    Assert-True ($scale -gt 0) "$Entry physical scale"
    $tolerance = [double]$reference.layoutPaintTolerance.logicalPixelTolerance
    Assert-Bounds $actual.initialPixels.colors.cardSurface.bounds $reference.layoutPaintTolerance.cardSurfaceBounds $scale $tolerance "$Entry Card"
    Assert-Bounds $actual.initialPixels.colors.initialStack.bounds $reference.layoutPaintTolerance.initialStackBounds $scale $tolerance "$Entry Stack"
    Assert-Bounds $actual.initialPixels.colors.blackInk.bounds $reference.layoutPaintTolerance.blackInkBounds $scale $tolerance "$Entry text ink"
    Assert-True ([long]$actual.initialPixels.colors.blackInk.count -ge ([long]$reference.layoutPaintTolerance.minimumBlackInkPixelsAt1x * $scale * $scale)) "$Entry text ink coverage"
    Assert-True ([long]$actual.initialPixels.colors.primaryControls.count -ge ([long]$reference.layoutPaintTolerance.minimumPrimaryControlPixelsAt1x * $scale * $scale)) "$Entry primary control paint"
    Assert-True ((-not $External -and $actual.source.consumer -eq 'repository-product-project') -or ($External -and $actual.source.consumer -eq 'clean-package-only-external')) "$Entry consumer identity"
    if ($LongCadence) {
        Assert-True ([long]$actual.cadence.requestedFrames -eq 300) '300 requested cadence frames'
        Assert-True ([double]$actual.cadence.requestedDurationMs -eq 30000) '30 second requested cadence'
        Assert-True ([long]$actual.cadence.presented -ge 300) '300 presented cadence frames'
        Assert-True ([double]$actual.cadence.actualDurationMs -ge 30000) '30 second actual cadence'
        Assert-True ([long]$actual.interaction.count -ge 13) 'native pointer plus six external UIA interactions'
    }
    return $actual
}
function Update-AggregateEvidence {
    $builder = if (Test-Path -LiteralPath $builderEvidence) { Get-Content $builderEvidence -Raw | ConvertFrom-Json } else { $null }
    $homeRun = if (Test-Path -LiteralPath $homeEvidence) { Get-Content $homeEvidence -Raw | ConvertFrom-Json } else { $null }
    $uia = if (Test-Path -LiteralPath $uiaEvidence) { Get-Content $uiaEvidence -Raw | ConvertFrom-Json } else { $null }
    $externalBuilder = if (Test-Path -LiteralPath $externalBuilderEvidence) { Get-Content $externalBuilderEvidence -Raw | ConvertFrom-Json } else { $null }
    $externalHome = if (Test-Path -LiteralPath $externalHomeEvidence) { Get-Content $externalHomeEvidence -Raw | ConvertFrom-Json } else { $null }
    $complete = $null -ne $builder -and $null -ne $homeRun -and $null -ne $uia -and $null -ne $externalBuilder -and $null -ne $externalHome
    Write-Json $evidencePath ([ordered]@{
        schemaVersion = 'doroti.g6-material-demo-evidence/v1'
        milestone = 'G6-3'
        capturedAtUtc = [DateTime]::UtcNow.ToString('O')
        status = if ($complete) { 'verified-windows-x64-strict-gpu' } else { 'partial' }
        target = 'actual HWND / skia-wgl-opengl-gpu'
        builder = $builder
        home = $homeRun
        externalUia = $uia
        cleanExternalConsumer = [ordered]@{ builder = $externalBuilder; home = $externalHome }
        artifacts = 'Doroti/artifacts/g6-material-demo/win-x64'
        physicalDevice = [ordered]@{ status = 'notVerified'; scope = 'automated Windows x64 native only' }
    })
}

try {
    if (Test-Shard 'LiveWindows') {
        if (Test-Path -LiteralPath $artifactRoot) {
            $resolved = [IO.Path]::GetFullPath($artifactRoot)
            $expected = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'artifacts/g6-material-demo/win-x64'))
            if ($resolved -cne $expected) { throw "Refusing to clean unexpected artifact path: $resolved" }
            [IO.Directory]::Delete($resolved, $true)
        }
        [IO.Directory]::CreateDirectory((Join-Path $artifactRoot 'builder')) | Out-Null
        [IO.Directory]::CreateDirectory((Join-Path $artifactRoot 'home')) | Out-Null
        Invoke-Checked {
            dotnet build $demoProject --configuration Release --nologo --no-restore --disable-build-servers --tl:off -nodeReuse:false -p:RunAnalyzers=false -p:UseSharedCompilation=false
        } 'G6-3 DemoApp build failed'
        $readyPath = Join-Path $temporaryRoot 'ready.json'
        $stdoutPath = Join-Path $temporaryRoot 'builder-stdout.log'
        $stderrPath = Join-Path $temporaryRoot 'builder-stderr.log'
        $arguments = @($demoDll, '--smoke', '--entry', 'builder', '--frames', '300', '--duration-ms', '15000', '--cadence-duration-ms', '30000', '--evidence', $builderEvidence, '--artifact-dir', (Join-Path $artifactRoot 'builder'), '--ready', $readyPath, '--require-external-uia')
        # This shard validates the visible HWND and its first native click.
        # Launch normally so Windows grants the app the same activation path
        # as an interactive user launch; only background helpers are hidden.
        $process = Start-Process dotnet -ArgumentList $arguments -PassThru -WindowStyle Normal -RedirectStandardOutput $stdoutPath -RedirectStandardError $stderrPath
        $deadline = [DateTime]::UtcNow.AddSeconds(20)
        while (-not (Test-Path -LiteralPath $readyPath) -and -not $process.HasExited -and [DateTime]::UtcNow -lt $deadline) { Start-Sleep -Milliseconds 50 }
        if (-not (Test-Path -LiteralPath $readyPath)) { throw "G6-3 builder did not publish readiness.`n$(Get-Content $stderrPath -Raw)" }
        $ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
        & (Join-Path $PSScriptRoot 'invoke-g6-material-demo-uia.ps1') -ProcessId $ready.processId -WindowHandle $ready.hwnd -SemanticsNames @($ready.semanticsNames) -OutputPath $uiaEvidence
        if (-not $process.WaitForExit(120000)) { $process.Kill(); throw 'G6-3 builder exceeded the 120 second live budget.' }
        $process.Refresh()
        if ([int]$process.ExitCode -ne 0) { throw "G6-3 builder failed.`n$(Get-Content $stdoutPath -Raw)`n$(Get-Content $stderrPath -Raw)" }
        Assert-MaterialRun $builderEvidence 'builder' $true $false | Out-Null
        $uia = Get-Content -LiteralPath $uiaEvidence -Raw | ConvertFrom-Json
        Assert-True ([bool]$uia.success -and @($uia.controls).Count -eq 6) 'external UIA six-control evidence'
        Invoke-Checked {
            dotnet $demoDll --smoke --entry home --frames 3 --duration-ms 15000 --cadence-duration-ms 0 --evidence $homeEvidence --artifact-dir (Join-Path $artifactRoot 'home')
        } 'G6-3 MaterialApp.home/Navigator smoke failed'
        Assert-MaterialRun $homeEvidence 'home' $false $false | Out-Null
        Update-AggregateEvidence
        Write-Output 'G6-3 builder/home, screenshot geometry, UIA, 30 second/300 frame gate: PASS'
    }

    if (Test-Shard 'ExternalConsumer') {
        $packageRoot = Join-Path $temporaryRoot 'packages'
        $externalRoot = Join-Path $temporaryRoot 'external-consumer'
        $nugetCache = Join-Path $temporaryRoot 'nuget-cache'
        [IO.Directory]::CreateDirectory($packageRoot) | Out-Null
        [IO.Directory]::CreateDirectory($externalRoot) | Out-Null
        Invoke-Checked { dotnet pack (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo --no-build --disable-build-servers --tl:off -nodeReuse:false --output $packageRoot } 'G6-3 product package graph failed'
        Copy-Item -LiteralPath (Join-Path $dorotiRoot 'validation/generated/g6-material-external-consumer/G6.Material.ExternalConsumer.csproj') -Destination $externalRoot
        Copy-Item -LiteralPath (Join-Path $repoRoot 'DorotiDemoApp/Program.cs') -Destination (Join-Path $externalRoot 'Program.cs')
        $externalProject = Join-Path $externalRoot 'G6.Material.ExternalConsumer.csproj'
        $nugetConfig = Join-Path $externalRoot 'NuGet.Config'
        $escapedPackageRoot = [Security.SecurityElement]::Escape($packageRoot)
        $nugetConfigText = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="doroti-local" value="$escapedPackageRoot" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
"@
        [IO.File]::WriteAllText($nugetConfig, $nugetConfigText, [Text.UTF8Encoding]::new($false))
        Invoke-Checked { dotnet restore $externalProject --configfile $nugetConfig --packages $nugetCache --force-evaluate --nologo --disable-build-servers --tl:off -nodeReuse:false } 'Clean Material consumer restore failed'
        $assetsPath = Join-Path $externalRoot 'obj/project.assets.json'
        $assetsText = Get-Content -LiteralPath $assetsPath -Raw
        $assets = $assetsText | ConvertFrom-Json
        $nonPackageLibraries = @($assets.libraries.PSObject.Properties | Where-Object { $_.Value.type -ne 'package' })
        Assert-True ($nonPackageLibraries.Count -eq 0) 'external consumer project/private fallback'
        Invoke-Checked { dotnet build $externalProject --configuration Release --no-restore --nologo --disable-build-servers --tl:off -nodeReuse:false } 'Clean Material consumer build failed'
        [IO.Directory]::CreateDirectory((Split-Path $externalBuilderEvidence -Parent)) | Out-Null
        [IO.Directory]::CreateDirectory((Split-Path $externalHomeEvidence -Parent)) | Out-Null
        Invoke-Checked { dotnet run --project $externalProject --configuration Release --no-build -- --smoke --entry builder --frames 3 --duration-ms 15000 --cadence-duration-ms 0 --package-only-consumer --evidence $externalBuilderEvidence --artifact-dir (Split-Path $externalBuilderEvidence -Parent) } 'Clean Material builder smoke failed'
        Invoke-Checked { dotnet run --project $externalProject --configuration Release --no-build -- --smoke --entry home --frames 3 --duration-ms 15000 --cadence-duration-ms 0 --package-only-consumer --evidence $externalHomeEvidence --artifact-dir (Split-Path $externalHomeEvidence -Parent) } 'Clean Material home smoke failed'
        Assert-MaterialRun $externalBuilderEvidence 'builder' $false $true | Out-Null
        Assert-MaterialRun $externalHomeEvidence 'home' $false $true | Out-Null
        Update-AggregateEvidence
        Write-Output 'G6-3 clean package-only external consumer builder/home gate: PASS'
    }

    if (Test-Shard 'Compiler') {
        Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-language-runtime.ps1') -Shard Fixtures } 'G6-3 compiler/runtime fixtures failed'
        Invoke-Checked { dotnet build-server shutdown --msbuild --vbcscompiler } 'G6-3 compiler fixture build-server shutdown failed'
    }
    if (Test-Shard 'Regression') {
        Invoke-Checked { dotnet run --project (Join-Path $dorotiRoot 'validation/Doroti.Validation.G5Widgets/Doroti.Validation.G5Widgets.csproj') --configuration Release } 'G5 Widgets regression failed'
        Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo --no-restore --disable-build-servers --tl:off -maxcpucount:1 -nodeReuse:false -p:UseSharedCompilation=false } 'Doroti product solution regression failed'
        Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g6-widgets-live.ps1') -Shard Evidence } 'G6-2 committed evidence regression failed'
    }
    if (Test-Shard 'Evidence') {
        $aggregate = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
        Assert-True ($aggregate.status -eq 'verified-windows-x64-strict-gpu') 'G6-3 aggregate evidence status'
        Assert-MaterialRun $builderEvidence 'builder' $true $false | Out-Null
        Assert-MaterialRun $homeEvidence 'home' $false $false | Out-Null
        Assert-MaterialRun $externalBuilderEvidence 'builder' $false $true | Out-Null
        Assert-MaterialRun $externalHomeEvidence 'home' $false $true | Out-Null
        Write-Output 'G6-3 committed evidence/reference gate: PASS'
    }
}
finally {
    dotnet build-server shutdown --msbuild --vbcscompiler | Out-Host
    if (-not $KeepTemporary) { Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot }
}
