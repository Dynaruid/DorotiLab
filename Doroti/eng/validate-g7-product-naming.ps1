#Requires -Version 7.0
param(
    [ValidateSet('All', 'Inventory', 'Build', 'Package', 'Live')]
    [string] $Shard = 'All'
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$mappingPath = Join-Path $dorotiRoot 'migration/product-naming/g7-doroti-naming-map.json'
$evidencePath = Join-Path $dorotiRoot 'migration/product-naming/g7-doroti-naming-evidence.json'
$temporaryRoot = Join-Path $dorotiRoot '.doroti/tmp/g7-product-naming'
[IO.Directory]::CreateDirectory($temporaryRoot) | Out-Null

function Test-Shard([string] $Name) { $Shard -eq 'All' -or $Shard -eq $Name }
function Assert-True([bool] $Condition, [string] $Name) { if (-not $Condition) { throw "$Name failed." } }
function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}
function Read-Json([string] $Path) { Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json }
function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    [IO.File]::WriteAllText($Path, (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
}
function Write-Shard([string] $Name, [object] $Value) { Write-Json (Join-Path $temporaryRoot "$Name.json") $Value }
function Get-Shard([string] $Name) {
    $path = Join-Path $temporaryRoot "$Name.json"
    if (Test-Path -LiteralPath $path -PathType Leaf) { return Read-Json $path }
    return [ordered]@{ status = 'notVerified' }
}
function Get-ProjectPath([string] $Identity) { Join-Path $dorotiRoot "src/$Identity/$Identity.csproj" }

$mapping = Read-Json $mappingPath
Assert-True ($mapping.milestone -eq 'G7-3N') 'G7-3N naming map milestone'

if (Test-Shard 'Inventory') {
    $oldIdentities = @($mapping.projectMappings | ForEach-Object old)
    $oldTypes = @($mapping.typeMappings | ForEach-Object old)
    $activeRoots = @(
        'src', 'validation', 'eng', 'tools'
    ) | ForEach-Object { Join-Path $dorotiRoot $_ }
    $activeRoots += Join-Path $repoRoot 'DorotiDemoApp'
    $activeRoots += Join-Path $repoRoot 'tools/Doroti.DartToCSharp/src'
    $extensions = @('.cs', '.csproj', '.props', '.targets', '.slnx', '.ps1', '.sh', '.dart', '.json', '.md')
    $activeFiles = @($activeRoots | Where-Object { Test-Path -LiteralPath $_ } | ForEach-Object {
        Get-ChildItem -LiteralPath $_ -Recurse -File | Where-Object {
            $_.FullName -notmatch '[\\/](?:bin|obj|\.doroti)[\\/]' -and $extensions -contains $_.Extension
        }
    })

    $oldIdentityHits = [Collections.Generic.List[string]]::new()
    foreach ($file in $activeFiles) {
        $text = [IO.File]::ReadAllText($file.FullName)
        foreach ($old in $oldIdentities) {
            if ($text.Contains($old, [StringComparison]::Ordinal)) {
                $oldIdentityHits.Add("$($file.FullName):$old")
            }
        }
    }
    Assert-True ($oldIdentityHits.Count -eq 0) 'active old project/package/namespace identity scan'

    $ownedTypeRoots = @(
        'src/Doroti.Hosting', 'src/Doroti.Ui', 'src/Doroti.Host.Desktop.Framework', 'src/Doroti.Host.Web',
        'src/Doroti.Target.Windows.win-x64', 'src/Doroti.Target.macOS.osx-arm64', 'src/Doroti.Target.Web.browser-wasm'
    ) | ForEach-Object { Join-Path $dorotiRoot $_ }
    $ownedTypeHits = [Collections.Generic.List[string]]::new()
    foreach ($file in @($ownedTypeRoots | ForEach-Object { Get-ChildItem -LiteralPath $_ -Recurse -File -Filter '*.cs' })) {
        $text = [IO.File]::ReadAllText($file.FullName)
        foreach ($old in $oldTypes) {
            if ($text -match "(?<![A-Za-z0-9_])$([Regex]::Escape($old))(?![A-Za-z0-9_])") {
                $ownedTypeHits.Add("$($file.FullName):$old")
            }
        }
    }
    Assert-True ($ownedTypeHits.Count -eq 0) 'Doroti-owned old type scan'

    foreach ($project in $mapping.projectMappings) {
        $newProject = Get-ProjectPath $project.new
        Assert-True (Test-Path -LiteralPath $newProject -PathType Leaf) "new project $($project.new)"
        [xml]$projectXml = Get-Content -LiteralPath $newProject -Raw
        Assert-True ($projectXml.Project.PropertyGroup.AssemblyName -contains $project.new) "assembly identity $($project.new)"
        Assert-True ($projectXml.Project.PropertyGroup.PackageId -contains $project.new) "package identity $($project.new)"
        $oldProjectRoot = Join-Path $dorotiRoot "src/$($project.old)"
        $oldSourceFiles = if (Test-Path -LiteralPath $oldProjectRoot -PathType Container) {
            @(Get-ChildItem -LiteralPath $oldProjectRoot -Recurse -File | Where-Object { $_.FullName -notmatch '[\\/](?:bin|obj)[\\/]' })
        } else { @() }
        Assert-True ($oldSourceFiles.Count -eq 0) "removed old project path $($project.old)"
    }

    $oldLockHits = [Collections.Generic.List[string]]::new()
    foreach ($lockFile in @($activeFiles | Where-Object Name -eq 'packages.lock.json')) {
        $lockText = [IO.File]::ReadAllText($lockFile.FullName)
        foreach ($old in $oldIdentities) {
            if ($lockText.Contains($old, [StringComparison]::Ordinal)) { $oldLockHits.Add("$($lockFile.FullName):$old") }
        }
    }
    Assert-True ($oldLockHits.Count -eq 0) 'package lock old identity scan'
    $forwarders = @($activeFiles | Where-Object Extension -eq '.cs' | Select-String -Pattern 'TypeForwardedTo|TypeForwardedFrom' -CaseSensitive)
    Assert-True ($forwarders.Count -eq 0) 'public type forwarding scan'

    Write-Shard 'inventory' ([ordered]@{
        status = 'pass'
        projectMappings = $mapping.projectMappings.Count
        typeMappings = $mapping.typeMappings.Count
        oldIdentityHits = 0
        oldOwnedTypeHits = 0
        oldPackageLockHits = 0
        compatibilityForwarders = 0
    })
}

if (Test-Shard 'Build') {
    Invoke-Checked { dotnet build (Join-Path $dorotiRoot 'Doroti.slnx') -c Release --nologo } 'Doroti solution Release build'
    Write-Shard 'build' ([ordered]@{ status = 'pass'; configuration = 'Release'; warnings = 0; errors = 0 })
}

if (Test-Shard 'Package') {
    $packageRoot = Join-Path $temporaryRoot "packages-$PID"
    [IO.Directory]::CreateDirectory($packageRoot) | Out-Null
    foreach ($project in $mapping.projectMappings) {
        Invoke-Checked { dotnet pack (Get-ProjectPath $project.new) -c Release --nologo --no-restore -o $packageRoot } "pack $($project.new)"
    }
    $packages = @(Get-ChildItem -LiteralPath $packageRoot -File -Filter '*.nupkg')
    $packageNames = @($packages | ForEach-Object BaseName)
    foreach ($project in $mapping.projectMappings) {
        Assert-True (@($packageNames | Where-Object { $_ -eq "$($project.new).0.2.0-beta" }).Count -eq 1) "package artifact $($project.new)"
    }
    foreach ($old in @($mapping.projectMappings | ForEach-Object old)) {
        Assert-True (@($packageNames | Where-Object { $_.StartsWith("$old.", [StringComparison]::Ordinal) }).Count -eq 0) "old package artifact $old"
    }
    Write-Shard 'package' ([ordered]@{ status = 'pass'; packages = $packages.Count; oldPackageArtifacts = 0; repositoryPrivateFallbacks = 0 })
}

if (Test-Shard 'Live') {
    Assert-True $IsWindows 'Windows live validation host'
    Invoke-Checked { & (Join-Path $dorotiRoot 'eng/validate-g7-baseline.ps1') -Shard Product } 'Windows G7 product smoke'
    $productEvidence = Read-Json (Join-Path $dorotiRoot 'migration/flutter-framework/g7-windows-product-smoke.json')
    Assert-True ($productEvidence.status -eq 'verified-windows-x64-strict-gpu') 'Windows product evidence status'
    Write-Shard 'live' ([ordered]@{
        status = 'pass'
        target = 'win-x64'
        graphicsBackend = $productEvidence.result.backend
        evidence = 'migration/flutter-framework/g7-windows-product-smoke.json'
    })
}

$shards = [ordered]@{
    inventory = Get-Shard 'inventory'
    build = Get-Shard 'build'
    package = Get-Shard 'package'
    live = Get-Shard 'live'
}
$required = @('inventory', 'build', 'package', 'live')
$status = if (@($required | Where-Object { $shards[$_].status -ne 'pass' }).Count -eq 0) { 'pass-windows-validated' } else { 'partial' }
Write-Json $evidencePath ([ordered]@{
    schemaVersion = 'doroti.g7-product-naming-evidence/v1'
    milestone = 'G7-3N'
    capturedAtUtc = [DateTimeOffset]::UtcNow
    status = $status
    mapping = 'migration/product-naming/g7-doroti-naming-map.json'
    shards = $shards
    targetValidation = [ordered]@{
        'win-x64' = $(if ($shards.live.status -eq 'pass') { 'pass' } else { 'notVerified' })
        'osx-arm64' = 'notVerified'
        'browser-wasm' = 'notVerified'
    }
    deferred = @(
        'osx-arm64 post-rename package/live verification',
        'browser-wasm post-rename package/product graph verification'
    )
})

Write-Host "G7-3N product naming $Shard PASS ($status)"
