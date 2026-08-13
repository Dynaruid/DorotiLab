#Requires -Version 5.1
param([string] $ArtifactDirectory)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if ([string]::IsNullOrWhiteSpace($ArtifactDirectory)) {
    $ArtifactDirectory = Join-Path $dorotiRoot 'artifacts/g5-2'
}
elseif (-not [IO.Path]::IsPathRooted($ArtifactDirectory)) {
    $ArtifactDirectory = Join-Path $dorotiRoot $ArtifactDirectory
}
$ArtifactDirectory = [IO.Path]::GetFullPath($ArtifactDirectory)
[IO.Directory]::CreateDirectory($ArtifactDirectory) | Out-Null
$ready = Join-Path $ArtifactDirectory 'automation-ready.json'
$actions = Join-Path $ArtifactDirectory 'automation-actions.json'
$stop = Join-Path $ArtifactDirectory 'automation-stop.signal'
$client = Join-Path $ArtifactDirectory 'automation-client.json'
foreach ($path in @($ready, $actions, $stop, $client)) {
    if (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path -Force }
}

$dll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G5PaintingRenderingSemantics/bin/Release/net10.0/Doroti.Validation.G5PaintingRenderingSemantics.dll'
$target = Start-Process dotnet -ArgumentList @($dll, '--automation-target', $ready, $actions, $stop) -PassThru -WindowStyle Hidden
try {
    $deadline = [DateTime]::UtcNow.AddSeconds(15)
    while (-not (Test-Path -LiteralPath $ready) -and [DateTime]::UtcNow -lt $deadline) {
        Start-Sleep -Milliseconds 50
    }
    if (-not (Test-Path -LiteralPath $ready)) { throw 'Automation target did not become ready.' }
    $targetInfo = Get-Content -LiteralPath $ready -Raw | ConvertFrom-Json
    & "$dorotiRoot/eng/invoke-g5-2-uia.ps1" -ProcessId $targetInfo.processId -WindowHandle $targetInfo.windowHandle -OutputPath $client
    if ($null -ne $LASTEXITCODE -and $LASTEXITCODE -ne 0) {
        throw "External UI Automation client failed with exit code $LASTEXITCODE."
    }

    $actionCount = 0
    $deadline = [DateTime]::UtcNow.AddSeconds(10)
    do {
        Start-Sleep -Milliseconds 50
        if (Test-Path -LiteralPath $actions) {
            $actionCount = @((Get-Content -LiteralPath $actions -Raw | ConvertFrom-Json).actions).Count
        }
    } while ($actionCount -lt 5 -and [DateTime]::UtcNow -lt $deadline)
    if ($actionCount -lt 5) { throw "Only $actionCount of five external automation actions reached Flutter." }
    $received = @((Get-Content -LiteralPath $actions -Raw | ConvertFrom-Json).actions)
    $required = @(
        @{ nodeId = 2; action = 'focus'; arguments = $null },
        @{ nodeId = 2; action = 'tap'; arguments = $null },
        @{ nodeId = 3; action = 'tap'; arguments = $null },
        @{ nodeId = 4; action = 'setText'; arguments = 'external-uia-text' },
        @{ nodeId = 5; action = 'scrollDown'; arguments = $null }
    )
    foreach ($expected in $required) {
        $match = @($received | Where-Object {
            $_.nodeId -eq $expected.nodeId -and $_.action -eq $expected.action -and $_.arguments -eq $expected.arguments
        }).Count
        if ($match -lt 1) { throw "External automation action did not round trip: node=$($expected.nodeId), action=$($expected.action)." }
    }

    [IO.File]::WriteAllText($stop, 'stop', [Text.UTF8Encoding]::new($false))
    if (-not $target.WaitForExit(10000)) { throw 'Automation target did not exit after the stop signal.' }
    if ($target.ExitCode -ne 0) { throw "Automation target failed with exit code $($target.ExitCode)." }
    Write-Output "G5-2 external automation round trip: PASS ($actionCount actions)"
}
finally {
    if (-not $target.HasExited) { Stop-Process -Id $target.Id -Force -ErrorAction SilentlyContinue }
    if (Test-Path -LiteralPath $stop) { Remove-Item -LiteralPath $stop -Force }
}
