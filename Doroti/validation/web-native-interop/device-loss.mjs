import { readFileSync } from 'node:fs';
import { runInNewContext } from 'node:vm';
import { test } from 'node:test';
import assert from 'node:assert/strict';

const source = readFileSync(new URL('../../src/Doroti.Target.Web.browser-wasm/build/DorotiSkiaInterop.js', import.meta.url), 'utf8');
function fixture(mapped) {
  const freed = [];
  const heap = new Uint8Array(128).fill(0xaa);
  const context = {
    Uint8Array, Module: {}, HEAPU8: heap,
    LibraryManager: { library: { emwgpuBufferGetMappedRange() {} } },
    autoAddDeps() {}, mergeInto: Object.assign,
    WebGPU: { getJsObject: () => ({ getMappedRange: () => mapped }), Internals: { bufferOnUnmaps: { 1: [] } } },
    _memalign: () => 32, _free: p => freed.push(p),
  };
  runInNewContext(source, context);
  return { ...context, freed, map: context.LibraryManager.library.emwgpuBufferGetMappedRange };
}

test('mapped writes reach a live GPU range and free staging memory once', () => {
  const mapped = new ArrayBuffer(8);
  const f = fixture(mapped);
  assert.equal(f.map(1, 0, 8), 32);
  assert.deepEqual([...f.HEAPU8.slice(32, 40)], Array(8).fill(0));
  assert.equal(f.HEAPU8[40], 0xaa);
  f.HEAPU8.set([1, 2, 3, 4, 5, 6, 7, 8], 32);
  f.WebGPU.Internals.bufferOnUnmaps[1][0]();
  assert.deepEqual([...new Uint8Array(mapped)], [1, 2, 3, 4, 5, 6, 7, 8]);
  assert.deepEqual(f.freed, [32]);
  assert.equal(f.Module.dorotiWebGpuMappedRanges.active, 0);
  assert.equal(f.Module.dorotiWebGpuMappedRanges.discarded, 0);
});

test('device-loss detachment still releases WASM staging memory', () => {
  const mapped = new ArrayBuffer(8);
  const f = fixture(mapped);
  f.map(1, 0, 8);
  structuredClone(mapped, { transfer: [mapped] });
  assert.equal(mapped.byteLength, 0);
  assert.doesNotThrow(() => f.WebGPU.Internals.bufferOnUnmaps[1][0]());
  assert.deepEqual(f.freed, [32]);
  assert.equal(f.Module.dorotiWebGpuMappedRanges.active, 0);
  assert.equal(f.Module.dorotiWebGpuMappedRanges.discarded, 1);
});
