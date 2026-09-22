// Generated from doroti.web.textures.ts; verify with the pinned TypeScript declaration emitter.
declare module "*_content/Doroti.Host.Web/doroti.web.textures.js" {
  /** Decimal Int64 wire ID; convert with long.Parse on the owning managed view. */
  export type BrowserTextureId = string;
  export type BrowserTextureFrame = VideoFrame | ImageBitmap;
  export interface TextureEndpoint extends EventTarget {
      postMessage(message: unknown, transfer?: Transferable[]): void;
  }
  export class BrowserTextureError extends Error {
      readonly code: string;
      constructor(code: string, message: string);
  }
  export function texturesForCanvas(canvasId?: string): BrowserTextureRegistry;
  export function attachTextureRegistry(canvasId: string, endpoint: TextureEndpoint): BrowserTextureRegistry;
  /** Owns sources for one render endpoint. No frame payload uses the JSON control channel. */
  export class BrowserTextureRegistry {
      #private;
      readonly endpoint: TextureEndpoint;
      constructor(endpoint: TextureEndpoint);
      /** @internal Includes candidates, unacknowledged transfers and outstanding snapshots. */
      adjustSourceBytes(delta: number): boolean;
      get sourceBytes(): number;
      request(operation: string, payload?: Record<string, unknown>, transfer?: Transferable[], onPosted?: () => void): Promise<Record<string, unknown>>;
      createFrameProducer(): Promise<BrowserTextureEntry>;
      registerCanvas(canvas: HTMLCanvasElement): Promise<BrowserTextureEntry>;
      registerVideo(video: HTMLVideoElement): Promise<BrowserTextureEntry>;
      /** Invoke from a user action. Only this helper owns and stops its stream. */
      startCamera(constraints?: MediaStreamConstraints): Promise<BrowserTextureEntry>;
      forget(id: string): void;
      diagnostics(): Promise<Record<string, unknown>>;
      dispose(): Promise<void>;
      disconnect(): void;
  }
  export class BrowserTextureEntry {
      #private;
      readonly registry: BrowserTextureRegistry;
      readonly textureId: BrowserTextureId;
      readonly generation: number;
      lastError?: Error;
      onError?: (error: Error) => void;
      readonly counters: {
          accepted: number;
          replaced: number;
          posted: number;
          acknowledged: number;
          locallyClosed: number;
      };
      constructor(registry: BrowserTextureRegistry, textureId: BrowserTextureId, generation: number);
      get ready(): boolean;
      get pendingFrames(): number;
      ownCleanup(cleanup: () => void): void;
      /** @internal Error delivery from the owning GPU consumer. */
      notifyError(error: Error): void;
      /** true transfers ownership to the entry, even when later posting fails. false leaves ownership with caller. */
      pushFrame(frame: BrowserTextureFrame): boolean;
      replaceCanvas(canvas: HTMLCanvasElement): void;
      replaceVideo(video: HTMLVideoElement): void;
      markFrameAvailable(): void;
      /** @internal Wake a dirty snapshot when another source returns its budget. */
      retrySnapshot(): void;
      stopLocal(): void;
      dispose(): Promise<void>;
  }
}
