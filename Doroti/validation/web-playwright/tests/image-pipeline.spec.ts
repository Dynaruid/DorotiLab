import { test, expect } from "@playwright/test";
import { readFile, writeFile } from "node:fs/promises";
import { resolve } from "node:path";
import { openDoroti } from "./helpers/doroti-diagnostics.js";

test("public image pipeline reads pixels and themes both supplied photographs", async ({ page }, testInfo) => {
  test.skip(process.env.DOROTI_IMAGE_VALIDATION !== "1", "Requires a DorotiImageValidation=true build and supplied image fixtures.");
  await openDoroti(page);
  const candidates = page.workers();
  let ui = null;
  for (const worker of candidates) {
    if (await worker.evaluate(() => typeof (globalThis as any).getDotnetRuntime === "function")) ui = worker;
  }
  if (!ui) throw new Error(`UI runtime unavailable: ${candidates.map(worker => worker.url()).join(", ")}`);
  const imagePath = process.env.DOROTI_IMAGE_FILE ?? resolve("../../../mae-mu-9002s2VnOAY-unsplash.webp");
  const input = await readFile(imagePath);
  for (const [name, source] of [
    ["local-photo", `base64:${input.toString("base64")}`],
    ["url-photo", "https://plus.unsplash.com/premium_photo-1734210255965-0a721514a34e"],
  ]) {
    const text = await ui.evaluate(async source => {
      const runtime = (globalThis as any).getDotnetRuntime(0);
      const exports = await runtime.getAssemblyExports("DorotiTestbedApp.Web");
      return await exports.DorotiTestbedApp.Web.ImagePipelineExport.Run(source);
    }, source);
    const result = JSON.parse(text);
    expect(Math.max(result.scaledWidth, result.scaledHeight)).toBe(112);
    expect(Buffer.from(result.rgba, "base64").length).toBe(result.scaledWidth * result.scaledHeight * 4);
    expect(Object.keys(result.colors).length).toBeLessThanOrEqual(128);
    expect(result.seed).toBeGreaterThanOrEqual(0xff000000);
    await writeFile(testInfo.outputPath(`${name}.json`), text);
    await testInfo.attach(name, { body: text, contentType: "application/json" });
    console.log("IMAGE_PIPELINE", JSON.stringify({ name, width: result.width, height: result.height, seed: result.seed }));
  }
});
