#Requires -Version 5.1
param(
    [ValidateSet('All', 'Graph', 'Build', 'Live', 'Evidence')]
    [string] $Shard = 'All'
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$project = Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.csproj'
$tmpRoot = Join-Path $dorotiRoot '.doroti/tmp/app-targets'
$publishRoot = Join-Path $tmpRoot 'windows-publish'
$rawLivePath = Join-Path $tmpRoot 'windows-live.json'
$evidencePath = Join-Path $dorotiRoot 'migration/maui/app-targets-evidence.json'
[IO.Directory]::CreateDirectory($tmpRoot) | Out-Null

function Test-Shard([string] $Name) { return $Shard -eq 'All' -or $Shard -eq $Name }

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

function Invoke-GraphGate {
    foreach ($target in @(
        [ordered]@{ Name='Windows'; Rid='win-x64'; Graph='windows' },
        [ordered]@{ Name='MacCatalyst'; Rid='maccatalyst-arm64'; Graph='maccatalyst' },
        [ordered]@{ Name='Web'; Rid='browser-wasm'; Graph='Web' }
    )) {
        Invoke-Checked {
            dotnet build $project -t:WriteDorotiTargetGraph -p:DorotiTarget=$($target.Name) -p:RuntimeIdentifier=$($target.Rid) --nologo
        } "$($target.Name) target graph failed"
        $graphPath = Join-Path (Split-Path $project -Parent) "obj/$($target.Graph)/doroti-target-graph.txt"
        Assert-True (Test-Path -LiteralPath $graphPath -PathType Leaf) "$($target.Name) target graph output"
        $graph = Get-Content -LiteralPath $graphPath
        Assert-True (@($graph | Where-Object { $_ -ceq 'compile=Program.cs' }).Count -eq 1) "$($target.Name) root bootstrap"
        Assert-True (@($graph | Where-Object { $_ -ceq 'compile=src\App.cs' }).Count -eq 1) "$($target.Name) shared app source"
        if ($target.Name -eq 'Windows') {
            Assert-True (@($graph | Where-Object { $_ -ceq 'applicationDefinition=Platforms\Windows\App.xaml' }).Count -eq 1) 'Windows ApplicationDefinition count'
            Assert-True (@($graph | Where-Object { $_ -like 'compile=Platforms\Web\*' }).Count -eq 0) 'Windows Web source exclusion'
        } elseif ($target.Name -eq 'MacCatalyst') {
            Assert-True (@($graph | Where-Object { $_ -like 'applicationDefinition=?*' }).Count -eq 0) 'Mac Catalyst XAML exclusion'
            Assert-True (@($graph | Where-Object { $_ -like 'compile=Platforms\Windows\*' -or $_ -like 'compile=Platforms\Web\*' }).Count -eq 0) 'Mac Catalyst other-platform source exclusion'
        } else {
            Assert-True (@($graph | Where-Object { $_ -like 'applicationDefinition=?*' }).Count -eq 0) 'Web XAML exclusion'
            Assert-True (@($graph | Where-Object { $_ -like 'compile=Platforms\Maui\*' -or $_ -like 'compile=Platforms\Windows\*' -or $_ -like 'compile=Platforms\MacCatalyst\*' }).Count -eq 0) 'Web desktop source exclusion'
        }
        Assert-True (@($graph | Where-Object { $_ -like 'mauiXaml=?*' }).Count -eq 0) "$($target.Name) MauiXaml count"
    }
}

function Invoke-BuildGate {
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64 --nologo } 'Windows Release build failed'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm --nologo } 'Web Release build failed'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64 --nologo } 'Mac Catalyst cross-build failed'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64 --nologo --no-restore } 'Windows repeat build failed'
}

function Invoke-LiveGate {
    Invoke-Checked {
        dotnet publish $project -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64 -o $publishRoot --nologo
    } 'Windows MAUI publish failed'
    if (Test-Path -LiteralPath $rawLivePath) { [IO.File]::Delete($rawLivePath) }
    $env:DOROTI_MAUI_EVIDENCE = $rawLivePath
    $env:DOROTI_MAUI_AUTO_QUIT_FRAMES = '1'
    $process = Start-Process -FilePath (Join-Path $publishRoot 'DorotiDemoApp.exe') -PassThru -WindowStyle Hidden
    try {
        Assert-True ($process.WaitForExit(60000)) 'Windows MAUI clean auto-exit'
        Assert-True ($process.ExitCode -eq 0) 'Windows MAUI exit code'
    }
    finally {
        if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
    }
    Assert-True (Test-Path -LiteralPath $rawLivePath -PathType Leaf) 'Windows MAUI live evidence'
    $live = Get-Content -LiteralPath $rawLivePath -Raw | ConvertFrom-Json
    Assert-True ([long]$live.Frame.Presented -gt 0 -and [long]$live.Frame.Failed -eq 0) 'Windows MAUI presented frame'
    Assert-True ([long]$live.Frame.Replayed -gt 0) 'Windows MAUI retained scene replay'
    Assert-True ([long]$live.SoftwareFallbackFrames -eq 0) 'Windows MAUI software fallback count'
    Assert-True ([string]$live.Surface.NativeViewType -match 'MauiSKSwapChainPanel') 'Windows MAUI native view type'
    Assert-True ([string]$live.Surface.GraphicsBackend -ceq 'win-x64/winui3/SKSwapChainPanel/ANGLE-DirectX-Skia') 'Windows MAUI backend identity'
}

function Write-Evidence {
    Assert-True (Test-Path -LiteralPath $rawLivePath -PathType Leaf) 'Windows live input for evidence'
    $live = Get-Content -LiteralPath $rawLivePath -Raw | ConvertFrom-Json
    Write-Json $evidencePath ([ordered]@{
        schemaVersion = 'doroti.app-targets-evidence/v1'
        scope = 'single-project-targets'
        capturedAtUtc = [DateTimeOffset]::UtcNow
        status = 'partial'
        project = 'DorotiDemoApp/DorotiDemoApp.csproj'
        graph = [ordered]@{
            status = 'pass'
            projectCount = 1
            applicationSource = 'DorotiDemoApp/src/App.cs'
            bootstrapSource = 'DorotiDemoApp/Program.cs'
            windowsApplicationDefinitionCount = 1
            otherXamlCount = 0
            selectedPlatformLeakage = 0
        }
        build = [ordered]@{
            status = 'pass'
            sequence = @('Windows','Web','MacCatalyst-cross-build','Windows-no-restore')
            windows = [ordered]@{ targetFramework='net10.0-windows10.0.19041.0';rid='win-x64' }
            web = [ordered]@{ targetFramework='net10.0';rid='browser-wasm' }
            macCatalyst = [ordered]@{ targetFramework='net10.0-maccatalyst';rid='maccatalyst-arm64';host='windows-cross-build-only' }
        }
        windowsLive = $live
        boundaries = [ordered]@{
            basicTouch = 'implemented-not-live-verified'
            hoverWheelCaptureKeyboardImeUia = 'notVerified'
            resizeDpiContextRecreate = 'notVerified-product-host'
            macCatalystBuild = 'pass-windows-cross-build-only'
            macCatalystPublishLive = 'notVerified'
            physical = 'notVerified'
            crossTarget = 'notVerified'
        }
    })
}

if (Test-Shard 'Graph') { Invoke-GraphGate }
if (Test-Shard 'Build') { Invoke-BuildGate }
if (Test-Shard 'Live') { Invoke-LiveGate }
if (Test-Shard 'Evidence') { Write-Evidence }
Write-Output "Doroti application target shard '$Shard': PASS"
