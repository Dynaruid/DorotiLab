// Synthetic transfer/owner contracts. Product GPU and pixel proof is verify-web.mjs.
import assert from 'node:assert/strict';
import { pathToFileURL } from 'node:url';
import { resolve } from 'node:path';
const root=process.argv[2]??'Doroti/src/Doroti.Host.Web/obj/Release/net10.0/Doroti.Web/wwwroot';
const {BrowserTextureRegistry}=await import(pathToFileURL(resolve(root,'doroti.web.textures.js')));
const {dorotiProtocolVersion}=await import(pathToFileURL(resolve(root,'doroti.web.protocol.js')));
class Frame { width=10; height=10; closes=0; close(){this.closes++;} }
globalThis.ImageBitmap=Frame;
class Endpoint extends EventTarget {
 messages=[]; held=[]; next=0; fail=false;
 postMessage(message,transfer){
  if(this.fail)throw Error('injected posting failure');
  this.messages.push(message);
  if(message.operation==='frame'){assert.equal(transfer[0],message.frame);this.held.push(message);return;}
  queueMicrotask(()=>this.ack(message,{textureId:String(++this.next),generation:this.next}));
 }
 ack(message,result={}){this.dispatchEvent(new MessageEvent('message',{data:{protocolVersion:dorotiProtocolVersion,kind:'texture-response',request:message.request,...result}}));}
 consume(error){const message=this.held.shift();message.frame.close();this.ack(message,error?{error:'stale',code:'Source'}:{});}
}
const tick=()=>new Promise(resolve=>setImmediate(resolve));
const endpoint=new Endpoint(), registry=new BrowserTextureRegistry(endpoint), entry=await registry.createFrameProducer();
const frames=Array.from({length:20},()=>new Frame());
for(const frame of frames)assert.equal(entry.pushFrame(frame),true);
assert.equal(endpoint.held.length,1);assert.equal(entry.pendingFrames,2);
assert.equal(frames.filter(frame=>frame.closes===1).length,18);
endpoint.consume();await tick();assert.equal(endpoint.held[0].frame,frames[19]);endpoint.consume();await tick();
assert.ok(frames.every(frame=>frame.closes===1));console.log('PASS bounded burst/latest-only/exactly-once close');
const rejected=new Frame();entry.pushFrame(rejected);endpoint.consume(true);await tick();assert.equal(rejected.closes,1);console.log('PASS worker rejection does not double-close transferred frame');
endpoint.fail=true;const failed=new Frame();entry.pushFrame(failed);await tick();assert.equal(failed.closes,1);assert.equal(entry.pendingFrames,0);endpoint.fail=false;console.log('PASS posting failure retains and closes local ownership');
let complete;let snapshots=0;globalThis.createImageBitmap=()=>{snapshots++;return new Promise(resolve=>{complete=resolve;});};
entry.replaceCanvas({width:10,height:10});entry.markFrameAvailable();entry.markFrameAvailable();entry.markFrameAvailable();assert.equal(snapshots,1);
const late=new Frame();const disposed=entry.dispose();complete(late);await disposed;assert.equal(late.closes,1);assert.equal(snapshots,1);console.log('PASS coalescing and late snapshot after dispose');
const caller=new Frame();assert.equal(entry.pushFrame(caller),false);assert.equal(caller.closes,0);caller.close();
const next=await registry.registerCanvas({width:10,height:10});next.markFrameAvailable();const previous=complete;next.replaceCanvas({width:10,height:10});const stale=new Frame();previous(stale);await tick();assert.equal(stale.closes,1);await next.dispose();console.log('PASS source generation rejects asynchronous old snapshot');
registry.disconnect();await assert.rejects(()=>registry.createFrameProducer());console.log('PASS disconnected owner rejects registration');

const worker=await import(pathToFileURL(resolve(root,'doroti.web.texture-worker.js')));
let id=0;worker.initializeTextures({RegisterBrowserTexture:()=>String(++id),MarkBrowserTexture(){},UnregisterBrowserTexture(){},ReleaseBrowserTextureImage(){}},undefined,()=>{throw Error('GPU must not run in protocol contracts');});
const registration=await worker.textureMessage({request:1,operation:'register'});
const wire={...registration,request:2,sourceGeneration:1};
const first=new Frame(),last=new Frame();await worker.textureMessage({...wire,operation:'frame',sequence:1,frame:first});await worker.textureMessage({...wire,operation:'frame',sequence:2,frame:last});assert.equal(first.closes,1);
const old=new Frame();await assert.rejects(()=>worker.textureMessage({...wire,operation:'frame',sequence:1,frame:old}));assert.equal(old.closes,1);
const foreign=new Frame();await assert.rejects(()=>worker.textureMessage({...wire,operation:'frame',generation:999,sequence:3,frame:foreign}));assert.equal(foreign.closes,1);
await worker.textureMessage({...wire,operation:'unregister'});assert.equal(last.closes,1);
const d=worker.diagnostics();assert.equal(d.received,d.closed);assert.equal(d.pending,0);console.log('PASS worker latest slot, stale sequence, foreign generation, unregister cleanup');
const securityEndpoint=new Endpoint(),securityRegistry=new BrowserTextureRegistry(securityEndpoint);
globalThis.createImageBitmap=async()=>{throw new DOMException('Canvas is not origin clean','SecurityError');};
const secureEntry=await securityRegistry.registerCanvas({width:10,height:10});secureEntry.markFrameAvailable();await tick();assert.equal(secureEntry.lastError.name,'SecurityError');await secureEntry.dispose();assert.equal(securityRegistry.sourceBytes,0);securityRegistry.disconnect();console.log('PASS DOMException identity and rejected snapshot budget cleanup');
const budgetEndpoint=new Endpoint(),budgetRegistry=new BrowserTextureRegistry(budgetEndpoint);
const budgetEntries=[];
for(let i=0;i<5;i++)budgetEntries.push(await budgetRegistry.createFrameProducer());
const large=Array.from({length:5},()=>Object.assign(new Frame(),{width:2048,height:2048}));
for(let i=0;i<4;i++)assert.equal(budgetEntries[i].pushFrame(large[i]),true);
assert.equal(budgetEntries[4].pushFrame(large[4]),false);assert.equal(large[4].closes,0);large[4].close();
assert.equal(budgetRegistry.sourceBytes,64*1024*1024);
for(let i=0;i<4;i++)budgetEndpoint.consume();await tick();await budgetRegistry.dispose();assert.equal(budgetRegistry.sourceBytes,0);console.log('PASS view source budget rejects without stealing caller ownership');

const pendingRegistrations=[];
for(let i=0;i<5;i++)pendingRegistrations.push(await worker.textureMessage({request:10+i,operation:'register'}));
for(let i=0;i<5;i++){
 const frame=Object.assign(new Frame(),{width:2048,height:2048});
 const message={...pendingRegistrations[i],request:20+i,operation:'frame',sourceGeneration:1,sequence:1,frame};
 if(i<4)await worker.textureMessage(message);else{await assert.rejects(()=>worker.textureMessage(message));assert.equal(frame.closes,1);}
}
assert.equal(worker.diagnostics().pendingBytes,64*1024*1024);
for(const registration of pendingRegistrations)await worker.textureMessage({...registration,request:30,operation:'unregister'});
assert.equal(worker.diagnostics().pendingBytes,0);console.log('PASS worker pending-byte budget and rejection cleanup');

class Video extends EventTarget{readyState=2;next=0;callbacks=new Map();pauses=0;requestVideoFrameCallback(cb){this.callbacks.set(++this.next,cb);return this.next;}cancelVideoFrameCallback(id){this.callbacks.delete(id);}pause(){this.pauses++;}}
globalThis.VideoFrame=class extends Frame{displayWidth=10;displayHeight=10;};
const videoEndpoint=new Endpoint(),videoRegistry=new BrowserTextureRegistry(videoEndpoint),video=new Video();
const videoEntry=await videoRegistry.registerVideo(video);let cleanup=0;videoEntry.ownCleanup(()=>cleanup++);
video.dispatchEvent(new Event('pause'));videoEndpoint.consume();await tick();assert.equal(videoEndpoint.held.length,1);
videoEndpoint.consume();await tick();assert.equal(videoEndpoint.held.length,0);
video.dispatchEvent(new Event('emptied'));assert.equal(cleanup,0);videoEndpoint.consume();await tick();
await videoEntry.dispose();assert.equal(cleanup,1);assert.equal(video.pauses,0);assert.equal(video.callbacks.size,0);videoRegistry.disconnect();
console.log('PASS paused final frame, emptied generation, borrowed video and owned cleanup');
let completeGpu;let destroyed=0;let released=0;let rejectCopy=false;
const gpuCompletion=()=>new Promise(resolve=>{completeGpu=resolve;});
const fakeGpu={copyTextureSource(){if(rejectCopy)throw new DOMException('copy rejected','OperationError');return{handle:42,destroy(){destroyed++;}};},textureWorkDone:gpuCompletion,diagnostics(){return{};}};
let sourceError;
worker.initializeTextures({RegisterBrowserTexture:()=>String(++id),MarkBrowserTexture(){},UnregisterBrowserTexture(){},ReleaseBrowserTextureImage(){released++;}},fakeGpu,()=>{throw Error('wrong backend');},undefined,(_id,error)=>{sourceError=error;});
const gpuRegistration=await worker.textureMessage({request:40,operation:'register'}),tokens=[];
for(let i=1;i<=4;i++){
 await worker.textureMessage({...gpuRegistration,request:40+i,operation:'frame',sourceGeneration:1,sequence:i,frame:new Frame()});
 const allocation=worker.acquireTexture(gpuRegistration.textureId);
 if(i<=3){assert.ok(allocation);tokens.push(allocation.token);}else assert.equal(allocation,null);
}
for(const token of tokens)worker.retireTexture(token);
const retirement=worker.flushRetired();assert.equal(destroyed,0);assert.equal(released,0);completeGpu();await retirement;assert.equal(destroyed,3);assert.equal(released,3);
const latest=worker.acquireTexture(gpuRegistration.textureId);assert.ok(latest);worker.retireTexture(latest.token);const finish=worker.flushRetired();completeGpu();await finish;
rejectCopy=true;const badCopy=new Frame();await worker.textureMessage({...gpuRegistration,request:50,operation:'frame',sourceGeneration:1,sequence:5,frame:badCopy});assert.equal(worker.acquireTexture(gpuRegistration.textureId),null);assert.equal(badCopy.closes,1);assert.equal(sourceError.name,'OperationError');
await worker.textureMessage({...gpuRegistration,request:51,operation:'unregister'});assert.equal(worker.diagnostics().live,0);
console.log('PASS asynchronous GPU retirement, allocation bound, latest retry and failed-copy cleanup');
const bridge=await import(pathToFileURL(resolve(root,'doroti.web.js')));
let finishCapture;const lateBitmap=new Frame();
bridge.stagePlatformBitmap(0,0,0,10,10,10,10,new Promise(resolve=>{finishCapture=resolve;}));
bridge.stagePlatformFrame('{}');const canceledCommit=bridge.commitPlatformFrame();bridge.discardPlatformFrame();
finishCapture(lateBitmap);assert.equal(await canceledCommit,false);await bridge.drainPlatformCaptures();assert.equal(lateBitmap.closes,1);
console.log('PASS canceled composition closes late GPU snapshot before shutdown');
await worker.disposeTextures();console.log('PASS all source ownership contracts');
