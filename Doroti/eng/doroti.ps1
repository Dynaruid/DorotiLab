[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('doctor', 'build', 'validate', 'audit', 'release', 'clean')]
    [string] $Command = 'doctor',

    [ValidateSet('Source', 'Build', 'Targets', 'Fcr0', 'Developer', 'Release')]
    [string] $ValidationSuite = 'Developer'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
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

function Invoke-Doctor {
    $dotnet = Get-CommandResult 'dotnet' @('--version')
    $powerShell = [ordered]@{ available = $PSVersionTable.PSVersion.Major -ge 7; output = $PSVersionTable.PSVersion.ToString() }
    $flutterCheckout = Test-Path -LiteralPath (Join-Path $repositoryRoot 'reference/flutter-master') -PathType Container
    $success = $dotnet.available -and $powerShell.available
    $report = [ordered]@{
        schemaVersion = 'doroti.doctor/v3'
        success = $success
        dotnet = $dotnet
        powerShell = $powerShell
        referenceTools = [ordered]@{
            requiredForProductDevelopment = $false
            flutterCheckout = $flutterCheckout
            note = 'The pinned Flutter source is optional for reference comparison and migration work.'
        }
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
        "- PowerShell: $($powerShell.output)",
        "- Flutter reference checkout (optional): $flutterCheckout"
    ) -join "`n"
    [System.IO.File]::WriteAllText((Join-Path $outputDirectory 'doctor.md'), $markdown + "`n", [System.Text.UTF8Encoding]::new($false))
    Write-Host "Doctor: $(if ($success) { 'PASS' } else { 'FAIL' })"
    if (-not $success) { throw 'Development environment is missing a required product-development tool. See artifacts/doctor/doctor.json.' }
}

function Invoke-Build {
    Invoke-Checked 'dotnet' @('build', $productSolution, '--nologo')
}

function Invoke-Validation {
    & (Join-Path $PSScriptRoot 'validate.ps1') -Suite $ValidationSuite
}

function Invoke-Audit {
    & (Join-Path $PSScriptRoot 'validate-local-storage.ps1')
    & (Join-Path $PSScriptRoot 'validate.ps1') -Suite Source
}

function Invoke-Release {
    & (Join-Path $PSScriptRoot 'validate.ps1') -Suite Release
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
            if ($entries | Where-Object { $_ -match '\.(dart|yaml|lock)$' }) {
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
    Write-Host 'Release: PASS'
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
    'release' { Invoke-Release }
    'clean' { Invoke-Clean }
}
