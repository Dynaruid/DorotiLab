/// <reference path="../doroti-textures/index.d.ts" />
declare module "*_content/Doroti.Host.Web/doroti.loader.js" {
  export type DorotiBootstrapStage = "before-start" | "starting" | "started" | "failed";
  export type ExecutionMode = "auto" | "threaded" | "compatible";
  export interface ExecutionSelection {
    requested: ExecutionMode;
    selected: "threaded" | "compatible";
    reason: string;
    runtimeLocation: "main" | "worker";
    runtimeContract: "mono-10" | "mono-11";
    runtimeVersion?: string;
    buildId?: string;
    loaderUrl?: string;
    profileUrl: string;
    applicationBaseUrl: string;
    renderers: readonly ("worker-direct-webgl" | "worker-direct-webgpu")[];
  }

  export interface DorotiBootstrapContext {
    stage: DorotiBootstrapStage;
    runtimeLocation?: "main" | "worker";
    executionMode?: ExecutionMode;
    execution?: ExecutionSelection;
    rendererPolicy?: {
      requested: "auto" | "worker-direct-webgl" | "worker-direct-webgpu";
      selected: "worker-direct-webgl" | "worker-direct-webgpu";
      reason: string;
      fallbackReason: string | null;
      memoryProfile: "mobile" | "desktop";
    };
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

  export function startDoroti(options?: DorotiBootstrapOptions): Promise<DorotiBootstrapContext>;
}
