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
exit 0
