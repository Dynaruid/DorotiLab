import { dorotiProtocolVersion } from "./doroti.web.protocol.js";

/** Decimal Int64 wire ID; convert with long.Parse on the owning managed view. */
export type BrowserTextureId = string;
export type BrowserTextureFrame = VideoFrame | ImageBitmap;
export interface TextureEndpoint extends EventTarget {
  postMessage(message: unknown, transfer?: Transferable[]): void;
}
export class BrowserTextureError extends Error {
  constructor(readonly code: string, message: string) { super(message); this.name = "BrowserTextureError"; }
}
const registries = new Map<string, BrowserTextureRegistry>();
export function texturesForCanvas(canvasId = "doroti-surface"): BrowserTextureRegistry {
  const registry = registries.get(canvasId);
  if (!registry) throw new BrowserTextureError("NotReady", "The owning view is not ready.");
  return registry;
}
export function attachTextureRegistry(canvasId: string, endpoint: TextureEndpoint): BrowserTextureRegistry {
  registries.get(canvasId)?.disconnect();
  const registry = new BrowserTextureRegistry(endpoint);
  registries.set(canvasId, registry);
  return registry;
}

/** Owns sources for one render endpoint. No frame payload uses the JSON control channel. */
export class BrowserTextureRegistry {
  #sequence = 0;
  #disposed = false;
  #sourceBytes = 0;
  #requests = new Map<number, { resolve(value: Record<string, unknown>): void; reject(error: Error): void }>();
  #entries = new Map<string, BrowserTextureEntry>();
  readonly #message = (event: Event): void => {
    const message = (event as MessageEvent).data;
    if (message?.kind === "texture-error" && message.protocolVersion === dorotiProtocolVersion) {
      this.#entries.get(String(message.textureId))?.notifyError(new BrowserTextureError(message.code ?? "Source", String(message.error)));
      return;
    }
    if (message?.kind === "fatal" || message?.kind === "disposed" || message?.kind === "context-lost") {
      this.disconnect(); return;
    }
    if (message?.kind !== "texture-response" || message.protocolVersion !== dorotiProtocolVersion) return;
    const request = this.#requests.get(message.request);
    if (!request) return;
    this.#requests.delete(message.request);
    if (message.error) request.reject(new BrowserTextureError(message.code ?? "Source", message.error));
    else request.resolve(message);
  };
  constructor(readonly endpoint: TextureEndpoint) { endpoint.addEventListener("message", this.#message); }
  /** @internal Includes candidates, unacknowledged transfers and outstanding snapshots. */
  adjustSourceBytes(delta: number): boolean {
    if (delta > 0 && this.#sourceBytes + delta > 64 * 1024 * 1024) return false;
    this.#sourceBytes += delta;
    if (delta < 0 && !this.#disposed) queueMicrotask(() => {
      for (const entry of this.#entries.values()) entry.retrySnapshot();
    });
    return true;
  }
  get sourceBytes(): number { return this.#sourceBytes; }
  request(operation: string, payload: Record<string, unknown> = {}, transfer: Transferable[] = [], onPosted?: () => void): Promise<Record<string, unknown>> {
    if (this.#disposed) return Promise.reject(new BrowserTextureError("Disposed", "Texture view has closed; rebind to a new view."));
    if (this.#requests.size >= 64) return Promise.reject(new BrowserTextureError("Budget", "Texture control limit reached."));
    const request = ++this.#sequence;
    return new Promise((resolve, reject) => {
      // A lost owner cannot leave registration/disposal callers waiting forever.
      const timeout = setTimeout(() => { this.disconnect(); reject(new BrowserTextureError("Timeout", "Texture owner did not acknowledge.")); }, 30000);
      this.#requests.set(request, {
        resolve: value => { clearTimeout(timeout); resolve(value); },
        reject: error => { clearTimeout(timeout); reject(error); },
      });
      try { this.endpoint.postMessage({ protocolVersion: dorotiProtocolVersion, kind: "texture", operation, request, ...payload }, transfer); onPosted?.(); }
      catch (error) {
        const code = typeof DOMException !== "undefined" && error instanceof DOMException ? error.name : "Transfer";
        this.#requests.get(request)!.reject(new BrowserTextureError(code, String(error))); this.#requests.delete(request);
      }
    });
  }
  async createFrameProducer(): Promise<BrowserTextureEntry> {
    const result = await this.request("register");
    const id = String(result.textureId);
    if (!/^[1-9][0-9]*$/.test(id) || BigInt(id) > 9223372036854775807n) throw new BrowserTextureError("Protocol", "Invalid texture ID.");
    const entry = new BrowserTextureEntry(this, id, Number(result.generation));
    this.#entries.set(id, entry);
    return entry;
  }
  async registerCanvas(canvas: HTMLCanvasElement): Promise<BrowserTextureEntry> {
    const entry = await this.createFrameProducer(); entry.replaceCanvas(canvas); return entry;
  }
  async registerVideo(video: HTMLVideoElement): Promise<BrowserTextureEntry> {
    const entry = await this.createFrameProducer();
    try { entry.replaceVideo(video); return entry; } catch (error) { await entry.dispose(); throw error; }
  }
  /** Invoke from a user action. Only this helper owns and stops its stream. */
  async startCamera(constraints: MediaStreamConstraints = { video: true, audio: false }): Promise<BrowserTextureEntry> {
    const stream = await navigator.mediaDevices.getUserMedia(constraints);
    const video = document.createElement("video"); video.muted = true; video.playsInline = true; video.srcObject = stream;
    try {
      await video.play();
      const entry = await this.registerVideo(video);
      entry.ownCleanup(() => { video.pause(); video.srcObject = null; stream.getTracks().forEach(track => track.stop()); });
      return entry;
    } catch (error) { stream.getTracks().forEach(track => track.stop()); video.srcObject = null; throw error; }
  }
  forget(id: string): void { this.#entries.delete(id); }
  async diagnostics(): Promise<Record<string, unknown>> { return this.request("diagnostics"); }
  async dispose(): Promise<void> {
    await Promise.all([...this.#entries.values()].map(entry => entry.dispose())); this.disconnect();
  }
  disconnect(): void {
    if (this.#disposed) return;
    this.#disposed = true;
    for (const entry of this.#entries.values()) entry.stopLocal();
    this.#entries.clear();
    for (const request of this.#requests.values()) request.reject(new BrowserTextureError("Disposed", "Texture owner closed or lost its context."));
    this.#requests.clear(); this.endpoint.removeEventListener("message", this.#message);
    for (const [key, value] of registries) if (value === this) registries.delete(key);
  }
}

export class BrowserTextureEntry {
  #sourceGeneration = 1;
  #sequence = 0;
  #disposed = false;
  #inFlight = false;
  #latest?: BrowserTextureFrame;
  #latestBytes = 0;
  #canvas?: HTMLCanvasElement;
  #snapshot = false;
  #snapshotTask?: Promise<void>;
  #captureVideo?: () => void;
  #videoDirty = false;
  #dirty = false;
  #stopSource?: () => void;
  #ownedCleanup?: () => void;
  #disposePromise?: Promise<void>;
  lastError?: Error;
  onError?: (error: Error) => void;
  readonly counters = { accepted: 0, replaced: 0, posted: 0, acknowledged: 0, locallyClosed: 0 };
  constructor(readonly registry: BrowserTextureRegistry, readonly textureId: BrowserTextureId, readonly generation: number) { }
  get ready(): boolean { return !this.#disposed && !this.#inFlight && !this.#latest; }
  get pendingFrames(): number { return Number(this.#inFlight) + Number(!!this.#latest); }
  ownCleanup(cleanup: () => void): void { this.#ownedCleanup = cleanup; }
  /** @internal Error delivery from the owning GPU consumer. */
  notifyError(error: Error): void { this.#error(error); }
  #error(error: unknown): void {
    this.lastError = error instanceof Error || (typeof DOMException !== "undefined" && error instanceof DOMException)
      ? error : new Error(String(error));
    this.onError?.(this.lastError);
  }
  #close(frame?: BrowserTextureFrame): void { if (frame) { frame.close(); this.counters.locallyClosed++; } }
  /** true transfers ownership to the entry, even when later posting fails. false leaves ownership with caller. */
  pushFrame(frame: BrowserTextureFrame): boolean {
    if (this.#disposed) return false;
    const video = typeof VideoFrame !== "undefined" && frame instanceof VideoFrame;
    if (!video && !(typeof ImageBitmap !== "undefined" && frame instanceof ImageBitmap))
      throw new BrowserTextureError("Unsupported", "Expected VideoFrame or ImageBitmap.");
    const width = video ? frame.displayWidth : (frame as ImageBitmap).width;
    const height = video ? frame.displayHeight : (frame as ImageBitmap).height;
    if (!width || !height || width * height * 4 > 16 * 1024 * 1024)
      throw new BrowserTextureError("Size", "Frame is empty or exceeds the 16 MiB source budget.");
    const size = width * height * 4;
    if (!this.registry.adjustSourceBytes(size - this.#latestBytes)) return false;
    if (this.#latest) { this.#close(this.#latest); this.counters.replaced++; }
    this.#latest = frame; this.#latestBytes = size; this.counters.accepted++; this.#pump(); return true;
  }
  #pump(): void {
    if (this.#disposed || this.#inFlight || !this.#latest) return;
    const frame = this.#latest; this.#latest = undefined; this.#inFlight = true;
    const bytes = this.#latestBytes; this.#latestBytes = 0;
    const sourceGeneration = this.#sourceGeneration;
    let posted = false;
    void this.registry.request("frame", { textureId: this.textureId, generation: this.generation,
      sourceGeneration, sequence: ++this.#sequence, frame }, [frame], () => { posted = true; this.counters.posted++; }).then(() => {
      this.counters.acknowledged++;
    }, error => { if (!posted) this.#close(frame); this.#error(error); }).finally(() => {
      this.#inFlight = false; this.#pump(); this.#snapshotCanvas();
      this.registry.adjustSourceBytes(-bytes);
      if (this.#videoDirty) this.#captureVideo?.();
    });
  }
  #replace(preserveOwnedSource = false): void {
    if (this.#disposed) throw new BrowserTextureError("Disposed", "Texture entry closed.");
    this.#sourceGeneration++; this.#stopSource?.(); this.#stopSource = undefined;
    if (!preserveOwnedSource) { this.#ownedCleanup?.(); this.#ownedCleanup = undefined; }
    this.#canvas = undefined; this.#dirty = false; this.#close(this.#latest); this.#latest = undefined;
    this.registry.adjustSourceBytes(-this.#latestBytes); this.#latestBytes = 0;
    this.#captureVideo = undefined; this.#videoDirty = false;
    void this.registry.request("replace", { textureId: this.textureId, generation: this.generation,
      sourceGeneration: this.#sourceGeneration }).catch(error => this.#error(error));
  }
  replaceCanvas(canvas: HTMLCanvasElement): void { this.#replace(); this.#canvas = canvas; }
  replaceVideo(video: HTMLVideoElement): void {
    this.#bindVideo(video, false);
  }
  #bindVideo(video: HTMLVideoElement, preserveOwnedSource: boolean): void {
    if (typeof video.requestVideoFrameCallback !== "function" || typeof VideoFrame === "undefined")
      throw new BrowserTextureError("Unsupported", "VideoFrame and requestVideoFrameCallback are required.");
    this.#replace(preserveOwnedSource); const generation = this.#sourceGeneration; let callback = 0;
    const capture = (): void => {
      if (this.#disposed || generation !== this.#sourceGeneration || video.readyState < 2 || !this.ready) return;
      this.#videoDirty = false;
      try { const frame = new VideoFrame(video); if (!this.pushFrame(frame)) frame.close(); }
      catch (error) { this.#error(error); }
    };
    const tick: VideoFrameRequestCallback = () => { capture(); callback = video.requestVideoFrameCallback(tick); };
    const captureFinal = (): void => { this.#videoDirty = true; capture(); };
    this.#captureVideo = capture;
    callback = video.requestVideoFrameCallback(tick);
    const reset = (): void => {
      // Invalidate snapshots from the old URL while preserving the borrowed element/stream.
      this.#bindVideo(video, true);
    };
    for (const name of ["loadeddata", "seeked", "pause", "ended"]) video.addEventListener(name, captureFinal);
    video.addEventListener("emptied", reset);
    this.#stopSource = () => {
      video.cancelVideoFrameCallback(callback);
      for (const name of ["loadeddata", "seeked", "pause", "ended"]) video.removeEventListener(name, captureFinal);
      video.removeEventListener("emptied", reset);
    };
    capture();
  }
  markFrameAvailable(): void {
    if (this.#disposed) return;
    this.#dirty = true; this.#snapshotCanvas();
  }
  /** @internal Wake a dirty snapshot when another source returns its budget. */
  retrySnapshot(): void { this.#snapshotCanvas(); }
  #snapshotCanvas(): void {
    if (this.#disposed || !this.#canvas || !this.#dirty || this.#snapshot || !this.ready) return;
    const bytes = this.#canvas.width * this.#canvas.height * 4;
    if (!bytes || bytes > 16 * 1024 * 1024) { this.#dirty = false; this.#error(new BrowserTextureError("Size", "Canvas dimensions exceed the source budget.")); return; }
    if (!this.registry.adjustSourceBytes(bytes)) return;
    const generation = this.#sourceGeneration; this.#snapshot = true; this.#dirty = false;
    this.#snapshotTask = createImageBitmap(this.#canvas, { premultiplyAlpha: "premultiply", colorSpaceConversion: "default" }).then(frame => {
      this.registry.adjustSourceBytes(-bytes);
      if (this.#disposed || generation !== this.#sourceGeneration) this.#close(frame);
      else { try { if (!this.pushFrame(frame)) this.#close(frame); } catch (error) { this.#close(frame); this.#error(error); } }
    }, error => { this.registry.adjustSourceBytes(-bytes); this.#error(error); }).finally(() => { this.#snapshot = false; this.#snapshotCanvas(); });
  }
  stopLocal(): void {
    if (this.#disposed) return;
    this.#disposed = true; this.#sourceGeneration++; this.#stopSource?.(); this.#ownedCleanup?.();
    this.#stopSource = undefined; this.#ownedCleanup = undefined; this.#canvas = undefined; this.#captureVideo = undefined;
    this.#close(this.#latest); this.#latest = undefined;
    this.registry.adjustSourceBytes(-this.#latestBytes); this.#latestBytes = 0;
  }
  dispose(): Promise<void> {
    if (this.#disposePromise) return this.#disposePromise;
    this.stopLocal(); this.registry.forget(this.textureId);
    return this.#disposePromise = Promise.all([this.#snapshotTask, this.registry.request("unregister", { textureId: this.textureId,
      generation: this.generation })]).then(() => {});
  }
}
