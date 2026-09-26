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
interface EffectAllocation { input: GPUTexture; output: GPUTexture; outputHandle: number; width: number; height: number; buffers: GPUBuffer[]; }
const effectAllocations = new Map<number, EffectAllocation>();
const effectPipelines = new Map<string, GPURenderPipeline>();
function effectConstants() {
  return globalThis as unknown as {
    GPUTextureUsage: { RENDER_ATTACHMENT: number; TEXTURE_BINDING: number; COPY_SRC: number; COPY_DST: number };
    GPUBufferUsage: { UNIFORM: number; COPY_DST: number };
    GPUShaderStage: { FRAGMENT: number };
  };
}

export function allocateGpuEffect(width: number, height: number): { input: number; output: number; destroy(): void } {
  const { GPUTextureUsage } = effectConstants();
  if (failure) throw failure;
  if (disposed || !device || !native || !Number.isSafeInteger(width) || !Number.isSafeInteger(height) ||
      width < 1 || height < 1 || width > device.limits.maxTextureDimension2D || height > device.limits.maxTextureDimension2D ||
      width * height * 8 > 64 * 1024 * 1024) throw new Error("WebGPU effect dimensions exceed device limits.");
  const textures: GPUTexture[] = []; const imported: number[] = [];
  let input = 0;
  const buffers: GPUBuffer[] = [];
  const destroy = (): void => {
    for (const handle of imported) native!._wgpuTextureRelease(handle);
    textures.forEach(texture => texture.destroy()); buffers.forEach(buffer => buffer.destroy());
    effectAllocations.delete(input);
  };
  try {
    for (let index = 0; index < 2; index++) {
      const texture = device.createTexture({ size: [width, height], format: "rgba8unorm",
        usage: GPUTextureUsage.RENDER_ATTACHMENT | GPUTextureUsage.TEXTURE_BINDING | GPUTextureUsage.COPY_SRC | GPUTextureUsage.COPY_DST });
      textures.push(texture); imported.push(native.WebGPU.importJsTexture(texture));
      if (!imported[index]) throw new Error("Cannot import WebGPU effect texture into Dawn.");
    }
    input = imported[0];
    effectAllocations.set(input, { input: textures[0], output: textures[1], outputHandle: imported[1], width, height, buffers });
    return { input, output: imported[1], destroy };
  } catch (error) { destroy(); throw error; }
}

export function executeGpuEffect(handle: number, key: string, vertex: string, fragment: string, entry: string, parameters: Uint8Array, time = 0, deltaTime = 0, logicalWidth = 0, logicalHeight = 0): void {
  const { GPUShaderStage, GPUBufferUsage } = effectConstants();
  if (failure) throw failure;
  const effect = effectAllocations.get(handle);
  if (disposed || !device || !effect) throw new Error("Stale WebGPU effect device/lease.");
  const owner = device;
  if (parameters.length > owner.limits.maxUniformBufferBindingSize) throw new Error("WebGPU effect uniform limit exceeded.");
  owner.pushErrorScope("validation");
  try {
    let pipeline = effectPipelines.get(key);
    if (!pipeline) {
      const engine = owner.createBindGroupLayout({ entries: [
        { binding: 0, visibility: GPUShaderStage.FRAGMENT, texture: { sampleType: "float" } },
        { binding: 1, visibility: GPUShaderStage.FRAGMENT, sampler: { type: "filtering" } },
        { binding: 2, visibility: GPUShaderStage.FRAGMENT, buffer: { type: "uniform" } },
      ] });
      const user = owner.createBindGroupLayout({ entries: [{ binding: 0, visibility: GPUShaderStage.FRAGMENT, buffer: { type: "uniform" } }] });
      pipeline = owner.createRenderPipeline({ layout: owner.createPipelineLayout({ bindGroupLayouts: [engine, user] }),
        vertex: { module: owner.createShaderModule({ code: vertex }), entryPoint: "main" },
        fragment: { module: owner.createShaderModule({ code: fragment }), entryPoint: entry, targets: [{ format: "rgba8unorm" }] },
        primitive: { topology: "triangle-list" } });
      if (effectPipelines.size >= 64) effectPipelines.delete(effectPipelines.keys().next().value!);
      effectPipelines.set(key, pipeline);
    }
    const uniform = (bytes: Uint8Array): GPUBuffer => {
      const buffer = owner.createBuffer({ size: Math.max(16, Math.ceil(bytes.length / 16) * 16), usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST });
      effect.buffers.push(buffer);
      if (bytes.length) owner.queue.writeBuffer(buffer, 0, bytes as Uint8Array<ArrayBuffer>);
      return buffer;
    };
    const frameBytes = new Uint8Array(32); const frame = new DataView(frameBytes.buffer);
    frame.setUint32(0, effect.width, true); frame.setUint32(4, effect.height, true);
    frame.setUint32(8, effect.width, true); frame.setUint32(12, effect.height, true);
    frame.setFloat32(16, logicalWidth || effect.width, true); frame.setFloat32(20, logicalHeight || effect.height, true);
    frame.setFloat32(24, time, true); frame.setFloat32(28, deltaTime, true);
    const engine = owner.createBindGroup({ layout: pipeline.getBindGroupLayout(0), entries: [
      { binding: 0, resource: effect.input.createView() },
      { binding: 1, resource: owner.createSampler({ magFilter: "nearest", minFilter: "nearest" }) },
      { binding: 2, resource: { buffer: uniform(frameBytes) } },
    ] });
    const user = owner.createBindGroup({ layout: pipeline.getBindGroupLayout(1), entries: [{ binding: 0, resource: { buffer: uniform(parameters) } }] });
    const encoder = owner.createCommandEncoder();
    const pass = encoder.beginRenderPass({ colorAttachments: [{ view: effect.output.createView(), loadOp: "clear", storeOp: "store", clearValue: [0, 0, 0, 0] }] });
    pass.setPipeline(pipeline); pass.setBindGroup(0, engine); pass.setBindGroup(1, user); pass.draw(3); pass.end();
    owner.queue.submit([encoder.finish()]);
  } finally { void owner.popErrorScope().then(error => { if (error) fail(error); }, fail); }
}

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
  // Capacity is changed by the presenter owner after admission, before acquire.
  if (canvas.width < width || canvas.height < height)
    throw new Error("Doroti WebGPU frame exceeds presenter capacity.");
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
  if (effectAllocations.size) throw new Error("Retire effect textures before releasing Dawn.");
  effectPipelines.clear();
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

// Sampling textures share the presenter's exact device/queue. Source capture is
// consumed by copyExternalImageToTexture; destination retirement is separate.
export function copyTextureSource(source: VideoFrame | ImageBitmap | OffscreenCanvas, width: number, height: number) {
  if (!device || !native || failure || disposed) throw failure ?? new Error("WebGPU texture owner unavailable.");
  if (width > device.limits.maxTextureDimension2D || height > device.limits.maxTextureDimension2D)
    throw new Error("Source exceeds the WebGPU device dimension limit.");
  const usage = (globalThis as unknown as { GPUTextureUsage: { COPY_DST: number; TEXTURE_BINDING: number; RENDER_ATTACHMENT: number } }).GPUTextureUsage;
  const texture = device.createTexture({ size: [width, height], format: "rgba8unorm",
    usage: usage.COPY_DST | usage.TEXTURE_BINDING | usage.RENDER_ATTACHMENT });
  try {
    device.queue.copyExternalImageToTexture({ source, flipY: false },
      { texture, premultipliedAlpha: true, colorSpace: "srgb" }, [width, height]);
    const handle = native.WebGPU.importJsTexture(texture);
    if (!handle) throw new Error("Dawn texture import failed.");
    return { handle, destroy: () => { native!._wgpuTextureRelease(handle); texture.destroy(); } };
  } catch (error) { texture.destroy(); throw error; }
}
export function textureWorkDone(): Promise<void> {
  if (!device) return Promise.resolve();
  return Promise.race([device.queue.onSubmittedWorkDone().catch(() => {}), device.lost.then(() => {})]);
}
