export interface WorkerVisibleSurface {
  readonly display: ImageBitmapRenderingContext | null;
  readonly offscreen: OffscreenCanvas | null;
}

export function createWorkerVisibleSurface(
  canvas: HTMLCanvasElement,
  direct: boolean,
): WorkerVisibleSurface {
  if (direct) {
    if (typeof canvas.transferControlToOffscreen !== "function")
      throw new Error("Doroti direct worker requires transferControlToOffscreen.");
    return { display: null, offscreen: canvas.transferControlToOffscreen() };
  }
  const display = canvas.getContext("bitmaprenderer");
  if (!display) throw new Error("Doroti worker display requires bitmaprenderer.");
  return { display, offscreen: null };
}
