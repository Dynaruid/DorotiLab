"""Two actual HWNDs with one shared State/InheritedWidget and primary-survivor GPU frames."""
from pathlib import Path
import subprocess,sys,os,time,json,re,uuid
ROOT=Path(__file__).resolve().parents[2]
run=ROOT/'temp/testing/platform-decoupling/g1'/uuid.uuid4().hex
run.mkdir(parents=True)
exe=ROOT/'samples/DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.dll'
env=os.environ|dict(DOROTI_SAMPLE='shared-tree',DOROTI_DESKTOP_SAMPLE='solid',DOROTI_MULTIWINDOW_PROBE=str(run/'windows.json'),DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1',DOROTI_DESKTOP_LIFETIME='OnLastWindowClosed')
for name in ('DOROTI_WINDOWS_APPSDK_REPORT','DOROTI_DESKTOP_PROBE','DOROTI_INPUT_PROBE','DOROTI_WINDOWS_APPSDK_SMOKE_MS'):env.pop(name,None)
with (run/'application.log').open('w',encoding='utf-8') as log:
    process=subprocess.Popen(['dotnet',str(exe)],cwd=ROOT,env=env,stdout=log,stderr=subprocess.STDOUT)
    try:exitcode=process.wait(timeout=90)
    finally:
        if process.poll() is None:process.kill();process.wait(timeout=10)
output=(run/'application.log').read_text(encoding='utf-8',errors='replace')
assert exitcode==0,output[-10000:]
assert not any(marker in output for marker in ('Unhandled exception','AssertionError','FlutterError','DorotiError','System.InvalidOperationException','System.ArgumentException')),output[-10000:]
builds=[(app,int(view),int(revision)) for app,view,revision in re.findall(r'doroti.g1.build app=([0-9a-f-]+) view=(\d+) revision=(\d+)',output)]
assert len({app for app,_,_ in builds})==1,builds
assert {view for _,view,_ in builds}=={1,2},builds
shared=set(revision for _,view,revision in builds if view==1)&set(revision for _,view,revision in builds if view==2)
assert len(shared)>=2,builds
assert max(revision for _,view,revision in builds if view==2)>max(revision for _,view,revision in builds if view==1),builds
summaries=[(int(view),int(presented),int(scenes)) for _,view,presented,scenes in re.findall(r'doroti.g1.summary app=([0-9a-f-]+) view=(\d+) presented=(\d+) scenes=(\d+)',output)]
assert {view for view,_,_ in summaries}=={1,2} and all(presented>0 and scenes>0 for _,presented,scenes in summaries),summaries
drains=[(int(view),int(pending)) for view,pending in re.findall(r'doroti.g1.drain app=[0-9a-f-]+ view=(\d+) complete=true pending=(\d+)',output)]
assert {view for view,_ in drains}=={1,2} and all(pending==0 for _,pending in drains),drains
window=json.loads((run/'windows.json').read_text());assert window['remaining']==0 and window['survivor'],window
evidence=dict(schema='doroti.windows-shared-tree-verification/v1',result='PASS',scope='two real native Regular HWNDs, one application dispatcher/root/InheritedWidget state, per-view Graphite/Vulkan frame submissions and terminal summaries, primary close with newer survivor frame, native render join/GPU cleanup completion',builds=builds,summaries=summaries,drains=drains,windows=window,notVerified=['physical keyboard/IME','mixed-monitor DPI','slow GPU consumer isolation','modal/close reentrancy','separate target nupkg consumer'],artifacts=str(run.relative_to(ROOT)))
(ROOT/'Doroti/docs/migrations/design-platform/windows-shared-tree-verification.json').write_text(json.dumps(evidence,indent=2)+'\n',encoding='utf-8')
print('PASS: real Windows shared tree and primary-survivor frame/drain subset; '+str(run),flush=True)
