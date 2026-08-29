import { test, expect } from "./helpers/fixtures.js";
import {
  assertPresenterContract,
  openDoroti,
  percentile,
  resetDiagnostics,
  waitForSettledPresenter,
} from "./helpers/doroti-diagnostics.js";

test("wheel samples dispatch immediately and rendering stays bounded", async ({ page, runtimeErrors }, testInfo) => {
  await openDoroti(page);
  await resetDiagnostics(page);
  const root = page.locator(".doroti-root");
  const box = await root.boundingBox();
  if (!box) throw new Error("Doroti root has no browser bounds.");
  await page.mouse.move(box.x + box.width / 2, box.y + box.height / 2);
  const latencySampleCount = 60;
  for (let index = 0; index < latencySampleCount; index++) {
    const deltaY = index % 20 < 10 ? 8 : -8;
    if (index === 0) {
      await page.mouse.wheel(0, deltaY);
    } else {
      await page.evaluate((delta) => {
        const rootElement = document.querySelector(".doroti-root");
        if (!(rootElement instanceof HTMLElement)) throw new Error("Doroti root is unavailable.");
        rootElement.dispatchEvent(new WheelEvent("wheel", {
          bubbles: true,
          cancelable: true,
          clientX: 100,
          clientY: 100,
          deltaMode: WheelEvent.DOM_DELTA_PIXEL,
          deltaY: delta,
        }));
      }, deltaY);
    }
    await page.waitForFunction((expectedSamples) => {
      const diagnostics = (globalThis as typeof globalThis & {
        __dorotiResizeDiagnostics?: { hosts(): number[]; trace(hostId: number): string };
      }).__dorotiResizeDiagnostics;
      if (!diagnostics) return false;
      const hostId = diagnostics.hosts()[0];
      const trace = JSON.parse(diagnostics.trace(hostId)) as Array<{
        phase: string;
        timestampMicroseconds: number;
      }>;
      const ingress = trace.filter((entry) => entry.phase === "wheel-ingress");
      if (ingress.length < expectedSamples) return false;
      const latestTimestamp = ingress[expectedSamples - 1].timestampMicroseconds;
      return trace.some((entry) =>
        entry.phase === "front-commit" && entry.timestampMicroseconds >= latestTimestamp);
    }, index + 1, { timeout: 5_000 });
  }

  const prevented = await page.evaluate(() => {
    const rootElement = document.querySelector(".doroti-root");
    if (!(rootElement instanceof HTMLElement)) throw new Error("Doroti root is unavailable.");
    const samples = [
      { mode: WheelEvent.DOM_DELTA_PIXEL, x: 1.5, y: 3.25 },
      { mode: WheelEvent.DOM_DELTA_LINE, x: 0, y: 2 },
      { mode: WheelEvent.DOM_DELTA_PAGE, x: 0, y: -1 },
    ];
    const results = samples.map((sample) => {
      const event = new WheelEvent("wheel", {
        bubbles: true,
        cancelable: true,
        clientX: 100,
        clientY: 100,
        deltaMode: sample.mode,
        deltaX: sample.x,
        deltaY: sample.y,
      });
      rootElement.dispatchEvent(event);
      return event.defaultPrevented;
    });
    return results;
  });

  expect(prevented).toEqual([true, true, true]);
  const latencyBundle = await waitForSettledPresenter(page);
  const latencyIngress = latencyBundle.trace
    .filter((entry) => entry.phase === "wheel-ingress")
    .slice(0, latencySampleCount);
  const latencyDispatch = latencyBundle.trace
    .filter((entry) => entry.phase === "wheel-framework-dispatch")
    .slice(0, latencySampleCount);
  expect(latencyDispatch.map((entry) => entry.inputSequence)).toEqual(
    latencyIngress.map((entry) => entry.inputSequence),
  );
  const latencyCommits = latencyBundle.trace.filter((entry) => entry.phase === "front-commit");
  const commitLatency = latencyIngress.map((entry) => {
    const commit = latencyCommits.find((candidate) => candidate.timestampMicroseconds >= entry.timestampMicroseconds);
    return commit ? (commit.timestampMicroseconds - entry.timestampMicroseconds) / 1_000 : Number.POSITIVE_INFINITY;
  }).filter(Number.isFinite);
  const latencyEvidence = {
    commitLatency,
    phases: latencyBundle.trace.map((entry) => ({
      sequence: entry.sequence,
      phase: entry.phase,
      timestampMicroseconds: entry.timestampMicroseconds,
      durationMicroseconds: entry.durationMicroseconds,
      inputSequence: entry.inputSequence,
      requestId: entry.requestId,
      queueDepth: entry.queueDepth,
      terminal: entry.terminal,
    })),
  };
  await testInfo.attach("wheel-latency.json", {
    body: Buffer.from(`${JSON.stringify(latencyEvidence, null, 2)}\n`),
    contentType: "application/json",
  });
  console.log(`WHEEL_LATENCY ${JSON.stringify({
    samples: commitLatency.length,
    p95Milliseconds: percentile(commitLatency, 0.95),
    maxMilliseconds: Math.max(...commitLatency),
  })}`);
  expect(commitLatency.length).toBe(latencySampleCount);
  if (process.env.DOROTI_WEB_REQUIRE_LATENCY === "1") {
    expect(percentile(commitLatency, 0.95)).toBeLessThanOrEqual(33.4);
    expect(Math.max(...commitLatency)).toBeLessThan(100);
  }

  await resetDiagnostics(page);
  const generated = await page.evaluate(() => {
    const rootElement = document.querySelector(".doroti-root");
    if (!(rootElement instanceof HTMLElement)) throw new Error("Doroti root is unavailable.");
    const sampleCount = 1_200;
    const started = performance.now();
    for (let index = 0; index < sampleCount; index++) {
      const delta = index % 120 < 60 ? 1.25 : -1.25;
      rootElement.dispatchEvent(new WheelEvent("wheel", {
        bubbles: true,
        cancelable: true,
        clientX: 100,
        clientY: 100,
        deltaMode: WheelEvent.DOM_DELTA_PIXEL,
        deltaY: delta,
      }));
    }
    return { sampleCount, durationMilliseconds: performance.now() - started };
  });
  const bundle = await waitForSettledPresenter(page);
  expect(runtimeErrors).toEqual([]);
  const ingress = bundle.trace.filter((entry) => entry.phase === "wheel-ingress");
  const dispatched = bundle.trace.filter((entry) => entry.phase === "wheel-framework-dispatch");
  expect(ingress.length).toBe(generated.sampleCount);
  expect(dispatched.map((entry) => entry.inputSequence)).toEqual(
    ingress.map((entry) => entry.inputSequence),
  );

  const dispatchByInput = new Map(dispatched.map((entry) => [entry.inputSequence, entry]));
  const dispatchLatency = ingress.map((entry) =>
    (dispatchByInput.get(entry.inputSequence)!.timestampMicroseconds - entry.timestampMicroseconds) / 1_000);
  expect(percentile(dispatchLatency, 0.95)).toBeLessThanOrEqual(5);

  expect(generated.durationMilliseconds).toBeLessThan(10_000);
  expect(Math.max(...bundle.trace.map((entry) => entry.queueDepth))).toBeLessThanOrEqual(2);
  assertPresenterContract(bundle);
});
