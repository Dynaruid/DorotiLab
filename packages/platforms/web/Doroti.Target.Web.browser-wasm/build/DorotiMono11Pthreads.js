// Link-time compatibility for Mono 11.0.0's dirty JSWebWorker retirement.
// Mono still calls the Emscripten 3 `killThread` command, removed in Emscripten 6.
// Extend the compiler's library before JS generation/minification; the host does
// not inspect or replace a live runtime's PThread functions or installed packs.
// Cleanup follows Emscripten 6.0.3's returnWorkerToPool, except a dirty Worker is
// terminated instead of reused. Free pthread data last: free can service queued
// native calls, which must see consistent thread ownership.
// References (MIT): dotnet/dotnet 297acf930fb0f5c07fd4e51d78e521b4ad2e5d1f,
// src/runtime/src/mono/browser/runtime/pthreads/ui-thread.ts;
// emscripten 6.0.3 src/lib/libpthread.js. SPDX-License-Identifier: MIT.
var dorotiMono11Pthread = LibraryManager.library.$PThread;
var dorotiMono11Loader = dorotiMono11Pthread && dorotiMono11Pthread.loadWasmModuleToWorker;
var dorotiMono11Source = typeof dorotiMono11Loader === 'function' ? dorotiMono11Loader.toString() : '';
var dorotiMono11Switch = 'switch (cmd) {';
if (dorotiMono11Source.split(dorotiMono11Switch).length !== 2 ||
    dorotiMono11Source.includes('case "killThread"') ||
    !LibraryManager.library.$terminateWorker || !LibraryManager.library.$cleanupThread)
    throw new Error('Doroti Mono 11 retirement requires the qualified Emscripten 6.0.3 pthread library.');
dorotiMono11Pthread.loadWasmModuleToWorker = new Function('return (' + dorotiMono11Source.replace(dorotiMono11Switch,
    dorotiMono11Switch + '\n' +
    'case "killThread":\n' +
    '  if (!d.thread || PThread.pthreads[d.thread] !== worker || worker.pthread_ptr !== d.thread)\n' +
    '    throw new Error("Doroti Mono 11 retirement received a mismatched pthread owner.");\n' +
    '  var retiredThread = d.thread;\n' +
    '  delete PThread.pthreads[retiredThread];\n' +
    '  worker.pthread_ptr = 0;\n' +
    '  terminateWorker(worker);\n' +
    '  __emscripten_thread_free_data(retiredThread);\n' +
    '  break;\n') + ');')();
