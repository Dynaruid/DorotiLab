import assert from "node:assert/strict";
import { chromium, expect, firefox } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";
import { installControlledPicker, selectFiles } from "./browser-picker.mjs";

assert.ok(process.argv[2], "Pass the Testbed dual-published wwwroot directory.");
const fixture = await startPublishServers(process.argv[2]);
const browser = process.argv.includes("--firefox")
  ? await firefox.launch({ firefoxUserPrefs: { "webgl.force-enabled": true, "gfx.webrender.software": true } })
  : await chromium.launch({ args: ["--enable-unsafe-swiftshader"] });
const results = [];
const controlledPicker = process.argv.includes("--controlled-picker");
const passed = result => { results.push(result); console.log(JSON.stringify(result)); };
let current;
let recentErrors = [];
const ready = page => page.waitForFunction(() => document.querySelector('[data-doroti-worker-runtime="ready"]') &&
  document.querySelector("[data-doroti-first-frame-at]"), null, { timeout: 120000 });
const runtimeState = page => page.evaluate(async () =>
  (await import(new URL("./_content/Doroti.Host.Web/doroti.web.js", document.baseURI))).getDorotiRuntimeState());
const diagnostics = page => page.evaluate(async () => ({ url: location.href, history: history.state,
  runtime: (await import(new URL("./_content/Doroti.Host.Web/doroti.web.js", document.baseURI))).getDorotiRuntimeState(),
  text: document.querySelector("#app")?.textContent,
  bootstrap: { ...document.documentElement.dataset },
  host: [...document.querySelectorAll("[data-doroti-host]")].map(node => Object.fromEntries(Object.entries(node.dataset).filter(([key]) => key !== "dorotiResizeDiagnostics"))),
  labels: [...document.querySelectorAll("[aria-label]")].map(node => [node.tagName, node.getAttribute("role"), node.getAttribute("aria-label")]),
}));
async function shutdown(page) {
  await page.evaluate(() => dispatchEvent(new Event("pagehide")));
  await page.waitForFunction(() => document.querySelector('[data-doroti-worker-runtime="disposed"]'), null, { timeout: 30000 });
  assert.equal(await page.locator("[data-doroti-platform-view]").count(), 0);
  assert.equal(await page.locator('input[type="file"]').count(), 0);
}
try {
  const requestedMode = process.argv.find(value => value.startsWith("--mode="))?.split("=")[1];
  for (const mode of ["threaded", "compatible"].filter(value => !requestedMode || value === requestedMode)) {
    const origin = fixture.addresses.find(value => value.isolated === (mode === "threaded")).url;
    const context = await browser.newContext();
    if (controlledPicker) await installControlledPicker(context);
    try {
      // Two simultaneous same-origin pages must retain distinct runtime/DOM owners.
      const pages = await Promise.all([context.newPage(), context.newPage()]);
      await Promise.all(pages.map(page => page.goto(`${origin}/?dorotiSample=navigation&dorotiResizeDiagnostics=1`)));
      await Promise.all(pages.map(ready));
      current = pages[0];
      for (const page of pages) assert.equal(await page.locator("#doroti-surface").count(), 1);
      await pages[0].getByRole("button", { name: "First page", exact: true }).click({ force: true });
      await pages[0].waitForURL(/#\/first$/);
      assert.ok(!pages[1].url().includes("#/first"));
      await pages[0].getByRole("button", { name: "Second page", exact: true }).click({ force: true });
      await pages[0].waitForURL(/#\/second$/);
      await pages[0].goBack();
      await pages[0].waitForURL(/#\/first$/);
      await shutdown(pages[0]); await pages[0].close();
      await pages[1].getByRole("button", { name: "Second page", exact: true }).click({ force: true });
      await pages[1].waitForURL(/#\/second$/);
      await shutdown(pages[1]); await pages[1].close();
      passed({ mode, scenario: "concurrent pages; Back navigation; independent shutdown", result: "PASS" });

      const page = current = await context.newPage();
      const errors = [], consoleErrors = recentErrors = [];
      page.on("pageerror", error => errors.push(String(error)));
      page.on("console", message => { if (message.type() === "error") consoleErrors.push(message.text()); });
      await page.goto(`${origin}/?dorotiSample=input&dorotiResizeDiagnostics=1`); await ready(page);
      const field = page.getByRole("textbox", { name: "Framework input before native views", exact: true });
      // Click through the product's text-focus admission, then type into its
      // native IME endpoint. Filling an ARIA projection directly can race its
      // asynchronous focus handoff and is not the browser editing contract.
      await field.click({ force: true });
      await page.waitForFunction(() => document.activeElement?.id === "doroti-ime");
      await page.keyboard.insertText("한글 입력");
      await page.waitForFunction(() => document.querySelector('[role="textbox"][aria-label="Framework input before native views"]')?.value === "한글 입력");
      await page.getByRole("button", { name: "Use WebView", exact: true }).click({ force: true });
      const native = page.frameLocator("[data-doroti-platform-view] iframe").getByRole("textbox", { name: "Web input", exact: true });
      await native.fill("native Korean 한글");
      const previousFrame = await page.locator("[data-doroti-platform-view] iframe").elementHandle();
      await page.getByRole("button", { name: "Recreate native views", exact: true }).click({ force: true });
      await page.waitForFunction(element => !element.isConnected, previousFrame);
      await previousFrame.dispose();
      await page.waitForFunction(() => document.querySelectorAll("[data-doroti-platform-view] iframe").length === 1);
      await native.fill("replacement view");
      await page.getByRole("button", { name: "Remove native views", exact: true }).click({ force: true });
      await page.waitForFunction(() => document.querySelectorAll("[data-doroti-platform-view]").length === 0);
      assert.equal(await field.inputValue(), "한글 입력");
      await page.getByRole("button", { name: "Create native views", exact: true }).click({ force: true });
      await native.fill("restored view");
      await page.setViewportSize({ width: 1024, height: 720 });
      if (mode === "compatible") {
        await page.evaluate(async () => (await import(new URL("./_content/Doroti.Host.Web/doroti.web.js", document.baseURI))).restartDorotiWebHost());
        await expect.poll(() => runtimeState(page), { timeout: 120000 }).toMatchObject({ generation: 2, state: "ready" });
        const heading = page.getByRole("heading", { name: "Input and native view lifetime", exact: true });
        await heading.waitFor({ state: "attached", timeout: 10000 });
        assert.equal(await heading.count(), 1,
          "Bootstrap application options must survive Router query removal and Worker replacement.");
      }
      await shutdown(page);
      assert.deepEqual(errors, []);
      // The cross-platform scene intentionally starts with native-editor, which
      // has no browser factory. Require that exact diagnosed boundary; iframe
      // creation, focus, replacement and shutdown are the supported path here.
      assert.ok(consoleErrors.some(value => value.includes("No factory is registered for this view type.")));
      assert.deepEqual(consoleErrors.filter(value => !/PlatformView creation failed|DorotiCapabilityException:.*No factory is registered|^\s+at Doroti\.(Hosting\.PlatformViewCoordinator|Framework\.(Services\.PlatformViewClient|Widgets\.(PlatformViewStateImpl|StatefulElement|ComponentElement)))|Exception_EndOfInnerExceptionStack/.test(value)), []);
      consoleErrors.length = 0;
      passed({ mode, scenario: "framework text; iframe create/recreate/remove; resize; DOM owner disposal", result: "PASS",
        unsupported: "native-editor has no browser factory; its explicit capability error is expected, not counted as native-editor success" });

      await page.goto(`${origin}/?dorotiSample=plugins&dorotiResizeDiagnostics=1`); await ready(page);
      const choose = page.getByRole("button", { name: "Choose files", exact: true });
      await selectFiles(page, () => choose.click({ force: true }),
        { name: "fixture.txt", mimeType: "text/plain", buffer: Buffer.from("Doroti 한글") }, controlledPicker);
      await page.waitForFunction(() => [...document.querySelectorAll("[aria-label]")].some(node => node.getAttribute("aria-label")?.includes("fixture.txt:")));
      assert.ok((await page.locator('[aria-label*="fixture.txt:"]').first().getAttribute("aria-label")).includes(Buffer.from("Doroti 한글").toString("hex").toUpperCase()));
      await selectFiles(page, () => choose.click({ force: true }), [], controlledPicker);
      await page.waitForFunction(() => [...document.querySelectorAll("[aria-label]")].some(node => /^cancelled$/i.test(node.getAttribute("aria-label") ?? "")));
      await shutdown(page);
      assert.deepEqual(errors, []);
      assert.deepEqual(consoleErrors, []);
      passed({ mode, scenario: "managed file picker; bounded file read; cancel; grant/DOM cleanup", result: "PASS",
        picker: controlledPicker ? "controlled input and synthetic cancel; native dialog not qualified" : "browser filechooser interception" });
    } catch (error) {
      if (current && !current.isClosed()) console.error(JSON.stringify({ ...(await diagnostics(current)), errors: recentErrors }, null, 2));
      throw error;
    } finally { await context.close(); }
  }
  // Use the actual non-shared renderer's existing context diagnostic command.
  const page = current = await browser.newPage();
  await page.goto(`${fixture.addresses.find(value => !value.isolated).url}/?dorotiResizeDiagnostics=1`); await ready(page);
  const worker = page.workers()[0];
  const protocol = await page.evaluate(async () => (await import(new URL("./_content/Doroti.Host.Web/doroti.web.protocol.js", document.baseURI))).dorotiProtocolVersion);
  await worker.evaluate(protocolVersion => dispatchEvent(new MessageEvent("message", { data: { protocolVersion, kind: "context", action: "lose" } })), protocol);
  await page.waitForFunction(() => document.querySelector('[data-doroti-platform-context-lost="true"]'));
  await worker.evaluate(protocolVersion => dispatchEvent(new MessageEvent("message", { data: { protocolVersion, kind: "context", action: "restore" } })), protocol);
  await expect.poll(() => runtimeState(page), { timeout: 120000 }).toMatchObject({ generation: 2, state: "ready" });
  await ready(page);
  assert.equal(await page.locator("#doroti-surface").count(), 1);
  await shutdown(page); await page.close();
  passed({ mode: "compatible", scenario: "actual WEBGL_lose_context; restoration; fresh canvas/runtime owner; bounded recovery", result: "PASS" });

  const threaded = current = await browser.newPage();
  await threaded.goto(`${fixture.addresses.find(value => value.isolated).url}/?dorotiResizeDiagnostics=1`); await ready(threaded);
  const stoppedRoot = await threaded.locator("[data-doroti-host]").elementHandle();
  const candidates = await Promise.all(threaded.workers().map(async worker => {
    const ownsGl = await Promise.race([worker.evaluate(() => !!globalThis.__dorotiDirectDiagnostics).catch(() => false),
      new Promise(done => setTimeout(() => done(false), 5000))]);
    return ownsGl ? worker : null;
  }));
  const renderOwner = candidates.find(Boolean);
  assert.ok(renderOwner, "The affine render owner must answer diagnostics.");
  await renderOwner.evaluate(() => {
    globalThis.__testContextLoss = globalThis.SkiaSharpGL.currentContext.GLctx.getExtension("WEBGL_lose_context");
    globalThis.__testContextLoss.loseContext();
  });
  await threaded.waitForFunction(() => document.querySelector('[data-doroti-platform-context-lost="true"]'));
  await renderOwner.evaluate(() => globalThis.__testContextLoss.restoreContext());
  await expect.poll(() => runtimeState(threaded), { timeout: 30000 }).toMatchObject({ state: "failed" });
  // Terminal failure replaces the app DOM with its actionable error after
  // disposal. The retired root still carries the owner's shutdown receipt.
  await threaded.waitForFunction(root => root.dataset.dorotiWorkerRuntime === "disposed" && root.dataset.dorotiShutdown === "complete", stoppedRoot);
  await stoppedRoot.dispose();
  await threaded.reload(); await ready(threaded);
  assert.equal(await threaded.locator("#doroti-surface").count(), 1);
  assert.equal(await threaded.evaluate(() => document.documentElement.dataset.dorotiExecutionMode), "threaded");
  assert.equal(await threaded.evaluate(() => !!globalThis.getDotnetRuntime(1)), false);
  await shutdown(threaded); await threaded.close();
  passed({ mode: "threaded", scenario: "actual GL loss/restore; owner disposal; explicit page reload; single shared runtime", result: "PASS" });
  console.log(JSON.stringify({ browser: browser.version(), controlledPicker,
    limitations: "Headless software GPU; synthetic Korean DOM text commit is not physical Korean IME or assistive technology acceptance. Controlled picker results do not qualify native dialog selection.", results }, null, 2));
} catch (error) {
  if (current && !current.isClosed()) console.error(JSON.stringify(await current.evaluate(() => ({
    url: location.href, bootstrap: { ...document.documentElement.dataset },
    host: [...document.querySelectorAll("[data-doroti-host]")].map(node => ({ ...node.dataset })),
    labels: [...document.querySelectorAll("[aria-label]")].map(node => [node.tagName, node.getAttribute("role"), node.getAttribute("aria-label")]),
  })), null, 2));
  throw error;
} finally { await browser.close(); await fixture.close(); }
