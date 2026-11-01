"""Summarize traced UP-to-scrollEnd CPU/submission windows, excluding idle flushes.
Only complete trace windows are accepted. This is not displayed FPS.
"""
import argparse,json,importlib.util
from pathlib import Path
spec=importlib.util.spec_from_file_location('h',Path(__file__).with_name('measure-android-resume-scroll.py'));h=importlib.util.module_from_spec(spec);spec.loader.exec_module(h)
parser=argparse.ArgumentParser(description=__doc__);parser.add_argument('manifests',type=Path,nargs='+');parser.add_argument('--output',type=Path,required=True);args=parser.parse_args();reports=[]
for path in args.manifests:
 root=path.parent;manifest=json.loads(path.read_text());label=manifest['label']
 for trial in manifest['trials']:
  d=json.loads((root/(label+'-'+str(trial['trial'])+'.json')).read_text());trace=d['trace'];lo=min(t['recordedAtMicroseconds'] for t in trace)
  for stage in trial['stages']:
   gesture=stage['summary']['gesture'];up=next(i for i in d['inputs'] if i['gesture']==gesture and i['action']==1)
   if lo>up['atUs']: continue
   updates=[t for t in trace if t['phase']==22 and t['scrollActivity']=='BallisticScrollActivity' and up['atUs']<t['recordedAtMicroseconds']<up['atUs']+3e6]
   if not updates: continue
   from collections import Counter
   pos=Counter(t['scrollPositionId'] for t in updates).most_common(1)[0][0];updates=[t for t in updates if t['scrollPositionId']==pos]
   ends=[t for t in trace if t['phase']==23 and t['scrollPositionId']==pos and updates[-1]['recordedAtMicroseconds']<=t['recordedAtMicroseconds']<up['atUs']+3e6]
   if not ends:continue
   stop=ends[0]['recordedAtMicroseconds'];end=next((f['atUs'] for f in d['frames'] if f['atUs']>=stop and f['submitted']),None)
   if end is None:continue
   fs=[f for f in d['frames'] if up['atUs']<f['atUs']<=end];submitted=[f for f in fs if f['submitted']]
   gaps=[(b['vsyncNanos']-a['vsyncNanos'])/1e6 for a,b in zip(submitted,submitted[1:])]
   r={'label':label,'trial':trial['trial'],'stage':stage['name'],'gesture':gesture,'window':'UP through the first submission completing the recorded scrollEnd frame','durationMs':round((end-up['atUs'])/1000,3),'submitted':len(submitted),'ownerMs':h.stats([f['ownerMs'] for f in fs]),'callbackMs':h.stats([f['callbackMs'] for f in fs]),'allocated':h.stats([f['allocated'] for f in fs]),'gc':{g:fs[-1][g]-fs[0][g] for g in ['gen0','gen1','gen2']},'nativeGapMs':h.stats(gaps),'nativeGapsOver12ms':sum(g>12 for g in gaps),'semanticsBuilds':sum(t['phase']==27 and up['atUs']<t['recordedAtMicroseconds']<=end for t in trace),'offsetStart':updates[0]['scrollOffset'],'offsetEnd':updates[-1]['scrollOffset'],'extentMax':updates[-1]['scrollMaxExtent'],'updates':len(updates)}
   reports.append(r);print(label,trial['trial'],stage['name'],'n',len(submitted),'p95',r['ownerMs']['p95'],'max',r['ownerMs']['max'],'alloc',r['allocated']['p50'],'GC',r['gc']['gen0'],'sem',r['semanticsBuilds'],'gaps12',r['nativeGapsOver12ms'])
args.output.parent.mkdir(parents=True,exist_ok=True);args.output.write_text(json.dumps(reports,indent=2),encoding='utf-8')
