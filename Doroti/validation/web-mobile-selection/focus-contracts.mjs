import assert from 'node:assert/strict';
import {resolve} from 'node:path';
import {pathToFileURL} from 'node:url';

const root=process.argv[2];
if (!root) throw Error('focus-contracts.mjs PUBLISHED_WWWROOT');
const {TextInputTapFocus}=await import(pathToFileURL(resolve(root,'_content/Doroti.Host.Web/doroti.web.text-focus.js')));
const calls=[];
const focus=new TextInputTapFocus(target=>calls.push(target));
// Browser PointerEvent properties live on its prototype, not as own fields.
const point=(overrides={})=>Object.create(Object.fromEntries(Object.entries({
  pointerId:1,clientX:20,clientY:30,timeStamp:0,isPrimary:true,button:0,...overrides
})));
focus.start(point(),'field');
focus.end(point({timeStamp:80}));
assert.deepEqual(calls,['field'],'tap commits synchronously');
for(const test of ['scroll','long-press','cancel','secondary','non-primary','empty','multi-touch']) {
  calls.length=0;
  focus.start(point({button:test==='secondary'?2:0,isPrimary:test!=='non-primary'}),test==='empty'?null:'field');
  if(test==='scroll') {focus.move(point({clientX:60}));focus.move(point());}
  if(test==='cancel') focus.cancel();
  if(test==='multi-touch') focus.start(point({pointerId:2,isPrimary:false}),'field');
  focus.end(point({timeStamp:test==='long-press'?850:80}));
  assert.deepEqual(calls,[],test+' must not request keyboard focus');
}
focus.start(point(),'field');
focus.end(point({pointerId:2,timeStamp:60}));
assert.deepEqual(calls,[],'unrelated pointer cannot commit');
focus.end(point({timeStamp:90}));
assert.deepEqual(calls,['field'],'original pointer can still commit');
console.log('PASS: tap, scroll, long press, cancellation, secondary/multi-touch and prototype-backed pointer events');
