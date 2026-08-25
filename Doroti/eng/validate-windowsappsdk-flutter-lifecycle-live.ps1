#Requires -Version 7.0
[CmdletBinding()]
param(
    [string]$ProjectPath=(Join-Path $PSScriptRoot '../validation/windowsappsdk-flutter-lifecycle/Doroti.Validation.WindowsAppSdkFlutterLifecycle.csproj'),
    [string]$OutputRoot=(Join-Path $PSScriptRoot '../artifacts/windowsappsdk-flutter-lifecycle')
)
$ErrorActionPreference='Stop'
$timeout=[TimeSpan]::FromMinutes(20)
$static=& (Join-Path $PSScriptRoot 'validate-windowsappsdk-flutter-lifecycle.ps1') | Out-String | ConvertFrom-Json
$run=Join-Path ([IO.Path]::GetFullPath($OutputRoot)) ('f8-live-'+[DateTimeOffset]::UtcNow.ToString('yyyyMMdd-HHmmss')+'-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $run | Out-Null
& dotnet publish ([IO.Path]::GetFullPath($ProjectPath)) -c Release -r win-x64 --self-contained true --nologo -o $run
if($LASTEXITCODE -ne 0){throw 'F8 self-contained publish failed.'}
$exe=Join-Path $run 'Doroti.Validation.WindowsAppSdkFlutterLifecycle.exe'
$evidence=Join-Path $run 'f8-live-evidence.json'
$start=[Diagnostics.ProcessStartInfo]::new();$start.FileName=$exe;$start.WorkingDirectory=$run;$start.UseShellExecute=$false;$start.RedirectStandardOutput=$true;$start.RedirectStandardError=$true;$start.CreateNoWindow=$true;$start.Environment['PATH']='C:\Windows\System32'
foreach($arg in @('--evidence',$evidence,'--source-fingerprint',[string]$static.sourceFingerprint)){[void]$start.ArgumentList.Add($arg)}
$process=[Diagnostics.Process]::new();$process.StartInfo=$start;if(-not $process.Start()){throw 'F8 live fixture did not start.'}
$stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
if(-not $process.WaitForExit([int]$timeout.TotalMilliseconds)){$process.Kill($true);$process.WaitForExit();throw 'F8 live fixture exceeded 20 minutes.'}
$out=$stdout.GetAwaiter().GetResult();$err=$stderr.GetAwaiter().GetResult();if($process.ExitCode -ne 0){throw "F8 live fixture failed.`n$out`n$err"}
$json=Get-Content -LiteralPath $evidence -Raw | ConvertFrom-Json -Depth 64
if([string]$json.status -ne 'PASS' -or [string]$json.sourceFingerprint -ne [string]$static.sourceFingerprint){throw 'F8 live evidence validation failed.'}
[ordered]@{schemaVersion='doroti.windowsappsdk-flutter-lifecycle-live-validation/v1';status='PASS';sourceFingerprint=$static.sourceFingerprint;publishDirectory=$run;evidencePath=$evidence;validation=$json.validation;lifecycle=$json.lifecycle;scopeBoundary=$json.scopeBoundary;notVerified=@('physical mixed-DPI cross-monitor and monitor disconnect','physical sleep/resume and RDP attach/detach','FG visible black/white-stuck recovery')}|ConvertTo-Json -Depth 64
