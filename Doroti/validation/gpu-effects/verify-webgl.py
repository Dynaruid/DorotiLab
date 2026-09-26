"""Native WebGL2 fragment/UBO/state/retirement probe, not a Ganesh product run."""
from pathlib import Path
from http.server import ThreadingHTTPServer, SimpleHTTPRequestHandler
from functools import partial
import subprocess
import threading
import json
import html
import re
import time
import sys

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'Doroti/artifacts/gpu-effects' / f'webgl-{time.time_ns()}'
OUT.mkdir(parents=True)
TOOL = ROOT / 'tools/Doroti.Wgsl/target/debug/doroti-wgsl.exe'
EDGE = Path('C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe')
assert EDGE.exists(), 'Microsoft Edge is required for this Windows-hosted GL probe'
definition = json.loads((ROOT / 'Doroti/validation/gpu-effects/shaders/uniform.effect.json').read_text())
definition['requiredBackends'] = ['webgl2-fragment']
(OUT / 'uniform.effect.json').write_text(json.dumps(definition), encoding='utf-8')
source = ROOT / 'Doroti/validation/gpu-effects/shaders/uniform.wgsl'
if '--complex' in sys.argv:
    source = OUT / 'complex.wgsl'
    source.write_text('''struct Params { a: f32, v: vec3<f32>, m: mat3x3<f32>, values: array<vec4<u32>, 2>, signed: i32 }
@group(0) @binding(0) var input_image: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;
@group(1) @binding(0) var<uniform> params: Params;
@fragment fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
 let gain = params.a + params.v.x + params.m[1][2] + f32(params.values[1].w) + f32(params.signed);
 return textureSample(input_image, input_sampler, uv).bgra * vec4<f32>(gain, 1., 1., 1.);
}''', encoding='utf-8')
subprocess.run([str(TOOL), 'compile', str(source),
                str(OUT / 'uniform.effect.json'), str(OUT / 'uniform')], check=True, timeout=1200)
assets = {name: (OUT / 'uniform' / file).read_text(encoding='utf-8') for name, file in
          [('vertex', 'vertex.glsl'), ('fragment', 'fragment.glsl'), ('metadata', 'webgl2-bindings.json')]}
module = '/Doroti/src/Doroti.Host.Web/obj/Debug/net10.0/Doroti.Web/wwwroot/doroti.web.texture-worker.js'
script = r'''
const result = document.querySelector('#result');
try {
 const owner = await import(MODULE);
 const canvas = document.createElement('canvas'); canvas.width=64; canvas.height=48;
 const gl = canvas.getContext('webgl2'); if (!gl) throw new Error('WebGL2 unavailable');
 const table={ textures:[null], currentContext:{GLctx:gl}, getNewId(a){a.push(null);return a.length-1;} };
 owner.initializeTextures({ReleaseBrowserTextureImage(){},RegisterBrowserTexture(){throw new Error('unexpected');},
  MarkBrowserTexture(){},UnregisterBrowserTexture(){}}, undefined, ()=>table);
 const allocation=owner.allocateEffect(64,48);
 gl.activeTexture(gl.TEXTURE0); gl.bindTexture(gl.TEXTURE_2D,table.textures[allocation.input]);
 const input=new Uint8Array(64*48*4);
 for(let y=0;y<48;y++) for(let x=0;x<64;x++) input.set([x*3,y*4,x+y,255],(y*64+x)*4);
 gl.texSubImage2D(gl.TEXTURE_2D,0,0,0,64,48,gl.RGBA,gl.UNSIGNED_BYTE,input);
 gl.activeTexture(gl.TEXTURE3); gl.viewport(1,2,33,34); gl.scissor(4,5,23,24);
 gl.enable(gl.BLEND); gl.enable(gl.SCISSOR_TEST); gl.colorMask(false,true,false,true);
 PARAMETER_CODE
 const encoded=btoa(String.fromCharCode(...new Uint8Array(parameters.buffer)));
 owner.executeEffect(allocation.token,'uniform-probe',ASSETS.vertex,ASSETS.fragment,'main',ASSETS.metadata,encoded);
 if(gl.getParameter(gl.ACTIVE_TEXTURE)!==gl.TEXTURE3 || !gl.isEnabled(gl.BLEND) || !gl.isEnabled(gl.SCISSOR_TEST) ||
  Array.from(gl.getParameter(gl.VIEWPORT)).join()!=='1,2,33,34' || Array.from(gl.getParameter(gl.COLOR_WRITEMASK)).join()!=='false,true,false,true')
  throw new Error('External pass leaked GL state');
 const framebuffer=gl.createFramebuffer(); gl.bindFramebuffer(gl.FRAMEBUFFER,framebuffer);
 gl.framebufferTexture2D(gl.FRAMEBUFFER,gl.COLOR_ATTACHMENT0,gl.TEXTURE_2D,table.textures[allocation.output],0);
 const actual=new Uint8Array(input.length); gl.readPixels(0,0,64,48,gl.RGBA,gl.UNSIGNED_BYTE,actual);
 for(let y=0;y<48;y++) for(let x=0;x<64;x++) {
  const p=(y*64+x)*4;
  if(Math.abs(actual[p]-(x+y)*.5)>1 || actual[p+1]!==y*4 || actual[p+2]!==x*3 || actual[p+3]!==255)
   throw new Error(`Pixel ${x},${y}: ${actual.slice(p,p+4)}`);
 }
 gl.deleteFramebuffer(framebuffer); owner.retireTexture(allocation.token); await owner.flushRetired();
 if(owner.diagnostics().live!==0) throw new Error('Effect allocation did not retire');
 const extension=gl.getExtension('WEBGL_debug_renderer_info');
 const renderer=extension?gl.getParameter(extension.UNMASKED_RENDERER_WEBGL):gl.getParameter(gl.RENDERER);
 await owner.disposeTextures();
 result.textContent=JSON.stringify({status:'PASS',pixels:3072,tolerance:1,stateRestored:true,retired:true,renderer,productGanesh:'notVerified'});
} catch(error) { result.textContent=JSON.stringify({status:'FAIL',error:String(error),stack:error.stack}); }
'''.replace('MODULE', json.dumps(module)).replace('ASSETS', json.dumps(assets)).replace('PARAMETER_CODE',
'''const parameters=new Uint8Array(128); const view=new DataView(parameters.buffer);
 view.setFloat32(0,.1,true); view.setFloat32(16,.1,true); view.setFloat32(56,.3,true);
 view.setUint32(108,1,true); view.setInt32(112,-1,true);''' if '--complex' in sys.argv
 else 'const parameters=new Float32Array([.5,1,1,1]);')
(OUT / 'index.html').write_text('<!doctype html><pre id="result">pending</pre><script type="module">' + script + '</script>', encoding='utf-8')
class Handler(SimpleHTTPRequestHandler):
    def log_message(self, *_):
        pass
server = ThreadingHTTPServer(('127.0.0.1', 0), partial(Handler, directory=str(ROOT)))
threading.Thread(target=server.serve_forever, daemon=True).start()
try:
    url = f'http://127.0.0.1:{server.server_port}/' + (OUT / 'index.html').relative_to(ROOT).as_posix()
    completed = subprocess.run([str(EDGE), '--headless=new', '--no-first-run', '--no-default-browser-check',
        '--disable-extensions', f'--user-data-dir={OUT / "browser"}', '--virtual-time-budget=5000', '--dump-dom', url],
        capture_output=True, text=True, encoding='utf-8', errors='replace', timeout=60)
    (OUT / 'browser.log').write_text(completed.stderr, encoding='utf-8')
    (OUT / 'dom.html').write_text(completed.stdout, encoding='utf-8')
    match = re.search(r'<pre id="result">(.*?)</pre>', completed.stdout, re.S)
    assert match, 'No result from browser'
    result = json.loads(html.unescape(match.group(1)))
    (OUT / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    print(json.dumps(result))
    assert result['status'] == 'PASS', result
finally:
    server.shutdown()
