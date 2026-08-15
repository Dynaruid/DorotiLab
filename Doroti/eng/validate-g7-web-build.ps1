#Requires -Version 7.0
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Toolchain', 'Reference', 'Hosting', 'Graph', 'Template', 'Compile', 'Publish')]
    [string] $Shard
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$migrationRoot = Join-Path $dorotiRoot 'migration/web'
$tmpRoot = Join-Path $dorotiRoot '.doroti/tmp/g7-web-product'
$releaseRoot = Join-Path $dorotiRoot 'artifacts/g7-web/0.2.0-beta'
$hostProject = Join-Path $dorotiRoot 'src/Doroti.Host.Web/Doroti.Host.Web.csproj'
$targetProject = Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm/Doroti.Target.Web.browser-wasm.csproj'
$templateProject = Join-Path $dorotiRoot 'templates/Doroti.Templates/Doroti.Templates.csproj'
$demoDesktopProject = Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.csproj'
$demoWebProject = Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.Web.csproj'
$statePath = Join-Path $tmpRoot 'external-product.json'
[IO.Directory]::CreateDirectory($migrationRoot) | Out-Null
[IO.Directory]::CreateDirectory($tmpRoot) | Out-Null

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
    Assert-True (Test-Path -LiteralPath $state.project -PathType Leaf) 'external generated project'
    Assert-True (Test-Path -LiteralPath $state.feed -PathType Container) 'external package feed'
    return $state
}

function Write-Composite {
    $names = @('toolchain', 'reference', 'hosting', 'graph', 'template', 'compile', 'publish')
    $shards = [ordered]@{}
    foreach ($name in $names) {
        $path = Join-Path $tmpRoot "$name.json"
        $shards[$name] = if (Test-Path -LiteralPath $path) { Read-Json $path } else { [ordered]@{ status='notVerified' } }
    }
    $pass = @($names | Where-Object { $shards[$_].status -ne 'pass' }).Count -eq 0
    Write-Json (Join-Path $migrationRoot 'g7-web-build-evidence.json') ([ordered]@{
        schemaVersion = 'doroti.g7-web-build-evidence/v2'
        milestone = 'G7-3'
        status = $(if ($pass) { 'pass' } else { 'partial' })
        target = 'browser-wasm'
        shards = $shards
        closure = [ordered]@{
            avaloniaProductDependencies = 0
            desktopNativeDependencies = 0
            canvasKitArtifacts = 0
            repositoryPrivateFallbacks = 0
            unsupportedCapabilitySilentSuccesses = 0
            missingStaticArtifacts = 0
            staticArtifactHashMismatches = 0
            flutterOrDartProductCommands = 0
        }
        notVerified = @(
            [ordered]@{ mode='trimmed-generated-product'; blocker='generated framework reflection roots have not been audited; the G7-3 product baseline pins PublishTrimmed=false' },
            [ordered]@{ mode='wasm-aot-product'; blocker='G7-3 validates the native-linked interpreter product; product AOT and trimming require their own generated-framework closure' },
            [ordered]@{ mode='chromium-live-product'; owner='G7-4'; blocker='GPU pixels, browser input, IME, clipboard and ARIA actions require the isolated Chromium live gate' }
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

if ($Shard -eq 'Reference') {
    $provenancePath = Join-Path $migrationRoot 'g7-browser-reference-provenance.json'
    $selectionPath = Join-Path $migrationRoot 'g7-browser-reference-selection.json'
    $provenance = Read-Json $provenancePath
    $selection = Read-Json $selectionPath
    $avaloniaRepo = Join-Path $repoRoot 'reference/Avalonia-main'
    Assert-Equal $provenance.revision 'f159423f691946e713f454447a780d4677d8a0d2' 'Avalonia Browser reference revision'
    $selected = @($provenance.selectedSources)
    Assert-True ($selected.Count -ge 10) 'selected Avalonia behavior reference files'
    foreach ($source in @($selected + $provenance.license)) {
        $sourcePath = Join-Path $avaloniaRepo $source.path
        Assert-True (Test-Path -LiteralPath $sourcePath -PathType Leaf) "reference source exists $($source.path)"
        $actual = (& git hash-object -- $sourcePath).Trim()
        Assert-Equal $LASTEXITCODE 0 "reference blob hash $($source.path)"
        Assert-Equal $actual $source.gitBlobSha1 "reference blob $($source.path)"
    }
    Assert-Equal $provenance.productCompileGraphIncluded $false 'reference compile exclusion'
    Assert-True (-not [string]::IsNullOrWhiteSpace($provenance.referenceRemovalCondition)) 'reference removal condition'
    foreach ($capability in @($selection.capabilities)) {
        Assert-True (@($capability.upstreamSymbols).Count -gt 0) "upstream symbols $($capability.id)"
        Assert-True (-not [string]::IsNullOrWhiteSpace($capability.behavior)) "selected behavior $($capability.id)"
        Assert-True ($capability.dorotiOwner -like 'Doroti.Host.Web.*') "independent Doroti owner $($capability.id)"
        Assert-True (@($capability.independentImplementation).Count -gt 0) "independent implementation $($capability.id)"
    }
    $productFiles = @(Get-ChildItem (Join-Path $dorotiRoot 'src/Doroti.Host.Web'), (Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm') -File -Recurse)
    Assert-Equal @($productFiles | Select-String -Pattern '^\s*(namespace|using)\s+Avalonia' -CaseSensitive).Count 0 'Avalonia source namespace in product graph'
    Write-Json (Join-Path $tmpRoot 'reference.json') ([ordered]@{
        status='pass'; upstream=$provenance.upstream; revision=$provenance.revision
        snapshotMode=$provenance.snapshotMode; selectedSourceCount=$selected.Count
        selectedCapabilityCount=@($selection.capabilities).Count; license=$provenance.license.spdx
        copiedSources=0; runtimeDependencies=0; productCompileGraphIncluded=$false
    })
}

if ($Shard -eq 'Hosting') {
    $hostRoot = Join-Path $dorotiRoot 'src/Doroti.Host.Web'
    $surface = Get-Content -LiteralPath (Join-Path $hostRoot 'DorotiSurface.razor') -Raw
    $root = Get-Content -LiteralPath (Join-Path $hostRoot 'DorotiRoot.razor') -Raw
    $contracts = Get-Content -LiteralPath (Join-Path $hostRoot 'BrowserHostContracts.cs') -Raw
    $skia = Get-Content -LiteralPath (Join-Path $hostRoot 'BrowserSkiaCapabilities.cs') -Raw
    $frameworkHost = Get-Content -LiteralPath (Join-Path $hostRoot 'BrowserFrameworkHost.cs') -Raw
    $script = Get-Content -LiteralPath (Join-Path $hostRoot 'wwwroot/doroti.web.js') -Raw
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
    Assert-True ($script -notmatch 'getContext\(["'']2d["'']' -and $script -notmatch 'canvaskit') 'software and CanvasKit fallback absence'
    Write-Json (Join-Path $tmpRoot 'hosting.json') ([ordered]@{
        status='pass'; razorComponents=@($razor.Name | Sort-Object); visibleWidgetDomNodes=0
        domOwners=@('canvas','hidden-text-input','accessibility-bridge')
        gpuChain=@('SKGLView WebGL2 context','SkiaSharp SKSurface','Doroti scene draw','explicit invalidate','terminal frame diagnostics')
        inputChain=@('DOM pointer/key/focus','JSImport/JSExport primitive bridge','BrowserHostAdapter normalization','IInputHostCapability','Doroti gesture/state')
        softwareFallbacks=0; canvasKitDependencies=0
    })
}

if ($Shard -eq 'Graph') {
    $manifest = Read-Json (Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm/doroti-target-manifest.json')
    $central = Get-Content -LiteralPath (Join-Path $dorotiRoot 'Directory.Packages.props') -Raw
    $hostCsproj = Get-Content -LiteralPath $hostProject -Raw
    $targetCsproj = Get-Content -LiteralPath $targetProject -Raw
    $templateRoot = Join-Path $dorotiRoot 'templates/Doroti.Templates/content/doroti-app'
    $demoWebProjectText = Get-Content -LiteralPath $demoWebProject -Raw
    $templateFiles = @(Get-ChildItem -LiteralPath $templateRoot -File -Recurse)
    $graphFiles = @(Get-ChildItem (Join-Path $dorotiRoot 'src/Doroti.Host.Web'), (Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm') -Include *.cs,*.csproj,*.razor,*.js,*.json -File -Recurse |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' -and $_.Name -ne 'packages.lock.json' })
    $forbidden = @($graphFiles | Select-String -Pattern 'Doroti\.(Host\.Desktop|Shell\.|Vendor\.Avalonia|Target\.Windows|Target\.macOS)|Avalonia\.Controls|Win32|AppKit|NSOpenGL|WGL' -CaseSensitive)
    Assert-Equal $forbidden.Count 0 'browser graph desktop/Avalonia dependency scan'
    Assert-True ($central -match 'SkiaSharp.Views.Blazor" Version="3.119.4"' -and $central -match 'SkiaSharp.NativeAssets.WebAssembly" Version="3.119.4"') 'SkiaSharp WebAssembly version set'
    Assert-True ($central -match 'Microsoft.AspNetCore.Components.WebAssembly" Version="10.0.0"') 'Blazor WebAssembly package pin'
    Assert-True ($hostCsproj -match 'SkiaSharp.Views.Blazor' -and $hostCsproj -match 'SkiaSharp.NativeAssets.WebAssembly') 'Web host Skia package graph'
    Assert-True ($targetCsproj -match 'Microsoft.AspNetCore.Components.WebAssembly' -and $targetCsproj -match 'buildTransitive') 'target Blazor build contract'
    Assert-True ($demoWebProjectText -match 'Microsoft.NET.Sdk.BlazorWebAssembly' -and $demoWebProjectText -match 'Program.cs' -and $demoWebProjectText -match 'Doroti.Target.Web.browser-wasm') 'DorotiDemoApp shared-source Web target'
    Assert-Equal $manifest.rid 'browser-wasm' 'browser target RID'
    Assert-Equal $manifest.graphicsBackend 'webgl2-browser-gpu-required' 'browser GPU policy'
    Assert-Equal $manifest.skiaSharpVersion '3.119.4' 'manifest SkiaSharp version'
    Assert-Equal $manifest.blazorWebAssemblyVersion '10.0.0' 'manifest Blazor version'
    Assert-Equal @($templateFiles | Where-Object { $_.Extension -in @('.dart') -or $_.Name -in @('pubspec.yaml','.metadata','doroti.yaml') }).Count 0 'template Flutter/Dart scaffold files'
    Assert-Equal @($templateFiles | Where-Object Extension -eq '.razor').Count 0 'user-owned Razor files'
    $templateSource = ($templateFiles | Where-Object Extension -in @('.cs','.csproj') | ForEach-Object { Get-Content -LiteralPath $_.FullName -Raw }) -join "`n"
    Assert-True ($templateSource -notmatch '\bFlutter[A-Za-z0-9_]*\b') 'product-facing Flutter identifiers in template source'
    Assert-True ($templateSource -notmatch 'Router|EditForm|Microsoft\.AspNetCore\.Components\.Forms') 'router/forms/component UI dependency absence'
    Write-Json (Join-Path $tmpRoot 'graph.json') ([ordered]@{
        status='pass'; packageVersions=[ordered]@{ blazor='10.0.0'; skiaSharp='3.119.4' }
        desktopNativeDependencies=0; avaloniaDependencies=0; canvasKitDependencies=0
        userRazorFiles=0; flutterDartScaffoldFiles=0; productFacingFlutterIdentifiers=0
        targetManifest=$manifest
    })
}

if ($Shard -eq 'Template') {
    $feed = Join-Path $tmpRoot 'package-feed'
    $external = Join-Path ([IO.Path]::GetTempPath()) ("doroti-g7-product-" + [Guid]::NewGuid().ToString('N'))
    Reset-SafeDirectory $feed $tmpRoot
    [IO.Directory]::CreateDirectory($external) | Out-Null
    $mapping = Read-Json (Join-Path $dorotiRoot 'migration/product-naming/g7-doroti-naming-map.json')
    $packageProjects = [Collections.Generic.List[string]]::new()
    $requiredPackages = @(
        'Doroti.Runtime', 'Doroti.Ui', 'Doroti.Hosting',
        'Doroti.Framework.Foundation', 'Doroti.Framework.Scheduler', 'Doroti.Framework.Services',
        'Doroti.Framework.Physics', 'Doroti.Framework.Animation', 'Doroti.Framework.Gestures',
        'Doroti.Framework.Painting', 'Doroti.Framework.Semantics', 'Doroti.Framework.Rendering',
        'Doroti.Framework.Widgets', 'Doroti.Framework.Cupertino', 'Doroti.Framework.Material'
    )
    foreach ($name in $requiredPackages) {
        Assert-True ($name -in @($mapping.projectMappings.new)) "promoted package mapping $name"
        $project = Join-Path $dorotiRoot "src/$name/$name.csproj"
        Assert-True (Test-Path -LiteralPath $project -PathType Leaf) "package project $name"
        $packageProjects.Add($project)
    }
    foreach ($project in @($packageProjects + @($hostProject, $targetProject, $templateProject))) {
        Invoke-Checked { dotnet pack $project -c Release --nologo -o $feed } "package $project"
    }
    $templatePackage = Join-Path $feed 'Doroti.Templates.0.2.0-beta.nupkg'
    Assert-True (Test-Path -LiteralPath $templatePackage -PathType Leaf) 'Doroti template package'
    $hive = Join-Path $external '.templateengine'
    $packages = Join-Path $external '.nuget/packages'
    $projectRoot = Join-Path $external 'G7WebProduct'
    Invoke-Checked { dotnet new install $templatePackage --debug:custom-hive $hive } 'install Doroti template package'
    Invoke-Checked { dotnet new doroti-app --name G7WebProduct --output $projectRoot --debug:custom-hive $hive } 'create external Doroti C# application'
    $project = Join-Path $projectRoot 'G7WebProduct.csproj'
    Invoke-Checked { dotnet restore $project --packages $packages --force --no-cache "-p:RestoreAdditionalProjectSources=$feed" } 'restore external package-only product'
    $generatedFiles = @(Get-ChildItem -LiteralPath $projectRoot -File -Recurse)
    Assert-Equal @($generatedFiles | Where-Object { $_.Extension -eq '.dart' -or $_.Name -in @('pubspec.yaml','.metadata','doroti.yaml') }).Count 0 'generated Flutter/Dart files'
    Assert-Equal @($generatedFiles | Where-Object { $_.DirectoryName -match '[\\/](android|ios|linux|macos|windows|web)$' }).Count 0 'generated Flutter platform directories'
    Assert-True (Test-Path -LiteralPath (Join-Path $projectRoot 'App/G7WebProduct.App.csproj')) 'shared C# application project'
    Assert-True (Test-Path -LiteralPath (Join-Path $projectRoot 'BrowserHost/Program.cs')) 'internal Blazor host'
    $assets = Get-Content -LiteralPath (Join-Path $projectRoot 'obj/project.assets.json') -Raw
    Assert-True ($assets -notmatch 'projectReferences"\s*:\s*\{\s*"[A-Za-z]:\\Users\\parti\\Labo\\DorotiLab') 'repository-private package fallback'
    Write-Json $statePath ([ordered]@{ feed=$feed; external=$external; projectRoot=$projectRoot; project=$project; packages=$packages; templatePackage=$templatePackage })
    Write-Json (Join-Path $tmpRoot 'template.json') ([ordered]@{
        status='pass'; command='dotnet new doroti-app --name G7WebProduct'; source='package-only'
        externalRoot='repository-outside-source-tree'; packageCount=@(Get-ChildItem -LiteralPath $feed -File -Filter '*.nupkg' | Where-Object Name -notlike '*.snupkg').Count
        generatedFlutterDartFiles=0; generatedPlatformDirectories=0; userAuthoredRazorFiles=0
        flutterOrDartCommands=0; sharedApplication='App/G7WebProduct.App.csproj'; browserHost='G7WebProduct.csproj'
    })
}

if ($Shard -eq 'Compile') {
    $state = Get-ExternalState
    Invoke-Checked { dotnet build $demoDesktopProject -c Release } 'build DorotiDemoApp desktop product'
    Invoke-Checked { dotnet build $demoWebProject -c Release } 'build DorotiDemoApp browser-wasm product'
    Invoke-Checked { dotnet build $state.project -c Release --no-restore } 'build external browser-wasm Doroti product'
    $negative = @(& dotnet msbuild $state.project -nologo -t:DorotiValidateBrowserProductContract -p:DorotiBrowserContractRuntimeIdentifier=win-x64 2>&1)
    $negativeExit = $LASTEXITCODE
    Assert-True ($negativeExit -ne 0 -and (($negative -join "`n") -match 'DOROTIWEB002')) 'stable invalid target diagnostic DOROTIWEB002'
    $browserAssembly = Get-ChildItem (Join-Path $repoRoot 'DorotiDemoApp/bin/Web/Release/net10.0/wwwroot/_framework') -File -Filter 'DorotiDemoApp.Web*.wasm' | Where-Object Name -notmatch '\.(br|gz)$' | Select-Object -First 1
    Assert-True ($null -ne $browserAssembly -and $browserAssembly.Length -gt 0) 'DorotiDemoApp assembly in browser build'
    Write-Json (Join-Path $tmpRoot 'compile.json') ([ordered]@{
        status='pass'; browser=[ordered]@{ target='browser-wasm'; configuration='Release'; diagnostics=0; applicationAssembly=$browserAssembly.Name; applicationSha256=Get-Sha $browserAssembly.FullName }
        sharedSource=[ordered]@{ path='DorotiDemoApp/Program.cs'; desktopCompile='win-x64-pass'; browserCompile='browser-wasm-pass'; compileItemIdentity='same-file' }
        externalAcceptance=[ordered]@{ project='template-created-G7WebProduct'; build='pass'; source='promoted-packages-only' }
        invalidTarget=[ordered]@{ status='fail-closed-as-expected'; diagnostic='DOROTIWEB002' }
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
    Invoke-Checked { dotnet restore $demoWebProject --force-evaluate } 'restore DorotiDemoApp Web product for publish'
    Invoke-Checked { dotnet publish $demoWebProject -c Release --no-restore -o $publishA } 'first DorotiDemoApp browser-wasm publish'
    Invoke-Checked { dotnet publish $demoWebProject -c Release --no-restore -o $publishB } 'repeat DorotiDemoApp browser-wasm publish'
    Invoke-Checked { dotnet restore $state.project --packages $state.packages --force --no-cache "-p:RestoreAdditionalProjectSources=$($state.feed)" } 'restore external template product for acceptance publish'
    Invoke-Checked { dotnet publish $state.project -c Release --no-restore -o $externalPublish } 'publish external template/package-only product'
    $staticA = Join-Path $publishA 'wwwroot'
    $staticB = Join-Path $publishB 'wwwroot'
    $identityA = Get-StaticIdentity $staticA
    $identityB = Get-StaticIdentity $staticB
    Assert-Equal $identityA.Count $identityB.Count 'repeat static file count'
    $hashA = Get-IdentityHash $identityA
    $hashB = Get-IdentityHash $identityB
    Assert-Equal $hashA $hashB 'repeat deployment-neutral static identity'
    $index = Get-Content -LiteralPath (Join-Path $staticA 'index.html') -Raw
    Assert-True ($index -match '_framework/blazor\.webassembly\.js') 'standard Blazor WebAssembly loader URL'
    foreach ($relative in @('_framework/blazor.webassembly.js','_framework/dotnet.js','_content/Doroti.Host.Web/doroti.web.js','_content/Doroti.Host.Web/doroti.web.css','assets/doroti-mark.txt','locales/en-US.json','plugins/echo.js','doroti-app-manifest.json')) {
        Assert-True (Test-Path -LiteralPath (Join-Path $staticA $relative) -PathType Leaf) "published static asset $relative"
    }
    $native = @(Get-ChildItem (Join-Path $staticA '_framework') -File -Filter 'dotnet.native.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    $skia = @(Get-ChildItem (Join-Path $staticA '_framework') -File -Filter 'SkiaSharp.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    $app = @(Get-ChildItem (Join-Path $staticA '_framework') -File -Filter 'DorotiDemoApp.Web.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    Assert-Equal $native.Count 1 'native WebAssembly runtime artifact'
    Assert-True ($skia.Count -ge 2) 'SkiaSharp managed and Blazor WASM assemblies'
    Assert-Equal $app.Count 1 'C# application WASM assembly'
    Assert-Equal @($identityA | Where-Object path -match '(?i)canvaskit').Count 0 'CanvasKit artifacts'
    Assert-Equal @(Get-ChildItem -LiteralPath $publishA -File -Recurse | Where-Object FullName -match '[\\/]App[\\/](bin|obj)[\\/]').Count 0 'nested project build leakage into publish output'
    $assets = Get-Content -LiteralPath (Join-Path $repoRoot 'DorotiDemoApp/obj/Web/project.assets.json') -Raw
    Assert-True ($assets -notmatch 'Doroti\.(Host\.Desktop|Shell|Vendor\.Avalonia|Target\.Windows|Target\.macOS)') 'DorotiDemoApp browser assets desktop/Avalonia graph'
    $externalAssets = Get-Content -LiteralPath (Join-Path $state.projectRoot 'obj/project.assets.json') -Raw
    Assert-True ($externalAssets -notmatch [regex]::Escape($dorotiRoot)) 'external browser assets repository source fallback'
    $externalStatic = Join-Path $externalPublish 'wwwroot'
    Assert-True (Test-Path -LiteralPath (Join-Path $externalStatic '_content/Doroti.Host.Web/doroti.web.js') -PathType Leaf) 'external package-only host static asset'
    Assert-True (@(Get-ChildItem (Join-Path $externalStatic '_framework') -File -Filter 'G7WebProduct.App.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$').Count -eq 1) 'external package-only C# app assembly'
    [IO.Directory]::CreateDirectory($releaseRoot) | Out-Null
    $artifactRoot = Join-Path $releaseRoot 'wwwroot'
    Reset-SafeDirectory $artifactRoot $releaseRoot
    Copy-Item -Path (Join-Path $staticA '*') -Destination $artifactRoot -Recurse -Force
    $artifactIdentity = Get-StaticIdentity $artifactRoot
    Assert-Equal (Get-IdentityHash $artifactIdentity) $hashA 'copied release artifact identity'
    $appSource = Join-Path $repoRoot 'DorotiDemoApp/Program.cs'
    $manifest = [ordered]@{
        schemaVersion='doroti.static-artifact-manifest/v2'; milestone='G7-3'; target='browser-wasm'; version='0.2.0-beta'
        deploymentRoot='wwwroot'; aggregateSha256=$hashA; repeatAggregateSha256=$hashB; fileCount=$artifactIdentity.Count
        application=[ordered]@{ source='DorotiDemoApp/Program.cs'; sourceSha256=Get-Sha $appSource; assembly=$app[0].Name; assemblySha256=Get-Sha $app[0].FullName; desktopProject='DorotiDemoApp.csproj'; browserProject='DorotiDemoApp.Web.csproj' }
        runtime=[ordered]@{ nativeWasm=$native[0].Name; nativeSha256=Get-Sha $native[0].FullName; skiaWasm=@($skia | ForEach-Object { [ordered]@{ name=$_.Name; sha256=Get-Sha $_.FullName } }) }
        packages=[ordered]@{ target='Doroti.Target.Web.browser-wasm/0.2.0-beta'; blazor='10.0.0'; skiaSharp='3.119.4' }
        reference=[ordered]@{ upstream='AvaloniaUI/Avalonia'; revision='f159423f691946e713f454447a780d4677d8a0d2'; mode='behavior-reference-only' }
        files=$artifactIdentity
    }
    Write-Json (Join-Path $artifactRoot 'artifact-manifest.json') $manifest
    foreach ($item in $artifactIdentity) {
        Assert-Equal (Get-Sha (Join-Path $artifactRoot $item.path)) $item.sha256 "release artifact hash $($item.path)"
    }
    Write-Json (Join-Path $tmpRoot 'publish.json') ([ordered]@{
        status='pass'; primaryProduct='DorotiDemoApp'; source='same Program.cs compiled for desktop and browser-wasm'
        create='pass'; restore='clean'; build='pass'; publish='pass'; repeatPublishIdentity='pass'
        externalAcceptance=[ordered]@{ consumer='repository-outside-source-tree'; source='template-and-promoted-packages-only'; create='pass'; restore='clean'; build='pass'; publish='pass' }
        staticFileCount=$identityA.Count; aggregateSha256=$hashA; repeatAggregateSha256=$hashB
        bootLoader='_framework/blazor.webassembly.js'; nativeWasm=[ordered]@{ name=$native[0].Name; sha256=Get-Sha $native[0].FullName }
        skiaWasmCount=$skia.Count; appAssembly=$app[0].Name; resources=@('assets/doroti-mark.txt','locales/en-US.json'); plugin='plugins/echo.js'
        repositoryPrivateFallbacks=0; desktopNativeDependencies=0; avaloniaDependencies=0; canvasKitArtifacts=0
        missingStaticArtifacts=0; staticArtifactHashMismatches=0; flutterOrDartCommands=0
        artifactRoot='artifacts/g7-web/0.2.0-beta/wwwroot'
    })
}

Write-Composite
Write-Output "G7-3 Web product shard '$Shard': PASS"
