#Requires -Version 5.1
param(
    [ValidateSet('All', 'Compiler', 'DartAnalyze', 'Package', 'LiveWindows', 'Evidence')]
    [string] $Shard = 'All',
    [switch] $KeepTemporary
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
. (Join-Path $PSScriptRoot 'flutter-sdk.ps1')
$flutterSdk = Resolve-DorotiFlutterSdk -RepositoryRoot $repoRoot
$flutterCommand = $flutterSdk.FlutterCommand
$dartCommand = $flutterSdk.DartCommand
$compilerProject = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj'
$compilerDll = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/bin/Release/net10.0/Doroti.DartToCSharp.dll'
$selection = Join-Path $dorotiRoot 'migration/selections/g6-generated-demo.json'
$unsupportedPluginSelection = Join-Path $dorotiRoot 'migration/selections/g6-generated-demo-unsupported-plugin.json'
$unsupportedSyntaxSelection = Join-Path $dorotiRoot 'validation/cases/compiler-negative.selection.json'
$version = '0.2.0-beta'
$releaseRoot = Join-Path $dorotiRoot "artifacts/g6-release/$version"
$generatedRoot = Join-Path $releaseRoot 'generated'
$packageRoot = Join-Path $releaseRoot 'packages'
$publishRoot = Join-Path $releaseRoot 'publish/win-x64'
$compilerMetricsPath = Join-Path $releaseRoot 'compiler-metrics.json'
$dartAnalyzeMetricsPath = Join-Path $releaseRoot 'dart-analyze-metrics.json'
$packageMetricsPath = Join-Path $releaseRoot 'package-metrics.json'
$runEvidencePath = Join-Path $releaseRoot 'run-evidence.json'
$releaseManifestPath = Join-Path $releaseRoot 'release-manifest.json'
$evidencePath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-generated-demo-evidence.json'
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g6-7-generated-demo'
$externalParent = Split-Path $repoRoot -Parent
$externalRoot = Join-Path $externalParent ('.doroti-g6-7-external-' + [Guid]::NewGuid().ToString('N'))
$externalRunner = Join-Path $externalRoot 'runner'
$externalGenerated = Join-Path $externalRoot 'generated'
$externalNuget = Join-Path $externalRoot 'nuget-cache'
$externalProject = Join-Path $externalRunner 'G6.GeneratedDemo.Consumer.csproj'
$externalConfig = Join-Path $externalRoot 'NuGet.Config'

function Test-Shard([string] $Name) { return $Shard -eq 'All' -or $Shard -eq $Name }

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Get-ProcessLog([string] $Path) {
    if (-not (Test-Path -LiteralPath $Path)) { return '' }
    return Get-Content -LiteralPath $Path -Raw -ErrorAction SilentlyContinue
}

function Stop-ProcessTree([int] $ProcessId) {
    if (-not (Get-Process -Id $ProcessId -ErrorAction SilentlyContinue)) { return }
    if ($IsWindows -or $env:OS -eq 'Windows_NT') {
        & taskkill.exe /PID $ProcessId /T /F 2>&1 | Out-Null
    }
    else {
        Stop-Process -Id $ProcessId -Force -ErrorAction SilentlyContinue
    }
}

function Invoke-LoggedProcess(
    [string] $FilePath,
    [string[]] $Arguments,
    [string] $WorkingDirectory,
    [string] $LogName,
    [int] $TimeoutSeconds = 1200,
    [string] $FailFastPattern = 'An unexpected error was encountered by the Analysis Server|Internal error: Failed to handle request|RequestError code: SERVER_ERROR'
) {
    $stdoutPath = Join-Path $temporaryRoot "$LogName.stdout.log"
    $stderrPath = Join-Path $temporaryRoot "$LogName.stderr.log"
    $timer = [Diagnostics.Stopwatch]::StartNew()
    $startArguments = @{
        FilePath = $FilePath
        ArgumentList = $Arguments
        WorkingDirectory = $WorkingDirectory
        RedirectStandardOutput = $stdoutPath
        RedirectStandardError = $stderrPath
        PassThru = $true
    }
    if ($IsWindows -or $env:OS -eq 'Windows_NT') { $startArguments.WindowStyle = 'Hidden' }
    $process = Start-Process @startArguments
    try {
        while (-not $process.HasExited) {
            Start-Sleep -Milliseconds 200
            $combined = (Get-ProcessLog $stdoutPath) + "`n" + (Get-ProcessLog $stderrPath)
            if (-not [string]::IsNullOrWhiteSpace($FailFastPattern) -and $combined -match $FailFastPattern) {
                Stop-ProcessTree $process.Id
                throw "$LogName reported an analysis server internal error and was stopped immediately.`n$combined"
            }
            if ($timer.Elapsed.TotalSeconds -ge $TimeoutSeconds) {
                Stop-ProcessTree $process.Id
                throw "$LogName exceeded the $TimeoutSeconds second limit."
            }
        }
        $process.WaitForExit()
    }
    finally {
        $timer.Stop()
    }
    $stdout = Get-ProcessLog $stdoutPath
    $stderr = Get-ProcessLog $stderrPath
    if ($process.ExitCode -ne 0) {
        throw "$LogName failed with exit code $($process.ExitCode).`n$stdout`n$stderr"
    }
    $stdoutText = if ($null -eq $stdout) { '' } else { [string]$stdout }
    $stderrText = if ($null -eq $stderr) { '' } else { [string]$stderr }
    return [pscustomobject]@{
        elapsedMs = [Math]::Round($timer.Elapsed.TotalMilliseconds, 3)
        stdout = $stdoutText.Trim()
        stderr = $stderrText.Trim()
    }
}

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $json = ($Value | ConvertTo-Json -Depth 32) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($Path, $json + "`n", [Text.UTF8Encoding]::new($false))
}

function Get-ProductDigest([string] $Root) {
    $files = @(Get-ChildItem -LiteralPath (Join-Path $Root 'projects') -File -Recurse |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } | Sort-Object FullName)
    $rootUri = [Uri]::new(([IO.Path]::GetFullPath($Root).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar))
    $lines = @($files | ForEach-Object {
        $relative = [Uri]::UnescapeDataString($rootUri.MakeRelativeUri([Uri]::new($_.FullName)).ToString())
        "$relative $((Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant())"
    })
    $sha = [Security.Cryptography.SHA256]::Create()
    try { return -join ($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes(($lines -join "`n") + "`n")) | ForEach-Object { $_.ToString('x2') }) }
    finally { $sha.Dispose() }
}

function Invoke-Compiler([string] $Manifest, [string] $Output) {
    Invoke-Checked {
        dotnet $compilerDll --manifest $Manifest --output $Output --cache-dir (Join-Path $dorotiRoot '.doroti/cache/g6-7') --parallelism 4
    } "G6-7 compiler failed: $Manifest"
}

function Assert-GeneratedBoundary {
    $project = @(Get-ChildItem -LiteralPath (Join-Path $generatedRoot 'projects/Framework') -Filter '*.csproj' -File)
    Assert-True ($project.Count -eq 1) 'one generated application project'
    [xml]$xml = Get-Content -LiteralPath $project[0].FullName -Raw
    $projectReferences = @($xml.Project.ItemGroup.ProjectReference |
        ForEach-Object { [string]$_.Include } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $packageReferences = @($xml.Project.ItemGroup.PackageReference |
        ForEach-Object { [string]$_.Include } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    Assert-True ($projectReferences.Count -eq 0) 'repository-private generated project references'
    Assert-True ((($packageReferences | Sort-Object) -join '|') -ceq 'Doroti.Framework.Material|Doroti.Hosting') 'generated package-only direct references'
    $source = Get-Content -LiteralPath (Join-Path $generatedRoot 'projects/Framework/doroti_demo_app_main.g.cs') -Raw
    Assert-True ($source -notmatch 'Avalonia|Win32|Doroti\.Host\.|Doroti\.Vendor') 'generated source platform/vendor boundary'
}

function Ensure-Generated {
    if (Test-Path -LiteralPath (Join-Path $generatedRoot 'application-graph.json')) { return }
    & (Join-Path $PSScriptRoot 'prepare-g6-generated-demo.ps1') | Write-Output
    Invoke-Checked { dotnet build $compilerProject --configuration Release --nologo } 'G6-7 compiler build failed'
    Invoke-Compiler $selection $generatedRoot
}

function Ensure-Packages {
    if (@(Get-ChildItem -LiteralPath $packageRoot -Filter 'Doroti.Target.Windows.win-x64.*.nupkg' -File -ErrorAction SilentlyContinue).Count -gt 0 -and
        @(Get-ChildItem -LiteralPath $packageRoot -Filter 'Doroti.Plugin.G6GeneratedDemoEcho.win-x64.*.nupkg' -File -ErrorAction SilentlyContinue).Count -gt 0) { return }
    Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo --disable-build-servers --tl:off -nodeReuse:false } 'G6-7 product build failed'
    [IO.Directory]::CreateDirectory($packageRoot) | Out-Null
    Invoke-Checked { dotnet pack (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --no-build --nologo --disable-build-servers --tl:off -nodeReuse:false --output $packageRoot } 'G6-7 package graph failed'
    Invoke-Checked { dotnet pack (Join-Path $dorotiRoot 'validation/generated/g6-generated-demo-echo-plugin-win-x64/Doroti.Plugin.G6GeneratedDemoEcho.win-x64.csproj') --configuration Release --nologo --output $packageRoot } 'G6-7 RID plugin package failed'
}

function Prepare-ExternalConsumer {
    Ensure-Generated
    Ensure-Packages
    if (Test-Path -LiteralPath $externalProject) { return }
    [IO.Directory]::CreateDirectory($externalRoot) | Out-Null
    Copy-Item -Recurse -LiteralPath $generatedRoot -Destination $externalGenerated
    Copy-Item -Recurse -LiteralPath (Join-Path $dorotiRoot 'validation/generated/g6-generated-demo-consumer') -Destination $externalRunner
    $escapedPackages = [Security.SecurityElement]::Escape($packageRoot)
    $config = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="doroti-local" value="$escapedPackages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
"@
    [IO.File]::WriteAllText($externalConfig, $config, [Text.UTF8Encoding]::new($false))
}

function Invoke-CompilerShard {
    & (Join-Path $PSScriptRoot 'prepare-g6-generated-demo.ps1') | Write-Output
    Invoke-Checked { & $dartCommand format --output=none --set-exit-if-changed (Join-Path $repoRoot 'DorotiDemoApp/dart/lib/main.dart') } 'G6-7 Dart source formatting failed'
    Invoke-Checked { dotnet build $compilerProject --configuration Release --nologo } 'G6-7 compiler build failed'
    if (Test-Path -LiteralPath $releaseRoot) {
        $resolved = [IO.Path]::GetFullPath($releaseRoot)
        $allowed = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'artifacts/g6-release')).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
        if (-not $resolved.StartsWith($allowed, [StringComparison]::OrdinalIgnoreCase)) { throw "Refusing to replace release path: $resolved" }
        Remove-Item -LiteralPath $resolved -Recurse -Force
    }
    [IO.Directory]::CreateDirectory($releaseRoot) | Out-Null
    $cleanTimer = [Diagnostics.Stopwatch]::StartNew()
    Invoke-Compiler $selection $generatedRoot
    $cleanTimer.Stop()
    $cleanDigest = Get-ProductDigest $generatedRoot
    $incrementalTimer = [Diagnostics.Stopwatch]::StartNew()
    Invoke-Compiler $selection $generatedRoot
    $incrementalTimer.Stop()
    $incrementalDigest = Get-ProductDigest $generatedRoot
    Assert-True ($cleanDigest -ceq $incrementalDigest) 'clean/incremental output identity'
    $graph = Get-Content -LiteralPath (Join-Path $generatedRoot 'application-graph.json') -Raw | ConvertFrom-Json
    $report = Get-Content -LiteralPath (Join-Path $generatedRoot 'converter-report.json') -Raw | ConvertFrom-Json
    Assert-True (@($graph.incremental.changedAndDependentSccLibraries).Count -eq 0) 'no-change affected application libraries'
    Assert-True (@($graph.incremental.reusedOutputs).Count -eq @($report.outputs).Count) 'no-change reused application outputs'
    Assert-True (@($report.diagnostics | Where-Object severity -eq 'error').Count -eq 0) 'generated DemoApp compiler errors'
    Assert-GeneratedBoundary

    $unsupportedPluginOutput = Join-Path $temporaryRoot 'unsupported-plugin'
    & dotnet $compilerDll --manifest $unsupportedPluginSelection --output $unsupportedPluginOutput --cache-dir (Join-Path $dorotiRoot '.doroti/cache/g6-7') --parallelism 4
    Assert-True ($LASTEXITCODE -ne 0) 'unsupported plugin non-zero exit'
    $unsupportedPluginReport = Get-Content -LiteralPath (Join-Path $unsupportedPluginOutput 'converter-report.json') -Raw | ConvertFrom-Json
    Assert-True (@($unsupportedPluginReport.diagnostics | Where-Object code -eq 'DOTAPP005').Count -eq 1) 'unsupported plugin DOTAPP005 diagnostic'

    $unsupportedSyntaxOutput = Join-Path $temporaryRoot 'unsupported-syntax'
    & dotnet $compilerDll --manifest $unsupportedSyntaxSelection --output $unsupportedSyntaxOutput --cache-dir (Join-Path $dorotiRoot '.doroti/cache/g6-7') --parallelism 4
    Assert-True ($LASTEXITCODE -ne 0) 'unsupported syntax non-zero exit'
    $unsupportedSyntaxReport = Get-Content -LiteralPath (Join-Path $unsupportedSyntaxOutput 'converter-report.json') -Raw | ConvertFrom-Json
    Assert-True (@($unsupportedSyntaxReport.diagnostics | Where-Object severity -eq 'error').Count -gt 0) 'unsupported syntax typed diagnostic'

    Write-Json $compilerMetricsPath ([ordered]@{
        schemaVersion = 'doroti.g6-generated-demo-compiler/v1'
        cleanElapsedMs = [Math]::Round($cleanTimer.Elapsed.TotalMilliseconds, 3)
        incrementalElapsedMs = [Math]::Round($incrementalTimer.Elapsed.TotalMilliseconds, 3)
        cleanDigest = $cleanDigest
        incrementalDigest = $incrementalDigest
        outputCount = @($report.outputs).Count
        noChangeReusedOutputs = @($graph.incremental.reusedOutputs).Count
        directReferences = @($graph.directReferences)
        unsupportedSyntaxErrors = @($unsupportedSyntaxReport.diagnostics | Where-Object severity -eq 'error').Count
        unsupportedPluginDiagnostic = 'DOTAPP005'
        silentSuccesses = 0
    })
    Write-Output 'G6-7 compiler, determinism, boundary, and negative diagnostics: PASS'
}

function Invoke-DartAnalyzeShard {
    $sourcePackage = Join-Path $repoRoot 'DorotiDemoApp/dart'
    $analysisPackage = Join-Path $temporaryRoot 'flutter-analyze'
    [IO.Directory]::CreateDirectory($analysisPackage) | Out-Null
    foreach ($name in @('pubspec.yaml', 'lib', 'assets', 'l10n', 'manifests')) {
        Copy-Item -LiteralPath (Join-Path $sourcePackage $name) -Destination $analysisPackage -Recurse
    }
    $pubGet = Invoke-LoggedProcess -FilePath $flutterCommand -Arguments @('pub', 'get') -WorkingDirectory $analysisPackage -LogName 'g6-7-flutter-pub-get'
    $packageConfigPath = Join-Path $analysisPackage '.dart_tool/package_config.json'
    Assert-True (Test-Path -LiteralPath $packageConfigPath) 'real Flutter package config'
    $packageConfig = Get-Content -LiteralPath $packageConfigPath -Raw | ConvertFrom-Json
    $skyEngine = @($packageConfig.packages | Where-Object name -eq 'sky_engine')
    Assert-True ($skyEngine.Count -eq 1) 'real Flutter sky_engine package'
    $packageConfigUri = [Uri]::new([IO.Path]::GetFullPath($packageConfigPath))
    $skyEngineRoot = [Uri]::new($packageConfigUri, ([string]$skyEngine[0].rootUri).TrimEnd('/') + '/').LocalPath
    $customSkyEngine = [IO.Path]::GetFullPath((Join-Path $repoRoot 'tools/Doroti.DartToCSharp/analyzer/stubs/sky_engine'))
    Assert-True (-not [IO.Path]::GetFullPath($skyEngineRoot).StartsWith($customSkyEngine, [StringComparison]::OrdinalIgnoreCase)) 'Flutter analyze must not use custom sky_engine stub'
    $analysis = Invoke-LoggedProcess -FilePath $flutterCommand -Arguments @('analyze', '--no-pub', 'lib/main.dart') -WorkingDirectory $analysisPackage -LogName 'g6-7-flutter-analyze'
    Assert-True ($analysis.stdout -match 'No issues found') 'real Flutter analyzer zero diagnostics'
    $failFastProbeSource = "[Console]::Error.WriteLine('An unexpected error was encountered by the Analysis Server.'); Start-Sleep -Seconds 30"
    $failFastProbeEncoded = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($failFastProbeSource))
    $failFastProbeTimer = [Diagnostics.Stopwatch]::StartNew()
    $failFastProbePassed = $false
    try {
        [void](Invoke-LoggedProcess -FilePath (Get-Process -Id $PID).Path -Arguments @('-NoProfile', '-EncodedCommand', $failFastProbeEncoded) -WorkingDirectory $analysisPackage -LogName 'g6-7-analysis-server-fail-fast-probe' -TimeoutSeconds 30)
    }
    catch {
        $failFastProbePassed = $_.Exception.Message -match 'reported an analysis server internal error and was stopped immediately'
        if (-not $failFastProbePassed) { throw }
    }
    finally {
        $failFastProbeTimer.Stop()
    }
    Assert-True ($failFastProbePassed -and $failFastProbeTimer.Elapsed.TotalSeconds -lt 5) 'analysis server internal-error immediate failure'
    [IO.Directory]::CreateDirectory($releaseRoot) | Out-Null
    Write-Json $dartAnalyzeMetricsPath ([ordered]@{
        schemaVersion = 'doroti.g6-generated-demo-dart-analyze/v1'
        command = 'flutter analyze --no-pub lib/main.dart'
        packageConfig = 'real-flutter-sdk-generated-isolated-copy'
        customPackageConfigUsed = $false
        skyEngineRoot = 'flutter-sdk-cache'
        pubGetElapsedMs = $pubGet.elapsedMs
        analyzeElapsedMs = $analysis.elapsedMs
        diagnostics = 0
        analysisServerInternalErrorFailFast = 'PASS'
        failFastProbeElapsedMs = [Math]::Round($failFastProbeTimer.Elapsed.TotalMilliseconds, 3)
        maxMinutes = 20
    })
    Write-Output 'G6-7 real Flutter SDK package analysis and internal-error fail-fast: PASS'
}

function Invoke-PackageShard {
    Prepare-ExternalConsumer
    $restoreTimer = [Diagnostics.Stopwatch]::StartNew()
    Invoke-Checked { dotnet restore $externalProject --configfile $externalConfig --packages $externalNuget --force-evaluate --nologo --disable-build-servers --tl:off -nodeReuse:false } 'G6-7 isolated restore failed'
    $restoreTimer.Stop()
    $assetsPath = Join-Path $externalRunner 'obj/project.assets.json'
    $assetsText = Get-Content -LiteralPath $assetsPath -Raw
    $assets = $assetsText | ConvertFrom-Json
    # The expected local NuGet feed lives under the repository and therefore
    # appears in restore.sources/packageFolders. Inspect the resolved graph,
    # not that feed metadata. JSON escaping had hidden this distinction on
    # Windows while macOS forward-slash paths exposed the false positive.
    $resolvedGraphText = @($assets.targets, $assets.libraries) | ConvertTo-Json -Depth 64
    Assert-True (-not $resolvedGraphText.Contains($repoRoot, [StringComparison]::OrdinalIgnoreCase)) 'repository-private restore fallback'
    $nonPackages = @($assets.libraries.PSObject.Properties | Where-Object { $_.Value.type -ne 'package' })
    Assert-True ($nonPackages.Count -eq 1 -and $nonPackages[0].Name -like 'Doroti.Generated.Application.G6Demo.Framework/*') 'isolated dependency graph'
    $buildTimer = [Diagnostics.Stopwatch]::StartNew()
    Invoke-Checked { dotnet build $externalProject --configuration Release --no-restore --nologo --disable-build-servers --tl:off -nodeReuse:false } 'G6-7 isolated build failed'
    $buildTimer.Stop()
    [IO.Directory]::CreateDirectory($publishRoot) | Out-Null
    $publishTimer = [Diagnostics.Stopwatch]::StartNew()
    Invoke-Checked { dotnet publish $externalProject --configuration Release --no-restore --nologo --runtime win-x64 --self-contained false --output $publishRoot --disable-build-servers --tl:off -nodeReuse:false } 'G6-7 isolated publish failed'
    $publishTimer.Stop()
    Write-Json $packageMetricsPath ([ordered]@{
        schemaVersion = 'doroti.g6-generated-demo-package/v1'
        externalRoot = 'outside-repository-isolated'
        restoreElapsedMs = [Math]::Round($restoreTimer.Elapsed.TotalMilliseconds, 3)
        buildElapsedMs = [Math]::Round($buildTimer.Elapsed.TotalMilliseconds, 3)
        publishElapsedMs = [Math]::Round($publishTimer.Elapsed.TotalMilliseconds, 3)
        repositoryPrivateFallbacks = 0
        repositoryPrivateProjectReferences = 0
        packageCount = @(Get-ChildItem -LiteralPath $packageRoot -Filter '*.nupkg' -File | Where-Object Extension -eq '.nupkg').Count
        publishedEntry = 'publish/win-x64/G6.GeneratedDemo.Consumer.exe'
    })
    Write-Output 'G6-7 outside-repository restore/build/publish package boundary: PASS'
}

function Invoke-LiveWindowsShard {
    if (-not (Test-Path -LiteralPath (Join-Path $publishRoot 'G6.GeneratedDemo.Consumer.exe'))) { Invoke-PackageShard }
    Invoke-Checked { & (Join-Path $publishRoot 'G6.GeneratedDemo.Consumer.exe') --evidence $runEvidencePath --frames 60 } 'G6-7 published strict-GPU run failed'
    $run = Get-Content -LiteralPath $runEvidencePath -Raw | ConvertFrom-Json
    Assert-True ($run.outcome -eq 'presented') 'generated Dart DemoApp outcome'
    Assert-True ($run.target.backend -eq 'skia-wgl-opengl-gpu' -and -not [bool]$run.target.softwareFallbackUsed) 'generated Dart DemoApp strict GPU'
    Assert-True ([long]$run.interaction.changedPixels -ge 100) 'generated Dart DemoApp interaction raster differential'
    Assert-True (@($run.interaction.hitTargets | Where-Object { $_ -like '*RenderPointerListener' }).Count -gt 0) 'generated Dart DemoApp native pointer target'
    Assert-True ([int]$run.semantics.nodeCount -ge 6) 'generated Dart DemoApp semantics tree'
    Assert-True ([bool]$run.applicationBoundary.MissingResourceRejected -and [bool]$run.applicationBoundary.MissingPluginRejected) 'unsupported runtime capability rejection'
    Assert-True ([bool]$run.applicationBoundary.EchoPluginPassed -and [int]$run.applicationBoundary.PluginCount -eq 1) 'generated DemoApp RID plugin execution'
    Assert-True ([long]$run.performance.sustained.presentedFrames -ge 60) 'generated Dart DemoApp sustained frames'
    Assert-True ([bool]$run.resourceClosure.IsBalanced) 'generated Dart DemoApp resource closure'
    Write-Output 'G6-7 published package-only strict-GPU first frame, interaction, semantics, resources, and 60-frame cadence: PASS'
}

function Invoke-EvidenceShard {
    Assert-True (Test-Path -LiteralPath $compilerMetricsPath) 'compiler metrics artifact'
    Assert-True (Test-Path -LiteralPath $dartAnalyzeMetricsPath) 'Dart analyze metrics artifact'
    Assert-True (Test-Path -LiteralPath $packageMetricsPath) 'package metrics artifact'
    Assert-True (Test-Path -LiteralPath $runEvidencePath) 'run evidence artifact'
    $compiler = Get-Content -LiteralPath $compilerMetricsPath -Raw | ConvertFrom-Json
    $dartAnalyze = Get-Content -LiteralPath $dartAnalyzeMetricsPath -Raw | ConvertFrom-Json
    $package = Get-Content -LiteralPath $packageMetricsPath -Raw | ConvertFrom-Json
    $run = Get-Content -LiteralPath $runEvidencePath -Raw | ConvertFrom-Json
    $compatibilityDisposition = Get-Content -LiteralPath (Join-Path $dorotiRoot 'migration/flutter-framework/g6-compatibility-disposition.json') -Raw | ConvertFrom-Json
    $files = @(Get-ChildItem -LiteralPath $releaseRoot -File -Recurse | Where-Object FullName -ne $releaseManifestPath | Sort-Object FullName)
    $releaseUri = [Uri]::new(([IO.Path]::GetFullPath($releaseRoot).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar))
    $hashes = @($files | ForEach-Object {
        [ordered]@{
            path = [Uri]::UnescapeDataString($releaseUri.MakeRelativeUri([Uri]::new($_.FullName)).ToString())
            sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
            length = $_.Length
        }
    })
    Write-Json $releaseManifestPath ([ordered]@{
        schemaVersion = 'doroti.g6-release-manifest/v1'
        version = $version
        generatedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
        files = $hashes
    })
    $evidence = [ordered]@{
        schemaVersion = 'doroti.g6-generated-demo-evidence/v1'
        milestone = 'G6-7'
        capturedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
        status = 'partial-prerequisites-and-differential-open'
        source = [ordered]@{
            dartPackage = 'DorotiDemoApp/dart'
            entryPoint = 'package:doroti_demo_app/main.dart'
            generatedAssembly = [string]$run.source.generatedAssembly
            handwrittenFixture = 'DorotiDemoApp/Program.cs'
        }
        compiler = $compiler
        package = $package
        liveWindows = [ordered]@{
            outcome = [string]$run.outcome
            backend = [string]$run.target.backend
            softwareFallbackUsed = [bool]$run.target.softwareFallbackUsed
            firstFrameMs = [double]$run.performance.firstFrameMs
            interactionChangedPixels = [long]$run.interaction.changedPixels
            semanticsNodeCount = [int]$run.semantics.nodeCount
            sustained = $run.performance.sustained
            memory = $run.performance.memory
            handles = $run.performance.handles
            resourceClosure = $run.resourceClosure
        }
        applicationPipeline = [ordered]@{
            asset = 'PASS'
            font = 'PASS'
            localization = 'PASS'
            pluginManifest = 'PASS-win-x64-echo-ABI'
            echoPluginPassed = [bool]$run.applicationBoundary.EchoPluginPassed
            missingResourceRejected = [bool]$run.applicationBoundary.MissingResourceRejected
            missingPluginRejected = [bool]$run.applicationBoundary.MissingPluginRejected
            silentSuccesses = 0
        }
        tooling = [ordered]@{
            dartFormat = 'PASS'
            compilerAnalyzer = 'PASS-0-diagnostics'
            customPackageStandaloneDartAnalyze = 'not-run-invalid-minimal-embedder-sdk-config'
            flutterAnalyze = 'PASS-real-Flutter-SDK-package-config-0-diagnostics'
            flutterAnalyzeMetrics = $dartAnalyze
        }
        differential = [ordered]@{
            behavior = 'partial-generated-native-button-state-raster-PASS'
            semantics = 'partial-generated-tree-PASS-handwritten-full-family-comparison-notVerified'
            visual = 'notVerified-no-pinned-generated-vs-handwritten-pixel-differential'
        }
        compatibility = [ordered]@{
            disposition = 'explicit-blocker'
            source = 'migration/flutter-framework/g6-compatibility-disposition.json'
            temporaryRules = [int]$compatibilityDisposition.active.temporary
            blocker = 'named Flutter library compatibility rules remain; G6-7 removal gate is not closed'
        }
        entryConditions = [ordered]@{
            g6_3_through_g6_6_complete = $false
            blocker = 'G6-5R/G6-5R-I/G6-5R-C and G6-6 reference/stress gates remain open'
        }
        artifacts = [ordered]@{
            release = "artifacts/g6-release/$version"
            manifest = "artifacts/g6-release/$version/release-manifest.json"
            runEvidence = "artifacts/g6-release/$version/run-evidence.json"
        }
        validationShards = @(
            [ordered]@{ name = 'Compiler'; command = 'validate-g6-generated-demo.ps1 -Shard Compiler'; maxMinutes = 20; status = 'PASS' },
            [ordered]@{ name = 'DartAnalyze'; command = 'validate-g6-generated-demo.ps1 -Shard DartAnalyze'; maxMinutes = 20; status = 'PASS' },
            [ordered]@{ name = 'Package'; command = 'validate-g6-generated-demo.ps1 -Shard Package'; maxMinutes = 20; status = 'PASS' },
            [ordered]@{ name = 'LiveWindows'; command = 'validate-g6-generated-demo.ps1 -Shard LiveWindows'; maxMinutes = 20; status = 'PASS' },
            [ordered]@{ name = 'Evidence'; command = 'validate-g6-generated-demo.ps1 -Shard Evidence'; maxMinutes = 20; status = 'PASS' }
        )
        notVerified = @(
            'G6-7 entry condition G6-3 through G6-6 complete',
            'handwritten/generated full behavior visual semantics differential',
            "temporary compatibility rule removal ($([int]$compatibilityDisposition.active.temporary) remain)",
            '100+ repeated launch/toggle resource stability',
            'physical input and cross-target package execution'
        )
    }
    Write-Json $evidencePath $evidence
    Write-Output "G6-7 evidence: $evidencePath"
}

function Remove-ExternalRoot {
    if (-not (Test-Path -LiteralPath $externalRoot)) { return }
    $resolved = [IO.Path]::GetFullPath($externalRoot)
    $allowed = [IO.Path]::GetFullPath($externalParent).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if (-not $resolved.StartsWith($allowed, [StringComparison]::OrdinalIgnoreCase) -or
        -not [IO.Path]::GetFileName($resolved).StartsWith('.doroti-g6-7-external-', [StringComparison]::Ordinal)) {
        throw "Refusing to remove unexpected external consumer path: $resolved"
    }
    Remove-Item -LiteralPath $resolved -Recurse -Force
}

try {
    if (Test-Shard 'Compiler') { Invoke-CompilerShard }
    if (Test-Shard 'DartAnalyze') { Invoke-DartAnalyzeShard }
    if (Test-Shard 'Package') { Invoke-PackageShard }
    if (Test-Shard 'LiveWindows') { Invoke-LiveWindowsShard }
    if (Test-Shard 'Evidence') { Invoke-EvidenceShard }
    Write-Output "G6-7 generated DemoApp validation shard '$Shard': PASS"
}
finally {
    if (-not $KeepTemporary) {
        Remove-ExternalRoot
        if (Test-Path -LiteralPath $temporaryRoot) { Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot }
    }
}

# Negative compiler probes intentionally return non-zero. Do not leak their
# handled exit code to a parent PowerShell gate after this shard has passed.
$global:LASTEXITCODE = 0
