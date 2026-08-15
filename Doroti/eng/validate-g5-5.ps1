#Requires -Version 5.1
param([switch] $KeepTemporary)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$compilerProject = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj'
$compiler = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/bin/Release/net10.0/Doroti.DartToCSharp.dll'
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g5-5'
$candidateRoot = Join-Path $temporaryRoot 'generated'
$cacheRoot = Join-Path $temporaryRoot 'compiler-cache'
$packageRoot = Join-Path $temporaryRoot 'packages'
$externalRoot = Join-Path $temporaryRoot 'external-consumer'
$incrementalSelectionPath = Join-Path $dorotiRoot ('migration/selections/.g5-5-incremental-' + [Guid]::NewGuid().ToString('N') + '.json')
[IO.Directory]::CreateDirectory($candidateRoot) | Out-Null
[IO.Directory]::CreateDirectory($packageRoot) | Out-Null

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    & $Command
    if ($LASTEXITCODE -ne 0) { throw $Failure }
}

function Write-Json([string] $Path, [object] $Value) {
    $json = ($Value | ConvertTo-Json -Depth 24) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($Path, $json + "`n", [Text.UTF8Encoding]::new($false))
}

function Get-ProductDigest([string] $Root) {
    $files = @(Get-ChildItem -LiteralPath (Join-Path $Root 'projects') -File -Recurse |
        Where-Object { $_.FullName -notmatch '[\/](bin|obj)[\/]' } |
        Sort-Object FullName)
    $rootUri = [Uri]::new(([IO.Path]::GetFullPath($Root).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar))
    $lines = @($files | ForEach-Object {
        $relative = [Uri]::UnescapeDataString($rootUri.MakeRelativeUri([Uri]::new($_.FullName)).ToString())
        "$relative $((Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant())"
    })
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { $hash = $algorithm.ComputeHash([Text.Encoding]::UTF8.GetBytes(($lines -join "`n") + "`n")) }
    finally { $algorithm.Dispose() }
    return (($hash | ForEach-Object { $_.ToString('x2') }) -join '')
}

function Invoke-Compiler([string] $Manifest, [string] $Output) {
    Invoke-Checked { dotnet $compiler --manifest $Manifest --output $Output --cache-dir $cacheRoot --parallelism 4 } "G5-5 compiler failed: $Manifest"
}

function Assert-ApplicationProjectBoundary([string] $Candidate, [string[]] $ExpectedReferences) {
    $project = @(Get-ChildItem -LiteralPath (Join-Path $Candidate 'projects/Framework') -Filter '*.csproj' -File)
    if ($project.Count -ne 1) { throw "Expected one generated application project under $Candidate." }
    [xml]$xml = Get-Content -LiteralPath $project[0].FullName -Raw
    $references = @($xml.Project.ItemGroup.ProjectReference | ForEach-Object {
        $include = [string]$_.Include
        if ($include -match 'Doroti\.Flutter\.Framework\.([A-Za-z]+)') { "Doroti.Framework.$($Matches[1])" }
        elseif ($include -match 'g5-4-reviewed\\projects\\(Material|Cupertino)\\') { "Doroti.Framework.$($Matches[1])" }
        elseif ($include -match 'src\\(Doroti\.Flutter\.Hosting)') { $Matches[1] }
        else { "unexpected:$include" }
    }) + @($xml.Project.ItemGroup.PackageReference | ForEach-Object { [string]$_.Include })
    $actual = @($references | Sort-Object -Unique)
    $expected = @($ExpectedReferences | Sort-Object -Unique)
    if (($actual -join '|') -cne ($expected -join '|')) {
        throw "Generated application direct references drifted: $($actual -join ', ')."
    }
    $sourceFiles = @(Get-ChildItem -LiteralPath (Join-Path $Candidate 'projects/Framework') -Filter '*.g.cs' -File)
    $concrete = @($sourceFiles | Select-String -Pattern 'Avalonia|Win32|Skia|Doroti\.Host\.|Doroti\.Platform|Doroti\.Vendor')
    if ($concrete.Count -ne 0) { throw "Generated application source contains $($concrete.Count) platform/vendor concrete reference(s)." }
}

try {
    & (Join-Path $PSScriptRoot 'prepare-g5-5.ps1') | Write-Output
    & (Join-Path $PSScriptRoot 'stage-g5-4.ps1') | Write-Output
    Invoke-Checked { dotnet build $compilerProject --configuration Release --nologo } 'G5-5 compiler build failed.'
    Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo } 'G5-5 product build failed.'

    $apps = @(
        [ordered]@{ id = 'material-assets'; surface = 'Material'; entry = 'G55MaterialAssetsApp' },
        [ordered]@{ id = 'material-plugin'; surface = 'Material'; entry = 'G55MaterialPluginApp' },
        [ordered]@{ id = 'cupertino-localized'; surface = 'Cupertino'; entry = 'G55CupertinoLocalizedApp' },
        [ordered]@{ id = 'widgets-base'; surface = 'Widgets'; entry = 'G55WidgetsBaseApp' }
    )
    $applicationEvidence = [Collections.Generic.List[object]]::new()
    foreach ($app in $apps) {
        $manifest = Join-Path $dorotiRoot "migration/selections/g5-5-$($app.id).json"
        $candidate = Join-Path $candidateRoot $app.id
        Invoke-Compiler $manifest $candidate
        $cleanDigest = Get-ProductDigest $candidate
        $cleanGraph = Get-Content -LiteralPath (Join-Path $candidate 'application-graph.json') -Raw | ConvertFrom-Json
        Invoke-Compiler $manifest $candidate
        $incrementalDigest = Get-ProductDigest $candidate
        $incrementalGraph = Get-Content -LiteralPath (Join-Path $candidate 'application-graph.json') -Raw | ConvertFrom-Json
        if ($cleanDigest -cne $incrementalDigest) { throw "$($app.id) clean/incremental product bytes drifted." }
        if (@($incrementalGraph.incremental.changedAndDependentSccLibraries).Count -ne 0) { throw "$($app.id) no-change incremental build regenerated libraries." }
        Assert-ApplicationProjectBoundary $candidate @("Doroti.Framework.$($app.surface)", 'Doroti.Hosting')
        Invoke-Checked { dotnet build (Join-Path $candidate 'Doroti.Generated.Application.slnx') --configuration Release --nologo "-p:DorotiRepositoryRoot=$dorotiRoot" } "$($app.id) generated application build failed."
        $report = Get-Content -LiteralPath (Join-Path $candidate 'converter-report.json') -Raw | ConvertFrom-Json
        $applicationEvidence.Add([ordered]@{
            id = $app.id
            surface = $app.surface
            entryType = $app.entry
            libraries = @($cleanGraph.libraries).Count
            conditionalEdges = @($cleanGraph.edges | Where-Object { @($_.candidates).Count -gt 1 }).Count
            generatedFiles = @($report.outputs).Count
            compilerErrors = @($report.diagnostics | Where-Object severity -eq 'error').Count
            cleanIncrementalDigest = $incrementalDigest
            noChangeReusedOutputs = @($incrementalGraph.incremental.reusedOutputs).Count
        })
    }

    $incrementalPackage = Join-Path $temporaryRoot 'incremental-package'
    Copy-Item -Recurse -LiteralPath (Join-Path $dorotiRoot 'validation/cases/g5-5-apps') -Destination $incrementalPackage
    & (Join-Path $PSScriptRoot 'prepare-g5-5.ps1') -PackageRoot $incrementalPackage | Write-Output
    $incrementalSelection = Get-Content -LiteralPath (Join-Path $dorotiRoot 'migration/selections/g5-5-widgets-base.json') -Raw | ConvertFrom-Json
    $incrementalSelection.packageRoot = $incrementalPackage
    Write-Json $incrementalSelectionPath $incrementalSelection
    $incrementalCandidate = Join-Path $temporaryRoot 'incremental-candidate'
    Invoke-Compiler $incrementalSelectionPath $incrementalCandidate
    [IO.File]::AppendAllText((Join-Path $incrementalPackage 'lib/shared/platform_io.dart'), "`n// dependent-SCC validation`n", [Text.UTF8Encoding]::new($false))
    Invoke-Compiler $incrementalSelectionPath $incrementalCandidate
    $changedGraph = Get-Content -LiteralPath (Join-Path $incrementalCandidate 'application-graph.json') -Raw | ConvertFrom-Json
    $affected = @($changedGraph.incremental.changedAndDependentSccLibraries)
    $expectedAffected = @(
        'package:g55_apps/apps/widgets_base.dart',
        'package:g55_apps/shared/common.dart',
        'package:g55_apps/shared/platform_io.dart'
    )
    if ((($affected | Sort-Object) -join '|') -cne (($expectedAffected | Sort-Object) -join '|')) {
        throw "Changed/dependent SCC regeneration drifted: $($affected -join ', ')."
    }
    if (@($changedGraph.incremental.reusedOutputs | Where-Object { [string]$_ -match 'platform_stub' }).Count -ne 1) { throw 'Unaffected conditional branch output was not reused.' }

    $unsupportedOutput = Join-Path $temporaryRoot 'unsupported-plugin'
    & dotnet $compiler --manifest (Join-Path $dorotiRoot 'migration/selections/g5-5-unsupported-plugin.json') --output $unsupportedOutput --cache-dir $cacheRoot --parallelism 4
    $unsupportedExit = $LASTEXITCODE
    if ($unsupportedExit -eq 0) { throw 'Unsupported plugin compiler fixture silently succeeded.' }
    $unsupportedReport = Get-Content -LiteralPath (Join-Path $unsupportedOutput 'converter-report.json') -Raw | ConvertFrom-Json
    $unsupportedDiagnostics = @($unsupportedReport.diagnostics | Where-Object code -eq 'DOTAPP005')
    if ($unsupportedDiagnostics.Count -ne 1) { throw 'Unsupported plugin did not emit exactly one DOTAPP005 diagnostic.' }

    $packageProjects = @(
        'Doroti.Runtime', 'Doroti.Ui', 'Doroti.Hosting',
        'Doroti.Framework.Foundation', 'Doroti.Framework.Scheduler', 'Doroti.Framework.Services',
        'Doroti.Framework.Physics', 'Doroti.Framework.Animation', 'Doroti.Framework.Gestures',
        'Doroti.Framework.Painting', 'Doroti.Framework.Semantics', 'Doroti.Framework.Rendering',
        'Doroti.Framework.Widgets'
    )
    foreach ($project in $packageProjects) {
        Invoke-Checked { dotnet pack (Join-Path $dorotiRoot "src/$project/$project.csproj") --configuration Release --nologo --output $packageRoot } "Package failed: $project"
    }
    foreach ($surface in 'Cupertino', 'Material') {
        $project = Join-Path $dorotiRoot "migration/generated-candidates/g5-4-reviewed/projects/$surface/Doroti.Generated.Framework.G54.$surface.csproj"
        Invoke-Checked { dotnet pack $project --configuration Release --nologo --output $packageRoot "-p:DorotiRepositoryRoot=$dorotiRoot" } "G5-4 framework package failed: $surface"
    }
    $pluginProject = Join-Path $dorotiRoot 'validation/generated/g5-5-echo-plugin-win-x64/Doroti.Plugin.G55Echo.win-x64.csproj'
    Invoke-Checked { dotnet pack $pluginProject --configuration Release --nologo --output $packageRoot } 'RID plugin package failed.'

    [IO.Directory]::CreateDirectory($externalRoot) | Out-Null
    Copy-Item -Recurse -LiteralPath $candidateRoot -Destination (Join-Path $externalRoot 'generated')
    Copy-Item -Recurse -LiteralPath (Join-Path $dorotiRoot 'validation/generated/g5-5-external-consumer') -Destination (Join-Path $externalRoot 'runner')
    $externalGenerated = Join-Path $externalRoot 'generated'
    Get-ChildItem -LiteralPath $externalGenerated -Directory -Recurse |
        Where-Object Name -in @('bin', 'obj') |
        Sort-Object FullName -Descending |
        ForEach-Object {
            $resolved = [IO.Path]::GetFullPath($_.FullName)
            if (-not $resolved.StartsWith([IO.Path]::GetFullPath($externalGenerated), [StringComparison]::OrdinalIgnoreCase)) {
                throw "External cleanup target escaped generated root: $resolved"
            }
            Remove-Item -LiteralPath $resolved -Recurse -Force
        }
    $runnerProject = Join-Path $externalRoot 'runner/G5.Application.ExternalConsumer.csproj'
    Invoke-Checked { dotnet restore $runnerProject --source $packageRoot --packages (Join-Path $temporaryRoot 'nuget-cache') --force-evaluate --nologo } 'Clean external consumer restore failed.'
    $consumerOutput = (& dotnet run --project $runnerProject --configuration Release --no-restore 2>&1 | Out-String)
    if ($LASTEXITCODE -ne 0 -or $consumerOutput -notlike '*G5-5-EXTERNAL-APPLICATION-CONSUMER-PASS*') {
        throw "Clean external consumer execution failed:`n$consumerOutput"
    }
    $assetFiles = @(Get-ChildItem -LiteralPath (Join-Path $externalRoot 'runner/obj') -Filter 'project.assets.json' -Recurse)
    $privateFallbacks = @($assetFiles | Select-String -Pattern ([Text.RegularExpressions.Regex]::Escape($repoRoot)) -SimpleMatch:$false).Count
    if ($privateFallbacks -ne 0) { throw 'External consumer restored a repository-private fallback.' }

    $pluginPackages = @(Get-ChildItem -LiteralPath $packageRoot -Filter 'Doroti.Plugin.G55Echo.win-x64.1.0.0-beta.nupkg')
    if ($pluginPackages.Count -ne 1) { throw "Expected one RID plugin package, got $($pluginPackages.Count)." }
    $pluginPackagePath = $pluginPackages[0].FullName
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [IO.Compression.ZipFile]::OpenRead($pluginPackagePath)
    try { $pluginCapabilityEntries = @($archive.Entries | Where-Object FullName -eq 'doroti/doroti-plugin-capabilities.json').Count }
    finally { $archive.Dispose() }
    if ($pluginCapabilityEntries -ne 1) { throw 'RID plugin package capability manifest is missing.' }

    $evidence = [ordered]@{
        schemaVersion = 'doroti.g5-5-evidence/v1'
        milestone = 'G5-5'
        capturedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
        status = 'verified-automated-current-machine'
        applications = @($applicationEvidence)
        packageGraph = [ordered]@{
            packageImports = 'verified'
            conditionalGraph = 'verified-both-branches'
            fixtureSpecificInputs = 0
            changedAndDependentSccOnly = 'PASS'
            affectedLibraries = $affected
            reusedUnaffectedOutputs = @($changedGraph.incremental.reusedOutputs).Count
        }
        resources = [ordered]@{
            assets = 'PASS'
            fonts = 'PASS'
            localization = 'PASS'
            embeddedIntegrity = 'SHA-256-and-length'
        }
        plugins = [ordered]@{
            dartApiCodec = 'MethodChannel/StandardMethodCodec'
            nativeRid = 'win-x64'
            nativePackage = 'Doroti.Plugin.G55Echo.win-x64/1.0.0-beta'
            abi = 'doroti.plugin-abi/v1'
            capabilityManifestEntries = $pluginCapabilityEntries
            unsupportedDiagnostic = 'DOTAPP005'
            silentSuccesses = 0
        }
        dependencyAudit = [ordered]@{
            generatedApplicationDirectReferences = @('selected Doroti.Framework package', 'Doroti.Hosting')
            platformVendorConcreteReferences = 0
            repositoryPrivateFallbacks = $privateFallbacks
        }
        externalConsumer = [ordered]@{
            restore = 'PASS'
            build = 'PASS'
            run = 'PASS'
            marker = 'G5-5-EXTERNAL-APPLICATION-CONSUMER-PASS'
        }
        evidence = [ordered]@{
            selections = 'migration/selections/g5-5-*.json'
            resourceManifest = 'validation/cases/g5-5-apps/manifests/resources.json'
            pluginManifest = 'validation/cases/g5-5-apps/manifests/plugins-echo.json'
            nativeCapabilityManifest = 'validation/generated/g5-5-echo-plugin-win-x64/doroti-plugin-capabilities.json'
        }
        notVerified = @('physical Windows plugin integration', 'physical asset/font rendering', 'physical localization UI')
        deferredTo = 'G5-8 DorotiDemoApp'
    }
    $evidencePath = Join-Path $dorotiRoot 'migration/flutter-framework/g5-5-evidence.json'
    Write-Json $evidencePath $evidence
    Write-Output $consumerOutput.Trim()
    Write-Output 'G5-5 application compiler/resource/plugin validation: PASS'
    Write-Output "Evidence: $evidencePath"
}
finally {
    if (Test-Path -LiteralPath $incrementalSelectionPath) {
        $selectionParent = [IO.Path]::GetFullPath((Split-Path $incrementalSelectionPath -Parent))
        $expectedParent = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'migration/selections'))
        if ($selectionParent -cne $expectedParent -or -not [IO.Path]::GetFileName($incrementalSelectionPath).StartsWith('.g5-5-incremental-', [StringComparison]::Ordinal)) {
            throw "Refusing to clean unexpected incremental selection: $incrementalSelectionPath"
        }
        Remove-Item -LiteralPath $incrementalSelectionPath -Force
    }
    if (-not $KeepTemporary -and (Test-Path -LiteralPath $temporaryRoot)) {
        Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
    }
}
