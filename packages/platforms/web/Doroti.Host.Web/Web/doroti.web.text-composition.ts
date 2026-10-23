// A textarea exposes its caret, but not the IME's complete marked range. Keep
// the unchanged document on either side of the initial replacement instead.
export class BrowserTextComposition {
  private prefix: string;
  private suffix: string;
  private data: string | null = null;

  constructor(value: string, start: number, end: number) {
    start = Math.max(0, Math.min(value.length, start));
    end = Math.max(start, Math.min(value.length, end));
    this.prefix = value.slice(0, start);
    this.suffix = value.slice(end);
  }

  update(data: string): void { this.data = data; }

  range(value: string, synchronizeData = false): readonly [number, number] | null {
    if (value.length < this.prefix.length + this.suffix.length ||
        !value.startsWith(this.prefix) || !value.endsWith(this.suffix)) return null;
    const end = value.length - this.suffix.length;
    // compositionupdate can precede the DOM mutation. Consume its data only
    // from input or the deferred callback, after the DOM has caught up. Some
    // IMEs commit a prefix while leaving the rest of the session composing.
    if (synchronizeData && this.data !== null) {
      const start = end - this.data.length;
      if (start >= this.prefix.length && value.slice(start, end) === this.data)
        this.prefix = value.slice(0, start);
      this.data = null;
    }
    return [this.prefix.length, end];
  }
}
