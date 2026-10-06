"""External provider alias, typed tool services, NuGet-only runtime and actual CLI operations."""
from pathlib import Path
import json, subprocess, sys, uuid, shutil
ROOT = Path(__file__).resolve().parents[2]
RUN_ROOT = ROOT / "temp/testing/platform-decoupling/providers"
run = RUN_ROOT / uuid.uuid4().hex
run.mkdir(parents=True)
feed = run / "feed"
feed.mkdir()
results=[]
def call(label, args, expected=0):
    print("Running " + label, flush=True)
    cmd=[sys.executable, str(ROOT / "Doroti/eng/run-with-timeout.py"), "--timeout", "1200", *map(str,args)]
    result=subprocess.run(cmd,cwd=ROOT,text=True,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
    (run / (label + ".log")).write_text(result.stdout,encoding="utf-8")
    if bool(result.returncode) != bool(expected): raise RuntimeError(label + ": " + result.stdout[-12000:])
    results.append(dict(command=cmd,result="PASS",expectedFailure=bool(expected)))
    return result.stdout
def pack(project):
    call("pack-" + project.stem,["dotnet","pack",project,"-c","Debug","-o",feed])
seen=set()
def closure(project):
    project=project.resolve()
    if project in seen:return
    seen.add(project)
    data=json.loads(subprocess.check_output(["dotnet","msbuild",str(project),"-nologo","-getItem:ProjectReference"],cwd=ROOT))
    for item in data['Items']['ProjectReference']:closure(Path(item['FullPath']))
    pack(project)
try:
    closure(ROOT/'Doroti/tests/fixtures/test-headless/runtime/TestHeadless.Runtime.csproj')
    closure(ROOT/'Doroti/tests/fixtures/test-headless/tooling/TestHeadless.Tool.csproj')
    call("build-cli",["dotnet","build",ROOT/'Doroti/src/Doroti.Tooling/Doroti.Tooling.csproj',"-c","Debug"])
    cli=ROOT/'Doroti/src/Doroti.Tooling/bin/Debug/net10.0/Doroti.Tooling.dll'
    app=run/'app'; app.mkdir()
    for name in ('Directory.Build.props','Directory.Build.targets','Directory.Packages.props'):(app/name).write_text('<Project/>',encoding='utf-8')
    (app/'NuGet.Config').write_text(f'<configuration><packageSources><clear/><add key="local" value="{feed}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources><config><add key="globalPackagesFolder" value="{run / "nuget"}"/></config><packageSourceMapping><packageSource key="local"><package pattern="Doroti.*"/></packageSource><packageSource key="nuget"><package pattern="*"/></packageSource></packageSourceMapping></configuration>',encoding='utf-8')
    (app/'App.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType></PropertyGroup><ItemGroup><PackageReference Include="Doroti.TestProvider.Runtime" Version="0.4.0-alpha.1"/><PackageReference Include="Doroti.TestProvider.Tool" Version="0.4.0-alpha.1"/></ItemGroup></Project>',encoding='utf-8')
    (app/'Program.cs').write_text('await Doroti.TestProvider.Runtime.HeadlessApplication.RunAsync();',encoding='utf-8')
    call('restore-consumer',['dotnet','restore',app/'App.csproj'])
    assets=json.loads((app/'obj/project.assets.json').read_text())
    assert all(item['type']=='package' for item in assets['libraries'].values())
    platform=dict(provider='test-headless',providerManifest='nuget:Doroti.TestProvider.Tool',target='test-headless',targetPackage='Doroti.TestProvider.Runtime',runner='App.csproj',backend='cpu',profile='default',targetFramework='net10.0',runtimeIdentifier='')
    workspace=dict(schemaVersion='doroti.workspace/v2',applicationProject='App.csproj',platforms={'laboratory-x':platform})
    manifest=app/'doroti-workspace.json';manifest.write_text(json.dumps(workspace),encoding='utf-8')
    def command(name, extra=(), expected=0):
        return call(name,['dotnet',cli,name.split('-negative')[0],'-App',app,'-Platform','laboratory-x',*extra],expected)
    described=json.loads(command('describe'))
    assert described['schemaVersion']=='doroti.cli-workspace/v2' and 'laboratory-x' in described['platforms']
    assert 'undeclared-platform' in call('describe-negative-missing-alias', ['dotnet', cli, 'describe', '-App', app, '-Platform', 'unavailable'], expected=1)
    doctor=json.loads(command('doctor'));assert doctor['diagnostics'][0]['status']=='Pass'
    devices=json.loads(command('devices'));assert devices['devices'][0]['id']=='host-process'
    assert json.loads(command('configuration'))['options'][0]['name']=='delay'
    assert json.loads(command('templates'))['templates'][0]['id']=='headless-app'
    command('build',['-Device','host-process'])
    output=command('run',['-Device','host-process'])
    assert output.index('PrepareProcess')<output.index('startup-constructor')<output.index('Configure')<output.index('Bootstrap')<output.index('Attach:1')<output.index('Attach:2')<output.index('Detach:1')
    assert 'PASS: external provider' in output
    generated=app/'generated'
    command('generate',['-Output',generated,'-Template','headless-app','-Design','widgets'])
    call('generated-run',['dotnet','run','--project',generated/'Generated.csproj'])
    assert 'unsupported-device' in command('run-negative-device',['-Device','unavailable'],expected=1)
    target_manifest=run/'nuget/doroti.testprovider.runtime/0.4.0-alpha.1/doroti/doroti-target-manifest.json'
    original_target=target_manifest.read_bytes(); target_model=json.loads(original_target)
    for field,wrong in [('schemaVersion','doroti.target-package/v1'),('packageId','wrong'),('packageVersion','0.4.0-alpha.9'),('providerVersion','0.4.0-alpha.9'),('providerProfile','unknown'),('rid','unexpected'),('coreRange','[0.5.0,0.6.0)'),('protocolVersion',99)]:
        target_manifest.write_text(json.dumps(target_model|{field:wrong}),encoding='utf-8')
        assert 'resolved-target-mismatch' in command('describe-negative-target-'+field,expected=1)
    target_manifest.write_bytes(original_target)
    tool_manifest=run/'nuget/doroti.testprovider.tool/0.4.0-alpha.1/lib/net10.0/doroti-provider.json'
    original_tool=tool_manifest.read_bytes();tool_model=json.loads(original_tool)
    tool_manifest.write_text(json.dumps(tool_model|dict(coreRange='[0.5.0,0.6.0)')),encoding='utf-8')
    assert 'unsupported-core' in command('describe-negative-core-range',expected=1)
    tool_manifest.write_text(json.dumps(tool_model|dict(version='0.4.0-alpha.9')),encoding='utf-8')
    assert 'resolved-provider-mismatch' in command('describe-negative-provider-version',expected=1)
    tool_manifest.write_bytes(original_tool)
    workspace['schemaVersion']='doroti.workspace/v1';manifest.write_text(json.dumps(workspace),encoding='utf-8')
    assert 'unsupported-schema' in command('describe-negative-old-schema',expected=1)
    workspace['schemaVersion']='doroti.workspace/v2';workspace['platforms']={'../escape':platform};manifest.write_text(json.dumps(workspace),encoding='utf-8')
    assert 'invalid-alias' in command('describe-negative-alias',expected=1)
    report=dict(schema='doroti.platform-provider-verification/v1',result='PASS',results=results,scope='external NuGet-only headless runtime and installed managed tool descriptor discovery; arbitrary alias; describe/real SDK probe/config/device/templates/generated build/run; startup trace/primary-survivor/Stop; target schema/package/provider identity/profile/RID/core-range/protocol and installed tool version/core-range mismatch rejection',notVerified=['native Windows G1','physical input/GPU','complete built-in provider migration'],runId=run.name)
    evidence=ROOT/'Doroti/docs/migrations/design-platform/platform-provider-verification.json';evidence.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    assert run.resolve().parent==RUN_ROOT.resolve();shutil.rmtree(run)
    print('PASS: external provider NuGet and CLI contracts; '+str(evidence),flush=True)
except BaseException:
    print('Failure evidence retained: '+str(run),flush=True);raise
