import { test } from 'node:test';
import * as assert from 'node:assert/strict';
import { start, stop, completion } from '../processes';
test('process argv preserves Korean, spaces, quotes and shell metacharacters', async () => {
    let output = '';
    const values = ['한글 경로', 'a"b', 'x;echo nope', '$(not-a-command)'];
    const child = start(process.execPath, ['-e', 'process.stdout.write(JSON.stringify(process.argv.slice(1)))', '--', ...values], process.cwd(), value => output += value);
    await completion(child); assert.deepEqual(JSON.parse(output), values);
});
test('nonzero exit and missing tool propagate errors', async () => {
    await assert.rejects(completion(start(process.execPath, ['-e', 'process.exit(7)'], process.cwd(), () => {})), /7/);
    await assert.rejects(completion(start('doroti-tool-does-not-exist', [], process.cwd(), () => {})), /ENOENT/);
});
test('structured CLI stdout stays separate from localized build diagnostics on stderr', async () => {
    let output = ''; let diagnostics = '';
    const child = start(process.execPath, ['-e', 'process.stderr.write("복원할 프로젝트를 확인하는 중...\\n"); process.stdout.write(JSON.stringify({schemaVersion:"doroti.cli-workspace/v2"}));'],
        process.cwd(), value => output += value, value => diagnostics += value);
    await completion(child);
    assert.equal(JSON.parse(output).schemaVersion, 'doroti.cli-workspace/v2');
    assert.match(diagnostics, /복원할 프로젝트/);
});
test('Stop terminates the owned subprocess tree', async () => {
    let descendant = 0;
    const root = start(process.execPath, ['-e', `const {spawn}=require('node:child_process'); const p=spawn(process.execPath,['-e','setInterval(()=>{},1000)'],{stdio:'ignore',windowsHide:true}); console.log(p.pid); setInterval(()=>{},1000);`], process.cwd(), value => descendant = Number(value.trim()));
    const ended = completion(root).catch(() => {});
    const deadline = Date.now() + 5000;
    while (!descendant && Date.now() < deadline) await new Promise(resolve => setTimeout(resolve, 25));
    assert.ok(descendant); await stop(root); await ended;
    assert.throws(() => process.kill(descendant, 0), /ESRCH/);
});
test('Unix Stop waits for descendants that outlive the launcher', { skip: process.platform === 'win32' }, async () => {
    let descendant = 0;
    const script = `process.on('SIGTERM', () => setTimeout(() => process.exit(0), 250)); console.log(process.pid); setInterval(() => {}, 1000);`;
    const root = start(process.execPath, ['-e', `const {spawn}=require('node:child_process'); const p=spawn(process.execPath,['-e',${JSON.stringify(script)}],{stdio:['ignore','pipe','ignore']}); p.stdout.pipe(process.stdout); setInterval(()=>{},1000);`], process.cwd(), value => descendant = Number(value.trim()));
    const ended = completion(root).catch(() => {});
    try {
        const deadline = Date.now() + 10000;
        while (!descendant && Date.now() < deadline) await new Promise(resolve => setTimeout(resolve, 25));
        assert.ok(descendant);
        await stop(root); await ended;
        assert.throws(() => process.kill(descendant, 0), /ESRCH/);
    } finally { if (root.pid) { try { process.kill(-root.pid, 'SIGKILL'); } catch { /* already stopped */ } } }
});
