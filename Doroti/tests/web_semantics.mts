import assert from 'node:assert/strict';
import { test } from 'node:test';
import { BrowserSemanticsState, type SemanticsNode, type SemanticsPacket } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.semantics.ts';

const node = (id: number, children: number[] = []): SemanticsNode => ({ id, children, rect: [id, id, id + 10, id + 10], label: `node ${id}` });
const packet = (revision: number, nodes: SemanticsPacket['nodes'], extra: Partial<SemanticsPacket> = {}): SemanticsPacket => ({
  schemaVersion: 1, kind: revision === 1 ? 'full' : 'delta', stream: 1, revision, baseRevision: revision - 1,
  generation: revision, nodes, removed: [], ...(revision === 1 ? { roots: [0] } : {}), ...extra,
});
const applied = (value: ReturnType<BrowserSemanticsState['apply']>) => {
  assert.notEqual(typeof value, 'string'); return value as Exclude<typeof value, string>;
};

test('parent motion visits its children but preserves content, siblings and traversal', () => {
  const state = new BrowserSemanticsState();
  state.apply(packet(1, [node(0, [1, 2]), node(1, [3]), node(2), node(3)]));
  const sibling = state.nodes.get(2), child = state.nodes.get(3);
  const plan = applied(state.apply(packet(2, [{ id: 1, contentUnchanged: true, rect: [10, 20, 30, 40] }])));
  assert.deepEqual([...plan.changed].sort(), [1, 3]);
  assert.equal(plan.content.size, 0); assert.equal(plan.parents.size, 0); assert.equal(plan.rootsChanged, false);
  assert.equal(state.nodes.get(2), sibling); assert.equal(state.nodes.get(3), child);
  assert.deepEqual(state.nodes.get(1)?.children, [3]); assert.equal(state.parents.get(3), 1);
});

test('reparent, reorder, removal and kind changes retain the complete tree', () => {
  const state = new BrowserSemanticsState();
  state.apply(packet(1, [node(0, [1, 2]), node(1, [3, 4]), node(2), node(3), node(4)]));
  const child = state.nodes.get(3);
  const move = applied(state.apply(packet(2, [{ id: 1, contentUnchanged: true, children: [4] },
    { id: 2, contentUnchanged: true, children: [3] }], { roots: [0] })));
  assert.equal(state.nodes.get(3), child); assert.equal(state.parents.get(3), 2);
  assert.ok(move.changed.has(3)); assert.ok(move.parents.has(1) && move.parents.has(2));
  applied(state.apply(packet(3, [{ id: 0, contentUnchanged: true, children: [2, 1] }], { roots: [0] })));
  assert.deepEqual(state.nodes.get(0)?.children, [2, 1]);
  applied(state.apply(packet(4, [{ ...node(3), flags: { textField: true, multiline: true }, value: '한글' }])));
  assert.equal(state.nodes.get(3)?.flags?.multiline, true);
  const remove = applied(state.apply(packet(5, [{ id: 0, contentUnchanged: true, children: [2] }], { removed: [1, 4], roots: [0] })));
  assert.deepEqual(remove.removed, [1, 4]); assert.equal(state.nodes.size, 3);
  assert.equal(state.parents.has(4), false); assert.equal(state.parents.get(3), 2);
});

test('identifier changes update untouched aria-controls dependents', () => {
  const state = new BrowserSemanticsState();
  state.apply(packet(1, [node(0, [1, 2]), { ...node(1), controlsNodes: ['details'] }, { ...node(2), identifier: 'details' }]));
  const plan = applied(state.apply(packet(2, [{ ...node(2), identifier: 'other' }])));
  assert.ok(plan.content.has(1)); assert.equal(state.identifiers.has('details'), false);
  const added = applied(state.apply(packet(3, [node(0, [1, 2, 3]), { ...node(3), identifier: 'details' }], { roots: [0] })));
  assert.ok(added.content.has(1)); assert.equal(state.identifiers.get('details'), 3);
  const removed = applied(state.apply(packet(4, [node(0, [1, 2])], { removed: [3], roots: [0] })));
  assert.ok(removed.content.has(1)); assert.equal(state.identifiers.has('details'), false);
});

test('gaps request one full snapshot; stale, duplicate and retired streams cannot overwrite it', () => {
  const state = new BrowserSemanticsState();
  state.apply(packet(1, [node(0)]));
  assert.equal(state.apply(packet(3, [{ ...node(0), label: 'missing baseline' }])), 'resync');
  assert.equal(state.nodes.get(0)?.label, 'node 0');
  assert.equal(state.apply(packet(4, [])), 'waiting');
  assert.equal(state.apply(packet(2, [])), 'waiting');
  applied(state.apply(packet(5, [{ ...node(0), label: 'recovered' }], { kind: 'full', roots: [0], baseRevision: 0 })));
  assert.equal(state.apply(packet(5, [])), 'stale');
  assert.equal(state.apply(packet(4, [])), 'stale');
  applied(state.apply(packet(1, [node(0)], { stream: 2 })));
  assert.equal(state.apply(packet(6, [], { kind: 'full', roots: [], baseRevision: 0 })), 'stale');
  assert.equal(state.nodes.size, 1);
});

test('missing compact baselines recover without partial mutation, and clear deletes all state', () => {
  const state = new BrowserSemanticsState();
  assert.equal(state.apply(packet(2, [])), 'resync');
  applied(state.apply(packet(3, [node(0), { ...node(1), identifier: 'editor' }], { kind: 'full', roots: [0, 1], baseRevision: 0 })));
  assert.equal(state.apply(packet(4, [node(0), { id: 99, contentUnchanged: true, rect: [0, 0, 1, 1] }])), 'resync');
  assert.equal(state.nodes.size, 2);
  applied(state.apply(packet(5, [], { kind: 'full', roots: [], baseRevision: 0 })));
  assert.equal(state.nodes.size, 0); assert.equal(state.parents.size, 0); assert.equal(state.identifiers.size, 0);
});

test('one-node deltas in a 1001-node tree retain unrelated node objects and visit one node', () => {
  const state = new BrowserSemanticsState();
  state.apply(packet(1, [node(0, Array.from({ length: 1000 }, (_, i) => i + 1)), ...Array.from({ length: 1000 }, (_, i) => node(i + 1))]));
  const retained = state.nodes.get(999);
  const plan = applied(state.apply(packet(2, [{ id: 42, contentUnchanged: true, rect: [50, 50, 60, 60] }])));
  assert.equal(plan.changed.size, 1); assert.equal(plan.parents.size, 0); assert.equal(plan.content.size, 0);
  assert.equal(state.nodes.get(999), retained); assert.equal(state.nodes.size, 1001);
});
