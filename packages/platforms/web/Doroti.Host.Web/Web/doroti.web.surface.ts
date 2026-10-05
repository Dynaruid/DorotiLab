export interface WorkerVisibleSurface {
  readonly offscreen: OffscreenCanvas;
}

export function createWorkerVisibleSurface(canvas: HTMLCanvasElement): WorkerVisibleSurface {
  if (typeof canvas.transferControlToOffscreen !== "function")
    throw new Error("Doroti direct worker requires transferControlToOffscreen.");
  return { offscreen: canvas.transferControlToOffscreen() };
}
