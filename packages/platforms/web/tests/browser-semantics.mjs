import assert from "node:assert/strict";
import { chromium, expect } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";

const [root] = process.argv.slice(2);
assert.ok(root, "Pass a dual-published Demo wwwroot.");
const servers = await startPublishServers(root);
try {
  const browser = await chromium.launch({ args: ["--enable-unsafe-swiftshader"] });
  try {
    for (const mode of ["threaded", "compatible"]) {
      const page = await browser.newPage({ viewport: { width: 840, height: 760 } });
      try {
        let moduleUrl;
        const errors = [];
        page.on("pageerror", error => errors.push(String(error)));
        page.on("request", request => {
          if (new URL(request.url()).pathname.endsWith("/doroti.web.js")) moduleUrl ??= request.url();
        });
        const origin = servers.addresses.find(address => address.isolated === (mode === "threaded")).url;
        await page.goto(`${origin}/?dorotiExecutionMode=${mode}&dorotiRenderer=worker-webgl`, { waitUntil: "commit" });
        await page.waitForFunction(() => document.querySelector("[data-doroti-first-frame-at]"), null, { timeout: 120000 });
        await expect(page.locator("html")).toHaveAttribute("data-doroti-execution-mode", mode);
        const layer = page.locator("#doroti-semantics");
        await expect(layer).toHaveAttribute("data-revision", /^\d+$/, { timeout: 120000 });
        const button = layer.getByRole("button", { name: "Elevated", exact: true }).first();
        await expect(button).toBeAttached({ timeout: 30000 });
        await button.focus();
        assert.ok(await button.evaluate(element => document.activeElement === element));
        const anchor = await button.evaluate(element => ({
          id: element.id,
          revision: Number(document.getElementById("doroti-semantics").dataset.revision),
        }));
        await button.evaluate(element => {
          globalThis.semanticsAnchor = element;
          globalThis.semanticsFocus = document.activeElement;
        });

        await page.setViewportSize({ width: 884, height: 787 });
        await expect.poll(async () => Number(await layer.getAttribute("data-revision"))).toBeGreaterThan(anchor.revision);
        assert.ok(await page.evaluate(id => globalThis.semanticsAnchor === document.getElementById(id), anchor.id));
        assert.ok(await page.evaluate(() => globalThis.semanticsAnchor === document.activeElement));

        assert.ok(moduleUrl, "The published DOM bridge must be loaded.");
        const beforeRecovery = Number(await layer.getAttribute("data-revision"));
        await page.evaluate(async ({ url, id }) => {
          const bridge = await import(url);
          const { stream, revision, generation } = document.getElementById("doroti-semantics").dataset;
          const host = Number(id.split("-")[2]);
          bridge.updateSemantics(host, JSON.stringify({
            schemaVersion: 1, kind: "delta", stream: Number(stream), revision: Number(revision) + 500,
            baseRevision: Number(revision) + 499, generation: Number(generation), nodes: [], removed: [],
          }));
        }, { url: moduleUrl, id: anchor.id });
        await expect.poll(async () => Number(await layer.getAttribute("data-revision"))).toBeGreaterThan(beforeRecovery);
        assert.ok(await page.evaluate(id => globalThis.semanticsAnchor === document.getElementById(id), anchor.id));
        assert.ok(await page.evaluate(() => globalThis.semanticsFocus === document.activeElement));

        const cdp = await page.context().newCDPSession(page);
        const { root: document } = await cdp.send("DOM.getDocument");
        const { nodeId } = await cdp.send("DOM.querySelector", { nodeId: document.nodeId, selector: `#${anchor.id}` });
        const ax = await cdp.send("Accessibility.getPartialAXTree", { nodeId, fetchRelatives: false });
        assert.ok(ax.nodes.some(node => !node.ignored && node.role?.value === "button"), "Chromium exposes the retained button.");
        assert.deepEqual(errors, []);
        console.log(`PASS ${mode}: Demo resize, DOM identity, focus, revision recovery and Chromium button`);
      } finally { await page.close(); }
    }
  } finally { await browser.close(); }
} finally { await servers.close(); }
