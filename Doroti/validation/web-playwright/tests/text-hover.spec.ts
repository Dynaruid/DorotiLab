import { test, expect } from "./helpers/fixtures.js";
import { PNG } from "pngjs";
import { openDoroti } from "./helpers/doroti-diagnostics.js";

for (const dpr of [1, 1.25, 1.5, 2]) {
  test.describe(`DPR ${dpr}`, () => {
    test.use({ deviceScaleFactor: dpr });
    test("button hover preserves glyph placement", async ({ page, runtimeErrors }, testInfo) => {
      await page.setViewportSize({ width: 1280, height: 900 });
      await openDoroti(page, "&dorotiTestbedMode=sample");
      const button = page.getByRole("button", { name: "Elevated", exact: true }).first();
      await expect(button).toBeAttached();
      await page.waitForTimeout(1500);
      const bounds = (await button.boundingBox())!;
      const clip = { x: Math.floor(bounds.x + 15), y: Math.floor(bounds.y + bounds.height / 2 - 9), width: Math.floor(bounds.width - 30), height: 18 };
      const samples: { x: number; y: number; mass: number }[] = [];
      for (let frame = 0; frame < 8; frame++) {
        await page.mouse.move(frame % 2 ? bounds.x + bounds.width / 2 : 1250, frame % 2 ? bounds.y + bounds.height / 2 : 80);
        await page.waitForTimeout(frame < 4 ? 20 : 300);
        const bytes = await page.screenshot({ clip });
        await testInfo.attach(`hover-${frame}`, { body: bytes, contentType: "image/png" });
        const png = PNG.sync.read(bytes);
        const values = Array.from({ length: png.width * png.height }, (_, i) => png.data[i * 4] + png.data[i * 4 + 1] + png.data[i * 4 + 2]);
        const light = Math.max(...values), dark = Math.min(...values);
        let x = 0, y = 0, mass = 0;
        values.forEach((v, i) => { const weight = (light - v) / (light - dark); mass += weight; x += i % png.width * weight; y += Math.floor(i / png.width) * weight; });
        samples.push({ x: x / mass / dpr, y: y / mass / dpr, mass });
      }
      console.log("HOVER_GLYPH_SAMPLES", JSON.stringify(samples));
      expect(Math.max(...samples.map(s => s.x)) - Math.min(...samples.map(s => s.x))).toBeLessThan(0.1);
      expect(Math.max(...samples.map(s => s.y)) - Math.min(...samples.map(s => s.y))).toBeLessThan(0.1);
      expect(runtimeErrors).toEqual([]);
    });
  });
}
