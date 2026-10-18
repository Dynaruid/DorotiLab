"""Build the provider-owned Qt native profile explicitly on Linux."""
from pathlib import Path
import argparse
import platform
import subprocess

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--configuration', choices=['Debug', 'Release'], default='Release')
parser.add_argument('--output', type=Path)
for name, default in (('quick', 'ON'), ('graphite', 'ON'), ('webengine', 'OFF'), ('gstreamer', 'OFF')):
    parser.add_argument('--' + name, choices=['ON', 'OFF'], default=default)
parser.add_argument('--icon', type=Path)
parser.add_argument('--qt-version', default='6.12.0')
parser.add_argument('--qt-sdk', type=Path)
parser.add_argument('--webengine-version', default='6.140.0')
parser.add_argument('--webengine-sdk', type=Path)
args = parser.parse_args()
if platform.system() != 'Linux':
    parser.error('Qt native builds require Linux; no native qualification was run.')
root = Path(__file__).resolve().parents[1]
output = (args.output or root / 'artifacts/native').resolve()
command = ['cmake', '-S', str(root / 'native'), '-B', str(output), '-DCMAKE_BUILD_TYPE=' + args.configuration,
           '-DDOROTI_QT_QUICK=' + args.quick, '-DDOROTI_QT_GRAPHITE=' + args.graphite,
           '-DDOROTI_QT_WEBENGINE=' + args.webengine, '-DDOROTI_GSTREAMER_TEXTURES=' + args.gstreamer,
           '-DDOROTI_QT_VERSION=' + args.qt_version, '-DDOROTI_QT_WEBENGINE_VERSION=' + args.webengine_version,
           '-DDOROTI_QT_SDK=' + (str(args.qt_sdk.resolve()) if args.qt_sdk else ''),
           '-DDOROTI_QT_WEBENGINE_SDK=' + (str(args.webengine_sdk.resolve()) if args.webengine_sdk else '')]
if args.icon:
    command.append('-DDOROTI_APP_ICON=' + str(args.icon.resolve()))
subprocess.run(command, check=True, timeout=1200)
subprocess.run(['cmake', '--build', str(output), '--config', args.configuration], check=True, timeout=1200)
