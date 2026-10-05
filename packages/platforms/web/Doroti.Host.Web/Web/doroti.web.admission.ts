/** Bounded transport window plus one replaceable latest resize. No frame payload retention. */
export class ResizeAdmissionWindow<T extends { epoch: { generation: number } }> {
  private readonly inFlight = new Set<number>();
  private latest: T | null = null;
  private readonly send: (value: T) => void;
  constructor(send: (value: T) => void) { this.send = send; }
  get pendingCount(): number { return this.inFlight.size + (this.latest ? 1 : 0); }
  queue(value: T): void { this.latest = value; this.flush(); }
  acknowledge(generation: number): void { if (this.inFlight.delete(generation)) this.flush(); }
  reset(): void { this.inFlight.clear(); this.latest = null; }
  private flush(): void {
    if (this.inFlight.size >= 4 || !this.latest) return;
    const next = this.latest;
    this.latest = null;
    this.inFlight.add(next.epoch.generation);
    this.send(next);
  }
}
