import type { RuntimeContract } from "./doroti.web.execution.js";

export interface MonoWorkerConfig {
  resources?: { jsModuleWorker?: { name: string; resolvedUrl?: string }[] };
}
export function monoWorkerUrls(contract: RuntimeContract, loaderUrl: string, config: MonoWorkerConfig): ReadonlySet<string> {
  const loader = new URL(loaderUrl);
  const urls = contract === "mono-11" ? [loader.href] :
    config.resources?.jsModuleWorker?.map(asset => new URL(asset.resolvedUrl ?? asset.name, loader).href) ?? [];
  if (urls.length !== 1 || new URL(urls[0]!).origin !== loader.origin)
    throw new Error("DOROTIWEB040: Expected one same-origin Mono pthread entry from the selected runtime metadata.");
  return new Set(urls);
}
