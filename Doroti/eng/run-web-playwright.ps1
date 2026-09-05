[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [switch] $SkipBuild,

    [switch] $HeadlessOnly,

    [switch] $HeadedOnly,

    [switch] $FastResize,

    [string[]] $TestFile,

    [ValidateSet('auto', 'document-webgl', 'offscreen-bitmap', 'offscreen-worker', 'worker-direct-webgl', 'worker-canvaskit-webgl')]
    [string] $RendererMode = 'auto',

    [switch] $RequireLatencyGate,

    [string] $ArtifactLabel = '',

    [ValidateRange(1024, 65535)]
    [int] $Port = 5088
)

$ErrorActionPreference = 'Stop'
if ($FastResize) {
    if ($HeadlessOnly -or $TestFile.Count -gt 0) { throw '-FastResize selects its headed native test; omit -HeadlessOnly and -TestFile.' }
    if (-not $PSBoundParameters.ContainsKey('RendererMode')) { $RendererMode = 'worker-canvaskit-webgl' }
    if ($RendererMode -ne 'worker-canvaskit-webgl') { throw '-FastResize currently measures worker-canvaskit-webgl.' }
    $HeadedOnly = $true
    $TestFile = @('tests/canvaskit-native-fast-resize.spec.ts')
    if ([string]::IsNullOrWhiteSpace($ArtifactLabel)) { $ArtifactLabel = 'canvaskit-native-fast-resize' }
}
$timeout = [TimeSpan]::FromMinutes(20)
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$project = Join-Path $repositoryRoot 'DorotiDemoApp/web/DorotiDemoApp.Web.csproj'
$playwrightRoot = Join-Path $dorotiRoot 'validation/web-playwright'
$resolvedArtifactLabel = if ([string]::IsNullOrWhiteSpace($ArtifactLabel)) { $RendererMode } else { $ArtifactLabel }
if ($resolvedArtifactLabel -match '\.\.' -or $resolvedArtifactLabel -notmatch '^[a-zA-Z0-9._/-]+$') {
    throw "Invalid artifact label '$resolvedArtifactLabel'."
}
$artifactRoot = Join-Path $playwrightRoot "artifacts/wrapper/$resolvedArtifactLabel"
$baseUrl = "http://127.0.0.1:$Port"

if ($HeadlessOnly -and $HeadedOnly) {
    throw '-HeadlessOnly and -HeadedOnly are mutually exclusive.'
}

if (-not (Test-Path -LiteralPath $project -PathType Leaf)) {
    throw "Doroti web project was not found: $project"
}
if (-not (Test-Path -LiteralPath (Join-Path $playwrightRoot 'package.json') -PathType Leaf)) {
    throw "Playwright validation package was not found: $playwrightRoot"
}

[IO.Directory]::CreateDirectory($artifactRoot) | Out-Null
$dotnet = (Get-Command dotnet -ErrorAction Stop).Source
$npx = (Get-Command npx.cmd -ErrorAction Stop).Source

function Invoke-OwnedProcess {
    param(
        [Parameter(Mandatory)] [string] $FilePath,
        [Parameter(Mandatory)] [string[]] $ArgumentList,
        [Parameter(Mandatory)] [string] $WorkingDirectory,
        [Parameter(Mandatory)] [string] $Name
    )

    $stdout = Join-Path $artifactRoot "$Name.stdout.log"
    $stderr = Join-Path $artifactRoot "$Name.stderr.log"
    $process = Start-Process -FilePath $FilePath -ArgumentList $ArgumentList `
        -WorkingDirectory $WorkingDirectory -WindowStyle Hidden -PassThru `
        -RedirectStandardOutput $stdout -RedirectStandardError $stderr
    try {
        if (-not $process.WaitForExit([int] $timeout.TotalMilliseconds)) {
            try { $process.Kill($true) } catch { }
            throw "$Name exceeded the repository 20-minute timeout."
        }
        if ($process.ExitCode -ne 0) {
            $output = if (Test-Path -LiteralPath $stdout) { Get-Content -LiteralPath $stdout -Raw } else { '' }
            $errors = if (Test-Path -LiteralPath $stderr) { Get-Content -LiteralPath $stderr -Raw } else { '' }
            throw "$Name failed with exit code $($process.ExitCode).`n$output`n$errors"
        }
    }
    finally {
        $process.Dispose()
    }
}

$listener = Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue
if ($listener) {
    throw "Port $Port is already in use. Select another -Port for the owned validation server."
}

if (-not $SkipBuild) {
    Invoke-OwnedProcess -FilePath $dotnet `
        -ArgumentList @('build', $project, '--configuration', $Configuration, '--nologo') `
        -WorkingDirectory $repositoryRoot -Name 'build'
}
if ($FastResize) {
    $cmake = (Get-Command cmake -ErrorAction Stop).Source
    Invoke-OwnedProcess -FilePath $cmake `
        -ArgumentList @('-S', 'Doroti/validation/windows-resize-capture', '-B', '.doroti/build/windows-resize-capture-vulkan', '-A', 'x64') `
        -WorkingDirectory $repositoryRoot -Name 'native-drag-driver-configure'
    Invoke-OwnedProcess -FilePath $cmake `
        -ArgumentList @('--build', '.doroti/build/windows-resize-capture-vulkan', '--config', 'Release', '--target', 'Doroti.WindowsResizeCapture') `
        -WorkingDirectory $repositoryRoot -Name 'native-drag-driver-build'
}

$serverStdout = Join-Path $artifactRoot 'server.stdout.log'
$serverStderr = Join-Path $artifactRoot 'server.stderr.log'
$server = Start-Process -FilePath $dotnet `
    -ArgumentList @('run', '--project', $project, '--configuration', $Configuration, '--no-build', '--no-restore', '--no-launch-profile', '--urls', $baseUrl) `
    -WorkingDirectory $repositoryRoot -WindowStyle Hidden -PassThru `
    -RedirectStandardOutput $serverStdout -RedirectStandardError $serverStderr

try {
    $readyDeadline = [DateTime]::UtcNow.AddMinutes(2)
    $ready = $false
    while ([DateTime]::UtcNow -lt $readyDeadline) {
        if ($server.HasExited) {
            $output = if (Test-Path -LiteralPath $serverStdout) { Get-Content -LiteralPath $serverStdout -Raw } else { '' }
            $errors = if (Test-Path -LiteralPath $serverStderr) { Get-Content -LiteralPath $serverStderr -Raw } else { '' }
            throw "Doroti web server exited before readiness.`n$output`n$errors"
        }
        try {
            $response = Invoke-WebRequest -Uri $baseUrl -UseBasicParsing -TimeoutSec 2
            if ($response.StatusCode -eq 200) {
                $ready = $true
                break
            }
        }
        catch { }
        Start-Sleep -Milliseconds 250
    }
    if (-not $ready) {
        throw "Doroti web server did not become ready at $baseUrl within two minutes."
    }

    $previousBaseUrl = $env:DOROTI_WEB_BASE_URL
    $previousRendererMode = $env:DOROTI_WEB_RENDERER_MODE
    $previousRequireLatency = $env:DOROTI_WEB_REQUIRE_LATENCY
    $previousArtifactLabel = $env:DOROTI_WEB_ARTIFACT_LABEL
    $previousFastResize = $env:DOROTI_WEB_FAST_RESIZE
    try {
        $env:DOROTI_WEB_BASE_URL = $baseUrl
        $env:DOROTI_WEB_RENDERER_MODE = $RendererMode
        $env:DOROTI_WEB_REQUIRE_LATENCY = if ($RequireLatencyGate) { '1' } else { '0' }
        $env:DOROTI_WEB_ARTIFACT_LABEL = $resolvedArtifactLabel
        $env:DOROTI_WEB_FAST_RESIZE = if ($FastResize) { '1' } else { '0' }
        $arguments = @('playwright', 'test')
        if ($TestFile.Count -gt 0) { $arguments += $TestFile }
        if ($HeadlessOnly) {
            $arguments += '--project=chromium-hardware'
            $arguments += '--project=chromium-dpr2'
        }
        elseif ($HeadedOnly) {
            $arguments += '--project=desktop-chrome-headed'
        }
        Invoke-OwnedProcess -FilePath $npx -ArgumentList $arguments `
            -WorkingDirectory $playwrightRoot -Name 'playwright'
    }
    finally {
        if ($null -eq $previousFastResize) { Remove-Item Env:DOROTI_WEB_FAST_RESIZE -ErrorAction SilentlyContinue }
        else { $env:DOROTI_WEB_FAST_RESIZE = $previousFastResize }
        if ($null -eq $previousBaseUrl) { Remove-Item Env:DOROTI_WEB_BASE_URL -ErrorAction SilentlyContinue }
        else { $env:DOROTI_WEB_BASE_URL = $previousBaseUrl }
        if ($null -eq $previousRendererMode) { Remove-Item Env:DOROTI_WEB_RENDERER_MODE -ErrorAction SilentlyContinue }
        else { $env:DOROTI_WEB_RENDERER_MODE = $previousRendererMode }
        if ($null -eq $previousRequireLatency) { Remove-Item Env:DOROTI_WEB_REQUIRE_LATENCY -ErrorAction SilentlyContinue }
        else { $env:DOROTI_WEB_REQUIRE_LATENCY = $previousRequireLatency }
        if ($null -eq $previousArtifactLabel) { Remove-Item Env:DOROTI_WEB_ARTIFACT_LABEL -ErrorAction SilentlyContinue }
        else { $env:DOROTI_WEB_ARTIFACT_LABEL = $previousArtifactLabel }
    }
}
finally {
    if (-not $server.HasExited) {
        try { $server.Kill($true) } catch { }
        try { $server.WaitForExit(10000) | Out-Null } catch { }
    }
    $server.Dispose()
}
