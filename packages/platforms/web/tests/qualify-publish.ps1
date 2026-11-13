[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('10.0.403', '11.0.100')][string] $Sdk,
    [ValidateSet('net10.0', 'net11.0')][string] $Framework = 'net10.0',
    [ValidateSet('Debug', 'Release')][string] $Configuration = 'Debug',
    [ValidateSet('auto', 'threaded', 'compatible')][string] $Mode = 'auto',
    [ValidateSet('DorotiTestbedApp', 'DorotiSampleApp2')][string] $App = 'DorotiTestbedApp'
)
$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$dotnet = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
$sdkDll = Join-Path $env:ProgramFiles "dotnet\sdk\$Sdk\dotnet.dll"
if (-not (Test-Path -LiteralPath $sdkDll -PathType Leaf)) { throw "System SDK is not installed: $Sdk" }
$output = Join-Path $repository "Doroti\artifacts\web-qualification\$App-$Sdk-$Framework-$Configuration-$Mode"
$project = Join-Path $repository "samples\$App\web\$App.Web.csproj"
$runner = Join-Path $PSScriptRoot 'run-validation.ps1'
$probe = if ($Framework -eq 'net11.0') { @('-p:DorotiWebQualificationProbe=true') } else { @() }
if ($Configuration -eq 'Release' -and $Mode -eq 'auto') {
    $generation = [Guid]::NewGuid().ToString('N')
    $childRoot = Join-Path (Split-Path $project) "obj\doroti-web\$generation"
    foreach ($variant in @('threaded', 'compatible')) {
        $threads = if ($variant -eq 'threaded') { 'true' } else { 'false' }
        $arguments = @($sdkDll, 'publish', $project, '-c', $Configuration, '-p:DorotiWebPublishChild=true',
            "-p:DorotiWebExecutionMode=$variant", "-p:WasmEnableThreads=$threads", "-p:DorotiWebTargetFramework=$Framework",
            "-p:DorotiWebBuildId=$generation", "-p:PublishDir=$childRoot\$variant\publish\", '--artifacts-path', "$childRoot\$variant",
            '--nologo', '-v:minimal') + $probe
        & $runner -Executable $dotnet -Arguments $arguments -LogPath "$output\$variant.log" -WorkingDirectory $repository
        if ($LASTEXITCODE) { exit $LASTEXITCODE }
    }
    $arguments = @($sdkDll, 'msbuild', $project, '-t:_DorotiMergeDualPublish', "-p:Configuration=$Configuration",
        '-p:DorotiWebExecutionMode=auto', "-p:DorotiWebTargetFramework=$Framework", "-p:DorotiWebBuildId=$generation",
        "-p:PublishDir=$output\publish\", '--nologo', '-v:minimal') + $probe
    & $runner -Executable $dotnet -Arguments $arguments -LogPath "$output\finalize.log" -WorkingDirectory $repository
    exit $LASTEXITCODE
}
$arguments = @($sdkDll, 'publish', (Join-Path $repository "samples\$App\web\$App.Web.csproj"), '-c', $Configuration,
    "-p:DorotiWebExecutionMode=$Mode", "-p:DorotiWebTargetFramework=$Framework", "-p:PublishDir=$output\publish\",
    '--artifacts-path', "$output\parent", '--nologo', '-v:minimal') + $probe
& (Join-Path $PSScriptRoot 'run-validation.ps1') -Executable $dotnet -Arguments $arguments -LogPath "$output\publish.log" -WorkingDirectory $repository
exit $LASTEXITCODE
