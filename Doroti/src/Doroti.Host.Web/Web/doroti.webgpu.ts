interface NativeDawn {
  dorotiWebGpuMappedRanges?: { active: number; discarded: number };
  WebGPU: {
    importJsDevice(value: GPUDevice, parent: number): number;
    importJsQueue(value: GPUQueue, parent: number): number;
    importJsTexture(value: GPUTexture): number;
  };
  _wgpuCreateInstance(descriptor: number): number;
  _wgpuTextureRelease(handle: number): void;
  _wgpuDeviceRelease(handle: number): void;
  _wgpuQueueRelease(handle: number): void;
  _wgpuInstanceRelease(handle: number): void;
}

let native: NativeDawn | undefined;
let device: GPUDevice | undefined;
let canvas: OffscreenCanvas | undefined;
let context: GPUCanvasContext | undefined;
let handles: { instance: number; device: number; queue: number } | undefined;
let disposed = false;
let failure: Error | undefined;
let reportFailure: (error: Error) => void;
const pending = new Set<Promise<void>>();
// Queue completion does not imply completion of staging-buffer mapAsync.
// Observe buffers created by this owned device only, preserving the original
// promise and rejection delivery to Dawn's native callback.
const pendingMaps = new Set<Promise<void>>();
let submittedFrames = 0;
let completedSubmissions = 0;

function fail(value: unknown): void {
  if (disposed || failure) return;
  // GPUError is not a JavaScript Error; String(value) loses its validation message.
  failure = value instanceof Error ? value : new Error(
    value && typeof value === "object" && "message" in value ? String(value.message) : String(value));
  reportFailure(failure);
}

export async function initialize(target: OffscreenCanvas, onFailure: (error: Error) => void) {
  reportFailure = onFailure;
  if (!isSecureContext || !crossOriginIsolated || !navigator.gpu)
    throw new Error("Doroti WebGPU requires a secure isolated origin and navigator.gpu.");
  const adapter = await navigator.gpu.requestAdapter();
  if (!adapter) throw new Error("Doroti WebGPU adapter is unavailable.");
  device = await adapter.requestDevice();
  const createBuffer = device.createBuffer.bind(device);
  device.createBuffer = descriptor => {
    const buffer = createBuffer(descriptor);
    const mapAsync = buffer.mapAsync.bind(buffer);
    buffer.mapAsync = (mode, offset, size) => {
      const result = mapAsync(mode, offset, size);
      const completion = result.then(() => {}, () => {});
      pendingMaps.add(completion);
      void completion.then(() => pendingMaps.delete(completion));
      return result;
    };
    return buffer;
  };
  device.addEventListener("uncapturederror", event => fail(event.error));
  void device.lost.then(info => { if (!disposed) fail(new Error(`WebGPU device lost: ${info.reason}: ${info.message}`)); });
  canvas = target;
  context = canvas.getContext("webgpu") as GPUCanvasContext | null ?? undefined;
  if (!context) throw new Error("Doroti requires an OffscreenCanvas WebGPU context.");
  // Graphite destination reads (blends/backdrop effects) sample the current
  // texture, and snapshots can copy it. RenderAttachment alone is insufficient.
  const textureUsage = (globalThis as unknown as { GPUTextureUsage: {
    RENDER_ATTACHMENT: number; TEXTURE_BINDING: number; COPY_SRC: number; COPY_DST: number;
  } }).GPUTextureUsage;
  context.configure({ device, format: "bgra8unorm", alphaMode: "premultiplied", colorSpace: "srgb",
    usage: textureUsage.RENDER_ATTACHMENT | textureUsage.TEXTURE_BINDING |
      textureUsage.COPY_SRC | textureUsage.COPY_DST });
  return { api: "webgpu", vendor: adapter.info.vendor,
    renderer: `${adapter.info.architecture} ${adapter.info.device} ${adapter.info.description}`.trim(),
    hardware: !adapter.info.isFallbackAdapter, softwareFallbackUsed: adapter.info.isFallbackAdapter };
}

export function attachNative(module: NativeDawn) {
  native = module;
  for (const name of ["_wgpuCreateInstance", "_wgpuTextureRelease", "_wgpuDeviceRelease", "_wgpuQueueRelease", "_wgpuInstanceRelease"] as const)
    if (typeof native[name] !== "function") throw new Error(`Doroti WebGPU missing ${name}.`);
  if (!device || !native.WebGPU?.importJsDevice || !native.WebGPU.importJsQueue || !native.WebGPU.importJsTexture)
    throw new Error("Doroti WebGPU native registration is unavailable on the render owner.");
  const instance = native._wgpuCreateInstance(0);
  if (!instance) throw new Error("Doroti Dawn instance creation failed.");
  handles = { instance, device: native.WebGPU.importJsDevice(device, instance), queue: native.WebGPU.importJsQueue(device.queue, instance) };
  return handles;
}

export function acquire(width: number, height: number): number {
  if (failure) throw failure;
  if (disposed || !context || !canvas || !native) throw new Error("Doroti WebGPU is not initialized.");
  if (canvas.width !== width) canvas.width = width;
  if (canvas.height !== height) canvas.height = height;
  return native.WebGPU.importJsTexture(context.getCurrentTexture());
}

export function releaseTexture(handle: number): void { native!._wgpuTextureRelease(handle); }
export async function waitForCapacity(): Promise<void> {
  if (pending.size >= 2) await Promise.race(pending);
  if (failure) throw failure;
}
export function submitted(): void {
  submittedFrames++;
  const completion = device!.queue.onSubmittedWorkDone().then(() => { completedSubmissions++; }, fail);
  pending.add(completion);
  void completion.finally(() => pending.delete(completion));
}
export function diagnostics() {
  return { backend: "Graphite/Dawn", format: "bgra8unorm", alphaMode: "premultiplied",
    inFlight: pending.size, pendingMaps: pendingMaps.size, submittedFrames, completedSubmissions, deviceGeneration: 1,
    mappedRanges: native?.dorotiWebGpuMappedRanges ?? null,
    failure: failure?.message ?? null, physicalPresentation: "notMeasured" };
}
export async function drainForShutdown(): Promise<void> {
  disposed = true;
  if (!device) return;
  await Promise.race([device.queue.onSubmittedWorkDone().catch(() => {}), device.lost.then(() => {})]);
  await Promise.allSettled(pending);
  while (pendingMaps.size) await Promise.allSettled(pendingMaps);
}
export function releaseNative(): void {
  if (handles && native) {
    native._wgpuQueueRelease(handles.queue);
    native._wgpuDeviceRelease(handles.device);
    native._wgpuInstanceRelease(handles.instance);
  }
  handles = undefined;
  context?.unconfigure();
  device?.destroy();
  context = undefined; canvas = undefined; device = undefined; native = undefined;
}
export function loseDevice(): void { device?.destroy(); }
