[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [switch] $SkipBuild,

    [switch] $HeadlessOnly,

    [string] $TestFile
)

$ErrorActionPreference = 'Stop'
$timeout = [TimeSpan]::FromMinutes(20)
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$project = Join-Path $repositoryRoot 'DorotiDemoApp/web/DorotiDemoApp.Web.csproj'
$playwrightRoot = Join-Path $dorotiRoot 'validation/web-playwright'
$artifactRoot = Join-Path $playwrightRoot 'artifacts/wrapper'
$baseUrl = 'http://127.0.0.1:5088'

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

$listener = Get-NetTCPConnection -State Listen -LocalPort 5088 -ErrorAction SilentlyContinue
if ($listener) {
    throw 'Port 5088 is already in use. Stop the existing listener before running Doroti Playwright validation.'
}

if (-not $SkipBuild) {
    Invoke-OwnedProcess -FilePath $dotnet `
        -ArgumentList @('build', $project, '--configuration', $Configuration, '--nologo') `
        -WorkingDirectory $repositoryRoot -Name 'build'
}

$serverStdout = Join-Path $artifactRoot 'server.stdout.log'
$serverStderr = Join-Path $artifactRoot 'server.stderr.log'
$server = Start-Process -FilePath $dotnet `
    -ArgumentList @('run', '--project', $project, '--configuration', $Configuration, '--no-build', '--no-restore') `
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
    try {
        $env:DOROTI_WEB_BASE_URL = $baseUrl
        $arguments = @('playwright', 'test')
        if (-not [string]::IsNullOrWhiteSpace($TestFile)) { $arguments += $TestFile }
        if ($HeadlessOnly) {
            $arguments += '--project=chromium-hardware'
            $arguments += '--project=chromium-dpr2'
        }
        Invoke-OwnedProcess -FilePath $npx -ArgumentList $arguments `
            -WorkingDirectory $playwrightRoot -Name 'playwright'
    }
    finally {
        if ($null -eq $previousBaseUrl) { Remove-Item Env:DOROTI_WEB_BASE_URL -ErrorAction SilentlyContinue }
        else { $env:DOROTI_WEB_BASE_URL = $previousBaseUrl }
    }
}
finally {
    if (-not $server.HasExited) {
        try { $server.Kill($true) } catch { }
        try { $server.WaitForExit(10000) | Out-Null } catch { }
    }
    $server.Dispose()
}
