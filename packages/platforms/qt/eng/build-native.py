"""Build the provider-owned Qt native profile explicitly on Linux."""
from pathlib import Path
import argparse,platform,subprocess
p=argparse.ArgumentParser();p.add_argument('--configuration',choices=['Debug','Release'],default='Release');p.add_argument('--output',type=Path);p.add_argument('--quick',choices=['ON','OFF'],default='ON');p.add_argument('--graphite',choices=['ON','OFF'],default='ON');p.add_argument('--webengine',choices=['ON','OFF'],default='OFF');p.add_argument('--gstreamer',choices=['ON','OFF'],default='OFF');p.add_argument('--icon',type=Path);a=p.parse_args()
if platform.system()!='Linux':p.error('Qt native builds require Linux; no native qualification was run.')
root=Path(__file__).resolve().parents[1];out=(a.output or root/'artifacts/native').resolve()
args=['cmake','-S',str(root/'native'),'-B',str(out),'-DCMAKE_BUILD_TYPE='+a.configuration,'-DDOROTI_QT_QUICK='+a.quick,'-DDOROTI_QT_GRAPHITE='+a.graphite,'-DDOROTI_QT_WEBENGINE='+a.webengine,'-DDOROTI_GSTREAMER_TEXTURES='+a.gstreamer]
if a.icon:args+=['-DDOROTI_APP_ICON='+str(a.icon.resolve())]
subprocess.run(args,check=True,timeout=1200);subprocess.run(['cmake','--build',str(out),'--config',a.configuration],check=True,timeout=1200)
