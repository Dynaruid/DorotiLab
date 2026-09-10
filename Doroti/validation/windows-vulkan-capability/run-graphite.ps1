param(
    [Parameter(Mandatory)][string[]]$Device,
    [string]$NativeLibrary,
    [switch]$ExtendedContext,
    [switch]$SharedSession,
    [switch]$Composition,
    [switch]$ProductPresenter,
    [ValidateRange(1,5000)][uint32]$PreparedReceiptTimeoutMs,
    [switch]$ProductHost,
    [ValidateSet('lifecycle','acrylic','device-lost')][string[]]$HostScenario = @('lifecycle'),
    [switch]$Regressions,
    [switch]$CanonicalBuilds,
    [switch]$MaterialSample,
    [ValidateSet('graphite-after-submit','graphite-after-present','graphite-device-lost')][string]$SelfTest,
    [ValidateRange(1,10)][int]$ContextCycles = 1
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$resultRoot = Join-Path $repoRoot ('Doroti/artifacts/native-graphite/runs/' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssfffZ'))
New-Item -ItemType Directory -Force $resultRoot | Out-Null

function Invoke-Bounded([string]$Label, [string]$Executable, [string[]]$Arguments, [hashtable]$EnvironmentOverrides = @{}) {
    $startInfo = [Diagnostics.ProcessStartInfo]::new($Executable)
    $startInfo.WorkingDirectory = $repoRoot
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    foreach ($argument in $Arguments) { $startInfo.ArgumentList.Add($argument) }
    foreach ($name in $EnvironmentOverrides.Keys) { $startInfo.Environment[$name] = $EnvironmentOverrides[$name] }
    $process = [Diagnostics.Process]::Start($startInfo)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    $timedOut = -not $process.WaitForExit(1200000)
    if ($timedOut) { $process.Kill($true); $process.WaitForExit() }
    $stdout.GetAwaiter().GetResult() | Set-Content (Join-Path $resultRoot "$Label.stdout.log")
    $stderr.GetAwaiter().GetResult() | Set-Content (Join-Path $resultRoot "$Label.stderr.log")
    @{ executable=$Executable; arguments=$Arguments; environmentOverrides=$EnvironmentOverrides; timeoutSeconds=1200; timedOut=$timedOut; exitCode=$process.ExitCode } |
        ConvertTo-Json -Depth 10 | Set-Content (Join-Path $resultRoot "$Label.process.json")
    $code = $process.ExitCode
    $process.Dispose()
    if ($timedOut) { throw "$Label exceeded the external 20-minute timeout. Evidence: $resultRoot" }
    return $code
}

Push-Location $repoRoot
try {
    git rev-parse HEAD | Set-Content (Join-Path $resultRoot 'commit.txt')
    git status --short | Set-Content (Join-Path $resultRoot 'status.txt')
    git diff --binary | Set-Content (Join-Path $resultRoot 'working.diff')
    Get-CimInstance Win32_OperatingSystem | Select-Object Caption,Version,BuildNumber,OSArchitecture |
        ConvertTo-Json | Set-Content (Join-Path $resultRoot 'os.json')
    Get-CimInstance Win32_VideoController | Select-Object Name,DriverVersion,PNPDeviceID |
        ConvertTo-Json | Set-Content (Join-Path $resultRoot 'gpu.json')
    $version = ([xml](Get-Content Doroti/Directory.Packages.props -Raw)).Project.ItemGroup.PackageVersion |
        Where-Object Include -EQ 'SkiaSharp' | Select-Object -ExpandProperty Version
    $packageRoot = if ($env:NUGET_PACKAGES) { $env:NUGET_PACKAGES } else { Join-Path ([Environment]::GetFolderPath('UserProfile')) '.nuget/packages' }
    $assets = foreach ($package in Get-ChildItem $packageRoot -Directory -Filter 'skiasharp*') {
        $versionRoot = Join-Path $package.FullName $version
        if (!(Test-Path $versionRoot)) { continue }
        foreach ($file in Get-ChildItem $versionRoot -Recurse -File | Where-Object { $_.Name -match '^(libSkiaSharp|SkiaSharp\.dll)' }) {
            @{ package=$package.Name; path=$file.FullName; bytes=$file.Length; sha256=(Get-FileHash $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
        }
    }
    $assets | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $resultRoot 'package-assets.json')
    Get-ChildItem Doroti/src -Directory -Filter 'Doroti.Target.*' | ForEach-Object {
        $manifest = Join-Path $_.FullName 'doroti-target-manifest.json'
        if (Test-Path $manifest) { Get-Content $manifest -Raw | ConvertFrom-Json }
    } | ConvertTo-Json -Depth 10 | Set-Content (Join-Path $resultRoot 'current-targets.json')
    $project = Join-Path $PSScriptRoot 'Doroti.Validation.WindowsVulkanCapability.csproj'
    if ((Invoke-Bounded 'build' 'dotnet' @('build',$project,'-c','Release','--nologo')) -ne 0) {
        throw "Probe build failed. Evidence: $resultRoot"
    }
    $exe = Join-Path $PSScriptRoot 'bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.WindowsVulkanCapability.exe'
    if ($ProductPresenter) {
        $productProject = Join-Path $PSScriptRoot '../windowsappsdk-native-abi/Doroti.Validation.WindowsAppSdkNativeAbi.csproj'
        if ((Invoke-Bounded 'product-build' 'dotnet' @('build',$productProject,'-c','Release','--nologo')) -ne 0) {
            throw "Product presenter fixture build failed. Evidence: $resultRoot"
        }
        $productExe = Join-Path $PSScriptRoot '../windowsappsdk-native-abi/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.WindowsAppSdkNativeAbi.exe'
    }
    if ($ProductHost) {
        $hostProject = Join-Path $PSScriptRoot '../hwnd-exact-cpp-product/Doroti.Validation.HwndExactCppProduct.csproj'
        if ((Invoke-Bounded 'host-build' 'dotnet' @('build',$hostProject,'-c','Release','--nologo')) -ne 0) {
            throw "Product host fixture build failed. Evidence: $resultRoot"
        }
        $hostExe = Join-Path $PSScriptRoot '../hwnd-exact-cpp-product/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.HwndExactCppProduct.exe'
    }
    if ($Regressions) {
        foreach ($contract in @('runtime-shader-contract/Doroti.Validation.RuntimeShaderContract.csproj', 'fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj')) {
            $label = ($contract -split '/')[0]
            $contractPath = Join-Path $PSScriptRoot "../$contract"
            if ((Invoke-Bounded $label 'dotnet' @('run','--project',$contractPath,'-c','Release')) -ne 0) {
                throw "Shared regression failed: $label. Evidence: $resultRoot"
            }
        }
    }
    $canonicalRunner = Join-Path $repoRoot 'Doroti/eng/doroti.ps1'
    $sampleApp = Join-Path $repoRoot 'DorotiTestbedApp/DorotiTestbedApp.csproj'
    if ($CanonicalBuilds) {
        foreach ($platform in @('windows', 'web')) {
            if ((Invoke-Bounded "canonical-build-$platform" 'pwsh' @('-NoProfile','-File',$canonicalRunner,'build','-App',$sampleApp,'-Platform',$platform)) -ne 0) {
                throw "Canonical $platform build failed. Evidence: $resultRoot"
            }
        }
    }
    $index = 0
    $failed = $false
    foreach ($selector in $Device) {
        $common = @('--device',$selector)
        $baseline = Invoke-Bounded "baseline-$index" $exe ($common + @('--output',(Join-Path $resultRoot "baseline-$index.json")))
        $probeArgs = $common + @('--graphite','--output',(Join-Path $resultRoot "graphite-$index.json"))
        if ($NativeLibrary) { $probeArgs += @('--graphite-native',[IO.Path]::GetFullPath($NativeLibrary)) }
        if ($ExtendedContext) { $probeArgs += '--graphite-extended-context' }
        if ($SharedSession) { $probeArgs += '--graphite-session' }
        if ($Composition) { $probeArgs += '--graphite-composition' }
        if ($SelfTest) { $probeArgs += @('--self-test', $SelfTest) }
        $probeArgs += @('--graphite-context-cycles', "$ContextCycles")
        $probe = Invoke-Bounded "graphite-$index" $exe $probeArgs
        # An intentional failure must keep its raw exit 1 and match the original
        # injected error; an arbitrary crash is not a successful negative contract.
        $injectedFailure = $SelfTest -in @('graphite-after-submit', 'graphite-after-present')
        $expectedCode = if ($injectedFailure) { 1 } else { 2 }
        if ($baseline -ne 0 -or $probe -ne $expectedCode) { $failed = $true }
        if ($injectedFailure) {
            $result = Get-Content (Join-Path $resultRoot "graphite-$index.json") -Raw | ConvertFrom-Json
            $expectedError = if ($SelfTest -eq 'graphite-after-submit') { '*Injected Graphite failure after asynchronous submit*' }
                else { '*Injected Graphite failure after Composition present*' }
            if ($result.error -notlike $expectedError -or
                $result.validationErrors -ne 0 -or $result.validationWarnings -ne 0) { $failed = $true }
        }
        if ($ProductPresenter) {
            $preparedArgs = @('--prepared-frame')
            if ($PSBoundParameters.ContainsKey('PreparedReceiptTimeoutMs')) {
                $preparedArgs += @('--receipt-timeout-ms',"$PreparedReceiptTimeoutMs")
            }
            if ((Invoke-Bounded "product-baseline-$index" $productExe $preparedArgs @{
                DOROTI_WINDOWS_GRAPHITE='0'; DOROTI_WINDOWS_VULKAN_DEVICE=$selector
            }) -ne 0) { $failed = $true }
            $productEnvironment = @{
                DOROTI_WINDOWS_GRAPHITE='1'; DOROTI_WINDOWS_GRAPHITE_NATIVE=[IO.Path]::GetFullPath($NativeLibrary)
                DOROTI_WINDOWS_VULKAN_DEVICE=$selector
            }
            if ((Invoke-Bounded "product-$index" $productExe $preparedArgs $productEnvironment) -ne 0) { $failed = $true }
        }
        if ($ProductHost) {
            foreach ($renderer in @('0', '1')) {
                $hostEnvironment = @{
                    DOROTI_WINDOWS_GRAPHITE=$renderer; DOROTI_WINDOWS_GRAPHITE_NATIVE=[IO.Path]::GetFullPath($NativeLibrary)
                    DOROTI_WINDOWS_VULKAN_DEVICE=$selector
                }
                foreach ($scenario in $HostScenario) {
                    $hostArgs = @('--presenter','Vulkan','--no-resize-burst','--device-resets','2','--lifecycle-cycles','2','--smoke-ms','5000',
                        '--report',(Join-Path $resultRoot "host-$index-$renderer-$scenario.json"))
                    if ($scenario -eq 'acrylic') { $hostArgs += '--acrylic' }
                    if ($scenario -eq 'device-lost') { $hostArgs += @('--inject-vulkan-result','DEVICE_LOST') }
                    if ((Invoke-Bounded "host-$index-$renderer-$scenario" $hostExe $hostArgs $hostEnvironment) -ne 0) { $failed = $true }
                }
            }
        }
        if ($MaterialSample) {
            foreach ($renderer in @('0', '1')) {
                $sampleEnvironment = @{
                    DOROTI_WINDOWS_GRAPHITE=$renderer; DOROTI_WINDOWS_GRAPHITE_NATIVE=[IO.Path]::GetFullPath($NativeLibrary)
                    DOROTI_WINDOWS_VULKAN_DEVICE=$selector; DOROTI_TESTBED_MODE='sample'; DOROTI_RESIZE_FIXTURE='none'
                    DOROTI_WINDOWS_APPSDK_SMOKE_MS='7000'; DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1'
                }
                if ((Invoke-Bounded "material-$index-$renderer" 'pwsh' @('-NoProfile','-File',$canonicalRunner,'run','-App',$sampleApp,
                    '-Platform','windows','-NoBuild') $sampleEnvironment) -ne 0) { $failed = $true }
            }
        }
        $index++
    }
    Write-Output "Evidence: $resultRoot"
    if ($failed) { exit 1 }
    # Exit 2 explicitly preserves PARTIAL, even if every diagnostic check passed.
    exit 2
}
finally { Pop-Location }
