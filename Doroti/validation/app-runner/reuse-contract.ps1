param([Parameter(Mandatory)][string] $App, [string] $EvidenceDirectory = '.doroti/evidence/boot/cli-reuse', [string] $Url = 'http://127.0.0.1:5088', [switch] $MissingOnly, [switch] $PositiveOnly)
$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$appRoot = [IO.Path]::GetFullPath($App, $repo)
$evidence = [IO.Path]::GetFullPath($EvidenceDirectory, $repo)
New-Item -ItemType Directory -Force $evidence | Out-Null
$cli = Join-Path $repo 'Doroti/eng/doroti.ps1'
$statePath = Join-Path $appRoot '.doroti/launch-state/web-WindowsAppSdk-Release-default-rid.json'
$originalState = [IO.File]::ReadAllText($statePath)
$state = $originalState | ConvertFrom-Json -Depth 100
$results = [Collections.Generic.List[object]]::new()
function Invoke-Reuse([string] $Name, [bool] $ExpectedSuccess) {
    $start = [Diagnostics.ProcessStartInfo]::new((Get-Command pwsh).Source)
    $start.WorkingDirectory = $repo
    $start.UseShellExecute = $false; $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true; $start.RedirectStandardError = $true
    foreach ($arg in @('-NoProfile','-File',$cli,'run','-App',$appRoot,'-Platform','web','-LastSuccessful')) { $start.ArgumentList.Add($arg) }
    $process = [Diagnostics.Process]::Start($start)
    $stdout = $process.StandardOutput.ReadToEndAsync(); $stderr = $process.StandardError.ReadToEndAsync()
    $watch = [Diagnostics.Stopwatch]::StartNew(); $ready = $false
    try {
        while (!$process.HasExited -and $watch.Elapsed.TotalMilliseconds -lt 1200000) {
            if ($ExpectedSuccess) {
                try { $response = Invoke-WebRequest $Url -TimeoutSec 1; $ready = $response.StatusCode -eq 200 } catch { }
                if ($ready) { break }
            }
            Start-Sleep -Milliseconds 250
        }
        if (!$process.HasExited) { $process.Kill($true); $process.WaitForExit() }
        $stdout.Result | Set-Content (Join-Path $evidence "$Name.stdout.log")
        $stderr.Result | Set-Content (Join-Path $evidence "$Name.stderr.log")
        $expectedError = switch ($Name) {
            'missing-output' { 'The evaluated runner artifact is missing' }
            'tampered-output' { 'output/RID/framework mismatch' }
            'tampered-generated-asset' { 'output/RID/framework mismatch' }
            default { 'The last successful artifact is stale' }
        }
        $passed = if ($ExpectedSuccess) { $ready -and $stdout.Result.Contains('build=reused') } else {
            $process.ExitCode -ne 0 -and $stderr.Result.Contains($expectedError)
        }
        $results.Add([ordered]@{name=$Name;status=$(if ($passed) {'PASS'} else {'FAIL'});ready=$ready;exitCode=$process.ExitCode;elapsedSeconds=$watch.Elapsed.TotalSeconds})
        if (!$passed) { throw "Reuse case failed: $Name" }
    } finally { if (!$process.HasExited) { $process.Kill($true) }; $process.Dispose() }
}
try {
    if (!$MissingOnly) { Invoke-Reuse 'unchanged' $true }
    if ($PositiveOnly) { return }
    $target = $state.artifact.targetPath
    $bytes = [IO.File]::ReadAllBytes($target)
    $allowed = $appRoot.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if (![IO.Path]::GetFullPath($target).StartsWith($allowed, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Output mutation must remain inside the isolated application workspace.'
    }
    $savedTarget = "$target.boot-test-saved"
    try {
        Move-Item -LiteralPath $target -Destination $savedTarget
        Invoke-Reuse 'missing-output' $false
    } finally { Move-Item -LiteralPath $savedTarget -Destination $target }
    if ($MissingOnly) { return }
    try {
        [IO.File]::WriteAllBytes($target, [byte[]](1,2,3))
        Invoke-Reuse 'tampered-output' $false
    } finally { [IO.File]::WriteAllBytes($target, $bytes) }
    # A referenced generated asset outside the runner bin directory must also be bound.
    $asset = @($state.artifact.files | Where-Object { $_.path -match 'Doroti.Host.Web.*Doroti.Web.*doroti.loader.js$' })[0].path
    $assetBytes = [IO.File]::ReadAllBytes($asset)
    try {
        [IO.File]::WriteAllBytes($asset, [byte[]](1,2,3))
        Invoke-Reuse 'tampered-generated-asset' $false
    } finally { [IO.File]::WriteAllBytes($asset, $assetBytes) }
    $source = Join-Path $appRoot 'Program.cs'
    $sourceBytes = [IO.File]::ReadAllBytes($source)
    try {
        [IO.File]::AppendAllText($source, "`n// changed input`n")
        Invoke-Reuse 'changed-source' $false
    } finally { [IO.File]::WriteAllBytes($source, $sourceBytes) }
    $state.toolchain = 'different-sdk'
    [IO.File]::WriteAllText($statePath, ($state | ConvertTo-Json -Depth 100))
    Invoke-Reuse 'changed-toolchain' $false
    $state.schemaVersion = 'doroti.launch-state/v1'
    [IO.File]::WriteAllText($statePath, ($state | ConvertTo-Json -Depth 100))
    Invoke-Reuse 'old-state-schema' $false
} finally {
    [IO.File]::WriteAllText($statePath, $originalState)
    $results | ConvertTo-Json -Depth 10 | Tee-Object -FilePath (Join-Path $evidence 'results.json')
}
