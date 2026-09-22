import { mkdir, readFile, writeFile, open, unlink } from 'node:fs/promises';
import { resolve } from 'node:path';
import { spawn } from 'node:child_process';

// Each invocation consumes a slot before starting, including failed launches.
// GPU/browser invocations must remain sequential. The caller uses the repository
// 20-minute process-tree wrapper; this script adds no retries.
const [out, product, backend = 'auto', mode = 'memory', mobile = '1'] = process.argv.slice(2);
if (!out || !product) throw Error('run.mjs OUTPUT WWWROOT [backend] [memory|memory-off|memory-soak|smoke|minimal|textures|webview] [mobile 0|1]');
if (!['memory', 'memory-off', 'memory-soak', 'memory-soak-off', 'minimal', 'smoke', 'textures', 'webview'].includes(mode)) throw Error('Unknown validation mode');
if (!['0', '1'].includes(mobile)) throw Error('Mobile policy must be 0 or 1');
const ledger = resolve('Doroti/artifacts/web-memory/runs.json');
await mkdir(resolve('Doroti/artifacts/web-memory'), { recursive: true });
const lock = await open(ledger + '.lock', 'wx');
try {
const runs = await readFile(ledger, 'utf8').then(JSON.parse, e => { if (e.code === 'ENOENT') return []; throw e; });
if (runs.length >= 30) throw Error('M0–M5 browser/device budget exhausted (30 runs, no retries)');
if (runs.some(r => r.out === resolve(out))) throw Error('Output directory already used');
const entry = { run: runs.length + 1, out: resolve(out), product: /^https?:/.test(product) ? product : resolve(product), backend, mode, mobile,
  started: new Date().toISOString(), status: 'RUNNING' };
runs.push(entry);
await writeFile(ledger, JSON.stringify(runs, null, 2));
const contract = mode === 'textures' || mode === 'webview';
const args = contract ? ['Doroti/validation/web-memory/run-contract.mjs', mode, out, product, backend] :
  ['Doroti/validation/web-frame-cost/run.mjs', out, product, mode, backend, mobile === '1' ? '390' : '1280', mobile === '1' ? '3' : '1'];
const child = spawn(process.execPath, args, {
  stdio: 'inherit', env: { ...process.env, DOROTI_MEMORY_MOBILE: mobile },
});
const code = await new Promise((resolve, reject) => { child.on('error', error => { entry.error = String(error); resolve(1); }); child.on('exit', resolve); });
entry.ended = new Date().toISOString(); entry.exitCode = code; entry.status = code === 0 ? 'OBSERVED' : 'FAILED';
await writeFile(ledger, JSON.stringify(runs, null, 2));
process.exitCode = code ?? 1;

} finally { await lock.close(); await unlink(ledger + '.lock'); }
