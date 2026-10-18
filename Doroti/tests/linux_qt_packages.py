"""Build a fresh Linux Qt package feed and publish a template with an isolated NuGet cache.
Run with eng/run-with-timeout.py; this is local package consumption, not a clean OS test.
"""
from pathlib import Path
import argparse
import hashlib
import json
import os
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('output', type=Path)
parser.add_argument('--qpa', choices=['wayland', 'xcb'], default='wayland' if os.environ.get('WAYLAND_DISPLAY') else 'xcb')
parser.add_argument('--qt-sdk', type=Path, required=True, help='Reviewed Qt 6.12.0 SDK prefix for native build and publication')
parser.add_argument('--deb', action='store_true', help='Also exercise provider deb publish from the isolated NuGet consumer (Debian 13 only)')
args = parser.parse_args()
out = args.output.resolve()
assert out.is_relative_to(ROOT / 'temp/testing') and not out.exists()
out.mkdir(parents=True)
suffix = '.qt.' + uuid.uuid4().hex[:12]
version = next(ET.parse(ROOT / 'Doroti/Directory.Build.props').iter('DorotiCoreVersion')).text + suffix
provider_version = next(ET.parse(ROOT / 'packages/platforms/qt/Version.props').iter('DorotiQtProviderVersion')).text + suffix
application_id = 'org.doroti.qt.packageprobe'
feed = out / 'feed'
native = out / 'native'
properties = ['-p:Version=' + version, '-p:DorotiCoreVersion=' + version,
    '-p:DorotiQtProviderVersion=' + provider_version, '-p:ArtifactsPath=' + str(out / 'source-build'),
    '-p:DorotiQtPrebuiltDirectory=' + str(native), '-p:DorotiRequireNativePackage=true']

def command(name, args, env=None, cwd=ROOT, success=True):
    with (out / (name + '.log')).open('w') as log:
        result = subprocess.run(list(map(str, args)), cwd=cwd, env=env, stdout=log, stderr=subprocess.STDOUT, timeout=1200)
    if (result.returncode == 0) != success:
        raise RuntimeError(name + ': ' + (out / (name + '.log')).read_text()[-6000:])
    print('PASS', name, flush=True)
    return (out / (name + '.log')).read_text()

projects = {}
def visit(project):
    project = project.resolve()
    if project in projects: return
    # Evaluate conditions, project references, independent versions and packability.
    model = json.loads(command('evaluate-' + project.stem, ['dotnet', 'msbuild', project, '-nologo',
        '-getProperty:PackageId,PackageVersion,IsPackable', '-getItem:ProjectReference', *properties]))
    projects[project] = model['Properties']
    for reference in model['Items']['ProjectReference']:
        visit(Path(reference['FullPath']))
roots = [ROOT / path for path in [
    'packages/platforms/qt/Doroti.Target.Linux.Qt.linux-x64/Doroti.Target.Linux.Qt.linux-x64.csproj',
    'packages/platforms/qt/tooling/Doroti.Tool.Qt.csproj',
    'Doroti/src/Doroti.Framework.Widgets/Doroti.Framework.Widgets.csproj',
    'Doroti/src/Doroti.Plugins/Doroti.Plugins.csproj',
    'Doroti/src/Doroti.App.Sdk/Doroti.App.Sdk.csproj',
    'Doroti/src/Doroti.Runner.Sdk/Doroti.Runner.Sdk.csproj',
    'Doroti/templates/Doroti.Templates/Doroti.Templates.csproj',
]]
for project in roots: visit(project)
command('native-build', [sys.executable, ROOT / 'packages/platforms/qt/eng/build-native.py',
    '--output', native, '--qt-sdk', args.qt_sdk.resolve(), '--icon', ROOT / 'packages/platforms/build/Icons/appicon.png'])
# Build roots once; packing dependencies then reuses their Release output.
for project in roots:
    command('build-' + project.stem, ['dotnet', 'build', project, '-c', 'Release', *properties, '-v:q'])
versions = {}
for project, model in projects.items():
    if model['IsPackable'] != 'true': continue
    command('pack-' + project.stem, ['dotnet', 'pack', project, '-c', 'Release', '--no-build', *properties, '-o', feed, '-v:q'])
    package = feed / (model['PackageId'] + '.' + model['PackageVersion'] + '.nupkg')
    with zipfile.ZipFile(package) as archive:
        metadata = ET.fromstring(archive.read(next(name for name in archive.namelist() if name.endswith('.nuspec'))))
        actual = next(node.text for node in metadata.iter() if node.tag.endswith('}version'))
        assert actual == model['PackageVersion'], (project, actual, model)
    versions[model['PackageId']] = actual
consumer = out / 'consumer'
hive = out / 'template-hive'
command('template-install', ['dotnet', 'new', 'install', feed / ('Doroti.Templates.' + versions['Doroti.Templates'] + '.nupkg'),
    '--debug:custom-hive', hive])
command('template-create', ['dotnet', 'new', 'doroti-app', '-n', 'DorotiTemplateApp', '--design', 'widgets',
    '--applicationId', application_id, '--displayVersion', '1.0', '--applicationVersion', '1',
    '-o', consumer, '--debug:custom-hive', hive])
for path in consumer.rglob('*.csproj'):
    tree = ET.parse(path)
    root = tree.getroot()
    sdk = root.get('Sdk', '')
    if sdk.startswith('Doroti.') and '/' in sdk:
        identity = sdk.split('/')[0]
        if identity in versions: root.set('Sdk', identity + '/' + versions[identity])
    for node in root.iter('Sdk'):
        if node.get('Name') in versions: node.set('Version', versions[node.get('Name')])
    for node in root.iter('PackageReference'):
        if node.get('Include') in versions: node.set('Version', versions[node.get('Include')])
    tree.write(path, encoding='utf-8', xml_declaration=True)
linux_project = consumer / 'linux/DorotiTemplateApp.Linux.csproj'
tree = ET.parse(linux_project)
group = ET.SubElement(tree.getroot(), 'PropertyGroup')
ET.SubElement(group, 'DorotiQtSdkDirectory').text = str(args.qt_sdk.resolve())
ET.SubElement(group, 'DorotiQtVersion').text = '6.12.0'
tree.write(linux_project, encoding='utf-8', xml_declaration=True)
# Block ambient repository props/targets and central package versions.
for name in ['Directory.Build.targets', 'Directory.Packages.props']:
    (consumer / name).write_text('<Project />')
if args.deb:
    (consumer / 'COPYRIGHT').write_text((ROOT / 'LICENSE').read_text())
    project = consumer / 'linux/DorotiTemplateApp.Linux.csproj'
    tree = ET.parse(project)
    group = ET.SubElement(tree.getroot(), 'PropertyGroup')
    for key, value in dict(DorotiDebPackageId='doroti-nuget-probe', DorotiDebMaintainer='Doroti test <test@example.org>',
                           DorotiDebDescription='Isolated Doroti NuGet consumer', DorotiDebVersion='1.0.0',
                           DorotiDebExcludeLttngProvider='true',
                           DorotiDebCopyright='../COPYRIGHT').items():
        ET.SubElement(group, key).text = value
    tree.write(project, encoding='utf-8', xml_declaration=True)
app = consumer / 'DorotiTemplateApp.csproj'
app.write_text(app.read_text().replace('</Project>', f'<ItemGroup><PackageReference Include="Doroti.Plugins" Version="{version}" /></ItemGroup></Project>'))
config = ET.Element('configuration')
sources = ET.SubElement(config, 'packageSources'); ET.SubElement(sources, 'clear')
ET.SubElement(sources, 'add', key='candidate', value=str(feed))
ET.SubElement(sources, 'add', key='nuget', value='https://api.nuget.org/v3/index.json')
mapping = ET.SubElement(config, 'packageSourceMapping')
ET.SubElement(ET.SubElement(mapping, 'packageSource', key='candidate'), 'package', pattern='Doroti.*')
ET.SubElement(ET.SubElement(mapping, 'packageSource', key='nuget'), 'package', pattern='*')
ET.ElementTree(config).write(consumer / 'NuGet.Config', encoding='utf-8', xml_declaration=True)
env = os.environ | {'NUGET_PACKAGES': str(out / 'nuget'), 'NUGET_HTTP_CACHE_PATH': str(out / 'http-cache')}
project = consumer / 'linux/DorotiTemplateApp.Linux.csproj'
# Qualify the template and packaged SDK defaults without forcing Quick on.
props = []
command('restore', ['dotnet', 'restore', str(project), *props], env, consumer)
command('publish', ['dotnet', 'publish', str(project), '-c', 'Release', *props, '-o', str(out / 'publish'), '-v:q'], env, consumer)
assets_path = command('assets-path', ['dotnet', 'msbuild', project, '-nologo', '-getProperty:ProjectAssetsFile'], env, consumer).strip()
assets = json.loads(Path(assets_path).read_text())
assert not any(item.get('type') == 'project' and name.startswith('Doroti.') for name, item in assets['libraries'].items())
for name in assets['libraries']:
    identity, resolved = name.split('/')
    if identity.startswith('Doroti.'): assert resolved == versions[identity], name
for name in ['libdoroti_qt_host.so', 'libSkiaSharp.so', 'Doroti.Host.Qt.dll']:
    assert (out / 'publish' / name).is_file(), name
for name in ['licenses/doroti-linux/LGPL-3.txt', 'licenses/doroti-linux/GPL-3.txt',
             'licenses/Doroti.Runtime/LICENSE', 'licenses/Doroti.Skia.Rendering/Flutter-LICENSE.md']:
    assert (out / 'publish' / name).is_file(), name
assert not (out / 'publish/licenses/nuget').exists()
assert not list((out / 'publish/licenses').rglob('SkiaSharp*.txt'))
assert not list((out / 'publish/licenses').rglob('Silk.NET-LICENSE.md'))
command('published-run', [str(out / 'publish/DorotiTemplateApp.Linux')], env | {'QT_QPA_PLATFORM': args.qpa,
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
command('reject-missing-prebuilt-no-build', ['dotnet', 'publish', str(project), '-c', 'Release', '--no-build',
    '-p:DorotiQtNativeBuildDirectory=' + str(out / 'missing-native'), '-o', str(out / 'publish-missing-native'), '-v:q'], env, consumer, success=False)
assert 'DOROTIQT006' in (out / 'reject-missing-prebuilt-no-build.log').read_text()
command('reject-trimming', ['dotnet', 'publish', str(project), '-c', 'Release', *props, '-p:PublishTrimmed=true', '-v:q'], env, consumer, success=False)
assert 'DOROTIQT001' in (out / 'reject-trimming.log').read_text()
report = {'version': version, 'providerVersion': provider_version, 'applicationId': application_id,
    'revision': subprocess.check_output(['git','rev-parse','HEAD'], cwd=ROOT, text=True).strip(),
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
command('installed-run', [str(install_root / 'run')], env | {'QT_QPA_PLATFORM': args.qpa,
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
if args.deb:
    tool_package = feed / ('Doroti.Tool.Qt.' + versions['Doroti.Tool.Qt'] + '.nupkg')
    with zipfile.ZipFile(tool_package) as archive:
        assert 'lib/net10.0/linux_deb.py' in archive.namelist()
        assert 'lib/net10.0/qt_runtime.py' in archive.namelist()
    cli = ROOT / 'Doroti/src/Doroti.Tooling/bin/Release/net10.0/Doroti.Tooling.dll'
    command('deb-publish', ['dotnet', cli, 'publish', '-App', consumer, '-Platform', 'linux',
                           '-PackageFormat', 'deb', '-Output', out / 'deb'], env, consumer)
    receipt_path = next((out / 'deb').glob('*/receipt.json'))
    receipt = json.loads(receipt_path.read_text())
    assert receipt['metadata']['selfContained'] and not receipt['metadata']['webView']
    command('deb-inspection', [sys.executable, ROOT / 'Doroti/tests/linux_deb_package.py',
                              '--receipt', receipt_path, '--output', out / 'deb-inspection'], env, consumer)
    report['deb'] = dict(packageOnly=True, receipt=str(receipt_path), install='not-run', gui='not-run')
    (out / 'candidate.json').write_text(json.dumps(report, indent=2))
print('PASS Linux Qt package-only Release candidate and portable install; unsigned and local OS only.', flush=True)
