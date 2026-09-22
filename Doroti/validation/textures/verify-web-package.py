"""Validate the actual NuGet and publish output, including declaration parity."""
import hashlib
import json
from pathlib import Path
from zipfile import ZipFile

root = Path('Doroti/artifacts/textures/web')
package = next((root/'packages').glob('*.nupkg'))
with ZipFile(package) as archive:
    files = archive.namelist()
    for suffix in ['doroti.web.textures.js', 'doroti.web.texture-worker.js', 'types/doroti-textures/index.d.ts']:
        assert any(name.endswith(suffix) for name in files), suffix
    assert b'doroti-textures' in archive.read('types/doroti-loader/index.d.ts')
    declarations = archive.read('types/doroti-textures/index.d.ts').decode()
generated = (root/'types-final/doroti.web.textures.d.ts').read_text().replace('export declare ', 'export ')
expected = '// Generated from doroti.web.textures.ts; verify with the pinned TypeScript declaration emitter.\ndeclare module "*_content/Doroti.Host.Web/doroti.web.textures.js" {\n' + ''.join('  '+line+'\n' for line in generated.splitlines()) + '}\n'
assert declarations.replace('\r\n','\n') == expected, 'Public declaration drift'
publish = root/'publish/wwwroot'
for name in ['_content/Doroti.Host.Web/doroti.web.textures.js',
             '_content/Doroti.Host.Web/doroti.web.texture-worker.js',
             'plugins/textures.js', 'plugins/texture-local.js', 'media/texture-pattern.mp4']:
    assert (publish/name).is_file(), name
for name in ['_wgpuTextureRelease', '_wgpuDeviceRelease', '_wgpuQueueRelease', '_wgpuInstanceRelease']:
    assert any(name in file.read_text(encoding='utf-8') for file in (publish/'_framework').glob('dotnet.native*.js')), name
result = {'status':'PASS', 'package':str(package), 'sha256':hashlib.sha256(package.read_bytes()).hexdigest(),
          'requiredFiles':[name for name in files if 'texture' in name.lower()], 'declarations':'match pinned emitter',
          'publishModulesAndNativeExports':'PASS'}
(root/'package-check.json').write_text(json.dumps(result,indent=2))
print('PASS NuGet modules/types, declaration parity, published modules/media and native releases')
