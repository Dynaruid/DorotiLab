param([string] $EvidenceDirectory = '.doroti/evidence/windows-startup-contract')
$ErrorActionPreference = 'Stop'
if (!$IsWindows) { throw 'This regression requires the Windows product host.' }
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$evidence = [IO.Path]::GetFullPath($EvidenceDirectory, $repo)
[IO.Directory]::CreateDirectory($evidence) | Out-Null
$results = [Collections.Generic.List[object]]::new()

# Exercise the real entrypoint without a validation fixture attaching a root or
# requesting framework frames for it. Both modes must show their first scene.
foreach ($mode in @('default', 'sample')) {
    $runDirectory = Join-Path $evidence "$mode-$([Guid]::NewGuid().ToString('N'))"
    [IO.Directory]::CreateDirectory($runDirectory) | Out-Null
    $start = [Diagnostics.ProcessStartInfo]::new((Get-Command pwsh).Source)
    $start.WorkingDirectory = $repo
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    foreach ($key in @($start.Environment.Keys | Where-Object { $_ -like 'DOROTI_*' })) {
        $start.Environment.Remove($key) | Out-Null
    }
    if ($mode -eq 'sample') { $start.Environment['DOROTI_TESTBED_MODE'] = 'sample' }
    $start.Environment['DOROTI_WINDOWS_APPSDK_SMOKE_MS'] = '5000'
    foreach ($argument in @('-NoProfile', '-File', './Doroti/eng/doroti.ps1', 'run',
            '-App', './DorotiTestbedApp', '-Platform', 'windows', '-Configuration', 'Release')) {
        $start.ArgumentList.Add($argument)
    }
    $process = [Diagnostics.Process]::Start($start)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    try {
        # Repository test timeout: 20 minutes, including build and shutdown.
        $exited = $process.WaitForExit(1200000)
        if (!$exited) { $process.Kill($true); $process.WaitForExit() }
        $stdout.Result | Set-Content (Join-Path $runDirectory 'stdout.log')
        $stderr.Result | Set-Content (Join-Path $runDirectory 'stderr.log')
        if (!$exited) { throw "Windows $mode startup timed out after 20 minutes." }
        if ($process.ExitCode -ne 0) { throw "Windows $mode startup failed. See $runDirectory" }
        $summaryLine = @($stderr.Result -split '\r?\n' |
            Where-Object { $_.StartsWith('doroti.windows.summary=') })
        if ($summaryLine.Count -ne 1) { throw "Windows $mode startup did not report presentation evidence." }
        $summary = $summaryLine[0].Substring('doroti.windows.summary='.Length) | ConvertFrom-Json
        if (!$summary.VisibleAfterExactPresent -or $summary.PresentedTerminals -lt 1 -or
            $summary.RendererPresented -lt 1) {
            throw "Windows $mode startup did not display its first exact frame."
        }
        $results.Add([ordered]@{ mode=$mode; status='PASS'; diagnostics=$summary; logs=$runDirectory })
        Write-Host "Windows $mode startup: PASS (visible after exact present)"
    }
    finally {
        if (!$process.HasExited) { $process.Kill($true); $process.WaitForExit() }
        $process.Dispose()
    }
}
$results | ConvertTo-Json -Depth 30 | Set-Content (Join-Path $evidence 'results.json')
