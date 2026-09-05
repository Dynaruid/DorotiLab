// Explicitly enabled experiment trace. Each role owns a bounded ring; collection
// happens on a diagnostic command so normal per-scene diagnostics stay small.
export interface CanvasKitStage {
  stage: string;
  time: number;
  generation: number;
  sequence: number;
  detail: Record<string, unknown>;
}

export class CanvasKitStageTrace {
  enabled = false;
  private readonly entries: CanvasKitStage[] = [];
  private cursor = 0;
  private count = 0;
  record(stage: string, generation = 0, sequence = 0, detail: Record<string, unknown> = {}): void {
    if (!this.enabled) return;
    this.entries[this.cursor] = {
      stage, time: performance.timeOrigin + performance.now(), generation, sequence, detail,
    };
    this.cursor = (this.cursor + 1) % 8192;
    this.count++;
  }
  snapshot(): Record<string, unknown> {
    const deltas: number[] = [];
    let prior = performance.now();
    for (let index = 0; index < 256; index++) {
      const now = performance.now();
      if (now > prior) deltas.push(now - prior);
      prior = now;
    }
    return {
      clockResolutionProbe: { reads: 256, positiveDeltas: deltas.length,
        minimumMilliseconds: deltas.length ? Math.min(...deltas) : null,
        limitation: "sampled timer precision, not clock synchronization accuracy" },
      timeOrigin: performance.timeOrigin,
      clock: "performance.timeOrigin + performance.now",
      dropped: Math.max(0, this.count - 8192),
      entries: this.count <= 8192 ? [...this.entries]
        : [...this.entries.slice(this.cursor), ...this.entries.slice(0, this.cursor)],
    };
  }
}
