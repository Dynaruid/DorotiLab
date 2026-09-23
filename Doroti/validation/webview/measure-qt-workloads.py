"""Bounded 0/1/4-view observations. Does not qualify physical GPU or input latency."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import time
p=argparse.ArgumentParser()
p.add_argument('--app',type=Path,required=True)
p.add_argument('--driver',type=Path,required=True)
p.add_argument('--output',type=Path,required=True)
p.add_argument('--qpa',choices=['xcb','wayland'],required=True)
p.add_argument('--counts',default='0,1,4',help='Comma-separated subset of 0,1,4')
p.add_argument('--modes',default='idle,animation,scroll,modal',help='Comma-separated workload subset')
a=p.parse_args();out=a.output.resolve();out.mkdir(parents=True,exist_ok=False)
counts=tuple(int(v) for v in a.counts.split(','))
modes=tuple(a.modes.split(','))
if not counts or any(v not in (0,1,4) for v in counts) or not modes or any(v not in ('idle','animation','scroll','modal') for v in modes):
    p.error('invalid --counts or --modes')
def memory(pid):
    seen=set();total=0;pending=[pid];unreadable=[];webengine=0;cpu_ticks=0
    while pending:
        current=pending.pop()
        if current in seen:continue
        seen.add(current)
        try:
            for line in Path(f'/proc/{current}/smaps_rollup').read_text().splitlines():
                if line.startswith('Pss:'):total+=int(line.split()[1])*1024
        except (OSError,ValueError):unreadable.append(current)
        try:
            command=(Path(f'/proc/{current}/cmdline').read_bytes()).replace(b'\x00',b' ')
            if b'QtWebEngineProcess' in command:webengine+=1
            stat=Path(f'/proc/{current}/stat').read_text()
            fields=stat[stat.rfind(')')+2:].split()
            cpu_ticks+=int(fields[11])+int(fields[12])
        except (OSError,ValueError):pass
        try:
            for task in Path(f'/proc/{current}/task').iterdir():
                pending.extend(int(v) for v in (task/'children').read_text().split())
        except (OSError,ValueError):pass
    return dict(observableTreePssBytes=total,processCount=len(seen),
                unreadablePssCount=len(unreadable),webEngineProcesses=webengine,
                cpuTicks=cpu_ticks,timestamp=time.monotonic())
reports=[]
for count in counts:
    for mode in modes:
        directory=out/f'{count}-{mode}';directory.mkdir()
        script=directory/'script.json'
        script.write_text(json.dumps([dict(action='wait',wait=2000),dict(action='capture',path=str(directory/'visible')),
            dict(action='measure',duration=10000,path=str(directory/'timings.json'))]))
        env=dict(os.environ,QT_QPA_PLATFORM=a.qpa,DOROTI_TESTBED_MODE='webview-workload',DOROTI_WEBVIEW_COUNT=str(count),
            DOROTI_WEBVIEW_WORKLOAD=mode,DOROTI_QT_DIAGNOSTICS='1',DOROTI_QT_TEST_SCRIPT=str(script),LD_PRELOAD=str(a.driver.resolve()),
            XDG_DATA_HOME=str(directory/'data'),XDG_CACHE_HOME=str(directory/'cache'))
        with (directory/'product.log').open('w') as log:
            process=subprocess.Popen(['dotnet',str(a.app.resolve())],env=env,stdout=log,stderr=log)
            samples=[];start=time.monotonic()
            try:
                while process.poll() is None:
                    if time.monotonic()-start>90:raise TimeoutError('workload timeout')
                    if time.monotonic()-start>4:samples.append(memory(process.pid))
                    time.sleep(.5)
            finally:
                if process.poll() is None:process.kill();process.wait(timeout=10)
        report=dict(count=count,mode=mode,qpa=a.qpa,exit=process.returncode)
        if (directory/'timings.json').exists():report.update(json.loads((directory/'timings.json').read_text()))
        if report.get('samples') == 0:
            for key in ('p50Ms','p95Ms','p99Ms'):report[key]=None
        if samples:
            report['peakObservableTreePssBytes']=max(s['observableTreePssBytes'] for s in samples)
            report['maxProcesses']=max(s['processCount'] for s in samples)
            report['maxWebEngineProcesses']=max(s['webEngineProcesses'] for s in samples)
            report['maxUnreadablePssCount']=max(s['unreadablePssCount'] for s in samples)
            report['memorySamples']=len(samples)
            if mode=='idle' and len(samples)>1:
                elapsed=samples[-1]['timestamp']-samples[0]['timestamp']
                ticks=samples[-1]['cpuTicks']-samples[0]['cpuTicks']
                report['idleCpuPercent']=max(0,100*ticks/(os.sysconf('SC_CLK_TCK')*elapsed)) if elapsed>0 else None
        if (directory/'visible.json').exists():
            capture=json.loads((directory/'visible.json').read_text());report['visibleNative']=sum(c['visible'] for c in capture['controls'])
        for line in (directory/'product.log').read_text().splitlines():
            if line.startswith('doroti.qt.summary='):report['host']=json.loads(line.split('=',1)[1])
        report['status']='observed' if process.returncode==0 and report.get('visibleNative')==count else 'FAIL'
        reports.append(report);print(json.dumps({k:v for k,v in report.items() if k!='host'}),flush=True)
        (out/'result.json').write_text(json.dumps(dict(scope='VM observation; no baseline, scanout, first-content or input latency approval',
            appSha256=hashlib.sha256(a.app.read_bytes()).hexdigest(),runs=reports),indent=2)+'\n')
raise SystemExit(1 if any(r['status']=='FAIL' for r in reports) else 0)
