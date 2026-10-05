"""Evaluate Qt runner profiles and exercise native copying without a display.

The copy fixtures deliberately contain equal-size, equal-mtime different bytes.
They detect stale payloads, option contamination and incomplete no-build publish.
"""
from pathlib import Path
import argparse
import json
import os
import subprocess
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--output', required=True, type=Path)
args = parser.parse_args()
out = args.output.resolve()
assert out.is_relative_to(ROOT / 'temp/testing')
out.mkdir(parents=True, exist_ok=False)
sdk = ROOT / 'Doroti/src/Doroti.Runner.Sdk/Sdk'
fixture = out / 'Copy.proj'
project = ET.Element('Project')
properties = ET.SubElement(project, 'PropertyGroup')
for name, value in {
    'DorotiHostKind': 'Qt', 'Configuration': 'Debug',
    'BaseIntermediateOutputPath': str(out / 'obj') + '/',
    'TargetDir': str(out / 'bin') + '/', 'PublishDir': str(out / 'publish') + '/',
}.items():
    ET.SubElement(properties, name).text = value
ET.SubElement(project, 'Import', Project=str(ROOT / 'packages/platforms/build/Doroti.Qt.targets'))
ET.ElementTree(project).write(fixture, encoding='utf-8', xml_declaration=True)

def run(name, project=fixture, switches=(), target=None, query=None, error=None):
    command = ['dotnet', 'msbuild', str(project), '-nologo', *switches]
    if target: command.append('-t:' + target)
    if query: command.append('-getProperty:' + query)
    result = subprocess.run(command, cwd=ROOT, capture_output=True, text=True, timeout=60)
    (out / (name + '.log')).write_text(result.stdout + result.stderr)
    if error:
        assert result.returncode != 0 and error in result.stdout, (name, result.stdout, result.stderr)
    else:
        assert result.returncode == 0, (name, result.stdout, result.stderr)
    print('PASS', name, flush=True)
    return json.loads(result.stdout)['Properties'] if query else result.stdout

query = 'DorotiQtQuick,DorotiQtGraphite,DorotiQtWebEngine,DorotiQtNativeBuildDirectory'
defaults = run('sdk-defaults', query=query)
assert defaults['DorotiQtQuick'] == defaults['DorotiQtGraphite'] == 'true'
assert defaults['DorotiQtWebEngine'] == 'false'
profiles = [defaults]
for name, switches in [
    ('webengine', ['-p:DorotiQtWebEngine=true']),
    ('widgets-vulkan', ['-p:DorotiQtQuick=false']),
    ('widgets-opengl', ['-p:DorotiQtQuick=false', '-p:DorotiQtGraphite=false']),
    ('gstreamer', ['-p:DorotiGStreamerTextures=true']),
    ('release', ['-p:Configuration=Release']),
]:
    profiles.append(run(name, switches=switches, target='ValidateDorotiQtConfiguration', query=query))
assert len({profile['DorotiQtNativeBuildDirectory'] for profile in profiles}) == len(profiles)
for name in ['DorotiSampleApp2', 'DorotiTestbedApp']:
    profile = run(name, ROOT / f'samples/{name}/linux/{name}.Linux.csproj', query=query)
    assert profile['DorotiQtQuick'] == profile['DorotiQtGraphite'] == profile['DorotiQtWebEngine'] == 'true'

runner = ROOT / 'samples/DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj'
development = run('development-profile', runner, switches=['-p:DorotiQtDevelopment=true'],
    target='ValidateDorotiQtDevelopment', query='Optimize,DebugType,StartupHookSupport')
assert development == {'Optimize': 'false', 'DebugType': 'portable', 'StartupHookSupport': 'true'}
for option in ['Configuration=Release', 'PublishAot=true', 'PublishTrimmed=true',
               'PublishSingleFile=true', 'Optimize=true', 'StartupHookSupport=false', 'DebugType=none']:
    run('reject-development-' + option.split('=')[0], runner,
        switches=['-p:DorotiQtDevelopment=true', '-p:' + option],
        target='ValidateDorotiQtDevelopment', error='DOROTIQT007')

# Provider-owned typed operation plans; arbitrary-alias installed discovery and
# actual CLI execution are qualified by platform_provider_contract.py.
subprocess.run(['dotnet', 'run', '--project', str(ROOT / 'packages/platforms/qt/tests/Doroti.Tool.Qt.Tests/Doroti.Tool.Qt.Tests.csproj'),
    '--artifacts-path', str(out / 'tool-build')], cwd=ROOT, check=True, timeout=1200)

# Evaluate the actual template against the source SDK; package qualification
# separately restores this same project against the freshly packed SDK.
template = ROOT / 'Doroti/templates/Doroti.Templates/content/doroti-app/linux/DorotiTemplateApp.Linux.csproj'
tree = ET.parse(template)
root = tree.getroot()
root.remove(root.find('Sdk'))
root.insert(0, ET.Element('Import', Project=str(sdk / 'Sdk.props')))
settings = ET.SubElement(root, 'PropertyGroup')
ET.SubElement(settings, 'DorotiProviderBootstrapTargets').text = str(ROOT / 'packages/platforms/qt/bootstrap/Doroti.Provider.Bootstrap.targets')
root.append(ET.Element('Import', Project=str(sdk / 'Sdk.targets')))
template_fixture = out / 'Template.csproj'
tree.write(template_fixture, encoding='utf-8', xml_declaration=True)
profile = run('template-defaults', template_fixture, query=query)
assert profile['DorotiQtQuick'] == profile['DorotiQtGraphite'] == 'true'
assert profile['DorotiQtWebEngine'] == 'false'

for name, switches, error in [
    ('quick-without-graphite', ['-p:DorotiQtGraphite=false'], 'DOROTIQT003'),
    ('webengine-without-quick', ['-p:DorotiQtQuick=false', '-p:DorotiQtWebEngine=true'], 'DOROTIQT004'),
    ('desktop-without-quick', ['-p:DorotiQtQuick=false', '-p:DorotiLinuxDesktop=true'], 'DOROTIQT005'),
]:
    run(name, switches=switches, target='ValidateDorotiQtConfiguration', error=error)
for option in ['DorotiQtQuick', 'DorotiQtGraphite', 'DorotiQtWebEngine', 'DorotiGStreamerTextures', 'DorotiBuildQtNative']:
    run('invalid-' + option, switches=['-p:' + option + '=treu'], target='ValidateDorotiQtConfiguration', error='DOROTIQT002')
run('missing-native', target='CopyDorotiQtNative', error='DOROTIQT006')

native = Path(defaults['DorotiQtNativeBuildDirectory'])
native.mkdir(parents=True)
library = native / 'libdoroti_qt_host.so'
library.write_bytes(b'new-library')
optional = ['libdoroti_texture_gstreamer.so', 'libdoroti_webview_qt.so', 'doroti-webview-runtime.json']
for directory in [out / 'bin', out / 'publish']:
    directory.mkdir()
    destination = directory / library.name
    destination.write_bytes(b'old-library')
    os.utime(destination, ns=(library.stat().st_atime_ns, library.stat().st_mtime_ns))
    for name in optional: (directory / name).write_bytes(b'stale')
run('replace-equal-metadata-and-remove-disabled', target='CopyDorotiQtNative')
for directory in [out / 'bin', out / 'publish']:
    assert (directory / library.name).read_bytes() == b'new-library'
    assert all(not (directory / name).exists() for name in optional)
# A cache for another profile is present, but cannot satisfy this profile.
run('reject-other-profile-cache', switches=['-p:DorotiQtWebEngine=true'], target='CopyDorotiQtNative', error='DOROTIQT006')
for name in optional: (native / name).write_bytes(name.encode())
switches = ['-p:DorotiQtWebEngine=true', '-p:DorotiGStreamerTextures=true', '-p:DorotiBuildQtNative=false',
            '-p:DorotiQtNativeBuildDirectory=' + str(native)]
run('explicit-prebuilt-copy', switches=switches, target='CopyDorotiQtNative')
for directory in [out / 'bin', out / 'publish']:
    assert all((directory / name).read_bytes() == (native / name).read_bytes() for name in optional)
(native / 'libdoroti_webview_qt.so').unlink()
run('reject-missing-enabled-shim', switches=switches, target='CopyDorotiQtNative', error='MSB3030')

provider_source = ROOT / 'packages/platforms/qt/native'
assert (provider_source / 'CMakeLists.txt').is_file()
for removed in [ROOT / 'samples/DorotiSampleApp2/linux/native', ROOT / 'samples/DorotiTestbedApp/linux/native', template.parent / 'native']:
    assert not removed.exists(), 'Duplicate framework native source: ' + str(removed)
print('PASS canonical provider native ownership; Qt build profiles, typed development plans and payload copying.', flush=True)
