import assert from "node:assert/strict";

// Exercise the actual TypeScript build output without requiring a GPU or a vibrating device.
const moduleUrl = new URL("../../src/Doroti.Host.Web/obj/Release/net10.0/Doroti.Web/wwwroot/doroti.web.js", import.meta.url);
const bridge = await import(moduleUrl.href);
const pulses = [];
Object.defineProperty(globalThis, "navigator", {
  configurable: true,
  value: { vibrate: duration => { pulses.push(duration); return true; } },
});
for (const duration of [50, 10, 20, 30, 10, 20, 20, 30]) await bridge.vibrate(duration);
assert.deepEqual(pulses, [50, 10, 20, 30, 10, 20, 20, 30]);
navigator.vibrate = () => false;
await bridge.vibrate(10); // Browser denied activation / device support.
delete navigator.vibrate;
await bridge.vibrate(10); // API absent, including browsers without Vibration support.

const workerBridge = await import(`${moduleUrl.href}?worker`);
const requests = [];
let acknowledge;
workerBridge.configureWorkerBridge({
  requestControl: (kind, payload) => {
    requests.push({ kind, payload });
    return new Promise(resolve => { acknowledge = resolve; });
  },
});
let complete = false;
const pending = workerBridge.vibrate(30).then(() => { complete = true; });
await Promise.resolve();
assert.equal(complete, false);
assert.deepEqual(requests, [{ kind: "haptic-feedback", payload: { durationMilliseconds: 30 } }]);
acknowledge("");
await pending;
assert.equal(complete, true);
assert.equal(pulses.length, 8, "worker must not attempt navigator.vibrate directly");
console.log("PASS: emitted web bridge dispatch, unsupported/denied browser and awaited Worker control request.");
