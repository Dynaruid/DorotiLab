import { selectRendererPolicy, resolveRendererPolicy } from "./doroti.web.policy.js";
import type { RendererPolicy } from "./doroti.web.policy.js";
import { attachDorotiSplash } from "./doroti.web.splash.js";
import { executionMode, loadExecutionSelection } from "./doroti.web.execution.js";
import type { ExecutionMode, ExecutionSelection } from "./doroti.web.execution.js";
export type { ExecutionMode, ExecutionSelection } from "./doroti.web.execution.js";
export type DorotiBootstrapStage = "before-start" | "starting" | "started" | "failed";

export interface DorotiBootstrapContext {
  stage: DorotiBootstrapStage;
  /** Legacy ownership override. Prefer executionMode. */
  runtimeLocation?: "main" | "worker";
  executionMode?: ExecutionMode;
  execution?: ExecutionSelection;
  rendererPolicy?: RendererPolicy;
  rendererMode?: "worker-direct-webgl" | "worker-direct-webgpu";
}

export interface DorotiBootstrapOptions {
  /** Show the built-in splash until the first frame. Defaults to true. */
  splash?: boolean;
  executionMode?: ExecutionMode;
  configure?: (context: DorotiBootstrapContext) => void;
  onStage?: (stage: DorotiBootstrapStage, context: DorotiBootstrapContext) => void;
  onError?: (error: unknown, context: DorotiBootstrapContext) => void;
}

let startPromise: Promise<DorotiBootstrapContext> | undefined;

export function startDoroti(options: DorotiBootstrapOptions = {}): Promise<DorotiBootstrapContext> {
  startPromise ??= runStart(options);
  return startPromise;
}

async function runStart(options: DorotiBootstrapOptions): Promise<DorotiBootstrapContext> {
  const splash = attachDorotiSplash(options.splash);
  const context: DorotiBootstrapContext = {
    stage: "before-start",
  };

  try {
    options.configure?.(context);
    notifyStage("before-start", context, options);

    notifyStage("starting", context, options);
    context.executionMode = executionMode(context.executionMode ?? options.executionMode ??
      new URLSearchParams(location.search).get("dorotiExecutionMode") ?? "auto");
    context.execution = await loadExecutionSelection(context.executionMode, context.runtimeLocation);
    context.runtimeLocation = context.execution.runtimeLocation;
    if (context.execution.selected === "compatible" && document.querySelector("script[src*='aspnetcore-browser-refresh']"))
      console.info("Doroti standalone Worker Hot Reload is not qualified; use rebuild and page reload.");
    Object.assign(document.documentElement.dataset, { dorotiExecutionRequested: context.execution.requested,
      dorotiExecutionMode: context.execution.selected, dorotiExecutionReason: context.execution.reason,
      dorotiRuntimeContract: context.execution.runtimeContract, dorotiBuildId: context.execution.buildId ?? "legacy" });
    context.rendererPolicy = await resolveRendererPolicy(
      selectRendererPolicy(globalThis.location.search, navigator), context.runtimeLocation ?? "main");
    context.rendererMode = context.rendererPolicy.selected;
    document.documentElement.dataset.dorotiRenderer = context.rendererMode;
    const module = await import("./doroti.web.js");
    await module.startDorotiWorkerHost(context.rendererMode, context.runtimeLocation, context.rendererPolicy, context.execution);
    notifyStage("started", context, options);
    splash.complete();
    return context;
  } catch (error: unknown) {
    splash.fail();
    context.stage = "failed";
    try {
      options.onStage?.("failed", context);
    } catch (callbackError: unknown) {
      console.error("DOROTIWEB021: Doroti failed-stage callback threw.", callbackError);
    }
    try {
      options.onError?.(error, context);
    } catch (callbackError: unknown) {
      console.error("DOROTIWEB022: Doroti error callback threw.", callbackError);
    }
    console.error("Doroti Web bootstrap failed.", error);
    throw error;
  }
}

function notifyStage(
  stage: DorotiBootstrapStage,
  context: DorotiBootstrapContext,
  options: DorotiBootstrapOptions,
): void {
  context.stage = stage;
  options.onStage?.(stage, context);
}
