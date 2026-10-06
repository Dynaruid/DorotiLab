export type RendererMode = "worker-direct-webgl" | "worker-direct-webgpu";
export interface RendererPolicy {
  requested: RendererMode | "auto";
  selected: RendererMode;
  reason: string;
  fallbackReason: string | null;
  memoryProfile: "mobile" | "desktop";
}

// A conservative platform policy, not an estimate of available physical RAM.
// Desktop-UA iPadOS exposes MacIntel plus multiple touch points. UA spoofing
// remains possible; explicit renderer overrides always win.
export function selectRendererPolicy(search: string, platform: {
  userAgent: string; platform: string; maxTouchPoints: number;
}): RendererPolicy {
  const value = new URLSearchParams(search).get("dorotiRenderer");
  const requested = value === "worker-direct-webgl" || value === "worker-direct-webgpu" ? value : "auto";
  const ios = /iPhone|iPad|iPod/.test(platform.userAgent) ||
    (platform.platform === "MacIntel" && platform.maxTouchPoints > 1);
  const android = !ios && /Android/.test(platform.userAgent);
  const selected = requested === "auto" ? android ? "worker-direct-webgpu" : "worker-direct-webgl" : requested;
  return { requested, selected, reason: requested !== "auto" ? "explicit-override" :
    ios ? "ios-webkit-stability" : android ? "android-prefer-webgpu" : "default-webgl2", fallbackReason: null,
    memoryProfile: ios || android ? "mobile" : "desktop" };
}

// Resolve auto preference before transferring the visible canvas or starting a
// GPU owner. Explicit overrides and failures after initialization stay visible.
export async function resolveRendererPolicy(policy: RendererPolicy, runtimeLocation: "main" | "worker",
  environment: { isSecureContext: boolean; crossOriginIsolated: boolean;
    navigator: { gpu?: { requestAdapter(): Promise<unknown | null> } } } = globalThis): Promise<RendererPolicy> {
  if (policy.requested !== "auto" || policy.selected !== "worker-direct-webgpu") return policy;
  let fallbackReason: string | null = null;
  if (runtimeLocation !== "main") fallbackReason = "webgpu-requires-main-runtime";
  else if (!environment.isSecureContext || !environment.crossOriginIsolated)
    fallbackReason = "webgpu-requires-secure-isolated-origin";
  else if (!environment.navigator.gpu) fallbackReason = "webgpu-api-unavailable";
  else {
    try {
      if (!await environment.navigator.gpu.requestAdapter()) fallbackReason = "webgpu-adapter-unavailable";
    } catch {
      fallbackReason = "webgpu-adapter-probe-failed";
    }
  }
  return fallbackReason ? { ...policy, selected: "worker-direct-webgl", reason: "auto-webgl2-fallback", fallbackReason } : policy;
}

export interface CanvasLimits { dimension: number; bytes: number; }
// Avoid oversized Firefox OffscreenCanvas allocations at high DPR. Headroom
// on both axes increases a 4.6MP viewport to a 10.4MP GPU surface at DPR 2.
// Use exact growth and bounded delayed shrink; retain desktop Skia budgets.
export function useCompactCanvas(policy: RendererPolicy, userAgent: string): boolean {
  return policy.memoryProfile === "mobile" || /Firefox\//.test(userAgent);
}
export const defaultCanvasLimits: CanvasLimits = { dimension: 8192, bytes: 256 * 1024 * 1024 };
export function boundCanvasCapacity(width: number, height: number, desiredWidth: number, desiredHeight: number,
  limits: CanvasLimits = defaultCanvasLimits) {
  if (![width, height, desiredWidth, desiredHeight, limits.dimension, limits.bytes].every(value => Number.isSafeInteger(value) && value > 0)
    || width > limits.dimension || height > limits.dimension || width > Math.floor(limits.bytes / 4 / height))
    throw new RangeError("Canvas backing exceeds device dimension or color byte admission.");
  const capacity = { width: Math.max(width, Math.min(limits.dimension, desiredWidth)), height: Math.max(height, Math.min(limits.dimension, desiredHeight)) };
  return capacity.width > Math.floor(limits.bytes / 4 / capacity.height) ? { width, height } : capacity;
}
export function initialCanvasCapacity(width: number, height: number, dpr: number,
  mobile: boolean, screenWidth = 0, screenHeight = 0, limits: CanvasLimits = defaultCanvasLimits) {
  if (!Number.isFinite(dpr) || dpr <= 0) throw new RangeError("Invalid canvas DPR.");
  const desired = mobile ? { width: Math.ceil(width * dpr), height: Math.ceil(height * dpr) } : {
    width: Math.ceil(Math.max(width * 1.5, screenWidth, width) * dpr),
    height: Math.ceil(Math.max(height * 1.5, screenHeight, height) * dpr),
  };
  return boundCanvasCapacity(Math.ceil(width * dpr), Math.ceil(height * dpr), desired.width, desired.height, limits);
}

// Mobile: exact initial/growth size, shrink after 1s stable, at most once per
// 2s, when excess area is >=25%. No DPR change. Desktop keeps resize headroom.
export class CanvasCapacityPolicy {
  private width = 0;
  private height = 0;
  private changedAt = 0;
  private allocatedAt = -Infinity;
  constructor(readonly mobile: boolean) {}
  next(width: number, height: number, currentWidth: number, currentHeight: number, now: number,
    limits: CanvasLimits = defaultCanvasLimits) {
    boundCanvasCapacity(width, height, width, height, limits);
    if (width !== this.width || height !== this.height) {
      this.width = width; this.height = height; this.changedAt = now;
    }
    const grow = width > currentWidth || height > currentHeight;
    const excess = currentWidth * currentHeight >= width * height * 1.25;
    const delay = Math.max(this.changedAt + 1000, this.allocatedAt + 2000) - now;
    if (grow || (this.mobile && excess && delay <= 0)) {
      this.allocatedAt = now;
      return { ...boundCanvasCapacity(width, height,
        this.mobile ? width : width > currentWidth ? Math.max(width, Math.ceil(currentWidth * 1.5)) : currentWidth,
        this.mobile ? height : height > currentHeight ? Math.max(height, Math.ceil(currentHeight * 1.5)) : currentHeight,
        limits), wakeAfter: 0 };
    }
    return { width: currentWidth, height: currentHeight,
      wakeAfter: this.mobile && excess ? Math.max(1, delay) : 0 };
  }
}

// Shrink axes first so orientation changes never allocate a square intermediate
// backing larger than both the old and new buffers.
export function applyCanvasCapacity(canvas: { width: number; height: number }, width: number, height: number): void {
  if (width < canvas.width) canvas.width = width;
  if (height < canvas.height) canvas.height = height;
  if (width > canvas.width) canvas.width = width;
  if (height > canvas.height) canvas.height = height;
}
