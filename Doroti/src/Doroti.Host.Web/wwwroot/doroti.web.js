const hosts = new Map();
let managed = null;

function snapshot(host) {
  const ratio = Math.max(1, globalThis.devicePixelRatio || 1);
  host.canvas.width = Math.max(1, Math.round(host.logicalWidth * ratio));
  host.canvas.height = Math.max(1, Math.round(host.logicalHeight * ratio));
  const visible = document.visibilityState !== "hidden";
  const focused = document.hasFocus();
  return JSON.stringify({
    canvasId: host.canvas.id,
    logicalWidth: host.logicalWidth,
    logicalHeight: host.logicalHeight,
    devicePixelRatio: ratio,
    visible,
    focused,
    languageTag: navigator.language || "en-US",
    brightness: globalThis.matchMedia?.("(prefers-color-scheme: dark)").matches ? "dark" : "light",
    generation: ++host.generation,
    surfaceGeneration: host.surfaceGeneration,
    gpu: host.gpu,
  });
}

function emit(host) {
  if (managed) managed.dispatchSnapshot(host.id, snapshot(host));
}

function gpuIdentity(canvas) {
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
  const text = `${vendor} ${renderer}`.toLowerCase();
  const softwareFallbackUsed = /swiftshader|llvmpipe|software/.test(text);
  if (softwareFallbackUsed) throw new Error(`Doroti rejected software WebGL renderer '${renderer}'.`);
  return { api: "webgl2", vendor: String(vendor), renderer: String(renderer), hardware: true, softwareFallbackUsed };
}

export function configureManagedCallbacks(callbacks) {
  const required = ["dispatchAnimationFrame", "dispatchSnapshot", "dispatchPointerBatch",
    "dispatchWheel", "dispatchKey", "dispatchFocus", "dispatchTextEditing", "dispatchTextAction"];
  if (!callbacks || required.some(name => typeof callbacks[name] !== "function")) {
    throw new Error("Doroti browser managed callback ABI v1 is incomplete.");
  }
  managed = callbacks;
}

export async function initializeManagedCallbacks() {
  if (managed) return "ready";
  const runtime = globalThis.getDotnetRuntime?.(0);
  if (!runtime) throw new Error("Doroti could not resolve the active Blazor WebAssembly runtime.");
  const exports = await runtime.getAssemblyExports("Doroti.Host.Web.dll");
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
  });
  return "ready";
}

export function createHost(hostId, canvasId, logicalWidth, logicalHeight) {
  if (!managed) throw new Error("Doroti browser managed callbacks must be configured before host creation.");
  if (hosts.has(hostId)) throw new Error(`Doroti browser host ${hostId} already exists.`);
  const canvas = document.getElementById(canvasId);
  if (!(canvas instanceof HTMLCanvasElement)) throw new Error(`Canvas '#${canvasId}' was not found.`);
  const input = document.getElementById("doroti-ime");
  const semantics = document.getElementById("doroti-semantics");
  if (!(input instanceof HTMLTextAreaElement)) throw new Error("Doroti hidden text input was not found.");
  if (!(semantics instanceof HTMLElement)) throw new Error("Doroti semantics host was not found.");
  const host = {
    id: hostId, canvas, input, semantics, logicalWidth, logicalHeight,
    generation: 0, surfaceGeneration: 1,
    gpu: gpuIdentity(canvas), observers: [], listeners: [],
    composing: false, compositionStart: -1,
  };
  const observe = (target, name, handler) => {
    target.addEventListener(name, handler);
    host.listeners.push([target, name, handler]);
  };
  observe(document, "visibilitychange", () => emit(host));
  observe(globalThis, "focus", () => emit(host));
  observe(globalThis, "blur", () => emit(host));
  observe(globalThis, "resize", () => { host.surfaceGeneration++; emit(host); });
  const pointerKind = type => type === "touch" ? 1 : type === "pen" ? 2 : 0;
  const pointerSamples = event => {
    const source = event.type === "pointermove" && typeof event.getCoalescedEvents === "function"
      ? event.getCoalescedEvents() : [event];
    const samples = [];
    for (const point of source.length ? source : [event]) {
      samples.push(point.offsetX, point.offsetY, point.pressure || (point.buttons ? 0.5 : 0),
        point.tiltX || 0, point.tiltY || 0, point.twist || 0, point.timeStamp);
    }
    return samples;
  };
  const pointer = phase => event => {
    event.preventDefault();
    if (phase === 1) canvas.setPointerCapture(event.pointerId);
    managed.dispatchPointerBatch(host.id, phase, pointerKind(event.pointerType), event.pointerId,
      event.buttons, modifierMask(event), pointerSamples(event));
    if ((phase === 2 || phase === 3) && canvas.hasPointerCapture(event.pointerId))
      canvas.releasePointerCapture(event.pointerId);
  };
  observe(canvas, "pointermove", pointer(0));
  observe(canvas, "pointerdown", pointer(1));
  observe(canvas, "pointerup", pointer(2));
  observe(canvas, "pointercancel", pointer(3));
  observe(canvas, "wheel", event => {
    event.preventDefault();
    managed.dispatchWheel(host.id, event.offsetX, event.offsetY, event.deltaX, event.deltaY, event.timeStamp);
  });
  observe(canvas, "keydown", event => {
    if (["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", " ", "Tab"].includes(event.key)) event.preventDefault();
    managed.dispatchKey(host.id, true, event.repeat, event.code, event.key, event.timeStamp);
  });
  observe(canvas, "keyup", event => managed.dispatchKey(host.id, false, false, event.code, event.key, event.timeStamp));
  observe(canvas, "focus", event => managed.dispatchFocus(host.id, true, event.timeStamp));
  observe(canvas, "blur", event => managed.dispatchFocus(host.id, false, event.timeStamp));
  observe(input, "compositionstart", () => { host.composing = true; host.compositionStart = input.selectionStart; emitText(host); });
  observe(input, "compositionupdate", () => emitText(host));
  observe(input, "compositionend", () => { host.composing = false; host.compositionStart = -1; emitText(host); });
  observe(input, "input", () => emitText(host));
  observe(input, "keydown", event => {
    if (event.key === "Enter" && !event.shiftKey) managed.dispatchTextAction(host.id, 1);
  });
  observe(canvas, "webglcontextlost", event => { event.preventDefault(); host.surfaceGeneration++; emit(host); });
  observe(canvas, "webglcontextrestored", () => { host.surfaceGeneration++; host.gpu = gpuIdentity(canvas); emit(host); });
  if (globalThis.ResizeObserver) {
    const observer = new ResizeObserver(entries => {
      const rect = entries[0]?.contentRect;
      if (rect && rect.width > 0 && rect.height > 0 &&
          (rect.width !== host.logicalWidth || rect.height !== host.logicalHeight)) {
        host.logicalWidth = rect.width;
        host.logicalHeight = rect.height;
        host.surfaceGeneration++;
        emit(host);
      }
    });
    observer.observe(canvas);
    host.observers.push(observer);
  }
  hosts.set(hostId, host);
  return snapshot(host);
}

export function showHost(hostId) {
  const host = requireHost(hostId);
  host.canvas.hidden = false;
  host.canvas.tabIndex = host.canvas.tabIndex < 0 ? 0 : host.canvas.tabIndex;
  host.canvas.focus({ preventScroll: true });
  return snapshot(host);
}

export function resizeHost(hostId, logicalWidth, logicalHeight) {
  const host = requireHost(hostId);
  host.logicalWidth = logicalWidth;
  host.logicalHeight = logicalHeight;
  host.canvas.style.width = `${logicalWidth}px`;
  host.canvas.style.height = `${logicalHeight}px`;
  host.surfaceGeneration++;
  return snapshot(host);
}

export function requestFrame(hostId, callbackId) {
  requireHost(hostId);
  requestAnimationFrame(timestamp => {
    if (hosts.has(hostId) && managed) managed.dispatchAnimationFrame(hostId, callbackId, timestamp);
  });
}

export function closeHost(hostId) {
  const host = hosts.get(hostId);
  if (!host) return;
  for (const observer of host.observers) observer.disconnect();
  for (const [target, name, handler] of host.listeners) target.removeEventListener(name, handler);
  hosts.delete(hostId);
}

export function resolveResourceUrl(relativeUrl) {
  return new URL(relativeUrl, document.baseURI).href;
}

export function setCursor(hostId, cursor) {
  requireHost(hostId).canvas.style.cursor = cursor;
}

export function setTextInputState(hostId, text, selectionBase, selectionExtent) {
  const host = requireHost(hostId);
  host.input.value = text;
  host.input.hidden = false;
  host.input.setSelectionRange(selectionBase, selectionExtent);
  host.input.focus({ preventScroll: true });
}

export function setCaretRect(hostId, left, top, width, height) {
  const input = requireHost(hostId).input;
  input.style.left = `${left}px`;
  input.style.top = `${top}px`;
  input.style.width = `${Math.max(1, width)}px`;
  input.style.height = `${Math.max(1, height)}px`;
  input.focus({ preventScroll: true });
}

export function clearTextInput(hostId) {
  const host = requireHost(hostId);
  host.input.value = "";
  host.input.hidden = true;
  host.canvas.focus({ preventScroll: true });
}

export async function readClipboardText() {
  if (!navigator.clipboard?.readText) throw new Error("Doroti clipboard read capability is unavailable.");
  return await navigator.clipboard.readText();
}

export async function writeClipboardText(text) {
  if (!navigator.clipboard?.writeText) throw new Error("Doroti clipboard write capability is unavailable.");
  await navigator.clipboard.writeText(text);
  return "written";
}

export function updateSemantics(hostId, json) {
  const host = requireHost(hostId);
  const update = JSON.parse(json);
  host.semantics.replaceChildren();
  host.semantics.dataset.generation = String(update.generation);
  for (const node of update.nodes || []) {
    const element = document.createElement("div");
    element.dataset.dorotiSemanticsId = String(node.id);
    element.setAttribute("role", semanticsRole(node.role));
    if (node.label) element.setAttribute("aria-label", node.label);
    if (node.value) element.setAttribute("aria-valuetext", node.value);
    element.style.position = "absolute";
    element.style.left = `${node.rect[0]}px`;
    element.style.top = `${node.rect[1]}px`;
    element.style.width = `${Math.max(0, node.rect[2] - node.rect[0])}px`;
    element.style.height = `${Math.max(0, node.rect[3] - node.rect[1])}px`;
    host.semantics.append(element);
  }
}

export async function invokePlugin(moduleUrl, exportName, channel, codec, payloadBase64) {
  const resolved = new URL(moduleUrl, document.baseURI).href;
  const pluginModule = await import(resolved);
  const handler = pluginModule[exportName];
  if (typeof handler !== "function") {
    throw new Error(`Doroti JavaScript plugin export '${exportName}' is missing from '${resolved}'.`);
  }
  const value = await handler({ channel, codec, payloadBase64 });
  if (value === null || value === undefined) return JSON.stringify({ hasValue: false, base64: "" });
  if (typeof value === "string") return JSON.stringify({ hasValue: true, base64: value });
  if (value instanceof Uint8Array) {
    let binary = "";
    for (const byte of value) binary += String.fromCharCode(byte);
    return JSON.stringify({ hasValue: true, base64: btoa(binary) });
  }
  throw new Error(`Doroti JavaScript plugin '${exportName}' returned an unsupported response type.`);
}

function requireHost(hostId) {
  const host = hosts.get(hostId);
  if (!host) throw new Error(`Doroti browser host ${hostId} is not active.`);
  return host;
}

function modifierMask(event) {
  return (event.shiftKey ? 1 : 0) | (event.ctrlKey ? 2 : 0) |
    (event.altKey ? 4 : 0) | (event.metaKey ? 8 : 0);
}

function emitText(host) {
  const start = host.input.selectionStart ?? 0;
  const end = host.input.selectionEnd ?? start;
  const composingBase = host.composing ? Math.max(0, host.compositionStart) : -1;
  const composingExtent = host.composing ? end : -1;
  managed.dispatchTextEditing(host.id, host.input.value, start, end, composingBase, composingExtent);
}

function semanticsRole(role) {
  const key = String(role || "").toLowerCase();
  if (key.includes("button")) return "button";
  if (key.includes("textfield")) return "textbox";
  if (key.includes("slider")) return "slider";
  if (key.includes("listitem")) return "listitem";
  if (key.includes("list")) return "list";
  if (key.includes("image")) return "img";
  return "group";
}
