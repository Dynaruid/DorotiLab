[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$dotnet = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
$project = Join-Path $repository 'samples\DorotiTestbedApp\web\DorotiTestbedApp.Web.csproj'
$cases = @(
    @{ Name = 'invalid-mode'; Sdk = '10.0.403'; Code = 'DOROTIWEB041'; Args = @('-p:DorotiWebExecutionMode=invalid') },
    @{ Name = 'thread-conflict'; Sdk = '10.0.403'; Code = 'DOROTIWEB042'; Args = @('-p:DorotiWebExecutionMode=compatible', '-p:WasmEnableThreads=true') },
    @{ Name = 'unsupported-coreclr'; Sdk = '11.0.100'; Code = 'DOROTIWEB043'; Args = @('-p:DorotiWebTargetFramework=net11.0', '-p:UseMonoRuntime=false') },
    @{ Name = 'unqualified-mono11'; Sdk = '11.0.100'; Code = 'DOROTIWEB050'; Args = @('-p:DorotiWebTargetFramework=net11.0') }
)
foreach ($case in $cases) {
    $sdk = Join-Path $env:ProgramFiles "dotnet\sdk\$($case.Sdk)\dotnet.dll"
    $log = Join-Path $repository "Doroti\artifacts\web-qualification\build-contract-$($case.Name).log"
    $arguments = @($sdk, 'msbuild', $project, '-t:ValidateDorotiRunnerProvider', '--nologo', '-v:quiet') + $case.Args
    & (Join-Path $PSScriptRoot 'run-validation.ps1') -Executable $dotnet -Arguments $arguments -LogPath $log -WorkingDirectory $repository
    if ($LASTEXITCODE -eq 0 -or (Get-Content -LiteralPath $log -Raw) -notmatch $case.Code) {
        throw "Expected $($case.Code) for $($case.Name)."
    }
    Write-Host "PASS $($case.Name): $($case.Code)"
}
$artifactRoot = Join-Path $repository 'Doroti\artifacts\web-qualification'
$fixture = Join-Path $artifactRoot ('cache-contracts-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory((Join-Path $fixture 'obj\linked')) | Out-Null
[IO.File]::WriteAllText((Join-Path $fixture 'input.dll'), 'unchanged assembly content')
[IO.File]::WriteAllText((Join-Path $fixture 'obj\linked\Link.semaphore'), 'previous linked output')
foreach ($kind in @('for-build', 'for-publish')) {
    $native = Join-Path $fixture "obj\wasm\$kind"
    [IO.Directory]::CreateDirectory($native) | Out-Null
    foreach ($name in @('runtime-icall-table.h', 'm2n_cache.txt')) { [IO.File]::WriteAllText((Join-Path $native $name), 'previous native bindings') }
}
$project = Join-Path $fixture 'cache.proj'
$xml = @'
<Project>
  <PropertyGroup>
    <DorotiTarget>Web</DorotiTarget><PublishTrimmed>true</PublishTrimmed><TargetFramework>net10.0</TargetFramework>
    <IntermediateOutputPath>__ROOT__\obj\</IntermediateOutputPath>
    <_LinkSemaphore>__ROOT__\obj\linked\Link.semaphore</_LinkSemaphore>
    <TargetPath>__ROOT__\input.dll</TargetPath>
  </PropertyGroup>
  <Target Name="ResolveReferences" /><Target Name="_ResolveWasmConfiguration" />
  <Target Name="_ResolveWasmOutputs" /><Target Name="ProcessPublishFilesForWasm" />
  <Target Name="_GenerateManagedToNative" />
  <ItemGroup>
    <NativeBindingCache Include="__ROOT__\obj\wasm\for-build\runtime-icall-table.h;__ROOT__\obj\wasm\for-build\m2n_cache.txt;__ROOT__\obj\wasm\for-publish\runtime-icall-table.h;__ROOT__\obj\wasm\for-publish\m2n_cache.txt" />
  </ItemGroup>
  <Import Project="__TARGETS__" />
  <Target Name="PrepareForILLink">
    <Error Condition="'$(ExpectedState)' == 'invalidated' and Exists('$(_LinkSemaphore)')" Text="Old linker semaphore survived an execution identity change." />
    <Error Condition="'$(ExpectedState)' == 'retained' and !Exists('$(_LinkSemaphore)')" Text="Unchanged execution identity invalidated the linker cache." />
    <Error Condition="'$(ExpectedState)' == 'invalidated' and Exists('%(NativeBindingCache.Identity)')" Text="Old native binding cache survived an execution identity change." />
    <Error Condition="'$(ExpectedState)' == 'retained' and !Exists('%(NativeBindingCache.Identity)')" Text="Unchanged execution identity invalidated native bindings." />
    <Touch Files="$(_LinkSemaphore)" AlwaysCreate="true" />
    <WriteLinesToFile File="%(NativeBindingCache.Identity)" Lines="current native bindings" Overwrite="true" />
  </Target>
</Project>
'@
$xml = $xml.Replace('__ROOT__', [Security.SecurityElement]::Escape($fixture))
$xml = $xml.Replace('__TARGETS__', [Security.SecurityElement]::Escape((Join-Path $repository 'packages\platforms\build\Doroti.Web.targets')))
[IO.File]::WriteAllText($project, $xml)
try {
    $step = 0
    foreach ($case in @(@{ Threads = 'true'; State = 'invalidated' }, @{ Threads = 'true'; State = 'retained' },
        @{ Threads = 'false'; State = 'invalidated' }, @{ Threads = 'false'; State = 'retained' })) {
        $step++
        & (Join-Path $PSScriptRoot 'run-validation.ps1') -Executable $dotnet -Arguments @(
            (Join-Path $env:ProgramFiles 'dotnet\sdk\10.0.403\dotnet.dll'), 'msbuild', $project, '-t:PrepareForILLink',
            "-p:WasmEnableThreads=$($case.Threads)", "-p:ExpectedState=$($case.State)", '--nologo', '-v:quiet') `
            -LogPath (Join-Path $artifactRoot "build-contract-cache-$step.log") -WorkingDirectory $repository
        if ($LASTEXITCODE) { throw "Linker execution cache contract failed at step $step." }
        Write-Host "PASS linker/native caches: threads=$($case.Threads); $($case.State) before PrepareForILLink."
    }
} finally {
    $resolved = [IO.Path]::GetFullPath($fixture)
    if (-not $resolved.StartsWith($artifactRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase) -or
        [IO.Path]::GetFileName($resolved) -notmatch '^cache-contracts-[a-f0-9]{32}$') { throw 'Unsafe cache fixture cleanup path.' }
    Remove-Item -LiteralPath $resolved -Recurse -Force
}
exit 0
