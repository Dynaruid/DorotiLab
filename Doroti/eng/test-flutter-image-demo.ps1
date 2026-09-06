#Requires -Version 7.0
[CmdletBinding()]
param([int] $Port = 5097, [string] $ArtifactLabel = 'flutter-image-demo')
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$output = Join-Path $repoRoot ".doroti/evidence/$ArtifactLabel"
if (Test-Path -LiteralPath $output) { throw 'Select a new artifact label to preserve evidence.' }
if (Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue) { throw "Port $Port is in use." }
if (!(Test-Path (Join-Path $repoRoot 'reference/flutter_sample_app/build/web/index.html'))) {
    throw 'Build reference/flutter_sample_app with flutter build web --release first.'
}
New-Item -ItemType Directory -Path $output | Out-Null
$previousEnvironment = @{}
foreach ($name in @('DOROTI_FLUTTER_IMAGE_DEMO','DOROTI_WEB_BASE_URL','DOROTI_WEB_ARTIFACT_LABEL')) {
    $previousEnvironment[$name] = [Environment]::GetEnvironmentVariable($name)
}
$server = Start-Process -FilePath (Get-Command python.exe).Source `
    -ArgumentList @('Doroti/eng/serve-web-static.py','--port',"$Port",'--directory','reference/flutter_sample_app/build/web') `
    -WorkingDirectory $repoRoot -WindowStyle Hidden -PassThru `
    -RedirectStandardOutput (Join-Path $output 'server.stdout.log') -RedirectStandardError (Join-Path $output 'server.stderr.log')
try {
    $env:DOROTI_FLUTTER_IMAGE_DEMO = '1'
    $env:DOROTI_WEB_BASE_URL = "http://127.0.0.1:$Port"
    $env:DOROTI_WEB_ARTIFACT_LABEL = $ArtifactLabel
    $process = Start-Process -FilePath (Get-Command npx.cmd).Source `
        -ArgumentList @('playwright','test','tests/flutter-image-demo.spec.ts','--project=chromium-hardware') `
        -WorkingDirectory (Join-Path $repoRoot 'Doroti/validation/web-playwright') -WindowStyle Hidden -PassThru `
        -RedirectStandardOutput (Join-Path $output 'playwright.stdout.log') -RedirectStandardError (Join-Path $output 'playwright.stderr.log')
    try {
        if (!$process.WaitForExit(1200000)) { $process.Kill($true); throw 'Flutter image demo exceeded 20 minutes.' }
        Get-Content (Join-Path $output 'playwright.stdout.log')
        if ($process.ExitCode -ne 0) { throw "Flutter image demo failed. Evidence: $output" }
    } finally { $process.Dispose() }
} finally {
    if (!$server.HasExited) { $server.Kill($true) }
    $server.Dispose()
    foreach ($name in $previousEnvironment.Keys) {
        [Environment]::SetEnvironmentVariable($name, $previousEnvironment[$name])
    }
}
