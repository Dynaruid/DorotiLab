"""Owned-device synthetic lifecycle probe, never a physical-input acceptance.
Run with eng/run-with-timeout.py and an explicit device ID. Restores rotation.
"""
import argparse
import json
from pathlib import Path
import subprocess
import time

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--adb', default='adb')
parser.add_argument('--device', required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--package', default='dev.doroti.sample2')
parser.add_argument('--activity', default='crc6467bcc435301192e0.MainActivity')
args = parser.parse_args()
out = args.output.resolve()
assert out.is_relative_to(ROOT/'temp/testing')
out.mkdir(parents=True, exist_ok=False)
cache = f'/sdcard/Android/data/{args.package}/cache/doroti-maui-evidence.json'
report = {'physicalInput': 'notVerified', 'resumes': []}

def adb(*values, **kwargs):
    return subprocess.check_output([args.adb,'-s',args.device,*values],timeout=30,**kwargs)

def start():
    return adb('shell','am','start','-W','-n',f'{args.package}/{args.activity}',
               '--es','DOROTI_MAUI_EVIDENCE','1').decode()

def snapshot(name, minimum=0):
    deadline = time.monotonic()+40
    while time.monotonic() < deadline:
        try:
            data = adb('shell','cat',cache,stderr=subprocess.DEVNULL)
            value = json.loads(data)
            if value['frame']['presented'] > minimum:
                (out/f'{name}.json').write_bytes(data)
                return value
        except (subprocess.CalledProcessError, json.JSONDecodeError): pass
        time.sleep(.25)
    raise TimeoutError(f'No progressing frame receipt: {name}')

auto = adb('shell','settings','get','system','accelerometer_rotation').decode().strip()
rotation = adb('shell','settings','get','system','user_rotation').decode().strip()
try:
    adb('shell','am','force-stop',args.package)
    adb('shell','rm','-f',cache)
    (out/'cold-launch.log').write_text(start())
    initial = snapshot('cold')
    first_pid = adb('shell','pidof',args.package).decode().strip()
    for index in range(1,4):
        adb('shell','input','keyevent','KEYCODE_HOME')
        time.sleep(.5)
        (out/f'resume-{index}.log').write_text(start())
        before = snapshot(f'resume-{index}-initial')
        time.sleep(5.1)
        after = snapshot(f'resume-{index}-after-5s',before['frame']['presented'])
        report['resumes'].append({'index':index,'initial':before['frame']['presented'],
            'after5s':after['frame']['presented'],'surfaceGeneration':after['surface']['surfaceGeneration']})
    adb('shell','settings','put','system','accelerometer_rotation','0')
    adb('shell','settings','put','system','user_rotation','1')
    time.sleep(3)
    rotated = snapshot('rotated')
    report['rotationExtent'] = [rotated['surface']['pixelWidth'],rotated['surface']['pixelHeight']]
    assert report['rotationExtent'][0] > report['rotationExtent'][1]
    adb('shell','settings','put','system','user_rotation',rotation)
    time.sleep(2)
    adb('shell','am','force-stop',args.package)
    adb('shell','rm','-f',cache)
    (out/'recreated-launch.log').write_text(start())
    recreated = snapshot('recreated')
    new_pid = adb('shell','pidof',args.package).decode().strip()
    assert first_pid != new_pid
    time.sleep(5.1)
    final = snapshot('recreated-after-5s',recreated['frame']['presented'])
    with (out/'screen.png').open('wb') as image:
        image.write(adb('exec-out','screencap','-p'))
    report.update(status='PASS',firstPid=first_pid,recreatedPid=new_pid,
                  recreatedAfter5s=final['frame']['presented'])
    print('PASS three 5-second continuing resumes, rotation and process/surface recreation; physical input notVerified')
except Exception as error:
    report.update(status='FAIL',error=str(error)); raise
finally:
    adb('shell','settings','put','system','accelerometer_rotation',auto)
    adb('shell','settings','put','system','user_rotation',rotation)
    (out/'results.json').write_text(json.dumps(report,indent=2))
