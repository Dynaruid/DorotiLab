"""Build/run the production Qt input fixture in WSL after the Qt native build."""
from pathlib import Path
import shlex
import subprocess

root = Path(__file__).resolve().parents[3]
def linux(path):
    text = str(path.resolve()).replace('\\', '/')
    return '/mnt/' + text[0].lower() + text[2:]

out = root / 'Doroti/artifacts/validation/pointer-device-kind'
build = out / 'qt-build'
native = root / 'DorotiTestbedApp/linux/native'
wsl = ['wsl', '-d', 'Ubuntu', '--']
flags = shlex.split(subprocess.check_output(wsl + ['pkg-config', '--cflags', '--libs',
    'Qt6Widgets', 'Qt6OpenGL', 'wayland-client'], text=True))
objects = [linux(build / 'CMakeFiles/doroti_qt_host.dir/wayland-generated' / name)
    for name in ['ext-background-effect-v1-protocol.c.o', 'kde-blur-protocol.c.o']]
binary = linux(out / 'qt-input-contract')
subprocess.run(wsl + ['g++', '-std=c++20', '-fPIC', '-DDOROTI_QT_HOST_BUILD',
    '-I' + linux(native / 'include'), '-I' + linux(build / 'wayland-generated'),
    linux(Path(__file__).with_name('qt.cpp')), linux(native / 'src/doroti_qt_platform_views.cpp'),
    *objects, *flags, '-o', binary], check=True)
subprocess.run(wsl + ['env', 'QT_QPA_PLATFORM=offscreen', binary], check=True)
