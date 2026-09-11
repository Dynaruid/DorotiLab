import { chromium } from "../web-playwright/node_modules/playwright/index.mjs";
import { execFileSync } from "node:child_process";
import { createServer } from "node:http";
import { readFile, mkdir, writeFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";

const here = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(here, "../../..");
const output = path.join(root, "Doroti/artifacts/validation/platform-views/web-dom");
await mkdir(output, { recursive: true });
execFileSync(process.execPath, [path.join(here, "../web-playwright/node_modules/typescript/bin/tsc"),
  path.join(root, "Doroti/src/Doroti.Host.Web/Web/doroti.web.platform-views.ts"),
  "--target", "ES2022", "--module", "ESNext", "--strict", "--skipLibCheck", "--outDir", output], { stdio: "inherit" });
const server = createServer(async (request, response) => {
  if (request.url === "/registry.js") {
    response.setHeader("content-type", "text/javascript");
    response.end(await readFile(path.join(output, "doroti.web.platform-views.js")));
  } else {
    response.setHeader("content-type", "text/html");
    response.end('<!doctype html><html><head><meta charset="utf-8"></head><body><div id="root" style="position:relative;width:500px;height:400px;background:#ccddee"></div></body></html>');
  }
});
await new Promise(resolve => server.listen(0, "127.0.0.1", resolve));
let browser;
const reports = [];
try {
  browser = await chromium.launch({ headless: true });
  for (const dpr of [1, 1.25, 1.5, 2]) {
    const context = await browser.newContext({ viewport: { width: 700, height: 550 }, deviceScaleFactor: dpr });
    const page = await context.newPage();
    await page.goto(`http://127.0.0.1:${server.address().port}/`);
    await page.evaluate(async () => {
      const { DorotiPlatformViewDomRegistry } = await import("/registry.js");
      const root = document.querySelector("#root");
      window.counts = { native: 0, ingress: 0, root: 0, focus: 0, loads: 0, disposed: 0 };
      root.addEventListener("pointerdown", () => window.counts.root++);
      const registry = window.registry = new DorotiPlatformViewDomRegistry(root, "1", () => window.counts.focus++,
        event => { if (event.type === "pointerdown") window.counts.ingress++; });
      registry.register("button", () => {
        const element = document.createElement("button"); element.textContent = "Native button";
        element.addEventListener("click", () => window.counts.native++);
        return { element, dispose: () => window.counts.disposed++ };
      });
      registry.register("iframe", () => {
        const element = document.createElement("iframe");
        element.srcdoc = '<button onclick="this.textContent=\'Clicked\'">Iframe button</button>';
        element.addEventListener("load", () => window.counts.loads++);
        window.originalIframe = element;
        return { element, dispose: () => window.counts.disposed++ };
      });
      window.buttonId = { owner: "1", id: "1", generation: "1" };
      window.iframeId = { owner: "1", id: "2", generation: "2" };
      await registry.create(window.buttonId, "button");
      await registry.create(window.iframeId, "iframe");
      window.frame = 0;
      window.batch = (shields = [], left = 20) => ({ version: 1, owner: "1", epoch: 1, surfaceGeneration: 1, frame: ++window.frame,
        views: [
          { identity: window.buttonId, bounds: { left, top: 20, width: 180, height: 60 }, visible: true, order: 1 },
          { identity: window.iframeId, bounds: { left: 20, top: 150, width: 260, height: 120 }, visible: true, order: 2 },
        ], shields });
      registry.setEpoch(1, 1); registry.commit(window.batch());
    });
    await page.waitForFunction(() => window.counts.loads === 1);
    await page.getByRole("button", { name: "Native button", exact: true }).click();
    await page.evaluate(() => {
      if (window.counts.native !== 1) throw new Error("native click missing");
      window.registry.commit(window.batch([{ id: "menu", bounds: { left: 20, top: 20, width: 80, height: 60 }, order: 3, debug: true }]));
    });
    await page.mouse.click(48, 48);
    await page.evaluate(() => {
      if (window.counts.native !== 1 || window.counts.ingress !== 1 || window.counts.root !== 1) throw new Error("shield double dispatch");
    });
    await page.mouse.click(158, 48);
    await page.frameLocator("iframe").getByRole("button").click();
    await page.evaluate(() => {
      if (window.counts.native !== 2) throw new Error("native input outside shield failed");
      const batch = window.batch();
      window.registry.commit(batch);
      let rejected = 0;
      for (const invalid of [batch, { ...batch, version: 0, frame: 900 }, { ...batch, epoch: 0, frame: 900 },
        { ...batch, frame: 900, views: [{ ...batch.views[0], identity: { ...window.buttonId, generation: "999" } }] }]) {
        try { window.registry.commit(invalid); } catch { rejected++; }
      }
      if (rejected !== 4) throw new Error("stale packet was accepted");
      for (let i = 0; i < 10; i++) window.registry.commit(window.batch([], 20 + i));
      if (window.registry.shieldCount !== 0 || document.querySelector("iframe") !== window.originalIframe) throw new Error("DOM identity/listener leak");
    });
    await page.getByRole("button", { name: "Native button", exact: true }).click();
    await page.screenshot({ path: path.join(output, `dpr-${dpr}.png`) });
    const result = await page.evaluate(async () => {
      if (window.counts.native !== 3 || window.counts.ingress !== 1 || window.counts.loads !== 1) throw new Error("input restoration/iframe preservation failed");
      for (let i = 0; i < 100; i++) {
        const identity = { owner: "1", id: "3", generation: String(i + 10) };
        await window.registry.create(identity, "button"); await window.registry.remove(identity);
      }
      let release;
      const gate = new Promise(resolve => release = resolve);
      window.registry.register("late", async () => { await gate; return { element: document.createElement("div"), dispose: () => window.counts.disposed++ }; });
      const late = { owner: "1", id: "4", generation: "1000" };
      const creation = window.registry.create(late, "late");
      const removal = window.registry.remove(late); release(); await creation; await removal;
      await window.registry.dispose();
      if (window.registry.liveCount !== 0 || document.querySelectorAll("[data-doroti-platform-view]").length !== 0 || window.counts.disposed !== 103)
        throw new Error("native DOM cleanup failed");
      return window.counts;
    });
    reports.push({ dpr, ...result });
    await context.close();
  }
  const result = { automated: "passed", browser: browser.version(), reports, productLive: "notVerified",
    workerDirectWebgpu: "notVerified", workerDirectWebgl: "notVerified", physical: "notVerified" };
  await writeFile(path.join(output, "result.json"), JSON.stringify(result, null, 2));
  console.log(JSON.stringify(result));
} finally {
  await browser?.close();
  await new Promise(resolve => server.close(resolve));
}
