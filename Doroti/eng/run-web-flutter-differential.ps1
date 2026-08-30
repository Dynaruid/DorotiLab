#Requires -Version 5.1

[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [ValidateRange(1, 10)]
    [int] $Runs = 3
)

$ErrorActionPreference = 'Stop'
$timeout = [TimeSpan]::FromMinutes(20)
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter_sample_app'
$playwrightRoot = Join-Path $dorotiRoot 'validation/web-playwright'
$artifactRoot = Join-Path $playwrightRoot 'artifacts/wrapper/flutter-differential'
$dorotiProject = Join-Path $repositoryRoot 'DorotiDemoApp/web/DorotiDemoApp.Web.csproj'
$dorotiUrl = 'http://127.0.0.1:5088'
$flutterUrl = 'http://127.0.0.1:5089'

foreach ($port in @(5088, 5089)) {
    if (Get-NetTCPConnection -State Listen -LocalPort $port -ErrorAction SilentlyContinue) {
        throw "Port $port is already in use. Stop the existing listener before differential validation."
    }
}

[IO.Directory]::CreateDirectory($artifactRoot) | Out-Null
$dotnet = (Get-Command dotnet -ErrorAction Stop).Source
$npx = (Get-Command npx.cmd -ErrorAction Stop).Source
$python = (Get-Command python.exe -ErrorAction Stop).Source
. (Join-Path $PSScriptRoot 'flutter-sdk.ps1')
$flutter = Resolve-DorotiFlutterSdk -RepositoryRoot $repositoryRoot

function Invoke-DifferentialProcess {
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
        if (-not $process.WaitForExit([int]$timeout.TotalMilliseconds)) {
            try { $process.Kill($true) } catch { }
            throw "$Name exceeded the repository 20-minute timeout."
        }
        if ($process.ExitCode -ne 0) {
            $output = if (Test-Path -LiteralPath $stdout) { Get-Content -LiteralPath $stdout -Raw } else { '' }
            $errors = if (Test-Path -LiteralPath $stderr) { Get-Content -LiteralPath $stderr -Raw } else { '' }
            throw "$Name failed with exit code $($process.ExitCode).`n$output`n$errors"
        }
    }
    finally { $process.Dispose() }
}

Invoke-DifferentialProcess -FilePath $dotnet `
    -ArgumentList @('build', $dorotiProject, '--configuration', $Configuration, '--nologo') `
    -WorkingDirectory $repositoryRoot -Name 'doroti-build'
Invoke-DifferentialProcess -FilePath $flutter.FlutterCommand `
    -ArgumentList @('build', 'web', '--release', '--wasm') `
    -WorkingDirectory $flutterRoot -Name 'flutter-build'

$dorotiServer = Start-Process -FilePath $dotnet `
    -ArgumentList @('run', '--project', $dorotiProject, '--configuration', $Configuration, '--no-build', '--no-restore') `
    -WorkingDirectory $repositoryRoot -WindowStyle Hidden -PassThru `
    -RedirectStandardOutput (Join-Path $artifactRoot 'doroti-server.stdout.log') `
    -RedirectStandardError (Join-Path $artifactRoot 'doroti-server.stderr.log')
$flutterServer = Start-Process -FilePath $python `
    -ArgumentList @((Join-Path $PSScriptRoot 'serve-web-static.py'), '--port', '5089', '--directory', (Join-Path $flutterRoot 'build/web')) `
    -WorkingDirectory $flutterRoot -WindowStyle Hidden -PassThru `
    -RedirectStandardOutput (Join-Path $artifactRoot 'flutter-server.stdout.log') `
    -RedirectStandardError (Join-Path $artifactRoot 'flutter-server.stderr.log')

try {
    foreach ($url in @($dorotiUrl, $flutterUrl)) {
        $deadline = [DateTime]::UtcNow.AddMinutes(2)
        do {
            try {
                if ((Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 2).StatusCode -eq 200) { break }
            } catch { }
            Start-Sleep -Milliseconds 250
        } while ([DateTime]::UtcNow -lt $deadline)
        if ([DateTime]::UtcNow -ge $deadline) { throw "Server did not become ready at $url." }
    }

    $previous = @{
        Base = $env:DOROTI_WEB_BASE_URL
        Flutter = $env:DOROTI_FLUTTER_BASE_URL
        Renderer = $env:DOROTI_WEB_RENDERER_MODE
        Artifact = $env:DOROTI_WEB_ARTIFACT_LABEL
    }
    try {
        $env:DOROTI_WEB_BASE_URL = $dorotiUrl
        $env:DOROTI_FLUTTER_BASE_URL = $flutterUrl
        $env:DOROTI_WEB_RENDERER_MODE = 'worker-direct-webgl'
        $env:DOROTI_WEB_ARTIFACT_LABEL = 'flutter-differential'
        $env:DOROTI_DIFFERENTIAL_RUNS = [string]$Runs
        Invoke-DifferentialProcess -FilePath $npx `
            -ArgumentList @('playwright', 'test', 'tests/flutter-differential.spec.ts', '--project=chromium-hardware') `
            -WorkingDirectory $playwrightRoot -Name 'playwright'
    }
    finally {
        if ($null -eq $previous.Base) { Remove-Item Env:DOROTI_WEB_BASE_URL -ErrorAction SilentlyContinue } else { $env:DOROTI_WEB_BASE_URL = $previous.Base }
        if ($null -eq $previous.Flutter) { Remove-Item Env:DOROTI_FLUTTER_BASE_URL -ErrorAction SilentlyContinue } else { $env:DOROTI_FLUTTER_BASE_URL = $previous.Flutter }
        if ($null -eq $previous.Renderer) { Remove-Item Env:DOROTI_WEB_RENDERER_MODE -ErrorAction SilentlyContinue } else { $env:DOROTI_WEB_RENDERER_MODE = $previous.Renderer }
        if ($null -eq $previous.Artifact) { Remove-Item Env:DOROTI_WEB_ARTIFACT_LABEL -ErrorAction SilentlyContinue } else { $env:DOROTI_WEB_ARTIFACT_LABEL = $previous.Artifact }
        Remove-Item Env:DOROTI_DIFFERENTIAL_RUNS -ErrorAction SilentlyContinue
    }
}
finally {
    foreach ($process in @($dorotiServer, $flutterServer)) {
        if ($null -eq $process) { continue }
        if (-not $process.HasExited) {
            try { $process.Kill($true) } catch { }
            try { $process.WaitForExit(10000) | Out-Null } catch { }
        }
        $process.Dispose()
    }
}
