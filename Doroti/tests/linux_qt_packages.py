"""Build a fresh Linux Qt package feed and publish a template with an isolated NuGet cache.
Run with eng/run-with-timeout.py; this is local package consumption, not a clean OS test.
"""
from pathlib import Path
import hashlib
import json
import os
import shutil
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
out = Path(sys.argv[1]).resolve()
assert out.is_relative_to(ROOT / 'temp/testing') and not out.exists()
out.mkdir(parents=True)
version = '0.3.0-beta.qt.' + uuid.uuid4().hex[:12]
feed = out / 'feed'

def command(name, args, env=None, cwd=ROOT, success=True):
    with (out / (name + '.log')).open('w') as log:
        result = subprocess.run(args, cwd=cwd, env=env, stdout=log, stderr=subprocess.STDOUT, timeout=1200)
    if (result.returncode == 0) != success:
        raise RuntimeError(name + ': ' + (out / (name + '.log')).read_text()[-6000:])
    print('PASS', name, flush=True)

projects = {}
def visit(project):
    project = project.resolve()
    if project in projects: return
    projects[project] = True
    for reference in ET.parse(project).iter('ProjectReference'):
        visit(project.parent / reference.attrib['Include'].replace('\\', '/'))
for name in ['Doroti.Target.Linux.Qt.linux-x64', 'Doroti.Framework.Material', 'Doroti.Plugins', 'Doroti.App.Sdk', 'Doroti.Runner.Sdk']:
    visit(ROOT / f'Doroti/src/{name}/{name}.csproj')
# Build roots once; packing dependencies then reuses their Release output.
for name in ['Doroti.Target.Linux.Qt.linux-x64', 'Doroti.Framework.Material', 'Doroti.Plugins', 'Doroti.App.Sdk', 'Doroti.Runner.Sdk']:
    command('build-' + name, ['dotnet', 'build', str(ROOT / f'Doroti/src/{name}/{name}.csproj'), '-c', 'Release', '-p:Version=' + version, '-v:q'])
for project in projects:
    command('pack-' + project.stem, ['dotnet', 'pack', str(project), '-c', 'Release', '--no-build', '-p:Version=' + version, '-o', str(feed), '-v:q'])
consumer = out / 'consumer'
shutil.copytree(ROOT / 'Doroti/templates/Doroti.Templates/content/doroti-app', consumer)
for path in consumer.rglob('*'):
    if path.is_file() and path.suffix in ['.cs', '.csproj', '.props', '.json', '.targets']:
        value = path.read_text().replace('0.3.0-beta', version).replace('__DOROTI_APPLICATION_ID__', 'org.doroti.qt.packageprobe')
        value = value.replace('__DOROTI_DISPLAY_VERSION__', '1.0').replace('__DOROTI_APPLICATION_VERSION__', '1')
        path.write_text(value)
app = consumer / 'DorotiTemplateApp.csproj'
app.write_text(app.read_text().replace('</Project>', f'<ItemGroup><PackageReference Include="Doroti.Plugins" Version="{version}" /></ItemGroup></Project>'))
config = ET.Element('configuration')
sources = ET.SubElement(config, 'packageSources'); ET.SubElement(sources, 'clear')
ET.SubElement(sources, 'add', key='candidate', value=str(feed))
ET.SubElement(sources, 'add', key='nuget', value='https://api.nuget.org/v3/index.json')
ET.ElementTree(config).write(consumer / 'NuGet.Config', encoding='utf-8', xml_declaration=True)
env = os.environ | {'NUGET_PACKAGES': str(out / 'nuget')}
project = consumer / 'linux/DorotiTemplateApp.Linux.csproj'
# Qualify the template and packaged SDK defaults without forcing Quick on.
props = []
command('restore', ['dotnet', 'restore', str(project), *props], env, consumer)
command('publish', ['dotnet', 'publish', str(project), '-c', 'Release', *props, '-o', str(out / 'publish'), '-v:q'], env, consumer)
assets = json.loads((consumer / 'linux/obj/linux-x64/project.assets.json').read_text())
assert not any(item.get('type') == 'project' and name.startswith('Doroti.') for name, item in assets['libraries'].items())
for name in ['libdoroti_qt_host.so', 'libSkiaSharp.so', 'Doroti.Host.Qt.dll', 'licenses/doroti-linux/LGPL-3.txt']:
    assert (out / 'publish' / name).is_file(), name
command('published-run', [str(out / 'publish/DorotiTemplateApp.Linux')], env | {'QT_QPA_PLATFORM': 'wayland',
    'DOROTI_QT_VALIDATION_RESIZE_CYCLES': '20', 'DOROTI_QT_DIAGNOSTICS': '1'}, out / 'publish')
log = (out / 'published-run.log').read_text()
summary = json.loads(next(line.split('=', 1)[1] for line in log.splitlines() if line.startswith('doroti.qt.summary=')))
assert summary['frames']['presented'] > 0 and summary['frames']['failed'] == 0
assert summary['nativeFrameMode'] == 'C' and 0 < summary['quickMaximumPending'] <= 2
assert summary['quickConsumersSubmitted'] == summary['quickConsumersCompleted'] > 0
assert summary['quickReservedBytes'] == summary['quickRetiringLayers'] == 0
command('publish-no-build', ['dotnet', 'publish', str(project), '-c', 'Release', '--no-build',
    '-o', str(out / 'publish-no-build'), '-v:q'], env, consumer)
assert (out / 'publish-no-build/libdoroti_qt_host.so').read_bytes() == (out / 'publish/libdoroti_qt_host.so').read_bytes()
command('reject-other-profile-no-build', ['dotnet', 'publish', str(project), '-c', 'Release', '--no-build',
    '-p:DorotiQtQuick=false', '-o', str(out / 'publish-wrong-profile'), '-v:q'], env, consumer, success=False)
assert 'DOROTIQT006' in (out / 'reject-other-profile-no-build.log').read_text()
command('reject-trimming', ['dotnet', 'publish', str(project), '-c', 'Release', *props, '-p:PublishTrimmed=true', '-v:q'], env, consumer, success=False)
assert 'DOROTIQT001' in (out / 'reject-trimming.log').read_text()
report = {'version': version, 'revision': subprocess.check_output(['git','rev-parse','HEAD'], cwd=ROOT, text=True).strip(),
    'workingTree': True, 'packageOnlyRestorePublishRun': True, 'trimming': 'unsupported/rejected', 'configuration': 'Release/JIT/framework-dependent',
    'qt': subprocess.check_output(['pkg-config','--modversion','Qt6Core'], text=True).strip(), 'summary': summary,
    'packages': {path.name: hashlib.sha256(path.read_bytes()).hexdigest() for path in feed.glob('*.nupkg')},
    'payload': {str(path.relative_to(out / 'publish')): hashlib.sha256(path.read_bytes()).hexdigest() for path in (out / 'publish').rglob('*') if path.is_file()}}
(out / 'candidate.json').write_text(json.dumps(report, indent=2))
# Exercise the same portable installer shipped to users, without global registration.
import runpy
installer = runpy.run_path(str(ROOT / 'Doroti/eng/install-linux-qt.py'))
install_root = out / 'installed'
installer['install'](out, install_root, 'DorotiTemplateApp.Linux', 'doroti-qt-probe')
command('installed-run', [str(install_root / 'run')], env | {'QT_QPA_PLATFORM': 'wayland',
    'DOROTI_QT_VALIDATION_RESIZE_CYCLES': '10'}, install_root)
(install_root / 'userdata/keep.txt').write_text('업데이트 보존')
report['version'] = version + '.2'
(out / 'candidate.json').write_text(json.dumps(report))
installer['install'](out, install_root, 'DorotiTemplateApp.Linux', 'doroti-qt-probe')
selected = os.readlink(install_root / 'current')
extra = out / 'publish/unlisted.txt'; extra.write_text('not in manifest')
try:
    installer['install'](out, install_root, 'DorotiTemplateApp.Linux')
    raise AssertionError('Tampered package was accepted')
except ValueError:
    assert os.readlink(install_root / 'current') == selected
finally: extra.unlink()
installer['remove'](install_root)
assert (install_root / 'userdata/keep.txt').read_text() == '업데이트 보존' and not (install_root / 'current').exists()
report['version'] = version
report['portableInstall'] = dict(launch=True, update=True, unlistedFileRejected=True, uninstallPreservesData=True, globalProtocolRegistration='notVerified')
(out / 'candidate.json').write_text(json.dumps(report, indent=2))
print('PASS Linux Qt package-only Release candidate and portable install; unsigned and local OS only.', flush=True)
