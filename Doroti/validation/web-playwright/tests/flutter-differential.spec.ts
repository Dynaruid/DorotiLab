import { test, expect } from "./helpers/fixtures.js";
import { captureDiagnostics, openDoroti, percentile } from "./helpers/doroti-diagnostics.js";

test("Doroti direct and Flutter use the matching interaction workload", async ({ page, runtimeErrors }, testInfo) => {
  const flutterBaseUrl = process.env.DOROTI_FLUTTER_BASE_URL;
  test.skip(!flutterBaseUrl, "Flutter differential server is not configured");
  const dorotiRuns: number[][] = [];
  const flutterRuns: number[][] = [];

  const runCount = Number(process.env.DOROTI_DIFFERENTIAL_RUNS ?? 3);
  for (let run = 0; run < runCount; run++) {
    const initial = await openDoroti(page);
    expect(initial.presenter.mode).toBe(process.env.DOROTI_WEB_RENDERER_MODE ?? "worker-direct-webgl");
    const doroti: number[] = [];
    for (let sample = 0; sample < 40; sample++) {
      const before = await captureDiagnostics(page);
      const started = await page.evaluate(() => performance.now());
      await page.getByRole("button", { name: /G6 Material button/ })
        .evaluate((element) => (element as HTMLElement).click());
      await page.waitForFunction((previousRequestId) => {
        const diagnostics = (globalThis as typeof globalThis & {
          __dorotiResizeDiagnostics?: {
            hosts(): number[]; snapshot(id: number): string; presenter(id: string): string;
          };
        }).__dorotiResizeDiagnostics;
        if (!diagnostics) return false;
        const hostId = diagnostics.hosts()[0];
        const snapshot = JSON.parse(diagnostics.snapshot(hostId)) as { canvasId: string };
        const presenter = JSON.parse(diagnostics.presenter(snapshot.canvasId)) as { frontRequestId: number | null };
        return Number(presenter.frontRequestId ?? 0) > Number(previousRequestId);
      }, before.presenter.frontRequestId ?? 0, { polling: "raf" });
      doroti.push(await page.evaluate((value) => performance.now() - value, started));
    }
    dorotiRuns.push(doroti);

    await page.goto(flutterBaseUrl!, { waitUntil: "domcontentloaded" });
    await page.waitForFunction(() =>
      Number((globalThis as typeof globalThis & {
        __flutterDifferentialFrame?: { sequence: number };
      }).__flutterDifferentialFrame?.sequence ?? 0) > 0, undefined, { timeout: 120_000 });
    const flutter: number[] = [];
    for (let sample = 0; sample < 40; sample++) {
      const before = await page.evaluate(() => Number((globalThis as typeof globalThis & {
        __flutterDifferentialFrame?: { sequence: number };
      }).__flutterDifferentialFrame?.sequence ?? 0));
      const started = await page.evaluate(() => performance.now());
      await page.getByRole("button", { name: "Increment" })
        .evaluate((element) => (element as HTMLElement).click());
      await page.waitForFunction((sequence) => Number((globalThis as typeof globalThis & {
        __flutterDifferentialFrame?: { sequence: number };
      }).__flutterDifferentialFrame?.sequence ?? 0) > Number(sequence), before);
      flutter.push(await page.evaluate((value) => performance.now() - value, started));
    }
    flutterRuns.push(flutter);
  }

  const flattenWarm = (runs: number[][]) => runs.flatMap((run) => run.slice(1));
  const dorotiWarm = flattenWarm(dorotiRuns);
  const flutterWarm = flattenWarm(flutterRuns);
  const report = {
    schemaVersion: "doroti.web-flutter-differential/v1",
    limitation: "Browser interaction-to-observed-framework-frame proxy; not compositor scan-out acknowledgement.",
    viewport: await page.viewportSize(),
    sampleCount: { doroti: dorotiWarm.length, flutter: flutterWarm.length },
    first: { doroti: dorotiRuns.map((run) => run[0]), flutter: flutterRuns.map((run) => run[0]) },
    warm: {
      dorotiP50: percentile(dorotiWarm, .5),
      dorotiP95: percentile(dorotiWarm, .95),
      flutterP50: percentile(flutterWarm, .5),
      flutterP95: percentile(flutterWarm, .95),
    },
  };
  await testInfo.attach("flutter-differential", {
    body: Buffer.from(`${JSON.stringify(report, null, 2)}\n`),
    contentType: "application/json",
  });
  console.log("FLUTTER_DIFFERENTIAL", JSON.stringify(report));
  expect(dorotiWarm).toHaveLength(runCount * 39);
  expect(flutterWarm).toHaveLength(runCount * 39);
  expect(report.warm.dorotiP95).toBeLessThanOrEqual(report.warm.flutterP95 + 20);
  expect(runtimeErrors).toEqual([]);
});
