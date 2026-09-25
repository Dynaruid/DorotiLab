#!/usr/bin/env python3
"""Build legacy -> Desktop serially and verify generated bootstrap isolation."""
import json
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
output = Path(sys.argv[1]).resolve()
output.mkdir(parents=True, exist_ok=False)
runner = ROOT / 'DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj'
generated = runner.parent / 'obj/linux-x64/Doroti.Generated'
desktop = generated / 'DorotiBootstrap.Desktop.DorotiTestbedApp.Desktop.LinuxDesktopStartup.g.cs'
legacy = generated / 'DorotiBootstrap.g.cs'
timeout = ROOT / 'Doroti/validation/run-with-timeout.py'
results = []
for enabled in ('false', 'true'):
    with (output / (enabled + '.log')).open('w') as log:
        result = subprocess.run([sys.executable, str(timeout), 'dotnet', 'build', str(runner),
                                 '-c', 'Release', '-r', 'linux-x64', '-p:DorotiLinuxDesktop=' + enabled],
                                cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
    assert result.returncode == 0, enabled
    selected = desktop if enabled == 'true' else legacy
    assert ('DesktopApplication.Configure' in selected.read_text()) == (enabled == 'true')
    results.append({'desktop': enabled == 'true', 'build': 'passed', 'bootstrap': str(selected)})
before = desktop.read_bytes()
with (output / 'legacy-generation.log').open('w') as log:
    result = subprocess.run([sys.executable, str(timeout), 'dotnet', 'msbuild', str(runner),
                             '-t:GenerateDorotiRunnerBootstrap', '-p:DorotiLinuxDesktop=false',
                             '-p:Configuration=Release'], cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
assert result.returncode == 0
assert desktop.read_bytes() == before
assert 'DesktopApplication.Configure' not in legacy.read_text()
report = {'status': 'passed', 'builds': results, 'legacyGenerationPreservesDesktopSource': True,
          'runtime': 'Run verify-linux.py against the final Desktop binary separately.'}
(output / 'result.json').write_text(json.dumps(report, indent=2) + '\n')
print(json.dumps(report, indent=2))
