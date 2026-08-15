#Requires -Version 5.1
param([switch] $KeepTemporary)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g5-6w'
$packageRoot = Join-Path $temporaryRoot 'packages'
$externalRoot = Join-Path $temporaryRoot 'external-consumer'
$nugetCache = Join-Path $temporaryRoot 'nuget-cache'
$packageArtifactRoot = Join-Path $dorotiRoot 'artifacts/g5-6w/windows-package'
$publishRoot = Join-Path $dorotiRoot 'artifacts/g5-6w/windows-publish/win-x64'
$smokeEvidencePath = Join-Path $publishRoot 'package-smoke.json'
$aggregateEvidencePath = Join-Path $dorotiRoot 'migration/flutter-avalonia/bridge-validation/g5-6w.json'
$capabilityEvidencePath = Join-Path $dorotiRoot 'migration/flutter-avalonia/target-capabilities/win-x64.json'
$selectionPath = Join-Path $dorotiRoot 'migration/avalonia-shell/port-selection.json'
$provenancePath = Join-Path $dorotiRoot 'migration/avalonia-shell/g5-6w-source-port-provenance.json'
$targetProject = Join-Path $dorotiRoot 'src/Doroti.Target.Windows.win-x64/Doroti.Target.Windows.win-x64.csproj'
$externalFixture = Join-Path $dorotiRoot 'validation/generated/g5-6w-external-consumer'
[IO.Directory]::CreateDirectory($packageRoot) | Out-Null
[IO.Directory]::CreateDirectory($externalRoot) | Out-Null

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command
    if ($LASTEXITCODE -ne 0) { throw $Failure }
}

function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $json = ($Value | ConvertTo-Json -Depth 32) -replace "`r`n", "`n"
    $temporary = $Path + '.tmp-' + [Guid]::NewGuid().ToString('N')
    [IO.File]::WriteAllText($temporary, $json + "`n", [Text.UTF8Encoding]::new($false))
    if (Test-Path -LiteralPath $Path) {
        $backup = $Path + '.bak-' + [Guid]::NewGuid().ToString('N')
        try { [IO.File]::Replace($temporary, $Path, $backup) }
        finally { if (Test-Path -LiteralPath $backup) { Remove-Item -LiteralPath $backup -Force } }
    }
    else { [IO.File]::Move($temporary, $Path) }
}

function Get-TreeDigest([string] $Root) {
    $rootPath = [IO.Path]::GetFullPath($Root).TrimEnd([IO.Path]::DirectorySeparatorChar)
    $lines = @(Get-ChildItem -LiteralPath $rootPath -Recurse -File | Sort-Object FullName | ForEach-Object {
        $relative = $_.FullName.Substring($rootPath.Length).TrimStart([IO.Path]::DirectorySeparatorChar).Replace('\', '/')
        "$relative $((Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant())"
    })
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { $hash = $algorithm.ComputeHash([Text.Encoding]::UTF8.GetBytes(($lines -join "`n") + "`n")) }
    finally { $algorithm.Dispose() }
    return (($hash | ForEach-Object { $_.ToString('x2') }) -join '')
}

function Get-TextDigest([string] $Value) {
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { $hash = $algorithm.ComputeHash([Text.Encoding]::UTF8.GetBytes($Value)) }
    finally { $algorithm.Dispose() }
    return (($hash | ForEach-Object { $_.ToString('x2') }) -join '')
}

function Read-ZipText([IO.Compression.ZipArchive] $Archive, [string] $Name) {
    $entry = $Archive.GetEntry($Name)
    if ($null -eq $entry) { throw "Package entry is missing: $Name" }
    $stream = $entry.Open()
    try {
        $reader = [IO.StreamReader]::new($stream, [Text.Encoding]::UTF8, $true, 4096, $true)
        try { return $reader.ReadToEnd() }
        finally { $reader.Dispose() }
    }
    finally { $stream.Dispose() }
}

try {
    Invoke-Checked {
        dotnet run --project (Join-Path $dorotiRoot 'tools/Doroti.AvaloniaPort/Doroti.AvaloniaPort.csproj') --configuration Release -- audit
    } 'Selected Avalonia source-port audit failed.'
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-g5-3-platform-foundation.ps1') } 'G5-3B Windows predecessor gate failed.'
    Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo } 'G5-6W product build failed.'
    Invoke-Checked { dotnet pack (Join-Path $dorotiRoot 'Doroti.Product.slnx') --configuration Release --nologo --output $packageRoot } 'G5-6W package graph failed.'

    $targetPackages = @(Get-ChildItem -LiteralPath $packageRoot -Filter 'Doroti.Target.Windows.win-x64.0.2.0-beta.nupkg')
    if ($targetPackages.Count -ne 1) { throw "Expected one Windows RID package, got $($targetPackages.Count)." }
    $targetPackage = $targetPackages[0].FullName
    $frameworkPackage = Join-Path $packageRoot 'Doroti.Framework.Widgets.0.2.0-beta.nupkg'
    if (-not (Test-Path -LiteralPath $frameworkPackage)) { throw 'The framework package used for host-swap identity is missing.' }
    $frameworkDigestBefore = (Get-FileHash -LiteralPath $frameworkPackage -Algorithm SHA256).Hash.ToLowerInvariant()
    Invoke-Checked { dotnet pack $targetProject --configuration Release --nologo --output $packageRoot --no-build } 'Independent Windows RID package repack failed.'
    $frameworkDigestAfter = (Get-FileHash -LiteralPath $frameworkPackage -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($frameworkDigestBefore -cne $frameworkDigestAfter) { throw 'Repacking the RID host changed the framework package.' }

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [IO.Compression.ZipFile]::OpenRead($targetPackage)
    try {
        $manifest = Read-ZipText $archive 'doroti/doroti-target-manifest.json' | ConvertFrom-Json
        $packagedSelection = Read-ZipText $archive 'source-port/port-selection.json'
        $packagedProvenance = Read-ZipText $archive 'source-port/provenance.json'
        $entryNames = @($archive.Entries.FullName)
    }
    finally { $archive.Dispose() }
    $selectionHash = (Get-FileHash -LiteralPath $selectionPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $provenanceHash = (Get-FileHash -LiteralPath $provenancePath -Algorithm SHA256).Hash.ToLowerInvariant()
    $packagedSelectionHash = Get-TextDigest $packagedSelection
    $packagedProvenanceHash = Get-TextDigest $packagedProvenance
    if ($selectionHash -cne $manifest.sourcePortSelectionSha256 -or $selectionHash -cne $packagedSelectionHash) { throw 'Packaged source-port selection identity drifted.' }
    if ($provenanceHash -cne $manifest.sourcePortProvenanceSha256 -or $provenanceHash -cne $packagedProvenanceHash) { throw 'Packaged source-port provenance identity drifted.' }
    foreach ($required in 'doroti/doroti-target-manifest.json', 'source-port/port-selection.json', 'source-port/provenance.json', 'licenses/THIRD-PARTY-NOTICES.md') {
        if ($entryNames -notcontains $required) { throw "Windows RID package release input is missing: $required" }
    }

    if (Test-Path -LiteralPath $packageArtifactRoot) {
        $resolvedPackageArtifact = [IO.Path]::GetFullPath($packageArtifactRoot)
        $expectedPackageArtifact = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'artifacts/g5-6w/windows-package'))
        if ($resolvedPackageArtifact -cne $expectedPackageArtifact) { throw "Refusing to clean unexpected package artifact path: $resolvedPackageArtifact" }
        [IO.Directory]::Delete($resolvedPackageArtifact, $true)
    }
    [IO.Directory]::CreateDirectory($packageArtifactRoot) | Out-Null
    Copy-Item -LiteralPath $targetPackage -Destination $packageArtifactRoot
    $symbolPackage = Join-Path $packageRoot 'Doroti.Target.Windows.win-x64.0.2.0-beta.snupkg'
    if (-not (Test-Path -LiteralPath $symbolPackage)) { throw 'The Windows RID symbol package is missing.' }
    Copy-Item -LiteralPath $symbolPackage -Destination $packageArtifactRoot
    $targetPackageArtifact = Join-Path $packageArtifactRoot 'Doroti.Target.Windows.win-x64.0.2.0-beta.nupkg'

    Get-ChildItem -LiteralPath $externalFixture -Force | Copy-Item -Destination $externalRoot -Recurse
    $externalProject = Join-Path $externalRoot 'G56.WindowsTarget.ExternalConsumer.csproj'
    Invoke-Checked {
        dotnet restore $externalProject --packages $nugetCache --force-evaluate --nologo "-p:RestoreAdditionalProjectSources=$packageRoot"
    } 'Package-only Windows consumer restore failed.'
    $assetsPath = Join-Path $externalRoot 'obj/project.assets.json'
    $assetsText = Get-Content -LiteralPath $assetsPath -Raw
    if ($assetsText.IndexOf($repoRoot, [StringComparison]::OrdinalIgnoreCase) -ge 0) { throw 'External consumer restored a repository-private fallback.' }
    $assets = $assetsText | ConvertFrom-Json
    $officialAvaloniaPackages = @($assets.libraries.PSObject.Properties.Name | Where-Object { $_ -match '^Avalonia(\.|/)' })
    if ($officialAvaloniaPackages.Count -ne 0) { throw "Official Avalonia packages entered the RID graph: $($officialAvaloniaPackages -join ', ')." }
    Invoke-Checked { dotnet build $externalProject --configuration Release --no-restore --nologo } 'Package-only Windows consumer build failed.'

    if (Test-Path -LiteralPath $publishRoot) {
        $resolvedPublish = [IO.Path]::GetFullPath($publishRoot)
        $expectedParent = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'artifacts/g5-6w/windows-publish')) + [IO.Path]::DirectorySeparatorChar
        if (-not $resolvedPublish.StartsWith($expectedParent, [StringComparison]::OrdinalIgnoreCase) -or [IO.Path]::GetFileName($resolvedPublish) -cne 'win-x64') {
            throw "Refusing to clean unexpected publish path: $resolvedPublish"
        }
        [IO.Directory]::Delete($resolvedPublish, $true)
    }
    [IO.Directory]::CreateDirectory($publishRoot) | Out-Null
    Invoke-Checked {
        dotnet publish $externalProject --configuration Release --runtime win-x64 --self-contained false --no-restore --nologo --output $publishRoot
    } 'Package-only Windows consumer publish failed.'
    $consumerExe = Join-Path $publishRoot 'G56.WindowsTarget.ExternalConsumer.exe'
    $consumerOutput = (& $consumerExe --evidence $smokeEvidencePath 2>&1 | Out-String)
    if ($LASTEXITCODE -ne 0 -or $consumerOutput -notlike '*G5-6W-WINDOWS-TARGET-PACKAGE-SMOKE-PASS*') {
        throw "Published Windows synthetic smoke failed:`n$consumerOutput"
    }
    $smoke = Get-Content -LiteralPath $smokeEvidencePath -Raw | ConvertFrom-Json
    if ($smoke.status -ne 'PASS' -or $smoke.diagnostics.frame.softwareFallbackUsed -or $smoke.diagnostics.frame.recoveryCount -lt 1) {
        throw 'Published Windows diagnostic smoke did not close strict GPU/recovery.'
    }
    $publishedDeps = Get-Content -LiteralPath (Join-Path $publishRoot 'G56.WindowsTarget.ExternalConsumer.deps.json') -Raw | ConvertFrom-Json
    $officialAvaloniaPublished = @($publishedDeps.libraries.PSObject.Properties.Name | Where-Object { $_ -match '^Avalonia(\.|/)' })
    if ($officialAvaloniaPublished.Count -ne 0) { throw 'Official Avalonia binaries entered the publish artifact.' }

    $requiredCapabilities = @(
        'window.lifecycle', 'view.lifecycle-metrics', 'view.frame-dispatch', 'input.events',
        'text.input', 'platform.services', 'platform.environment', 'platform.messaging',
        'graphics.scene', 'graphics.text', 'graphics.image', 'accessibility.semantics'
    )
    $missingCapabilities = @($requiredCapabilities | Where-Object { $_ -notin @($smoke.diagnostics.capabilityIds) })
    if ($missingCapabilities.Count -ne 0) { throw "Packaged target is missing capabilities: $($missingCapabilities -join ', ')." }
    $publishDigest = Get-TreeDigest $publishRoot
    $targetPackageDigest = (Get-FileHash -LiteralPath $targetPackageArtifact -Algorithm SHA256).Hash.ToLowerInvariant()

    $capabilityEvidence = [ordered]@{
        schemaVersion = 'doroti.flutter-avalonia-target-capabilities/v2'
        milestone = 'G5-6W'
        rid = 'win-x64'
        targetIdentitySchema = $manifest.targetIdentitySchema
        diagnosticSchema = $manifest.diagnosticSchema
        flutterRevision = $manifest.flutterRevision
        avaloniaRevision = $manifest.avaloniaRevision
        package = [ordered]@{ id = $manifest.packageId; version = $manifest.packageVersion; sha256 = $targetPackageDigest }
        implementation = [ordered]@{ window = 'win32'; graphics = 'wgl-opengl-skia'; officialAvaloniaBinaryDependencies = 0 }
        capabilities = @($requiredCapabilities | ForEach-Object { [ordered]@{ id = $_; status = 'verified-automated-current-machine' } })
        diagnostics = [ordered]@{ frame = 'PASS'; input = 'PASS-synthetic'; automation = 'PASS-synthetic-native-entrypoint'; resource = 'PASS'; recovery = 'PASS-injected' }
        physical = [ordered]@{ status = 'notVerified'; deferredTo = 'G5-8 DorotiDemoApp' }
    }
    Write-Json $capabilityEvidencePath $capabilityEvidence

    $evidence = [ordered]@{
        schemaVersion = 'doroti.g5-6w-evidence/v1'
        milestone = 'G5-6W'
        capturedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
        status = 'verified-automated-current-machine'
        targetPackage = [ordered]@{
            id = $manifest.packageId
            version = $manifest.packageVersion
            rid = $manifest.rid
            sha256 = $targetPackageDigest
            artifact = 'artifacts/g5-6w/windows-package/Doroti.Target.Windows.win-x64.0.2.0-beta.nupkg'
            sourcePortSelectionSha256 = $selectionHash
            sourcePortProvenanceSha256 = $provenanceHash
            packagedReleaseInputs = @('target manifest', 'source-port selection', 'source-port provenance', 'third-party notices')
            officialAvaloniaBinaryDependencies = 0
        }
        hostSwap = [ordered]@{
            frameworkPackage = 'Doroti.Framework.Widgets/0.2.0-beta'
            beforeSha256 = $frameworkDigestBefore
            afterSha256 = $frameworkDigestAfter
            frameworkRegenerated = $false
        }
        contracts = [ordered]@{
            targetIdentitySchema = $manifest.targetIdentitySchema
            diagnosticSchema = $manifest.diagnosticSchema
            capabilitySchema = $manifest.capabilitySchema
            capabilityIds = $requiredCapabilities
            hooks = @('frame', 'input', 'automation', 'resource')
        }
        publish = [ordered]@{
            rid = 'win-x64'
            selfContained = $false
            artifact = 'artifacts/g5-6w/windows-publish/win-x64'
            digest = $publishDigest
            restore = 'PASS-isolated-package-cache'
            build = 'PASS'
            publish = 'PASS'
            syntheticSmoke = 'PASS'
            repositoryPrivateFallbacks = 0
            officialAvaloniaBinaryDependencies = 0
            marker = 'G5-6W-WINDOWS-TARGET-PACKAGE-SMOKE-PASS'
        }
        automatedDiagnostics = $smoke.diagnostics
        targetIdentity = $smoke.identity
        predecessor = [ordered]@{ g5_3bPlatformFoundation = 'PASS'; avaloniaPortAudit = 'PASS' }
        physical = [ordered]@{
            status = 'notVerified'
            successfulResultsUsedForMilestone = 0
            deferredTo = 'G5-8 DorotiDemoApp'
            items = @('physical mouse', 'precision touchpad', 'touch', 'Korean IME', 'cross-monitor DPI', 'external physical accessibility', 'sustained GPU')
        }
    }
    Write-Json $aggregateEvidencePath $evidence
    Write-Output $consumerOutput.Trim()
    Write-Output 'G5-6W Windows RID package, isolated publish, and synthetic diagnostics: PASS'
    Write-Output "Evidence: $aggregateEvidencePath"
}
finally {
    if (-not $KeepTemporary -and (Test-Path -LiteralPath $temporaryRoot)) {
        Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
    }
}
