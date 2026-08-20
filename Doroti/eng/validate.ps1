#Requires -Version 7.0
param(
    [ValidateSet('Source', 'Build', 'Targets', 'Fcr0', 'Fcr1', 'Fcr2', 'Fcr3', 'Fcr4', 'Fcr5', 'Fcr6', 'Fcr7', 'Fcr8', 'Developer', 'Release')]
    [string] $Suite = 'Developer'
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repositoryRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$productSolution = Join-Path $dorotiRoot 'Doroti.Product.slnx'
$mauiHostProject = Join-Path $dorotiRoot 'src/Doroti.Host.Maui/Doroti.Host.Maui.csproj'
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
            targets = 'platform-neutral app plus fixed-target runner graph and package build'
            interaction = 'source-contract coverage for pointer, keyboard, IME, lifecycle and semantics; live proof is separate'
            release = 'extended cross-build, native interop, package-only template, and Web static publish; live gates retain their own status'
            androidPhysical = 'notVerified'
            iosPhysical = 'notVerified'
            linuxX11 = 'notVerified'
            linuxWayland = 'notVerified'
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
    $templateRoot = Join-Path $dorotiRoot 'templates/Doroti.Templates/content/doroti-app'
    $templateProjects = @(Get-ChildItem -LiteralPath $templateRoot -Recurse -File -Filter '*.csproj' |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
    Assert-True ($templateProjects.Count -eq 12) 'template app plus seven runner and four native binding projects'
    $templateXaml = @(Get-ChildItem -LiteralPath $templateRoot -Recurse -File -Filter '*.xaml' |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
    Assert-True ($templateXaml.Count -eq 1 -and $templateXaml[0].Name -ceq 'App.xaml' -and $templateXaml[0].Directory.Name -ceq 'windows') 'template bootstrap XAML boundary'
    $templateLegacyFiles = if (Test-Path -LiteralPath (Join-Path $templateRoot 'Platforms')) {
        @(Get-ChildItem -LiteralPath (Join-Path $templateRoot 'Platforms') -Recurse -File)
    } else { @() }
    Assert-True ($templateLegacyFiles.Count -eq 0) 'template legacy Platforms source absence'
    $completed.Add('source')
}

function Invoke-BuildGate {
    Invoke-Checked { dotnet restore $productSolution --force-evaluate --nologo -m:1 } 'Release product restore failed'
    foreach ($targetFramework in @(
        'net10.0-windows10.0.19041.0',
        'net10.0-maccatalyst',
        'net10.0-macos',
        'net10.0-android'
    )) {
        Invoke-Checked {
            dotnet restore $mauiHostProject --no-dependencies --force-evaluate -p:TargetFramework=$targetFramework --nologo
        } "MAUI host restore failed for $targetFramework"
    }
    Invoke-Checked { dotnet build $productSolution -c Release --no-restore --nologo -m:1 } 'Release product build failed'
    $completed.Add('build')
}

function Invoke-TargetGate {
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Graph
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Build
    $completed.Add('targets')
}

function Invoke-Fcr0Gate {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-flutter-conformance.ps1') } 'Flutter conformance FCR-0 validation failed'
    $completed.Add('fcr-0-conformance')
}

function Invoke-Fcr1Gate {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-framework-shaders.ps1') } 'Framework shader FCR-1 validation failed'
    $completed.Add('fcr-1-framework-shaders')
}

function Invoke-Fcr2Gate {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-fcr2-semantics.ps1') } 'Dart-to-C# FCR-2 semantic validation failed'
    $completed.Add('fcr-2-dart-csharp-semantics')
}

function Invoke-Fcr3Gate {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-fcr3-scheduler.ps1') } 'Flutter scheduler FCR-3 validation failed'
    $completed.Add('fcr-3-scheduler')
}

function Invoke-Fcr4Gate {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-fcr4-retained-rendering.ps1') } 'Flutter retained rendering FCR-4 validation failed'
    $completed.Add('fcr-4-retained-rendering')
}

function Invoke-Fcr5Gate {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-fcr5-scroll.ps1') } 'Flutter scroll/viewport FCR-5 validation failed'
    $completed.Add('fcr-5-scroll')
}

function Invoke-Fcr6Gate {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-fcr6-semantics.ps1') } 'Flutter semantics FCR-6 validation failed'
    $completed.Add('fcr-6-semantics')
}

function Invoke-Fcr7Gate {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-fcr7-material-widget.ps1') } 'Material/widget FCR-7 validation failed'
    $completed.Add('fcr-7-material-widget')
}

function Invoke-Fcr8DeveloperGate {
    foreach ($shard in @('Inventory', 'Contracts', 'Differential', 'Evidence')) {
        Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-fcr8-stability.ps1') -Shard $shard } "FCR-8 stability shard '$shard' failed"
    }
    $completed.Add('fcr-8-stability-representative')
}

function Invoke-ReleaseGate {
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Package
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Template
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard NativeInterop
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Live
    & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard Evidence
    foreach ($shard in @('Toolchain', 'Hosting', 'Publish')) {
        & (Join-Path $PSScriptRoot 'validate-web-product.ps1') -Shard $shard
    }
    $completed.Add('release')
}

switch ($Suite) {
    'Source' { Invoke-SourceGate }
    'Build' { Invoke-BuildGate }
    'Targets' { Invoke-TargetGate }
    'Fcr0' { Invoke-Fcr0Gate }
    'Fcr1' { Invoke-Fcr1Gate }
    'Fcr2' { Invoke-Fcr2Gate }
    'Fcr3' { Invoke-Fcr3Gate }
    'Fcr4' { Invoke-Fcr4Gate }
    'Fcr5' { Invoke-Fcr5Gate }
    'Fcr6' { Invoke-Fcr6Gate }
    'Fcr7' { Invoke-Fcr7Gate }
    'Fcr8' {
        Invoke-Fcr7Gate
        Invoke-Fcr8DeveloperGate
    }
    'Developer' {
        Invoke-SourceGate
        Invoke-Fcr0Gate
        Invoke-Fcr1Gate
        Invoke-Fcr2Gate
        Invoke-Fcr3Gate
        Invoke-Fcr4Gate
        Invoke-Fcr5Gate
        Invoke-Fcr6Gate
        Invoke-Fcr7Gate
        Invoke-Fcr8DeveloperGate
        Invoke-BuildGate
        Invoke-TargetGate
    }
    'Release' {
        Invoke-SourceGate
        Invoke-Fcr0Gate
        Invoke-Fcr1Gate
        Invoke-Fcr2Gate
        Invoke-Fcr3Gate
        Invoke-Fcr4Gate
        Invoke-Fcr5Gate
        Invoke-Fcr6Gate
        Invoke-Fcr7Gate
        Invoke-Fcr8DeveloperGate
        Invoke-BuildGate
        Invoke-TargetGate
        Invoke-ReleaseGate
    }
}

Write-Summary
Write-Output "Doroti validation suite '$Suite': PASS"
