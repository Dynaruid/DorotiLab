import { chromium } from "../web-playwright/node_modules/playwright/index.mjs";
import { createServer } from "node:http";
import { readFile, writeFile, mkdir } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";

const here = path.dirname(fileURLToPath(import.meta.url));
const output = path.resolve(here, "../../artifacts/validation/pointer-device-kind");
const bundle = path.join(output, "build/obj/Doroti.Host.Web/debug/net10.0/Doroti.Web/wwwroot");
const server = createServer(async (request, response) => {
  try {
    if (/^\/[\w.-]+\.js$/.test(request.url)) {
      response.setHeader("content-type", "text/javascript");
      let code = await readFile(path.join(bundle, request.url.slice(1)), "utf8");
      // Admission-only fixture: skip GPU bootstrap, retain production DOM handlers.
      // No rendering or managed-WASM qualification is claimed by this test.
      if (request.url === "/doroti.web.js") code += "\ndirectWorkerBootstrap = true;\n";
      response.end(code);
    } else {
      response.setHeader("content-type", "text/html");
      response.end('<!doctype html><div class="doroti-root" style="width:200px;height:200px"><canvas id="canvas" tabindex="0"></canvas><textarea id="doroti-ime" hidden></textarea><div id="doroti-semantics"></div></div>');
    }
  } catch (error) { response.statusCode = 500; response.end(String(error)); }
});
await new Promise(resolve => server.listen(0, "127.0.0.1", resolve));
let browser;
try {
  browser = await chromium.launch({ headless: true });
  const page = await browser.newPage();
  await page.goto(`http://127.0.0.1:${server.address().port}/`);
  const result = await page.evaluate(async () => {
    const module = await import("/doroti.web.js");
    const packets = [];
    const signals = [];
    const callbacks = Object.fromEntries([
      "dispatchAnimationFrame", "dispatchSnapshot", "dispatchResizeEpoch", "dispatchPointerBatch", "dispatchWheel",
      "dispatchKey", "dispatchFocus", "dispatchTextEditing", "dispatchTextAction", "dispatchTextConnectionClosed", "dispatchSemanticsAction",
    ].map(name => [name, () => {}]));
    callbacks.dispatchPointerBatch = (...args) => packets.push(args);
    callbacks.dispatchWheel = (...args) => signals.push(args);
    module.configureManagedCallbacks(callbacks);
    module.createHost(1, "canvas", 200, 200);
    const root = document.querySelector(".doroti-root");
    for (const [pointerType, expected] of [["mouse", 0], ["touch", 1], ["pen", 2], ["", 4], ["future-device", 4]]) {
      for (const [event, phase] of [["pointerenter", 5], ["pointermove", 4], ["pointerleave", 6]]) {
        root.dispatchEvent(new PointerEvent(event, { pointerType, pointerId: 7, clientX: 40, clientY: 60 }));
        const packet = packets.at(-1);
        if (packet?.[1] !== phase || packet?.[2] !== expected)
          throw new Error(`${pointerType}/${event}: ${JSON.stringify(packet)}`);
      }
    }
    Object.defineProperty(navigator, "userAgentData", { configurable: true, value: { platform: "Windows" } });
    root.dispatchEvent(new WheelEvent("wheel", { deltaY: -20, ctrlKey: true, clientX: 40, clientY: 60 }));
    if (signals.at(-1)[8] !== 3 || Math.abs(signals.at(-1)[9] - Math.exp(.1)) > 1e-9)
      throw new Error("Browser pinch was not converted to an exponential scale signal");
    root.dispatchEvent(new WheelEvent("wheel", { deltaY: 20, clientX: 40, clientY: 60 }));
    if (signals.at(-1)[8] !== 1) throw new Error("Ordinary scrolling became scale");
    Object.defineProperty(navigator, "userAgentData", { configurable: true, value: { platform: "macOS" } });
    document.querySelector("#canvas").focus();
    document.dispatchEvent(new KeyboardEvent("keydown", { code: "ControlLeft", key: "Control", ctrlKey: true }));
    root.dispatchEvent(new WheelEvent("wheel", { deltaY: 20, ctrlKey: true }));
    if (signals.at(-1)[8] !== 1) throw new Error("macOS physical Control-wheel must remain scrolling");
    document.dispatchEvent(new KeyboardEvent("keyup", { code: "ControlLeft", key: "Control" }));
    module.dispatchWorkerInput({ hostId: 1, inputKind: "wheel", inputSequence: 101,
      payload: { x: 40, y: 60, deltaX: 0, deltaY: -20, timestamp: 100, kind: 3, signalKind: 3, scale: 1.1 } });
    if (signals.at(-1)[8] !== 3 || signals.at(-1)[9] !== 1.1)
      throw new Error("Worker transport lost the pinch scale");
    module.closeHost(1);
    return { status: "PASS", packets: packets.length, signals: signals.length,
      scope: "compiled production DOM pointer handlers; mocked managed callbacks and GPU admission" };
  });
  await mkdir(output, { recursive: true });
  await writeFile(path.join(output, "browser.json"), JSON.stringify(result, null, 2));
  console.log(JSON.stringify(result));
} finally {
  if (browser) await browser.close();
  await new Promise(resolve => server.close(resolve));
}
