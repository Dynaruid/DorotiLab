import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, percentile } from "./helpers/doroti-diagnostics.js";

test("direct transferred canvas owns WebGL2 and Worker rAF cadence", async ({ page, runtimeErrors }, testInfo) => {
  const bundle = await openDoroti(page);
  test.skip(bundle.presenter.mode !== "worker-direct-webgl", "direct-worker feasibility only");
  expect(bundle.presenter.visibleContext).toBe("transferred-offscreen-webgl2");
  expect(bundle.presenter.bitmapCreated).toBe(0);
  expect(bundle.presenter.bitmapConsumed).toBe(0);
  expect(bundle.presenter.bitmapClosed).toBe(0);

  const runs = await page.evaluate(async () => {
    const callbackCount = 660;
    const workerSource = `
      onmessage = (event) => {
        const canvas = event.data.canvas;
        const gl = canvas.getContext('webgl2', { failIfMajorPerformanceCaveat: true });
        if (!gl) { postMessage({ error: 'WebGL2 unavailable' }); return; }
        const values = [];
        const tick = (timestamp) => {
          values.push(timestamp);
          gl.clearColor((values.length % 17) / 17, 0.25, 0.75, 1);
          gl.clear(gl.COLOR_BUFFER_BIT);
          gl.flush();
          if (values.length < ${callbackCount}) requestAnimationFrame(tick);
          else postMessage({ values, width: gl.drawingBufferWidth, height: gl.drawingBufferHeight });
        };
        requestAnimationFrame(tick);
      };`;
    const results: Array<{ main: number[]; worker: number[]; width: number; height: number }> = [];
    for (let run = 0; run < 3; run++) {
      const canvas = document.createElement("canvas");
      canvas.width = 320 + run;
      canvas.height = 180 + run;
      canvas.style.position = "fixed";
      canvas.style.left = "-10000px";
      document.body.append(canvas);
      const offscreen = canvas.transferControlToOffscreen();
      const url = URL.createObjectURL(new Blob([workerSource], { type: "text/javascript" }));
      const worker = new Worker(url);
      const workerResult = new Promise<{ values: number[]; width: number; height: number }>((resolve, reject) => {
        worker.onmessage = (event) => event.data.error ? reject(new Error(event.data.error)) : resolve(event.data);
        worker.onerror = (event) => reject(event.error ?? new Error(event.message));
      });
      const mainResult = new Promise<number[]>((resolve) => {
        const values: number[] = [];
        const tick = (timestamp: number) => {
          values.push(timestamp);
          if (values.length < callbackCount) requestAnimationFrame(tick);
          else resolve(values);
        };
        requestAnimationFrame(tick);
      });
      worker.postMessage({ canvas: offscreen }, [offscreen]);
      const [main, measuredWorker] = await Promise.all([mainResult, workerResult]);
      worker.terminate();
      URL.revokeObjectURL(url);
      canvas.remove();
      results.push({ main, worker: measuredWorker.values, width: measuredWorker.width, height: measuredWorker.height });
    }
    return results;
  });

  const refresh = (values: number[]) => {
    const warm = values.slice(60);
    return warm.slice(1).map((value, index) => value - warm[index]);
  };
  const report = runs.map((run) => ({
    mainCallbacks: run.main.length - 60,
    workerCallbacks: run.worker.length - 60,
    mainP95: percentile(refresh(run.main), .95),
    workerP95: percentile(refresh(run.worker), .95),
    width: run.width,
    height: run.height,
  }));
  await testInfo.attach("direct-worker-raf-cadence", {
    body: Buffer.from(`${JSON.stringify(report, null, 2)}\n`),
    contentType: "application/json",
  });
  for (const run of report) {
    expect(Math.abs(run.workerCallbacks - run.mainCallbacks) / run.mainCallbacks).toBeLessThanOrEqual(.01);
    expect(run.workerP95).toBeLessThanOrEqual(run.mainP95 + 20);
    expect(run.width).toBeGreaterThanOrEqual(320);
    expect(run.height).toBeGreaterThanOrEqual(180);
  }
  expect(runtimeErrors).toEqual([]);
});
