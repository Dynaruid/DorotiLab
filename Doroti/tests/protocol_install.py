"""Current-user OS protocol installation/update/removal using a disposable console consumer.

Run via eng/run-with-timeout.py. This proves OS URI dispatch and installer ownership,
not production signing or the GUI application's warm-instance navigation contract.
"""
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import time
import uuid

root = Path(__file__).resolve().parents[2]
run = Path(sys.argv[1]).resolve()
assert run.is_relative_to(root / 'temp/testing') and not run.exists()
run.mkdir(parents=True)
scheme = 'doroti-test-' + uuid.uuid4().hex[:12]
installed = run / 'installed'
installer = root / 'Doroti/eng/install-candidate.ps1'
registration = root / 'Doroti/eng/register-app-protocol.ps1'
project = run / 'probe'
project.mkdir()
(project / 'ProtocolProbe.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings></PropertyGroup></Project>')
evidence = installed / 'userdata/activation.json'
(project / 'Program.cs').write_text('using System.Text.Json; File.WriteAllText(' + json.dumps(str(evidence)) + ', JsonSerializer.Serialize(new { args, directory = AppContext.BaseDirectory }));')
(run / 'activate.ps1').write_text('param([string] $Uri)\nStart-Process -FilePath $Uri -WindowStyle Hidden\n')

def command(*args, succeeds=True):
    result = subprocess.run(args, cwd=root, capture_output=True, text=True, encoding='utf-8', errors='replace')
    assert (result.returncode == 0) == succeeds, result.stdout[-4000:] + result.stderr[-4000:]

def install(candidate, **options):
    command('pwsh', '-NoProfile', '-File', str(installer), '-InstallRoot', str(installed),
            '-CandidateRoot', str(candidate), *[value for pair in options.items() for value in ('-' + pair[0], pair[1])])

try:
    for index in (1, 2):
        candidate = run / f'candidate-{index}'
        payload = candidate / 'windows'
        command('dotnet', 'publish', str(project / 'ProtocolProbe.csproj'), '-c', 'Release', '-r', 'win-x64',
                '--self-contained', 'false', '-o', str(payload), '--nologo')
        manifest = {'status': 'PASS: local protocol fixture', 'version': f'9.0.0-work2.{index}', 'revision': 'fixture',
                    'artifacts': {'windows': {p.relative_to(payload).as_posix(): hashlib.sha256(p.read_bytes()).hexdigest()
                                             for p in payload.rglob('*') if p.is_file()}}}
        (candidate / 'candidate.json').write_text(json.dumps(manifest))
        install(candidate, **({'ProtocolScheme': scheme, 'ProtocolExecutable': 'ProtocolProbe.exe'} if index == 1 else {}))
        if evidence.exists(): evidence.unlink()
        uri = scheme + f'://activation/{index}?value=unicode-%ED%95%9C'
        command('pwsh', '-NoProfile', '-File', str(run / 'activate.ps1'), uri)
        deadline = time.monotonic() + 15
        while not evidence.exists():
            if time.monotonic() > deadline: raise TimeoutError('OS protocol did not launch the installed consumer.')
            time.sleep(.1)
        result = json.loads(evidence.read_text())
        assert result['args'] == [uri] and f'9.0.0-work2.{index}-windows' in result['directory'], result
    import winreg
    def registered_command():
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, 'Software\\Classes\\' + scheme + '\\shell\\open\\command') as key:
            return winreg.QueryValueEx(key, '')[0]
    before = registered_command()
    current = (installed / 'current.json').read_bytes()
    command('pwsh', '-NoProfile', '-File', str(installer), '-InstallRoot', str(installed), '-CandidateRoot', str(candidate),
            '-ProtocolScheme', scheme, '-ProtocolExecutable', 'missing.exe', succeeds=False)
    assert registered_command() == before and (installed / 'current.json').read_bytes() == current, 'Failed protocol update did not roll back.'
    # Ownership and traversal failures cannot hijack another installation.
    other = run / 'other'
    command('pwsh', '-NoProfile', '-File', str(installer), '-InstallRoot', str(other), '-CandidateRoot', str(candidate))
    command('pwsh', '-NoProfile', '-File', str(registration), '-InstallRoot', str(other), '-Scheme', scheme,
            '-Executable', 'ProtocolProbe.exe', succeeds=False)
    command('pwsh', '-NoProfile', '-File', str(registration), '-InstallRoot', str(installed), '-Scheme', scheme,
            '-Executable', '../outside.exe', succeeds=False)
    command('pwsh', '-NoProfile', '-File', str(installer), '-InstallRoot', str(installed), '-Action', 'Remove')
    try:
        winreg.OpenKey(winreg.HKEY_CURRENT_USER, 'Software\\Classes\\' + scheme)
        raise AssertionError('Protocol registration survived uninstall.')
    except FileNotFoundError:
        pass
    assert evidence.exists(), 'Uninstall deleted userdata.'
    print('PASS: real OS protocol dispatch, quoted URI, update retargeting, ownership/traversal rejection, unregister and preserved userdata.')
finally:
    if (installed / '.doroti-candidate-install.json').exists():
        command('pwsh', '-NoProfile', '-File', str(registration), '-InstallRoot', str(installed), '-Scheme', scheme, '-Action', 'Remove')
