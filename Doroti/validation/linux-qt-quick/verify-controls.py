"""Mounted Material Quick Controls: all ten cases and ten disposal/remount cycles."""
import argparse
import json
import os
from pathlib import Path
import subprocess

p=argparse.ArgumentParser()
p.add_argument('--app',type=Path,required=True)
p.add_argument('--driver',type=Path,required=True)
p.add_argument('--output',type=Path,required=True)
p.add_argument('--qpa',default='wayland',choices=['wayland','xcb'])
p.add_argument('--cycles',type=int,default=10)
args=p.parse_args()
if not 1 <= args.cycles <= 10:
    p.error('--cycles must be between 1 and 10')
out=args.output.resolve();out.mkdir(parents=True,exist_ok=True)
steps=[dict(action='wait',wait=1200)]
def click(x,y): steps.append(dict(action='click',x=x,y=y,wait=220))
def capture(name, expected=2): steps.append(dict(action='capture',path=str(out/name),wait=80,expectedNative=expected))
for case in range(10):
    capture('case-'+str((5+case)%10));click(265,104)
for cycle in range(args.cycles):
    click(403,104);capture(f'disposed-{cycle}',0)
    click(403,104);capture(f'created-{cycle}')
script=out/'script.json';script.write_text(json.dumps(steps))
env=dict(os.environ,QT_QPA_PLATFORM=args.qpa,DOROTI_TESTBED_MODE='platform-views',
         DOROTI_PLATFORM_VIEW_COMPOSITION='interleaved',
         DOROTI_QT_TEST_SCRIPT=str(script),LD_PRELOAD=str(args.driver.resolve()))
with (out/'product.log').open('w') as log:
    run=subprocess.run(['dotnet',str(args.app.resolve())],env=env,stdout=log,stderr=subprocess.STDOUT,timeout=300)
def read(name): return json.loads((out/(name+'.json')).read_text())
checks={'exit':run.returncode==0}
try:
    generations=[]
    for cycle in range(args.cycles):
        assert not read(f'disposed-{cycle}')['controls'],f'controls remain after disposal {cycle}'
        ids={c['id'] for c in read(f'created-{cycle}')['controls']}
        assert len(ids)==2,f'missing native controls at cycle {cycle}'
        assert not any(ids & old for old in generations),f'old native identity reused {cycle}'
        generations.append(ids)
    checks['productLifecycles']=True
    checks['tenCaseCaptures']=all(read('case-'+str(i))['windowCapture'] for i in range(10))
except (AssertionError,OSError,ValueError,KeyError) as error:
    checks['completeEvidence']=False;checks['error']=str(error)
log=(out/'product.log').read_text()
checks['noManagedFatal']='managed.fatal=' not in log and 'Unhandled exception' not in log
checks['noVulkanErrors']='VUID-' not in log and 'Validation Error' not in log
result=dict(qpa=args.qpa,exit=run.returncode,cycles=args.cycles,checks=checks,input='automated Qt product ingress',
            coverage='Ten captures are for visual review; lifecycle assertions are automated.',
            physical='notVerified',performance='notQualified')
(out/'result.json').write_text(json.dumps(result,indent=2));print(json.dumps(result,indent=2))
raise SystemExit(0 if all(v is True for v in checks.values()) else 1)
