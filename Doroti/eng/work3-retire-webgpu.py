"""Archive the failed work3 G candidate, then restore only its owned changes."""
import difflib
import hashlib
import json
from pathlib import Path
import subprocess

root = Path(__file__).resolve().parents[2]
archive = root / 'history/26-09-08/work3-webgpu-candidate'
tracked = [
 'Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs',
 'Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs',
 'Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.cs',
 'Doroti/src/Doroti.Host.Web/Web/doroti.loader.ts',
 'Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts',
 'Doroti/src/Doroti.Host.Web/Web/doroti.web.protocol.ts',
 'Doroti/src/Doroti.Host.Web/Web/doroti.web.ts',
 'Doroti/src/Doroti.Host.Web/Web/types/doroti-loader/index.d.ts',
 'Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs',
 'Doroti/src/Doroti.Skia.RuntimeEffects/DorotiSkiaImageFilterRenderer.cs',
 'Doroti/src/Doroti.Skia.RuntimeEffects/DorotiSkiaRuntimeEffects.cs',
 'Doroti/src/Doroti.Target.Web.browser-wasm/build/Doroti.Target.Web.browser-wasm.props',
 'DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj',
 'Doroti/validation/web-playwright/tests/helpers/doroti-diagnostics.ts',
 'Doroti/validation/web-playwright/tests/threaded-runtime.spec.ts',
]
added = [
 'Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.Graphite.cs',
 'Doroti/src/Doroti.Host.Web/Web/doroti.webgpu.ts',
 'Doroti/src/Doroti.Skia.RuntimeEffects/SkiaGpuSurfaces.cs',
 'Doroti/validation/web-playwright/probe-work3-graphite.mjs',
 'Doroti/validation/web-playwright/work3-graphite.mjs',
 'Doroti/validation/web-playwright/tests/webgpu-unavailable.spec.ts',
 'DorotiTestbedApp/web/Validation/Work3GraphiteExport.cs',
]
if archive.exists():
    raise SystemExit('Candidate archive already exists; refusing to overwrite')
head = subprocess.check_output(['git','rev-parse','HEAD'],cwd=root,text=True).strip()
if head != '69a2cf7e793f440ab00224e16e45c0cee282d23b':
    raise SystemExit('Unexpected HEAD; review ownership before retirement')
baseline = {name:subprocess.check_output(['git','show',f'HEAD:{name}'],cwd=root) for name in tracked}
current = {name:(root/name).read_bytes() for name in tracked+added}
archive.mkdir(parents=True)
patch = subprocess.check_output(['git','diff','--binary','HEAD','--',*tracked],cwd=root)
for name in added:
    body = current[name].decode('utf-8').replace('\r\n','\n').splitlines(keepends=True)
    patch += (f'diff --git a/{name} b/{name}\nnew file mode 100644\n' +
              ''.join(difflib.unified_diff([],body,fromfile='/dev/null',tofile='b/'+name))).encode()
(archive/'candidate.patch').write_bytes(patch)
manifest = dict(head=head, reason='G3 required normal shutdown and device-loss gates failed; no performance adoption',
                patchSha256=hashlib.sha256(patch).hexdigest(),
                files={name:dict(candidateSha256=hashlib.sha256(data).hexdigest(),
                                originalSha256=hashlib.sha256(baseline[name]).hexdigest() if name in baseline else None)
                       for name,data in current.items()})
(archive/'manifest.json').write_text(json.dumps(manifest,indent=2),encoding='utf-8')
for name,data in current.items():
    target = archive/'sources'/(name+'.txt')
    target.parent.mkdir(parents=True,exist_ok=True)
    target.write_bytes(data)
    if target.read_bytes()!=data or (root/name).read_bytes()!=data:
        raise SystemExit('Archive verification or source ownership changed: '+name)
# Verify every target is a file within this workspace; no recursive deletion.
for name in added:
    path=(root/name).resolve()
    if not path.is_relative_to(root) or not path.is_file():
        raise SystemExit('Invalid retirement target: '+str(path))
for name,data in baseline.items():
    (root/name).write_bytes(data)
for name in added:
    (root/name).unlink()
for name,data in baseline.items():
    if (root/name).read_bytes()!=data: raise SystemExit('Restore mismatch: '+name)
print(json.dumps(dict(archived=len(current),restored=len(tracked),removedAdded=len(added),
                     patchSha256=manifest['patchSha256']),indent=2))
