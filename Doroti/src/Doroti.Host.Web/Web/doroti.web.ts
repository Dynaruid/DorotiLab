interface ManagedCallbacks {
  dispatchAnimationFrame(hostId: number, callbackId: number, timestamp: number): void;
  dispatchSnapshot(hostId: number, snapshotJson: string): void;
  dispatchPointerBatch(hostId: number, phase: number, kind: number, pointerId: number, buttons: number, modifiers: number, samples: number[]): void;
  dispatchWheel(hostId: number, x: number, y: number, deltaX: number, deltaY: number, timestamp: number): void;
  dispatchKey(hostId: number, pressed: boolean, repeat: boolean, synthesized: boolean, code: string, key: string, timestamp: number): void;
  dispatchFocus(hostId: number, focused: boolean, timestamp: number): void;
  dispatchTextEditing(hostId: number, text: string, selectionBase: number, selectionExtent: number, composingBase: number, composingExtent: number): void;
  dispatchTextAction(hostId: number, action: number): void;
  dispatchSemanticsAction(hostId: number, nodeId: number, action: number, argumentsJson: string): void;
}

interface GpuIdentity {
  api: "webgl2";
  vendor: string;
  renderer: string;
  hardware: true;
  softwareFallbackUsed: boolean;
}

interface ListenerRegistration {
  target: EventTarget;
  name: string;
  handler: EventListener;
}

interface BrowserHost {
  id: number;
  root: HTMLElement;
  canvas: HTMLCanvasElement;
  input: HTMLTextAreaElement;
  semantics: HTMLElement;
  semanticsElements: Map<number, HTMLElement>;
  semanticsListeners: Map<number, AbortController>;
  logicalWidth: number;
  logicalHeight: number;
  generation: number;
  surfaceGeneration: number;
  resizeGeneration: number;
  emittedResizeGeneration: number;
  resizeEpoch: ResizeEpoch;
  resizeTrace: ResizeTraceEntry[];
  resizeTraceSequence: number;
  dprQuery: MediaQueryList | null;
  frameRaf: number;
  latestFrameCallback: number;
  gpu: GpuIdentity;
  observers: ResizeObserver[];
  listeners: ListenerRegistration[];
  composing: boolean;
  compositionStart: number;
  viewFocused: boolean;
  pressedKeys: Map<string, string>;
  inputAction: number;
  multiline: boolean;
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
}

interface PresentDescriptor extends ResizeEpoch {
  requestId: number;
  terminalRecorded: boolean;
}

interface ManagedCanvasPresenter {
  invokeMethodAsync(name: string, ...args: unknown[]): Promise<unknown>;
}

interface EmscriptenGlRuntime {
  createContext(canvas: HTMLCanvasElement, attributes: Record<string, number>): number;
  makeContextCurrent(context: number): void;
  deleteContext?(context: number): void;
  getNewId<T>(table: Array<T | null>): number;
  framebuffers: Array<(WebGLFramebuffer & { name?: number }) | null>;
  currentContext?: { GLctx: WebGL2RenderingContext };
}

interface GpuSurface {
  framebuffer: WebGLFramebuffer & { name?: number };
  framebufferId: number;
  color: WebGLTexture;
  depthStencil: WebGLRenderbuffer;
  width: number;
  height: number;
  logicalWidth: number;
  logicalHeight: number;
  devicePixelRatio: number;
  generation: number;
}

interface CanvasPresenter {
  canvas: HTMLCanvasElement;
  callback: ManagedCanvasPresenter;
  context: number;
  contextGeneration: number;
  contextLossExtension: WEBGL_lose_context | null;
  current: PresentDescriptor | null;
  latest: PresentDescriptor | null;
  drainScheduled: boolean;
  nextRequestId: number;
  contextLost: boolean;
  front: GpuSurface | null;
  frontGeneration: number;
  staging: GpuSurface | null;
  glStateDirty: boolean;
  listeners: ListenerRegistration[];
}

interface ResizeDiagnostics {
  capture(hostId: number): string;
  snapshot(hostId: number): string;
  presenter(canvasId: string): string;
  loseContext(canvasId: string): boolean;
  restoreContext(canvasId: string): boolean;
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
        BrowserInterop: {
          DispatchAnimationFrame: ManagedCallbacks["dispatchAnimationFrame"];
          DispatchSnapshot: ManagedCallbacks["dispatchSnapshot"];
          DispatchPointerBatch: ManagedCallbacks["dispatchPointerBatch"];
          DispatchWheel: ManagedCallbacks["dispatchWheel"];
          DispatchKey: ManagedCallbacks["dispatchKey"];
          DispatchFocus: ManagedCallbacks["dispatchFocus"];
          DispatchTextEditing: ManagedCallbacks["dispatchTextEditing"];
          DispatchTextAction: ManagedCallbacks["dispatchTextAction"];
          DispatchSemanticsAction: ManagedCallbacks["dispatchSemanticsAction"];
        };
      };
    };
  };
}

const hosts = new Map<number, BrowserHost>();
const canvasPresenters = new Map<string, CanvasPresenter>();
let managed: ManagedCallbacks | null = null;

const resizeDiagnostics: ResizeDiagnostics = {
  capture: (hostId) => captureResizeTrace(hostId),
  snapshot: (hostId) => snapshot(requireHost(hostId)),
  presenter: (canvasId) => {
    const presenter = canvasPresenters.get(canvasId);
    if (!presenter) throw new Error(`Canvas presenter '${canvasId}' is not initialized.`);
    return JSON.stringify({
      context: presenter.context,
      contextGeneration: presenter.contextGeneration,
      currentRequestId: presenter.current?.requestId ?? null,
      latestRequestId: presenter.latest?.requestId ?? null,
      queueDepth: Number(presenter.current !== null) + Number(presenter.latest !== null),
      contextLost: presenter.contextLost,
      frontGeneration: presenter.frontGeneration || null,
      frontFramebufferId: presenter.front?.framebufferId ?? null,
      stagingFramebufferId: presenter.staging?.framebufferId ?? null,
    });
  },
  loseContext: (canvasId) => changeDiagnosticContextState(canvasId, true),
  restoreContext: (canvasId) => changeDiagnosticContextState(canvasId, false),
};
(globalThis as typeof globalThis & { __dorotiResizeDiagnostics?: ResizeDiagnostics })
  .__dorotiResizeDiagnostics = resizeDiagnostics;

function snapshot(host: BrowserHost): string {
  const ratio = host.resizeEpoch.devicePixelRatio;
  return JSON.stringify({
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
    gpu: host.gpu,
    resizeEpoch: host.resizeEpoch,
  });
}

function recordResize(
  host: BrowserHost,
  phase: string,
  source: string,
  options: Partial<Pick<ResizeTraceEntry,
    "durationMicroseconds" | "rafId" | "backingWidth" | "backingHeight" |
    "surfaceWidth" | "surfaceHeight" | "terminal" | "detail">> = {}): void {
  host.resizeTrace.push({
    sequence: ++host.resizeTraceSequence,
    timestampMicroseconds: Math.round(performance.now() * 1000),
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
      const presenter = canvasPresenters.get(host.canvas.id);
      return presenter ? Number(presenter.current !== null) + Number(presenter.latest !== null) : 0;
    })(),
  });
  if (host.resizeTrace.length > 16384) host.resizeTrace.splice(0, host.resizeTrace.length - 16384);
  publishResizeDiagnostics(host);
}

function publishResizeDiagnostics(host: BrowserHost): void {
  if (new URLSearchParams(globalThis.location.search).get("dorotiResizeDiagnostics") !== "1") return;
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

function updateResizeEpoch(
  host: BrowserHost,
  source: string,
  logicalWidth: number,
  logicalHeight: number,
  forceGeneration = false,
  physicalWidth = Math.max(1, Math.round(logicalWidth * Math.max(1, globalThis.devicePixelRatio || 1))),
  physicalHeight = Math.max(1, Math.round(logicalHeight * Math.max(1, globalThis.devicePixelRatio || 1))),
  ratio = Math.max(1, globalThis.devicePixelRatio || 1)): boolean {
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
  if (!applyProvisionalEpoch(host, host.resizeEpoch, source)) return;
  if (changed || host.emittedResizeGeneration !== host.resizeEpoch.generation) {
    host.emittedResizeGeneration = host.resizeEpoch.generation;
    emit(host);
  }
}

function armDprWatcher(host: BrowserHost): void {
  if (!globalThis.matchMedia) return;
  const query = globalThis.matchMedia(`(resolution: ${Math.max(1, globalThis.devicePixelRatio || 1)}dppx)`);
  host.dprQuery = query;
  const handler: EventListener = () => {
    const rect = host.root.getBoundingClientRect();
    const ratio = Math.max(1, globalThis.devicePixelRatio || 1);
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
  const ratio = Math.max(1, globalThis.devicePixelRatio || 1);
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

function hostForCanvas(canvas: HTMLCanvasElement): BrowserHost | undefined {
  for (const host of hosts.values()) if (host.canvas === canvas) return host;
  return undefined;
}

function changeDiagnosticContextState(canvasId: string, lose: boolean): boolean {
  const presenter = canvasPresenters.get(canvasId);
  if (!presenter) throw new Error(`Canvas presenter '${canvasId}' is not initialized.`);
  const extension = presenter.contextLossExtension;
  if (!extension) return false;
  if (lose) extension.loseContext();
  else extension.restoreContext();
  return true;
}

// WebGL bootstrap adapted from SkiaSharp 4.151.1 SKHtmlCanvas.js at
// mono/SkiaSharp commit 279f93f4ffa7f9fe4e9c0bc298bedc3c9e439764 (MIT).
// Doroti owns the context, rAF queue, backing-store commit and managed callback.
function emscriptenGl(): EmscriptenGlRuntime {
  const scope = globalThis as typeof globalThis & {
    SkiaSharpGL?: EmscriptenGlRuntime;
    SkiaSharpModule?: { GL?: EmscriptenGlRuntime };
    Module?: { GL?: EmscriptenGlRuntime };
    GL?: EmscriptenGlRuntime;
  };
  const runtime = scope.SkiaSharpGL ?? scope.SkiaSharpModule?.GL ?? scope.Module?.GL ?? scope.GL;
  if (!runtime) throw new Error("Doroti could not resolve the SkiaSharp Emscripten GL runtime.");
  return runtime;
}

function presenterGlInfo(presenter: CanvasPresenter): {
  context: number; fboId: number; stencilBits: number; sampleCount: number; depthBits: number;
} {
  const runtime = emscriptenGl();
  runtime.makeContextCurrent(presenter.context);
  const context = runtime.currentContext?.GLctx;
  if (!context) throw new Error("Doroti WebGL2 context is not current.");
  const framebuffer = context.getParameter(context.FRAMEBUFFER_BINDING) as WebGLFramebuffer & { id?: number } | null;
  return {
    context: presenter.context,
    fboId: framebuffer?.id ?? 0,
    stencilBits: context.getParameter(context.STENCIL_BITS) as number,
    sampleCount: 0,
    depthBits: context.getParameter(context.DEPTH_BITS) as number,
  };
}

function presenterGl(presenter: CanvasPresenter): WebGL2RenderingContext {
  const runtime = emscriptenGl();
  runtime.makeContextCurrent(presenter.context);
  return runtime.currentContext?.GLctx ??
    (() => { throw new Error("Doroti WebGL2 context is not current."); })();
}

function createGpuSurface(presenter: CanvasPresenter, width: number, height: number): GpuSurface {
  const runtime = emscriptenGl();
  const gl = presenterGl(presenter);
  const framebuffer = gl.createFramebuffer() as (WebGLFramebuffer & { name?: number }) | null;
  const color = gl.createTexture();
  const depthStencil = gl.createRenderbuffer();
  if (!framebuffer || !color || !depthStencil)
    throw new Error("Doroti could not allocate the retained WebGL framebuffer resources.");
  const framebufferId = runtime.getNewId(runtime.framebuffers);
  framebuffer.name = framebufferId;
  runtime.framebuffers[framebufferId] = framebuffer;
  try {
    gl.bindFramebuffer(gl.FRAMEBUFFER, framebuffer);
    gl.bindTexture(gl.TEXTURE_2D, color);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA8, width, height, 0, gl.RGBA, gl.UNSIGNED_BYTE, null);
    gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, color, 0);
    gl.bindRenderbuffer(gl.RENDERBUFFER, depthStencil);
    gl.renderbufferStorage(gl.RENDERBUFFER, gl.DEPTH24_STENCIL8, width, height);
    gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.DEPTH_STENCIL_ATTACHMENT, gl.RENDERBUFFER, depthStencil);
    const status = gl.checkFramebufferStatus(gl.FRAMEBUFFER);
    if (status !== gl.FRAMEBUFFER_COMPLETE)
      throw new Error(`Doroti retained framebuffer is incomplete (0x${status.toString(16)}).`);
    return {
      framebuffer, framebufferId, color, depthStencil, width, height,
      logicalWidth: 0, logicalHeight: 0, devicePixelRatio: 1, generation: 0,
    };
  } catch (error) {
    runtime.framebuffers[framebufferId] = null;
    framebuffer.name = 0;
    gl.deleteFramebuffer(framebuffer);
    gl.deleteTexture(color);
    gl.deleteRenderbuffer(depthStencil);
    throw error;
  } finally {
    gl.bindTexture(gl.TEXTURE_2D, null);
    gl.bindRenderbuffer(gl.RENDERBUFFER, null);
    gl.bindFramebuffer(gl.FRAMEBUFFER, null);
    presenter.glStateDirty = true;
  }
}

function releaseGpuSurface(presenter: CanvasPresenter, surface: GpuSurface | null, deleteObjects = true): void {
  if (!surface) return;
  const runtime = emscriptenGl();
  runtime.framebuffers[surface.framebufferId] = null;
  surface.framebuffer.name = 0;
  if (!deleteObjects || presenter.contextLost) return;
  const gl = presenterGl(presenter);
  gl.deleteFramebuffer(surface.framebuffer);
  gl.deleteTexture(surface.color);
  gl.deleteRenderbuffer(surface.depthStencil);
  presenter.glStateDirty = true;
}

function ensureStaging(presenter: CanvasPresenter, width: number, height: number): GpuSurface {
  if (presenter.staging &&
      (presenter.staging.width !== width || presenter.staging.height !== height)) {
    releaseGpuSurface(presenter, presenter.staging);
    presenter.staging = null;
  }
  return presenter.staging ??= createGpuSurface(presenter, width, height);
}

function blitRectToDefault(
  presenter: CanvasPresenter,
  source: GpuSurface,
  sourceX0: number,
  sourceY0: number,
  sourceX1: number,
  sourceY1: number,
  destinationX0: number,
  destinationY0: number,
  destinationX1: number,
  destinationY1: number,
  destinationWidth: number,
  destinationHeight: number,
  filter: number,
  clearBackground: boolean): {
    sourceStatus: number; destinationStatus: number; priorErrors: number[]; error: number;
  } {
  const gl = presenterGl(presenter);
  const priorErrors: number[] = [];
  for (let value = gl.getError(); value !== gl.NO_ERROR; value = gl.getError()) priorErrors.push(value);
  gl.disable(gl.SCISSOR_TEST);
  gl.colorMask(true, true, true, true);
  gl.depthMask(true);
  gl.stencilMask(0xff);
  gl.viewport(0, 0, destinationWidth, destinationHeight);
  gl.bindFramebuffer(gl.DRAW_FRAMEBUFFER, null);
  gl.drawBuffers([gl.BACK]);
  if (clearBackground) {
    const dark = globalThis.matchMedia?.("(prefers-color-scheme: dark)").matches ?? false;
    gl.clearColor(...(dark ? [20 / 255, 18 / 255, 24 / 255, 1] as const : [1, 251 / 255, 254 / 255, 1] as const));
    gl.clearDepth(1);
    gl.clearStencil(0);
    gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT | gl.STENCIL_BUFFER_BIT);
  }
  gl.bindFramebuffer(gl.READ_FRAMEBUFFER, source.framebuffer);
  gl.readBuffer(gl.COLOR_ATTACHMENT0);
  const sourceStatus = gl.checkFramebufferStatus(gl.READ_FRAMEBUFFER);
  gl.bindFramebuffer(gl.DRAW_FRAMEBUFFER, null);
  gl.drawBuffers([gl.BACK]);
  const destinationStatus = gl.checkFramebufferStatus(gl.DRAW_FRAMEBUFFER);
  if (sourceX1 > sourceX0 && sourceY1 > sourceY0 &&
      destinationX1 > destinationX0 && destinationY1 > destinationY0) {
    gl.blitFramebuffer(
      sourceX0, sourceY0, sourceX1, sourceY1,
      destinationX0, destinationY0, destinationX1, destinationY1,
      gl.COLOR_BUFFER_BIT, filter);
  }
  const error = gl.getError();
  gl.bindFramebuffer(gl.READ_FRAMEBUFFER, null);
  gl.bindFramebuffer(gl.DRAW_FRAMEBUFFER, null);
  gl.flush();
  presenter.glStateDirty = true;
  return { sourceStatus, destinationStatus, priorErrors, error };
}

function clearDefaultFramebuffer(presenter: CanvasPresenter): {
  destinationStatus: number; priorErrors: number[]; error: number;
} {
  const gl = presenterGl(presenter);
  const priorErrors: number[] = [];
  for (let value = gl.getError(); value !== gl.NO_ERROR; value = gl.getError()) priorErrors.push(value);
  gl.bindFramebuffer(gl.READ_FRAMEBUFFER, null);
  gl.readBuffer(gl.BACK);
  gl.bindFramebuffer(gl.DRAW_FRAMEBUFFER, null);
  gl.drawBuffers([gl.BACK]);
  const dark = globalThis.matchMedia?.("(prefers-color-scheme: dark)").matches ?? false;
  gl.disable(gl.SCISSOR_TEST);
  gl.colorMask(true, true, true, true);
  gl.depthMask(true);
  gl.stencilMask(0xff);
  gl.viewport(0, 0, presenter.canvas.width, presenter.canvas.height);
  gl.clearColor(...(dark ? [20 / 255, 18 / 255, 24 / 255, 1] as const : [1, 251 / 255, 254 / 255, 1] as const));
  gl.clearDepth(1);
  gl.clearStencil(0);
  gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT | gl.STENCIL_BUFFER_BIT);
  const destinationStatus = gl.checkFramebufferStatus(gl.DRAW_FRAMEBUFFER);
  const error = gl.getError();
  gl.flush();
  presenter.glStateDirty = true;
  return { destinationStatus, priorErrors, error };
}

function applyProvisionalEpoch(host: BrowserHost, target: ResizeEpoch, source: string): boolean {
  const presenter = canvasPresenters.get(host.canvas.id);
  if (!presenter || presenter.contextLost) return true;
  const gl = presenterGl(presenter);
  const started = performance.now();
  if (!presenter.front) {
    if (!commitCanvasEpoch(host, presenter, target, source)) return false;
    const status = clearDefaultFramebuffer(presenter);
    recordResize(host, "startup-background-commit",
      "doroti-presenter", {
        durationMicroseconds: Math.round((performance.now() - started) * 1000),
        detail: JSON.stringify({ trigger: source, noRetainedFront: true, ...status }),
      });
    return status.destinationStatus === gl.FRAMEBUFFER_COMPLETE &&
      status.priorErrors.length === 0 && status.error === gl.NO_ERROR;
  }

  const front = presenter.front;
  // ResizeObserver runs before browser paint. Commit the target-sized canvas
  // backing store in this callback, clear it, and copy only the overlap from
  // the retained exact FBO at 1:1. The browser therefore never receives an old
  // intrinsic canvas bitmap paired with a new CSS display rectangle to scale.
  if (!commitCanvasEpoch(host, presenter, target, source)) return false;
  const copyWidth = Math.min(front.width, target.physicalWidth);
  const copyHeight = Math.min(front.height, target.physicalHeight);
  const status = blitRectToDefault(
    presenter, front,
    0, 0, copyWidth, copyHeight,
    0, 0, copyWidth, copyHeight,
    target.physicalWidth, target.physicalHeight,
    gl.NEAREST, true);
  recordResize(host, "stable-front-refresh", "doroti-presenter", {
      durationMicroseconds: Math.round((performance.now() - started) * 1000),
      surfaceWidth: target.physicalWidth, surfaceHeight: target.physicalHeight,
      detail: JSON.stringify({
        trigger: source,
        policy: "target-sized-default-top-left-crop",
        target: [target.physicalWidth, target.physicalHeight],
        committed: [front.width, front.height],
        sourceRect: [0, 0, copyWidth, copyHeight],
        destinationRect: [0, 0, copyWidth, copyHeight],
        scaleX: 1,
        scaleY: 1,
        ...status,
      }),
    });
  return status.sourceStatus === gl.FRAMEBUFFER_COMPLETE &&
    status.destinationStatus === gl.FRAMEBUFFER_COMPLETE &&
    status.priorErrors.length === 0 && status.error === gl.NO_ERROR;
}

function commitCanvasEpoch(
  host: BrowserHost,
  presenter: CanvasPresenter,
  target: ResizeEpoch,
  source: string): boolean {
  const changed = host.canvas.width !== target.physicalWidth ||
    host.canvas.height !== target.physicalHeight;
  if (changed) {
    recordResize(host, "backing-reset-start", source, {
      backingWidth: host.canvas.width,
      backingHeight: host.canvas.height,
      detail: `target=${target.physicalWidth}x${target.physicalHeight}`,
    });
    host.canvas.width = target.physicalWidth;
    host.canvas.height = target.physicalHeight;
    host.surfaceGeneration++;
    presenter.glStateDirty = true;
  }
  host.canvas.style.width = `${target.logicalWidth}px`;
  host.canvas.style.height = `${target.logicalHeight}px`;
  const gl = presenterGl(presenter);
  const supported = gl.drawingBufferWidth === target.physicalWidth &&
    gl.drawingBufferHeight === target.physicalHeight;
  if (!supported) {
    recordResize(host, "ack", source, {
      terminal: "failed",
      backingWidth: gl.drawingBufferWidth,
      backingHeight: gl.drawingBufferHeight,
      detail: JSON.stringify({ reason: "unsupported-size", requested: target }),
    });
  } else if (changed) {
    recordResize(host, "backing-reset-end", source, {
      detail: `committed=${target.physicalWidth}x${target.physicalHeight}`,
    });
  }
  return supported;
}

function blitExactToDefault(
  presenter: CanvasPresenter,
  source: GpuSurface,
  width: number,
  height: number) {
  const gl = presenterGl(presenter);
  return blitRectToDefault(
    presenter, source,
    0, 0, width, height,
    0, 0, width, height,
    width, height, gl.NEAREST, false);
}

export function consumePresenterGlStateDirty(canvasId: string): boolean {
  const presenter = canvasPresenters.get(canvasId);
  if (!presenter) throw new Error(`Canvas presenter '${canvasId}' is not initialized.`);
  const dirty = presenter.glStateDirty;
  presenter.glStateDirty = false;
  return dirty;
}

export function initializeCanvasPresenter(
  canvasId: string,
  callback: ManagedCanvasPresenter): ReturnType<typeof presenterGlInfo> {
  if (canvasPresenters.has(canvasId)) throw new Error(`Canvas presenter '${canvasId}' already exists.`);
  const canvas = document.getElementById(canvasId);
  if (!(canvas instanceof HTMLCanvasElement)) throw new Error(`Canvas '#${canvasId}' was not found.`);
  const runtime = emscriptenGl();
  const context = runtime.createContext(canvas, {
    alpha: 1, depth: 1, stencil: 8, antialias: 0, premultipliedAlpha: 1,
    preserveDrawingBuffer: 0, preferLowPowerToHighPerformance: 0,
    failIfMajorPerformanceCaveat: 1, majorVersion: 2, minorVersion: 0,
    enableExtensionsByDefault: 1, explicitSwapControl: 0, renderViaOffscreenBackBuffer: 0,
  });
  if (!context) throw new Error("Doroti requires a hardware WebGL2 context; creation failed.");
  const presenter: CanvasPresenter = {
    canvas, callback, context, contextGeneration: 1, contextLossExtension: null,
    current: null, latest: null, drainScheduled: false,
    nextRequestId: 0, contextLost: false, front: null, frontGeneration: 0,
    staging: null, glStateDirty: true, listeners: [],
  };
  presenter.contextLossExtension = presenterGl(presenter).getExtension("WEBGL_lose_context");
  const listen = (name: string, handler: EventListener): void => {
    canvas.addEventListener(name, handler);
    presenter.listeners.push({ target: canvas, name, handler });
  };
  listen("webglcontextlost", (event) => {
    event.preventDefault();
    presenter.contextLost = true;
    releaseGpuSurface(presenter, presenter.front, false);
    releaseGpuSurface(presenter, presenter.staging, false);
    presenter.front = null;
    presenter.staging = null;
    presenter.frontGeneration = 0;
    const interruptedGeneration = presenter.current?.generation ?? 0;
    if (presenter.current) recordPresenterTerminal(presenter, presenter.current, "superseded", "context lost");
    presenter.current = null;
    const host = hostForCanvas(canvas);
    if (host) {
      updateResizeEpoch(host, "webgl-context-lost", host.logicalWidth, host.logicalHeight, true);
      recordResize(host, "context-lost", "doroti-presenter", {
        detail: `interruptedGeneration=${interruptedGeneration}`,
      });
      emit(host);
    }
    void callback.invokeMethodAsync("ContextLost", interruptedGeneration);
  });
  listen("webglcontextrestored", () => {
    presenter.contextLost = false;
    presenter.contextGeneration++;
    const restoredGl = presenterGl(presenter);
    for (const extensionName of restoredGl.getSupportedExtensions() ?? [])
      restoredGl.getExtension(extensionName);
    const host = hostForCanvas(canvas);
    if (host) {
      const epoch = host.resizeEpoch;
      commitObservedResize(
        host, "webgl-context-restored", host.logicalWidth, host.logicalHeight,
        epoch.physicalWidth, epoch.physicalHeight, epoch.devicePixelRatio, true);
      host.surfaceGeneration++;
      host.gpu = gpuIdentity(canvas);
      recordResize(host, "context-restored", "doroti-presenter", {
        backingWidth: canvas.width, backingHeight: canvas.height,
      });
    }
    void callback.invokeMethodAsync("ContextRestored").finally(() => schedulePresenter(presenter));
  });
  canvasPresenters.set(canvasId, presenter);
  return presenterGlInfo(presenter);
}

export function requestPresent(
  canvasId: string, generation: number, logicalWidth: number, logicalHeight: number,
  physicalWidth: number, physicalHeight: number, devicePixelRatio: number,
  timestampMicroseconds: number): void {
  const presenter = canvasPresenters.get(canvasId);
  if (!presenter) throw new Error(`Canvas presenter '${canvasId}' is not initialized.`);
  const descriptor: PresentDescriptor = {
    requestId: ++presenter.nextRequestId, generation, logicalWidth, logicalHeight,
    physicalWidth, physicalHeight, devicePixelRatio, timestampMicroseconds,
    terminalRecorded: false,
  };
  const host = hostForCanvas(presenter.canvas);
  if (host) recordResize(host, "present-requested", "doroti-presenter", {
    rafId: descriptor.requestId,
    surfaceWidth: descriptor.physicalWidth, surfaceHeight: descriptor.physicalHeight,
  });
  if (presenter.latest) recordPresenterTerminal(presenter, presenter.latest, "superseded", "latest replaced");
  presenter.latest = descriptor;
  schedulePresenter(presenter);
}

function schedulePresenter(presenter: CanvasPresenter): void {
  if (presenter.drainScheduled || presenter.current || presenter.contextLost || !presenter.latest) return;
  presenter.drainScheduled = true;
  queueMicrotask(() => {
    presenter.drainScheduled = false;
    void runPresenter(presenter);
  });
}

async function runPresenter(presenter: CanvasPresenter): Promise<void> {
  const descriptor = presenter.latest;
  presenter.latest = null;
  if (!descriptor || presenter.contextLost) return;
  presenter.current = descriptor;
  const host = hostForCanvas(presenter.canvas);
  if (host && host.resizeEpoch.generation !== descriptor.generation) {
    recordPresenterTerminal(presenter, descriptor, "superseded", "target changed before presenter drain");
    presenter.current = null;
    schedulePresenter(presenter);
    return;
  }
  const started = performance.now();
  const gl = presenterGl(presenter);
  const staging = ensureStaging(presenter, descriptor.physicalWidth, descriptor.physicalHeight);
  gl.bindFramebuffer(gl.FRAMEBUFFER, staging.framebuffer);
  gl.drawBuffers([gl.COLOR_ATTACHMENT0]);
  gl.disable(gl.SCISSOR_TEST);
  gl.colorMask(true, true, true, true);
  gl.depthMask(true);
  gl.stencilMask(0xff);
  gl.viewport(0, 0, staging.width, staging.height);
  presenter.glStateDirty = true;
  try {
    const renderResult = String(await presenter.callback.invokeMethodAsync("RenderFrame",
      descriptor.generation, descriptor.logicalWidth, descriptor.logicalHeight,
      descriptor.physicalWidth, descriptor.physicalHeight, descriptor.devicePixelRatio,
      descriptor.timestampMicroseconds, staging.framebufferId, 8, 0));
    const latestHost = hostForCanvas(presenter.canvas);
    const exactRendered = renderResult === "exact-rendered" || renderResult === "replay-rendered";
    if (!latestHost || latestHost.resizeEpoch.generation !== descriptor.generation) {
      await presenter.callback.invokeMethodAsync("CompleteFrame", descriptor.generation, false,
        "target changed during staging raster");
      recordPresenterTerminal(presenter, descriptor, "superseded", "target changed during raster");
    } else if (!exactRendered) {
      await presenter.callback.invokeMethodAsync("CompleteFrame", descriptor.generation, false,
        `managed raster result=${renderResult}`);
      recordPresenterTerminal(presenter, descriptor, "superseded", `managed raster result=${renderResult}`);
    } else {
      if (!commitCanvasEpoch(latestHost, presenter, descriptor, "exact-front-commit"))
        throw new Error(`Doroti canvas rejected exact size ${descriptor.physicalWidth}x${descriptor.physicalHeight}.`);
      const commitStatus = blitExactToDefault(
        presenter, staging, descriptor.physicalWidth, descriptor.physicalHeight);
      if (commitStatus.sourceStatus !== gl.FRAMEBUFFER_COMPLETE ||
          commitStatus.destinationStatus !== gl.FRAMEBUFFER_COMPLETE ||
          commitStatus.priorErrors.length !== 0 || commitStatus.error !== gl.NO_ERROR) {
        throw new Error(`Doroti exact WebGL commit failed: ${JSON.stringify(commitStatus)}`);
      }
      const previousFront = presenter.front;
      staging.logicalWidth = descriptor.logicalWidth;
      staging.logicalHeight = descriptor.logicalHeight;
      staging.devicePixelRatio = descriptor.devicePixelRatio;
      staging.generation = descriptor.generation;
      presenter.front = staging;
      presenter.frontGeneration = descriptor.generation;
      presenter.staging = previousFront;
      await presenter.callback.invokeMethodAsync("CompleteFrame", descriptor.generation, true, "front commit");
      recordResize(latestHost, "front-commit", "doroti-presenter", {
        rafId: descriptor.requestId,
        backingWidth: presenter.canvas.width, backingHeight: presenter.canvas.height,
        surfaceWidth: descriptor.physicalWidth, surfaceHeight: descriptor.physicalHeight,
        detail: JSON.stringify(commitStatus),
      });
      recordResize(latestHost, "browser-present-unverified", "browser-compositor", {
        rafId: descriptor.requestId,
        detail: "GPU blit and rAF completion are not a display scan-out acknowledgement",
      });
      recordPresenterTerminal(presenter, descriptor, "submitted",
        "exact staging GPU surface committed to the default framebuffer",
        Math.round((performance.now() - started) * 1000));
    }
  } catch (error) {
    try {
      await presenter.callback.invokeMethodAsync("CompleteFrame", descriptor.generation, false, String(error));
    } catch { }
    recordPresenterTerminal(presenter, descriptor, "failed", String(error));
  } finally {
    presenter.current = null;
    schedulePresenter(presenter);
  }
}

function recordPresenterTerminal(
  presenter: CanvasPresenter,
  descriptor: PresentDescriptor,
  terminal: string,
  detail: string,
  durationMicroseconds = 0): void {
  if (descriptor.terminalRecorded) return;
  descriptor.terminalRecorded = true;
  const host = hostForCanvas(presenter.canvas);
  if (!host) return;
  recordResize(host, terminal === "submitted" ? "submitted" : "ack", "doroti-presenter", {
    durationMicroseconds, rafId: descriptor.requestId,
    backingWidth: presenter.canvas.width, backingHeight: presenter.canvas.height,
    surfaceWidth: descriptor.physicalWidth, surfaceHeight: descriptor.physicalHeight,
    terminal, detail,
  });
}

export function disposeCanvasPresenter(canvasId: string): void {
  const presenter = canvasPresenters.get(canvasId);
  if (!presenter) return;
  presenter.drainScheduled = false;
  if (presenter.current)
    recordPresenterTerminal(presenter, presenter.current, "superseded", "presenter disposed");
  if (presenter.latest)
    recordPresenterTerminal(presenter, presenter.latest, "superseded", "presenter disposed");
  presenter.current = null;
  presenter.latest = null;
  for (const listener of presenter.listeners)
    listener.target.removeEventListener(listener.name, listener.handler);
  releaseGpuSurface(presenter, presenter.front);
  releaseGpuSurface(presenter, presenter.staging);
  emscriptenGl().deleteContext?.(presenter.context);
  canvasPresenters.delete(canvasId);
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
  managed?.dispatchSnapshot(host.id, snapshot(host));
}

function gpuIdentity(canvas: HTMLCanvasElement): GpuIdentity {
  const presenter = canvasPresenters.get(canvas.id);
  let gl: WebGL2RenderingContext | null = null;
  if (presenter) {
    const runtime = emscriptenGl();
    runtime.makeContextCurrent(presenter.context);
    gl = runtime.currentContext?.GLctx ?? null;
  } else {
    gl = canvas.getContext("webgl2", {
      alpha: true,
      antialias: true,
      depth: true,
      failIfMajorPerformanceCaveat: true,
      premultipliedAlpha: true,
    });
  }
  if (!gl) throw new Error("Doroti requires a hardware WebGL2 canvas; CPU/2D fallback is forbidden.");
  const debug = gl.getExtension("WEBGL_debug_renderer_info");
  const vendor = debug ? gl.getParameter(debug.UNMASKED_VENDOR_WEBGL) : gl.getParameter(gl.VENDOR);
  const renderer = debug ? gl.getParameter(debug.UNMASKED_RENDERER_WEBGL) : gl.getParameter(gl.RENDERER);
  const softwareFallbackUsed = /swiftshader|llvmpipe|software/.test(`${String(vendor)} ${String(renderer)}`.toLowerCase());
  if (softwareFallbackUsed) throw new Error(`Doroti rejected software WebGL renderer '${String(renderer)}'.`);
  return { api: "webgl2", vendor: String(vendor), renderer: String(renderer), hardware: true, softwareFallbackUsed };
}

export function configureManagedCallbacks(callbacks: ManagedCallbacks): void {
  const required: (keyof ManagedCallbacks)[] = [
    "dispatchAnimationFrame", "dispatchSnapshot", "dispatchPointerBatch", "dispatchWheel",
    "dispatchKey", "dispatchFocus", "dispatchTextEditing", "dispatchTextAction", "dispatchSemanticsAction",
  ];
  if (!callbacks || required.some((name) => typeof callbacks[name] !== "function")) {
    throw new Error("Doroti browser managed callback ABI v1 is incomplete.");
  }
  managed = callbacks;
}

export async function initializeManagedCallbacks(): Promise<"ready"> {
  if (managed) return "ready";
  const getDotnetRuntime = (globalThis as typeof globalThis & { getDotnetRuntime?: (index: number) => DotnetRuntime }).getDotnetRuntime;
  const runtime = getDotnetRuntime?.(0);
  if (!runtime) throw new Error("Doroti could not resolve the active Blazor WebAssembly runtime.");
  const exports = await runtime.getAssemblyExports("Doroti.Host.Web.dll") as DorotiAssemblyExports;
  const interop = exports.Doroti.Host.Web.BrowserInterop;
  configureManagedCallbacks({
    dispatchAnimationFrame: interop.DispatchAnimationFrame,
    dispatchSnapshot: interop.DispatchSnapshot,
    dispatchPointerBatch: interop.DispatchPointerBatch,
    dispatchWheel: interop.DispatchWheel,
    dispatchKey: interop.DispatchKey,
    dispatchFocus: interop.DispatchFocus,
    dispatchTextEditing: interop.DispatchTextEditing,
    dispatchTextAction: interop.DispatchTextAction,
    dispatchSemanticsAction: interop.DispatchSemanticsAction,
  });
  return "ready";
}

export function createHost(hostId: number, canvasId: string, logicalWidth: number, logicalHeight: number): string {
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
  const ratio = Math.max(1, globalThis.devicePixelRatio || 1);
  const initialEpoch: ResizeEpoch = {
    generation: 1, logicalWidth, logicalHeight,
    physicalWidth: Math.max(1, Math.round(logicalWidth * ratio)),
    physicalHeight: Math.max(1, Math.round(logicalHeight * ratio)),
    devicePixelRatio: ratio, timestampMicroseconds: Math.round(performance.now() * 1000),
  };
  const host: BrowserHost = {
    id: hostId, root, canvas, input, semantics,
    semanticsElements: new Map(), semanticsListeners: new Map(),
    logicalWidth, logicalHeight,
    generation: 1, surfaceGeneration: 0, resizeGeneration: 1,
    emittedResizeGeneration: 1, resizeEpoch: initialEpoch,
    resizeTrace: [], resizeTraceSequence: 0, dprQuery: null,
    frameRaf: 0, latestFrameCallback: 0,
    gpu: gpuIdentity(canvas), observers: [], listeners: [],
    composing: false, compositionStart: -1, viewFocused: false, pressedKeys: new Map(),
    inputAction: 2, multiline: false,
  };
  hosts.set(hostId, host);
  recordResize(host, "target-observed", "host-initial");
  if (!applyProvisionalEpoch(host, initialEpoch, "host-initial")) {
    hosts.delete(hostId);
    throw new Error("Doroti browser drawing buffer does not support the initial viewport size.");
  }
  const observe = (target: EventTarget, name: string, handler: EventListener): void => {
    target.addEventListener(name, handler);
    host.listeners.push({ target, name, handler });
  };
  observe(document, "visibilitychange", () => emit(host));
  observe(globalThis, "focus", () => emit(host));
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
      root.setPointerCapture(event.pointerId);
      canvas.focus({ preventScroll: true });
    }
    requireManaged().dispatchPointerBatch(host.id, phase, pointerKind(event.pointerType), event.pointerId,
      event.buttons, modifierMask(event), pointerSamples(event));
    if ((phase === 2 || phase === 3) && root.hasPointerCapture(event.pointerId)) root.releasePointerCapture(event.pointerId);
  };
  observe(root, "pointerenter", (event) => pointer(5)(event as PointerEvent));
  observe(root, "pointermove", (event) => {
    const pointerEvent = event as PointerEvent;
    pointer(pointerEvent.buttons ? 0 : 4)(pointerEvent);
  });
  observe(root, "pointerdown", (event) => pointer(1)(event as PointerEvent));
  observe(root, "pointerup", (event) => pointer(2)(event as PointerEvent));
  observe(root, "pointercancel", (event) => pointer(3)(event as PointerEvent));
  observe(root, "pointerleave", (event) => pointer(6)(event as PointerEvent));
  observe(root, "wheel", (event) => {
    const wheel = event as WheelEvent;
    wheel.preventDefault();
    const rect = root.getBoundingClientRect();
    requireManaged().dispatchWheel(host.id, wheel.clientX - rect.left, wheel.clientY - rect.top,
      wheel.deltaX, wheel.deltaY, wheel.timeStamp);
  });
  const belongsToHost = (target: EventTarget | null): boolean =>
    target === root || target === canvas || target === input ||
    (target instanceof Node && (root.contains(target) || semantics.contains(target)));
  observe(document, "keydown", (event) => {
    const key = event as KeyboardEvent;
    if (!host.viewFocused || !belongsToHost(document.activeElement)) return;
    const nativeTextEditing = document.activeElement === input && !input.hidden;
    // The managed KeyData callback has no synchronous handled result, so it
    // cannot decide whether this DOM event should be prevented as Flutter's
    // web engine does. While the native input is active, let it own text,
    // selection, clipboard, and IME keys and report the result through input
    // events. Tab remains a framework focus-traversal key.
    if (nativeTextEditing && key.key !== "Tab") return;
    if (key.key === "Tab" ||
        (!nativeTextEditing && ["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", " "].includes(key.key)))
      key.preventDefault();
    host.pressedKeys.set(key.code, key.key);
    requireManaged().dispatchKey(host.id, true, key.repeat, false, key.code, key.key, key.timeStamp);
  });
  observe(document, "keyup", (event) => {
    const key = event as KeyboardEvent;
    if (!host.pressedKeys.has(key.code)) return;
    host.pressedKeys.delete(key.code);
    requireManaged().dispatchKey(host.id, false, false, false, key.code, key.key, key.timeStamp);
  });
  observe(canvas, "focus", (event) => setViewFocus(host, true, event.timeStamp));
  observe(input, "focus", (event) => setViewFocus(host, true, event.timeStamp));
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
  observe(document, "selectionchange", () => {
    if (document.activeElement === input && !input.hidden) emitText(host);
  });
  observe(input, "keydown", (event) => {
    const key = event as KeyboardEvent;
    if (key.key === "Enter" && !key.shiftKey && (!host.multiline || host.inputAction !== 12)) {
      key.preventDefault();
      requireManaged().dispatchTextAction(host.id, host.inputAction);
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
  armDprWatcher(host);
  return snapshot(host);
}

export function showHost(hostId: number): string {
  const host = requireHost(hostId);
  host.canvas.hidden = false;
  host.canvas.tabIndex = host.canvas.tabIndex < 0 ? 0 : host.canvas.tabIndex;
  host.canvas.focus({ preventScroll: true });
  return snapshot(host);
}

export function requestFocus(hostId: number, focused: boolean): string {
  const host = requireHost(hostId);
  if (focused) host.canvas.focus({ preventScroll: true }); else host.canvas.blur();
  return snapshot(host);
}

export function resizeHost(hostId: number, logicalWidth: number, logicalHeight: number): string {
  const host = requireHost(hostId);
  host.root.style.width = `${logicalWidth}px`;
  host.root.style.height = `${logicalHeight}px`;
  return snapshot(host);
}

export function requestFrame(hostId: number, callbackId: number): void {
  const host = requireHost(hostId);
  host.latestFrameCallback = callbackId;
  if (host.frameRaf !== 0) return;
  host.frameRaf = requestAnimationFrame((timestamp) => {
    host.frameRaf = 0;
    const latest = host.latestFrameCallback;
    host.latestFrameCallback = 0;
    if (hosts.has(hostId) && latest !== 0) managed?.dispatchAnimationFrame(hostId, latest, timestamp);
  });
}

export function recordManagedRaster(
  hostId: number,
  phase: string,
  surfaceWidth: number,
  surfaceHeight: number,
  durationMicroseconds: number): void {
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
  return JSON.stringify(requireHost(hostId).resizeTrace);
}

export function closeHost(hostId: number): void {
  const host = hosts.get(hostId);
  if (!host) return;
  releasePressedKeys(host);
  if (host.frameRaf !== 0) cancelAnimationFrame(host.frameRaf);
  for (const observer of host.observers) observer.disconnect();
  for (const listener of host.listeners) listener.target.removeEventListener(listener.name, listener.handler);
  for (const controller of host.semanticsListeners.values()) controller.abort();
  host.semanticsListeners.clear();
  host.semanticsElements.clear();
  host.semantics.replaceChildren();
  hosts.delete(hostId);
}

export function resolveResourceUrl(relativeUrl: string): string {
  return new URL(relativeUrl, document.baseURI).href;
}

export function setCursor(hostId: number, cursor: string): void {
  requireHost(hostId).canvas.style.cursor = cursor;
}

export function setTextInputState(
  hostId: number, text: string, selectionBase: number, selectionExtent: number,
  inputMode: string, enterKeyHint: string, readOnly: boolean, obscureText: boolean,
  autocapitalize: string, autocorrect: boolean, inputAction: number, multiline: boolean): void {
  const host = requireHost(hostId);
  host.input.inputMode = inputMode as typeof host.input.inputMode;
  host.input.enterKeyHint = enterKeyHint;
  host.input.readOnly = readOnly;
  host.input.autocapitalize = autocapitalize;
  host.input.autocomplete = autocorrect ? "on" : "off";
  host.input.spellcheck = autocorrect;
  host.inputAction = inputAction;
  host.multiline = multiline;
  host.input.style.setProperty("-webkit-text-security", obscureText ? "disc" : "none");
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

  // The managed TextField normally accepts a native edit and immediately
  // publishes it back. Reassigning an identical value or selection here can
  // terminate the browser's live composition, so only apply real changes.
  if (!sameText) host.input.value = text;
  host.input.hidden = false;
  if (!sameText || !sameSelection)
    host.input.setSelectionRange(selectionStart, selectionEnd, selectionDirection);
  if (document.activeElement !== host.input) host.input.focus({ preventScroll: true });
}

export function setCaretRect(hostId: number, left: number, top: number, width: number, height: number): void {
  const input = requireHost(hostId).input;
  input.style.left = `${left}px`;
  input.style.top = `${top}px`;
  input.style.width = `${Math.max(1, width)}px`;
  input.style.height = `${Math.max(1, height)}px`;
  input.focus({ preventScroll: true });
}

export function clearTextInput(hostId: number): void {
  const host = requireHost(hostId);
  host.input.value = "";
  host.input.hidden = true;
  host.canvas.focus({ preventScroll: true });
}

export async function readClipboardText(): Promise<string> {
  if (!navigator.clipboard?.readText) throw new Error("Doroti clipboard read capability is unavailable.");
  return navigator.clipboard.readText();
}

export async function writeClipboardText(text: string): Promise<"written"> {
  if (!navigator.clipboard?.writeText) throw new Error("Doroti clipboard write capability is unavailable.");
  await navigator.clipboard.writeText(text);
  return "written";
}

export function updateSemantics(hostId: number, json: string): void {
  const host = requireHost(hostId);
  const update = JSON.parse(json) as SemanticsUpdate;
  host.semantics.dataset.generation = String(update.generation);
  const nodes = update.nodes ?? [];
  const nodesById = new Map(nodes.map((node) => [Number(node.id), node]));
  const identifiersByValue = new Map(nodes
    .filter((node) => Boolean(node.identifier))
    .map((node) => [node.identifier!, Number(node.id)]));
  const parentById = new Map<number, number>();
  for (const node of nodes) {
    for (const childId of node.children ?? []) {
      if (nodesById.has(childId) && !parentById.has(childId)) parentById.set(childId, Number(node.id));
    }
  }

  const liveIds = new Set<number>();
  for (const node of nodes) {
    const id = Number(node.id);
    liveIds.add(id);
    const tag = semanticsElementTag(node);
    let element = host.semanticsElements.get(id);
    if (!element || element.tagName.toLowerCase() !== tag) {
      const replacement = document.createElement(tag);
      if (element?.parentElement) element.replaceWith(replacement);
      element = replacement;
      host.semanticsElements.set(id, element);
    }
    host.semanticsListeners.get(id)?.abort();
    const listeners = new AbortController();
    host.semanticsListeners.set(id, listeners);
    resetSemanticsAttributes(element);
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
    setOptionalAttribute(element, "aria-controls", node.controlsNodes
      ?.map((identifier) => identifiersByValue.get(identifier))
      .filter((controlledId): controlledId is number => controlledId !== undefined)
      .map((controlledId) => semanticsDomId(host.id, controlledId))
      .join(" "));
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
      element.readOnly = node.flags.readOnly === true;
      if (element instanceof HTMLInputElement) element.type = node.flags.obscured ? "password" : "text";
      element.inputMode = semanticsInputMode(node.inputType);
      if (node.maxValueLength !== undefined && node.maxValueLength >= 0) element.maxLength = node.maxValueLength;
      if (element.value !== (node.value ?? "")) element.value = node.value ?? "";
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

    const parent = nodesById.get(parentById.get(id) ?? Number.NaN);
    const parentLeft = parent?.rect[0] ?? 0;
    const parentTop = parent?.rect[1] ?? 0;
    element.style.position = "absolute";
    element.style.left = `${node.rect[0] - parentLeft}px`;
    element.style.top = `${node.rect[1] - parentTop}px`;
    element.style.width = `${Math.max(0, node.rect[2] - node.rect[0])}px`;
    element.style.height = `${Math.max(0, node.rect[3] - node.rect[1])}px`;
  }

  for (const [id, controller] of host.semanticsListeners) {
    if (liveIds.has(id)) continue;
    controller.abort();
    host.semanticsListeners.delete(id);
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
}

export async function invokePlugin(moduleUrl: string, exportName: string, channel: string, codec: string, payloadBase64: string): Promise<string> {
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

function emitText(host: BrowserHost): void {
  const start = host.input.selectionStart ?? 0;
  const end = host.input.selectionEnd ?? start;
  const selectionBackward = host.input.selectionDirection === "backward";
  const selectionBase = selectionBackward ? end : start;
  const selectionExtent = selectionBackward ? start : end;
  const composingBase = host.composing ? Math.max(0, host.compositionStart) : -1;
  const composingExtent = host.composing ? end : -1;
  requireManaged().dispatchTextEditing(
    host.id, host.input.value, selectionBase, selectionExtent, composingBase, composingExtent);
}

function setViewFocus(host: BrowserHost, focused: boolean, timestamp: number): void {
  if (host.viewFocused === focused) return;
  host.viewFocused = focused;
  requireManaged().dispatchFocus(host.id, focused, timestamp);
  emit(host);
}

function releasePressedKeys(host: BrowserHost): void {
  const timestamp = performance.now();
  for (const [code, key] of host.pressedKeys) {
    requireManaged().dispatchKey(host.id, false, false, true, code, key, timestamp);
  }
  host.pressedKeys.clear();
}

function dispatchSemantics(host: BrowserHost, nodeId: number | string, action: number, args: unknown = null): void {
  requireManaged().dispatchSemanticsAction(host.id, Number(nodeId), action, JSON.stringify(args));
}

function textSelectionOffsets(element: HTMLElement): { base: number; extent: number } | null {
  if (element instanceof HTMLInputElement || element instanceof HTMLTextAreaElement) {
    const base = element.selectionStart;
    const extent = element.selectionEnd;
    return base === null || extent === null ? null : { base, extent };
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

function applySemanticsFlags(element: HTMLElement, flags: SemanticsFlags | undefined, role: string): void {
  if (!flags) return;
  if (flags.hidden) element.setAttribute("aria-hidden", "true");
  if (flags.liveRegion) element.setAttribute("aria-live", "polite");
  if (flags.checked && flags.checked !== "none") element.setAttribute("aria-checked", flags.checked === "mixed" ? "mixed" : String(flags.checked === "isTrue"));
  if (flags.selected !== undefined && flags.selected !== null) {
    element.setAttribute(role === "radio" ? "aria-checked" : "aria-selected", String(flags.selected));
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
