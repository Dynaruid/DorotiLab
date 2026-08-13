#Requires -Version 5.1
param(
    [string] $CacheDirectory
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repositoryRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
if ([string]::IsNullOrWhiteSpace($CacheDirectory)) {
    $CacheDirectory = Join-Path $repositoryRoot '.doroti/cache/dart-analyzer'
}
elseif (-not [IO.Path]::IsPathRooted($CacheDirectory)) {
    $CacheDirectory = Join-Path $repositoryRoot $CacheDirectory
}
$CacheDirectory = [IO.Path]::GetFullPath($CacheDirectory)
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g4-4-compiler'
$started = [DateTime]::UtcNow
$artifactPath = Join-Path $dorotiRoot 'artifacts/validation/g4-4-compiler-gate.json'

function Invoke-Checked {
    param([scriptblock] $Command, [string] $Operation)
    & $Command
    if ($LASTEXITCODE -ne 0) {
        throw "$Operation failed with exit code $LASTEXITCODE."
    }
}

function Write-AtomicJson {
    param([string] $Path, [object] $Value)
    $temporary = $Path + '.tmp-' + [Guid]::NewGuid().ToString('N')
    [IO.File]::WriteAllText($temporary, (($Value | ConvertTo-Json -Depth 12) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
    if (Test-Path -LiteralPath $Path) {
        $backup = $Path + '.bak-' + [Guid]::NewGuid().ToString('N')
        [IO.File]::Replace($temporary, $Path, $backup, $true)
        Remove-Item -LiteralPath $backup
    }
    else { [IO.File]::Move($temporary, $Path) }
}

try {
    New-Item -ItemType Directory -Path $temporaryRoot | Out-Null
    Push-Location $repositoryRoot
    try {
        Invoke-Checked {
            dotnet run --project tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj `
                --configuration Release -- `
                --manifest Doroti/migration/selections/g4-4-physics-animation-gestures.json `
                --output $temporaryRoot `
                --cache-dir $CacheDirectory `
                --parallelism 16
        } 'G4-4 candidate generation'
    }
    finally {
        Pop-Location
    }

    $aggregateOutput = @()
    $aggregateExitCode = 0
    Push-Location $temporaryRoot
    try {
        $aggregateOutput = @(& {
            dotnet build Doroti.Generated.Framework.slnx `
                --configuration Release `
                --nologo `
                --verbosity quiet `
                "-p:DorotiRepositoryRoot=$dorotiRoot"
        } 2>&1)
        $aggregateExitCode = $LASTEXITCODE
        $aggregateOutput | ForEach-Object { Write-Output $_ }
    }
    finally {
        Pop-Location
    }

    $report = Get-Content (Join-Path $temporaryRoot 'converter-report.json') -Raw | ConvertFrom-Json
    $coverage = Get-Content (Join-Path $temporaryRoot 'framework-coverage.json') -Raw | ConvertFrom-Json
    $aggregateErrors = @($aggregateOutput | ForEach-Object { [string]$_ } |
        Where-Object { $_ -match '^.+\(\d+,\d+\): error CS\d+:' } |
        Sort-Object -Unique)
    $aggregateWarnings = @($aggregateOutput | ForEach-Object { [string]$_ } |
        Where-Object { $_ -match '^.+\(\d+,\d+\): warning CS\d+:' } |
        Sort-Object -Unique)
    if (-not $report.success) { throw 'G4-4 converter report is not successful.' }
    if ($coverage.unclassifiedAstNodeCount -ne 0) { throw 'G4-4 has unclassified analyzer nodes.' }
    if ($coverage.silentOmissionCount -ne 0) { throw 'G4-4 has silent declaration omissions.' }
    if ($coverage.generatedCompileErrorCount -ne 0) { throw 'G4-4 has generated compile errors.' }

    $artifact = [ordered]@{
        schemaVersion = 'doroti.g5-0-truth-reset/v2'
        milestone = 'G4-4'
        previousBaseline = [ordered]@{ aggregateErrorCount = 27; claimedStatus = 'stale-pass' }
        currentRun = [ordered]@{
            capturedAtUtc = [DateTimeOffset]::UtcNow
            timeoutMilliseconds = 15 * 60 * 1000
            candidate = [ordered]@{
                status = if ($report.success) { 'verified' } else { 'failed' }
                generatedOutputCount = @($report.outputs).Count
                declarationCount = $coverage.declarationCount
                memberCount = $coverage.memberCount
                astNodeCount = $coverage.astNodeCount
                unclassifiedAstNodeCount = $coverage.unclassifiedAstNodeCount
                silentOmissionCount = $coverage.silentOmissionCount
                generatedCompileErrorCount = $coverage.generatedCompileErrorCount
            }
            aggregate = [ordered]@{
                status = if ($aggregateExitCode -eq 0) { 'verified' } else { 'failed' }
                warningCount = $aggregateWarnings.Count
                errorCount = $aggregateErrors.Count
                errorCodes = @($aggregateErrors | ForEach-Object { if ($_ -match 'error (CS\d+):') { $Matches[1] } } | Group-Object | Sort-Object Count -Descending | ForEach-Object { [ordered]@{ code = $_.Name; count = $_.Count } })
            }
            promotion = [ordered]@{ status = 'not-verified'; reason = 'G4-4 product promotion is outside this compiler candidate gate.' }
            productBehavior = [ordered]@{ status = 'not-verified'; reason = 'Run eng/validate-g4-4.ps1 separately.' }
            nativeEvidence = [ordered]@{ status = 'not-verified'; reason = 'No physical device run is part of G5-0.' }
            elapsedMilliseconds = [int64]([DateTime]::UtcNow - $started).TotalMilliseconds
        }
    }
    New-Item -ItemType Directory -Force -Path (Split-Path $artifactPath) | Out-Null
    Write-AtomicJson $artifactPath $artifact
    if ($aggregateExitCode -ne 0) {
        throw "G4-4 aggregate build failed with exit code $aggregateExitCode and $($aggregateErrors.Count) unique compiler error(s)."
    }
    Write-Output "G4-4 compiler gate: PASS"
    Write-Output "Artifact: $artifactPath"
}
catch {
    if (-not (Test-Path -LiteralPath $artifactPath) -or (Get-Item -LiteralPath $artifactPath).LastWriteTimeUtc -lt $started) {
        New-Item -ItemType Directory -Force -Path (Split-Path $artifactPath) | Out-Null
        Write-AtomicJson $artifactPath ([ordered]@{
            schemaVersion = 'doroti.g5-0-truth-reset/v2'; milestone = 'G4-4'
            previousBaseline = [ordered]@{ aggregateErrorCount = 27; claimedStatus = 'stale-pass' }
            currentRun = [ordered]@{
                capturedAtUtc = [DateTimeOffset]::UtcNow; timeoutMilliseconds = 15 * 60 * 1000
                candidate = [ordered]@{ status = 'failed'; failure = $_.Exception.Message }
                aggregate = [ordered]@{ status = 'blocked'; warningCount = 0; errorCount = 0; errorCodes = @() }
                promotion = [ordered]@{ status = 'not-verified' }; productBehavior = [ordered]@{ status = 'not-verified' }; nativeEvidence = [ordered]@{ status = 'not-verified' }
            }
        })
    }
    throw
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) {
        Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
    }
}
