[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('start', 'stop', 'url', 'status', 'logs', 'validate')]
    [string] $Command = 'start',

    [switch] $SkipPublish,

    [switch] $Obfuscate,

    [switch] $DisableAot,

    [ValidateSet('threaded', 'compatible', 'auto')]
    [string] $ExecutionMode = 'threaded',

    [string] $App = './samples/DorotiTestbedApp',

    [string] $PublishRoot,

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
$webRoot = if ($PublishRoot) { [IO.Path]::GetFullPath($PublishRoot) } else { Join-Path $appRoot 'web/bin/Release/net10.0/publish/wwwroot' }
if ([IO.Path]::GetFileName($webRoot.TrimEnd('\', '/')) -ne 'wwwroot') { throw 'PublishRoot must name a published wwwroot directory.' }

function Get-PublishedRuntimeRoots {
    foreach ($asset in @('index.html', 'doroti_bootstrap.js', 'doroti.runtime-profile.json', '_content/Doroti.Host.Web/doroti.loader.js', '_content/Doroti.Host.Web/doroti.raster.worker.js')) {
        if (-not (Test-Path -LiteralPath (Join-Path $webRoot $asset) -PathType Leaf)) { throw "Missing published asset: $asset" }
    }
    $profile = Get-Content -LiteralPath (Join-Path $webRoot 'doroti.runtime-profile.json') -Raw | ConvertFrom-Json
    $roots = if ($profile.schemaVersion -eq 2) {
        if ($profile.hostContract -ne 'doroti.web.execution/v1' -or @($profile.variants.PSObject.Properties).Count -ne 2) { throw 'Expected both dual runtime variants.' }
        if (Test-Path -LiteralPath (Join-Path $webRoot '_framework')) { throw 'Dual deployment must not contain a root framework.' }
        foreach ($mode in @('threaded', 'compatible')) {
            $variant = $profile.variants.$mode
            $prefix = "runtimes/$mode/"
            if ($null -eq $variant -or $variant.threads -isnot [bool] -or $variant.threads -ne ($mode -eq 'threaded') -or
                $variant.profileUrl -ne "${prefix}doroti.runtime-profile.json" -or $variant.loaderUrl -notmatch "^${prefix}_framework/dotnet(\.[a-zA-Z0-9_-]+)?\.js$" -or
                $variant.loaderUrl -match '/dotnet\.(native|runtime)') { throw "Invalid runtime variant: $mode" }
            $child = Get-Content -LiteralPath (Join-Path $webRoot $variant.profileUrl) -Raw | ConvertFrom-Json
            foreach ($field in @('buildId', 'runtimeContract', 'runtimeVersion')) {
                if (-not $profile.$field -or $child.$field -ne $profile.$field) { throw "Runtime variant $mode differs: $field" }
            }
            if ($child.schemaVersion -ne 1 -or $child.threads -isnot [bool] -or $child.threads -ne $variant.threads) { throw "Invalid runtime profile: $mode" }
            if (($child.provenance | ConvertTo-Json -Compress -Depth 8) -ne ($profile.provenance | ConvertTo-Json -Compress -Depth 8)) { throw "Runtime variant toolchain differs: $mode" }
            @{ Relative = "${prefix}_framework"; Threads = $variant.threads; Contract = $child.runtimeContract; Loader = [IO.Path]::GetFileName($variant.loaderUrl) }
        }
    } elseif ($profile.schemaVersion -eq 1 -and $profile.threads -is [bool]) {
        $html = Get-Content -LiteralPath (Join-Path $webRoot 'index.html') -Raw
        $loaders = @(
            foreach ($match in [regex]::Matches($html, '<script\b[^>]*type\s*=\s*["'']importmap["''][^>]*>(.*?)</script>', 'IgnoreCase,Singleline')) {
                $map = $match.Groups[1].Value | ConvertFrom-Json
                foreach ($key in @('./_framework/dotnet.js', '/_framework/dotnet.js', '_framework/dotnet.js')) {
                    $mapped = $map.imports.PSObject.Properties[$key].Value
                    if ($mapped) {
                        if ($mapped -notmatch '^(\./|/)?_framework/(dotnet(\.[a-zA-Z0-9_-]+)?\.js)$') { throw 'Invalid dotnet loader import mapping.' }
                        $Matches[2]
                    }
                }
            }
        ) | Select-Object -Unique
        if (-not $loaders -and (Test-Path -LiteralPath (Join-Path $webRoot '_framework/dotnet.js'))) { $loaders = @('dotnet.js') }
        if (@($loaders).Count -ne 1) { throw 'Missing or ambiguous dotnet loader mapping.' }
        @{ Relative = '_framework'; Threads = $profile.threads; Contract = $profile.runtimeContract; Loader = @($loaders)[0] }
    } else { throw 'Unsupported runtime profile.' }
    foreach ($runtimeRoot in $roots) {
        if ($runtimeRoot.Contract -notin @('mono-10', 'mono-11', $null)) { throw 'Unsupported runtime contract.' }
        $assets = @(@{ Name = $runtimeRoot.Loader; Mime = 'application/javascript' })
        foreach ($pattern in @('dotnet.native*.wasm', 'dotnet.native*.js', 'dotnet.runtime*.js')) {
            $files = @(Get-ChildItem -LiteralPath (Join-Path $webRoot $runtimeRoot.Relative) -Filter $pattern -File)
            if (-not $files.Count) { throw "Missing runtime asset: $pattern" }
            $assets += @{ Name = $files[0].Name; Mime = $(if ($pattern.EndsWith('.wasm')) { 'application/wasm' } else { 'application/javascript' }) }
        }
        # Mono 11 uses the exact loader entry, rather than a separate worker.mjs.
        if ($runtimeRoot.Threads -and $runtimeRoot.Contract -ne 'mono-11') {
            $worker = Get-ChildItem -LiteralPath (Join-Path $webRoot $runtimeRoot.Relative) -Filter 'dotnet.native.worker*.mjs' -File | Select-Object -First 1
            if (-not $worker) { throw 'Missing Mono 10 pthread worker asset.' }
            $assets += @{ Name = $worker.Name; Mime = 'application/javascript' }
        }
        foreach ($asset in $assets) {
            if (-not (Test-Path -LiteralPath (Join-Path $webRoot "$($runtimeRoot.Relative)/$($asset.Name)") -PathType Leaf)) { throw "Missing runtime asset: $($asset.Name)" }
        }
        $runtimeRoot.Assets = $assets
        $runtimeRoot
    }
}

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
if (($Obfuscate -or $DisableAot) -and ($Command -ne 'start' -or $SkipPublish)) {
    throw '-Obfuscate and -DisableAot require start without -SkipPublish so the selected build settings are applied.'
}

switch ($Command) {
    'validate' { Get-PublishedRuntimeRoots | ConvertTo-Json -Depth 6 }
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
                $publishArguments = @('publish', $webProject, '-c', 'Release', '--nologo', '-p:UseSharedCompilation=false', "-p:DorotiWebExecutionMode=$ExecutionMode")
                if ($PublishRoot) { $publishArguments += "-p:PublishDir=$([IO.Path]::GetDirectoryName($webRoot))/" }
                if ($Obfuscate) { $publishArguments += '-p:DorotiObfuscate=true' }
                if ($DisableAot) { $publishArguments += '-p:RunAOTCompilation=false' }
                & dotnet @publishArguments
                if ($LASTEXITCODE -ne 0) { throw 'Doroti web Release publish failed.' }
            }
            finally { Pop-Location }
        }

        $runtimeRoots = @(Get-PublishedRuntimeRoots)

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
                        foreach ($runtimeRoot in $runtimeRoots) {
                          foreach ($runtimeAsset in $runtimeRoot.Assets) {
                            $assetResponse = Invoke-WebRequest -Method Head -Uri "$url/$($runtimeRoot.Relative)/$($runtimeAsset.Name)" -TimeoutSec 10
                            if ($assetResponse.Headers['Content-Type'] -notcontains $runtimeAsset.Mime -or
                                $assetResponse.Headers['Cross-Origin-Embedder-Policy'] -notcontains 'require-corp') {
                                throw "Incorrect runtime MIME/isolation headers: $($runtimeAsset.Name)"
                            }
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
