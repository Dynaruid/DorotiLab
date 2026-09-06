/// <reference types="doroti-loader" />

import {
  startDoroti,
  type DorotiBootstrapContext,
} from "./_content/Doroti.Host.Web/doroti.loader.js";

await startDoroti({
  configure(context: DorotiBootstrapContext) {
    document.documentElement.dataset.dorotiBootstrapConfigured = "true";
    context.blazorOptions.loadBootResource = (_type, _name, _defaultUri, integrity) => {
      document.documentElement.dataset.dorotiBootIntegrityObserved = integrity ? "true" : "false";
      return undefined;
    };
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
