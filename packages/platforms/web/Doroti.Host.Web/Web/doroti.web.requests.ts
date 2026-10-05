interface RequestErrors {
  full(): Error;
  closed(): Error;
  timeout(): Error;
}

interface PendingRequest<T> {
  resolve(value: T): void;
  reject(reason: unknown): void;
  timer?: ReturnType<typeof setTimeout>;
}

/** Correlated endpoint requests. Closing cancels callers; it is not a GPU completion receipt. */
export class WorkerRequestMailbox<T> {
  #sequence = 0;
  #closed = false;
  readonly #pending = new Map<number, PendingRequest<T>>();
  readonly #tasks = new Set<Promise<T>>();

  constructor(
    readonly limit: number,
    readonly timeoutMilliseconds: number,
    readonly errors: RequestErrors,
    readonly onTimeout?: () => void,
  ) { }

  get pendingCount(): number { return this.#pending.size; }

  request(send: (requestId: number) => void, timeoutMilliseconds: number | null = this.timeoutMilliseconds): Promise<T> {
    if (this.#closed) return Promise.reject(this.errors.closed());
    if (this.#pending.size >= this.limit) return Promise.reject(this.errors.full());
    const requestId = ++this.#sequence;
    let pending!: PendingRequest<T>;
    const task = new Promise<T>((resolve, reject) => { pending = { resolve, reject }; });
    this.#pending.set(requestId, pending);
    this.#tasks.add(task);
    // Observe failures even when the endpoint shuts down before the caller awaits.
    void task.then(() => this.#tasks.delete(task), () => this.#tasks.delete(task));
    if (timeoutMilliseconds !== null) pending.timer = setTimeout(() => {
      // Preserve the timeout error before owner shutdown rejects other requests.
      if (this.reject(requestId, this.errors.timeout())) this.onTimeout?.();
    }, timeoutMilliseconds);
    try { send(requestId); }
    catch (error) { this.reject(requestId, error); }
    return task;
  }

  resolve(requestId: number, value: T): boolean {
    const pending = this.#take(requestId);
    if (!pending) return false;
    pending.resolve(value);
    return true;
  }

  reject(requestId: number, reason: unknown): boolean {
    const pending = this.#take(requestId);
    if (!pending) return false;
    pending.reject(reason);
    return true;
  }

  #take(requestId: number): PendingRequest<T> | undefined {
    const pending = this.#pending.get(requestId);
    if (!pending) return undefined;
    this.#pending.delete(requestId);
    clearTimeout(pending.timer);
    return pending;
  }

  /** Keeps admission open for a replacement endpoint; IDs are never reused. */
  rejectAll(reason: unknown): void {
    for (const requestId of this.#pending.keys()) this.reject(requestId, reason);
  }

  close(reason: unknown = this.errors.closed()): void {
    this.#closed = true;
    this.rejectAll(reason);
  }

  async drain(): Promise<void> { await Promise.allSettled([...this.#tasks]); }
}
