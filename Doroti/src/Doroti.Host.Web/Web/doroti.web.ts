import { BrowserViewEnvironment } from "./doroti.web.environment.js";
import { decodeDorotiMessage, dorotiProtocolVersion, dorotiWebGpuRendererVersion } from "./doroti.web.protocol.js";
import { initializeBrowserTimers } from "./doroti.web.timers.js";
export { scheduleBrowserTimer, cancelBrowserTimer, cancelBrowserTimerOwner } from "./doroti.web.timers.js";
import { createDorotiDomEndpoints, createReplacementCanvas } from "./doroti.web.dom.js";
import { pushBounded } from "./doroti.web.diagnostics.js";
import { createWorkerVisibleSurface } from "./doroti.web.surface.js";
import { closeExternalLeases, createDorotiWorker } from "./doroti.web.worker-host.js";
import { createManagedDorotiWorker, type DorotiWorkerEndpoint } from "./doroti.web.managed-worker.js";

interface ManagedCallbacks {
  dispatchAnimationFrame(hostId: number, callbackId: number, timestamp: number): void;
  dispatchSnapshot(hostId: number, snapshotJson: string): void;
  dispatchResizeEpoch(
    hostId: number, hostGeneration: number, generation: number,
    logicalWidth: number, logicalHeight: number, physicalWidth: number, physicalHeight: number,
    devicePixelRatio: number, timestampMicroseconds: number): void;
  dispatchPointerBatch(hostId: number, phase: number, kind: number, pointerId: number, buttons: number, modifiers: number, inputSequence: number, samples: number[]): void;
  dispatchWheel(hostId: number, x: number, y: number, deltaX: number, deltaY: number, timestamp: number, kind: number, inputSequence: number): void;
  dispatchKey(hostId: number, pressed: boolean, repeat: boolean, synthesized: boolean, code: string, key: string, timestamp: number, inputSequence: number): void;
  dispatchFocus(hostId: number, focused: boolean, timestamp: number, inputSequence: number): void;
  dispatchTextEditing(hostId: number, text: string, selectionBase: number, selectionExtent: number, composingBase: number, composingExtent: number, inputSequence: number): void;
  dispatchTextAction(hostId: number, action: number, inputSequence: number): void;
  dispatchTextConnectionClosed(hostId: number, inputSequence: number): void;
  dispatchSemanticsAction(hostId: number, nodeId: number, action: number, inputSequence: number, argumentsJson: string): void;
}

interface GpuIdentity {
  api: "webgl2" | "webgpu";
  vendor: string;
  renderer: string;
  hardware: true;
  softwareFallbackUsed: boolean;
  contextGeneration?: number;
  surfaceGeneration?: number;
}

interface ListenerRegistration {
  target: EventTarget;
  name: string;
  handler: EventListener;
}

interface BrowserHost {
  environment?: BrowserViewEnvironment;
  id: number;
  root: HTMLElement;
  canvas: HTMLCanvasElement;
  input: HTMLTextAreaElement;
  semantics: HTMLElement;
  semanticsElements: Map<number, HTMLElement>;
  semanticsListeners: Map<number, AbortController>;
  semanticsContentSignatures: Map<number, string>;
  semanticsActionNodes: Map<number, SemanticsNode>;
  semanticsObservedEditing: Map<number, { value: string; base: number; extent: number }>;
  semanticsProjectionDepth: number;
  focusedTextFieldSemanticsId: number | null;
  logicalWidth: number;
  logicalHeight: number;
  generation: number;
  surfaceGeneration: number;
  resizeGeneration: number;
  emittedResizeGeneration: number;
  resizeEpoch: ResizeEpoch;
  resizeTrace: ResizeTraceEntry[];
  resizeTraceSequence: number;
  inputSequence: number;
  diagnosticsPublishTimer: number;
  dprQuery: MediaQueryList | null;
  frameRaf: number;
  latestFrameCallback: number;
  lastWheelDeltaX: number;
  lastWheelDeltaY: number;
  lastWheelTimestamp: number;
  lastWheelWasTrackpad: boolean;
  gpu: GpuIdentity;
  observers: ResizeObserver[];
  listeners: ListenerRegistration[];
  composing: boolean;
  compositionStart: number;
  lastTextValue: string;
  lastSelectionBase: number;
  lastSelectionExtent: number;
  lastComposingBase: number;
  lastComposingExtent: number;
  viewFocused: boolean;
  pressedKeys: Map<string, string>;
  inputAction: number;
  multiline: boolean;
  interactiveSelectionEnabled: boolean;
  pendingBlurConnectionCloseTimer: number;
  editableGeometryApplied: boolean;
  contextMenuEnabled: boolean;
  frameworkCursor: string;
  pointerCaptureCursor: string | null;
}

interface ResizeEpoch {
  generation: number;
  logicalWidth: number;
  logicalHeight: number;
  physicalWidth: number;
  physicalHeight: number;
  devicePixelRatio: number;
  timestampMicroseconds: number;
}

interface ResizeTraceEntry {
  sequence: number;
  timestampMicroseconds: number;
  phase: string;
  epoch: ResizeEpoch;
  threadId: number;
  source: string;
  durationMicroseconds: number;
  rafId: number;
  backingWidth: number;
  backingHeight: number;
  surfaceWidth: number;
  surfaceHeight: number;
  terminal: string | null;
  detail: string | null;
  queueDepth: number;
  inputSequence: number;
  requestId: number;
}

type RequestedPresenterMode = "auto" | "worker-direct-webgl" | "worker-direct-webgpu";

interface PresenterPolicy {
  requested: RequestedPresenterMode;
  selected: "worker-direct-webgl" | "worker-direct-webgpu";
  fallbackReason: string | null;
}

interface WorkerBridge {
  rendererIdentity(): string;
  createHost(hostId: number, canvasId: string, logicalWidth: number, logicalHeight: number): string;
  showHost(hostId: number): string;
  resizeHost(hostId: number, logicalWidth: number, logicalHeight: number): string;
  requestFrame(hostId: number, callbackId: number): void;
  recordManagedRaster(hostId: number, phase: string, width: number, height: number, duration: number): void;
  requestPresent(canvasId: string, descriptor: ResizeEpoch): void;
  captureResizeTrace(hostId: number): string;
  closeHost(hostId: number): void;
  resolveResourceUrl(relativeUrl: string): string;
  postControl(kind: string, payload: Record<string, unknown>): void;
  requestControl(kind: string, payload: Record<string, unknown>): Promise<string>;
}

interface WorkerDisplayPresenter {
  worker: DorotiWorkerEndpoint;
  runtimeLocation: "main" | "worker";
  mode: "worker-direct-webgl" | "worker-direct-webgpu";
  currentRequestId: number | null;
  latestRequestId: number | null;
  contextGeneration: number;
  contextLost: boolean;
  frontGeneration: number;
  frontRequestId: number;
  rasterWidth: number;
  rasterHeight: number;
  displayWidth: number;
  displayHeight: number;
  bitmapCreated: number;
  bitmapConsumed: number;
  bitmapClosed: number;
  activeBitmaps: number;
  restartCount: number;
  runtimeSessionId: number;
  pendingLeases: Map<number, { runtimeSessionId: number; causalFrameId: number }>;
}

interface ResizeDiagnostics {
  hosts(): number[];
  capture(hostId: number): string;
  trace(hostId: number): string;
  reset(hostId: number): void;
  snapshot(hostId: number): string;
  presenter(canvasId: string): string;
  capability(canvasId: string): string;
  loseContext(canvasId: string): boolean;
  restoreContext(canvasId: string): boolean;
  crashWorker(canvasId: string): boolean;
  violateWorkerProtocol(canvasId: string): boolean;
}

interface SemanticsFlags {
  checked?: string; selected?: boolean; enabled?: boolean; toggled?: boolean;
  expanded?: boolean; required?: boolean; focused?: boolean; button?: boolean;
  textField?: boolean; header?: boolean; hidden?: boolean; image?: boolean;
  liveRegion?: boolean; multiline?: boolean; readOnly?: boolean; link?: boolean; slider?: boolean;
  focusable?: boolean; obscured?: boolean; mutuallyExclusive?: boolean; keyboardKey?: boolean;
}

interface SemanticsNode {
  id: number | string;
  contentUnchanged?: boolean;
  role?: string;
  label?: string;
  value?: string;
  actions?: number;
  children?: number[];
  flags?: SemanticsFlags;
  textSelectionBase?: number;
  textSelectionExtent?: number;
  identifier?: string;
  hint?: string;
  tooltip?: string;
  increasedValue?: string;
  decreasedValue?: string;
  headingLevel?: number;
  linkUrl?: string;
  validationResult?: string;
  hitTestBehavior?: string;
  inputType?: string;
  minValue?: string;
  maxValue?: string;
  maxValueLength?: number;
  currentValueLength?: number;
  scrollPosition?: number;
  scrollExtentMin?: number;
  scrollExtentMax?: number;
  scrollChildCount?: number;
  scrollIndex?: number;
  controlsNodes?: string[];
  locale?: string;
  rect: [number, number, number, number];
}

interface SemanticsUpdate {
  generation: number;
  nodes?: SemanticsNode[];
}

interface PluginRequest {
  channel: string;
  codec: string;
  payloadBase64: string;
}

interface DotnetRuntime {
  getAssemblyExports(assemblyName: string): Promise<unknown>;
}

interface DorotiAssemblyExports {
  Doroti: {
    Host: {
      Web: {
        BrowserTimeProvider: { DispatchTimer(id: number, generation: number): void };
        BrowserInterop: {
          DispatchAnimationFrame: ManagedCallbacks["dispatchAnimationFrame"];
          DispatchSnapshot: ManagedCallbacks["dispatchSnapshot"];
          DispatchResizeEpoch: ManagedCallbacks["dispatchResizeEpoch"];
          DispatchPointerBatch: ManagedCallbacks["dispatchPointerBatch"];
          DispatchWheel: ManagedCallbacks["dispatchWheel"];
          DispatchKey: ManagedCallbacks["dispatchKey"];
          DispatchFocus: ManagedCallbacks["dispatchFocus"];
          DispatchTextEditing: ManagedCallbacks["dispatchTextEditing"];
          DispatchTextAction: ManagedCallbacks["dispatchTextAction"];
          DispatchTextConnectionClosed: ManagedCallbacks["dispatchTextConnectionClosed"];
          DispatchSemanticsAction: ManagedCallbacks["dispatchSemanticsAction"];
        };
      };
    };
  };
}

const hosts = new Map<number, BrowserHost>();
const workerDisplayPresenters = new Map<string, WorkerDisplayPresenter>();
let managed: ManagedCallbacks | null = null;
let activeWorkerBridge: WorkerBridge | null = null;
let directWorkerBootstrap = false;

export function configureWorkerBridge(bridge: WorkerBridge): void {
  if (typeof document !== "undefined")
    throw new Error("Doroti worker bridge can only be installed in a Web Worker.");
  activeWorkerBridge = bridge;
}

export function dispatchWorkerSnapshot(hostId: number, json: string): void {
  requireManaged().dispatchSnapshot(hostId, json);
}

export function dispatchWorkerResizeEpoch(
  hostId: number, hostGeneration: number, generation: number,
  logicalWidth: number, logicalHeight: number, physicalWidth: number, physicalHeight: number,
  devicePixelRatio: number, timestampMicroseconds: number): void {
  requireManaged().dispatchResizeEpoch(
    hostId, hostGeneration, generation, logicalWidth, logicalHeight,
    physicalWidth, physicalHeight, devicePixelRatio, timestampMicroseconds);
}

export function dispatchWorkerAnimationFrame(
  hostId: number, callbackId: number, timestamp: number): void {
  requireManaged().dispatchAnimationFrame(hostId, callbackId, timestamp);
}

export function dispatchWorkerInput(message: Record<string, unknown>): void {
  const callbacks = requireManaged();
  const id = Number(message.hostId);
  const payload = (message.payload ?? {}) as Record<string, unknown>;
  switch (message.inputKind) {
    case "pointer":
      callbacks.dispatchPointerBatch(
        id, Number(payload.phase), Number(payload.kind), Number(payload.pointerId),
        Number(payload.buttons), Number(payload.modifiers), Number(message.inputSequence), payload.samples as number[]);
      break;
    case "wheel":
      callbacks.dispatchWheel(
        id, Number(payload.x), Number(payload.y), Number(payload.deltaX), Number(payload.deltaY),
        Number(payload.timestamp), Number(payload.kind), Number(message.inputSequence));
      break;
    case "key":
      callbacks.dispatchKey(
        id, Boolean(payload.pressed), Boolean(payload.repeat), Boolean(payload.synthesized),
        String(payload.code), String(payload.key), Number(payload.timestamp), Number(message.inputSequence));
      break;
    case "focus":
      callbacks.dispatchFocus(id, Boolean(payload.focused), Number(payload.timestamp), Number(message.inputSequence));
      break;
    case "text":
      callbacks.dispatchTextEditing(
        id, String(payload.text), Number(payload.selectionBase), Number(payload.selectionExtent),
        Number(payload.composingBase), Number(payload.composingExtent), Number(message.inputSequence));
      break;
    case "text-action":
      callbacks.dispatchTextAction(id, Number(payload.action), Number(message.inputSequence));
      break;
    case "text-closed":
      callbacks.dispatchTextConnectionClosed(id, Number(message.inputSequence));
      break;
    case "semantics-action":
      callbacks.dispatchSemanticsAction(
        id, Number(payload.nodeId), Number(payload.action), Number(message.inputSequence),
        String(payload.argumentsJson ?? "null"));
      break;
    default:
      throw new Error(`Unknown Doroti worker input kind '${String(message.inputKind)}'.`);
  }
}

function presenterPolicy(): PresenterPolicy {
  const value = new URLSearchParams(globalThis.location.search).get("dorotiRenderer");
  const requested = value === "worker-direct-webgl" || value === "worker-direct-webgpu" ? value : "auto";
  return { requested, selected: requested === "auto" ? "worker-direct-webgpu" : requested, fallbackReason: null };
}

const resizeDiagnostics: ResizeDiagnostics = {
  hosts: () => [...hosts.keys()],
  capture: (hostId) => captureResizeTrace(hostId),
  trace: (hostId) => captureResizeTrace(hostId),
  reset: (hostId) => resetDiagnostics(hostId),
  snapshot: (hostId) => snapshot(requireHost(hostId)),
  presenter: (canvasId) => {
    const workerPresenter = workerDisplayPresenters.get(canvasId);
    if (!workerPresenter)
      throw new Error(`Canvas presenter '${canvasId}' is not initialized.`);
    return JSON.stringify({
      context: 0,
      requestedMode: presenterPolicy().requested,
      mode: workerPresenter.mode,
      fallbackReason: null,
      contextGeneration: workerPresenter.contextGeneration,
      currentRequestId: workerPresenter.currentRequestId,
      latestRequestId: workerPresenter.latestRequestId,
      queueDepth: Number(workerPresenter.currentRequestId !== null) + Number(workerPresenter.latestRequestId !== null),
      contextLost: workerPresenter.contextLost,
      frontGeneration: workerPresenter.frontGeneration || null,
      frontRequestId: workerPresenter.frontRequestId || null,
      frontFramebufferId: null,
      stagingFramebufferId: null,
      rasterCanvasAttached: false,
      visibleContext: workerPresenter.mode === "worker-direct-webgl"
        ? "transferred-offscreen-webgl2" : "transferred-offscreen-webgpu",
      rasterWidth: workerPresenter.rasterWidth,
      rasterHeight: workerPresenter.rasterHeight,
      displayWidth: workerPresenter.displayWidth,
      displayHeight: workerPresenter.displayHeight,
      bitmapCreated: workerPresenter.bitmapCreated,
      bitmapConsumed: workerPresenter.bitmapConsumed,
      bitmapClosed: workerPresenter.bitmapClosed,
      activeBitmaps: workerPresenter.activeBitmaps,
      mainManagedRuntimeCount: workerPresenter.runtimeLocation === "main" ? 1 : 0,
      workerManagedRuntimeCount: workerPresenter.runtimeLocation === "main" ? 0 : 1,
      workerRestartCount: workerPresenter.restartCount,
      runtimeSessionId: workerPresenter.runtimeSessionId,
      unpairedRequestCount: workerPresenter.pendingLeases.size,
    });
  },
  capability: (canvasId) => {
    const host = [...hosts.values()].find((candidate) => candidate.canvas.id === canvasId);
    if (!host) throw new Error(`Canvas host '${canvasId}' is not initialized.`);
    const workerPresenter = workerDisplayPresenters.get(canvasId);
    const json = JSON.parse(resizeDiagnostics.presenter(canvasId)) as Record<string, unknown>;
    return JSON.stringify({
      offscreenCanvas: typeof OffscreenCanvas !== "undefined",
      mode: json.mode,
      actualManagedSkiaRaster: Number(json.frontGeneration ?? 0) > 0,
      rasterCanvasAttached: json.rasterCanvasAttached,
      hardwareWebGl2: host.gpu.hardware && !host.gpu.softwareFallbackUsed && host.gpu.api === "webgl2",
      gpu: host.gpu,
      hardwareWebGpu: host.gpu.hardware && !host.gpu.softwareFallbackUsed && host.gpu.api === "webgpu",
      exactBitmapCommit: false,
      exactDirectCommit:
        Number(json.frontGeneration ?? 0) === host.resizeEpoch.generation,
      bitmapCreated: json.bitmapCreated,
      bitmapConsumed: json.bitmapConsumed,
      bitmapClosed: json.bitmapClosed,
      activeBitmaps: json.activeBitmaps,
    });
  },
  loseContext: (canvasId) => changeDiagnosticContextState(canvasId, true),
  restoreContext: (canvasId) => changeDiagnosticContextState(canvasId, false),
  crashWorker: (canvasId) => {
    const presenter = workerDisplayPresenters.get(canvasId);
    if (!presenter) return false;
    presenter.worker.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "crash" });
    return true;
  },
  violateWorkerProtocol: (canvasId) => {
    const presenter = workerDisplayPresenters.get(canvasId);
    if (!presenter) return false;
    presenter.worker.postMessage({ protocolVersion: 999, kind: "input" });
    return true;
  },
};
(globalThis as typeof globalThis & { __dorotiResizeDiagnostics?: ResizeDiagnostics })
  .__dorotiResizeDiagnostics = resizeDiagnostics;

function snapshot(host: BrowserHost): string {
  const ratio = host.resizeEpoch.devicePixelRatio;
  return JSON.stringify({
    ...host.environment?.value,
    canvasId: host.canvas.id,
    logicalWidth: host.logicalWidth,
    logicalHeight: host.logicalHeight,
    devicePixelRatio: ratio,
    visible: document.visibilityState !== "hidden",
    focused: document.hasFocus() && host.viewFocused,
    languageTag: navigator.language || "en-US",
    brightness: globalThis.matchMedia?.("(prefers-color-scheme: dark)").matches ? "dark" : "light",
    operatingSystem: browserOperatingSystem(),
    generation: host.generation,
    surfaceGeneration: host.surfaceGeneration,
    inputSequence: host.inputSequence,
    gpu: host.gpu,
    resizeEpoch: host.resizeEpoch,
  });
}

function recordResize(
  host: BrowserHost,
  phase: string,
  source: string,
  options: Partial<Pick<ResizeTraceEntry,
    "timestampMicroseconds" | "durationMicroseconds" | "rafId" | "backingWidth" | "backingHeight" |
    "surfaceWidth" | "surfaceHeight" | "terminal" | "detail" |
    "inputSequence" | "requestId">> = {}): void {
  if (!diagnosticsEnabled() && !(phase === "front-commit" && source === "worker-direct-surface" &&
      new URL(location.href).searchParams.get("dorotiInputMarkers") === "1")) return;
  const entry: ResizeTraceEntry = {
    sequence: ++host.resizeTraceSequence,
    timestampMicroseconds: options.timestampMicroseconds ?? Math.round(performance.now() * 1000),
    phase,
    epoch: host.resizeEpoch,
    threadId: 0,
    source,
    durationMicroseconds: options.durationMicroseconds ?? 0,
    rafId: options.rafId ?? 0,
    backingWidth: options.backingWidth ?? host.canvas.width,
    backingHeight: options.backingHeight ?? host.canvas.height,
    surfaceWidth: options.surfaceWidth ?? 0,
    surfaceHeight: options.surfaceHeight ?? 0,
    terminal: options.terminal ?? null,
    detail: options.detail ?? null,
    queueDepth: (() => {
      const workerPresenter = workerDisplayPresenters.get(host.canvas.id);
      if (workerPresenter) return Number(workerPresenter.currentRequestId !== null) +
        Number(workerPresenter.latestRequestId !== null);
      return 0;
    })(),
    inputSequence: options.inputSequence ?? 0,
    requestId: options.requestId ?? 0,
  };
  pushBounded(host.resizeTrace, entry, 16384);
  scheduleResizeDiagnosticsPublish(host);
}

function diagnosticsEnabled(): boolean {
  return new URLSearchParams(globalThis.location.search).get("dorotiResizeDiagnostics") === "1";
}

function scheduleResizeDiagnosticsPublish(host: BrowserHost): void {
  if (!diagnosticsEnabled()) return;
  if (host.diagnosticsPublishTimer !== 0) return;
  host.diagnosticsPublishTimer = globalThis.setTimeout(() => {
    host.diagnosticsPublishTimer = 0;
    if (hosts.has(host.id)) publishResizeDiagnostics(host);
  }, 100);
}

function publishResizeDiagnostics(host: BrowserHost): void {
  if (!diagnosticsEnabled()) return;
  const json = JSON.stringify({ snapshot: JSON.parse(snapshot(host)), trace: host.resizeTrace });
  host.root.setAttribute("data-doroti-resize-diagnostics", json);
  let output = document.getElementById("doroti-resize-diagnostics");
  if (!output) {
    output = document.createElement("script");
    output.id = "doroti-resize-diagnostics";
    output.setAttribute("type", "application/json");
    document.body.appendChild(output);
  }
  output.textContent = json;
}

function resetDiagnostics(hostId: number): void {
  const host = requireHost(hostId);
  host.resizeTrace = [];
  host.resizeTraceSequence = 0;
  publishResizeDiagnostics(host);
}

function updateResizeEpoch(
  host: BrowserHost,
  source: string,
  logicalWidth: number,
  logicalHeight: number,
  forceGeneration = false,
  physicalWidth = Math.max(1, Math.round(logicalWidth * (globalThis.devicePixelRatio > 0 && Number.isFinite(globalThis.devicePixelRatio) ? globalThis.devicePixelRatio : 1))),
  physicalHeight = Math.max(1, Math.round(logicalHeight * (globalThis.devicePixelRatio > 0 && Number.isFinite(globalThis.devicePixelRatio) ? globalThis.devicePixelRatio : 1))),
  ratio = (globalThis.devicePixelRatio > 0 && Number.isFinite(globalThis.devicePixelRatio) ? globalThis.devicePixelRatio : 1)): boolean {
  const previous = host.resizeEpoch;
  const changed = forceGeneration || logicalWidth !== previous.logicalWidth ||
    logicalHeight !== previous.logicalHeight || ratio !== previous.devicePixelRatio ||
    physicalWidth !== previous.physicalWidth || physicalHeight !== previous.physicalHeight;
  if (!changed) {
    recordResize(host, "size-signal", source, { detail: "unchanged" });
    return false;
  }
  host.logicalWidth = logicalWidth;
  host.logicalHeight = logicalHeight;
  host.generation++;
  host.resizeEpoch = {
    generation: ++host.resizeGeneration,
    logicalWidth,
    logicalHeight,
    physicalWidth,
    physicalHeight,
    devicePixelRatio: ratio,
    timestampMicroseconds: Math.round(performance.now() * 1000),
  };
  recordResize(host, "target-observed", source);
  return true;
}

function commitDirectCanvasLogicalSize(
  host: BrowserHost,
  logicalWidth: number,
  logicalHeight: number): void {
  const workerPresenter = workerDisplayPresenters.get(host.canvas.id);
  if (!directWorkerBootstrap && !workerPresenter) return;
  // Direct Skia owns a grow-only physical backing. Its completed front
  // establishes the pixel scale; the root clips it to the observed viewport.
  // Observer targets must not rescale that backing while a new frame is built.
  if (host.canvas.dataset.dorotiCapacityWidth) return;
  // Flutter keeps the DOM display canvas in logical CSS pixels while its
  // raster surface uses physical pixels. A transferred canvas still exposes
  // its Worker-mutated width/height attributes to layout, so main must pin the
  // element's CSS box or DPR > 1 enlarges it by the device scale factor.
  host.canvas.style.width = `${logicalWidth}px`;
  host.canvas.style.height = `${logicalHeight}px`;
  // A transferred canvas can expose its next Worker-owned intrinsic size
  // before the matching direct-commit message reaches main. Preserve the
  // rendered aspect ratio during that cross-thread hand-off instead of
  // allowing the browser's default object-fit: fill to distort the frame.
  host.canvas.style.objectFit = "cover";
  host.canvas.style.objectPosition = "left top";
  host.canvas.style.removeProperty("transform");
  host.canvas.style.removeProperty("transform-origin");
}

function configureDirectCanvasCapacity(
  host: BrowserHost,
  logicalWidth: number,
  logicalHeight: number,
  ratio: number,
  physicalWidth?: number,
  physicalHeight?: number,
  initializeBacking = false): void {
  const screenWidth = Number(globalThis.screen?.availWidth ?? globalThis.screen?.width ?? 0);
  const screenHeight = Number(globalThis.screen?.availHeight ?? globalThis.screen?.height ?? 0);
  const capacityWidth = physicalWidth ?? Math.ceil(
    Math.max(logicalWidth * 1.5, screenWidth, logicalWidth) * ratio);
  const capacityHeight = physicalHeight ?? Math.ceil(
    Math.max(logicalHeight * 1.5, screenHeight, logicalHeight) * ratio);
  // width/height cannot be assigned from main after control was transferred.
  // Worker capacity growth is reported here only to update the matching CSS
  // pixel ratio; initial/replacement canvases opt in before transfer.
  if (initializeBacking) {
    host.canvas.width = capacityWidth;
    host.canvas.height = capacityHeight;
  }
  // Let the browser update intrinsic geometry together with the transferred
  // bitmap. Explicit CSS capacity dimensions arrive via a separate Worker
  // message and can temporarily squeeze a newly grown front into the old box.
  // This transform converts physical pixels to CSS pixels only; it never
  // depends on viewport dimensions or stretches an old frame to a new target.
  host.canvas.style.width = "auto";
  host.canvas.style.height = "auto";
  host.canvas.style.objectFit = "none";
  host.canvas.style.objectPosition = "left top";
  host.canvas.style.transform = `scale(${1 / ratio})`;
  host.canvas.style.transformOrigin = "left top";
  host.canvas.dataset.dorotiCapacityWidth = String(capacityWidth);
  host.canvas.dataset.dorotiCapacityHeight = String(capacityHeight);
  host.canvas.dataset.dorotiCapacityDevicePixelRatio = String(ratio);
}

function commitObservedResize(
  host: BrowserHost,
  source: string,
  logicalWidth: number,
  logicalHeight: number,
  physicalWidth: number,
  physicalHeight: number,
  ratio: number,
  forceGeneration = false): void {
  if (logicalWidth <= 0 || logicalHeight <= 0 || physicalWidth <= 0 || physicalHeight <= 0) return;
  const changed = updateResizeEpoch(
    host, source, logicalWidth, logicalHeight, forceGeneration,
    physicalWidth, physicalHeight, ratio);
  if (changed) commitDirectCanvasLogicalSize(host, logicalWidth, logicalHeight);
  if (changed || host.emittedResizeGeneration !== host.resizeEpoch.generation) {
    host.emittedResizeGeneration = host.resizeEpoch.generation;
    emitResize(host);
  }
}

function armDprWatcher(host: BrowserHost): void {
  if (!globalThis.matchMedia) return;
  const query = globalThis.matchMedia(`(resolution: ${(globalThis.devicePixelRatio > 0 && Number.isFinite(globalThis.devicePixelRatio) ? globalThis.devicePixelRatio : 1)}dppx)`);
  host.dprQuery = query;
  const handler: EventListener = () => {
    const rect = host.root.getBoundingClientRect();
    const ratio = (globalThis.devicePixelRatio > 0 && Number.isFinite(globalThis.devicePixelRatio) ? globalThis.devicePixelRatio : 1);
    commitObservedResize(
      host, "dpr-watcher", rect.width, rect.height,
      Math.max(1, Math.round(rect.width * ratio)),
      Math.max(1, Math.round(rect.height * ratio)), ratio);
    armDprWatcher(host);
  };
  query.addEventListener("change", handler, { once: true });
  host.listeners.push({ target: query, name: "change", handler });
}

function observeResizeEntry(host: BrowserHost, entry: ResizeObserverEntry): void {
  const content = Array.isArray(entry.contentBoxSize)
    ? entry.contentBoxSize[0]
    : entry.contentBoxSize;
  const logicalWidth = content?.inlineSize ?? entry.contentRect.width;
  const logicalHeight = content?.blockSize ?? entry.contentRect.height;
  const ratio = (globalThis.devicePixelRatio > 0 && Number.isFinite(globalThis.devicePixelRatio) ? globalThis.devicePixelRatio : 1);
  const physicalBoxes = entry.devicePixelContentBoxSize;
  const physical = Array.isArray(physicalBoxes) ? physicalBoxes[0] : physicalBoxes;
  const declaredPhysicalWidth = Math.max(1, Math.round(logicalWidth * ratio));
  const declaredPhysicalHeight = Math.max(1, Math.round(logicalHeight * ratio));
  const observedPhysicalWidth = physical ? Math.max(1, Math.round(physical.inlineSize)) : declaredPhysicalWidth;
  const observedPhysicalHeight = physical ? Math.max(1, Math.round(physical.blockSize)) : declaredPhysicalHeight;
  // Chromium can deliver a device-pixel-content-box from the previous native
  // scale during a DPR/zoom transition. Never combine that stale physical box
  // with the new window.devicePixelRatio in one viewport epoch: doing so makes
  // the entire Skia scene uniformly too small or too large. A one-pixel delta
  // is retained for independent rounding; anything larger falls back to the
  // declared logical-size/DPR pair until the observer sources converge.
  const physicalBoxIsCoherent =
    Math.abs(observedPhysicalWidth - declaredPhysicalWidth) <= 1 &&
    Math.abs(observedPhysicalHeight - declaredPhysicalHeight) <= 1;
  const physicalWidth = physicalBoxIsCoherent ? observedPhysicalWidth : declaredPhysicalWidth;
  const physicalHeight = physicalBoxIsCoherent ? observedPhysicalHeight : declaredPhysicalHeight;
  if (physical && !physicalBoxIsCoherent) {
    recordResize(host, "device-pixel-box-fallback", "host-observer", {
      detail: JSON.stringify({
        logical: [logicalWidth, logicalHeight],
        devicePixelRatio: ratio,
        observedPhysical: [observedPhysicalWidth, observedPhysicalHeight],
        declaredPhysical: [declaredPhysicalWidth, declaredPhysicalHeight],
      }),
    });
  }
  commitObservedResize(
    host, "host-observer", logicalWidth, logicalHeight,
    physicalWidth, physicalHeight, ratio);
}

function observeFullPageViewport(host: BrowserHost, source: string): void {
  // visualViewport.width/height shrink during pinch zoom even though the
  // full-page layout viewport and fixed Doroti root do not. Treat the visual
  // viewport as a change signal only; publishing its scaled dimensions as
  // layout metrics makes the direct canvas shrink and the browser magnify it
  // again, producing gaps and distorted content. Page zoom and real window
  // resize both change the root's layout box, so it remains the size authority.
  const rect = host.root.getBoundingClientRect();
  const logicalWidth = rect.width > 0 ? rect.width : globalThis.innerWidth;
  const logicalHeight = rect.height > 0 ? rect.height : globalThis.innerHeight;
  const ratio = (globalThis.devicePixelRatio > 0 && Number.isFinite(globalThis.devicePixelRatio) ? globalThis.devicePixelRatio : 1);
  commitObservedResize(
    host, source, logicalWidth, logicalHeight,
    Math.max(1, Math.round(logicalWidth * ratio)),
    Math.max(1, Math.round(logicalHeight * ratio)), ratio);
}

function hostForCanvas(canvas: HTMLCanvasElement): BrowserHost | undefined {
  for (const host of hosts.values()) if (host.canvas === canvas) return host;
  return undefined;
}

function changeDiagnosticContextState(canvasId: string, lose: boolean): boolean {
  const presenter = workerDisplayPresenters.get(canvasId);
  if (!presenter) throw new Error(`Worker presenter '${canvasId}' is not initialized.`);
  presenter.worker.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "context", action: lose ? "lose" : "restore" });
  return true;
}

export function requestPresent(
  canvasId: string, generation: number, logicalWidth: number, logicalHeight: number,
  physicalWidth: number, physicalHeight: number, devicePixelRatio: number,
  timestampMicroseconds: number): void {
  if (!activeWorkerBridge) throw new Error("Doroti presentation requires a render Worker bridge.");
  activeWorkerBridge.requestPresent(canvasId, {
    generation, logicalWidth, logicalHeight, physicalWidth, physicalHeight,
    devicePixelRatio, timestampMicroseconds,
  });
}

function browserOperatingSystem(): string {
  const navigatorWithUaData = navigator as Navigator & { userAgentData?: { platform?: string } };
  const platform = String(navigatorWithUaData.userAgentData?.platform || navigator.platform || navigator.userAgent || "").toLowerCase();
  if (/android/.test(platform)) return "android";
  if (/iphone|ipad|ipod/.test(platform) || (platform.includes("mac") && navigator.maxTouchPoints > 1)) return "iOS";
  if (/win/.test(platform)) return "windows";
  if (/mac/.test(platform)) return "macOS";
  if (/linux|x11|cros/.test(platform)) return "linux";
  return "web";
}

function emit(host: BrowserHost): void {
  // Lifecycle/focus/configuration snapshots carry the current immutable resize
  // epoch, so an older notification can never roll metrics back.
  recordResize(host, "managed-snapshot-dispatched", "browser-state");
  managed?.dispatchSnapshot(host.id, snapshot(host));
}

function emitResize(host: BrowserHost): void {
  // Metrics are browser state, not a presentation acknowledgement. Deliver
  // every observer generation immediately and let the framework's rAF/latest
  // frame mailboxes coalesce raster work. Waiting for an old frame to display
  // makes interactive resize advance at raster/bitmap-transfer cadence.
  recordResize(host, "managed-snapshot-dispatched", "resize-metrics");
  const started = performance.now();
  const epoch = host.resizeEpoch;
  managed?.dispatchResizeEpoch(
    host.id, host.generation, epoch.generation,
    epoch.logicalWidth, epoch.logicalHeight, epoch.physicalWidth, epoch.physicalHeight,
    epoch.devicePixelRatio, epoch.timestampMicroseconds);
  recordResize(host, "managed-snapshot-completed", "resize-metrics", {
    durationMicroseconds: Math.round((performance.now() - started) * 1000),
  });
}

function gpuIdentity(canvas: HTMLCanvasElement): GpuIdentity {
  if (!directWorkerBootstrap && !workerDisplayPresenters.has(canvas.id))
    throw new Error("Doroti GPU identity requires an initialized render Worker.");
  return hostForCanvas(canvas)?.gpu ?? {
    api: "webgl2", vendor: "worker-pending", renderer: "worker-pending",
    hardware: true, softwareFallbackUsed: false,
  };
}

export function configureManagedCallbacks(callbacks: ManagedCallbacks): void {
  const required: (keyof ManagedCallbacks)[] = [
    "dispatchAnimationFrame", "dispatchSnapshot", "dispatchResizeEpoch", "dispatchPointerBatch", "dispatchWheel",
    "dispatchKey", "dispatchFocus", "dispatchTextEditing", "dispatchTextAction",
    "dispatchTextConnectionClosed", "dispatchSemanticsAction",
  ];
  const missing = callbacks
    ? required.filter((name) => typeof callbacks[name] !== "function")
    : required;
  if (missing.length > 0) {
    throw new Error(`Doroti browser managed callback ABI v1 is incomplete: ${missing.join(", ")}.`);
  }
  managed = callbacks;
}

export function getRendererIdentity(): string {
  return activeWorkerBridge?.rendererIdentity() ?? presenterPolicy().selected;
}

export async function initializeManagedCallbacks(): Promise<"ready"> {
  if (managed) return "ready";
  const getDotnetRuntime = (globalThis as typeof globalThis & { getDotnetRuntime?: (index: number) => DotnetRuntime }).getDotnetRuntime;
  const runtime = getDotnetRuntime?.(0);
  if (!runtime) throw new Error("Doroti could not resolve the active Web runtime.");
  const exports = await runtime.getAssemblyExports("Doroti.Host.Web.dll") as DorotiAssemblyExports;
  const interop = exports.Doroti.Host.Web.BrowserInterop;
  initializeBrowserTimers(exports.Doroti.Host.Web.BrowserTimeProvider.DispatchTimer);
  configureManagedCallbacks({
    dispatchAnimationFrame: interop.DispatchAnimationFrame,
    dispatchSnapshot: interop.DispatchSnapshot,
    dispatchResizeEpoch: interop.DispatchResizeEpoch,
    dispatchPointerBatch: interop.DispatchPointerBatch,
    dispatchWheel: interop.DispatchWheel,
    dispatchKey: interop.DispatchKey,
    dispatchFocus: interop.DispatchFocus,
    dispatchTextEditing: interop.DispatchTextEditing,
    dispatchTextAction: interop.DispatchTextAction,
    dispatchTextConnectionClosed: interop.DispatchTextConnectionClosed,
    dispatchSemanticsAction: interop.DispatchSemanticsAction,
  });
  return "ready";
}

export function createHost(hostId: number, canvasId: string, logicalWidth: number, logicalHeight: number): string {
  if (activeWorkerBridge)
    return activeWorkerBridge.createHost(hostId, canvasId, logicalWidth, logicalHeight);
  if (!managed) throw new Error("Doroti browser managed callbacks must be configured before host creation.");
  if (hosts.has(hostId)) throw new Error(`Doroti browser host ${hostId} already exists.`);
  const canvas = document.getElementById(canvasId);
  const root = canvas?.closest(".doroti-root");
  const input = document.getElementById("doroti-ime");
  const semantics = document.getElementById("doroti-semantics");
  if (!(canvas instanceof HTMLCanvasElement)) throw new Error(`Canvas '#${canvasId}' was not found.`);
  if (!(root instanceof HTMLElement)) throw new Error("Doroti root host was not found.");
  if (!(input instanceof HTMLTextAreaElement)) throw new Error("Doroti hidden text input was not found.");
  if (!(semantics instanceof HTMLElement)) throw new Error("Doroti semantics host was not found.");

  const initialRect = root.getBoundingClientRect();
  logicalWidth = initialRect.width > 0 ? initialRect.width : logicalWidth;
  logicalHeight = initialRect.height > 0 ? initialRect.height : logicalHeight;
  const ratio = (globalThis.devicePixelRatio > 0 && Number.isFinite(globalThis.devicePixelRatio) ? globalThis.devicePixelRatio : 1);
  const initialEpoch: ResizeEpoch = {
    generation: 1, logicalWidth, logicalHeight,
    physicalWidth: Math.max(1, Math.round(logicalWidth * ratio)),
    physicalHeight: Math.max(1, Math.round(logicalHeight * ratio)),
    devicePixelRatio: ratio, timestampMicroseconds: Math.round(performance.now() * 1000),
  };
  const host: BrowserHost = {
    id: hostId, root, canvas, input, semantics,
    semanticsElements: new Map(), semanticsListeners: new Map(),
    semanticsContentSignatures: new Map(),
    semanticsActionNodes: new Map(), semanticsObservedEditing: new Map(), semanticsProjectionDepth: 0,
    focusedTextFieldSemanticsId: null,
    logicalWidth, logicalHeight,
    generation: 1, surfaceGeneration: 0, resizeGeneration: 1,
    emittedResizeGeneration: 1, resizeEpoch: initialEpoch,
    resizeTrace: [], resizeTraceSequence: 0, inputSequence: 0,
    diagnosticsPublishTimer: 0, dprQuery: null,
    frameRaf: 0, latestFrameCallback: 0,
    lastWheelDeltaX: 0, lastWheelDeltaY: 0, lastWheelTimestamp: 0,
    lastWheelWasTrackpad: false,
    gpu: gpuIdentity(canvas), observers: [], listeners: [],
    composing: false, compositionStart: -1,
    lastTextValue: "", lastSelectionBase: 0, lastSelectionExtent: 0,
    lastComposingBase: -1, lastComposingExtent: -1,
    viewFocused: false, pressedKeys: new Map(),
    inputAction: 2, multiline: false, interactiveSelectionEnabled: true,
    pendingBlurConnectionCloseTimer: 0,
    editableGeometryApplied: false, contextMenuEnabled: true,
    frameworkCursor: "default", pointerCaptureCursor: null,
  };
  commitDirectCanvasLogicalSize(host, logicalWidth, logicalHeight);
  hosts.set(hostId, host);
  root.dataset.dorotiHostId = String(hostId);
  recordResize(host, "target-observed", "host-initial");
  const observe = (target: EventTarget, name: string, handler: EventListener): void => {
    target.addEventListener(name, handler);
    host.listeners.push({ target, name, handler });
  };
  const belongsToHost = (target: EventTarget | null): boolean =>
    target === root || target === canvas || target === input ||
    (target instanceof Node && (root.contains(target) || semantics.contains(target)));
  observe(document, "visibilitychange", () => emit(host));
  observe(globalThis, "focus", (event) => {
    if (belongsToHost(document.activeElement)) setViewFocus(host, true, event.timeStamp);
    emit(host);
  });
  observe(globalThis, "blur", () => { releasePressedKeys(host); setViewFocus(host, false, performance.now()); emit(host); });
  observe(globalThis, "languagechange", () => emit(host));
  const colorScheme = globalThis.matchMedia?.("(prefers-color-scheme: dark)");
  if (colorScheme) observe(colorScheme, "change", () => emit(host));

  const pointerKind = (type: string): number => type === "touch" ? 1 : type === "pen" ? 2 : 0;
  const pointerSamples = (event: PointerEvent): number[] => {
    const source = event.type === "pointermove" && typeof event.getCoalescedEvents === "function"
      ? event.getCoalescedEvents() : [event];
    const samples: number[] = [];
    const rect = root.getBoundingClientRect();
    for (const point of source.length ? source : [event]) {
      samples.push(point.clientX - rect.left, point.clientY - rect.top,
        point.pressure || (point.buttons ? 0.5 : 0),
        point.tiltX || 0, point.tiltY || 0, point.twist || 0, point.timeStamp);
    }
    return samples;
  };
  const pointer = (phase: number) => (event: PointerEvent): void => {
    event.preventDefault();
    if (phase === 1) {
      const semanticTextField = semanticsTextFieldAtPoint(host, event.clientX, event.clientY);
      if (semanticTextField?.dataset.dorotiSemanticsId) {
        host.focusedTextFieldSemanticsId = Number(semanticTextField.dataset.dorotiSemanticsId);
        placeTextInputAtSemanticsElement(host, semanticTextField);
      }
      const targetCursor = event.target instanceof Element
        ? getComputedStyle(event.target).cursor
        : host.frameworkCursor;
      host.pointerCaptureCursor = targetCursor && targetCursor !== "auto"
        ? targetCursor
        : host.frameworkCursor;
      root.style.cursor = host.pointerCaptureCursor;
      root.setPointerCapture(event.pointerId);
      // A quick window activation or hidden-tab return retains Flutter's text
      // connection, so do not steal DOM focus from its native endpoint. A
      // longer external-window blur closes the connection after the grace
      // period and hides the input, making this select the canvas instead.
      focusActiveEndpoint(host);
    }
    const inputSequence = ++host.inputSequence;
    requireManaged().dispatchPointerBatch(host.id, phase, pointerKind(event.pointerType), event.pointerId,
      event.buttons, modifierMask(event), inputSequence, pointerSamples(event));
    if (phase === 2 || phase === 3) {
      if (root.hasPointerCapture(event.pointerId)) root.releasePointerCapture(event.pointerId);
      host.pointerCaptureCursor = null;
      root.style.cursor = host.frameworkCursor;
    }
  };
  observe(root, "pointerenter", (event) => pointer(5)(event as PointerEvent));
  observe(root, "pointermove", (event) => {
    const pointerEvent = event as PointerEvent;
    pointer(pointerEvent.buttons ? 0 : 4)(pointerEvent);
  });
  observe(root, "pointerdown", (event) => pointer(1)(event as PointerEvent));
  observe(root, "pointerup", (event) => pointer(2)(event as PointerEvent));
  observe(root, "pointercancel", (event) => pointer(3)(event as PointerEvent));
  observe(root, "lostpointercapture", () => {
    host.pointerCaptureCursor = null;
    root.style.cursor = host.frameworkCursor;
  });
  observe(root, "pointerleave", (event) => pointer(6)(event as PointerEvent));
  observe(root, "contextmenu", (event) => {
    if (!host.contextMenuEnabled) event.preventDefault();
  });
  observe(root, "wheel", (event) => {
    const wheel = event as WheelEvent;
    const rect = root.getBoundingClientRect();
    const deltaScale = wheel.deltaMode === WheelEvent.DOM_DELTA_LINE
      ? defaultScrollLineHeight()
      : wheel.deltaMode === WheelEvent.DOM_DELTA_PAGE
        ? Math.max(1, root.clientHeight)
        : 1;
    const kind = isTrackpadWheel(host, wheel) ? 3 : 0;
    const inputSequence = ++host.inputSequence;
    const detail = JSON.stringify({
      deltaMode: wheel.deltaMode,
      rawDeltaX: wheel.deltaX,
      rawDeltaY: wheel.deltaY,
      normalizedDeltaX: wheel.deltaX * deltaScale,
      normalizedDeltaY: wheel.deltaY * deltaScale,
      eventTimestampMilliseconds: wheel.timeStamp,
      kind,
      trusted: wheel.isTrusted,
    });
    recordResize(host, "wheel-ingress", "browser-wheel", { inputSequence, detail });
    // Flutter forwards every wheel sample immediately and coalesces only the
    // resulting frame request. Accumulating deltas here destroys trackpad
    // cadence and delays the scroll-position update until rAF.
    requireManaged().dispatchWheel(
      host.id, wheel.clientX - rect.left, wheel.clientY - rect.top,
      wheel.deltaX * deltaScale, wheel.deltaY * deltaScale,
      wheel.timeStamp, kind, inputSequence);
    recordResize(host, "wheel-framework-dispatch", "managed-callback", { inputSequence, detail });
    wheel.preventDefault();
  });
  observe(document, "keydown", (event) => {
    const key = event as KeyboardEvent;
    if (!host.viewFocused || !belongsToHost(document.activeElement)) return;
    const nativeTextEditing = document.activeElement === input && !input.hidden;
    // The managed KeyData callback has no synchronous handled result, so it
    // cannot decide whether this DOM event should be prevented as Flutter's
    // web engine does. While the native input is active, let it own text,
    // selection, clipboard, and IME keys and report the result through input
    // events. Tab remains a framework focus-traversal key.
    // Keep modifier state current for framework-owned Tab/Shift+Tab even
    // while the DOM input owns editing and clipboard commands.
    const modifierKey = ["Shift", "Control", "Alt", "Meta"].includes(key.key);
    if (nativeTextEditing && key.key !== "Tab" && !modifierKey) return;
    if (key.key === "Tab" ||
        (!nativeTextEditing && ["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", " "].includes(key.key)))
      key.preventDefault();
    host.pressedKeys.set(key.code, key.key);
    requireManaged().dispatchKey(host.id, true, key.repeat, false, key.code, key.key, key.timeStamp,
      ++host.inputSequence);
  });
  observe(document, "keyup", (event) => {
    const key = event as KeyboardEvent;
    if (!host.pressedKeys.has(key.code)) return;
    const pressedKey = host.pressedKeys.get(key.code)!;
    host.pressedKeys.delete(key.code);
    requireManaged().dispatchKey(host.id, false, false, false, key.code, pressedKey, key.timeStamp,
      ++host.inputSequence);
  });
  observe(canvas, "focus", (event) => setViewFocus(host, true, event.timeStamp));
  observe(input, "focus", (event) => setViewFocus(host, true, event.timeStamp));
  observe(input, "blur", (event) => handleTextInputBlur(host, event as FocusEvent, belongsToHost));
  observe(semantics, "focusin", (event) => setViewFocus(host, true, event.timeStamp));
  observe(document, "focusout", (event) => queueMicrotask(() => {
    if (!belongsToHost(document.activeElement)) {
      releasePressedKeys(host);
      setViewFocus(host, false, event.timeStamp);
    }
  }));
  observe(input, "compositionstart", () => { host.composing = true; host.compositionStart = input.selectionStart; emitText(host); });
  observe(input, "compositionupdate", () => emitText(host));
  observe(input, "compositionend", () => { host.composing = false; host.compositionStart = -1; emitText(host); });
  observe(input, "input", () => emitText(host));
  for (const clipboardEvent of ["copy", "cut", "paste"]) {
    observe(input, clipboardEvent, (event) => {
      if (!host.interactiveSelectionEnabled) event.preventDefault();
    });
  }
  observe(document, "selectionchange", () => {
    if (document.activeElement === input && !input.hidden) emitText(host);
  });
  observe(input, "keydown", (event) => {
    const key = event as KeyboardEvent;
    if (key.key === "Enter" && !key.shiftKey && (!host.multiline || host.inputAction !== 12)) {
      key.preventDefault();
      requireManaged().dispatchTextAction(host.id, host.inputAction, ++host.inputSequence);
    }
  });
  if (globalThis.ResizeObserver) {
    const observer = new ResizeObserver((entries) => {
      const entry = entries.find((candidate) => candidate.target === root);
      if (entry) observeResizeEntry(host, entry);
    });
    try {
      observer.observe(root, { box: "device-pixel-content-box" });
    } catch {
      observer.observe(root, { box: "content-box" });
    }
    host.observers.push(observer);
  }
  // Flutter's full-page dimensions provider listens to visualViewport.resize
  // (or window.resize on older browsers). Keep ResizeObserver for the owned
  // root and embedded-host correctness, but do not wait for its post-layout
  // delivery before publishing top-level browser-window metrics.
  const viewportResizeTarget: EventTarget = globalThis.visualViewport ?? globalThis;
  observe(viewportResizeTarget, "resize", () => observeFullPageViewport(
    host, globalThis.visualViewport ? "visual-viewport" : "window-resize"));
  host.environment = new BrowserViewEnvironment(root, input, () => { host.generation++; emit(host); });
  armDprWatcher(host);
  return snapshot(host);
}

export function showHost(hostId: number): string {
  if (activeWorkerBridge) return activeWorkerBridge.showHost(hostId);
  const host = requireHost(hostId);
  host.canvas.hidden = false;
  host.canvas.tabIndex = host.canvas.tabIndex < 0 ? 0 : host.canvas.tabIndex;
  focusActiveEndpoint(host);
  return snapshot(host);
}

export function requestFocus(hostId: number, focused: boolean): string {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("focus-request", { hostId, focused });
    return activeWorkerBridge.showHost(hostId);
  }
  const host = requireHost(hostId);
  if (focused) focusActiveEndpoint(host);
  else if (document.activeElement === host.input) host.input.blur();
  else host.canvas.blur();
  return snapshot(host);
}

export function resizeHost(hostId: number, logicalWidth: number, logicalHeight: number): string {
  if (activeWorkerBridge) return activeWorkerBridge.resizeHost(hostId, logicalWidth, logicalHeight);
  const host = requireHost(hostId);
  host.root.style.width = `${logicalWidth}px`;
  host.root.style.height = `${logicalHeight}px`;
  return snapshot(host);
}

export function requestFrame(hostId: number, callbackId: number): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.requestFrame(hostId, callbackId);
    return;
  }
  const host = requireHost(hostId);
  host.latestFrameCallback = callbackId;
  recordResize(host, "framework-frame-requested", "managed-scheduler", {
    rafId: callbackId,
    detail: JSON.stringify({ callbackId }),
  });
  scheduleHostFrame(host);
}

function scheduleHostFrame(host: BrowserHost): void {
  if (host.frameRaf !== 0) return;
  host.frameRaf = requestAnimationFrame((timestamp) => {
    // -1 means that callbacks scheduled synchronously while sampling input or
    // metrics belong to this browser frame. They must not create a redundant
    // rAF and then wait an extra refresh before the framework sees them.
    host.frameRaf = -1;
    try {
      if (!hosts.has(host.id)) return;

      const latest = host.latestFrameCallback;
      host.latestFrameCallback = 0;
      if (latest !== 0) {
        recordResize(host, "framework-frame-dispatched", "browser-raf", {
          rafId: latest,
          detail: JSON.stringify({ callbackId: latest, timestampMilliseconds: timestamp }),
        });
        managed?.dispatchAnimationFrame(host.id, latest, timestamp);
      }
    } finally {
      host.frameRaf = 0;
      if (hosts.has(host.id) && host.latestFrameCallback !== 0) {
        scheduleHostFrame(host);
      }
    }
  });
  recordResize(host, "browser-raf-scheduled", "browser-raf", { rafId: host.frameRaf });
}

export function recordManagedRaster(
  hostId: number,
  phase: string,
  surfaceWidth: number,
  surfaceHeight: number,
  durationMicroseconds: number): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.recordManagedRaster(
      hostId, phase, surfaceWidth, surfaceHeight, durationMicroseconds);
    return;
  }
  const host = requireHost(hostId);
  recordResize(host, phase, "managed-skia", {
    durationMicroseconds,
    backingWidth: host.canvas.width,
    backingHeight: host.canvas.height,
    surfaceWidth,
    surfaceHeight,
  });
}

export function captureResizeTrace(hostId: number): string {
  if (activeWorkerBridge) return activeWorkerBridge.captureResizeTrace(hostId);
  return JSON.stringify(requireHost(hostId).resizeTrace);
}

export function closeHost(hostId: number): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.closeHost(hostId);
    return;
  }
  const host = hosts.get(hostId);
  if (!host) return;
  releasePressedKeys(host);
  if (host.pendingBlurConnectionCloseTimer !== 0)
    clearTimeout(host.pendingBlurConnectionCloseTimer);
  if (host.diagnosticsPublishTimer !== 0)
    clearTimeout(host.diagnosticsPublishTimer);
  if (host.frameRaf > 0) cancelAnimationFrame(host.frameRaf);
  host.environment?.dispose();
  for (const observer of host.observers) observer.disconnect();
  for (const listener of host.listeners) listener.target.removeEventListener(listener.name, listener.handler);
  for (const controller of host.semanticsListeners.values()) controller.abort();
  host.semanticsListeners.clear();
  host.semanticsElements.clear();
  host.semanticsActionNodes.clear();
  host.semanticsObservedEditing.clear();
  host.semanticsContentSignatures.clear();
  host.semantics.replaceChildren();
  delete host.root.dataset.dorotiHostId;
  hosts.delete(hostId);
}

export function resolveResourceUrl(relativeUrl: string): string {
  if (activeWorkerBridge) return activeWorkerBridge.resolveResourceUrl(relativeUrl);
  return new URL(relativeUrl, document.baseURI).href;
}

export function setCursor(hostId: number, cursor: string): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("cursor", { hostId, cursor });
    return;
  }
  const host = requireHost(hostId);
  host.frameworkCursor = cursor;
  host.canvas.style.cursor = cursor;
  if (host.pointerCaptureCursor === null) host.root.style.cursor = cursor;
}

function applyTextInputConfiguration(
  host: BrowserHost, inputMode: string, enterKeyHint: string, readOnly: boolean,
  obscureText: boolean, autocapitalize: string, autocorrect: boolean,
  inputAction: number, multiline: boolean, enableInteractiveSelection: boolean): void {
  host.input.inputMode = inputMode as typeof host.input.inputMode;
  host.input.enterKeyHint = enterKeyHint;
  host.input.readOnly = readOnly;
  host.input.autocapitalize = autocapitalize;
  host.input.autocomplete = "off";
  host.input.setAttribute("autocorrect", autocorrect ? "on" : "off");
  host.input.spellcheck = autocorrect;
  host.inputAction = inputAction;
  host.multiline = multiline;
  host.interactiveSelectionEnabled = enableInteractiveSelection;
  host.input.style.setProperty("-webkit-text-security", obscureText ? "disc" : "none");
}

export function updateTextInputConfiguration(
  hostId: number, inputMode: string, enterKeyHint: string, readOnly: boolean,
  obscureText: boolean, autocapitalize: string, autocorrect: boolean,
  inputAction: number, multiline: boolean, enableInteractiveSelection: boolean): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("text-config", {
      hostId, inputMode, enterKeyHint, readOnly, obscureText, autocapitalize,
      autocorrect, inputAction, multiline, enableInteractiveSelection,
    });
    return;
  }
  applyTextInputConfiguration(
    requireHost(hostId), inputMode, enterKeyHint, readOnly, obscureText,
    autocapitalize, autocorrect, inputAction, multiline, enableInteractiveSelection);
}

interface TextInputStylePayload {
  fontFamily?: string | null;
  fontSize?: number | null;
  fontWeight?: number | null;
  textDirection: "ltr" | "rtl";
  textAlign: "left" | "right" | "center" | "justify" | "start" | "end";
  letterSpacing?: number | null;
  wordSpacing?: number | null;
  lineHeight?: number | null;
}

export function setTextInputStyle(hostId: number, styleJson: string): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("text-style", { hostId, styleJson });
    return;
  }
  const host = requireHost(hostId);
  const style = JSON.parse(styleJson) as TextInputStylePayload;
  const finite = (value: number | null | undefined): value is number =>
    typeof value === "number" && Number.isFinite(value);
  host.input.style.fontFamily = style.fontFamily ?? "";
  host.input.style.fontSize = finite(style.fontSize) ? `${style.fontSize}px` : "";
  host.input.style.fontWeight = finite(style.fontWeight) ? String(style.fontWeight) : "";
  host.input.dir = style.textDirection;
  host.input.style.textAlign = style.textAlign;
  host.input.style.letterSpacing = finite(style.letterSpacing) ? `${style.letterSpacing}px` : "";
  host.input.style.wordSpacing = finite(style.wordSpacing) ? `${style.wordSpacing}px` : "";
  host.input.style.lineHeight = finite(style.lineHeight) ? `${style.lineHeight}px` : "normal";
}

export function setTextInputState(
  hostId: number, text: string, selectionBase: number, selectionExtent: number,
  inputMode: string, enterKeyHint: string, readOnly: boolean, obscureText: boolean,
  autocapitalize: string, autocorrect: boolean, inputAction: number, multiline: boolean,
  attach: boolean, enableInteractiveSelection: boolean): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("text-state", {
      hostId, text, selectionBase, selectionExtent, inputMode, enterKeyHint,
      readOnly, obscureText, autocapitalize, autocorrect, inputAction, multiline, attach,
      enableInteractiveSelection,
    });
    return;
  }
  const host = requireHost(hostId);
  if (attach && host.pendingBlurConnectionCloseTimer !== 0) {
    clearTimeout(host.pendingBlurConnectionCloseTimer);
    host.pendingBlurConnectionCloseTimer = 0;
  }
  applyTextInputConfiguration(
    host, inputMode, enterKeyHint, readOnly, obscureText, autocapitalize,
    autocorrect, inputAction, multiline, enableInteractiveSelection);
  const normalizedBase = Math.max(0, Math.min(text.length, selectionBase));
  const normalizedExtent = Math.max(0, Math.min(text.length, selectionExtent));
  const selectionStart = Math.min(normalizedBase, normalizedExtent);
  const selectionEnd = Math.max(normalizedBase, normalizedExtent);
  const selectionDirection: "forward" | "backward" =
    normalizedBase > normalizedExtent ? "backward" : "forward";
  const sameText = host.input.value === text;
  const sameSelection = host.input.selectionStart === selectionStart &&
    host.input.selectionEnd === selectionEnd &&
    (selectionStart === selectionEnd || host.input.selectionDirection === selectionDirection);

  host.input.hidden = false;
  const focusedTextField = host.focusedTextFieldSemanticsId === null
    ? null
    : host.semanticsElements.get(host.focusedTextFieldSemanticsId) ?? null;
  if (focusedTextField) placeTextInputAtSemanticsElement(host, focusedTextField);
  if (document.activeElement !== host.input) host.input.focus({ preventScroll: true });

  // While an IME composition owns the textarea, managed state is an
  // acknowledgement of an earlier native edit and may already be stale. Any
  // value or selection write here can cancel the browser composition and leave
  // the field apparently focused but unable to accept more text. The native
  // endpoint remains authoritative until compositionend publishes the final
  // state back to managed code.
  if (host.composing) return;

  if (!sameText) host.input.value = text;
  if (!sameText || !sameSelection)
    host.input.setSelectionRange(selectionStart, selectionEnd, selectionDirection);
  rememberTextState(host, text, normalizedBase, normalizedExtent, -1, -1);
}

export function setEditableSizeAndTransform(
  hostId: number, width: number, height: number, transformJson: string): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("editable-geometry", { hostId, width, height, transformJson });
    return;
  }
  const host = requireHost(hostId);
  const transform = JSON.parse(transformJson) as number[];
  if (transform.length !== 16 || transform.some((value) => !Number.isFinite(value)))
    throw new Error("Doroti editable transform must contain sixteen finite values.");

  host.editableGeometryApplied = true;
  host.input.style.left = "0";
  host.input.style.top = "0";
  host.input.style.width = `${Math.max(1, width)}px`;
  host.input.style.height = `${Math.max(1, height)}px`;
  host.input.style.transform = `matrix3d(${transform.join(",")})`;
}

export function setCaretRect(hostId: number, left: number, top: number, width: number, height: number): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("caret", { hostId, left, top, width, height });
    return;
  }
  const host = requireHost(hostId);
  if (!host.editableGeometryApplied) {
    host.input.style.left = `${left}px`;
    host.input.style.top = `${top}px`;
    host.input.style.width = `${Math.max(1, width)}px`;
    host.input.style.height = `${Math.max(1, height)}px`;
  }
  host.input.focus({ preventScroll: true });
}

export function setContextMenuEnabled(hostId: number, enabled: boolean): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("context-menu", { hostId, enabled });
    return;
  }
  requireHost(hostId).contextMenuEnabled = enabled;
}

export function clearTextInput(hostId: number): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("text-clear", { hostId });
    return;
  }
  const host = requireHost(hostId);
  if (host.pendingBlurConnectionCloseTimer !== 0) {
    clearTimeout(host.pendingBlurConnectionCloseTimer);
    host.pendingBlurConnectionCloseTimer = 0;
  }
  host.composing = false;
  host.compositionStart = -1;
  host.input.value = "";
  host.input.hidden = true;
  rememberTextState(host, "", 0, 0, -1, -1);
  host.canvas.focus({ preventScroll: true });
}

export async function launchExternalUrl(url: string): Promise<string> {
  if (activeWorkerBridge) return activeWorkerBridge.requestControl("url-launch", { url });
  const parsed = new URL(url);
  if (parsed.protocol !== "https:" && parsed.protocol !== "http:") throw new Error("Unsupported URL scheme");
  // A Worker round trip can lose transient activation. Report the browser result explicitly.
  const opened = window.open(parsed.href, "_blank");
  if (!opened) return "blocked";
  opened.opener = null;
  return "opened";
}

export async function vibrate(durationMilliseconds: number): Promise<void> {
  if (activeWorkerBridge) {
    await activeWorkerBridge.requestControl("haptic-feedback", { durationMilliseconds });
    return;
  }
  // Like Flutter web, unsupported hardware/API or denied activation is a no-op.
  navigator.vibrate?.(durationMilliseconds);
}

export async function readClipboardText(): Promise<string> {
  if (activeWorkerBridge) return activeWorkerBridge.requestControl("clipboard-read", {});
  if (!navigator.clipboard?.readText) throw new Error("Doroti clipboard read capability is unavailable.");
  return navigator.clipboard.readText();
}

export async function writeClipboardText(text: string): Promise<"written"> {
  if (activeWorkerBridge) {
    await activeWorkerBridge.requestControl("clipboard-write", { text });
    return "written";
  }
  if (!navigator.clipboard?.writeText) throw new Error("Doroti clipboard write capability is unavailable.");
  await navigator.clipboard.writeText(text);
  return "written";
}

export function updateSemantics(hostId: number, json: string): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("semantics", { hostId, json });
    return;
  }
  const host = requireHost(hostId);
  const started = performance.now();
  const update = JSON.parse(json) as SemanticsUpdate;
  host.semanticsProjectionDepth++;
  try { applySemanticsProjection(host, update, started); }
  finally { host.semanticsProjectionDepth--; }
}

function applySemanticsProjection(host: BrowserHost, update: SemanticsUpdate, started: number): void {
  host.semantics.dataset.generation = String(update.generation);
  const nodes = update.nodes ?? [];
  const nodesById = new Map(nodes.map((node) => [Number(node.id), node]));
  const identifiersByValue = new Map(nodes
    .map((node) => [node.identifier ?? host.semanticsElements.get(Number(node.id))
      ?.dataset.dorotiSemanticsIdentifier, Number(node.id)] as const)
    .filter((entry): entry is readonly [string, number] => Boolean(entry[0])));
  const parentById = new Map<number, number>();
  for (const node of nodes) {
    for (const childId of node.children ?? []) {
      if (nodesById.has(childId) && !parentById.has(childId)) parentById.set(childId, Number(node.id));
    }
  }

  const liveIds = new Set<number>();
  let contentUpdates = 0;
  let geometryUpdates = 0;
  for (const node of nodes) {
    const id = Number(node.id);
    liveIds.add(id);
    const previousActionNode = host.semanticsActionNodes.get(id);
    host.semanticsActionNodes.set(id, node.contentUnchanged === true && previousActionNode
      ? { ...previousActionNode, rect: node.rect, children: node.children } : node);
    let element = host.semanticsElements.get(id);
    // A contentUnchanged node intentionally omits flags/role/actions. Preserve
    // the existing native element kind instead of deriving a div from absent
    // content and replacing inputs, buttons, or links during geometry updates.
    const tag = node.contentUnchanged === true && element
      ? element.tagName.toLowerCase()
      : semanticsElementTag(node);
    let replaced = false;
    if (!element || element.tagName.toLowerCase() !== tag) {
      const replacement = document.createElement(tag);
      if (element?.parentElement) element.replaceWith(replacement);
      element = replacement;
      host.semanticsElements.set(id, element);
      replaced = true;
    }
    const canRetainContent = node.contentUnchanged === true && !replaced &&
      host.semanticsContentSignatures.has(id);
    if (!canRetainContent && node.flags?.textField) {
      if (node.flags.focused === true) host.focusedTextFieldSemanticsId = id;
      else if (node.flags.focused === false && host.focusedTextFieldSemanticsId === id)
        host.focusedTextFieldSemanticsId = null;
    }
    const controlledIds = canRetainContent ? undefined : node.controlsNodes
      ?.map((identifier) => identifiersByValue.get(identifier))
      .filter((controlledId): controlledId is number => controlledId !== undefined)
      .map((controlledId) => semanticsDomId(host.id, controlledId));
    // Geometry, scroll extents, and child ordering can change on every layout
    // without changing the element's ARIA contract or action closures. Keep
    // the signature limited to values actually consumed below so interactive
    // resize does not tear down every listener and attribute on every frame.
    const contentSignature = JSON.stringify({
      tag,
      role: node.role,
      label: node.label,
      value: node.value,
      actions: node.actions,
      flags: node.flags,
      textSelectionBase: node.textSelectionBase,
      textSelectionExtent: node.textSelectionExtent,
      identifier: node.identifier,
      hint: node.hint,
      tooltip: node.tooltip,
      headingLevel: node.headingLevel,
      linkUrl: node.linkUrl,
      validationResult: node.validationResult,
      inputType: node.inputType,
      minValue: node.minValue,
      maxValue: node.maxValue,
      maxValueLength: node.maxValueLength,
      controlledIds,
      locale: node.locale,
    });
    if (!canRetainContent &&
        (replaced || host.semanticsContentSignatures.get(id) !== contentSignature)) {
      contentUpdates++;
      host.semanticsListeners.get(id)?.abort();
      const listeners = new AbortController();
      host.semanticsListeners.set(id, listeners);
      resetSemanticsAttributes(element);
      host.semanticsObservedEditing.delete(id);
      element.dataset.dorotiSemanticsId = String(node.id);
      element.id = semanticsDomId(host.id, id);
      if (node.identifier) element.dataset.dorotiSemanticsIdentifier = node.identifier;
      const role = semanticsRole(node);
      element.setAttribute("role", role);
      const valueRole = role === "slider" || role === "progressbar" || role === "spinbutton";
      const accessibleLabel = valueRole || node.flags?.textField
        ? node.label
        : [node.label, node.value].filter((value) => Boolean(value)).join(" ");
      setOptionalAttribute(element, "aria-label", accessibleLabel);
      setOptionalAttribute(element, "aria-description", [node.hint, node.tooltip].filter((value) => Boolean(value)).join(" "));
      setOptionalAttribute(element, "aria-controls", controlledIds?.join(" "));
      setOptionalAttribute(element, "lang", node.locale);
      if (node.validationResult === "invalid") element.setAttribute("aria-invalid", "true");
      if (node.headingLevel && node.headingLevel > 0) element.setAttribute("aria-level", String(node.headingLevel));
      if (valueRole) {
        setOptionalAttribute(element, "aria-valuetext", node.value);
        setOptionalAttribute(element, "aria-valuemin", node.minValue);
        setOptionalAttribute(element, "aria-valuemax", node.maxValue);
        const numericValue = Number(node.value);
        if (Number.isFinite(numericValue)) element.setAttribute("aria-valuenow", String(numericValue));
      }
      applySemanticsFlags(element, node.flags, role);
      const actions = node.actions ?? 0;
      const enabled = node.flags?.enabled !== false;
      if (node.flags?.focusable && !isNativeFocusableElement(element)) {
        element.tabIndex = 0;
      } else if (!isNativeFocusableElement(element)) {
        element.removeAttribute("tabindex");
      }
      if (enabled && (actions & 1) !== 0) {
        element.addEventListener("click", (event) => {
          event.stopPropagation();
          dispatchSemantics(host, node.id, 1);
        }, { signal: listeners.signal });
      }
      element.addEventListener("keydown", (event) => {
        const key = event as KeyboardEvent;
        let action = 0;
        if (enabled && (actions & 1) !== 0 && (key.key === "Enter" || key.key === " ")) action = 1;
        else if ((actions & (1 << 6)) !== 0 && (key.key === "ArrowUp" || key.key === "ArrowRight")) action = 1 << 6;
        else if ((actions & (1 << 7)) !== 0 && (key.key === "ArrowDown" || key.key === "ArrowLeft")) action = 1 << 7;
        else if ((actions & (1 << 2)) !== 0 && key.key === "ArrowLeft") action = 1 << 2;
        else if ((actions & (1 << 3)) !== 0 && key.key === "ArrowRight") action = 1 << 3;
        else if ((actions & (1 << 4)) !== 0 && (key.key === "ArrowUp" || key.key === "PageUp")) action = 1 << 4;
        else if ((actions & (1 << 5)) !== 0 && (key.key === "ArrowDown" || key.key === "PageDown")) action = 1 << 5;
        else if ((actions & (1 << 18)) !== 0 && key.key === "Escape") action = 1 << 18;
        if (action === 0) return;
        key.preventDefault();
        key.stopPropagation();
        dispatchSemantics(host, node.id, action);
      }, { signal: listeners.signal });
      element.addEventListener("focus", () => {
        if ((actions & (1 << 22)) !== 0) dispatchSemantics(host, node.id, 1 << 22);
      }, { signal: listeners.signal });

      if (node.flags?.textField && (element instanceof HTMLInputElement || element instanceof HTMLTextAreaElement)) {
        element.disabled = !enabled;
        element.readOnly = node.flags.readOnly === true;
        if (element instanceof HTMLInputElement) element.type = node.flags.obscured ? "password" : "text";
        element.inputMode = semanticsInputMode(node.inputType);
        if (node.maxValueLength !== undefined && node.maxValueLength >= 0) element.maxLength = node.maxValueLength;
        if (element.value !== (node.value ?? "")) element.value = node.value ?? "";
        if (node.textSelectionBase !== undefined && node.textSelectionBase >= 0 &&
            node.textSelectionExtent !== undefined && node.textSelectionExtent >= 0) {
          const base = Math.min(element.value.length, node.textSelectionBase);
          const extent = Math.min(element.value.length, node.textSelectionExtent);
          const actual = textSelectionOffsets(element);
          if (actual?.base !== base || actual.extent !== extent)
            element.setSelectionRange(Math.min(base, extent), Math.max(base, extent), base > extent ? "backward" : "forward");
        }
        const projectedSelection = textSelectionOffsets(element);
        host.semanticsObservedEditing.set(id, {
          value: element.value, base: projectedSelection?.base ?? 0, extent: projectedSelection?.extent ?? 0,
        });
        element.addEventListener("input", () => dispatchSemantics(host, node.id, 1 << 21, element.value), { signal: listeners.signal });
        const selection = () => {
          if ((actions & (1 << 11)) === 0) return;
          const offsets = textSelectionOffsets(element);
          if (offsets) dispatchSemantics(host, node.id, 1 << 11, offsets);
        };
        element.addEventListener("select", selection, { signal: listeners.signal });
        element.addEventListener("keyup", selection, { signal: listeners.signal });
        element.addEventListener("mouseup", selection, { signal: listeners.signal });
        if ((actions & (1 << 12)) !== 0) element.addEventListener("copy", () => dispatchSemantics(host, node.id, 1 << 12), { signal: listeners.signal });
        if ((actions & (1 << 13)) !== 0) element.addEventListener("cut", () => dispatchSemantics(host, node.id, 1 << 13), { signal: listeners.signal });
        if ((actions & (1 << 14)) !== 0) element.addEventListener("paste", () => dispatchSemantics(host, node.id, 1 << 14), { signal: listeners.signal });
      }
      host.semanticsContentSignatures.set(id, contentSignature);
    }

    const parent = nodesById.get(parentById.get(id) ?? Number.NaN);
    const parentLeft = parent?.rect[0] ?? 0;
    const parentTop = parent?.rect[1] ?? 0;
    element.style.position = "absolute";
    const left = `${node.rect[0] - parentLeft}px`;
    const top = `${node.rect[1] - parentTop}px`;
    const width = `${Math.max(0, node.rect[2] - node.rect[0])}px`;
    const height = `${Math.max(0, node.rect[3] - node.rect[1])}px`;
    if (element.style.left !== left) { element.style.left = left; geometryUpdates++; }
    if (element.style.top !== top) { element.style.top = top; geometryUpdates++; }
    if (element.style.width !== width) { element.style.width = width; geometryUpdates++; }
    if (element.style.height !== height) { element.style.height = height; geometryUpdates++; }
  }

  for (const [id, controller] of host.semanticsListeners) {
    if (liveIds.has(id)) continue;
    controller.abort();
    host.semanticsListeners.delete(id);
    host.semanticsContentSignatures.delete(id);
    host.semanticsActionNodes.delete(id);
    host.semanticsObservedEditing.delete(id);
    host.semanticsElements.get(id)?.remove();
    host.semanticsElements.delete(id);
  }

  const desiredByParent = new Map<HTMLElement, HTMLElement[]>();
  for (const node of nodes) {
    const parent = host.semanticsElements.get(Number(node.id));
    if (!parent) continue;
    desiredByParent.set(parent, (node.children ?? [])
      .map((childId) => host.semanticsElements.get(childId))
      .filter((child): child is HTMLElement => child !== undefined));
  }
  const roots = nodes
    .filter((node) => !parentById.has(Number(node.id)))
    .map((node) => host.semanticsElements.get(Number(node.id)))
    .filter((element): element is HTMLElement => element !== undefined);
  desiredByParent.set(host.semantics, roots);
  for (const [parent, desired] of desiredByParent) placeSemanticsChildren(parent, desired);
  for (const [parent, desired] of desiredByParent) removeUnexpectedSemanticsChildren(parent, desired);
  if (host.focusedTextFieldSemanticsId !== null &&
      !liveIds.has(host.focusedTextFieldSemanticsId)) {
    host.focusedTextFieldSemanticsId = null;
  }
  const focusedTextField = host.focusedTextFieldSemanticsId === null
    ? null
    : host.semanticsElements.get(host.focusedTextFieldSemanticsId) ?? null;
  if (!host.input.hidden && focusedTextField) placeTextInputAtSemanticsElement(host, focusedTextField);
  recordResize(host, "semantics-dom-applied", "browser-semantics", {
    durationMicroseconds: Math.round((performance.now() - started) * 1000),
    detail: JSON.stringify({ nodes: nodes.length, contentUpdates, geometryUpdates }),
  });
}

export function setApplicationTitle(hostId: number, title: string): void {
  if (activeWorkerBridge) {
    activeWorkerBridge.postControl("application-title", { hostId, title });
    return;
  }
  requireHost(hostId).semantics.setAttribute("aria-label", title);
}

export async function invokePlugin(moduleUrl: string, exportName: string, channel: string, codec: string, payloadBase64: string): Promise<string> {
  if (activeWorkerBridge)
    return activeWorkerBridge.requestControl("plugin", { moduleUrl, exportName, channel, codec, payloadBase64 });
  const resolved = new URL(moduleUrl, document.baseURI).href;
  const pluginModule = await import(resolved) as Record<string, unknown>;
  const handler = pluginModule[exportName];
  if (typeof handler !== "function") throw new Error(`Doroti JavaScript plugin export '${exportName}' is missing from '${resolved}'.`);
  const request: PluginRequest = { channel, codec, payloadBase64 };
  const value: unknown = await handler(request);
  if (value === null || value === undefined) return JSON.stringify({ hasValue: false, base64: "" });
  if (typeof value === "string") return JSON.stringify({ hasValue: true, base64: value });
  if (value instanceof Uint8Array) {
    let binary = "";
    for (const byte of value) binary += String.fromCharCode(byte);
    return JSON.stringify({ hasValue: true, base64: btoa(binary) });
  }
  throw new Error(`Doroti JavaScript plugin '${exportName}' returned an unsupported response type.`);
}

export async function startDorotiWorkerHost(
  mode: "worker-direct-webgl" | "worker-direct-webgpu" = "worker-direct-webgpu",
  runtimeLocation: "main" | "worker" = "main",
): Promise<"started"> {
  if (mode === "worker-direct-webgpu" && runtimeLocation !== "main")
    throw new Error("Doroti WebGPU requires runtimeLocation=main and a threaded build.");
  if (typeof Worker === "undefined" || typeof OffscreenCanvas === "undefined" ||
      typeof HTMLCanvasElement.prototype.transferControlToOffscreen !== "function")
    throw new Error(`Doroti ${mode} required browser capabilities are unavailable.`);
  const app = document.getElementById("app");
  if (!app) throw new Error("Doroti worker bootstrap could not find '#app'.");
  const endpoints = createDorotiDomEndpoints(app);
  const root = endpoints.root;
  let canvas = endpoints.canvas;
  const dotnetModuleUrl = resolveCurrentDotnetModuleUrl();

  let activeWorker: DorotiWorkerEndpoint;
  const placeholder = runtimeLocation === "main"
    ? await createManagedDorotiWorker(dotnetModuleUrl, new URL("./doroti.raster.worker.js", import.meta.url))
    : createDorotiWorker(new URL("./doroti.raster.worker.js", import.meta.url));
  activeWorker = placeholder;
  const display: WorkerDisplayPresenter = {
    worker: activeWorker, runtimeLocation, mode,
    currentRequestId: null, latestRequestId: null,
    contextGeneration: 0, contextLost: false, frontGeneration: 0, frontRequestId: 0,
    rasterWidth: 0, rasterHeight: 0, displayWidth: 0, displayHeight: 0,
    bitmapCreated: 0, bitmapConsumed: 0, bitmapClosed: 0, activeBitmaps: 0,
    restartCount: 0, runtimeSessionId: 1, pendingLeases: new Map(),
  };
  workerDisplayPresenters.set(canvas.id, display);
  let snapshotInFlight = false;
  let snapshotInFlightGeneration = 0;
  let latestWorkerSnapshot: { hostId: number; value: Record<string, unknown> } | null = null;
  const admissionInFlightGenerations = new Set<number>();
  let latestDirectAdmission: {
    hostId: number;
    hostGeneration: number;
    epoch: ResizeEpoch;
  } | null = null;
  const sendLatestWorkerSnapshot = (): void => {
    if (snapshotInFlight || !latestWorkerSnapshot) return;
    const next = latestWorkerSnapshot;
    latestWorkerSnapshot = null;
    snapshotInFlight = true;
    snapshotInFlightGeneration = Number(
      (next.value.resizeEpoch as Record<string, unknown>)?.generation);
    const targetHost = hosts.get(next.hostId);
    if (targetHost) recordResize(targetHost, "worker-snapshot-sent", "worker-mailbox", {
      detail: JSON.stringify({ generation: (next.value.resizeEpoch as Record<string, unknown>)?.generation }),
    });
    activeWorker.postMessage({
      protocolVersion: dorotiProtocolVersion, kind: "snapshot", hostId: next.hostId, snapshot: next.value,
    });
  };
  const queueWorkerSnapshot = (hostId: number, snapshotJson: string): void => {
    const value = JSON.parse(snapshotJson) as Record<string, unknown>;
    latestWorkerSnapshot = {
      hostId,
      value,
    };
    const targetHost = hosts.get(hostId);
    if (targetHost) recordResize(targetHost, "worker-snapshot-queued", "worker-mailbox");
    sendLatestWorkerSnapshot();
  };
  const sendLatestDirectAdmission = (): void => {
    // The browser/Worker message queue itself is not observable. A small fixed
    // transport window covers the 2-4 ResizeObserver generations that can
    // arrive while one complex frame is rasterizing; everything beyond it is
    // still replaced in the local latest slot. Managed scheduling remains
    // latest-only, so these cheap typed metrics never force stale raster work.
    if (admissionInFlightGenerations.size >= 4 || !latestDirectAdmission) return;
    const next = latestDirectAdmission;
    latestDirectAdmission = null;
    admissionInFlightGenerations.add(next.epoch.generation);
    activeWorker.postMessage({
      protocolVersion: dorotiProtocolVersion,
      kind: "admission-target",
      generation: next.epoch.generation,
      resizeEpoch: next.epoch,
      hostGeneration: next.hostGeneration,
    });
  };
  const queueWorkerResizeEpoch: ManagedCallbacks["dispatchResizeEpoch"] = (
    hostId, hostGeneration, generation,
    logicalWidth, logicalHeight, physicalWidth, physicalHeight,
    devicePixelRatio, timestampMicroseconds): void => {
    latestDirectAdmission = {
      hostId,
      hostGeneration,
      epoch: {
        generation, logicalWidth, logicalHeight, physicalWidth, physicalHeight,
        devicePixelRatio, timestampMicroseconds,
      },
    };
    sendLatestDirectAdmission();
  };
  const postInput = (
    inputKind: string, hostId: number, inputSequence: number, payload: Record<string, unknown>): void =>
    activeWorker.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "input", inputKind, hostId, inputSequence, payload });
  configureManagedCallbacks({
    dispatchAnimationFrame: () => { throw new Error("main worker host cannot receive managed frame callbacks"); },
    dispatchSnapshot: queueWorkerSnapshot,
    dispatchResizeEpoch: queueWorkerResizeEpoch,
    dispatchPointerBatch: (hostId, phase, kind, pointerId, buttons, modifiers, inputSequence, samples) =>
      postInput("pointer", hostId, inputSequence, { phase, kind, pointerId, buttons, modifiers, samples }),
    dispatchWheel: (hostId, x, y, deltaX, deltaY, timestamp, kind, inputSequence) =>
      postInput("wheel", hostId, inputSequence, { x, y, deltaX, deltaY, timestamp, kind }),
    dispatchKey: (hostId, pressed, repeat, synthesized, code, key, timestamp, inputSequence) =>
      postInput("key", hostId, inputSequence, { pressed, repeat, synthesized, code, key, timestamp }),
    dispatchFocus: (hostId, focused, timestamp, inputSequence) =>
      postInput("focus", hostId, inputSequence, { focused, timestamp }),
    dispatchTextEditing: (hostId, text, selectionBase, selectionExtent, composingBase, composingExtent, inputSequence) =>
      postInput("text", hostId, inputSequence, { text, selectionBase, selectionExtent, composingBase, composingExtent }),
    dispatchTextAction: (hostId, action, inputSequence) => postInput("text-action", hostId, inputSequence, { action }),
    dispatchTextConnectionClosed: (hostId, inputSequence) => postInput("text-closed", hostId, inputSequence, {}),
    dispatchSemanticsAction: (hostId, nodeId, action, inputSequence, argumentsJson) =>
      postInput("semantics-action", hostId, inputSequence, { nodeId, action, argumentsJson }),
  });
  const initialRect = root.getBoundingClientRect();
  directWorkerBootstrap = true;
  createHost(1, canvas.id, Math.max(1, initialRect.width), Math.max(1, initialRect.height));
  directWorkerBootstrap = false;
  let host = requireHost(1);
  {
    configureDirectCanvasCapacity(
      host, initialRect.width, initialRect.height, host.resizeEpoch.devicePixelRatio,
      undefined, undefined, true);
  }
  document.documentElement.dataset.dorotiRenderer = mode;

  let ready = false;
  let terminalFailure: Error | null = null;
  let resolveReady!: (value: "started") => void;
  let rejectReady!: (reason: unknown) => void;
  const readyPromise = new Promise<"started">((resolve, reject) => {
    resolveReady = resolve;
    rejectReady = reject;
  });

  const sendControlResponse = (correlationId: number, result: string, error?: unknown): void =>
    activeWorker.postMessage({
      protocolVersion: dorotiProtocolVersion, kind: "control-response", correlationId, result,
      error: error ? String(error) : undefined,
    });

  const handleControl = async (message: Record<string, unknown>): Promise<void> => {
    const kind = String(message.controlKind);
    const payload = (message.payload ?? {}) as Record<string, unknown>;
    switch (kind) {
      case "cursor": setCursor(Number(payload.hostId), String(payload.cursor)); break;
      case "focus-request": requestFocus(Number(payload.hostId), Boolean(payload.focused)); break;
      case "text-state":
        setTextInputState(
          Number(payload.hostId), String(payload.text), Number(payload.selectionBase), Number(payload.selectionExtent),
          String(payload.inputMode), String(payload.enterKeyHint), Boolean(payload.readOnly), Boolean(payload.obscureText),
          String(payload.autocapitalize), Boolean(payload.autocorrect), Number(payload.inputAction),
          Boolean(payload.multiline), Boolean(payload.attach), Boolean(payload.enableInteractiveSelection));
        break;
      case "text-config":
        updateTextInputConfiguration(
          Number(payload.hostId), String(payload.inputMode), String(payload.enterKeyHint),
          Boolean(payload.readOnly), Boolean(payload.obscureText), String(payload.autocapitalize),
          Boolean(payload.autocorrect), Number(payload.inputAction), Boolean(payload.multiline),
          Boolean(payload.enableInteractiveSelection));
        break;
      case "text-style":
        setTextInputStyle(Number(payload.hostId), String(payload.styleJson));
        break;
      case "caret":
        setCaretRect(Number(payload.hostId), Number(payload.left), Number(payload.top), Number(payload.width), Number(payload.height));
        break;
      case "editable-geometry":
        setEditableSizeAndTransform(
          Number(payload.hostId), Number(payload.width), Number(payload.height), String(payload.transformJson));
        break;
      case "context-menu": setContextMenuEnabled(Number(payload.hostId), Boolean(payload.enabled)); break;
      case "text-clear": clearTextInput(Number(payload.hostId)); break;
      case "semantics": updateSemantics(Number(payload.hostId), String(payload.json)); break;
      case "application-title":
        setApplicationTitle(Number(payload.hostId), String(payload.title));
        break;
      default: throw new Error(`Unknown Doroti worker control '${kind}'.`);
    }
  };

  const attachWorker = (worker: DorotiWorkerEndpoint): void => {
    worker.addEventListener("message", (event) => {
      let message: Record<string, unknown>;
      try {
        message = decodeDorotiMessage((event as MessageEvent).data, new Set([
          "runtime-ready", "gpu-ready", "snapshot-applied", "admission-applied", "managed-raster",
          "present-requested", "direct-commit", "terminal", "resource", "context-lost", "gpu-disposed",
        "context-restored", "control", "control-request", "closed", "disposed", "fatal",
        ]));
      } catch (error) {
        message = { kind: "fatal", error: `protocol violation: ${String(error)}` };
      }
      switch (message.kind) {
        case "runtime-ready":
          ready = true;
          root.dataset.dorotiWorkerRuntime = "ready";
          root.dataset.dorotiSharedRuntimeRenderThread = String(message.sharedRuntimeRenderThread === true);
          root.dataset.dorotiRenderThreadId = String(message.renderThreadId ?? 0);
          root.dataset.dorotiRenderWorkerIsolated = String(message.workerIsolated === true);
          resolveReady("started");
          break;
        case "gpu-ready":
          host.gpu = message.gpu as GpuIdentity;
          display.contextGeneration = Number(message.contextGeneration);
          emit(host);
          break;
        case "snapshot-applied": {
          const appliedGeneration = Number(message.generation);
          if (worker !== activeWorker || !snapshotInFlight ||
              appliedGeneration !== snapshotInFlightGeneration) break;
          snapshotInFlight = false;
          snapshotInFlightGeneration = 0;
          recordResize(host, "worker-snapshot-applied", "worker-mailbox", {
            detail: JSON.stringify({ generation: appliedGeneration }),
          });
          sendLatestWorkerSnapshot();
          break;
        }
        case "admission-applied": {
          const acknowledgedGeneration = Number(message.generation);
          if (worker === activeWorker && admissionInFlightGenerations.delete(acknowledgedGeneration)) {
            sendLatestDirectAdmission();
          }
          recordResize(host, "worker-admission-applied", "worker-resize-fast-lane", {
            detail: JSON.stringify({
              previousGeneration: Number(message.previousGeneration),
              generation: acknowledgedGeneration,
              mailboxGeneration: Number(message.mailboxGeneration),
              accepted: Boolean(message.accepted),
            }),
          });
          break;
        }
        case "managed-raster":
          recordResize(host, String(message.phase), "worker-managed-skia", {
            timestampMicroseconds: Number.isFinite(Number(message.epochMilliseconds))
              ? Math.round((Number(message.epochMilliseconds) - performance.timeOrigin) * 1000) : undefined,
            detail: JSON.stringify({ callbackId: message.callbackId, generation: message.generation }),
            durationMicroseconds: Number(message.durationMicroseconds),
            surfaceWidth: Number(message.width), surfaceHeight: Number(message.height),
          });
          break;
        case "present-requested": {
          const requestId = Number(message.requestId);
          display.pendingLeases.set(requestId, {
            runtimeSessionId: display.runtimeSessionId,
            causalFrameId: requestId,
          });
          if (display.latestRequestId !== null) display.latestRequestId = requestId;
          else if (display.currentRequestId === null) display.currentRequestId = requestId;
          else display.latestRequestId = requestId;
          recordResize(host, "present-requested", "worker-presenter", {
            requestId, rafId: requestId,
            surfaceWidth: (message.epoch as ResizeEpoch).physicalWidth,
            surfaceHeight: (message.epoch as ResizeEpoch).physicalHeight,
          });
          break;
        }
        case "direct-commit": {
          const requestId = Number(message.requestId);
          const frameGeneration = Number(message.generation);
          const frameLogicalWidth = Number(message.logicalWidth);
          const frameLogicalHeight = Number(message.logicalHeight);
          const framePhysicalWidth = Number(message.physicalWidth);
          const framePhysicalHeight = Number(message.physicalHeight);
          const frameDevicePixelRatio = Number(message.devicePixelRatio);
          const capacityWidth = Number(message.capacityWidth);
          const capacityHeight = Number(message.capacityHeight);
          display.rasterWidth = framePhysicalWidth;
          display.rasterHeight = framePhysicalHeight;
          display.displayWidth = capacityWidth;
          display.displayHeight = capacityHeight;
          const validDimensions = Number.isFinite(frameLogicalWidth) && frameLogicalWidth > 0 &&
            Number.isFinite(frameLogicalHeight) && frameLogicalHeight > 0 &&
            Number.isInteger(framePhysicalWidth) && framePhysicalWidth > 0 &&
            Number.isInteger(framePhysicalHeight) && framePhysicalHeight > 0 &&
            Number.isFinite(frameDevicePixelRatio) && frameDevicePixelRatio > 0 &&
            Number.isInteger(capacityWidth) && capacityWidth >= framePhysicalWidth &&
            Number.isInteger(capacityHeight) && capacityHeight >= framePhysicalHeight;
          const admitted = validDimensions && requestId > display.frontRequestId &&
            frameGeneration >= display.frontGeneration &&
            frameGeneration <= host.resizeEpoch.generation;
          if (admitted) {
            display.frontGeneration = frameGeneration;
            display.frontRequestId = requestId;
            configureDirectCanvasCapacity(
              host, frameLogicalWidth, frameLogicalHeight, frameDevicePixelRatio,
              capacityWidth, capacityHeight);
            canvas.dataset.dorotiFrontLogicalWidth = String(frameLogicalWidth);
            canvas.dataset.dorotiFrontLogicalHeight = String(frameLogicalHeight);
          }
          recordResize(host, admitted ? "front-commit" : "ack", "worker-direct-surface", {
            timestampMicroseconds: Number.isFinite(Number(message.commitEpochMilliseconds))
              ? Math.round((Number(message.commitEpochMilliseconds) - performance.timeOrigin) * 1000)
              : undefined,
            requestId, rafId: requestId, inputSequence: Number(message.inputSequence ?? 0),
            backingWidth: capacityWidth, backingHeight: capacityHeight,
            surfaceWidth: display.rasterWidth, surfaceHeight: display.rasterHeight,
            detail: JSON.stringify({
              generation: frameGeneration,
              targetGeneration: host.resizeEpoch.generation,
              contextGeneration: Number(message.contextGeneration),
              direct: true,
              sceneDisposition: message.sceneDisposition,
              capacityWidth,
              capacityHeight,
              progressive: frameGeneration < host.resizeEpoch.generation,
              admitted,
              managedSurfaceMicroseconds: Number(message.managedSurfaceMicroseconds),
              directFinalizeMicroseconds: Number(message.directFinalizeMicroseconds),
            }),
          });
          break;
        }
        case "terminal": {
          const requestId = Number(message.requestId);
          const terminal = String(message.terminal);
          display.pendingLeases.delete(requestId);
          if (display.currentRequestId === requestId) display.currentRequestId = null;
          if (display.latestRequestId === requestId) display.latestRequestId = null;
          recordResize(host, terminal === "submitted" ? "submitted" : "ack", "worker-presenter", {
            requestId, rafId: requestId, terminal, detail: String(message.detail),
            backingWidth: canvas.width, backingHeight: canvas.height,
            surfaceWidth: display.rasterWidth, surfaceHeight: display.rasterHeight,
          });
          break;
        }
        case "closed":
          // The managed host detached; GPU/queued port work is still draining.
          // Only "disposed" ends the renderer role and retires DOM endpoints.
          break;
        case "disposed":
          root.dataset.dorotiWorkerRuntime = "disposed";
          if (display.pendingLeases.size !== 0)
            throw new Error(`Doroti worker disposed with ${display.pendingLeases.size} external leases.`);
          recordResize(host, "disposed", "worker-supervisor", {
            detail: JSON.stringify({ runtimeSessionId: display.runtimeSessionId }),
          });
          if (runtimeLocation === "main") {
            closeHost(host.id);
            workerDisplayPresenters.delete(canvas.id);
            if (terminalFailure) app.textContent = terminalFailure.message;
            return;
          }
          break;
        case "resource":
          display.bitmapCreated = Number(message.bitmapCreated);
          display.bitmapConsumed = Number(message.bitmapConsumed);
          display.bitmapClosed = Number(message.bitmapClosed);
          display.activeBitmaps = Number(message.activeBitmaps);
          display.contextGeneration = Number(message.contextGeneration);
          display.rasterWidth = Number(message.rasterWidth);
          display.rasterHeight = Number(message.rasterHeight);
          display.displayWidth = Number(message.displayWidth ?? message.rasterWidth);
          display.displayHeight = Number(message.displayHeight ?? message.rasterHeight);
          break;
        case "context-lost": display.contextLost = true; break;
        case "context-restored":
          display.contextLost = false;
          display.contextGeneration = Number(message.contextGeneration);
          break;
        case "control":
          void handleControl(message).catch((error) => console.error("Doroti worker control failed.", error));
          break;
        case "control-request": {
          const correlationId = Number(message.correlationId);
          const kind = String(message.controlKind);
          const payload = (message.payload ?? {}) as Record<string, unknown>;
          void (async () => {
            if (kind === "url-launch") return launchExternalUrl(String(payload.url));
            if (kind === "haptic-feedback") {
              await vibrate(Number(payload.durationMilliseconds));
              return "";
            }
            if (kind === "clipboard-read") return readClipboardText();
            if (kind === "clipboard-write") return writeClipboardText(String(payload.text));
            if (kind === "plugin") return invokePlugin(
              String(payload.moduleUrl), String(payload.exportName), String(payload.channel),
              String(payload.codec), String(payload.payloadBase64));
            throw new Error(`Unknown Doroti worker request '${kind}'.`);
          })().then((result) => sendControlResponse(correlationId, result),
            (error) => sendControlResponse(correlationId, "", error));
          break;
        }
        case "fatal": {
          const error = new Error(`Doroti worker runtime failed: ${String(message.error)}`);
          if (runtimeLocation !== "main" && display.restartCount < 1) {
            display.restartCount++;
            display.runtimeSessionId++;
            worker.terminate();
            display.currentRequestId = null;
            display.latestRequestId = null;
            display.frontGeneration = 0;
            display.frontRequestId = 0;
            display.contextLost = false;
            snapshotInFlight = false;
            snapshotInFlightGeneration = 0;
            latestWorkerSnapshot = null;
            admissionInFlightGenerations.clear();
            latestDirectAdmission = null;
            closeExternalLeases(display.pendingLeases, (requestId, lease) => {
              recordResize(host, "ack", "worker-supervisor", {
                requestId, rafId: requestId, terminal: "failed",
                detail: `runtime-lost: session ${lease.runtimeSessionId} causal frame ${lease.causalFrameId}`,
              });
            });
            let replacementOffscreen: OffscreenCanvas | null = null;
            {
              const lifetimeInputSequence = host.inputSequence;
              const previousCanvas = canvas;
              closeHost(host.id);
              canvas = createReplacementCanvas(previousCanvas);
              directWorkerBootstrap = true;
              const rect = root.getBoundingClientRect();
              createHost(1, canvas.id, Math.max(1, rect.width), Math.max(1, rect.height));
              directWorkerBootstrap = false;
              host = requireHost(1);
              configureDirectCanvasCapacity(
                host, rect.width, rect.height, host.resizeEpoch.devicePixelRatio,
                undefined, undefined, true);
              host.inputSequence = lifetimeInputSequence;
              replacementOffscreen = createWorkerVisibleSurface(canvas).offscreen;
              workerDisplayPresenters.set(canvas.id, display);
            }
            const replacement = createDorotiWorker(new URL("./doroti.raster.worker.js", import.meta.url));
            activeWorker = replacement;
            display.worker = replacement;
            attachWorker(replacement);
            replacement.postMessage({
              protocolVersion: dorotiProtocolVersion, kind: "init", snapshot: JSON.parse(snapshot(host)),
              dotnetModuleUrl, mode, canvas: replacementOffscreen,
              testbedMode: new URL(location.href).searchParams.get("dorotiTestbedMode") ?? "diagnostics",
    progressScope: new URL(location.href).searchParams.get("dorotiProgressScope") ?? "local",
              resizeDiagnostics: diagnosticsEnabled(),
            }, replacementOffscreen ? [replacementOffscreen] : []);
          } else {
            terminalFailure = error;
            document.documentElement.dataset.dorotiRendererError = error.message;
            if (runtimeLocation === "main") worker.terminate();
            if (!ready) rejectReady(error);
            else root.dataset.dorotiWorkerRuntime = "failed";
          }
          break;
        }
      }
      // A frame emits several worker messages. Rewriting the entire trace DOM
      // on every message overwhelms the main thread during animation probes.
      // Live capture reads the trace directly; batch only its DOM publication.
      scheduleResizeDiagnosticsPublish(host);
    });
    worker.addEventListener("error", (event) => {
      const error = event as ErrorEvent;
      if (!ready && (runtimeLocation === "main" || display.restartCount >= 1))
        rejectReady(error.error ?? new Error(error.message));
    });
  };
  attachWorker(activeWorker);
  const initialOffscreen = createWorkerVisibleSurface(canvas).offscreen;
  const initialMessage = {
    protocolVersion: dorotiProtocolVersion, kind: "init", snapshot: JSON.parse(snapshot(host)),
    rendererContractVersion: mode === "worker-direct-webgpu" ? dorotiWebGpuRendererVersion : undefined,
    dotnetModuleUrl, mode, canvas: initialOffscreen,
    testbedMode: new URL(location.href).searchParams.get("dorotiTestbedMode") ?? "diagnostics",
    progressScope: new URL(location.href).searchParams.get("dorotiProgressScope") ?? "local",
    resizeDiagnostics: diagnosticsEnabled(),
  };
  activeWorker.postMessage(initialMessage, initialOffscreen ? [initialOffscreen] : []);
  globalThis.addEventListener("pagehide", () => {
    if (runtimeLocation === "main") {
      // Keep DOM endpoints alive until the role has drained cursor/text/frame
      // messages and acknowledged disposal. The main runtime still owns it.
      activeWorker.terminate();
      return;
    }
    activeWorker.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "dispose" });
    closeHost(host.id);
    workerDisplayPresenters.delete(canvas.id);
  }, { once: true });
  return readyPromise;
}

function resolveCurrentDotnetModuleUrl(): string {
  const stableUrl = new URL("./_framework/dotnet.js", document.baseURI).href;
  // The development static-web-assets loader fingerprints this module and
  // pairs the stable dotnet alias with the current native Skia relink. The
  // published module is served under its stable name and must receive the
  // document import map's fingerprinted standalone runtime URL instead.
  if (!new URL(import.meta.url).pathname.endsWith("/doroti.web.js")) return stableUrl;
  for (const script of document.querySelectorAll<HTMLScriptElement>('script[type="importmap"]')) {
    try {
      const imports = (JSON.parse(script.textContent ?? "{}") as { imports?: Record<string, string> }).imports;
      const mapped = imports?.["./_framework/dotnet.js"] ?? imports?.["/_framework/dotnet.js"];
      if (mapped) return new URL(mapped, document.baseURI).href;
    } catch {
      // Ignore unrelated or malformed maps and use the stable development alias.
    }
  }
  return stableUrl;
}

function requireHost(hostId: number): BrowserHost {
  const host = hosts.get(hostId);
  if (!host) throw new Error(`Doroti browser host ${hostId} is not active.`);
  return host;
}

function requireManaged(): ManagedCallbacks {
  if (!managed) throw new Error("Doroti browser managed callbacks are unavailable.");
  return managed;
}

function modifierMask(event: MouseEvent | KeyboardEvent): number {
  return (event.shiftKey ? 1 : 0) | (event.ctrlKey ? 2 : 0) |
    (event.altKey ? 4 : 0) | (event.metaKey ? 8 : 0);
}

let cachedDefaultScrollLineHeight: number | null = null;

function defaultScrollLineHeight(): number {
  if (cachedDefaultScrollLineHeight !== null) return cachedDefaultScrollLineHeight;
  const probe = document.createElement("div");
  probe.style.fontSize = "initial";
  probe.style.display = "none";
  document.body.appendChild(probe);
  const parsed = Number.parseFloat(globalThis.getComputedStyle(probe).fontSize);
  probe.remove();
  // Match Flutter Web: Firefox line deltas use one quarter of the browser's
  // default font height, falling back to 4 logical pixels.
  cachedDefaultScrollLineHeight = Number.isFinite(parsed) ? parsed / 4 : 4;
  return cachedDefaultScrollLineHeight;
}

function isTrackpadWheel(host: BrowserHost, event: WheelEvent): boolean {
  const legacy = event as WheelEvent & { wheelDeltaX?: number; wheelDeltaY?: number };
  const accelerated = (delta: number, wheelDelta: number | undefined): boolean =>
    wheelDelta !== undefined && Math.abs(wheelDelta - (-3 * delta)) > 1;
  let trackpad = true;
  if (accelerated(event.deltaX, legacy.wheelDeltaX) ||
      accelerated(event.deltaY, legacy.wheelDeltaY)) {
    trackpad = false;
  } else if ((event.deltaX % 120 === 0 && event.deltaY % 120 === 0) ||
      (((legacy.wheelDeltaX ?? 1) % 120 === 0) && ((legacy.wheelDeltaY ?? 1) % 120 === 0))) {
    const deltaXChange = Math.abs(event.deltaX - host.lastWheelDeltaX);
    const deltaYChange = Math.abs(event.deltaY - host.lastWheelDeltaY);
    const first = host.lastWheelTimestamp === 0;
    const looksUnlikePreviousTrackpadSample = first ||
      (deltaXChange === 0 && deltaYChange === 0) ||
      !(deltaXChange < 20 && deltaYChange < 20);
    if (looksUnlikePreviousTrackpadSample) {
      const continuedTrackpadGesture = !first &&
        event.timeStamp - host.lastWheelTimestamp < 50 && host.lastWheelWasTrackpad;
      trackpad = continuedTrackpadGesture;
    }
  }
  host.lastWheelDeltaX = event.deltaX;
  host.lastWheelDeltaY = event.deltaY;
  host.lastWheelTimestamp = event.timeStamp;
  host.lastWheelWasTrackpad = trackpad;
  return trackpad;
}

function closeTextConnectionAfterBlur(host: BrowserHost): void {
  if (host.input.hidden) return;
  host.composing = false;
  host.compositionStart = -1;
  host.input.value = "";
  host.input.hidden = true;
  rememberTextState(host, "", 0, 0, -1, -1);
  requireManaged().dispatchTextConnectionClosed(host.id, ++host.inputSequence);
}

function handleTextInputBlur(
  host: BrowserHost,
  event: FocusEvent,
  belongsToHost: (target: EventTarget | null) => boolean): void {
  const willGainFocus = event.relatedTarget;
  if (willGainFocus === null) {
    if (!document.hasFocus()) {
      if (host.pendingBlurConnectionCloseTimer !== 0)
        clearTimeout(host.pendingBlurConnectionCloseTimer);
      // A tab switch reports input blur before visibilitychange. Flutter waits
      // briefly so hidden tabs keep their text connection, while an ordinary
      // window/iframe blur closes it and unfocuses EditableText.
      host.pendingBlurConnectionCloseTimer = globalThis.setTimeout(() => {
        host.pendingBlurConnectionCloseTimer = 0;
        if (document.visibilityState === "hidden" || document.hasFocus()) return;
        closeTextConnectionAfterBlur(host);
      }, 100);
      return;
    }
    closeTextConnectionAfterBlur(host);
  } else if (belongsToHost(willGainFocus) && !host.input.hidden) {
    host.input.focus({ preventScroll: true });
  }
}

function emitText(host: BrowserHost): void {
  let start = host.input.selectionStart ?? 0;
  let end = host.input.selectionEnd ?? start;
  const selectionBackward = host.input.selectionDirection === "backward";
  let selectionBase = selectionBackward ? end : start;
  let selectionExtent = selectionBackward ? start : end;
  if (!host.interactiveSelectionEnabled && selectionBase !== selectionExtent) {
    host.input.setSelectionRange(selectionExtent, selectionExtent);
    start = selectionExtent;
    end = selectionExtent;
    selectionBase = selectionExtent;
  }
  const composingBase = host.composing ? Math.max(0, host.compositionStart) : -1;
  const composingExtent = host.composing ? end : -1;
  if (host.lastTextValue === host.input.value &&
      host.lastSelectionBase === selectionBase &&
      host.lastSelectionExtent === selectionExtent &&
      host.lastComposingBase === composingBase &&
      host.lastComposingExtent === composingExtent) return;
  rememberTextState(
    host, host.input.value, selectionBase, selectionExtent, composingBase, composingExtent);
  const inputSequence = ++host.inputSequence;
  recordResize(host, "text-editing-dispatched", "browser-text-input", {
    inputSequence,
    detail: JSON.stringify({
      textLength: host.input.value.length,
      selectionBase, selectionExtent, composingBase, composingExtent,
    }),
  });
  requireManaged().dispatchTextEditing(
    host.id, host.input.value, selectionBase, selectionExtent, composingBase, composingExtent,
    inputSequence);
}

function rememberTextState(
  host: BrowserHost,
  text: string,
  selectionBase: number,
  selectionExtent: number,
  composingBase: number,
  composingExtent: number): void {
  host.lastTextValue = text;
  host.lastSelectionBase = selectionBase;
  host.lastSelectionExtent = selectionExtent;
  host.lastComposingBase = composingBase;
  host.lastComposingExtent = composingExtent;
}

function setViewFocus(host: BrowserHost, focused: boolean, timestamp: number): void {
  if (host.viewFocused === focused) return;
  host.viewFocused = focused;
  requireManaged().dispatchFocus(host.id, focused, timestamp, ++host.inputSequence);
  emit(host);
}

function focusActiveEndpoint(host: BrowserHost): void {
  const endpoint = host.input.hidden ? host.canvas : host.input;
  if (document.activeElement !== endpoint) endpoint.focus({ preventScroll: true });
}

function releasePressedKeys(host: BrowserHost): void {
  const timestamp = performance.now();
  for (const [code, key] of host.pressedKeys) {
    requireManaged().dispatchKey(host.id, false, false, true, code, key, timestamp,
      ++host.inputSequence);
  }
  host.pressedKeys.clear();
}

function dispatchSemantics(host: BrowserHost, nodeId: number | string, action: number, args: unknown = null): void {
  const id = Number(nodeId);
  const node = host.semanticsActionNodes.get(id);
  if (host.semanticsProjectionDepth !== 0 || !node || node.flags?.enabled === false ||
      ((node.actions ?? 0) & action) !== action) return;
  if (node.flags?.readOnly && (action & ((1 << 21) | (1 << 13) | (1 << 14) | (1 << 6) | (1 << 7))) !== 0) return;
  // Programmatic value/selection changes can queue DOM events after projection
  // has ended. Reject their unchanged values, while retaining real AT actions.
  const observedEditing = host.semanticsObservedEditing.get(id);
  if (action === (1 << 21) && args === (observedEditing?.value ?? node.value ?? "")) return;
  if (action === (1 << 11)) {
    const selection = args as { base: number; extent: number } | null;
    if (selection && selection.base === (observedEditing?.base ?? node.textSelectionBase) &&
        selection.extent === (observedEditing?.extent ?? node.textSelectionExtent)) return;
  }
  if (action === (1 << 22) && node.flags?.focused === true) return;
  // Track accepted changes before dispatch (which may synchronously project a
  // new tree), so editing back to the previous value is still genuine input.
  if (observedEditing && action === (1 << 21) && typeof args === "string") observedEditing.value = args;
  if (observedEditing && action === (1 << 11) && args) {
    const selection = args as { base: number; extent: number };
    observedEditing.base = selection.base;
    observedEditing.extent = selection.extent;
  }
  requireManaged().dispatchSemanticsAction(
    host.id, id, action, ++host.inputSequence, JSON.stringify(args));
}

function textSelectionOffsets(element: HTMLElement): { base: number; extent: number } | null {
  if (element instanceof HTMLInputElement || element instanceof HTMLTextAreaElement) {
    const base = element.selectionStart;
    const extent = element.selectionEnd;
    return base === null || extent === null ? null : element.selectionDirection === "backward"
      ? { base: extent, extent: base } : { base, extent };
  }
  const selection = globalThis.getSelection?.();
  if (!selection || selection.rangeCount === 0 || !element.contains(selection.anchorNode) || !element.contains(selection.focusNode)) return null;
  const offset = (node: Node | null, nodeOffset: number): number => {
    const range = document.createRange();
    range.selectNodeContents(element);
    if (node) range.setEnd(node, nodeOffset);
    return range.toString().length;
  };
  return { base: offset(selection.anchorNode, selection.anchorOffset), extent: offset(selection.focusNode, selection.focusOffset) };
}

function semanticsElementTag(node: SemanticsNode): "div" | "input" | "textarea" {
  if (!node.flags?.textField) return "div";
  return node.flags.multiline ? "textarea" : "input";
}

function semanticsDomId(hostId: number, nodeId: number): string {
  return `doroti-semantics-${hostId}-${nodeId}`;
}

function isNativeFocusableElement(element: HTMLElement): boolean {
  return element instanceof HTMLInputElement || element instanceof HTMLTextAreaElement;
}

function semanticsInputMode(inputType: string | undefined): typeof HTMLInputElement.prototype.inputMode {
  switch ((inputType ?? "").toLowerCase()) {
    case "email": return "email";
    case "phone": return "tel";
    case "url": return "url";
    case "search": return "search";
    case "text": return "text";
    default: return "";
  }
}

function resetSemanticsAttributes(element: HTMLElement): void {
  delete element.dataset.dorotiSemanticsIdentifier;
  for (const attribute of [
    "role", "aria-label", "aria-description", "aria-controls", "aria-invalid", "aria-level",
    "aria-valuetext", "aria-valuemin", "aria-valuemax", "aria-valuenow", "aria-hidden",
    "aria-live", "aria-checked", "aria-selected", "aria-disabled", "aria-pressed",
    "aria-expanded", "aria-required", "aria-multiline", "aria-readonly", "lang",
  ]) element.removeAttribute(attribute);
  if (element instanceof HTMLInputElement || element instanceof HTMLTextAreaElement) {
    element.removeAttribute("maxlength");
    element.removeAttribute("inputmode");
  }
}

function setOptionalAttribute(element: HTMLElement, name: string, value: string | null | undefined): void {
  if (value) element.setAttribute(name, value);
  else element.removeAttribute(name);
}

function placeSemanticsChildren(parent: HTMLElement, desired: HTMLElement[]): void {
  for (let index = 0; index < desired.length; index += 1) {
    const child = desired[index];
    if (parent.children.item(index) !== child) parent.insertBefore(child, parent.children.item(index));
  }
}

function removeUnexpectedSemanticsChildren(parent: HTMLElement, desired: HTMLElement[]): void {
  const expected = new Set(desired);
  for (const child of Array.from(parent.children)) {
    if (child instanceof HTMLElement && child.dataset.dorotiSemanticsId !== undefined && !expected.has(child)) child.remove();
  }
}

function semanticsTextFieldAtPoint(host: BrowserHost, clientX: number, clientY: number): HTMLElement | null {
  const candidates = Array.from(host.semanticsElements.values()).reverse();
  for (const element of candidates) {
    if (!(element instanceof HTMLInputElement || element instanceof HTMLTextAreaElement)) continue;
    if (element.getAttribute("role") !== "textbox") continue;
    const rect = element.getBoundingClientRect();
    if (clientX >= rect.left && clientX <= rect.right && clientY >= rect.top && clientY <= rect.bottom)
      return element;
  }
  return null;
}

function placeTextInputAtSemanticsElement(host: BrowserHost, element: HTMLElement): void {
  const rootRect = host.root.getBoundingClientRect();
  const fieldRect = element.getBoundingClientRect();
  host.editableGeometryApplied = true;
  host.input.style.left = `${fieldRect.left - rootRect.left}px`;
  host.input.style.top = `${fieldRect.top - rootRect.top}px`;
  host.input.style.width = `${Math.max(1, fieldRect.width)}px`;
  host.input.style.height = `${Math.max(1, fieldRect.height)}px`;
  host.input.style.transform = "none";
}

function applySemanticsFlags(element: HTMLElement, flags: SemanticsFlags | undefined, role: string): void {
  if (!flags) return;
  if (flags.hidden) element.setAttribute("aria-hidden", "true");
  if (flags.liveRegion) element.setAttribute("aria-live", "polite");
  if (flags.checked && flags.checked !== "none") element.setAttribute("aria-checked", flags.checked === "mixed" ? "mixed" : String(flags.checked === "isTrue"));
  if (flags.selected !== undefined && flags.selected !== null) {
    // A merged RadioListTile has the radio's checked state and the independent
    // tile-highlight selected state. Only selected-only segmented radios derive
    // aria-checked from selected; a tile highlight must not overwrite a radio.
    if (role !== "radio") element.setAttribute("aria-selected", String(flags.selected));
    else if (!flags.checked || flags.checked === "none") element.setAttribute("aria-checked", String(flags.selected));
  }
  if (flags.enabled === false) element.setAttribute("aria-disabled", "true");
  if (flags.toggled !== undefined && flags.toggled !== null) {
    element.setAttribute(role === "switch" ? "aria-checked" : "aria-pressed", String(flags.toggled));
  }
  if (flags.expanded !== undefined && flags.expanded !== null) element.setAttribute("aria-expanded", String(flags.expanded));
  if (flags.required !== undefined && flags.required !== null) element.setAttribute("aria-required", String(flags.required));
  if (flags.multiline) element.setAttribute("aria-multiline", "true");
  if (flags.readOnly) element.setAttribute("aria-readonly", "true");
}

function semanticsRole(node: SemanticsNode): string {
  const flags = node.flags;
  const key = String(node.role ?? "").toLowerCase();
  if (key.includes("alertdialog")) return "alertdialog";
  if (key.includes("dialog")) return "dialog";
  if (key.includes("navigation")) return "navigation";
  if (key.includes("contentinfo")) return "contentinfo";
  if (key.includes("complementary")) return "complementary";
  if (key === "main") return "main";
  if (key.includes("progressbar")) return "progressbar";
  if (key.includes("spinbutton")) return "spinbutton";
  if (key.includes("combobox")) return "combobox";
  if (key.includes("menuitemcheckbox")) return "menuitemcheckbox";
  if (key.includes("menuitemradio")) return "menuitemradio";
  if (key.includes("menuitem")) return "menuitem";
  if (key.includes("menubar")) return "menubar";
  if (key === "menu") return "menu";
  if (key.includes("tabpanel")) return "tabpanel";
  if (key.includes("tabbar")) return "tablist";
  if (key === "tab") return "tab";
  if (key.includes("columnheader")) return "columnheader";
  if (key === "row") return "row";
  if (key === "cell") return "cell";
  if (key === "table") return "table";
  if (key.includes("radiogroup")) return "radiogroup";
  if (key === "radio") return "radio";
  if (key.includes("tooltip")) return "tooltip";
  if (key.includes("button")) return "button";
  if (key.includes("textfield")) return "textbox";
  if (key.includes("slider") || key.includes("draghandle")) return "slider";
  if (key.includes("listitem")) return "listitem";
  if (key.includes("list")) return "list";
  if (key.includes("image")) return "img";
  if (key.includes("status")) return "status";
  if (key.includes("alert")) return "alert";
  if (key.includes("form")) return "form";
  if (key.includes("region")) return "region";
  if (flags?.textField) return "textbox";
  if (flags?.slider) return "slider";
  if (flags?.mutuallyExclusive &&
      ((flags.checked && flags.checked !== "none") || flags.selected !== undefined)) return "radio";
  if (flags?.toggled !== undefined && flags.toggled !== null) return "switch";
  if (flags?.checked && flags.checked !== "none") return "checkbox";
  if (flags?.button || flags?.keyboardKey) return "button";
  if (flags?.link || node.linkUrl) return "link";
  if (flags?.image) return "img";
  if (flags?.header) return "heading";
  return "group";
}
