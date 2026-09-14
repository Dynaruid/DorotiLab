"""Browser DOM adapter gate. Does not qualify Doroti worker composition."""
from functools import partial
import html
import http.server
import json
from pathlib import Path
import re
import shutil
import subprocess
import threading
import time

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'Doroti/artifacts/platform-views/2026-09-14/rearchitecture' / ('web-dom-' + time.strftime('%H%M%S'))
OUT.mkdir(parents=True)
compiled = ROOT / 'Doroti/src/Doroti.Host.Web/obj/Debug/net10.0/Doroti.Web/wwwroot/doroti.web.platform-views.js'
shutil.copyfile(compiled, OUT / 'platform-views.js')
(OUT / 'index.html').write_text('''<!doctype html><div id="owner" style="position:relative;width:640px;height:480px"></div><pre id="result">pending</pre>
<script type="module">
import {DorotiPlatformViewDomRegistry, platformViewBatchVersion} from './platform-views.js';
try {
 const root=document.querySelector('#owner'), passed=[];
 const check=(name,value)=>{if(!value)throw Error(name);passed.push(name)};
 const registry=new DorotiPlatformViewDomRegistry(root,'1',()=>{},()=>{});
 let disposed=0;
 registry.register('iframe',()=>{const element=document.createElement('iframe');element.srcdoc='<input value="native state">';return {element,dispose(){disposed++}}});
 const identity={owner:'1',id:'1',generation:'1'};
 await registry.create(identity,'iframe');
 const element=root.querySelector('iframe');
 const bounds={left:0,top:0,width:300,height:200};
 const base={version:platformViewBatchVersion,owner:'1',epoch:0,surfaceGeneration:0,frame:1,
  views:[{identity,bounds,visible:true,order:1}],shields:[],effects:[{id:'blur',bounds:{left:50,top:50,width:100,height:80},order:2,strength:.75,tint:'#ffffff33',saturation:1}]};
 registry.commit(base);
 check('live DOM node preserved',root.querySelector('iframe')===element);
 check('bounded common effect',registry.effectCount===1&&root.querySelector('[data-doroti-platform-effect]').style.backdropFilter.includes('12px'));
 check('effect passes input',root.querySelector('[data-doroti-platform-effect]').style.pointerEvents==='none');
 let rejected=false;try{registry.commit({...base,frame:2,effects:[{...base.effects[0],strength:NaN}]})}catch{rejected=true}
 check('invalid batch preserves effect',rejected&&registry.effectCount===1);
 registry.commit({...base,frame:2,effects:[]});check('effect removal',registry.effectCount===0);
 rejected=false;try{registry.commit(base)}catch{rejected=true}check('old frame rejected',rejected);
 registry.commit({...base,frame:3});check('effect restored without iframe reload',root.querySelector('iframe')===element);
 await registry.dispose();check('owner disposal',disposed===1&&registry.effectCount===0&&registry.liveCount===0);
 document.querySelector('#result').textContent=JSON.stringify({status:'PASS',scope:'DOM adapter harness',productWorker:'notVerified',tests:passed});
}catch(error){document.querySelector('#result').textContent=JSON.stringify({status:'FAIL',error:String(error),stack:error.stack})}
</script>''', encoding='utf-8')
handler = partial(http.server.SimpleHTTPRequestHandler, directory=str(OUT))
server = http.server.ThreadingHTTPServer(('127.0.0.1', 0), handler)
thread = threading.Thread(target=server.serve_forever, daemon=True)
thread.start()
try:
    process = subprocess.run([r'C:\Program Files\Google\Chrome\Application\chrome.exe', '--headless=new',
        '--no-first-run', '--no-default-browser-check', '--disable-gpu', '--dump-dom', '--virtual-time-budget=4000',
        '--user-data-dir=' + str(OUT / 'chrome-profile'), f'http://127.0.0.1:{server.server_port}/index.html'],
        capture_output=True, text=True, encoding='utf-8', errors='replace', timeout=1200)
    (OUT / 'chrome.log').write_text(process.stderr, encoding='utf-8')
    (OUT / 'page.html').write_text(process.stdout, encoding='utf-8')
    match = re.search(r'<pre id="result">(.*?)</pre>', process.stdout, re.S)
    try:
        result = json.loads(html.unescape(match.group(1))) if match else {'status': 'FAIL', 'error': 'no result'}
    except ValueError:
        result = {'status': 'FAIL', 'error': 'module did not produce JSON; inspect page.html'}
    (OUT / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    print(OUT); print(json.dumps(result))
    if result['status'] != 'PASS' or process.returncode != 0:
        raise SystemExit(1)
finally:
    server.shutdown(); server.server_close()
