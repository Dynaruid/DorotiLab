#Requires -Version 7.0
param(
    [ValidateSet('All', 'Graph', 'Build', 'Package', 'Template', 'NativeInterop', 'Live', 'WindowsLive', 'WebLive', 'AndroidLive', 'AndroidPhysical', 'Evidence')]
    [string] $Shard = 'All',
    [string] $AndroidSerial = '',
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$demoRoot = Join-Path $repoRoot 'DorotiDemoApp'
$workspacePath = Join-Path $demoRoot 'doroti-workspace.json'
$appProject = Join-Path $demoRoot 'DorotiDemoApp.csproj'
$templateRoot = Join-Path $dorotiRoot 'templates/Doroti.Templates/content/doroti-app'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/app-targets-evidence.json'
$tmpRoot = Join-Path $dorotiRoot '.doroti/tmp/app-targets'
$stampRoot = Join-Path $tmpRoot 'stamps'
[IO.Directory]::CreateDirectory($tmpRoot) | Out-Null

function Test-Shard([string] $Name) { $Shard -eq 'All' -or $Shard -eq $Name }
function Assert-True([bool] $Condition, [string] $Name) { if (-not $Condition) { throw "$Name failed." } }
function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}
function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $json = (($Value | ConvertTo-Json -Depth 32) -replace "`r`n", "`n") + "`n"
    [IO.File]::WriteAllText($Path, $json, [Text.UTF8Encoding]::new($false))
}
function Get-SourceFingerprint {
    $sourceFiles = @($appProject, (Join-Path $demoRoot 'Program.cs'), (Join-Path $demoRoot 'src/App.cs'), $workspacePath,
        (Join-Path $dorotiRoot 'src/Doroti.App.Sdk/Sdk/Sdk.targets'), (Join-Path $dorotiRoot 'src/Doroti.Runner.Sdk/Sdk/Sdk.targets'),
        (Join-Path $dorotiRoot 'src/Doroti.Hosting/DorotiNativePlatformBridge.cs'))
    $sourceFiles += @(Get-ChildItem -LiteralPath $demoRoot -Recurse -File | Where-Object {
        $_.FullName -match '[\\/](android|ios|macos)[\\/](binding|native)[\\/]' -and
        $_.FullName -notmatch '[\\/](bin|obj|build|\.gradle|DerivedData|xcuserdata)[\\/]' -and
        $_.Extension -notin @('.aar', '.xcframework')
    } | Select-Object -ExpandProperty FullName)
    $sourceFiles += @(Get-ChildItem -LiteralPath $templateRoot -Recurse -File | Where-Object {
        $_.FullName -notmatch '[\\/](bin|obj|build|\.gradle|DerivedData|xcuserdata)[\\/]'
    } | Select-Object -ExpandProperty FullName)
    $sourceFiles = @($sourceFiles | Sort-Object -Unique)
    $input = ($sourceFiles | ForEach-Object { "$_=$((Get-FileHash -LiteralPath $_ -Algorithm SHA256).Hash.ToLowerInvariant())" }) -join "`n"
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($input))).ToLowerInvariant()
}
function Write-ShardStamp([string] $Name) {
    Write-Json (Join-Path $stampRoot "$Name.json") ([ordered]@{
        schemaVersion = 'doroti.validation-stamp/v1'
        shard = $Name
        sourceFingerprint = Get-SourceFingerprint
        capturedAt = [DateTimeOffset]::UtcNow.ToString('O')
        status = 'pass'
    })
}
function Get-ShardStatus([string] $Name, [string] $Fingerprint) {
    $path = Join-Path $stampRoot "$Name.json"
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { return 'notVerified' }
    $stamp = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
    if ([string]$stamp.schemaVersion -ne 'doroti.validation-stamp/v1' -or [string]$stamp.shard -ne $Name -or
        [string]$stamp.sourceFingerprint -ne $Fingerprint -or [string]$stamp.status -ne 'pass') { return 'notVerified' }
    return 'pass'
}

function Read-Workspace([string] $Path) {
    $workspace = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json -AsHashtable
    Assert-True ($workspace.schemaVersion -ceq 'doroti.workspace/v1') 'workspace schema version'
    Assert-True ($workspace.applicationProject -ceq 'DorotiDemoApp.csproj') 'workspace application project'
    $aliases = @($workspace.platforms.Keys)
    $expected = @('android', 'ios', 'linux', 'macos', 'maccatalyst', 'web', 'windows')
    Assert-True ($aliases.Count -eq 7) 'workspace alias count'
    foreach ($alias in $expected) {
        Assert-True ($aliases -ccontains $alias) "workspace alias $alias"
        $relative = [string]$workspace.platforms[$alias]
        $candidate = [IO.Path]::GetFullPath((Join-Path $demoRoot $relative))
        $allowed = [IO.Path]::GetFullPath($demoRoot).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
        Assert-True ($candidate.StartsWith($allowed, [StringComparison]::OrdinalIgnoreCase)) "workspace path confinement $alias"
        Assert-True (Test-Path -LiteralPath $candidate -PathType Leaf) "workspace runner exists $alias"
    }
    return $workspace
}
function Get-Runner([hashtable] $Workspace, [string] $Alias) {
    [IO.Path]::GetFullPath((Join-Path $demoRoot ([string]$Workspace.platforms[$Alias])))
}
function Get-Graph([string] $Project, [string] $Rid) {
    Invoke-Checked { dotnet msbuild $Project -t:WriteDorotiTargetGraph -p:RuntimeIdentifier=$Rid -nologo } "target graph failed for $Project ($Rid)"
    $objRoot = Join-Path (Split-Path $Project -Parent) 'obj'
    $matchingGraph = @(Get-ChildItem -LiteralPath $objRoot -Recurse -File -Filter 'doroti-target-graph.txt' |
        Sort-Object LastWriteTimeUtc -Descending | Where-Object {
            @(Get-Content -LiteralPath $_.FullName) -ccontains "rid=$Rid"
        } | Select-Object -First 1)
    Assert-True ($matchingGraph.Count -eq 1) "target graph output $Project"
    return @(Get-Content -LiteralPath $matchingGraph[0].FullName)
}

function Invoke-GraphGate {
    $workspace = Read-Workspace $workspacePath
    $program = Get-Content -LiteralPath (Join-Path $demoRoot 'Program.cs') -Raw
    $appSdkTargets = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.App.Sdk/Sdk/Sdk.targets') -Raw
    $runnerSdkTargets = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Runner.Sdk/Sdk/Sdk.targets') -Raw
    Assert-True ($program -match 'public sealed class Program : IDorotiApplicationStartup') 'target-neutral startup type'
    Assert-True ($program -notmatch '#if|Maui|Blazor|UIKit|Android\.|WinUI|Qt') 'startup platform neutrality'
    Assert-True ($appSdkTargets -match 'DOROTIAPP100') 'legacy single-project command diagnostic'
    foreach ($code in 201..210) { Assert-True ($runnerSdkTargets -match "DOROTIAPP$code") "runner diagnostic DOROTIAPP$code" }
    foreach ($code in 301..310) { Assert-True ($runnerSdkTargets -match "DOROTIRUNNER$code") "native binding diagnostic DOROTIRUNNER$code" }
    $demoLegacyFiles = if (Test-Path -LiteralPath (Join-Path $demoRoot 'Platforms')) {
        @(Get-ChildItem -LiteralPath (Join-Path $demoRoot 'Platforms') -Recurse -File)
    } else { @() }
    Assert-True ($demoLegacyFiles.Count -eq 0) 'DemoApp legacy Platforms source absence'

    $templateProjects = @(Get-ChildItem -LiteralPath $templateRoot -Recurse -File -Filter '*.csproj' | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
    Assert-True ($templateProjects.Count -eq 12) 'template app plus seven runner and four binding projects'
    $templateLegacyFiles = if (Test-Path -LiteralPath (Join-Path $templateRoot 'Platforms')) {
        @(Get-ChildItem -LiteralPath (Join-Path $templateRoot 'Platforms') -Recurse -File)
    } else { @() }
    Assert-True ($templateLegacyFiles.Count -eq 0) 'template legacy Platforms source absence'
    Assert-True ((Get-Content -LiteralPath (Join-Path $templateRoot 'doroti-workspace.json') -Raw) -match 'doroti.workspace/v1') 'template workspace manifest'
    $templateXaml = @(Get-ChildItem -LiteralPath $templateRoot -Recurse -File -Filter '*.xaml' | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
    Assert-True ($templateXaml.Count -eq 1 -and $templateXaml[0].Name -ceq 'App.xaml' -and $templateXaml[0].Directory.Name -ceq 'windows') 'template XAML boundary'

    foreach ($root in @($demoRoot, $templateRoot)) {
        $appKitRunner = @(Get-ChildItem -LiteralPath (Join-Path $root 'macos') -File -Filter '*.MacOS.csproj' | Select-Object -First 1)
        $appKitSources = @($appKitRunner[0].FullName, (Join-Path $root 'macos/AppKitDelegate.cs'),
            (Join-Path $root 'macos/DorotiNativePlatformBridge.MacOS.cs'),
            @(Get-ChildItem -LiteralPath (Join-Path $root 'macos/binding') -File -Filter '*.MacOS.Native.csproj' | Select-Object -ExpandProperty FullName -First 1))
        $appKitText = ($appKitSources | ForEach-Object { Get-Content -LiteralPath $_ -Raw }) -join "`n"
        Assert-True ($appKitText -notmatch 'MacCatalyst|UIKit') "$root AppKit product source isolation"

        $catalystSources = @((Get-ChildItem -LiteralPath (Join-Path $root 'macos') -File -Filter '*.MacCatalyst.csproj' | Select-Object -ExpandProperty FullName -First 1),
            (Join-Path $root 'macos/AppDelegate.cs'), (Join-Path $root 'macos/DorotiNativePlatformBridge.cs'),
            @(Get-ChildItem -LiteralPath (Join-Path $root 'macos/binding') -File -Filter '*.MacCatalyst.Native.csproj' | Select-Object -ExpandProperty FullName -First 1))
        $catalystText = ($catalystSources | ForEach-Object { Get-Content -LiteralPath $_ -Raw }) -join "`n"
        Assert-True ($catalystText -notmatch 'Microsoft\.Maui\.Platforms\.MacOS|AppKit-Main|net10\.0-macos') "$root Catalyst product source isolation"
    }

    foreach ($root in @($demoRoot, $templateRoot)) {
        $shader = Join-Path $root 'assets/shaders/aurora.sksl'
        Assert-True (Test-Path -LiteralPath $shader -PathType Leaf) "$root shared shader"
        $hash = (Get-FileHash -LiteralPath $shader -Algorithm SHA256).Hash.ToLowerInvariant()
        Assert-True ($hash -ceq '658695aca71aa8cdc9e776e037ea404c29c5759a481d9955c9f21e4a1e664e41') "$root shader integrity"
        foreach ($manifest in Get-ChildItem -LiteralPath $root -Recurse -File -Filter 'application-manifest*.json' | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }) {
            $text = Get-Content -LiteralPath $manifest.FullName -Raw
            Assert-True ($text -match 'Doroti\.Shaders\.aurora\.sksl') "$($manifest.FullName) shader registration"
        }
    }

    $cases = @(
        @{ Alias='windows'; Rid='win-x64'; Target='Windows'; Host='Maui'; Entry='WinUI-Xaml' },
        @{ Alias='web'; Rid='browser-wasm'; Target='Web'; Host='BlazorWebAssembly'; Entry='Managed-Main' },
        @{ Alias='android'; Rid='android-arm64'; Target='Android'; Host='Maui'; Entry='Android-Application' },
        @{ Alias='android'; Rid='android-x64'; Target='Android'; Host='Maui'; Entry='Android-Application' },
        @{ Alias='macos'; Rid='osx-arm64'; Target='macOS'; Host='Maui'; Entry='AppKit-Main' },
        @{ Alias='maccatalyst'; Rid='maccatalyst-arm64'; Target='MacCatalyst'; Host='Maui'; Entry='UIKit-Main' },
        @{ Alias='ios'; Rid='iossimulator-x64'; Target='iOS'; Host='Maui'; Entry='UIKit-Main' },
        @{ Alias='ios'; Rid='ios-arm64'; Target='iOS'; Host='Maui'; Entry='UIKit-Main' },
        @{ Alias='linux'; Rid='linux-x64'; Target='Linux'; Host='Qt'; Entry='Qt-Main' }
    )
    foreach ($case in $cases) {
        $runner = Get-Runner $workspace $case.Alias
        $graph = Get-Graph $runner $case.Rid
        Assert-True ($graph -ccontains "runnerProject=$runner") "$($case.Alias) graph runner"
        Assert-True ($graph -ccontains "target=$($case.Target)") "$($case.Alias) graph target"
        Assert-True ($graph -ccontains "host=$($case.Host)") "$($case.Alias) graph host"
        Assert-True ($graph -ccontains "entry=$($case.Entry)") "$($case.Alias) graph entry"
        Assert-True ($graph -ccontains "rid=$($case.Rid)") "$($case.Alias) graph RID"
        Assert-True (@($graph | Where-Object { $_ -like 'generatedBootstrap=*obj*DorotiBootstrap.g.cs' }).Count -eq 1) "$($case.Alias) generated bootstrap isolation"
        if ($case.Alias -in @('android', 'ios', 'macos', 'maccatalyst')) {
            Assert-True (@($graph | Where-Object { $_ -like 'binding=*binding*.Native.csproj' }).Count -eq 1) "$($case.Alias) binding graph"
            Assert-True (@($graph | Where-Object { $_ -like 'nativeProject=*native*' }).Count -eq 1) "$($case.Alias) native project graph"
        }
    }

    Invoke-Checked { dotnet run --project (Join-Path $dorotiRoot 'validation/app-bootstrap/descriptor-contract/DescriptorContract.csproj') -c Release --nologo } 'application descriptor contract failed'
    Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'validation/app-runner/synthetic-runner/SyntheticRunner.csproj') -c Release --nologo } 'synthetic runner failed'
    $legacyOutput = @(& dotnet build $appProject -p:DorotiTarget=Windows -nologo 2>&1 | ForEach-Object ToString)
    Assert-True ($LASTEXITCODE -ne 0 -and ($legacyOutput -join "`n") -match 'DOROTIAPP100') 'legacy single-project actionable failure'
    Write-ShardStamp 'Graph'
    Write-Output 'Doroti runner graph contract: PASS'
}

function Invoke-BuildGate {
    $workspace = Read-Workspace $workspacePath
    Invoke-Checked { dotnet build $appProject -c Release --nologo } 'platform-neutral application build failed'
    Invoke-Checked { dotnet build (Get-Runner $workspace 'windows') -c Release --nologo } 'Windows runner build failed'
    Invoke-Checked { dotnet build (Get-Runner $workspace 'web') -c Release --nologo } 'Web runner build failed'
    Invoke-Checked { dotnet build (Get-Runner $workspace 'windows') -c Release --no-restore --nologo } 'Windows no-restore repeat build failed'
    Write-ShardStamp 'Build'
    Write-Output 'Doroti fast runner build gate: PASS'
}

function Invoke-PackageGate {
    $workspace = Read-Workspace $workspacePath
    Invoke-Checked { dotnet build (Get-Runner $workspace 'android') -c Release -r android-arm64 --nologo } 'Android arm64 build failed'
    Invoke-Checked { dotnet build (Get-Runner $workspace 'android') -c Release -r android-x64 --nologo } 'Android x64 build failed'
    Invoke-Checked { dotnet build (Get-Runner $workspace 'macos') -c Release -r osx-arm64 --nologo } 'native AppKit macOS build failed'
    Invoke-Checked { dotnet build (Get-Runner $workspace 'maccatalyst') -c Release -r maccatalyst-arm64 --nologo } 'Mac Catalyst cross-build failed'
    Invoke-Checked { dotnet build (Get-Runner $workspace 'ios') -c Release -r iossimulator-x64 --nologo } 'iOS simulator cross-build failed'
    Invoke-Checked { dotnet build (Get-Runner $workspace 'ios') -c Release -r ios-arm64 --nologo } 'iOS arm64 cross-build failed'
    Invoke-Checked { dotnet build (Get-Runner $workspace 'linux') -c Release -r linux-x64 -p:DorotiBuildQtNative=false --nologo } 'Linux managed cross-build failed'
    Write-ShardStamp 'Package'
    Write-Output 'Doroti extended platform build gate: PASS'
}

function Invoke-NativeInteropGate {
    $androidBinding = Join-Path $demoRoot 'android/binding/DorotiDemoApp.Android.Native.csproj'
    $iosBinding = Join-Path $demoRoot 'ios/binding/DorotiDemoApp.iOS.Native.csproj'
    $macBinding = Join-Path $demoRoot 'macos/binding/DorotiDemoApp.MacCatalyst.Native.csproj'
    $appKitBinding = Join-Path $demoRoot 'macos/binding/DorotiDemoApp.MacOS.Native.csproj'
    Invoke-Checked { dotnet build $androidBinding -c Release -r android-arm64 --nologo } 'Android default native binding build failed'
    Invoke-Checked { dotnet build $iosBinding -c Release -r iossimulator-x64 --nologo } 'iOS default native binding cross-build failed'
    Invoke-Checked { dotnet build $macBinding -c Release -r maccatalyst-arm64 --nologo } 'Mac Catalyst default native binding cross-build failed'
    if ($IsMacOS) { Invoke-Checked { dotnet build $appKitBinding -c Release -r osx-arm64 --nologo } 'AppKit macOS default native binding build failed' }
    Invoke-Checked { dotnet build (Join-Path $demoRoot 'android/DorotiDemoApp.Android.csproj') -c Release -r android-arm64 --nologo } 'Android runner to native binding graph failed'
    $androidJavaHome = if (Test-Path -LiteralPath 'C:\Program Files\Android\openjdk' -PathType Container) {
        @(Get-ChildItem -LiteralPath 'C:\Program Files\Android\openjdk' -Directory | Sort-Object Name -Descending | Where-Object { Test-Path -LiteralPath (Join-Path $_.FullName 'bin/java.exe') } | Select-Object -ExpandProperty FullName -First 1)
    } else { @($env:JAVA_HOME) }
    Assert-True (-not [string]::IsNullOrWhiteSpace([string]$androidJavaHome)) 'Android OpenJDK discovery'
    $previousJavaHome = $env:JAVA_HOME
    try {
        $env:JAVA_HOME = [string]$androidJavaHome
        Push-Location (Join-Path $demoRoot 'android/native')
        try {
            Invoke-Checked { & '.\gradlew.bat' :bridge:test --no-daemon } 'Android native bridge Gradle unit test failed'
        }
        finally { Pop-Location }
    }
    finally { $env:JAVA_HOME = $previousJavaHome }
    $wrapper = Join-Path $demoRoot 'android/native/gradle/wrapper/gradle-wrapper.jar'
    Assert-True ((Get-FileHash -LiteralPath $wrapper -Algorithm SHA256).Hash -ceq 'E996D452D2645E70C01C11143CA2D3742734A28DA2BF61F25C82BDC288C9E637') 'Android Gradle wrapper integrity'
    $aar = @(Get-ChildItem -LiteralPath (Join-Path $demoRoot 'android/binding/bin') -Recurse -File -Filter 'bridge-release.aar')
    Assert-True ($aar.Count -ge 1) 'Android Gradle AAR output'
    $runnerAar = @(Get-ChildItem -LiteralPath (Join-Path $demoRoot 'android/bin/android-arm64/Release') -Recurse -File -Filter 'bridge-release.aar')
    $runnerApk = @(Get-ChildItem -LiteralPath (Join-Path $demoRoot 'android/bin/android-arm64/Release') -Recurse -File -Filter '*.apk')
    Assert-True ($runnerAar.Count -ge 1) 'Android runner bridge artifact propagation'
    Assert-True ($runnerApk.Count -ge 1) 'Android runner APK output'
    foreach ($alias in @('ios', 'macos')) {
        $abi = Get-Content -LiteralPath (Join-Path $demoRoot "$alias/binding/abi-contract.json") -Raw | ConvertFrom-Json
        Assert-True ($abi.bridgeVersion -ceq '1.0.0' -and $abi.exports.Count -eq 3) "$alias native ABI contract"
        $schemeCount = @(Get-ChildItem -LiteralPath (Join-Path $demoRoot "$alias/native") -Recurse -File -Filter '*.xcscheme').Count
        Assert-True ($(if ($alias -ceq 'ios') { $schemeCount -eq 1 } else { $schemeCount -ge 2 })) "$alias shared Xcode schemes"
        $bindingApi = Get-Content -LiteralPath (Join-Path $demoRoot "$alias/binding/ApiDefinition.cs") -Raw
        $swiftApi = Get-Content -LiteralPath (Join-Path $demoRoot "$alias/native/DorotiDemoAppNative/DorotiNativeBridge.swift") -Raw
        foreach ($export in $abi.exports) {
            Assert-True ($bindingApi -match [Regex]::Escape("Export(`"$export`")")) "$alias managed ABI export $export"
            Assert-True ($swiftApi -match [Regex]::Escape("@objc($export)")) "$alias Swift ABI export $export"
        }
        $pbx = Get-Content -LiteralPath (Join-Path $demoRoot "$alias/native/DorotiDemoAppNative.xcodeproj/project.pbxproj") -Raw
        if ($alias -ceq 'ios') { Assert-True ($pbx -match 'SUPPORTED_PLATFORMS = "iphoneos iphonesimulator"' -and $pbx -match 'SUPPORTS_MACCATALYST = NO') 'iOS Xcode destinations' }
        else { Assert-True ($pbx -match 'SUPPORTED_PLATFORMS = "macosx"' -and $pbx -match 'SUPPORTS_MACCATALYST = YES') 'Mac Catalyst Xcode destination' }
    }
    $appKitAbi = Get-Content -LiteralPath (Join-Path $demoRoot 'macos/binding/macos/abi-contract.json') -Raw | ConvertFrom-Json
    $appKitApi = Get-Content -LiteralPath (Join-Path $demoRoot 'macos/binding/macos/ApiDefinition.cs') -Raw
    Assert-True ($appKitAbi.platform -ceq 'macOS' -and $appKitAbi.exports.Count -eq 3) 'AppKit macOS native ABI contract'
    foreach ($export in $appKitAbi.exports) {
        Assert-True ($appKitApi -match [Regex]::Escape("Export(`"$export`")")) "AppKit managed ABI export $export"
    }
    $appKitHost = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/DorotiMacOSMetalView.cs') -Raw
    Assert-True ($appKitHost -match 'MTKView' -and $appKitHost -match 'AddCompletedHandler' -and $appKitHost -notmatch 'SKGLViewHandler') 'AppKit Metal completion surface contract'
    $macPbx = Get-Content -LiteralPath (Join-Path $demoRoot 'macos/native/DorotiDemoAppNative.xcodeproj/project.pbxproj') -Raw
    $appKitScheme = Join-Path $demoRoot 'macos/native/DorotiDemoAppNative.xcodeproj/xcshareddata/xcschemes/DorotiDemoAppNative-macOS.xcscheme'
    $macSwift = Get-Content -LiteralPath (Join-Path $demoRoot 'macos/native/DorotiDemoAppNative/DorotiNativeBridge.swift') -Raw
    Assert-True (Test-Path -LiteralPath $appKitScheme -PathType Leaf) 'AppKit shared Xcode scheme'
    Assert-True ($macPbx -match 'DorotiDemoAppNative-macOS' -and $macPbx -match 'MACOSX_DEPLOYMENT_TARGET = 14\.0' -and
        $macPbx -match 'SUPPORTED_PLATFORMS = "macosx"' -and $macPbx -match 'SUPPORTS_MACCATALYST = NO') 'AppKit Xcode target isolation'
    Assert-True ($macSwift -match '#if targetEnvironment\(macCatalyst\)' -and $macSwift -match 'import UIKit' -and
        $macSwift -match '#else' -and $macSwift -match 'import AppKit') 'dual Apple desktop Swift source boundary'
    Write-ShardStamp 'NativeInterop'
    Write-Output 'Doroti default native bridge build gate: PASS (Apple Xcode execution notVerified on Windows)'
}

function Invoke-TemplateGate {
    $id = [Guid]::NewGuid().ToString('N')
    $shortId = $id.Substring(0, 8)
    $feed = Join-Path $tmpRoot "template-feed-$id"
    $hive = Join-Path $tmpRoot "template-hive-$id"
    # MAUI Resizetizer and Apple's actool disagree about /var versus /private/var
    # symlink-normalized output paths. Keep Apple template builds on a canonical path.
    $templateTempRoot = if ($IsMacOS) { '/private/tmp' } else { [IO.Path]::GetTempPath() }
    $externalRoot = Join-Path $templateTempRoot "dt-$shortId"
    $kotlinRoot = Join-Path $templateTempRoot "dk-$shortId"
    [IO.Directory]::CreateDirectory($feed) | Out-Null
    [IO.Directory]::CreateDirectory($externalRoot) | Out-Null
    [IO.Directory]::CreateDirectory($kotlinRoot) | Out-Null
    if ($IsMacOS) {
        $packProjects = @(Get-ChildItem -LiteralPath (Join-Path $dorotiRoot 'src') -Directory | ForEach-Object {
            $project = Join-Path $_.FullName "$($_.Name).csproj"
            if (-not (Test-Path -LiteralPath $project -PathType Leaf)) { return }
            $text = Get-Content -LiteralPath $project -Raw
            if ($text -notmatch '<IsDorotiProduct>true</IsDorotiProduct>') { return }
            if ($_.Name -match '^Doroti\.Target\.(Windows|Android|iOS)') { return }
            $project
        })
        $packProjects += Join-Path $dorotiRoot 'src/Doroti.App.Sdk/Doroti.App.Sdk.csproj'
        $packProjects += Join-Path $dorotiRoot 'src/Doroti.Runner.Sdk/Doroti.Runner.Sdk.csproj'
        $packProjects += Join-Path $dorotiRoot 'templates/Doroti.Templates/Doroti.Templates.csproj'
        foreach ($project in $packProjects) {
            Invoke-Checked {
                dotnet pack $project -c Release -o $feed --nologo -p:EnableWindowsTargeting=true -p:DorotiHostMacOnly=true
            } "Doroti package pack failed for $project"
        }
    }
    else {
        Invoke-Checked { dotnet pack (Join-Path $dorotiRoot 'Doroti.Product.slnx') -c Release -o $feed --nologo -m:1 -p:EnableWindowsTargeting=true } 'Doroti package and template pack failed'
    }
    $templatePackage = Join-Path $feed 'Doroti.Templates.0.2.0-beta.nupkg'
    Assert-True (Test-Path -LiteralPath $templatePackage -PathType Leaf) 'template package output'
    Invoke-Checked { dotnet new install $templatePackage --debug:custom-hive $hive } 'template package install failed'
    Invoke-Checked {
        dotnet new doroti-app --name DorotiWorkspaceProbe --applicationId dev.doroti.validation --displayVersion 1.2.3 --applicationVersion 42 --output $externalRoot --debug:custom-hive $hive
    } 'template creation failed'
    Invoke-Checked {
        dotnet new doroti-app --name DorotiKotlinProbe --applicationId dev.doroti.kotlin --nativeAndroidLanguage kotlin --output $kotlinRoot --debug:custom-hive $hive
    } 'Kotlin template creation failed'
    Assert-True (Test-Path -LiteralPath (Join-Path $externalRoot 'android/native/bridge/src/main/java/dev/doroti/bridge/DorotiNativeBridge.java') -PathType Leaf) 'default Java native bridge source'
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $externalRoot 'android/native/bridge/src/main/kotlin'))) 'default Java template Kotlin source absence'
    Assert-True (Test-Path -LiteralPath (Join-Path $kotlinRoot 'android/native/bridge/src/main/kotlin/dev/doroti/bridge/DorotiNativeBridge.kt') -PathType Leaf) 'optional Kotlin native bridge source'
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $kotlinRoot 'android/native/bridge/src/main/java'))) 'Kotlin template Java source absence'
    Assert-True ((Get-Content -LiteralPath (Join-Path $kotlinRoot 'android/native/bridge/build.gradle.kts') -Raw) -match 'org\.jetbrains\.kotlin\.android') 'Kotlin Gradle plugin selection'
    $configPath = Join-Path $externalRoot 'NuGet.Config'
    $escapedFeed = [Security.SecurityElement]::Escape($feed)
    $config = "<?xml version=`"1.0`" encoding=`"utf-8`"?><configuration><packageSources><clear/><add key=`"doroti-validation`" value=`"$escapedFeed`"/><add key=`"nuget.org`" value=`"https://api.nuget.org/v3/index.json`"/></packageSources></configuration>"
    [IO.File]::WriteAllText($configPath, $config, [Text.UTF8Encoding]::new($false))
    $previousPackages = $env:NUGET_PACKAGES
    try {
        $env:NUGET_PACKAGES = Join-Path $externalRoot '.nuget/packages'
        Invoke-Checked { dotnet build (Join-Path $externalRoot 'DorotiWorkspaceProbe.csproj') -c Release --configfile $configPath --nologo } 'template neutral app build failed'
        if ($IsWindows) {
            Invoke-Checked { dotnet build (Join-Path $externalRoot 'windows/DorotiWorkspaceProbe.Windows.csproj') -c Release --configfile $configPath --nologo } 'template Windows runner build failed'
        }
        Invoke-Checked { dotnet build (Join-Path $externalRoot 'web/DorotiWorkspaceProbe.Web.csproj') -c Release --configfile $configPath --nologo } 'template Web runner build failed'
        if ($IsMacOS) {
            Invoke-Checked { dotnet build (Join-Path $externalRoot 'macos/DorotiWorkspaceProbe.MacOS.csproj') -c Release -r osx-arm64 --configfile $configPath --nologo } 'template AppKit runner build failed'
            Invoke-Checked { dotnet build (Join-Path $externalRoot 'macos/DorotiWorkspaceProbe.MacCatalyst.csproj') -c Release -r maccatalyst-arm64 --configfile $configPath --nologo } 'template Mac Catalyst runner build failed'
            Invoke-Checked { xcodebuild -list -project (Join-Path $externalRoot 'macos/native/DorotiWorkspaceProbe.xcodeproj') } 'template Apple desktop Xcode graph failed'
        }
    }
    finally { $env:NUGET_PACKAGES = $previousPackages }
    $createdProjects = @(Get-ChildItem -LiteralPath $externalRoot -Recurse -File -Filter '*.csproj' | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
    Assert-True ($createdProjects.Count -eq 12) 'template-created project count'
    foreach ($alias in @('android', 'ios', 'linux', 'macos', 'web', 'windows')) {
        Assert-True (Test-Path -LiteralPath (Join-Path $externalRoot $alias) -PathType Container) "template-created $alias workspace"
    }
    foreach ($alias in @('android', 'ios', 'macos', 'maccatalyst')) {
        $directory = if ($alias -ceq 'maccatalyst') { 'macos' } else { $alias }
        Assert-True (Test-Path -LiteralPath (Join-Path $externalRoot "$directory/binding") -PathType Container) "template-created $alias native binding directory"
        Assert-True (Test-Path -LiteralPath (Join-Path $externalRoot "$directory/native") -PathType Container) "template-created $alias native project"
        $runnerName = switch ($alias) {
            'android' { 'DorotiWorkspaceProbe.Android.csproj' }
            'ios' { 'DorotiWorkspaceProbe.iOS.csproj' }
            'macos' { 'DorotiWorkspaceProbe.MacOS.csproj' }
            'maccatalyst' { 'DorotiWorkspaceProbe.MacCatalyst.csproj' }
        }
        $runnerText = Get-Content -LiteralPath (Join-Path $externalRoot "$directory/$runnerName") -Raw
        Assert-True ($runnerText -match 'DorotiNativeBindingProject') "template-created $alias binding reference"
    }
    Assert-True (Test-Path -LiteralPath (Join-Path $externalRoot 'android/native/gradlew') -PathType Leaf) 'template-created Gradle launcher path'
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $externalRoot 'android/native/gradlew/android'))) 'template-created Gradle launcher nesting absence'
    $androidDirectoryProps = Get-Content -LiteralPath (Join-Path $externalRoot 'android/Directory.Build.props') -Raw
    Assert-True ($androidDirectoryProps -match '<DefaultItemExcludes>\$\(DefaultItemExcludes\);obj\\\*\*;bin\\\*\*</DefaultItemExcludes>') 'template-created Android cross-RID source exclusion'
    Assert-True ($androidDirectoryProps -match '<MSBuildProjectExtensionsPath>\$\(BaseIntermediateOutputPath\)</MSBuildProjectExtensionsPath>') 'template-created Android restore isolation'
    $generatedText = @(Get-ChildItem -LiteralPath $externalRoot -Recurse -File | Where-Object {
        $_.FullName -notmatch '[\\/](bin|obj|\.nuget)[\\/]' -and $_.Extension -in @('.json', '.props', '.csproj', '.cs', '.java', '.kt', '.kts', '.swift', '.h', '.pbxproj', '.xcscheme')
    } | ForEach-Object { Get-Content -LiteralPath $_.FullName -Raw }) -join "`n"
    Assert-True ($generatedText -notmatch '__DOROTI_|DorotiTemplateApp|DorotiDemoApp|NewBinding' -and $generatedText -match 'dev\.doroti\.validation') 'template identity replacement'
    $generatedProps = Get-Content -LiteralPath (Join-Path $externalRoot 'Directory.Build.props') -Raw
    Assert-True ($generatedProps -match '<ApplicationDisplayVersion>1\.2\.3</ApplicationDisplayVersion>' -and $generatedProps -match '<ApplicationVersion>42</ApplicationVersion>') 'template version replacement'
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $externalRoot 'Platforms'))) 'template-created legacy Platforms absence'
    Write-ShardStamp 'Template'
    Write-Output "Doroti package-only workspace template gate: PASS ($externalRoot)"
}

function Write-Evidence {
    $fingerprint = Get-SourceFingerprint
    $graphStatus = Get-ShardStatus 'Graph' $fingerprint
    $buildStatus = Get-ShardStatus 'Build' $fingerprint
    $packageStatus = Get-ShardStatus 'Package' $fingerprint
    $interopStatus = Get-ShardStatus 'NativeInterop' $fingerprint
    $templateStatus = Get-ShardStatus 'Template' $fingerprint
    $appKitProductEvidencePath = Join-Path $dorotiRoot 'validation/evidence/appkit-macos/product-live.json'
    $appKitNativeStatus = 'notVerified'
    if (Test-Path -LiteralPath $appKitProductEvidencePath -PathType Leaf) {
        $appKitProductEvidence = Get-Content -LiteralPath $appKitProductEvidencePath -Raw | ConvertFrom-Json
        if ([string]$appKitProductEvidence.verification.nativeLaunch -ceq 'pass' -and
            [string]$appKitProductEvidence.verification.metalPresentCompletion -ceq 'pass') {
            $appKitNativeStatus = 'pass'
        }
    }
    $evidence = [ordered]@{
        schemaVersion = 'doroti.platform-workspace-evidence/v1'
        capturedAt = [DateTimeOffset]::UtcNow.ToString('O')
        sourceFingerprint = $fingerprint
        applicationProject = 'DorotiDemoApp/DorotiDemoApp.csproj'
        workspaceManifest = 'DorotiDemoApp/doroti-workspace.json'
        automated = [ordered]@{
            graph = $graphStatus; rootApp = $buildStatus; windowsBuild = $buildStatus; webBuild = $buildStatus
            androidArm64Build = $packageStatus; androidX64Build = $packageStatus; appKitMacOSBuild = $packageStatus; macCatalystCrossBuild = $packageStatus
            iosSimulatorCrossBuild = $packageStatus; iosArm64CrossBuild = $packageStatus; linuxManagedCrossBuild = $packageStatus
            androidInteropBinding = $interopStatus; iosInteropCrossBuild = $interopStatus; macCatalystInteropCrossBuild = $interopStatus; packageOnlyTemplate = $templateStatus
            androidGradleUnitTest = $interopStatus; androidAar = $interopStatus; androidRunnerBridgeArtifact = $interopStatus
            iosXcodeFramework = 'notVerified'; appKitMacOSXcodeFramework = $(if ($IsMacOS) { $interopStatus } else { 'notVerified' }); macCatalystXcodeFramework = 'notVerified'
        }
        nativeBridge = [ordered]@{
            abiVersion = 'doroti.native-platform-bridge/v1'; bridgeVersion = '1.0.0'; projectCount = 12
            androidStudioSync = 'notVerified'; androidEmulator = 'notVerified'; androidPhysical = 'notVerified'
            iosXcodeBuild = 'notVerified'; iosSimulator = 'notVerified'; iosDeviceSigningArchive = 'notVerified'
            macOSXcodeBuild = $(if ($IsMacOS) { $interopStatus } else { 'notVerified' }); macOSLaunch = $appKitNativeStatus; macOSSigningArchive = 'notVerified'
            macCatalystXcodeBuild = 'notVerified'; macCatalystLaunch = 'notVerified'; macCatalystSigningArchive = 'notVerified'
        }
        targets = @(
            [ordered]@{ alias='windows'; runner='DorotiDemoApp/windows/DorotiDemoApp.Windows.csproj'; targetFramework='net10.0-windows10.0.19041.0'; rid='win-x64'; host='Maui'; automated=$buildStatus; nativeLive='notVerified' }
            [ordered]@{ alias='web'; runner='DorotiDemoApp/web/DorotiDemoApp.Web.csproj'; targetFramework='net10.0'; rid='browser-wasm'; host='BlazorWebAssembly'; automated=$buildStatus; browserLive='notVerified' }
            [ordered]@{ alias='android'; runner='DorotiDemoApp/android/DorotiDemoApp.Android.csproj'; targetFramework='net10.0-android'; rid='android-arm64'; host='Maui'; automated=$packageStatus; nativeLive='notVerified'; physical='notVerified' }
            [ordered]@{ alias='android'; runner='DorotiDemoApp/android/DorotiDemoApp.Android.csproj'; targetFramework='net10.0-android'; rid='android-x64'; host='Maui'; automated=$packageStatus; nativeLive='notVerified' }
            [ordered]@{ alias='macos'; runner='DorotiDemoApp/macos/DorotiDemoApp.MacOS.csproj'; targetFramework='net10.0-macos'; rid='osx-arm64'; host='Maui'; entry='AppKit-Main'; backend='AppKit/MTKView/Metal-Skia'; automated=$packageStatus; nativeLive=$appKitNativeStatus; liveEvidence='Doroti/validation/evidence/appkit-macos/product-live.json' }
            [ordered]@{ alias='maccatalyst'; runner='DorotiDemoApp/macos/DorotiDemoApp.MacCatalyst.csproj'; targetFramework='net10.0-maccatalyst'; rid='maccatalyst-arm64'; host='Maui'; entry='UIKit-Main'; automated=$packageStatus; nativeLive='notVerified' }
            [ordered]@{ alias='ios'; runner='DorotiDemoApp/ios/DorotiDemoApp.iOS.csproj'; targetFramework='net10.0-ios'; rid='iossimulator-x64'; host='Maui'; automated=$packageStatus; nativeLive='notVerified' }
            [ordered]@{ alias='ios'; runner='DorotiDemoApp/ios/DorotiDemoApp.iOS.csproj'; targetFramework='net10.0-ios'; rid='ios-arm64'; host='Maui'; automated=$packageStatus; nativeLive='notVerified'; physical='notVerified'; signing='notVerified' }
            [ordered]@{ alias='linux'; runner='DorotiDemoApp/linux/DorotiDemoApp.Linux.csproj'; targetFramework='net10.0'; rid='linux-x64'; host='Qt'; automated=$packageStatus; nativeBuild='notVerified'; x11='notVerified'; wayland='notVerified' }
        )
        execution = [ordered]@{
            windowsNativeLive = 'notVerified'; webBrowserLive = 'notVerified'; androidEmulator = 'notVerified'; androidPhysical = 'notVerified'
            appKitMacOSNative = $appKitNativeStatus; macCatalystNative = 'notVerified'; iosSimulatorNative = 'notVerified'; iosPhysicalSigningVoiceOver = 'notVerified'
            linuxX11 = 'notVerified'; linuxWayland = 'notVerified'
        }
        toolchain = [ordered]@{
            dotnetSdk = (& dotnet --version).Trim(); hostOS = [Runtime.InteropServices.RuntimeInformation]::OSDescription
            qt = 'notInstalled'; xcode = 'notAvailableOnWindows'; gradle = '8.10.2'; androidGradlePlugin = '8.6.1'; javaSourceLevel = '17'; androidBuildJdk = 'OpenJDK 21.0.8'; nativeLibraryInteropRevision = '07df778f1f85c2ad06cb74d3c8faa6ee9011191c'
        }
        note = 'Automated build results are not native-live, browser-live, physical-device, accessibility, signing, store, X11, or Wayland evidence.'
    }
    Write-Json $evidencePath $evidence
    Write-Output "Doroti workspace evidence written: $evidencePath"
}
function Write-LiveNotVerified([string] $Name) {
    Write-Warning "$Name remains notVerified. Run this shard on the required native/browser/device host with an evidence collector before changing its status."
}

if (Test-Shard 'Graph') { Invoke-GraphGate }
if (Test-Shard 'Build') { Invoke-BuildGate }
if (Test-Shard 'Package') { Invoke-PackageGate }
if (Test-Shard 'Template') { Invoke-TemplateGate }
if (Test-Shard 'NativeInterop') { Invoke-NativeInteropGate }
if ($Shard -in @('Live', 'WindowsLive')) { Write-LiveNotVerified 'Windows native live' }
if ($Shard -eq 'WebLive') { Write-LiveNotVerified 'Web browser live' }
if ($Shard -eq 'AndroidLive') { Write-LiveNotVerified 'Android emulator live' }
if ($Shard -eq 'AndroidPhysical') { Write-LiveNotVerified "Android physical live ($AndroidSerial)" }
if (Test-Shard 'Evidence') { Write-Evidence }
Write-Output "Doroti application target shard '$Shard' ($Configuration): COMPLETE"
