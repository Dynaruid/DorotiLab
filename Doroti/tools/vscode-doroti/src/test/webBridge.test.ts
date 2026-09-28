import { test } from 'node:test';
import * as assert from 'node:assert/strict';
import { randomUUID } from 'node:crypto';
import { WebBridge } from '../webBridge';

test('Web reload waits for the current browser to prepare its request before saving', async () => {
    const session = randomUUID(); const runtimeId = randomUUID();
    const bridge = await WebBridge.start(session);
    try {
        const appUrl = new URL(bridge.browserUrl('http://127.0.0.1:5088/base/'));
        assert.equal(appUrl.searchParams.get('dorotiDevSession'), session);
        const headers = { Origin: appUrl.origin, 'Content-Type': 'application/json' };
        assert.equal((await fetch(bridge.url + '/poll')).status, 403);
        assert.equal((await fetch(bridge.url + '/poll', { headers: { Origin: 'https://unrelated.invalid' } })).status, 403);
        const state = { schemaVersion: 'doroti.dev/v1', sessionId: session, runtimeId, processId: 1, revision: 0, supported: true, status: 'ready' };
        assert.equal((await fetch(bridge.url + '/status', { method: 'POST', headers, body: JSON.stringify({ ...state, sessionId: 'stale' }) })).status, 400);
        assert.equal((await fetch(bridge.url + '/status', { method: 'POST', headers, body: JSON.stringify(state) })).status, 204);
        const requestId = randomUUID(); let prepared = false;
        const pending = bridge.prepare(runtimeId, requestId).then(() => { prepared = true; });
        const poll = await (await fetch(bridge.url + '/poll', { headers })).json() as { request: unknown };
        assert.deepEqual(poll.request, { runtimeId, requestId }); assert.equal(prepared, false);
        assert.equal((await fetch(bridge.url + '/prepared', { method: 'POST', headers, body: JSON.stringify({ runtimeId, requestId: randomUUID() }) })).status, 409);
        assert.equal(prepared, false);
        assert.equal((await fetch(bridge.url + '/prepared', { method: 'POST', headers, body: JSON.stringify({ runtimeId, requestId }) })).status, 204);
        await pending; assert.equal(prepared, true);
        const stopping = assert.rejects(bridge.prepare(runtimeId, randomUUID()), /stopped/);
        await bridge.close(); await stopping;
        assert.equal(bridge.runtime, undefined);
    } finally { await bridge.close(); }
});
