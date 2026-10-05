"""Evaluate provider ownership and Apple closures; no native allocation or build is performed."""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
from datetime import datetime, timezone

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
output = args.output.resolve()
if not output.is_relative_to(ROOT / 'temp/testing'):
    parser.error('Output must be under temp/testing.')
output.parent.mkdir(parents=True, exist_ok=True)
projects = [(f'Doroti/src/{name}/{name}.csproj', [], 'core') for name in
            ['Doroti.Runtime', 'Doroti.Ui', 'Doroti.Hosting', 'Doroti.Desktop', 'Doroti.Tooling.Contracts', 'Doroti.Tooling.Extension.Sdk']]
projects += [(f'packages/Doroti.{name}/Doroti.{name}.csproj', [], name) for name in ['Material', 'Cupertino']]
projects += [('packages/platforms/maui/tooling/Doroti.Tool.Maui.csproj', [], 'tool')]
for tfm, rid in [('macos', 'osx-arm64'), ('maccatalyst', 'maccatalyst-arm64'), ('ios', 'iossimulator-arm64'), ('ios', 'ios-arm64')]:
    props = ['-p:TargetFramework=net10.0-' + tfm + '27.0', '-p:RuntimeIdentifier=' + rid,
             '-p:DorotiMacOSTargetFramework=net10.0-macos27.0',
             '-p:DorotiMacCatalystTargetFramework=net10.0-maccatalyst27.0',
             '-p:DorotiIosTargetFramework=net10.0-ios27.0', '-p:DorotiCompilationMode=Mono']
    projects.append(('packages/platforms/maui/Doroti.Host.Maui/Doroti.Host.Maui.csproj', props, 'provider:' + rid))
results = []
for relative, overrides, owner in projects:
    command = ['dotnet', 'msbuild', str(ROOT / relative), '-nologo',
               '-getProperty:TargetFramework,TargetFrameworks,RuntimeIdentifier,UseMaui,Version,PackageVersion',
               '-getItem:Compile,EmbeddedResource,ProjectReference,PackageReference,InternalsVisibleTo', *overrides]
    probe = subprocess.run(command, cwd=ROOT, text=True, capture_output=True, timeout=60)
    if probe.returncode:
        raise RuntimeError(probe.stdout + probe.stderr)
    data = json.loads(probe.stdout)
    items = data['Items']
    sources = [str(Path(item['FullPath']).resolve()) for item in items['Compile']]
    assert len(sources) == len(set(sources)), (relative, 'duplicate Compile', sources)
    resources = [item.get('LogicalName') or item['Identity'] for item in items['EmbeddedResource']]
    assert len(resources) == len(set(resources)), (relative, 'duplicate resources', resources)
    references = [Path(item['FullPath']).resolve().relative_to(ROOT).as_posix() for item in items['ProjectReference']]
    if owner == 'core':
        assert not any(ref.startswith('packages/') for ref in references), (relative, references)
        assert data['Properties']['UseMaui'] != 'true', data
    if owner == 'Cupertino':
        assert not any('Doroti.Material' in ref for ref in references), references
    if owner == 'tool':
        assert all(ref.startswith('Doroti/src/Doroti.Tooling.') for ref in references), references
        assert data['Properties']['UseMaui'] != 'true', data
    results.append({'project': relative, 'owner': owner, 'properties': data['Properties'],
                    'compileCount': len(sources), 'resourceCount': len(resources), 'projectReferences': references,
                    'packageReferences': [item['Identity'] for item in items['PackageReference']],
                    'friendAssemblies': [item['Identity'] for item in items['InternalsVisibleTo']],
                    'evaluationSha256': hashlib.sha256(probe.stdout.encode()).hexdigest()})
    print('PASS evaluated ownership/duplicate inputs:', owner, flush=True)
output.write_text(json.dumps({'schema': 'doroti.apple-provider-evaluation/v1',
    'verifiedUtc': datetime.now(timezone.utc).isoformat(), 'result': 'PASS', 'results': results,
    'notVerified': ['complete Roslyn/public API compatibility audit', 'independent NuGet Apple consumers',
                    'trim/NativeAOT', 'physical input/accessibility/signing']}, ensure_ascii=False, indent=2) + '\n')
