import { createDorotiDomEndpoints, createReplacementCanvas } from "./doroti.web.dom.js";
import { CanvasKitStageTrace } from "./doroti.canvaskit.trace.js";
import { createWorkerVisibleSurface } from "./doroti.web.surface.js";
import {
  captureHostSnapshot,
  clearTextInput,
  closeHost,
  configureManagedCallbacks,
  createHost,
  invokePlugin,
  readClipboardText,
  recordExternalWorkerTrace,
  registerExternalWorkerPresenter,
  requestFocus,
  restoreHostInputSequence,
  setApplicationTitle,
  setCaretRect,
  setContextMenuEnabled,
  setCursor,
  setEditableSizeAndTransform,
  setTextInputStyle,
  setTextInputState,
  unregisterExternalWorkerPresenter,
  updateTextInputConfiguration,
  updateExternalWorkerGpu,
  updateSemantics,
  writeClipboardText,
  type ExternalWorkerPresenterDiagnostics,
} from "./doroti.web.js";
import {
  decodeDorotiMessage,
  dorotiCanvasKitTopologyVersion,
  dorotiProtocolVersion,
} from "./doroti.web.protocol.js";
import { CanvasLeaseLedger, createDorotiClassicWorker } from "./doroti.web.worker-host.js";

interface CanvasKitAssetManifest {
  readonly schema: "doroti.canvaskit-assets/v1";
  readonly version: "0.42.0";
  readonly variant: "default";
  readonly logicalBasePath: string;
  readonly canvasKitJsPath: string;
  readonly canvasKitWasmPath: string;
  readonly lockfileIntegrity: string;
  readonly files: readonly { path: string; byteLength: number; sha256: string }[];
}

interface CanvasKitAssetVerification {
  readonly verifiedRuntimeAssetCount: number;
  readonly verifiedRuntimeAssetBytes: number;
  readonly elapsedMicroseconds: number;
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
  generation: number;
  surfaceGeneration: number;
  inputSequence: number;
  gpu: Readonly<Record<string, unknown>>;
  resizeEpoch: ResizeEpoch;
  readonly [name: string]: unknown;
}

interface PresenterState {
  canvas: HTMLCanvasElement;
  uiWorker: Worker;
  rasterWorker: Worker;
  uiSessionId: number;
  rasterSessionId: number;
  canvasLeaseId: number;
  restartCount: number;
  contextGeneration: number;
  surfaceGeneration: number;
  frontGeneration: number;
  frontRequestId: number;
  contextLost: boolean;
  uiReady: boolean;
  rasterReady: boolean;
  runtimeReady: boolean;
  uiDiagnostics: Readonly<Record<string, unknown>>;
  rasterDiagnostics: Readonly<Record<string, unknown>>;
}

interface PendingUiMessage {
  readonly envelope: Readonly<Record<string, unknown>>;
}

const protocolVersion = dorotiProtocolVersion;
const topologyVersion = dorotiCanvasKitTopologyVersion;
const canvasKitVersion = "0.42.0";
const rasterRestartBudget = 3;
const manifestLogicalUrl = `_content/Doroti.Host.Web/canvaskit/${canvasKitVersion}/canvaskit.manifest.json`;
const mainInboundKinds = new Set([
  "bootstrap-ready", "ui-canvaskit-ready", "gpu-ready", "runtime-ready", "ui-heartbeat",
  "ui-diagnostics", "raster-diagnostics", "direct-commit", "managed-work", "control", "control-request",
  "closed", "context-lost", "raster-fatal", "disposed", "fatal", "stage-trace",
]);

export async function startDorotiCanvasKitWorkerHost(): Promise<"started"> {
  requireCapabilities();
  const { manifest, verification: assetVerification } = await loadCanvasKitManifest();
  const baseUrl = new URL(`./${manifest.logicalBasePath}`, document.baseURI);
  // URL objects are accepted by Worker/fetch but are not structured-cloneable.
  // Freeze all bootstrap-envelope URLs as canonical strings at the main boundary.
  const canvasKitJsUrl = requireSameOrigin(
    new URL(manifest.canvasKitJsPath, baseUrl), "CanvasKit JS").href;
  const canvasKitWasmUrl = requireSameOrigin(
    new URL(manifest.canvasKitWasmPath, baseUrl), "CanvasKit WASM").href;
  const bootstrapUrl = requireSameOrigin(new URL("./doroti.canvaskit.bootstrap.js", import.meta.url), "classic bootstrap");
  const uiRoleUrl = resolveStaticAssetModuleUrl(
    "./_content/Doroti.Host.Web/doroti.ui.worker.js", "UI role");
  const rasterRoleUrl = resolveStaticAssetModuleUrl(
    "./_content/Doroti.Host.Web/doroti.canvaskit.worker.js", "Raster role");
  const dotnetModuleUrl = resolveCurrentDotnetModuleUrl();

  const app = document.getElementById("app");
  if (!app) throw new Error("Doroti CanvasKit bootstrap could not find '#app'.");
  const endpoints = createDorotiDomEndpoints(app);
  const root = endpoints.root;
  let canvas = endpoints.canvas;
  root.dataset.dorotiHost = "worker-canvaskit-webgl";
  const leaseLedger = new CanvasLeaseLedger();
  let activeInputSequence = 0;
  let currentSnapshot: HostSnapshot | null = null;
  const pendingUiMessages: PendingUiMessage[] = [];
  let disposed = false;
  let restarting = false;
  let diagnosticContextLossPending = false;
  let resolveReady!: (value: "started") => void;
  let rejectReady!: (reason: unknown) => void;
  const readyPromise = new Promise<"started">((resolve, reject) => {
    resolveReady = resolve;
    rejectReady = reject;
  });

  let presenter!: PresenterState;
  const stageTrace = new CanvasKitStageTrace();
  stageTrace.enabled = new URL(location.href).searchParams.get("dorotiCanvasKitTrace") === "1";
  let collectionId = 0;
  const collectors = new Map<number, (role: string, trace: unknown) => void>();
  if (stageTrace.enabled) {
    Object.assign(globalThis, {
      __dorotiCanvasKitExperiment: {
        async collect() {
          const id = ++collectionId;
          return new Promise<Record<string, unknown>>((resolve, reject) => {
            const result: Record<string, unknown> = { main: stageTrace.snapshot() };
            const timeout = setTimeout(() => {
              collectors.delete(id);
              reject(new Error("CanvasKit stage collection timed out"));
            }, 5000);
            collectors.set(id, (role, trace) => {
              result[role] = trace;
              if (result.ui && result.raster) {
                clearTimeout(timeout);
                collectors.delete(id);
                resolve(result);
              }
            });
            presenter.uiWorker.postMessage({ protocolVersion, topologyVersion, kind: "collect-stage-trace", collectionId: id });
            presenter.rasterWorker.postMessage({ protocolVersion, topologyVersion, kind: "collect-stage-trace", collectionId: id, rasterSessionId: presenter.rasterSessionId });
          });
        },
      },
    });
  }
  const diagnostics: ExternalWorkerPresenterDiagnostics = {
    commitCanvasCssWithFront: true,
    snapshot: () => presenterSnapshot(
      presenter, leaseLedger, currentSnapshot, assetVerification),
    command(action) {
      if (disposed) return false;
      if (action === "crash") {
        presenter.rasterWorker.postMessage({ protocolVersion, topologyVersion, kind: "crash" });
        return true;
      }
      if (action === "violate-protocol") {
        presenter.rasterWorker.postMessage({ protocolVersion: 999, topologyVersion, kind: "context" });
        return true;
      }
      if (action === "stall-raster-100ms") {
        presenter.rasterWorker.postMessage({ protocolVersion, topologyVersion, kind: action });
        return true;
      }
      if (action === "lose-context") {
        diagnosticContextLossPending = true;
        presenter.contextLost = true;
        presenter.rasterWorker.postMessage({
          protocolVersion, topologyVersion, kind: "context", action: "lose",
        });
        return true;
      }
      if (action === "restore-context" && diagnosticContextLossPending) {
        diagnosticContextLossPending = false;
        void restartRaster("diagnostic CanvasKit context restore requested");
        return true;
      }
      presenter.rasterWorker.postMessage({
        protocolVersion, topologyVersion, kind: "context", action: "restore",
      });
      return true;
    },
  };

  let uiWorker = createDorotiClassicWorker(bootstrapUrl);
  let rasterWorker = createDorotiClassicWorker(bootstrapUrl);
  presenter = {
    canvas, uiWorker, rasterWorker,
    uiSessionId: 1, rasterSessionId: 1, canvasLeaseId: 0, restartCount: 0,
    contextGeneration: 0, surfaceGeneration: 0, frontGeneration: 0, frontRequestId: 0,
    contextLost: false, uiReady: false, rasterReady: false, runtimeReady: false,
    uiDiagnostics: {}, rasterDiagnostics: {},
  };
  registerExternalWorkerPresenter(canvas.id, diagnostics);

  const postUi = (kind: string, payload: Record<string, unknown> = {}, transfer: Transferable[] = []): void => {
    const envelope = { protocolVersion, topologyVersion, kind, ...payload };
    if (!presenter.uiReady) {
      if (transfer.length > 0)
        throw new Error("Doroti CanvasKit cannot queue a transferable before the UI role is ready.");
      pendingUiMessages.push({ envelope: structuredClone(envelope) });
      return;
    }
    presenter.uiWorker.postMessage(envelope, transfer);
  };
  const flushPendingUiMessages = (): void => {
    for (const message of pendingUiMessages.splice(0))
      presenter.uiWorker.postMessage(message.envelope);
  };
  const postInput = (
    inputKind: string,
    hostId: number,
    inputSequence: number,
    payload: Record<string, unknown>,
  ): void => {
    activeInputSequence = inputSequence;
    postUi("input", { inputKind, hostId, inputSequence, payload });
  };

  configureManagedCallbacks({
    dispatchAnimationFrame: () => {
      throw new Error("Doroti CanvasKit main supervisor cannot receive managed frame callbacks.");
    },
    dispatchSnapshot(hostId, snapshotJson) {
      currentSnapshot = JSON.parse(snapshotJson) as HostSnapshot;
      postUi("snapshot", { hostId, snapshot: currentSnapshot });
    },
    dispatchResizeEpoch(
      hostId, hostGeneration, generation,
      logicalWidth, logicalHeight, physicalWidth, physicalHeight,
      devicePixelRatio, timestampMicroseconds,
    ) {
      const resizeEpoch = {
        generation, logicalWidth, logicalHeight, physicalWidth, physicalHeight,
        devicePixelRatio, timestampMicroseconds,
      };
      if (currentSnapshot) currentSnapshot = {
        ...currentSnapshot,
        generation: Math.max(currentSnapshot.generation, hostGeneration),
        resizeEpoch,
      };
      // The UI Worker can spend tens of milliseconds inside one managed WASM
      // frame and cannot service its message queue during that call. Give the
      // independent Raster Worker the browser's newest immutable target
      // directly; the ordered UI -> Raster copy below remains the authority
      // required before a matching DisplayList is submitted.
      if (presenter.rasterReady && !restarting && !disposed) {
        presenter.rasterWorker.postMessage({
          protocolVersion,
          topologyVersion,
          rasterSessionId: presenter.rasterSessionId,
          kind: "resize-target-fast-lane",
          resizeEpoch,
        });
      }
      postUi("resize-epoch", { hostId, hostGeneration, resizeEpoch });
      stageTrace.record("main-resize-observed", resizeEpoch.generation, 0, { ...resizeEpoch });
    },
    dispatchPointerBatch: (hostId, phase, kind, pointerId, buttons, modifiers, inputSequence, samples) =>
      postInput("pointer", hostId, inputSequence, { phase, kind, pointerId, buttons, modifiers, samples }),
    dispatchWheel: (hostId, x, y, deltaX, deltaY, timestamp, kind, inputSequence) =>
      postInput("wheel", hostId, inputSequence, { x, y, deltaX, deltaY, timestamp, kind }),
    dispatchKey: (hostId, pressed, repeat, synthesized, code, key, timestamp, inputSequence) =>
      postInput("key", hostId, inputSequence, { pressed, repeat, synthesized, code, key, timestamp }),
    dispatchFocus: (hostId, focused, timestamp, inputSequence) =>
      postInput("focus", hostId, inputSequence, { focused, timestamp }),
    dispatchTextEditing: (
      hostId, text, selectionBase, selectionExtent, composingBase, composingExtent, inputSequence,
    ) => postInput("text", hostId, inputSequence, {
      text, selectionBase, selectionExtent, composingBase, composingExtent,
    }),
    dispatchTextAction: (hostId, action, inputSequence) =>
      postInput("text-action", hostId, inputSequence, { action }),
    dispatchTextConnectionClosed: (hostId, inputSequence) =>
      postInput("text-closed", hostId, inputSequence, {}),
    dispatchSemanticsAction: (hostId, nodeId, action, inputSequence, argumentsJson) =>
      postInput("semantics-action", hostId, inputSequence, { nodeId, action, argumentsJson }),
  });

  const initialRect = root.getBoundingClientRect();
  currentSnapshot = JSON.parse(createHost(
    1, canvas.id, Math.max(1, initialRect.width), Math.max(1, initialRect.height))) as HostSnapshot;
  configureCanvasBeforeTransfer(canvas, currentSnapshot.resizeEpoch);
  document.documentElement.dataset.dorotiRenderer = "worker-canvaskit-webgl";

  const handleControl = async (message: Record<string, unknown>): Promise<void> => {
    const kind = String(message.controlKind);
    const payload = (message.payload ?? {}) as Record<string, unknown>;
    switch (kind) {
      case "cursor": setCursor(Number(payload.hostId), String(payload.cursor)); break;
      case "focus-request": requestFocus(Number(payload.hostId), Boolean(payload.focused)); break;
      case "text-state":
        setTextInputState(
          Number(payload.hostId), String(payload.text), Number(payload.selectionBase), Number(payload.selectionExtent),
          String(payload.inputMode), String(payload.enterKeyHint), Boolean(payload.readOnly), Boolean(payload.obscureText),
          String(payload.autocapitalize), Boolean(payload.autocorrect), Number(payload.inputAction),
          Boolean(payload.multiline), Boolean(payload.attach), Boolean(payload.enableInteractiveSelection));
        break;
      case "text-config":
        updateTextInputConfiguration(
          Number(payload.hostId), String(payload.inputMode), String(payload.enterKeyHint),
          Boolean(payload.readOnly), Boolean(payload.obscureText), String(payload.autocapitalize),
          Boolean(payload.autocorrect), Number(payload.inputAction), Boolean(payload.multiline),
          Boolean(payload.enableInteractiveSelection));
        break;
      case "text-style":
        setTextInputStyle(Number(payload.hostId), String(payload.styleJson));
        break;
      case "caret":
        setCaretRect(Number(payload.hostId), Number(payload.left), Number(payload.top), Number(payload.width), Number(payload.height));
        break;
      case "editable-geometry":
        setEditableSizeAndTransform(
          Number(payload.hostId), Number(payload.width), Number(payload.height), String(payload.transformJson));
        break;
      case "context-menu": setContextMenuEnabled(Number(payload.hostId), Boolean(payload.enabled)); break;
      case "text-clear": clearTextInput(Number(payload.hostId)); break;
      case "semantics": updateSemantics(Number(payload.hostId), String(payload.json)); break;
      case "application-title": setApplicationTitle(Number(payload.hostId), String(payload.title)); break;
      default: throw new Error(`Unknown Doroti CanvasKit UI control '${kind}'.`);
    }
  };

  const sendControlResponse = (correlationId: number, result: string, error?: unknown): void =>
    postUi("control-response", {
      correlationId, result, error: error ? String(error) : undefined,
    });

  const attachUiWorker = (worker: Worker): void => {
    worker.addEventListener("message", (event) => {
      let message: Record<string, unknown>;
      try {
        message = decodeDorotiMessage(event.data, mainInboundKinds);
      } catch (error) {
        fail(new Error(`Doroti CanvasKit UI control protocol violation: ${String(error)}`));
        return;
      }
      switch (message.kind) {
        case "stage-trace": collectors.get(Number(message.collectionId))?.("ui", message.trace); break;
        case "ui-canvaskit-ready":
          presenter.uiReady = true;
          flushPendingUiMessages();
          break;
        case "runtime-ready": presenter.runtimeReady = true; settleReady(); break;
        case "ui-diagnostics": presenter.uiDiagnostics = message.diagnostics as Record<string, unknown>; break;
        case "ui-heartbeat":
          presenter.uiDiagnostics = {
            ...presenter.uiDiagnostics,
            heartbeatSequence: Number(message.sequence),
            inputDispatchCount: Number(message.inputDispatchCount),
            lastInputSequence: Number(message.lastInputSequence),
          };
          break;
        case "raster-diagnostics": presenter.rasterDiagnostics = message.diagnostics as Record<string, unknown>; break;
        case "managed-work":
          recordExternalWorkerTrace(1, String(message.phase), "canvaskit-ui-managed", message);
          break;
        case "control":
          void handleControl(message).catch((error) => fail(error));
          break;
        case "control-request": {
          const correlationId = Number(message.correlationId);
          const kind = String(message.controlKind);
          const payload = (message.payload ?? {}) as Record<string, unknown>;
          void (async () => {
            if (kind === "clipboard-read") return readClipboardText();
            if (kind === "clipboard-write") return writeClipboardText(String(payload.text));
            if (kind === "plugin") return invokePlugin(
              String(payload.moduleUrl), String(payload.exportName), String(payload.channel),
              String(payload.codec), String(payload.payloadBase64));
            throw new Error(`Unknown Doroti CanvasKit UI request '${kind}'.`);
          })().then((result) => sendControlResponse(correlationId, result),
            (error) => sendControlResponse(correlationId, "", error));
          break;
        }
        case "raster-fatal":
          if (Number(message.rasterSessionId) === presenter.rasterSessionId) {
            const reason = String(message.error);
            if (!(diagnosticContextLossPending && /context lost|context restore/i.test(reason)))
              void restartRaster(reason);
          }
          break;
        case "fatal": fail(new Error(`Doroti CanvasKit UI Worker failed: ${String(message.error)}`)); break;
      }
    });
    worker.addEventListener("error", (event) => fail(event.error ?? new Error(event.message)));
  };

  const attachRasterWorker = (worker: Worker): void => {
    const workerSessionId = presenter.rasterSessionId;
    worker.addEventListener("message", (event) => {
      if (worker !== presenter.rasterWorker || workerSessionId !== presenter.rasterSessionId) return;
      let message: Record<string, unknown>;
      try {
        message = decodeDorotiMessage(event.data, mainInboundKinds);
      } catch (error) {
        void restartRaster(`Raster control protocol violation: ${String(error)}`);
        return;
      }
      if (Number(message.rasterSessionId ?? presenter.rasterSessionId) !== presenter.rasterSessionId) return;
      switch (message.kind) {
        case "stage-trace": collectors.get(Number(message.collectionId))?.("raster", message.trace); break;
        case "gpu-ready": {
          presenter.rasterReady = true;
          presenter.contextLost = false;
          presenter.contextGeneration = Number(message.contextGeneration);
          presenter.surfaceGeneration = Number(message.surfaceGeneration);
          updateExternalWorkerGpu(1, {
            ...(message.gpu as {
              api: "webgl2"; vendor: string; renderer: string; hardware: true; softwareFallbackUsed: boolean;
            }),
            contextGeneration: presenter.contextGeneration,
            surfaceGeneration: presenter.surfaceGeneration,
          });
          currentSnapshot = JSON.parse(captureHostSnapshot(1)) as HostSnapshot;
          settleReady();
          break;
        }
        case "raster-diagnostics": presenter.rasterDiagnostics = message.diagnostics as Record<string, unknown>; break;
        case "direct-commit": {
          const generation = Number(message.generation);
          const logicalWidth = Number(message.logicalWidth);
          const logicalHeight = Number(message.logicalHeight);
          const physicalWidth = Number(message.physicalWidth);
          const physicalHeight = Number(message.physicalHeight);
          const devicePixelRatio = Number(message.devicePixelRatio);
          const capacityWidth = Number(message.capacityWidth);
          const capacityHeight = Number(message.capacityHeight);
          const targetGeneration = currentSnapshot?.resizeEpoch.generation ?? generation;
          if (!Number.isSafeInteger(generation) || generation <= 0 ||
              generation < presenter.frontGeneration || generation > targetGeneration ||
              !Number.isFinite(logicalWidth) || logicalWidth <= 0 ||
              !Number.isFinite(logicalHeight) || logicalHeight <= 0 ||
              !Number.isSafeInteger(physicalWidth) || physicalWidth <= 0 ||
              !Number.isSafeInteger(physicalHeight) || physicalHeight <= 0 ||
              !Number.isFinite(devicePixelRatio) || devicePixelRatio <= 0 ||
              !Number.isSafeInteger(capacityWidth) || capacityWidth < physicalWidth ||
              !Number.isSafeInteger(capacityHeight) || capacityHeight < physicalHeight ||
              physicalWidth !== Math.round(logicalWidth * devicePixelRatio) ||
              physicalHeight !== Math.round(logicalHeight * devicePixelRatio)) {
            void restartRaster("CanvasKit Raster emitted an invalid or non-monotonic direct commit");
            break;
          }
          commitCanvasKitFrontGeometry(
            canvas, logicalWidth, logicalHeight, physicalWidth, physicalHeight,
            capacityWidth, capacityHeight, devicePixelRatio);
          presenter.frontGeneration = generation;
          stageTrace.record("main-commit-received", generation, Number(message.requestId), {
            logicalWidth, logicalHeight, commitTime: message.commitEpochMilliseconds,
          });
          presenter.frontRequestId = Number(message.requestId);
          presenter.contextGeneration = Number(message.contextGeneration);
          presenter.surfaceGeneration = Number(message.surfaceGeneration);
          updateExternalWorkerGpu(1, {
            ...(currentSnapshot?.gpu as {
              api: "webgl2"; vendor: string; renderer: string; hardware: true; softwareFallbackUsed: boolean;
            }),
            contextGeneration: presenter.contextGeneration,
            surfaceGeneration: presenter.surfaceGeneration,
          });
          currentSnapshot = JSON.parse(captureHostSnapshot(1)) as HostSnapshot;
          recordExternalWorkerTrace(1, "front-commit", "worker-canvaskit-surface", {
            ...message,
            targetGeneration,
            progressive: generation < targetGeneration,
            terminal: "submitted",
            backingWidth: capacityWidth,
            backingHeight: capacityHeight,
            surfaceWidth: message.physicalWidth,
            surfaceHeight: message.physicalHeight,
          });
          break;
        }
        case "context-lost": presenter.contextLost = true; break;
        case "fatal": {
          const reason = String(message.error);
          if (!(diagnosticContextLossPending && /context lost|context restore/i.test(reason)))
            void restartRaster(reason);
          break;
        }
      }
    });
    worker.addEventListener("error", (event) => {
      if (worker !== presenter.rasterWorker || workerSessionId !== presenter.rasterSessionId) return;
      void restartRaster(String(event.message));
    });
  };

  const settleReady = (): void => {
    if (presenter.rasterReady && presenter.runtimeReady) {
      root.dataset.dorotiWorkerRuntime = "ready";
      resolveReady("started");
    }
  };

  const fail = (reason: unknown): void => {
    if (disposed) return;
    disposed = true;
    root.dataset.dorotiWorkerRuntime = "failed";
    presenter.uiWorker.terminate();
    presenter.rasterWorker.terminate();
    const activeLease = leaseLedger.snapshot().find(
      (lease) => lease.leaseId === presenter.canvasLeaseId && lease.state !== "retired");
    if (activeLease) leaseLedger.retire(activeLease.leaseId);
    closeHost(1);
    unregisterExternalWorkerPresenter(canvas.id);
    leaseLedger.assertClosed();
    if (!presenter.runtimeReady) rejectReady(reason);
    else console.error("Doroti CanvasKit runtime failed closed.", reason);
  };

  const startRaster = (
    worker: Worker,
    channelPort: MessagePort,
    offscreen: OffscreenCanvas,
    leaseId: number,
  ): void => {
    attachRasterWorker(worker);
    worker.postMessage({
      protocolVersion,
      topologyVersion,
      kind: "canvaskit-bootstrap-init",
      role: "raster",
      sessionId: presenter.uiSessionId,
      rasterSessionId: presenter.rasterSessionId,
      canvasKitJsUrl,
      canvasKitWasmUrl,
      roleModuleUrl: rasterRoleUrl,
      stageTrace: stageTrace.enabled,
      resizeEpoch: currentSnapshot!.resizeEpoch,
      canvas: offscreen,
      rasterPort: channelPort,
    }, [offscreen, channelPort]);
    leaseLedger.transferred(leaseId);
  };

  const restartRaster = async (reason: string): Promise<void> => {
    if (disposed || restarting) return;
    if (presenter.restartCount >= rasterRestartBudget) {
      fail(new Error(`Doroti CanvasKit Raster restart budget exhausted: ${reason}`));
      return;
    }
    restarting = true;
    try {
      presenter.restartCount++;
      presenter.rasterSessionId++;
      diagnosticContextLossPending = false;
      presenter.rasterReady = false;
      presenter.contextLost = false;
      presenter.contextGeneration = 0;
      presenter.surfaceGeneration = 0;
      presenter.frontGeneration = 0;
      presenter.frontRequestId = 0;
      presenter.rasterDiagnostics = {};
      presenter.rasterWorker.terminate();
      leaseLedger.retire(presenter.canvasLeaseId);
      const priorCanvas = canvas;
      closeHost(1);
      unregisterExternalWorkerPresenter(priorCanvas.id);
      canvas = createReplacementCanvas(priorCanvas);
      presenter.canvas = canvas;
      registerExternalWorkerPresenter(canvas.id, diagnostics);
      const rect = root.getBoundingClientRect();
      currentSnapshot = JSON.parse(createHost(
        1, canvas.id, Math.max(1, rect.width), Math.max(1, rect.height))) as HostSnapshot;
      restoreHostInputSequence(1, activeInputSequence);
      currentSnapshot = JSON.parse(captureHostSnapshot(1)) as HostSnapshot;
      configureCanvasBeforeTransfer(canvas, currentSnapshot.resizeEpoch);
      const nextChannel = new MessageChannel();
      postUi("raster-port-rebind", {
        rasterSessionId: presenter.rasterSessionId,
        rasterPort: nextChannel.port1,
      }, [nextChannel.port1]);
      postUi("snapshot", { hostId: 1, snapshot: currentSnapshot });
      const nextWorker = createDorotiClassicWorker(bootstrapUrl);
      presenter.rasterWorker = nextWorker;
      const leaseId = leaseLedger.create(canvas.id, presenter.rasterSessionId);
      presenter.canvasLeaseId = leaseId;
      const offscreen = createWorkerVisibleSurface(canvas, true).offscreen;
      if (!offscreen) throw new Error("Doroti CanvasKit replacement canvas transfer failed.");
      startRaster(nextWorker, nextChannel.port2, offscreen, leaseId);
      recordExternalWorkerTrace(1, "worker-restart", "canvaskit-supervisor", {
        reason,
        restartCount: presenter.restartCount,
        restartBudget: rasterRestartBudget,
      });
    } catch (error) {
      fail(error);
    } finally {
      restarting = false;
    }
  };

  attachUiWorker(uiWorker);
  const initialChannel = new MessageChannel();
  uiWorker.postMessage({
    protocolVersion,
    topologyVersion,
    kind: "canvaskit-bootstrap-init",
    role: "ui",
    stageTrace: stageTrace.enabled,
    resizeScheduling: new URL(location.href).searchParams.get("dorotiResizeScheduling") === "display" ? "display" : "baseline-30fps",
    sessionId: presenter.uiSessionId,
    rasterSessionId: presenter.rasterSessionId,
    canvasKitJsUrl,
    canvasKitWasmUrl,
    roleModuleUrl: uiRoleUrl,
    dotnetModuleUrl,
    snapshot: currentSnapshot,
    rasterPort: initialChannel.port1,
  }, [initialChannel.port1]);
  const initialLeaseId = leaseLedger.create(canvas.id, presenter.rasterSessionId);
  presenter.canvasLeaseId = initialLeaseId;
  const offscreen = createWorkerVisibleSurface(canvas, true).offscreen;
  if (!offscreen) throw new Error("Doroti CanvasKit visible canvas transfer failed.");
  startRaster(rasterWorker, initialChannel.port2, offscreen, initialLeaseId);

  globalThis.addEventListener("pagehide", () => {
    if (disposed) return;
    disposed = true;
    presenter.uiWorker.postMessage({ protocolVersion, topologyVersion, kind: "dispose" });
    presenter.rasterWorker.postMessage({ protocolVersion, topologyVersion, kind: "dispose" });
    leaseLedger.retire(presenter.canvasLeaseId);
    closeHost(1);
    unregisterExternalWorkerPresenter(canvas.id);
    leaseLedger.assertClosed();
  }, { once: true });
  return readyPromise;
}

function presenterSnapshot(
  presenter: PresenterState,
  leases: CanvasLeaseLedger,
  snapshot: HostSnapshot | null,
  assetVerification: CanvasKitAssetVerification,
): Readonly<Record<string, unknown>> {
  const raster = presenter.rasterDiagnostics;
  const ui = presenter.uiDiagnostics;
  const rasterQueue = Number(raster.queueDepth ?? 0);
  const uiQueue = Number(ui.queueDepth ?? 0);
  return {
    context: 0,
    requestedMode: "worker-canvaskit-webgl",
    mode: "worker-canvaskit-webgl",
    fallbackReason: null,
    contextGeneration: presenter.contextGeneration,
    surfaceGeneration: presenter.surfaceGeneration,
    currentRequestId: raster.currentScene ?? null,
    latestRequestId: raster.latestScene ?? null,
    queueDepth: Math.max(rasterQueue, uiQueue),
    uiQueueDepth: uiQueue,
    rasterQueueDepth: rasterQueue,
    uiQueueHighWater: Number(ui.queueHighWater ?? 0),
    rasterQueueHighWater: Number(raster.queueHighWater ?? 0),
    contextLost: presenter.contextLost,
    frontGeneration: presenter.frontGeneration || null,
    frontRequestId: presenter.frontRequestId || null,
    frontFramebufferId: null,
    stagingFramebufferId: null,
    rasterCanvasAttached: false,
    visibleContext: "transferred-offscreen-webgl2-canvaskit",
    rasterWidth: Number(raster.physicalWidth ?? 0),
    rasterHeight: Number(raster.physicalHeight ?? 0),
    displayWidth: Number(raster.capacityWidth ?? raster.physicalWidth ?? 0),
    displayHeight: Number(raster.capacityHeight ?? raster.physicalHeight ?? 0),
    mainManagedRuntimeCount: 0,
    uiManagedRuntimeCount: Number(ui.managedRuntimeCount ?? 0),
    rasterManagedRuntimeCount: 0,
    uiCanvasKitOwnerCount: Number(ui.canvasKitOwnerCount ?? 0),
    rasterCanvasKitOwnerCount: Number(raster.canvasKitOwnerCount ?? 0),
    rasterWebGlOwnerCount: Number(raster.visibleCanvasContextOwnerCount ?? 0),
    mainCanvasGetContextCount: 0,
    uiCanvasGetContextCount: 0,
    bitmapCreated: 0,
    bitmapConsumed: 0,
    bitmapClosed: 0,
    activeBitmaps: 0,
    workerRestartCount: presenter.restartCount,
    workerRestartBudget: rasterRestartBudget,
    runtimeSessionId: presenter.uiSessionId,
    rasterSessionId: presenter.rasterSessionId,
    admittedSceneCount: Number(ui.admittedCount ?? 0),
    sceneTerminalCount: Number(ui.terminalCount ?? 0),
    rasterAttemptCount: Number(raster.rasterAttempts ?? 0),
    rasterReceiptCount: Number(raster.rasterReceipts ?? 0),
    unpairedRequestCount: Math.max(0,
      Number(ui.admittedCount ?? 0) - Number(ui.terminalCount ?? 0)),
    resourceReplayCount: Number(ui.resourceReplayCount ?? 0),
    transferBufferOutstanding: Number(
      (ui.buffers as Record<string, unknown> | undefined)?.outstanding ?? 0),
    transferBufferAbandoned: Number(
      (ui.buffers as Record<string, unknown> | undefined)?.abandoned ?? 0),
    resourceCount: Number(raster.resourceCount ?? 0),
    resourceBytes: Number(raster.resourceBytes ?? 0),
    canvasLeases: leases.snapshot(),
    activeCanvasLeaseCount: leases.activeCount(),
    assetVerification,
    exactCommit: presenter.frontGeneration > 0 &&
      presenter.frontGeneration === Number(snapshot?.resizeEpoch.generation ?? 0),
    uiDiagnostics: ui,
    rasterDiagnostics: raster,
  };
}

async function loadCanvasKitManifest(): Promise<{
  manifest: CanvasKitAssetManifest;
  verification: CanvasKitAssetVerification;
}> {
  const started = performance.now();
  const url = requireSameOrigin(new URL(manifestLogicalUrl, document.baseURI), "CanvasKit manifest");
  const response = await fetch(url, { credentials: "same-origin", cache: "no-cache" });
  if (!response.ok) throw new Error(`DOROTIWEB032: CanvasKit manifest fetch failed (${response.status}).`);
  const manifest = await response.json() as CanvasKitAssetManifest;
  if (manifest.schema !== "doroti.canvaskit-assets/v1" || manifest.version !== canvasKitVersion ||
      manifest.variant !== "default" || !manifest.lockfileIntegrity.startsWith("sha512-") ||
      manifest.logicalBasePath !== `_content/Doroti.Host.Web/canvaskit/${canvasKitVersion}/` ||
      manifest.canvasKitJsPath !== "canvaskit.js" || manifest.canvasKitWasmPath !== "canvaskit.wasm")
    throw new Error("DOROTIWEB033: CanvasKit manifest version/variant/runtime contract mismatch.");
  const files = new Map(manifest.files.map((file) => [file.path, file]));
  for (const path of ["canvaskit.js", "canvaskit.wasm", "types/index.d.ts", "LICENSE"]) {
    const file = files.get(path);
    if (!file || !Number.isSafeInteger(file.byteLength) || file.byteLength <= 0 ||
        !/^[0-9a-f]{64}$/.test(file.sha256))
      throw new Error(`DOROTIWEB034: CanvasKit manifest is missing verified '${path}'.`);
  }
  let verifiedRuntimeAssetBytes = 0;
  const baseUrl = new URL(`./${manifest.logicalBasePath}`, document.baseURI);
  for (const path of [manifest.canvasKitJsPath, manifest.canvasKitWasmPath]) {
    const expected = files.get(path)!;
    const assetUrl = requireSameOrigin(new URL(path, baseUrl), `CanvasKit runtime asset '${path}'`);
    const assetResponse = await fetch(assetUrl, {
      credentials: "same-origin",
      cache: "no-cache",
    });
    if (!assetResponse.ok)
      throw new Error(`DOROTIWEB034: CanvasKit runtime asset '${path}' fetch failed (${assetResponse.status}).`);
    const bytes = await assetResponse.arrayBuffer();
    if (bytes.byteLength !== expected.byteLength)
      throw new Error(
        `DOROTIWEB034: CanvasKit runtime asset '${path}' length mismatch ` +
        `(manifest=${expected.byteLength}, actual=${bytes.byteLength}).`);
    const digest = await crypto.subtle.digest("SHA-256", bytes);
    const actualSha256 = [...new Uint8Array(digest)]
      .map((value) => value.toString(16).padStart(2, "0")).join("");
    if (actualSha256 !== expected.sha256)
      throw new Error(`DOROTIWEB034: CanvasKit runtime asset '${path}' SHA-256 mismatch.`);
    verifiedRuntimeAssetBytes += bytes.byteLength;
  }
  return {
    manifest,
    verification: {
      verifiedRuntimeAssetCount: 2,
      verifiedRuntimeAssetBytes,
      elapsedMicroseconds: Math.round((performance.now() - started) * 1000),
    },
  };
}

function configureCanvasBeforeTransfer(canvas: HTMLCanvasElement, epoch: ResizeEpoch): void {
  const capacity = initialCanvasKitCapacity(epoch);
  canvas.width = capacity.width;
  canvas.height = capacity.height;
  commitCanvasKitFrontGeometry(
    canvas, epoch.logicalWidth, epoch.logicalHeight,
    epoch.physicalWidth, epoch.physicalHeight,
    capacity.width, capacity.height, epoch.devicePixelRatio);
}

function initialCanvasKitCapacity(epoch: ResizeEpoch): { readonly width: number; readonly height: number } {
  const screenWidth = Number(globalThis.screen?.availWidth ?? globalThis.screen?.width ?? 0);
  const screenHeight = Number(globalThis.screen?.availHeight ?? globalThis.screen?.height ?? 0);
  return {
    width: Math.ceil(Math.max(epoch.logicalWidth * 1.5, screenWidth, epoch.logicalWidth) *
      epoch.devicePixelRatio),
    height: Math.ceil(Math.max(epoch.logicalHeight * 1.5, screenHeight, epoch.logicalHeight) *
      epoch.devicePixelRatio),
  };
}

function commitCanvasKitFrontGeometry(
  canvas: HTMLCanvasElement,
  logicalWidth: number,
  logicalHeight: number,
  physicalWidth: number,
  physicalHeight: number,
  capacityWidth: number,
  capacityHeight: number,
  devicePixelRatio: number,
): void {
  // Keep one backing pixel mapped to one device pixel while the DOM root clips
  // this grow-only capacity to the live viewport. Raster can then present an
  // immutable target without resizing/recreating the on-screen GL surface on
  // every ResizeObserver generation.
  canvas.style.width = "auto";
  canvas.style.height = "auto";
  canvas.style.setProperty("zoom", String(1 / devicePixelRatio));
  canvas.style.objectFit = "cover";
  canvas.style.objectPosition = "left top";
  canvas.style.removeProperty("transform");
  canvas.style.removeProperty("transform-origin");
  canvas.dataset.dorotiFrontLogicalWidth = String(logicalWidth);
  canvas.dataset.dorotiFrontLogicalHeight = String(logicalHeight);
  canvas.dataset.dorotiFrontPhysicalWidth = String(physicalWidth);
  canvas.dataset.dorotiFrontPhysicalHeight = String(physicalHeight);
  canvas.dataset.dorotiCapacityWidth = String(capacityWidth);
  canvas.dataset.dorotiCapacityHeight = String(capacityHeight);
  delete canvas.dataset.dorotiResizePreview;
}

function requireCapabilities(): void {
  if (typeof Worker === "undefined" || typeof MessageChannel === "undefined" ||
      typeof OffscreenCanvas === "undefined" ||
      typeof HTMLCanvasElement.prototype.transferControlToOffscreen !== "function" ||
      typeof crypto?.subtle?.digest !== "function")
    throw new Error("Doroti worker-canvaskit-webgl requires Worker, MessageChannel, and transferred OffscreenCanvas.");
}

function requireSameOrigin(url: URL, label: string): URL {
  if (url.origin !== globalThis.location.origin || (url.protocol !== "http:" && url.protocol !== "https:"))
    throw new Error(`Doroti ${label} must resolve to same-origin HTTP(S).`);
  return url;
}

function resolveCurrentDotnetModuleUrl(): string {
  const stableUrl = new URL("./_framework/dotnet.js", document.baseURI).href;
  for (const script of document.querySelectorAll<HTMLScriptElement>('script[type="importmap"]')) {
    try {
      const imports = (JSON.parse(script.textContent ?? "{}") as { imports?: Record<string, string> }).imports;
      const mapped = imports?.["./_framework/dotnet.js"] ?? imports?.["/_framework/dotnet.js"];
      if (mapped) return new URL(mapped, document.baseURI).href;
    } catch {
      // The stable development alias remains valid when an unrelated import map is malformed.
    }
  }
  return stableUrl;
}

function resolveStaticAssetModuleUrl(logicalSpecifier: string, label: string): string {
  const logicalUrl = requireSameOrigin(new URL(logicalSpecifier, document.baseURI), label);
  for (const script of document.querySelectorAll<HTMLScriptElement>('script[type="importmap"]')) {
    try {
      const imports = (JSON.parse(script.textContent ?? "{}") as {
        imports?: Record<string, string>;
      }).imports;
      const mapped = imports?.[logicalSpecifier];
      if (!mapped) continue;
      const mappedUrl = requireSameOrigin(new URL(mapped, document.baseURI), `${label} fingerprint`);
      if (mappedUrl.search || mappedUrl.hash || !isSdkFingerprintOf(logicalUrl.pathname, mappedUrl.pathname))
        throw new Error(`Doroti ${label} import-map endpoint is not its approved SDK fingerprint.`);
      return mappedUrl.href;
    } catch (error) {
      if (error instanceof SyntaxError) continue;
      throw error;
    }
  }
  return logicalUrl.href;
}

function isSdkFingerprintOf(logicalPath: string, candidatePath: string): boolean {
  if (candidatePath === logicalPath) return true;
  const dot = logicalPath.lastIndexOf(".");
  if (dot < 0) return false;
  return candidatePath.startsWith(`${logicalPath.slice(0, dot)}.`) &&
    candidatePath.endsWith(logicalPath.slice(dot)) &&
    /^[a-z0-9]{10}$/.test(candidatePath.slice(logicalPath.slice(0, dot).length + 1, -logicalPath.slice(dot).length));
}
