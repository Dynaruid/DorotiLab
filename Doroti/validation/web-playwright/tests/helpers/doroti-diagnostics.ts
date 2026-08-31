import { expect, type Page, type TestInfo } from "@playwright/test";

export interface ResizeEpoch {
  generation: number;
  logicalWidth: number;
  logicalHeight: number;
  physicalWidth: number;
  physicalHeight: number;
  devicePixelRatio: number;
  timestampMicroseconds: number;
}

export interface TraceEntry {
  sequence: number;
  timestampMicroseconds: number;
  phase: string;
  epoch: ResizeEpoch;
  source: string;
  durationMicroseconds: number;
  rafId: number;
  backingWidth: number;
  backingHeight: number;
  surfaceWidth: number;
  surfaceHeight: number;
  terminal: string | null;
  detail: string | null;
  queueDepth: number;
  inputSequence: number;
  requestId: number;
}

export interface HostSnapshot {
  canvasId: string;
  logicalWidth: number;
  logicalHeight: number;
  devicePixelRatio: number;
  visible: boolean;
  focused: boolean;
  generation: number;
  surfaceGeneration: number;
  gpu: {
    api: string;
    vendor: string;
    renderer: string;
    hardware: boolean;
    softwareFallbackUsed: boolean;
  };
  resizeEpoch: ResizeEpoch;
}

export interface PresenterSnapshot {
  context: number;
  contextGeneration: number;
  currentRequestId: number | null;
  latestRequestId: number | null;
  queueDepth: number;
  contextLost: boolean;
  frontGeneration: number | null;
  frontRequestId: number | null;
  frontFramebufferId: number | null;
  stagingFramebufferId: number | null;
  requestedMode: string;
  mode: string;
  fallbackReason: string | null;
  rasterCanvasAttached: boolean;
  visibleContext: string;
  rasterWidth: number;
  rasterHeight: number;
  displayWidth: number;
  displayHeight: number;
  bitmapCreated: number;
  bitmapConsumed: number;
  bitmapClosed: number;
  activeBitmaps: number;
  mainManagedRuntimeCount?: number;
  workerManagedRuntimeCount?: number;
  uiManagedRuntimeCount?: number;
  rasterManagedRuntimeCount?: number;
  uiCanvasKitOwnerCount?: number;
  rasterCanvasKitOwnerCount?: number;
  rasterWebGlOwnerCount?: number;
  mainCanvasGetContextCount?: number;
  uiCanvasGetContextCount?: number;
  workerRestartCount?: number;
  workerRestartBudget?: number;
  unpairedRequestCount?: number;
  runtimeSessionId?: number;
  rasterSessionId?: number;
  admittedSceneCount?: number;
  sceneTerminalCount?: number;
  rasterAttemptCount?: number;
  rasterReceiptCount?: number;
  resourceCount?: number;
  resourceBytes?: number;
  activeCanvasLeaseCount?: number;
  exactCommit?: boolean;
  assetVerification?: {
    verifiedRuntimeAssetCount: number;
    verifiedRuntimeAssetBytes: number;
    elapsedMicroseconds: number;
  };
  canvasLeases?: CanvasLeaseSnapshot[];
  uiDiagnostics?: CanvasKitUiDiagnostics;
  rasterDiagnostics?: CanvasKitRasterDiagnostics;
}

export interface CanvasLeaseSnapshot {
  leaseId: number;
  canvasId: string;
  sessionId: number;
  state: "created" | "transferred" | "retired";
  terminalCount: number;
}

export interface TransferBufferPoolSnapshot {
  capacity: number;
  pooled: number;
  pooledBytes: number;
  created: number;
  borrowed: number;
  returned: number;
  abandoned: number;
  discarded: number;
  outstanding: number;
  outstandingBytes: number;
}

export interface CanvasKitUiDiagnostics {
  topologyVersion: number;
  canvasKitOwnerCount: number;
  managedRuntimeCount: number;
  rasterSessionId: number;
  rasterReady: boolean;
  currentScene: number | null;
  latestScene: number | null;
  queueDepth: number;
  queueHighWater: number;
  terminalCount: number;
  rasterReceiptCount: number;
  resourceReceiptCount: number;
  resourceReplayCount: number;
  resourceJournalCount: number;
  pendingResourceOperations: number;
  text: { ready?: boolean; fontCount?: number; [name: string]: unknown };
  buffers: TransferBufferPoolSnapshot;
  heartbeatSequence: number;
  inputDispatchCount: number;
  lastInputSequence: number;
}

export interface CanvasKitObjectCounter {
  created: number;
  deleted: number;
  live: number;
  bytes: number;
}

export interface CanvasKitRasterDiagnostics {
  topologyVersion: number;
  canvasKitOwnerCount: number;
  managedRuntimeCount: number;
  visibleCanvasContextOwnerCount: number;
  currentScene: number | null;
  latestScene: number | null;
  queueDepth: number;
  queueHighWater: number;
  admittedScenes: number;
  terminalScenes: number;
  rasterAttempts: number;
  rasterReceipts: number;
  submittedScenes: number;
  supersededScenes: number;
  failedScenes: number;
  lastFailedSceneSequence?: number;
  lastFailureReason?: string;
  flushCount: number;
  lastFrontGeneration: number;
  resourceCount: number;
  resourceBytes: number;
  objects: Record<string, CanvasKitObjectCounter>;
  referenceCompatibility: {
    addSuperellipseNoOps: number;
    appliedBlurCropBounds: number;
  };
  physicalWidth: number;
  physicalHeight: number;
  diagnosticRasterStallCount: number;
  lastDiagnosticRasterStallMilliseconds: number;
}

export interface DiagnosticBundle {
  hostId: number;
  snapshot: HostSnapshot;
  presenter: PresenterSnapshot;
  trace: TraceEntry[];
}

export function frontCommitGeneration(entry: TraceEntry): number {
  if (entry.detail) {
    try {
      const generation = Number((JSON.parse(entry.detail) as { generation?: unknown }).generation);
      if (Number.isSafeInteger(generation) && generation > 0) return generation;
    } catch {
      // Older/non-direct trace entries do not have to carry structured detail.
    }
  }
  return entry.epoch.generation;
}

type BrowserDiagnostics = {
  hosts(): number[];
  trace(hostId: number): string;
  reset(hostId: number): void;
  snapshot(hostId: number): string;
  presenter(canvasId: string): string;
  capability(canvasId: string): string;
  loseContext(canvasId: string): boolean;
  restoreContext(canvasId: string): boolean;
};

function readJson<T>(value: string): T {
  return JSON.parse(value) as T;
}

export async function openDoroti(page: Page): Promise<DiagnosticBundle> {
  const rendererMode = process.env.DOROTI_WEB_RENDERER_MODE;
  const rendererQuery = rendererMode && rendererMode !== "auto"
    ? `&dorotiRenderer=${encodeURIComponent(rendererMode)}` : "";
  await page.goto(`/?dorotiResizeDiagnostics=1${rendererQuery}`, { waitUntil: "domcontentloaded" });
  await expect(page.locator(".doroti-root")).toBeVisible({ timeout: 120_000 });
  await expect(page.locator("#doroti-surface")).toBeVisible({ timeout: 120_000 });
  await page.waitForFunction(() => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: BrowserDiagnostics;
    }).__dorotiResizeDiagnostics;
    return (diagnostics?.hosts().length ?? 0) > 0;
  }, undefined, { timeout: 120_000 });
  return captureSettledPresenter(page, 120_000);
}

export async function captureDiagnostics(page: Page): Promise<DiagnosticBundle> {
  return page.evaluate(() => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: BrowserDiagnostics;
    }).__dorotiResizeDiagnostics;
    if (!diagnostics) throw new Error("Doroti diagnostics are unavailable.");
    const hostId = diagnostics.hosts()[0];
    if (!hostId) throw new Error("Doroti host is unavailable.");
    const snapshot = JSON.parse(diagnostics.snapshot(hostId)) as HostSnapshot;
    return {
      hostId,
      snapshot,
      presenter: JSON.parse(diagnostics.presenter(snapshot.canvasId)) as PresenterSnapshot,
      trace: JSON.parse(diagnostics.trace(hostId)) as TraceEntry[],
    };
  });
}

export async function resetDiagnostics(page: Page): Promise<void> {
  await page.evaluate(() => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: BrowserDiagnostics;
    }).__dorotiResizeDiagnostics;
    if (!diagnostics) throw new Error("Doroti diagnostics are unavailable.");
    const hostId = diagnostics.hosts()[0];
    if (!hostId) throw new Error("Doroti host is unavailable.");
    diagnostics.reset(hostId);
  });
}

export async function waitForSettledPresenter(page: Page): Promise<DiagnosticBundle> {
  return captureSettledPresenter(page, 60_000);
}

async function captureSettledPresenter(page: Page, timeout: number): Promise<DiagnosticBundle> {
  let settled: DiagnosticBundle | null = null;
  await expect.poll(async () => {
    const candidate = await captureDiagnostics(page);
    const isSettled = candidate.presenter.queueDepth === 0 &&
      !candidate.presenter.contextLost &&
      (candidate.presenter.unpairedRequestCount ?? 0) === 0 &&
      candidate.presenter.frontGeneration === candidate.snapshot.resizeEpoch.generation;
    if (isSettled) settled = candidate;
    return isSettled;
  }, { timeout }).toBe(true);
  if (!settled) throw new Error("Doroti presenter did not produce a settled diagnostic snapshot.");
  return settled;
}

export async function attachDiagnostics(
  page: Page,
  testInfo: TestInfo,
  name = "doroti-diagnostics",
): Promise<DiagnosticBundle | null> {
  try {
    const bundle = await captureDiagnostics(page);
    await testInfo.attach(name, {
      body: Buffer.from(`${JSON.stringify(bundle, null, 2)}\n`),
      contentType: "application/json",
    });
    return bundle;
  } catch {
    return null;
  }
}

export function assertPresenterContract(bundle: DiagnosticBundle): void {
  expect(bundle.snapshot.gpu.api).toBe("webgl2");
  expect(bundle.snapshot.gpu.hardware).toBe(true);
  expect(bundle.snapshot.gpu.softwareFallbackUsed).toBe(false);
  expect(bundle.presenter.contextLost).toBe(false);
  expect(bundle.presenter.queueDepth).toBeLessThanOrEqual(2);
  expect(bundle.presenter.unpairedRequestCount ?? 0).toBe(0);
  expect(bundle.trace.filter((entry) => entry.terminal === "failed")).toEqual([]);

  const requested = new Set(bundle.trace
    .filter((entry) => entry.phase === "present-requested" && entry.requestId > 0)
    .map((entry) => entry.requestId));
  const terminals = new Map<number, TraceEntry[]>();
  for (const entry of bundle.trace.filter((candidate) => candidate.terminal && candidate.requestId > 0)) {
    const values = terminals.get(entry.requestId) ?? [];
    values.push(entry);
    terminals.set(entry.requestId, values);
  }
  for (const requestId of requested) {
    const evidence = bundle.trace.filter((entry) => entry.requestId === requestId);
    expect(terminals.get(requestId)?.length,
      `request ${requestId} terminal count; evidence=${JSON.stringify(evidence)}`).toBe(1);
  }

  if (bundle.presenter.mode === "worker-direct-webgl") {
    let priorGeneration = 0;
    let priorRequestId = 0;
    for (const entry of bundle.trace.filter((candidate) =>
      candidate.phase === "front-commit" && candidate.source === "worker-direct-surface")) {
      const detail = JSON.parse(entry.detail ?? "{}") as {
        generation?: number;
        targetGeneration?: number;
        progressive?: boolean;
        admitted?: boolean;
      };
      const generation = frontCommitGeneration(entry);
      expect(detail.admitted, `direct request ${entry.requestId} admission`).toBe(true);
      expect(generation, `direct request ${entry.requestId} target generation`)
        .toBeLessThanOrEqual(detail.targetGeneration ?? 0);
      expect(generation, `direct request ${entry.requestId} main epoch`)
        .toBeLessThanOrEqual(entry.epoch.generation);
      expect(detail.progressive, `direct request ${entry.requestId} progressive marker`)
        .toBe(generation < (detail.targetGeneration ?? 0));
      expect(generation, `direct request ${entry.requestId} monotonic generation`)
        .toBeGreaterThanOrEqual(priorGeneration);
      expect(entry.requestId, `direct request ${entry.requestId} monotonic request`)
        .toBeGreaterThan(priorRequestId);
      priorGeneration = generation;
      priorRequestId = entry.requestId;
    }
  }
}

export function percentile(values: number[], fraction: number): number {
  if (values.length === 0) return Number.POSITIVE_INFINITY;
  const ordered = [...values].sort((left, right) => left - right);
  return ordered[Math.min(ordered.length - 1, Math.ceil(ordered.length * fraction) - 1)];
}

export async function setDiagnosticContextState(
  page: Page,
  state: "lose" | "restore",
): Promise<boolean> {
  return page.evaluate((requestedState) => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: BrowserDiagnostics;
    }).__dorotiResizeDiagnostics;
    if (!diagnostics) throw new Error("Doroti diagnostics are unavailable.");
    const hostId = diagnostics.hosts()[0];
    const snapshot = JSON.parse(diagnostics.snapshot(hostId)) as HostSnapshot;
    return requestedState === "lose"
      ? diagnostics.loseContext(snapshot.canvasId)
      : diagnostics.restoreContext(snapshot.canvasId);
  }, state);
}
