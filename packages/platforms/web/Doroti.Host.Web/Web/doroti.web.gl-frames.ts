interface Submission { fence: WebGLSync; started: number; }

/** Bounds GPU work, including frames without an externally owned texture. */
export class WebGlFrameQueue {
  private readonly pending: Submission[] = [];
  private closed = false;
  private failure: Error | null = null;
  private submittedFrames = 0;
  private completedSubmissions = 0;
  private peakInFlight = 0;

  constructor(private readonly gl: WebGL2RenderingContext, readonly limit = 2) {
    if (!Number.isSafeInteger(limit) || limit < 1) throw new RangeError("Invalid WebGL frame limit.");
  }

  async waitForCapacity(): Promise<void> {
    while (!this.closed) {
      this.reap();
      if (this.pending.length < this.limit) return;
      // Yield the worker so input can replace an obsolete pending frame.
      // A zero-timeout fence query never blocks the CPU on GPU completion.
      await new Promise<void>(resolve => setTimeout(resolve, 4));
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
    this.reap();
    while (this.pending.length) {
      this.reap();
      if (this.pending.length) await new Promise<void>(resolve => setTimeout(resolve, 4));
    }
  }

  diagnostics() {
    return { inFlight: this.pending.length, limit: this.limit, peakInFlight: this.peakInFlight,
      submittedFrames: this.submittedFrames, completedSubmissions: this.completedSubmissions,
      failure: this.failure?.message ?? null };
  }

  private reap(): void {
    if (this.gl.isContextLost()) { this.contextLost(); return; }
    if (this.failure) throw this.failure;
    while (this.pending.length) {
      const submission = this.pending[0];
      const status = this.gl.clientWaitSync(submission.fence, 0, 0);
      if (status === this.gl.TIMEOUT_EXPIRED) {
        if (performance.now() - submission.started >= 30000)
          throw this.fail("WebGL frame completion timed out; GPU resources remain owned.");
        return;
      }
      if (status !== this.gl.ALREADY_SIGNALED && status !== this.gl.CONDITION_SATISFIED)
        throw this.fail("WebGL frame completion failed; GPU resources remain owned.");
      this.gl.deleteSync(submission.fence);
      this.pending.shift();
      this.completedSubmissions++;
    }
  }

  private fail(message: string): Error { return this.failure ??= new Error(message); }
}
