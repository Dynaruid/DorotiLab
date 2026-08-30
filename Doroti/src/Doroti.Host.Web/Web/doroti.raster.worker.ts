import {
  configureWorkerBridge,
  dispatchWorkerAnimationFrame,
  dispatchWorkerInput,
  dispatchWorkerSnapshot,
  initializeManagedCallbacks,
} from "./doroti.web.js";
import {
  decodeDorotiMessage,
  dorotiProtocolVersion,
  DorotiRuntimeStateMachine,
} from "./doroti.web.protocol.js";

const protocolVersion = dorotiProtocolVersion;
const inboundKinds = new Set([
  "init", "snapshot", "frame", "input", "receipt", "control-response",
  "context", "dispose", "crash",
]);
const runtimeState = new DorotiRuntimeStateMachine();

interface ResizeEpoch {
  generation: number;
  logicalWidth: number;
  logicalHeight: number;
  physicalWidth: number;
  physicalHeight: number;
  devicePixelRatio: number;
  timestampMicroseconds: number;
}

interface HostSnapshot {
  canvasId: string;
  logicalWidth: number;
  logicalHeight: number;
  devicePixelRatio: number;
  visible: boolean;
  focused: boolean;
  languageTag: string;
  brightness: string;
  operatingSystem: string;
  generation: number;
  surfaceGeneration: number;
  inputSequence: number;
  gpu: { api: string; vendor: string; renderer: string; hardware: boolean; softwareFallbackUsed: boolean };
  resizeEpoch: ResizeEpoch;
}

interface SurfaceExports {
  RenderFrame(
    requestId: number, generation: number, logicalWidth: number, logicalHeight: number,
    physicalWidth: number, physicalHeight: number, devicePixelRatio: number,
    timestampMicroseconds: number, framebuffer: number, stencilBits: number,
    sampleCount: number, contextGeneration: number, glStateDirty: boolean): string;
  CompleteFrame(requestId: number, generation: number, terminal: string, reason: string): void;
  ContextLost(requestId: number, generation: number): void;
  ContextRestored(): void;
}

interface PresentRequest extends ResizeEpoch {
  requestId: number;
  terminal: boolean;
}

interface WorkerPresenter {
  canvas: OffscreenCanvas;
  context: number;
  contextGeneration: number;
  extension: WEBGL_lose_context | null;
  current: PresentRequest | null;
  latest: PresentRequest | null;
  draining: boolean;
  nextRequestId: number;
  contextLost: boolean;
  bitmapCreated: number;
  bitmapConsumed: number;
  bitmapClosed: number;
  activeBitmaps: number;
}

type WorkerMode = "worker-direct-webgl" | "offscreen-worker";

interface EmscriptenGlRuntime {
  createContext(canvas: OffscreenCanvas, attributes: Record<string, number>): number;
  makeContextCurrent(context: number): void;
  deleteContext?(context: number): void;
  currentContext?: { GLctx: WebGL2RenderingContext };
}

interface DotnetRuntime {
  getAssemblyExports(name: string): Promise<unknown>;
  getConfig(): { mainAssemblyName: string };
  exit(code: number, reason?: unknown): void;
}

let snapshot: HostSnapshot | null = null;
let hostId = 0;
let surface: SurfaceExports | null = null;
let presenter: WorkerPresenter | null = null;
let stopManagedRuntime: (() => void) | null = null;
let managedRuntime: DotnetRuntime | null = null;
let dotnetModuleUrl: string | null = null;
let managedHostReady = false;
let workerMode: WorkerMode = "offscreen-worker";
let transferredCanvas: OffscreenCanvas | null = null;
let pendingManagedSnapshot: { hostId: number; value: HostSnapshot } | null = null;
const pendingManagedInputs: Record<string, unknown>[] = [];
let requestSequence = 0;
let controlSequence = 0;
const pendingControls = new Map<number, { resolve(value: string): void; reject(reason: unknown): void }>();
const pendingReceipts = new Map<number, {
  resolve(value: { committed: boolean; consumed: boolean; reason: string }): void;
}>();
const pendingReceiptWork = new Set<Promise<void>>();

function post(kind: string, payload: Record<string, unknown> = {}, transfer: Transferable[] = []): void {
  (globalThis as unknown as { postMessage(message: unknown, transfer: Transferable[]): void })
    .postMessage({ protocolVersion, hostId, kind, ...payload }, transfer);
}

function applyManagedSnapshot(messageHostId: number, value: HostSnapshot): void {
  snapshot = value;
  if (!managedHostReady) {
    pendingManagedSnapshot = { hostId: messageHostId, value };
    return;
  }
  dispatchWorkerSnapshot(messageHostId, JSON.stringify(value));
  // Metrics admission is independent from presentation. Acknowledge it now so
  // the main thread can forward the latest ResizeObserver generation while the
  // framework/raster mailboxes coalesce older work.
  post("snapshot-applied", { generation: value.resizeEpoch.generation });
}

function glRuntime(): EmscriptenGlRuntime {
  const scope = globalThis as typeof globalThis & {
    SkiaSharpGL?: EmscriptenGlRuntime;
    SkiaSharpModule?: { GL?: EmscriptenGlRuntime };
    Module?: { GL?: EmscriptenGlRuntime };
    GL?: EmscriptenGlRuntime;
  };
  const runtime = scope.SkiaSharpGL ?? scope.SkiaSharpModule?.GL ?? scope.Module?.GL ?? scope.GL;
  if (!runtime) throw new Error("Doroti worker could not resolve the SkiaSharp Emscripten GL runtime.");
  return runtime;
}

function currentGl(value: WorkerPresenter): WebGL2RenderingContext {
  const runtime = glRuntime();
  runtime.makeContextCurrent(value.context);
  if (!runtime.currentContext?.GLctx) throw new Error("Doroti worker WebGL2 context is not current.");
  return runtime.currentContext.GLctx;
}

function gpuIdentity(gl: WebGL2RenderingContext) {
  const debug = gl.getExtension("WEBGL_debug_renderer_info");
  const vendor = String(debug ? gl.getParameter(debug.UNMASKED_VENDOR_WEBGL) : gl.getParameter(gl.VENDOR));
  const renderer = String(debug ? gl.getParameter(debug.UNMASKED_RENDERER_WEBGL) : gl.getParameter(gl.RENDERER));
  const softwareFallbackUsed = /swiftshader|llvmpipe|software/.test(`${vendor} ${renderer}`.toLowerCase());
  if (softwareFallbackUsed) throw new Error(`Doroti rejected software WebGL renderer '${renderer}'.`);
  return { api: "webgl2", vendor, renderer, hardware: true, softwareFallbackUsed };
}

function ensurePresenter(): WorkerPresenter {
  if (presenter) return presenter;
  if (!surface) throw new Error("Doroti worker managed surface exports are unavailable.");
  const canvas = transferredCanvas ?? new OffscreenCanvas(1, 1);
  const runtime = glRuntime();
  const context = runtime.createContext(canvas, {
    alpha: 1, depth: 1, stencil: 8, antialias: 0, premultipliedAlpha: 1,
    preserveDrawingBuffer: 0, preferLowPowerToHighPerformance: 0,
    failIfMajorPerformanceCaveat: 1, majorVersion: 2, minorVersion: 0,
    enableExtensionsByDefault: 1, explicitSwapControl: 0, renderViaOffscreenBackBuffer: 0,
  });
  if (!context) throw new Error("Doroti worker requires an actual hardware OffscreenCanvas WebGL2 context.");
  presenter = {
    canvas, context, contextGeneration: 1, extension: null,
    current: null, latest: null, draining: false, nextRequestId: 0, contextLost: false,
    bitmapCreated: 0, bitmapConsumed: 0, bitmapClosed: 0, activeBitmaps: 0,
  };
  const gl = currentGl(presenter);
  presenter.extension = gl.getExtension("WEBGL_lose_context");
  canvas.addEventListener("webglcontextlost", (event) => {
    event.preventDefault();
    if (!presenter) return;
    presenter.contextLost = true;
    const interrupted = presenter.current;
    if (interrupted) terminal(interrupted, "failed", "worker WebGL context lost");
    for (const [requestId, receipt] of pendingReceipts) {
      pendingReceipts.delete(requestId);
      receipt.resolve({ committed: false, consumed: false, reason: "worker WebGL context lost" });
    }
    presenter.current = null;
    surface?.ContextLost(interrupted?.requestId ?? 0, interrupted?.generation ?? 0);
    post("context-lost", { contextGeneration: presenter.contextGeneration });
  });
  canvas.addEventListener("webglcontextrestored", () => {
    if (!presenter) return;
    presenter.contextLost = false;
    presenter.contextGeneration++;
    surface?.ContextRestored();
    post("context-restored", { contextGeneration: presenter.contextGeneration });
    scheduleDrain();
  });
  const identity = gpuIdentity(gl);
  if (snapshot) snapshot = { ...snapshot, gpu: identity };
  post("gpu-ready", { gpu: identity, contextGeneration: presenter.contextGeneration });
  return presenter;
}

function terminal(request: PresentRequest, value: "submitted" | "superseded" | "dropped" | "failed", detail: string): void {
  if (request.terminal) return;
  request.terminal = true;
  post("terminal", {
    requestId: request.requestId, generation: request.generation, terminal: value, detail,
    queueDepth: presenter ? Number(presenter.current !== null) + Number(presenter.latest !== null) : 0,
  });
}

function requestPresent(epoch: ResizeEpoch): void {
  const value = ensurePresenter();
  const request: PresentRequest = { ...epoch, requestId: ++value.nextRequestId, terminal: false };
  post("present-requested", { requestId: request.requestId, epoch });
  if (value.latest) terminal(value.latest, "superseded", "latest worker request replaced");
  value.latest = request;
  scheduleDrain();
}

function scheduleDrain(): void {
  const value = presenter;
  if (!value || value.draining || value.current || value.contextLost || !value.latest) return;
  value.draining = true;
  void drain(value);
}

async function drain(value: WorkerPresenter): Promise<void> {
  try {
    while (!value.current && !value.contextLost && value.latest) {
      // ImageBitmap has snapshot semantics, so raster can safely continue
      // before the preceding bitmaprenderer receipt returns. Bound that
      // overlap to two display receipts; this removes a cross-thread ACK from
      // every resize frame's critical path without creating an unbounded
      // transferable/resource queue.
      if (pendingReceiptWork.size >= 2) {
        await Promise.race(pendingReceiptWork);
        continue;
      }
      const request = value.latest;
      value.latest = null;
      value.current = request;
      await render(value, request);
      value.current = null;
    }
  } finally {
    value.draining = false;
    if (!value.current && !value.contextLost && value.latest) scheduleDrain();
  }
}

async function render(value: WorkerPresenter, request: PresentRequest): Promise<void> {
  let bitmap: ImageBitmap | null = null;
  try {
    if (!snapshot || snapshot.resizeEpoch.generation !== request.generation) {
      terminal(request, "superseded", "worker target changed before raster");
      return;
    }
    if (value.canvas.width !== request.physicalWidth || value.canvas.height !== request.physicalHeight) {
      value.canvas.width = request.physicalWidth;
      value.canvas.height = request.physicalHeight;
      snapshot = { ...snapshot, surfaceGeneration: snapshot.surfaceGeneration + 1 };
      dispatchWorkerSnapshot(hostId, JSON.stringify(snapshot));
    }
    const gl = currentGl(value);
    gl.bindFramebuffer(gl.FRAMEBUFFER, null);
    gl.drawBuffers([gl.BACK]);
    gl.viewport(0, 0, request.physicalWidth, request.physicalHeight);
    const result = String(surface!.RenderFrame(
      request.requestId, request.generation, request.logicalWidth, request.logicalHeight,
      request.physicalWidth, request.physicalHeight, request.devicePixelRatio,
      request.timestampMicroseconds, 0, 8, 0, value.contextGeneration, true));
    if (result !== "exact-rendered" && result !== "replay-rendered") {
      surface!.CompleteFrame(request.requestId, request.generation, "superseded", `managed raster result=${result}`);
      terminal(request, "superseded", `managed raster result=${result}`);
      return;
    }
    gl.flush();
    if (workerMode === "worker-direct-webgl") {
      surface!.CompleteFrame(request.requestId, request.generation, "submitted",
        "exact direct framebuffer submitted in the worker");
      terminal(request, "submitted", "exact direct framebuffer submitted in the worker");
      post("direct-commit", {
        requestId: request.requestId, generation: request.generation,
        contextGeneration: value.contextGeneration,
        physicalWidth: request.physicalWidth, physicalHeight: request.physicalHeight,
        logicalWidth: request.logicalWidth, logicalHeight: request.logicalHeight,
      });
      post("resource", {
        bitmapCreated: 0, bitmapConsumed: 0, bitmapClosed: 0, activeBitmaps: 0,
        contextGeneration: value.contextGeneration,
        rasterWidth: value.canvas.width, rasterHeight: value.canvas.height,
      });
      return;
    }
    bitmap = await createImageBitmap(value.canvas);
    value.bitmapCreated++;
    value.activeBitmaps++;
    // The bitmap remains exact for the immutable request epoch even if newer
    // metrics arrive while createImageBitmap is in flight. Let the main-thread
    // visible owner admit it monotonically, then continue with the queued latest
    // target. Rejecting every completed bitmap here starves live resize whenever
    // metrics arrive faster than the worker bitmap pipeline.
    if (value.contextLost ||
        bitmap.width !== request.physicalWidth || bitmap.height !== request.physicalHeight) {
      bitmap.close();
      bitmap = null;
      value.bitmapClosed++;
      value.activeBitmaps--;
      surface!.CompleteFrame(request.requestId, request.generation, "superseded", "worker bitmap became invalid");
      terminal(request, "superseded", "worker bitmap became invalid");
      return;
    }
    const receipt = new Promise<{ committed: boolean; consumed: boolean; reason: string }>((resolve) => {
      pendingReceipts.set(request.requestId, { resolve });
    });
    post("bitmap", {
      requestId: request.requestId, generation: request.generation,
      contextGeneration: value.contextGeneration,
      logicalWidth: request.logicalWidth, logicalHeight: request.logicalHeight,
      physicalWidth: request.physicalWidth, physicalHeight: request.physicalHeight,
      devicePixelRatio: request.devicePixelRatio, bitmap,
    }, [bitmap]);
    bitmap = null;
    const receiptWork = receipt.then((resultReceipt) => {
      try {
        value.activeBitmaps--;
        if (resultReceipt.consumed) value.bitmapConsumed++;
        else value.bitmapClosed++;
        surface!.CompleteFrame(request.requestId, request.generation,
          resultReceipt.committed ? "submitted" : "superseded", resultReceipt.reason);
        terminal(request, resultReceipt.committed ? "submitted" : "superseded", resultReceipt.reason);
      } catch (error) {
        terminal(request, "failed", String(error));
      } finally {
        post("resource", {
          bitmapCreated: value.bitmapCreated, bitmapConsumed: value.bitmapConsumed,
          bitmapClosed: value.bitmapClosed, activeBitmaps: value.activeBitmaps,
          contextGeneration: value.contextGeneration,
          rasterWidth: value.canvas.width, rasterHeight: value.canvas.height,
        });
      }
    });
    pendingReceiptWork.add(receiptWork);
    void receiptWork.finally(() => {
      pendingReceiptWork.delete(receiptWork);
      scheduleDrain();
    });
  } catch (error) {
    if (bitmap) {
      bitmap.close();
      value.bitmapClosed++;
      value.activeBitmaps--;
    }
    try { surface?.CompleteFrame(request.requestId, request.generation, "failed", String(error)); } catch { }
    terminal(request, "failed", String(error));
  }
}

configureWorkerBridge({
  rendererIdentity() {
    return workerMode === "worker-direct-webgl"
      ? "worker-transferred-visible-canvas-webgl2-direct"
      : "worker-offscreen-canvas-webgl2-imagebitmap";
  },
  createHost(id, canvasId, logicalWidth, logicalHeight) {
    if (!snapshot) throw new Error("Doroti worker initial snapshot is unavailable.");
    hostId = id;
    snapshot = { ...snapshot, canvasId, logicalWidth, logicalHeight };
    return JSON.stringify(snapshot);
  },
  showHost() {
    if (!snapshot) throw new Error("Doroti worker snapshot is unavailable.");
    return JSON.stringify(snapshot);
  },
  resizeHost(_id, logicalWidth, logicalHeight) {
    if (!snapshot) throw new Error("Doroti worker snapshot is unavailable.");
    snapshot = { ...snapshot, logicalWidth, logicalHeight };
    return JSON.stringify(snapshot);
  },
  requestFrame(id, callbackId) {
    if (workerMode === "worker-direct-webgl") {
      if (typeof globalThis.requestAnimationFrame !== "function")
        throw new Error("Doroti direct worker requires Worker requestAnimationFrame.");
      globalThis.requestAnimationFrame((timestamp) => dispatchWorkerAnimationFrame(id, callbackId, timestamp));
      return;
    }
    post("frame-request", { hostId: id, callbackId });
  },
  recordManagedRaster(id, phase, width, height, duration) {
    post("managed-raster", { hostId: id, phase, width, height, durationMicroseconds: duration });
  },
  requestPresent(_canvasId, epoch) { requestPresent(epoch); },
  captureResizeTrace() { return "[]"; },
  closeHost(id) { post("closed", { hostId: id }); },
  resolveResourceUrl(relativeUrl) {
    const applicationBase = new URL("../../", globalThis.location.href);
    return new URL(relativeUrl, applicationBase).href;
  },
  postControl(kind, payload) { post("control", { controlKind: kind, payload }); },
  requestControl(kind, payload) {
    const correlationId = ++controlSequence;
    const promise = new Promise<string>((resolve, reject) => pendingControls.set(correlationId, { resolve, reject }));
    post("control-request", { correlationId, controlKind: kind, payload });
    return promise;
  },
});

globalThis.addEventListener("message", (event: MessageEvent) => {
  let message: Record<string, unknown>;
  try {
    message = decodeDorotiMessage(event.data, inboundKinds);
  } catch (error) {
    post("fatal", { error: String(error) });
    return;
  }
  switch (message.kind) {
    case "init":
      runtimeState.transition("booting");
      snapshot = message.snapshot as HostSnapshot;
      workerMode = String(message.mode ?? "offscreen-worker") as WorkerMode;
      if (workerMode !== "offscreen-worker" && workerMode !== "worker-direct-webgl")
        throw new Error(`Unknown Doroti worker mode '${workerMode}'.`);
      transferredCanvas = message.canvas instanceof OffscreenCanvas ? message.canvas : null;
      if (workerMode === "worker-direct-webgl" && !transferredCanvas)
        throw new Error("Doroti direct worker init requires a transferred visible OffscreenCanvas.");
      dotnetModuleUrl = String(message.dotnetModuleUrl ?? "");
      void startManagedRuntime();
      break;
    case "snapshot":
      applyManagedSnapshot(Number(message.hostId), message.snapshot as HostSnapshot);
      break;
    case "frame":
      // BrowserInterop is installed before StartWorker creates the host. Its
      // first framework frame is part of host startup, so waiting for
      // StartWorker to return here drops the only callback and deadlocks the
      // initial GPU/presenter handshake.
      dispatchWorkerAnimationFrame(hostId, Number(message.callbackId), Number(message.timestamp));
      break;
    case "input":
      if (!managedHostReady) {
        // Window activation can deliver focus/pointer state while the worker
        // runtime is still bootstrapping. Keep a bounded startup mailbox rather
        // than calling the not-yet-installed managed callback ABI.
        if (pendingManagedInputs.length >= 256) pendingManagedInputs.shift();
        pendingManagedInputs.push(message);
      } else {
        dispatchWorkerInput(message);
      }
      break;
    case "receipt": {
      const pending = pendingReceipts.get(Number(message.requestId));
      if (pending) {
        pendingReceipts.delete(Number(message.requestId));
        pending.resolve({
          committed: Boolean(message.committed),
          consumed: Boolean(message.consumed),
          reason: String(message.reason ?? "display receipt"),
        });
      }
      break;
    }
    case "control-response": {
      const pending = pendingControls.get(Number(message.correlationId));
      if (pending) {
        pendingControls.delete(Number(message.correlationId));
        if (message.error) pending.reject(new Error(String(message.error)));
        else pending.resolve(String(message.result ?? ""));
      }
      break;
    }
    case "context":
      if (message.action === "lose") ensurePresenter().extension?.loseContext();
      else ensurePresenter().extension?.restoreContext();
      break;
    case "dispose":
      runtimeState.transition("disposing");
      if (presenter?.current) {
        surface?.CompleteFrame(presenter.current.requestId, presenter.current.generation,
          "dropped", "worker runtime disposing");
        terminal(presenter.current, "dropped", "worker runtime disposing");
      }
      if (presenter?.latest) {
        surface?.CompleteFrame(presenter.latest.requestId, presenter.latest.generation,
          "dropped", "worker runtime disposing");
        terminal(presenter.latest, "dropped", "worker runtime disposing");
      }
      if (presenter) {
        presenter.current = null;
        presenter.latest = null;
      }
      for (const [requestId, receipt] of pendingReceipts) {
        pendingReceipts.delete(requestId);
        receipt.resolve({ committed: false, consumed: false, reason: "worker runtime disposing" });
      }
      stopManagedRuntime?.();
      stopManagedRuntime = null;
      glRuntime().deleteContext?.(presenter?.context ?? 0);
      managedRuntime?.exit(0);
      managedRuntime = null;
      runtimeState.transition("disposed");
      post("disposed", { activeRequests: 0, activeReceipts: pendingReceipts.size });
      close();
      break;
    case "crash":
      runtimeState.transition("fatal");
      post("fatal", { error: "diagnostic worker crash" });
      break;
  }
});

async function startManagedRuntime(): Promise<void> {
  try {
    const dotnetUrl = dotnetModuleUrl || new URL("../../_framework/dotnet.js", import.meta.url).href;
    const dotnetModule = await import(dotnetUrl) as { dotnet: { create(): Promise<DotnetRuntime> } };
    const runtime = await dotnetModule.dotnet.create();
    managedRuntime = runtime;
    await initializeManagedCallbacks();
    const hostExports = await runtime.getAssemblyExports("Doroti.Host.Web.dll") as {
      Doroti: { Host: { Web: { DorotiWebWorkerSurface: SurfaceExports } } };
    };
    surface = hostExports.Doroti.Host.Web.DorotiWebWorkerSurface;
    const config = runtime.getConfig();
    const appExports = await runtime.getAssemblyExports(config.mainAssemblyName) as {
      Doroti: { Generated: { DorotiBootstrap: { StartWorker(): Promise<string>; StopWorker(): void } } };
    };
    stopManagedRuntime = appExports.Doroti.Generated.DorotiBootstrap.StopWorker;
    const result = await appExports.Doroti.Generated.DorotiBootstrap.StartWorker();
    managedHostReady = true;
    runtimeState.transition("ready");
    if (pendingManagedSnapshot) {
      const pending = pendingManagedSnapshot;
      pendingManagedSnapshot = null;
      applyManagedSnapshot(pending.hostId, pending.value);
    }
    for (const input of pendingManagedInputs.splice(0)) dispatchWorkerInput(input);
    post("runtime-ready", { result, mainManagedRuntimeCount: 0, workerManagedRuntimeCount: 1 });
  } catch (error) {
    post("fatal", { error: String(error instanceof Error ? error.stack ?? error.message : error) });
  }
}
