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
    const worker = new Worker(bootstrap, { type: "module" });
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
  for (const [requestId, lease] of leases) close(requestId, lease);
  leases.clear();
}
