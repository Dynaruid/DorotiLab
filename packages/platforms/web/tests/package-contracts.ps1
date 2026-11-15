[CmdletBinding()]
param([Parameter(Mandatory)][string] $Package, [Parameter(Mandatory)][string] $WebRoot)
$ErrorActionPreference = 'Stop'
$repository = (Resolve-Path (Join-Path $PSScriptRoot '../../../..')).Path
$artifactRoot = [IO.Path]::GetFullPath((Join-Path $repository 'Doroti/artifacts/web-qualification'))
$scratch = Join-Path $artifactRoot ('provider-contract-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($scratch) | Out-Null
$dotnet = Join-Path $env:ProgramFiles 'dotnet/dotnet.exe'
$sdk = Join-Path $env:ProgramFiles 'dotnet/sdk/10.0.403/dotnet.dll'
$runner = Join-Path $PSScriptRoot 'run-validation.ps1'
try {
    $unpacked = Join-Path $scratch 'package'
    [IO.Compression.ZipFile]::ExtractToDirectory((Resolve-Path -LiteralPath $Package).Path, $unpacked)
    foreach ($file in @('MergeWebPublish.cs', 'Doroti.Web.PublishTasks.targets', 'Doroti.Web.targets', 'WebPublishTasks/Doroti.Web.Publish.Tasks.csproj')) {
        $packed = Join-Path $unpacked "provider-build/$file"
        if (-not (Test-Path -LiteralPath $packed -PathType Leaf)) { throw "Missing packaged build source: $file" }
        if ((Get-FileHash -LiteralPath $packed).Hash -ne (Get-FileHash -LiteralPath (Join-Path $repository "packages/platforms/build/$file")).Hash) { throw "Stale packaged build source: $file" }
    }
    $root = (Resolve-Path -LiteralPath $WebRoot).Path
    $profile = Get-Content -LiteralPath (Join-Path $root 'doroti.runtime-profile.json') -Raw | ConvertFrom-Json
    $escapedPackage = [Security.SecurityElement]::Escape($unpacked)
    $escapedPublish = [Security.SecurityElement]::Escape((Split-Path $root))
    $project = Join-Path $scratch 'consumer.proj'
    @"
<Project>
  <PropertyGroup><DorotiProviderSuppliesBootstrap>true</DorotiProviderSuppliesBootstrap><DorotiTarget>Web</DorotiTarget><DorotiWebExecutionMode>auto</DorotiWebExecutionMode><PublishDir>$escapedPublish/</PublishDir></PropertyGroup>
  <Import Project="$escapedPackage/provider-build/Doroti.Web.PublishTasks.targets" />
  <Import Project="$escapedPackage/buildTransitive/Doroti.Target.Web.browser-wasm.targets" />
</Project>
"@ | Set-Content -LiteralPath $project -Encoding utf8NoBOM
    foreach ($scenario in @('complete', 'wrong-generation')) {
        $buildId = if ($scenario -eq 'complete') { $profile.buildId } else { 'stale-generation' }
        $log = Join-Path $artifactRoot "remaining-package-$scenario.log"
        & $runner -Executable $dotnet -Arguments @($sdk, 'msbuild', $project, '-t:DorotiValidatePublishedBrowserProduct', "-p:DorotiWebBuildId=$buildId", '--nologo', '-v:minimal') -LogPath $log -WorkingDirectory $repository
        if ($scenario -eq 'complete' -and $LASTEXITCODE -ne 0) { throw 'Packaged publish validator rejected the complete deployment.' }
        if ($scenario -eq 'wrong-generation' -and ($LASTEXITCODE -eq 0 -or (Get-Content -LiteralPath $log -Raw) -notmatch 'DOROTIWEB046')) { throw 'Packaged validator accepted a mismatched generation.' }
        Write-Host "PASS packaged validator: $scenario"
    }
} finally {
    $resolved = [IO.Path]::GetFullPath($scratch)
    if (-not $resolved.StartsWith($artifactRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase) -or [IO.Path]::GetFileName($resolved) -notmatch '^provider-contract-[a-f0-9]{32}$') { throw 'Unsafe fixture cleanup path.' }
    Remove-Item -LiteralPath $resolved -Recurse -Force
}
exit 0
