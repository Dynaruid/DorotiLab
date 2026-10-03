/// <reference types="doroti-loader" />

import {
  startDoroti,
  type DorotiBootstrapContext,
} from "./_content/Doroti.Host.Web/doroti.loader.js";

// Navigation can replace the URL during managed startup. Keep the selected
// runtime owner independent of those application route changes.
const runtimeLocation = new URL(location.href).searchParams.get("dorotiRuntimeLocation") === "worker" ? "worker" : "main";

await startDoroti({
  configure(context: DorotiBootstrapContext) {
    context.runtimeLocation = runtimeLocation;
    document.documentElement.dataset.dorotiBootstrapConfigured = "true";
  },
  onStage(stage) {
    if (stage === "started" && runtimeLocation === "main") {
      const runtime = (globalThis as unknown as { getDotnetRuntime(id: number): { getAssemblyExports(name: string): Promise<{ DorotiTestbedApp: { Web: { Validation: { WebTextureExport: { Initialize(): void } } } } }> } }).getDotnetRuntime(0);
      void runtime.getAssemblyExports("DorotiTestbedApp.Web.dll").then(exports => exports.DorotiTestbedApp.Web.Validation.WebTextureExport.Initialize());
    }
    document.documentElement.dataset.dorotiBootstrapStage = stage;
    const history = document.documentElement.dataset.dorotiBootstrapStages;
    document.documentElement.dataset.dorotiBootstrapStages = history ? `${history},${stage}` : stage;
  },
  onError(error) {
    document.documentElement.dataset.dorotiBootstrapError = String(error);
    const app = document.getElementById("app");
    if (app) app.textContent = `Doroti failed to start: ${String(error)}`;
  },
});
