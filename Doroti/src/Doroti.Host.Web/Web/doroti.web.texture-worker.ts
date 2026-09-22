import type { BrowserTextureFrame } from "./doroti.web.textures.js";
import { stagePlatformBitmap } from "./doroti.web.js";

export interface TextureSurface {
  RegisterBrowserTexture(): string;
  MarkBrowserTexture(id: string): void;
  UnregisterBrowserTexture(id: string): void;
  ReleaseBrowserTextureImage(token: number): void;
}
interface Entry {
  generation: number; sourceGeneration: number; sequence: number;
  pending?: BrowserTextureFrame; canvas?: OffscreenCanvas; dirty?: boolean;
  pendingBytes?: number;
  allocations: number;
}
interface Allocation { id: string; bytes: number; handle: number; retiring?: boolean; destroy(): void; }
interface GlTable { textures: (WebGLTexture | null)[]; getNewId(table: (WebGLTexture | null)[]): number; currentContext: { GLctx: WebGL2RenderingContext }; }
let surface: TextureSurface;
let gpu: typeof import("./doroti.webgpu.js") | undefined;
let getGl: () => GlTable;
let getCanvas: () => OffscreenCanvas;
let reportError: (id: string, error: unknown) => void = () => {};
let generation = 0;
let nextToken = 0;
let closed = false;
let lost = false;
let bytes = 0;
let pendingBytes = 0;
let peakBytes = 0;
const entries = new Map<string, Entry>();
const allocations = new Map<number, Allocation>();
const retired = new Set<number>();
const completions = new Set<Promise<void>>();
const counts = { received: 0, accepted: 0, rejected: 0, closed: 0, dropped: 0, imported: 0, drawn: 0, retired: 0, errors: 0 };
const sourceBudget = 16 * 1024 * 1024;
const viewBudget = 64 * 1024 * 1024;

export function initializeTextures(exports: TextureSurface, webgpu: typeof gpu, gl: () => GlTable, canvas?: () => OffscreenCanvas,
  onError?: (id: string, error: unknown) => void): void {
  surface = exports; gpu = webgpu; getGl = gl; getCanvas = canvas!;
  if (onError) reportError = onError;
}
export function captureTextureRaster(order: number, left: number, top: number, width: number, height: number, scaleX: number, scaleY: number): void {
  stagePlatformBitmap(order, left / scaleX, top / scaleY, width / scaleX, height / scaleY, width, height,
    createImageBitmap(getCanvas(), left, top, width, height, { premultiplyAlpha: "premultiply" }));
}
function closeFrame(frame?: BrowserTextureFrame): void { if (frame) { frame.close(); counts.closed++; } }
function clearPending(entry: Entry): void {
  closeFrame(entry.pending); entry.pending = undefined;
  pendingBytes -= entry.pendingBytes ?? 0; entry.pendingBytes = 0;
}
export function rejectTransferred(message: unknown): void {
  const frame = (message as { frame?: unknown } | null)?.frame;
  if (isFrame(frame)) { counts.received++; counts.rejected++; closeFrame(frame); }
}
function positive(value: unknown): number {
  if (typeof value !== "number" || !Number.isSafeInteger(value) || value <= 0) throw new Error("Invalid texture sequence/generation.");
  return value;
}
function requireEntry(message: Record<string, unknown>): [string, Entry] {
  const id = String(message.textureId);
  const entry = entries.get(id);
  if (!entry || entry.generation !== positive(message.generation)) throw new Error("Stale or foreign texture registration.");
  return [id, entry];
}
function isFrame(frame: unknown): frame is BrowserTextureFrame {
  return (typeof VideoFrame !== "undefined" && frame instanceof VideoFrame) ||
    (typeof ImageBitmap !== "undefined" && frame instanceof ImageBitmap);
}
export async function textureMessage(message: Record<string, unknown>): Promise<Record<string, unknown>> {
  const frame = message.frame;
  let retained = false;
  if (isFrame(frame)) counts.received++;
  try {
    positive(message.request);
    if (message.operation === "diagnostics") return diagnostics();
    if (closed || lost) throw new Error("Texture owner unavailable; a new view is required.");
    if (message.operation === "register") {
      if (entries.size >= 16) throw new Error("View registration budget exceeded.");
      const textureId = surface.RegisterBrowserTexture();
      const entry = { generation: ++generation, sourceGeneration: 1, sequence: 0, allocations: 0 };
      entries.set(textureId, entry); return { textureId, generation: entry.generation };
    }
    const [id, entry] = requireEntry(message);
    if (message.operation === "unregister") {
      clearPending(entry); entries.delete(id); surface.UnregisterBrowserTexture(id);
      await flushRetired(); await Promise.all([...completions]); return {};
    }
    if (message.operation === "replace") {
      const sourceGeneration = positive(message.sourceGeneration);
      if (sourceGeneration <= entry.sourceGeneration) throw new Error("Stale source replacement.");
      entry.sourceGeneration = sourceGeneration;
      clearPending(entry); entry.canvas = undefined;
      return {};
    }
    if (message.operation !== "frame" || !isFrame(frame)) throw new Error("Unsupported texture message/source.");
    const sequence = positive(message.sequence);
    if (positive(message.sourceGeneration) !== entry.sourceGeneration || sequence <= entry.sequence)
      throw new Error("Stale source/frame generation.");
    const [width, height] = dimensions(frame); validateSize(width, height);
    const size = width * height * 4;
    if (pendingBytes - (entry.pendingBytes ?? 0) + size > viewBudget) throw new Error("View pending-source budget exceeded.");
    entry.sequence = sequence;
    if (entry.pending) { clearPending(entry); counts.dropped++; }
    entry.pending = frame; entry.pendingBytes = size; pendingBytes += size;
    retained = true; counts.accepted++; surface.MarkBrowserTexture(id);
    return {};
  } catch (error) { counts.errors++; if (isFrame(frame)) counts.rejected++; throw error; }
  finally { if (isFrame(frame) && !retained) closeFrame(frame); }
}
function dimensions(source: BrowserTextureFrame | OffscreenCanvas): [number, number] {
  return typeof VideoFrame !== "undefined" && source instanceof VideoFrame
    ? [source.displayWidth, source.displayHeight] : [(source as ImageBitmap).width, (source as ImageBitmap).height];
}
function validateSize(width: number, height: number): void {
  if (!Number.isSafeInteger(width) || !Number.isSafeInteger(height) || width <= 0 || height <= 0 || width * height * 4 > sourceBudget)
    throw new Error("Texture source dimensions exceed the 16 MiB limit.");
}
/** Owner-local input. Canvas ownership stays here; no transferToImageBitmap clears its backing. */
export function registerLocalCanvas(canvas: OffscreenCanvas): { textureId: string; markFrameAvailable(): void; dispose(): Promise<void> } {
  if (closed || lost || entries.size >= 16) throw new Error("Texture owner unavailable or full.");
  validateSize(canvas.width, canvas.height);
  const textureId = surface.RegisterBrowserTexture();
  const entry: Entry = { generation: ++generation, sourceGeneration: 1, sequence: 0, allocations: 0, canvas, dirty: true };
  entries.set(textureId, entry); surface.MarkBrowserTexture(textureId);
  return { textureId, markFrameAvailable: () => { if (entries.get(textureId) !== entry) return; entry.dirty = true; surface.MarkBrowserTexture(textureId); },
    dispose: async () => { if (!entries.delete(textureId)) return; surface.UnregisterBrowserTexture(textureId); await flushRetired(); } };
}

/** Called only from the current Skia recorder/context, after freeze admission. */
export function acquireTexture(id: string): { token: number; handle: number; width: number; height: number } | null {
  const entry = entries.get(id);
  if (!entry || closed || lost) return null;
  const source = entry.pending ?? (entry.dirty ? entry.canvas : undefined);
  if (!source) return null;
  const [width, height] = dimensions(source); validateSize(width, height);
  const size = width * height * 4;
  if (entry.allocations >= 3 || bytes + size > viewBudget) { counts.dropped++; return null; }
  entry.pending = undefined; entry.dirty = false; pendingBytes -= entry.pendingBytes ?? 0; entry.pendingBytes = 0;
  try {
    const allocation = gpu ? gpu.copyTextureSource(source, width, height) : copyGl(source, width, height);
    const token = ++nextToken;
    allocations.set(token, { ...allocation, bytes: size, id });
    entry.allocations++; bytes += size; peakBytes = Math.max(peakBytes, bytes); counts.imported++;
    return { token, handle: allocation.handle, width, height };
  } catch (error) { counts.errors++; reportError(id, error); return null; }
  finally { if (isFrame(source)) closeFrame(source); }
}
function copyGl(source: BrowserTextureFrame | OffscreenCanvas, width: number, height: number) {
  const table = getGl(); const gl = table.currentContext.GLctx;
  if (gl.isContextLost() || width > gl.getParameter(gl.MAX_TEXTURE_SIZE) || height > gl.getParameter(gl.MAX_TEXTURE_SIZE)) throw new Error("WebGL source exceeds the current context limits.");
  const texture = gl.createTexture(); if (!texture) throw new Error("WebGL texture allocation failed.");
  const binding = gl.getParameter(gl.TEXTURE_BINDING_2D) as WebGLTexture | null;
  const pbo = gl.getParameter(gl.PIXEL_UNPACK_BUFFER_BINDING) as WebGLBuffer | null;
  const unpack = [gl.UNPACK_FLIP_Y_WEBGL, gl.UNPACK_PREMULTIPLY_ALPHA_WEBGL, gl.UNPACK_COLORSPACE_CONVERSION_WEBGL,
    gl.UNPACK_ALIGNMENT, gl.UNPACK_ROW_LENGTH, gl.UNPACK_SKIP_PIXELS, gl.UNPACK_SKIP_ROWS, gl.UNPACK_IMAGE_HEIGHT, gl.UNPACK_SKIP_IMAGES];
  const values = unpack.map(key => gl.getParameter(key) as number);
  try {
    gl.bindBuffer(gl.PIXEL_UNPACK_BUFFER, null); gl.bindTexture(gl.TEXTURE_2D, texture);
    gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, 0); gl.pixelStorei(gl.UNPACK_PREMULTIPLY_ALPHA_WEBGL, 1);
    gl.pixelStorei(gl.UNPACK_COLORSPACE_CONVERSION_WEBGL, gl.BROWSER_DEFAULT_WEBGL);
    gl.pixelStorei(gl.UNPACK_ALIGNMENT, 4);
    for (const key of unpack.slice(4)) gl.pixelStorei(key, 0);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR); gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE); gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA8, gl.RGBA, gl.UNSIGNED_BYTE, source);
    const error = gl.getError(); if (error !== gl.NO_ERROR) throw new Error(`WebGL source upload failed: ${error}`);
    const handle = table.getNewId(table.textures); table.textures[handle] = texture;
    (texture as WebGLTexture & { name: number }).name = handle;
    return { handle, destroy: () => { table.textures[handle] = null; gl.deleteTexture(texture); } };
  } catch (error) { gl.deleteTexture(texture); throw error; }
  finally {
    gl.bindTexture(gl.TEXTURE_2D, binding); gl.bindBuffer(gl.PIXEL_UNPACK_BUFFER, pbo);
    unpack.forEach((key, index) => gl.pixelStorei(key, values[index]));
  }
}
export function retireTexture(token: number): void {
  const allocation = allocations.get(token);
  if (allocation) { allocation.retiring = true; retired.add(token); }
}
export function textureDrawn(token: number): void { if (allocations.has(token)) counts.drawn++; }
function glWorkDone(): Promise<void> {
  const gl = getGl().currentContext.GLctx;
  if (gl.isContextLost()) return Promise.resolve();
  const fence = gl.fenceSync(gl.SYNC_GPU_COMMANDS_COMPLETE, 0);
  if (!fence) throw new Error("WebGL completion fence failed.");
  gl.flush();
  const deadline = performance.now() + 30000;
  return new Promise((resolve, reject) => {
    const poll = (): void => {
      if (gl.isContextLost()) { resolve(); return; }
      const status = gl.clientWaitSync(fence, 0, 0);
      if (status === gl.TIMEOUT_EXPIRED && performance.now() < deadline) { setTimeout(poll, 4); return; }
      gl.deleteSync(fence);
      if (status === gl.WAIT_FAILED || status === gl.TIMEOUT_EXPIRED) reject(new Error("WebGL texture fence failed or timed out; resources remain pinned.")); else resolve();
    }; poll();
  });
}
/** Called after Snap/submit (also canceled recordings), never while recording. */
export function flushRetired(): Promise<void> {
  if (!retired.size) return Promise.resolve();
  const batch = [...retired]; retired.clear();
  const completion = (gpu ? gpu.textureWorkDone() : glWorkDone()).then(() => {
    for (const token of batch) {
      const allocation = allocations.get(token); if (!allocation) continue;
      surface.ReleaseBrowserTextureImage(token);
      allocation.destroy(); allocations.delete(token); bytes -= allocation.bytes; counts.retired++;
      const entry = entries.get(allocation.id);
      if (entry) { entry.allocations--; if (entry.pending || entry.dirty) surface.MarkBrowserTexture(allocation.id); }
    }
  }).catch(error => { for (const token of batch) retired.add(token); throw error; });
  completions.add(completion); void completion.then(() => completions.delete(completion), () => completions.delete(completion));
  return completion;
}
export async function disposeTextures(contextLost = false): Promise<void> {
  closed = true; lost = contextLost;
  for (const [id, entry] of entries) { clearPending(entry); surface.UnregisterBrowserTexture(id); }
  entries.clear(); await flushRetired(); await Promise.all(completions);
}
export function diagnostics(): Record<string, unknown> {
  return { schema: 1, ...counts, registrations: entries.size, pending: [...entries.values()].filter(e => e.pending).length,
    live: allocations.size, gpuBytes: bytes, peakGpuBytes: peakBytes, pendingBytes, retiring: retired.size, completions: completions.size,
    current: [...allocations.values()].filter(allocation => !allocation.retiring).length,
    inFlightRetirements: [...allocations.values()].filter(allocation => allocation.retiring).length,
    sources: [...entries].map(([textureId, entry]) => ({ textureId, generation: entry.generation,
      sourceGeneration: entry.sourceGeneration, sequence: entry.sequence, pending: !!entry.pending, allocations: entry.allocations })),
    sourceBudget, viewBudget, ownerLost: lost, backend: gpu ? "webgpu" : "webgl", gpu: gpu?.diagnostics() ?? null };
}
