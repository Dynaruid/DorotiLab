[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('start', 'stop', 'url', 'status', 'logs')]
    [string] $Command = 'start',

    [switch] $SkipPublish,

    [string] $App = './samples/DorotiTestbedApp',

    [ValidateRange(15, 600)]
    [int] $TimeoutSeconds = 120
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$composePath = Join-Path $PSScriptRoot 'compose.yaml'
$urlPath = Join-Path $PSScriptRoot 'generated-url.txt'
$appRoot = if ([IO.Path]::IsPathFullyQualified($App)) {
    [IO.Path]::GetFullPath($App)
} else {
    [IO.Path]::GetFullPath((Join-Path $repositoryRoot $App))
}
$webRoot = Join-Path $appRoot 'web/bin/Release/net10.0/publish/wwwroot'

function Invoke-Compose {
    $previousWebRoot = $env:DOROTI_PREVIEW_WEBROOT
    try {
        $env:DOROTI_PREVIEW_WEBROOT = $webRoot.Replace('\', '/')
        & docker compose --project-name doroti-web-tunnel --project-directory $PSScriptRoot -f $composePath @args
        if ($LASTEXITCODE -ne 0) {
            throw "Docker Compose failed (exit $LASTEXITCODE): $args"
        }
    }
    finally {
        $env:DOROTI_PREVIEW_WEBROOT = $previousWebRoot
    }
}

function Get-TunnelUrl {
    $container = Invoke-Compose ps --status running -q cloudflared
    if (-not $container) { return $null }
    $log = (Invoke-Compose logs --no-color cloudflared) -join "`n"
    $urls = [regex]::Matches($log, 'https://[a-zA-Z0-9-]+\.trycloudflare\.com')
    if ($urls.Count -gt 0) { return $urls[$urls.Count - 1].Value }
    return $null
}

function Save-TunnelUrl([string] $Url) {
    [IO.File]::WriteAllText($urlPath, "$Url`n", [Text.UTF8Encoding]::new($false))
    Write-Host "Doroti HTTPS: $Url"
    Write-Host "URL file: $urlPath"
}

if ($SkipPublish -and $Command -ne 'start') {
    throw '-SkipPublish is supported only by start.'
}

switch ($Command) {
    'stop' {
        Invoke-Compose down
        Remove-Item -LiteralPath $urlPath -ErrorAction SilentlyContinue
        Write-Host 'Doroti web and Quick Tunnel stopped.'
    }
    'status' { Invoke-Compose ps }
    'logs' { Invoke-Compose logs --follow --tail 100 }
    'url' {
        $url = Get-TunnelUrl
        if (-not $url) {
            Remove-Item -LiteralPath $urlPath -ErrorAction SilentlyContinue
            throw 'No running Quick Tunnel URL found. Run tunnel.ps1 start first.'
        }
        Save-TunnelUrl $url
    }
    'start' {
        if (-not (Test-Path -LiteralPath (Join-Path $appRoot 'doroti-workspace.json') -PathType Leaf)) {
            throw "Missing Doroti workspace: $appRoot"
        }
        $workspace = Get-Content -LiteralPath (Join-Path $appRoot 'doroti-workspace.json') -Raw | ConvertFrom-Json
        $webPlatform = $workspace.platforms.web
        $webRunner = if ($webPlatform -is [string]) { $webPlatform } else { $webPlatform.runner }
        if ([string]::IsNullOrWhiteSpace($webRunner)) {
            throw "Workspace has no web runner: $appRoot"
        }
        $webProject = Join-Path $appRoot $webRunner
        if (-not (Test-Path -LiteralPath $webProject -PathType Leaf)) {
            throw "Missing web runner: $webProject"
        }
        # Check Docker before spending time publishing the app.
        & docker info --format '{{.ServerVersion}}'
        if ($LASTEXITCODE -ne 0) { throw 'Start Docker Desktop with Linux containers first.' }
        Invoke-Compose version

        if (-not $SkipPublish) {
            # Stop an older preview before overwriting its mounted publish files.
            Invoke-Compose down
            Remove-Item -LiteralPath $urlPath -ErrorAction SilentlyContinue
            Push-Location $repositoryRoot
            try {
                # Publish the declared web runner even for samples that only
                # provide a subset of the workspace CLI's platform aliases.
                & dotnet publish $webProject -c Release --nologo -p:UseSharedCompilation=false
                if ($LASTEXITCODE -ne 0) { throw 'Doroti web Release publish failed.' }
            }
            finally { Pop-Location }
        }

        foreach ($asset in @('index.html', 'doroti_bootstrap.js', 'doroti-app-manifest.json')) {
            if (-not (Test-Path -LiteralPath (Join-Path $webRoot $asset) -PathType Leaf)) {
                throw "Missing published asset: $asset. Run start without -SkipPublish."
            }
        }
        if (-not (Get-ChildItem -LiteralPath (Join-Path $webRoot '_framework') -Filter 'dotnet.*.js' -ErrorAction SilentlyContinue)) {
            throw 'Missing published .NET runtime. Run start without -SkipPublish.'
        }

        Remove-Item -LiteralPath $urlPath -ErrorAction SilentlyContinue
        try {
            Invoke-Compose up --detach --wait --wait-timeout $TimeoutSeconds
            $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
            $lastFailure = 'Waiting for the Quick Tunnel URL.'
            do {
                $url = Get-TunnelUrl
                if ($url) {
                    try {
                        $response = Invoke-WebRequest -Uri "$url/index.html" -TimeoutSec 10
                        if ($response.StatusCode -ne 200 -or
                            $response.Content -notmatch 'doroti_bootstrap\.js' -or
                            $response.Headers['Cross-Origin-Opener-Policy'] -notcontains 'same-origin' -or
                            $response.Headers['Cross-Origin-Embedder-Policy'] -notcontains 'require-corp') {
                            throw 'Public response is missing the Doroti page or COOP/COEP headers.'
                        }
                        foreach ($runtimeAsset in @(
                            @{ Filter = 'dotnet.native.worker.*.mjs'; Mime = 'application/javascript' },
                            @{ Filter = 'dotnet.native.*.wasm'; Mime = 'application/wasm' }
                        )) {
                            $file = Get-ChildItem -LiteralPath (Join-Path $webRoot '_framework') -Filter $runtimeAsset.Filter | Select-Object -First 1
                            if (-not $file) { throw "Missing runtime asset: $($runtimeAsset.Filter)" }
                            $assetResponse = Invoke-WebRequest -Method Head -Uri "$url/_framework/$($file.Name)" -TimeoutSec 10
                            if ($assetResponse.Headers['Content-Type'] -notcontains $runtimeAsset.Mime -or
                                $assetResponse.Headers['Cross-Origin-Embedder-Policy'] -notcontains 'require-corp') {
                                throw "Incorrect runtime MIME/isolation headers: $($file.Name)"
                            }
                        }
                        Save-TunnelUrl $url
                        Write-Host 'HTTPS page, COOP/COEP headers and worker/WASM MIME types verified. Open this URL on the other device.'
                        Write-Host 'Stop: pwsh -NoProfile -File ./tools/doroti-cloudflared/tunnel.ps1 stop'
                        return
                    }
                    catch { $lastFailure = $_.Exception.Message }
                }
                Start-Sleep -Seconds 3
            } while ([DateTime]::UtcNow -lt $deadline)
            throw "Quick Tunnel was not ready within $TimeoutSeconds seconds. $lastFailure"
        }
        catch {
            Write-Warning 'Startup did not complete; stopping the preview and tunnel.'
            Invoke-Compose logs --no-color --tail 40
            Invoke-Compose down
            Remove-Item -LiteralPath $urlPath -ErrorAction SilentlyContinue
            throw
        }
    }
}
