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
  const selected = requested === "auto" ? (ios ? "worker-direct-webgl" : "worker-direct-webgpu") : requested;
  return { requested, selected, reason: requested !== "auto" ? "explicit-override" :
    ios ? "ios-webkit-stability" : "default-webgpu", fallbackReason: null,
    memoryProfile: ios || /Android/.test(platform.userAgent) ? "mobile" : "desktop" };
}

export function initialCanvasCapacity(width: number, height: number, dpr: number,
  mobile: boolean, screenWidth = 0, screenHeight = 0) {
  return mobile ? { width: Math.ceil(width * dpr), height: Math.ceil(height * dpr) } : {
    width: Math.ceil(Math.max(width * 1.5, screenWidth, width) * dpr),
    height: Math.ceil(Math.max(height * 1.5, screenHeight, height) * dpr),
  };
}

// Mobile: exact initial/growth size, shrink after 1s stable, at most once per
// 2s, when excess area is >=25%. No DPR change. Desktop keeps resize headroom.
export class CanvasCapacityPolicy {
  private width = 0;
  private height = 0;
  private changedAt = 0;
  private allocatedAt = -Infinity;
  constructor(readonly mobile: boolean) {}
  next(width: number, height: number, currentWidth: number, currentHeight: number, now: number) {
    if (width !== this.width || height !== this.height) {
      this.width = width; this.height = height; this.changedAt = now;
    }
    const grow = width > currentWidth || height > currentHeight;
    const excess = currentWidth * currentHeight >= width * height * 1.25;
    const delay = Math.max(this.changedAt + 1000, this.allocatedAt + 2000) - now;
    if (grow || (this.mobile && excess && delay <= 0)) {
      this.allocatedAt = now;
      return { width: this.mobile ? width : Math.max(width, Math.ceil(currentWidth * 1.5)),
        height: this.mobile ? height : Math.max(height, Math.ceil(currentHeight * 1.5)), wakeAfter: 0 };
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
