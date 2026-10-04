# Read-only diagnostics. Imported by doroti.ps1 so workspace/runner selection is shared.
if (-not ('Doroti.Doctor.OutputDrain' -as [type])) {
    Add-Type -TypeDefinition @'
using System.IO;
using System.Text;
using System.Threading.Tasks;
namespace Doroti.Doctor {
    public static class OutputDrain {
        public static async Task<string> ReadAsync(StreamReader reader, int limit) {
            var output = new StringBuilder(); var buffer = new char[4096]; bool truncated = false;
            int count;
            while ((count = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0) {
                int keep = System.Math.Min(count, limit - output.Length);
                if (keep > 0) output.Append(buffer, 0, keep);
                if (keep < count) truncated = true;
            }
            return output.ToString() + (truncated ? " [truncated]" : "");
        }
    }
}
'@
}
function Invoke-DorotiProbe {
    param([string] $File, [string[]] $Arguments, [string] $WorkingDirectory, [int] $TimeoutSeconds = 15,
        [Threading.CancellationToken] $CancellationToken = [Threading.CancellationToken]::None)
    $clock = [Diagnostics.Stopwatch]::StartNew()
    if ($doctorDeadline) { $TimeoutSeconds = [Math]::Min($TimeoutSeconds, [Math]::Max(1, [Math]::Floor(($doctorDeadline - [DateTime]::UtcNow).TotalSeconds))) }
    $process = $null
    $result = [ordered]@{ available=$false; status='FAIL'; reason=$null; toolPath=$File; command=@($File)+@($Arguments); cwd=$WorkingDirectory; exitCode=$null; durationMs=0; output=''; stderr='' }
    try {
        if ($CancellationToken.IsCancellationRequested -or ($DoctorCancellationFile -and (Test-Path -LiteralPath $DoctorCancellationFile))) { $result.reason='canceled'; return $result }
        if ($doctorDeadline -and [DateTime]::UtcNow -ge $doctorDeadline) { $result.reason='doctorTimeout'; return $result }
        $tool = Get-Command $File -CommandType Application -ErrorAction Stop | Select-Object -First 1
        $result.toolPath = $tool.Source
        $info = [Diagnostics.ProcessStartInfo]::new($tool.Source)
        $info.WorkingDirectory = $WorkingDirectory
        $info.UseShellExecute = $false; $info.CreateNoWindow = $true
        $info.RedirectStandardOutput = $true; $info.RedirectStandardError = $true
        $info.Environment['DOTNET_CLI_UI_LANGUAGE'] = 'en'
        foreach ($argument in $Arguments) { $info.ArgumentList.Add($argument) }
        $process = [Diagnostics.Process]::Start($info)
        $stdout = [Doroti.Doctor.OutputDrain]::ReadAsync($process.StandardOutput, 8192)
        $stderr = [Doroti.Doctor.OutputDrain]::ReadAsync($process.StandardError, 8192)
        $completed=$false
        while (-not ($completed=$process.WaitForExit(100)) -and $clock.Elapsed.TotalSeconds -lt $TimeoutSeconds) {
            if ($CancellationToken.IsCancellationRequested -or ($DoctorCancellationFile -and (Test-Path -LiteralPath $DoctorCancellationFile))) { $result.reason='canceled'; break }
        }
        if (-not $completed) {
            if (-not $result.reason) {$result.reason = 'timeout'}
            $process.Kill($true); $process.WaitForExit(5000) | Out-Null
        } else { $result.exitCode = $process.ExitCode; $result.reason = if ($process.ExitCode -eq 0) { 'completed' } else { 'nonzeroExit' } }
        if ([Threading.Tasks.Task]::WaitAll([Threading.Tasks.Task[]]@($stdout, $stderr), 5000)) {
            $result.output = $stdout.Result.Trim(); $result.stderr = $stderr.Result.Trim()
            foreach ($name in @('output','stderr')) { if ($result[$name].Length -gt 8192) { $result[$name] = $result[$name].Substring(0,8192) + ' [truncated]' } }
        } else { if ($result.reason -notin @('timeout','canceled')) {$result.reason = 'outputTimeout'} }
        $result.available = $result.reason -eq 'completed'
        if ($result.available) { $result.status = 'PASS' }
    } catch {
        $result.reason = if ($null -eq $process) { 'missingExecutable' } else { 'probeFailure' }
        $result.stderr = $_.Exception.Message
    } finally {
        if ($null -ne $process) { if (-not $process.HasExited) { $process.Kill($true) }; $process.Dispose() }
        $result.durationMs = $clock.ElapsedMilliseconds
    }
    return $result
}

function Test-DorotiSdkPolicy($Selected, $Policy) {
    if ($Selected -notmatch '^(\d+)\.(\d+)\.(\d+)(-.+)?$') { return $false }
    $actual = [version]($Selected.Split('-')[0]); $prerelease = $Selected.Contains('-')
    if ($null -eq $Policy) { return $true }
    if ($prerelease -and $Policy.allowPrerelease -eq $false) { return $false }
    $requested = [version]($Policy.version.Split('-')[0])
    if ($actual -lt $requested) { return $false }
    $sameBand = $actual.Major -eq $requested.Major -and $actual.Minor -eq $requested.Minor -and [math]::Floor($actual.Build/100) -eq [math]::Floor($requested.Build/100)
    switch ($Policy.rollForward) {
        'disable' { return $Selected -ceq $Policy.version }
        { $_ -in @('feature','latestFeature') } { return $actual.Major -eq $requested.Major -and $actual.Minor -eq $requested.Minor }
        { $_ -in @('minor','latestMinor') } { return $actual.Major -eq $requested.Major }
        { $_ -in @('major','latestMajor') } { return $true }
        default { return $sameBand }
    }
}

function Invoke-DorotiDoctorV4 {
    param([switch] $NativeBinding)
    $checks = [Collections.Generic.List[object]]::new()
    $doctorDeadline = [DateTime]::UtcNow.AddSeconds(1100)
    $contexts = [Collections.Generic.List[object]]::new()
    $profile = if ($DoctorProfile) { $DoctorProfile } elseif ($App -or $Platform) { 'build' } else { 'common' }
    $scopeName = if ($NativeBinding) { 'native build prerequisites' } elseif ($profile -eq 'common') { 'common tools' } else { "$profile prerequisites" }
    $context = [ordered]@{ app=$App; platform=$Platform; windowsBackend=$WindowsBackend; configuration=$Configuration; rid=$Rid; compilationMode=$CompilationMode;
        profile=$profile; nativeBinding=[bool]$NativeBinding; validationSuite=$ValidationSuite; device=$Device; rootCwd=(Get-Location).Path; repository=$repositoryRoot; targets=$contexts;
        iosTargetFramework=$IosTargetFramework; macOSTargetFramework=$MacOSTargetFramework; macCatalystTargetFramework=$MacCatalystTargetFramework }
    function Add-DoctorCheck([string]$Id, [string]$Scope, [bool]$Required, [string]$Status, $Expected, $Actual, [string]$Action, $Probe=$null) {
        $checks.Add([ordered]@{ id=$Id; scope=$Scope; required=$Required; status=$Status; expected=$Expected; actual=$Actual;
            toolPath=if ($Probe) {$Probe.toolPath} else {$null}; command=if ($Probe) {$Probe.command} else {@()}; cwd=if ($Probe) {$Probe.cwd} else {$null};
            exitCode=if ($Probe) {$Probe.exitCode} else {$null}; durationMs=if ($Probe) {$Probe.durationMs} else {0};
            reason=if ($Probe) {$Probe.reason} else {$null}; evidence=if ($Probe) {$Probe.output + "`n" + $Probe.stderr} else {$Actual}; action=$Action })
    }
    function Probe-DoctorTool([string]$Id, [string]$File, [string[]]$Arguments, [string]$Cwd, [bool]$Required=$true) {
        $probe = Invoke-DorotiProbe $File $Arguments $Cwd $DoctorProbeTimeoutSeconds
        Add-DoctorCheck $Id $scopeName $Required $(if ($probe.available) {'PASS'} elseif ($Required) {'FAIL'} else {'WARN'}) 'executable succeeds within probe limit' $probe.output "Install/configure $File; see recorded command and working directory." $probe
        return $probe
    }
    function Check-DoctorPath([string]$Id, [string]$Path, [bool]$Required=$true, [string]$Expected='existing local prerequisite') {
        $exists = $Path -and (Test-Path -LiteralPath $Path)
        Add-DoctorCheck $Id $scopeName $Required $(if ($exists) {'PASS'} elseif ($Required) {'FAIL'} else {'WARN'}) $Expected $Path "Prepare the local prerequisite: $Path"
    }
    $dotnetResult=$null; $workloadsResult=$null; $xcodeResult=$null; $macosSdkResult=$null
    try {
        Add-DoctorCheck 'powershell' 'common tools' $true $(if ($PSVersionTable.PSVersion.Major -ge 7) {'PASS'} else {'FAIL'}) 'PowerShell 7+' $PSVersionTable.PSVersion.ToString() 'Install PowerShell 7.'
        $workspace = $null; $nativeWorkspace = $null
        if ($App) { $workspace = Resolve-DorotiWorkspace $App -AllowAllPlatforms }
        if ($NativeBinding) { $nativeWorkspace = Resolve-DorotiNativeWorkspace }
        if ($profile -notin @('common','compiler-development') -and -not $workspace) { throw 'A target profile requires -App and a declared workspace.' }
        $selected = if ($profile -in @('common','compiler-development')) { @('common') } elseif ($Platform -and $Platform -ne 'all') { @($Platform) } else { @($workspace.Runners.Keys) }
        if ($profile -eq 'compiler-development') {
            $compilerRoot = Join-Path $repositoryRoot 'tools/Doroti.Wgsl'
            $toolchainFile = Join-Path $compilerRoot 'rust-toolchain.toml'
            $context.compilerToolchainFile = $toolchainFile
            $toolchainText = Get-Content -LiteralPath $toolchainFile -Raw
            if ($toolchainText -notmatch '(?m)^channel\s*=\s*"([^"]+)"') { throw 'Cannot read the WGSL compiler pinned Rust toolchain.' }
            $channel = $Matches[1]
            $inventory = Probe-DoctorTool 'compiler.rustup' 'rustup' @('toolchain','list') $compilerRoot
            $installed = $inventory.available -and ($inventory.output -split '\r?\n' | Where-Object { $_ -match ('^' + [regex]::Escape($channel) + '(-|\s|$)') })
            Add-DoctorCheck 'compiler.toolchain' $scopeName $true $(if ($installed) {'PASS'} else {'FAIL'}) $channel $inventory.output 'Install the pinned toolchain separately; doctor does not download it.' $inventory
            if ($installed) {
                Probe-DoctorTool 'compiler.rustc' 'rustup' @('run',$channel,'rustc','--version') $compilerRoot | Out-Null
                Probe-DoctorTool 'compiler.cargo' 'rustup' @('run',$channel,'cargo','--version') $compilerRoot | Out-Null
            }
        }
        if ($profile -in @('dev','validation','release')) {
            try { $python = Resolve-DorotiPython; Probe-DoctorTool 'python' $python @('--version') $repositoryRoot | Out-Null }
            catch { Add-DoctorCheck 'python' $scopeName $true 'FAIL' 'Python 3' $_.Exception.Message 'Install python3 or python.' }
        }
        if ($profile -eq 'validation' -and $ValidationSuite -in @('Developer','Release','Targets')) { Probe-DoctorTool 'node' 'node' @('--version') $repositoryRoot | Out-Null }
        foreach ($alias in $selected) {
            try {
                $runner = if ($alias -ne 'common') { Resolve-WorkspaceRunner $workspace $alias $WindowsBackend } else { $null }
                $cwd = if ($NativeBinding) { $nativeWorkspace.PlatformRoot } elseif ($alias -eq 'ios') { Split-Path -Parent $runner } elseif ($workspace) { $workspace.Root } else { (Get-Location).Path }
                $target = [ordered]@{ platform=$alias; runner=$runner; cwd=$cwd; dotnetPath=$DotnetPath; sdk=$null; globalJson=$null; sdkPolicy=$null; properties=$null; runtimeAcceptance='notVerified' }
                $contexts.Add($target)
                $version = Probe-DoctorTool "$alias.sdk" $DotnetPath @('--version') $cwd
                $dotnetResult=$version; $target.dotnetPath=$version.toolPath; $target.sdk=$version.output
                $info = Probe-DoctorTool "$alias.sdk-info" $DotnetPath @('--info') $cwd
                $target.sdkInfo = $info.output
                Probe-DoctorTool "$alias.installed-sdks" $DotnetPath @('--list-sdks') $cwd | Out-Null
                for ($directory=[IO.DirectoryInfo]::new($cwd); $null -ne $directory; $directory=$directory.Parent) {
                    $global = Join-Path $directory.FullName 'global.json'
                    if (Test-Path -LiteralPath $global) { $target.globalJson=$global; $target.sdkPolicy=(Get-Content -LiteralPath $global -Raw | ConvertFrom-Json).sdk; break }
                }
                $policyOK = $version.available -and (Test-DorotiSdkPolicy $version.output $target.sdkPolicy)
                Add-DoctorCheck "$alias.sdk-policy" $scopeName $true $(if ($policyOK) {'PASS'} else {'FAIL'}) $target.sdkPolicy $version.output 'Install the requested SDK or correct this working-directory global.json policy.'
                if ($alias -eq 'common') {
                    $workloadsResult=Invoke-DorotiProbe $DotnetPath @('workload','list') $cwd $DoctorProbeTimeoutSeconds
                    Add-DoctorCheck 'common.workload-information' 'common tools' $false $(if ($workloadsResult.available) {'PASS'} else {'WARN'}) 'informational workload inventory' $workloadsResult.output 'Target profiles check their own required workloads.' $workloadsResult
                    continue
                }
                $foreign = ($alias -in @('ios','macos','maccatalyst') -and -not $IsMacOS) -or ($alias -eq 'windows' -and -not $IsWindows)
                Add-DoctorCheck "$alias.host" $scopeName $true $(if ($foreign) {'notVerified'} else {'PASS'}) "supported $alias build host" ([Runtime.InteropServices.RuntimeInformation]::OSDescription) "Run this target's prerequisites and acceptance on its supported host."
                $mode = Resolve-DorotiCompilationMode $alias $Configuration $CompilationMode $Rid
                $effectiveRid = if ($mode -eq 'NativeAot' -and -not $Rid) { 'ios-arm64' } else { $Rid }
                $target.compilationMode=$mode; $target.rid=$effectiveRid
                if ($mode -eq 'NativeAot' -and ($alias -ne 'ios' -or $effectiveRid -ne 'ios-arm64')) { throw 'NativeAot requires iOS ios-arm64.' }
                $names = 'TargetFramework,TargetPlatformVersion,RuntimeIdentifier,DorotiHostKind,NETCoreSdkVersion,JavaSdkDirectory,AndroidSdkDirectory,AndroidApiLevel,AndroidSdkBuildToolsVersion,UseMaui,WasmBuildNative,WasmEnableThreads,TypeScriptMSBuildVersion,DorotiTypeScriptVersion,DorotiTypeScriptCompilerExecutable,DorotiQtQuick,DorotiQtWebEngine,DorotiGStreamerTextures,DorotiBuildQtNative,DorotiQtNativeBuildDirectory,DorotiWgslTool,UseMonoRuntime,RunAOTCompilation,PublishAot,PublishTrimmed,Optimize,StartupHookSupport,NuGetPackageRoot'
                $arguments = @('msbuild', $(if ($NativeBinding) {$nativeWorkspace.Binding} else {$runner}), '-nologo', "-p:Configuration=$Configuration", "-getProperty:$names", '-getItem:ProjectReference,PackageReference,DorotiGpuEffect')
                if ($effectiveRid) { $arguments += "-p:RuntimeIdentifier=$effectiveRid" }
                if ($alias -eq 'ios' -and $IosTargetFramework) { $arguments += "-p:DorotiIosTargetFramework=$IosTargetFramework" }
                if ($alias -eq 'macos' -and $MacOSTargetFramework) { $arguments += "-p:DorotiMacOSTargetFramework=$MacOSTargetFramework" }
                if ($alias -eq 'maccatalyst' -and $MacCatalystTargetFramework) { $arguments += "-p:DorotiMacCatalystTargetFramework=$MacCatalystTargetFramework" }
                if ($profile -eq 'dev' -and $alias -eq 'android') { $arguments += '-p:DorotiAndroidDevelopment=true' }
                if ($mode) { $arguments += "-p:DorotiCompilationMode=$mode" }
                $evaluation = Invoke-DorotiProbe $DotnetPath $arguments $cwd $DoctorProbeTimeoutSeconds
                $props=$null; $items=$null
                if ($evaluation.available) { try { $evaluated=$evaluation.output | ConvertFrom-Json; $props=$evaluated.Properties; $items=$evaluated.Items } catch { $evaluation.reason='parseFailure' } }
                Add-DoctorCheck "$alias.project-evaluation" $scopeName $true $(if ($props) {'PASS'} else {'notVerified'}) 'read-only MSBuild evaluation; no restore/build' $evaluation.output 'Restore imports/dependencies separately, then repeat doctor; no build is performed by doctor.' $evaluation
                $target.properties=$props
                $target.packages=@($items.PackageReference | ForEach-Object {[ordered]@{id=$_.Identity; version=if ($_.VersionOverride) {$_.VersionOverride} else {$_.Version}}})
                $identity=[ordered]@{}
                foreach ($input in @($workspace.Manifest,$runner,$target.globalJson) | Where-Object {$_}) { $identity[$input]=(Get-FileHash -LiteralPath $input -Algorithm SHA256).Hash.ToLowerInvariant() }
                $target.inputSha256=$identity
                if (-not $props) { continue }
                if ($props.RuntimeIdentifier) { $target.rid=$props.RuntimeIdentifier }
                if ($effectiveRid -and $effectiveRid -notmatch $(switch ($alias) {'windows' {'^win-(x64|arm64)$'}
 'web' {'^browser-wasm$'}
 'android' {'^android-(arm64|x64)$'}
 'linux' {'^linux-(x64|arm64)$'}
 'ios' {'^ios(simulator)?-(arm64|x64)$'}
 default {'^(osx|maccatalyst)-(arm64|x64)$'}})) { throw "Unsupported $alias RID: $effectiveRid" }
                $workloadsResult = Invoke-DorotiProbe $DotnetPath @('workload','list') $cwd $DoctorProbeTimeoutSeconds
                $requiredWorkload = switch ($alias) {'android' {'android'}
 'ios' {'ios'}
 'macos' {'macos'}
 'maccatalyst' {'maccatalyst'}
 'web' {if ($props.WasmBuildNative -eq 'true' -or $props.RunAOTCompilation -eq 'true') {'wasm-tools'}}
 'windows' {if ($WindowsBackend -eq 'Maui') {'maui-windows'}}}
                if ($requiredWorkload) {
                    $installed = $workloadsResult.available -and ($workloadsResult.output -match "(?m)^\s*(maui(-$requiredWorkload)?|$requiredWorkload)\s")
                    Add-DoctorCheck "$alias.workload" $scopeName $true $(if ($installed) {'PASS'} else {'FAIL'}) $requiredWorkload $workloadsResult.output "Install the $requiredWorkload workload for SDK $($target.sdk) using the recorded dotnet path/CWD." $workloadsResult
                }
                if ($items.DorotiGpuEffect.Count -gt 0) { Check-DoctorPath "$alias.wgsl-compiler" $props.DorotiWgslTool }
                if (-not $NativeBinding) {
                    $appEvaluation=Invoke-DorotiProbe $DotnetPath @('msbuild',$workspace.ApplicationProject,'-nologo',"-p:Configuration=$Configuration",'-getProperty:DorotiWgslTool','-getItem:DorotiGpuEffect,DorotiAppResource') $cwd $DoctorProbeTimeoutSeconds
                    $appData=$null
                    if ($appEvaluation.available) { try {$appData=$appEvaluation.output | ConvertFrom-Json} catch {$appEvaluation.reason='parseFailure'} }
                    Add-DoctorCheck "$alias.app-evaluation" $scopeName $true $(if ($appData) {'PASS'} else {'notVerified'}) 'application generation/resource graph' $appEvaluation.output 'Prepare application imports and repeat doctor.' $appEvaluation
                    if ($appData.Items.DorotiGpuEffect.Count -gt 0) {Check-DoctorPath "$alias.app-wgsl-compiler" $appData.Properties.DorotiWgslTool}
                    foreach ($resource in $appData.Items.DorotiAppResource) { if ($resource.FullPath) {Check-DoctorPath "$alias.resource.$($resource.Identity)" $resource.FullPath} }
                }
                switch ($alias) {
                    'web' {
                        Add-DoctorCheck 'web.typescript-version' $scopeName $true $(if ($props.TypeScriptMSBuildVersion -and $props.TypeScriptMSBuildVersion -eq $props.DorotiTypeScriptVersion) {'PASS'} else {'notVerified'}) $props.DorotiTypeScriptVersion $props.TypeScriptMSBuildVersion 'Restore the selected Microsoft.TypeScript.MSBuild package.'
                        Check-DoctorPath 'web.typescript-compiler' $props.DorotiTypeScriptCompilerExecutable
                        Add-DoctorCheck 'web.runtime-profile' 'runtime acceptance' $false 'notVerified' 'matching thread/ownership profile and COOP/COEP/CSP/browser adapter' $props.WasmEnableThreads 'Verify the deployed matching profile and browser startup separately.'
                    }
                    'windows' {
                        if ($WindowsBackend -eq 'WindowsAppSdk') {
                            Add-DoctorCheck 'windows.native-rid' $scopeName $true $(if ($target.rid -eq 'win-x64') {'PASS'} else {'FAIL'}) 'current C++ native host win-x64 profile' $target.rid 'Use the supported win-x64 native host profile.'
                            $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
                            $vs = Probe-DoctorTool 'windows.vswhere' $vswhere @('-latest','-products','*','-requires','Microsoft.VisualStudio.Component.VC.Tools.x86.x64','-property','installationPath') $cwd
                            if ($vs.available -and $vs.output) {
                                $vsRoot=$vs.output.Split("`n")[0].Trim()
                                Check-DoctorPath 'windows.msbuild' (Join-Path $vsRoot 'MSBuild/Current/Bin/MSBuild.exe')
                                [xml]$nativeProject=Get-Content (Join-Path $dorotiRoot 'src/Doroti.Host.WindowsAppSdk.Native/Doroti.Host.WindowsAppSdk.Native.vcxproj') -Raw
                                $toolset=@($nativeProject.Project.PropertyGroup.PlatformToolset | Where-Object {$_})[0]
                                $toolsetFile=@(Get-ChildItem -Path (Join-Path $vsRoot "MSBuild/Microsoft/VC/*/Platforms/x64/PlatformToolsets/$toolset/Toolset.props") -File -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName -First 1)
                                Check-DoctorPath 'windows.toolset' ([string]($toolsetFile | Select-Object -First 1)) $true "project PlatformToolset=$toolset"
                            }
                            [xml]$nativeVersions=Get-Content (Join-Path $dorotiRoot 'src/Doroti.Host.WindowsAppSdk.Native/Doroti.NativeVersions.props') -Raw
                            $sdkVersion=$nativeVersions.Project.PropertyGroup.DorotiNativeWindowsSdkVersion
                            Check-DoctorPath 'windows.native-sdk' (Join-Path ${env:ProgramFiles(x86)} "Windows Kits/10/Include/$sdkVersion/um/Windows.h") $true "native SDK $sdkVersion"
                            Check-DoctorPath 'windows.cppwinrt' (Join-Path ${env:ProgramFiles(x86)} "Windows Kits/10/bin/$sdkVersion/x64/cppwinrt.exe")
                            Check-DoctorPath 'windows.winmd' (Join-Path ${env:ProgramFiles(x86)} "Windows Kits/10/UnionMetadata/$sdkVersion/Windows.winmd")
                            $packageRoot=$props.NuGetPackageRoot
                            if (-not $packageRoot) { $packageRoot=if ($env:NUGET_PACKAGES) {$env:NUGET_PACKAGES} else {Join-Path $HOME '.nuget/packages'} }
                            foreach ($package in @(@{Name='interactiveexperiences'; Version=$nativeVersions.Project.PropertyGroup.DorotiNativeInteractiveVersion},@{Name='foundation'; Version=$nativeVersions.Project.PropertyGroup.DorotiNativeFoundationVersion},@{Name='runtime'; Version=$nativeVersions.Project.PropertyGroup.DorotiNativeRuntimeVersion})) {
                                Check-DoctorPath "windows.package.$($package.Name)" (Join-Path $packageRoot "microsoft.windowsappsdk.$($package.Name)/$($package.Version)")
                            }
                        } else {
                            Add-DoctorCheck 'windows.maui-backend' $scopeName $true $(if ($props.DorotiHostKind -eq 'Maui') {'PASS'} else {'FAIL'}) 'Maui runner' $props.DorotiHostKind 'Select the workspace Windows MAUI runner.'
                        }
                    }
                    'android' {
                        $nativeRoot=Join-Path $workspace.Root 'android/native'
                        $wrapper=Join-Path $nativeRoot $(if ($IsWindows) {'gradlew.bat'} else {'gradlew'})
                        Check-DoctorPath 'android.gradle-wrapper' $wrapper
                        Check-DoctorPath 'android.gradle-wrapper-jar' (Join-Path $nativeRoot 'gradle/wrapper/gradle-wrapper.jar')
                        Check-DoctorPath 'android.gradle-wrapper-config' (Join-Path $nativeRoot 'gradle/wrapper/gradle-wrapper.properties')
                        if (-not $IsWindows -and (Test-Path $wrapper)) { Probe-DoctorTool 'android.wrapper-executable' 'test' @('-x',$wrapper) $cwd | Out-Null }
                        $javaRoot=$props.JavaSdkDirectory
                        if (-not $javaRoot) { try {$javaRoot=(Resolve-AndroidJavaHome).Home} catch { Add-DoctorCheck 'android.jdk-selection' $scopeName $true 'FAIL' 'evaluated JavaSdkDirectory or supported local JDK' $_.Exception.Message 'Configure JavaSdkDirectory/JAVA_HOME.' } }
                        if ($javaRoot) {
                            $java=Probe-DoctorTool 'android.java' (Join-Path $javaRoot "bin/java$(if ($IsWindows) {'.exe'})") @('-version') $cwd
                            Add-DoctorCheck 'android.jdk-version' $scopeName $true $(if (($java.output+$java.stderr) -match 'version "(17|18|19|20|21)(\.|\")' -and ($java.output+$java.stderr) -notmatch 'GraalVM') {'PASS'} else {'FAIL'}) 'OpenJDK 17-21 (Android launcher policy)' ($java.output+$java.stderr) 'Select the same supported JDK as the Android build.'
                            $target.javaHome=$javaRoot; $target.ambientJavaHome=$env:JAVA_HOME
                        }
                        $androidSdk=$props.AndroidSdkDirectory
                        if (-not $androidSdk) {$androidSdk=if ($env:ANDROID_SDK_ROOT) {$env:ANDROID_SDK_ROOT} elseif ($env:ANDROID_HOME) {$env:ANDROID_HOME} elseif ($IsWindows) {Join-Path $env:LOCALAPPDATA 'Android/Sdk'} else {Join-Path $HOME 'Android/Sdk'}}
                        $gradle=Join-Path $nativeRoot 'bridge/build.gradle.kts'; $bridgeApi=$null
                        if (Test-Path $gradle) { $text=Get-Content $gradle -Raw; if ($text -match 'compileSdk\s*=\s*(\d+)') {$bridgeApi=$Matches[1]} }
                        if ($bridgeApi) {Check-DoctorPath 'android.bridge-platform' (Join-Path $androidSdk "platforms/android-$bridgeApi/android.jar")}
                        $api=if ($props.AndroidApiLevel) {$props.AndroidApiLevel} elseif ($props.TargetPlatformVersion) {$props.TargetPlatformVersion.Split('.')[0]} else {$null}
                        if ($api) {Check-DoctorPath 'android.dotnet-platform' (Join-Path $androidSdk "platforms/android-$api/android.jar")}
                        else {Add-DoctorCheck 'android.dotnet-platform' $scopeName $true 'notVerified' 'evaluated target Android API platform' $null 'Evaluate the selected Android workload/target API.'}
                        if ($props.AndroidSdkBuildToolsVersion) {Check-DoctorPath 'android.build-tools' (Join-Path $androidSdk "build-tools/$($props.AndroidSdkBuildToolsVersion)")}
                        else {Add-DoctorCheck 'android.build-tools' $scopeName $true 'notVerified' 'selected Build Tools version' $null 'Qualify Build Tools selection during the Android build; doctor does not install packages.'}
                        if ($profile -eq 'dev') {
                            $adb=Join-Path $androidSdk "platform-tools/adb$(if ($IsWindows) {'.exe'})"
                            $devices=Probe-DoctorTool 'android.devices' $adb @('devices') $cwd
                            $authorized=@([regex]::Matches($devices.output,'(?m)^(\S+)\s+device\s*$') | ForEach-Object {$_.Groups[1].Value})
                            $deviceOK=if ($Device) {$Device -in $authorized} else {$authorized.Count -eq 1}
                            Add-DoctorCheck 'android.selected-device' $scopeName $true $(if ($deviceOK) {'PASS'} else {'FAIL'}) 'one selected authorized device' $authorized 'Connect/authorize a device or specify -Device.'
                            $debugOK=$Configuration -eq 'Debug' -and $props.UseMonoRuntime -eq 'true' -and $props.StartupHookSupport -eq 'true' -and @('RunAOTCompilation','PublishAot','PublishTrimmed','Optimize').Where({$props.$_ -eq 'true'}).Count -eq 0
                            Add-DoctorCheck 'android.metadata-profile' $scopeName $true $(if ($debugOK) {'PASS'} else {'FAIL'}) 'unoptimized/untrimmed Debug Mono with startup hooks and no AOT' $props 'Use -Configuration Debug and DorotiAndroidDevelopment=true.'
                        }
                    }
                    {$_ -in @('ios','macos','maccatalyst')} {
                        if (-not $foreign) {
                            $xcode=Probe-DoctorTool "$alias.xcode-selection" 'xcode-select' @('-p') $cwd
                            $xcodeResult=Probe-DoctorTool "$alias.xcode-version" 'xcodebuild' @('-version') $cwd
                            Add-DoctorCheck "$alias.full-xcode" $scopeName $true $(if ($xcode.output -match '\.app/Contents/Developer$') {'PASS'} else {'FAIL'}) 'selected full Xcode installation' $xcode.output 'Select a full supported Xcode, not CommandLineTools.'
                            $sdk=if ($alias -eq 'ios') {if ($target.rid -match 'simulator') {'iphonesimulator'} else {'iphoneos'}} else {'macosx'}
                            $macosSdkResult=Probe-DoctorTool "$alias.native-sdk" 'xcrun' @('--sdk',$sdk,'--show-sdk-version') $cwd
                            if ($profile -eq 'release' -and $alias -eq 'ios' -and $target.rid -notmatch 'simulator') {
                                Add-DoctorCheck "$alias.signing" $scopeName $true 'notVerified' 'selected provisioning/signing for device deployment' 'Signing secrets are not inspected or logged.' 'Qualify the selected signing and provisioning in a separate release run.'
                            }
                        }
                    }
                    'linux' {
                        if ($props.DorotiBuildQtNative -eq 'false') {
                            Add-DoctorCheck 'linux.managed-only' $scopeName $false 'PASS' 'explicit prebuilt native profile' $props.DorotiQtNativeBuildDirectory 'This does not qualify a Linux native build.'
                            Check-DoctorPath 'linux.prebuilt-shim' (Join-Path $cwd "$($props.DorotiQtNativeBuildDirectory)/libdoroti_qt_host.so")
                        } elseif (-not $IsLinux) {
                            Add-DoctorCheck 'linux.native-host' $scopeName $true 'notVerified' 'native Qt build on Linux, or explicitly selected managed-only cross-build' 'native Qt acceptance unavailable on this host' 'Run Qt/native doctor on Linux.'
                        } else {
                            $cmake=Probe-DoctorTool 'linux.cmake' 'cmake' @('--version') $cwd
                            Add-DoctorCheck 'linux.cmake-version' $scopeName $true $(if ($cmake.output -match 'version (\d+\.\d+\.\d+)' -and [version]$Matches[1] -ge [version]'3.24.0') {'PASS'} else {'FAIL'}) 'CMake 3.24+' $cmake.output 'Install the minimum CMake from the Qt native project.'
                            Probe-DoctorTool 'linux.cxx' 'c++' @('--version') $cwd | Out-Null
                            Probe-DoctorTool 'linux.wayland-scanner' 'wayland-scanner' @('--version') $cwd | Out-Null
                            $modules=@('Qt6Core','Qt6Gui','Qt6Widgets','wayland-client','vulkan','fontconfig')
                            if ($props.DorotiQtQuick -eq 'true') {$modules+=@('Qt6Quick','Qt6Qml','Qt6QuickControls2')} else {$modules+='Qt6OpenGL'}
                            if ($props.DorotiQtWebEngine -eq 'true') {$modules+=@('Qt6WebEngineQuick','Qt6WebChannel')}
                            if ($props.DorotiGStreamerTextures -eq 'true') {$modules+=@('gstreamer-1.0','gstreamer-app-1.0','gstreamer-video-1.0')}
                            $cmakeSource=Join-Path (Split-Path -Parent $runner) 'native/CMakeLists.txt'
                            $requirements=if (Test-Path $cmakeSource) {Get-Content $cmakeSource -Raw} else {''}
                            foreach ($module in $modules) {
                                $moduleProbe=Probe-DoctorTool "linux.$module" 'pkg-config' @('--modversion',$module) $cwd
                                if ($module -like 'Qt6*') {
                                    $minimum=if ($module -in @('Qt6WebEngineQuick','Qt6WebChannel') -and $requirements -match 'Qt6\s+(\d+\.\d+)\s+REQUIRED COMPONENTS WebEngineQuick') {$Matches[1]} elseif ($module -in @('Qt6Quick','Qt6Qml','Qt6QuickControls2') -and $requirements -match 'Qt6\s+(\d+\.\d+)\s+REQUIRED COMPONENTS Quick') {$Matches[1]} elseif ($requirements -match 'Qt6\s+(\d+\.\d+)\s+REQUIRED COMPONENTS Core') {$Matches[1]} else {$null}
                                    if ($minimum) {Add-DoctorCheck "linux.$module.version" $scopeName $true $(if ($moduleProbe.available -and [version]$moduleProbe.output -ge [version]$minimum) {'PASS'} else {'FAIL'}) "Qt $minimum+ from selected CMake profile" $moduleProbe.output 'Install the selected profile Qt API/modules.'}
                                }
                            }
                        }
                    }
                }
            } catch { Add-DoctorCheck "$alias.context" $scopeName $true 'FAIL' 'valid runner/SDK/native context' $_.Exception.Message 'Correct the selected context and repeat doctor.' }
        }
    } catch { Add-DoctorCheck 'workspace.context' $scopeName $true 'FAIL' 'valid workspace and target' $_.Exception.Message 'Specify a valid -App/-Platform and declared runner.' }
    $failed=@($checks | Where-Object {$_.required -and $_.status -eq 'FAIL'}).Count
    $unknown=@($checks | Where-Object {$_.required -and $_.status -eq 'notVerified'}).Count
    $status=if ($failed) {'FAIL'} elseif ($unknown) {'PARTIAL'} else {'PASS'}
    $revision=Invoke-DorotiProbe 'git' @('rev-parse','HEAD') $repositoryRoot $DoctorProbeTimeoutSeconds
    $report=[ordered]@{schemaVersion='doroti.doctor/v4'; timestamp=[DateTimeOffset]::UtcNow.ToString('o'); status=$status; success=$status -eq 'PASS'; scope=$scopeName; context=$context;
        commit=$revision.output; checks=$checks; dotnet=$dotnetResult; workloads=$workloadsResult; xcode=$xcodeResult; macosSdk=$macosSdkResult;
        hostArchitecture=[Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString(); powerShell=[ordered]@{available=$PSVersionTable.PSVersion.Major -ge 7; output=$PSVersionTable.PSVersion.ToString()};
        referenceTools=[ordered]@{requiredForProductDevelopment=$false; flutterCheckout=(Test-Path (Join-Path $repositoryRoot 'reference/flutter-master'))}; runtimeAcceptance='notVerified'}
    $outputDirectory=if ($DoctorReportDirectory) {[IO.Path]::GetFullPath($DoctorReportDirectory)} else {Join-Path $artifacts 'doctor'}
    [IO.Directory]::CreateDirectory($outputDirectory) | Out-Null
    $json=($report | ConvertTo-Json -Depth 32) -replace "`r`n", "`n"
    $markdown=@('# Doroti doctor', '', "Status: **$status**; scope: $scopeName", '', 'Build, device, GPU, physical input and deployment acceptance require separate evidence.', '', '| Check | Required | Status | Expected | Actual / action |', '| --- | --- | --- | --- | --- |')
    foreach ($check in $checks) { $markdown += "| $($check.id) | $($check.required) | $($check.status) | $(($check.expected | ConvertTo-Json -Compress -Depth 8) -replace '\|','/') | $(($check.actual | ConvertTo-Json -Compress -Depth 8) -replace '[\r\n|]',' ') / $($check.action) |" }
    foreach ($item in @(@{Name='doctor.json'; Text=$json}, @{Name='doctor.md'; Text=$markdown -join "`n"})) {
        $path=Join-Path $outputDirectory $item.Name; $temporary=$path+'.'+[guid]::NewGuid().ToString('N')+'.tmp'
        [IO.File]::WriteAllText($temporary, $item.Text+"`n", [Text.UTF8Encoding]::new($false)); [IO.File]::Move($temporary,$path,$true)
    }
    Write-Host "Doctor: $status ($scopeName); report: $(Join-Path $outputDirectory 'doctor.json')"
    if ($status -ne 'PASS') { throw "Doctor $status; required prerequisites remain unresolved. See $outputDirectory/doctor.json." }
}
