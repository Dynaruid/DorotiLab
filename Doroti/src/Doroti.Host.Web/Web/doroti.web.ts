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
  logicalWidth: number;
  logicalHeight: number;
  generation: number;
  surfaceGeneration: number;
  resizeGeneration: number;
  resizeEpoch: ResizeEpoch;
  resizeTrace: ResizeTraceEntry[];
  resizeTraceSequence: number;
  dprCheckRaf: number;
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
}

interface SkiaSharpCanvasRuntime {
  requestAnimationFrame(renderLoop?: boolean, width?: number, height?: number): void;
  renderFrameCallback: (() => void) | { invokeMethod(name: string): void };
  dorotiResizeContinuity?: {
    requestAnimationFrame: SkiaSharpCanvasRuntime["requestAnimationFrame"];
    renderFrameCallback: SkiaSharpCanvasRuntime["renderFrameCallback"];
    nextRafId: number;
  };
}

type SkiaSharpCanvasElement = HTMLCanvasElement & { SKHtmlCanvas?: SkiaSharpCanvasRuntime };

interface SemanticsFlags {
  checked?: string; selected?: boolean; enabled?: boolean; toggled?: boolean;
  expanded?: boolean; required?: boolean; focused?: boolean; button?: boolean;
  textField?: boolean; header?: boolean; hidden?: boolean; image?: boolean;
  liveRegion?: boolean; multiline?: boolean; readOnly?: boolean; link?: boolean; slider?: boolean;
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
let managed: ManagedCallbacks | null = null;

function snapshot(host: BrowserHost): string {
  const ratio = Math.max(1, globalThis.devicePixelRatio || 1);
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
    generation: ++host.generation,
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
  });
  if (host.resizeTrace.length > 4096) host.resizeTrace.splice(0, host.resizeTrace.length - 4096);
}

function updateResizeEpoch(
  host: BrowserHost,
  source: string,
  logicalWidth: number,
  logicalHeight: number,
  forceGeneration = false): boolean {
  const ratio = Math.max(1, globalThis.devicePixelRatio || 1);
  const physicalWidth = Math.max(1, Math.round(logicalWidth * ratio));
  const physicalHeight = Math.max(1, Math.round(logicalHeight * ratio));
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
  host.surfaceGeneration++;
  host.resizeEpoch = {
    generation: ++host.resizeGeneration,
    logicalWidth,
    logicalHeight,
    physicalWidth,
    physicalHeight,
    devicePixelRatio: ratio,
    timestampMicroseconds: Math.round(performance.now() * 1000),
  };
  recordResize(host, "target", source);
  return true;
}

function hostForCanvas(canvas: HTMLCanvasElement): BrowserHost | undefined {
  for (const host of hosts.values()) if (host.canvas === canvas) return host;
  return undefined;
}

export function installCanvasResizeContinuity(canvasId: string): void {
  const canvas = document.getElementById(canvasId) as SkiaSharpCanvasElement | null;
  const runtime = canvas?.SKHtmlCanvas;
  if (!canvas || !runtime) throw new Error(`SkiaSharp canvas runtime '#${canvasId}' is not initialized.`);
  if (runtime.dorotiResizeContinuity) return;

  const originalRequest = runtime.requestAnimationFrame.bind(runtime);
  const originalCallback = runtime.renderFrameCallback;
  let pendingWidth = 0;
  let pendingHeight = 0;
  runtime.dorotiResizeContinuity = {
    requestAnimationFrame: originalRequest,
    renderFrameCallback: originalCallback,
    nextRafId: 0,
  };
  runtime.requestAnimationFrame = (renderLoop?: boolean, width?: number, height?: number): void => {
    const host = hostForCanvas(canvas);
    const rafId = ++runtime.dorotiResizeContinuity!.nextRafId;
    if (width && height) {
      pendingWidth = width;
      pendingHeight = height;
      if (host) recordResize(host, "size-signal", "skia-watcher", {
        rafId, backingWidth: canvas.width, backingHeight: canvas.height,
        surfaceWidth: width, surfaceHeight: height,
      });
    }
    // SkiaSharp normally changes canvas.width/height here, which clears the
    // WebGL backing store before the requested animation frame can repaint it.
    originalRequest(renderLoop);
  };
  runtime.renderFrameCallback = (): void => {
    const started = performance.now();
    const host = hostForCanvas(canvas);
    const rafId = runtime.dorotiResizeContinuity?.nextRafId ?? 0;
    if (pendingWidth > 0 && pendingHeight > 0) {
      if (canvas.width !== pendingWidth) canvas.width = pendingWidth;
      if (canvas.height !== pendingHeight) canvas.height = pendingHeight;
      pendingWidth = 0;
      pendingHeight = 0;
      if (host) recordResize(host, "backing-store", "backing-store", {
        rafId, backingWidth: canvas.width, backingHeight: canvas.height,
      });
    }
    if (typeof originalCallback === "function") originalCallback.call(runtime);
    else originalCallback.invokeMethod("Invoke");
    if (host) {
      recordResize(host, "submitted", "browser-rAF", {
        durationMicroseconds: Math.round((performance.now() - started) * 1000),
        rafId, backingWidth: canvas.width, backingHeight: canvas.height,
        terminal: "submitted",
        detail: "browser callback return; not a display scan-out acknowledgement",
      });
    }
  };
}

export function uninstallCanvasResizeContinuity(canvasId: string): void {
  const runtime = (document.getElementById(canvasId) as SkiaSharpCanvasElement | null)?.SKHtmlCanvas;
  const registration = runtime?.dorotiResizeContinuity;
  if (!runtime || !registration) return;
  runtime.requestAnimationFrame = registration.requestAnimationFrame;
  runtime.renderFrameCallback = registration.renderFrameCallback;
  delete runtime.dorotiResizeContinuity;
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
  const gl = canvas.getContext("webgl2", {
    alpha: true,
    antialias: true,
    depth: true,
    failIfMajorPerformanceCaveat: true,
    premultipliedAlpha: true,
  });
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
    id: hostId, root, canvas, input, semantics, logicalWidth, logicalHeight,
    generation: 0, surfaceGeneration: 1, resizeGeneration: 1, resizeEpoch: initialEpoch,
    resizeTrace: [], resizeTraceSequence: 0, dprCheckRaf: 0,
    gpu: gpuIdentity(canvas), observers: [], listeners: [],
    composing: false, compositionStart: -1, viewFocused: false, pressedKeys: new Map(),
    inputAction: 2, multiline: false,
  };
  recordResize(host, "target", "host-initial");
  const observe = (target: EventTarget, name: string, handler: EventListener): void => {
    target.addEventListener(name, handler);
    host.listeners.push({ target, name, handler });
  };
  observe(document, "visibilitychange", () => emit(host));
  observe(globalThis, "focus", () => emit(host));
  observe(globalThis, "blur", () => { releasePressedKeys(host); setViewFocus(host, false, performance.now()); emit(host); });
  observe(globalThis, "resize", () => {
    if (host.dprCheckRaf !== 0) return;
    host.dprCheckRaf = requestAnimationFrame(() => {
      host.dprCheckRaf = 0;
      const rect = host.root.getBoundingClientRect();
      if (updateResizeEpoch(host, "window-resize-dpr-signal", rect.width, rect.height)) emit(host);
    });
  });
  observe(globalThis, "languagechange", () => emit(host));
  const colorScheme = globalThis.matchMedia?.("(prefers-color-scheme: dark)");
  if (colorScheme) observe(colorScheme, "change", () => emit(host));

  const pointerKind = (type: string): number => type === "touch" ? 1 : type === "pen" ? 2 : 0;
  const pointerSamples = (event: PointerEvent): number[] => {
    const source = event.type === "pointermove" && typeof event.getCoalescedEvents === "function"
      ? event.getCoalescedEvents() : [event];
    const samples: number[] = [];
    for (const point of source.length ? source : [event]) {
      samples.push(point.offsetX, point.offsetY, point.pressure || (point.buttons ? 0.5 : 0),
        point.tiltX || 0, point.tiltY || 0, point.twist || 0, point.timeStamp);
    }
    return samples;
  };
  const pointer = (phase: number) => (event: PointerEvent): void => {
    event.preventDefault();
    if (phase === 1) canvas.setPointerCapture(event.pointerId);
    requireManaged().dispatchPointerBatch(host.id, phase, pointerKind(event.pointerType), event.pointerId,
      event.buttons, modifierMask(event), pointerSamples(event));
    if ((phase === 2 || phase === 3) && canvas.hasPointerCapture(event.pointerId)) canvas.releasePointerCapture(event.pointerId);
  };
  observe(canvas, "pointerenter", (event) => pointer(5)(event as PointerEvent));
  observe(canvas, "pointermove", (event) => {
    const pointerEvent = event as PointerEvent;
    pointer(pointerEvent.buttons ? 0 : 4)(pointerEvent);
  });
  observe(canvas, "pointerdown", (event) => pointer(1)(event as PointerEvent));
  observe(canvas, "pointerup", (event) => pointer(2)(event as PointerEvent));
  observe(canvas, "pointercancel", (event) => pointer(3)(event as PointerEvent));
  observe(canvas, "pointerleave", (event) => pointer(6)(event as PointerEvent));
  observe(canvas, "wheel", (event) => {
    const wheel = event as WheelEvent;
    wheel.preventDefault();
    requireManaged().dispatchWheel(host.id, wheel.offsetX, wheel.offsetY, wheel.deltaX, wheel.deltaY, wheel.timeStamp);
  });
  const belongsToHost = (target: EventTarget | null): boolean =>
    target === canvas || target === input || (target instanceof Node && semantics.contains(target));
  observe(document, "keydown", (event) => {
    const key = event as KeyboardEvent;
    if (!host.viewFocused || !belongsToHost(document.activeElement)) return;
    if (["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", " ", "Tab"].includes(key.key)) key.preventDefault();
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
  observe(input, "keydown", (event) => {
    const key = event as KeyboardEvent;
    if (key.key === "Enter" && !key.shiftKey && (!host.multiline || host.inputAction !== 12)) {
      key.preventDefault();
      requireManaged().dispatchTextAction(host.id, host.inputAction);
    }
  });
  observe(canvas, "webglcontextlost", (event) => {
    event.preventDefault();
    updateResizeEpoch(host, "webgl-context-lost", host.logicalWidth, host.logicalHeight, true);
    recordResize(host, "context-lost", "webgl-context-lost", { terminal: "failed" });
    emit(host);
  });
  observe(canvas, "webglcontextrestored", () => {
    updateResizeEpoch(host, "webgl-context-restored", host.logicalWidth, host.logicalHeight, true);
    host.gpu = gpuIdentity(canvas);
    recordResize(host, "context-restored", "webgl-context-restored");
    emit(host);
  });
  if (globalThis.ResizeObserver) {
    const observer = new ResizeObserver((entries) => {
      const rect = entries[0]?.contentRect;
      if (rect && rect.width > 0 && rect.height > 0 &&
          updateResizeEpoch(host, "host-observer", rect.width, rect.height)) emit(host);
    });
    observer.observe(root);
    host.observers.push(observer);
  }
  hosts.set(hostId, host);
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
  updateResizeEpoch(host, "inline-style", logicalWidth, logicalHeight);
  return snapshot(host);
}

export function requestFrame(hostId: number, callbackId: number): void {
  requireHost(hostId);
  requestAnimationFrame((timestamp) => {
    if (hosts.has(hostId)) managed?.dispatchAnimationFrame(hostId, callbackId, timestamp);
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
  if (host.dprCheckRaf !== 0) cancelAnimationFrame(host.dprCheckRaf);
  for (const observer of host.observers) observer.disconnect();
  for (const listener of host.listeners) listener.target.removeEventListener(listener.name, listener.handler);
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
  host.input.value = text;
  host.input.hidden = false;
  host.input.setSelectionRange(selectionBase, selectionExtent);
  host.input.focus({ preventScroll: true });
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
  host.semantics.replaceChildren();
  host.semantics.dataset.generation = String(update.generation);
  const elements = new Map<number, HTMLElement>();
  for (const node of update.nodes ?? []) {
    const element = document.createElement("div");
    element.dataset.dorotiSemanticsId = String(node.id);
    element.setAttribute("role", semanticsRole(node.role));
    if (node.label) element.setAttribute("aria-label", node.label);
    if (node.value) element.setAttribute("aria-valuetext", node.value);
    applySemanticsFlags(element, node.flags);
    const actions = node.actions ?? 0;
    if (actions !== 0) {
      element.tabIndex = 0;
      // The semantics tree is an accessibility projection over the canvas. It must not
      // steal ordinary mouse/touch input from Flutter's pointer/gesture pipeline.
      element.style.pointerEvents = "none";
      if ((actions & 1) !== 0) {
        element.addEventListener("click", (event) => {
          event.stopPropagation();
          dispatchSemantics(host, node.id, 1);
        });
        element.addEventListener("keydown", (event) => {
          const key = event as KeyboardEvent;
          if (key.key !== "Enter" && key.key !== " ") return;
          key.preventDefault();
          key.stopPropagation();
          dispatchSemantics(host, node.id, 1);
        });
      }
      if ((actions & (1 << 6)) !== 0) element.addEventListener("keydown", (event) => {
        if ((event as KeyboardEvent).key === "ArrowUp") dispatchSemantics(host, node.id, 1 << 6);
      });
      if ((actions & (1 << 7)) !== 0) element.addEventListener("keydown", (event) => {
        if ((event as KeyboardEvent).key === "ArrowDown") dispatchSemantics(host, node.id, 1 << 7);
      });
      element.addEventListener("focus", () => {
        if ((actions & (1 << 22)) !== 0) dispatchSemantics(host, node.id, 1 << 22);
      });
    }
    if (node.flags?.textField) {
      element.setAttribute("contenteditable", node.flags.readOnly ? "false" : "true");
      element.textContent = node.value ?? "";
      element.addEventListener("input", () => dispatchSemantics(host, node.id, 1 << 21, element.textContent ?? ""));
      const selection = () => {
        if ((actions & (1 << 11)) === 0) return;
        const offsets = textSelectionOffsets(element);
        if (offsets) dispatchSemantics(host, node.id, 1 << 11, offsets);
      };
      element.addEventListener("keyup", selection);
      element.addEventListener("mouseup", selection);
      if ((actions & (1 << 12)) !== 0) element.addEventListener("copy", () => dispatchSemantics(host, node.id, 1 << 12));
      if ((actions & (1 << 13)) !== 0) element.addEventListener("cut", () => dispatchSemantics(host, node.id, 1 << 13));
      if ((actions & (1 << 14)) !== 0) element.addEventListener("paste", () => dispatchSemantics(host, node.id, 1 << 14));
    }
    if ((actions & (1 << 18)) !== 0) element.addEventListener("keydown", (event) => {
      if ((event as KeyboardEvent).key === "Escape") dispatchSemantics(host, node.id, 1 << 18);
    });
    element.style.position = "absolute";
    element.style.left = `${node.rect[0]}px`;
    element.style.top = `${node.rect[1]}px`;
    element.style.width = `${Math.max(0, node.rect[2] - node.rect[0])}px`;
    element.style.height = `${Math.max(0, node.rect[3] - node.rect[1])}px`;
    elements.set(Number(node.id), element);
  }
  const childIds = new Set<number>();
  for (const node of update.nodes ?? []) {
    const parent = elements.get(Number(node.id));
    if (!parent) continue;
    for (const childId of node.children ?? []) {
      const child = elements.get(childId);
      if (child) { parent.append(child); childIds.add(childId); }
    }
  }
  for (const [id, element] of elements) {
    if (!childIds.has(id)) host.semantics.append(element);
  }
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
  const composingBase = host.composing ? Math.max(0, host.compositionStart) : -1;
  const composingExtent = host.composing ? end : -1;
  requireManaged().dispatchTextEditing(host.id, host.input.value, start, end, composingBase, composingExtent);
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

function applySemanticsFlags(element: HTMLElement, flags: SemanticsFlags | undefined): void {
  if (!flags) return;
  if (flags.hidden) element.setAttribute("aria-hidden", "true");
  if (flags.liveRegion) element.setAttribute("aria-live", "polite");
  if (flags.checked && flags.checked !== "none") element.setAttribute("aria-checked", flags.checked === "mixed" ? "mixed" : String(flags.checked === "isTrue"));
  if (flags.selected !== undefined && flags.selected !== null) element.setAttribute("aria-selected", String(flags.selected));
  if (flags.enabled === false) element.setAttribute("aria-disabled", "true");
  if (flags.toggled !== undefined && flags.toggled !== null) element.setAttribute("aria-pressed", String(flags.toggled));
  if (flags.expanded !== undefined && flags.expanded !== null) element.setAttribute("aria-expanded", String(flags.expanded));
  if (flags.required !== undefined && flags.required !== null) element.setAttribute("aria-required", String(flags.required));
  if (flags.multiline) element.setAttribute("aria-multiline", "true");
  if (flags.readOnly) element.setAttribute("aria-readonly", "true");
}

function semanticsRole(role: string | undefined): string {
  const key = String(role ?? "").toLowerCase();
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
  if (key.includes("button")) return "button";
  if (key.includes("textfield")) return "textbox";
  if (key.includes("slider")) return "slider";
  if (key.includes("listitem")) return "listitem";
  if (key.includes("list")) return "list";
  if (key.includes("image")) return "img";
  if (key.includes("status")) return "status";
  if (key.includes("alert")) return "alert";
  if (key.includes("form")) return "form";
  if (key.includes("region")) return "region";
  return "group";
}
