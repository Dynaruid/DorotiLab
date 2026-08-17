#Requires -Version 5.1
param(
    [ValidateSet('All', 'Graph', 'Build', 'Live', 'Evidence')]
    [string] $Shard = 'All'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$project = Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.csproj'
$productSolution = Join-Path $dorotiRoot 'Doroti.Product.slnx'
$templateRoot = Join-Path $dorotiRoot 'templates/Doroti.Templates/content/doroti-app'
$descriptorContract = Join-Path $dorotiRoot 'validation/app-bootstrap/descriptor-contract/DescriptorContract.csproj'
$syntheticProject = Join-Path $dorotiRoot 'validation/app-bootstrap/synthetic-fourth-host/SyntheticFourthHost.csproj'
$invalidRegistrationProject = Join-Path $dorotiRoot 'validation/app-bootstrap/invalid-required-registration/InvalidRequiredRegistration.csproj'
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

function Invoke-AppRestore([string] $Target, [string] $Rid) {
    if (-not $script:productRestoreComplete) {
        Invoke-Checked { dotnet restore $productSolution --nologo } 'Doroti product dependency restore failed'
        $script:productRestoreComplete = $true
    }
    Invoke-Checked {
        dotnet restore $project --no-dependencies -p:DorotiTarget=$Target -p:RuntimeIdentifier=$Rid --nologo
    } "$Target application restore failed"
}

function Invoke-GraphGate {
    $program = Get-Content -LiteralPath (Join-Path (Split-Path $project -Parent) 'Program.cs') -Raw
    Assert-True ($program -match 'public sealed class Program : IDorotiApplicationStartup') 'public target-neutral startup type'
    Assert-True ($program -notmatch '#if|DOROTI_BROWSER|MACCATALYST|Maui|Blazor|Qt') 'Program target neutrality'
    foreach ($root in @((Split-Path $project -Parent), $templateRoot)) {
        Assert-True (-not (Test-Path -LiteralPath (Join-Path $root 'Platforms/Maui'))) "$root legacy Platforms/Maui absence"
        Assert-True (@(Get-ChildItem -LiteralPath (Join-Path $root 'Platforms') -Filter 'PlatformBootstrap.cs' -File -Recurse).Count -eq 0) "$root legacy PlatformBootstrap absence"
    }
    $sourceRoots = @(
        (Join-Path $dorotiRoot 'src/Doroti.Hosting'),
        (Join-Path $dorotiRoot 'src/Doroti.Host.Maui'),
        (Join-Path $dorotiRoot 'src/Doroti.Host.Web'),
        (Split-Path $project -Parent),
        $templateRoot
    )
    $reflectionBootstrap = @($sourceRoots | Get-ChildItem -File -Recurse -Include *.cs | Select-String -Pattern 'GetType\("(?:DorotiApp\.)?App"|Type\.GetType\(')
    Assert-True ($reflectionBootstrap.Count -eq 0) 'reflection/string startup lookup absence'
    Invoke-Checked { dotnet run --project $descriptorContract -c Release --nologo } 'application descriptor contract failed'

    foreach ($target in @(
        [ordered]@{ Name='Windows'; Rid='win-x64'; Graph='windows'; Host='Maui'; Entry='WinUI-Xaml' },
        [ordered]@{ Name='MacCatalyst'; Rid='maccatalyst-arm64'; Graph='maccatalyst'; Host='Maui'; Entry='UIKit-Main' },
        [ordered]@{ Name='Web'; Rid='browser-wasm'; Graph='Web'; Host='BlazorWebAssembly'; Entry='Managed-Main' }
    )) {
        Invoke-Checked {
            dotnet msbuild $project -t:WriteDorotiTargetGraph -p:DorotiTarget=$($target.Name) -p:RuntimeIdentifier=$($target.Rid) -nologo
        } "$($target.Name) target graph failed"
        $graphPath = Join-Path (Split-Path $project -Parent) "obj/$($target.Graph)/doroti-target-graph.txt"
        Assert-True (Test-Path -LiteralPath $graphPath -PathType Leaf) "$($target.Name) target graph output"
        $graph = Get-Content -LiteralPath $graphPath
        Assert-True (@($graph | Where-Object { $_ -ceq 'compile=Program.cs' }).Count -eq 1) "$($target.Name) root bootstrap"
        Assert-True (@($graph | Where-Object { $_ -ceq 'compile=src\App.cs' }).Count -eq 1) "$($target.Name) shared app source"
        Assert-True (@($graph | Where-Object { $_ -ceq "startup=DorotiDemoApp.Program" }).Count -eq 1) "$($target.Name) startup identity"
        Assert-True (@($graph | Where-Object { $_ -like "descriptor=$($target.Name)|$($target.Host)|$($target.Entry)|*" }).Count -eq 1) "$($target.Name) descriptor identity"
        Assert-True (@($graph | Where-Object { $_ -like 'compile=*Doroti.Generated\DorotiBootstrap.g.cs' }).Count -eq 1) "$($target.Name) generated bootstrap count"
        Assert-True (@($graph | Where-Object { $_ -like 'compile=*Doroti.Generated\DorotiPluginRegistration.g.cs' }).Count -eq 1) "$($target.Name) generated plugin registration count"
        if ($target.Name -eq 'Windows') {
            Assert-True (@($graph | Where-Object { $_ -ceq 'applicationDefinition=Platforms\Windows\App.xaml' }).Count -eq 1) 'Windows ApplicationDefinition count'
            Assert-True (@($graph | Where-Object { $_ -like 'compile=Platforms\Web\*' }).Count -eq 0) 'Windows Web source exclusion'
        } elseif ($target.Name -eq 'MacCatalyst') {
            Assert-True (@($graph | Where-Object { $_ -like 'applicationDefinition=?*' }).Count -eq 0) 'Mac Catalyst XAML exclusion'
            Assert-True (@($graph | Where-Object { $_ -like 'compile=Platforms\Windows\*' -or $_ -like 'compile=Platforms\Web\*' }).Count -eq 0) 'Mac Catalyst other-platform source exclusion'
        } else {
            Assert-True (@($graph | Where-Object { $_ -like 'applicationDefinition=?*' }).Count -eq 0) 'Web XAML exclusion'
            Assert-True (@($graph | Where-Object { $_ -like 'compile=Platforms\Maui\*' -or $_ -like 'compile=Platforms\Windows\*' -or $_ -like 'compile=Platforms\MacCatalyst\*' }).Count -eq 0) 'Web desktop source exclusion'
            Assert-True (@($graph | Where-Object { $_ -ceq 'typescriptVersion=7.0.0' }).Count -eq 1) 'Web TypeScript compiler version'
            Assert-True (@($graph | Where-Object { $_ -ceq 'typescriptSourceCount=2' }).Count -eq 1) 'Web TypeScript source count'
            Assert-True (@($graph | Where-Object { $_ -like 'typescriptOutputRoot=*obj\web\*\net10.0\Doroti.Generated\wwwroot' }).Count -eq 1) 'Web TypeScript isolated output root'
        }
        if ($target.Name -ne 'Web') {
            Assert-True (@($graph | Where-Object { $_ -like 'typescript*=*' }).Count -eq 0) "$($target.Name) TypeScript graph exclusion"
        }
        Assert-True (@($graph | Where-Object { $_ -like 'mauiXaml=?*' }).Count -eq 0) "$($target.Name) MauiXaml count"
    }

    Invoke-Checked { dotnet msbuild $syntheticProject -t:WriteDorotiTargetGraph -nologo } 'synthetic fourth host descriptor failed'
    $syntheticGraph = Get-Content -LiteralPath (Join-Path (Split-Path $syntheticProject -Parent) 'obj/linux/doroti-target-graph.txt')
    Assert-True (@($syntheticGraph | Where-Object { $_ -ceq 'descriptor=Linux|SyntheticQt|Synthetic|Doroti.Target.Linux.Qt.linux-x64' }).Count -eq 1) 'synthetic fourth host descriptor identity'
}

function Invoke-BuildGate {
    Invoke-AppRestore 'Windows' 'win-x64'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64 --nologo --no-restore } 'Windows Release build failed'
    $windowsGeneratedFiles = @(
        (Join-Path (Split-Path $project -Parent) 'obj/windows/Doroti.Generated/DorotiBootstrap.g.cs'),
        (Join-Path (Split-Path $project -Parent) 'obj/windows/Doroti.Generated/DorotiPluginRegistration.g.cs')
    )
    $windowsAssembly = Join-Path (Split-Path $project -Parent) 'bin/windows/Release/net10.0-windows10.0.19041.0/win-x64/DorotiDemoApp.dll'
    foreach ($path in @($windowsGeneratedFiles + $windowsAssembly)) {
        Assert-True (Test-Path -LiteralPath $path -PathType Leaf) "Windows incremental build input $path"
    }
    $windowsWriteTimes = @{}
    foreach ($path in @($windowsGeneratedFiles + $windowsAssembly)) {
        $windowsWriteTimes[$path] = [IO.File]::GetLastWriteTimeUtc($path)
    }
    Invoke-AppRestore 'Web' 'browser-wasm'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm --nologo --no-restore } 'Web Release build failed'
    Invoke-AppRestore 'MacCatalyst' 'maccatalyst-arm64'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64 --nologo --no-restore } 'Mac Catalyst cross-build failed'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64 --nologo --no-restore } 'Windows repeat build failed'
    foreach ($nativeTarget in @('windows','maccatalyst')) {
        $nativeAssets = Get-Content -LiteralPath (Join-Path (Split-Path $project -Parent) "obj/$nativeTarget/project.assets.json") -Raw
        Assert-True ($nativeAssets -notmatch 'Microsoft\.TypeScript\.MSBuild') "$nativeTarget TypeScript package isolation"
    }
    $webAssets = Get-Content -LiteralPath (Join-Path (Split-Path $project -Parent) 'obj/web/project.assets.json') -Raw
    Assert-True ($webAssets -match 'Microsoft\.TypeScript\.MSBuild/7\.0\.0') 'Web TypeScript package graph'
    foreach ($path in @($windowsGeneratedFiles + $windowsAssembly)) {
        Assert-True ([IO.File]::GetLastWriteTimeUtc($path) -eq $windowsWriteTimes[$path]) "Windows repeat build preserved $path"
    }
    $invalidStartup = @(& dotnet build $syntheticProject -c Release -p:DorotiApplicationType=System.String --nologo 2>&1)
    Assert-True ($LASTEXITCODE -ne 0 -and (($invalidStartup -join "`n") -match 'CS0311')) 'startup interface diagnostic fail-closed'
    $invalidRegistration = @(& dotnet build $invalidRegistrationProject -c Release --nologo 2>&1)
    Assert-True ($LASTEXITCODE -ne 0 -and (($invalidRegistration -join "`n") -match 'CS0239')) 'mandatory registration override fail-closed'
    $global:LASTEXITCODE = 0
}

function Invoke-LiveGate {
    Invoke-AppRestore 'Windows' 'win-x64'
    Invoke-Checked {
        dotnet publish $project -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64 -o $publishRoot --nologo --no-restore
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
        schemaVersion = 'doroti.app-targets-evidence/v3'
        scope = 'generated-application-bootstrap'
        capturedAtUtc = [DateTimeOffset]::UtcNow
        status = 'partial'
        project = 'DorotiDemoApp/DorotiDemoApp.csproj'
        graph = [ordered]@{
            status = 'pass'
            projectCount = 1
            applicationSource = 'DorotiDemoApp/src/App.cs'
            bootstrapSource = 'DorotiDemoApp/Program.cs'
            generatedBootstrap = 'obj/<target>/Doroti.Generated/DorotiBootstrap.g.cs'
            generatedPluginRegistration = 'obj/<target>/Doroti.Generated/DorotiPluginRegistration.g.cs'
            startupType = 'DorotiDemoApp.Program'
            targetDescriptorCount = 1
            syntheticFourthHost = 'pass-graph-only'
            reflectionStartupLookupCount = 0
            legacyPlatformBootstrapCount = 0
            legacyPlatformsMauiCount = 0
            windowsApplicationDefinitionCount = 1
            otherXamlCount = 0
            selectedPlatformLeakage = 0
        }
        build = [ordered]@{
            status = 'pass'
            sequence = @('Windows','Web','MacCatalyst-cross-build','Windows-no-restore')
            windows = [ordered]@{ targetFramework='net10.0-windows10.0.19041.0';rid='win-x64' }
            web = [ordered]@{ targetFramework='net10.0';rid='browser-wasm';typeScriptMsBuild='7.0.0';sourceCount=2;outputRoot='obj/web/<configuration>/net10.0/Doroti.Generated/wwwroot' }
            macCatalyst = [ordered]@{ targetFramework='net10.0-maccatalyst';rid='maccatalyst-arm64';host='windows-cross-build-only' }
            startupNegative = 'pass-failed-closed-CS0311'
            requiredRegistrationNegative = 'pass-failed-closed-CS0239'
            nativeTypeScriptPackageCount = 0
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
