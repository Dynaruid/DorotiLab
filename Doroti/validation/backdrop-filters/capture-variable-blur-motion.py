import sys,time,json,threading,subprocess
from pathlib import Path
base=Path(sys.argv[2]).resolve() if len(sys.argv)>2 else Path(__file__).resolve().parents[2]/'artifacts/variable-blur-performance'
control=Path(__file__).with_name('variable-blur-window-control.ps1')
sys.path.insert(0,str(base/'capture-python'))
import dxcam
import numpy as np
name=sys.argv[1] if len(sys.argv)>1 else 'full'
camera=dxcam.create(output_color='RGB',max_buffer_len=8,processor_backend='numpy')
# A narrow content strip, excluding controls, cursor and taskbar. Capture both
# blur and clear rows; hashes count visual content changes, not repeated copies.
state=json.loads((base/'native-state.json').read_text(encoding='utf-8-sig'))
region=(state['originX']+40,state['originY']+572,state['originX']+340,state['originY']+1222)
camera.start(region=region,target_fps=240,video_mode=True)
rows=[]
errors=[]
start=time.perf_counter()
def stimulus():
    time.sleep(.5)
    try:
        subprocess.run(['powershell','-NoProfile','-ExecutionPolicy','Bypass','-File',str(control),'-ArtifactDirectory',str(base),
            '-Action','scroll','-X','1000','-Y','1000','-Delta','-8','-Count','120','-IntervalMs','16','-Name',name+'-after'],stdout=subprocess.DEVNULL,check=True,timeout=15)
    except Exception as error:
        errors.append(error)
thread=threading.Thread(target=stimulus)
thread.start()
last=None
try:
    while time.perf_counter()-start<5:
        frame,timestamp=camera.get_latest_frame(with_timestamp=True)
        # Threshold tiny color noise. Preserve a sample for visual verification.
        small=frame[::3,::3].copy()
        changed=last is None or np.count_nonzero(np.max(np.abs(small.astype(np.int16)-last.astype(np.int16)),axis=2)>5)>30
        rows.append(dict(t=time.perf_counter()-start,desktopTimestamp=timestamp,changed=bool(changed)))
        last=small
finally:
    camera.stop();camera.release();thread.join()
if errors:
    raise RuntimeError('Native input failed') from errors[0]
changed=[r['t'] for r in rows if r['changed']]
inputs=json.loads((base/(name+'-after-input.json')).read_text(encoding='utf-8-sig'))
activeStart=inputs[0]-start+.1
activeEnd=inputs[-1]-start+.1
if activeEnd>rows[-1]['t']:
    raise RuntimeError('Capture ended before the measured wheel stream')
changed=[t for t in changed if activeStart<=t<=activeEnd]
gaps=np.diff(changed)
report=dict(mode=name,captureSamples=len(rows),uniqueChanges=len(changed),inputMedianIntervalMs=float(np.median(np.diff(inputs))*1000),activeStart=activeStart,activeEnd=activeEnd,
    activeSpan=(changed[-1]-changed[0]) if len(changed)>1 else 0,
    changeRate=(len(changed)-1)/(changed[-1]-changed[0]) if len(changed)>1 else 0,
    medianChangeIntervalMs=float(np.median(gaps)*1000) if len(gaps) else None,
    p95ChangeIntervalMs=float(np.quantile(gaps,.95)*1000) if len(gaps) else None,
    note='DXGI desktop visual changes during a 120-event wheel stream at measured ~16 ms intervals. 100 ms onset allowance; cursor/controls excluded. Sampling can miss updates; not scan-out or application FPS.',samples=rows)
(base/(name+'-motion.json')).write_text(json.dumps(report,indent=2))
print(json.dumps({k:v for k,v in report.items() if k!='samples'}))
