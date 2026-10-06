interface Submission { fence: WebGLSync; started: number; }

/** Bounds GPU work, including frames without an externally owned texture. */
export class WebGlFrameQueue {
  private readonly pending: Submission[] = [];
  private closed = false;
  private failure: Error | null = null;
  private submittedFrames = 0;
  private completedSubmissions = 0;
  private peakInFlight = 0;
  private polls = 0;
  private timeouts = 0;
  private waitingSince: number | null = null;
  private synchronizations = 0;
  private maxCapacityWaitMs = 0;
  private static readonly maxFenceWaitMs = 250;

  constructor(private readonly gl: WebGL2RenderingContext, readonly limit = 2) {
    if (!Number.isSafeInteger(limit) || limit < 1) throw new RangeError("Invalid WebGL frame limit.");
  }

  async waitForCapacity(): Promise<void> {
    const started = this.waitingSince = performance.now();
    try {
      while (!this.closed) {
        this.reap(performance.now() - started >= WebGlFrameQueue.maxFenceWaitMs);
        if (this.pending.length < this.limit) return;
        // Yield the worker so input can replace an obsolete pending frame.
        // A zero-timeout fence query never blocks the CPU on GPU completion.
        await new Promise<void>(resolve => setTimeout(resolve, 4));
      }
    } finally {
      this.maxCapacityWaitMs = Math.max(this.maxCapacityWaitMs, performance.now() - started);
      this.waitingSince = null;
    }
  }

  submitted(): void {
    if (this.failure) throw this.failure;
    if (this.closed || this.gl.isContextLost()) return;
    if (this.pending.length >= this.limit) throw this.fail("WebGL frame capacity was not awaited.");
    const fence = this.gl.fenceSync(this.gl.SYNC_GPU_COMMANDS_COMPLETE, 0);
    if (!fence) throw this.fail("WebGL frame completion fence allocation failed.");
    this.pending.push({ fence, started: performance.now() });
    this.submittedFrames++;
    this.peakInFlight = Math.max(this.peakInFlight, this.pending.length);
    this.gl.flush();
  }

  contextLost(): void {
    this.closed = true;
    for (const submission of this.pending) this.gl.deleteSync(submission.fence);
    this.pending.length = 0;
  }

  async drainForShutdown(): Promise<void> {
    this.closed = true;
    const started = performance.now();
    this.reap(false);
    while (this.pending.length) {
      this.reap(performance.now() - started >= WebGlFrameQueue.maxFenceWaitMs);
      if (this.pending.length) await new Promise<void>(resolve => setTimeout(resolve, 4));
    }
  }

  diagnostics() {
    return { inFlight: this.pending.length, limit: this.limit, peakInFlight: this.peakInFlight,
      submittedFrames: this.submittedFrames, completedSubmissions: this.completedSubmissions,
      failure: this.failure?.message ?? null, polls: this.polls, timeouts: this.timeouts,
      synchronizations: this.synchronizations, maxCapacityWaitMs: this.maxCapacityWaitMs,
      oldestSubmissionAgeMs: this.pending.length ? performance.now() - this.pending[0].started : 0,
      capacityWaitMs: this.waitingSince === null ? 0 : performance.now() - this.waitingSince };
  }

  private reap(synchronize: boolean): void {
    if (this.gl.isContextLost()) { this.contextLost(); return; }
    if (this.failure) throw this.failure;
    while (this.pending.length) {
      const submission = this.pending[0];
      this.polls++;
      const status = this.gl.clientWaitSync(submission.fence, 0, 0);
      if (status === this.gl.TIMEOUT_EXPIRED) {
        this.timeouts++;
        // Measure an actual blocked admission, not the age of a fence left
        // behind by an idle/background page. Its cached status may just need
        // another WebGL task before the next asynchronous query can see it.
        if (synchronize) this.completeSynchronously();
        return;
      }
      if (status !== this.gl.ALREADY_SIGNALED && status !== this.gl.CONDITION_SATISFIED)
        throw this.fail("WebGL frame completion failed; GPU resources remain owned.");
      this.gl.deleteSync(submission.fence);
      this.pending.shift();
      this.completedSubmissions++;
    }
  }

  private completeSynchronously(): void {
    // WebKit refreshes a sync object's cached status in a separate WebGL task.
    // If that cache stops advancing, waiting on it must not disable the app.
    // finish() proves actual completion before freeing either slot; merely
    // deleting a timed-out fence would let GPU work and memory grow unbounded.
    try { this.gl.finish(); }
    catch { throw this.fail("WebGL completion synchronization failed; GPU resources remain owned."); }
    if (this.gl.isContextLost()) { this.contextLost(); return; }
    this.synchronizations++;
    for (const submission of this.pending) this.gl.deleteSync(submission.fence);
    this.completedSubmissions += this.pending.length;
    this.pending.length = 0;
  }

  private fail(message: string): Error { return this.failure ??= new Error(message); }
}
