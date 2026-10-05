interface TapPoint {
  pointerId: number;
  clientX: number;
  clientY: number;
  timeStamp: number;
  isPrimary: boolean;
  button: number;
}

/** Commit keyboard focus within a short pointer-up gesture, never after a
 * Worker round trip and never for a scroll, long press, or cancelled gesture. */
export class TextInputTapFocus<T> {
  private tap: { point: TapPoint; target: T } | null = null;

  constructor(private readonly focus: (target: T) => void) {}

  start(point: TapPoint, target: T | null): void {
    this.tap = point.isPrimary && point.button === 0 && target !== null
      ? { point: { pointerId: point.pointerId, clientX: point.clientX, clientY: point.clientY,
          timeStamp: point.timeStamp, isPrimary: point.isPrimary, button: point.button }, target } : null;
  }

  move(point: TapPoint): void {
    if (this.tap?.point.pointerId === point.pointerId && this.moved(point)) this.tap = null;
  }

  end(point: TapPoint): void {
    const tap = this.tap;
    if (!tap || tap.point.pointerId !== point.pointerId) return;
    this.tap = null;
    if (point.timeStamp - tap.point.timeStamp < 500 &&
        Math.hypot(point.clientX - tap.point.clientX, point.clientY - tap.point.clientY) <= 18)
      this.focus(tap.target);
  }

  cancel(): void { this.tap = null; }

  private moved(point: TapPoint): boolean {
    return !!this.tap && Math.hypot(point.clientX - this.tap.point.clientX, point.clientY - this.tap.point.clientY) > 18;
  }
}
