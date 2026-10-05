"""Android provider aggregate with explicit RID/device and scoped persistent receipts.

Use run-with-timeout.py --timeout 1200. Device cases install only the selected
sample packages, retain their data, and distinguish synthetic from physical input.
"""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import time
import uuid

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--device')
    parser.add_argument('--rid', choices=['android-arm64', 'android-x64'], default='android-arm64')
    parser.add_argument('--adb', default='adb')
    parser.add_argument('--build-root', type=Path)
    parser.add_argument('--cases', default='profiles,tools,build,lifecycle,shutdown,reload')
    args = parser.parse_args()
    out = args.output.resolve()
    if not out.is_relative_to(ROOT / 'temp/testing'):
        parser.error('Evidence must be under temp/testing.')
    out.mkdir(parents=True, exist_ok=False)
    cases = args.cases.split(',')
    if len(set(cases)) != len(cases) or set(cases) - {'profiles', 'tools', 'build', 'lifecycle', 'shutdown', 'reload', 'input', 'stability'}:
        parser.error('Invalid or duplicate cases.')
    report = dict(schemaVersion='doroti.android-smoke/v1', createdUtc=datetime.now(timezone.utc).isoformat(),
                  status='running', rid=args.rid, device=args.device, checks=[], physicalInput='notVerified',
                  accessibility='notVerified', scanout='notVerified', releaseAot='notVerified', cleanMachine='notVerified')

    def command(name, argv):
        path = out / (name + '.log')
        with path.open('w', encoding='utf-8') as log:
            result = subprocess.run([sys.executable, str(ROOT / 'Doroti/eng/run-with-timeout.py'), '--timeout', '1200', *map(str, argv)],
                                    cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
        report['checks'].append(dict(name=name, status='PASS' if result.returncode == 0 else 'FAIL', exitCode=result.returncode,
                                     log=str(path.relative_to(ROOT)), sha256=hashlib.sha256(path.read_bytes()).hexdigest()))
        if result.returncode:
            raise RuntimeError(name + ': ' + path.read_text(encoding='utf-8', errors='replace')[-8000:])
        print(name + ': PASS', flush=True)

    def adb(*values):
        return subprocess.check_output([args.adb, '-s', args.device, *values], timeout=30, stderr=subprocess.DEVNULL)

    def wait_json(remote, local, seconds=50):
        deadline = time.monotonic() + seconds
        while time.monotonic() < deadline:
            try:
                raw = adb('shell', 'cat', remote)
                value = json.loads(raw)
                local.write_bytes(raw)
                return value
            except (subprocess.SubprocessError, ValueError):
                try:
                    error = adb('shell', 'cat', remote + '.error').decode()
                    raise RuntimeError(error)
                except subprocess.SubprocessError:
                    pass
                time.sleep(.2)
        raise TimeoutError(remote)

    try:
        report['sdk'] = subprocess.check_output(['dotnet', '--version'], cwd=ROOT, text=True).strip()
        device_cases = set(cases) & {'lifecycle', 'shutdown', 'reload', 'input', 'stability'}
        if args.device:
            if adb('get-state').strip() != b'device':
                raise RuntimeError('The selected device is not authorized.')
            abi = adb('shell', 'getprop', 'ro.product.cpu.abi').decode().strip()
            if {'arm64-v8a': 'android-arm64', 'x86_64': 'android-x64'}.get(abi) != args.rid:
                raise RuntimeError('Selected device ABI does not match RID.')
            report.update(abi=abi, model=adb('shell', 'getprop', 'ro.product.model').decode().strip(),
                          osVersion=adb('shell', 'getprop', 'ro.build.version.release').decode().strip(),
                          deviceKind='emulator' if adb('shell', 'getprop', 'ro.kernel.qemu').strip() == b'1' else 'physical')
        if 'profiles' in cases:
            command('profiles', [sys.executable, 'Doroti/tests/android_development_profile.py', out / 'profiles'])
        if 'tools' in cases:
            command('adb-session-contract', [sys.executable, 'Doroti/tests/android_development_bridge.py'])
            command('typed-tool-contract', ['dotnet', 'run', '--project', 'packages/platforms/maui/tests/Doroti.Tool.Maui.Tests', '--artifacts-path', out / 'tool-build'])
        payloads = {}
        if 'build' in cases or device_cases - {'reload'} and args.device:
            # aapt2 needs a short path on Windows, independent of evidence location.
            build = (args.build_root or ROOT / 'Doroti/artifacts/a3' / uuid.uuid4().hex[:8]).resolve()
            if not build.is_relative_to(ROOT / 'Doroti/artifacts'):
                raise RuntimeError('Build output must be under Doroti/artifacts.')
            for sample in ('DorotiSampleApp2', 'DorotiTestbedApp'):
                runner = ROOT / f'samples/{sample}/android/{sample}.Android.csproj'
                # adb install needs a complete APK, unlike the SDK's fast deployment.
                properties = ['-p:ArtifactsPath=' + str(build), '-p:RuntimeIdentifier=' + args.rid, '-p:EmbedAssembliesIntoApk=true']
                command('build-' + sample, ['dotnet', 'build', runner, '-c', 'Debug', '--nologo', *properties])
                raw = subprocess.check_output(['dotnet', 'msbuild', runner, '-p:Configuration=Debug', *properties,
                    '-t:_ResolveMonoAndroidSdks', '-getProperty:ApplicationId,TargetFramework,RuntimeIdentifier,UseMonoRuntime,PublishTrimmed,RunAOTCompilation,PublishAot,Optimize,_JavaSdkDirectory,_AndroidSdkDirectory,_AndroidNdkDirectory,_AndroidApiLevel,AndroidSdkBuildToolsVersion'], cwd=ROOT, text=True)
                profile = json.loads(raw)['Properties']
                apks = list((build / 'bin' / (sample + '.Android')).rglob('*-Signed.apk'))
                if len(apks) != 1: raise RuntimeError('Expected one signed debug APK: ' + str(apks))
                payloads[sample] = dict(apk=str(apks[0]), sha256=hashlib.sha256(apks[0].read_bytes()).hexdigest(), profile=profile)
            report['payloads'] = payloads
        if device_cases and not args.device:
            report['checks'] += [dict(name=name, status='SKIPPED', reason='No explicit authorized device selected.') for name in sorted(device_cases)]
        if args.device and device_cases - {'reload'}:
            for sample, payload in payloads.items():
                command('install-' + sample, [args.adb, '-s', args.device, 'install', '-r', payload['apk']])
                package = payload['profile']['ApplicationId']
                component = adb('shell', 'cmd', 'package', 'resolve-activity', '--brief', package).decode().strip().splitlines()[-1]
                if not component.startswith(package + '/'):
                    raise RuntimeError('Unable to resolve installed launcher: ' + component)
                payload['component'] = component
            if 'lifecycle' in cases:
                payload = payloads['DorotiSampleApp2']
                command('lifecycle', [sys.executable, 'Doroti/tests/native_frame_android_lifecycle.py', '--adb', args.adb,
                    '--device', args.device, '--package', payload['profile']['ApplicationId'], '--activity', payload['component'].split('/', 1)[1],
                    '--output', out / 'lifecycle'])
            testbed = payloads['DorotiTestbedApp']
            package = testbed['profile']['ApplicationId']
            for case in ('shutdown', 'input'):
                if case not in cases: continue
                name = 'shutdown-probe.json' if case == 'shutdown' else 'text-focus-probe.json'
                remote = '/sdcard/Android/data/' + package + '/cache/' + name
                adb('shell', 'am', 'force-stop', package)
                adb('shell', 'rm', '-f', remote, remote + '.error')
                adb('shell', 'am', 'start', '-W', '-n', testbed['component'], '--es',
                    'doroti_shutdown_probe' if case == 'shutdown' else 'doroti_text_focus_probe', '1')
                result = wait_json(remote, out / name)
                if result.get('status') != 'PASS': raise RuntimeError(str(result))
                report['checks'].append(dict(name=case, status='PASS', scope='native automated probe; physical input notVerified'))
            if 'stability' in cases:
                remote = '/sdcard/Android/data/' + package + '/cache/doroti-maui-evidence.json'
                adb('shell', 'am', 'force-stop', package)
                adb('shell', 'rm', '-f', remote)
                adb('shell', 'am', 'start', '-W', '-n', testbed['component'], '--es', 'DOROTI_MAUI_EVIDENCE', '1')
                first = wait_json(remote, out / 'stability-before.json')
                pid = adb('shell', 'pidof', package).strip()
                time.sleep(60)
                after = wait_json(remote, out / 'stability-after.json')
                if pid != adb('shell', 'pidof', package).strip() or after['frame']['presented'] <= first['frame']['presented']:
                    raise RuntimeError('The 60-second run did not retain a progressing process.')
                report['checks'].append(dict(name='stability', status='PASS', scope='60-second automated foreground frame progress'))
        if 'reload' in cases and args.device:
            command('metadata-reload', [sys.executable, 'Doroti/tests/android_hot_reload_smoke.py', out / 'reload', '--device', args.device])
        report['status'] = 'PARTIAL' if any(check['status'] == 'SKIPPED' for check in report['checks']) else 'PASS'
    except Exception as error:
        report.update(status='FAIL', error=str(error))
        raise
    finally:
        (out / 'summary.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    print('Android aggregate: ' + report['status'] + '; physical input/AOT/clean-machine notVerified', flush=True)


if __name__ == '__main__':
    main()
