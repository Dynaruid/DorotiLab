"""Actual owned OS menu creation, strict owner, cancellation/dismissal and native cleanup."""
from pathlib import Path
import subprocess,sys,os,json,uuid
ROOT=Path(__file__).resolve().parents[2]
run=ROOT/'temp/testing/platform-decoupling/native-menu'/uuid.uuid4().hex;run.mkdir(parents=True)
app=ROOT/'samples/DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.dll'
env=os.environ|dict(DOROTI_NATIVE_MENU_PROBE=str(run/'menu.json'),DOROTI_SAMPLE='shared-tree',DOROTI_DESKTOP_SAMPLE='solid',DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1')
for key in ('DOROTI_MULTIWINDOW_PROBE','DOROTI_WINDOW_KINDS_PROBE','DOROTI_WINDOWS_APPSDK_REPORT','DOROTI_DESKTOP_PROBE','DOROTI_INPUT_PROBE'):env.pop(key,None)
with (run/'application.log').open('w',encoding='utf-8') as log:
    child=subprocess.Popen(['dotnet',str(app)],cwd=ROOT,env=env,stdout=log,stderr=subprocess.STDOUT)
    try:code=child.wait(timeout=60)
    finally:
        if child.poll() is None:child.kill();child.wait(timeout=10)
text=(run/'application.log').read_text(encoding='utf-8',errors='replace')
assert code==0 and not any(x in text for x in ('Unhandled exception','System.InvalidOperationException','System.ArgumentException','FlutterError')),text[-12000:]
probe=json.loads((run/'menu.json').read_text());assert probe['result']=='PASS' and probe['actualOsMenuObserved'] and probe['canceledInvocation'] and probe['dismissedResult']=='Canceled' and probe['actualMenuBarObserved'] and probe['menuBarDetached'] and probe['nativeCommandGeneration']==2 and probe['staleMenuCallbacks']==0
assert text.count('complete=true pending=0')==1,text[-10000:]
report=dict(schema='doroti.native-menu-verification/v1',result='PASS',scope='actual Windows OS popup menu observed in the owned process; typed owner rejection, cancellation/error propagation, dismissal result, actual HWND menu bar and native command generation/retirement, invocation/native cleanup',probe=probe,artifacts=str(run.relative_to(ROOT)),notVerified=['physical keyboard/shortcut action selection','material/cupertino dialog/theme/restoration integration','AppKit/Qt menus'])
(ROOT/'Doroti/docs/migrations/design-platform/windows-native-menu-verification.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
print('PASS: actual Windows OS menu, typed cancellation/dismissal and drain; '+str(run),flush=True)
