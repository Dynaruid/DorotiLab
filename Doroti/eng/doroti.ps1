[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('doctor', 'build', 'validate', 'audit', 'format', 'release', 'clean')]
    [string] $Command = 'doctor',

    [ValidateSet('auto', 'software', 'avalonia')]
    [string] $Backend = 'auto',

    [ValidateSet('compiler')]
    [string] $ValidationSuite = 'compiler'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
. (Join-Path $PSScriptRoot 'flutter-sdk.ps1')
$solution = Join-Path $dorotiRoot 'Doroti.slnx'
$productSolution = Join-Path $dorotiRoot 'Doroti.Product.slnx'
$artifacts = Join-Path $dorotiRoot 'artifacts'

function Invoke-Checked {
    param(
        [Parameter(Mandatory)] [string] $File,
        [Parameter(Mandatory)] [AllowEmptyCollection()] [string[]] $Arguments,
        [string] $WorkingDirectory = $dorotiRoot
    )

    Push-Location $WorkingDirectory
    try {
        & $File @Arguments
        if ($LASTEXITCODE -ne 0) {
            throw "Command failed with exit code ${LASTEXITCODE}: $File $($Arguments -join ' ')"
        }
    }
    finally {
        Pop-Location
    }
}

function Invoke-CheckedCapture {
    param(
        [Parameter(Mandatory)] [string] $File,
        [Parameter(Mandatory)] [AllowEmptyCollection()] [string[]] $Arguments,
        [string] $WorkingDirectory = $dorotiRoot
    )

    Push-Location $WorkingDirectory
    try {
        $output = @(& $File @Arguments 2>&1 | ForEach-Object { $_.ToString() })
        if ($LASTEXITCODE -ne 0) {
            throw "Command failed with exit code ${LASTEXITCODE}: $File $($Arguments -join ' ')`n$($output -join "`n")"
        }
        $output | ForEach-Object { Write-Host $_ }
        return $output
    }
    finally {
        Pop-Location
    }
}

function Get-CommandResult {
    param([string] $File, [string[]] $Arguments)
    try {
        $output = (& $File @Arguments 2>&1 | Out-String).Trim()
        return [ordered]@{ available = ($LASTEXITCODE -eq 0); output = $output }
    }
    catch {
        return [ordered]@{ available = $false; output = $_.Exception.Message }
    }
}

function Get-SelectedContentRevision {
    param([string] $SourcePath, [object[]] $AuditFiles)
    if (@($AuditFiles).Count -eq 0) { return $null }
    $entries = @()
    foreach ($relativePath in ($AuditFiles | Sort-Object)) {
        $path = [System.IO.Path]::GetFullPath((Join-Path $SourcePath $relativePath))
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { return $null }
        $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
        $entries += "$($relativePath -replace '\\','/'):$hash"
    }
    $bytes = [System.Text.Encoding]::UTF8.GetBytes(($entries -join "`n") + "`n")
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $hash = -join ($sha.ComputeHash($bytes) | ForEach-Object { $_.ToString('x2') })
        return "selected-content-sha256:$hash"
    }
    finally {
        $sha.Dispose()
    }
}

function Invoke-Doctor {
    $dotnet = Get-CommandResult 'dotnet' @('--version')
    try {
        $flutterSdk = Resolve-DorotiFlutterSdk -RepositoryRoot $repositoryRoot
        $flutter = Get-CommandResult $flutterSdk.FlutterCommand @('--version')
        $dart = Get-CommandResult $flutterSdk.DartCommand @('--version')
    }
    catch {
        $flutter = [ordered]@{ available = $false; output = $_.Exception.Message }
        $dart = [ordered]@{ available = $false; output = $_.Exception.Message }
    }
    $runtimeInformation = [System.Runtime.InteropServices.RuntimeInformation]
    $isDesktop = $runtimeInformation::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows) -or
        $runtimeInformation::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Linux) -or
        $runtimeInformation::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::OSX)
    $selectedBackend = if ($Backend -eq 'auto') { 'avalonia' } else { $Backend }
    $backendAvailable = ($selectedBackend -eq 'software') -or $isDesktop

    $sourceManifestPath = Join-Path $dorotiRoot 'migration/source-manifest.json'
    $sourceManifest = Get-Content -Raw $sourceManifestPath | ConvertFrom-Json
    $sources = @()
    foreach ($source in ($sourceManifest.sources | Sort-Object id)) {
        $sourcePath = [System.IO.Path]::GetFullPath((Join-Path (Split-Path $sourceManifestPath) $source.path))
        $licensePath = Join-Path $sourcePath $source.license
        $gitTopLevel = Get-CommandResult 'git' @('-C', $sourcePath, 'rev-parse', '--show-toplevel')
        $revision = $source.pinnedRevision
        if ($gitTopLevel.available -and ([System.IO.Path]::GetFullPath($gitTopLevel.output) -eq $sourcePath)) {
            $gitRevision = Get-CommandResult 'git' @('-C', $sourcePath, 'rev-parse', 'HEAD')
            if ($gitRevision.available) { $revision = "git:$($gitRevision.output)" }
        }
        if (-not $revision) { $revision = Get-SelectedContentRevision $sourcePath @($source.auditFiles) }
        $sources += [ordered]@{
            id = $source.id
            pathExists = (Test-Path -LiteralPath $sourcePath -PathType Container)
            licenseExists = (Test-Path -LiteralPath $licensePath -PathType Leaf)
            revision = $revision
        }
    }

    $success = $dotnet.available -and $flutter.available -and $dart.available -and $backendAvailable -and
        (@($sources | Where-Object { -not $_.pathExists -or -not $_.licenseExists -or -not $_.revision }).Count -eq 0)
    $report = [ordered]@{
        schemaVersion = 'doroti.doctor/v2'
        success = $success
        dotnet = $dotnet
        flutter = $flutter
        dart = $dart
        backend = [ordered]@{ selected = $selectedBackend; available = $backendAvailable; operatingSystem = [System.Environment]::OSVersion.VersionString }
        sources = $sources
    }

    $outputDirectory = Join-Path $artifacts 'doctor'
    New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
    $json = $report | ConvertTo-Json -Depth 8
    [System.IO.File]::WriteAllText((Join-Path $outputDirectory 'doctor.json'), ($json -replace "`r`n", "`n") + "`n", [System.Text.UTF8Encoding]::new($false))
    $markdown = @(
        '# Doroti doctor',
        '',
        "Status: **$(if ($success) { 'PASS' } else { 'FAIL' })**",
        '',
        "- .NET SDK: $($dotnet.output)",
        "- Repository-local Flutter SDK: $($flutter.output)",
        "- Repository-local Dart SDK: $($dart.output)",
        "- Backend: $selectedBackend (available: $backendAvailable)",
        "- Reference sources: $($sources.Count)"
    ) -join "`n"
    [System.IO.File]::WriteAllText((Join-Path $outputDirectory 'doctor.md'), $markdown + "`n", [System.Text.UTF8Encoding]::new($false))
    Write-Host "Doctor: $(if ($success) { 'PASS' } else { 'FAIL' })"
    if (-not $success) { throw 'Development environment does not satisfy R1 requirements. See artifacts/doctor/doctor.json.' }
}

function Invoke-Build {
    Invoke-Checked 'dotnet' @('build', $productSolution, '--nologo')
}

function Invoke-Validation {
    Write-Host 'Validate: compiler validation suite has been removed.'
}

function Invoke-Audit {
    & (Join-Path $PSScriptRoot 'validate-local-storage.ps1')
    Invoke-Checked 'dotnet' @('run', '--project', (Join-Path $dorotiRoot 'tools/Doroti.AvaloniaPort/Doroti.AvaloniaPort.csproj'), '--', 'audit')
    Invoke-Checked 'dotnet' @('run', '--project', (Join-Path $dorotiRoot 'tools/Doroti.SourceTools/Doroti.SourceTools.csproj'), '--', 'audit')
    Invoke-Validation
}

function Invoke-Format {
    Invoke-Checked 'dotnet' @('format', $productSolution, '--verify-no-changes', '--no-restore', '--verbosity', 'minimal')
}

function Invoke-Release {
    Invoke-Build
    Invoke-Audit

    $packageDirectory = Join-Path $artifacts 'packages'
    New-Item -ItemType Directory -Force -Path $packageDirectory | Out-Null
    Get-ChildItem -LiteralPath $packageDirectory -File |
        Where-Object { $_.Extension -in @('.nupkg', '.snupkg') } |
        Remove-Item -Force
    Invoke-Checked 'dotnet' @('pack', $productSolution, '--configuration', 'Release', '--output', $packageDirectory, '--nologo')
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    foreach ($packagePath in (Get-ChildItem -LiteralPath $packageDirectory -Filter '*.nupkg' -File)) {
        $archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath.FullName)
        try {
            $entries = @($archive.Entries | ForEach-Object { $_.FullName })
            if ($entries | Where-Object { $_ -match '\.(dart|yaml|lock)$' -or $_ -match 'migration/(fixtures|generated|cache)' }) {
                throw "Package contains compiler/source snapshot content: $($packagePath.Name)"
            }
            $nuspec = $archive.Entries | Where-Object { $_.FullName -like '*.nuspec' } | Select-Object -First 1
            $reader = [System.IO.StreamReader]::new($nuspec.Open())
            try { [xml] $packageXml = $reader.ReadToEnd() } finally { $reader.Dispose() }
            $packageId = [string] $packageXml.package.metadata.id
            $dependencies = @($packageXml.package.metadata.dependencies.group.dependency | ForEach-Object { $_.id })
            $avaloniaDependencies = @($dependencies | Where-Object { $_ -eq 'Avalonia' -or $_ -like 'Avalonia.*' })
            if ($avaloniaDependencies.Count -gt 0) { throw "C0 package retains an Avalonia binary dependency: $packageId" }
        }
        finally {
            $archive.Dispose()
        }
    }
    throw 'Package build and inspection passed, but external application-template acceptance remains notVerified until Goal7 G7-3C replaces the removed legacy template.'
}

function Invoke-Clean {
    Invoke-Checked 'dotnet' @('clean', $solution, '--nologo')
    $allowedRoot = $dorotiRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
    $targets = @()
    foreach ($project in (Get-ChildItem -LiteralPath $dorotiRoot -Recurse -Filter '*.csproj' -File)) {
        $projectDirectory = $project.Directory.FullName
        foreach ($outputName in @('bin', 'obj')) {
            $outputPath = Join-Path $projectDirectory $outputName
            if (Test-Path -LiteralPath $outputPath -PathType Container) {
                $targets += Get-Item -LiteralPath $outputPath
            }
        }
    }
    if (Test-Path -LiteralPath $artifacts) { $targets += Get-Item -LiteralPath $artifacts }
    foreach ($target in $targets) {
        $resolved = [System.IO.Path]::GetFullPath($target.FullName)
        if (-not $resolved.StartsWith($allowedRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "Refusing to remove path outside Doroti root: $resolved"
        }
        Remove-Item -LiteralPath $resolved -Recurse -Force
    }
    & (Join-Path $PSScriptRoot 'clean-local-state.ps1') -Action temporary -Force
    Write-Host 'Clean: PASS'
}

switch ($Command) {
    'doctor' { Invoke-Doctor }
    'build' { Invoke-Build }
    'validate' { Invoke-Validation }
    'audit' { Invoke-Audit }
    'format' { Invoke-Format }
    'release' { Invoke-Release }
    'clean' { Invoke-Clean }
}
