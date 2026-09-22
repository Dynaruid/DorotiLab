/// <reference path="../doroti-textures/index.d.ts" />
declare module "*_content/Doroti.Host.Web/doroti.loader.js" {
  export type DorotiBootstrapStage = "before-start" | "starting" | "started" | "failed";

  export interface DorotiBootstrapContext {
    stage: DorotiBootstrapStage;
    runtimeLocation?: "main" | "worker";
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
    configure?: (context: DorotiBootstrapContext) => void;
    onStage?: (stage: DorotiBootstrapStage, context: DorotiBootstrapContext) => void;
    onError?: (error: unknown, context: DorotiBootstrapContext) => void;
  }

  export function startDoroti(options?: DorotiBootstrapOptions): Promise<DorotiBootstrapContext>;
}
