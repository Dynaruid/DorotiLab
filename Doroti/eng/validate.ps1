#Requires -Version 7.0
param(
    [ValidateSet('Source', 'Build', 'Targets', 'Developer', 'Release')]
    [string] $Suite = 'Developer'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repositoryRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$productSolution = Join-Path $dorotiRoot 'Doroti.Product.slnx'
$summaryPath = Join-Path $dorotiRoot 'artifacts/validation/summary.json'
$completed = [Collections.Generic.List[string]]::new()

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Write-Summary {
    [IO.Directory]::CreateDirectory((Split-Path $summaryPath -Parent)) | Out-Null
    $summary = [ordered]@{
        schemaVersion = 'doroti.validation-summary/v1'
        suite = $Suite
        status = 'pass'
        completed = @($completed)
        boundaries = [ordered]@{
            source = 'namespace and product-source ownership'
            build = 'Release product compilation'
            targets = 'single-project target graph and package build'
            release = 'Windows GPU presentation plus Web package-only publish'
            physical = 'notVerified'
            macCatalystNative = 'notVerified unless run on Apple Silicon macOS'
        }
    }
    $json = (($summary | ConvertTo-Json -Depth 16) -replace "`r`n", "`n") + "`n"
    [IO.File]::WriteAllText($summaryPath, $json, [Text.UTF8Encoding]::new($false))
}

function Invoke-SourceGate {
    Invoke-Checked { git -C $repositoryRoot diff --check } 'changed-file whitespace check failed'
    $scanRoots = @(
        (Join-Path $dorotiRoot 'src'),
        (Join-Path $dorotiRoot 'templates'),
        (Join-Path $dorotiRoot 'eng'),
        (Join-Path $dorotiRoot 'migration/selections'),
        (Join-Path $dorotiRoot 'migration/ports'),
        (Join-Path $repositoryRoot 'DorotiDemoApp'),
        (Join-Path $repositoryRoot 'tools/Doroti.DartToCSharp')
    ) | Where-Object { Test-Path -LiteralPath $_ -PathType Container }
    $textFiles = @($scanRoots | ForEach-Object {
        Get-ChildItem -LiteralPath $_ -Recurse -File |
            Where-Object {
                $_.FullName -notmatch '[\\/](bin|obj|build|artifacts|\.doroti)[\\/]' -and
                $_.Extension -in @('.cs', '.csproj', '.props', '.targets', '.ps1', '.sh', '.json', '.md', '.dart')
            }
    })
    $staleNamespacePattern = 'Doroti.' + 'Generated.Framework'
    $staleNamespace = @($textFiles | Select-String -SimpleMatch $staleNamespacePattern)
    Assert-True ($staleNamespace.Count -eq 0) 'legacy generated namespace absence'

    $frameworkRoots = @(Get-ChildItem -LiteralPath (Join-Path $dorotiRoot 'src') -Directory -Filter 'Doroti.Framework.*')
    $namespaceMismatches = [Collections.Generic.List[string]]::new()
    foreach ($frameworkRoot in $frameworkRoots) {
        $expectedNamespace = $frameworkRoot.Name
        foreach ($file in (Get-ChildItem -LiteralPath $frameworkRoot.FullName -Recurse -File -Filter '*.cs')) {
            if ($file.FullName -match '[\\/](bin|obj)[\\/]') { continue }
            $text = Get-Content -LiteralPath $file.FullName -Raw
            foreach ($match in [regex]::Matches($text, '(?m)^\s*namespace\s+([A-Za-z_][A-Za-z0-9_.]*)\s*[;{]')) {
                if ($match.Groups[1].Value -cne $expectedNamespace -and $match.Groups[1].Value -cne 'Doroti.Framework') {
                    $namespaceMismatches.Add("$($file.FullName):$($match.Groups[1].Value)")
                }
            }
        }
    }
    Assert-True ($namespaceMismatches.Count -eq 0) "framework namespace alignment: $($namespaceMismatches -join ', ')"
    $productGeneratedFiles = @($frameworkRoots | ForEach-Object {
        Get-ChildItem -LiteralPath $_.FullName -Recurse -File -Filter '*.g.cs' |
            Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
    })
    Assert-True ($productGeneratedFiles.Count -eq 0) 'compiler-owned framework source absence'

    $demoDartPackage = Join-Path $repositoryRoot 'DorotiDemoApp/dart'
    Assert-True (-not (Test-Path -LiteralPath $demoDartPackage)) 'C#-only DorotiDemoApp Dart package absence'
    $legacyDemoSelections = @(
        (Join-Path $dorotiRoot 'migration/selections/g6-generated-demo.json'),
        (Join-Path $dorotiRoot 'migration/selections/g6-generated-demo-unsupported-plugin.json')
    ) | Where-Object { Test-Path -LiteralPath $_ }
    Assert-True ($legacyDemoSelections.Count -eq 0) 'legacy generated-Dart DemoApp selection absence'

    $templateRoot = Join-Path $dorotiRoot 'templates/Doroti.Templates/content/doroti-app'
    Assert-True (@(Get-ChildItem -LiteralPath $templateRoot -Recurse -File -Filter '*.csproj').Count -eq 1) 'template project count'
    $templateXaml = @(Get-ChildItem -LiteralPath $templateRoot -Recurse -File -Filter '*.xaml')
    Assert-True ($templateXaml.Count -eq 1 -and $templateXaml[0].FullName.EndsWith('Platforms\Windows\App.xaml')) 'template bootstrap XAML boundary'
    $completed.Add('source')
}

function Invoke-BuildGate {
    Invoke-Checked { dotnet build $productSolution -c Release --nologo } 'Release product build failed'
    $completed.Add('build')
}

function Invoke-TargetGate {
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Graph
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Build
    $completed.Add('targets')
}

function Invoke-ReleaseGate {
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Live
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Evidence
    foreach ($shard in @('Toolchain', 'Reference', 'Hosting', 'Graph', 'Template', 'Compile', 'Publish')) {
        & (Join-Path $PSScriptRoot 'validate-web-product.ps1') -Shard $shard
    }
    $completed.Add('release')
}

switch ($Suite) {
    'Source' { Invoke-SourceGate }
    'Build' { Invoke-BuildGate }
    'Targets' { Invoke-TargetGate }
    'Developer' {
        Invoke-SourceGate
        Invoke-BuildGate
        Invoke-TargetGate
    }
    'Release' {
        Invoke-SourceGate
        Invoke-BuildGate
        Invoke-TargetGate
        Invoke-ReleaseGate
    }
}

Write-Summary
Write-Output "Doroti validation suite '$Suite': PASS"
