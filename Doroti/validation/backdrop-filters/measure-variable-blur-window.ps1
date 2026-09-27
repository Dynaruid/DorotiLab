param([string]$Name='native-blur',
 [ValidateSet('full','adaptive','fast','fixed','off')][string]$Mode='adaptive',
 [string]$ArtifactDirectory=(Join-Path $PSScriptRoot '../../artifacts/variable-blur-performance'))
$ErrorActionPreference='Stop'
$artifact=[IO.Path]::GetFullPath($ArtifactDirectory)
New-Item -ItemType Directory -Force -Path $artifact | Out-Null
$root=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$control=Join-Path $PSScriptRoot 'variable-blur-window-control.ps1'
$env:DOROTI_VARIABLE_BLUR_PROFILE='1'
$env:DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1'
$env:DOROTI_WINDOWS_APPSDK_REPORT=Join-Path $artifact ($Name+'-report.json')
$exe=Join-Path $root 'samples/DorotiSampleApp2/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiSampleApp2.WindowsAppSdk.exe'
$sample=Start-Process -FilePath $exe -WorkingDirectory (Split-Path $exe) -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $artifact ($Name+'-out.log')) -RedirectStandardError (Join-Path $artifact ($Name+'-error.log'))
$sample.Id | Set-Content (Join-Path $artifact 'native-pid.txt')
Start-Sleep -Milliseconds 900
try {
& $control -ArtifactDirectory $artifact -Action size -Width 2500 -Height 1450 -Name ($Name+'-start')
$state=Get-Content (Join-Path $artifact 'native-state.json') -Raw | ConvertFrom-Json
if($state.clientWidth -ne 2500 -or $state.clientHeight -ne 1450 -or $state.dpi -ne 192){throw 'This input fixture requires a 2500x1450 client at 200% DPI on the primary display'}
$cursor=New-Object BlurWindowProbe+POINT
[BlurWindowProbe]::GetCursorPos([ref]$cursor) | Out-Null
& $control -ArtifactDirectory $artifact -Action click -X 2180 -Y 1380 -Name ($Name+'-tab')
$modeX=if($Mode -in @('adaptive','fixed')){1420}else{160}
$modeY=if($Mode -in @('fast','fixed')){394}else{320}
& $control -ArtifactDirectory $artifact -Action click -X $modeX -Y $modeY -Name ($Name+'-mode')
if($Mode -eq 'off'){& $control -ArtifactDirectory $artifact -Action click -X 2420 -Y 158 -Name ($Name+'-off')}
python (Join-Path $PSScriptRoot 'capture-variable-blur-motion.py') $Name $artifact
if($LASTEXITCODE -ne 0){throw 'Desktop motion capture failed'}
} finally {
& $control -ArtifactDirectory $artifact -Action close
if($cursor){[BlurWindowProbe]::SetCursorPos($cursor.X,$cursor.Y) | Out-Null}
if(-not $sample.WaitForExit(10000)){throw 'Sample did not exit after WM_CLOSE'}
}
