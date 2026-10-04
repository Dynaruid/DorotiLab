import { dorotiProtocolVersion } from "./doroti.web.protocol.js";
import { developmentBridge, startBrowserHotReload } from "./doroti.web.hot-reload.js";

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
const connections = new Map<string, (port: MessagePort, worker: Worker) => void>();

class ManagedEndpoint extends EventTarget implements DorotiWorkerEndpoint {
  #disposing = false;
  readonly #onError = (event: ErrorEvent): void => this.fail(event.error ?? event.message);
  constructor(readonly port: MessagePort, readonly nativeWorker: Worker) {
    super();
    port.addEventListener("message", event => {
      const terminal = event.data?.kind === "disposed" || (event.data?.kind === "fatal" && typeof event.data.cleanupComplete === "boolean");
      if (terminal) this.#disposing = true;
      this.dispatchEvent(new MessageEvent("message", { data: event.data }));
      if (terminal) {
        port.close();
        nativeWorker.removeEventListener("error", this.#onError);
      }
    });
    port.addEventListener("messageerror", () => this.fail("Renderer MessagePort could not decode a message."));
    nativeWorker.addEventListener("error", this.#onError);
    port.start();
  }
  postMessage(message: unknown, transfer: Transferable[] = []): void { this.port.postMessage(message, transfer); }
  terminate(): void {
    if (this.#disposing) return;
    this.#disposing = true;
    // The runtime owns the pthread. Killing its Worker would abandon managed
    // stacks and shared-heap resources. Finish the role on its JS owner instead.
    this.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "dispose" });
  }
  fail(error: unknown): void {
    this.dispatchEvent(new MessageEvent("message", { data: {
      protocolVersion: dorotiProtocolVersion, kind: "fatal", error: String(error),
    } }));
  }
}

async function initializeMainRuntime(dotnetUrl: string): Promise<MainRuntime> {
  if (!crossOriginIsolated || typeof SharedArrayBuffer === "undefined")
    throw new Error("Doroti main-runtime rendering requires COOP/COEP isolation and shared memory.");
  const runtimeBase = new URL("./", dotnetUrl);
  const NativeWorker = globalThis.Worker;
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
  globalThis.Worker = class extends NativeWorker {
    constructor(url: string | URL, options?: WorkerOptions) {
      super(url, options);
      const resolved = new URL(String(url), document.baseURI);
      if (resolved.origin !== runtimeBase.origin ||
          !resolved.pathname.startsWith(`${runtimeBase.pathname}dotnet.native.worker`) ||
          !resolved.pathname.endsWith(".mjs")) return;
      this.addEventListener("error", event => rejectStartup(new Error(`Doroti .NET runtime worker failed during startup: ${event.message || String(event.error)}`)));
      this.addEventListener("message", event => {
        const data = event.data;
        if (data?.kind !== "doroti-managed-port") return;
        event.stopImmediatePropagation();
        const accept = connections.get(data.sessionToken);
        if (data.protocolVersion !== dorotiProtocolVersion || !accept || !(data.port instanceof MessagePort)) {
          if (data.port instanceof MessagePort) {
            data.port.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "managed-rejected", sessionToken: data.sessionToken });
            data.port.close();
          }
          return;
        }
        connections.delete(data.sessionToken);
        accept(data.port, this);
      });
    }
  };
  try {
    const module = await import(dotnetUrl);
    const params = new URL(location.href).searchParams;
    const diagnostics = params.get("dorotiResizeDiagnostics") === "1";
    // .NET 10's SDK Hot Reload agent uses synchronous JSExport entry points.
    // The threaded runtime otherwise rejects them on the browser UI thread.
    // Enable only for the SDK-injected development transport; blocking waits
    // still throw, including during metadata handlers, rather than deadlocking.
    if (document.querySelector("script[src*='aspnetcore-browser-refresh']"))
      module.dotnet.withConfig({ jsThreadBlockingMode: "ThrowWhenBlockingWait" });
    const runtime = await Promise.race([module.dotnet.withEnvironmentVariables({
      DOROTI_DEV_SESSION_ID: developmentBridge(location.search)?.sessionId ?? "browser",
      DOROTI_TESTBED_MODE: params.get("dorotiTestbedMode") ?? "diagnostics",
      DOROTI_SAMPLE: params.get("dorotiSample") ?? "",
      DOROTI_LAYOUT_PROFILE: params.get("dorotiLayoutProfile") === "1" ? "1" : "0",
      DOROTI_ALLOCATION_PROFILE: params.get("dorotiAllocationProfile") === "1" ? "1" : "0",
      DOROTI_WEB_DIRECT_TRACE: diagnostics ? "1" : "0", DOROTI_STAGE_TRACE: diagnostics ? "1" : "0",
      DOROTI_WEBVIEW_COUNT: params.get("dorotiWebViewCount") ?? "1",
      DOROTI_WEBVIEW_WORKLOAD: params.get("dorotiWebViewWorkload") ?? "idle",
      DOROTI_SAMPLE_PROGRESS_SCOPE: params.get("dorotiProgressScope") ?? "local",
    }).create() as Promise<MainRuntime>, interrupted]);
    if (!runtime.runtimeBuildInfo.wasmEnableThreads || !(runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer))
      throw new Error("Doroti main-runtime rendering requires a threaded .NET build with a shared heap.");
    document.documentElement.dataset.dorotiRuntimeLocation = "main";
    await startBrowserHotReload(runtime);
    return runtime;
  } catch (error) {
    globalThis.Worker = NativeWorker;
    throw error;
  } finally {
    clearTimeout(startupTimer);
    globalThis.removeEventListener("securitypolicyviolation", policyViolation);
  }
}

export async function createManagedDorotiWorker(dotnetUrl: string, roleUrl: URL, signal?: AbortSignal): Promise<DorotiWorkerEndpoint> {
  const runtime = await (runtimePromise ??= initializeMainRuntime(dotnetUrl));
  const exports = await runtime.getAssemblyExports("Doroti.Host.Web.dll");
  const token = crypto.randomUUID();
  let endpoint: ManagedEndpoint | undefined;
  let rejectConnection!: (error: unknown) => void;
  const connected = new Promise<DorotiWorkerEndpoint>((resolve, reject) => {
    rejectConnection = reject;
    connections.set(token, (port, worker) => {
      endpoint = new ManagedEndpoint(port, worker);
      port.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "managed-accepted", sessionToken: token });
      resolve(endpoint);
    });
  });
  const cancel = (): void => {
    connections.delete(token);
    endpoint?.terminate();
    void exports.Doroti.Host.Web.BrowserManagedRenderThread.CancelAsync(token).catch(error => endpoint?.fail(error));
    rejectConnection(new Error("Managed render connection canceled or timed out."));
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
