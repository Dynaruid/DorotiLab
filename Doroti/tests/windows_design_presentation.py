"""Actual Windows design route/native branch results, captured theme/localization and cleanup."""
from pathlib import Path
from datetime import datetime, timezone
import subprocess,sys,os,json,uuid
ROOT=Path(__file__).resolve().parents[2]
run=ROOT/'temp/testing/platform-decoupling/design-presentation'/uuid.uuid4().hex;run.mkdir(parents=True)
app=ROOT/'samples/DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.dll'
probes=[]
for design in ('material','cupertino'):
    output=run/(design+'.json')
    env=os.environ|dict(DOROTI_SAMPLE='native-design',DOROTI_DESIGN_PROBE=design,DOROTI_DESIGN_PROBE_OUTPUT=str(output),DOROTI_DESKTOP_SAMPLE='solid',DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1')
    for key in ('DOROTI_MULTIWINDOW_PROBE','DOROTI_WINDOW_KINDS_PROBE','DOROTI_NATIVE_MENU_PROBE','DOROTI_WINDOWS_APPSDK_REPORT','DOROTI_DESKTOP_PROBE','DOROTI_INPUT_PROBE'):env.pop(key,None)
    with (run/(design+'.log')).open('w',encoding='utf-8') as log:
        child=subprocess.Popen(['dotnet',str(app)],cwd=ROOT,env=env,stdout=log,stderr=subprocess.STDOUT)
        try:code=child.wait(timeout=90)
        finally:
            if child.poll() is None:child.kill();child.wait(timeout=10)
    text=(run/(design+'.log')).read_text(encoding='utf-8',errors='replace')
    assert code==0 and not any(x in text for x in ('Unhandled exception','doroti.design.probe FAIL','FlutterError','DorotiError','doroti.native.fatal')),text[-12000:]
    probe=json.loads(output.read_text());assert probe['result']=='PASS' and probe['capturedTheme'] and probe['capturedLocalization'] and probe['nativeResult']==23 and probe['overlayResult']==23 and probe['remainingWindows']==1
    assert probe['nativeVisibleBeforeResult'] and probe['tooltipVisible'] and probe['tooltipCancellation'] and probe['tooltipEarlyCancellation'] and probe['tooltipConcurrentDispose'],probe
    assert text.count('complete=true pending=0')==6,text[-10000:]
    probes.append(probe)
report=dict(schema='doroti.native-design-verification/v1',verifiedUtc=datetime.now(timezone.utc).isoformat(),result='PASS',probes=probes,artifacts=str(run.relative_to(ROOT)),notVerified=['physical keyboard/focus traversal','route restoration across process restart','RawTooltip hover/late widget install','Cupertino context menu'])
(ROOT/'Doroti/docs/migrations/design-platform/windows-design-verification.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
print('PASS: Windows Material/Cupertino native and Overlay route results, captured themes/localization, branch and GPU cleanup; '+str(run),flush=True)
