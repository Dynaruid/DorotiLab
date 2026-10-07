/// <reference types="doroti-loader" />
import { startDoroti } from "./_content/Doroti.Host.Web/doroti.loader.js";

await startDoroti({
  splash: true, // Set to false to disable the built-in loading screen.
  configure(context) { context.runtimeLocation = "main"; },
  onStage(stage) { document.documentElement.dataset.dorotiBootstrapStage = stage; },
  onError(error) { document.documentElement.dataset.dorotiBootstrapError = String(error); },
});
