/// <reference types="doroti-loader" />

import {
  startDoroti,
  type DorotiBootstrapContext,
} from "./_content/Doroti.Host.Web/doroti.loader.js";

const runtimeLocation = new URL(location.href).searchParams.get("dorotiRuntimeLocation");

await startDoroti({
  splash: true, // Set to false to disable the built-in loading screen.
  configure(context: DorotiBootstrapContext) {
    if (runtimeLocation === "main" || runtimeLocation === "worker") context.runtimeLocation = runtimeLocation;
    document.documentElement.dataset.dorotiBootstrapConfigured = "true";
  },
  onStage(stage) {
    document.documentElement.dataset.dorotiBootstrapStage = stage;
    const history = document.documentElement.dataset.dorotiBootstrapStages;
    document.documentElement.dataset.dorotiBootstrapStages = history ? `${history},${stage}` : stage;
  },
  onError(error) {
    document.documentElement.dataset.dorotiBootstrapError = String(error);
  },
});
