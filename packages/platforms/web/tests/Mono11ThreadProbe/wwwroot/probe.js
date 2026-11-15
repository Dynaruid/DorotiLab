import { dotnet } from "./_framework/dotnet.js";
const parameters = new URL(location.href).searchParams;
const preload = new Map();
const waiters = [];
let preloaded = 0;
const NativeWorker = globalThis.Worker;
if (parameters.get("waitPreload") === "1") {
  const entry = import.meta.resolve("./_framework/dotnet.js");
  globalThis.Worker = class extends NativeWorker {
    constructor(url, options) {
      super(url, options);
      if (new URL(url, document.baseURI).href !== entry) return;
      preload.set(this, false);
      this.addEventListener("message", event => {
        if (event.data?.cmd !== 3 || preload.get(this)) return;
        preload.set(this, true);
        setTimeout(() => {
          preload.delete(this); preloaded++;
          if (!preload.size) for (const ready of waiters.splice(0)) ready();
        }, 0);
      });
    }
  };
}
if (parameters.get("trace") === "1") dotnet.withConfig({ diagnosticTracing: true });
if (parameters.has("pool")) {
  const size = Number(parameters.get("pool"));
  if (!Number.isInteger(size) || size < 7 || size > 32) throw new Error("Probe pool must be an integer from 7 to 32.");
  dotnet.withConfig({ pthreadPoolInitialSize: size });
}
const runtime = await dotnet.create();
if (parameters.get("waitPreload") === "1") {
  if (preload.size) await new Promise(ready => waiters.push(ready));
  console.info(`probe: native preload receipts=${preloaded}`);
}
console.info(`probe config: ${JSON.stringify({ version: runtime.runtimeBuildInfo.productVersion, initial: runtime.getConfig().pthreadPoolInitialSize, unused: runtime.getConfig().pthreadPoolUnusedSize })}`);
console.info("Mono 11 runtime-only probe; not a Skia render qualification.");
document.querySelector("output").textContent = "runtime-ready";
const exports = await runtime.getAssemblyExports(runtime.getConfig().mainAssemblyName);
document.querySelector("output").textContent = "exports-ready";
const id = await exports.Probe.Start();
document.querySelector("output").textContent = `owner=${id}`;
