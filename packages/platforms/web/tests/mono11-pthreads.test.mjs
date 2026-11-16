import assert from "node:assert/strict";
import test from "node:test";
import { readFileSync } from "node:fs";
import vm from "node:vm";

const adapter = readFileSync(new URL("../Doroti.Target.Web.browser-wasm/build/DorotiMono11Pthreads.js", import.meta.url), "utf8");
function fixture() {
  const events = [], worker = { pthread_ptr: 123 }, unusedWorkers = [];
  const pthread = { pthreads: { 123: worker }, unusedWorkers,
    loadWasmModuleToWorker: (worker) => new Promise((loaded) => {
      worker.onmessage = (event) => {
        var d = event.data, cmd = d.cmd;
        switch (cmd) {
          case 3: loaded(worker); break;
          default: report(cmd);
        }
      };
    }) };
  const scope = { LibraryManager: { library: { $PThread: pthread, $terminateWorker() {}, $cleanupThread() {} } },
    PThread: pthread, terminateWorker: value => events.push(["terminated", value]),
    __emscripten_thread_free_data: ptr => { assert.equal(pthread.pthreads[ptr], undefined); assert.equal(worker.pthread_ptr, 0); events.push(["freed", ptr]); },
    report: value => events.push(["unhandled", value]) };
  vm.runInNewContext(adapter, scope);
  pthread.loadWasmModuleToWorker(worker);
  return { events, worker, pthread };
}
test("Mono 11 dirty worker is detached and terminated before freeing native thread data, without reusing it", () => {
  const { worker, events, pthread } = fixture();
  worker.onmessage({ data: { cmd: "killThread", thread: 123 } });
  assert.deepEqual(events, [["terminated", worker], ["freed", 123]]);
  assert.deepEqual(pthread.unusedWorkers, []);
});
test("a mismatched retirement cannot free or terminate another pthread", () => {
  const { worker, events, pthread } = fixture();
  assert.throws(() => worker.onmessage({ data: { cmd: "killThread", thread: 456 } }), /mismatched pthread owner/);
  assert.deepEqual(events, []); assert.equal(pthread.pthreads[123], worker);
});
test("unrecognized commands remain visible to the original native handler", () => {
  const { worker, events } = fixture();
  worker.onmessage({ data: { cmd: "unexpected" } });
  assert.deepEqual(events, [["unhandled", "unexpected"]]);
});
test("an unqualified compiler library is rejected instead of silently changing runtime ownership", () => {
  assert.throws(() => vm.runInNewContext(adapter, { LibraryManager: { library: {} } }), /qualified Emscripten/);
});
