param([string]$Label, [string]$Directory = '.', [Parameter(ValueFromRemainingArguments=$true)][string[]]$Command)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$logs = Join-Path $root 'Doroti/artifacts/framework-web-work2'
[IO.Directory]::CreateDirectory($logs) | Out-Null
$process = Start-Process -FilePath $Command[0] -ArgumentList $Command[1..($Command.Length-1)] -WorkingDirectory (Join-Path $root $Directory) -WindowStyle Hidden -PassThru -RedirectStandardOutput "$logs/$Label.log" -RedirectStandardError "$logs/$Label.err.log"
try {
    if (!$process.WaitForExit(1200000)) { $process.Kill($true); throw '20-minute timeout' }
    Get-Content "$logs/$Label.log" -Tail 15
    Get-Content "$logs/$Label.err.log" -Tail 15
    exit $process.ExitCode
} finally { $process.Dispose() }
