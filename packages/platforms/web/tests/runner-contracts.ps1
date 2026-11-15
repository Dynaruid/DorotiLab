[CmdletBinding()]
param([Parameter(Mandatory)][string] $ProviderPackage, [Parameter(Mandatory)][string] $SdkFeed)
$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '../../../..')).Path
$output = Join-Path $repository 'Doroti/artifacts/web-qualification/remaining-runners'
[IO.Directory]::CreateDirectory($output) | Out-Null
$dotnet = Join-Path $env:ProgramFiles 'dotnet/dotnet.exe'
$sdk = Join-Path $env:ProgramFiles 'dotnet/sdk/10.0.403/dotnet.dll'
$runner = Join-Path $PSScriptRoot 'run-validation.ps1'
function Check-Runner([string] $Name, [string] $Project, [string[]] $Extra) {
    foreach ($mode in @('threaded', 'compatible', 'auto')) {
        $log = Join-Path $output "$Name-$mode.log"
        # Evaluate mode selection before restore. Full provider validation also
        # requires restored Blazor assets and is exercised by actual publishes.
        $arguments = @($sdk, 'msbuild', $Project, "-p:DorotiWebExecutionMode=$mode", '-getProperty:WasmEnableThreads,TargetFramework', '--nologo', '-v:quiet') + $Extra
        & $runner -Executable $dotnet -Arguments $arguments -LogPath $log -WorkingDirectory $repository
        if ($LASTEXITCODE) { throw "Runner configuration failed: $Name/$mode" }
        $properties = (Get-Content -LiteralPath $log -Raw | ConvertFrom-Json).Properties
        $expected = if ($mode -eq 'compatible') { 'false' } else { 'true' }
        if ($properties.WasmEnableThreads -ne $expected -or $properties.TargetFramework -ne 'net10.0') { throw "Wrong mode/default TFM: $Name/$mode" }
        Write-Host "PASS runner $Name/${mode}: threads=$expected; net10 default."
    }
    $log = Join-Path $output "$Name-conflict.log"
    & $runner -Executable $dotnet -Arguments (@($sdk, 'msbuild', $Project, '-t:ValidateDorotiRunnerProvider', '-p:DorotiWebExecutionMode=compatible', '-p:WasmEnableThreads=true', '--nologo', '-v:quiet') + $Extra) -LogPath $log -WorkingDirectory $repository
    if ($LASTEXITCODE -eq 0 -or (Get-Content -LiteralPath $log -Raw) -notmatch 'DOROTIWEB042') { throw "Thread override conflict was not rejected: $Name" }
    Write-Host "PASS runner ${Name}: conflicting raw thread override rejected."
}
foreach ($app in @('DorotiTestbedApp', 'DorotiSampleApp2', 'DorotiDemoApp', 'DorotiCarouselApp')) {
    Check-Runner $app (Join-Path $repository "samples/$app/web/$app.Web.csproj") @()
}
# Keep SDK resolution and package caches local to this generated template fixture.
$fixture = Join-Path $output ('template-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($fixture) | Out-Null
Copy-Item -Path (Join-Path $repository 'Doroti/templates/Doroti.Templates/content/doroti-app/*') -Destination $fixture -Recurse -Force
$provider = Join-Path $fixture 'provider'
[IO.Compression.ZipFile]::ExtractToDirectory((Resolve-Path -LiteralPath $ProviderPackage).Path, $provider)
$feed = [Security.SecurityElement]::Escape((Resolve-Path -LiteralPath $SdkFeed).Path)
$cache = [Security.SecurityElement]::Escape((Join-Path $fixture 'packages'))
@"
<configuration><packageSources><clear/><add key="candidate" value="$feed"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources><config><add key="globalPackagesFolder" value="$cache"/></config></configuration>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'NuGet.Config') -Encoding utf8NoBOM
$previousPackages = $env:NUGET_PACKAGES
try {
    $env:NUGET_PACKAGES = Join-Path $fixture 'packages'
    Check-Runner 'template' (Join-Path $fixture 'web/DorotiTemplateApp.Web.csproj') @(
        "-p:DorotiProviderSdkProps=$provider/provider-build/Sdk.props",
        "-p:DorotiProviderBootstrapTargets=$provider/bootstrap/Doroti.Provider.Bootstrap.targets")
} finally {
    if ($null -eq $previousPackages) { Remove-Item Env:NUGET_PACKAGES -ErrorAction SilentlyContinue } else { $env:NUGET_PACKAGES = $previousPackages }
}
Write-Host '20 runner/template mode contracts passed. This is MSBuild configuration validation, not a template browser publish.'
exit 0
