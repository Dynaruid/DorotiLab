import { writeFile } from "node:fs/promises";
import { test, expect } from "./helpers/fixtures.js";
import { PNG } from "pngjs";
import { assertPresenterContract, captureDiagnostics, openDoroti } from "./helpers/doroti-diagnostics.js";

for (const scenario of ["testbed", "autofocus-dark"] as const) {
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
    const query = scenario === "autofocus-dark" ? "&dorotiTestbedMode=boot-autofocus" : "&dorotiTestbedMode=sample";
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

for (const path of ["/", "/?dorotiTestbedMode=sample"]) {
  test(`default Web startup renders content: ${path}`, async ({ page, runtimeErrors }, testInfo) => {
    // Exercise the user's launch URL without renderer overrides or diagnostics.
    await page.goto(path);
    await page.waitForFunction(() =>
      ["started", "failed"].includes(document.documentElement.dataset.dorotiBootstrapStage ?? ""));
    const state = await page.evaluate(() => ({ ...document.documentElement.dataset }));
    expect(state.dorotiBootstrapStage, state.dorotiBootstrapError).toBe("started");
    expect(state.dorotiBootstrapError).toBeUndefined();
    expect(state.dorotiRuntimeLocation).toBe("main");
    const canvas = page.locator("#doroti-surface");
    await expect(canvas).toBeVisible();
    // Runtime readiness can precede the first framework frame. Require actual
    // painted canvas content rather than accepting an empty ready surface.
    await expect.poll(async () => {
      const png = PNG.sync.read(await canvas.screenshot());
      const colors = new Set<number>();
      for (let offset = 0; offset < png.data.length; offset += 16)
        colors.add(png.data.readUInt32BE(offset));
      return colors.size;
    }).toBeGreaterThan(20);
    await testInfo.attach("first-content", { body: await page.screenshot(), contentType: "image/png" });
    expect(runtimeErrors).toEqual([]);
  });
}
