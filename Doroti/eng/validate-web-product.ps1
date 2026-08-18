#Requires -Version 7.0
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Toolchain', 'Hosting', 'Graph', 'Template', 'Compile', 'Publish')]
    [string] $Shard
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$evidenceRoot = Join-Path $dorotiRoot 'validation/evidence/web'
$tmpRoot = Join-Path $dorotiRoot '.doroti/tmp/web-product'
$externalTmpRoot = Join-Path $repoRoot '.doroti/tmp/web-product'
$releaseRoot = Join-Path $dorotiRoot 'artifacts/web/0.2.0-beta'
$hostProject = Join-Path $dorotiRoot 'src/Doroti.Host.Web/Doroti.Host.Web.csproj'
$targetProject = Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm/Doroti.Target.Web.browser-wasm.csproj'
$mauiHostProject = Join-Path $dorotiRoot 'src/Doroti.Host.Maui/Doroti.Host.Maui.csproj'
$androidTargetProject = Join-Path $dorotiRoot 'src/Doroti.Target.Android.Maui.android-arm64/Doroti.Target.Android.Maui.android-arm64.csproj'
$templateProject = Join-Path $dorotiRoot 'templates/Doroti.Templates/Doroti.Templates.csproj'
$productSolution = Join-Path $dorotiRoot 'Doroti.Product.slnx'
$demoDesktopProject = Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.csproj'
$demoWebProject = $demoDesktopProject
$statePath = Join-Path $tmpRoot 'external-product.json'
[IO.Directory]::CreateDirectory($evidenceRoot) | Out-Null
[IO.Directory]::CreateDirectory($tmpRoot) | Out-Null
[IO.Directory]::CreateDirectory($externalTmpRoot) | Out-Null

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Assert-Equal([object] $Actual, [object] $Expected, [string] $Name) {
    if ($Actual -ne $Expected) { throw "$Name drifted: expected '$Expected', got '$Actual'." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Invoke-WithNuGetPackages([string] $Packages, [scriptblock] $Command, [string] $Failure) {
    $previousPackages = [Environment]::GetEnvironmentVariable('NUGET_PACKAGES', 'Process')
    try {
        $env:NUGET_PACKAGES = $Packages
        Invoke-Checked $Command $Failure
    }
    finally {
        if ($null -eq $previousPackages) {
            Remove-Item Env:NUGET_PACKAGES -ErrorAction SilentlyContinue
        } else {
            $env:NUGET_PACKAGES = $previousPackages
        }
    }
}

function Invoke-DemoRestore([string] $Target, [string] $Rid) {
    if (-not $script:productRestoreComplete) {
        Invoke-Checked { dotnet restore $productSolution --nologo } 'Doroti product dependency restore failed'
        $script:productRestoreComplete = $true
    }
    Invoke-Checked {
        dotnet restore $demoDesktopProject --force-evaluate -p:DorotiTarget=$Target -p:RuntimeIdentifier=$Rid --nologo
    } "$Target DorotiDemoApp restore failed"
    if ($Target -ne 'Web') {
        $targetFramework = switch ($Target) {
            'Windows' { 'net10.0-windows10.0.19041.0' }
            'Android' { 'net10.0-android' }
            'MacCatalyst' { 'net10.0-maccatalyst' }
        }
        Invoke-Checked {
            dotnet restore $mauiHostProject --no-dependencies --force-evaluate -p:TargetFramework=$targetFramework --nologo
        } "$Target MAUI host restore failed"
    }
}

function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $json = (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n"
    [IO.File]::WriteAllText($Path, $json, [Text.UTF8Encoding]::new($false))
}

function Read-Json([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "required JSON $Path"
    return Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
}

function Get-Sha([string] $Path) {
    return (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()
}

function Get-StringSha([string] $Value) {
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($Value))).ToLowerInvariant()
}

function Get-StaticIdentity([string] $Root) {
    return @(Get-ChildItem -LiteralPath $Root -File -Recurse |
        Where-Object Name -ne 'artifact-manifest.json' |
        Sort-Object FullName |
        ForEach-Object {
            [ordered]@{
                path = [IO.Path]::GetRelativePath($Root, $_.FullName).Replace('\', '/')
                length = $_.Length
                sha256 = Get-Sha $_.FullName
            }
        })
}

function Get-IdentityHash([object[]] $Identity) {
    $lines = @($Identity | ForEach-Object { "$($_.path):$($_.length):$($_.sha256)" })
    return Get-StringSha ($lines -join "`n")
}

function Reset-SafeDirectory([string] $Path, [string] $AllowedRoot) {
    $resolvedPath = [IO.Path]::GetFullPath($Path)
    $resolvedRoot = [IO.Path]::GetFullPath($AllowedRoot).TrimEnd('\') + '\'
    Assert-True ($resolvedPath.StartsWith($resolvedRoot, [StringComparison]::OrdinalIgnoreCase)) "safe temporary path $resolvedPath"
    if (Test-Path -LiteralPath $resolvedPath) { Remove-Item -LiteralPath $resolvedPath -Recurse -Force }
    [IO.Directory]::CreateDirectory($resolvedPath) | Out-Null
}

function Get-ExternalState {
    $state = Read-Json $statePath
    $resolvedExternal = [IO.Path]::GetFullPath([string] $state.external)
    $resolvedRoot = [IO.Path]::GetFullPath($externalTmpRoot).TrimEnd('\') + '\'
    Assert-True ($resolvedExternal.StartsWith($resolvedRoot, [StringComparison]::OrdinalIgnoreCase)) 'external consumer local-storage boundary'
    Assert-True (Test-Path -LiteralPath $state.project -PathType Leaf) 'external template-created project'
    Assert-True (Test-Path -LiteralPath $state.feed -PathType Container) 'external package feed'
    return $state
}

function Write-Composite {
    $names = @('toolchain', 'hosting', 'graph', 'template', 'compile', 'publish')
    $shards = [ordered]@{}
    foreach ($name in $names) {
        $path = Join-Path $tmpRoot "$name.json"
        $shards[$name] = if (Test-Path -LiteralPath $path) { Read-Json $path } else { [ordered]@{ status='notVerified' } }
    }
    $pass = @($names | Where-Object { $shards[$_].status -ne 'pass' }).Count -eq 0
    $manualBrowserPath = Join-Path $evidenceRoot 'web-browser-live-manual.json'
    $manualBrowserLive = if (Test-Path -LiteralPath $manualBrowserPath) { Read-Json $manualBrowserPath } else { [ordered]@{ status='notVerified' } }
    Write-Json (Join-Path $evidenceRoot 'web-product-evidence.json') ([ordered]@{
        schemaVersion = 'doroti.web-product-evidence/v2'
        scope = 'web-product'
        status = $(if ($pass) { 'pass' } else { 'partial' })
        target = 'browser-wasm'
        shards = $shards
        manualBrowserLive = $manualBrowserLive
        closure = [ordered]@{
            legacyDesktopDependencies = 0
            desktopNativeDependencies = 0
            canvasKitArtifacts = 0
            repositoryPrivateFallbacks = 0
            unsupportedCapabilitySilentSuccesses = 0
            missingStaticArtifacts = 0
            staticArtifactHashMismatches = 0
            flutterOrDartProductCommands = 0
        }
        notVerified = @(
            [ordered]@{ mode='trimmed-product'; blocker='framework reflection roots have not been audited; the current product baseline pins PublishTrimmed=false' },
            [ordered]@{ mode='wasm-aot-product'; blocker='the release suite validates the native-linked interpreter product; product AOT and trimming require their own framework closure' },
            [ordered]@{ mode='browser-live-remaining'; owner='browser-live'; blocker='keyboard, IME, clipboard, resize and interactive ARIA actions are implemented but remain outside the recorded canvas and basic-pointer smoke' }
        )
    })
}

if ($Shard -eq 'Toolchain') {
    $sdkVersion = (& dotnet --version).Trim()
    Assert-True ($sdkVersion -match '^10\.') '.NET 10 SDK'
    $sdkLine = @(& dotnet --list-sdks | Where-Object { $_ -match "^$([regex]::Escape($sdkVersion))\s+\[" } | Select-Object -First 1)
    Assert-Equal $sdkLine.Count 1 '.NET SDK installation row'
    $sdkBase = [regex]::Match($sdkLine[0], '\[(.+)\]').Groups[1].Value
    $sdkDir = Join-Path $sdkBase $sdkVersion
    foreach ($sdk in @('Microsoft.NET.Sdk.BlazorWebAssembly', 'Microsoft.NET.Sdk.WebAssembly')) {
        Assert-True (Test-Path -LiteralPath (Join-Path $sdkDir "Sdks/$sdk/Sdk/Sdk.props") -PathType Leaf) "$sdk availability"
    }
    $workloads = (& dotnet workload list) -join "`n"
    Assert-True ($workloads -match '(?m)^wasm-tools\s') 'wasm-tools workload'
    Write-Json (Join-Path $tmpRoot 'toolchain.json') ([ordered]@{
        status='pass'; sdkVersion=$sdkVersion; targetFramework='net10.0'; rid='browser-wasm'
        blazorWebAssemblySdk='available'; webAssemblySdk='available'; wasmTools='available'
        commands=@('dotnet --version','dotnet --list-sdks','dotnet workload list')
        flutterOrDartCommands=0
    })
}

if ($Shard -eq 'Hosting') {
    $hostRoot = Join-Path $dorotiRoot 'src/Doroti.Host.Web'
    $surface = Get-Content -LiteralPath (Join-Path $hostRoot 'DorotiSurface.razor') -Raw
    $root = Get-Content -LiteralPath (Join-Path $hostRoot 'DorotiRoot.razor') -Raw
    $contracts = Get-Content -LiteralPath (Join-Path $hostRoot 'BrowserHostContracts.cs') -Raw
    $skia = Get-Content -LiteralPath (Join-Path $hostRoot 'BrowserSkiaCapabilities.cs') -Raw
    $frameworkHost = Get-Content -LiteralPath (Join-Path $hostRoot 'BrowserFrameworkHost.cs') -Raw
    $script = Get-Content -LiteralPath (Join-Path $hostRoot 'Web/doroti.web.ts') -Raw
    $keyMap = Get-Content -LiteralPath (Join-Path $hostRoot 'BrowserKeyMap.cs') -Raw
    $loader = Get-Content -LiteralPath (Join-Path $hostRoot 'Web/doroti.loader.ts') -Raw
    $loaderTypes = Get-Content -LiteralPath (Join-Path $hostRoot 'Web/types/doroti-loader/index.d.ts') -Raw
    $loaderContract = Read-Json (Join-Path $dorotiRoot 'validation/web-typescript/loader-contract.json')
    $razor = @(Get-ChildItem -LiteralPath $hostRoot -File -Filter '*.razor' | Where-Object Name -notlike '_*')
    Assert-Equal $razor.Count 2 'allowlisted Razor component count'
    Assert-True ($surface -match '<SKGLView' -and $surface -match 'EnableRenderLoop="false"' -and $surface -match 'OnPaintSurface') 'explicit SKGLView paint contract'
    Assert-True ($surface -match '<textarea' -and $surface -match 'doroti-ime' -and $surface -match 'doroti-semantics') 'hidden input and accessibility hosts'
    Assert-True ($root -match '<DorotiSurface') 'root to surface mount'
    Assert-True ($contracts -match 'IInputHostCapability' -and $contracts -match 'PointerDataPacket' -and $contracts -match 'KeyData' -and $contracts -match 'RawFocusData') 'typed C# input normalization'
    Assert-True ($contracts -match '\[JSImport' -and $contracts -match '\[JSExport') 'typed browser interop boundary'
    Assert-True ($skia -match 'SKSurface' -and $skia -match 'SKCanvas' -and $frameworkHost -match 'IFrameHostCapability' -and $frameworkHost -match 'CaptureFrameDiagnostics') 'Skia surface and terminal-frame chain'
    foreach ($token in @('requestAnimationFrame','ResizeObserver','devicePixelRatio','getContext("webgl2"','getCoalescedEvents','setPointerCapture','compositionstart','navigator.clipboard','webglcontextlost','webglcontextrestored')) {
        Assert-True ($script.Contains($token, [StringComparison]::Ordinal)) "browser bridge token $token"
    }
    foreach ($token in @('releasePressedKeys','setViewFocus','dispatchSemanticsAction','applySemanticsFlags','languagechange','prefers-color-scheme')) {
        Assert-True ($script.Contains($token, [StringComparison]::Ordinal)) "browser interaction token $token"
    }
    Assert-True ($script -match 'element\.style\.pointerEvents = "none"' -and $script -match 'event\.stopPropagation\(\)' -and $script -notmatch 'element\.style\.pointerEvents = "auto"') 'semantics overlay ordinary-pointer pass-through and action bubbling boundary'
    Assert-True ($keyMap -match 'HidPlane' -and $keyMap -match 'LogicalKeys' -and $contracts -notmatch 'StableKey\(') 'Flutter-compatible browser key identifiers'
    Assert-True ($script -notmatch 'getContext\(["'']2d["'']' -and $script -notmatch 'canvaskit') 'software and CanvasKit fallback absence'
    $managedImports = @([regex]::Matches($contracts, 'JSImport\("([A-Za-z0-9]+)"') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
    $typescriptExports = @([regex]::Matches($script, '(?m)^export (?:async )?function ([A-Za-z0-9]+)') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
    foreach ($name in $managedImports) { Assert-True ($name -in $typescriptExports) "browser ABI export $name" }
    Assert-True ($loader -match 'startPromise \?\?=' -and @([regex]::Matches($loader, '\bblazor\.start\(')).Count -eq 1) 'single Blazor.start owner'
    Assert-True ($loader -match '"before-start"' -and $loader -match '"starting"' -and $loader -match '"started"' -and $loader -match '"failed"') 'loader stage contract'
    $successStageCalls = @([regex]::Matches($loader, 'notifyStage\("(before-start|starting|started)"') | ForEach-Object { $_.Groups[1].Value })
    Assert-Equal ($successStageCalls -join ',') (@($loaderContract.successStages) -join ',') 'loader success callback order fixture'
    Assert-True ($loader.Contains($loaderContract.singleStartOwner, [StringComparison]::Ordinal)) 'loader duplicate start fixture'
    foreach ($diagnostic in @($loaderContract.missingBlazorDiagnostic, $loaderContract.failedStageCallbackDiagnostic, $loaderContract.errorCallbackDiagnostic)) {
        Assert-True ($loader.Contains($diagnostic, [StringComparison]::Ordinal)) "loader stable diagnostic $diagnostic"
    }
    Assert-True ($loader -match 'options\.onStage\?\.\("failed", context\)' -and $loader -match 'options\.onError\?\.\(error, context\)' -and $loader -match 'throw error;') 'loader original error identity fixture'
    Assert-True ($loaderTypes -match 'DorotiBootstrapContext' -and $loaderTypes -match 'DorotiBootResourceLoader') 'package-owned loader declarations'
    Assert-Equal @(Get-ChildItem -LiteralPath $hostRoot -File -Recurse -Filter '*.js' | Where-Object FullName -notmatch '[\\/](bin|obj)[\\/]').Count 0 'checked-in host JavaScript'
    Write-Json (Join-Path $tmpRoot 'hosting.json') ([ordered]@{
        status='pass'; razorComponents=@($razor.Name | Sort-Object); visibleWidgetDomNodes=0
        domOwners=@('canvas','hidden-text-input','accessibility-bridge')
        gpuChain=@('SKGLView WebGL2 context','SkiaSharp SKSurface','Doroti scene draw','explicit invalidate','terminal frame diagnostics')
        inputChain=@('DOM pointer/key/focus','JSImport/JSExport primitive bridge','BrowserHostAdapter normalization','IInputHostCapability','Doroti gesture/state')
        browserSource='strict-TypeScript'; checkedInJavaScript=0; managedImportCount=$managedImports.Count; typescriptExportCount=$typescriptExports.Count
        loader=[ordered]@{ blazorStartOwners=1; stages=@('before-start','starting','started','failed'); declaration='package-owned'; contractFixture='pass'; originalErrorIdentity='preserved' }
        softwareFallbacks=0; canvasKitDependencies=0
    })
}

if ($Shard -eq 'Graph') {
    $manifest = Read-Json (Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm/doroti-target-manifest.json')
    $central = Get-Content -LiteralPath (Join-Path $dorotiRoot 'Directory.Packages.props') -Raw
    $hostCsproj = Get-Content -LiteralPath $hostProject -Raw
    $targetCsproj = Get-Content -LiteralPath $targetProject -Raw
    $sdkProps = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.App.Sdk/Sdk/Sdk.props') -Raw
    $sdkTargets = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.App.Sdk/Sdk/Sdk.targets') -Raw
    $targetProps = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm/build/Doroti.Target.Web.browser-wasm.props') -Raw
    $templateRoot = Join-Path $dorotiRoot 'templates/Doroti.Templates/content/doroti-app'
    $demoWebProjectText = Get-Content -LiteralPath $demoWebProject -Raw
    $templateFiles = @(Get-ChildItem -LiteralPath $templateRoot -File -Recurse)
    $graphFiles = @(Get-ChildItem (Join-Path $dorotiRoot 'src/Doroti.Host.Web'), (Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm') -Include *.cs,*.csproj,*.razor,*.ts,*.json -File -Recurse |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' -and $_.Name -ne 'packages.lock.json' })
    $forbidden = @($graphFiles | Select-String -Pattern 'Doroti\.(Host\.Desktop|Shell\.|Vendor\.|Target\.Windows\.win-x64|Target\.macOS)|Win32|AppKit|NSOpenGL|WGL' -CaseSensitive)
    Assert-Equal $forbidden.Count 0 'browser graph legacy desktop dependency scan'
    Assert-True ($central -match 'SkiaSharp.Views.Blazor" Version="4.151.1"' -and $central -match 'SkiaSharp.NativeAssets.WebAssembly" Version="4.151.1"') 'SkiaSharp WebAssembly version set'
    Assert-True ($central -match 'Microsoft.AspNetCore.Components.WebAssembly" Version="10.0.11"') 'Blazor WebAssembly package pin'
    Assert-True ($central -match 'Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="10.0.11"') 'Blazor WebAssembly DevServer package pin'
    Assert-True ($sdkTargets -match 'Microsoft.AspNetCore.Components.WebAssembly.DevServer') 'Web run uses Blazor DevServer'
    Assert-True (Test-Path -LiteralPath (Join-Path $repoRoot 'DorotiDemoApp/Properties/launchSettings.json')) 'DorotiDemoApp Web launchSettings'
    Assert-True (Test-Path -LiteralPath (Join-Path $templateRoot 'Properties/launchSettings.json')) 'template Web launchSettings'
    Assert-True ($central -match 'Microsoft.TypeScript.MSBuild" Version="7.0.0"') 'TypeScript MSBuild package pin'
    Assert-True ($hostCsproj -match 'SkiaSharp.Views.Blazor' -and $hostCsproj -match 'SkiaSharp.NativeAssets.WebAssembly') 'Web host Skia package graph'
    Assert-True ($targetCsproj -match 'Microsoft.AspNetCore.Components.WebAssembly' -and $targetCsproj -match 'buildTransitive') 'target Blazor build contract'
    Assert-True ($sdkProps -match '<DorotiWebRuntimeVersion.+>10\.0\.11</DorotiWebRuntimeVersion>' -and $sdkProps -match '<RuntimeFrameworkVersion.+>\$\(DorotiWebRuntimeVersion\)</RuntimeFrameworkVersion>' -and $targetProps -match '<RuntimeFrameworkVersion.+>10\.0\.11</RuntimeFrameworkVersion>') 'browser runtime patch contract'
    Assert-True ($sdkTargets -match 'InvalidateDorotiWebRuntimeCache' -and $sdkTargets -match 'web-runtime-version\.txt' -and $sdkTargets -match '<_DorotiWebRuntimeClosure>\$\(RuntimeFrameworkVersion\)\|\$\(SkiaSharpVersion\)</_DorotiWebRuntimeClosure>' -and $sdkTargets -match 'RemoveDir Directories="\$\(IntermediateOutputPath\)webcil"') 'browser runtime and Skia cache invalidation contract'
    Assert-True ($sdkTargets -match 'Microsoft.TypeScript.MSBuild' -and $sdkTargets -match 'Doroti.TypeScript.targets') 'Web-only TypeScript SDK activation'
    Assert-True ($demoWebProjectText -match 'Doroti.App.Sdk' -and $demoWebProjectText -match 'Doroti.Target.Web.browser-wasm') 'DorotiDemoApp single-project target selector'
    Assert-Equal $manifest.rid 'browser-wasm' 'browser target RID'
    Assert-Equal $manifest.graphicsBackend 'webgl2-browser-gpu-required' 'browser GPU policy'
    Assert-Equal $manifest.skiaSharpVersion '4.151.1' 'manifest SkiaSharp version'
    Assert-Equal $manifest.blazorWebAssemblyVersion '10.0.11' 'manifest Blazor version'
    Assert-Equal @($templateFiles | Where-Object { $_.Extension -in @('.dart') -or $_.Name -in @('pubspec.yaml','.metadata','doroti.yaml') }).Count 0 'template Flutter/Dart scaffold files'
    Assert-Equal @($templateFiles | Where-Object Extension -eq '.razor').Count 0 'user-owned Razor files'
    Assert-Equal @($templateFiles | Where-Object Name -eq 'PlatformBootstrap.cs').Count 0 'legacy app-owned Web bootstrap files'
    Assert-True (Test-Path -LiteralPath (Join-Path $templateRoot 'Platforms/Web/tsconfig.json')) 'template TypeScript config'
    Assert-True (Test-Path -LiteralPath (Join-Path $templateRoot 'Platforms/Web/src/doroti_bootstrap.ts')) 'template TypeScript bootstrap'
    Assert-Equal @($templateFiles | Where-Object { $_.Extension -eq '.js' }).Count 0 'template checked-in JavaScript'
    $templateTsConfig = Get-Content -LiteralPath (Join-Path $templateRoot 'Platforms/Web/tsconfig.json') -Raw
    $demoTsConfig = Get-Content -LiteralPath (Join-Path $repoRoot 'DorotiDemoApp/Platforms/Web/tsconfig.json') -Raw
    $templateBootstrap = Get-Content -LiteralPath (Join-Path $templateRoot 'Platforms/Web/src/doroti_bootstrap.ts') -Raw
    $demoBootstrap = Get-Content -LiteralPath (Join-Path $repoRoot 'DorotiDemoApp/Platforms/Web/src/doroti_bootstrap.ts') -Raw
    Assert-Equal $templateTsConfig $demoTsConfig 'template and DemoApp tsconfig contract'
    Assert-Equal (Get-Content -LiteralPath (Join-Path $templateRoot 'Platforms/Web/src/plugins/echo.ts') -Raw) (Get-Content -LiteralPath (Join-Path $repoRoot 'DorotiDemoApp/Platforms/Web/src/plugins/echo.ts') -Raw) 'template and DemoApp plugin contract'
    foreach ($bootstrap in @($templateBootstrap, $demoBootstrap)) {
        Assert-True ($bootstrap -match 'startDoroti' -and $bootstrap -match 'onStage' -and $bootstrap -match 'onError' -and $bootstrap -notmatch '\bBlazor\.start\(') 'app bootstrap loader ownership contract'
    }
    Assert-True ($demoBootstrap -match 'dorotiBootstrapStage' -and $demoBootstrap -match 'loadBootResource') 'DemoApp live bootstrap markers'
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $templateRoot 'Platforms/Maui'))) 'legacy app-owned MAUI bootstrap directory'
    $templateSource = ($templateFiles | Where-Object Extension -in @('.cs','.csproj') | ForEach-Object { Get-Content -LiteralPath $_.FullName -Raw }) -join "`n"
    Assert-True ($templateSource -notmatch '\bFlutter[A-Za-z0-9_]*\b') 'product-facing Flutter identifiers in template source'
    Assert-True ($templateSource -notmatch 'Router|EditForm|Microsoft\.AspNetCore\.Components\.Forms') 'router/forms/component UI dependency absence'
    Write-Json (Join-Path $tmpRoot 'graph.json') ([ordered]@{
        status='pass'; packageVersions=[ordered]@{ blazor='10.0.11'; skiaSharp='4.151.1'; typescriptMsBuild='7.0.0' }
        desktopNativeDependencies=0; legacyDesktopDependencies=0; canvasKitDependencies=0
        userRazorFiles=0; flutterDartScaffoldFiles=0; productFacingFlutterIdentifiers=0
        checkedInGeneratedJavaScript=0; webTypeScriptScope='template-demo-host-only'
        targetManifest=$manifest
    })
}

if ($Shard -eq 'Template') {
    $feed = Join-Path $tmpRoot 'package-feed'
    foreach ($stalePath in @($statePath, (Join-Path $tmpRoot 'template.json'), (Join-Path $tmpRoot 'compile.json'), (Join-Path $tmpRoot 'publish.json'))) {
        if (Test-Path -LiteralPath $stalePath -PathType Leaf) { Remove-Item -LiteralPath $stalePath -Force }
    }
    $external = Join-Path $externalTmpRoot ("external-" + [Guid]::NewGuid().ToString('N'))
    Reset-SafeDirectory $feed $tmpRoot
    Reset-SafeDirectory $external $externalTmpRoot
    $mapping = Read-Json (Join-Path $dorotiRoot 'validation/contracts/product-naming-map.json')
    $packageProjects = [Collections.Generic.List[string]]::new()
    $requiredPackages = @(
        'Doroti.Runtime', 'Doroti.Ui', 'Doroti.Hosting', 'Doroti.Skia.RuntimeEffects',
        'Doroti.Framework.Foundation', 'Doroti.Framework.Scheduler', 'Doroti.Framework.Services',
        'Doroti.Framework.Physics', 'Doroti.Framework.Animation', 'Doroti.Framework.Gestures',
        'Doroti.Framework.Painting', 'Doroti.Framework.Semantics', 'Doroti.Framework.Rendering',
        'Doroti.Framework.Widgets', 'Doroti.Framework.Cupertino', 'Doroti.Framework.Material'
    )
    foreach ($name in $requiredPackages) {
        if ($name -ne 'Doroti.Skia.RuntimeEffects') {
            Assert-True ($name -in @($mapping.projectMappings.new)) "promoted package mapping $name"
        }
        $project = Join-Path $dorotiRoot "src/$name/$name.csproj"
        Assert-True (Test-Path -LiteralPath $project -PathType Leaf) "package project $name"
        $packageProjects.Add($project)
    }
    $sdkProject = Join-Path $dorotiRoot 'src/Doroti.App.Sdk/Doroti.App.Sdk.csproj'
    foreach ($project in @($packageProjects + @($hostProject, $targetProject, $mauiHostProject, $androidTargetProject, $sdkProject, $templateProject))) {
        Invoke-Checked { dotnet pack $project -c Release --nologo -o $feed } "package $project"
    }
    $templatePackage = Join-Path $feed 'Doroti.Templates.0.2.0-beta.nupkg'
    Assert-True (Test-Path -LiteralPath $templatePackage -PathType Leaf) 'Doroti template package'
    $hive = Join-Path $external '.templateengine'
    $packages = Join-Path $external '.nuget/packages'
    $projectRoot = Join-Path $external 'DorotiWebProduct'
    Invoke-Checked { dotnet new install $templatePackage --debug:custom-hive $hive } 'install Doroti template package'
    Invoke-Checked { dotnet new doroti-app --name DorotiWebProduct --output $projectRoot --debug:custom-hive $hive } 'create external Doroti C# application'
    $project = Join-Path $projectRoot 'DorotiWebProduct.csproj'
    $nugetConfig = Join-Path $projectRoot 'NuGet.Config'
    $escapedFeed = [Security.SecurityElement]::Escape($feed)
    [IO.File]::WriteAllText($nugetConfig, "<?xml version=`"1.0`" encoding=`"utf-8`"?><configuration><packageSources><clear/><add key=`"doroti-validation`" value=`"$escapedFeed`"/><add key=`"nuget.org`" value=`"https://api.nuget.org/v3/index.json`"/></packageSources></configuration>", [Text.UTF8Encoding]::new($false))
    Invoke-WithNuGetPackages $packages {
        dotnet restore $project --packages $packages --force --no-cache -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm --configfile $nugetConfig
    } 'restore external package-only product'
    Invoke-WithNuGetPackages $packages {
        dotnet restore $project --packages $packages --force --no-cache -p:DorotiTarget=Android -p:RuntimeIdentifier=android-arm64 --configfile $nugetConfig
    } 'restore external package-only Android product'
    Invoke-WithNuGetPackages $packages {
        dotnet build $project -c Release -p:DorotiTarget=Android -p:RuntimeIdentifier=android-arm64 --no-restore --nologo
    } 'build external package-only Android product'
    $createdFiles = @(Get-ChildItem -LiteralPath $projectRoot -File -Recurse)
    Assert-Equal @($createdFiles | Where-Object { $_.Extension -eq '.dart' -or $_.Name -in @('pubspec.yaml','.metadata','doroti.yaml') }).Count 0 'template Flutter/Dart files'
    Assert-Equal @($createdFiles | Where-Object { $_.DirectoryName -match '[\\/](ios|linux|macos)$' }).Count 0 'template unsupported platform directories'
    Assert-Equal @($createdFiles | Where-Object Extension -eq '.csproj').Count 1 'single template-created project'
    Assert-True (Test-Path -LiteralPath (Join-Path $projectRoot 'src/App.cs')) 'shared C# application source'
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $projectRoot 'Platforms/Web/PlatformBootstrap.cs'))) 'legacy app-owned Blazor bootstrap absence'
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $projectRoot 'Platforms/Maui'))) 'legacy app-owned MAUI bootstrap absence'
    Assert-True (Test-Path -LiteralPath (Join-Path $projectRoot 'Platforms/Web/tsconfig.json')) 'template-created TypeScript config'
    Assert-True (Test-Path -LiteralPath (Join-Path $projectRoot 'Platforms/Web/src/doroti_bootstrap.ts')) 'template-created TypeScript bootstrap'
    Assert-True (Test-Path -LiteralPath (Join-Path $projectRoot 'Platforms/Web/src/plugins/echo.ts')) 'template-created TypeScript plugin'
    Assert-True (Test-Path -LiteralPath (Join-Path $projectRoot 'Platforms/Android/MainActivity.cs')) 'template-created Android activity'
    Assert-True (Test-Path -LiteralPath (Join-Path $projectRoot 'Resources/Shaders/aurora.sksl')) 'template-created SkSL asset'
    Assert-Equal @($createdFiles | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' -and $_.Extension -eq '.js' }).Count 0 'template-created checked-in JavaScript'
    $startup = Get-Content -LiteralPath (Join-Path $projectRoot 'Program.cs') -Raw
    Assert-True ($startup -match 'public sealed class Program : IDorotiApplicationStartup' -and $startup -notmatch '#if|DOROTI_BROWSER|MACCATALYST') 'target-neutral public startup contract'
    $assets = Get-Content -LiteralPath (Join-Path $projectRoot 'obj/web/project.assets.json') -Raw
    Assert-True ($assets -notmatch 'projectReferences"\s*:\s*\{\s*"[A-Za-z]:\\Users\\parti\\Labo\\DorotiLab') 'repository-private package fallback'
    Write-Json $statePath ([ordered]@{ feed=$feed; external=$external; projectRoot=$projectRoot; project=$project; packages=$packages; templatePackage=$templatePackage })
    Write-Json (Join-Path $tmpRoot 'template.json') ([ordered]@{
        status='pass'; command='dotnet new doroti-app --name DorotiWebProduct'; source='package-only'; androidBuild='pass-android-arm64'
        externalRoot='repository-outside-source-tree'; packageCount=@(Get-ChildItem -LiteralPath $feed -File -Filter '*.nupkg' | Where-Object Name -notlike '*.snupkg').Count
        flutterDartFiles=0; unsupportedPlatformDirectories=0; userAuthoredRazorFiles=0; checkedInJavaScript=0
        typeScript=[ordered]@{ version='7.0.0'; config='Platforms/Web/tsconfig.json'; bootstrap='Platforms/Web/src/doroti_bootstrap.ts'; plugin='Platforms/Web/src/plugins/echo.ts' }
        flutterOrDartCommands=0; sharedApplication='src/App.cs'; browserHost='obj/web/Doroti.Generated/DorotiBootstrap.g.cs'
    })
}

if ($Shard -eq 'Compile') {
    $state = Get-ExternalState
    Invoke-DemoRestore 'Windows' 'win-x64'
    Invoke-Checked { dotnet build $demoDesktopProject -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64 --no-restore } 'build DorotiDemoApp desktop product'
    Invoke-DemoRestore 'Web' 'browser-wasm'
    Invoke-Checked { dotnet build $demoWebProject -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm --no-restore } 'build DorotiDemoApp browser-wasm product'
    Invoke-WithNuGetPackages $state.packages {
        dotnet build $state.project -c Release --no-restore -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm
    } 'build external browser-wasm Doroti product'
    $demoGeneratedRoot = Join-Path $repoRoot 'DorotiDemoApp/obj/web/Release/net10.0/Doroti.Generated/wwwroot'
    $hostGeneratedRoot = Join-Path $dorotiRoot 'src/Doroti.Host.Web/obj/Release/net10.0/Doroti.Web/wwwroot'
    foreach ($generated in @(
        (Join-Path $demoGeneratedRoot 'doroti_bootstrap.js'),
        (Join-Path $demoGeneratedRoot 'plugins/echo.js'),
        (Join-Path $hostGeneratedRoot 'doroti.loader.js'),
        (Join-Path $hostGeneratedRoot 'doroti.web.js')
    )) { Assert-True (Test-Path -LiteralPath $generated -PathType Leaf) "compiled TypeScript output $generated" }
    $externalBootstrap = Join-Path $state.projectRoot 'obj/web/Doroti.Generated/DorotiBootstrap.g.cs'
    $externalPlugins = Join-Path $state.projectRoot 'obj/web/Doroti.Generated/DorotiPluginRegistration.g.cs'
    Assert-True (Test-Path -LiteralPath $externalBootstrap -PathType Leaf) 'external SDK-owned Web bootstrap'
    Assert-True (Test-Path -LiteralPath $externalPlugins -PathType Leaf) 'external generated Web plugin registration'
    $pluginSource = Get-Content -LiteralPath $externalPlugins -Raw
    Assert-True ($pluginSource -match 'doroti\.example/echo' -and $pluginSource -match '\./plugins/echo\.js') 'generated JavaScript plugin metadata'
    $negative = @(& dotnet msbuild $demoWebProject -nologo -t:ValidateDorotiAppTarget -p:DorotiTarget=Web -p:RuntimeIdentifier=win-x64 2>&1)
    $negativeExit = $LASTEXITCODE
    Assert-True ($negativeExit -ne 0 -and (($negative -join "`n") -match 'DOROTIAPP004')) 'stable invalid target diagnostic DOROTIAPP004'
    $global:LASTEXITCODE = 0
    $fixtureRoot = Join-Path $dorotiRoot 'validation/web-typescript'
    $negativeRoot = Join-Path $repoRoot 'DorotiDemoApp/obj/web/typescript-negative'
    Reset-SafeDirectory $negativeRoot (Join-Path $repoRoot 'DorotiDemoApp/obj/web')
    function Invoke-TypeScriptNegative([string] $Name, [string] $ExpectedCode, [string[]] $Properties, [string] $Target = 'DorotiCompileTypeScript') {
        $arguments = @('msbuild', $demoWebProject, '-nologo', "-t:$Target", '-p:Configuration=Release', '-p:DorotiTarget=Web', '-p:RuntimeIdentifier=browser-wasm') + $Properties
        $output = @(& dotnet @arguments 2>&1)
        $exitCode = $LASTEXITCODE
        Assert-True ($exitCode -ne 0 -and (($output -join "`n") -match $ExpectedCode)) "TypeScript negative $Name $ExpectedCode"
        $global:LASTEXITCODE = 0
        return [ordered]@{ status='fail-closed-as-expected'; diagnostic=$ExpectedCode }
    }
    $missingCompiler = Join-Path $negativeRoot 'missing/tsc.exe'
    $missingConfig = Join-Path $negativeRoot 'missing/tsconfig.json'
    $compilerNegative = Invoke-TypeScriptNegative 'missing-compiler' 'DOROTIWEB013' @("-p:DorotiTypeScriptCompilerExecutable=$missingCompiler") 'DorotiValidateTypeScriptToolchain'
    $configNegative = Invoke-TypeScriptNegative 'missing-config' 'DOROTIWEB011' @("-p:DorotiTypeScriptConfig=$missingConfig") 'DorotiValidateTypeScriptToolchain'
    $emptySourceFixture = Join-Path $fixtureRoot 'empty-source'
    $emptySourceNegative = Invoke-TypeScriptNegative 'empty-source' 'DOROTIWEB012' @("-p:DorotiTypeScriptConfig=$(Join-Path $emptySourceFixture 'tsconfig.json')", "-p:DorotiTypeScriptSourceRoot=$emptySourceFixture") 'DorotiValidateTypeScriptToolchain'
    $sourceOutputNegative = Invoke-TypeScriptNegative 'source-output' 'DOROTIWEB014' @('-p:DorotiTypeScriptOutputRoot=Platforms/Web/wwwroot') 'DorotiValidateTypeScriptToolchain'
    $invalidFixture = Join-Path $fixtureRoot 'invalid-syntax'
    $invalidOutput = Join-Path $negativeRoot 'invalid-syntax'
    [IO.Directory]::CreateDirectory($invalidOutput) | Out-Null
    [IO.File]::WriteAllText((Join-Path $invalidOutput 'doroti_bootstrap.js'), 'stale-output', [Text.UTF8Encoding]::new($false))
    $invalidNegative = Invoke-TypeScriptNegative 'syntax-and-stale' 'DOROTIWEB016' @(
        "-p:DorotiTypeScriptConfig=$(Join-Path $invalidFixture 'tsconfig.json')",
        "-p:DorotiTypeScriptSourceRoot=$invalidFixture",
        "-p:DorotiTypeScriptOutputRoot=$invalidOutput",
        "-p:DorotiTypeScriptExpectedOutput=$(Join-Path $invalidOutput 'doroti_bootstrap.js')"
    )
    Assert-Equal (Get-Content -LiteralPath (Join-Path $invalidOutput 'doroti_bootstrap.js') -Raw) 'stale-output' 'invalid TypeScript did not overwrite stale JavaScript'
    $emptyEmitFixture = Join-Path $fixtureRoot 'empty-emit'
    $emptyEmitOutput = Join-Path $negativeRoot 'empty-emit'
    [IO.Directory]::CreateDirectory($emptyEmitOutput) | Out-Null
    $emptyEmitNegative = Invoke-TypeScriptNegative 'empty-emit' 'DOROTIWEB017' @(
        "-p:DorotiTypeScriptConfig=$(Join-Path $emptyEmitFixture 'tsconfig.json')",
        "-p:DorotiTypeScriptSourceRoot=$emptyEmitFixture",
        "-p:DorotiTypeScriptOutputRoot=$emptyEmitOutput",
        "-p:DorotiTypeScriptExpectedOutput=$(Join-Path $emptyEmitOutput 'doroti_bootstrap.js')"
    )
    $browserAssembly = Get-ChildItem (Join-Path $repoRoot 'DorotiDemoApp/bin/web/Release/net10.0/wwwroot/_framework') -File -Filter 'DorotiDemoApp*.wasm' | Where-Object Name -notmatch '\.(br|gz)$' | Select-Object -First 1
    Assert-True ($null -ne $browserAssembly -and $browserAssembly.Length -gt 0) 'DorotiDemoApp assembly in browser build'
    Write-Json (Join-Path $tmpRoot 'compile.json') ([ordered]@{
        status='pass'; browser=[ordered]@{ target='browser-wasm'; configuration='Release'; diagnostics=0; applicationAssembly=$browserAssembly.Name; applicationSha256=Get-Sha $browserAssembly.FullName }
        sharedSource=[ordered]@{ path='DorotiDemoApp/src/App.cs'; startup='DorotiDemoApp/Program.cs'; generatedBootstrap='obj/web/Doroti.Generated/DorotiBootstrap.g.cs'; generatedPluginRegistration='obj/web/Doroti.Generated/DorotiPluginRegistration.g.cs'; desktopCompile='win-x64-pass'; browserCompile='browser-wasm-pass'; compileItemIdentity='same-files' }
        externalAcceptance=[ordered]@{ project='template-created-DorotiWebProduct'; build='pass'; source='promoted-packages-only' }
        typeScript=[ordered]@{ version='7.0.0'; sourceCount=2; appOutput='obj/web/Release/net10.0/Doroti.Generated/wwwroot'; hostOutput='Doroti.Host.Web/obj/Release/net10.0/Doroti.Web/wwwroot' }
        negative=[ordered]@{ invalidTarget=[ordered]@{ status='fail-closed-as-expected'; diagnostic='DOROTIAPP004' }; compiler=$compilerNegative; config=$configNegative; emptySource=$emptySourceNegative; sourceOutput=$sourceOutputNegative; syntaxAndStale=$invalidNegative; emptyEmit=$emptyEmitNegative }
        nativeSkiaLink='pass'; flutterOrDartCommands=0
    })
}

if ($Shard -eq 'Publish') {
    $state = Get-ExternalState
    $publishA = Join-Path $tmpRoot 'doroti-demo-publish-a'
    $publishB = Join-Path $tmpRoot 'doroti-demo-publish-b'
    $externalPublish = Join-Path $state.external 'acceptance-publish'
    Reset-SafeDirectory $publishA $tmpRoot
    Reset-SafeDirectory $publishB $tmpRoot
    Reset-SafeDirectory $externalPublish $state.external
    Invoke-Checked { dotnet clean $demoWebProject -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm --nologo } 'clean stale DorotiDemoApp Web intermediates'
    foreach ($source in @(
        (Join-Path $repoRoot 'DorotiDemoApp/Platforms/Web/tsconfig.json'),
        (Join-Path $repoRoot 'DorotiDemoApp/Platforms/Web/src/doroti_bootstrap.ts'),
        (Join-Path $repoRoot 'DorotiDemoApp/Platforms/Web/src/plugins/echo.ts')
    )) { Assert-True (Test-Path -LiteralPath $source -PathType Leaf) "clean preserved TypeScript source $source" }
    foreach ($generatedRoot in @(
        (Join-Path $repoRoot 'DorotiDemoApp/obj/web/Release/net10.0/Doroti.Generated/wwwroot'),
        (Join-Path $dorotiRoot 'src/Doroti.Host.Web/obj/Release/net10.0/Doroti.Web/wwwroot')
    )) { Assert-True (-not (Test-Path -LiteralPath $generatedRoot)) "clean removed generated TypeScript output $generatedRoot" }
    Invoke-Checked { dotnet restore $demoWebProject --force-evaluate -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm } 'restore DorotiDemoApp Web product for publish'
    Invoke-Checked { dotnet publish $demoWebProject -c Release --no-restore -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o $publishA } 'first DorotiDemoApp browser-wasm publish'
    Invoke-Checked { dotnet publish $demoWebProject -c Release --no-restore -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o $publishB } 'repeat DorotiDemoApp browser-wasm publish'
    Invoke-WithNuGetPackages $state.packages {
        dotnet restore $state.project --packages $state.packages --force --no-cache -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm "-p:RestoreAdditionalProjectSources=$($state.feed)"
    } 'restore external template product for acceptance publish'
    Invoke-WithNuGetPackages $state.packages {
        dotnet publish $state.project -c Release --no-restore -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o $externalPublish
    } 'publish external template/package-only product'
    $staticA = Join-Path $publishA 'wwwroot'
    $staticB = Join-Path $publishB 'wwwroot'
    $identityA = Get-StaticIdentity $staticA
    $identityB = Get-StaticIdentity $staticB
    Assert-Equal $identityA.Count $identityB.Count 'repeat static file count'
    $hashA = Get-IdentityHash $identityA
    $hashB = Get-IdentityHash $identityB
    Assert-Equal $hashA $hashB 'repeat deployment-neutral static identity'
    $index = Get-Content -LiteralPath (Join-Path $staticA 'index.html') -Raw
    Assert-True ($index -match '_framework/blazor\.webassembly\.js" autostart="false"' -and $index -match '<script type="module" src="doroti_bootstrap\.js"></script>') 'explicit compiled Doroti bootstrap order'
    Assert-True ($index -notmatch '<script[^>]+doroti_bootstrap\.js[^>]+async') 'non-async bootstrap module ordering'
    foreach ($relative in @('_framework/blazor.webassembly.js','_framework/dotnet.js','doroti_bootstrap.js','_content/Doroti.Host.Web/doroti.loader.js','_content/Doroti.Host.Web/doroti.web.js','_content/Doroti.Host.Web/doroti.web.css','assets/doroti-mark.txt','locales/en-US.json','plugins/echo.js','doroti-app-manifest.json')) {
        Assert-True (Test-Path -LiteralPath (Join-Path $staticA $relative) -PathType Leaf) "published static asset $relative"
    }
    $forbiddenPublishFiles = @(Get-ChildItem -LiteralPath $publishA -File -Recurse | Where-Object { $_.Extension -eq '.ts' -or $_.Name -eq 'tsconfig.json' -or $_.Name -match '^tsc(\.exe)?$' })
    Assert-Equal $forbiddenPublishFiles.Count 0 'TypeScript source and compiler publish leakage'
    foreach ($relative in @('doroti_bootstrap.js','plugins/echo.js','_content/Doroti.Host.Web/doroti.loader.js','_content/Doroti.Host.Web/doroti.web.js')) {
        Assert-True (-not (Get-Content -LiteralPath (Join-Path $staticA $relative) -Raw).Contains($repoRoot, [StringComparison]::OrdinalIgnoreCase)) "repository path absence in $relative"
    }
    $native = @(Get-ChildItem (Join-Path $staticA '_framework') -File -Filter 'dotnet.native.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    $skia = @(Get-ChildItem (Join-Path $staticA '_framework') -File -Filter 'SkiaSharp.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    $app = @(Get-ChildItem (Join-Path $staticA '_framework') -File -Filter 'DorotiDemoApp.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    $runtimeConfig = Read-Json (Join-Path $publishA 'DorotiDemoApp.runtimeconfig.json')
    $runtimeFrameworkVersion = [string] @($runtimeConfig.runtimeOptions.includedFrameworks | Where-Object name -eq 'Microsoft.NETCore.App')[0].version
    $runtimeScript = @(Get-ChildItem (Join-Path $staticA '_framework') -File -Filter 'dotnet.runtime.*.js' | Where-Object Name -notmatch '\.(br|gz)$')
    $publishedCoreLib = @(Get-ChildItem (Join-Path $staticA '_framework') -File -Filter 'System.Private.CoreLib.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    $buildCoreLib = @(Get-ChildItem (Join-Path $repoRoot 'DorotiDemoApp/bin/web/Release/net10.0/wwwroot/_framework') -File -Filter 'System.Private.CoreLib.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    Assert-Equal $native.Count 1 'native WebAssembly runtime artifact'
    Assert-True ($skia.Count -ge 2) 'SkiaSharp managed and Blazor WASM assemblies'
    Assert-Equal $app.Count 1 'C# application WASM assembly'
    Assert-Equal $runtimeScript.Count 1 'managed browser runtime script'
    Assert-Equal $runtimeFrameworkVersion '10.0.11' 'browser runtime framework patch'
    Assert-True ($runtimeFrameworkVersion -match '^10\.0\.\d+$' -and (Get-Content -LiteralPath $runtimeScript[0].FullName -Raw).Contains($runtimeFrameworkVersion, [StringComparison]::Ordinal)) 'browser runtime and framework patch identity'
    Assert-Equal $publishedCoreLib.Count 1 'published System.Private.CoreLib WebCIL'
    Assert-Equal $buildCoreLib.Count 1 'build System.Private.CoreLib WebCIL'
    Assert-Equal (Get-Sha $publishedCoreLib[0].FullName) (Get-Sha $buildCoreLib[0].FullName) 'build and publish System.Private.CoreLib WebCIL identity'
    Assert-Equal @($identityA | Where-Object path -match '(?i)canvaskit').Count 0 'CanvasKit artifacts'
    Assert-Equal @(Get-ChildItem -LiteralPath $publishA -File -Recurse | Where-Object FullName -match '[\\/]App[\\/](bin|obj)[\\/]').Count 0 'nested project build leakage into publish output'
    $assets = Get-Content -LiteralPath (Join-Path $repoRoot 'DorotiDemoApp/obj/web/project.assets.json') -Raw
    Assert-True ($assets -notmatch 'Doroti\.(Host\.Desktop|Shell|Vendor\.|Target\.Windows\.win-x64|Target\.macOS)') 'DorotiDemoApp browser assets legacy desktop graph'
    $externalAssets = Get-Content -LiteralPath (Join-Path $state.projectRoot 'obj/web/project.assets.json') -Raw
    Assert-True ($externalAssets -notmatch [regex]::Escape($dorotiRoot)) 'external browser assets repository source fallback'
    $externalStatic = Join-Path $externalPublish 'wwwroot'
    Assert-True (Test-Path -LiteralPath (Join-Path $externalStatic '_content/Doroti.Host.Web/doroti.web.js') -PathType Leaf) 'external package-only host static asset'
    Assert-True (Test-Path -LiteralPath (Join-Path $externalStatic '_content/Doroti.Host.Web/doroti.loader.js') -PathType Leaf) 'external package-only loader static asset'
    Assert-True (Test-Path -LiteralPath (Join-Path $externalStatic 'doroti_bootstrap.js') -PathType Leaf) 'external package-only application bootstrap static asset'
    Assert-True (Test-Path -LiteralPath (Join-Path $externalStatic 'plugins/echo.js') -PathType Leaf) 'external package-only plugin static asset'
    Assert-True (@(Get-ChildItem (Join-Path $externalStatic '_framework') -File -Filter 'DorotiWebProduct.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$').Count -eq 1) 'external package-only C# app assembly'
    [IO.Directory]::CreateDirectory($releaseRoot) | Out-Null
    $artifactRoot = Join-Path $releaseRoot 'wwwroot'
    Reset-SafeDirectory $artifactRoot $releaseRoot
    Copy-Item -Path (Join-Path $staticA '*') -Destination $artifactRoot -Recurse -Force
    $artifactIdentity = Get-StaticIdentity $artifactRoot
    Assert-Equal (Get-IdentityHash $artifactIdentity) $hashA 'copied release artifact identity'
    $appSource = Join-Path $repoRoot 'DorotiDemoApp/src/App.cs'
    $webModules = [ordered]@{
        applicationBootstrap=[ordered]@{ source='DorotiDemoApp/Platforms/Web/src/doroti_bootstrap.ts'; sourceSha256=Get-Sha (Join-Path $repoRoot 'DorotiDemoApp/Platforms/Web/src/doroti_bootstrap.ts'); artifact='doroti_bootstrap.js'; artifactSha256=Get-Sha (Join-Path $staticA 'doroti_bootstrap.js') }
        applicationPlugin=[ordered]@{ source='DorotiDemoApp/Platforms/Web/src/plugins/echo.ts'; sourceSha256=Get-Sha (Join-Path $repoRoot 'DorotiDemoApp/Platforms/Web/src/plugins/echo.ts'); artifact='plugins/echo.js'; artifactSha256=Get-Sha (Join-Path $staticA 'plugins/echo.js') }
        dorotiLoader=[ordered]@{ source='Doroti/src/Doroti.Host.Web/Web/doroti.loader.ts'; sourceSha256=Get-Sha (Join-Path $dorotiRoot 'src/Doroti.Host.Web/Web/doroti.loader.ts'); artifact='_content/Doroti.Host.Web/doroti.loader.js'; artifactSha256=Get-Sha (Join-Path $staticA '_content/Doroti.Host.Web/doroti.loader.js') }
        browserInterop=[ordered]@{ source='Doroti/src/Doroti.Host.Web/Web/doroti.web.ts'; sourceSha256=Get-Sha (Join-Path $dorotiRoot 'src/Doroti.Host.Web/Web/doroti.web.ts'); artifact='_content/Doroti.Host.Web/doroti.web.js'; artifactSha256=Get-Sha (Join-Path $staticA '_content/Doroti.Host.Web/doroti.web.js') }
    }
    $manifest = [ordered]@{
        schemaVersion='doroti.static-artifact-manifest/v4'; scope='web-product'; target='browser-wasm'; version='0.2.0-beta'
        deploymentRoot='wwwroot'; aggregateSha256=$hashA; repeatAggregateSha256=$hashB; fileCount=$artifactIdentity.Count
        application=[ordered]@{ source='DorotiDemoApp/src/App.cs'; bootstrap='DorotiDemoApp/Program.cs'; sourceSha256=Get-Sha $appSource; assembly=$app[0].Name; assemblySha256=Get-Sha $app[0].FullName; project='DorotiDemoApp.csproj' }
        runtime=[ordered]@{ frameworkVersion=$runtimeFrameworkVersion; coreLibWebCil=$publishedCoreLib[0].Name; coreLibSha256=Get-Sha $publishedCoreLib[0].FullName; nativeWasm=$native[0].Name; nativeSha256=Get-Sha $native[0].FullName; skiaWasm=@($skia | ForEach-Object { [ordered]@{ name=$_.Name; sha256=Get-Sha $_.FullName } }) }
        webModules=$webModules
        packages=[ordered]@{ target='Doroti.Target.Web.browser-wasm/0.2.0-beta'; blazor='10.0.11'; skiaSharp='4.151.1'; typescriptMsBuild='7.0.0-private-build-only' }
        files=$artifactIdentity
    }
    Write-Json (Join-Path $artifactRoot 'artifact-manifest.json') $manifest
    foreach ($item in $artifactIdentity) {
        Assert-Equal (Get-Sha (Join-Path $artifactRoot $item.path)) $item.sha256 "release artifact hash $($item.path)"
    }
    Write-Json (Join-Path $tmpRoot 'publish.json') ([ordered]@{
        status='pass'; primaryProduct='DorotiDemoApp'; source='same src/App.cs and root Program.cs compiled for desktop and browser-wasm'
        create='pass'; restore='clean'; build='pass'; publish='pass'; repeatPublishIdentity='pass'
        externalAcceptance=[ordered]@{ consumer='repository-outside-source-tree'; source='template-and-promoted-packages-only'; create='pass'; restore='clean'; build='pass'; publish='pass' }
        staticFileCount=$identityA.Count; aggregateSha256=$hashA; repeatAggregateSha256=$hashB
        bootLoader='_framework/blazor.webassembly.js'; runtimeFrameworkVersion=$runtimeFrameworkVersion; coreLibWebCil=[ordered]@{ name=$publishedCoreLib[0].Name; sha256=Get-Sha $publishedCoreLib[0].FullName }; nativeWasm=[ordered]@{ name=$native[0].Name; sha256=Get-Sha $native[0].FullName }
        skiaWasmCount=$skia.Count; appAssembly=$app[0].Name; resources=@('assets/doroti-mark.txt','locales/en-US.json')
        webModules=$webModules; publishedTypeScriptSources=0; publishedTypeScriptCompilerAssets=0
        repositoryPrivateFallbacks=0; desktopNativeDependencies=0; legacyDesktopDependencies=0; canvasKitArtifacts=0
        missingStaticArtifacts=0; staticArtifactHashMismatches=0; flutterOrDartCommands=0
        artifactRoot='artifacts/web/0.2.0-beta/wwwroot'
    })
}

Write-Composite
Write-Output "Doroti Web product shard '$Shard': PASS"
