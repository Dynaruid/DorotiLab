[CmdletBinding()]
param(
    [Parameter(Mandatory)][string] $Executable,
    [Parameter(Mandatory)][string[]] $Arguments,
    [Parameter(Mandatory)][string] $LogPath,
    [string] $WorkingDirectory = (Get-Location).Path
)
$ErrorActionPreference = 'Stop'
$start = [Diagnostics.ProcessStartInfo]::new($Executable)
$start.UseShellExecute = $false
$start.WorkingDirectory = $WorkingDirectory
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
$start.Environment['DOTNET_CLI_USE_MSBUILD_SERVER'] = '0'
$start.Environment['MSBUILDDISABLENODEREUSE'] = '1'
foreach ($argument in $Arguments) { $start.ArgumentList.Add($argument) }
$process = [Diagnostics.Process]::Start($start)
$stdout = $process.StandardOutput.ReadToEndAsync()
$stderr = $process.StandardError.ReadToEndAsync()
$completed = $process.WaitForExit(1200000)
if (-not $completed) {
    $pending = [Collections.Generic.Stack[int]]::new()
    $ids = [Collections.Generic.List[int]]::new()
    $pending.Push($process.Id)
    while ($pending.Count) {
        $parent = $pending.Pop()
        $ids.Add($parent)
        Get-CimInstance Win32_Process -Filter "ParentProcessId=$parent" | ForEach-Object { $pending.Push([int]$_.ProcessId) }
    }
    for ($index = $ids.Count - 1; $index -ge 0; $index--) { Stop-Process -Id $ids[$index] -Force -ErrorAction SilentlyContinue }
    $process.WaitForExit()
}
$fullLogPath = [IO.Path]::GetFullPath($LogPath)
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($fullLogPath)) | Out-Null
[IO.File]::WriteAllText($fullLogPath, $stdout.GetAwaiter().GetResult() + $stderr.GetAwaiter().GetResult())
Get-Content -LiteralPath $fullLogPath -Tail 18
if (-not $completed) { throw "Validation exceeded 1200 seconds. Log: $fullLogPath" }
exit $process.ExitCode
