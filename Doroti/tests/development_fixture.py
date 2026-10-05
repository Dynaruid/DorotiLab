"""Generate the installed template, then bind it to current repository projects for development tests.

This is explicitly a source-tree fixture, not package-only release qualification.
"""
from pathlib import Path
import json
import re
import subprocess
import sys
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]

def create(destination, template_hive=None):
    destination = Path(destination).resolve()
    assert destination.is_relative_to(ROOT / 'temp/testing') and not destination.exists()
    hive_args = ['--debug:custom-hive', str(template_hive)] if template_hive else []
    if template_hive:
        subprocess.run(['dotnet', 'new', 'install', str(ROOT / 'Doroti/templates/Doroti.Templates/content/doroti-app'), *hive_args], check=True, timeout=60)
    subprocess.run(['dotnet', 'new', 'doroti-app', '-n', 'ReloadApp', '-o', str(destination), *hive_args], check=True, timeout=60)
    for relative in ['ReloadApp.csproj', 'desktop/ReloadApp.Desktop.csproj', 'windows/ReloadApp.Windows.csproj',
                     'windows/maui/ReloadApp.Windows.Maui.csproj', 'web/ReloadApp.Web.csproj', 'linux/ReloadApp.Linux.csproj']:
        file = destination / relative
        tree = ET.parse(file)
        project = tree.getroot()
        sdk = project.attrib.get('Sdk', '')
        if sdk.startswith('Doroti.App.Sdk'):
            del project.attrib['Sdk']
            sdk_root = ROOT / 'Doroti/src/Doroti.App.Sdk/Sdk'
            project.insert(0, ET.Element('Import', Project=str(sdk_root / 'Sdk.props')))
            ET.SubElement(project, 'Import', Project=str(sdk_root / 'Sdk.targets'))
        for item in list(project):
            if item.tag == 'Sdk' and item.attrib.get('Name') == 'Doroti.Runner.Sdk':
                project.remove(item)
                sdk_root = ROOT / 'Doroti/src/Doroti.Runner.Sdk/Sdk'
                project.insert(0, ET.Element('Import', Project=str(sdk_root / 'Sdk.props')))
                ET.SubElement(project, 'Import', Project=str(sdk_root / 'Sdk.targets'))
        for group in project.findall('ItemGroup'):
            for item in list(group):
                name = item.attrib.get('Include', '')
                if item.tag == 'PackageReference' and name.startswith('Doroti.'):
                    group.remove(item)
                    ET.SubElement(group, 'ProjectReference', Include=str(ROOT / f'Doroti/src/{name}/{name}.csproj'))
        group = ET.SubElement(project, 'PropertyGroup')
        ET.SubElement(group, 'DorotiUseRepositoryProjects').text = 'true'
        ET.SubElement(group, 'RestorePackagesWithLockFile').text = 'false'
        if relative.startswith('web/'):
            project.insert(0, ET.Element('Import', Project=str(ROOT / 'packages/platforms/web/Doroti.Target.Web.browser-wasm/build/Doroti.Target.Web.browser-wasm.props')))
        ET.indent(tree)
        tree.write(file, encoding='unicode')
    # Keep generated app/desktop/runner contract; replace only its demonstration page.
    app = destination / 'src/App.cs'
    text = app.read_text(encoding='utf-8-sig').replace('home: new CounterPage()', 'home: new MaterialSample.HotReloadSample()')
    app.write_text(text, encoding='utf-8')
    (destination / 'src/HotReloadSample.cs').write_text((ROOT / 'samples/DorotiTestbedApp/src/MaterialSample/HotReloadSample.cs').read_text(), encoding='utf-8')
    snippets = json.loads((ROOT / 'Doroti/tools/vscode-doroti/snippets/widgets.json').read_text())
    def snippet(name, class_name='SnippetWidget'):
        value = '\n'.join(snippets[name]['body'])
        value = re.sub(r'\$\{1(?::[^}]*)?\}', class_name, value) if name in ('StatelessWidget', 'StatefulWidget') else value
        value = re.sub(r'\$\{\d+:([^}]*)\}', lambda match: match[1], value)
        return re.sub(r'\$\d+|\$\{\d+\}', '', value)
    examples = ['using Doroti.Framework.Widgets;', 'using Doroti.Material;', 'using Doroti.Cupertino;', 'namespace SnippetCompilation;',
        snippet('StatelessWidget', 'SnippetStateless'), snippet('StatefulWidget', 'SnippetStateful'),
        'public sealed class OtherState : State<SnippetStateful> {', snippet('build'), snippet('initState'), snippet('dispose'),
        'private void Change() { ' + snippet('setState') + ' }', '}','public static class Expressions {']
    for index, name in enumerate(['Row', 'Column', 'Container', 'Text', 'Material app', 'Cupertino app']):
        examples.append(f'public static Widget Example{index}() => {snippet(name)};')
    examples.append('}')
    (destination / 'src/SnippetCompilation.cs').write_text('\n'.join(examples), encoding='utf-8')
    print(f'Generated source-tree development fixture: {destination}', flush=True)
    return destination

if __name__ == '__main__':
    create(sys.argv[1], Path(sys.argv[1]).parent / 'template-hive')
