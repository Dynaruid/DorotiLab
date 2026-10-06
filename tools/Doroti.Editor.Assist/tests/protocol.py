"""Persistent process, evaluated projects and unsaved snapshot regression. Run with the repository 1200s wrapper."""
import json
import pathlib
import queue
import subprocess
import sys
import threading
import time
import uuid

ROOT = pathlib.Path(__file__).resolve().parents[3]
destination = pathlib.Path(sys.argv[1]).resolve()
destination.mkdir(parents=True, exist_ok=True)
(destination / 'src').mkdir(exist_ok=True)
libraries = ROOT / 'tools/Doroti.Editor.Assist/tests/bin/Release/net10.0'
references = '\n'.join(f'<Reference Include="{p.stem}"><HintPath>{p}</HintPath></Reference>' for p in libraries.glob('Doroti.*.dll') if not p.name.startswith('Doroti.Editor.'))
project = destination / 'Fixture.csproj'
project.write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><LangVersion>14.0</LangVersion><Nullable>enable</Nullable><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup><Compile Include="src/**/*.cs"/>{references}</ItemGroup></Project>')
file = destination / 'src/한글 #Probe.cs'
source = 'using Doroti.Framework.Widgets;\nnamespace Probe;\npublic sealed class A : StatelessWidget { public override Widget build(BuildContext context) => new Text("한글😀"); }'
file.write_text(source, encoding='utf-8')
other = destination / 'src/Other.cs'
other_source = 'using Doroti.Framework.Widgets; namespace Probe; public class SavedWidget : StatelessWidget { public override Widget build(BuildContext c) => new SizedBox(); }'
other.write_text(other_source, encoding='utf-8')
lines = queue.Queue()
stderr = (destination / 'helper.stderr.log').open('w', encoding='utf-8')
process = subprocess.Popen(['dotnet', str(ROOT / 'tools/vscode-doroti/helper/Doroti.Editor.Assist.dll')], stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=stderr, text=True, encoding='utf-8')
threading.Thread(target=lambda: [lines.put(line) for line in process.stdout], daemon=True).start()
def send(request):
    process.stdin.write(json.dumps(request, ensure_ascii=False) + '\n'); process.stdin.flush()
def reply():
    value = json.loads(lines.get(timeout=120))
    assert value['schemaVersion'] == 'doroti.editor/v1'
    return value
def request(operation, snapshots=None, text=source, **extra):
    message = dict(id=str(uuid.uuid4()), operation=operation, project=str(project), uri=file.as_uri(), version=1, offset=text.index('new Text') + 5, snapshots=snapshots or [dict(uri=file.as_uri(), version=1, text=text)])
    message.update(extra); send(message); result = reply(); assert result['id'] == message['id']; return result
passed = []
try:
    send(dict(id='hello', operation='hello')); assert reply()['result']['roslyn'] == '5.9.0'; passed.append('handshake')
    print('PASS: handshake', flush=True)
    result = request('analyze')['result']; assert result['semantic'] and result['included'] and any(a['id'] == 'wrap:Center' for a in result['actions']); passed.append('Unicode/hash URI and semantic Widget identity')
    print('PASS: semantic identity', flush=True)
    changed = other_source.replace('SavedWidget', 'UnsavedWidget')
    catalog = request('catalog', [dict(uri=file.as_uri(), version=1, text=source), dict(uri=other.as_uri(), version=2, text=changed)])['result']['types']
    assert any(t['name'] == 'UnsavedWidget' for t in catalog); passed.append('unsaved cross-document type catalog')
    print('PASS: unsaved catalog', flush=True)
    catalog = request('catalog')['result']['types']; assert any(t['name'] == 'SavedWidget' for t in catalog) and not any(t['name'] == 'UnsavedWidget' for t in catalog); passed.append('discarded snapshot restored from disk')
    print('PASS: discarded snapshot', flush=True)
    for _ in range(20): assert request('analyze')['result']['semantic']
    stats = request('stats')['result']; assert stats['projectLoads'] == 1 and stats['cachedProjects'] == 1; passed.append('20 requests keep one helper/project load')
    print('PASS: cached project', flush=True)
    result = request('analyze', [dict(uri=file.as_uri(), version=1, text=source)] * 33); assert 'Snapshot limit' in result['error']; passed.append('bounded snapshots')
    # Cancellation is read separately from the serial worker. A canceled queued analysis cannot produce an edit.
    cancel_id = str(uuid.uuid4())
    send(dict(id=cancel_id, operation='catalog', project=str(project), uri=file.as_uri(), version=1, offset=0, snapshots=[dict(uri=file.as_uri(), version=1, text=source)]))
    send(dict(id=cancel_id, operation='cancel'))
    canceled = reply(); assert canceled['id'] == cancel_id
    assert canceled.get('error') == 'Canceled' or 'edits' not in canceled.get('result', {}); passed.append('cancel never applies edits')
    broken = destination / 'Broken.csproj'; broken.write_text('<Project Sdk="Microsoft.NET.Sdk"><Import Project="missing.props"/></Project>')
    result = request('analyze', project=str(broken))['result']; assert not result['semantic'] and result['actions'] == [] and result['reason'] and result['context'] == 'fallback'; passed.append('failed evaluation gives safe syntax fallback')
    (destination / 'result.json').write_text(json.dumps(dict(status='PASS', checks=passed, stats=stats), indent=2), encoding='utf-8')
    print(json.dumps(dict(status='PASS', checks=passed, stats=stats)))
finally:
    process.stdin.close()
    try: process.wait(timeout=15)
    except subprocess.TimeoutExpired: process.kill(); process.wait()
    stderr.close()
