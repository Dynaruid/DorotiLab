import { dotnet } from "./_framework/dotnet.js";

const status = document.querySelector("#status");
try {
  const runtime = await dotnet.withApplicationCulture("en-US").create();
  const { wasmEnableThreads: threads, productVersion } = runtime.runtimeBuildInfo;
  const sharedHeap = typeof SharedArrayBuffer !== "undefined" && runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer;
  if (threads !== crossOriginIsolated || sharedHeap !== threads) {
    throw new Error(`Paragraph probe mode does not match the runtime: isolated=${crossOriginIsolated} threads=${threads} sharedHeap=${sharedHeap}`);
  }
  const exports = await runtime.getAssemblyExports(runtime.getConfig().mainAssemblyName);
  const names = ["Arabic", "Devanagari", "Thai", "Emoji"];
  if (exports.Probe.RunProduct) names.push("Lao", "Khmer", "Burmese");
  const fonts = await Promise.all(names.map(async name => {
    const response = await fetch(`fonts/${name}.ttf`);
    if (!response.ok) throw new Error(`Missing font ${name}: ${response.status}`);
    return new Uint8Array(await response.arrayBuffer());
  }));
  const run = exports.Probe.RunProduct ?? exports.Probe.Run;
  const [receipt, png] = (await run(...fonts)).trim().split("\n");
  document.querySelector("#result").src = "data:image/png;base64," + png;
  await document.querySelector("#result").decode();
  status.textContent = `${receipt} | isolated=${crossOriginIsolated} threads=${threads} sharedHeap=${sharedHeap} runtime=${productVersion}`;
  status.dataset.outcome = "pass";
} catch (error) {
  status.textContent = String(error);
  status.dataset.outcome = "fail";
}
