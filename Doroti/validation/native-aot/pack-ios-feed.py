#!/usr/bin/env python3
"""Pack the validated iOS NativeAOT graph into an isolated local feed (never push)."""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import sys

HERE = Path(__file__).resolve().parent
PRODUCT = HERE.parents[1]
ROOT = PRODUCT.parent
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--artifacts', type=Path, required=True, help='Output path of a successful source Testbed publish')
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
output = args.output.resolve()
feed = output / 'feed'
feed.mkdir(parents=True, exist_ok=True)
projects = []
seen = set()

def collect(project):
    project = project.resolve()
    if project in seen:
        return
    seen.add(project)
    evaluated = subprocess.run(['dotnet', 'msbuild', str(project), '-nologo', '-getItem:ProjectReference',
        '-p:DorotiCompilationMode=NativeAot', '-p:DorotiHostTargetFrameworks=net11.0-ios',
        *(['-p:TargetFramework=net11.0-ios'] if project.stem == 'Doroti.Host.Maui' else [])],
        cwd=ROOT, capture_output=True, text=True, timeout=1200, check=True)
    for item in json.loads(evaluated.stdout)['Items']['ProjectReference']:
        collect(Path(item['FullPath']))
    projects.append(project)

for name in ['Doroti.Framework.Material', 'Doroti.Target.iOS.Maui.ios-arm64', 'Doroti.App.Sdk', 'Doroti.Runner.Sdk']:
    collect(PRODUCT / 'src' / name / (name + '.csproj'))
projects.append(PRODUCT / 'templates/Doroti.Templates/Doroti.Templates.csproj')
steps = []
passed = False
try:
    for project in projects:
        # Rebuild when source/inputs changed; --no-build can silently package
        # an earlier handler even after the source product was republished.
        command = ['dotnet', 'pack', str(project), '-c', 'Release', '-o', str(feed),
                   '-p:UseSharedCompilation=false', '-p:DorotiCompilationMode=NativeAot',
                   '-p:DorotiHostTargetFrameworks=net11.0-ios',
                   '-p:ArtifactsPath=' + str(args.artifacts.resolve()), '-v:minimal']
        if project.stem in ('Doroti.Host.Maui', 'Doroti.Target.iOS.Maui.ios-arm64'):
            # The target's ProjectReference supplies the device RID at build
            # time; make it available in the host's restore graph as well.
            command.append('-p:RuntimeIdentifiers=ios-arm64')
        log = output / (project.stem + '.log')
        result = subprocess.run([sys.executable, str(HERE / 'run.py'), '--log', str(log), '--', *command],
                                cwd=ROOT, timeout=1230, check=False)
        steps.append({'project': str(project), 'exitCode': result.returncode, 'log': str(log)})
        if result.returncode:
            raise RuntimeError(f'{project.stem}: pack failed; see {log}')
    passed = True
finally:
    packages = [{'path': str(p), 'sha256': hashlib.sha256(p.read_bytes()).hexdigest()}
                for p in sorted(feed.glob('*.nupkg'))]
    (output / 'result.json').write_text(json.dumps({'schemaVersion': 'doroti.nativeaot-local-feed/v1',
        'passed': passed, 'scope': 'experimental iOS net11 feed; not a replacement for other platform packages',
        'steps': steps, 'packages': packages}, indent=2) + '\n')
