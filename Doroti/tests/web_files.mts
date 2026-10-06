import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { stripTypeScriptTypes } from 'node:module';
import { test } from 'node:test';
import vm from 'node:vm';
import { randomUUID } from 'node:crypto';

async function fixture() {
  let active = true;
  const inputs: any[] = [];
  class Input {
    type = ''; multiple = false; accept = ''; hidden = false; removed = false;
    files: File[] = []; onchange: (() => void) | null = null; oncancel: (() => void) | null = null;
    click() {
      assert.equal(active, true, 'Picker must open synchronously inside the trusted gesture');
      inputs.push(this);
    }
    remove() { this.removed = true; }
  }
  const context = vm.createContext({ File, Uint8Array, DOMException, crypto: { randomUUID },
    navigator: { userActivation: { get isActive() { return active; } } },
    document: { createElement: () => new Input(), body: { append() {} } }, btoa,
  });
  const source = await readFile(new URL('../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.files.ts', import.meta.url), 'utf8');
  const module = new vm.SourceTextModule(stripTypeScriptTypes(source), { context });
  await module.link(() => { throw new Error('Unexpected import'); });
  await module.evaluate();
  const api = module.namespace as any;
  api.openFileOwner(1);
  const boundary = { dataset: { dorotiSemanticsIdentifier: 'upload' }, parentElement: null };
  const control = { isConnected: true, dataset: {}, parentElement: boundary,
    getAttribute: (name: string) => name === 'role' ? 'button' : null };
  api.registerFileActivation(1, 'upload', true, '.txt');
  return { api, inputs, control, setActive(value: boolean) { active = value; } };
}

test('trusted tap opens the filtered picker before Worker admission and late selection produces bounded read grants', async () => {
  const { api, inputs, control, setActive } = await fixture();
  assert.equal(api.reserveBrowserPicker(1, control), true);
  assert.equal(inputs.length, 1);
  assert.equal(inputs[0].multiple, true);
  assert.equal(inputs[0].accept, '.txt');
  setActive(false); // Model the Worker hop losing WebKit gesture activation.
  const reply = api.pickBrowserFiles(1, true, '.txt');
  inputs[0].files = [new File(['hello 한글'], 'chosen.txt')];
  inputs[0].onchange();
  const result = JSON.parse(await reply);
  assert.equal(result.status, 'selected');
  assert.equal(result.files[0].name, 'chosen.txt');
  assert.equal(Buffer.from(await api.readBrowserFile(1, result.files[0].token, 0, 65536)).toString(), 'hello 한글');
  await assert.rejects(api.readBrowserFile(1, result.files[0].token, 0, 65537), /bounded/);
  api.releaseBrowserFile(1, result.files[0].token);
  await assert.rejects(api.readBrowserFile(1, result.files[0].token, 0, 1), /revoked/);
  assert.equal(inputs[0].removed, true);
});

test('selection and cancellation completed before the Worker request are consumed once', async () => {
  for (const selected of [true, false]) {
    const { api, inputs, control, setActive } = await fixture();
    api.reserveBrowserPicker(1, control);
    if (selected) { inputs[0].files = [new File(['a'], 'a.txt')]; inputs[0].onchange(); }
    else inputs[0].oncancel();
    setActive(false);
    assert.equal(JSON.parse(await api.pickBrowserFiles(1, true, '.txt')).status, selected ? 'selected' : 'cancelled');
    assert.equal(JSON.parse(await api.pickBrowserFiles(1, true, '.txt')).status, 'denied');
  }
});

test('disabled, detached, unrelated and unregistered controls never open a picker', async () => {
  const { api, inputs, control } = await fixture();
  for (const candidate of [null, { ...control, isConnected: false }, { ...control, parentElement: null },
    { ...control, getAttribute: (name: string) => name === 'role' ? 'button' : 'true' }])
    assert.equal(api.reserveBrowserPicker(1, candidate), false);
  api.unregisterFileActivation(1, 'upload');
  assert.equal(api.reserveBrowserPicker(1, control), false);
  assert.equal(inputs.length, 0);
});

test('concurrent and mismatched requests cannot consume another pending picker', async () => {
  const { api, inputs, control } = await fixture();
  api.reserveBrowserPicker(1, control);
  assert.equal(JSON.parse(await api.pickBrowserFiles(1, false, '')).status, 'failed');
  const reply = api.pickBrowserFiles(1, true, '.txt');
  assert.equal(JSON.parse(await api.pickBrowserFiles(1, true, '.txt')).status, 'failed');
  assert.equal(api.reserveBrowserPicker(1, control), false);
  inputs[0].oncancel();
  assert.equal(JSON.parse(await reply).status, 'cancelled');
});

test('cancellation and unregister revoke even a selection awaiting managed admission', async () => {
  for (const revoke of ['cancelBrowserPicker', 'unregisterFileActivation', 'closeFileOwner']) {
    const { api, inputs, control } = await fixture();
    api.reserveBrowserPicker(1, control);
    const reply = api.pickBrowserFiles(1, true, '.txt');
    inputs[0].files = [new File(['a'], 'a.txt')]; inputs[0].onchange();
    api[revoke](1, 'upload');
    assert.equal(JSON.parse(await reply).status, 'cancelled');
  }
});

test('cancelled activation can be retried and no unclaimed result survives unregister or view disposal', async () => {
  const { api, inputs, control, setActive } = await fixture();
  api.reserveBrowserPicker(1, control);
  api.cancelBrowserPicker(1);
  assert.equal(inputs[0].removed, true);
  assert.equal(api.reserveBrowserPicker(1, control), true);
  inputs[1].files = [new File(['a'], 'a.txt')]; inputs[1].onchange();
  api.unregisterFileActivation(1, 'upload');
  setActive(false);
  assert.equal(JSON.parse(await api.pickBrowserFiles(1, true, '.txt')).status, 'denied');
  api.closeFileOwner(1);
  assert.equal(JSON.parse(await api.pickBrowserFiles(1, true, '.txt')).status, 'unsupported');
});
