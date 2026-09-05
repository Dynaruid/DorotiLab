import { CanvasKitStageTrace } from "./doroti.canvaskit.trace.js";
const stageTrace = new CanvasKitStageTrace();

import type {
  Canvas,
  CanvasKit,
  CanvasKitInitOptions,
  ColorFilter,
  EmbindObject,
  FontCollection,
  GrDirectContext,
  Image,
  ImageFilter,
  MaskFilter,
  Paint,
  Paragraph,
  Path,
  RuntimeEffect,
  Shader,
  SkPicture,
  Surface,
  TextStyle,
  Typeface,
  TypefaceFontProvider,
} from "canvaskit-wasm";
import {
  canvasKitSurfaceGeneration,
  displayListSequenceAsNumber,
  dorotiCanvasKitTopologyVersion,
  dorotiProtocolVersion,
  validateDorotiDisplayList,
  type DorotiSceneTerminal,
  type DorotiValidatedDisplayList,
} from "./doroti.web.protocol.js";

interface CanvasKitRoleContext {
  readonly CanvasKitInit: (options?: CanvasKitInitOptions) => Promise<CanvasKit>;
  readonly canvasKitWasmUrl: string;
  readonly initEnvelope: Readonly<Record<string, unknown>>;
}

interface ResizeEpoch {
  generation: number;
  logicalWidth: number;
  logicalHeight: number;
  physicalWidth: number;
  physicalHeight: number;
  devicePixelRatio: number;
  timestampMicroseconds: number;
}

interface RasterScene {
  readonly sequence: number;
  readonly transferId: number;
  readonly buffer: ArrayBuffer;
  readonly document: DorotiValidatedDisplayList;
  terminal: boolean;
  attempted: boolean;
  receipt: boolean;
}

type RasterResourceObject = Typeface | Image | RuntimeEffect | SkPicture;

interface RasterResource {
  readonly resourceId: number;
  readonly generation: number;
  readonly kind: string;
  readonly descriptorJson: string;
  readonly object: RasterResourceObject;
  readonly bytes: Uint8Array;
  readonly fingerprint: string;
  readonly byteLength: number;
}

interface DisplayResourceReference {
  readonly kind: number;
  readonly version: number;
  readonly id: bigint;
}

interface OwnedObject<T extends EmbindObject<string>> {
  readonly kind: string;
  readonly object: T;
}

interface OwnedImageFilter extends OwnedObject<ImageFilter> {
  readonly cropBounds?: Float32Array | null;
}

interface ReplayStackFrame {
  readonly kind: "save" | "layer" | "shader-mask";
  readonly shader?: OwnedObject<Shader>;
  readonly maskRect?: Float32Array;
  readonly blendMode?: number;
  readonly restoreCount?: number;
}

interface RuntimeEffectImageFilterRecipe {
  readonly effect: RuntimeEffect;
  readonly uniforms: Float32Array;
  readonly childImages: readonly Image[];
  readonly sampling: number;
}

interface ParsedParagraph {
  readonly text: string;
  readonly font: DisplayResourceReference;
  readonly fontFamily: string;
  readonly locale: string;
  readonly ellipsis: string | null;
  readonly fontSize: number;
  readonly heightMultiplier: number;
  readonly color: number;
  readonly fontWeight: number;
  readonly fontSlant: number;
  readonly direction: number;
  readonly align: number;
  readonly maxLines: number;
  readonly layoutWidth: number;
  readonly measuredWidth: number;
  readonly measuredHeight: number;
  readonly metricsHash: bigint;
  readonly fallbackFonts: readonly DisplayResourceReference[];
  readonly textRuns: readonly ParsedParagraphTextRun[];
}

interface ParsedParagraphTextRun {
  readonly text: string;
  readonly fontFamily: string;
  readonly locale: string;
  readonly fontSize: number;
  readonly heightMultiplier: number;
  readonly color: number;
  readonly fontWeight: number;
  readonly fontSlant: number;
  readonly decoration: number;
  readonly backgroundColor: number | null;
  readonly decorationColor: number | null;
  readonly decorationStyle: number | null;
  readonly decorationThickness: number | null;
  readonly textBaseline: number | null;
  readonly letterSpacing: number | null;
  readonly wordSpacing: number | null;
  readonly halfLeading: boolean | null;
  readonly fontFamilyFallback: readonly string[];
  readonly shadows: readonly { color: number; dx: number; dy: number; blurRadius: number }[];
  readonly fontFeatures: readonly { name: string; value: number }[];
  readonly fontVariations: readonly { axis: string; value: number }[];
}

interface ParagraphFontCollection {
  readonly provider: OwnedObject<TypefaceFontProvider>;
  readonly collection: OwnedObject<FontCollection>;
}

interface CachedParagraph {
  readonly paragraph: OwnedObject<Paragraph>;
  readonly fontCollectionKey: string;
}

interface CachedImageFilter {
  readonly filter: OwnedImageFilter;
  readonly consumedBytes: number;
}

interface ImageFilterLease {
  readonly object: ImageFilter;
  readonly cropBounds?: Float32Array | null;
  readonly transient: OwnedImageFilter | null;
}

interface ImageFilterSurfaceLease {
  readonly object: Surface;
  readonly transient: OwnedObject<Surface> | null;
}

interface ParagraphGraphemeSnapshot {
  readonly start: number;
  readonly end: number;
  readonly left: number;
  readonly top: number;
  readonly right: number;
  readonly bottom: number;
  readonly strutTop: number;
  readonly strutBottom: number;
  readonly direction: "ltr" | "rtl";
}

interface ReplayContext {
  readonly kit: CanvasKit;
  readonly canvas: Canvas;
  readonly view: DataView;
  readonly strings: readonly string[];
  readonly declaredResources: ReadonlyMap<string, string>;
  readonly resourceDeclarationKey: string;
}

const protocolVersion = dorotiProtocolVersion;
const topologyVersion = dorotiCanvasKitTopologyVersion;
const blendModeNames = [
  "Clear", "Src", "Dst", "SrcOver", "DstOver", "SrcIn", "DstIn", "SrcOut", "DstOut",
  "SrcATop", "DstATop", "Xor", "Plus", "Modulate", "Screen", "Overlay", "Darken", "Lighten",
  "ColorDodge", "ColorBurn", "HardLight", "SoftLight", "Difference", "Exclusion", "Multiply",
  "Hue", "Saturation", "Color", "Luminosity",
] as const;

export const dorotiCanvasKitImplementedOpcodes = Object.freeze([
  1, 2, 3, 4, 5, 6, 7,
  16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
  48, 49, 50, 51, 52, 53,
] as const);

const radiansToDegrees = 180 / Math.PI;
const maximumCollectionCount = 1_000_000;
const maximumNestingDepth = 32;
const maximumImageFilterSurfacePoolSize = 8;
const maximumResizeTargetHistory = 64;
const maximumParagraphCacheSize = 256;
const maximumParagraphFontCollectionCacheSize = 16;
const maximumPaintCacheSize = 512;
const maximumCachedPaintWireBytes = 4096;
const maximumImageFilterCacheSize = 128;
const maximumCachedImageFilterWireBytes = 8192;
const minimumResizeStagingGrowthPixels = 64;
const maximumResizeStagingGrowthPixels = 256;
// Prefer a newer immutable resize target when a DisplayList is materially
// behind. One-generation-progressive scenes are usually the freshest managed
// output available; skipping those costs another managed frame. After an
// expensive Raster replay, older scenes may be skipped briefly, but are still
// admitted after the age budget so continuous resize cannot starve the front.
const latestTargetPriorityMinimumGenerationGap = 2;
const latestTargetPriorityMaximumFrontAgeMilliseconds = 40;
const latestTargetPriorityMinimumPriorReplayMilliseconds = 8;
const displayResourceKinds = ["", "font", "image", "runtime-effect", "retained-scene"] as const;

let canvasKit: CanvasKit | null = null;
let canvas: OffscreenCanvas | null = null;
let port: MessagePort | null = null;
let sessionId = 0;
let rasterSessionId = 0;
let contextHandle = 0;
let grContext: GrDirectContext | null = null;
let surface: Surface | null = null;
let resizeStagingSurface: Surface | null = null;
let resizeCommitPaint: Paint | null = null;
let contextGeneration = 1;
let surfaceGeneration = 0;
let resizeTarget: ResizeEpoch | null = null;
const resizeTargets = new Map<number, ResizeEpoch>();
let frontPhysicalWidth = 0;
let frontPhysicalHeight = 0;
let contextLost = false;
let contextLossExtension: WEBGL_lose_context | null = null;
let currentScene: RasterScene | null = null;
let latestScene: RasterScene | null = null;
let draining = false;
let drainScheduling = "microtask";
let presentation = "direct";
let frameMarker = false;
let rasterDisposed = false;
const bitmapCredits = new Set<number>();
const bitmapBudgetBytes = 128 * 1024 * 1024;
let bitmapCreated = 0;
let bitmapAcknowledged = 0;
let queueHighWater = 0;
let admittedScenes = 0;
let terminalScenes = 0;
let rasterAttempts = 0;
let rasterReceipts = 0;
let failedScenes = 0;
let lastFailedSceneSequence = 0;
let lastFailureReason = "";
let submittedScenes = 0;
let supersededScenes = 0;
let flushCount = 0;
let lastFrontGeneration = 0;
let lastFrontCommitMilliseconds = 0;
let latestTargetPrioritySkippedScenes = 0;
let latestTargetPriorityForcedProgressiveScenes = 0;
let latestTargetPriorityMaximumSkippedGenerationGap = 0;
let mainFastLaneResizeTargetCount = 0;
let uiOrderedResizeTargetCount = 0;
let referenceCompatibilityAddSuperellipseNoOps = 0;
let appliedBlurCropBounds = 0;
let diagnosticRasterStallCount = 0;
let lastDiagnosticRasterStallMilliseconds = 0;
const resources = new Map<string, RasterResource>();
const objectCounters = new Map<string, { created: number; deleted: number; live: number; bytes: number }>();
const imageFilterSurfacePool: Surface[] = [];
const paragraphCache = new Map<string, CachedParagraph>();
const paragraphFontCollectionCache = new Map<string, ParagraphFontCollection>();
const paintCache = new Map<string, OwnedObject<Paint>>();
const imageFilterCache = new Map<string, CachedImageFilter>();
let nextImageFilterSurfaceSlot = 0;
let paragraphCacheHits = 0;
let paragraphCacheMisses = 0;
let paragraphCacheEvictions = 0;
let paragraphCacheInvalidations = 0;
let paragraphFontCollectionCacheEvictions = 0;
let paintCacheHits = 0;
let paintCacheMisses = 0;
let paintCacheEvictions = 0;
let paintCacheInvalidations = 0;
let imageFilterCacheHits = 0;
let imageFilterCacheMisses = 0;
let imageFilterCacheEvictions = 0;
let imageFilterCacheInvalidations = 0;
let replayCount = 0;
let replayTotalMilliseconds = 0;
let replayLastMilliseconds = 0;
let replayMaximumMilliseconds = 0;
let resizeStagingCount = 0;
let resizeStagingTotalMilliseconds = 0;
let resizeStagingLastMilliseconds = 0;
let resizeStagingMaximumMilliseconds = 0;
let resizeStagingSurfaceAllocations = 0;
let resizeStagingSurfaceReuses = 0;
let resizeStagingCapacityWidth = 0;
let resizeStagingCapacityHeight = 0;
let resizeStagingPeakPixels = 0;

function post(kind: string, payload: Record<string, unknown> = {}, transfer: Transferable[] = []): void {
  (globalThis as unknown as { postMessage(message: unknown, transfer: Transferable[]): void }).postMessage({
    protocolVersion, topologyVersion, role: "raster", sessionId, rasterSessionId, kind, ...payload,
  }, transfer);
}

function postPort(kind: string, payload: Record<string, unknown> = {}, transfer: Transferable[] = []): void {
  if (!port) throw new Error("Doroti CanvasKit Raster port is unavailable.");
  port.postMessage({ protocolVersion, topologyVersion, rasterSessionId, kind, ...payload }, transfer);
}

export async function startCanvasKitRole(context: CanvasKitRoleContext): Promise<void> {
  stageTrace.enabled = context.initEnvelope.stageTrace === true;
  drainScheduling = String(context.initEnvelope.drainScheduling ?? "microtask");
  if (!["microtask", "raf"].includes(drainScheduling)) throw new Error("Unknown Raster drain scheduling");
  if (drainScheduling === "raf" && typeof globalThis.requestAnimationFrame !== "function")
    throw new Error("P1 requires Worker requestAnimationFrame");
  const envelope = context.initEnvelope;
  presentation = String(envelope.presentation ?? "direct");
  frameMarker = envelope.frameMarker === true;
  if (!["direct", "bitmap-crop", "bitmap-exact"].includes(presentation)) throw new Error("Unknown CanvasKit presentation");
  if (envelope.role !== "raster") throw new Error("Doroti Raster role received a non-Raster bootstrap envelope.");
  sessionId = positiveInteger(envelope.sessionId, "sessionId");
  rasterSessionId = positiveInteger(envelope.rasterSessionId, "rasterSessionId");
  contextGeneration = rasterSessionId;
  if (!(envelope.canvas instanceof OffscreenCanvas))
    throw new Error("Doroti CanvasKit Raster role requires one transferred visible OffscreenCanvas.");
  canvas = envelope.canvas;
  port = requireMessagePort(envelope.rasterPort);
  resizeTarget = envelope.resizeEpoch as ResizeEpoch;
  validateResizeTarget(resizeTarget);
  rememberResizeTarget(resizeTarget);
  if (canvas.width < resizeTarget.physicalWidth || canvas.height < resizeTarget.physicalHeight)
    throw new Error(
      `Doroti CanvasKit initial visible capacity ${canvas.width}x${canvas.height} is smaller than ` +
      `${resizeTarget.physicalWidth}x${resizeTarget.physicalHeight}.`);
  installPort();
  installGlobalListener();

  const started = performance.now();
  canvasKit = await context.CanvasKitInit({
    locateFile(file) {
      if (file === "canvaskit.wasm" || file.endsWith("/canvaskit.wasm")) return context.canvasKitWasmUrl;
      throw new Error(`Doroti CanvasKit Raster role rejected unexpected runtime file '${file}'.`);
    },
  });
  contextHandle = canvasKit.GetWebGLContext(canvas, {
    alpha: 1,
    antialias: 0,
    depth: 1,
    stencil: 8,
    premultipliedAlpha: 1,
    preserveDrawingBuffer: 0,
    failIfMajorPerformanceCaveat: 1,
    majorVersion: 2,
    minorVersion: 0,
    enableExtensionsByDefault: 1,
  });
  if (!contextHandle)
    throw new Error("Doroti CanvasKit requires an explicit hardware OffscreenCanvas WebGL2 context.");
  grContext = canvasKit.MakeWebGLContext(contextHandle);
  if (!grContext) throw new Error("Doroti CanvasKit MakeWebGLContext failed; software fallback is forbidden.");
  if (presentation !== "direct") grContext.setResourceCacheLimitBytes(64 * 1024 * 1024);
  countCreated("GrDirectContext", 0);
  const gl = canvas.getContext("webgl2");
  if (!gl) throw new Error("Doroti CanvasKit context is not WebGL2.");
  const gpu = gpuIdentity(gl);
  contextLossExtension = gl.getExtension("WEBGL_lose_context");
  canvas.addEventListener("webglcontextlost", (event) => {
    event.preventDefault();
    contextLost = true;
    if (currentScene) failScene(currentScene, "CanvasKit WebGL context lost", true);
    if (latestScene) failScene(latestScene, "CanvasKit WebGL context lost", false);
    currentScene = null;
    latestScene = null;
    post("context-lost", { contextGeneration });
    postPort("fatal", { error: "CanvasKit WebGL context lost; transferred canvas replacement required" });
  });
  canvas.addEventListener("webglcontextrestored", () => {
    postPort("fatal", { error: "CanvasKit context restore requires a fresh canvas lease and Raster Worker" });
  });
  ensureSurface(resizeTarget);
  const ready = {
    gpu: { ...gpu, contextGeneration, surfaceGeneration },
    contextGeneration,
    surfaceGeneration,
    rasterSessionId,
    canvasKitOwnerCount: 1,
    managedRuntimeCount: 0,
    visibleCanvasContextOwnerCount: 1,
    softwareFallbackUsed: false,
    initMicroseconds: Math.round((performance.now() - started) * 1000),
  };
  post("gpu-ready", ready);
  postPort("raster-ready", ready);
  publishDiagnostics();
}

function installPort(): void {
  port!.addEventListener("message", (event: MessageEvent) => {
    const message = event.data as Record<string, unknown> | null;
    if (!message || message.protocolVersion !== protocolVersion ||
        message.topologyVersion !== topologyVersion || message.rasterSessionId !== rasterSessionId) {
      postPort("fatal", { error: "Raster port protocol violation" });
      return;
    }
    try {
      switch (message.kind) {
        case "resize-target": {
          const next = message.resizeEpoch as ResizeEpoch;
          validateResizeTarget(next);
          rememberResizeTarget(next);
          uiOrderedResizeTargetCount++;
          break;
        }
        case "display-list":
          admitScene(message);
          break;
        case "retain-resource":
          retainResource(message);
          break;
        case "release-resource":
          releaseResource(message);
          break;
        case "shutdown":
          disposeRasterRole();
          break;
        default:
          throw new Error(`Unknown Raster port message '${String(message.kind)}'.`);
      }
    } catch (error) {
      postPort("fatal", { error: String(error instanceof Error ? error.stack ?? error.message : error) });
    }
    publishDiagnostics();
  });
  port!.start();
}

function installGlobalListener(): void {
  globalThis.addEventListener("message", (event: MessageEvent) => {
    const message = event.data as Record<string, unknown> | null;
    if (!message) return;
    if (message.protocolVersion !== protocolVersion) {
      if (message.kind === "context" || message.kind === "crash" || message.kind === "dispose" ||
          message.kind === "resize-target-fast-lane" ||
          message.kind === "stall-raster-100ms") {
        const error = `Raster global protocol violation: ${String(message.protocolVersion)}`;
        post("fatal", { error });
        if (port) postPort("fatal", { error });
      }
      return;
    }
    try {
      switch (message.kind) {
        case "resize-target-fast-lane": {
          if (message.topologyVersion !== topologyVersion ||
              Number(message.rasterSessionId) !== rasterSessionId)
            break;
          const next = message.resizeEpoch as ResizeEpoch;
          validateResizeTarget(next);
          rememberResizeTarget(next);
          stageTrace.record("raster-target-received", next.generation);
          mainFastLaneResizeTargetCount++;
          break;
        }
        case "bitmap-consumed":
          if (message.topologyVersion !== topologyVersion || Number(message.rasterSessionId) !== rasterSessionId) break;
          if (!bitmapCredits.delete(Number(message.requestId))) throw new Error("Duplicate/unknown bitmap acknowledgement");
          bitmapAcknowledged++;
          scheduleDrain();
          publishDiagnostics();
          break;
        case "collect-stage-trace":
          if (message.topologyVersion === topologyVersion && Number(message.rasterSessionId) === rasterSessionId)
            post("stage-trace", { collectionId: message.collectionId, trace: stageTrace.snapshot() });
          break;
        case "context":
          if (message.action === "lose") contextLossExtension?.loseContext();
          else contextLossExtension?.restoreContext();
          break;
        case "crash":
          post("fatal", { error: "diagnostic Raster Worker crash" });
          postPort("fatal", { error: "diagnostic Raster Worker crash" });
          break;
        case "stall-raster-100ms": {
          const started = performance.now();
          while (performance.now() - started < 100) {
            // Intentional test-only Raster stall: proves UI heartbeat/input independence.
          }
          lastDiagnosticRasterStallMilliseconds = performance.now() - started;
          diagnosticRasterStallCount++;
          publishDiagnostics();
          break;
        }
        case "dispose":
          disposeRasterRole();
          break;
      }
    } catch (error) {
      post("fatal", { error: String(error) });
    }
  });
}

function admitScene(message: Record<string, unknown>): void {
  stageTrace.record("raster-scene-received", 0, Number(message.sceneSequence));
  const buffer = message.buffer;
  if (!(buffer instanceof ArrayBuffer))
    throw new Error("Doroti CanvasKit display-list message requires a transferred ArrayBuffer.");
  const transferId = positiveInteger(message.transferId, "transferId");
  let document: DorotiValidatedDisplayList;
  let sequence = Number(message.sceneSequence);
  try {
    document = validateDorotiDisplayList(buffer);
    sequence = displayListSequenceAsNumber(document.metadata.sceneSequence);
    if (sequence !== Number(message.sceneSequence) || buffer.byteLength !== Number(message.byteLength))
      throw new Error("Doroti CanvasKit DisplayList envelope/header identity mismatch.");
  } catch (error) {
    postPort("scene-terminal", {
      sceneSequence: Number.isSafeInteger(sequence) && sequence > 0 ? sequence : 0,
      terminal: "failed",
      reason: String(error),
      transferId,
      buffer,
    }, [buffer]);
    return;
  }
  const scene: RasterScene = {
    sequence, transferId, buffer, document, terminal: false, attempted: false, receipt: false,
  };
  stageTrace.record("raster-decoded", Number(document.metadata.resizeEpoch), sequence);
  admittedScenes++;
  if (latestScene) terminalScene(latestScene, "superseded", "Raster current+latest mailbox replaced pending scene", true);
  latestScene = scene;
  queueHighWater = Math.max(queueHighWater, Number(currentScene !== null) + Number(latestScene !== null));
  scheduleDrain();
}

function scheduleDrain(): void {
  if (draining || currentScene || contextLost || rasterDisposed || !latestScene || bitmapCredits.size >= 2) return;
  draining = true;
  const drain = async () => {
    try {
      while (!contextLost && !rasterDisposed && !currentScene && latestScene && bitmapCredits.size < 2) {
        currentScene = latestScene;
        latestScene = null;
        const completion = render(currentScene);
        if (completion) await completion;
        currentScene = null;
      }
    } finally {
      draining = false;
      if (!currentScene && latestScene && !contextLost) scheduleDrain();
      publishDiagnostics();
    }
  };
  if (drainScheduling === "raf") globalThis.requestAnimationFrame(drain);
  else queueMicrotask(drain);
}

function render(scene: RasterScene): void | Promise<void> {
  stageTrace.record("raster-start", Number(scene.document.metadata.resizeEpoch), scene.sequence);
  scene.attempted = true;
  rasterAttempts++;
  try {
    const latestTarget = requireResizeTarget();
    const metadata = scene.document.metadata;
    const sceneGeneration = exactResizeGeneration(metadata.resizeEpoch);
    const target = resizeTargets.get(sceneGeneration);
    if (!target || sceneGeneration > latestTarget.generation ||
        sceneGeneration < lastFrontGeneration ||
        (sceneGeneration === lastFrontGeneration && sceneGeneration < latestTarget.generation)) {
      terminalScene(scene, "superseded", "DisplayList resize target is no longer presentable", true);
      return;
    }
    if (metadata.resizeEpoch !== BigInt(target.generation) ||
        metadata.physicalWidth !== target.physicalWidth || metadata.physicalHeight !== target.physicalHeight ||
        Math.abs(metadata.logicalWidth - target.logicalWidth) > 0.01 ||
        Math.abs(metadata.logicalHeight - target.logicalHeight) > 0.01 ||
        Math.abs(metadata.devicePixelRatio - target.devicePixelRatio) > 0.001) {
      terminalScene(scene, "superseded", "DisplayList no longer matches the immutable resize target", true);
      return;
    }
    if (metadata.contextGeneration !== BigInt(contextGeneration)) {
      terminalScene(scene, "failed", "DisplayList context generation is stale", true);
      return;
    }
    const expectedSurfaceGeneration = canvasKitSurfaceGeneration(rasterSessionId, target.generation);
    if (metadata.surfaceGeneration !== BigInt(expectedSurfaceGeneration)) {
      terminalScene(scene, "failed", "DisplayList surface generation is stale", true);
      return;
    }
    const progressive = target.generation < latestTarget.generation;
    if (progressive && lastFrontCommitMilliseconds > 0) {
      const generationGap = latestTarget.generation - target.generation;
      const frontAgeMilliseconds = performance.now() - lastFrontCommitMilliseconds;
      const rasterOverloaded = replayLastMilliseconds >= latestTargetPriorityMinimumPriorReplayMilliseconds;
      if (generationGap >= latestTargetPriorityMinimumGenerationGap && rasterOverloaded &&
          frontAgeMilliseconds < latestTargetPriorityMaximumFrontAgeMilliseconds) {
        latestTargetPrioritySkippedScenes++;
        latestTargetPriorityMaximumSkippedGenerationGap = Math.max(
          latestTargetPriorityMaximumSkippedGenerationGap,
          generationGap);
        terminalScene(
          scene,
          "superseded",
          "Latest-target overload policy skipped a progressive DisplayList before replay",
          true);
        return;
      }
      if (generationGap >= latestTargetPriorityMinimumGenerationGap && rasterOverloaded)
        latestTargetPriorityForcedProgressiveScenes++;
    }
    const visible = requireCanvas();
    if (presentation !== "direct") {
      const grows = visible.width < target.physicalWidth || visible.height < target.physicalHeight;
      const nextWidth = presentation === "bitmap-exact" ? target.physicalWidth :
        resizeStagingCapacity(visible.width, target.physicalWidth);
      const nextHeight = presentation === "bitmap-exact" ? target.physicalHeight :
        resizeStagingCapacity(visible.height, target.physicalHeight);
      // Check before any growth: old/new output during replacement, staging,
      // two in-flight exports and the main bitmaprenderer backing.
      const pixels = nextWidth * nextHeight + 3 * target.physicalWidth * target.physicalHeight +
        (grows ? visible.width * visible.height + nextWidth * nextHeight :
          resizeStagingCapacityWidth * resizeStagingCapacityHeight);
      if (pixels * 4 > bitmapBudgetBytes) throw new Error("P2 backing/staging/bitmap budget exceeded before allocation");
    }
    if (presentation === "bitmap-exact" &&
        (visible.width !== target.physicalWidth || visible.height !== target.physicalHeight)) {
      if (4 * target.physicalWidth * target.physicalHeight * 4 > bitmapBudgetBytes)
        throw new Error("P2 exact bitmap/backing budget exceeded");
      // This surface is independent of the visible bitmaprenderer. Recreate
      // exact backing as the P2a allocation experiment before drawing a frame.
      if (surface) { surface.delete(); countDeleted("Surface", 0); surface = null; }
      visible.width = target.physicalWidth;
      visible.height = target.physicalHeight;
    }
    if (visible.width < target.physicalWidth || visible.height < target.physicalHeight)
      renderThroughResizeStaging(scene, target);
    else {
      ensureSurface(target);
      replayIntoVisibleCapacity(scene, target, requireSurface());
      requireSurface().flush();
    }
    if (frameMarker) {
      drawFrameMarker(target);
      requireSurface().flush();
    }
    flushCount++;
    stageTrace.record("gpu-submit", target.generation, scene.sequence);
    lastFrontGeneration = target.generation;
    lastFrontCommitMilliseconds = performance.now();
    frontPhysicalWidth = target.physicalWidth;
    frontPhysicalHeight = target.physicalHeight;
    pruneResizeTargets();
    receiptScene(scene, true, "CanvasKit surface.flush submitted GPU work");
    terminalScene(scene, "submitted", "CanvasKit exact surface GPU work submitted", false);
    const commit = {
      requestId: scene.sequence,
      transferId: scene.transferId,
      generation: target.generation,
      contextGeneration,
      surfaceGeneration,
      physicalWidth: target.physicalWidth,
      physicalHeight: target.physicalHeight,
      logicalWidth: target.logicalWidth,
      logicalHeight: target.logicalHeight,
      devicePixelRatio: target.devicePixelRatio,
      capacityWidth: presentation === "direct" ? visible.width : target.physicalWidth,
      capacityHeight: presentation === "direct" ? visible.height : target.physicalHeight,
      targetGeneration: latestTarget.generation,
      progressive,
      commitEpochMilliseconds: performance.timeOrigin + performance.now(),
    };
    if (presentation === "direct") post("direct-commit", commit);
    else {
      // Two exported bitmaps plus raster/display backing. GPU resource caches
      // have separate budgets; this is an explicit prototype bitmap envelope.
      if ((visible.width * visible.height + 3 * target.physicalWidth * target.physicalHeight) * 4 > bitmapBudgetBytes)
        throw new Error("P2 bitmap/backing budget exceeded");
      bitmapCredits.add(scene.sequence);
      const started = performance.now();
      return createImageBitmap(visible, 0, 0, target.physicalWidth, target.physicalHeight).then(bitmap => {
        if (rasterDisposed || contextLost) { bitmap.close(); bitmapCredits.delete(scene.sequence); return; }
        bitmapCreated++;
        stageTrace.record("bitmap-created", target.generation, scene.sequence, { milliseconds: performance.now() - started });
        try { post("bitmap-commit", { ...commit, bitmap }, [bitmap]); }
        catch (error) { bitmap.close(); throw error; }
      }).catch(error => {
        bitmapCredits.delete(scene.sequence);
        post("fatal", { error: `P2 bitmap export failed: ${String(error)}` });
      });
    }
  } catch (error) {
    if (scene.terminal) { post("fatal", { error: String(error) }); return; }
    if (!scene.receipt) receiptScene(scene, false, String(error));
    terminalScene(scene, "failed", String(error), false);
  }
}

function drawFrameMarker(target: ResizeEpoch): void {
  const targetCanvas = requireSurface().getCanvas();
  const paint = new (requireCanvasKit().Paint)();
  const values = [target.generation, target.physicalWidth, target.physicalHeight];
  const colors = [[0, 1, 1, 1], [1, 0, 1, 1], [0, 1, 1, 1], [1, 0, 1, 1]];
  for (let field = 0; field < values.length; field++)
    for (let bit = 0; bit < (field === 0 ? 32 : 16); bit++)
      colors.push((values[field] >>> bit) & 1 ? [0, 1, 0, 1] : [0, 0, 0, 1]);
  const save = targetCanvas.save();
  try {
    const inverse = requireCanvasKit().Matrix.invert(targetCanvas.getTotalMatrix());
    if (!inverse) throw new Error("Frame marker requires an invertible canvas transform");
    targetCanvas.concat(inverse);
    paint.setAntiAlias(false);
    colors.forEach((color, index) => {
      paint.setColor(new Float32Array(color));
      targetCanvas.drawRect(new Float32Array([32 + index * 4, 32, 36 + index * 4, 36]), paint);
    });
  } finally { targetCanvas.restoreToCount(save); paint.delete(); }
}

function replaySupportedCommands(scene: RasterScene, targetSurface: Surface): void {
  const started = performance.now();
  const kit = requireCanvasKit();
  const targetCanvas = targetSurface.getCanvas();
  nextImageFilterSurfaceSlot = 0;
  const declaredResources = readDeclaredResources(scene);
  const context: ReplayContext = {
    kit,
    canvas: targetCanvas,
    view: new DataView(scene.buffer),
    strings: readStringTable(scene),
    declaredResources,
    resourceDeclarationKey: JSON.stringify([...declaredResources]),
  };
  const rootSaveCount = targetCanvas.getSaveCount();
  targetCanvas.save();
  // RenderView already records its device-pixel-ratio root transform in the
  // scene command stream. The CanvasKit surface is physical-sized, so applying
  // the metadata DPR again here would scale every scene twice.
  try { replayCommandRange(scene, context, 0, scene.document.commands.length); }
  finally {
    targetCanvas.restoreToCount(rootSaveCount);
    replayLastMilliseconds = performance.now() - started;
    replayTotalMilliseconds += replayLastMilliseconds;
    replayMaximumMilliseconds = Math.max(replayMaximumMilliseconds, replayLastMilliseconds);
    replayCount++;
  }
}

function replayIntoVisibleCapacity(
  scene: RasterScene,
  target: ResizeEpoch,
  targetSurface: Surface,
): void {
  const kit = requireCanvasKit();
  const targetCanvas = targetSurface.getCanvas();
  // The DOM root clips this grow-only backing. Clear bands removed by a
  // shrinking front so a later root growth can reveal only transparent pixels
  // until its matching immutable frame is committed.
  if (frontPhysicalWidth > target.physicalWidth) {
    clearVisibleCapacityRect(
      targetCanvas,
      target.physicalWidth, 0,
      frontPhysicalWidth, frontPhysicalHeight);
  }
  if (frontPhysicalHeight > target.physicalHeight) {
    clearVisibleCapacityRect(
      targetCanvas,
      0, target.physicalHeight,
      Math.min(target.physicalWidth, frontPhysicalWidth), frontPhysicalHeight);
  }

  const rootSaveCount = targetCanvas.getSaveCount();
  targetCanvas.save();
  try {
    targetCanvas.clipRect(
      kit.LTRBRect(0, 0, target.physicalWidth, target.physicalHeight),
      kit.ClipOp.Intersect,
      false);
    targetCanvas.clear(kit.TRANSPARENT);
    replaySupportedCommands(scene, targetSurface);
  } finally {
    targetCanvas.restoreToCount(rootSaveCount);
  }
}

function clearVisibleCapacityRect(
  targetCanvas: Canvas,
  left: number,
  top: number,
  right: number,
  bottom: number,
): void {
  if (right <= left || bottom <= top) return;
  const kit = requireCanvasKit();
  const rootSaveCount = targetCanvas.getSaveCount();
  targetCanvas.save();
  try {
    targetCanvas.clipRect(kit.LTRBRect(left, top, right, bottom), kit.ClipOp.Intersect, false);
    targetCanvas.clear(kit.TRANSPARENT);
  } finally {
    targetCanvas.restoreToCount(rootSaveCount);
  }
}

function renderThroughResizeStaging(scene: RasterScene, target: ResizeEpoch): void {
  const started = performance.now();
  const kit = requireCanvasKit();
  const staging = acquireResizeStagingSurface(target.physicalWidth, target.physicalHeight);
  let snapshot: OwnedObject<Image> | null = null;
  let nextSurface: OwnedObject<Surface> | null = null;
  let visibleReset = false;
  try {
    const stagingCanvas = staging.getCanvas();
    const stagingRootSaveCount = stagingCanvas.getSaveCount();
    stagingCanvas.save();
    try {
      // The reusable render target can be larger than this immutable resize
      // epoch. Clear and replay only the exact target rectangle so stale pool
      // pixels can never enter the snapshot or consume unnecessary fill work.
      stagingCanvas.clipRect(
        kit.LTRBRect(0, 0, target.physicalWidth, target.physicalHeight),
        kit.ClipOp.Intersect,
        false);
      stagingCanvas.clear(kit.TRANSPARENT);
      replaySupportedCommands(scene, staging);
    } finally {
      stagingCanvas.restoreToCount(stagingRootSaveCount);
    }
    // Snapshot the whole pooled texture so Skia can retain its native GPU
    // representation. The destination copy below selects only this epoch's
    // exact rectangle; pixels outside it are never presented.
    snapshot = own("ResizeStagingImage", staging.makeImageSnapshot());

    const visible = requireCanvas();
    const capacityWidth = resizeStagingCapacity(visible.width, target.physicalWidth);
    const capacityHeight = resizeStagingCapacity(visible.height, target.physicalHeight);
    if (visible.width !== capacityWidth) visible.width = capacityWidth;
    if (visible.height !== capacityHeight) visible.height = capacityHeight;
    visibleReset = true;
    const createdVisible = kit.MakeOnScreenGLSurface(
      requireGrContext(), capacityWidth, capacityHeight, kit.ColorSpace.SRGB);
    if (!createdVisible)
      throw new Error("CanvasKit could not grow the on-screen GPU surface capacity after resize.");
    nextSurface = own<Surface>("Surface", createdVisible);
    const nextCanvas = nextSurface.object.getCanvas();
    const exactRect = kit.LTRBRect(0, 0, target.physicalWidth, target.physicalHeight);
    nextCanvas.drawImageRectOptions(
      snapshot.object,
      exactRect,
      exactRect,
      kit.FilterMode.Nearest,
      kit.MipmapMode.None,
      requireResizeCommitPaint());
    // makeImageSnapshot preserves the same-GrDirectContext dependency graph.
    // This single destination flush submits both staging replay and the exact
    // on-screen copy. Subsequent frames render directly into this grow-only
    // capacity and avoid another backing reset or copy.
    nextSurface.object.flush();

    const priorSurface = surface;
    surface = nextSurface.object;
    nextSurface = null;
    surfaceGeneration = canvasKitSurfaceGeneration(rasterSessionId, target.generation);
    if (priorSurface) {
      priorSurface.delete();
      countDeleted("Surface", 0);
    }
  } catch (error) {
    if (visibleReset) {
      const reason = `CanvasKit visible resize commit failed closed: ${String(error)}`;
      post("fatal", { error: reason });
    }
    throw error;
  } finally {
    deleteOwned(nextSurface);
    deleteOwned(snapshot);
    resizeStagingLastMilliseconds = performance.now() - started;
    resizeStagingTotalMilliseconds += resizeStagingLastMilliseconds;
    resizeStagingMaximumMilliseconds = Math.max(
      resizeStagingMaximumMilliseconds, resizeStagingLastMilliseconds);
    resizeStagingCount++;
  }
}

function acquireResizeStagingSurface(requiredWidth: number, requiredHeight: number): Surface {
  if (resizeStagingSurface &&
      resizeStagingCapacityWidth >= requiredWidth &&
      resizeStagingCapacityHeight >= requiredHeight) {
    resizeStagingSurfaceReuses++;
    return resizeStagingSurface;
  }

  const width = resizeStagingCapacity(resizeStagingCapacityWidth, requiredWidth);
  const height = resizeStagingCapacity(resizeStagingCapacityHeight, requiredHeight);
  const created = requireCanvasKit().MakeRenderTarget(requireGrContext(), width, height);
  if (!created)
    throw new Error(`CanvasKit could not allocate the ${width}x${height} GPU resize staging pool.`);
  if (!created.reportBackendTypeIsGPU()) {
    created.delete();
    throw new Error("CanvasKit resize staging pool is not GPU backed; fallback is forbidden.");
  }

  const prior = resizeStagingSurface;
  resizeStagingSurface = created;
  resizeStagingCapacityWidth = width;
  resizeStagingCapacityHeight = height;
  resizeStagingPeakPixels = Math.max(resizeStagingPeakPixels, width * height);
  resizeStagingSurfaceAllocations++;
  countCreated("ResizeStagingSurface", 0);
  if (prior) {
    prior.delete();
    countDeleted("ResizeStagingSurface", 0);
  }
  return created;
}

function resizeStagingCapacity(current: number, required: number): number {
  if (current >= required) return current;
  if (current === 0) return required;
  const growth = Math.max(
    minimumResizeStagingGrowthPixels,
    Math.min(maximumResizeStagingGrowthPixels, Math.ceil(current / 8)));
  return Math.max(required, current + growth);
}

function requireResizeCommitPaint(): Paint {
  if (resizeCommitPaint) return resizeCommitPaint;
  const kit = requireCanvasKit();
  const created = new kit.Paint();
  created.setBlendMode(kit.BlendMode.Src);
  resizeCommitPaint = created;
  countCreated("ResizeCommitPaint", 0);
  return created;
}

function replayCommandRange(
  scene: RasterScene,
  context: ReplayContext,
  start: number,
  end: number,
): void {
  const kit = context.kit;
  const targetCanvas = context.canvas;
  const stack: ReplayStackFrame[] = [];
  const rootSaveCount = targetCanvas.getSaveCount();
  try {
    for (let commandIndex = start; commandIndex < end; commandIndex++) {
      const command = scene.document.commands[commandIndex];
      const reader = new DisplayListCursor(
        context.view, command.payloadOffset, command.payloadLength,
        `DisplayList opcode ${command.opcode}`);
      switch (command.opcode) {
        case 1:
          reader.finish();
          targetCanvas.save();
          stack.push({ kind: "save" });
          break;
        case 2: {
          reader.finish();
          const frame = stack.pop();
          if (!frame) throw reader.error("Restore has no matching save or layer command.");
          completeReplayFrame(context, frame);
          break;
        }
        case 3: {
          const bounds = readOptionalRect(reader);
          const paint = readOptionalPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.saveLayer(paint?.object, bounds);
            stack.push({ kind: "layer" });
          } finally {
            deleteOwned(paint);
          }
          break;
        }
        case 4:
          targetCanvas.concat(readMatrix(reader));
          reader.finish();
          break;
        case 5: {
          const rect = readRect(reader);
          const operation = readEnum(reader, 2, "clip operation");
          const antiAlias = reader.boolean();
          reader.reserved16();
          reader.finish();
          targetCanvas.clipRect(rect, clipOperation(kit, operation), antiAlias);
          break;
        }
        case 6: {
          const roundedRect = readRoundedRect(reader);
          const operation = readEnum(reader, 2, "clip operation");
          const antiAlias = reader.boolean();
          reader.reserved16();
          reader.finish();
          targetCanvas.clipRRect(roundedRect, clipOperation(kit, operation), antiAlias);
          break;
        }
        case 7: {
          const path = readPath(reader, context);
          try {
            const operation = readEnum(reader, 2, "clip operation");
            const antiAlias = reader.boolean();
            reader.reserved16();
            reader.finish();
            targetCanvas.clipPath(path.object, clipOperation(kit, operation), antiAlias);
          } finally {
            deleteOwned(path);
          }
          break;
        }
        case 16: {
          const color = reader.uint32();
          const blend = readEnum(reader, blendModeNames.length, "blend mode");
          reader.finish();
          targetCanvas.drawColor(argbColor(kit, color), blendMode(kit, blend));
          break;
        }
        case 17: {
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.drawPaint(paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 18: {
          const start = readPoint(reader);
          const end = readPoint(reader);
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.drawLine(start[0], start[1], end[0], end[1], paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 19: {
          const mode = readEnum(reader, 3, "point mode");
          const count = reader.collectionCount("point");
          const points = new Float32Array(checkedElementCount(count, 2, "point coordinates"));
          for (let index = 0; index < points.length; index++) points[index] = reader.float32();
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.drawPoints(pointMode(kit, mode), points, paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 20: {
          const rect = readRect(reader);
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.drawRect(rect, paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 21: {
          const roundedRect = readRoundedRect(reader);
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.drawRRect(roundedRect, paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 22: {
          const outer = readRoundedRect(reader);
          const inner = readRoundedRect(reader);
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.drawDRRect(outer, inner, paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 23: {
          const center = readPoint(reader);
          const radius = reader.nonnegativeFloat("circle radius");
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.drawCircle(center[0], center[1], radius, paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 24: {
          const oval = readRect(reader);
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.drawOval(oval, paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 25: {
          const oval = readRect(reader);
          const start = reader.float32() * radiansToDegrees;
          const sweep = reader.float32() * radiansToDegrees;
          const useCenter = reader.boolean();
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            targetCanvas.drawArc(oval, start, sweep, useCenter, paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 26: {
          const path = readPath(reader, context);
          let paint: OwnedObject<Paint> | null = null;
          try {
            paint = readPaint(reader, context, 0);
            reader.finish();
            targetCanvas.drawPath(path.object, paint.object);
          } finally {
            deleteOwned(paint);
            deleteOwned(path);
          }
          break;
        }
        case 27: {
          const path = readPath(reader, context);
          try {
            const color = reader.uint32();
            const elevation = reader.nonnegativeFloat("shadow elevation");
            const transparentOccluder = reader.boolean();
            reader.finish();
            drawShadow(context, path.object, color, elevation, transparentOccluder);
          } finally {
            deleteOwned(path);
          }
          break;
        }
        case 28: {
          const image = requireImage(readResourceReference(reader, context, 2), context);
          const offset = readPoint(reader);
          const sampling = readEnum(reader, 4, "sampling quality");
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            drawImage(context, image, offset[0], offset[1], sampling, paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 29: {
          const image = requireImage(readResourceReference(reader, context, 2), context);
          const source = readRect(reader);
          const destination = readRect(reader);
          const sampling = readEnum(reader, 4, "sampling quality");
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            drawImageRect(context, image, source, destination, sampling, paint.object);
          }
          finally { deleteOwned(paint); }
          break;
        }
        case 30: {
          const image = requireImage(readResourceReference(reader, context, 2), context);
          const center = readRect(reader);
          const destination = readRect(reader);
          const sampling = readEnum(reader, 4, "sampling quality");
          const paint = readPaint(reader, context, 0);
          try {
            reader.finish();
            drawNinePatch(context, image, center, destination, sampling, paint.object);
          } finally {
            deleteOwned(paint);
          }
          break;
        }
        case 31: {
          const paragraph = readParagraph(reader, context);
          const offset = readPoint(reader);
          reader.finish();
          drawParagraph(context, paragraph, offset[0], offset[1]);
          break;
        }
        case 48: {
          const opacity = reader.unitFloat("opacity");
          const offset = readPoint(reader);
          reader.finish();
          const paint = own("Paint", new kit.Paint());
          try {
            paint.object.setColor(kit.Color(
              255, 255, 255, roundToEven(opacity * 255) / 255));
            targetCanvas.saveLayer(paint.object);
            targetCanvas.translate(offset[0], offset[1]);
            stack.push({ kind: "layer" });
          } finally {
            deleteOwned(paint);
          }
          break;
        }
        case 49: {
          const filter = readColorFilter(reader, context, 0, false);
          let paint: OwnedObject<Paint> | null = null;
          try {
            const offset = readPoint(reader);
            reader.finish();
            paint = own("Paint", new kit.Paint());
            paint.object.setColorFilter(filter!.object);
            targetCanvas.saveLayer(paint.object);
            targetCanvas.translate(offset[0], offset[1]);
            stack.push({ kind: "layer" });
          } finally {
            deleteOwned(paint);
            deleteOwned(filter);
          }
          break;
        }
        case 50: {
          if (context.view.getUint8(reader.offset) === 4) {
            const recipe = readRuntimeEffectImageFilter(reader, context, 0);
            const offset = readPoint(reader);
            const bounds = readOptionalRect(reader);
            reader.finish();
            const matchingRestore = findMatchingRestore(
              scene.document.commands, commandIndex, end);
            drawRuntimeEffectImageFilter(
              scene, context, recipe, offset, bounds,
              commandIndex + 1, matchingRestore);
            commandIndex = matchingRestore;
            break;
          }
          const filter = readCachedImageFilter(reader, context);
          let paint: OwnedObject<Paint> | null = null;
          try {
            const offset = readPoint(reader);
            readOptionalRect(reader); // Bounds are reserved for the explicit runtime-effect path.
            reader.finish();
            paint = own("Paint", new kit.Paint());
            paint.object.setImageFilter(filter!.object);
            targetCanvas.saveLayer(paint.object);
            targetCanvas.translate(offset[0], offset[1]);
            stack.push({ kind: "layer" });
          } finally {
            deleteOwned(paint);
            releaseImageFilterLease(filter);
          }
          break;
        }
        case 51: {
          const filter = readCachedImageFilter(reader, context);
          let paint: OwnedObject<Paint> | null = null;
          try {
            const blend = readEnum(reader, blendModeNames.length, "blend mode");
            reader.uint64(); // Stable BackdropId participates in cache identity, not Skia drawing state.
            const offset = readPoint(reader);
            reader.finish();
            paint = own("Paint", new kit.Paint());
            paint.object.setBlendMode(blendMode(kit, blend));
            let restoreCount = 1;
            const cropBounds = filter!.cropBounds ?? null;
            if (cropBounds) {
              targetCanvas.save();
              targetCanvas.clipRect(cropBounds, kit.ClipOp.Intersect, true);
              restoreCount++;
              appliedBlurCropBounds++;
            }
            targetCanvas.saveLayer(
              paint.object, cropBounds, filter!.object, 0, kit.TileMode.Clamp);
            targetCanvas.translate(offset[0], offset[1]);
            stack.push({ kind: "layer", restoreCount });
          } finally {
            deleteOwned(paint);
            releaseImageFilterLease(filter);
          }
          break;
        }
        case 52: {
          const shader = readShader(reader, context, 0, false);
          try {
            const maskRect = readRect(reader);
            const blend = readEnum(reader, blendModeNames.length, "blend mode");
            reader.finish();
            targetCanvas.saveLayer(undefined, maskRect);
            stack.push({ kind: "shader-mask", shader: shader!, maskRect, blendMode: blend });
          } catch (error) {
            deleteOwned(shader);
            throw error;
          }
          break;
        }
        case 53: {
          const picture = requirePicture(readResourceReference(reader, context, 4), context);
          const offset = readPoint(reader);
          const cacheHint = reader.uint8();
          if ((cacheHint & ~3) !== 0) throw reader.error(`Invalid retained-scene cache hint ${cacheHint}.`);
          reader.finish();
          targetCanvas.save();
          try {
            targetCanvas.translate(offset[0], offset[1]);
            targetCanvas.drawPicture(picture);
          } finally {
            targetCanvas.restore();
          }
          break;
        }
        default:
          throw reader.error(
            `DOROTIWEB031: CanvasKit renderer has no DisplayList opcode ${command.opcode}; fallback is forbidden.`);
      }
    }
    if (stack.length !== 0)
      throw new Error(`CanvasKit DisplayList ended with ${stack.length} unbalanced save/layer command(s).`);
  } finally {
    while (stack.length > 0) {
      const frame = stack.pop()!;
      deleteOwned(frame.shader);
    }
    targetCanvas.restoreToCount(rootSaveCount);
  }
}

class DisplayListCursor {
  readonly #view: DataView;
  readonly #end: number;
  readonly #label: string;
  #offset: number;

  constructor(view: DataView, offset: number, length: number, label: string) {
    if (!Number.isSafeInteger(offset) || !Number.isSafeInteger(length) || offset < 0 || length < 0 ||
        offset > view.byteLength - length)
      throw new Error(`${label} has invalid bounds ${offset}+${length}/${view.byteLength}.`);
    this.#view = view;
    this.#offset = offset;
    this.#end = offset + length;
    this.#label = label;
  }

  get remaining(): number { return this.#end - this.#offset; }
  get offset(): number { return this.#offset; }

  error(message: string): Error { return new Error(`${this.#label} at byte ${this.#offset}: ${message}`); }

  uint8(): number {
    this.#require(1);
    return this.#view.getUint8(this.#offset++);
  }

  boolean(): boolean {
    const value = this.uint8();
    if (value > 1) throw this.error(`Boolean value ${value} is non-canonical.`);
    return value === 1;
  }

  uint16(): number {
    this.#require(2);
    const value = this.#view.getUint16(this.#offset, true);
    this.#offset += 2;
    return value;
  }

  reserved16(): void {
    if (this.uint16() !== 0) throw this.error("Reserved UInt16 must be zero.");
  }

  uint32(): number {
    this.#require(4);
    const value = this.#view.getUint32(this.#offset, true);
    this.#offset += 4;
    return value;
  }

  int32(): number {
    this.#require(4);
    const value = this.#view.getInt32(this.#offset, true);
    this.#offset += 4;
    return value;
  }

  uint64(): bigint {
    this.#require(8);
    const value = this.#view.getBigUint64(this.#offset, true);
    this.#offset += 8;
    return value;
  }

  float32(): number {
    this.#require(4);
    const bits = this.#view.getInt32(this.#offset, true);
    const value = this.#view.getFloat32(this.#offset, true);
    this.#offset += 4;
    if (bits === -0x80000000) throw this.error("Negative zero is not a canonical float encoding.");
    if (!Number.isFinite(value)) throw this.error("Float value must be finite.");
    return value;
  }

  nonnegativeFloat(name: string): number {
    const value = this.float32();
    if (value < 0) throw this.error(`${name} must be nonnegative.`);
    return value;
  }

  positiveFloat(name: string): number {
    const value = this.float32();
    if (value <= 0) throw this.error(`${name} must be positive.`);
    return value;
  }

  unitFloat(name: string): number {
    const value = this.float32();
    if (value < 0 || value > 1) throw this.error(`${name} must be in [0,1].`);
    return value;
  }

  collectionCount(name: string): number {
    const value = this.uint32();
    if (value > maximumCollectionCount)
      throw this.error(`${name} count ${value} exceeds ${maximumCollectionCount}.`);
    return value;
  }

  bytes(length: number): Uint8Array {
    if (!Number.isSafeInteger(length) || length < 0) throw this.error(`Invalid byte length ${length}.`);
    this.#require(length);
    const result = new Uint8Array(
      this.#view.buffer, this.#view.byteOffset + this.#offset, length);
    this.#offset += length;
    return result;
  }

  finish(): void {
    if (this.#offset !== this.#end)
      throw this.error(`${this.remaining} trailing payload byte(s) are non-canonical.`);
  }

  #require(length: number): void {
    if (length > this.#end - this.#offset)
      throw this.error(`Payload is truncated; need ${length}, have ${this.#end - this.#offset}.`);
  }
}

function readStringTable(scene: RasterScene): readonly string[] {
  const metadata = scene.document.metadata;
  const start = 112 + metadata.resourceBytes;
  const reader = new DisplayListCursor(
    new DataView(scene.buffer), start, metadata.stringBytes, "DisplayList string table");
  const decoder = new TextDecoder("utf-8", { fatal: true });
  const values: string[] = [];
  let previous: Uint8Array | null = null;
  while (reader.remaining > 0) {
    const length = reader.uint32();
    const encoded = reader.bytes(length);
    if (previous && compareBytes(previous, encoded) >= 0)
      throw reader.error("Strings must be unique and sorted by canonical UTF-8 bytes.");
    try { values.push(decoder.decode(encoded)); }
    catch { throw reader.error("String contains invalid UTF-8."); }
    previous = encoded.slice();
  }
  reader.finish();
  return values;
}

function readDeclaredResources(scene: RasterScene): ReadonlyMap<string, string> {
  const metadata = scene.document.metadata;
  const view = new DataView(scene.buffer);
  const result = new Map<string, string>();
  for (let index = 0; index < metadata.resourceCount; index++) {
    const offset = 112 + index * 32;
    const kind = view.getUint16(offset, true);
    const version = view.getUint32(offset + 4, true);
    const id = view.getBigUint64(offset + 8, true);
    result.set(
      resourceKey(kind, id, version),
      fingerprintKey(view.getBigUint64(offset + 16, true), view.getBigUint64(offset + 24, true)));
  }
  return result;
}

function readResourceReference(
  reader: DisplayListCursor,
  context: ReplayContext,
  expectedKind?: number,
): DisplayResourceReference {
  const kind = reader.uint16();
  reader.reserved16();
  const version = reader.uint32();
  const id = reader.uint64();
  if (kind < 1 || kind > 4 || version === 0 || id === 0n)
    throw reader.error(`Invalid resource reference ${kind}/${id}/${version}.`);
  if (expectedKind !== undefined && kind !== expectedKind)
    throw reader.error(`Resource kind ${kind} does not match required kind ${expectedKind}.`);
  const reference = { kind, version, id };
  if (!context.declaredResources.has(resourceKey(kind, id, version)))
    throw reader.error(`Resource ${kind}/${id}/${version} is not declared by this DisplayList.`);
  return reference;
}

function requireResource(reference: DisplayResourceReference, context: ReplayContext): RasterResource {
  const resource = resources.get(resourceKey(reference.kind, reference.id, reference.version));
  if (!resource)
    throw new Error(`CanvasKit resource ${reference.kind}/${reference.id}/${reference.version} is not retained.`);
  const expectedKind = displayResourceKinds[reference.kind];
  if (resource.kind !== expectedKind)
    throw new Error(
      `CanvasKit resource ${reference.id}/${reference.version} is '${resource.kind}', expected '${expectedKind}'.`);
  const declaredFingerprint = context.declaredResources.get(
    resourceKey(reference.kind, reference.id, reference.version));
  if (declaredFingerprint !== resource.fingerprint)
    throw new Error(
      `CanvasKit resource ${reference.kind}/${reference.id}/${reference.version} fingerprint mismatch.`);
  return resource;
}

function requireImage(reference: DisplayResourceReference, context: ReplayContext): Image {
  return requireResource(reference, context).object as Image;
}

function requirePicture(reference: DisplayResourceReference, context: ReplayContext): SkPicture {
  return requireResource(reference, context).object as SkPicture;
}

function resourceKey(kind: number, id: bigint, version: number): string {
  return `${kind}:${id.toString(10)}:${version}`;
}

function fingerprintKey(low: bigint, high: bigint): string {
  return `${low.toString(16).padStart(16, "0")}:${high.toString(16).padStart(16, "0")}`;
}

function fingerprintFromSha256(value: unknown): string {
  const text = String(value ?? "").trim().toLowerCase();
  if (!/^[0-9a-f]{64}$/.test(text))
    throw new Error("CanvasKit resource descriptor requires a 64-character SHA-256.");
  const bytes = Uint8Array.from(text.match(/../g)!, pair => Number.parseInt(pair, 16));
  const view = new DataView(bytes.buffer);
  return fingerprintKey(view.getBigUint64(0, true), view.getBigUint64(8, true));
}

function resourceKindCode(kind: string): number {
  const value = displayResourceKinds.indexOf(kind as typeof displayResourceKinds[number]);
  if (value <= 0) throw new Error(`Unknown CanvasKit resource kind '${kind}'.`);
  return value;
}

function compareBytes(left: Uint8Array, right: Uint8Array): number {
  const length = Math.min(left.length, right.length);
  for (let index = 0; index < length; index++) {
    if (left[index] !== right[index]) return left[index] - right[index];
  }
  return left.length - right.length;
}

function readEnum(reader: DisplayListCursor, count: number, name: string): number {
  const value = reader.uint8();
  if (value >= count) throw reader.error(`Invalid ${name} ${value}.`);
  return value;
}

function checkedElementCount(count: number, width: number, name: string): number {
  const result = count * width;
  if (!Number.isSafeInteger(result) || result > maximumCollectionCount * width)
    throw new Error(`${name} count overflow.`);
  return result;
}

function readPoint(reader: DisplayListCursor): Float32Array {
  return new Float32Array([reader.float32(), reader.float32()]);
}

function readRect(reader: DisplayListCursor): Float32Array {
  return new Float32Array([
    reader.float32(), reader.float32(), reader.float32(), reader.float32(),
  ]);
}

function readOptionalRect(reader: DisplayListCursor): Float32Array | null {
  return reader.boolean() ? readRect(reader) : null;
}

function readRoundedRect(reader: DisplayListCursor): Float32Array {
  const result = new Float32Array(12);
  for (let index = 0; index < 4; index++) result[index] = reader.float32();
  for (let index = 4; index < 12; index++) result[index] = reader.nonnegativeFloat("rounded-rect radius");
  return result;
}

function readMatrix(reader: DisplayListCursor): Float32Array {
  const wire = new Float32Array(16);
  for (let index = 0; index < 16; index++) wire[index] = reader.float32();
  return dorotiCanvasKitDisplayMatrix(wire);
}

export function dorotiCanvasKitDisplayMatrix(wire: ArrayLike<number>): Float32Array {
  if (wire.length !== 16)
    throw new Error(`Doroti DisplayMatrix requires 16 values, received ${wire.length}.`);
  // DisplayList matrices preserve Flutter/Skia's column-major 4x4 layout.
  // CanvasKit's public InputMatrix accepts a row-major 3x3, matching the
  // native renderer's Matrix4 -> SKMatrix projection.
  return new Float32Array([
    wire[0], wire[4], wire[12],
    wire[1], wire[5], wire[13],
    wire[3], wire[7], wire[15],
  ]);
}

function readOptionalMatrix(reader: DisplayListCursor): Float32Array | undefined {
  return reader.boolean() ? readMatrix(reader) : undefined;
}

function own<T extends EmbindObject<string>>(kind: string, object: T): OwnedObject<T> {
  if (!object) throw new Error(`CanvasKit ${kind} factory returned null.`);
  countCreated(kind, 0);
  return { kind, object };
}

function deleteOwned(value: OwnedObject<EmbindObject<string>> | null | undefined): void {
  if (!value) return;
  value.object.delete();
  countDeleted(value.kind, 0);
}

function argbColor(kit: CanvasKit, value: number): Float32Array {
  return kit.Color(
    (value >>> 16) & 0xff,
    (value >>> 8) & 0xff,
    value & 0xff,
    ((value >>> 24) & 0xff) / 255);
}

function withOpacityByte(kit: CanvasKit, value: number, multiplier: number): Float32Array {
  return kit.Color(
    (value >>> 16) & 0xff,
    (value >>> 8) & 0xff,
    value & 0xff,
    roundToEven(((value >>> 24) & 0xff) * multiplier) / 255);
}

function roundToEven(value: number): number {
  const floor = Math.floor(value);
  const fraction = value - floor;
  if (fraction < 0.5) return floor;
  if (fraction > 0.5) return floor + 1;
  return (floor & 1) === 0 ? floor : floor + 1;
}

function blendMode(kit: CanvasKit, value: number) {
  const name = blendModeNames[value];
  if (!name) throw new Error(`Invalid CanvasKit blend mode ${value}.`);
  return kit.BlendMode[name];
}

function clipOperation(kit: CanvasKit, value: number) {
  return value === 0 ? kit.ClipOp.Difference : kit.ClipOp.Intersect;
}

function pointMode(kit: CanvasKit, value: number) {
  return value === 0 ? kit.PointMode.Points : value === 1 ? kit.PointMode.Lines : kit.PointMode.Polygon;
}

function tileMode(kit: CanvasKit, value: number) {
  switch (value) {
    case 0: return kit.TileMode.Clamp;
    case 1: return kit.TileMode.Repeat;
    case 2: return kit.TileMode.Mirror;
    case 3: return kit.TileMode.Decal;
    default: throw new Error(`Invalid CanvasKit tile mode ${value}.`);
  }
}

function completeReplayFrame(context: ReplayContext, frame: ReplayStackFrame): void {
  if (frame.kind === "shader-mask") {
    const paint = own("Paint", new context.kit.Paint());
    try {
      paint.object.setShader(frame.shader!.object);
      paint.object.setBlendMode(blendMode(context.kit, frame.blendMode!));
      paint.object.setAntiAlias(true);
      context.canvas.drawRect(frame.maskRect!, paint.object);
    } finally {
      deleteOwned(paint);
      deleteOwned(frame.shader);
    }
  }
  for (let remaining = frame.restoreCount ?? 1; remaining > 0; remaining--)
    context.canvas.restore();
}

function readPath(reader: DisplayListCursor, context: ReplayContext): OwnedObject<Path> {
  const fillType = readEnum(reader, 2, "path fill type");
  if (reader.uint8() !== 0 || reader.uint16() !== 0)
    throw reader.error("Path reserved fields must be zero.");
  const verbCount = reader.collectionCount("path verb");
  const valueCount = reader.collectionCount("path value");
  const verbs = reader.bytes(verbCount).slice();
  const values = new Float32Array(valueCount);
  for (let index = 0; index < valueCount; index++) values[index] = reader.float32();
  const builder = own("PathBuilder", new context.kit.PathBuilder());
  let cursor = 0;
  const take = (count: number): Float32Array => {
    if (cursor > values.length - count)
      throw reader.error(`Path verb stream requires more than ${valueCount} values.`);
    const result = values.subarray(cursor, cursor + count);
    cursor += count;
    return result;
  };
  try {
    builder.object.setFillType(fillType === 0 ? context.kit.FillType.Winding : context.kit.FillType.EvenOdd);
    for (const verb of verbs) {
      switch (verb) {
        case 0: { const p = take(2); builder.object.moveTo(p[0], p[1]); break; }
        case 1: { const p = take(2); builder.object.lineTo(p[0], p[1]); break; }
        case 2: { const p = take(2); builder.object.rMoveTo(p[0], p[1]); break; }
        case 3: { const p = take(2); builder.object.rLineTo(p[0], p[1]); break; }
        case 4: { const p = take(4); builder.object.quadTo(p[0], p[1], p[2], p[3]); break; }
        case 5: { const p = take(5); builder.object.conicTo(p[0], p[1], p[2], p[3], p[4]); break; }
        case 6: {
          const p = take(6);
          builder.object.cubicTo(p[0], p[1], p[2], p[3], p[4], p[5]);
          break;
        }
        case 7: builder.object.addRect(take(4)); break;
        case 8: builder.object.addOval(take(4)); break;
        case 9: {
          const p = take(6);
          builder.object.addArc(p.subarray(0, 4), p[4] * radiansToDegrees, p[5] * radiansToDegrees);
          break;
        }
        case 10: builder.object.addRRect(take(12)); break;
        case 11:
          take(12);
          // Intentional reference compatibility: SkiaSceneRenderer.ToPath(UiPath) consumes no
          // geometry for addRSuperellipse. Keep that current behavior visible in diagnostics.
          referenceCompatibilityAddSuperellipseNoOps++;
          break;
        case 12: {
          const p = take(7);
          if ((p[5] !== 0 && p[5] !== 1) || (p[6] !== 0 && p[6] !== 1))
            throw reader.error("arcToPoint boolean values must be zero or one.");
          builder.object.arcToRotated(p[2], p[3], p[4], p[5] === 0, p[6] === 0, p[0], p[1]);
          break;
        }
        case 13: {
          const p = take(7);
          if (p[6] !== 0 && p[6] !== 1)
            throw reader.error("arcTo forceMoveTo value must be zero or one.");
          builder.object.arcToOval(
            p.subarray(0, 4), p[4] * radiansToDegrees, p[5] * radiansToDegrees, p[6] !== 0);
          break;
        }
        case 14: builder.object.close(); break;
        default: throw reader.error(`Unknown path verb ${verb}.`);
      }
    }
    if (cursor !== values.length)
      throw reader.error(`Path has ${values.length - cursor} trailing value(s).`);
    return own("Path", builder.object.detach());
  } finally {
    deleteOwned(builder);
  }
}

function readOptionalPaint(
  reader: DisplayListCursor,
  context: ReplayContext,
  depth: number,
): OwnedObject<Paint> | null {
  return reader.boolean() ? readPaint(reader, context, depth) : null;
}

function readPaint(
  reader: DisplayListCursor,
  context: ReplayContext,
  depth: number,
): OwnedObject<Paint> {
  requireDepth(depth, reader);
  const cacheKey = paintCacheKey(reader, context);
  const cached = cacheKey === null ? undefined : paintCache.get(cacheKey);
  if (cached) {
    paintCacheHits++;
    paintCache.delete(cacheKey!);
    paintCache.set(cacheKey!, cached);
    reader.bytes(reader.remaining);
    return own("Paint", cached.object.copy());
  }
  paintCacheMisses++;
  const color = reader.uint32();
  const style = readEnum(reader, 2, "paint style");
  const cap = readEnum(reader, 3, "stroke cap");
  const join = readEnum(reader, 3, "stroke join");
  const antiAlias = reader.boolean();
  const blend = readEnum(reader, blendModeNames.length, "blend mode");
  readEnum(reader, 4, "paint sampling quality");
  const invertColors = reader.boolean();
  if (reader.uint8() !== 0) throw reader.error("Paint reserved byte must be zero.");
  const strokeWidth = reader.nonnegativeFloat("stroke width");
  const strokeMiter = reader.nonnegativeFloat("stroke miter limit");
  let shader: OwnedObject<Shader> | null = null;
  let colorFilter: OwnedObject<ColorFilter> | null = null;
  let maskFilter: OwnedObject<MaskFilter> | null = null;
  let imageFilter: OwnedObject<ImageFilter> | null = null;
  let paint: OwnedObject<Paint> | null = null;
  try {
    shader = readShader(reader, context, depth + 1, true);
    colorFilter = readColorFilter(reader, context, depth + 1, true);
    maskFilter = readMaskFilter(reader, context);
    imageFilter = readImageFilter(reader, context, depth + 1, true);
    if (invertColors) {
      const invert = own("ColorFilter", context.kit.ColorFilter.MakeMatrix(new Float32Array([
        -1, 0, 0, 0, 1,
        0, -1, 0, 0, 1,
        0, 0, -1, 0, 1,
        0, 0, 0, 1, 0,
      ])));
      if (colorFilter) {
        try {
          const composed = own(
            "ColorFilter", context.kit.ColorFilter.MakeCompose(invert.object, colorFilter.object));
          deleteOwned(colorFilter);
          colorFilter = composed;
        } finally {
          deleteOwned(invert);
        }
      } else {
        colorFilter = invert;
      }
    }
    paint = own("Paint", new context.kit.Paint());
    paint.object.setColorInt(color >>> 0);
    paint.object.setStyle(style === 0 ? context.kit.PaintStyle.Fill : context.kit.PaintStyle.Stroke);
    paint.object.setStrokeCap(
      cap === 0 ? context.kit.StrokeCap.Butt : cap === 1 ? context.kit.StrokeCap.Round : context.kit.StrokeCap.Square);
    paint.object.setStrokeJoin(
      join === 0 ? context.kit.StrokeJoin.Miter : join === 1 ? context.kit.StrokeJoin.Round : context.kit.StrokeJoin.Bevel);
    paint.object.setAntiAlias(antiAlias);
    paint.object.setBlendMode(blendMode(context.kit, blend));
    paint.object.setStrokeWidth(strokeWidth);
    paint.object.setStrokeMiter(strokeMiter);
    if (shader) paint.object.setShader(shader.object);
    if (colorFilter) paint.object.setColorFilter(colorFilter.object);
    if (maskFilter) paint.object.setMaskFilter(maskFilter.object);
    if (imageFilter) paint.object.setImageFilter(imageFilter.object);
    if (cacheKey !== null) {
      while (paintCache.size >= maximumPaintCacheSize) {
        const oldestKey = paintCache.keys().next().value as string | undefined;
        if (oldestKey === undefined) break;
        deleteCachedPaint(oldestKey, true);
      }
      paintCache.set(cacheKey, own("Paint", paint.object.copy()));
    }
    return paint;
  } catch (error) {
    deleteOwned(paint);
    throw error;
  } finally {
    deleteOwned(imageFilter);
    deleteOwned(maskFilter);
    deleteOwned(colorFilter);
    deleteOwned(shader);
  }
}

function paintCacheKey(reader: DisplayListCursor, context: ReplayContext): string | null {
  if (reader.remaining > maximumCachedPaintWireBytes) return null;
  const bytes = new Uint8Array(
    context.view.buffer,
    context.view.byteOffset + reader.offset,
    reader.remaining);
  const characters = new Array<string>(bytes.length);
  for (let index = 0; index < bytes.length; index++) characters[index] = String.fromCharCode(bytes[index]);
  return `${context.resourceDeclarationKey}|${characters.join("")}`;
}

function deleteCachedPaint(key: string, eviction: boolean): void {
  const cached = paintCache.get(key);
  if (!cached) return;
  paintCache.delete(key);
  deleteOwned(cached);
  if (eviction) paintCacheEvictions++;
}

function clearPaintCache(invalidation: boolean): void {
  for (const key of [...paintCache.keys()]) deleteCachedPaint(key, false);
  if (invalidation) paintCacheInvalidations++;
}

function readShader(
  reader: DisplayListCursor,
  context: ReplayContext,
  depth: number,
  allowNull: boolean,
): OwnedObject<Shader> | null {
  requireDepth(depth, reader);
  const tag = reader.uint8();
  if (tag === 0) {
    if (!allowNull) throw reader.error("A shader is required.");
    return null;
  }
  switch (tag) {
    case 1: {
      const start = readPoint(reader);
      const end = readPoint(reader);
      const tile = readEnum(reader, 4, "tile mode");
      const gradient = readGradient(reader, context);
      const matrix = readOptionalMatrix(reader);
      return own("Shader", context.kit.Shader.MakeLinearGradient(
        start, end, gradient.colors, gradient.stops, tileMode(context.kit, tile), matrix));
    }
    case 2: {
      const center = readPoint(reader);
      const radius = reader.nonnegativeFloat("radial-gradient radius");
      const tile = readEnum(reader, 4, "tile mode");
      const hasFocal = reader.boolean();
      const focal = hasFocal ? readPoint(reader) : null;
      const focalRadius = reader.nonnegativeFloat("radial-gradient focal radius");
      const gradient = readGradient(reader, context);
      const matrix = readOptionalMatrix(reader);
      const shader = focal
        ? context.kit.Shader.MakeTwoPointConicalGradient(
          focal, focalRadius, center, radius, gradient.colors, gradient.stops,
          tileMode(context.kit, tile), matrix)
        : context.kit.Shader.MakeRadialGradient(
          center, radius, gradient.colors, gradient.stops, tileMode(context.kit, tile), matrix);
      return own("Shader", shader);
    }
    case 3: {
      const center = readPoint(reader);
      const startAngle = reader.float32() * radiansToDegrees;
      const endAngle = reader.float32() * radiansToDegrees;
      const tile = readEnum(reader, 4, "tile mode");
      const gradient = readGradient(reader, context);
      const matrix = readOptionalMatrix(reader);
      return own("Shader", context.kit.Shader.MakeSweepGradient(
        center[0], center[1], gradient.colors, gradient.stops,
        tileMode(context.kit, tile), matrix, 0, startAngle, endAngle));
    }
    case 4: {
      const image = requireImage(readResourceReference(reader, context, 2), context);
      const tileX = readEnum(reader, 4, "horizontal tile mode");
      const tileY = readEnum(reader, 4, "vertical tile mode");
      const sampling = readEnum(reader, 4, "sampling quality");
      if (reader.uint8() !== 0) throw reader.error("Image-shader reserved byte must be zero.");
      const matrix = readMatrix(reader);
      let mipmapped: OwnedObject<Image> | null = null;
      try {
        if (sampling === 2) mipmapped = own("Image", image.makeCopyWithDefaultMipmaps());
        const source = mipmapped?.object ?? image;
        const shader = sampling === 3
          ? source.makeShaderCubic(
            tileMode(context.kit, tileX), tileMode(context.kit, tileY), 1 / 3, 1 / 3, matrix)
          : source.makeShaderOptions(
            tileMode(context.kit, tileX), tileMode(context.kit, tileY),
            sampling === 0 ? context.kit.FilterMode.Nearest : context.kit.FilterMode.Linear,
            sampling === 2 ? context.kit.MipmapMode.Linear : context.kit.MipmapMode.None,
            matrix);
        return own("Shader", shader);
      } finally {
        deleteOwned(mipmapped);
      }
    }
    case 5: {
      const effectReference = readResourceReference(reader, context, 3);
      const effect = requireResource(effectReference, context).object as RuntimeEffect;
      const uniformByteCount = reader.collectionCount("runtime-effect uniform byte");
      if ((uniformByteCount & 3) !== 0)
        throw reader.error("Runtime-effect uniform byte count must be divisible by four.");
      const uniformBytes = reader.bytes(uniformByteCount);
      const suppliedUniforms = new Float32Array(uniformByteCount / 4);
      const uniformView = new DataView(
        uniformBytes.buffer, uniformBytes.byteOffset, uniformBytes.byteLength);
      for (let index = 0; index < suppliedUniforms.length; index++)
        suppliedUniforms[index] = uniformView.getFloat32(index * 4, true);
      if (suppliedUniforms.length > effect.getUniformFloatCount())
        throw reader.error(
          `Runtime-effect declares ${effect.getUniformFloatCount()} floats, received ${suppliedUniforms.length}.`);
      const uniforms = new Float32Array(effect.getUniformFloatCount());
      uniforms.set(suppliedUniforms);
      const childCount = reader.collectionCount("runtime-effect child");
      const children: OwnedObject<Shader>[] = [];
      try {
        for (let index = 0; index < childCount; index++) {
          const childReference = readResourceReference(reader, context);
          if (childReference.kind !== 2)
            throw reader.error(
              `Runtime-effect child ${index} has resource kind ${childReference.kind}; only image shaders are representable.`);
          const image = requireImage(childReference, context);
          children.push(own("Shader", image.makeShaderOptions(
            context.kit.TileMode.Clamp, context.kit.TileMode.Clamp,
            context.kit.FilterMode.Nearest, context.kit.MipmapMode.None)));
        }
        const shader = children.length === 0
          ? effect.makeShader(uniforms)
          : effect.makeShaderWithChildren(uniforms, children.map(child => child.object));
        return own("Shader", shader);
      } finally {
        for (const child of children) deleteOwned(child);
      }
    }
    default: throw reader.error(`Unknown shader tag ${tag}.`);
  }
}

function readGradient(
  reader: DisplayListCursor,
  context: ReplayContext,
): { colors: Float32Array[]; stops: number[] } {
  const count = reader.collectionCount("gradient stop");
  if (count < 2) throw reader.error("A gradient requires at least two stops.");
  const colors: Float32Array[] = [];
  const stops: number[] = [];
  let previous = Number.NEGATIVE_INFINITY;
  for (let index = 0; index < count; index++) {
    colors.push(argbColor(context.kit, reader.uint32()));
    const stop = reader.float32();
    if (stop < previous) throw reader.error("Gradient stops must be nondecreasing.");
    stops.push(stop);
    previous = stop;
  }
  return { colors, stops };
}

function readColorFilter(
  reader: DisplayListCursor,
  context: ReplayContext,
  depth: number,
  allowNull: boolean,
): OwnedObject<ColorFilter> | null {
  requireDepth(depth, reader);
  const tag = reader.uint8();
  switch (tag) {
    case 0:
      if (!allowNull) throw reader.error("A color filter is required.");
      return null;
    case 1:
      return own("ColorFilter", context.kit.ColorFilter.MakeBlend(
        argbColor(context.kit, reader.uint32()),
        blendMode(context.kit, readEnum(reader, blendModeNames.length, "blend mode"))));
    case 2: {
      const matrix = new Float32Array(20);
      for (let index = 0; index < matrix.length; index++) matrix[index] = reader.float32();
      return own("ColorFilter", context.kit.ColorFilter.MakeMatrix(matrix));
    }
    case 3: return own("ColorFilter", context.kit.ColorFilter.MakeLinearToSRGBGamma());
    case 4: return own("ColorFilter", context.kit.ColorFilter.MakeSRGBToLinearGamma());
    default: throw reader.error(`Unknown color-filter tag ${tag}.`);
  }
}

function readMaskFilter(
  reader: DisplayListCursor,
  context: ReplayContext,
): OwnedObject<MaskFilter> | null {
  if (!reader.boolean()) return null;
  const style = readEnum(reader, 4, "blur style");
  const sigma = reader.nonnegativeFloat("mask-filter sigma");
  const blurStyle = style === 0 ? context.kit.BlurStyle.Normal
    : style === 1 ? context.kit.BlurStyle.Solid
      : style === 2 ? context.kit.BlurStyle.Outer : context.kit.BlurStyle.Inner;
  return own("MaskFilter", context.kit.MaskFilter.MakeBlur(blurStyle, sigma, true));
}

function readRuntimeEffectImageFilter(
  reader: DisplayListCursor,
  context: ReplayContext,
  depth: number,
): RuntimeEffectImageFilterRecipe {
  requireDepth(depth, reader);
  if (reader.uint8() !== 4)
    throw reader.error("A runtime-effect image-filter tag was required.");
  if (reader.uint8() !== 5)
    throw reader.error("ImageFilter.shader requires a runtime-effect shader recipe.");
  const effectReference = readResourceReference(reader, context, 3);
  const effectResource = requireResource(effectReference, context);
  const effect = effectResource.object as RuntimeEffect;
  const source = new TextDecoder().decode(effectResource.bytes);
  const firstFloatUniform = source.match(
    /^\s*(?:layout\s*\([^)]*\)\s*)?uniform\s+(?<type>(?:float|half)(?:[234](?:x[234])?)?)\s+[A-Za-z_]\w*\s*(?<array>\[\s*\d+\s*\])?\s*;/m);
  if (!firstFloatUniform || !["float2", "half2"].includes(firstFloatUniform.groups?.type ?? "") ||
      firstFloatUniform.groups?.array)
    throw reader.error(
      "Runtime-effect image filter requires first source declaration non-array float2 input size.");
  const uniformByteCount = reader.collectionCount("runtime-effect image-filter uniform byte");
  if ((uniformByteCount & 3) !== 0)
    throw reader.error("Runtime-effect image-filter uniform byte count must be divisible by four.");
  const uniformBytes = reader.bytes(uniformByteCount);
  const suppliedUniforms = new Float32Array(uniformByteCount / 4);
  const uniformView = new DataView(
    uniformBytes.buffer, uniformBytes.byteOffset, uniformBytes.byteLength);
  for (let index = 0; index < suppliedUniforms.length; index++)
    suppliedUniforms[index] = uniformView.getFloat32(index * 4, true);
  if (suppliedUniforms.length > effect.getUniformFloatCount())
    throw reader.error(
      `Runtime-effect image filter declares ${effect.getUniformFloatCount()} floats, received ${suppliedUniforms.length}.`);
  const uniforms = new Float32Array(effect.getUniformFloatCount());
  uniforms.set(suppliedUniforms);
  const firstUniform = effect.getUniformCount() > 0 ? effect.getUniform(0) : null;
  if (!firstUniform || firstUniform.slot !== 0 || firstUniform.columns !== 2 ||
      firstUniform.rows !== 1 || firstUniform.isInteger || uniforms.length < 2)
    throw reader.error("Runtime-effect image filter requires first uniform float2 input size.");
  const childCount = reader.collectionCount("runtime-effect image-filter child");
  const childImages: Image[] = [];
  for (let index = 0; index < childCount; index++) {
    const childReference = readResourceReference(reader, context);
    if (childReference.kind !== 2)
      throw reader.error(
        `Runtime-effect image-filter child ${index + 1} has resource kind ${childReference.kind}; only images are representable.`);
    childImages.push(requireImage(childReference, context));
  }
  const declaredShaderChildren = source.match(
    /^\s*(?:layout\s*\([^)]*\)\s*)?uniform\s+shader\s+[A-Za-z_]\w*\s*;/gm)?.length ?? 0;
  if (declaredShaderChildren !== childImages.length + 1)
    throw reader.error(
      `Runtime-effect image filter declares ${declaredShaderChildren} shader children; ` +
      `wire supplies implicit input plus ${childImages.length} image children.`);
  const sampling = readEnum(reader, 4, "runtime-effect image-filter sampling quality");
  return { effect, uniforms, childImages, sampling };
}

function readImageFilter(
  reader: DisplayListCursor,
  context: ReplayContext,
  depth: number,
  allowNull: boolean,
): OwnedImageFilter | null {
  requireDepth(depth, reader);
  const tag = reader.uint8();
  switch (tag) {
    case 0:
      if (!allowNull) throw reader.error("An image filter is required.");
      return null;
    case 1: {
      const sigmaX = reader.nonnegativeFloat("blur sigma X");
      const sigmaY = reader.nonnegativeFloat("blur sigma Y");
      const tile = readEnum(reader, 4, "tile mode");
      const bounds = readOptionalRect(reader);
      return {
        ...own("ImageFilter", context.kit.ImageFilter.MakeBlur(
          sigmaX, sigmaY, tileMode(context.kit, tile), null)),
        // CanvasKit has no blur crop argument. Preserve the wire crop so the
        // backdrop SaveLayer can apply the same explicit clip/bounds as the
        // shared Skia renderer.
        cropBounds: bounds,
      };
    }
    case 2: {
      const colorFilter = readColorFilter(reader, context, depth + 1, false)!;
      try {
        return own("ImageFilter", context.kit.ImageFilter.MakeColorFilter(colorFilter.object, null));
      } finally {
        deleteOwned(colorFilter);
      }
    }
    case 3: {
      const matrix = readMatrix(reader);
      const sampling = readEnum(reader, 4, "sampling quality");
      return own("ImageFilter", context.kit.ImageFilter.MakeMatrixTransform(
        matrix, samplingOptions(context.kit, sampling), null));
    }
    case 4: {
      const shader = readShader(reader, context, depth + 1, false)!;
      try {
        readEnum(reader, 4, "sampling quality");
        throw reader.error(
          "DOROTIWEB032: CanvasKit 0.42 public ImageFilter.MakeShader cannot inject the filtered input " +
          "as runtime-effect child 0 or bind its size uniforms; fallback is forbidden.");
      } finally {
        deleteOwned(shader);
      }
    }
    case 5: {
      const outer = readImageFilter(reader, context, depth + 1, false)!;
      let inner: OwnedObject<ImageFilter> | null = null;
      try {
        inner = readImageFilter(reader, context, depth + 1, false)!;
        return own("ImageFilter", context.kit.ImageFilter.MakeCompose(outer.object, inner.object));
      } finally {
        deleteOwned(inner);
        deleteOwned(outer);
      }
    }
    case 6: {
      const dx = reader.float32();
      const dy = reader.float32();
      const sigmaX = reader.nonnegativeFloat("drop-shadow sigma X");
      const sigmaY = reader.nonnegativeFloat("drop-shadow sigma Y");
      const color = argbColor(context.kit, reader.uint32());
      const shadowOnly = reader.boolean();
      return own("ImageFilter", shadowOnly
        ? context.kit.ImageFilter.MakeDropShadowOnly(dx, dy, sigmaX, sigmaY, color, null)
        : context.kit.ImageFilter.MakeDropShadow(dx, dy, sigmaX, sigmaY, color, null));
    }
    default: throw reader.error(`Unknown image-filter tag ${tag}.`);
  }
}

function readCachedImageFilter(
  reader: DisplayListCursor,
  context: ReplayContext,
): ImageFilterLease {
  const cacheKey = imageFilterCacheKey(reader, context);
  const cached = cacheKey === null ? undefined : imageFilterCache.get(cacheKey);
  if (cached) {
    imageFilterCacheHits++;
    imageFilterCache.delete(cacheKey!);
    imageFilterCache.set(cacheKey!, cached);
    reader.bytes(cached.consumedBytes);
    return { object: cached.filter.object, cropBounds: cached.filter.cropBounds, transient: null };
  }
  imageFilterCacheMisses++;
  const startedAt = reader.offset;
  const filter = readImageFilter(reader, context, 0, false)!;
  if (cacheKey === null) return { object: filter.object, cropBounds: filter.cropBounds, transient: filter };
  while (imageFilterCache.size >= maximumImageFilterCacheSize) {
    const oldestKey = imageFilterCache.keys().next().value as string | undefined;
    if (oldestKey === undefined) break;
    deleteCachedImageFilter(oldestKey, true);
  }
  imageFilterCache.set(cacheKey, { filter, consumedBytes: reader.offset - startedAt });
  return { object: filter.object, cropBounds: filter.cropBounds, transient: null };
}

function imageFilterCacheKey(reader: DisplayListCursor, context: ReplayContext): string | null {
  if (reader.remaining > maximumCachedImageFilterWireBytes) return null;
  const bytes = new Uint8Array(
    context.view.buffer,
    context.view.byteOffset + reader.offset,
    reader.remaining);
  const characters = new Array<string>(bytes.length);
  for (let index = 0; index < bytes.length; index++) characters[index] = String.fromCharCode(bytes[index]);
  return `${context.resourceDeclarationKey}|${characters.join("")}`;
}

function releaseImageFilterLease(lease: ImageFilterLease): void {
  deleteOwned(lease.transient);
}

function deleteCachedImageFilter(key: string, eviction: boolean): void {
  const cached = imageFilterCache.get(key);
  if (!cached) return;
  imageFilterCache.delete(key);
  deleteOwned(cached.filter);
  if (eviction) imageFilterCacheEvictions++;
}

function clearImageFilterCache(invalidation: boolean): void {
  for (const key of [...imageFilterCache.keys()]) deleteCachedImageFilter(key, false);
  if (invalidation) imageFilterCacheInvalidations++;
}

function findMatchingRestore(
  commands: readonly { readonly opcode: number }[],
  scopeStart: number,
  end: number,
): number {
  let depth = 1;
  for (let index = scopeStart + 1; index < end; index++) {
    const opcode = commands[index].opcode;
    if (opcode === 1 || opcode === 3 || (opcode >= 48 && opcode <= 52)) depth++;
    else if (opcode === 2 && --depth === 0) return index;
  }
  throw new Error(`CanvasKit DisplayList scope at command ${scopeStart} has no matching Restore.`);
}

function drawRuntimeEffectImageFilter(
  scene: RasterScene,
  context: ReplayContext,
  recipe: RuntimeEffectImageFilterRecipe,
  offset: Float32Array,
  bounds: Float32Array | null,
  childStart: number,
  childEnd: number,
): void {
  const kit = context.kit;
  const target = context.canvas;
  const matrix = target.getTotalMatrix();
  const clip = target.getDeviceClipBounds();
  let mappedLeft = Number(clip[0]);
  let mappedTop = Number(clip[1]);
  let mappedRight = Number(clip[2]);
  let mappedBottom = Number(clip[3]);
  if (bounds) {
    const left = bounds[0] + offset[0];
    const top = bounds[1] + offset[1];
    const right = bounds[2] + offset[0];
    const bottom = bounds[3] + offset[1];
    const points = kit.Matrix.mapPoints(matrix, [
      left, top, right, top, right, bottom, left, bottom,
    ]);
    mappedLeft = Math.min(points[0], points[2], points[4], points[6]);
    mappedTop = Math.min(points[1], points[3], points[5], points[7]);
    mappedRight = Math.max(points[0], points[2], points[4], points[6]);
    mappedBottom = Math.max(points[1], points[3], points[5], points[7]);
  }
  if (![mappedLeft, mappedTop, mappedRight, mappedBottom].every(Number.isFinite))
    throw new Error("Runtime-effect image-filter bounds mapped to non-finite device coordinates.");
  const metadata = scene.document.metadata;
  const left = Math.max(0, Number(clip[0]), Math.floor(mappedLeft));
  const top = Math.max(0, Number(clip[1]), Math.floor(mappedTop));
  const right = Math.min(metadata.physicalWidth, Number(clip[2]), Math.ceil(mappedRight));
  const bottom = Math.min(metadata.physicalHeight, Number(clip[3]), Math.ceil(mappedBottom));
  if (right <= left || bottom <= top) return;
  const width = right - left;
  const height = bottom - top;
  const inverse = kit.Matrix.invert(matrix);
  if (!inverse)
    throw new Error("Runtime-effect image-filter output cannot invert the active canvas transform.");
  const inputSurface = acquireImageFilterSurface(width, height);
  if (!inputSurface.object.reportBackendTypeIsGPU()) {
    releaseImageFilterSurface(inputSurface);
    throw new Error("Runtime-effect image-filter input surface is not GPU backed; fallback is forbidden.");
  }
  let inputImage: OwnedObject<Image> | null = null;
  let mipmappedInput: OwnedObject<Image> | null = null;
  let inputShader: OwnedObject<Shader> | null = null;
  const explicitChildShaders: OwnedObject<Shader>[] = [];
  let runtimeShader: OwnedObject<Shader> | null = null;
  let paint: OwnedObject<Paint> | null = null;
  try {
    const inputCanvas = inputSurface.object.getCanvas();
    const inputRootMatrix = inputCanvas.getTotalMatrix();
    const identity = [1, 0, 0, 0, 1, 0, 0, 0, 1];
    if (inputRootMatrix.some((value, index) => Math.abs(value - identity[index]) > 0.000001))
      throw new Error("CanvasKit pooled runtime-effect surface retained drawing state between scenes.");
    inputCanvas.clear(kit.TRANSPARENT);
    const inputRootSaveCount = inputCanvas.getSaveCount();
    inputCanvas.save();
    try {
      inputCanvas.translate(-left, -top);
      inputCanvas.concat(matrix);
      inputCanvas.translate(offset[0], offset[1]);
      replayCommandRange(scene, { ...context, canvas: inputCanvas }, childStart, childEnd);
    } finally {
      // Pooled filter surfaces are reused by later scenes. Keep their root
      // matrix and clip immutable instead of compounding the DPR transform on
      // every runtime-effect pass.
      inputCanvas.restoreToCount(inputRootSaveCount);
    }
    inputSurface.object.flush();
    inputImage = own("ImageFilterInputImage", inputSurface.object.makeImageSnapshot());
    if (recipe.sampling === 2)
      mipmappedInput = own("ImageFilterInputImage", inputImage.object.makeCopyWithDefaultMipmaps());
    const shaderImage = mipmappedInput?.object ?? inputImage.object;
    inputShader = own("Shader", recipe.sampling === 3
      ? shaderImage.makeShaderCubic(kit.TileMode.Decal, kit.TileMode.Decal, 1 / 3, 1 / 3)
      : shaderImage.makeShaderOptions(
        kit.TileMode.Decal, kit.TileMode.Decal,
        recipe.sampling === 0 ? kit.FilterMode.Nearest : kit.FilterMode.Linear,
        recipe.sampling === 2 ? kit.MipmapMode.Linear : kit.MipmapMode.None));
    for (const image of recipe.childImages) {
      explicitChildShaders.push(own("Shader", image.makeShaderOptions(
        kit.TileMode.Clamp, kit.TileMode.Clamp,
        kit.FilterMode.Nearest, kit.MipmapMode.None)));
    }
    const uniforms = recipe.uniforms.slice();
    uniforms[0] = width;
    uniforms[1] = height;
    runtimeShader = own("Shader", recipe.effect.makeShaderWithChildren(
      uniforms, [inputShader.object, ...explicitChildShaders.map(child => child.object)]));
    paint = own("Paint", new kit.Paint());
    paint.object.setShader(runtimeShader.object);
    paint.object.setBlendMode(kit.BlendMode.SrcOver);
    target.save();
    try {
      target.concat(inverse);
      target.translate(left, top);
      target.drawRect(kit.XYWHRect(0, 0, width, height), paint.object);
    } finally {
      target.restore();
    }
  } finally {
    deleteOwned(paint);
    deleteOwned(runtimeShader);
    for (const child of explicitChildShaders) deleteOwned(child);
    deleteOwned(inputShader);
    deleteOwned(mipmappedInput);
    deleteOwned(inputImage);
    releaseImageFilterSurface(inputSurface);
  }
}

function acquireImageFilterSurface(width: number, height: number): ImageFilterSurfaceLease {
  const slot = nextImageFilterSurfaceSlot++;
  if (slot >= maximumImageFilterSurfacePoolSize) {
    const created = requireCanvasKit().MakeRenderTarget(requireGrContext(), width, height);
    if (!created)
      throw new Error("CanvasKit could not create a transient GPU runtime-effect image-filter surface.");
    return { object: created, transient: own("ImageFilterSurfaceOverflow", created) };
  }
  const existing = imageFilterSurfacePool[slot];
  if (existing && existing.width() === width && existing.height() === height)
    return { object: existing, transient: null };
  if (existing) {
    existing.delete();
    countDeleted("ImageFilterSurfacePool", 0);
  }
  const created = requireCanvasKit().MakeRenderTarget(requireGrContext(), width, height);
  if (!created)
    throw new Error("CanvasKit could not create a pooled GPU runtime-effect image-filter surface.");
  imageFilterSurfacePool[slot] = created;
  countCreated("ImageFilterSurfacePool", 0);
  return { object: created, transient: null };
}

function releaseImageFilterSurface(value: ImageFilterSurfaceLease): void {
  deleteOwned(value.transient);
}

function samplingOptions(kit: CanvasKit, value: number) {
  return value === 3
    ? { B: 1 / 3, C: 1 / 3 }
    : {
      filter: value === 0 ? kit.FilterMode.Nearest : kit.FilterMode.Linear,
      mipmap: value === 2 ? kit.MipmapMode.Linear : kit.MipmapMode.None,
    };
}

function requireDepth(depth: number, reader: DisplayListCursor): void {
  if (depth > maximumNestingDepth)
    throw reader.error(`Tagged-value nesting exceeds ${maximumNestingDepth}.`);
}

function readParagraph(reader: DisplayListCursor, context: ReplayContext): ParsedParagraph {
  const text = stringAt(reader, context, reader.uint32());
  const font = readResourceReference(reader, context, 1);
  const fontFamily = stringAt(reader, context, reader.uint32());
  const locale = stringAt(reader, context, reader.uint32());
  const ellipsisIndex = reader.uint32();
  const ellipsis = ellipsisIndex === 0xffffffff ? null : stringAt(reader, context, ellipsisIndex);
  const fontSize = reader.positiveFloat("paragraph font size");
  const heightMultiplier = reader.positiveFloat("paragraph height multiplier");
  const color = reader.uint32();
  const fontWeight = reader.int32();
  if (fontWeight < 1 || fontWeight > 1000)
    throw reader.error(`Paragraph font weight ${fontWeight} is outside [1,1000].`);
  const fontSlant = readEnum(reader, 2, "font slant");
  const direction = readEnum(reader, 2, "text direction");
  const align = readEnum(reader, 6, "text alignment");
  if (reader.uint8() !== 0) throw reader.error("Paragraph reserved byte must be zero.");
  const maxLines = reader.uint32();
  const layoutWidth = reader.nonnegativeFloat("paragraph layout width");
  const measuredWidth = reader.nonnegativeFloat("paragraph measured width");
  const measuredHeight = reader.nonnegativeFloat("paragraph measured height");
  const metricsHash = reader.uint64();
  const fallbackCount = reader.collectionCount("fallback font");
  const fallbackFonts: DisplayResourceReference[] = [];
  for (let index = 0; index < fallbackCount; index++)
    fallbackFonts.push(readResourceReference(reader, context, 1));
  const runCount = reader.collectionCount("paragraph text run");
  const textRuns: ParsedParagraphTextRun[] = [];
  for (let index = 0; index < runCount; index++) {
    const runText = stringAt(reader, context, reader.uint32());
    if (!runText) throw reader.error("Paragraph text runs must be nonempty.");
    const runFontFamily = stringAt(reader, context, reader.uint32());
    const runLocale = stringAt(reader, context, reader.uint32());
    const runFontSize = reader.positiveFloat("run font size");
    const runHeightMultiplier = reader.positiveFloat("run height multiplier");
    const runColor = reader.uint32();
    const runFontWeight = reader.int32();
    if (runFontWeight < 1 || runFontWeight > 1000)
      throw reader.error(`Run font weight ${runFontWeight} is outside [1,1000].`);
    const runFontSlant = readEnum(reader, 2, "run font slant");
    const decoration = reader.uint32();
    if ((decoration & ~7) !== 0)
      throw reader.error(`Run decoration ${decoration} contains unknown bits.`);
    const backgroundColor = reader.boolean() ? reader.uint32() : null;
    const decorationColor = reader.boolean() ? reader.uint32() : null;
    const decorationStyle = reader.boolean() ? readEnum(reader, 5, "run decoration style") : null;
    const decorationThickness = reader.boolean()
      ? reader.nonnegativeFloat("run decoration thickness") : null;
    const textBaseline = reader.boolean() ? readEnum(reader, 2, "run text baseline") : null;
    const letterSpacing = reader.boolean() ? reader.float32() : null;
    const wordSpacing = reader.boolean() ? reader.float32() : null;
    const halfLeadingValue = reader.uint8();
    const halfLeading = halfLeadingValue === 0 ? null
      : halfLeadingValue === 1 ? false
        : halfLeadingValue === 2 ? true
          : (() => { throw reader.error(`Run half-leading state ${halfLeadingValue} is invalid.`); })();
    const fallbackFamilyCount = reader.collectionCount("run fallback font family");
    const fontFamilyFallback: string[] = [];
    for (let fallbackIndex = 0; fallbackIndex < fallbackFamilyCount; fallbackIndex++)
      fontFamilyFallback.push(stringAt(reader, context, reader.uint32()));
    const shadowCount = reader.collectionCount("run shadow");
    const shadows: { color: number; dx: number; dy: number; blurRadius: number }[] = [];
    for (let shadowIndex = 0; shadowIndex < shadowCount; shadowIndex++) {
      shadows.push({
        color: reader.uint32(),
        dx: reader.float32(),
        dy: reader.float32(),
        blurRadius: reader.nonnegativeFloat("run shadow blur radius"),
      });
    }
    const featureCount = reader.collectionCount("run font feature");
    const fontFeatures: { name: string; value: number }[] = [];
    for (let featureIndex = 0; featureIndex < featureCount; featureIndex++)
      fontFeatures.push({ name: stringAt(reader, context, reader.uint32()), value: reader.int32() });
    const variationCount = reader.collectionCount("run font variation");
    const fontVariations: { axis: string; value: number }[] = [];
    for (let variationIndex = 0; variationIndex < variationCount; variationIndex++)
      fontVariations.push({ axis: stringAt(reader, context, reader.uint32()), value: reader.float32() });
    textRuns.push({
      text: runText,
      fontFamily: runFontFamily,
      locale: runLocale,
      fontSize: runFontSize,
      heightMultiplier: runHeightMultiplier,
      color: runColor,
      fontWeight: runFontWeight,
      fontSlant: runFontSlant,
      decoration,
      backgroundColor,
      decorationColor,
      decorationStyle,
      decorationThickness,
      textBaseline,
      letterSpacing,
      wordSpacing,
      halfLeading,
      fontFamilyFallback,
      shadows,
      fontFeatures,
      fontVariations,
    });
  }
  if (textRuns.length !== 0 && textRuns.map((run) => run.text).join("") !== text)
    throw reader.error("Paragraph text runs do not concatenate to paragraph text.");
  return {
    text, font, fontFamily, locale, ellipsis, fontSize, heightMultiplier, color, fontWeight, fontSlant,
    direction, align, maxLines, layoutWidth, measuredWidth, measuredHeight, metricsHash,
    fallbackFonts, textRuns,
  };
}

function stringAt(reader: DisplayListCursor, context: ReplayContext, index: number): string {
  const value = context.strings[index];
  if (value === undefined) throw reader.error(`String-table index ${index} is out of range.`);
  return value;
}

function drawParagraph(
  context: ReplayContext,
  recipe: ParsedParagraph,
  x: number,
  y: number,
): void {
  const cacheKey = paragraphCacheKey(recipe);
  const cached = paragraphCache.get(cacheKey);
  if (cached) {
    paragraphCacheHits++;
    paragraphCache.delete(cacheKey);
    paragraphCache.set(cacheKey, cached);
    context.canvas.drawParagraph(cached.paragraph.object, x, y);
    return;
  }
  paragraphCacheMisses++;
  let builder: OwnedObject<ReturnType<CanvasKit["ParagraphBuilder"]["MakeFromFontCollection"]>> | null = null;
  let paragraph: OwnedObject<Paragraph> | null = null;
  try {
    const fontCollectionKey = paragraphFontCollectionKey(recipe);
    const fonts = paragraphFontCollection(context, recipe, fontCollectionKey);
    const paragraphStyle = new context.kit.ParagraphStyle({
      ellipsis: recipe.ellipsis ?? undefined,
      maxLines: recipe.maxLines === 0 ? undefined : recipe.maxLines,
      textAlign: textAlign(context.kit, recipe.align),
      textDirection: recipe.direction === 0 ? context.kit.TextDirection.LTR : context.kit.TextDirection.RTL,
      textStyle: {
        color: argbColor(context.kit, recipe.color),
        fontFamilies: paragraphFontFamilies(context, recipe),
        fontSize: recipe.fontSize,
        heightMultiplier: recipe.heightMultiplier,
        fontStyle: {
          weight: fontWeight(context.kit, recipe.fontWeight),
          width: context.kit.FontWidth.Normal,
          slant: recipe.fontSlant === 0 ? context.kit.FontSlant.Upright : context.kit.FontSlant.Italic,
        },
        locale: recipe.locale || undefined,
      },
    });
    builder = own("ParagraphBuilder", context.kit.ParagraphBuilder.MakeFromFontCollection(
      paragraphStyle, fonts.collection.object));
    if (recipe.textRuns.length === 0) {
      builder.object.addText(recipe.text);
    } else {
      for (const run of recipe.textRuns) {
        builder.object.pushStyle(paragraphRunTextStyle(context.kit, run));
        builder.object.addText(run.text);
        builder.object.pop();
      }
    }
    paragraph = own("Paragraph", builder.object.build());
    paragraph.object.layout(recipe.layoutWidth);
    const unresolved = paragraph.object.unresolvedCodepoints();
    if (unresolved.length !== 0)
      throw new Error(`CanvasKit paragraph has unresolved codepoints: ${unresolved.join(",")}.`);
    const actualHash = paragraphMetricsHash(context, paragraph.object, recipe.text, recipe.layoutWidth);
    if (actualHash !== recipe.metricsHash)
      throw new Error(
        `CanvasKit paragraph metrics hash mismatch: UI=${recipe.metricsHash}, Raster=${actualHash}.`);
    const actualLongestLine = Math.max(0, paragraph.object.getLongestLine());
    if (Math.abs(paragraph.object.getHeight() - recipe.measuredHeight) > 0.01 ||
        Math.abs(actualLongestLine - recipe.measuredWidth) > 0.01)
      throw new Error(
        `CanvasKit paragraph geometry mismatch: UI=${recipe.measuredWidth}x${recipe.measuredHeight}, ` +
        `Raster=${actualLongestLine}x${paragraph.object.getHeight()}.`);
    while (paragraphCache.size >= maximumParagraphCacheSize) {
      const oldestKey = paragraphCache.keys().next().value as string | undefined;
      if (oldestKey === undefined) break;
      deleteCachedParagraph(oldestKey, true);
    }
    const retained = paragraph;
    paragraph = null;
    paragraphCache.set(cacheKey, { paragraph: retained, fontCollectionKey });
    context.canvas.drawParagraph(retained.object, x, y);
  } finally {
    deleteOwned(paragraph);
    deleteOwned(builder);
  }
}

function paragraphCacheKey(recipe: ParsedParagraph): string {
  return JSON.stringify({
    ...recipe,
    font: paragraphResourceIdentity(recipe.font),
    metricsHash: recipe.metricsHash.toString(16),
    fallbackFonts: recipe.fallbackFonts.map(paragraphResourceIdentity),
  });
}

function paragraphResourceIdentity(reference: DisplayResourceReference): readonly [number, string, number] {
  return [reference.kind, reference.id.toString(16), reference.version];
}

function deleteCachedParagraph(key: string, eviction: boolean): void {
  const cached = paragraphCache.get(key);
  if (!cached) return;
  paragraphCache.delete(key);
  deleteOwned(cached.paragraph);
  if (eviction) paragraphCacheEvictions++;
}

function clearParagraphCache(invalidation: boolean): void {
  for (const key of [...paragraphCache.keys()]) deleteCachedParagraph(key, false);
  if (invalidation) paragraphCacheInvalidations++;
}

function clearParagraphCaches(invalidation: boolean): void {
  clearParagraphCache(invalidation);
  for (const fonts of paragraphFontCollectionCache.values()) {
    deleteOwned(fonts.collection);
    deleteOwned(fonts.provider);
  }
  paragraphFontCollectionCache.clear();
}

function paragraphRunTextStyle(kit: CanvasKit, run: ParsedParagraphTextRun) {
  const style: TextStyle = {
    color: argbColor(kit, run.color),
    decoration: run.decoration,
    fontFamilies: [run.fontFamily, ...run.fontFamilyFallback]
      .filter((family, index, values) => family.length !== 0 && values.indexOf(family) === index),
    fontSize: run.fontSize,
    fontStyle: {
      weight: fontWeight(kit, run.fontWeight),
      width: kit.FontWidth.Normal,
      slant: run.fontSlant === 0 ? kit.FontSlant.Upright : kit.FontSlant.Italic,
    },
    heightMultiplier: run.heightMultiplier,
    locale: run.locale || undefined,
    shadows: run.shadows.map((shadow) => ({
      color: argbColor(kit, shadow.color),
      offset: [shadow.dx, shadow.dy],
      blurRadius: shadow.blurRadius,
    })),
    fontFeatures: [...run.fontFeatures],
    fontVariations: run.fontVariations.some((variation) => variation.axis === "wght")
      ? [...run.fontVariations]
      : [...run.fontVariations, { axis: "wght", value: run.fontWeight }],
  };
  if (run.backgroundColor !== null) style.backgroundColor = argbColor(kit, run.backgroundColor);
  if (run.decorationColor !== null) style.decorationColor = argbColor(kit, run.decorationColor);
  if (run.decorationStyle !== null) {
    style.decorationStyle = [
      kit.DecorationStyle.Solid,
      kit.DecorationStyle.Double,
      kit.DecorationStyle.Dotted,
      kit.DecorationStyle.Dashed,
      kit.DecorationStyle.Wavy,
    ][run.decorationStyle];
  }
  if (run.decorationThickness !== null) style.decorationThickness = run.decorationThickness;
  if (run.textBaseline !== null) style.textBaseline = run.textBaseline === 0
    ? kit.TextBaseline.Alphabetic : kit.TextBaseline.Ideographic;
  if (run.letterSpacing !== null) style.letterSpacing = run.letterSpacing;
  if (run.wordSpacing !== null) style.wordSpacing = run.wordSpacing;
  if (run.halfLeading !== null) style.halfLeading = run.halfLeading;
  return new kit.TextStyle(style);
}

function paragraphFontCollection(
  context: ReplayContext,
  recipe: ParsedParagraph,
  key: string,
): ParagraphFontCollection {
  const cached = paragraphFontCollectionCache.get(key);
  if (cached) {
    paragraphFontCollectionCache.delete(key);
    paragraphFontCollectionCache.set(key, cached);
    return cached;
  }
  const references = [recipe.font, ...recipe.fallbackFonts];
  let provider: OwnedObject<TypefaceFontProvider> | null = null;
  let collection: OwnedObject<FontCollection> | null = null;
  try {
    provider = own("TypefaceFontProvider", context.kit.TypefaceFontProvider.Make());
    const seen = new Set<string>();
    registerParagraphFont(context, provider.object, references[0], recipe.fontFamily, seen);
    for (let index = 1; index < references.length; index++)
      registerParagraphFont(context, provider.object, references[index], undefined, seen);
    collection = own("FontCollection", context.kit.FontCollection.Make());
    collection.object.setDefaultFontManager(provider.object);
    const created = { provider, collection };
    while (paragraphFontCollectionCache.size >= maximumParagraphFontCollectionCacheSize) {
      // Paragraphs retain shaping data owned by their font collection. Drop
      // those dependants before evicting the least-recently-used collection.
      const oldestKey = paragraphFontCollectionCache.keys().next().value as string | undefined;
      if (oldestKey === undefined) break;
      for (const [paragraphKey, paragraph] of paragraphCache) {
        if (paragraph.fontCollectionKey === oldestKey) deleteCachedParagraph(paragraphKey, false);
      }
      const oldest = paragraphFontCollectionCache.get(oldestKey)!;
      paragraphFontCollectionCache.delete(oldestKey);
      deleteOwned(oldest.collection);
      deleteOwned(oldest.provider);
      paragraphFontCollectionCacheEvictions++;
    }
    paragraphFontCollectionCache.set(key, created);
    return created;
  } catch (error) {
    deleteOwned(collection);
    deleteOwned(provider);
    throw error;
  }
}

function paragraphFontCollectionKey(recipe: ParsedParagraph): string {
  return JSON.stringify([
    paragraphResourceIdentity(recipe.font),
    recipe.fontFamily,
    recipe.fallbackFonts.map(paragraphResourceIdentity),
  ]);
}

function registerParagraphFont(
  context: ReplayContext,
  provider: TypefaceFontProvider,
  reference: DisplayResourceReference,
  requestedFamily: string | undefined,
  seen: Set<string>,
): void {
  const resource = requireResource(reference, context);
  const descriptor = parseDescriptor(resource.descriptorJson);
  const family = (requestedFamily?.trim() || String(descriptor.family ?? descriptor.fontFamily ?? "").trim());
  if (!family) throw new Error(`CanvasKit font ${reference.id}/${reference.version} has no family name.`);
  const key = `${reference.id}/${reference.version}/${family}`;
  if (seen.has(key)) return;
  provider.registerFont(resource.bytes, family);
  seen.add(key);
}

function paragraphFontFamilies(context: ReplayContext, recipe: ParsedParagraph): string[] {
  const result = [recipe.fontFamily];
  for (const reference of recipe.fallbackFonts) {
    const resource = requireResource(reference, context);
    const descriptor = parseDescriptor(resource.descriptorJson);
    const family = String(descriptor.family ?? descriptor.fontFamily ?? "").trim();
    if (family && !result.includes(family)) result.push(family);
  }
  return result;
}

function parseDescriptor(value: string): Record<string, unknown> {
  const parsed = JSON.parse(value || "{}");
  if (!parsed || typeof parsed !== "object" || Array.isArray(parsed))
    throw new Error("CanvasKit resource descriptor must be a JSON object.");
  return parsed as Record<string, unknown>;
}

function textAlign(kit: CanvasKit, value: number) {
  switch (value) {
    case 0: return kit.TextAlign.Start;
    case 1: return kit.TextAlign.End;
    case 2: return kit.TextAlign.Left;
    case 3: return kit.TextAlign.Right;
    case 4: return kit.TextAlign.Center;
    case 5: return kit.TextAlign.Justify;
    default: throw new Error(`Invalid text alignment ${value}.`);
  }
}

function fontWeight(kit: CanvasKit, value: number) {
  if (value <= 150) return kit.FontWeight.Thin;
  if (value <= 250) return kit.FontWeight.ExtraLight;
  if (value <= 350) return kit.FontWeight.Light;
  if (value <= 450) return kit.FontWeight.Normal;
  if (value <= 550) return kit.FontWeight.Medium;
  if (value <= 650) return kit.FontWeight.SemiBold;
  if (value <= 750) return kit.FontWeight.Bold;
  if (value <= 850) return kit.FontWeight.ExtraBold;
  if (value <= 950) return kit.FontWeight.Black;
  return kit.FontWeight.ExtraBlack;
}

function paragraphMetricsHash(
  context: ReplayContext,
  paragraph: Paragraph,
  text: string,
  width: number,
): bigint {
  const lines = paragraph.getLineMetrics().map(line => ({
    start: line.startIndex,
    end: line.endIndex,
    hardBreak: line.isHardBreak,
    ascent: line.ascent,
    descent: line.descent,
    height: line.height,
    width: line.width,
    left: line.left,
    baseline: line.baseline,
  }));
  const graphemes = paragraphGraphemeSnapshots(context.kit, paragraph, text);
  const codeUnitAdvances = new Array<number>(text.length).fill(0);
  for (const grapheme of graphemes)
    codeUnitAdvances[grapheme.start] = grapheme.right - grapheme.left;
  const result = {
    width,
    height: paragraph.getHeight(),
    alphabeticBaseline: paragraph.getAlphabeticBaseline(),
    ideographicBaseline: paragraph.getIdeographicBaseline(),
    minIntrinsicWidth: paragraph.getMinIntrinsicWidth(),
    maxIntrinsicWidth: paragraph.getMaxIntrinsicWidth(),
    longestLine: Math.max(0, paragraph.getLongestLine()),
    didExceedMaxLines: paragraph.didExceedMaxLines(),
    numberOfLines: lines.length,
    metricsHash: "",
    codeUnitAdvances,
    graphemes,
    lines,
    unresolvedCodepoints: paragraph.unresolvedCodepoints(),
  };
  return fnv1a64(JSON.stringify(result));
}

function paragraphGraphemeSnapshots(
  kit: CanvasKit,
  paragraph: Paragraph,
  text: string,
): ParagraphGraphemeSnapshot[] {
  const result: ParagraphGraphemeSnapshot[] = [];
  const seen = new Set<string>();
  for (let index = 0; index < text.length; index++) {
    const info = paragraph.getGlyphInfoAt(index);
    if (!info) continue;
    const start = Number(info.graphemeClusterTextRange.start);
    const end = Number(info.graphemeClusterTextRange.end);
    if (!Number.isSafeInteger(start) || !Number.isSafeInteger(end) ||
        start < 0 || end <= start || end > text.length)
      throw new Error(`CanvasKit returned invalid grapheme range [${start}, ${end}).`);
    const key = `${start}:${end}`;
    if (seen.has(key)) continue;
    seen.add(key);
    const [left, top, right, bottom] = [...info.graphemeLayoutBounds].map(Number);
    if (![left, top, right, bottom].every(Number.isFinite) || right < left || bottom < top)
      throw new Error(`CanvasKit returned invalid grapheme bounds for [${start}, ${end}).`);
    const strutRects = paragraph.getRectsForRange(
      start, end, kit.RectHeightStyle.Strut, kit.RectWidthStyle.Tight);
    if (strutRects.length !== 1)
      throw new Error(
        `CanvasKit returned ${strutRects.length} strut rectangles for grapheme [${start}, ${end}).`);
    const [strutLeft, strutTop, strutRight, strutBottom] = [...strutRects[0].rect].map(Number);
    if (![strutLeft, strutTop, strutRight, strutBottom].every(Number.isFinite) ||
        strutRight < strutLeft || strutBottom < strutTop)
      throw new Error(`CanvasKit returned invalid strut bounds for [${start}, ${end}).`);
    if (Math.abs(strutLeft - left) > 0.001 || Math.abs(strutRight - right) > 0.001)
      throw new Error(`CanvasKit strut bounds changed the tight width for [${start}, ${end}).`);
    const direction = info.dir === kit.TextDirection.LTR
      ? "ltr"
      : info.dir === kit.TextDirection.RTL
        ? "rtl"
        : null;
    if (!direction)
      throw new Error(`CanvasKit returned invalid grapheme direction for [${start}, ${end}).`);
    result.push({ start, end, left, top, right, bottom, strutTop, strutBottom, direction });
  }
  result.sort((left, right) => left.start - right.start || left.end - right.end);
  for (let index = 1; index < result.length; index++) {
    if (result[index - 1].end > result[index].start)
      throw new Error("CanvasKit returned overlapping grapheme ranges.");
  }
  return result;
}

function fnv1a64(value: string): bigint {
  const bytes = new TextEncoder().encode(value);
  let hash = 0xcbf29ce484222325n;
  for (const byte of bytes) {
    hash ^= BigInt(byte);
    hash = BigInt.asUintN(64, hash * 0x100000001b3n);
  }
  return hash;
}

function drawShadow(
  context: ReplayContext,
  path: Path,
  color: number,
  elevation: number,
  transparentOccluder: boolean,
): void {
  const drawPass = (offsetY: number, opacity: number, sigma: number): void => {
    const filter = own("ImageFilter", context.kit.ImageFilter.MakeBlur(
      sigma, sigma, context.kit.TileMode.Decal, null));
    const paint = own("Paint", new context.kit.Paint());
    try {
      paint.object.setColor(withOpacityByte(context.kit, color, opacity));
      paint.object.setImageFilter(filter.object);
      paint.object.setAntiAlias(true);
      context.canvas.save();
      try {
        context.canvas.translate(0, offsetY);
        context.canvas.drawPath(path, paint.object);
      } finally {
        context.canvas.restore();
      }
    } finally {
      deleteOwned(paint);
      deleteOwned(filter);
    }
  };
  drawPass(elevation * 0.2, transparentOccluder ? 0.18 : 0.24, Math.max(0.75, elevation * 0.45));
  drawPass(elevation * 0.55, transparentOccluder ? 0.24 : 0.32, Math.max(1, elevation * 0.8));
}

function drawImage(
  context: ReplayContext,
  image: Image,
  x: number,
  y: number,
  sampling: number,
  paint: Paint,
): void {
  let mipmapped: OwnedObject<Image> | null = null;
  try {
    if (sampling === 2) mipmapped = own("Image", image.makeCopyWithDefaultMipmaps());
    const source = mipmapped?.object ?? image;
    if (sampling === 3) context.canvas.drawImageCubic(source, x, y, 1 / 3, 1 / 3, paint);
    else context.canvas.drawImageOptions(
      source, x, y,
      sampling === 0 ? context.kit.FilterMode.Nearest : context.kit.FilterMode.Linear,
      sampling === 2 ? context.kit.MipmapMode.Linear : context.kit.MipmapMode.None,
      paint);
  } finally {
    deleteOwned(mipmapped);
  }
}

function drawImageRect(
  context: ReplayContext,
  image: Image,
  source: Float32Array,
  destination: Float32Array,
  sampling: number,
  paint: Paint,
): void {
  let mipmapped: OwnedObject<Image> | null = null;
  try {
    if (sampling === 2) mipmapped = own("Image", image.makeCopyWithDefaultMipmaps());
    const drawable = mipmapped?.object ?? image;
    if (sampling === 3)
      context.canvas.drawImageRectCubic(drawable, source, destination, 1 / 3, 1 / 3, paint);
    else context.canvas.drawImageRectOptions(
      drawable, source, destination,
      sampling === 0 ? context.kit.FilterMode.Nearest : context.kit.FilterMode.Linear,
      sampling === 2 ? context.kit.MipmapMode.Linear : context.kit.MipmapMode.None,
      paint);
  } finally {
    deleteOwned(mipmapped);
  }
}

function drawNinePatch(
  context: ReplayContext,
  image: Image,
  center: Float32Array,
  destination: Float32Array,
  sampling: number,
  paint: Paint,
): void {
  const imageWidth = image.width();
  const imageHeight = image.height();
  if (center[0] < 0 || center[0] > center[2] || center[2] > imageWidth ||
      center[1] < 0 || center[1] > center[3] || center[3] > imageHeight)
    throw new Error(
      `Nine-patch center ${[...center].join(",")} is outside image ${imageWidth}x${imageHeight}.`);
  if (destination[0] > destination[2] || destination[1] > destination[3])
    throw new Error(`Nine-patch destination ${[...destination].join(",")} is inverted.`);

  const sourceX = [0, center[0], center[2], imageWidth];
  const sourceY = [0, center[1], center[3], imageHeight];
  const destinationX = ninePatchDestinationAxis(
    sourceX, destination[0], destination[2]);
  const destinationY = ninePatchDestinationAxis(
    sourceY, destination[1], destination[3]);
  // CanvasKit.drawImageNine accepts only an integer center plus nearest/linear filtering.
  // Nine public drawImageRect calls preserve fractional centers and all four sampling modes.
  let mipmapped: OwnedObject<Image> | null = null;
  try {
    if (sampling === 2) mipmapped = own("Image", image.makeCopyWithDefaultMipmaps());
    const drawable = mipmapped?.object ?? image;
    for (let row = 0; row < 3; row++) {
      if (sourceY[row] === sourceY[row + 1] || destinationY[row] === destinationY[row + 1]) continue;
      for (let column = 0; column < 3; column++) {
        if (sourceX[column] === sourceX[column + 1] ||
            destinationX[column] === destinationX[column + 1]) continue;
        const source = new Float32Array([
          sourceX[column], sourceY[row], sourceX[column + 1], sourceY[row + 1],
        ]);
        const target = new Float32Array([
          destinationX[column], destinationY[row],
          destinationX[column + 1], destinationY[row + 1],
        ]);
        if (sampling === 3) {
          context.canvas.drawImageRectCubic(
            drawable, source, target, 1 / 3, 1 / 3, paint);
        } else {
          context.canvas.drawImageRectOptions(
            drawable, source, target,
            sampling === 0 ? context.kit.FilterMode.Nearest : context.kit.FilterMode.Linear,
            sampling === 2 ? context.kit.MipmapMode.Linear : context.kit.MipmapMode.None,
            paint);
        }
      }
    }
  } finally {
    deleteOwned(mipmapped);
  }
}

function ninePatchDestinationAxis(
  source: readonly number[],
  destinationStart: number,
  destinationEnd: number,
): readonly number[] {
  const fixedStart = source[1] - source[0];
  const fixedEnd = source[3] - source[2];
  const fixedTotal = fixedStart + fixedEnd;
  const destinationLength = destinationEnd - destinationStart;
  if (fixedTotal > destinationLength && fixedTotal > 0) {
    const scale = destinationLength / fixedTotal;
    const split = destinationStart + fixedStart * scale;
    return [destinationStart, split, split, destinationEnd];
  }
  return [
    destinationStart,
    destinationStart + fixedStart,
    destinationEnd - fixedEnd,
    destinationEnd,
  ];
}

function ensureSurface(target: ResizeEpoch): void {
  if (contextLost) throw new Error("Doroti CanvasKit context is lost.");
  const visible = requireCanvas();
  const expectedSurfaceGeneration = canvasKitSurfaceGeneration(rasterSessionId, target.generation);
  if (visible.width < target.physicalWidth || visible.height < target.physicalHeight)
    throw new Error(
      `CanvasKit visible capacity ${visible.width}x${visible.height} is smaller than ` +
      `${target.physicalWidth}x${target.physicalHeight}.`);
  if (surface) {
    surfaceGeneration = expectedSurfaceGeneration;
    return;
  }
  const nextSurface = requireCanvasKit().MakeOnScreenGLSurface(
    requireGrContext(), visible.width, visible.height, requireCanvasKit().ColorSpace.SRGB);
  if (!nextSurface)
    throw new Error("Doroti CanvasKit MakeOnScreenGLSurface failed; no software/Canvas2D fallback is allowed.");
  surface = nextSurface;
  surfaceGeneration = expectedSurfaceGeneration;
  countCreated("Surface", 0);
}

function receiptScene(scene: RasterScene, success: boolean, reason: string): void {
  if (scene.receipt) throw new Error(`Duplicate CanvasKit raster receipt for scene ${scene.sequence}.`);
  scene.receipt = true;
  rasterReceipts++;
  postPort("scene-receipt", {
    sceneSequence: scene.sequence,
    transferId: scene.transferId,
    success,
    reason,
    contextGeneration,
    surfaceGeneration,
    buffer: scene.buffer,
  }, [scene.buffer]);
}

function terminalScene(
  scene: RasterScene,
  terminal: DorotiSceneTerminal,
  reason: string,
  returnBuffer: boolean,
): void {
  if (scene.terminal) throw new Error(`Duplicate CanvasKit scene terminal ${scene.sequence}.`);
  scene.terminal = true;
  terminalScenes++;
  if (terminal === "submitted") submittedScenes++;
  else if (terminal === "superseded") supersededScenes++;
  else {
    failedScenes++;
    lastFailedSceneSequence = scene.sequence;
    lastFailureReason = reason;
  }
  const payload: Record<string, unknown> = {
    sceneSequence: scene.sequence,
    sentTime: stageTrace.enabled ? performance.timeOrigin + performance.now() : undefined,
    terminal,
    reason,
    attempted: scene.attempted,
    receiptCount: Number(scene.receipt),
  };
  stageTrace.record("raster-terminal-sent", Number(scene.document.metadata.resizeEpoch), scene.sequence, { terminal });
  if (returnBuffer && scene.buffer.byteLength > 0) {
    payload.transferId = scene.transferId;
    payload.buffer = scene.buffer;
    postPort("scene-terminal", payload, [scene.buffer]);
  } else {
    postPort("scene-terminal", payload);
  }
}

function failScene(scene: RasterScene, reason: string, attempted: boolean): void {
  scene.attempted ||= attempted;
  if (scene.attempted && !scene.receipt && scene.buffer.byteLength > 0) receiptScene(scene, false, reason);
  if (!scene.terminal) terminalScene(scene, "failed", reason, !scene.receipt);
}

function retainResource(message: Record<string, unknown>): void {
  const resourceId = positiveInteger(message.resourceId, "resourceId");
  const generation = positiveInteger(message.generation, "generation");
  const transferId = positiveInteger(message.transferId, "transferId");
  const kind = String(message.resourceKind ?? "");
  const descriptorJson = String(message.descriptorJson ?? "{}");
  const replay = Boolean(message.replay);
  const buffer = message.buffer;
  if (!(buffer instanceof ArrayBuffer) || buffer.byteLength !== Number(message.byteLength))
    throw new Error("Doroti CanvasKit resource requires an exact transferred buffer.");
  let terminal = "retained";
  let reason = "CanvasKit resource retained";
  let receiptJson = "{}";
  try {
    const kindCode = resourceKindCode(kind);
    const descriptor = parseDescriptor(descriptorJson);
    if (String(descriptor.kind ?? "") !== kind || Number(descriptor.id) !== resourceId ||
        Number(descriptor.version) !== generation)
      throw new Error(
        `CanvasKit resource envelope/descriptor mismatch for ${kind}/${resourceId}/${generation}.`);
    const key = resourceKey(kindCode, BigInt(resourceId), generation);
    const fingerprint = fingerprintFromSha256(descriptor.sha256);
    const existing = [...resources.values()].find(resource => resource.resourceId === resourceId);
    if (existing && existing.generation >= generation)
      throw new Error(`stale resource generation ${generation}; current=${existing.generation}`);
    const ownedBytes = new Uint8Array(buffer).slice();
    const object = createResourceObject(kind, descriptorJson, ownedBytes);
    if (existing) {
      clearPaintCache(true);
      clearImageFilterCache(true);
      if (existing.kind === "font" || kind === "font") clearParagraphCaches(true);
    }
    if (existing) deleteResource(existing);
    if (existing) resources.delete(resourceKey(resourceKindCode(existing.kind), BigInt(resourceId), existing.generation));
    const resource: RasterResource = {
      resourceId, generation, kind, descriptorJson, object, bytes: ownedBytes, fingerprint,
      byteLength: buffer.byteLength,
    };
    resources.set(key, resource);
    countCreated(kind, buffer.byteLength);
    if (kind === "image" && object) {
      const image = object as Image;
      const info = image.getImageInfo();
      receiptJson = JSON.stringify({ width: image.width(), height: image.height(), colorType: Number(info.colorType) });
    }
  } catch (error) {
    terminal = "failed";
    reason = String(error);
  }
  postPort("resource-receipt", {
    resourceId, generation, transferId, operation: "retain", terminal, reason, receiptJson, replay, buffer,
  }, [buffer]);
}

function releaseResource(message: Record<string, unknown>): void {
  const resourceId = positiveInteger(message.resourceId, "resourceId");
  const generation = positiveInteger(message.generation, "generation");
  const existing = [...resources.values()].find(resource => resource.resourceId === resourceId);
  let terminal = "released";
  let reason = "CanvasKit resource released";
  if (!existing) {
    reason = "CanvasKit resource was already absent";
  } else if (generation < existing.generation) {
    terminal = "failed";
    reason = `stale release generation ${generation}; current=${existing.generation}`;
  } else {
    clearPaintCache(true);
    clearImageFilterCache(true);
    if (existing.kind === "font") clearParagraphCaches(true);
    deleteResource(existing);
    resources.delete(resourceKey(resourceKindCode(existing.kind), BigInt(resourceId), existing.generation));
  }
  postPort("resource-receipt", {
    resourceId, generation, operation: "release", terminal, reason, receiptJson: "{}", replay: false,
  });
}

function createResourceObject(kind: string, descriptorJson: string, bytes: Uint8Array): RasterResourceObject {
  const kit = requireCanvasKit();
  switch (kind) {
    case "font": {
      const owned = new Uint8Array(bytes.byteLength);
      owned.set(bytes);
      const typeface = kit.Typeface.MakeFreeTypeFaceFromData(
        owned.buffer as ArrayBuffer);
      if (!typeface) throw new Error("CanvasKit could not decode font resource.");
      return typeface;
    }
    case "image": {
      const image = kit.MakeImageFromEncoded(bytes);
      if (!image) throw new Error("CanvasKit could not decode image resource.");
      return image;
    }
    case "runtime-effect": {
      const descriptor = JSON.parse(descriptorJson) as Record<string, unknown>;
      const source = String(descriptor.sksl ?? new TextDecoder().decode(bytes));
      let compileError = "";
      const effect = kit.RuntimeEffect.Make(source, (error) => { compileError = error; });
      if (!effect) throw new Error(`CanvasKit RuntimeEffect compile failed: ${compileError}`);
      return effect;
    }
    case "retained-scene":
      {
        const picture = kit.MakePicture(bytes);
        if (!picture) throw new Error("CanvasKit could not decode retained-scene picture resource.");
        return picture;
      }
    default:
      throw new Error(`Unknown CanvasKit resource kind '${kind}'.`);
  }
}

function deleteResource(resource: RasterResource): void {
  resource.object?.delete();
  countDeleted(resource.kind, resource.byteLength);
}

function disposeRasterRole(): void {
  rasterDisposed = true;
  if (currentScene && !currentScene.terminal) failScene(currentScene, "Raster Worker disposing", false);
  if (latestScene && !latestScene.terminal) failScene(latestScene, "Raster Worker disposing", false);
  currentScene = null;
  latestScene = null;
  clearPaintCache(false);
  clearImageFilterCache(false);
  clearParagraphCaches(false);
  for (const resource of resources.values()) deleteResource(resource);
  resources.clear();
  for (const pooledSurface of imageFilterSurfacePool) {
    pooledSurface.delete();
    countDeleted("ImageFilterSurfacePool", 0);
  }
  imageFilterSurfacePool.length = 0;
  nextImageFilterSurfaceSlot = 0;
  if (resizeStagingSurface) {
    resizeStagingSurface.delete();
    countDeleted("ResizeStagingSurface", 0);
    resizeStagingSurface = null;
  }
  resizeStagingCapacityWidth = 0;
  resizeStagingCapacityHeight = 0;
  if (resizeCommitPaint) {
    resizeCommitPaint.delete();
    countDeleted("ResizeCommitPaint", 0);
    resizeCommitPaint = null;
  }
  if (surface) {
    surface.delete();
    countDeleted("Surface", 0);
    surface = null;
  }
  frontPhysicalWidth = 0;
  frontPhysicalHeight = 0;
  if (grContext) {
    grContext.delete();
    countDeleted("GrDirectContext", 0);
    grContext = null;
  }
  contextHandle = 0;
  contextLossExtension = null;
  canvasKit = null;
  canvas = null;
  publishDiagnostics();
  post("disposed", { diagnostics: diagnostics() });
  port?.close();
  port = null;
  close();
}

function publishDiagnostics(): void {
  const value = diagnostics();
  post("raster-diagnostics", { diagnostics: value });
  if (port) postPort("raster-diagnostics", { diagnostics: value });
}

function diagnostics(): Readonly<Record<string, unknown>> {
  return {
    topologyVersion,
    canvasKitOwnerCount: canvasKit ? 1 : 0,
    managedRuntimeCount: 0,
    visibleCanvasContextOwnerCount: contextHandle ? 1 : 0,
    drainScheduling,
    presentation,
    bitmapCredits: bitmapCredits.size,
    bitmapCreated,
    bitmapAcknowledged,
    bitmapBudgetBytes,
    contextGeneration,
    surfaceGeneration,
    contextLost,
    currentScene: currentScene?.sequence ?? null,
    latestScene: latestScene?.sequence ?? null,
    queueDepth: Number(currentScene !== null) + Number(latestScene !== null),
    queueHighWater,
    admittedScenes,
    terminalScenes,
    rasterAttempts,
    rasterReceipts,
    submittedScenes,
    supersededScenes,
    failedScenes,
    lastFailedSceneSequence,
    lastFailureReason,
    flushCount,
    lastFrontGeneration,
    latestTargetPriority: {
      minimumGenerationGap: latestTargetPriorityMinimumGenerationGap,
      maximumFrontAgeMilliseconds: latestTargetPriorityMaximumFrontAgeMilliseconds,
      minimumPriorReplayMilliseconds: latestTargetPriorityMinimumPriorReplayMilliseconds,
      skippedScenes: latestTargetPrioritySkippedScenes,
      forcedProgressiveScenes: latestTargetPriorityForcedProgressiveScenes,
      maximumSkippedGenerationGap: latestTargetPriorityMaximumSkippedGenerationGap,
    },
    resizeTargetIngress: {
      mainFastLaneCount: mainFastLaneResizeTargetCount,
      uiOrderedCount: uiOrderedResizeTargetCount,
    },
    resourceCount: resources.size,
    resourceBytes: [...resources.values()].reduce((total, resource) => total + resource.byteLength, 0),
    paragraphCache: {
      capacity: maximumParagraphCacheSize,
      size: paragraphCache.size,
      hits: paragraphCacheHits,
      misses: paragraphCacheMisses,
      evictions: paragraphCacheEvictions,
      invalidations: paragraphCacheInvalidations,
      fontCollectionCapacity: maximumParagraphFontCollectionCacheSize,
      fontCollectionSize: paragraphFontCollectionCache.size,
      fontCollectionEvictions: paragraphFontCollectionCacheEvictions,
    },
    paintCache: {
      capacity: maximumPaintCacheSize,
      size: paintCache.size,
      maximumWireBytes: maximumCachedPaintWireBytes,
      hits: paintCacheHits,
      misses: paintCacheMisses,
      evictions: paintCacheEvictions,
      invalidations: paintCacheInvalidations,
    },
    imageFilterCache: {
      capacity: maximumImageFilterCacheSize,
      size: imageFilterCache.size,
      maximumWireBytes: maximumCachedImageFilterWireBytes,
      hits: imageFilterCacheHits,
      misses: imageFilterCacheMisses,
      evictions: imageFilterCacheEvictions,
      invalidations: imageFilterCacheInvalidations,
    },
    resizeStagingPool: {
      allocations: resizeStagingSurfaceAllocations,
      reuses: resizeStagingSurfaceReuses,
      capacityWidth: resizeStagingCapacityWidth,
      capacityHeight: resizeStagingCapacityHeight,
      peakPixels: resizeStagingPeakPixels,
    },
    timings: {
      replayCount,
      replayTotalMilliseconds,
      replayLastMilliseconds,
      replayMaximumMilliseconds,
      resizeStagingCount,
      resizeStagingTotalMilliseconds,
      resizeStagingLastMilliseconds,
      resizeStagingMaximumMilliseconds,
    },
    objects: Object.fromEntries(objectCounters),
    referenceCompatibility: {
      addSuperellipseNoOps: referenceCompatibilityAddSuperellipseNoOps,
      appliedBlurCropBounds,
    },
    diagnosticRasterStallCount,
    lastDiagnosticRasterStallMilliseconds,
    physicalWidth: frontPhysicalWidth,
    physicalHeight: frontPhysicalHeight,
    capacityWidth: canvas?.width ?? 0,
    capacityHeight: canvas?.height ?? 0,
    gpuResourceCacheLimitBytes: grContext?.getResourceCacheLimitBytes() ?? null,
    gpuResourceCacheUsageBytes: grContext?.getResourceCacheUsageBytes() ?? null,
  };
}

function countCreated(kind: string, bytes: number): void {
  const value = objectCounters.get(kind) ?? { created: 0, deleted: 0, live: 0, bytes: 0 };
  value.created++;
  value.live++;
  value.bytes += bytes;
  objectCounters.set(kind, value);
}

function countDeleted(kind: string, bytes: number): void {
  const value = objectCounters.get(kind) ?? { created: 0, deleted: 0, live: 0, bytes: 0 };
  value.deleted++;
  value.live--;
  value.bytes -= bytes;
  if (value.live < 0 || value.bytes < 0) throw new Error(`CanvasKit object counter underflow for '${kind}'.`);
  objectCounters.set(kind, value);
}

function gpuIdentity(gl: WebGL2RenderingContext): Readonly<Record<string, unknown>> {
  const debug = gl.getExtension("WEBGL_debug_renderer_info");
  const vendor = String(debug ? gl.getParameter(debug.UNMASKED_VENDOR_WEBGL) : gl.getParameter(gl.VENDOR));
  const renderer = String(debug ? gl.getParameter(debug.UNMASKED_RENDERER_WEBGL) : gl.getParameter(gl.RENDERER));
  const softwareFallbackUsed = /swiftshader|llvmpipe|software/.test(`${vendor} ${renderer}`.toLowerCase());
  if (softwareFallbackUsed) throw new Error(`Doroti rejected software WebGL renderer '${renderer}'.`);
  return { api: "webgl2", vendor, renderer, hardware: true, softwareFallbackUsed: false };
}

function validateResizeTarget(value: ResizeEpoch | null): asserts value is ResizeEpoch {
  if (!value || !Number.isSafeInteger(value.generation) || value.generation <= 0 ||
      !Number.isFinite(value.logicalWidth) || value.logicalWidth <= 0 ||
      !Number.isFinite(value.logicalHeight) || value.logicalHeight <= 0 ||
      !Number.isSafeInteger(value.physicalWidth) || value.physicalWidth <= 0 ||
      !Number.isSafeInteger(value.physicalHeight) || value.physicalHeight <= 0 ||
      !Number.isFinite(value.devicePixelRatio) || value.devicePixelRatio <= 0)
    throw new Error("Doroti CanvasKit Raster role received invalid resize metrics.");
}

function rememberResizeTarget(value: ResizeEpoch): void {
  const current = resizeTargets.get(value.generation);
  if (current) {
    if (current.logicalWidth !== value.logicalWidth || current.logicalHeight !== value.logicalHeight ||
        current.physicalWidth !== value.physicalWidth || current.physicalHeight !== value.physicalHeight ||
        current.devicePixelRatio !== value.devicePixelRatio)
      throw new Error(`Doroti CanvasKit resize generation ${value.generation} changed identity.`);
    return;
  }
  if (resizeTarget && value.generation < resizeTarget.generation) return;
  resizeTarget = value;
  resizeTargets.set(value.generation, value);
  pruneResizeTargets();
}

function pruneResizeTargets(): void {
  for (const generation of resizeTargets.keys()) {
    if (generation < lastFrontGeneration) resizeTargets.delete(generation);
  }
  while (resizeTargets.size > maximumResizeTargetHistory) {
    const oldest = resizeTargets.keys().next().value as number | undefined;
    if (oldest === undefined || oldest === resizeTarget?.generation) break;
    resizeTargets.delete(oldest);
  }
}

function exactResizeGeneration(value: bigint): number {
  const generation = Number(value);
  if (!Number.isSafeInteger(generation) || generation <= 0 || BigInt(generation) !== value)
    throw new Error("Doroti CanvasKit DisplayList resize generation is not an exact positive integer.");
  return generation;
}

function requireCanvasKit(): CanvasKit {
  if (!canvasKit) throw new Error("Doroti CanvasKit Raster runtime is unavailable.");
  return canvasKit;
}

function requireCanvas(): OffscreenCanvas {
  if (!canvas) throw new Error("Doroti CanvasKit visible canvas lease is unavailable.");
  return canvas;
}

function requireGrContext(): GrDirectContext {
  if (!grContext) throw new Error("Doroti CanvasKit GrDirectContext is unavailable.");
  return grContext;
}

function requireSurface(): Surface {
  if (!surface) throw new Error("Doroti CanvasKit on-screen surface is unavailable.");
  return surface;
}

function requireResizeTarget(): ResizeEpoch {
  if (!resizeTarget) throw new Error("Doroti CanvasKit resize target is unavailable.");
  return resizeTarget;
}

function requireMessagePort(value: unknown): MessagePort {
  if (!(value instanceof MessagePort)) throw new Error("Doroti CanvasKit topology requires a MessagePort.");
  return value;
}

function positiveInteger(value: unknown, name: string): number {
  const number = Number(value);
  if (!Number.isSafeInteger(number) || number <= 0)
    throw new Error(`Doroti CanvasKit '${name}' must be a positive safe integer.`);
  return number;
}
