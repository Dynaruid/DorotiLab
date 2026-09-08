"""Run one work3 validation with a 20 minute process-tree deadline, no retries."""
import argparse
import json
import hashlib
import os
from pathlib import Path
import subprocess
import time

p = argparse.ArgumentParser()
p.add_argument('label')
p.add_argument('--stage', required=True)
p.add_argument('--batch', type=int, default=1)
p.add_argument('--cwd', default='.')
p.add_argument('command', nargs=argparse.REMAINDER)
a = p.parse_args()
root = Path(__file__).resolve().parents[2]
out = root / '.doroti/work3/evidence'
out.mkdir(parents=True, exist_ok=True)
command = a.command[1:] if a.command[:1] == ['--'] else a.command
if not command or not a.label.replace('-', '').replace('_', '').isalnum():
    p.error('label and command required')
record = out / (a.label + '.json')
if record.exists():
    raise SystemExit('Existing run labels cannot be reused')
patch = subprocess.check_output(['git', 'diff', '--binary', 'HEAD'], cwd=root)
(out / (a.label + '.patch')).write_bytes(patch)
untracked = subprocess.check_output(['git', 'ls-files', '--others', '--exclude-standard'], cwd=root, text=True).splitlines()
sources = {}
for name in untracked:
    path = root / name
    if path.is_file() and path.suffix in ('.cs','.ts','.mjs','.py','.md','.props','.targets','.json'):
        target = out / (a.label + '-sources') / name
        target.parent.mkdir(parents=True, exist_ok=True)
        data = path.read_bytes()
        target.write_bytes(data)
        sources[name] = hashlib.sha256(data).hexdigest()
row = dict(label=a.label, stage=a.stage, batch=a.batch, command=command, cwd=a.cwd,
           sourcePatchSha256=hashlib.sha256(patch).hexdigest(), untrackedSourceHashes=sources,
           environment={key:os.environ[key] for key in ['DOROTI_WEB_BASE_URL','DOROTI_WEB_ARTIFACT_LABEL','DOROTI_TESTBED_MODE','DOROTI_WINDOWS_APPSDK_SMOKE_MS','DOROTI_BROWSER_CHANNEL','DOROTI_WEB_RENDERER_MODE','DOROTI_PERF_QUERY'] if key in os.environ},
           timeoutSeconds=1200, retries=0, started=time.time(),
           head=subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=root, text=True).strip(),
           dirty=subprocess.check_output(['git', 'status', '--porcelain'], cwd=root, text=True))
record.write_text(json.dumps(row, indent=2), encoding='utf-8')
start = time.monotonic()
with (out / (a.label + '.log')).open('w', encoding='utf-8') as log:
    try:
        process = subprocess.Popen(command, cwd=root / a.cwd, stdout=log, stderr=subprocess.STDOUT)
        try:
            row['exitCode'] = process.wait(timeout=1200)
        except subprocess.TimeoutExpired:
            subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'], stdout=log, stderr=log)
            row['exitCode'] = 124
    except Exception as error:
        row['error'] = str(error)
        row['exitCode'] = 1
row['elapsedSeconds'] = round(time.monotonic() - start, 3)
record.write_text(json.dumps(row, indent=2), encoding='utf-8')
print(json.dumps(row, indent=2))
print((out / (a.label + '.log')).read_text(encoding='utf-8', errors='replace')[-6000:])
raise SystemExit(row['exitCode'])
