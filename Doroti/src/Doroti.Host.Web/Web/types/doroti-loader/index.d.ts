declare module "*_content/Doroti.Host.Web/doroti.loader.js" {
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
    rendererMode?: "worker-direct-webgl" | "offscreen-worker" | "offscreen-bitmap" | "document-webgl";
  }

  export interface DorotiBootstrapOptions {
    configure?: (context: DorotiBootstrapContext) => void;
    onStage?: (stage: DorotiBootstrapStage, context: DorotiBootstrapContext) => void;
    onError?: (error: unknown, context: DorotiBootstrapContext) => void;
  }

  export function startDoroti(options?: DorotiBootstrapOptions): Promise<DorotiBootstrapContext>;
}
