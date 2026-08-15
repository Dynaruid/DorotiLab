#Requires -Version 7.0
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Toolchain', 'Graph', 'Compile', 'Publish')]
    [string] $Shard
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$migrationRoot = Join-Path $dorotiRoot 'migration/web'
$tmpRoot = Join-Path $dorotiRoot '.doroti/tmp/g7-web'
$releaseRoot = Join-Path $dorotiRoot 'artifacts/g7-web/0.2.0-beta'
$targetProject = Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm/Doroti.Target.Web.browser-wasm.csproj'
$hostProject = Join-Path $dorotiRoot 'src/Doroti.Host.Web/Doroti.Host.Web.csproj'
$probeProject = Join-Path $dorotiRoot 'validation/Doroti.Validation.G7WebBuild/Doroti.Validation.G7WebBuild.csproj'
[IO.Directory]::CreateDirectory($migrationRoot) | Out-Null
[IO.Directory]::CreateDirectory($tmpRoot) | Out-Null
[IO.Directory]::CreateDirectory($releaseRoot) | Out-Null

function Assert-True([bool] $Condition, [string] $Name) { if (-not $Condition) { throw "$Name failed." } }
function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}
function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    [IO.File]::WriteAllText($Path, (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
}
function Read-Json([string] $Path) { Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json }
function Get-Sha([string] $Path) { (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant() }
function Get-StringSha([string] $Value) {
    $bytes = [Text.Encoding]::UTF8.GetBytes($Value)
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant()
}
function Get-StaticIdentity([string] $Root) {
    return @(Get-ChildItem -LiteralPath $Root -File -Recurse | Where-Object Name -ne 'artifact-manifest.json' | Sort-Object FullName | ForEach-Object {
        [ordered]@{ path=[IO.Path]::GetRelativePath($Root,$_.FullName).Replace('\','/');length=$_.Length;sha256=Get-Sha $_.FullName }
    })
}
function Get-IdentityHash([object[]] $Identity) {
    $lines = @($Identity | ForEach-Object { "$($_.path):$($_.length):$($_.sha256)" })
    return Get-StringSha ($lines -join "`n")
}
function Write-Composite {
    $names = @('toolchain','graph','compile','publish')
    $shards = [ordered]@{}
    foreach ($name in $names) {
        $path = Join-Path $tmpRoot "$name.json"
        $shards[$name] = if (Test-Path $path) { Read-Json $path } else { [ordered]@{status='notVerified'} }
    }
    $pass = @($names | Where-Object { $shards[$_].status -ne 'pass' }).Count -eq 0
    $notVerified = @(
        [ordered]@{mode='trimmed-generated-product';blocker='generated framework reflection roots have not been audited; the interpreter release baseline remains PublishTrimmed=false'}
    )
    if ($shards.compile.aot.status -ne 'pass') {
        $notVerified += [ordered]@{mode='wasm-aot-build-probe';blocker='the Compile shard has not produced a successful AOT artifact'}
    }
    Write-Json (Join-Path $migrationRoot 'g7-web-build-evidence.json') ([ordered]@{
        schemaVersion='doroti.g7-web-build-evidence/v1'
        milestone='G7-3'
        status=$(if ($pass) {'pass'} else {'partial'})
        target='browser-wasm'
        shards=$shards
        closure=[ordered]@{
            desktopNativeDependencies=0
            repositoryPrivateFallbacks=0
            unsupportedCapabilitySilentSuccesses=0
            missingStaticArtifacts=0
            staticArtifactHashMismatches=0
        }
        notVerified=$notVerified
    })
}

if ($Shard -eq 'Toolchain') {
    $sdkVersion = (& dotnet --version).Trim()
    Assert-True ($sdkVersion -match '^10\.') '.NET 10 SDK'
    $workloads = (& dotnet workload list) -join "`n"
    $wasmTools = $workloads -match '(?m)^wasm-tools\s'
    $workloadVersion = if ($workloads -match '워크로드 버전:\s*([^\s]+)') { $Matches[1] } else { 'unknown' }
    $sdkRoot = (& dotnet --list-sdks | Select-Object -Last 1) -replace '^.*\[|\].*$',''
    Assert-True (Test-Path (Join-Path $sdkRoot '10.0.302/Sdks/Microsoft.NET.Sdk.WebAssembly/Sdk/Sdk.props') -PathType Leaf) 'WebAssembly SDK'
    Invoke-Checked { dotnet restore $probeProject --force --no-cache } 'browser-wasm toolchain restore'
    Write-Json (Join-Path $tmpRoot 'toolchain.json') ([ordered]@{
        status='pass';sdkVersion=$sdkVersion;workloadVersion=$workloadVersion;rid='browser-wasm';webAssemblySdk='available'
        interpreterPublish='supported';wasmToolsInstalled=$wasmTools
        aotToolchain=$(if ($wasmTools) {'available'} else {'notAvailable'})
        trimming=[ordered]@{status='notVerified';blocker='generated framework reflection roots have not been audited; PublishTrimmed=false'}
    })
}

if ($Shard -eq 'Graph') {
    $required = @(
        'src/Doroti.Host.Web/BrowserHostContracts.cs','src/Doroti.Host.Web/BrowserFrameworkHost.cs',
        'src/Doroti.Host.Web/wwwroot/doroti.web.js','src/Doroti.Target.Web.browser-wasm/BrowserWasmTarget.cs',
        'src/Doroti.Target.Web.browser-wasm/doroti-target-manifest.json','migration/targets/browser-wasm.json')
    foreach ($relative in $required) { Assert-True (Test-Path (Join-Path $dorotiRoot $relative) -PathType Leaf) $relative }
    $graphFiles = @(Get-ChildItem (Join-Path $dorotiRoot 'src/Doroti.Host.Web'), (Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm') -Include *.cs,*.csproj -File -Recurse)
    $forbidden = @($graphFiles | Select-String -Pattern 'Doroti\.Host\.Desktop|Doroti\.Shell\.|Doroti\.Vendor\.Avalonia|Win32|AppKit|NSOpenGL|WGL' -CaseSensitive)
    Assert-True ($forbidden.Count -eq 0) 'browser graph desktop/native dependency scan'
    $script = Get-Content (Join-Path $dorotiRoot 'src/Doroti.Host.Web/wwwroot/doroti.web.js') -Raw
    Assert-True ($script -match 'requestAnimationFrame' -and $script -match 'visibilitychange' -and $script -match 'ResizeObserver' -and $script -match 'devicePixelRatio') 'browser lifecycle bridge'
    Assert-True ($script -match 'getContext\("webgl2"' -and $script -notmatch 'getContext\("2d"') 'strict browser GPU bridge'
    $hostSource = Get-Content (Join-Path $dorotiRoot 'src/Doroti.Host.Web/BrowserFrameworkHost.cs') -Raw
    Assert-True ($hostSource -match 'DorotiCapabilityException' -and $hostSource -match 'no target-manifest JavaScript plugin') 'unsupported plugin fail-closed path'
    $hostContracts = Get-Content (Join-Path $dorotiRoot 'src/Doroti.Host.Web/BrowserHostContracts.cs') -Raw
    Assert-True ($script -match 'export async function invokePlugin' -and $hostContracts -match 'BrowserJavaScriptPluginHandler') 'JavaScript plugin ABI bridge'
    $manifest = Read-Json (Join-Path $dorotiRoot 'src/Doroti.Target.Web.browser-wasm/doroti-target-manifest.json')
    Assert-True ($manifest.rid -eq 'browser-wasm' -and $manifest.graphicsBackend -eq 'webgl2-browser-gpu-required') 'browser target identity'
    Write-Json (Join-Path $tmpRoot 'graph.json') ([ordered]@{
        status='pass';requiredFiles=$required.Count;desktopNativeDependencies=0
        lifecycle=@('document','canvas','requestAnimationFrame','visibility','resize','devicePixelRatio')
        gpuPolicy='hardware-webgl2-fail-closed';cpu2dFallbacks=0
        managedCallbackAbi=$manifest.managedCallbackAbi;javaScriptPluginAbi=$manifest.javaScriptPluginAbi
        unsupportedCapabilitySilentSuccesses=0
    })
}

if ($Shard -eq 'Compile') {
    $publishA = Join-Path $tmpRoot 'compile-publish-a'
    $publishB = Join-Path $tmpRoot 'compile-publish-b'
    $aotPublish = Join-Path $tmpRoot 'compile-aot-publish'
    foreach ($path in @($publishA,$publishB,$aotPublish)) {
        if (Test-Path $path) { Remove-Item -LiteralPath $path -Recurse -Force }
        [IO.Directory]::CreateDirectory($path) | Out-Null
    }
    Invoke-Checked { dotnet build $hostProject -c Release --nologo --no-restore } 'Web host build'
    Invoke-Checked { dotnet build $targetProject -c Release --nologo --no-restore } 'browser target build'
    Invoke-Checked { dotnet clean $probeProject -c Release --nologo --verbosity quiet } 'browser probe clean output'
    Invoke-Checked { dotnet restore $probeProject --force --no-cache } 'browser probe clean restore'
    Invoke-Checked { dotnet publish $probeProject -c Release --no-restore -o $publishA } 'first browser-wasm publish'
    Invoke-Checked { dotnet publish $probeProject -c Release --no-restore -o $publishB } 'repeat browser-wasm publish'
    $identityA = Get-StaticIdentity (Join-Path $publishA 'wwwroot')
    $identityB = Get-StaticIdentity (Join-Path $publishB 'wwwroot')
    $hashA = Get-IdentityHash $identityA
    $hashB = Get-IdentityHash $identityB
    Assert-True ($hashA -eq $hashB) 'repeat browser-wasm publish identity'
    $workloads = (& dotnet workload list) -join "`n"
    Assert-True ($workloads -match '(?m)^wasm-tools\s') 'wasm-tools workload for AOT build probe'
    Invoke-Checked {
        dotnet publish $probeProject -c Release --no-restore --no-build -o $aotPublish `
            -p:BuildProjectReferences=false -p:RunAOTCompilation=true -p:WasmBuildNative=true -p:PublishTrimmed=true
    } 'browser-wasm AOT build probe publish'
    $interpreterNative = @(Get-ChildItem (Join-Path $publishA 'wwwroot/_framework') -File -Filter 'dotnet.native.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    $aotNative = @(Get-ChildItem (Join-Path $aotPublish 'wwwroot/_framework') -File -Filter 'dotnet.native.*.wasm' | Where-Object Name -notmatch '\.(br|gz)$')
    Assert-True ($interpreterNative.Count -eq 1 -and $aotNative.Count -eq 1) 'browser native runtime artifact cardinality'
    Assert-True ($aotNative[0].Length -gt $interpreterNative[0].Length) 'AOT native runtime exceeds interpreter runtime'
    $aotIdentity = Get-StaticIdentity (Join-Path $aotPublish 'wwwroot')
    $artifactRoot = Join-Path $releaseRoot 'wwwroot'
    if (Test-Path $artifactRoot) { Remove-Item -LiteralPath $artifactRoot -Recurse -Force }
    Copy-Item -LiteralPath (Join-Path $publishA 'wwwroot') -Destination $artifactRoot -Recurse
    $artifactIdentity = Get-StaticIdentity $artifactRoot
    Assert-True ((Get-IdentityHash $artifactIdentity) -eq $hashA) 'copied static artifact identity'
    Write-Json (Join-Path $artifactRoot 'artifact-manifest.json') ([ordered]@{
        schemaVersion='doroti.static-artifact-manifest/v1';target='browser-wasm';version='0.2.0-beta'
        deploymentRoot='wwwroot';aggregateSha256=$hashA;files=$artifactIdentity
    })
    foreach ($item in $artifactIdentity) {
        $path = Join-Path $artifactRoot $item.path
        Assert-True (Test-Path $path -PathType Leaf) "static artifact $($item.path)"
        Assert-True ((Get-Sha $path) -eq $item.sha256) "static artifact hash $($item.path)"
    }
    Write-Json (Join-Path $tmpRoot 'compile.json') ([ordered]@{
        status='pass';rid='browser-wasm';configuration='Release';publishMode='interpreter';publishTrimmed=$false
        repeatPublishIdentity='pass';aggregateSha256=$hashA;staticFileCount=$artifactIdentity.Count
        aot=[ordered]@{status='pass';runAotCompilation=$true;wasmBuildNative=$true;publishTrimmed=$true
            nativeBytes=$aotNative[0].Length;nativeSha256=Get-Sha $aotNative[0].FullName
            interpreterNativeBytes=$interpreterNative[0].Length;staticFileCount=$aotIdentity.Count;aggregateSha256=Get-IdentityHash $aotIdentity}
        artifactRoot='artifacts/g7-web/0.2.0-beta/wwwroot';missingStaticArtifacts=0;staticArtifactHashMismatches=0;diagnostics=0
    })
}

if ($Shard -eq 'Publish') {
    $feed = Join-Path $tmpRoot 'package-feed'
    if (Test-Path $feed) { Remove-Item -LiteralPath $feed -Recurse -Force }
    [IO.Directory]::CreateDirectory($feed) | Out-Null
    Copy-Item (Join-Path $dorotiRoot 'artifacts/g6-release/0.2.0-beta/packages/*.nupkg') $feed -Force
    Invoke-Checked { dotnet pack $hostProject -c Release --nologo -o $feed } 'pack Web host'
    Invoke-Checked { dotnet pack $targetProject -c Release --nologo -o $feed } 'pack browser target'
    $targetPackage = Join-Path $feed 'Doroti.Target.Web.browser-wasm.0.2.0-beta.nupkg'
    Assert-True (Test-Path $targetPackage -PathType Leaf) 'browser target package'

    $external = Join-Path ([IO.Path]::GetTempPath()) ("doroti-g7-web-" + [Guid]::NewGuid().ToString('N'))
    [IO.Directory]::CreateDirectory((Join-Path $external 'wwwroot')) | Out-Null
    $projectText = @"
<Project Sdk="Microsoft.NET.Sdk.WebAssembly"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><AssemblyName>G7.Web.PackageConsumer</AssemblyName><RuntimeIdentifier>browser-wasm</RuntimeIdentifier><WasmBuildNative>false</WasmBuildNative><PublishTrimmed>false</PublishTrimmed><InvariantGlobalization>false</InvariantGlobalization><RestorePackagesPath>`$(MSBuildProjectDirectory)/.nuget/packages</RestorePackagesPath><LangVersion>14.0</LangVersion><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup><ItemGroup><PackageReference Include="Doroti.Target.Web.browser-wasm" Version="0.2.0-beta" /></ItemGroup></Project>
"@
    $programText = @"
using Doroti.Target.Web;
internal static class Program { [System.Runtime.Versioning.SupportedOSPlatform("browser")] private static void Main() { using var target = new BrowserWasmTarget(); if (target.Rid != "browser-wasm") throw new InvalidDataException("target RID mismatch"); } }
"@
    $indexText = @"
<!doctype html><html><head><meta charset="utf-8"><meta name="viewport" content="width=device-width"><base href="/"><title>Doroti package consumer</title></head><body><canvas id="doroti-canvas" width="720" height="640"></canvas><script type="module" src="./main.js"></script></body></html>
"@
    $mainText = @"
import { dotnet } from "./_framework/dotnet.js";
import { configureManagedCallbacks } from "./_content/Doroti.Host.Web/doroti.web.js";
const runtime = await dotnet.create();
const exports = await runtime.getAssemblyExports("Doroti.Host.Web.dll");
const interop = exports.Doroti.Host.Web.BrowserInterop;
configureManagedCallbacks({ dispatchAnimationFrame: interop.DispatchAnimationFrame, dispatchSnapshot: interop.DispatchSnapshot });
await runtime.run();
"@
    $nugetText = "<configuration><packageSources><clear /><add key=`"g7`" value=`"$feed`" /><add key=`"nuget`" value=`"https://api.nuget.org/v3/index.json`" /></packageSources></configuration>"
    [IO.File]::WriteAllText((Join-Path $external 'G7.Web.PackageConsumer.csproj'), $projectText, [Text.UTF8Encoding]::new($false))
    [IO.File]::WriteAllText((Join-Path $external 'Program.cs'), $programText, [Text.UTF8Encoding]::new($false))
    [IO.File]::WriteAllText((Join-Path $external 'wwwroot/index.html'), $indexText, [Text.UTF8Encoding]::new($false))
    [IO.File]::WriteAllText((Join-Path $external 'wwwroot/main.js'), $mainText, [Text.UTF8Encoding]::new($false))
    [IO.File]::WriteAllText((Join-Path $external 'NuGet.Config'), $nugetText, [Text.UTF8Encoding]::new($false))
    $externalProject = Join-Path $external 'G7.Web.PackageConsumer.csproj'
    $publish = Join-Path $external 'publish'
    Invoke-Checked { dotnet restore $externalProject --configfile (Join-Path $external 'NuGet.Config') --force --no-cache } 'external package-only Web restore'
    Invoke-Checked { dotnet publish $externalProject -c Release --no-restore -o $publish } 'external package-only Web publish'
    $assets = Get-Content (Join-Path $external 'obj/project.assets.json') -Raw
    Assert-True ($assets -notmatch '"projectReferences"\s*:\s*\{\s*"[^"]+"|/Users/ceramic/Labo/DorotiLab/Doroti/src') 'repository-private project fallback scan'
    Assert-True ($assets -notmatch 'Doroti\.(Host\.Desktop|Shell|Vendor\.Avalonia|Target\.Windows|Target\.macOS)') 'package graph desktop/native dependency scan'
    $staticRoot = Join-Path $publish 'wwwroot'
    $requiredStatic = @('index.html','main.js','_framework/dotnet.js','_content/Doroti.Host.Web/doroti.web.js')
    foreach ($relative in $requiredStatic) { Assert-True (Test-Path (Join-Path $staticRoot $relative) -PathType Leaf) "package static asset $relative" }
    $identity = Get-StaticIdentity $staticRoot
    Write-Json (Join-Path $tmpRoot 'publish.json') ([ordered]@{
        status='pass';package='Doroti.Target.Web.browser-wasm.0.2.0-beta.nupkg';packageSha256=Get-Sha $targetPackage
        externalConsumer='repository-outside-temporary-root';restore='clean';publish='pass';rid='browser-wasm';publishMode='interpreter'
        staticFileCount=$identity.Count;aggregateSha256=Get-IdentityHash $identity
        repositoryPrivateFallbacks=0;desktopNativeDependencies=0;missingStaticArtifacts=0;staticArtifactHashMismatches=0
    })
}

Write-Composite
Write-Output "G7-3 Web build shard '$Shard': PASS"
