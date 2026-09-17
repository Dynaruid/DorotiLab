"""Capture the actual WKWebView + Metal + UIKit blur at four strengths in both themes.
Run through validation/run-with-timeout.py. Requires Pillow and pymobiledevice3.
"""
import argparse
import json
from pathlib import Path
import subprocess
import shutil
import time
import uuid
from PIL import Image, ImageChops, ImageStat

parser = argparse.ArgumentParser()
connection = parser.add_mutually_exclusive_group(required=True)
connection.add_argument('--device')
connection.add_argument('--simulator')
parser.add_argument('--app', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--screenshot-tool', default='pymobiledevice3')
parser.add_argument('--check-resume', action='store_true', help='Simulator: background and resume the same scene during capture')
args = parser.parse_args()
if args.check_resume and not args.simulator:
    parser.error('--check-resume requires --simulator')
args.output.mkdir(parents=True, exist_ok=True)
commands = []

def run(command, label, check=True):
    start = time.monotonic()
    with (args.output/(label+'.log')).open('w') as log:
        result = subprocess.run(command, stdout=log, stderr=subprocess.STDOUT, timeout=1200)
    commands.append(dict(command=command, exit=result.returncode, seconds=time.monotonic()-start))
    if check and result.returncode:
        raise RuntimeError(label+' failed')
    return result.returncode

base = ['xcrun','devicectl','device']
name = 'appearance-'+uuid.uuid4().hex+'.txt'
container = None

def copy(suffix, destination):
    if args.simulator:
        source=container/'Documents'/(name+suffix)
        if not source.exists(): return 1
        shutil.copyfile(source,destination)
        return 0
    return run(base+['copy','from','--device',args.device,'--domain-type','appDataContainer',
               '--domain-identifier','dev.doroti.testbed','--source','Documents/'+name+suffix,
               '--destination',str(destination.resolve())], 'copy', check=False)

try:
    env = dict(DOROTI_TESTBED_MODE='platform-effects',DOROTI_UIKIT_EVIDENCE='1',
               DOROTI_UIKIT_EVIDENCE_NAME=name,DOROTI_UIKIT_EFFECT_APPEARANCE_CAPTURE='1')
    if args.simulator:
        run(['xcrun','simctl','boot',args.simulator],'boot',check=False)
        run(['xcrun','simctl','bootstatus',args.simulator,'-b'],'bootstatus')
        run(['xcrun','simctl','install',args.simulator,str(args.app.resolve())],'install')
        container=Path(subprocess.check_output(['xcrun','simctl','get_app_container',args.simulator,'dev.doroti.testbed','data'],timeout=60,text=True).strip())
        run(['env']+['SIMCTL_CHILD_'+k+'='+v for k,v in env.items()]+['xcrun','simctl','launch','--terminate-running-process',args.simulator,'dev.doroti.testbed'],'launch')
    else:
        run(base+['install','app','--device',args.device,str(args.app.resolve())], 'install')
        run(base+['process','launch','--device',args.device,'--terminate-existing','--environment-variables',json.dumps(env),'dev.doroti.testbed'], 'launch')
    phases = [f'{s:.3f}-{theme}' for s in [.25,.375,.75,1] for theme in ['Light','Dark']]
    for phase in phases:
        deadline = time.monotonic()+60
        ready = args.output/'capture.txt'
        while time.monotonic()<deadline:
            if copy('.capture', ready)==0 and ready.read_text()==phase:
                break
            if copy('',args.output/'probe.txt')==0:
                raise RuntimeError((args.output/'probe.txt').read_text())
            time.sleep(.2)
        else:
            raise TimeoutError('waiting for '+phase)
        assert copy('.geometry',args.output/'geometry.txt')==0
        assert copy('.window-'+phase+'.png',args.output/('window-'+phase+'.png'))==0
        if args.simulator:
            run(['xcrun','simctl','io',args.simulator,'screenshot',str(args.output/(phase+'.png'))],phase)
        else:
            run([args.screenshot_tool,'developer','dvt','screenshot','--native',str(args.output/(phase+'.png'))], phase)
        if args.check_resume and phase == '0.750-Dark':
            run(['xcrun','simctl','launch',args.simulator,'com.apple.Preferences'],'background')
            time.sleep(1)
            run(['xcrun','simctl','launch',args.simulator,'dev.doroti.testbed'],'resume')
            time.sleep(1)
            run(['xcrun','simctl','io',args.simulator,'screenshot',str(args.output/'resume.png')],'resume-capture')
        ack = args.output/'ack.txt'; ack.write_text(phase)
        if args.simulator:
            shutil.copyfile(ack,container/'Documents'/(name+'.ack-'+phase))
        else:
            run(base+['copy','to','--device',args.device,'--domain-type','appDataContainer',
                '--domain-identifier','dev.doroti.testbed','--source',str(ack.resolve()),
                '--destination','Documents/'+name+'.ack-'+phase], 'ack')
    deadline=time.monotonic()+30
    while time.monotonic()<deadline:
        if copy('',args.output/'probe.txt')==0:
            text=(args.output/'probe.txt').read_text()
            assert 'RESULT=PASS' in text, text
            break
        time.sleep(.5)
    else:
        raise TimeoutError('final probe')
    x,y,w,h,scale=map(float,(args.output/'geometry.txt').read_text().split(','))
    results=[]
    for strength in [.25,.375,.75,1]:
        a=Image.open(args.output/f'window-{strength:.3f}-Light.png').convert('RGB')
        b=Image.open(args.output/f'window-{strength:.3f}-Dark.png').convert('RGB')
        roi=(round(x*scale),round(y*scale),min(a.width,round((x+w)*scale)),min(a.height,round((y+h)*scale)))
        stat=ImageStat.Stat(ImageChops.difference(a.crop(roi),b.crop(roi)))
        results.append(dict(strength=strength,roi=roi,themeMeanAbsoluteRGB=stat.mean))
    changes=[]
    for start,end in zip([.25,.375,.75],[.375,.75,1]):
        a=Image.open(args.output/f'window-{start:.3f}-Light.png').convert('RGB').crop(roi)
        b=Image.open(args.output/f'window-{end:.3f}-Light.png').convert('RGB').crop(roi)
        changes.append(dict(fromStrength=start,toStrength=end,
                            meanAbsoluteRGB=ImageStat.Stat(ImageChops.difference(a,b)).mean))
    report=dict(scope='whole UIWindow DrawViewHierarchy capture, static WKWebView + Metal child, parent appearance Light vs Dark; DVT display captures collected separately',
                measurements=results,strengthChanges=changes)
    if args.check_resume:
        before=Image.open(args.output/'0.750-Dark.png').convert('RGB').crop(roi)
        after=Image.open(args.output/'resume.png').convert('RGB').crop(roi)
        report['resumeMeanAbsoluteRGB']=ImageStat.Stat(ImageChops.difference(before,after)).mean
    (args.output/'appearance.json').write_text(json.dumps(report,indent=2))
    assert all(max(r['themeMeanAbsoluteRGB'])<1 for r in results),report
    if args.check_resume:
        assert max(report['resumeMeanAbsoluteRGB'])<1,report
    # This detects stale captures, not Gaussian accuracy (measured separately
    # against reference patterns). At large sigmas this mostly-flat fixture's
    # changes are naturally below one RGB level and vary with screen geometry.
    assert all(max(r['meanAbsoluteRGB'])>.1 for r in changes),'Stale capture or ineffective strength change: '+str(changes)
    print(json.dumps(report,indent=2))
finally:
    (args.output/'commands.json').write_text(json.dumps(commands,indent=2))
