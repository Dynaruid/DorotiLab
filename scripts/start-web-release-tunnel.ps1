#requires -Version 7.0
<#
.SYNOPSIS
Publish DorotiTestbedApp in Release and start the existing anonymous HTTPS tunnel.
.EXAMPLE
pwsh -NoProfile -File ./scripts/start-web-release-tunnel.ps1
.EXAMPLE
pwsh -NoProfile -File ./scripts/start-web-release-tunnel.ps1 -OpenBrowser
.EXAMPLE
pwsh -NoProfile -File ./scripts/start-web-release-tunnel.ps1 stop
.NOTES
Requires Docker Desktop (Linux containers), Python, and the Doroti .NET/WASM SDK.
Containers remain running after this command returns. Use stop to end sharing.
The existing tools/doroti-cloudflared configuration and images are reused.
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('start', 'stop', 'url', 'status', 'logs')]
    [string] $Command = 'start',

    [switch] $SkipPublish,

    [ValidateRange(15, 600)]
    [int] $TimeoutSeconds = 120,

    [switch] $OpenBrowser
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$tunnelScript = Join-Path $repositoryRoot 'tools/doroti-cloudflared/tunnel.ps1'
$timeoutRunner = Join-Path $repositoryRoot 'Doroti/validation/run-with-timeout.py'
$urlFile = Join-Path $repositoryRoot 'tools/doroti-cloudflared/generated-url.txt'

if ($SkipPublish -and $Command -ne 'start') {
    throw '-SkipPublish is supported only by start.'
}
if ($OpenBrowser -and $Command -notin @('start', 'url')) {
    throw '-OpenBrowser is supported only by start or url.'
}
if (-not (Test-Path -LiteralPath $tunnelScript -PathType Leaf)) {
    throw "Tunnel tool not found: $tunnelScript"
}

$shell = (Get-Command pwsh -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
$tunnelArguments = @('-NoProfile', '-File', $tunnelScript, $Command,
    '-TimeoutSeconds', [string]$TimeoutSeconds)
if ($SkipPublish) { $tunnelArguments += '-SkipPublish' }

Push-Location $repositoryRoot
try {
    if ($Command -eq 'start') {
        $python = (Get-Command python -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
        # Release publish and startup share the repository's 20-minute process-tree cap.
        & $python $timeoutRunner $shell @tunnelArguments
    }
    else {
        # In particular, following logs should not expire after twenty minutes.
        & $shell @tunnelArguments
    }
    if ($LASTEXITCODE -ne 0) {
        throw "Doroti tunnel '$Command' failed (exit $LASTEXITCODE)."
    }

    if ($Command -in @('start', 'url')) {
        $baseUrl = [IO.File]::ReadAllText($urlFile).Trim()
        if ($baseUrl -notmatch '^https://[a-z0-9-]+\.trycloudflare\.com/?$') {
            throw "The tunnel tool did not save a valid Quick Tunnel URL: $urlFile"
        }
        $sampleUrl = $baseUrl.TrimEnd('/') + '/?dorotiTestbedMode=sample'
        Write-Host "`nDoroti Release sample: $sampleUrl" -ForegroundColor Green
        Write-Host 'Stop: pwsh -NoProfile -File ./scripts/start-web-release-tunnel.ps1 stop'
        if ($OpenBrowser) { Start-Process -FilePath $sampleUrl }
    }
}
finally {
    Pop-Location
}
