"""Mounted Vulkan/D3D12 output gate. Invoke under a 1200-second parent timeout."""
import argparse
import ctypes as c
from ctypes import wintypes as w
import datetime
import importlib.util
import json
import os
import re
from pathlib import Path
import subprocess
import sys
import time

ROOT = Path(__file__).resolve().parents[3]
sys.dont_write_bytecode = True
parser = argparse.ArgumentParser()
parser.add_argument('--gpu', default='NoPreference', choices=['NoPreference', 'LowPowerPreference', 'HighPerformancePreference'])
parser.add_argument('--vulkan-validation', action='store_true')
args = parser.parse_args()
OUT = ROOT / 'Doroti/artifacts/validation/windows-d3d12-output' / (datetime.datetime.now().strftime('%Y%m%d-%H%M%S') + '-' + args.gpu)
OUT.mkdir(parents=True)
os.environ['DOROTI_PLATFORM_VIEW_GATE_OUTPUT'] = str(OUT)
spec = importlib.util.spec_from_file_location('composition_helpers', ROOT / 'Doroti/validation/windows-acrylic-composition/verify.py')
gate = importlib.util.module_from_spec(spec)
spec.loader.exec_module(gate)
u = gate.u
u.GetClientRect.argtypes = [w.HWND, c.POINTER(w.RECT)]
u.ShowWindow.argtypes = [w.HWND, c.c_int]


def main():
    environment = os.environ.copy()
    environment.update(DOROTI_TESTBED_MODE='sample', DOROTI_WINDOWS_PRESENTER='Vulkan',
        DOROTI_WINDOWS_GPU_PREFERENCE=args.gpu, DOROTI_WINDOWS_D3D12_VALIDATION='1',
        DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1', DOROTI_WINDOWS_APPSDK_DEVICE_RESET_COUNT='1',
        DOROTI_WINDOWS_APPSDK_REPORT=str(OUT / 'report.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(OUT / 'ready.json'))
    environment.pop('DOROTI_WINDOWS_APPSDK_SMOKE_MS', None)
    environment.pop('DOROTI_WINDOWS_VULKAN_DEVICE', None)
    if args.vulkan_validation:
        environment['DOROTI_WINDOWS_VULKAN_VALIDATION'] = '1'
    hwnd = 0
    samples = []
    with (OUT / 'product.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(gate.EXE)], cwd=gate.EXE.parent, env=environment, stdout=log, stderr=log)
        try:
            ready = gate.wait_for(lambda: gate.read(OUT / 'ready.json'), process)
            hwnd = ready['hwnd']
            u.SetWindowPos(hwnd, w.HWND(-1), 0, 0, 0, 0, 0x43)
            time.sleep(.5)
            for edge, left, top, width, height in [(2,100,100,800,620), (2,100,100,1100,760),
                    (1,180,100,1020,760), (3,180,160,1020,700), (4,220,190,980,670),
                    (2,220,190,720,540), (2,80,80,2900,2000), (2,220,190,1050,780)]:
                u.SendMessageW(hwnd, 0x231, 0, 0)  # WM_ENTERSIZEMOVE
                rectangle = w.RECT(left, top, left + width, top + height)
                u.SendMessageW(hwnd, 0x214, edge, c.addressof(rectangle))  # WM_SIZING
                assert u.SetWindowPos(hwnd, None, rectangle.left, rectangle.top,
                    rectangle.right - rectangle.left, rectangle.bottom - rectangle.top, 0x14)
                u.SendMessageW(hwnd, 0x232, 0, 0)  # WM_EXITSIZEMOVE
                time.sleep(.3)
                assert process.poll() is None, 'Product failed during resize'
                client = w.RECT()
                assert u.GetClientRect(hwnd, c.byref(client))
                image, _ = gate.capture(hwnd, f'resize-{len(samples)}')
                # Visible nonuniform content, not merely a live HWND or present counter.
                colors = image.convert('RGB').getcolors(image.width * image.height)
                assert colors and len(colors) > 100, 'Window lost its rendered content'
                samples.append(dict(edge=edge, clientWidth=client.right, clientHeight=client.bottom))
            u.ShowWindow(hwnd, 6)  # minimize
            time.sleep(.2)
            u.ShowWindow(hwnd, 9)  # restore
            time.sleep(.4)
            gate.capture(hwnd, 'restored')
        finally:
            if process.poll() is None:
                if hwnd:
                    u.PostMessageW(hwnd, 0x10, 0, 0)
                else:
                    process.terminate()  # no ready HWND after startup failure
            try:
                code = process.wait(timeout=20)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait()
                raise RuntimeError('Product close exceeded the 20-second shutdown gate')
    report = json.loads((OUT / 'report.json').read_text())
    frames = report['frames']
    vk = report['vulkan']
    output = vk['d3D12Output']
    assert code == 0, f'Product exited with {code}'
    assert report['presenter']['effective'] == 'Graphite/Vulkan/D3D12/DXGI'
    assert output['debugEnabled'] and output['debugErrors'] == 0, output
    assert output['submittedCopies'] > 0 and output['completedCopies'] >= output['submittedCopies'], output
    # Product report is intentionally captured before presenter disposal.
    assert output['activeSwapchains'] == 1, 'Runtime did not own a DXGI swapchain'
    destruction = re.findall(r'doroti.d3d12.destroyed submitted=(\d+) completed=(\d+)',
        (OUT / 'product.log').read_text(encoding='utf-8', errors='replace'))
    assert len(destruction) == 2 and all(int(done) >= int(sent) for sent, done in destruction), destruction
    assert output['swapchainResizes'] > 0, 'DXGI growth/ResizeBuffers was not exercised'
    assert vk['validationErrors'] == 0 and not vk['validationCallbackFault'], {
        'vulkanErrors': vk['validationErrors'], 'messages': vk['validationMessages'], 'report': str(OUT / 'report.json')}
    if args.vulkan_validation:
        assert vk['depthStencilBarrierStageCorrections'] > 0, 'Writable depth/stencil barriers were not exercised'
    assert frames['completedDeviceResets'] == 1, frames
    assert vk['movingOriginWindowPosFailed'] == 0 and vk['movingOriginWindowPosMismatch'] == 0, vk
    assert vk['movingOriginWindowPosCommitted'] > 0, 'Prepared moving-origin resize was not exercised'
    assert vk['compositionFrameWaitTimeouts'] == 0, 'Resize display receipt timed out'
    result = dict(status='passed', gpu=args.gpu, device=vk['device'], resizes=samples,
        d3d12=output, preparedCommits=vk['movingOriginWindowPosCommitted'], closeExitCode=code,
        vulkanValidationEnabled=vk['validationEnabled'], destroyedOwners=len(destruction),
        depthStencilBarrierStageCorrections=vk['depthStencilBarrierStageCorrections'],
        physicalInput='notVerified', resizeSmoothness='notVerified', performanceComparison='notVerified',
        artifacts=str(OUT))
    (OUT / 'result.json').write_text(json.dumps(result, indent=2))
    print(json.dumps(result), flush=True)


if __name__ == '__main__':
    try:
        main()
    except Exception:
        print((OUT / 'product.log').read_text(encoding='utf-8', errors='replace')[-6000:], flush=True)
        raise
