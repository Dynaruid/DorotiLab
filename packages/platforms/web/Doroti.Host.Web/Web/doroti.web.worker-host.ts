export function createDorotiWorker(url: URL): Worker {
  // Worker entry URLs are not resolved through the document import map.
  // Use the same published fingerprint selected for ordinary module imports.
  const script = typeof document === "undefined" ? null : document.querySelector<HTMLScriptElement>('script[type="importmap"]');
  if (script?.textContent) {
    const imports = (JSON.parse(script.textContent) as { imports?: Record<string, string> }).imports ?? {};
    for (const [source, target] of Object.entries(imports))
      if (new URL(source, document.baseURI).href === url.href) { url = new URL(target, document.baseURI); break; }
  }
  // Static-asset dev servers may isolate only the document/_framework worker
  // responses. A same-origin blob module inherits the document's COEP policy.
  // CSP deployments must allow worker-src blob: or serve the worker with COEP.
  if (globalThis.crossOriginIsolated) {
    const bootstrap = URL.createObjectURL(new Blob([`import ${JSON.stringify(url.href)};`], { type: "text/javascript" }));
    let worker: Worker;
    try { worker = new Worker(bootstrap, { type: "module" }); }
    catch (error) { URL.revokeObjectURL(bootstrap); throw error; }
    const release = (): void => URL.revokeObjectURL(bootstrap);
    worker.addEventListener("message", release, { once: true });
    worker.addEventListener("error", release, { once: true });
    const terminate = worker.terminate.bind(worker);
    worker.terminate = () => { release(); terminate(); };
    return worker;
  }
  return new Worker(url, { type: "module" });
}

export function closeExternalLeases<T>(
  leases: Map<number, T>,
  close: (requestId: number, lease: T) => void,
): void {
  const retired = [...leases];
  leases.clear();
  const failures: unknown[] = [];
  for (const [requestId, lease] of retired) {
    try { close(requestId, lease); } catch (error) { failures.push(error); }
  }
  if (failures.length) throw new AggregateError(failures, "External lease retirement failed.");
}

/** Retired endpoints can still have transferred a platform frame before replacement. */
export function closeRetiredWorkerMessage(value: unknown): void {
  if (!value || typeof value !== "object" || typeof ImageBitmap === "undefined") return;
  const message = value as Record<string, unknown>;
  const bitmaps = new Set<ImageBitmap>();
  if (message.bitmap instanceof ImageBitmap) bitmaps.add(message.bitmap);
  if (message.controlKind === "platform-frame") {
    const payload = message.payload as { rasters?: { bitmap?: unknown }[] } | undefined;
    if (Array.isArray(payload?.rasters)) for (const raster of payload.rasters)
      if (raster?.bitmap instanceof ImageBitmap) bitmaps.add(raster.bitmap);
  }
  for (const bitmap of bitmaps) bitmap.close();
}
