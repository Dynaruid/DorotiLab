"""Exercise native input/identity ABI contracts without submitting GPU frames."""
from pathlib import Path
import argparse
import os
import shlex
import subprocess

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--library', required=True, type=Path)
parser.add_argument('--output', required=True, type=Path)
parser.add_argument('--qpa', choices=['wayland', 'xcb'], default='wayland' if os.environ.get('WAYLAND_DISPLAY') else 'xcb')
args = parser.parse_args()
out = args.output.resolve()
assert out.is_relative_to(ROOT / 'temp/testing') and not out.exists()
out.mkdir(parents=True)
flags = shlex.split(subprocess.check_output(['pkg-config', '--cflags', '--libs', 'Qt6Quick', 'Qt6Widgets'], text=True))
executable = out / 'qt-input-contract'
library = args.library.resolve(strict=True)
with (out / 'build.log').open('w') as log:
    subprocess.run(['c++', '-std=c++20', str(ROOT / 'Doroti/tests/native/qt_input_contract.cpp'),
        '-I' + str(ROOT / 'packages/platforms/qt/native/include'), str(library),
        '-Wl,-rpath,' + str(library.parent), '-o', str(executable), *flags],
        stdout=log, stderr=subprocess.STDOUT, check=True, timeout=1200)
with (out / 'input.log').open('w') as log:
    for switches in [[], ['--legacy']]:
        subprocess.run([str(executable), *switches], env=os.environ | {'QT_QPA_PLATFORM': args.qpa,
            'XDG_DATA_HOME': str(out / 'userdata'), 'XDG_CACHE_HOME': str(out / 'cache')},
            stdout=log, stderr=subprocess.STDOUT, check=True, timeout=30)
print((out / 'input.log').read_text(), end='')
