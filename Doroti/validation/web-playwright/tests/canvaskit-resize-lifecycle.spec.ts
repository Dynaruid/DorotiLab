import { readFile, mkdir, writeFile } from "node:fs/promises";
import { spawn, execFile } from "node:child_process";
import { chromium } from "@playwright/test";
import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, captureDiagnostics, waitForSettledPresenter, assertPresenterContract } from "./helpers/doroti-diagnostics.js";

// Keep capture out of the actual background/foreground lifecycle exercise.
test.use({ video: "off", trace: "off", screenshot: "only-on-failure" });

const candidate = process.env.DOROTI_RESIZE_EXPERIMENT_QUERY ??
  "&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiEncodingCache=1&dorotiMetricsCoalescing=frame";

test("CanvasKit main and UI 100ms stalls recover the latest resize", async ({ page, runtimeErrors }, testInfo) => {
  test.skip(process.env.DOROTI_WEB_RENDERER_MODE !== "worker-canvaskit-webgl");
  await openDoroti(page, candidate);
  let uiWorker: ReturnType<typeof page.workers>[number] | undefined;
  for (const worker of page.workers()) {
    if (await worker.evaluate(() => performance.getEntriesByType("resource")
      .some(entry => entry.name.includes("/doroti.ui.worker.js")))) uiWorker = worker;
  }
  expect(uiWorker, "Identify UI by its loaded role module before injecting a stall").toBeDefined();
  const reports: unknown[] = [];
  for (const owner of ["main", "ui"]) {
    // CDP stimulus is a recovery test, separate from native performance trials.
    const stall = () => {
      const start = performance.now();
      while (performance.now() - start < 100) { /* diagnostic stall */ }
      return performance.now() - start;
    };
    const stalled = owner === "main" ? page.evaluate(stall) : uiWorker!.evaluate(stall);
    const width = owner === "main" ? 976 : 1104;
    await page.setViewportSize({ width, height: 704 });
    const milliseconds = await stalled;
    expect(milliseconds).toBeGreaterThanOrEqual(100);
    await expect.poll(async () => (await captureDiagnostics(page)).snapshot.resizeEpoch.logicalWidth).toBe(width);
    const final = await waitForSettledPresenter(page);
    assertPresenterContract(final);
    expect(final.presenter.uiDiagnostics?.buffers.outstanding).toBe(0);
    expect(final.presenter.workerRestartCount).toBe(0);
    reports.push({ owner, milliseconds, final });
  }
  expect(runtimeErrors).toEqual([]);
  await writeFile(testInfo.outputPath("stall-recovery.json"), JSON.stringify(reports, null, 2));
});

test("@headed CanvasKit maximize restore and background return keep exact geometry", async ({}, testInfo) => {
  test.skip(process.env.DOROTI_WEB_RENDERER_MODE !== "worker-canvaskit-webgl");
  // Focus emulation is session-owned: a second CDP session cannot cancel the
  // Playwright session's override. Use an isolated owned browser and attach
  // without default emulation so real tab visibility reaches the application.
  const profile = testInfo.outputPath("lifecycle-profile");
  await mkdir(profile, { recursive: true });
  const chromiumProcess = spawn(chromium.executablePath(), ["--remote-debugging-port=0",
    `--user-data-dir=${profile}`, "--no-first-run", "--no-default-browser-check",
    "--enable-gpu-rasterization", "--ignore-gpu-blocklist", "--disable-features=Translate", "about:blank"],
    { windowsHide: true, stdio: "ignore" });
  let browser: Awaited<ReturnType<typeof chromium.connectOverCDP>> | undefined;
  try {
    let port = "";
    await expect.poll(async () => {
      port = await readFile(`${profile}/DevToolsActivePort`, "utf8").then(s => s.split(/\r?\n/)[0]).catch(() => "");
      return port;
    }).not.toBe("");
    browser = await chromium.connectOverCDP(`http://127.0.0.1:${port}`, { noDefaults: true });
    const context = browser.contexts()[0];
    const page = context.pages()[0];
    const runtimeErrors: string[] = [];
    page.on("pageerror", error => runtimeErrors.push(error.message));
    page.on("console", message => { if (message.type() === "error") runtimeErrors.push(message.text()); });
    await openDoroti(page, candidate, process.env.DOROTI_WEB_BASE_URL ?? "http://127.0.0.1:5088");
    const cdp = await context.newCDPSession(page);
    const { windowId, bounds } = await cdp.send("Browser.getWindowForTarget");
    const reports: unknown[] = [];
    const record = async (phase: string) => {
      await expect.poll(async () => {
        const { width, height } = await page.evaluate(() => ({ width: innerWidth, height: innerHeight }));
        const epoch = (await captureDiagnostics(page)).snapshot.resizeEpoch;
        return epoch.logicalWidth === width && epoch.logicalHeight === height;
      }).toBe(true);
      const final = await waitForSettledPresenter(page);
      assertPresenterContract(final);
      expect(final.presenter.uiDiagnostics?.buffers.outstanding).toBe(0);
      reports.push({ phase, final });
    };
    try {
      const beforeMaximize = (await captureDiagnostics(page)).snapshot.resizeEpoch.generation;
      await cdp.send("Browser.setWindowBounds", { windowId, bounds: { windowState: "maximized" } });
      await expect.poll(async () => (await captureDiagnostics(page)).snapshot.resizeEpoch.generation).toBeGreaterThan(beforeMaximize);
      await expect.poll(async () => (await cdp.send("Browser.getWindowBounds", { windowId })).bounds.windowState).toBe("maximized");
      await record("maximized");
      const beforeRestore = (await captureDiagnostics(page)).snapshot.resizeEpoch.generation;
      await cdp.send("Browser.setWindowBounds", { windowId, bounds: { windowState: "normal" } });
      const { windowState: _, ...normalBounds } = bounds;
      await cdp.send("Browser.setWindowBounds", { windowId, bounds: normalBounds });
      await expect.poll(async () => (await captureDiagnostics(page)).snapshot.resizeEpoch.generation).toBeGreaterThan(beforeRestore);
      await record("restored");
      const other = await context.newPage();
      try {
        const otherCdp = await context.newCDPSession(other);
        try {
          expect((await otherCdp.send("Browser.getWindowForTarget")).windowId).toBe(windowId);
        } finally { await otherCdp.detach(); }
        await other.bringToFront();
        await expect.poll(() => page.evaluate(() => document.visibilityState)).toBe("hidden");
      } finally {
        await other.close();
        await page.bringToFront();
      }
      await expect.poll(() => page.evaluate(() => document.visibilityState)).toBe("visible");
      await record("background-return");
      expect(runtimeErrors).toEqual([]);
    } finally {
      await writeFile(testInfo.outputPath("window-lifecycle.json"), JSON.stringify(reports, null, 2));
      await cdp.send("Browser.setWindowBounds", { windowId, bounds: { windowState: "normal" } });
      const { windowState: _, ...normalBounds } = bounds;
      await cdp.send("Browser.setWindowBounds", { windowId, bounds: normalBounds });
      await cdp.detach();
    }
  } finally {
    if (browser) {
      try { await (await browser.newBrowserCDPSession()).send("Browser.close"); } catch { }
      await browser.close();
    }
    if (chromiumProcess.exitCode === null && chromiumProcess.pid) {
      await new Promise<void>(resolve => execFile("taskkill", ["/PID", String(chromiumProcess.pid), "/T", "/F"],
        { windowsHide: true }, () => resolve()));
    }
  }
});
