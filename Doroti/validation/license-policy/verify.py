#!/usr/bin/env python3
"""Exercise license guards. Optional --gst-library runs the real adapter too."""
import argparse
import ctypes
import os
import shlex
from pathlib import Path
import subprocess
import sys
import tempfile

ROOT = Path(__file__).resolve().parents[3]
NATIVE = ROOT / "DorotiTestbedApp/linux/native"


def cmake_cases(work):
    policy = (NATIVE / "cmake/DorotiLicensePolicy.cmake").as_posix()
    core = 'add_library(Qt6::Core SHARED IMPORTED)\n'
    cases = {
        "shared-core": (core, 'Qt6::Core', True),
        "static-core": ('add_library(Qt6::Core STATIC IMPORTED)\n', 'Qt6::Core', False),
        "gpl-module": ('add_library(Qt6::Charts SHARED IMPORTED)\n', 'Qt6::Charts', False),
        "alias": ('add_library(Qt6::Charts SHARED IMPORTED)\nadd_library(Qt::Charts ALIAS Qt6::Charts)\n', 'Qt::Charts', False),
        "transitive-expression": (core + 'add_library(helper INTERFACE)\n'
            'add_library(Qt6::Charts SHARED IMPORTED)\n'
            'target_link_libraries(helper INTERFACE "$<LINK_ONLY:Qt6::Charts>")\n',
            '"$<LINK_ONLY:helper>"', False),
        "raw-flag": ('', '-lQt6Charts', False),
        "raw-archive": ('', '/tmp/libQt6Core.a', False),
        "raw-link-option": ('add_library(helper INTERFACE)\n'
            'target_link_options(helper INTERFACE -lQt6Charts)\n', 'helper', False),
        "interface-cycle": (core + 'add_library(a INTERFACE)\nadd_library(b INTERFACE)\n'
            'target_link_libraries(a INTERFACE b)\ntarget_link_libraries(b INTERFACE a Qt6::Core)\n', 'a', True),
    }
    for name, (setup, link, allowed) in cases.items():
        source = work / name
        source.mkdir()
        (source / 'CMakeLists.txt').write_text(
            'cmake_minimum_required(VERSION 3.24)\nproject(policy NONE)\n'
            + setup + 'add_library(app INTERFACE)\n'
            + f'target_link_libraries(app INTERFACE {link})\ninclude("{policy}")\n'
            + 'doroti_check_qt_license_policy(app)\n')
        result = subprocess.run(['cmake', '-S', str(source), '-B', str(source / 'build')],
                                text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        assert (result.returncode == 0) == allowed, (name, result.stdout)
        if not allowed:
            assert 'Doroti license policy:' in result.stdout, result.stdout
        print(f'PASS Qt policy: {name}')


def notices(work):
    import shutil
    app = work / 'app'
    shutil.copytree(NATIVE / 'licenses', app / 'native/licenses')
    targets = ROOT / 'Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.Qt.targets'
    (app / 'check.proj').write_text(f'''<Project>
  <PropertyGroup><DorotiHostKind>Qt</DorotiHostKind>
    <TargetDir>{app}/bin/</TargetDir><PublishDir>{app}/publish/</PublishDir>
  </PropertyGroup><Import Project="{targets}" />
</Project>''')
    subprocess.run(['dotnet', 'msbuild', str(app / 'check.proj'),
                    '-t:CopyDorotiQtLicenseNotices', '-nologo', '-v:quiet'], check=True)
    for source in (NATIVE / 'licenses').iterdir():
        for output in ['bin', 'publish']:
            assert (app / output / 'licenses/doroti-linux' / source.name).read_bytes() == source.read_bytes()
    print('PASS notices: build and publish copies match')
    (app / 'native/licenses/LGPL-3.txt').unlink()
    result = subprocess.run(['dotnet', 'msbuild', str(app / 'check.proj'),
        '-t:CopyDorotiQtLicenseNotices', '-nologo', '-v:quiet'],
        text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    assert result.returncode != 0 and 'LGPL-3.txt is missing' in result.stdout, result.stdout
    print('PASS notices: missing license text blocks packaging')


def gst_child(library, case):
    with tempfile.TemporaryDirectory(prefix='doroti-gst-policy-') as temp:
        work = Path(temp)
        plugins = work / 'plugins'
        plugins.mkdir()
        os.environ['XDG_CACHE_HOME'] = str(work / 'cache')
        os.environ.pop('DOROTI_GSTREAMER_PLUGIN_DIR', None)
        if case != 'missing-directory':
            os.environ['DOROTI_GSTREAMER_PLUGIN_DIR'] = str(plugins)
        if case in ['approved-pipeline', 'blocked-element']:
            # Debian/Ubuntu CI fixture; no hardware or camera is required.
            for name in ['coreelements', 'app']:
                source = Path('/usr/lib/x86_64-linux-gnu/gstreamer-1.0') / f'libgst{name}.so'
                assert source.is_file(), source
                (plugins / source.name).symlink_to(source)
        if case == 'unreviewed-file':
            (plugins / 'libgstlibav.so').touch()
        if case == 'nested-directory':
            (plugins / 'extra').mkdir()
        if case == 'already-initialized':
            os.environ['GST_PLUGIN_SYSTEM_PATH_1_0'] = ''
            os.environ['GST_PLUGIN_PATH_1_0'] = ''
            ctypes.CDLL('libgstreamer-1.0.so.0').gst_init(None, None)
        adapter = ctypes.CDLL(library)
        adapter.doroti_texture_gst_open.argtypes = [ctypes.c_char_p, ctypes.POINTER(ctypes.c_void_p),
                                                  ctypes.c_char_p, ctypes.c_uint32]
        adapter.doroti_texture_gst_close.argtypes = [ctypes.c_void_p]
        handle = ctypes.c_void_p()
        error = ctypes.create_string_buffer(2048)
        pipeline = (b'avdec_h264 ! appsink name=doroti_texture' if case == 'blocked-element'
                    else b'fakesrc num-buffers=0 ! appsink name=doroti_texture')
        status = adapter.doroti_texture_gst_open(pipeline, ctypes.byref(handle), error, len(error))
        if case == 'approved-pipeline':
            assert status == 0 and handle.value, error.value
            adapter.doroti_texture_gst_close(handle)
        else:
            assert status == -1 and not handle.value, (status, error.value)
            expected = {
                'missing-directory': b'Set DOROTI_GSTREAMER_PLUGIN_DIR',
                'unreviewed-file': b'Unreviewed GStreamer plugin directory entry',
                'nested-directory': b'Unreviewed GStreamer plugin directory entry',
                'empty-directory': b'requires libgstcoreelements.so',
                'already-initialized': b'before other users',
                'blocked-element': b'no element',
            }[case]
            assert expected in error.value, error.value
        print(f'PASS GStreamer policy: {case}')


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--gst-library')
    parser.add_argument('--build-gstreamer', action='store_true')
    parser.add_argument('--gst-case', help=argparse.SUPPRESS)
    args = parser.parse_args()
    if args.gst_case:
        gst_child(args.gst_library, args.gst_case)
        return
    with tempfile.TemporaryDirectory(prefix='doroti-license-policy-') as temp:
        cmake_cases(Path(temp))
        notices(Path(temp))
        subprocess.run([sys.executable, str(ROOT / 'Doroti/validation/linux-qt-quick/check-native-sync.py')], check=True)
        if args.build_gstreamer:
            args.gst_library = str(Path(temp) / 'libdoroti_texture_gstreamer.so')
            flags = shlex.split(subprocess.check_output(['pkg-config', '--cflags', '--libs',
                'gstreamer-1.0 >= 1.24', 'gstreamer-app-1.0 >= 1.24',
                'gstreamer-video-1.0 >= 1.24', 'gstreamer-allocators-1.0 >= 1.24'], text=True))
            subprocess.run(['c++', '-std=c++20', '-shared', '-fPIC', '-I' + str(NATIVE / 'include'),
                str(NATIVE / 'src/doroti_texture_gstreamer.cpp'), '-o', args.gst_library, *flags], check=True)
        if args.gst_library:
            for case in ['missing-directory', 'empty-directory', 'unreviewed-file', 'nested-directory',
                         'already-initialized', 'approved-pipeline', 'blocked-element']:
                subprocess.run([sys.executable, __file__, '--gst-library', str(Path(args.gst_library).resolve()),
                                '--gst-case', case], check=True, timeout=30)


if __name__ == '__main__':
    main()
