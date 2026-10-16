"""Isolate an Apple source workspace and declare its test TFM/RID in workspace v2."""
import argparse
import json
import os
from pathlib import Path
import re
import shutil
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]


def create(destination, platform, framework=None, rid=None, sdk_version=None, source=None):
    source = Path(source or ROOT / 'samples/DorotiTestbedApp').resolve()
    destination = Path(destination).resolve()
    if not source.is_relative_to(ROOT) or not destination.is_relative_to(ROOT / 'temp/testing') or destination.exists() or destination.is_relative_to(source):
        raise ValueError('Use a fresh fixture directory under temp/testing, outside the source app.')
    manifest = json.loads((source / 'doroti-workspace.json').read_text(encoding='utf-8-sig'))
    selected = manifest['platforms'][platform]
    framework = framework or selected['targetFramework']
    rid = rid or selected['runtimeIdentifier']
    shutil.copytree(source, destination,
                    ignore=shutil.ignore_patterns('bin', 'obj', '.doroti', 'node_modules', '.git'))

    def relocate(value, original):
        parts = []
        for part in value.split(';'):
            expanded = part.replace('$(MSBuildProjectDirectory)', str(original.parent))
            expanded = expanded.replace('$(MSBuildThisFileDirectory)', str(original.parent) + os.sep)
            if '$(' not in expanded and (part.startswith(('..', './', '.\\')) or expanded != part):
                resolved = (original.parent / expanded.replace('\\', '/')).resolve()
                if resolved.is_relative_to(ROOT) and not resolved.is_relative_to(source):
                    part = resolved.as_posix()
            parts.append(part)
        return ';'.join(parts)

    for path in destination.rglob('*'):
        if path.suffix not in ('.csproj', '.props', '.targets') or path.is_symlink():
            continue
        original = source / path.relative_to(destination)
        tree = ET.parse(path)
        for node in tree.iter():
            for key in ('Project', 'Include', 'Update', 'Remove'):
                if key in node.attrib:
                    node.set(key, relocate(node.get(key), original))
            if node.text and node.text.strip() == node.text:
                node.text = relocate(node.text, original)
        ET.indent(tree)
        tree.write(path, encoding='unicode')
    for alias, entry in manifest['platforms'].items():
        if not entry['providerManifest'].startswith('nuget:'):
            entry['providerManifest'] = os.path.relpath(source / entry['providerManifest'], destination).replace('\\', '/')
    selected['targetFramework'] = framework
    selected['runtimeIdentifier'] = rid
    selected['profile'] = 'default:' + rid
    if platform == 'ios':
        selected['targetPackage'] = 'Doroti.Target.iOS.Maui.' + rid
    runner = destination / selected['runner']
    tree = ET.parse(runner)
    props = ET.Element('PropertyGroup')
    property_name = {'ios': 'DorotiIosTargetFramework', 'macos': 'DorotiMacOSTargetFramework',
                     'maccatalyst': 'DorotiMacCatalystTargetFramework'}[platform]
    ET.SubElement(props, property_name).text = framework
    for group in tree.getroot().findall('PropertyGroup'):
        for old in group.findall('RuntimeIdentifier'):
            group.remove(old)
    ET.SubElement(props, 'RuntimeIdentifier').text = rid
    if platform == 'ios' and framework.startswith('net11.'):
        ET.SubElement(props, 'DorotiIosDevelopment').text = 'true'
        ET.SubElement(props, 'DorotiCompilationMode').text = 'CoreClr'
    tree.getroot().insert(0, props)
    ET.indent(tree)
    tree.write(runner, encoding='unicode')
    (destination / 'doroti-workspace.json').write_text(json.dumps(manifest, indent=2) + '\n', encoding='utf-8')
    sdk_file = source / 'ios/global.json' if framework.startswith('net11.') else ROOT / 'global.json'
    sdk = json.loads(sdk_file.read_text())
    if sdk_version:
        if not re.fullmatch(r'\d+\.\d+\.\d+(?:-[A-Za-z0-9.-]+)?', sdk_version):
            raise ValueError('Invalid fixture SDK version')
        sdk['sdk'].update(version=sdk_version, rollForward='disable', allowPrerelease=True)
    (runner.parent / 'global.json').write_text(json.dumps(sdk, indent=2) + '\n', encoding='utf-8')
    return destination


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--source', type=Path)
    parser.add_argument('--platform', choices=['ios', 'macos', 'maccatalyst'], required=True)
    parser.add_argument('--framework')
    parser.add_argument('--rid')
    parser.add_argument('--sdk-version')
    args = parser.parse_args()
    print(create(args.output, args.platform, args.framework, args.rid, args.sdk_version, args.source))
