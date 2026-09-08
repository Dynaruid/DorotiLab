export type DorotiBootstrapStage = "before-start" | "starting" | "started" | "failed";

export interface DorotiBootstrapContext {
  stage: DorotiBootstrapStage;
  /** Main-owned threaded runtime with a JS-affine render Worker. */
  runtimeLocation?: "main" | "worker";
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
    context.rendererMode = selectRendererMode();
    document.documentElement.dataset.dorotiRenderer = context.rendererMode;
    const module = await import("./doroti.web.js");
    await module.startDorotiWorkerHost(context.rendererMode, context.runtimeLocation);
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

function selectRendererMode(): "worker-direct-webgl" | "worker-direct-webgpu" {
  const value = new URLSearchParams(globalThis.location.search).get("dorotiRenderer");
  if (value === "worker-direct-webgpu" ||
      value === "worker-direct-webgl")
    return value;
  // An omitted, auto, or unrecognized selection uses the product default.
  return "worker-direct-webgpu";
}

function notifyStage(
  stage: DorotiBootstrapStage,
  context: DorotiBootstrapContext,
  options: DorotiBootstrapOptions,
): void {
  context.stage = stage;
  options.onStage?.(stage, context);
}
