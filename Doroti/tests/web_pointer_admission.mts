import assert from 'node:assert/strict';
import { test } from 'node:test';
import { PointerMoveAdmission } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.pointer-admission.ts';

function motion(sequence: number, payload = {}, hostId = 1) {
  return { inputKind:'pointer', hostId, inputSequence:sequence, ingressEpochMilliseconds:sequence,
    payload:{ phase:0, kind:0, pointerId:1, buttons:1, modifiers:0,
      samples:[sequence, sequence*2, .5, 0, 0, 0, sequence*10], ...payload } };
}
const inputs = (sent: any[]) => sent.flatMap(input => input.inputBatch ?? input.pointerBatch ?? [input]);

test('motion dispatch backpressure retains all positions, pressure and timestamps in order', () => {
  const sent: ReturnType<typeof motion>[] = [];
  const queue = new PointerMoveAdmission(input => sent.push(input as any));
  for (let sequence=1;sequence<=30;sequence++) queue.push(motion(sequence));
  assert.equal(sent.length,1);
  queue.acknowledge(999); assert.equal(sent.length,1);
  queue.acknowledge(1);
  assert.equal(sent.length,2); assert.equal(sent[1].inputSequence,30);
  assert.deepEqual(inputs(sent).map(input=>input.inputSequence),Array.from({length:30},(_,i)=>i+1));
  assert.deepEqual(inputs(sent).flatMap(input => input.payload.samples),
    Array.from({length:30},(_,i)=>motion(i+1).payload.samples).flat());
  assert.equal(sent[1].ingressEpochMilliseconds,30);
  queue.acknowledge(1); assert.equal(sent.length,2);
});

test('release/cancel/key/focus/text/semantics/wheel boundaries flush older motion before the event', () => {
  for (const inputKind of ['pointer','key','focus','text','semantics-action','wheel']) {
    for (const phase of [2,3]) {
      const sent: any[]=[];
      const queue=new PointerMoveAdmission(input=>sent.push(input));
      queue.push(motion(1)); queue.push(motion(2)); queue.push(motion(3));
      queue.push({...motion(4,{phase}), inputKind});
      assert.deepEqual(sent.map(input=>input.inputSequence),[1,3,4]);
      assert.deepEqual(inputs([sent[1]]).flatMap(input=>input.payload.samples),[...motion(2).payload.samples,...motion(3).payload.samples]);
      queue.acknowledge(1); assert.equal(sent.length,3);
    }
  }
});

test('different pointer/host/phase/buttons/modifiers never merge and size pressure flushes without losing samples', () => {
  for (const other of [motion(4,{pointerId:2}),motion(4,{},2),motion(4,{phase:4}),
      motion(4,{buttons:2}),motion(4,{modifiers:1}),motion(4,{kind:2})]) {
    const sent:any[]=[]; const queue=new PointerMoveAdmission(input=>sent.push(input),2);
    queue.push(motion(1)); queue.push(motion(2)); queue.push(motion(3)); queue.push(other); queue.flush();
    assert.deepEqual(sent.map(input=>input.inputSequence),[1,3,4]);
    assert.equal(inputs(sent).flatMap(input=>input.payload.samples).length,4*7);
  }
  const sent:any[]=[]; const queue=new PointerMoveAdmission(input=>sent.push(input),2);
  for(let sequence=1;sequence<=30;sequence++) queue.push(motion(sequence)); queue.flush();
  assert.ok(sent.every(input=>inputs([input]).flatMap(message=>message.payload.samples).length<=14));
  assert.deepEqual(inputs(sent).flatMap(input=>input.payload.samples),
    Array.from({length:30},(_,i)=>motion(i+1).payload.samples).flat());
});

test('owner replacement discards pending motion and old acknowledgments; transport failure releases admission', () => {
  const sent:any[]=[]; const queue=new PointerMoveAdmission(input=>sent.push(input));
  queue.push(motion(1)); queue.push(motion(2)); queue.reset(); queue.acknowledge(1);
  queue.enable(); queue.push(motion(3)); assert.deepEqual(sent.map(input=>input.inputSequence),[1,3]);
  let failed=true;
  const failure=new PointerMoveAdmission(input=>{ if(failed) throw new Error('closed port'); sent.push(input); });
  assert.throws(()=>failure.push(motion(4)),/closed port/);
  failed=false; failure.push(motion(5)); assert.equal(sent.at(-1).inputSequence,5);
});

test('mixed-version endpoints stay on immediate input until the owner advertises admission support', () => {
  const sent:any[]=[]; const queue=new PointerMoveAdmission(input=>sent.push(input),128,false);
  queue.push(motion(1)); queue.push(motion(2));
  assert.deepEqual(sent.map(input=>input.inputSequence),[1,2]);
  assert.ok(sent.every(input=>!input.pointerAdmission && !input.pointerBatch));
  queue.enable(); queue.push(motion(3)); queue.push(motion(4)); queue.push(motion(5));
  assert.equal(sent.at(-1).pointerAdmission,true);
  queue.acknowledge(3);
  assert.deepEqual(inputs(sent).map(input=>input.inputSequence),[1,2,3,4,5]);
});

function wheel(sequence:number, payload={}) {
  return {inputKind:'wheel',hostId:1,inputSequence:sequence,ingressEpochMilliseconds:sequence,
    payload:{x:20,y:40,deltaX:sequence/10,deltaY:(sequence%2?-1:1)*sequence/3,
      timestamp:sequence*6,kind:3,signalKind:1,scale:1,...payload}};
}
test('continuous scroll preserves every delta, timestamp and contiguous sequence instead of replaying one frame per event', () => {
  const sent:any[]=[];const queue=new PointerMoveAdmission(input=>sent.push(input));queue.enableWheel();
  const original=Array.from({length:30},(_,i)=>wheel(i+1));original.forEach(input=>queue.push(input));
  assert.equal(sent.length,1);queue.acknowledge(1);assert.equal(sent.length,2);
  assert.equal(sent[1].wheelAdmission,true);
  assert.deepEqual(inputs(sent).map(({wheelAdmission,...input})=>input),original);
  assert.equal(inputs(sent).reduce((sum,input)=>sum+input.payload.deltaY,0),original.reduce((sum,input)=>sum+input.payload.deltaY,0));
});
test('scroll targets, pinch kind and ordered key/up boundaries cannot overtake pending wheel history', () => {
  for(const boundary of [wheel(4,{x:21}),wheel(4,{signalKind:4,scale:1.2}),wheel(4,{kind:0}),
      {...wheel(4),inputKind:'key'},motion(4,{phase:2})]) {
    const sent:any[]=[];const queue=new PointerMoveAdmission(input=>sent.push(input));queue.enableWheel();
    queue.push(wheel(1));queue.push(wheel(2));queue.push(wheel(3));queue.push(boundary);queue.flush();
    assert.deepEqual(inputs(sent).map(input=>input.inputSequence),[1,2,3,4]);
    assert.deepEqual(inputs(sent).slice(0,3).map(({wheelAdmission,...input})=>input),[wheel(1),wheel(2),wheel(3)]);
  }
});
test('wheel capability is negotiated separately from legacy pointer batching and reset retires both owners', () => {
  const sent:any[]=[];const queue=new PointerMoveAdmission(input=>sent.push(input));
  queue.push(wheel(1));queue.push(wheel(2));assert.equal(sent.length,2);
  assert.ok(sent.every(input=>!input.wheelAdmission));
  queue.enableWheel();queue.push(wheel(3));queue.push(wheel(4));queue.reset();queue.acknowledge(3);
  queue.push(wheel(5));assert.deepEqual(inputs(sent).map(input=>input.inputSequence),[1,2,3,5]);
  assert.equal(sent.at(-1).wheelAdmission,undefined);
});
