"""Pack the desktop dependency closure and run an external package-only consumer.

All subprocesses share the outer run-with-timeout.py 1,200 second deadline.
The uniquely named temporary consumer is retained as a reviewable local artifact.
"""
import json
from pathlib import Path
import subprocess
import tempfile
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'Doroti/artifacts/validation/desktop-package'
FEED = OUT / 'feed'
FEED.mkdir(parents=True, exist_ok=True)
consumer = Path(tempfile.mkdtemp(prefix='doroti-desktop-consumer-'))
projects = []
seen = set()
def visit(project):
    project = project.resolve()
    if project in seen:
        return
    seen.add(project)
    for ref in ET.parse(project).iter('ProjectReference'):
        visit(project.parent / ref.attrib['Include'].replace('\\', '/'))
    projects.append(project)

def run(args, *, expected=None, cwd=ROOT):
    result = subprocess.run(args, cwd=cwd, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    if expected:
        assert result.returncode != 0 and expected in result.stdout, result.stdout
    else:
        assert result.returncode == 0, result.stdout
    return result.stdout

visit(ROOT / 'Doroti/src/Doroti.Desktop.Widgets/Doroti.Desktop.Widgets.csproj')
run(['dotnet', 'build', str(projects[-1]), '-c', 'Release', '--nologo', '-v:q'])
for project in projects:
    run(['dotnet', 'pack', str(project), '-c', 'Release', '--no-build', '--no-restore', '-o', str(FEED), '--nologo', '-v:q'])
    print(f'PACK {project.stem}', flush=True)

(consumer / 'NuGet.Config').write_text(f'<configuration><packageSources><clear/><add key="local" value="{FEED}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources></configuration>', encoding='utf-8')
(consumer / 'Program.cs').write_text((Path(__file__).with_name('Program.cs')).read_text(encoding='utf-8'), encoding='utf-8')
project = consumer / 'Consumer.csproj'
project.write_text('''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
<RestorePackagesPath>packages</RestorePackagesPath></PropertyGroup><ItemGroup>
<PackageReference Include="Doroti.Desktop.Widgets" Version="0.3.0-beta"/>
</ItemGroup></Project>''', encoding='utf-8')
output = run(['dotnet', 'run', '--project', str(project), '-c', 'Release'], cwd=consumer)
assert 'contracts passed' in output, output
for target in ['Web', 'Android', 'iOS', 'MacCatalyst']:
    run(['dotnet', 'build', str(project), '--no-restore', f'-p:DorotiTarget={target}', '--nologo', '-v:q'], cwd=consumer, expected='DOROTIDESKTOP001')
    print(f'PASS package boundary {target}', flush=True)
assets = json.loads((consumer / 'obj/project.assets.json').read_text(encoding='utf-8'))
assert all(item.get('type') != 'project' for item in assets['libraries'].values()), 'Source project leaked into consumer'
result = dict(status='PASS', consumer=str(consumer), packages=len(projects), packageOnly=True,
              negativeTargets=['Web', 'Android', 'iOS', 'MacCatalyst'], windowsExecution='notRun', contractOutput=output)
(OUT / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result, indent=2))
