"""Actual Windows kind/owner/modal input/no-activation/native style and cleanup probe."""
from pathlib import Path
import subprocess,sys,os,json,uuid
ROOT=Path(__file__).resolve().parents[2]
run=ROOT/'temp/testing/platform-decoupling/window-kinds'/uuid.uuid4().hex;run.mkdir(parents=True)
app=ROOT/'samples/DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.dll'
env=os.environ|dict(DOROTI_WINDOW_KINDS_PROBE=str(run/'kinds.json'),DOROTI_SAMPLE='shared-tree',DOROTI_DESKTOP_SAMPLE='solid',DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1')
for key in ('DOROTI_MULTIWINDOW_PROBE','DOROTI_WINDOWS_APPSDK_REPORT','DOROTI_DESKTOP_PROBE','DOROTI_INPUT_PROBE'):env.pop(key,None)
with (run/'application.log').open('w',encoding='utf-8') as log:
    p=subprocess.Popen(['dotnet',str(app)],cwd=ROOT,env=env,stdout=log,stderr=subprocess.STDOUT)
    try:code=p.wait(timeout=120)
    finally:
        if p.poll() is None:p.kill();p.wait(timeout=10)
text=(run/'application.log').read_text(encoding='utf-8',errors='replace')
assert code==0 and not any(x in text for x in ('Unhandled exception','System.InvalidOperationException','System.ArgumentException','FlutterError','DorotiError')),text[-10000:]
probe=json.loads((run/'kinds.json').read_text());assert probe['result']=='PASS' and probe['remaining']==1
assert {x['kind'] for x in probe['windows']}=={'Regular','Dialog','Popup','Tooltip','Satellite'}
assert len({x['viewId'] for x in probe['windows']})==5 and text.count('complete=true pending=0')==6
report=dict(schema='doroti.windows-kinds-verification/v1',result='PASS',scope='actual HWND per kind, native owner, modal owner input disable/restore, popup native style, tooltip no-activate, unique view mappings and per-window cleanup completion',probe=probe,artifacts=str(run.relative_to(ROOT)),notVerified=['physical IME/focus restoration','mixed monitor DPI','nested modal reentrancy','design dialog/menu result bridge','OS menu interaction'])
(ROOT/'Doroti/docs/migrations/design-platform/windows-kinds-verification.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
print('PASS: actual Windows five kinds contract subset; '+str(run),flush=True)
