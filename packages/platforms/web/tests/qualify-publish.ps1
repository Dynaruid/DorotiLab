[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('10.0.403', '11.0.100')][string] $Sdk,
    [ValidateSet('net10.0', 'net11.0')][string] $Framework = 'net10.0',
    [ValidateSet('Debug', 'Release')][string] $Configuration = 'Debug',
    [ValidateSet('auto', 'threaded', 'compatible')][string] $Mode = 'auto',
    [ValidateSet('DorotiTestbedApp', 'DorotiSampleApp2', 'DorotiDemoApp', 'DorotiCarouselApp')][string] $App = 'DorotiTestbedApp',
    [ValidatePattern('^[a-zA-Z0-9-]*$')][string] $Label = '',
    [ValidatePattern('^([a-fA-F0-9]{32})?$')][string] $Generation = '',
    [switch] $DisableAot
)
$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$dotnet = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
$sdkDll = Join-Path $env:ProgramFiles "dotnet\sdk\$Sdk\dotnet.dll"
if (-not (Test-Path -LiteralPath $sdkDll -PathType Leaf)) { throw "System SDK is not installed: $Sdk" }
$output = Join-Path $repository "Doroti\artifacts\web-qualification\$App-$Sdk-$Framework-$Configuration-$Mode"
if ($Label) { $output += "-$Label" }
$reusedGeneration = [bool]$Generation
$clock = [Diagnostics.Stopwatch]::StartNew()
[IO.Directory]::CreateDirectory($output) | Out-Null
function Write-Qualification([string] $Status, [string] $Stage) {
    $receipt = [ordered]@{ status = $Status; stage = $Stage; sdk = $Sdk; framework = $Framework; configuration = $Configuration;
        mode = $Mode; app = $App; generationReused = $reusedGeneration; elapsedMilliseconds = $clock.ElapsedMilliseconds }
    if ($Status -eq 'passed') {
        $root = Join-Path $output 'publish/wwwroot'
        $profile = Get-Content -LiteralPath (Join-Path $root 'doroti.runtime-profile.json') -Raw | ConvertFrom-Json
        $receipt.buildId = $profile.buildId
        $receipt.provenance = $profile.provenance
        $receipt.publishBytes = (Get-ChildItem -LiteralPath $root -Recurse -File | Measure-Object -Property Length -Sum).Sum
    }
    $receipt | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $output 'qualification.json') -Encoding utf8NoBOM
}
Write-Qualification 'running' 'starting'
$project = Join-Path $repository "samples\$App\web\$App.Web.csproj"
$runner = Join-Path $PSScriptRoot 'run-validation.ps1'
[string[]] $probe = @()
if ($Framework -eq 'net11.0') { $probe += '-p:DorotiWebQualificationProbe=true' }
if ($DisableAot) { $probe += '-p:RunAOTCompilation=false' }
if ($Mode -eq 'auto') {
    $generation = if ($Generation) { $Generation } else { [Guid]::NewGuid().ToString('N') }
    $childRoot = Join-Path (Split-Path $project) "obj\doroti-web\$generation"
    foreach ($variant in @('threaded', 'compatible')) {
        Write-Qualification 'running' $variant
        $threads = if ($variant -eq 'threaded') { 'true' } else { 'false' }
        $arguments = @($sdkDll, 'publish', $project, '-c', $Configuration, '-p:DorotiWebPublishChild=true',
            "-p:DorotiWebExecutionMode=$variant", "-p:WasmEnableThreads=$threads", "-p:DorotiWebTargetFramework=$Framework",
            "-p:DorotiWebBuildId=$generation", "-p:PublishDir=$childRoot\$variant\publish\", '--artifacts-path', "$childRoot\$variant",
            '--nologo', '-v:minimal') + $probe
        & $runner -Executable $dotnet -Arguments $arguments -LogPath "$output\$variant.log" -WorkingDirectory $repository
        if ($LASTEXITCODE) { $code = $LASTEXITCODE; Write-Qualification 'failed' $variant; exit $code }
    }
    $arguments = @($sdkDll, 'msbuild', $project, '-t:_DorotiMergeDualPublish', "-p:Configuration=$Configuration",
        '-p:DorotiWebExecutionMode=auto', "-p:DorotiWebTargetFramework=$Framework", "-p:DorotiWebBuildId=$generation",
        "-p:PublishDir=$output\publish\", '--nologo', '-v:minimal') + $probe
    & $runner -Executable $dotnet -Arguments $arguments -LogPath "$output\finalize.log" -WorkingDirectory $repository
    $code = $LASTEXITCODE
    Write-Qualification $(if ($code) { 'failed' } else { 'passed' }) 'merge'
    exit $code
}
$arguments = @($sdkDll, 'publish', (Join-Path $repository "samples\$App\web\$App.Web.csproj"), '-c', $Configuration,
    "-p:DorotiWebExecutionMode=$Mode", "-p:DorotiWebTargetFramework=$Framework", "-p:PublishDir=$output\publish\",
    '--artifacts-path', "$output\parent", '--nologo', '-v:minimal') + $probe
& (Join-Path $PSScriptRoot 'run-validation.ps1') -Executable $dotnet -Arguments $arguments -LogPath "$output\publish.log" -WorkingDirectory $repository
$code = $LASTEXITCODE
Write-Qualification $(if ($code) { 'failed' } else { 'passed' }) 'publish'
exit $code
