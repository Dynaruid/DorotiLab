"""Qt IME and native Tab automation, not a physical keyboard/IBus/Orca approval."""
import argparse,json,os,subprocess
from pathlib import Path
p=argparse.ArgumentParser()
p.add_argument('--app',type=Path,required=True);p.add_argument('--driver',type=Path,required=True)
p.add_argument('--output',type=Path,required=True);p.add_argument('--qpa',choices=['xcb','wayland'],required=True)
a=p.parse_args();out=a.output.resolve();out.mkdir(parents=True,exist_ok=False)
steps=[dict(action='wait',wait=2000),dict(action='click',x=150,y=165),dict(action='text',text=''),
 dict(action='js',path=str(out/'initial'),code="window.compositions=[];for(const k of ['compositionstart','compositionupdate','compositionend'])document.querySelector('input').addEventListener(k,e=>compositions.push([k,e.data]));return true"),
 dict(action='ime',preedit='ㅎ'),dict(action='ime',preedit='한'),dict(action='ime',commit='한글'),
 dict(action='js',path=str(out/'ime'),code="return {value:document.querySelector('input').value,events:compositions}"),
 dict(action='key',key='Tab'),dict(action='js',path=str(out/'tab'),code='return document.activeElement.tagName'),
 dict(action='key',key='Backtab'),dict(action='js',path=str(out/'backtab'),code='return document.activeElement.tagName'),
 dict(action='click',x=200,y=545),dict(action='capture',path=str(out/'framework'))]
script=out/'script.json';script.write_text(json.dumps(steps,ensure_ascii=False))
env=dict(os.environ,QT_QPA_PLATFORM=a.qpa,DOROTI_TESTBED_MODE='platform-effects',DOROTI_QT_TEST_SCRIPT=str(script),LD_PRELOAD=str(a.driver.resolve()))
with (out/'product.log').open('w') as log:run=subprocess.run(['dotnet',str(a.app.resolve())],env=env,stdout=log,stderr=log,timeout=120)
result=dict(qpa=a.qpa,exit=run.returncode,physical='notVerified',scope='Qt synthetic input method and within-page Tab; not full framework traversal')
try:
 assert run.returncode==0
 read=lambda name:json.loads((out/name).read_text())
 ime=read('ime1.json');result['ime']=ime
 assert ime['value']=='한글',ime
 kinds=[e[0] for e in ime['events']]
 assert 'compositionstart' in kinds and 'compositionend' in kinds,kinds
 assert read('tab1.json')=='BUTTON'
 assert read('backtab1.json')=='INPUT'
 assert all(not c['focused'] for c in read('framework.json')['controls']), 'Native focus not released to the Doroti text field'
 result['status']='PASS'
except Exception as error:result.update(status='FAIL',error=str(error))
(out/'result.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n');print(json.dumps(result,ensure_ascii=False))
raise SystemExit(0 if result['status']=='PASS' else 1)
