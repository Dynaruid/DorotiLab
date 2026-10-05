"""Typed service regression: managed boundaries must bypass byte codecs."""
from pathlib import Path
from datetime import datetime,timezone
import json,subprocess,sys,uuid,shutil
root=Path(__file__).resolve().parents[2];runs=root/'temp/testing/platform-decoupling/typed-transport';run=runs/uuid.uuid4().hex;run.mkdir(parents=True)
command=[sys.executable,str(root/'Doroti/eng/run-with-timeout.py'),'--timeout','1200','dotnet','run','--project',str(root/'Doroti/tests/Doroti.Platform.Contracts.Tests'),'-c','Debug','--artifacts-path',str(run/'build'),'--','--transport']
with (run/'typed.log').open('w',encoding='utf-8') as log: result=subprocess.run(command,cwd=root,stdout=log,stderr=subprocess.STDOUT)
output=(run/'typed.log').read_text(encoding='utf-8',errors='replace');print(output,flush=True)
if result.returncode==0:
 evidence={'schema':'doroti.typed-transport-verification/v1','verifiedUtc':datetime.now(timezone.utc).isoformat(),'result':'PASS','scope':'real managed/headless typed service invocation, owner selection, unsupported/error, cancellation and lease-retained actual completion; codec traps remain unused','results':[line for line in output.splitlines() if line.startswith('PASS:')],'notVerified':['provider-native sound/haptic actuation','NativeAOT/trim','physical devices and all external ABI conversions'],'runId':run.name}
 (root/'Doroti/docs/migrations/design-platform/typed-transport-verification.json').write_text(json.dumps(evidence,indent=2)+'\n',encoding='utf-8');assert run.resolve().parent==runs.resolve();shutil.rmtree(run)
sys.exit(result.returncode)
