type Input = Record<string, unknown> & {
  inputKind: string; hostId: number; inputSequence: number; payload: Record<string, unknown>;
};

/** Batch consecutive motion/wheel input while the owner dispatches the previous batch.
 * Preserve all positions/timestamps and flush before every ordering boundary.
 * The size limit flushes rather than dropping handwriting/velocity history.
 */
export class PointerMoveAdmission {
  private inFlight: number | undefined;
  private pending: Input[] = [];
  private pendingSamples = 0;
  private wheelEnabled = false;
  constructor(private readonly send: (input: Input) => void, readonly maxSamples = 128, private enabled = true) {
    if (!Number.isSafeInteger(maxSamples) || maxSamples < 1) throw new Error("Invalid pointer batch limit.");
  }

  push(input: Input): void {
    const p = input.payload;
    const motion = this.enabled && input.inputKind === "pointer" && (p.phase === 0 || p.phase === 4);
    const wheel = this.wheelEnabled && input.inputKind === "wheel";
    if (!motion && !wheel) { this.flush(); this.send(input); return; }
    const samples = motion ? (p.samples as number[]).length / 7 : 1;
    const previous = this.pending[0];
    const keys = wheel ? ["x", "y", "kind", "signalKind"] : ["phase", "kind", "pointerId", "buttons", "modifiers"];
    if (previous && (previous.hostId !== input.hostId || previous.inputKind !== input.inputKind ||
        keys.some(key => previous.payload[key] !== p[key]) ||
        this.pendingSamples + samples > this.maxSamples)) this.flush();
    this.pending.push(input);
    this.pendingSamples += samples;
    if (this.inFlight === undefined || samples >= this.maxSamples) this.flush();
  }

  acknowledge(sequence: number): void {
    if (sequence !== this.inFlight) return;
    this.inFlight = undefined;
    this.flush();
  }

  flush(): void {
    if (!this.pending.length) return;
    const batch = this.pending;
    const last = batch[batch.length - 1];
    // Retain every input sequence as well as the native coalesced samples.
    // The managed host requires contiguous sequences, including non-pointer input.
    const input = last.inputKind === "wheel"
      ? { ...last, wheelAdmission: true, ...(batch.length > 1 ? { inputBatch: batch } : {}) }
      : { ...last, pointerAdmission: true, ...(batch.length > 1 ? { pointerBatch: batch } : {}) };
    this.pending = [];
    this.pendingSamples = 0;
    this.inFlight = input.inputSequence;
    try { this.send(input); }
    catch (error) { this.inFlight = undefined; throw error; }
  }

  enable(): void { this.enabled = true; }
  enableWheel(): void { this.wheelEnabled = true; }
  reset(): void { this.pending = []; this.pendingSamples = 0; this.inFlight = undefined; this.enabled = false; this.wheelEnabled = false; }
}
