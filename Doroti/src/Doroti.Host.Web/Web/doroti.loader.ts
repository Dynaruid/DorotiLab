import { selectRendererPolicy } from "./doroti.web.policy.js";
import type { RendererPolicy } from "./doroti.web.policy.js";
export type DorotiBootstrapStage = "before-start" | "starting" | "started" | "failed";

export interface DorotiBootstrapContext {
  stage: DorotiBootstrapStage;
  /** Main-owned threaded runtime with a JS-affine render Worker. */
  runtimeLocation?: "main" | "worker";
  rendererPolicy?: RendererPolicy;
  rendererMode?: "worker-direct-webgl" | "worker-direct-webgpu";
}

export interface DorotiBootstrapOptions {
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
  const context: DorotiBootstrapContext = {
    stage: "before-start",
  };

  try {
    options.configure?.(context);
    notifyStage("before-start", context, options);

    notifyStage("starting", context, options);
    context.rendererPolicy = selectRendererPolicy(globalThis.location.search, navigator);
    context.rendererMode = context.rendererPolicy.selected;
    document.documentElement.dataset.dorotiRenderer = context.rendererMode;
    const module = await import("./doroti.web.js");
    await module.startDorotiWorkerHost(context.rendererMode, context.runtimeLocation, context.rendererPolicy);
    notifyStage("started", context, options);
    return context;
  } catch (error: unknown) {
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
