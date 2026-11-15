/** Races startup awaits with owner closure and retires resources arriving late. */
export class WorkerStartupLifetime {
  constructor(readonly lateFailure: (error: unknown) => void = () => {}) {}
  #closed = false;
  #cancel!: () => void;
  readonly #closedPromise = new Promise<void>(resolve => { this.#cancel = resolve; });
  get closed(): boolean { return this.#closed; }
  close(): void { this.#closed = true; this.#cancel(); }
  check(): void { if (this.#closed) throw new Error("Render owner closed during startup."); }
  async wait<T>(pending: Promise<T>, retireLate?: (value: T) => void | Promise<void>): Promise<T> {
    this.check();
    const owned = pending.then(async value => {
      if (this.#closed) {
        try { await retireLate?.(value); } catch (error) { this.lateFailure(error); throw error; }
        this.check();
      }
      return value;
    });
    return Promise.race([owned, this.#closedPromise.then(() => { this.check(); throw new Error("Closed owner."); })]);
  }
}

export async function cleanupAll(actions: (() => void | Promise<void>)[]): Promise<unknown[]> {
  const failures: unknown[] = [];
  for (const action of actions) { try { await action(); } catch (error) { failures.push(error); } }
  return failures;
}

export async function awaitWithAbort<T>(pending: Promise<T>, signal?: AbortSignal): Promise<T> {
  if (!signal) return pending;
  signal.throwIfAborted();
  let cancel!: () => void;
  const aborted = new Promise<never>((_, reject) => { cancel = () => reject(signal.reason); });
  signal.addEventListener("abort", cancel, { once: true });
  try { return await Promise.race([pending, aborted]); }
  finally { signal.removeEventListener("abort", cancel); }
}

/** Runtime creation alone is not a usable render session. Bound both milestones. */
export class RenderSessionReadiness {
  #generation = 0;
  #runtimeReady = false;
  #firstFrame = false;
  #closed = true;
  #timer: ReturnType<typeof setTimeout> | undefined;
  constructor(readonly timeoutMs: number, readonly ready: () => void, readonly failed: (error: Error) => void) {}
  begin(generation: number): void {
    this.close();
    this.#generation = generation;
    this.#runtimeReady = this.#firstFrame = false;
    this.#closed = false;
    this.#timer = setTimeout(() => {
      const missing = this.#runtimeReady ? "first canvas frame" : "managed runtime readiness";
      this.close();
      this.failed(new Error(`Doroti render session ${generation} timed out waiting for ${missing}. Check Worker/CSP/network diagnostics.`));
    }, this.timeoutMs);
  }
  runtimeReady(generation: number): void { this.#mark(generation, false); }
  firstFrame(generation: number): void { this.#mark(generation, true); }
  #mark(generation: number, frame: boolean): void {
    if (this.#closed || generation !== this.#generation) return;
    if (frame) this.#firstFrame = true;
    else this.#runtimeReady = true;
    if (this.#runtimeReady && this.#firstFrame) { this.close(); this.ready(); }
  }
  close(): void { this.#closed = true; clearTimeout(this.#timer); this.#timer = undefined; }
}
