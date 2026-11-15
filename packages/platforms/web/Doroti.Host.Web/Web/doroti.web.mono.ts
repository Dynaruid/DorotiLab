import type { RuntimeContract } from "./doroti.web.execution.js";

export interface MonoWorkerConfig {
  resources?: { jsModuleWorker?: { name: string; resolvedUrl?: string }[] };
}
export function monoWorkerUrls(contract: RuntimeContract, loaderUrl: string, config: MonoWorkerConfig): ReadonlySet<string> {
  const loader = new URL(loaderUrl);
  const urls = contract === "mono-11" ? [loader.href] :
    config.resources?.jsModuleWorker?.map(asset => new URL(asset.resolvedUrl ?? asset.name, loader).href) ?? [];
  if (urls.length !== 1 || new URL(urls[0]!).origin !== loader.origin)
    throw new Error("DOROTIWEB040: Expected one same-origin Mono pthread entry from the selected runtime metadata.");
  return new Set(urls);
}

// Mono 11 / Emscripten 6 no longer sets Worker.loaded in its native loader.
// Its pool can consequently choose a still-loading Worker for synchronous
// Thread.Start. Use only the public Worker message boundary; never patch PThread.
export const mono11InitialWorkerPoolSize = 16;
export class MonoWorkerPreload {
  readonly #pending = new Map<Worker, (event: MessageEvent) => void>();
  readonly #waiters = new Set<{ resolve(): void; reject(error: Error): void }>();
  #failure: Error | undefined;
  observe(worker: Worker): void {
    if (this.#failure) return;
    const loaded = (event: MessageEvent): void => {
      // Emscripten 6.0.3's documented-in-the-pack native startup receipt.
      // Preload, config and all other Mono/Emscripten control messages pass on.
      if (event.data?.cmd !== 3) return;
      worker.removeEventListener("message", loaded);
      // The runtime's onmessage handler must process the receipt first.
      setTimeout(() => {
        this.#pending.delete(worker);
        if (this.#pending.size === 0) {
          for (const waiter of this.#waiters) waiter.resolve();
          this.#waiters.clear();
        }
      }, 0);
    };
    this.#pending.set(worker, loaded);
    worker.addEventListener("message", loaded);
  }
  ready(): Promise<void> {
    if (this.#failure) return Promise.reject(this.#failure);
    if (!this.#pending.size) return Promise.resolve();
    return new Promise((resolve, reject) => this.#waiters.add({ resolve, reject }));
  }
  fail(error: Error): void {
    this.#failure ??= error;
    for (const [worker, listener] of this.#pending) worker.removeEventListener("message", listener);
    this.#pending.clear();
    for (const waiter of this.#waiters) waiter.reject(this.#failure);
    this.#waiters.clear();
  }
}
