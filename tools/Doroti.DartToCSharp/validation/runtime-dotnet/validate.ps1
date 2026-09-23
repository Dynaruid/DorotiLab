$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$runner = Join-Path $repositoryRoot 'Doroti/validation/run-with-timeout.py'
$compiler = Join-Path $repositoryRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj'
$manifest = Join-Path $PSScriptRoot 'selection.json'
$output = Join-Path $repositoryRoot 'Doroti/artifacts/runtime-dotnet/compiler-fixture'
$generated = Join-Path $output 'projects/Framework/CompilerRuntimeInteropFixture.Framework.csproj'

python $runner dotnet run --project $compiler -c Release -- --manifest $manifest --output $output
if ($LASTEXITCODE -ne 0) { throw "Compiler fixture generation failed: $LASTEXITCODE" }

python $runner dotnet build $generated -c Release -v quiet "-p:DorotiRepositoryRoot=$(Join-Path $repositoryRoot 'Doroti')"
if ($LASTEXITCODE -ne 0) { throw "Compiler fixture build failed: $LASTEXITCODE" }
