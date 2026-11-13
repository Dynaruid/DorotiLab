[CmdletBinding()]
param([Parameter(Mandatory)][string] $WebRoot)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $WebRoot).Path
$p = Start-Process -FilePath (Get-Command node).Source -ArgumentList @((Join-Path $PSScriptRoot 'browser-smoke.mjs'), $root) -NoNewWindow -PassThru
if (-not $p.WaitForExit(1200000)) {
    $pending = [Collections.Generic.Stack[int]]::new()
    $ids = [Collections.Generic.List[int]]::new()
    $pending.Push($p.Id)
    while ($pending.Count) {
        $parent = $pending.Pop()
        $ids.Add($parent)
        Get-CimInstance Win32_Process -Filter "ParentProcessId=$parent" | ForEach-Object { $pending.Push([int]$_.ProcessId) }
    }
    for ($index = $ids.Count - 1; $index -ge 0; $index--) { Stop-Process -Id $ids[$index] -Force -ErrorAction SilentlyContinue }
    throw 'Browser validation exceeded 1200 seconds.'
}
exit $p.ExitCode
