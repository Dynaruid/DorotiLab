#Requires -Version 5.1
param(
    [ValidateSet('All', 'Graph', 'Build', 'Live', 'WindowsLive', 'AndroidLive', 'AndroidPhysical', 'Evidence')]
    [string] $Shard = 'All',
    [string] $AndroidSerial = ''
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$project = Join-Path $repoRoot 'DorotiDemoApp/DorotiDemoApp.csproj'
$productSolution = Join-Path $dorotiRoot 'Doroti.Product.slnx'
$mauiHostProject = Join-Path $dorotiRoot 'src/Doroti.Host.Maui/Doroti.Host.Maui.csproj'
$templateRoot = Join-Path $dorotiRoot 'templates/Doroti.Templates/content/doroti-app'
$descriptorContract = Join-Path $dorotiRoot 'validation/app-bootstrap/descriptor-contract/DescriptorContract.csproj'
$syntheticProject = Join-Path $dorotiRoot 'validation/app-bootstrap/synthetic-fourth-host/SyntheticFourthHost.csproj'
$invalidRegistrationProject = Join-Path $dorotiRoot 'validation/app-bootstrap/invalid-required-registration/InvalidRequiredRegistration.csproj'
$tmpRoot = Join-Path $dorotiRoot '.doroti/tmp/app-targets'
$publishRoot = Join-Path $tmpRoot 'windows-publish'
$rawLivePath = Join-Path $tmpRoot 'windows-live.json'
$rawAndroidLivePath = Join-Path $tmpRoot 'android-live.json'
$evidencePath = Join-Path $dorotiRoot 'migration/maui/app-targets-evidence.json'
[IO.Directory]::CreateDirectory($tmpRoot) | Out-Null

function Test-Shard([string] $Name) { return $Shard -eq 'All' -or $Shard -eq $Name }

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Get-DorotiTargetGraph([string] $Path) {
    return @(Get-Content -LiteralPath $Path | ForEach-Object { $_.Replace('/', '\') })
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

function Measure-RenderedScreenshot([string] $Path) {
    Add-Type -AssemblyName System.Drawing.Common
    $bitmap = [System.Drawing.Bitmap]::FromFile($Path)
    try {
        # Sample the scrollable body only. The fixed AppBar and FAB remain colored
        # when a broken clip makes the entire viewport white, so whole-screen
        # sampling can report a false pass.
        $left = [int]($bitmap.Width * 0.02)
        $right = [int]($bitmap.Width * 0.85)
        $top = [int]($bitmap.Height * 0.12)
        $bottom = [int]($bitmap.Height * 0.84)
        $samples = 0
        $nonLight = 0
        $colored = 0
        $dark = 0
        for ($y = $top; $y -lt $bottom; $y += 12) {
            for ($x = $left; $x -lt $right; $x += 12) {
                $pixel = $bitmap.GetPixel($x, $y)
                $samples++
                if ($pixel.R -lt 225 -or $pixel.G -lt 225 -or $pixel.B -lt 225) { $nonLight++ }
                $maximum = [Math]::Max($pixel.R, [Math]::Max($pixel.G, $pixel.B))
                $minimum = [Math]::Min($pixel.R, [Math]::Min($pixel.G, $pixel.B))
                if (($maximum - $minimum) -gt 25) { $colored++ }
                if ($pixel.R -lt 32 -and $pixel.G -lt 32 -and $pixel.B -lt 32) { $dark++ }
            }
        }
        Assert-True ($samples -gt 0) 'Android screenshot sample count'
        return [ordered]@{
            width = $bitmap.Width
            height = $bitmap.Height
            sampleRegion = [ordered]@{ left = $left; top = $top; right = $right; bottom = $bottom }
            sampleCount = $samples
            nonLightRatio = [Math]::Round($nonLight / $samples, 4)
            coloredRatio = [Math]::Round($colored / $samples, 4)
            darkRatio = [Math]::Round($dark / $samples, 4)
        }
    }
    finally { $bitmap.Dispose() }
}

function Invoke-AppRestore([string] $Target, [string] $Rid) {
    if (-not $script:productRestoreComplete) {
        Invoke-Checked { dotnet restore $productSolution --nologo } 'Doroti product dependency restore failed'
        $script:productRestoreComplete = $true
    }
    Invoke-Checked {
        dotnet restore $project --force-evaluate -p:DorotiTarget=$Target -p:RuntimeIdentifier=$Rid --nologo
    } "$Target application restore failed"
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

function Invoke-GraphGate {
    $program = Get-Content -LiteralPath (Join-Path (Split-Path $project -Parent) 'Program.cs') -Raw
    Assert-True ($program -match 'public sealed class Program : IDorotiApplicationStartup') 'public target-neutral startup type'
    Assert-True ($program -notmatch '#if|DOROTI_BROWSER|MACCATALYST|Maui|Blazor|Qt') 'Program target neutrality'
    $mauiHost = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiHostAdapter.cs') -Raw
    $mauiSurface = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/DorotiMauiSurface.cs') -Raw
    $mauiGraphics = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiSkiaCapabilities.cs') -Raw
    $mauiNativeInput = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiNativeInput.cs') -Raw
    $mauiSemantics = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiSemanticsBridge.cs') -Raw
    foreach ($token in @('SKTouchDeviceType.Mouse','SKTouchDeviceType.Pen','SKTouchAction.WheelChanged','_textInput.SetClient','MauiNativeInput.Attach')) {
        Assert-True ($mauiHost.IndexOf($token, [StringComparison]::Ordinal) -ge 0) "MAUI interaction token $token"
    }
    Assert-True ($mauiSurface -match 'MauiTextInputBridge' -and $mauiSurface -match 'MauiSemanticsBridge' -and $mauiSurface -match 'setSemanticsTreeEnabled\(true\)') 'MAUI native IME and semantics composition'
    Assert-True ($mauiSurface -notmatch 'AUTO_QUIT|auto_quit|Application\.Current\?\.Quit') 'MAUI application auto-quit absence'
    Assert-True ($mauiGraphics -match 'canvas\.Clear\(_backgroundColor\)' -and $mauiGraphics -match 'if \(frame is null\)') 'MAUI opaque startup surface clear'
    Assert-True ($mauiNativeInput -match 'KeyData' -and $mauiNativeInput -match '#if WINDOWS' -and $mauiNativeInput -match 'PressesBegan' -and $mauiNativeInput -match '#elif ANDROID') 'Windows, Mac Catalyst, and Android native keyboard bridges'
    Assert-True ($mauiSemantics -match 'SemanticsAction\.tap' -and $mauiSemantics -match 'SemanticsAction\.setText') 'MAUI actionable semantics bridge'
    foreach ($root in @((Split-Path $project -Parent), $templateRoot)) {
        $appSource = Get-Content -LiteralPath (Join-Path $root 'src/App.cs') -Raw
        Assert-True ($appSource -match 'DorotiViewConfiguration[\s\S]*0xfffffbfeL') "$root startup background contract"
        Assert-True (-not (Test-Path -LiteralPath (Join-Path $root 'Platforms/Maui'))) "$root legacy Platforms/Maui absence"
        Assert-True (@(Get-ChildItem -LiteralPath (Join-Path $root 'Platforms') -Filter 'PlatformBootstrap.cs' -File -Recurse).Count -eq 0) "$root legacy PlatformBootstrap absence"
        $shaderPath = Join-Path $root 'Resources/Shaders/aurora.sksl'
        Assert-True (Test-Path -LiteralPath $shaderPath -PathType Leaf) "$root custom shader asset"
        $shaderBytes = [IO.File]::ReadAllBytes($shaderPath)
        $shaderHash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($shaderBytes)).ToLowerInvariant()
        Assert-True ($shaderBytes.LongLength -eq 210 -and $shaderHash -eq '658695aca71aa8cdc9e776e037ea404c29c5759a481d9955c9f21e4a1e664e41') "$root custom shader integrity"
        foreach ($targetManifest in Get-ChildItem -LiteralPath (Join-Path $root 'Platforms') -Filter 'application-manifest*.json' -File -Recurse) {
            $manifestText = Get-Content -LiteralPath $targetManifest.FullName -Raw
            Assert-True ($manifestText -match 'shaders/aurora\.sksl' -and $manifestText -match 'Doroti\.Shaders\.aurora\.sksl') "$($targetManifest.FullName) custom shader registration"
        }
    }
    $sdkTargets = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.App.Sdk/Sdk/Sdk.targets') -Raw
    Assert-True ($sdkTargets -match 'MauiSplashScreen[^>]+Color="#FFFBFE"') 'MAUI splash and first-scene background continuity'
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
    Invoke-Checked { dotnet run --project (Join-Path $dorotiRoot 'validation/runtime-async-contract/Doroti.Validation.RuntimeAsyncContract.csproj') -c Release --nologo } 'runtime async contract failed'
    Invoke-Checked { dotnet run --project (Join-Path $dorotiRoot 'validation/runtime-shader-contract/Doroti.Validation.RuntimeShaderContract.csproj') -c Release --nologo } 'runtime shader contract failed'

    foreach ($target in @(
        [ordered]@{ Name='Windows'; Rid='win-x64'; Graph='windows'; Host='Maui'; Entry='WinUI-Xaml' },
        [ordered]@{ Name='Android'; Rid='android-arm64'; Graph='android'; Host='Maui'; Entry='Android-Application' },
        [ordered]@{ Name='MacCatalyst'; Rid='maccatalyst-arm64'; Graph='maccatalyst'; Host='Maui'; Entry='UIKit-Main' },
        [ordered]@{ Name='Web'; Rid='browser-wasm'; Graph='Web'; Host='BlazorWebAssembly'; Entry='Managed-Main' }
    )) {
        Invoke-Checked {
            dotnet msbuild $project -t:WriteDorotiTargetGraph -p:DorotiTarget=$($target.Name) -p:RuntimeIdentifier=$($target.Rid) -nologo
        } "$($target.Name) target graph failed"
        $graphPath = Join-Path (Split-Path $project -Parent) "obj/$($target.Graph)/doroti-target-graph.txt"
        Assert-True (Test-Path -LiteralPath $graphPath -PathType Leaf) "$($target.Name) target graph output"
        $graph = Get-DorotiTargetGraph $graphPath
        Assert-True (@($graph | Where-Object { $_ -ceq 'compile=Program.cs' }).Count -eq 1) "$($target.Name) root bootstrap"
        Assert-True (@($graph | Where-Object { $_ -ceq 'compile=src\App.cs' }).Count -eq 1) "$($target.Name) shared app source"
        Assert-True (@($graph | Where-Object { $_ -ceq "startup=DorotiDemoApp.Program" }).Count -eq 1) "$($target.Name) startup identity"
        Assert-True (@($graph | Where-Object { $_ -like "descriptor=$($target.Name)|$($target.Host)|$($target.Entry)|*" }).Count -eq 1) "$($target.Name) descriptor identity"
        Assert-True (@($graph | Where-Object { $_ -like 'compile=*Doroti.Generated\DorotiBootstrap.g.cs' }).Count -eq 1) "$($target.Name) generated bootstrap count"
        Assert-True (@($graph | Where-Object { $_ -like 'compile=*Doroti.Generated\DorotiPluginRegistration.g.cs' }).Count -eq 1) "$($target.Name) generated plugin registration count"
        if ($target.Name -eq 'Windows') {
            Assert-True (@($graph | Where-Object { $_ -ceq 'applicationDefinition=Platforms\Windows\App.xaml' }).Count -eq 1) 'Windows ApplicationDefinition count'
            Assert-True (@($graph | Where-Object { $_ -like 'compile=Platforms\Web\*' }).Count -eq 0) 'Windows Web source exclusion'
        } elseif ($target.Name -eq 'Android') {
            Assert-True (@($graph | Where-Object { $_ -like 'applicationDefinition=?*' }).Count -eq 0) 'Android XAML exclusion'
            Assert-True (@($graph | Where-Object { $_ -ceq 'compile=Platforms\Android\MainActivity.cs' }).Count -eq 1) 'Android MainActivity source'
            Assert-True (@($graph | Where-Object { $_ -ceq 'compile=Platforms\Android\MainApplication.cs' }).Count -eq 1) 'Android MainApplication source'
            Assert-True (@($graph | Where-Object { $_ -like 'compile=Platforms\Windows\*' -or $_ -like 'compile=Platforms\MacCatalyst\*' -or $_ -like 'compile=Platforms\Web\*' }).Count -eq 0) 'Android other-platform source exclusion'
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
    $syntheticGraph = Get-DorotiTargetGraph (Join-Path (Split-Path $syntheticProject -Parent) 'obj/linux/doroti-target-graph.txt')
    Assert-True (@($syntheticGraph | Where-Object { $_ -ceq 'descriptor=Linux|SyntheticQt|Synthetic|Doroti.Target.Linux.Qt.linux-x64' }).Count -eq 1) 'synthetic fourth host descriptor identity'
    $invalidAndroidRid = @(& dotnet msbuild $project -t:WriteDorotiTargetGraph -p:DorotiTarget=Android -p:RuntimeIdentifier=android-x86 -nologo 2>&1)
    Assert-True ($LASTEXITCODE -ne 0 -and (($invalidAndroidRid -join "`n") -match 'DOROTIAPP004')) 'Android RID diagnostic fail-closed'
    $global:LASTEXITCODE = 0
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
    Invoke-AppRestore 'Android' 'android-arm64'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Android -p:RuntimeIdentifier=android-arm64 --nologo --no-restore } 'Android Release build failed'
    $androidOutput = Join-Path (Split-Path $project -Parent) 'bin/android/Release/net10.0-android/android-arm64'
    $androidApk = @(Get-ChildItem -LiteralPath $androidOutput -Recurse -File -Filter '*-Signed.apk')
    $androidAab = @(Get-ChildItem -LiteralPath $androidOutput -Recurse -File -Filter '*.aab')
    Assert-True ($androidApk.Count -gt 0) 'Android signed APK output'
    Assert-True ($androidAab.Count -gt 0) 'Android AAB output'
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $apkArchive = [IO.Compression.ZipFile]::OpenRead($androidApk[0].FullName)
    try {
        Assert-True (@($apkArchive.Entries | Where-Object { $_.FullName -like 'lib/arm64-v8a/*' }).Count -gt 0) 'Android APK arm64 native assets'
    }
    finally { $apkArchive.Dispose() }
    Invoke-AppRestore 'Web' 'browser-wasm'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm --nologo --no-restore } 'Web Release build failed'
    Invoke-AppRestore 'MacCatalyst' 'maccatalyst-arm64'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64 --nologo --no-restore } 'Mac Catalyst cross-build failed'
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64 --nologo --no-restore } 'Windows repeat build failed'
    foreach ($nativeTarget in @('windows','android','maccatalyst')) {
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

function Invoke-WindowsLiveGate {
    Invoke-AppRestore 'Windows' 'win-x64'
    Invoke-Checked {
        dotnet publish $project -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64 -o $publishRoot --nologo --no-restore
    } 'Windows MAUI publish failed'
    if (Test-Path -LiteralPath $rawLivePath) { [IO.File]::Delete($rawLivePath) }
    $env:DOROTI_MAUI_EVIDENCE = $rawLivePath
    $process = Start-Process -FilePath (Join-Path $publishRoot 'DorotiDemoApp.exe') -PassThru -WindowStyle Hidden
    try {
        $live = $null
        for ($attempt = 0; $attempt -lt 120; $attempt++) {
            Start-Sleep -Milliseconds 500
            if (Test-Path -LiteralPath $rawLivePath -PathType Leaf) {
                try { $live = Get-Content -LiteralPath $rawLivePath -Raw | ConvertFrom-Json } catch { }
                if ($null -ne $live -and [long]$live.Frame.Replayed -gt 0) { break }
            }
            $process.Refresh()
            Assert-True (-not $process.HasExited) 'Windows MAUI process survival while collecting evidence'
        }
        Assert-True ($null -ne $live -and [long]$live.Frame.Replayed -gt 0) 'Windows MAUI retained scene evidence timeout'
    }
    finally {
        if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
        Remove-Item Env:DOROTI_MAUI_EVIDENCE -ErrorAction SilentlyContinue
    }
    Assert-True (Test-Path -LiteralPath $rawLivePath -PathType Leaf) 'Windows MAUI live evidence'
    $live = Get-Content -LiteralPath $rawLivePath -Raw | ConvertFrom-Json
    Assert-True ([long]$live.Frame.Presented -gt 0 -and [long]$live.Frame.Failed -eq 0) 'Windows MAUI presented frame'
    Assert-True ([long]$live.Frame.ShaderImageFiltersRendered -gt 0) 'Windows native ImageFilter.shader execution'
    Assert-True ([long]$live.Frame.Replayed -gt 0) 'Windows MAUI retained scene replay'
    Assert-True ([long]$live.Semantics.UpdatesReceived -gt 0 -and
        [long]$live.Semantics.UpdatesApplied -gt 0) 'Windows typed semantics bridge'
    Assert-True ([long]$live.Semantics.ElementsCreated -eq [long]$live.Semantics.ActiveElements) 'Windows initial semantics node reuse baseline'
    Assert-True ([long]$live.SoftwareFallbackFrames -eq 0) 'Windows MAUI software fallback count'
    Assert-True ([string]$live.Surface.NativeViewType -match 'MauiSKSwapChainPanel') 'Windows MAUI native view type'
    Assert-True ([string]$live.Surface.GraphicsBackend -ceq 'win-x64/winui3/SKSwapChainPanel/ANGLE-DirectX-Skia') 'Windows MAUI backend identity'
}

function Invoke-AndroidLiveGate([bool] $RequirePhysical) {
    Assert-True (-not [string]::IsNullOrWhiteSpace($AndroidSerial)) 'Android serial argument'
    Assert-True ($null -ne (Get-Command adb -ErrorAction SilentlyContinue)) 'adb command availability'
    $devices = @(adb devices | Select-String -Pattern "^$([regex]::Escape($AndroidSerial))\s+device$")
    Assert-True ($devices.Count -eq 1) "Android device '$AndroidSerial' availability"
    $model = (adb -s $AndroidSerial shell getprop ro.product.model).Trim()
    $api = (adb -s $AndroidSerial shell getprop ro.build.version.sdk).Trim()
    $abi = (adb -s $AndroidSerial shell getprop ro.product.cpu.abi).Trim()
    $qemu = (adb -s $AndroidSerial shell getprop ro.kernel.qemu).Trim()
    $runtimeIdentifier = switch ($abi) {
        'arm64-v8a' { 'android-arm64' }
        'x86_64' { 'android-x64' }
        default { throw "Android ABI '$abi' has no Doroti target package." }
    }
    if ($RequirePhysical) {
        Assert-True ($qemu -ne '1') 'physical Android device identity'
        Assert-True ($runtimeIdentifier -eq 'android-arm64') 'physical Android arm64 ABI'
    }

    Invoke-AppRestore 'Android' $runtimeIdentifier
    Invoke-Checked { dotnet build $project -c Release -p:DorotiTarget=Android -p:RuntimeIdentifier=$runtimeIdentifier --nologo --no-restore } 'Android live package build failed'
    $androidArtifactKey = if ($runtimeIdentifier -eq 'android-x64') { 'android-x64' } else { 'android' }
    $androidOutput = Join-Path (Split-Path $project -Parent) "bin/$androidArtifactKey/Release/net10.0-android/$runtimeIdentifier"
    $apk = Get-ChildItem -LiteralPath $androidOutput -Recurse -File -Filter '*-Signed.apk' | Select-Object -First 1
    Assert-True ($null -ne $apk) 'Android live signed APK'
    Invoke-Checked { adb -s $AndroidSerial install --user 0 -r $apk.FullName } 'Android APK install failed'
    $activity = (adb -s $AndroidSerial shell cmd package resolve-activity --user 0 --brief dev.doroti.demo | Select-Object -Last 1).Trim()
    Assert-True ($activity -match '^dev\.doroti\.demo/.+MainActivity$') 'Android launcher activity resolution'
    adb -s $AndroidSerial logcat -b all -c | Out-Null
    adb -s $AndroidSerial shell am force-stop --user 0 dev.doroti.demo | Out-Null
    try {
        function Capture-AndroidBody([string] $Name) {
            $remotePath = "/sdcard/doroti-$Name.png"
            $localPath = Join-Path $tmpRoot "$Name-$runtimeIdentifier.png"
            Invoke-Checked { adb -s $AndroidSerial shell screencap -p $remotePath } "Android screenshot '$Name' capture failed"
            Invoke-Checked { adb -s $AndroidSerial pull $remotePath $localPath } "Android screenshot '$Name' pull failed"
            return Measure-RenderedScreenshot $localPath
        }

        Invoke-Checked { adb -s $AndroidSerial shell am start --user 0 -n $activity } 'Android activity launch failed'
        $appFocused = $false
        for ($attempt = 0; $attempt -lt 50; $attempt++) {
            $focusedWindow = (adb -s $AndroidSerial shell dumpsys window | Select-String -Pattern 'mCurrentFocus' | Select-Object -First 1).ToString()
            if ($focusedWindow -match 'dev\.doroti\.demo') { $appFocused = $true; break }
            Start-Sleep -Milliseconds 50
        }
        Assert-True $appFocused 'Android foreground activity after launch'
        $launchScreenshot = Capture-AndroidBody 'android-launch-immediate'
        Assert-True ($launchScreenshot.darkRatio -lt 0.8) 'Android non-black startup surface'
        $json = $null
        for ($attempt = 0; $attempt -lt 30; $attempt++) {
            Start-Sleep -Seconds 1
            $startupLog = @(adb -s $AndroidSerial logcat -d -s 'DorotiMauiEvidence:I' '*:S' -v raw)
            foreach ($line in $startupLog) {
                if ($line.TrimStart().StartsWith('{')) {
                    try { $json = $line | ConvertFrom-Json } catch { }
                }
            }
            if ($null -ne $json -and [long]$json.Frame.Presented -gt 0) { break }
        }
        Assert-True ($null -ne $json -and [long]$json.Frame.Presented -gt 0) 'Android startup frame before scroll'
        $initialScreenshot = $null
        for ($attempt = 0; $attempt -lt 15; $attempt++) {
            $initialScreenshot = Capture-AndroidBody 'android-scroll-initial'
            if ($initialScreenshot.nonLightRatio -gt 0.05 -and $initialScreenshot.coloredRatio -gt 0.02) { break }
            Start-Sleep -Seconds 1
        }
        Assert-True ($initialScreenshot.nonLightRatio -gt 0.05 -and $initialScreenshot.coloredRatio -gt 0.02) 'Android visible scroll body before swipes'
        $sizeLine = @(adb -s $AndroidSerial shell wm size | Where-Object { $_ -match '(?:Physical|Override) size:\s*(\d+)x(\d+)' }) | Select-Object -Last 1
        Assert-True ($sizeLine -match '(\d+)x(\d+)') 'Android display size query'
        $screenWidth = [int]$Matches[1]
        $screenHeight = [int]$Matches[2]
        $scrollX = [int]($screenWidth / 2)
        $scrollTop = [int]($screenHeight * 0.22)
        $scrollBottom = [int]($screenHeight * 0.78)
        $scrollScreenshots = @()
        for ($index = 0; $index -lt 4; $index++) {
            Invoke-Checked { adb -s $AndroidSerial shell input swipe $scrollX $scrollBottom $scrollX $scrollTop 220 } 'Android upward scroll injection failed'
            $captured = Capture-AndroidBody "android-scroll-up-$index"
            Assert-True ($captured.nonLightRatio -gt 0.05 -and $captured.coloredRatio -gt 0.02) "Android visible body after upward swipe $index"
            $scrollScreenshots += $captured
        }
        for ($index = 0; $index -lt 2; $index++) {
            Invoke-Checked { adb -s $AndroidSerial shell input swipe $scrollX $scrollTop $scrollX $scrollBottom 220 } 'Android downward scroll injection failed'
            $captured = Capture-AndroidBody "android-scroll-down-$index"
            Assert-True ($captured.nonLightRatio -gt 0.05 -and $captured.coloredRatio -gt 0.02) "Android visible body after downward swipe $index"
            $scrollScreenshots += $captured
        }
        if ($RequirePhysical) {
            # A canceled press timer used to remain queued behind a busy Android
            # event loop and fire after the tap recognizer had cleared its down
            # event. Sustained alternating drags exercise that exact lifetime,
            # while periodic in-flight captures reject transient black frames.
            $adbPath = (Get-Command adb).Source
            for ($index = 0; $index -lt 48; $index++) {
                $upward = ($index % 2) -eq 0
                $fromY = if ($upward) { $scrollBottom } else { $scrollTop }
                $toY = if ($upward) { $scrollTop } else { $scrollBottom }
                if (($index % 6) -eq 0) {
                    $swipe = Start-Process -FilePath $adbPath -ArgumentList @(
                        '-s', $AndroidSerial, 'shell', 'input', 'swipe',
                        "$scrollX", "$fromY", "$scrollX", "$toY", '550'
                    ) -PassThru -WindowStyle Hidden
                    Start-Sleep -Milliseconds 180
                    $captured = Capture-AndroidBody "android-scroll-active-$index"
                    Assert-True ($captured.nonLightRatio -gt 0.05 -and $captured.coloredRatio -gt 0.02) "Android non-black body during active swipe $index"
                    $scrollScreenshots += $captured
                    Assert-True ($swipe.WaitForExit(5000) -and $swipe.ExitCode -eq 0) "Android active swipe $index"
                }
                else {
                    Invoke-Checked { adb -s $AndroidSerial shell input swipe $scrollX $fromY $scrollX $toY 180 } "Android stress swipe $index failed"
                }
            }
            $fabX = [int]($screenWidth * 0.88)
            $fabY = [int]($screenHeight * 0.92)
            Invoke-Checked { adb -s $AndroidSerial shell input tap $fabX $fabY } 'Android FAB tap injection failed'
            Start-Sleep -Milliseconds 750
            $fabDumpRemote = '/sdcard/doroti-fab-after.xml'
            $fabDumpLocal = Join-Path $tmpRoot 'android-fab-after.xml'
            Invoke-Checked { adb -s $AndroidSerial shell uiautomator dump $fabDumpRemote } 'Android FAB accessibility state dump failed'
            Invoke-Checked { adb -s $AndroidSerial pull $fabDumpRemote $fabDumpLocal } 'Android FAB accessibility state pull failed'
            $fabDump = Get-Content -LiteralPath $fabDumpLocal -Raw
            Assert-True ($fabDump -match 'content-desc="G6 Material FAB 1"') 'Android FAB framework action after scroll stress'
            Assert-True ($fabDump -match 'content-desc="Stack state · button=0;[^\"]*fab=1"') 'Android semantics overlay ordinary-touch pass-through'
        }
        for ($attempt = 0; $attempt -lt 30; $attempt++) {
            Start-Sleep -Seconds 2
            $log = @(adb -s $AndroidSerial logcat -d -s 'DorotiMauiEvidence:I' '*:S' -v raw)
            foreach ($line in $log) {
                if ($line.TrimStart().StartsWith('{')) {
                    try { $json = $line | ConvertFrom-Json } catch { }
                }
            }
            if ($null -ne $json -and [long]$json.Frame.Presented -ge 12 -and [long]$json.Frame.Replayed -gt 0) { break }
        }
        $appPid = (adb -s $AndroidSerial shell pidof dev.doroti.demo | Out-String).Trim()
        $failureLog = if ([string]::IsNullOrWhiteSpace($appPid)) {
            @(adb -s $AndroidSerial logcat -d -v raw)
        } else {
            @(adb -s $AndroidSerial logcat --pid=$appPid -d -v raw)
        }
        $failures = @($failureLog |
            Select-String -Pattern 'FATAL UNHANDLED EXCEPTION|NotSupportedException: Doroti MAUI canvas operation|Fatal signal|pthread_mutex_lock called on a destroyed mutex|TapGestureRecognizer\.handleTapDown|InvalidOperation_EnumFailedVersion|RuntimeBinderException|BadBinaryOps')
        Assert-True (-not [string]::IsNullOrWhiteSpace($appPid)) 'Android process survival after scroll'
        Assert-True ($failures.Count -eq 0) 'Android scroll crash log'
        Assert-True ($null -ne $json) 'Android structured live evidence'
        Assert-True ([long]$json.Frame.Presented -ge 12 -and [long]$json.Frame.Failed -eq 0) 'Android presented custom-shader frames'
        Assert-True ([long]$json.Frame.ShaderImageFiltersRendered -gt 0) 'Android native ImageFilter.shader execution'
        Assert-True ([long]$json.Frame.Replayed -gt 0) 'Android retained scene replay'
        Assert-True ([long]$json.Semantics.UpdatesReceived -gt [long]$json.Semantics.UpdatesApplied) 'Android semantics update throttling'
        Assert-True ([long]$json.Semantics.UpdatesCoalesced -gt 0) 'Android semantics latest-update coalescing'
        $naiveSemanticsRebuilds = [long]$json.Semantics.UpdatesApplied * [long]$json.Semantics.ActiveElements
        Assert-True ([long]$json.Semantics.ElementsCreated * 2 -lt $naiveSemanticsRebuilds) 'Android semantics native node reuse'
        Assert-True ([long]$json.Semantics.RetainedNodes -le 64) 'Android semantics reachable-node retention'
        Assert-True ([long]$json.SoftwareFallbackFrames -eq 0) 'Android software fallback count'
        Assert-True ([string]$json.Surface.NativeViewType -ceq 'SkiaSharp.Views.Maui.Handlers.SKGLViewHandler+MauiSKGLTextureView') 'Android MAUI native view type'
        Assert-True ([string]$json.Rid -ceq $runtimeIdentifier) 'Android runtime identifier'
        Assert-True ([string]$json.Surface.GraphicsBackend -ceq "$runtimeIdentifier/Android/MauiSKGLTextureView/OpenGL-ES-Skia") 'Android graphics backend identity'
        $screenshot = Capture-AndroidBody 'android-scroll-settled'
        Assert-True ($screenshot.nonLightRatio -gt 0.05 -and $screenshot.coloredRatio -gt 0.02) 'Android visible scroll content after repeated swipes'
        Write-Json $rawAndroidLivePath ([ordered]@{
            serial = $AndroidSerial; model = $model; api = $api; abi = $abi; rid = $runtimeIdentifier
            deviceKind = if ($qemu -eq '1') { 'emulator' } else { 'physical' }
            automatedGpu = $json
            automatedScroll = if ($RequirePhysical) { 'pass-adb-up-4-down-2-stress-48-active-samples-8-fab-action' } else { 'pass-adb-up-4-down-2' }
            automatedPersistentDisplay = [ordered]@{
                status = 'pass'
                launchScreenshot = $launchScreenshot
                initialScreenshot = $initialScreenshot
                swipeScreenshots = $scrollScreenshots
                settledScreenshot = $screenshot
            }
            manualPersistentDisplay = 'notVerified'
            imeTalkBackStylusMouse = 'notVerified'
        })
    }
    finally {
        adb -s $AndroidSerial shell am force-stop --user 0 dev.doroti.demo | Out-Null
    }
}

function Write-Evidence {
    Assert-True (Test-Path -LiteralPath $rawLivePath -PathType Leaf) 'Windows live input for evidence'
    $live = Get-Content -LiteralPath $rawLivePath -Raw | ConvertFrom-Json
    $androidLive = if (Test-Path -LiteralPath $rawAndroidLivePath -PathType Leaf) {
        Get-Content -LiteralPath $rawAndroidLivePath -Raw | ConvertFrom-Json
    } else { [ordered]@{ status = 'notVerified'; reason = 'Run AndroidLive or AndroidPhysical with an explicit serial.' } }
    $androidShaderFilterPresentation = if (
        [string]$androidLive.automatedGpu.SkiaSharpVersion -ceq '4.151.1' -and
        [long]$androidLive.automatedGpu.Frame.ShaderImageFiltersRendered -gt 0
    ) { "pass-$($androidLive.deviceKind)" } else { 'notVerified-after-4.151.1-upgrade' }
    Write-Json $evidencePath ([ordered]@{
        schemaVersion = 'doroti.app-targets-evidence/v4'
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
            sequence = @('Windows','Android','Web','MacCatalyst-cross-build','Windows-no-restore')
            windows = [ordered]@{ targetFramework='net10.0-windows10.0.19041.0';rid='win-x64' }
            android = [ordered]@{ targetFramework='net10.0-android';rids=@('android-arm64','android-x64');packaging='signed-apk-and-aab';abis=@('arm64-v8a','x86_64') }
            web = [ordered]@{ targetFramework='net10.0';rid='browser-wasm';typeScriptMsBuild='7.0.0';sourceCount=2;outputRoot='obj/web/<configuration>/net10.0/Doroti.Generated/wwwroot' }
            macCatalyst = [ordered]@{ targetFramework='net10.0-maccatalyst';rid='maccatalyst-arm64';host='windows-cross-build-only' }
            startupNegative = 'pass-failed-closed-CS0311'
            requiredRegistrationNegative = 'pass-failed-closed-CS0239'
            nativeTypeScriptPackageCount = 0
        }
        windowsLive = $live
        androidLive = $androidLive
        customShaders = [ordered]@{
            status = 'pass'
            contract = 'SkiaSharp 4.151.1 + FragmentProgram.fromSource/fromAsset + FragmentShader float uniforms/image samplers + Paint.shader/ShaderMask/ImageFilter.shader; bounded same-context GPU child capture with first-float2 size and first-sampler implicit input; Flutter Android SkSL stretch'
            compiler = 'shared Skia SKRuntimeEffect'
            targets = @('Windows','MacCatalyst','Android','Web')
            nativeLive = @('Windows','Android')
            imageFilterShader = [ordered]@{
                contract = 'pass-runtime-pixel-fixture'
                windowsNativePresentation = 'pass'
                androidNativePresentation = $androidShaderFilterPresentation
                macCatalystNativePresentation = 'notVerified'
                webBrowserPresentation = 'notVerified'
            }
            macCatalystLive = 'notVerified'
            webBrowserLive = 'notVerified'
        }
        boundaries = [ordered]@{
            pointerMousePenWheelPressure = 'implemented-not-live-verified'
            keyboardImeCursor = 'implemented-not-live-verified'
            lifecycleConfiguration = 'implemented-not-live-verified'
            nativeAccessibilityActions = 'implemented-not-live-verified'
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
if ($Shard -in @('All','Live','WindowsLive')) { Invoke-WindowsLiveGate }
if ($Shard -eq 'AndroidLive') { Invoke-AndroidLiveGate $false }
if ($Shard -eq 'AndroidPhysical') { Invoke-AndroidLiveGate $true }
if (Test-Shard 'Evidence') { Write-Evidence }
Write-Output "Doroti application target shard '$Shard': PASS"
