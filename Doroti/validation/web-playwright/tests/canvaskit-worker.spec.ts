import { test, expect } from "./helpers/fixtures.js";
import {
  assertPresenterContract,
  captureDiagnostics,
  openDoroti,
  waitForSettledPresenter,
  type CanvasKitRasterDiagnostics,
  type CanvasKitUiDiagnostics,
  type DiagnosticBundle,
} from "./helpers/doroti-diagnostics.js";

const canvasKitMode = "worker-canvaskit-webgl";

function requireCanvasKitDiagnostics(bundle: DiagnosticBundle): {
  ui: CanvasKitUiDiagnostics;
  raster: CanvasKitRasterDiagnostics;
} {
  expect(bundle.presenter.mode).toBe(canvasKitMode);
  expect(bundle.presenter.uiDiagnostics).toBeDefined();
  expect(bundle.presenter.rasterDiagnostics).toBeDefined();
  return {
    ui: bundle.presenter.uiDiagnostics!,
    raster: bundle.presenter.rasterDiagnostics!,
  };
}

test("CanvasKit split topology has exactly one owner per runtime and exact terminal accounting", async ({
  page,
  runtimeErrors,
}) => {
  let bundle = await openDoroti(page);
  test.skip(bundle.presenter.mode !== canvasKitMode, "CanvasKit split-Worker validation only");
  await expect.poll(async () => {
    bundle = await captureDiagnostics(page);
    const ui = bundle.presenter.uiDiagnostics;
    const raster = bundle.presenter.rasterDiagnostics;
    return ui?.pendingResourceOperations === 0 && ui.queueDepth === 0 && ui.buffers.outstanding === 0 &&
      raster?.queueDepth === 0 && raster.terminalScenes === raster.admittedScenes &&
      raster.rasterAttempts === raster.rasterReceipts &&
      ui.rasterReceiptCount === raster.rasterReceipts &&
      bundle.presenter.exactCommit === true;
  }, { timeout: 120_000 }).toBe(true);
  const { ui, raster } = requireCanvasKitDiagnostics(bundle);

  expect(await page.locator(".doroti-root").getAttribute("data-doroti-host")).toBe(canvasKitMode);
  expect(await page.locator("html").getAttribute("data-doroti-renderer")).toBe(canvasKitMode);
  await expect(page.locator("script[data-doroti-blazor-loader]")).toHaveCount(0);

  expect(bundle.presenter.mainManagedRuntimeCount).toBe(0);
  expect(bundle.presenter.uiManagedRuntimeCount).toBe(1);
  expect(bundle.presenter.rasterManagedRuntimeCount).toBe(0);
  expect(bundle.presenter.uiCanvasKitOwnerCount).toBe(1);
  expect(bundle.presenter.rasterCanvasKitOwnerCount).toBe(1);
  expect(bundle.presenter.rasterWebGlOwnerCount).toBe(1);
  expect(bundle.presenter.mainCanvasGetContextCount).toBe(0);
  expect(bundle.presenter.uiCanvasGetContextCount).toBe(0);
  expect(bundle.presenter.activeCanvasLeaseCount).toBe(1);
  expect(bundle.presenter.canvasLeases?.filter((lease) => lease.state !== "retired")).toEqual([
    expect.objectContaining({ state: "transferred", terminalCount: 0 }),
  ]);

  expect(ui.topologyVersion).toBe(1);
  expect(ui.canvasKitOwnerCount).toBe(1);
  expect(ui.managedRuntimeCount).toBe(1);
  expect(ui.rasterReady).toBe(true);
  expect(ui.queueDepth).toBe(0);
  expect(ui.queueHighWater).toBeLessThanOrEqual(2);
  expect(ui.pendingResourceOperations).toBe(0);
  expect(ui.buffers.capacity).toBe(4);
  expect(ui.buffers.pooled).toBeLessThanOrEqual(ui.buffers.capacity);
  expect(ui.buffers.outstanding).toBe(0);
  expect(ui.buffers.outstandingBytes).toBe(0);
  expect(ui.buffers.borrowed).toBe(ui.buffers.returned + ui.buffers.abandoned);

  expect(raster.topologyVersion).toBe(1);
  expect(raster.canvasKitOwnerCount).toBe(1);
  expect(raster.managedRuntimeCount).toBe(0);
  expect(raster.visibleCanvasContextOwnerCount).toBe(1);
  expect(raster.queueDepth).toBe(0);
  expect(raster.queueHighWater).toBeLessThanOrEqual(2);
  expect(raster.terminalScenes).toBe(raster.admittedScenes);
  expect(raster.rasterReceipts).toBe(raster.rasterAttempts);
  expect(raster.submittedScenes + raster.supersededScenes + raster.failedScenes)
    .toBe(raster.terminalScenes);
  expect(raster.failedScenes).toBe(0);
  expect(ui.rasterReceiptCount).toBe(raster.rasterReceipts);
  expect(ui.terminalCount).toBeGreaterThanOrEqual(raster.terminalScenes);
  expect(bundle.presenter.admittedSceneCount).toBe(raster.admittedScenes);
  expect(bundle.presenter.sceneTerminalCount).toBe(ui.terminalCount);
  expect(bundle.presenter.rasterAttemptCount).toBe(raster.rasterAttempts);
  expect(bundle.presenter.rasterReceiptCount).toBe(raster.rasterReceipts);
  expect(bundle.presenter.unpairedRequestCount).toBe(0);
  expect(bundle.presenter.exactCommit).toBe(true);

  const provenance = await page.evaluate(async () => {
    const url = new URL("/_content/Doroti.Host.Web/canvaskit/0.42.0/canvaskit.manifest.json", location.href);
    const response = await fetch(url, { cache: "no-cache" });
    if (!response.ok) throw new Error(`CanvasKit provenance fetch failed: ${response.status}`);
    const manifest = await response.json() as {
      schema: string;
      version: string;
      variant: string;
      lockfileIntegrity: string;
      files: { path: string; byteLength: number; sha256: string }[];
    };
    const runtimeAssetResponses = await Promise.all(["canvaskit.js", "canvaskit.wasm"].map(async (path) => {
      const assetUrl = new URL(path, url);
      const assetResponse = await fetch(assetUrl, { cache: "force-cache" });
      return { url: assetUrl.href, ok: assetResponse.ok };
    }));
    const runtimeUrls = performance.getEntriesByType("resource")
      .map((entry) => new URL(entry.name, location.href))
      .filter((entry) => entry.pathname.includes("/canvaskit/"));
    return {
      manifest,
      runtimeAssetResponses,
      runtimeUrls: runtimeUrls.map((entry) => entry.href),
      allRuntimeAssetsAreSameOrigin: runtimeAssetResponses.every((entry) =>
        entry.ok && new URL(entry.url).origin === location.origin),
    };
  });
  expect(provenance.manifest.schema).toBe("doroti.canvaskit-assets/v1");
  expect(provenance.manifest.version).toBe("0.42.0");
  expect(provenance.manifest.variant).toBe("default");
  expect(provenance.manifest.lockfileIntegrity).toMatch(/^sha512-/);
  expect(provenance.manifest.files.map((file) => file.path).sort()).toEqual([
    "LICENSE", "canvaskit.js", "canvaskit.wasm", "types/index.d.ts",
  ]);
  expect(provenance.manifest.files.every((file) =>
    file.byteLength > 0 && /^[0-9a-f]{64}$/.test(file.sha256))).toBe(true);
  expect(provenance.runtimeUrls.length).toBeGreaterThanOrEqual(2);
  expect(provenance.runtimeAssetResponses).toHaveLength(2);
  expect(provenance.allRuntimeAssetsAreSameOrigin).toBe(true);
  expect(bundle.presenter.assetVerification?.verifiedRuntimeAssetCount).toBe(2);
  expect(bundle.presenter.assetVerification?.verifiedRuntimeAssetBytes).toBeGreaterThan(0);
  expect(bundle.presenter.assetVerification?.elapsedMicroseconds).toBeGreaterThanOrEqual(0);
  expect(runtimeErrors).toEqual([]);
  assertPresenterContract(bundle);
});

test("CanvasKit Raster diagnostic stall leaves UI heartbeat and input dispatch live", async ({
  page,
  runtimeErrors,
}) => {
  const before = await openDoroti(page);
  test.skip(before.presenter.mode !== canvasKitMode, "CanvasKit split-Worker validation only");
  const { ui: beforeUi, raster: beforeRaster } = requireCanvasKitDiagnostics(before);
  const surface = await page.locator("#doroti-surface").boundingBox();
  expect(surface).not.toBeNull();

  const stalled = await page.evaluate((canvasId) => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: { stallRaster100ms(id: string): boolean };
    }).__dorotiResizeDiagnostics;
    return diagnostics?.stallRaster100ms(canvasId) ?? false;
  }, before.snapshot.canvasId);
  expect(stalled).toBe(true);
  await page.mouse.move(surface!.x + 10, surface!.y + 10);
  await page.mouse.move(surface!.x + 24, surface!.y + 18);
  await page.mouse.wheel(0, 24);

  let after = before;
  await expect.poll(async () => {
    after = await captureDiagnostics(page);
    const ui = after.presenter.uiDiagnostics;
    const raster = after.presenter.rasterDiagnostics;
    return (raster?.diagnosticRasterStallCount ?? -1) >= beforeRaster.diagnosticRasterStallCount + 1 &&
      (raster?.lastDiagnosticRasterStallMilliseconds ?? -1) >= 100 &&
      (ui?.heartbeatSequence ?? -1) >= beforeUi.heartbeatSequence + 2 &&
      (ui?.inputDispatchCount ?? -1) > beforeUi.inputDispatchCount;
  }, { timeout: 120_000 }).toBe(true);

  const { ui, raster } = requireCanvasKitDiagnostics(after);
  expect(raster.lastDiagnosticRasterStallMilliseconds).toBeGreaterThanOrEqual(100);
  expect(ui.heartbeatSequence).toBeGreaterThan(beforeUi.heartbeatSequence);
  expect(ui.inputDispatchCount).toBeGreaterThan(beforeUi.inputDispatchCount);
  expect(ui.lastInputSequence).toBeGreaterThanOrEqual(beforeUi.lastInputSequence);
  expect(after.presenter.workerRestartCount).toBe(0);
  expect(runtimeErrors).toEqual([]);
  assertPresenterContract(after);
});

test("@dpr CanvasKit keeps CSS 1080x720 and physical backing 2160x1440 without transform", async ({
  page,
  runtimeErrors,
}) => {
  const initial = await openDoroti(page);
  test.skip(initial.presenter.mode !== canvasKitMode, "CanvasKit split-Worker validation only");
  await page.setViewportSize({ width: 1080, height: 720 });
  await page.waitForFunction(() => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: { hosts(): number[]; snapshot(hostId: number): string };
    }).__dorotiResizeDiagnostics;
    if (!diagnostics) return false;
    const hostId = diagnostics.hosts()[0];
    const snapshot = JSON.parse(diagnostics.snapshot(hostId)) as {
      resizeEpoch: {
        logicalWidth: number; logicalHeight: number; physicalWidth: number;
        physicalHeight: number; devicePixelRatio: number;
      };
    };
    return snapshot.resizeEpoch.logicalWidth === 1080 && snapshot.resizeEpoch.logicalHeight === 720 &&
      snapshot.resizeEpoch.physicalWidth === 2160 && snapshot.resizeEpoch.physicalHeight === 1440 &&
      snapshot.resizeEpoch.devicePixelRatio === 2;
  }, undefined, { timeout: 120_000 });
  const bundle = await waitForSettledPresenter(page);
  const { raster } = requireCanvasKitDiagnostics(bundle);
  const geometry = await page.locator("#doroti-surface").evaluate((canvas) => {
    const element = canvas as HTMLCanvasElement;
    const rect = element.getBoundingClientRect();
    const style = getComputedStyle(element);
    return {
      cssWidth: rect.width,
      cssHeight: rect.height,
      backingWidth: element.width,
      backingHeight: element.height,
      transform: style.transform,
      transformOrigin: style.transformOrigin,
      objectFit: style.objectFit,
    };
  });

  expect(geometry.cssWidth).toBe(1080);
  expect(geometry.cssHeight).toBe(720);
  expect(geometry.backingWidth).toBe(2160);
  expect(geometry.backingHeight).toBe(1440);
  expect(geometry.transform).toBe("none");
  expect(geometry.objectFit).toBe("cover");
  expect(raster.physicalWidth).toBe(2160);
  expect(raster.physicalHeight).toBe(1440);
  expect(bundle.presenter.rasterWidth).toBe(2160);
  expect(bundle.presenter.rasterHeight).toBe(1440);
  expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
  expect(bundle.presenter.exactCommit).toBe(true);
  expect(runtimeErrors).toEqual([]);
  assertPresenterContract(bundle);
});

test("CanvasKit Raster survives three bounded replacements with exact leases and resource replay", async ({
  page,
  runtimeErrors,
}) => {
  let before = await openDoroti(page);
  test.skip(before.presenter.mode !== canvasKitMode, "CanvasKit split-Worker validation only");
  await expect.poll(async () => {
    before = await captureDiagnostics(page);
    const ui = before.presenter.uiDiagnostics;
    const raster = before.presenter.rasterDiagnostics;
    return (ui?.resourceJournalCount ?? 0) > 0 && ui?.pendingResourceOperations === 0 &&
      raster?.resourceCount === ui.resourceJournalCount;
  }, { timeout: 120_000 }).toBe(true);
  const beforeUi = before.presenter.uiDiagnostics!;
  const beforeRaster = before.presenter.rasterDiagnostics!;
  const beforeRasterSession = before.presenter.rasterSessionId!;
  const journalCount = beforeUi.resourceJournalCount;
  expect(beforeRaster.resourceCount).toBe(journalCount);

  let checkpoint = before;
  for (let restart = 1; restart <= 3; restart++) {
    const checkpointUi = checkpoint.presenter.uiDiagnostics!;
    const crashed = await page.evaluate((canvasId) => {
      const diagnostics = (globalThis as typeof globalThis & {
        __dorotiResizeDiagnostics?: { crashWorker(id: string): boolean };
      }).__dorotiResizeDiagnostics;
      return diagnostics?.crashWorker(canvasId) ?? false;
    }, checkpoint.snapshot.canvasId);
    expect(crashed).toBe(true);

    await expect.poll(async () => {
      checkpoint = await captureDiagnostics(page);
      const ui = checkpoint.presenter.uiDiagnostics;
      const raster = checkpoint.presenter.rasterDiagnostics;
      return checkpoint.presenter.workerRestartCount === restart &&
        checkpoint.presenter.rasterSessionId === beforeRasterSession + restart &&
        ui?.rasterReady === true && ui.queueDepth === 0 && ui.pendingResourceOperations === 0 &&
        ui.buffers.outstanding === 0 &&
        ui.resourceReceiptCount >= checkpointUi.resourceReceiptCount + journalCount &&
        raster?.resourceCount === journalCount;
    }, { timeout: 120_000 }).toBe(true);
    expect(checkpoint.presenter.activeCanvasLeaseCount).toBe(1);
    expect(checkpoint.presenter.canvasLeases?.filter((lease) => lease.state === "retired"))
      .toHaveLength(restart);
    expect(checkpoint.presenter.canvasLeases
      ?.filter((lease) => lease.state === "retired")
      .every((lease) => lease.terminalCount === 1)).toBe(true);
  }

  await page.setViewportSize({ width: 1210, height: 760 });
  const after = await waitForSettledPresenter(page);
  const { ui, raster } = requireCanvasKitDiagnostics(after);
  expect(after.presenter.workerRestartCount).toBe(3);
  expect(after.presenter.workerRestartBudget).toBe(3);
  expect(after.presenter.rasterSessionId).toBe(beforeRasterSession + 3);
  expect(after.presenter.activeCanvasLeaseCount).toBe(1);
  expect(after.presenter.canvasLeases?.filter((lease) => lease.state === "retired"))
    .toHaveLength(3);
  expect(after.presenter.canvasLeases?.filter((lease) => lease.state === "retired"))
    .toEqual(expect.arrayContaining([
      expect.objectContaining({ terminalCount: 1 }),
      expect.objectContaining({ terminalCount: 1 }),
      expect.objectContaining({ terminalCount: 1 }),
    ]));
  expect(after.presenter.canvasLeases?.filter((lease) => lease.state === "transferred")).toEqual([
    expect.objectContaining({ sessionId: beforeRasterSession + 3, terminalCount: 0 }),
  ]);
  expect(ui.resourceJournalCount).toBe(journalCount);
  expect(ui.resourceReceiptCount).toBeGreaterThanOrEqual(
    beforeUi.resourceReceiptCount + journalCount * 3);
  expect(ui.resourceReplayCount).toBeGreaterThanOrEqual(journalCount * 3);
  expect(ui.pendingResourceOperations).toBe(0);
  expect(ui.buffers.outstanding).toBe(0);
  expect(ui.buffers.outstandingBytes).toBe(0);
  expect(ui.buffers.borrowed).toBe(ui.buffers.returned + ui.buffers.abandoned);
  expect(raster.resourceCount).toBe(journalCount);
  expect(raster.resourceBytes).toBe(beforeRaster.resourceBytes);
  expect(["font", "image", "runtime-effect"]
    .reduce((total, kind) => total + (raster.objects[kind]?.live ?? 0), 0)).toBe(journalCount);
  expect(raster.objects.GrDirectContext?.live).toBe(1);
  expect(raster.objects.Surface?.live).toBe(1);
  expect(raster.flushCount).toBeGreaterThan(0);
  expect(raster.terminalScenes).toBe(raster.admittedScenes);
  expect(raster.rasterReceipts).toBe(raster.rasterAttempts);
  expect(after.presenter.exactCommit).toBe(true);
  expect(runtimeErrors).toEqual([]);
  assertPresenterContract(after);
});

test("CanvasKit malformed protocol is rejected and the bounded Raster recovery stays usable", async ({
  page,
  runtimeErrors,
}) => {
  const before = await openDoroti(page);
  test.skip(before.presenter.mode !== canvasKitMode, "CanvasKit split-Worker validation only");

  const localFailures = await page.evaluate(async () => {
    const protocolUrl = "/_content/Doroti.Host.Web/doroti.web.protocol.js";
    const protocol = await import(protocolUrl);
    const failures: string[] = [];
    for (const value of [
      null,
      {},
      { protocolVersion: 1, kind: "ready" },
      { protocolVersion: 2, kind: "unknown" },
    ]) {
      try { protocol.decodeDorotiMessage(value, new Set(["ready"])); }
      catch (error) { failures.push(String(error)); }
    }
    for (const bytes of [
      new Uint8Array(1),
      new Uint8Array(protocol.dorotiDisplayListHeaderSize),
      (() => {
        const value = new Uint8Array(protocol.dorotiDisplayListHeaderSize);
        const view = new DataView(value.buffer);
        view.setUint32(0, protocol.dorotiDisplayListMagic, true);
        view.setUint16(4, 99, true);
        return value;
      })(),
    ]) {
      try { protocol.validateDorotiDisplayList(bytes); }
      catch (error) { failures.push(String(error)); }
    }
    return failures;
  });
  expect(localFailures).toHaveLength(7);
  expect(localFailures.some((failure) => failure.includes("BufferTooShort"))).toBe(true);
  expect(localFailures.some((failure) => failure.includes("InvalidMagic"))).toBe(true);
  expect(localFailures.some((failure) => failure.includes("UnsupportedVersion"))).toBe(true);

  const violated = await page.evaluate((canvasId) => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: { violateWorkerProtocol(id: string): boolean };
    }).__dorotiResizeDiagnostics;
    return diagnostics?.violateWorkerProtocol(canvasId) ?? false;
  }, before.snapshot.canvasId);
  expect(violated).toBe(true);
  await expect.poll(async () => {
    const candidate = await captureDiagnostics(page);
    return candidate.presenter.workerRestartCount === 1 &&
      candidate.presenter.rasterSessionId === before.presenter.rasterSessionId! + 1 &&
      candidate.presenter.uiDiagnostics?.rasterReady === true;
  }, { timeout: 120_000 }).toBe(true);

  await page.setViewportSize({ width: 1190, height: 750 });
  const after = await waitForSettledPresenter(page);
  const { raster } = requireCanvasKitDiagnostics(after);
  expect(after.presenter.workerRestartCount).toBe(1);
  expect(raster.flushCount).toBeGreaterThan(0);
  expect(raster.failedScenes).toBe(0);
  expect(after.presenter.exactCommit).toBe(true);
  expect(runtimeErrors).toEqual([]);
  assertPresenterContract(after);
});
