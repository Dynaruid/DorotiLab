/// <reference types="doroti-loader" />

import {
  startDoroti,
  type DorotiBootstrapContext,
} from "./_content/Doroti.Host.Web/doroti.loader.js";

await startDoroti({
  configure(context: DorotiBootstrapContext) {
    context.runtimeLocation = "main";
    document.documentElement.dataset.dorotiBootstrapConfigured = "true";
  },
  onStage(stage) {
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
