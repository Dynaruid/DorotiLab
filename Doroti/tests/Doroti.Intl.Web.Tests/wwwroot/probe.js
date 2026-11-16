import { dotnet } from "./_framework/dotnet.js";

const state = globalThis.__intlProbe = { status: "loading", errors: [], requests: [] };
const start = performance.now();
const opportunity = () => new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));
function fail(error) {
  state.errors.push(String(error));
  state.status = "failed";
  document.querySelector("#status").textContent = String(error);
}
try {
  // The baseline and adapter both start in English, regardless of browser UI language.
  // Some native ICU aborts leave .NET 10's create promise pending. Keep the probe's failure
  // observable and bounded too; the outer command has its separate 1200-second tree timeout.
  let startupTimer;
  const deadline = new Promise((_, reject) => {
    startupTimer = setTimeout(() => reject(new Error(".NET globalization runtime initialization did not complete within 30 seconds; inspect ICU loading errors.")), 30000);
  });
  const runtime = await Promise.race([dotnet.withApplicationCulture("en-US").create(), deadline])
    .finally(() => clearTimeout(startupTimer));
  state.runtime = runtime.runtimeBuildInfo;
  state.icuResources = runtime.getConfig().resources.icu ?? [];
  const exports = await runtime.getAssemblyExports(runtime.getConfig().mainAssemblyName);
  const probe = exports.Doroti.Intl.Web.Tests.Probe;
  const firstFormat = performance.now();
  document.querySelector("#date").textContent = await probe.Format("en-US");
  state.firstEnglishFormatRoundTripMs = performance.now() - firstFormat;
  await opportunity();
  state.englishDomPaintOpportunityMs = performance.now() - start;
  state.status = "english";
  document.querySelector("#status").textContent = "english";
  document.querySelector("#korean").disabled = false;
  document.querySelector("#korean").addEventListener("click", async () => {
    try {
      const transition = performance.now();
      const text = await probe.Format("ko-KR");
      state.firstKoreanFormatRoundTripMs = performance.now() - transition;
      document.querySelector("#date").textContent = text;
      document.documentElement.lang = "ko-KR";
      await opportunity();
      state.koreanDomPaintOpportunityMs = performance.now() - transition;
      state.contract = JSON.parse(await probe.Contract());
      state.parallelTasks = await probe.Concurrent();
      state.status = "passed";
      document.querySelector("#status").textContent = "passed";
    } catch (error) { fail(error); }
  });
} catch (error) { fail(error); }
