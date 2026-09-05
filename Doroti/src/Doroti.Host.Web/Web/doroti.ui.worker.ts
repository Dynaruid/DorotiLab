import type { CanvasKit, CanvasKitInitOptions } from "canvaskit-wasm";
import { CanvasKitStageTrace } from "./doroti.canvaskit.trace.js";
import { captureCanvasKitFrameworkTrace } from "./doroti.web.js";
import {
  completeCanvasKitResource,
  completeCanvasKitScene,
  configureCanvasKitUiBridge,
  configureWorkerBridge,
  dispatchWorkerAnimationFrame,
  dispatchWorkerInput,
  dispatchWorkerResizeEpoch,
  dispatchWorkerSnapshot,
  initializeCanvasKitManagedCallbacks,
  initializeManagedCallbacks,
  type CanvasKitUiBridge,
} from "./doroti.web.js";
import {
  canvasKitSurfaceGeneration,
  decodeDorotiMessage,
  displayListSequenceAsNumber,
  dorotiCanvasKitTopologyVersion,
  dorotiProtocolVersion,
  validateDorotiDisplayList,
  type DorotiSceneTerminal,
} from "./doroti.web.protocol.js";
import { TransferBufferPool } from "./doroti.web.worker-host.js";
import { CanvasKitTextLayoutService } from "./doroti.canvaskit.text.js";

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

interface HostSnapshot {
  canvasId: string;
  logicalWidth: number;
  logicalHeight: number;
  devicePixelRatio: number;
  generation: number;
  surfaceGeneration: number;
  inputSequence: number;
  gpu: Readonly<Record<string, unknown>>;
  resizeEpoch: ResizeEpoch;
  readonly [name: string]: unknown;
}

interface DotnetRuntime {
  getAssemblyExports(name: string): Promise<unknown>;
  getConfig(): { mainAssemblyName: string };
  exit(code: number, reason?: unknown): void;
}

interface QueuedScene {
  readonly sequence: number;
  readonly bytes: Uint8Array;
  readonly expectedContextGeneration: number;
  readonly expectedSurfaceGeneration: number;
  sent: boolean;
  transferId: number | null;
  receiptCount: number;
  receiptSuccess: boolean | null;
  receiptContextGeneration: number | null;
  receiptSurfaceGeneration: number | null;
  terminal: boolean;
}

interface JournalResource {
  readonly resourceId: number;
  readonly generation: number;
  readonly kind: string;
  readonly descriptorJson: string;
  readonly bytes: Uint8Array;
  acknowledged: boolean;
}

const protocolVersion = dorotiProtocolVersion;
const topologyVersion = dorotiCanvasKitTopologyVersion;
const inboundKinds = new Set([
  "snapshot", "resize-epoch", "input", "control-response", "raster-port-rebind", "dispose", "crash", "collect-stage-trace",
]);
const pool = new TransferBufferPool(4);
const stageTrace = new CanvasKitStageTrace();
let resizeFixture = "F3";
let adoptManagedCopy = false;
let pictureCache = false;
let encodingCache = false;
let coalesceMetrics = false;
let pendingResizeMessage: Record<string, unknown> | null = null;
let applyingResizeMessage = false;
let coalescedResizeCount = 0;
const resources = new Map<number, JournalResource>();
const resourceOperations = new Set<string>();
const pendingControls = new Map<number, { resolve(value: string): void; reject(reason: unknown): void }>();

let canvasKit: CanvasKit | null = null;
let textService: CanvasKitTextLayoutService | null = null;
let snapshot: HostSnapshot | null = null;
let hostId = 1;
let sessionId = 0;
let rasterSessionId = 0;
let rasterPort: MessagePort | null = null;
let rasterReady = false;
let currentScene: QueuedScene | null = null;
let latestScene: QueuedScene | null = null;
let sceneQueueHighWater = 0;
let sceneAdmissions = 0;
let sceneTerminals = 0;
let rasterReceipts = 0;
let resourceReceipts = 0;
let resourceReplayCount = 0;
let lastAdmittedSceneSequence = 0;
let controlSequence = 0;
let heartbeatSequence = 0;
let heartbeatTimer = 0;
let inputDispatchCount = 0;
let lastInputSequence = 0;
let managedInputSequenceBaseline = 0;
let managedHostReady = false;
let managedRuntimeStarting = false;
let uiCanvasKitReady = false;
let managedRuntime: DotnetRuntime | null = null;
let stopManagedRuntime: (() => void) | null = null;
let dotnetModuleUrl = "";
let frameDispatchCount = 0;
let frameDispatchTotalMilliseconds = 0;
let frameDispatchMaximumMilliseconds = 0;
let frameWaitTotalMilliseconds = 0;
let frameWaitMaximumMilliseconds = 0;
let tracedParagraphMilliseconds = 0;
let tracedParagraphCount = 0;
let activeFrameCallbackId = 0;
const liveResizeTargetFramesPerSecond = 30;
let liveResizeThrottleEnabled = true;
const liveResizeFrameIntervalMilliseconds = 1000 / liveResizeTargetFramesPerSecond;
const liveResizeActivityWindowMilliseconds = 100;
const liveResizeFrameIntervalToleranceMilliseconds = 0.25;
let pendingManagedFrame: {
  hostId: number;
  callbackId: number;
  requestedMilliseconds: number;
} | null = null;
let managedFrameRaf = 0;
let lastManagedFrameDispatchMilliseconds = Number.NEGATIVE_INFINITY;
let lastResizeEpochIngressMilliseconds = Number.NEGATIVE_INFINITY;
let managedFrameRequestCoalescedCount = 0;
let liveResizeThrottleDeferredRafCount = 0;
let liveResizeThrottleDeferredTotalMilliseconds = 0;
let liveResizeThrottleDeferredMaximumMilliseconds = 0;
let liveResizeThrottleDispatchCount = 0;
let liveResizeThrottleMinimumDispatchIntervalMilliseconds = Number.POSITIVE_INFINITY;
const pendingInputs: Record<string, unknown>[] = [];

function post(kind: string, payload: Record<string, unknown> = {}, transfer: Transferable[] = []): void {
  (globalThis as unknown as { postMessage(message: unknown, transfer: Transferable[]): void }).postMessage({
    protocolVersion,
    topologyVersion,
    role: "ui",
    sessionId,
    kind,
    ...payload,
  }, transfer);
}

function postRaster(kind: string, payload: Record<string, unknown> = {}, transfer: Transferable[] = []): void {
  if (!rasterPort) throw new Error("Doroti CanvasKit Raster port is not bound.");
  rasterPort.postMessage({
    protocolVersion,
    topologyVersion,
    kind,
    uiSessionId: sessionId,
    rasterSessionId,
    ...payload,
  }, transfer);
}

function scheduleManagedFrame(): void {
  if ((pendingManagedFrame === null && pendingResizeMessage === null) || managedFrameRaf !== 0 || applyingResizeMessage) return;
  if (typeof globalThis.requestAnimationFrame !== "function")
    throw new Error("Doroti CanvasKit UI Worker requires requestAnimationFrame.");

  const scheduledRaf = globalThis.requestAnimationFrame((timestamp) => {
    if (managedFrameRaf !== scheduledRaf) return;
    managedFrameRaf = 0;
    flushPendingResize();
    const pending = pendingManagedFrame;
    if (pending === null) return;

    const now = performance.now();
    const dispatchInterval = now - lastManagedFrameDispatchMilliseconds;
    const liveResizeActive =
      now - lastResizeEpochIngressMilliseconds <= liveResizeActivityWindowMilliseconds;
    if (liveResizeThrottleEnabled && liveResizeActive && Number.isFinite(lastManagedFrameDispatchMilliseconds) &&
        dispatchInterval + liveResizeFrameIntervalToleranceMilliseconds <
          liveResizeFrameIntervalMilliseconds) {
      const deferredMilliseconds = liveResizeFrameIntervalMilliseconds - dispatchInterval;
      liveResizeThrottleDeferredRafCount++;
      liveResizeThrottleDeferredTotalMilliseconds += deferredMilliseconds;
      liveResizeThrottleDeferredMaximumMilliseconds = Math.max(
        liveResizeThrottleDeferredMaximumMilliseconds, deferredMilliseconds);
      // Stay on the Worker's animation clock. A newer managed request can
      // replace pendingManagedFrame before the next eligible resize frame.
      scheduleManagedFrame();
      return;
    }

    pendingManagedFrame = null;
    lastManagedFrameDispatchMilliseconds = now;
    if (liveResizeActive) {
      liveResizeThrottleDispatchCount++;
      if (Number.isFinite(dispatchInterval)) {
        liveResizeThrottleMinimumDispatchIntervalMilliseconds = Math.min(
          liveResizeThrottleMinimumDispatchIntervalMilliseconds, dispatchInterval);
      }
    }
    const wait = now - pending.requestedMilliseconds;
    const started = performance.now();
    const paragraphStarted = tracedParagraphMilliseconds;
    const paragraphCount = tracedParagraphCount;
    stageTrace.record("frame-start", snapshot?.resizeEpoch.generation, 0, { callbackId: pending.callbackId, wait });
    activeFrameCallbackId = pending.callbackId;
    try { dispatchWorkerAnimationFrame(pending.hostId, pending.callbackId, timestamp); }
    finally { activeFrameCallbackId = 0; }
    stageTrace.record("frame-end", snapshot?.resizeEpoch.generation, lastAdmittedSceneSequence, {
      callbackId: pending.callbackId,
      paragraphMilliseconds: tracedParagraphMilliseconds - paragraphStarted,
      paragraphCount: tracedParagraphCount - paragraphCount,
    });
    const duration = performance.now() - started;
    frameDispatchCount++;
    frameDispatchTotalMilliseconds += duration;
    frameDispatchMaximumMilliseconds = Math.max(frameDispatchMaximumMilliseconds, duration);
    frameWaitTotalMilliseconds += wait;
    frameWaitMaximumMilliseconds = Math.max(frameWaitMaximumMilliseconds, wait);
  });
  managedFrameRaf = scheduledRaf;
}

const bridge: CanvasKitUiBridge = {
  get adoptsManagedCopy() { return adoptManagedCopy; },
  recordInteropCopy(milliseconds, bytes, copies) {
    stageTrace.record("interop-copy", snapshot?.resizeEpoch.generation, 0, { milliseconds, bytes, copies, callbackId: activeFrameCallbackId });
  },
  submitDisplayList(bytes) {
    const copyStarted = performance.now();
    const document = validateDorotiDisplayList(bytes);
    const sequence = displayListSequenceAsNumber(document.metadata.sceneSequence);
    if (sequence <= lastAdmittedSceneSequence)
      throw new Error(
        `Doroti CanvasKit scene sequence ${sequence} is not newer than ${lastAdmittedSceneSequence}.`);
    lastAdmittedSceneSequence = sequence;
    sceneAdmissions++;
    const scene: QueuedScene = {
      sequence,
      // The bridge transfers exclusive ownership of its managed->JS copy.
      // Keep this journal through terminal/rebind; pool.copy remains necessary.
      bytes: adoptManagedCopy ? bytes : bytes.slice(),
      expectedContextGeneration: exactGeneration(
        document.metadata.contextGeneration, "DisplayList contextGeneration"),
      expectedSurfaceGeneration: exactGeneration(
        document.metadata.surfaceGeneration, "DisplayList surfaceGeneration"),
      sent: false,
      transferId: null,
      receiptCount: 0,
      receiptSuccess: null,
      receiptContextGeneration: null,
      receiptSurfaceGeneration: null,
      terminal: false,
    };
    stageTrace.record("scene-encoded", Number(document.metadata.resizeEpoch), sequence, {
      validateAndCopyMilliseconds: performance.now() - copyStarted, bytes: bytes.byteLength,
      callbackId: activeFrameCallbackId,
    });
    if (!currentScene) {
      currentScene = scene;
    } else {
      if (latestScene) finishScene(latestScene, "superseded", "UI current+latest mailbox replaced pending scene");
      latestScene = scene;
    }
    sceneQueueHighWater = Math.max(sceneQueueHighWater,
      Number(currentScene !== null) + Number(latestScene !== null));
    sendCurrentScene();
    publishDiagnostics();
    return sequence;
  },
  registerResource(resourceId, generation, kind, descriptorJson, bytes) {
    validateResourceIdentity(resourceId, generation, kind);
    const key = resourceKey(resourceId, generation, "retain");
    if (resourceOperations.has(key))
      throw new Error(`Doroti CanvasKit resource operation '${key}' is already pending.`);
    if (kind === "font") requireTextService().registerFont(resourceId, descriptorJson, bytes);
    const entry: JournalResource = {
      resourceId, generation, kind, descriptorJson, bytes: bytes.slice(), acknowledged: false,
    };
    resources.set(resourceId, entry);
    resourceOperations.add(key);
    if (rasterReady) sendResource(entry, false);
    publishDiagnostics();
  },
  releaseResource(resourceId, generation) {
    validateResourceIdentity(resourceId, generation, "release");
    const existing = resources.get(resourceId);
    if (existing?.kind === "font") requireTextService().releaseFont(resourceId);
    resources.delete(resourceId);
    const key = resourceKey(resourceId, generation, "release");
    if (resourceOperations.has(key))
      throw new Error(`Doroti CanvasKit resource operation '${key}' is already pending.`);
    resourceOperations.add(key);
    if (rasterReady) {
      postRaster("release-resource", { resourceId, generation });
    } else {
      resourceOperations.delete(key);
      completeCanvasKitResource(resourceId, generation, "released", "Raster owner is not active", "{}");
    }
    publishDiagnostics();
  },
  layoutParagraph(requestJson) {
    if (!stageTrace.enabled) return requireTextService().layout(requestJson);
    const started = performance.now();
    try { return requireTextService().layout(requestJson); }
    finally {
      tracedParagraphMilliseconds += performance.now() - started;
      tracedParagraphCount++;
    }
  },
};

export async function startCanvasKitRole(context: CanvasKitRoleContext): Promise<void> {
  const envelope = context.initEnvelope;
  stageTrace.enabled = envelope.stageTrace === true;
  resizeFixture = String(envelope.resizeFixture ?? "F3");
  if (!["baseline", "owned"].includes(String(envelope.copyOwnership ?? "baseline"))) throw new Error("Unknown copy ownership");
  adoptManagedCopy = envelope.copyOwnership === "owned";
  pictureCache = envelope.pictureCache === true;
  encodingCache = envelope.encodingCache === true;
  coalesceMetrics = envelope.metricsCoalescing === "frame";
  if (!["F0", "F1", "F2", "F3"].includes(resizeFixture)) throw new Error("Unknown resize fixture");
  liveResizeThrottleEnabled = envelope.resizeScheduling !== "display";
  if (envelope.role !== "ui") throw new Error("Doroti UI role received a non-UI bootstrap envelope.");
  sessionId = positiveInteger(envelope.sessionId, "sessionId");
  dotnetModuleUrl = String(envelope.dotnetModuleUrl ?? "");
  snapshot = envelope.snapshot as HostSnapshot;
  if (!snapshot?.resizeEpoch) throw new Error("Doroti CanvasKit UI role requires an initial host snapshot.");
  managedInputSequenceBaseline = nonNegativeInteger(
    snapshot.inputSequence, "initial snapshot inputSequence");
  bindRasterPort(requireMessagePort(envelope.rasterPort), positiveInteger(envelope.rasterSessionId, "rasterSessionId"));
  snapshot = withRasterIdentity(snapshot);
  const started = performance.now();
  canvasKit = await context.CanvasKitInit({
    locateFile(file) {
      if (file === "canvaskit.wasm" || file.endsWith("/canvaskit.wasm")) return context.canvasKitWasmUrl;
      throw new Error(`Doroti CanvasKit UI role rejected unexpected runtime file '${file}'.`);
    },
  });
  textService = new CanvasKitTextLayoutService(canvasKit);
  const textSmoke = textService.smoke();
  configureCanvasKitUiBridge(bridge);
  configureUiWorkerBridge();
  installMainListener();
  uiCanvasKitReady = true;
  heartbeatTimer = globalThis.setInterval(() => {
    post("ui-heartbeat", {
      sequence: ++heartbeatSequence,
      timestampMicroseconds: Math.round(performance.now() * 1000),
      inputDispatchCount,
      lastInputSequence,
    });
  }, 25);
  post("ui-canvaskit-ready", {
    canvasKitOwnerCount: 1,
    managedRuntimeCount: 0,
    domCanvasContextCount: 0,
    textSmoke,
    initMicroseconds: Math.round((performance.now() - started) * 1000),
  });
  maybeStartManagedRuntime();
}

function configureUiWorkerBridge(): void {
  configureWorkerBridge({
    rendererIdentity: () => "worker-canvaskit-webgl",
    createHost(id, canvasId, logicalWidth, logicalHeight) {
      const value = requireSnapshot();
      hostId = id;
      snapshot = { ...value, canvasId, logicalWidth, logicalHeight };
      return JSON.stringify(snapshot);
    },
    showHost: () => JSON.stringify(requireSnapshot()),
    resizeHost(_id, logicalWidth, logicalHeight) {
      snapshot = { ...requireSnapshot(), logicalWidth, logicalHeight };
      return JSON.stringify(snapshot);
    },
    requestFrame(id, callbackId) {
      if (pendingManagedFrame !== null) managedFrameRequestCoalescedCount++;
      pendingManagedFrame = { hostId: id, callbackId, requestedMilliseconds: performance.now() };
      scheduleManagedFrame();
    },
    recordManagedRaster(id, phase, width, height, duration) {
      stageTrace.record(phase, snapshot?.resizeEpoch.generation, 0, { durationMicroseconds: duration, width, height, callbackId: activeFrameCallbackId });
      if (phase.startsWith("canvaskit-picture-") || phase === "canvaskit-mapped-command-count") return;
      if (phase === "canvaskit-map" || phase === "canvaskit-encode") return;
      post("managed-work", { hostId: id, phase, width, height, durationMicroseconds: duration });
    },
    requestPresent() {
      throw new Error("DOROTIWEB030: legacy Skia requestPresent is forbidden in CanvasKit DisplayList mode.");
    },
    captureResizeTrace: () => "[]",
    closeHost(id) { post("closed", { hostId: id }); },
    resolveResourceUrl(relativeUrl) {
      return new URL(relativeUrl, new URL("../../", globalThis.location.href)).href;
    },
    postControl(kind, payload) { post("control", { controlKind: kind, payload }); },
    requestControl(kind, payload) {
      const correlationId = ++controlSequence;
      const promise = new Promise<string>((resolve, reject) =>
        pendingControls.set(correlationId, { resolve, reject }));
      post("control-request", { correlationId, controlKind: kind, payload });
      return promise;
    },
  });
}

function flushPendingResize(): void {
  if (!pendingResizeMessage) return;
  const message = pendingResizeMessage;
  pendingResizeMessage = null;
  applyingResizeMessage = true;
  try { applyResizeMessage(message); }
  finally { applyingResizeMessage = false; }
}

function applyResizeMessage(message: Record<string, unknown>): void {
  const epoch = message.resizeEpoch as ResizeEpoch;
  snapshot = {
    ...requireSnapshot(), logicalWidth: epoch.logicalWidth, logicalHeight: epoch.logicalHeight,
    devicePixelRatio: epoch.devicePixelRatio,
    generation: Math.max(requireSnapshot().generation, Number(message.hostGeneration)),
    surfaceGeneration: canvasKitSurfaceGeneration(rasterSessionId, epoch.generation),
    gpu: { ...requireSnapshot().gpu, contextGeneration: rasterSessionId,
      surfaceGeneration: canvasKitSurfaceGeneration(rasterSessionId, epoch.generation) },
    resizeEpoch: epoch,
  };
  if (rasterReady) postRaster("resize-target", { resizeEpoch: epoch });
  if (managedHostReady) {
    dispatchWorkerResizeEpoch(Number(message.hostId), Number(message.hostGeneration), epoch.generation,
      epoch.logicalWidth, epoch.logicalHeight, epoch.physicalWidth, epoch.physicalHeight,
      epoch.devicePixelRatio, epoch.timestampMicroseconds);
    dispatchWorkerSnapshot(Number(message.hostId), JSON.stringify(snapshot));
  }
  stageTrace.record("ui-resize-applied", epoch.generation);
}

function installMainListener(): void {
  globalThis.addEventListener("message", (event: MessageEvent) => {
    let message: Record<string, unknown>;
    try {
      message = decodeDorotiMessage(event.data, inboundKinds);
    } catch (error) {
      post("fatal", { error: `UI control protocol violation: ${String(error)}` });
      return;
    }
    try {
      // Input, focus, IME, snapshots and lifecycle are barriers. Only adjacent
      // metrics may be replaced, and a frame flushes metrics before it starts.
      if (message.kind !== "resize-epoch") { flushPendingResize(); scheduleManagedFrame(); }
      switch (message.kind) {
        case "collect-stage-trace":
          post("stage-trace", { collectionId: message.collectionId,
            trace: { ...stageTrace.snapshot(), framework: captureCanvasKitFrameworkTrace() } });
          break;
        case "snapshot":
          snapshot = withRasterIdentity(message.snapshot as HostSnapshot);
          if (!managedHostReady) snapshot = {
            ...snapshot,
            inputSequence: managedInputSequenceBaseline,
          };
          if (managedHostReady) dispatchWorkerSnapshot(Number(message.hostId), JSON.stringify(snapshot));
          break;
        case "resize-epoch": {
          const epoch = message.resizeEpoch as ResizeEpoch;
          stageTrace.record("ui-resize-received", epoch.generation);
          lastResizeEpochIngressMilliseconds = performance.now();
          if (coalesceMetrics && managedHostReady) {
            if (pendingResizeMessage) coalescedResizeCount++;
            pendingResizeMessage = message;
            scheduleManagedFrame();
          } else applyResizeMessage(message);
          break;
        }
        case "input":
          if (managedHostReady) dispatchWorkerInput(message);
          else {
            if (pendingInputs.length >= 256) pendingInputs.shift();
            pendingInputs.push(message);
          }
          inputDispatchCount++;
          lastInputSequence = Math.max(
            lastInputSequence, positiveInteger(message.inputSequence, "inputSequence"));
          break;
        case "control-response": {
          const pending = pendingControls.get(Number(message.correlationId));
          if (pending) {
            pendingControls.delete(Number(message.correlationId));
            if (message.error) pending.reject(new Error(String(message.error)));
            else pending.resolve(String(message.result ?? ""));
          }
          break;
        }
        case "raster-port-rebind":
          bindRasterPort(requireMessagePort(message.rasterPort), positiveInteger(message.rasterSessionId, "rasterSessionId"));
          break;
        case "crash":
          post("fatal", { error: "diagnostic UI Worker crash" });
          break;
        case "dispose":
          disposeUiRole();
          break;
      }
    } catch (error) {
      post("fatal", { error: String(error instanceof Error ? error.stack ?? error.message : error) });
    }
  });
}

function bindRasterPort(port: MessagePort, nextRasterSessionId: number): void {
  if (rasterSessionId > 0) {
    pool.abandonSession(rasterSessionId);
    if (Number(pool.snapshot().outstanding) !== 0)
      throw new Error("Doroti CanvasKit Raster rebind left transfer buffers outstanding.");
  }
  unbindRasterPort();
  rasterPort = port;
  rasterSessionId = nextRasterSessionId;
  rasterReady = false;
  if (currentScene) {
    currentScene.sent = false;
    currentScene.transferId = null;
  }
  port.addEventListener("message", handleRasterMessage);
  port.start();
}

function unbindRasterPort(): void {
  if (!rasterPort) return;
  rasterPort.removeEventListener("message", handleRasterMessage);
  rasterPort.close();
  rasterPort = null;
}

function handleRasterMessage(event: MessageEvent): void {
  const message = event.data as Record<string, unknown> | null;
  if (!message || message.protocolVersion !== protocolVersion || message.topologyVersion !== topologyVersion) {
    post("fatal", { error: "UI received a Raster port protocol violation" });
    return;
  }
  if (Number(message.rasterSessionId) !== rasterSessionId) return;
  try {
    switch (message.kind) {
      case "raster-ready":
        rasterReady = true;
        snapshot = withRasterIdentity({
          ...requireSnapshot(),
          gpu: message.gpu as Record<string, unknown>,
        });
        if (managedHostReady) dispatchWorkerSnapshot(hostId, JSON.stringify(snapshot));
        postRaster("resize-target", { resizeEpoch: requireSnapshot().resizeEpoch });
        for (const resource of resources.values()) sendResource(resource, resource.acknowledged);
        sendCurrentScene();
        maybeStartManagedRuntime();
        break;
      case "scene-receipt": {
        const sequence = Number(message.sceneSequence);
        const scene = currentScene?.sequence === sequence ? currentScene : latestScene?.sequence === sequence ? latestScene : null;
        if (!scene || scene.terminal) throw new Error(`Unexpected CanvasKit raster receipt for scene ${sequence}.`);
        scene.receiptCount++;
        scene.receiptSuccess = Boolean(message.success);
        scene.receiptContextGeneration = positiveInteger(
          message.contextGeneration, "scene receipt contextGeneration");
        scene.receiptSurfaceGeneration = positiveInteger(
          message.surfaceGeneration, "scene receipt surfaceGeneration");
        if (scene.receiptSuccess &&
            (scene.receiptContextGeneration !== scene.expectedContextGeneration ||
             scene.receiptSurfaceGeneration !== scene.expectedSurfaceGeneration))
          throw new Error(
            `CanvasKit scene ${sequence} receipt generations do not match its DisplayList header.`);
        rasterReceipts++;
        releaseSceneBuffer(scene, message);
        break;
      }
      case "scene-terminal": {
        const sequence = Number(message.sceneSequence);
        stageTrace.record("ui-terminal-received", 0, sequence, { terminal: message.terminal, sentTime: message.sentTime });
        const scene = currentScene?.sequence === sequence ? currentScene : latestScene?.sequence === sequence ? latestScene : null;
        if (!scene) throw new Error(`Unexpected CanvasKit scene terminal ${sequence}.`);
        if (message.buffer instanceof ArrayBuffer) releaseSceneBuffer(scene, message);
        if (scene.transferId !== null)
          throw new Error(`CanvasKit scene ${sequence} reached terminal without returning transfer ${scene.transferId}.`);
        const terminal = String(message.terminal) as DorotiSceneTerminal;
        if (terminal === "submitted" && scene.receiptSuccess !== true)
          throw new Error(`CanvasKit scene ${sequence} was submitted without a Raster receipt.`);
        finishScene(scene, terminal, String(message.reason ?? "Raster terminal"));
        break;
      }
      case "resource-receipt": {
        const resourceId = Number(message.resourceId);
        const generation = Number(message.generation);
        const operation = String(message.operation);
        if (operation !== "retain" && operation !== "release")
          throw new Error(`CanvasKit resource receipt has invalid operation '${operation}'.`);
        if (operation === "retain") releaseReturnedBuffer(message.buffer, message.transferId);
        else if (message.buffer !== undefined || message.transferId !== undefined)
          throw new Error("CanvasKit resource release receipt unexpectedly returned a transfer buffer.");
        const key = resourceKey(resourceId, generation, operation === "release" ? "release" : "retain");
        const replay = Boolean(message.replay);
        const terminal = String(message.terminal ?? "failed");
        const expectedTerminals = operation === "release"
          ? new Set(["released", "failed"])
          : new Set(["retained", "failed"]);
        if (!expectedTerminals.has(terminal))
          throw new Error(`CanvasKit resource receipt has invalid terminal '${terminal}'.`);
        resourceReceipts++;
        if (replay) {
          const entry = resources.get(resourceId);
          if (operation !== "retain" || !entry || entry.generation !== generation || !entry.acknowledged)
            throw new Error(`CanvasKit resource replay receipt ${resourceId}/${generation} has no journal owner.`);
          if (terminal !== "retained") {
            entry.acknowledged = false;
            throw new Error(
              `CanvasKit resource replay ${resourceId}/${generation} failed: ${String(message.reason)}`);
          }
        } else if (resourceOperations.delete(key)) {
          const entry = resources.get(resourceId);
          if (operation === "retain" && entry && entry.generation === generation)
            entry.acknowledged = terminal === "retained";
          completeCanvasKitResource(
            resourceId, generation, terminal, String(message.reason ?? "Raster resource receipt"),
            String(message.receiptJson ?? "{}"));
        } else {
          throw new Error(`CanvasKit resource receipt '${key}' has no pending operation.`);
        }
        break;
      }
      case "raster-diagnostics":
        post("raster-diagnostics", { diagnostics: message.diagnostics });
        break;
      case "fatal":
        post("raster-fatal", { rasterSessionId, error: String(message.error) });
        break;
    }
  } catch (error) {
    post("fatal", { error: String(error instanceof Error ? error.stack ?? error.message : error) });
  }
  publishDiagnostics();
}

function sendCurrentScene(): void {
  if (!rasterReady || !currentScene || currentScene.sent) return;
  const copyStarted = performance.now();
  const transfer = pool.copy(currentScene.bytes, rasterSessionId);
  currentScene.sent = true;
  currentScene.transferId = transfer.transferId;
  stageTrace.record("scene-send", 0, currentScene.sequence, { copyMilliseconds: performance.now() - copyStarted });
  postRaster("display-list", {
    sceneSequence: currentScene.sequence,
    byteLength: currentScene.bytes.byteLength,
    transferId: transfer.transferId,
    buffer: transfer.buffer,
  }, [transfer.buffer]);
}

function finishScene(scene: QueuedScene, terminal: DorotiSceneTerminal, reason: string): void {
  if (terminal !== "submitted" && terminal !== "superseded" && terminal !== "failed")
    throw new Error(`Unknown Doroti CanvasKit terminal '${terminal}'.`);
  if (scene.terminal)
    throw new Error(`Duplicate Doroti CanvasKit terminal for scene ${scene.sequence}.`);
  scene.terminal = true;
  sceneTerminals++;
  stageTrace.record("ui-scene-terminal", 0, scene.sequence, { terminal });
  completeCanvasKitScene(scene.sequence, terminal, reason, JSON.stringify({
    rasterReceiptCount: scene.receiptCount,
    rasterReceiptSuccess: scene.receiptSuccess,
    rasterSessionId,
    contextGeneration: scene.receiptContextGeneration,
    surfaceGeneration: scene.receiptSurfaceGeneration,
  }));
  if (currentScene === scene) {
    currentScene = latestScene;
    latestScene = null;
    sendCurrentScene();
  } else if (latestScene === scene) {
    latestScene = null;
  }
}

function sendResource(resource: JournalResource, replay: boolean): void {
  if (replay) resourceReplayCount++;
  const transfer = pool.copy(resource.bytes, rasterSessionId);
  postRaster("retain-resource", {
    resourceId: resource.resourceId,
    generation: resource.generation,
    resourceKind: resource.kind,
    descriptorJson: resource.descriptorJson,
    byteLength: resource.bytes.byteLength,
    transferId: transfer.transferId,
    replay,
    buffer: transfer.buffer,
  }, [transfer.buffer]);
}

function maybeStartManagedRuntime(): void {
  if (!uiCanvasKitReady || !rasterReady || managedHostReady || managedRuntimeStarting) return;
  managedRuntimeStarting = true;
  void startManagedRuntime();
}

async function startManagedRuntime(): Promise<void> {
  try {
    const resolvedDotnetModuleUrl = dotnetModuleUrl || new URL("../../_framework/dotnet.js", import.meta.url).href;
    type RuntimeBuilder = { create(): Promise<DotnetRuntime>; withEnvironmentVariables(values: Record<string, string>): RuntimeBuilder };
    const dotnetModule = await import(resolvedDotnetModuleUrl) as { dotnet: RuntimeBuilder };
    const runtime = await dotnetModule.dotnet.withEnvironmentVariables({
      DOROTI_RESIZE_FIXTURE: resizeFixture, DOROTI_PICTURE_CACHE: pictureCache ? "1" : "0",
      DOROTI_ENCODING_CACHE: encodingCache ? "1" : "0",
      DOROTI_STAGE_TRACE: stageTrace.enabled ? "1" : "0",
    }).create();
    managedRuntime = runtime;
    await initializeManagedCallbacks();
    await initializeCanvasKitManagedCallbacks();
    const config = runtime.getConfig();
    const appExports = await runtime.getAssemblyExports(config.mainAssemblyName) as {
      Doroti: { Generated: { DorotiBootstrap: { StartWorker(): Promise<string>; StopWorker(): void } } };
    };
    stopManagedRuntime = appExports.Doroti.Generated.DorotiBootstrap.StopWorker;
    const result = await appExports.Doroti.Generated.DorotiBootstrap.StartWorker();
    managedHostReady = true;
    for (const input of pendingInputs.splice(0)) dispatchWorkerInput(input);
    post("runtime-ready", {
      result,
      mainManagedRuntimeCount: 0,
      uiManagedRuntimeCount: 1,
      rasterManagedRuntimeCount: 0,
    });
  } catch (error) {
    post("fatal", { error: String(error instanceof Error ? error.stack ?? error.message : error) });
  }
}

function disposeUiRole(): void {
  pendingResizeMessage = null;
  if (heartbeatTimer !== 0) globalThis.clearInterval(heartbeatTimer);
  heartbeatTimer = 0;
  if (managedFrameRaf !== 0 && typeof globalThis.cancelAnimationFrame === "function")
    globalThis.cancelAnimationFrame(managedFrameRaf);
  managedFrameRaf = 0;
  pendingManagedFrame = null;
  const openScenes = [currentScene, latestScene].filter((scene): scene is QueuedScene => scene !== null);
  currentScene = null;
  latestScene = null;
  for (const scene of openScenes) {
    if (!scene.terminal) finishScene(scene, "failed", "UI Worker disposing");
  }
  for (const key of resourceOperations) {
    const [resourceId, generation] = key.split(":").map(Number);
    completeCanvasKitResource(resourceId, generation, "failed", "UI Worker disposing", "{}");
  }
  resourceOperations.clear();
  postRaster("shutdown");
  unbindRasterPort();
  stopManagedRuntime?.();
  stopManagedRuntime = null;
  managedRuntime?.exit(0);
  managedRuntime = null;
  textService?.dispose();
  textService = null;
  canvasKit = null;
  pool.clear();
  post("disposed", { diagnostics: diagnostics() });
  close();
}

function publishDiagnostics(): void {
  post("ui-diagnostics", { diagnostics: diagnostics() });
}

function diagnostics(): Readonly<Record<string, unknown>> {
  return {
    copyOwnership: adoptManagedCopy ? "owned" : "baseline",
    pictureCache,
    encodingCache,
    resizeFixture,
    topologyVersion,
    canvasKitOwnerCount: canvasKit ? 1 : 0,
    managedRuntimeCount: managedRuntime ? 1 : 0,
    rasterSessionId,
    rasterReady,
    currentScene: currentScene?.sequence ?? null,
    latestScene: latestScene?.sequence ?? null,
    queueDepth: Number(currentScene !== null) + Number(latestScene !== null),
    queueHighWater: sceneQueueHighWater,
    admittedCount: sceneAdmissions,
    terminalCount: sceneTerminals,
    rasterReceiptCount: rasterReceipts,
    resourceReceiptCount: resourceReceipts,
    resourceReplayCount,
    resourceJournalCount: resources.size,
    pendingResourceOperations: resourceOperations.size,
    text: textService?.diagnostics() ?? { ready: false, fontCount: 0 },
    buffers: pool.snapshot(),
    heartbeatSequence,
    inputDispatchCount,
    lastInputSequence,
    metricsCoalescing: coalesceMetrics ? "frame" : "immediate",
    coalescedResizeCount,
    frameTimings: {
      count: frameDispatchCount,
      dispatchTotalMilliseconds: frameDispatchTotalMilliseconds,
      dispatchMaximumMilliseconds: frameDispatchMaximumMilliseconds,
      waitTotalMilliseconds: frameWaitTotalMilliseconds,
      waitMaximumMilliseconds: frameWaitMaximumMilliseconds,
      coalescedRequestCount: managedFrameRequestCoalescedCount,
      liveResizeThrottle: {
        enabled: liveResizeThrottleEnabled,
        scheduling: liveResizeThrottleEnabled ? "baseline-30fps" : "display-raf",
        targetFramesPerSecond: liveResizeTargetFramesPerSecond,
        frameIntervalMilliseconds: liveResizeFrameIntervalMilliseconds,
        activityWindowMilliseconds: liveResizeActivityWindowMilliseconds,
        deferredRafCount: liveResizeThrottleDeferredRafCount,
        deferredTotalMilliseconds: liveResizeThrottleDeferredTotalMilliseconds,
        deferredMaximumMilliseconds: liveResizeThrottleDeferredMaximumMilliseconds,
        dispatchCount: liveResizeThrottleDispatchCount,
        minimumDispatchIntervalMilliseconds:
          Number.isFinite(liveResizeThrottleMinimumDispatchIntervalMilliseconds)
            ? liveResizeThrottleMinimumDispatchIntervalMilliseconds
            : null,
      },
    },
  };
}

function validateResourceIdentity(resourceId: number, generation: number, kind: string): void {
  if (!Number.isSafeInteger(resourceId) || resourceId <= 0 ||
      !Number.isSafeInteger(generation) || generation <= 0 || !kind)
    throw new Error("Doroti CanvasKit resource id/generation/kind must be positive and explicit.");
}

function resourceKey(resourceId: number, generation: number, operation: "retain" | "release"): string {
  return `${resourceId}:${generation}:${operation}`;
}

function releaseSceneBuffer(scene: QueuedScene, message: Record<string, unknown>): void {
  const transferId = positiveInteger(message.transferId, "scene transferId");
  if (scene.transferId !== transferId)
    throw new Error(
      `CanvasKit scene ${scene.sequence} returned transfer ${transferId}; expected ${scene.transferId}.`);
  releaseReturnedBuffer(message.buffer, transferId);
  scene.transferId = null;
}

function releaseReturnedBuffer(value: unknown, transferIdValue: unknown): void {
  if (!(value instanceof ArrayBuffer))
    throw new Error("CanvasKit transfer receipt did not return an ArrayBuffer.");
  const transferId = positiveInteger(transferIdValue, "transferId");
  pool.release(transferId, rasterSessionId, value);
}

function requireTextService(): CanvasKitTextLayoutService {
  if (!textService) throw new Error("Doroti CanvasKit UI text service is not ready.");
  return textService;
}

function requireSnapshot(): HostSnapshot {
  if (!snapshot) throw new Error("Doroti CanvasKit UI snapshot is unavailable.");
  return snapshot;
}

function withRasterIdentity(value: HostSnapshot): HostSnapshot {
  const surfaceGeneration = canvasKitSurfaceGeneration(
    rasterSessionId, value.resizeEpoch.generation);
  return {
    ...value,
    surfaceGeneration,
    gpu: {
      ...value.gpu,
      contextGeneration: rasterSessionId,
      surfaceGeneration,
    },
  };
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

function nonNegativeInteger(value: unknown, name: string): number {
  const number = Number(value);
  if (!Number.isSafeInteger(number) || number < 0)
    throw new Error(`Doroti CanvasKit '${name}' must be a non-negative safe integer.`);
  return number;
}

function exactGeneration(value: bigint, name: string): number {
  const number = Number(value);
  if (!Number.isSafeInteger(number) || number <= 0 || BigInt(number) !== value)
    throw new Error(`Doroti CanvasKit ${name} is not an exact positive JavaScript integer.`);
  return number;
}
