[CmdletBinding()]
param(
    [ValidatePattern('^[a-zA-Z0-9-]*$')][string] $Label = '',
    [switch] $IncludeAot
)
$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '../../../..')).Path
$output = Join-Path $repository 'Doroti/artifacts/web-qualification/remaining-switch-cache'
if ($Label) { $output += "-$Label" }
[IO.Directory]::CreateDirectory($output) | Out-Null
$dotnet = Join-Path $env:ProgramFiles 'dotnet/dotnet.exe'
$runner = Join-Path $PSScriptRoot 'run-validation.ps1'
$project = Join-Path $repository 'samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj'
$receipts = @()
$cases = @(
    @{ Sdk = '10.0.403'; Mode = 'threaded' },
    @{ Sdk = '10.0.403'; Mode = 'compatible' },
    @{ Sdk = '10.0.403'; Mode = 'threaded' },
    @{ Sdk = '11.0.100'; Mode = 'threaded' },
    @{ Sdk = '10.0.403'; Mode = 'threaded' },
    @{ Sdk = '11.0.100'; Mode = 'threaded'; Framework = 'net11.0' },
    @{ Sdk = '10.0.403'; Mode = 'threaded' }
)
if ($IncludeAot) {
    $cases += @(
        @{ Sdk = '10.0.403'; Mode = 'threaded'; Configuration = 'Release'; Aot = $true },
        @{ Sdk = '10.0.403'; Mode = 'compatible'; Configuration = 'Release'; Aot = $true },
        @{ Sdk = '10.0.403'; Mode = 'threaded'; Configuration = 'Release'; Aot = $true },
        @{ Sdk = '10.0.403'; Mode = 'threaded'; Configuration = 'Release'; Aot = $false },
        @{ Sdk = '10.0.403'; Mode = 'threaded'; Configuration = 'Release'; Aot = $true }
    )
}
foreach ($case in $cases) {
    $step = $receipts.Count + 1
    $generation = [Guid]::NewGuid().ToString('N')
    $sdk = Join-Path $env:ProgramFiles "dotnet/sdk/$($case.Sdk)/dotnet.dll"
    $threads = if ($case.Mode -eq 'threaded') { 'true' } else { 'false' }
    $configuration = if ($case.Configuration) { $case.Configuration } else { 'Debug' }
    $framework = if ($case.Framework) { $case.Framework } else { 'net10.0' }
    $aot = if ($case.Aot) { 'true' } else { 'false' }
    $qualification = if ($framework -eq 'net11.0') { @('-p:DorotiWebQualificationProbe=true') } else { @() }
    # Deliberately reuse all output/intermediate and deployment paths.
    & $runner -Executable $dotnet -Arguments (@($sdk, 'publish', $project, '-c', $configuration,
        "-p:DorotiWebExecutionMode=$($case.Mode)", "-p:WasmEnableThreads=$threads", "-p:DorotiWebTargetFramework=$framework",
        "-p:RunAOTCompilation=$aot",
        "-p:DorotiWebBuildId=$generation", '--artifacts-path', "$output/cache", "-p:PublishDir=$output/publish/", '--nologo', '-v:minimal') + $qualification) `
        -LogPath "$output/$step-publish.log" -WorkingDirectory $repository
    if ($LASTEXITCODE) { exit $LASTEXITCODE }
    $profile = Get-Content -LiteralPath "$output/publish/wwwroot/doroti.runtime-profile.json" -Raw | ConvertFrom-Json
    if ($profile.threads -ne ($threads -eq 'true') -or $profile.buildId -ne $generation -or $profile.provenance.sdk -ne $case.Sdk -or
        $profile.provenance.targetFramework -ne $framework -or $profile.provenance.configuration -ne $configuration -or $profile.provenance.aot -ne $aot) { throw 'Mode/SDK/TFM/AOT cache contamination.' }
    & $runner -Executable (Get-Command node).Source -Arguments @('packages/platforms/web/tests/browser-smoke.mjs', "$output/publish/wwwroot", "--single=$($case.Mode)") `
        -LogPath "$output/$step-browser.log" -WorkingDirectory $repository
    if ($LASTEXITCODE) { exit $LASTEXITCODE }
    $receipts += [ordered]@{ step = $step; sdk = $case.Sdk; framework = $framework; mode = $case.Mode; buildId = $generation; provenance = $profile.provenance;
        result = 'PASS native publish, actual first frame, paint owner, input, shutdown'; configuration = $configuration; aot = $aot }
    ConvertTo-Json -InputObject $receipts -Depth 8 | Set-Content -LiteralPath "$output/results.json" -Encoding utf8NoBOM
}
Write-Host "PASS $($cases.Count) in-place mode/SDK/TFM/AOT switches; each native publish and browser child used an external 1200-second timeout."
exit 0
