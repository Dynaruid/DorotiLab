"""Install a hash-checked NuGet-only Android candidate and observe its native first frame.

This verifies published package consumption, not physical input, production signing
or clean-machine installation. Use run-with-timeout.py --timeout 1200.
"""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import time
import uuid

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--candidate', type=Path, required=True)
    parser.add_argument('--device', required=True)
    parser.add_argument('--alias', default='android')
    parser.add_argument('--adb', default='adb')
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--run-id')
    args = parser.parse_args()
    candidate = args.candidate.resolve()
    out = args.output.resolve()
    if not out.is_relative_to(ROOT / 'temp/testing') and not out.is_relative_to(candidate):
        parser.error('Evidence must be under temp/testing or this candidate.')
    out.mkdir(parents=True, exist_ok=False)
    record = json.loads((candidate / 'candidate.json').read_text(encoding='utf-8'))
    payload = candidate / args.alias
    hashes = record['artifacts'][args.alias]
    for relative, expected in hashes.items():
        path = (payload / relative).resolve()
        if not path.is_relative_to(payload.resolve()) or hashlib.sha256(path.read_bytes()).hexdigest() != expected:
            raise ValueError('Candidate artifact hash mismatch: ' + relative)
    if {path.relative_to(payload).as_posix() for path in payload.rglob('*') if path.is_file()} != set(hashes):
        raise ValueError('Candidate contains unlisted artifacts.')
    apks = list(payload.glob('*-Signed.apk'))
    if len(apks) != 1: raise ValueError('Expected one published signed APK.')
    profile = record['consumerProfiles'][args.alias]
    package = profile['ApplicationId']
    rid = profile['RuntimeIdentifier']
    receipt = dict(schemaVersion='doroti.android-native-consumer/v1', status='running',
        runId=args.run_id or uuid.uuid4().hex, version=record['versions']['core'],
        providerVersions=record['versions']['providers'], design=record['design'], device=args.device,
        rid=rid, applicationId=package, apkSha256=hashlib.sha256(apks[0].read_bytes()).hexdigest(),
        physicalInput='notVerified', productionSigning='notVerified', cleanMachine='notVerified')

    def adb(*values):
        return subprocess.check_output([args.adb, '-s', args.device, *values], timeout=30, stderr=subprocess.DEVNULL)

    remote = '/sdcard/Android/data/' + package + '/cache/doroti-maui-evidence.json'
    try:
        abi = adb('shell', 'getprop', 'ro.product.cpu.abi').decode().strip()
        if {'arm64-v8a': 'android-arm64', 'x86_64': 'android-x64'}.get(abi) != rid:
            raise ValueError('Candidate RID does not match the device ABI.')
        (out / 'install.log').write_bytes(adb('install', '-r', str(apks[0])))
        component = adb('shell', 'cmd', 'package', 'resolve-activity', '--brief', package).decode().strip().splitlines()[-1]
        if not component.startswith(package + '/'): raise ValueError('Invalid installed launcher.')
        adb('shell', 'am', 'force-stop', package)
        adb('shell', 'rm', '-f', remote)
        (out / 'start.log').write_bytes(adb('shell', 'am', 'start', '-W', '-n', component, '--es', 'DOROTI_MAUI_EVIDENCE', '1'))
        deadline = time.monotonic() + 50
        frame = None
        while time.monotonic() < deadline:
            try:
                value = json.loads(adb('shell', 'cat', remote))
                if value['frame']['presented'] > 0 and value['frame']['failed'] == 0:
                    frame = value; break
            except (ValueError, subprocess.SubprocessError): pass
            time.sleep(.2)
        if frame is None: raise TimeoutError('No native first-frame receipt from the published APK.')
        (out / 'frame.json').write_text(json.dumps(frame, indent=2), encoding='utf-8')
        (out / 'screen.png').write_bytes(adb('exec-out', 'screencap', '-p'))
        receipt.update(status='PASS', nativeFirstFrame=True, nativeBackend=frame['surface']['graphicsBackend'],
            presented=frame['frame']['presented'], extent=[frame['surface']['pixelWidth'], frame['surface']['pixelHeight']],
            installedComponent=component, twoWindows='notApplicable: mobile native windows unsupported',
            shutdown='OS force-stop; joined application drain qualified by the separate source probe')
    except Exception as error:
        receipt.update(status='FAIL', error=str(error)); raise
    finally:
        try: adb('shell', 'am', 'force-stop', package)
        finally: (out / 'receipt.json').write_text(json.dumps(receipt, indent=2), encoding='utf-8')
    print('PASS published Android ' + record['design'] + ' NuGet consumer native first frame; physical/signing/clean-machine notVerified', flush=True)


if __name__ == '__main__':
    main()
