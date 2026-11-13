[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$output = Join-Path $repository 'Doroti\artifacts\web-contract-tests'
$compiler = Join-Path $HOME '.nuget\packages\microsoft.typescript.msbuild\7.0.1\tools\runtimes\win32-x64\tsc.exe'
if (-not (Test-Path -LiteralPath $compiler -PathType Leaf)) { throw 'Restore/build Doroti.Host.Web first to obtain the existing TypeScript compiler.' }
$timer = [System.Diagnostics.Stopwatch]::StartNew()
$compile = Start-Process -FilePath $compiler -ArgumentList @('--project', (Join-Path $repository 'packages\platforms\web\Doroti.Host.Web\Web\tsconfig.json'), '--outDir', $output) -NoNewWindow -PassThru
if (-not $compile.WaitForExit(1200000)) { Stop-Process -Id $compile.Id -Force; throw 'Contract tests exceeded 20 minutes.' }
if ($compile.ExitCode -ne 0) { exit $compile.ExitCode }
$env:DOROTI_WEB_MODULE_ROOT = $output
$tests = Start-Process -FilePath (Get-Command node).Source -ArgumentList @('--test', (Join-Path $PSScriptRoot 'execution.test.mjs')) -NoNewWindow -PassThru
if (-not $tests.WaitForExit([Math]::Max(1, 1200000 - [int]$timer.ElapsedMilliseconds))) { Stop-Process -Id $tests.Id -Force; throw 'Contract tests exceeded 20 minutes.' }
exit $tests.ExitCode
