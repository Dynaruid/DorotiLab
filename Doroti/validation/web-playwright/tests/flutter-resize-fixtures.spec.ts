import { PNG } from "pngjs";
import { writeFile } from "node:fs/promises";
import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, captureDiagnostics, waitForSettledPresenter } from "./helpers/doroti-diagnostics.js";

function inkLines(png: PNG) {
  const lines: { top: number; bottom: number; left: number; right: number }[] = [];
  for (let y = 40; y < Math.min(250, png.height); y++) {
    let left = png.width, right = -1;
    for (let x = 28; x < png.width - 28; x++) {
      const i = (y * png.width + x) * 4;
      if (png.data[i] < 100 && png.data[i + 1] < 140 && png.data[i + 2] < 170) {
        left = Math.min(left, x); right = x;
      }
    }
    if (right < 0) continue;
    const previous = lines.at(-1);
    if (previous && y <= previous.bottom + 2) {
      previous.bottom = y; previous.left = Math.min(previous.left, left); previous.right = Math.max(previous.right, right);
    } else lines.push({ top: y, bottom: y, left, right });
  }
  return lines;
}

test("Flutter and Doroti F0-F2 compare captured geometry with the same font", async ({ page }, testInfo) => {
  test.skip(!process.env.DOROTI_FLUTTER_BASE_URL);
  const results: unknown[] = [];
  for (const fixture of ["F0", "F1", "F2"]) {
    const baseline = new Map<string, PNG>();
    for (const framework of ["doroti", "flutter"]) {
      await page.setViewportSize({ width: 960, height: 640 });
      if (framework === "doroti") await openDoroti(page, `&dorotiResizeFixture=${fixture}`);
      else {
        await page.goto(`${process.env.DOROTI_FLUTTER_BASE_URL}/?renderer=canvaskit&dorotiResizeFixture=${fixture}`);
        await page.waitForFunction(() => Boolean((globalThis as any).__flutterResizeFrame));
      }
      for (const width of [960, 720, 1100, 960]) {
        await page.setViewportSize({ width, height: 640 });
        if (framework === "doroti") {
          await expect.poll(async () => (await captureDiagnostics(page)).snapshot.resizeEpoch.logicalWidth).toBe(width);
          await waitForSettledPresenter(page);
        }
        else await page.waitForFunction(() => (globalThis as any).__flutterResizeFrame?.width === innerWidth);
        const bytes = await page.screenshot();
        const png = PNG.sync.read(bytes);
        await writeFile(testInfo.outputPath(`${fixture}-${framework}-${width}.png`), bytes);
        if (framework === "doroti") baseline.set(String(width), png);
        else {
          const expected = baseline.get(String(width))!;
          expect([png.width, png.height]).toEqual([expected.width, expected.height]);
          let different = 0;
          for (let i = 0; i < png.data.length; i += 4)
            if ([0, 1, 2].some(c => Math.abs(png.data[i + c] - expected.data[i + c]) > 2)) different++;
          const ratio = different / (png.width * png.height);
          const expectedLines = fixture === "F2" ? inkLines(expected) : [];
          const actualLines = fixture === "F2" ? inkLines(png) : [];
          results.push({ fixture, width, ratio, expectedLines, actualLines, endpoint: "browser screenshot; no latency comparison" });
          if (fixture !== "F2") expect(ratio).toBeLessThan(.001);
          else {
            expect(actualLines.length, "same wrap breakpoints").toBe(expectedLines.length);
            for (let i = 0; i < actualLines.length; i++) {
              expect(Math.abs(actualLines[i].right - expectedLines[i].right), "text right edge").toBeLessThanOrEqual(2);
              expect(Math.abs(actualLines[i].top - expectedLines[i].top), "text baseline region").toBeLessThanOrEqual(2);
            }
          }
        }
      }
    }
  }
  await writeFile(testInfo.outputPath("flutter-fixture-comparison.json"), JSON.stringify(results, null, 2));
});
