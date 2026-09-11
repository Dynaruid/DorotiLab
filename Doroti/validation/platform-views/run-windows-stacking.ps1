$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$outDir = Join-Path $repository 'Doroti/artifacts/validation/platform-views/windows-stacking'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$installation = & $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
if (-not $installation) { throw 'MSVC x64 tools are unavailable.' }
$vcvars = Join-Path $installation 'VC/Auxiliary/Build/vcvars64.bat'
$source = Join-Path $PSScriptRoot 'windows-stacking.cpp'
$exe = Join-Path $outDir 'windows-stacking.exe'
$obj = Join-Path $outDir 'windows-stacking.obj'
$buildScript = Join-Path $outDir 'build.cmd'
@"
@echo off
call "$vcvars" >nul
if errorlevel 1 exit /b %errorlevel%
cl /nologo /std:c++17 /EHsc /W4 "$source" /Fo"$obj" /Fe"$exe" /link user32.lib gdi32.lib d3d11.lib dxgi.lib dcomp.lib dwmapi.lib
"@ | Set-Content -LiteralPath $buildScript -Encoding ascii
& cmd.exe /d /c $buildScript
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& $exe
exit $LASTEXITCODE
