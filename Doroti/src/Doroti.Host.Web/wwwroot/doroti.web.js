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
  if (!callbacks || typeof callbacks.dispatchAnimationFrame !== "function" ||
      typeof callbacks.dispatchSnapshot !== "function") {
    throw new Error("Doroti browser managed callback ABI v1 is incomplete.");
  }
  managed = callbacks;
}

export function createHost(hostId, canvasId, logicalWidth, logicalHeight) {
  if (!managed) throw new Error("Doroti browser managed callbacks must be configured before host creation.");
  if (hosts.has(hostId)) throw new Error(`Doroti browser host ${hostId} already exists.`);
  const canvas = document.getElementById(canvasId);
  if (!(canvas instanceof HTMLCanvasElement)) throw new Error(`Canvas '#${canvasId}' was not found.`);
  const host = {
    id: hostId, canvas, logicalWidth, logicalHeight,
    generation: 0, surfaceGeneration: 1,
    gpu: gpuIdentity(canvas), observers: [], listeners: [],
  };
  const observe = (target, name, handler) => {
    target.addEventListener(name, handler);
    host.listeners.push([target, name, handler]);
  };
  observe(document, "visibilitychange", () => emit(host));
  observe(globalThis, "focus", () => emit(host));
  observe(globalThis, "blur", () => emit(host));
  observe(globalThis, "resize", () => { host.surfaceGeneration++; emit(host); });
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
