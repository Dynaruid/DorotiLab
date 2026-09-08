param([string] $EvidenceDirectory = '.doroti/evidence/boot/webcil-dependency-contract')
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath($EvidenceDirectory)
if (Test-Path -LiteralPath $root) { throw 'Use a fresh evidence directory.' }
$targets = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../src/Doroti.Runner.Sdk/Sdk/Doroti.Web.targets'))
New-Item -ItemType Directory -Path "$root/obj/webcil/publish", "$root/pack-a", "$root/pack-b", "$root/skia" | Out-Null
$project = Join-Path $root 'WebCil.proj'
$xmlTargets = [Security.SecurityElement]::Escape($targets)
@"
<Project>
  <PropertyGroup>
    <DorotiTarget Condition="'`$(DorotiTarget)' == ''">Web</DorotiTarget>
    <IntermediateOutputPath>obj/</IntermediateOutputPath>
    <Pack Condition="'`$(Pack)' == ''">pack-a</Pack>
  </PropertyGroup>
  <ItemGroup>
    <ReferenceCopyLocalPaths Include="`$(Pack)/System.Private.CoreLib.dll"
        RuntimeIdentifier="browser-wasm" NuGetPackageId="Microsoft.NETCore.App.Runtime.Mono.multithread.browser-wasm" />
    <ReferenceCopyLocalPaths Include="skia/SkiaSharp.dll" NuGetPackageId="SkiaSharp" />
    <ReferenceCopyLocalPaths Include="Application.dll" />
  </ItemGroup>
  <Target Name="ResolveReferences" />
  <Target Name="_ResolveWasmConfiguration" />
  <Target Name="_ResolveWasmOutputs" />
  <Import Project="$xmlTargets" />
</Project>
"@ | Set-Content -LiteralPath $project
$source = Join-Path $root 'pack-a/System.Private.CoreLib.dll'
[IO.File]::WriteAllText($source, 'AAAA')
$packageTime = [datetime]'2026-01-01T00:00:00Z'
(Get-Item -LiteralPath $source).LastWriteTimeUtc = $packageTime
$skia = Join-Path $root 'skia/SkiaSharp.dll'
[IO.File]::WriteAllText($skia, '1520')
(Get-Item -LiteralPath $skia).LastWriteTimeUtc = $packageTime
$outputs = @("$root/obj/webcil/System.Private.CoreLib.wasm", "$root/obj/webcil/publish/System.Private.CoreLib.wasm",
             "$root/obj/webcil/SkiaSharp.wasm", "$root/obj/webcil/publish/SkiaSharp.wasm")
$appOutput = "$root/obj/webcil/Application.wasm"
[IO.File]::WriteAllText($appOutput, 'application-cache')
function Seed-Cache { foreach ($file in $outputs) { [IO.File]::WriteAllText($file, 'previous-runtime-cache') } }
function Assert-Cache([bool] $exists) {
    foreach ($file in $outputs) {
        if ((Test-Path -LiteralPath $file) -ne $exists) { throw "Unexpected runtime cache state: $file" }
    }
    if ([IO.File]::ReadAllText($appOutput) -ne 'application-cache') { throw 'Application cache was changed.' }
}
function Invoke-Check([string] $label, [string[]] $properties = @()) {
    $start = [Diagnostics.ProcessStartInfo]::new('dotnet')
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $start.WorkingDirectory = $root
    foreach ($argument in (@('msbuild', $project, '-t:_ResolveWasmOutputs', '-nologo') + $properties)) {
        $start.ArgumentList.Add($argument)
    }
    $process = [Diagnostics.Process]::Start($start)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    if (!$process.WaitForExit(1200000)) { $process.Kill($true); throw '20-minute test timeout.' }
    $log = $stdout.GetAwaiter().GetResult() + $stderr.GetAwaiter().GetResult()
    [IO.File]::WriteAllText((Join-Path $root "$label.log"), $log)
    if ($process.ExitCode -ne 0) { throw $log }
    $process.Dispose()
}
Seed-Cache
Invoke-Check 'existing-untracked-cache'
Assert-Cache $false
Seed-Cache
$identityTime = (Get-Item "$root/obj/doroti-webcil-dependencies.identity").LastWriteTimeUtc
Invoke-Check 'unchanged-runtime'
Assert-Cache $true
if ((Get-Item "$root/obj/doroti-webcil-dependencies.identity").LastWriteTimeUtc -ne $identityTime) { throw 'Unchanged identity was rewritten.' }
[IO.File]::WriteAllText($source, 'BBBB')
(Get-Item -LiteralPath $source).LastWriteTimeUtc = $packageTime
Invoke-Check 'same-size-and-timestamp-runtime-change'
Assert-Cache $false
Seed-Cache
Copy-Item -LiteralPath $source -Destination "$root/pack-b/System.Private.CoreLib.dll"
Invoke-Check 'runtime-pack-path-change' @('-p:Pack=pack-b')
Assert-Cache $false
Seed-Cache
[IO.File]::WriteAllText($skia, '1540')
(Get-Item -LiteralPath $skia).LastWriteTimeUtc = $packageTime
Invoke-Check 'native-package-change' @('-p:Pack=pack-b')
Assert-Cache $false
Seed-Cache
Invoke-Check 'non-web' @('-p:DorotiTarget=Windows')
Assert-Cache $true
Invoke-Check 'webcil-disabled' @('-p:WasmEnableWebcil=false')
Assert-Cache $true
[ordered]@{ status='PASS'; scenarios=7; buildAndPublishCacheChecked=$true; unrelatedApplicationCachePreserved=$true } |
    ConvertTo-Json | Tee-Object -FilePath "$root/result.json"
