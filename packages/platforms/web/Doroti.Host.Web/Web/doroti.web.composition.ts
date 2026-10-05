import { DorotiPlatformViewDomRegistry, type NativeBatch, type NativeIdentity, type NativeBounds } from "./doroti.web.platform-views.js";
import { BrowserWebView, WebViewFailure } from "./doroti.web.webview.js";

export interface RasterPacket { order: number; bounds: NativeBounds; width: number; height: number; pixels: Uint8Array; bitmap?: ImageBitmap; }
export interface CompositionPacket { batch: NativeBatch; rasters: RasterPacket[]; }

/** Owner-local DOM service. ACK means synchronous DOM/canvas acceptance, never physical presentation. */
export class BrowserPlatformComposition {
  readonly registry: DorotiPlatformViewDomRegistry;
  readonly #web = new Map<string, BrowserWebView>();
  #rasters: HTMLCanvasElement[] = [];
  #closed = false;
  #copiedBytes = 0;
  #commits = 0;
  constructor(readonly root: HTMLElement, readonly canvas: HTMLCanvasElement, owner: string,
    readonly emit: (event: Record<string, unknown>) => void) {
    this.registry = new DorotiPlatformViewDomRegistry(root, owner,
      identity => emit({ identity, focused: true }), event => {
        // Re-dispatch once at the existing root ingress with original coordinates.
        const forwarded = event instanceof WheelEvent ? new WheelEvent(event.type, event) :
          event instanceof PointerEvent ? new PointerEvent(event.type, event) : new Event(event.type);
        if (!root.dispatchEvent(forwarded)) event.preventDefault();
      });
    this.registry.register("doroti/webview", (identity, options) => {
      const web = new BrowserWebView(identity, options as ConstructorParameters<typeof BrowserWebView>[1], emit, root.ownerDocument);
      this.#web.set(identity.id, web);
      return { element: web.element, dispose: () => { web.dispose(); this.#web.delete(identity.id); } };
    });
  }
  async request(payload: Record<string, unknown>): Promise<string> {
    try {
      if (this.#closed) throw new WebViewFailure(1, "DOM owner is closed.");
      const identity = payload.identity as NativeIdentity;
      switch (payload.action) {
        case "create": await this.registry.create(identity, String(payload.viewType), payload.options); return "{}";
        case "remove": await this.registry.remove(identity); return "{}";
        case "hide": this.registry.hide(identity); return "{}";
        case "disable": this.registry.disable(identity); return "{}";
        case "focus": this.registry.focus(identity, Boolean(payload.focused)); return "{}";
        case "command": {
          const web = this.#web.get(identity.id);
          if (!web || identity.owner !== this.registry.owner || web.identity.generation !== identity.generation)
            throw new WebViewFailure(1, "Stale iframe identity.");
          return JSON.stringify(await web.execute(payload.command as Parameters<BrowserWebView["execute"]>[0]));
        }
        default: throw new WebViewFailure(3, "Unknown platform request.");
      }
    } catch (error) {
      return JSON.stringify({ error: error instanceof WebViewFailure ? error.code : 3, message: String(error) });
    }
  }
  commit(packet: CompositionPacket, generation: number): string {
    if (this.#closed || packet.batch.surfaceGeneration !== generation) {
      for (const raster of packet.rasters) raster.bitmap?.close();
      return JSON.stringify({ accepted: false, reason: "Closed/stale composition frame." });
    }
    let bytes = 0, residentBytes = 0;
    const orders = new Set<number>([...packet.batch.views, ...packet.batch.shields, ...(packet.batch.effects ?? [])].map(p => p.order));
    const next: HTMLCanvasElement[] = [];
    const created: HTMLCanvasElement[] = [];
    try {
      // Matches BrowserPlatformViewHost: at most eight slices per planner raster.
      if (packet.rasters.length > 17 * 8) throw new Error("Too many raster slices.");
      for (const raster of packet.rasters) {
        if (!Number.isSafeInteger(raster.width) || !Number.isSafeInteger(raster.height) || raster.width <= 0 || raster.height <= 0 ||
          (raster.pixels.byteLength !== 0 && raster.pixels.byteLength !== raster.width * raster.height * 4) ||
          (raster.bitmap && (raster.bitmap.width !== raster.width || raster.bitmap.height !== raster.height || raster.pixels.byteLength !== 0)) ||
          (residentBytes += raster.width * raster.height * 4) > 64 * 1024 * 1024 ||
          !Number.isSafeInteger(raster.order) || raster.order < 0 || orders.has(raster.order) ||
          !Object.values(raster.bounds).every(Number.isFinite) || raster.bounds.width <= 0 || raster.bounds.height <= 0)
          throw new Error("Invalid/budget-exceeding raster packet.");
        orders.add(raster.order);
        bytes += raster.pixels.byteLength;
        if (raster.pixels.byteLength === 0 && !raster.bitmap) {
          const old = this.#rasters.find(c => c.dataset.dorotiRaster === String(raster.order));
          if (!old || old.width !== raster.width || old.height !== raster.height) throw new Error("Stale raster reuse identity.");
          next.push(old); continue;
        }
        const element = this.root.ownerDocument.createElement("canvas");
        created.push(element);
        element.dataset.dorotiRaster = String(raster.order);
        element.width = raster.width; element.height = raster.height;
        Object.assign(element.style, { position: "absolute", pointerEvents: "none", left: "0px",
          top: "0px", width: `${raster.bounds.width}px`, height: `${raster.bounds.height}px`, zIndex: String(raster.order) });
        if (raster.bitmap) element.getContext("bitmaprenderer")!.transferFromImageBitmap(raster.bitmap);
        else element.getContext("2d")!.putImageData(new ImageData(new Uint8ClampedArray(raster.pixels), raster.width, raster.height), 0, 0);
        next.push(element);
      }
      this.registry.commit(packet.batch, true);
      // No await between the validated placements and raster swap.
      for (let index = 0; index < next.length; index++) {
        const element = next[index], raster = packet.rasters[index];
        element.style.transform = `translate(${raster.bounds.left}px, ${raster.bounds.top}px)`;
        element.style.width = `${raster.bounds.width}px`; element.style.height = `${raster.bounds.height}px`;
        if (!element.isConnected) this.root.append(element);
      }
      for (const old of this.#rasters) if (!next.includes(old)) old.remove();
      this.#rasters = next;
      this.canvas.style.opacity = next.length ? "0" : "1";
      this.root.dataset.dorotiPlatformFrame = String(packet.batch.frame);
      this.root.dataset.dorotiPlatformBytes = String(bytes);
      this.root.dataset.dorotiPlatformTotalBytes = String(this.#copiedBytes += bytes);
      this.root.dataset.dorotiPlatformCommits = String(++this.#commits);
      this.root.dataset.dorotiPlatformResidentBytes = String(residentBytes);
      return JSON.stringify({ accepted: true, frame: packet.batch.frame, observation: "BackendAccepted", bytes });
    } catch (error) {
      for (const element of created) element.remove();
      if (/stale|closed/i.test(String(error))) return JSON.stringify({ accepted: false, reason: String(error) });
      throw error;
    } finally {
      for (const raster of packet.rasters) raster.bitmap?.close();
    }
  }
  async dispose(): Promise<void> {
    if (this.#closed) return;
    this.#closed = true;
    for (const element of this.#rasters) element.remove();
    this.#rasters = []; this.canvas.style.opacity = "1";
    await this.registry.dispose();
  }
}
