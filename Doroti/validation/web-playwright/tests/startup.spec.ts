import { writeFile } from "node:fs/promises";
import { test, expect } from "./helpers/fixtures.js";
import { test as rawTest } from "@playwright/test";
import { PNG } from "pngjs";
import { assertPresenterContract, captureDiagnostics, openDoroti } from "./helpers/doroti-diagnostics.js";

for (const scenario of ["fixture-no-text", "testbed", "autofocus-dark"] as const) {
  test(`Boot evidence: ${scenario}`, async ({ page, context, runtimeErrors }, testInfo) => {
    const wire: unknown[] = [];
    const pendingRequests: Promise<void>[] = [];
    context.on("requestfinished", request => {
      pendingRequests.push((async () => {
        try { wire.push({ url: request.url(), type: request.resourceType(), sizes: await request.sizes(), timing: request.timing() }); }
        catch (error) { wire.push({ url: request.url(), error: String(error) }); }
      })());
    });
    if (scenario === "autofocus-dark") await page.emulateMedia({ colorScheme: "dark" });
    const query = scenario === "fixture-no-text" ? "&dorotiResizeFixture=F0"
      : scenario === "autofocus-dark" ? "&dorotiTestbedMode=boot-autofocus" : "&dorotiTestbedMode=sample";
    let bundle = await openDoroti(page, query);
    expect(runtimeErrors).toEqual([]);
    expect(await page.locator("html").getAttribute("data-doroti-bootstrap-stage")).toBe("started");
    assertPresenterContract(bundle);
    expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
    let capture: Buffer = Buffer.alloc(0);
    // A clear/splash/loader notification cannot satisfy this canvas-content gate.
    await expect.poll(async () => {
      capture = await page.locator("#doroti-surface").screenshot();
      const pixels = PNG.sync.read(capture);
      const colors = new Set<number>();
      for (let offset = 0; offset < pixels.data.length; offset += 4) colors.add(pixels.data.readUInt32BE(offset));
      return colors.size;
    }, { timeout: 120_000 }).toBeGreaterThan(2);
    bundle = await captureDiagnostics(page);
    const capturePath = testInfo.outputPath("first-content.png");
    await writeFile(capturePath, capture);
    await testInfo.attach("first-content.png", { path: capturePath, contentType: "image/png" });
    let input = "notVerified";
    if (scenario === "autofocus-dark") {
      const textField = page.getByRole("textbox", { name: /Text field/ });
      await expect(page.locator("#doroti-ime")).toBeFocused();
      await page.keyboard.insertText("한글 첫 입력");
      await expect(textField).toHaveValue("한글 첫 입력");
      input = "automated-text-and-aria";
    }
    const requests = await page.evaluate(() => performance.getEntriesByType("resource").map(item => {
      const resource = item as PerformanceResourceTiming;
      return { name: resource.name, initiatorType: resource.initiatorType, startTime: resource.startTime,
        duration: resource.duration, transferSize: resource.transferSize, encodedBodySize: resource.encodedBodySize,
        decodedBodySize: resource.decodedBodySize };
    }));
    await Promise.all(pendingRequests);
    const evidence = JSON.stringify({
      schemaVersion: "doroti.boot-evidence/v1", runId: testInfo.testId, scenario,
      revision: process.env.DOROTI_BOOT_REVISION ?? "unrecorded",
      artifact: process.env.DOROTI_BOOT_ARTIFACT ?? "unrecorded",
      configuration: "Release", platform: "Web", rid: "browser-wasm",
      renderer: bundle.presenter.mode, rendererSelection: process.env.DOROTI_WEB_RENDERER_MODE ?? "auto",
      loaderStage: "started", firstContentEvidence: "exact-front-and-canvas-capture", inputEvidence: input,
      session: bundle.presenter.runtimeSessionId, rasterSession: bundle.presenter.rasterSessionId,
      context: bundle.presenter.contextGeneration, surface: bundle.snapshot.surfaceGeneration,
      generation: bundle.presenter.frontGeneration, scene: bundle.presenter.frontRequestId,
      requests, wire, physicalScanOut: "notVerified", physicalImeAndScreenReader: "notVerified", performance: "notVerified",
      clock: "main performance timeline; worker raw clocks are not subtracted",
    }, null, 2);
    const evidencePath = testInfo.outputPath("boot-evidence.json");
    await writeFile(evidencePath, evidence);
    await testInfo.attach("boot-evidence.json", { contentType: "application/json", path: evidencePath });
  });
}

test("Boot plugin endpoint remains available", async ({ page, runtimeErrors }) => {
  await openDoroti(page);
  const response = await page.evaluate(async () => {
    const url = new URL("_content/Doroti.Host.Web/doroti.web.js", document.baseURI).href;
    const host = await import(url);
    return JSON.parse(await host.invokePlugin("./plugins/echo.js", "invoke", "doroti.example/echo", "standard-message", "AAECAw=="));
  });
  expect(response).toEqual({ hasValue: true, base64: "AAECAw==" });
  expect(runtimeErrors).toEqual([]);
});

for (const failure of ["404", "hash", "init"] as const) {
  rawTest(`CanvasKit startup fails closed: ${failure}`, async ({ page }) => {
    const errors: string[] = [];
    page.on("console", message => { if (message.type() === "error") errors.push(message.text()); });
    let workerRequests = 0;
    page.on("request", request => { if (request.url().includes("doroti.canvaskit.bootstrap")) workerRequests++; });
    if (failure === "404") await page.route("**/canvaskit.wasm", route => route.fulfill({ status: 404, body: "missing" }));
    if (failure === "hash") await page.route("**/canvaskit.manifest.json", async route => {
      const response = await route.fetch();
      const manifest = await response.json();
      manifest.files.find((file: { path: string }) => file.path === "canvaskit.js").sha256 = "0".repeat(64);
      await route.fulfill({ response, json: manifest });
    });
    if (failure === "init") await page.addInitScript(() => {
      const OriginalWorker = Worker;
      const counts = { created: 0, terminated: 0 };
      Object.assign(globalThis, { __dorotiBootWorkerCounts: counts });
      globalThis.Worker = class extends OriginalWorker {
        private ended = false;
        constructor(url: string | URL, options?: WorkerOptions) {
          super(url, options);
          counts.created++;
          const send = this.postMessage.bind(this);
          this.postMessage = ((message: Record<string, unknown>, transfer: Transferable[]) => {
            if (message.kind === "canvaskit-bootstrap-init") message.canvasKitWasmUrl = `${location.origin}/missing-init.wasm`;
            send(message, transfer);
          }) as typeof this.postMessage;
        }
        override terminate(): void {
          if (!this.ended) { this.ended = true; counts.terminated++; }
          super.terminate();
        }
      };
    });
    await page.goto("/?dorotiRenderer=worker-canvaskit-webgl");
    await expect(page.locator("html")).toHaveAttribute("data-doroti-bootstrap-stage", "failed", { timeout: 120_000 });
    expect(errors.join("\n")).toMatch(failure === "hash" ? /SHA-256 mismatch/ : failure === "404" ? /fetch failed.*404/ : /wasm|abort|failed/i);
    if (failure !== "init") expect(workerRequests).toBe(0);
    else expect(await page.evaluate(() => (globalThis as typeof globalThis & {
      __dorotiBootWorkerCounts: { created: number; terminated: number };
    }).__dorotiBootWorkerCounts)).toEqual({ created: 2, terminated: 2 });
  });
}

rawTest("CanvasKit starts WASM verification while JS verification is pending", async ({ page }) => {
  let releaseJs!: () => void;
  const gate = new Promise<void>(resolve => { releaseJs = resolve; });
  let wasmStarted = false;
  let scriptPending = false;
  await page.route("**/canvaskit.js", async route => {
    if (route.request().resourceType() === "fetch") { scriptPending = true; await gate; }
    await route.continue();
  });
  await page.route("**/canvaskit.wasm", async route => { wasmStarted = true; await route.continue(); });
  try {
    await page.goto("/?dorotiRenderer=worker-canvaskit-webgl", { waitUntil: "domcontentloaded" });
    await expect.poll(() => scriptPending && wasmStarted).toBe(true);
    await expect(page.locator("html")).not.toHaveAttribute("data-doroti-bootstrap-stage", "started");
  } finally { releaseJs(); }
  await expect(page.locator("html")).toHaveAttribute("data-doroti-bootstrap-stage", "started", { timeout: 120_000 });
});
