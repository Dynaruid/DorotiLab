#Requires -Version 5.1

[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$timeout = [TimeSpan]::FromMinutes(20)
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$artifactRoot = Join-Path $repositoryRoot 'Doroti/validation/web-playwright/artifacts/wrapper/direct-validation'
$dotnet = (Get-Command dotnet -ErrorAction Stop).Source
$npm = (Get-Command npm.cmd -ErrorAction Stop).Source
[IO.Directory]::CreateDirectory($artifactRoot) | Out-Null

function Invoke-ValidationProcess {
    param(
        [Parameter(Mandatory)] [string] $FilePath,
        [Parameter(Mandatory)] [string[]] $ArgumentList,
        [Parameter(Mandatory)] [string] $WorkingDirectory,
        [Parameter(Mandatory)] [string] $Name
    )

    $stdout = Join-Path $artifactRoot "$Name.stdout.log"
    $stderr = Join-Path $artifactRoot "$Name.stderr.log"
    Write-Host "[direct-validation] $Name"
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

$builds = @(
    @{ Name = 'build-skia'; Project = 'Doroti/src/Doroti.Skia.Rendering/Doroti.Skia.Rendering.csproj'; Extra = @() },
    @{ Name = 'build-web'; Project = 'Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj'; Extra = @() },
    @{ Name = 'build-qt'; Project = 'Doroti/src/Doroti.Host.Qt/Doroti.Host.Qt.csproj'; Extra = @() },
    @{ Name = 'build-windowsappsdk'; Project = 'Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj'; Extra = @() },
    @{ Name = 'build-maui'; Project = 'Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj'; Extra = @('-p:DorotiHostTargetFrameworks=net10.0-windows10.0.19041.0') },
    @{ Name = 'build-demo-web'; Project = 'DorotiDemoApp/web/DorotiDemoApp.Web.csproj'; Extra = @() }
)
foreach ($build in $builds) {
    Invoke-ValidationProcess -FilePath $dotnet `
        -ArgumentList (@('build', $build.Project, '--configuration', $Configuration, '--nologo') + $build.Extra) `
        -WorkingDirectory $repositoryRoot -Name $build.Name
}

Invoke-ValidationProcess -FilePath $npm -ArgumentList @('run', 'check') `
    -WorkingDirectory (Join-Path $repositoryRoot 'Doroti/validation/web-playwright') -Name 'typescript-check'

$contracts = @(
    @{ Name = 'fcr3-scheduler'; Project = 'Doroti/validation/fcr3-scheduler/Doroti.Validation.Fcr3Scheduler.csproj' },
    @{ Name = 'fcr4-retained-rendering'; Project = 'Doroti/validation/fcr4-retained-rendering/Doroti.Validation.Fcr4RetainedRendering.csproj' },
    @{ Name = 'fcr5-scroll'; Project = 'Doroti/validation/fcr5-scroll/Doroti.Validation.Fcr5Scroll.csproj' },
    @{ Name = 'fcr6-semantics'; Project = 'Doroti/validation/fcr6-semantics/Doroti.Validation.Fcr6Semantics.csproj' },
    @{ Name = 'fcr7-material-widget'; Project = 'Doroti/validation/fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj' },
    @{ Name = 'resize-contract'; Project = 'Doroti/validation/resize-contract/Doroti.Validation.ResizeContract.csproj' }
)
foreach ($contract in $contracts) {
    Invoke-ValidationProcess -FilePath $dotnet `
        -ArgumentList @('run', '--project', $contract.Project, '--configuration', $Configuration) `
        -WorkingDirectory $repositoryRoot -Name $contract.Name
}

Write-Host "Doroti direct validation PASS. Logs: $artifactRoot"
