#Requires -Version 5.1
param([string] $PackageRoot)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
if ([string]::IsNullOrWhiteSpace($PackageRoot)) {
    $packageRoot = Join-Path $repoRoot 'DorotiDemoApp/dart'
}
else {
    $packageRoot = [IO.Path]::GetFullPath($PackageRoot)
}

$sourceConfigPath = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/analyzer/flutter_package_config.json'
$targetDirectory = Join-Path $packageRoot '.dart_tool'
$targetConfigPath = Join-Path $targetDirectory 'package_config.json'
[IO.Directory]::CreateDirectory($targetDirectory) | Out-Null

$source = Get-Content -LiteralPath $sourceConfigPath -Raw | ConvertFrom-Json
$sourceConfigUri = [Uri]::new([IO.Path]::GetFullPath($sourceConfigPath))
$targetConfigUri = [Uri]::new([IO.Path]::GetFullPath($targetConfigPath))
$packages = [Collections.Generic.List[object]]::new()
foreach ($package in @($source.packages)) {
    $rootUri = [Uri]::new($sourceConfigUri, ([string]$package.rootUri).TrimEnd('/') + '/')
    $packages.Add([ordered]@{
        name = [string]$package.name
        rootUri = [Uri]::UnescapeDataString($targetConfigUri.MakeRelativeUri($rootUri).ToString()).TrimEnd('/')
        packageUri = [string]$package.packageUri
        languageVersion = [string]$package.languageVersion
    })
}
$packageRootUri = [Uri]::new(([IO.Path]::GetFullPath($packageRoot).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar))
$packages.Add([ordered]@{
    name = 'doroti_demo_app'
    rootUri = [Uri]::UnescapeDataString($targetConfigUri.MakeRelativeUri($packageRootUri).ToString()).TrimEnd('/')
    packageUri = 'lib/'
    languageVersion = '3.11'
    version = '0.2.0'
})
$document = [ordered]@{ configVersion = 2; packages = @($packages | Sort-Object name) }
$json = ($document | ConvertTo-Json -Depth 8) -replace "`r`n", "`n"
[IO.File]::WriteAllText($targetConfigPath, $json + "`n", [Text.UTF8Encoding]::new($false))
Write-Output 'G6-7 Dart DemoApp package configuration: PASS'
