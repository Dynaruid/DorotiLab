import { test } from 'node:test';
import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as os from 'node:os';
import * as path from 'node:path';
import { prepareFileReload, ReloadRequest } from '../fileReload';

const request: ReloadRequest = { schemaVersion: 'doroti.dev/v1', sessionId: 'session', runtimeId: 'runtime', requestId: 'request' };
const pause = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));
async function fixture(action: (directory: string) => Promise<void>) {
    const directory = await fs.mkdtemp(path.join(os.tmpdir(), 'doroti-reload-'));
    try { await action(directory); } finally { await fs.rm(directory, { recursive: true, force: true }); }
}

test('device reload saves only after its complete request reaches the matching runtime', async () => fixture(async directory => {
    let accepted = false;
    const preparing = prepareFileReload(directory, request, true, () => true, 2000).then(result => { accepted = result; });
    for (let i = 0; i < 100; i++) {
        try { assert.deepEqual(JSON.parse(await fs.readFile(path.join(directory, 'request.json'), 'utf8')), request); break; }
        catch (error) { if ((error as NodeJS.ErrnoException).code !== 'ENOENT') throw error; await pause(10); }
    }
    for (const stale of ['{', JSON.stringify({ ...request, sessionId: 'old' }), JSON.stringify({ ...request, runtimeId: 'old' }),
        JSON.stringify({ ...request, requestId: 'old' }), JSON.stringify({ ...request, schemaVersion: 'old' })]) {
        await fs.writeFile(path.join(directory, 'prepared.json'), stale);
        await pause(60);
        assert.equal(accepted, false);
    }
    await fs.writeFile(path.join(directory, 'prepared.json'), JSON.stringify(request));
    await preparing;
    assert.equal(accepted, true);
    assert.deepEqual((await fs.readdir(directory)).sort(), ['prepared.json', 'request.json']);
}));

test('Stop, disconnection or runtime replacement cancels preparation without saving', async () => fixture(async directory => {
    let active = true;
    let saves = 0;
    const preparing = prepareFileReload(directory, request, true, () => active, 2000).then(result => { if (result) saves++; });
    await pause(75);
    active = false;
    await fs.writeFile(path.join(directory, 'prepared.json'), JSON.stringify(request));
    await preparing;
    assert.equal(saves, 0);
}));

test('missing device acknowledgment times out and local Mac sessions need no relay', async () => fixture(async directory => {
    await assert.rejects(prepareFileReload(directory, request, true, () => true, 60), /Edits have not been saved/);
    assert.equal(await prepareFileReload(directory, request, false, () => true), true);
    assert.equal(await prepareFileReload(directory, request, false, () => false), false);
}));
