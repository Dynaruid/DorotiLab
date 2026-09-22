// Opt-in numeric ring. No per-event objects, strings, postMessage or JSON.
// Times are absolute performance epochs, shared by main and Worker clocks.
export class FrameCostBuffer {
  private readonly data: Float64Array;
  private count = 0;
  constructor(readonly capacity = 16384) {
    this.data = new Float64Array(capacity * 6);
  }
  record(kind: number, id: number, input: number, start: number, end: number, detail = 0): void {
    const offset = (this.count++ % this.capacity) * 6;
    this.data[offset] = kind;
    this.data[offset + 1] = id;
    this.data[offset + 2] = input;
    this.data[offset + 3] = start;
    this.data[offset + 4] = end;
    this.data[offset + 5] = detail;
  }
  reset(): void { this.count = 0; }
  capture(): { rows: number[][]; dropped: number } {
    const rows: number[][] = [];
    for (let i = Math.max(0, this.count - this.capacity); i < this.count; i++) {
      const offset = (i % this.capacity) * 6;
      rows.push(Array.from(this.data.subarray(offset, offset + 6)));
    }
    return { rows, dropped: Math.max(0, this.count - this.capacity) };
  }
}
