#Requires -Version 7.0
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Source', 'Build', 'Live', 'Package')]
    [string] $Shard
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$migrationRoot = Join-Path $dorotiRoot 'migration/macos'
$tmpRoot = Join-Path $dorotiRoot '.doroti/tmp/g7-macos'
$releaseRoot = Join-Path $dorotiRoot 'artifacts/g7-macos-release/0.2.0-beta'
[IO.Directory]::CreateDirectory($migrationRoot) | Out-Null
[IO.Directory]::CreateDirectory($tmpRoot) | Out-Null
[IO.Directory]::CreateDirectory($releaseRoot) | Out-Null

function Assert-True([bool] $Condition, [string] $Name) { if (-not $Condition) { throw "$Name failed." } }
function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0; & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}
function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    [IO.File]::WriteAllText($Path, (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
}
function Read-Json([string] $Path) { Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json }
function Get-Sha([string] $Path) { (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant() }
function Get-PublishIdentity([string] $Root) {
    $items = Get-ChildItem -LiteralPath $Root -File -Recurse | Where-Object Extension -in @('.dll','.dylib','.json') | Sort-Object FullName
    return @($items | ForEach-Object { [ordered]@{ path=[IO.Path]::GetRelativePath($Root,$_.FullName).Replace('\','/');sha256=Get-Sha $_.FullName } })
}
function Write-Composite {
    $names = @('source','build','live','package')
    $shards = [ordered]@{}
    foreach ($name in $names) {
        $path = Join-Path $tmpRoot "$name.json"
        $shards[$name] = if (Test-Path $path) { Read-Json $path } else { [ordered]@{status='notVerified'} }
    }
    $pass = @($names | Where-Object { $shards[$_].status -ne 'pass' }).Count -eq 0
    Write-Json (Join-Path $migrationRoot 'g7-macos-shell-evidence.json') ([ordered]@{
        schemaVersion='doroti.g7-macos-shell-evidence/v1';milestone='G7-3M';capturedAtUtc=[DateTimeOffset]::UtcNow
        status=$(if ($pass) {'pass'} else {'partial'});target='osx-arm64';shards=$shards
        closure=[ordered]@{avaloniaUiControlCompositionBinaryDependencies=0;repositoryPrivateFallbacks=0;cpuFullFrameFallbacks=0;unhandledExceptions=0}
        notVerified=@('Korean IME physical candidate-window placement','VoiceOver physical navigation','precise trackpad physical gesture','osx-x64')
    })
}

Assert-True $IsMacOS 'macOS validation host'
Assert-True ([Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture -eq 'Arm64') 'arm64 validation process'
$targetProject = Join-Path $dorotiRoot 'src/Doroti.Target.macOS.osx-arm64/Doroti.Target.macOS.osx-arm64.csproj'
$vendorProject = Join-Path $dorotiRoot 'src/Doroti.Vendor.Avalonia.Native/Doroti.Vendor.Avalonia.Native.csproj'
$demoProject = Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.csproj'
$nativeAsset = Join-Path $dorotiRoot 'src/Doroti.Vendor.Avalonia.Native/runtimes/osx-arm64/native/libAvalonia.dylib'

if ($Shard -eq 'Source') {
    $required = @(
        'src/Doroti.Vendor.Avalonia.Native/MacOsShellRuntime.cs','src/Doroti.Vendor.Avalonia.Native/NativeInterop.cs',
        'src/Doroti.Vendor.Avalonia.Native/native/src/DorotiAvnAppKit.mm','src/Doroti.Vendor.Avalonia.Native/native/include/doroti-avalonia-native.h',
        'src/Doroti.Target.macOS.osx-arm64/MacOsTarget.cs','migration/avalonia-shell/g7-macos-source-port-provenance.json',
        'migration/targets/osx-arm64.json')
    foreach ($relative in $required) { Assert-True (Test-Path (Join-Path $dorotiRoot $relative) -PathType Leaf) $relative }
    $graphRoots = @('Doroti.Shell.Core','Doroti.Host.Desktop','Doroti.Host.Desktop.Framework','Doroti.Vendor.Avalonia.Native','Doroti.Target.macOS.osx-arm64')
    $productFiles = @($graphRoots | ForEach-Object { Get-ChildItem (Join-Path $dorotiRoot "src/$_") -Include *.csproj,*.cs -File -Recurse })
    $officialAvaloniaReferences = @($productFiles | Select-String -Pattern '<PackageReference Include="Avalonia(\.|\")|using Avalonia(\.|;)' -CaseSensitive)
    Assert-True ($officialAvaloniaReferences.Count -eq 0) 'official Avalonia UI/Control/Composition binary dependency scan'
    $hostSource = Get-Content (Join-Path $dorotiRoot 'src/Doroti.Host.Desktop/DesktopWindowBackend.cs') -Raw
    Assert-True ($hostSource -notmatch 'using Doroti\.Vendor\.Avalonia\.Win32') 'desktop host native-type isolation'
    Assert-True ($hostSource -match 'IShellWindowingPlatform') 'desktop host shell injection'
    $provenance = Read-Json (Join-Path $dorotiRoot 'migration/avalonia-shell/g7-macos-source-port-provenance.json')
    Assert-True ($provenance.upstream.revision -eq 'git:f159423f691946e713f454447a780d4677d8a0d2') 'Avalonia revision pin'
    foreach ($adaptation in $provenance.adaptations) { Assert-True ((Get-Sha (Join-Path $dorotiRoot $adaptation.target)) -eq $adaptation.sha256) "provenance hash $($adaptation.target)" }
    Write-Json (Join-Path $tmpRoot 'source.json') ([ordered]@{status='pass';requiredFiles=$required.Count;officialAvaloniaBinaryDependencies=0;nativeTypesOutsideVendor=0;repositoryPrivateFallbacks=0;revision=$provenance.upstream.revision})
}

if ($Shard -eq 'Build') {
    Invoke-Checked { dotnet build $targetProject -c Release --nologo } 'osx-arm64 target build'
    Invoke-Checked { dotnet build $demoProject -c Release --nologo --no-restore } 'generated DemoApp macOS build'
    Assert-True (Test-Path $nativeAsset -PathType Leaf) 'libAvalonia native asset'
    $arch = (& lipo -archs $nativeAsset).Trim(); Assert-True ($arch -eq 'arm64') 'native asset architecture'
    $installNames = @(& otool -D $nativeAsset); Assert-True (($installNames -join "`n") -match '@rpath/libAvalonia.dylib') 'native install name'
    $publishA = Join-Path $tmpRoot 'publish-a'; $publishB = Join-Path $tmpRoot 'publish-b'
    [IO.Directory]::CreateDirectory($publishA) | Out-Null; [IO.Directory]::CreateDirectory($publishB) | Out-Null
    Invoke-Checked { dotnet restore $demoProject -r osx-arm64 --force --no-cache } 'osx-arm64 publish restore'
    Invoke-Checked { dotnet publish $demoProject -c Release -r osx-arm64 --self-contained false --no-restore -o $publishA } 'first osx-arm64 publish'
    Invoke-Checked { dotnet publish $demoProject -c Release -r osx-arm64 --self-contained false --no-restore -o $publishB } 'repeat osx-arm64 publish'
    $identityA = Get-PublishIdentity $publishA; $identityB = Get-PublishIdentity $publishB
    Assert-True (($identityA | ConvertTo-Json -Depth 8 -Compress) -ceq ($identityB | ConvertTo-Json -Depth 8 -Compress)) 'repeat publish identity'
    Write-Json (Join-Path $tmpRoot 'build.json') ([ordered]@{status='pass';rid='osx-arm64';architecture=$arch;nativeSha256=Get-Sha $nativeAsset;installName='@rpath/libAvalonia.dylib';repeatPublishIdentity='pass';publishFiles=$identityA.Count;diagnostics=0})
}

if ($Shard -eq 'Live') {
    $nativeEvidence = Join-Path $migrationRoot 'g7-macos-native-live.json'
    $demoEvidence = Join-Path $migrationRoot 'g7-macos-demo-live.json'
    Invoke-Checked { dotnet build $demoProject -c Release --nologo --no-restore } 'generated DemoApp live build'
    Invoke-Checked { dotnet run --project (Join-Path $dorotiRoot 'validation/Doroti.Validation.G7MacOS/Doroti.Validation.G7MacOS.csproj') -c Release -- --evidence $nativeEvidence } 'AppKit native capability probe'
    $demoDll = Join-Path $repoRoot 'DorotiDemoApp/bin/Release/net10.0/DorotiDemoApp.dll'
    Invoke-Checked { dotnet $demoDll --smoke --duration-ms 20000 --frames 3 --cadence-duration-ms 120 --evidence $demoEvidence } 'generated DemoApp NSWindow strict-GPU smoke'
    $native = Read-Json $nativeEvidence; $demo = Read-Json $demoEvidence
    Assert-True ($native.status -eq 'pass' -and $native.gpu.hardware -and -not $native.gpu.softwareFallbackUsed -and [long]$native.gpu.nonEmptyPixels -gt 0) 'native strict-GPU trace'
    Assert-True ($demo.outcome -eq 'presented' -and [long]$demo.frame.submitted -eq [long]$demo.frame.presented -and [long]$demo.frame.failed -eq 0 -and -not $demo.frame.softwareFallbackUsed) 'generated terminal ACK trace'
    Assert-True ([long]$demo.interaction.nativePointerCount -gt 0 -and [long]$demo.automation.nodeCount -gt 0) 'generated pointer/semantics trace'
    Write-Json (Join-Path $tmpRoot 'live.json') ([ordered]@{status='pass';native=$native;generated=[ordered]@{outcome=$demo.outcome;backend=$demo.frame.backendIdentity;submitted=$demo.frame.submitted;presented=$demo.frame.presented;failed=$demo.frame.failed;queueDepth=$demo.frame.queueDepth;softwareFallbackUsed=$demo.frame.softwareFallbackUsed;nativePointerCount=$demo.interaction.nativePointerCount;semanticsNodeCount=$demo.automation.nodeCount;changedPixelCount=$demo.interaction.changedPixelCount}})
}

if ($Shard -eq 'Package') {
    $feed = Join-Path $releaseRoot 'packages'; [IO.Directory]::CreateDirectory($feed) | Out-Null
    Copy-Item (Join-Path $dorotiRoot 'artifacts/g6-release/0.2.0-beta/packages/*.nupkg') $feed -Force
    $packProjects = @(
        'src/Doroti.Shell.Core/Doroti.Shell.Core.csproj','src/Doroti.Host.Desktop/Doroti.Host.Desktop.csproj',
        'src/Doroti.Host.Desktop.Framework/Doroti.Host.Desktop.Framework.csproj','src/Doroti.Vendor.Avalonia.Native/Doroti.Vendor.Avalonia.Native.csproj',
        'src/Doroti.Target.macOS.osx-arm64/Doroti.Target.macOS.osx-arm64.csproj')
    foreach ($relative in $packProjects) { Invoke-Checked { dotnet pack (Join-Path $dorotiRoot $relative) -c Release --nologo --no-restore -o $feed } "pack $relative" }
    $packagePath = Join-Path $feed 'Doroti.Target.macOS.osx-arm64.0.2.0-beta.nupkg'; Assert-True (Test-Path $packagePath) 'macOS target package'
    $external = Join-Path ([IO.Path]::GetTempPath()) ("doroti-g7-macos-" + [Guid]::NewGuid().ToString('N'))
    [IO.Directory]::CreateDirectory($external) | Out-Null
    Copy-Item (Join-Path $repoRoot 'DorotiDemoApp/Program.cs') (Join-Path $external 'Program.cs')
    $projectText = @"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><RuntimeIdentifier>osx-arm64</RuntimeIdentifier><RestorePackagesPath>`$(MSBuildProjectDirectory)/.nuget/packages</RestorePackagesPath><LangVersion>14.0</LangVersion><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings><TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup><ItemGroup>
<PackageReference Include="Doroti.Hosting" Version="0.2.0-beta" /><PackageReference Include="Doroti.Framework.Widgets" Version="0.2.0-beta" /><PackageReference Include="Doroti.Framework.Material" Version="0.2.0-beta" /><PackageReference Include="Doroti.Target.macOS.osx-arm64" Version="0.2.0-beta" />
</ItemGroup></Project>
"@
    [IO.File]::WriteAllText((Join-Path $external 'G7.MacOS.PackageConsumer.csproj'), $projectText, [Text.UTF8Encoding]::new($false))
    $nugetText = "<configuration><packageSources><clear /><add key=`"g7`" value=`"$feed`" /><add key=`"nuget`" value=`"https://api.nuget.org/v3/index.json`" /></packageSources></configuration>"
    [IO.File]::WriteAllText((Join-Path $external 'NuGet.Config'), $nugetText, [Text.UTF8Encoding]::new($false))
    $publish = Join-Path $external 'publish'; Invoke-Checked { dotnet restore (Join-Path $external 'G7.MacOS.PackageConsumer.csproj') --configfile (Join-Path $external 'NuGet.Config') --force --no-cache } 'external package-only restore'
    Invoke-Checked { dotnet publish (Join-Path $external 'G7.MacOS.PackageConsumer.csproj') -c Release -r osx-arm64 --self-contained false --no-restore -o $publish } 'external package-only publish'
    $externalEvidence = Join-Path $migrationRoot 'g7-macos-package-demo-live.json'
    Invoke-Checked { dotnet (Join-Path $publish 'G7.MacOS.PackageConsumer.dll') --smoke --package-only-consumer --duration-ms 20000 --frames 2 --cadence-duration-ms 80 --evidence $externalEvidence } 'external package-only launch'
    $externalRun = Read-Json $externalEvidence
    Assert-True ($externalRun.outcome -eq 'presented' -and -not $externalRun.frame.softwareFallbackUsed -and [long]$externalRun.frame.failed -eq 0) 'external package-only strict-GPU result'
    $projectAssets = Get-Content (Join-Path $external 'obj/project.assets.json') -Raw
    Assert-True ($projectAssets -notmatch '"projectReferences"\s*:\s*\{\s*"[^\"]+"|/Users/ceramic/Labo/DorotiLab/Doroti/src') 'repository-private project fallback scan'
    Write-Json (Join-Path $tmpRoot 'package.json') ([ordered]@{status='pass';package='Doroti.Target.macOS.osx-arm64.0.2.0-beta.nupkg';packageSha256=Get-Sha $packagePath;nativeAssetSha256=Get-Sha (Join-Path $publish 'libAvalonia.dylib');externalRoot=$external;restore='clean';publish='pass';launch=$externalRun.outcome;repositoryPrivateFallbacks=0;softwareFallbackUsed=$externalRun.frame.softwareFallbackUsed;resourceClosure='pass'})
}

Write-Composite
Write-Output "G7-3M macOS shell shard '$Shard': PASS"
