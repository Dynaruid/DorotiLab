import assert from "node:assert/strict";
import { deflateSync } from "node:zlib";
import { chromium, expect, firefox } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";
import { installControlledPicker, selectFiles } from "./browser-picker.mjs";

assert.ok(process.argv[2], "Pass Sample2's dual-published wwwroot directory.");
const fixture = await startPublishServers(process.argv[2]);
const browser = process.argv.includes("--firefox")
  ? await firefox.launch({ firefoxUserPrefs: { "webgl.force-enabled": true, "gfx.webrender.software": true } })
  : await chromium.launch({ args: ["--enable-unsafe-swiftshader"] });
const results = [];
const controlledPicker = process.argv.includes("--controlled-picker");
function tinyPng() {
  const chunk = (type, bytes) => {
    const name = Buffer.from(type), data = Buffer.concat([name, bytes]);
    let crc = 0xffffffff;
    for (const byte of data) { crc ^= byte; for (let bit = 0; bit < 8; bit++) crc = crc & 1 ? 0xedb88320 ^ (crc >>> 1) : crc >>> 1; }
    const size = Buffer.alloc(4), checksum = Buffer.alloc(4);
    size.writeUInt32BE(bytes.length); checksum.writeUInt32BE((crc ^ 0xffffffff) >>> 0);
    return Buffer.concat([size, data, checksum]);
  };
  const header = Buffer.alloc(13); header.writeUInt32BE(2); header.writeUInt32BE(2, 4); header[8] = 8; header[9] = 6;
  const row = Buffer.from([0, 255, 0, 0, 255, 0, 255, 0, 255]);
  return Buffer.concat([Buffer.from([137, 80, 78, 71, 13, 10, 26, 10]), chunk("IHDR", header), chunk("IDAT", deflateSync(Buffer.concat([row, row]))), chunk("IEND", Buffer.alloc(0))]);
}
const tap = (page, label) => page.locator(`[role="tab"][aria-label^="${label}"], [role="button"][aria-label^="${label}"], [aria-label="${label}"]`).first().click({ force: true });
const frames = page => page.evaluate(() => new Promise(done => requestAnimationFrame(() => requestAnimationFrame(done))));
try {
  for (const mode of ["threaded", "compatible"]) {
    const context = await browser.newContext({ viewport: { width: 1180, height: 760 } });
    if (controlledPicker) await installControlledPicker(context);
    const page = await context.newPage(), errors = [], fonts = [];
    page.on("pageerror", error => errors.push(String(error)));
    page.on("console", message => { if (message.type() === "error") errors.push(message.text()); });
    page.on("response", response => { if (/\.(?:woff2?|ttf)(?:[?#]|$)/i.test(response.url())) fonts.push({ url: response.url(), status: response.status() }); });
    try {
      const origin = fixture.addresses.find(value => value.isolated === (mode === "threaded")).url;
      await page.goto(`${origin}/?dorotiResizeDiagnostics=1`);
      await page.waitForFunction(() => document.querySelector('[data-doroti-worker-runtime="ready"]') && document.querySelector("[data-doroti-first-frame-at]"), null, { timeout: 120000 });
      await tap(page, "Tap me");
      await page.waitForFunction(() => document.querySelector('[aria-label="Button taps: 1"]'));
      await tap(page, "Profile");
      const field = page.getByRole("textbox").first();
      await field.click({ force: true });
      await page.waitForFunction(() => document.activeElement?.id === "doroti-ime");
      await page.keyboard.insertText("한글"); await tap(page, "Done");
      await page.waitForFunction(() => document.querySelector('[aria-label="Hello, 한글!"]'));
      await tap(page, "Fonts");
      for (const family of ["Roboto", "Galmuri11", "SUITE Variable"]) {
        await tap(page, family);
        await page.locator(`[aria-label^="${family} · wght"]`).waitFor({ state: "attached" });
      }
      await tap(page, "Variable Blur");
      await page.locator('[aria-label^="Blur strength:"]').waitFor({ state: "attached" });
      await page.mouse.move(500, 600);
      for (let step = 0; step < 6; step++) { await page.mouse.wheel(0, step < 3 ? 250 : -250); await frames(page); }
      const slider = await page.getByRole("slider").first().boundingBox();
      assert.ok(slider);
      await page.mouse.move(slider.x + slider.width * .65, slider.y + slider.height / 2);
      await page.mouse.down(); await page.mouse.move(slider.x + slider.width * .35, slider.y + slider.height / 2, { steps: 3 }); await page.mouse.up();
      await page.setViewportSize({ width: 1024, height: 720 }); await frames(page);
      await tap(page, "Upload");
      await selectFiles(page, () => tap(page, "파일 선택"), [
        { name: "fixture.txt", mimeType: "text/plain", buffer: Buffer.from("한글 preview") },
        { name: "tiny.png", mimeType: "image/png", buffer: tinyPng() },
      ], controlledPicker);
      // Switch views while asynchronous reads/decodes may still be pending.
      await tap(page, "Profile");
      assert.equal(await page.getByRole("textbox").first().inputValue(), "한글");
      await tap(page, "Upload");
      await page.waitForFunction(() => document.querySelector('[aria-label="추가한 파일 (2)"]'));
      const preview = page.locator('[role="img"][aria-label="tiny.png"]');
      await preview.waitFor({ state: "attached" });
      await page.locator('[aria-label="한글 preview"]').waitFor({ state: "attached" });
      await page.mouse.move(500, 550); await page.mouse.wheel(0, 700);
      await expect.poll(async () => {
        const box = await preview.boundingBox();
        return !!box && box.y >= 0 && box.y + box.height <= page.viewportSize().height;
      }, { timeout: 30000 }).toBe(true);
      await expect.poll(async () => {
        const image = await page.screenshot({ clip: await preview.boundingBox() });
        return page.evaluate(async base64 => {
          const bitmap = await createImageBitmap(await (await fetch(`data:image/png;base64,${base64}`)).blob());
          const canvas = new OffscreenCanvas(bitmap.width, bitmap.height), context = canvas.getContext("2d");
          context.drawImage(bitmap, 0, 0); bitmap.close();
          const pixels = context.getImageData(0, 0, canvas.width, canvas.height).data;
          let red = 0, green = 0;
          for (let i = 0; i < pixels.length; i += 4) {
            if (pixels[i] > 240 && pixels[i + 1] < 15 && pixels[i + 2] < 15) red++;
            if (pixels[i + 1] > 240 && pixels[i] < 15 && pixels[i + 2] < 15) green++;
          }
          return red > 100 && green > 100;
        }, image.toString("base64"));
      }, { timeout: 30000 }).toBe(true);
      assert.equal(await page.locator('[aria-label*="이미지 미리보기 실패"]').count(), 0);
      await tap(page, "모두 지우기");
      await page.waitForFunction(() => document.querySelector('[aria-label="추가한 파일 (0)"]'));
      await page.evaluate(() => dispatchEvent(new Event("pagehide")));
      await page.waitForFunction(() => document.querySelector('[data-doroti-worker-runtime="disposed"]'));
      assert.deepEqual(errors, []);
      assert.ok(fonts.length > 0, "Font selection must exercise actual font resources.");
      assert.ok(fonts.every(font => font.status === 200));
      const result = { mode, browser: browser.version(), result: "PASS", fonts,
        picker: controlledPicker ? "controlled input; native dialog not qualified" : "browser filechooser interception",
        scenarios: ["deep Cupertino mount", "counter", "Korean DOM text commit and retained tab state", "font selection", "blur/scroll/slider/resize", "text/PNG file decode and view return; rendered red/green PNG pixels", "file clear and owner shutdown"],
        limitations: "Headless software GPU; no physical IME, screen-reader, scanout, sustained device or Safari acceptance." };
      results.push(result); console.log(JSON.stringify(result));
    } catch (error) {
      console.error(JSON.stringify({ mode, errors, labels: await page.locator("[aria-label]").evaluateAll(nodes => nodes.map(node => [node.getAttribute("role"), node.getAttribute("aria-label")])) }, null, 2));
      throw error;
    } finally { await context.close(); }
  }
} finally { await browser.close(); await fixture.close(); }
