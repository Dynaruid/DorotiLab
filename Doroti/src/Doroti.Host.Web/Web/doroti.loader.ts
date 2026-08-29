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

const blazorWebAssemblyScriptPath = "_framework/blazor.webassembly.js";
const blazorWebAssemblyPreloadId = "doroti-blazor-loader";

let startPromise: Promise<DorotiBootstrapContext> | undefined;
let blazorLoaderPromise: Promise<void> | undefined;

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

    const blazor = await ensureBlazorWebAssembly();

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

async function ensureBlazorWebAssembly(): Promise<BlazorGlobal> {
  const existing = getBlazorGlobal();
  if (existing) return existing;

  blazorLoaderPromise ??= loadBlazorWebAssemblyScript();
  await blazorLoaderPromise;

  const loaded = getBlazorGlobal();
  if (!loaded) {
    throw new Error(
      `DOROTIWEB020: Blazor.start is unavailable after loading '${blazorWebAssemblyScriptPath}'.`,
    );
  }
  return loaded;
}

function getBlazorGlobal(): BlazorGlobal | undefined {
  const blazor = (globalThis as typeof globalThis & { Blazor?: BlazorGlobal }).Blazor;
  return blazor && typeof blazor.start === "function" ? blazor : undefined;
}

function loadBlazorWebAssemblyScript(): Promise<void> {
  const preload = document.getElementById(blazorWebAssemblyPreloadId) as HTMLLinkElement | null;
  const scriptUrl = preload?.href || new URL(blazorWebAssemblyScriptPath, document.baseURI).href;

  return new Promise<void>((resolve, reject) => {
    const script = document.createElement("script");
    script.src = scriptUrl;
    script.async = true;
    if (preload?.crossOrigin) script.crossOrigin = preload.crossOrigin;
    if (preload?.integrity) script.integrity = preload.integrity;
    if (preload?.referrerPolicy) script.referrerPolicy = preload.referrerPolicy;
    script.setAttribute("autostart", "false");
    script.dataset.dorotiBlazorLoader = "true";
    script.addEventListener("load", () => resolve(), { once: true });
    script.addEventListener(
      "error",
      () => reject(new Error(`DOROTIWEB024: Failed to load Blazor WebAssembly bootstrap from '${scriptUrl}'.`)),
      { once: true },
    );
    document.head.append(script);
  });
}

function notifyStage(
  stage: DorotiBootstrapStage,
  context: DorotiBootstrapContext,
  options: DorotiBootstrapOptions,
): void {
  context.stage = stage;
  options.onStage?.(stage, context);
}
