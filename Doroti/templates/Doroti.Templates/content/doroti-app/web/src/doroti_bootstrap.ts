/// <reference types="doroti-loader" />

import {
  startDoroti,
  type DorotiBootstrapContext,
} from "./_content/Doroti.Host.Web/doroti.loader.js";

await startDoroti({
  configure(_context: DorotiBootstrapContext) {
    // Configure context.blazorOptions before Blazor starts.
  },
  onStage(stage) {
    document.documentElement.dataset.dorotiBootstrapStage = stage;
    const history = document.documentElement.dataset.dorotiBootstrapStages;
    document.documentElement.dataset.dorotiBootstrapStages = history ? `${history},${stage}` : stage;
  },
  onError(error) {
    const app = document.getElementById("app");
    if (app) app.textContent = `Doroti failed to start: ${String(error)}`;
  },
});
