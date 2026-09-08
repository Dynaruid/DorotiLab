import { dorotiProtocolVersion } from "./doroti.web.protocol.js";

export interface DorotiWorkerEndpoint extends EventTarget {
  postMessage(message: unknown, transfer?: Transferable[]): void;
  terminate(): void;
}

interface MainRuntime {
  runtimeBuildInfo: { wasmEnableThreads: boolean; productVersion: string };
  localHeapViewU8(): Uint8Array;
  getAssemblyExports(name: string): Promise<{
    Doroti: { Host: { Web: { BrowserManagedRenderThread: { StartAsync(url: string, token: string): Promise<void> } } } };
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
      this.dispatchEvent(new MessageEvent("message", { data: event.data }));
      if (event.data?.kind === "disposed") {
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
      this.addEventListener("message", event => {
        const data = event.data;
        if (data?.kind !== "doroti-managed-port") return;
        event.stopImmediatePropagation();
        const accept = connections.get(data.sessionToken);
        if (data.protocolVersion !== dorotiProtocolVersion || !accept || !(data.port instanceof MessagePort)) {
          if (data.port instanceof MessagePort) data.port.close();
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
    const runtime = await module.dotnet.withEnvironmentVariables({
      DOROTI_TESTBED_MODE: params.get("dorotiTestbedMode") ?? "diagnostics",
      DOROTI_LAYOUT_PROFILE: params.get("dorotiLayoutProfile") === "1" ? "1" : "0",
      DOROTI_WEB_DIRECT_TRACE: diagnostics ? "1" : "0", DOROTI_STAGE_TRACE: diagnostics ? "1" : "0",
      DOROTI_SAMPLE_PROGRESS_SCOPE: params.get("dorotiProgressScope") ?? "local",
    }).create() as MainRuntime;
    if (!runtime.runtimeBuildInfo.wasmEnableThreads || !(runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer))
      throw new Error("Doroti main-runtime rendering requires a threaded .NET build with a shared heap.");
    document.documentElement.dataset.dorotiRuntimeLocation = "main";
    return runtime;
  } catch (error) {
    globalThis.Worker = NativeWorker;
    throw error;
  }
}

export async function createManagedDorotiWorker(dotnetUrl: string, roleUrl: URL): Promise<DorotiWorkerEndpoint> {
  const runtime = await (runtimePromise ??= initializeMainRuntime(dotnetUrl));
  const exports = await runtime.getAssemblyExports("Doroti.Host.Web.dll");
  const token = crypto.randomUUID();
  let endpoint: ManagedEndpoint | undefined;
  let rejectConnection!: (error: unknown) => void;
  const connected = new Promise<DorotiWorkerEndpoint>((resolve, reject) => {
    rejectConnection = reject;
    connections.set(token, (port, worker) => {
      endpoint = new ManagedEndpoint(port, worker);
      resolve(endpoint);
    });
  });
  const timer = setTimeout(() => {
    connections.delete(token);
    rejectConnection(new Error("Timed out connecting the managed render thread."));
  }, 120000);
  // A distinct role module instance is needed if .NET reuses a pthread Worker.
  roleUrl.searchParams.set("session", token);
  void exports.Doroti.Host.Web.BrowserManagedRenderThread.StartAsync(roleUrl.href, token).catch(error => {
    connections.delete(token);
    if (endpoint) endpoint.fail(error);
    else rejectConnection(error);
  });
  return connected.finally(() => clearTimeout(timer));
}
