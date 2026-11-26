import { dorotiProtocolVersion } from "./doroti.web.protocol.js";
import { startBrowserHotReload } from "./doroti.web.hot-reload.js";
import { applicationEnvironment } from "./doroti.web.session.js";
import { createRuntimeWithIcuDiagnostics, verifyRuntime, type DotnetRuntimeBuilder, type ExecutionSelection } from "./doroti.web.execution.js";
import { monoWorkerUrls, MonoWorkerPreload, mono11InitialWorkerPoolSize, type MonoWorkerConfig } from "./doroti.web.mono.js";
import { awaitWithAbort } from "./doroti.web.lifetime.js";

export interface DorotiWorkerEndpoint extends EventTarget {
  postMessage(message: unknown, transfer?: Transferable[]): void;
  terminate(): void;
}

interface MainRuntime {
  runtimeBuildInfo: { wasmEnableThreads: boolean; productVersion: string };
  localHeapViewU8(): Uint8Array;
  getAssemblyExports(name: string): Promise<{
    Doroti: { Host: { Web: { BrowserManagedRenderThread: { StartAsync(url: string, token: string): Promise<void>; CancelAsync(token: string): Promise<void> };
      BrowserHotReload: { ReadStatusAsync(): Promise<string>; PrepareAsync(runtimeId: string, requestId: string): Promise<boolean> } } } };
  }>;
}

let runtimePromise: Promise<MainRuntime> | undefined;
let runtimeFailure: Error | undefined;
const connections = new Map<string, {
  accept(port: MessagePort, worker: Worker): void;
  fail(error: Error): void;
}>();
const activeEndpoints = new Set<ManagedEndpoint>();
let runtimeLoaderUrl: string | undefined;
let restoreWorkerObserver: (() => void) | undefined;

function failRuntime(error: Error): void {
  runtimeFailure ??= error;
  restoreWorkerObserver?.();
  for (const connection of connections.values()) connection.fail(runtimeFailure);
  connections.clear();
  // A finalizer/deputy pthread failure invalidates the same shared runtime as a
  // render-thread failure. Startup's rejected Promise cannot notify an already
  // connected renderer; explicitly deliver its fatal event so Retry is shown.
  for (const endpoint of activeEndpoints) endpoint.fail(runtimeFailure);
}

export class ManagedEndpoint extends EventTarget implements DorotiWorkerEndpoint {
  #disposing = false;
  #closed = false;
  #failed = false;
  #disposeTimer: ReturnType<typeof setTimeout> | undefined;
  readonly #onError = (event: ErrorEvent): void => this.fail(event.error ?? event.message);
  readonly #onDecodeError = (): void => this.fail("Renderer MessagePort could not decode a message.");
  readonly #onMessage = (event: MessageEvent): void => {
    const terminal = event.data?.protocolVersion === dorotiProtocolVersion &&
      (event.data.kind === "disposed" || (event.data.kind === "fatal" && typeof event.data.cleanupComplete === "boolean"));
    if (terminal) this.#disposing = true;
    try { this.dispatchEvent(new MessageEvent("message", { data: event.data })); }
    finally { if (terminal) this.#close(); }
  };
  constructor(readonly port: MessagePort, readonly nativeWorker: Worker) {
    super();
    activeEndpoints.add(this);
    port.addEventListener("message", this.#onMessage);
    port.addEventListener("messageerror", this.#onDecodeError);
    nativeWorker.addEventListener("error", this.#onError);
    port.start();
  }
  postMessage(message: unknown, transfer: Transferable[] = []): void {
    if (this.#closed) throw new Error("Managed render endpoint is closed.");
    this.port.postMessage(message, transfer);
  }
  terminate(): void {
    if (this.#disposing) return;
    this.#disposing = true;
    // The runtime owns the pthread. Killing its Worker would abandon managed
    // stacks and shared-heap resources. Finish the role on its JS owner instead.
    this.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "dispose" });
    this.#disposeTimer = setTimeout(() => {
      const error = "Managed render owner did not acknowledge shutdown in 15 seconds. Native resources remain unconfirmed; reload the page.";
      console.error(error);
      this.dispatchEvent(new MessageEvent("message", { data: {
        protocolVersion: dorotiProtocolVersion, kind: "fatal", error, cleanupComplete: false, resourcesRetained: true,
      } }));
      this.#close();
    }, 15000);
  }
  #close(): void {
    this.#closed = true;
    clearTimeout(this.#disposeTimer);
    activeEndpoints.delete(this);
    this.port.removeEventListener("message", this.#onMessage);
    this.port.removeEventListener("messageerror", this.#onDecodeError);
    this.port.close();
    this.nativeWorker.removeEventListener("error", this.#onError);
  }
  fail(error: unknown): void {
    if (this.#failed) return;
    this.#failed = true;
    this.dispatchEvent(new MessageEvent("message", { data: {
      protocolVersion: dorotiProtocolVersion, kind: "fatal", error: String(error),
    } }));
  }
}

async function initializeMainRuntime(dotnetUrl: string, selection: ExecutionSelection): Promise<MainRuntime> {
  if (!crossOriginIsolated || typeof SharedArrayBuffer === "undefined")
    throw new Error("Doroti main-runtime rendering requires COOP/COEP isolation and shared memory.");
  const NativeWorker = globalThis.Worker;
  const preload = new MonoWorkerPreload();
  let workerUrls: ReadonlySet<string> = selection.runtimeContract === "mono-11" ? monoWorkerUrls("mono-11", dotnetUrl, {}) : new Set();
  let rejectStartup!: (error: Error) => void;
  const interrupted = new Promise<never>((_, reject) => { rejectStartup = reject; });
  const policyViolation = (event: SecurityPolicyViolationEvent): void => {
    if (event.effectiveDirective === "worker-src")
      rejectStartup(new Error("Doroti threaded runtime worker was blocked by Content Security Policy (worker-src)."));
  };
  globalThis.addEventListener("securitypolicyviolation", policyViolation);
  const startupTimer = setTimeout(() => rejectStartup(new Error("Doroti threaded runtime startup timed out; restart the page after checking worker/CSP/network diagnostics.")), 120000);
  // The public Worker constructor is the boundary where the host can receive a
  // transferable port from a runtime-owned pthread. No PThread/Mono internals are
  // inspected or patched. All .NET/Emscripten control messages pass through.
  const ObservedWorker = class extends NativeWorker {
    constructor(url: string | URL, options?: WorkerOptions) {
      super(url, options);
      const resolved = new URL(String(url), document.baseURI);
      if (!workerUrls.has(resolved.href)) return;
      if (selection.runtimeContract === "mono-11") preload.observe(this);
      this.addEventListener("error", event => {
        const error = new Error(`Doroti .NET runtime worker failed: ${event.message || String(event.error)}`);
        const originalStack = event.error?.stack;
        if (typeof originalStack === "string") error.stack += `\nCaused by: ${originalStack}`;
        else if (event.filename) error.stack += `\nWorker source: ${event.filename}:${event.lineno}:${event.colno}`;
        // Keep the worker's original stack in Inspector; the fatal UI message
        // intentionally only contains the short error description.
        console.error(error.stack ?? error.message);
        rejectStartup(error);
        preload.fail(error);
        failRuntime(error);
      });
      this.addEventListener("message", event => {
        const data = event.data;
        if (data?.kind !== "doroti-managed-port") return;
        event.stopImmediatePropagation();
        const connection = connections.get(data.sessionToken);
        if (data.protocolVersion !== dorotiProtocolVersion || !connection || !(data.port instanceof MessagePort) ||
            event.origin !== "" && event.origin !== location.origin || event.ports.length !== 1 || event.ports[0] !== data.port) {
          console.warn("Doroti rejected a stale/invalid managed-port handshake.", { protocolVersion: data.protocolVersion, sessionToken: data.sessionToken });
          if (data.port instanceof MessagePort) {
            data.port.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "managed-rejected", sessionToken: data.sessionToken });
            data.port.close();
          }
          return;
        }
        connections.delete(data.sessionToken);
        connection.accept(data.port, this);
      });
    }
  };
  globalThis.Worker = ObservedWorker;
  restoreWorkerObserver = () => {
    if (globalThis.Worker === ObservedWorker) globalThis.Worker = NativeWorker;
  };
  globalThis.addEventListener("pagehide", () => restoreWorkerObserver?.(), { once: true });
  try {
    const module = await Promise.race([import(dotnetUrl), interrupted]);
    module.dotnet.withModuleConfig({ onConfigLoaded: (config: MonoWorkerConfig) => {
      workerUrls = monoWorkerUrls(selection.runtimeContract, dotnetUrl, config);
    } });
    if (selection.runtimeContract === "mono-11")
      module.dotnet.withConfig({ pthreadPoolInitialSize: mono11InitialWorkerPoolSize });
    // .NET 10's SDK Hot Reload agent uses synchronous JSExport entry points.
    // The threaded runtime otherwise rejects them on the browser UI thread.
    // Enable only for the SDK-injected development transport; blocking waits
    // still throw, including during metadata handlers, rather than deadlocking.
    if (document.querySelector("script[src*='aspnetcore-browser-refresh']"))
      module.dotnet.withConfig({ jsThreadBlockingMode: "ThrowWhenBlockingWait" });
    const builder = module.dotnet.withEnvironmentVariables(
      applicationEnvironment(location.search, selection.applicationBaseUrl)) as DotnetRuntimeBuilder<MainRuntime>;
    const runtime = await Promise.race([createRuntimeWithIcuDiagnostics(builder), interrupted]);
    verifyRuntime(runtime, selection);
    if (selection.runtimeContract === "mono-11") await Promise.race([preload.ready(), interrupted]);
    document.documentElement.dataset.dorotiRuntimeLocation = "main";
    await startBrowserHotReload(runtime);
    return runtime;
  } catch (error) {
    preload.fail(error instanceof Error ? error : new Error(String(error)));
    failRuntime(error instanceof Error ? error : new Error(String(error)));
    throw error;
  } finally {
    clearTimeout(startupTimer);
    globalThis.removeEventListener("securitypolicyviolation", policyViolation);
  }
}

export async function createManagedDorotiWorker(dotnetUrl: string, roleUrl: URL, selection: ExecutionSelection, signal?: AbortSignal): Promise<DorotiWorkerEndpoint> {
  signal?.throwIfAborted();
  if (runtimeLoaderUrl !== undefined && runtimeLoaderUrl !== dotnetUrl)
    throw new Error("Doroti cannot create a second managed runtime in the same page.");
  runtimeLoaderUrl = dotnetUrl;
  const runtime = await awaitWithAbort(runtimePromise ??= initializeMainRuntime(dotnetUrl, selection), signal);
  signal?.throwIfAborted();
  if (runtimeFailure) throw runtimeFailure;
  const exports = await awaitWithAbort(runtime.getAssemblyExports("Doroti.Host.Web.dll"), signal);
  signal?.throwIfAborted();
  if (runtimeFailure) throw runtimeFailure;
  const token = crypto.randomUUID();
  let endpoint: ManagedEndpoint | undefined;
  let rejectConnection!: (error: unknown) => void;
  const connected = new Promise<DorotiWorkerEndpoint>((resolve, reject) => {
    rejectConnection = reject;
    connections.set(token, { fail: reject, accept: (port, worker) => {
      endpoint = new ManagedEndpoint(port, worker);
      port.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "managed-accepted", sessionToken: token });
      resolve(endpoint);
    } });
  });
  const cancel = (): void => {
    connections.delete(token);
    endpoint?.terminate();
    void exports.Doroti.Host.Web.BrowserManagedRenderThread.CancelAsync(token).catch(error => endpoint?.fail(error));
    rejectConnection(signal?.aborted ? signal.reason : new Error(`Managed render connection ${token} timed out before the port handshake.`));
  };
  const timer = setTimeout(cancel, 120000);
  signal?.addEventListener("abort", cancel, { once: true });
  // A distinct role module instance is needed if .NET reuses a pthread Worker.
  roleUrl.searchParams.set("session", token);
  void exports.Doroti.Host.Web.BrowserManagedRenderThread.StartAsync(roleUrl.href, token).catch(error => {
    connections.delete(token);
    if (endpoint) endpoint.fail(error);
    else rejectConnection(error);
  });
  if (signal?.aborted) cancel();
  return connected.finally(() => { clearTimeout(timer); signal?.removeEventListener("abort", cancel); });
}
