import { PNG } from "pngjs";
import { execFile } from "node:child_process";
import { writeFile } from "node:fs/promises";
import { promisify } from "node:util";
import { resolve } from "node:path";
import type { TestInfo } from "@playwright/test";
import { test, expect } from "./helpers/fixtures.js";
import {
  assertPresenterContract,
  openDoroti,
  percentile,
  resetDiagnostics,
  waitForSettledPresenter,
  type DiagnosticBundle,
  type TraceEntry,
} from "./helpers/doroti-diagnostics.js";

interface PixelOracle {
  blackRightBand: number;
  blackBottomBand: number;
  rootRightBand: number;
  rootBottomBand: number;
  distinctSampledColors: number;
  gridSpacingX: number | null;
  gridSpacingY: number | null;
  gridAspectDelta: number | null;
  cornerMarkerAspect: number | null;
}

interface LiveResizeSample {
  boundIndex: number;
  animationFrame: number;
  requestedWidth: number;
  requestedHeight: number;
  logicalWidth: number;
  logicalHeight: number;
  canvasConnected: boolean;
  rootConnected: boolean;
  transform: string;
  preview: string | null;
  scaleX: number;
  scaleY: number;
  skewX: number;
  skewY: number;
  pixel: PixelOracle;
}

const execFileAsync = promisify(execFile);

async function attachJson(testInfo: TestInfo, name: string, value: unknown): Promise<void> {
  const path = testInfo.outputPath(`${name}.json`);
  await writeFile(path, `${JSON.stringify(value, null, 2)}\n`, "utf8");
  await testInfo.attach(name, { path, contentType: "application/json" });
}

function colorAt(image: PNG, x: number, y: number): [number, number, number, number] {
  const offset = (y * image.width + x) * 4;
  return [image.data[offset], image.data[offset + 1], image.data[offset + 2], image.data[offset + 3]];
}

function contiguousEdgeBand(
  image: PNG,
  edge: "right" | "bottom",
  matches: (red: number, green: number, blue: number, alpha: number) => boolean,
  requiredFraction: number,
): number {
  const length = edge === "right" ? image.width : image.height;
  const crossLength = edge === "right" ? image.height : image.width;
  let band = 0;
  for (let offset = 0; offset < length; offset++) {
    let matched = 0;
    for (let cross = 0; cross < crossLength; cross++) {
      const x = edge === "right" ? image.width - 1 - offset : cross;
      const y = edge === "right" ? cross : image.height - 1 - offset;
      if (matches(...colorAt(image, x, y))) matched++;
    }
    if (matched / crossLength < requiredFraction) break;
    band++;
  }
  return band;
}

function groupedCenters(values: number[]): number[] {
  const groups: number[][] = [];
  for (const value of values) {
    const current = groups.at(-1);
    if (!current || value > current.at(-1)! + 1) groups.push([value]);
    else current.push(value);
  }
  return groups.map((group) => group.reduce((sum, value) => sum + value, 0) / group.length);
}

function median(values: number[]): number | null {
  if (values.length === 0) return null;
  const ordered = [...values].sort((left, right) => left - right);
  return ordered[Math.floor(ordered.length / 2)];
}

function gridSpacing(image: PNG, axis: "x" | "y"): number | null {
  const length = axis === "x" ? image.width : image.height;
  const crossLength = axis === "x" ? image.height : image.width;
  const candidates: number[] = [];
  const isGrid = (red: number, green: number, blue: number, alpha: number): boolean =>
    alpha > 240 && blue >= 220 && green >= 210 && blue - red >= 25 && green - red >= 20;
  for (let position = 0; position < length; position++) {
    let matched = 0;
    for (let cross = 0; cross < crossLength; cross += 8) {
      const x = axis === "x" ? position : cross;
      const y = axis === "x" ? cross : position;
      if (isGrid(...colorAt(image, x, y))) matched++;
    }
    if (matched / Math.ceil(crossLength / 8) >= 0.18) candidates.push(position);
  }
  const centers = groupedCenters(candidates);
  const deltas = centers.slice(1).map((value, index) => value - centers[index])
    .filter((value) => value >= 12 && value <= 160);
  return median(deltas);
}

function cornerMarkerAspect(image: PNG): number | null {
  const points: Array<[number, number]> = [];
  const maxX = Math.min(image.width, 80);
  const maxY = Math.min(image.height, 80);
  for (let y = 0; y < maxY; y++) {
    for (let x = 0; x < maxX; x++) {
      const [red, green, blue, alpha] = colorAt(image, x, y);
      if (alpha > 240 && red >= 220 && green <= 80 && blue <= 130) points.push([x, y]);
    }
  }
  if (points.length < 8) return null;
  const xs = points.map(([x]) => x);
  const ys = points.map(([, y]) => y);
  const width = Math.max(...xs) - Math.min(...xs) + 1;
  const height = Math.max(...ys) - Math.min(...ys) + 1;
  return width / height;
}

function inspectPixels(buffer: Buffer, rootRgb: [number, number, number]): PixelOracle {
  const image = PNG.sync.read(buffer);
  const black = (red: number, green: number, blue: number, alpha: number): boolean =>
    alpha === 255 && red <= 2 && green <= 2 && blue <= 2;
  const root = (red: number, green: number, blue: number, alpha: number): boolean =>
    alpha === 255 && Math.abs(red - rootRgb[0]) <= 1 &&
    Math.abs(green - rootRgb[1]) <= 1 && Math.abs(blue - rootRgb[2]) <= 1;
  const colors = new Set<string>();
  for (let y = 0; y < image.height; y += 16) {
    for (let x = 0; x < image.width; x += 16) {
      const [red, green, blue, alpha] = colorAt(image, x, y);
      if (alpha > 0) colors.add(`${red},${green},${blue}`);
    }
  }
  const spacingX = gridSpacing(image, "x");
  const spacingY = gridSpacing(image, "y");
  return {
    blackRightBand: contiguousEdgeBand(image, "right", black, 0.98),
    blackBottomBand: contiguousEdgeBand(image, "bottom", black, 0.98),
    rootRightBand: contiguousEdgeBand(image, "right", root, 0.999),
    rootBottomBand: contiguousEdgeBand(image, "bottom", root, 0.999),
    distinctSampledColors: colors.size,
    gridSpacingX: spacingX,
    gridSpacingY: spacingY,
    gridAspectDelta: spacingX !== null && spacingY !== null
      ? Math.abs(spacingX / spacingY - 1) : null,
    cornerMarkerAspect: cornerMarkerAspect(image),
  };
}

function parseRgb(value: string): [number, number, number] {
  const channels = value.match(/[\d.]+/g)?.slice(0, 3).map(Number);
  if (!channels || channels.length !== 3) throw new Error(`Unsupported CSS color '${value}'.`);
  return [channels[0], channels[1], channels[2]];
}

function latencies(trace: TraceEntry[], fromPhase: string, toPhases: string[]): number[] {
  const result: number[] = [];
  for (const from of trace.filter((entry) => entry.phase === fromPhase)) {
    const to = trace.find((entry) => entry.sequence > from.sequence &&
      entry.epoch.generation === from.epoch.generation && toPhases.includes(entry.phase));
    if (to) result.push((to.timestampMicroseconds - from.timestampMicroseconds) / 1000);
  }
  return result;
}

function distribution(values: number[]): { samples: number; p50: number | null; p95: number | null; max: number | null } {
  return {
    samples: values.length,
    p50: values.length > 0 ? percentile(values, 0.5) : null,
    p95: values.length > 0 ? percentile(values, 0.95) : null,
    max: values.length > 0 ? Math.max(...values) : null,
  };
}

function workerAdvancedBeforePriorTerminal(trace: TraceEntry[]): boolean {
  return trace.filter((entry) => entry.phase === "present-requested" && entry.requestId > 0)
    .some((request) => {
      const nextSnapshot = trace.find((entry) => entry.sequence > request.sequence &&
        entry.phase === "worker-snapshot-sent" &&
        entry.epoch.generation > request.epoch.generation);
      return nextSnapshot !== undefined && trace.some((entry) =>
        entry.sequence > nextSnapshot.sequence && entry.requestId === request.requestId && entry.terminal !== null);
    });
}

function liveResizeReport(bundle: DiagnosticBundle, samples: LiveResizeSample[]) {
  const trace = bundle.trace;
  const targetToManaged = latencies(trace, "target-observed", [
    "managed-snapshot-dispatched", "worker-snapshot-sent",
  ]);
  const targetToExact = latencies(trace, "target-observed", ["front-commit"]);
  const commitTimes = trace.filter((entry) => entry.phase === "front-commit")
    .map((entry) => entry.timestampMicroseconds / 1000);
  const commitCadence = commitTimes.slice(1).map((value, index) => value - commitTimes[index]);
  const previews = trace.filter((entry) => entry.phase === "resize-preview-commit");
  const terminalEntries = trace.filter((entry) => entry.terminal !== null);
  return {
    schemaVersion: "doroti.live-resize/v1",
    renderer: bundle.presenter.mode,
    sampleCount: samples.length,
    targetToManagedMilliseconds: distribution(targetToManaged),
    targetToExactFrontMilliseconds: distribution(targetToExact),
    exactFrontCadenceMilliseconds: distribution(commitCadence),
    preview: { count: previews.length },
    visual: {
      maximumScaleDelta: Math.max(...samples.map((sample) => Math.abs(sample.scaleX - sample.scaleY))),
      maximumGridAspectDelta: Math.max(0, ...samples.map((sample) => sample.pixel.gridAspectDelta ?? 0)),
      maximumBlackRightBand: Math.max(...samples.map((sample) => sample.pixel.blackRightBand)),
      maximumBlackBottomBand: Math.max(...samples.map((sample) => sample.pixel.blackBottomBand)),
      maximumRootRightBand: Math.max(...samples.map((sample) => sample.pixel.rootRightBand)),
      maximumRootBottomBand: Math.max(...samples.map((sample) => sample.pixel.rootBottomBand)),
      rootBandPaints: samples.filter((sample) =>
        sample.pixel.rootRightBand > 0 || sample.pixel.rootBottomBand > 0).length,
    },
    presenter: {
      presentRequested: trace.filter((entry) => entry.phase === "present-requested").length,
      terminalCount: terminalEntries.length,
      failed: terminalEntries.filter((entry) => entry.terminal === "failed").length,
      maxQueueDepth: Math.max(0, ...trace.map((entry) => entry.queueDepth)),
      bitmapCreated: bundle.presenter.bitmapCreated,
      bitmapConsumed: bundle.presenter.bitmapConsumed,
      bitmapClosed: bundle.presenter.bitmapClosed,
      activeBitmaps: bundle.presenter.activeBitmaps,
      unpairedRequestCount: bundle.presenter.unpairedRequestCount ?? 0,
    },
    workerMailbox: {
      queued: trace.filter((entry) => entry.phase === "worker-snapshot-queued").length,
      sent: trace.filter((entry) => entry.phase === "worker-snapshot-sent").length,
      applied: trace.filter((entry) => entry.phase === "worker-snapshot-applied").length,
      advancedBeforePriorTerminal: workerAdvancedBeforePriorTerminal(trace),
    },
    samples,
  };
}

test("viewport A-B-C resize commits exact fronts without retained previews", async ({ page, runtimeErrors }) => {
  await openDoroti(page);
  await resetDiagnostics(page);
  const sizes = [
    { width: 960, height: 640 },
    { width: 1180, height: 760 },
    { width: 840, height: 700 },
    { width: 1240, height: 820 },
  ];
  for (const size of sizes) {
    await page.setViewportSize(size);
    await page.waitForTimeout(16);
  }
  const bundle = await waitForSettledPresenter(page);
  const final = sizes.at(-1)!;
  expect(runtimeErrors).toEqual([]);
  expect(bundle.snapshot.logicalWidth).toBe(final.width);
  expect(bundle.snapshot.logicalHeight).toBe(final.height);
  expect(bundle.snapshot.resizeEpoch.physicalWidth).toBe(Math.round(final.width * bundle.snapshot.devicePixelRatio));
  expect(bundle.snapshot.resizeEpoch.physicalHeight).toBe(Math.round(final.height * bundle.snapshot.devicePixelRatio));
  expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
  const finalCommit = bundle.trace.filter((entry) => entry.phase === "front-commit").at(-1);
  expect(finalCommit?.backingWidth).toBe(bundle.snapshot.resizeEpoch.physicalWidth);
  expect(finalCommit?.backingHeight).toBe(bundle.snapshot.resizeEpoch.physicalHeight);
  expect(bundle.trace.filter((entry) => entry.phase === "resize-preview-commit")).toEqual([]);
  expect(bundle.trace.filter((entry) => entry.phase === "preview-front-refresh")).toEqual([]);
  assertPresenterContract(bundle);
});

test("@dpr DPR 2 keeps logical, physical, and front generations coherent", async ({ page, runtimeErrors }) => {
  await openDoroti(page);
  await resetDiagnostics(page);
  await page.setViewportSize({ width: 1080, height: 720 });
  await page.waitForFunction(() => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: { hosts(): number[]; snapshot(hostId: number): string };
    }).__dorotiResizeDiagnostics;
    if (!diagnostics) return false;
    const hostId = diagnostics.hosts()[0];
    const snapshot = JSON.parse(diagnostics.snapshot(hostId)) as {
      resizeEpoch: { logicalWidth: number; logicalHeight: number; devicePixelRatio: number };
    };
    return snapshot.resizeEpoch.logicalWidth === 1080 &&
      snapshot.resizeEpoch.logicalHeight === 720 &&
      snapshot.resizeEpoch.devicePixelRatio === 2;
  });
  const bundle = await waitForSettledPresenter(page);
  expect(runtimeErrors).toEqual([]);
  expect(bundle.snapshot.devicePixelRatio).toBe(2);
  expect(bundle.snapshot.resizeEpoch.logicalWidth).toBe(1080);
  expect(bundle.snapshot.resizeEpoch.logicalHeight).toBe(720);
  expect(bundle.snapshot.resizeEpoch.physicalWidth).toBe(2160);
  expect(bundle.snapshot.resizeEpoch.physicalHeight).toBe(1440);
  expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
  assertPresenterContract(bundle);
});

test("@headed Desktop Chrome live bounds expose only exact, unscaled fronts", async ({ page, context, runtimeErrors }, testInfo) => {
  await openDoroti(page);
  await resetDiagnostics(page);
  const session = await context.newCDPSession(page);
  const { windowId } = await session.send("Browser.getWindowForTarget");
  const keyBounds = [
    { width: 940, height: 680 },
    { width: 1380, height: 820 },
    { width: 1260, height: 700 },
    { width: 860, height: 900 },
    { width: 1280, height: 840 },
  ];
  const bounds = Array.from({ length: 42 }, (_, index) => {
    const start = keyBounds[Math.floor(index / 10)];
    const end = keyBounds[Math.min(keyBounds.length - 1, Math.floor(index / 10) + 1)];
    const fraction = (index % 10) / 10;
    return {
      width: Math.round(start.width + (end.width - start.width) * fraction),
      height: Math.round(start.height + (end.height - start.height) * fraction),
    };
  });
  const rootRgb = parseRgb(await page.locator(".doroti-root").evaluate((root) =>
    getComputedStyle(root).backgroundColor));
  const samples: LiveResizeSample[] = [];
  for (const [boundIndex, size] of bounds.entries()) {
    await session.send("Browser.setWindowBounds", { windowId, bounds: size });
    for (let animationFrame = 0; animationFrame < 3; animationFrame++) {
      if (animationFrame > 0) await page.evaluate(() => new Promise(requestAnimationFrame));
      const geometry = await page.evaluate(() => {
        const root = document.querySelector<HTMLElement>(".doroti-root");
        const canvas = document.querySelector<HTMLCanvasElement>("#doroti-surface");
        if (!root || !canvas) throw new Error("Doroti live-resize surface is disconnected.");
        const transform = getComputedStyle(canvas).transform;
        const matrix = transform === "none" ? null : new DOMMatrixReadOnly(transform);
        return {
          logicalWidth: root.getBoundingClientRect().width,
          logicalHeight: root.getBoundingClientRect().height,
          canvasConnected: canvas.isConnected,
          rootConnected: root.isConnected,
          transform,
          preview: canvas.dataset.dorotiResizePreview ?? null,
          scaleX: matrix?.a ?? 1,
          scaleY: matrix?.d ?? 1,
          skewX: matrix?.b ?? 0,
          skewY: matrix?.c ?? 0,
        };
      });
      const screenshot = await page.screenshot();
      const pixel = inspectPixels(screenshot, rootRgb);
      samples.push({ boundIndex, animationFrame, requestedWidth: size.width, requestedHeight: size.height,
        ...geometry, pixel });
      await testInfo.attach(`live-resize-${String(boundIndex).padStart(2, "0")}-raf-${animationFrame}`, {
        body: screenshot,
        contentType: "image/png",
      });
    }
  }
  const bundle = await waitForSettledPresenter(page);
  const report = liveResizeReport(bundle, samples);
  await attachJson(testInfo, "live-resize-report", report);
  await attachJson(testInfo, "live-resize-diagnostics", bundle);

  expect(runtimeErrors).toEqual([]);
  expect(samples).toHaveLength(bounds.length * 3);
  expect(samples.every((sample) => sample.canvasConnected && sample.rootConnected)).toBe(true);
  expect(samples.every((sample) => sample.pixel.distinctSampledColors >= 8)).toBe(true);
  expect(samples.every((sample) => sample.transform === "none" && sample.preview === null)).toBe(true);
  expect(samples.every((sample) => Math.abs(sample.scaleX - sample.scaleY) <= 0.0001 &&
    Math.abs(sample.skewX) <= 0.0001 && Math.abs(sample.skewY) <= 0.0001)).toBe(true);
  expect(samples.every((sample) => sample.pixel.blackRightBand === 0 &&
    sample.pixel.blackBottomBand === 0)).toBe(true);
  expect(samples.every((sample) => sample.pixel.gridAspectDelta === null ||
    sample.pixel.gridAspectDelta <= 0.08)).toBe(true);
  expect(samples.every((sample) => sample.pixel.cornerMarkerAspect === null ||
    Math.abs(sample.pixel.cornerMarkerAspect - 22 / 15) <= 0.25)).toBe(true);
  expect(bundle.trace.filter((entry) => entry.phase === "resize-preview-commit")).toEqual([]);
  expect(bundle.trace.filter((entry) => entry.phase === "preview-front-refresh")).toEqual([]);
  expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
  expect(bundle.presenter.queueDepth).toBe(0);
  expect(bundle.presenter.activeBitmaps).toBe(0);
  if (bundle.presenter.mode === "offscreen-worker") {
    expect(bundle.presenter.bitmapCreated).toBe(bundle.presenter.bitmapConsumed + bundle.presenter.bitmapClosed);
    expect(report.workerMailbox.sent).toBeGreaterThan(0);
    expect(report.workerMailbox.applied).toBeGreaterThan(0);
  }
  assertPresenterContract(bundle);
});

test("@headed Windows native edge resize keeps metrics independent from presentation", async ({ page, context, runtimeErrors }, testInfo) => {
  test.skip(process.platform !== "win32", "Native HWND resize validation is Windows-only.");
  await openDoroti(page);
  await resetDiagnostics(page);
  const titleToken = `doroti-native-resize-${Date.now()}-${testInfo.workerIndex}`;
  await page.evaluate((title) => { document.title = title; }, titleToken);
  await expect(page).toHaveTitle(titleToken);
  const session = await context.newCDPSession(page);
  const { windowId, bounds: initial } = await session.send("Browser.getWindowForTarget");
  const left = initial.left ?? 100;
  const top = initial.top ?? 100;
  const width = initial.width ?? 1100;
  const height = initial.height ?? 760;
  const sequence: Array<{ x: number; y: number; width: number; height: number }> = [];
  for (let step = 0; step < 36; step++)
    sequence.push({ x: left, y: top, width: width + step * 8, height });
  for (let step = 0; step < 36; step++) {
    const delta = step * 6;
    sequence.push({ x: left - delta, y: top, width: width + delta, height });
  }
  for (let step = 0; step < 36; step++)
    sequence.push({ x: left, y: top, width, height: height + step * 5 });
  for (let step = 0; step < 36; step++) {
    const delta = step * 4;
    sequence.push({ x: left, y: top - delta, width, height: height + delta });
  }
  for (let step = 35; step >= 0; step--) {
    const delta = step * 5;
    sequence.push({ x: left - delta, y: top - delta, width: width + delta, height: height + delta });
  }
  // End at a size that differs from the initial bounds. This makes a stale
  // accessibility geometry tree observable after resize quiesces.
  sequence.push({ x: left, y: top, width: width + 64, height: height + 48 });

  await page.evaluate(() => {
    const scope = globalThis as typeof globalThis & { __dorotiNativeResizeSamples?: Array<Record<string, unknown>> };
    scope.__dorotiNativeResizeSamples = [];
    const deadline = performance.now() + 4000;
    const sample = (): void => {
      const root = document.querySelector<HTMLElement>(".doroti-root");
      const canvas = document.querySelector<HTMLCanvasElement>("#doroti-surface");
      if (root && canvas) {
        scope.__dorotiNativeResizeSamples!.push({
          timestamp: performance.now(),
          rootWidth: root.getBoundingClientRect().width,
          rootHeight: root.getBoundingClientRect().height,
          transform: getComputedStyle(canvas).transform,
          preview: canvas.dataset.dorotiResizePreview ?? null,
          canvasConnected: canvas.isConnected,
          rootConnected: root.isConnected,
        });
      }
      if (performance.now() < deadline) requestAnimationFrame(sample);
    };
    requestAnimationFrame(sample);
  });

  const encoded = Buffer.from(JSON.stringify(sequence), "utf8").toString("base64");
  const script = resolve(process.cwd(), "../../eng/resize-window-native.ps1");
  const nativeResizeStarted = await page.evaluate(() => performance.now());
  await execFileAsync("pwsh", ["-NoProfile", "-File", script,
    "-TitleToken", titleToken, "-BoundsBase64", encoded,
    "-IntervalMilliseconds", "12", "-StartDelayMilliseconds", "250"], {
    windowsHide: true,
    timeout: 20_000,
  });
  const nativeResizeFinished = await page.evaluate(() => performance.now());
  await page.waitForTimeout(500);
  const nativeSamples = await page.evaluate(() => {
    const scope = globalThis as typeof globalThis & { __dorotiNativeResizeSamples?: Array<{
      timestamp: number; rootWidth: number; rootHeight: number; transform: string;
      preview: string | null; canvasConnected: boolean; rootConnected: boolean;
    }> };
    return scope.__dorotiNativeResizeSamples ?? [];
  });
  const bundle = await waitForSettledPresenter(page);
  const screenshot = await page.screenshot();
  const observedTimes = bundle.trace.filter((entry) => entry.phase === "target-observed")
    .map((entry) => entry.timestampMicroseconds / 1000);
  const finalTarget = bundle.trace.filter((entry) => entry.phase === "target-observed").at(-1);
  const semanticsAfterFinalTarget = finalTarget === undefined ? [] : bundle.trace.filter((entry) =>
    entry.phase === "semantics-dom-applied" && entry.sequence > finalTarget.sequence);
  const activeEpochExactCommits = bundle.trace.filter((entry) =>
    entry.phase === "front-commit" &&
    entry.timestampMicroseconds / 1000 >= nativeResizeStarted &&
    entry.timestampMicroseconds / 1000 <= nativeResizeFinished &&
    entry.backingWidth === entry.surfaceWidth && entry.backingHeight === entry.surfaceHeight);
  const activeCommitTimes = activeEpochExactCommits.map((entry) => entry.timestampMicroseconds / 1000);
  const activeCommittedGenerations = new Set(activeEpochExactCommits.map((entry) => {
    try {
      return Number((JSON.parse(entry.detail ?? "{}") as { generation?: number }).generation ?? entry.epoch.generation);
    } catch {
      return entry.epoch.generation;
    }
  }));
  const managedSnapshotDurations = bundle.trace
    .filter((entry) => entry.phase === "managed-snapshot-completed")
    .map((entry) => entry.durationMicroseconds / 1000);
  const report = { schemaVersion: "doroti.native-hwnd-resize/v1", windowId, sequence, samples: nativeSamples,
    semanticsAfterFinalTarget: semanticsAfterFinalTarget.length,
    observedResizeCount: observedTimes.length,
    observedResizeCadenceMilliseconds: distribution(observedTimes.slice(1)
      .map((value, index) => value - observedTimes[index])),
    activeEpochExactCommitCount: activeEpochExactCommits.length,
    activeCommittedGenerationCount: activeCommittedGenerations.size,
    activeEpochExactCommitCadenceMilliseconds: distribution(activeCommitTimes.slice(1)
      .map((value, index) => value - activeCommitTimes[index])),
    managedSnapshotDispatchDurationMilliseconds: distribution(managedSnapshotDurations),
    workerAdvancedBeforePriorTerminal: workerAdvancedBeforePriorTerminal(bundle.trace),
    diagnostics: bundle };
  await testInfo.attach("native-hwnd-final", { body: screenshot, contentType: "image/png" });
  await attachJson(testInfo, "native-hwnd-resize-report", report);

  expect(runtimeErrors).toEqual([]);
  expect(sequence).toHaveLength(181);
  expect(nativeSamples.length).toBeGreaterThan(20);
  expect(new Set(nativeSamples.map((sample) => `${sample.rootWidth}x${sample.rootHeight}`)).size).toBeGreaterThan(10);
  // SetWindowPos calls are inputs, not guaranteed browser viewport epochs.
  // Require sustained delivery while using the active committed-generation
  // gate below to catch a renderer that waits until resize ends.
  expect(observedTimes.length).toBeGreaterThanOrEqual(40);
  expect(nativeSamples.every((sample) => sample.canvasConnected && sample.rootConnected)).toBe(true);
  expect(nativeSamples.every((sample) => sample.transform === "none" && sample.preview === null)).toBe(true);
  expect(bundle.trace.filter((entry) => entry.phase === "resize-preview-commit")).toEqual([]);
  expect(bundle.trace.filter((entry) => entry.phase === "preview-front-refresh")).toEqual([]);
  expect(report.activeCommittedGenerationCount).toBeGreaterThanOrEqual(10);
  expect(report.semanticsAfterFinalTarget).toBeGreaterThan(0);
  expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
  expect(bundle.presenter.activeBitmaps).toBe(0);
  if (bundle.presenter.mode === "offscreen-worker") {
    expect(report.workerAdvancedBeforePriorTerminal).toBe(true);
    expect(bundle.presenter.bitmapCreated).toBe(bundle.presenter.bitmapConsumed + bundle.presenter.bitmapClosed);
  }
  assertPresenterContract(bundle);
});
