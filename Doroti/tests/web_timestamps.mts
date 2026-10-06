import assert from 'node:assert/strict';
import { test } from 'node:test';
import { normalizeBrowserTimestamp } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.timestamps.ts';

test('browser input retains exact high-resolution and legacy epoch timestamps', () => {
  for (const timestamp of [0, .125, 12345.6789, 1791279296455])
    assert.equal(normalizeBrowserTimestamp(timestamp, () => { assert.fail('Valid clock should be retained'); }), timestamp);
});

test('invalid and unrepresentable timestamps use the browser clock before TimeSpan conversion', () => {
  for (const timestamp of [NaN, Infinity, -Infinity, -1, Number.MAX_VALUE, 922_337_203_685_478])
    assert.equal(normalizeBrowserTimestamp(timestamp, () => 2345.125), 2345.125);
});

test('an invalid replacement clock cannot reintroduce a nonfinite or overflowing timestamp', () => {
  for (const replacement of [NaN, Infinity, -1, Number.MAX_VALUE])
    assert.equal(normalizeBrowserTimestamp(Infinity, () => replacement), 0);
});
