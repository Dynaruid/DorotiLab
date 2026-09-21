"""Native D3D11 -> Vulkan -> D3D12 texture gate. Run under run-with-timeout.py."""
import ctypes as c
from ctypes import wintypes as w
import importlib.util, os, json, subprocess, time, datetime, re
import numpy as np
from pathlib import Path
ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'Doroti/artifacts/textures/windows' / datetime.datetime.now().strftime('%Y%m%d-%H%M%S')
OUT.mkdir(parents=True)
os.environ['DOROTI_PLATFORM_VIEW_GATE_OUTPUT'] = str(OUT)
spec = importlib.util.spec_from_file_location('helpers', ROOT / 'Doroti/validation/windows-acrylic-composition/verify.py')
gate = importlib.util.module_from_spec(spec); spec.loader.exec_module(gate)
u = gate.u

def find_texture(pixels):
    delta = pixels.max(axis=2).astype(int) - pixels.min(axis=2)
    mask = (delta > 200) & (pixels.max(axis=2) > 230)
    ys, xs = np.nonzero(mask)
    if not len(xs):
        return None
    return int(xs.min()), int(ys.min()), int(xs.max()) + 1, int(ys.max()) + 1


def crop(pixels, rect):
    x0, y0, x1, y1 = rect
    return pixels[y0 + 12:y1 - 12, x0 + 12:x1 - 12]


def quadrants(pixels, rect):
    x0, y0, x1, y1 = rect
    colors = []
    for u, v in [(0.25, 0.25), (0.75, 0.25), (0.25, 0.75), (0.75, 0.75)]:
        colors.append(pixels[int(y0 + (y1-y0)*v), int(x0 + (x1-x0)*u)].astype(int))
    return all(np.max(np.abs(a-b)) < 12 for a, b in zip(colors, [[255,0,0], [0,255,0], [0,0,255], [255,255,0]]))


def buttons(pixels, bottom):
    r, g, b = [pixels[:, :, i] for i in range(3)]
    mask = (r > 75) & (r < 140) & (g > 50) & (g < 120) & (b > 135) & (b < 190)
    rows = np.flatnonzero(mask.sum(axis=1) > 100)
    rows = rows[rows > bottom]
    groups = np.split(rows, np.where(np.diff(rows) > 1)[0] + 1)
    result = []
    for group in groups:
        if len(group) < 30:
            continue
        y = int(group[len(group)//2])
        xs = np.flatnonzero(mask[y])
        result.append((int((xs.min()+xs.max())//2), y))
    return result



def main():
    env = os.environ.copy()
    env.update(DOROTI_TESTBED_MODE='texture-native', DOROTI_WINDOWS_PRESENTER='Vulkan',
        DOROTI_WINDOWS_VULKAN_VALIDATION='1', DOROTI_WINDOWS_D3D12_VALIDATION='1',
        DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1', DOROTI_WINDOWS_APPSDK_REPORT=str(OUT/'report.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(OUT/'ready.json'))
    hwnd=0
    with (OUT/'product.log').open('w', encoding='utf-8') as log:
        process=subprocess.Popen([str(gate.EXE)],cwd=gate.EXE.parent,env=env,stdout=log,stderr=log)
        try:
            ready=gate.wait_for(lambda: gate.read(OUT/'ready.json'),process)
            hwnd=ready['hwnd']; u.SetWindowPos(hwnd,w.HWND(-1),100,100,1000,1100,0x40)
            time.sleep(2)
            image, rect=gate.capture(hwnd,'native-1')
            time.sleep(.5)
            image2,_=gate.capture(hwnd,'native-2')
            assert process.poll() is None, 'Product crashed'
            camera = env.get('DOROTI_TEXTURE_CAMERA') == '1'
            if camera:
                assert image.tobytes()!=image2.tobytes(), 'Camera frames did not change'
            else:
                first=np.array(image.convert('RGB')); second=np.array(image2.convert('RGB'))
                bounds=find_texture(first); assert bounds is not None, 'Native texture is missing'
                assert quadrants(first,bounds), 'Native BGRA colors/orientation are wrong'
                assert not np.array_equal(crop(first,bounds),crop(second,bounds)), 'GPU frames did not animate'
                x0,y0,x1,y1=bounds
                assert first[y0+1,x0+1].min()>200, 'Rounded texture clip is missing'
                scale=(bounds[2]-bounds[0])/320
                controls=[((bounds[0]+bounds[2])//2,round(bounds[3]+y*scale)) for y in (36,68)]
                assert buttons(first,bounds[3]), 'Fixture controls are missing'
                def click(point):
                    origin=w.POINT();u.ClientToScreen(hwnd,c.byref(origin))
                    gate.click(hwnd,point[0]+rect.left-origin.x,point[1]+rect.top-origin.y,1,native_hit_test=False)
                click(controls[0]);time.sleep(.4)
                frozen1,_=gate.capture(hwnd,'frozen-1');time.sleep(.6);frozen2,_=gate.capture(hwnd,'frozen-2')
                assert np.array_equal(crop(np.array(frozen1),bounds),crop(np.array(frozen2),bounds)), 'Freeze changed native pixels'
                click(controls[0]);time.sleep(.3)
                resume1,_=gate.capture(hwnd,'resumed-1');time.sleep(.4);resume2,_=gate.capture(hwnd,'resumed-2')
                assert not np.array_equal(crop(np.array(resume1),bounds),crop(np.array(resume2),bounds)), 'Resume did not advance pixels'
                click(controls[1]);time.sleep(.6)
                recreated,_=gate.capture(hwnd,'recreated'); assert quadrants(np.array(recreated),bounds), 'Recreated texture is missing'
                u.ShowWindow(hwnd,6);time.sleep(.2);u.ShowWindow(hwnd,9);time.sleep(.5)
                restored,_=gate.capture(hwnd,'restored'); assert quadrants(np.array(restored),bounds), 'Restored texture is missing'
                assert process.poll() is None, 'Product failed during texture lifecycle'

        finally:
            if process.poll() is None:
                if hwnd: u.PostMessageW(hwnd,0x10,0,0)
                else: process.terminate()
            try: code=process.wait(timeout=20)
            except subprocess.TimeoutExpired: process.kill();process.wait();raise
    text=(OUT/'product.log').read_text(encoding='utf-8',errors='replace')
    assert code==0, text[-8000:]
    marker='doroti.texture.camera.frames=' if env.get('DOROTI_TEXTURE_CAMERA')=='1' else 'doroti.texture.synthetic.frames='
    assert re.search(re.escape(marker)+r'[1-9]\d*',text), text[-8000:]
    retired=re.findall(r'doroti.texture.importer platform=Windows imported=(\d+) retired=(\d+) live=(\d+)',text)
    assert retired and sum(int(item[0]) for item in retired)>0 and all(imported==released and live=='0' for imported,released,live in retired), retired
    assert 'doroti.texture.producer.error=' not in text, text[-8000:]
    report=json.loads((OUT/'report.json').read_text()); vk=report['vulkan']
    assert vk['validationEnabled'] and vk['validationErrors']==0, vk['validationMessages']
    assert vk['d3D12Output']['debugErrors']==0, vk['d3D12Output']
    resets=int(env.get('DOROTI_WINDOWS_APPSDK_DEVICE_RESET_COUNT','0'))
    assert report['frames']['completedDeviceResets']==resets, report['frames']
    result=dict(status='passed',source='camera' if env.get('DOROTI_TEXTURE_CAMERA')=='1' else 'gpu-pattern',imports=retired,deviceResets=resets,device=vk['device'],vulkanErrors=vk['validationErrors'],d3d12Errors=vk['d3D12Output']['debugErrors'],artifacts=str(OUT))
    (OUT/'result.json').write_text(json.dumps(result,indent=2));print(json.dumps(result))
if __name__=='__main__':
    try: main()
    except Exception:
        print((OUT/'product.log').read_text(encoding='utf-8',errors='replace')[-8000:]);raise
