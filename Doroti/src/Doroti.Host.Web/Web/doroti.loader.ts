export type DorotiBootstrapStage = "before-start" | "starting" | "started" | "failed";

export type DorotiBootResourceLoader = (
  type: string,
  name: string,
  defaultUri: string,
  integrity: string,
) => string | Response | Promise<Response> | null | undefined;

export interface DorotiBlazorStartOptions {
  configureRuntime?: (runtime: unknown) => void;
  loadBootResource?: DorotiBootResourceLoader;
  [name: string]: unknown;
}

export interface DorotiBootstrapContext {
  readonly blazorOptions: DorotiBlazorStartOptions;
  stage: DorotiBootstrapStage;
}

export interface DorotiBootstrapOptions {
  configure?: (context: DorotiBootstrapContext) => void;
  onStage?: (stage: DorotiBootstrapStage, context: DorotiBootstrapContext) => void;
  onError?: (error: unknown, context: DorotiBootstrapContext) => void;
}

interface BlazorGlobal {
  start(options?: DorotiBlazorStartOptions): Promise<void>;
}

let startPromise: Promise<DorotiBootstrapContext> | undefined;

export function startDoroti(options: DorotiBootstrapOptions = {}): Promise<DorotiBootstrapContext> {
  startPromise ??= runStart(options);
  return startPromise;
}

async function runStart(options: DorotiBootstrapOptions): Promise<DorotiBootstrapContext> {
  const context: DorotiBootstrapContext = {
    blazorOptions: {},
    stage: "before-start",
  };

  try {
    options.configure?.(context);
    notifyStage("before-start", context, options);

    const blazor = (globalThis as typeof globalThis & { Blazor?: BlazorGlobal }).Blazor;
    if (!blazor || typeof blazor.start !== "function") {
      throw new Error("DOROTIWEB020: Blazor.start is unavailable; load blazor.webassembly.js with autostart=false before doroti_bootstrap.js.");
    }

    notifyStage("starting", context, options);
    await blazor.start(context.blazorOptions);
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
