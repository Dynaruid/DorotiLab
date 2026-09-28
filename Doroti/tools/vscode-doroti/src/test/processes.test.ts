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
test('Stop terminates the owned subprocess tree', async () => {
    let descendant = 0;
    const root = start(process.execPath, ['-e', `const {spawn}=require('node:child_process'); const p=spawn(process.execPath,['-e','setInterval(()=>{},1000)'],{stdio:'ignore',windowsHide:true}); console.log(p.pid); setInterval(()=>{},1000);`], process.cwd(), value => descendant = Number(value.trim()));
    const ended = completion(root).catch(() => {});
    const deadline = Date.now() + 5000;
    while (!descendant && Date.now() < deadline) await new Promise(resolve => setTimeout(resolve, 25));
    assert.ok(descendant); await stop(root); await ended;
    assert.throws(() => process.kill(descendant, 0), /ESRCH/);
});
