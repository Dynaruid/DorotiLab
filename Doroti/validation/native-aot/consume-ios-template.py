#!/usr/bin/env python3
"""Create an isolated installed-template/NuGet consumer and publish its real iOS app."""
import argparse
import hashlib
import json
import os
import shutil
from pathlib import Path
import subprocess
import sys
import tempfile
from xml.sax.saxutils import escape
import zipfile

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--feed', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--resume', type=Path, help='Resume a generated consumer after a failed publish')
args = parser.parse_args()
output = args.output.resolve()
output.mkdir(parents=True, exist_ok=True)
feed = args.feed.resolve()
workspace = ROOT / '.doroti/tmp'
workspace.mkdir(parents=True, exist_ok=True)
consumer = args.resume.resolve() if args.resume else Path(tempfile.mkdtemp(prefix='nativeaot-consumer-', dir=workspace))
if not consumer.is_relative_to(workspace.resolve()) or not consumer.name.startswith('nativeaot-consumer-'):
    raise ValueError('Consumer must be an isolated generated directory under .doroti/tmp')
package_cache = output / 'nuget-cache/shared'
package_cache.mkdir(parents=True, exist_ok=True)
# Product packages retain their development version; always reload them from this feed.
# Third-party packages are immutable versioned dependencies and can be reused.
for cached in package_cache.glob('doroti.*'):
    if cached.is_dir(): shutil.rmtree(cached)
environment = dict(os.environ, NUGET_PACKAGES=str(package_cache))
steps = []

def run(name, command):
    log = output / (name + '.log')
    result = subprocess.run([sys.executable, str(HERE / 'run.py'), '--log', str(log), '--', *command],
                            cwd=consumer, env=environment, timeout=1230, check=False)
    steps.append({'name': name, 'exitCode': result.returncode, 'log': str(log)})
    if result.returncode:
        raise RuntimeError(f'{name} failed; see {log}')

passed = False
try:
    hive = str(output / 'template-hive' / consumer.name)
    if not args.resume:
        run('template-install', ['dotnet', 'new', 'install', str(feed / 'Doroti.Templates.0.2.0-beta.nupkg'), '--debug:custom-hive', hive])
        run('template-create', ['dotnet', 'new', 'doroti-app', '--debug:custom-hive', hive, '-n', 'AotConsumer', '-o', str(consumer), '--applicationId', 'dev.doroti.testbed'])
    (consumer / 'NuGet.Config').write_text(f'''<configuration>
  <packageSources><clear/><add key="candidate" value="{escape(str(feed))}"/><add key="nuget.org" value="https://api.nuget.org/v3/index.json"/></packageSources>
  <packageSourceMapping><packageSource key="candidate"><package pattern="Doroti.*"/></packageSource><packageSource key="nuget.org"><package pattern="*"/></packageSource></packageSourceMapping>
</configuration>''')
    source = consumer / 'src/App.cs'
    text = source.read_text().replace('            home: new CounterPage()', '            localizationsDelegates: [new ExternalDelegate()],\n            home: ConsumerExtensions.Create()')
    source.write_text(text)
    (consumer / 'src/ConsumerExtensions.cs').write_text('''using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
namespace AotConsumer;
public sealed record ExternalStrings(string Value);
public sealed class ExternalDelegate : LocalizationsDelegate<ExternalStrings>
{
    public override bool isSupported(Locale locale) => true;
    public override Future<ExternalStrings> load(Locale locale) => new SynchronousFuture<ExternalStrings>(new("external"));
    public override bool shouldReload(LocalizationsDelegate<ExternalStrings> old) => false;
}
public sealed class ExternalIntent : Intent { }
public sealed class ExternalAction : Doroti.Framework.Widgets.Action<ExternalIntent>
{
    public override object? invoke(ExternalIntent intent, BuildContext? context = null) => 42;
}
public sealed class ExternalGenericWidget<T>(T value) : StatefulWidget
{
    public T Value { get; } = value;
    public override IState createState() => new ExternalGenericState<T>();
}
public sealed class ExternalGenericState<T> : State<ExternalGenericWidget<T>>
{
    private bool reported;
    public override Widget build(BuildContext context)
    {
        if (Localizations.of<ExternalStrings>(context, typeof(ExternalStrings))?.Value != "external" || !Equals(Actions.invoke(context, new ExternalIntent()), 42))
            throw new System.Exception("External localization/action bridge failed");
        if (!reported) { reported = true; System.Console.WriteLine($"NATIVEAOT_CONSUMER external generic state/localization/action PASS value={widget.Value}"); }
        return new CounterPage();
    }
}
public static class ConsumerExtensions
{
    public static Widget Create() => new Actions(actions: new DartMap<System.Type, object> { [typeof(ExternalIntent)] = new ExternalAction() }, child: new ExternalGenericWidget<int>(7));
}
''')
    with zipfile.ZipFile(output / 'consumer-sources.zip', 'w', zipfile.ZIP_DEFLATED) as archive:
        for file in sorted(consumer.rglob('*')):
            if file.is_file() and not {'obj', 'bin', '.gradle'}.intersection(file.relative_to(consumer).parts):
                archive.write(file, file.relative_to(consumer))
    project = consumer / 'ios/AotConsumer.iOS.csproj'
    run('publish', ['dotnet', 'publish', str(project), '-c', 'Release', '-r', 'ios-arm64',
                    '-p:DorotiCompilationMode=NativeAot', '-p:ArtifactsPath=' + str(output / 'build'),
                    '-p:UseSharedCompilation=false', '-bl:' + str(output / 'publish.binlog'), '-v:diagnostic'])
    # App -> native binding/app references are expected; source product references are not.
    asset_files = list((output / 'build').rglob('project.assets.json')) + list(consumer.rglob('project.assets.json'))
    if not asset_files: raise RuntimeError('No dependency assets were found to verify package consumption')
    for file in asset_files:
        assets = json.loads(file.read_text())
        for name, value in assets.get('libraries', {}).items():
            if name.startswith('Doroti.') and value.get('type') == 'project':
                raise RuntimeError(f'Unexpected product ProjectReference: {name}')
    passed = True
finally:
    (output / 'result.json').write_text(json.dumps({'schemaVersion':'doroti.nativeaot-template-consumer/v1',
        'passed':passed, 'consumerSource':str(consumer), 'steps':steps,
        'packages':[{'path':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()} for p in sorted(feed.glob('*.nupkg'))],
        'installedOnDevice':False}, indent=2)+'\n')
