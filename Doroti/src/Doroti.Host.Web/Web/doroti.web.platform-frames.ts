import type { CompositionPacket, RasterPacket } from "./doroti.web.composition.js";

interface StagedFrame {
  packet: CompositionPacket;
  captures: Promise<void>[];
  generation: number;
}

/** Owns captures until transfer. Each sealed frame waits only for its own captures. */
export class PlatformFrameStager {
  #rasters: RasterPacket[] = [];
  #captures: Promise<void>[] = [];
  #frame: StagedFrame | null = null;
  #inFlight: StagedFrame | null = null;
  #generation = 0;
  readonly #pendingCaptures = new Set<Promise<void>>();
  readonly #retiredRasters = new WeakSet<RasterPacket>();

  stageBitmap(raster: RasterPacket, bitmap: Promise<ImageBitmap>): void {
    this.#rasters.push(raster);
    const generation = this.#generation;
    const task = bitmap.then(value => {
      if (generation !== this.#generation || this.#retiredRasters.has(raster)) value.close();
      else raster.bitmap = value;
    });
    this.#pendingCaptures.add(task);
    void task.then(() => this.#pendingCaptures.delete(task), () => this.#pendingCaptures.delete(task));
    this.#captures.push(task);
  }

  stageRaster(raster: RasterPacket): void { this.#rasters.push(raster); }

  stageFrame(batch: CompositionPacket["batch"]): void {
    if (this.#frame) throw new Error("A platform frame is already staged; commit or discard it first.");
    this.#frame = { packet: { batch, rasters: this.#rasters }, captures: this.#captures, generation: this.#generation };
    this.#rasters = [];
    this.#captures = [];
  }

  discard(): void {
    this.#generation++;
    this.#closeRasters(this.#rasters);
    if (this.#frame) this.#closeRasters(this.#frame.packet.rasters);
    if (this.#inFlight) this.#closeRasters(this.#inFlight.packet.rasters);
    this.#frame = null;
    this.#rasters = [];
    this.#captures = [];
  }

  async drainCaptures(): Promise<void> { await Promise.allSettled([...this.#pendingCaptures]); }

  async commit(send: (frame: CompositionPacket, transfer: Transferable[]) => Promise<boolean>): Promise<boolean> {
    if (this.#inFlight) throw new Error("A platform frame commit is already in flight.");
    const frame = this.#frame;
    if (!frame) return true;
    this.#frame = null;
    this.#inFlight = frame;
    try {
      await Promise.all(frame.captures);
      if (frame.generation !== this.#generation) return false;
      const transfer = frame.packet.rasters.flatMap(raster => raster.bitmap ? [raster.bitmap] : []);
      const accepted = await send(frame.packet, transfer);
      return frame.generation === this.#generation && accepted;
    } finally {
      this.#closeRasters(frame.packet.rasters);
      this.#inFlight = null;
    }
  }

  #closeRasters(rasters: RasterPacket[]): void {
    for (const raster of rasters) {
      // A sibling capture can resolve after Promise.all has already rejected.
      this.#retiredRasters.add(raster);
      raster.bitmap?.close();
      delete raster.bitmap;
    }
  }
}
