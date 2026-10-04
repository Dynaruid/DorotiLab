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
