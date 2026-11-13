export type ExecutionMode = "auto" | "threaded" | "compatible";
export type RuntimeContract = "mono-10" | "mono-11";
export const executionHostContract = "doroti.web.execution/v1";

export interface RuntimeVariant {
  threads: boolean;
  loaderUrl: string;
  profileUrl: string;
  renderers: ("worker-direct-webgl" | "worker-direct-webgpu")[];
}
export interface RuntimeProfile {
  schemaVersion: 1;
  threads: boolean;
  buildId?: string;
  runtimeContract?: RuntimeContract;
  runtimeVersion?: string;
}
export interface ExecutionManifest {
  schemaVersion: 2;
  hostContract: typeof executionHostContract;
  buildId: string;
  runtimeContract: RuntimeContract;
  runtimeVersion: string;
  variants: Partial<Record<Exclude<ExecutionMode, "auto">, RuntimeVariant>>;
}
export interface ExecutionSelection {
  requested: ExecutionMode;
  selected: Exclude<ExecutionMode, "auto">;
  reason: string;
  runtimeLocation: "main" | "worker";
  runtimeContract: RuntimeContract;
  runtimeVersion?: string;
  buildId?: string;
  loaderUrl?: string;
  profileUrl: string;
  applicationBaseUrl: string;
  renderers: readonly ("worker-direct-webgl" | "worker-direct-webgpu")[];
}
export interface ExecutionCapabilities {
  secureContext: boolean;
  isolated: boolean;
  sharedMemory: boolean;
}

function record(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}
function contract(value: unknown): value is RuntimeContract { return value === "mono-10" || value === "mono-11"; }
function versionMatches(version: unknown, runtime: RuntimeContract): version is string {
  return typeof version === "string" && new RegExp(`^${runtime === "mono-10" ? 10 : 11}\\.\\d+\\.\\d+(?:[-+].*)?$`).test(version);
}
export function executionMode(value: unknown): ExecutionMode {
  if (value !== "auto" && value !== "threaded" && value !== "compatible")
    throw new Error(`DOROTIWEB030: Invalid execution mode '${String(value)}'.`);
  return value;
}
export function parseExecutionManifest(value: unknown): RuntimeProfile | ExecutionManifest {
  if (!record(value))
    throw new Error("DOROTIWEB031: Invalid runtime profile.");
  if (value.schemaVersion === 1) {
    const { threads, runtimeContract, runtimeVersion, buildId } = value;
    if (typeof threads !== "boolean") throw new Error("DOROTIWEB031: Invalid runtime thread profile.");
    if (runtimeContract !== undefined && (!contract(runtimeContract) || !versionMatches(runtimeVersion, runtimeContract)) ||
        runtimeVersion !== undefined && !versionMatches(runtimeVersion, runtimeContract ?? "mono-10"))
      throw new Error("DOROTIWEB032: Unsupported Mono runtime contract/version (CoreCLR is not supported).");
    if (buildId !== undefined && (typeof buildId !== "string" || !buildId))
      throw new Error("DOROTIWEB031: Invalid build ID.");
    return { schemaVersion: 1, threads, buildId, runtimeContract, runtimeVersion };
  }
  if (value.schemaVersion !== 2 || value.hostContract !== executionHostContract || typeof value.buildId !== "string" || !value.buildId ||
      !contract(value.runtimeContract) || !versionMatches(value.runtimeVersion, value.runtimeContract) || !record(value.variants))
    throw new Error("DOROTIWEB031: Unsupported execution manifest schema/host/runtime contract.");
  const variants: ExecutionManifest["variants"] = {};
  for (const [name, variant] of Object.entries(value.variants)) {
    if (name !== "threaded" && name !== "compatible" || !record(variant) || variant.threads !== (name === "threaded") ||
        typeof variant.loaderUrl !== "string" || !variant.loaderUrl || typeof variant.profileUrl !== "string" || !variant.profileUrl ||
        !Array.isArray(variant.renderers) || !variant.renderers.includes("worker-direct-webgl"))
      throw new Error("DOROTIWEB031: Invalid execution variant.");
    const renderers: RuntimeVariant["renderers"] = [];
    for (const renderer of variant.renderers) {
      if (renderer !== "worker-direct-webgl" && renderer !== "worker-direct-webgpu" || name === "compatible" && renderer !== "worker-direct-webgl")
        throw new Error("DOROTIWEB031: Unsupported execution renderer contract.");
      renderers.push(renderer);
    }
    variants[name] = { threads: variant.threads, loaderUrl: variant.loaderUrl, profileUrl: variant.profileUrl, renderers };
  }
  if (!Object.keys(variants).length) throw new Error("DOROTIWEB031: No execution variants were published.");
  return { schemaVersion: 2, hostContract: executionHostContract, buildId: value.buildId,
    runtimeContract: value.runtimeContract, runtimeVersion: value.runtimeVersion, variants };
}
export function sameOriginRuntimeUrl(relative: string, base: string): string {
  const url = new URL(relative, base);
  if (url.origin !== new URL(base).origin || !["https:", "http:"].includes(url.protocol) || url.username || url.password || url.hash)
    throw new Error("DOROTIWEB033: Runtime assets must use same-origin HTTP(S) URLs without credentials or fragments.");
  return url.href;
}
export function selectExecution(manifest: RuntimeProfile | ExecutionManifest, requested: ExecutionMode,
  capabilities: ExecutionCapabilities, applicationBaseUrl: string, legacyLocation?: "main" | "worker"): ExecutionSelection {
  requested = executionMode(requested);
  if (legacyLocation !== undefined && legacyLocation !== "main" && legacyLocation !== "worker")
    throw new Error("DOROTIWEB034: Invalid legacy runtimeLocation.");
  const legacyMode = legacyLocation === undefined ? undefined : legacyLocation === "main" ? "threaded" : "compatible";
  if (legacyMode && requested !== "auto" && requested !== legacyMode)
    throw new Error("DOROTIWEB034: executionMode conflicts with legacy runtimeLocation.");
  const effective = legacyMode ?? requested;
  const shared = capabilities.secureContext && capabilities.isolated && capabilities.sharedMemory;
  if (manifest.schemaVersion === 2) for (const variant of Object.values(manifest.variants)) {
    sameOriginRuntimeUrl(variant.loaderUrl, applicationBaseUrl);
    sameOriginRuntimeUrl(variant.profileUrl, applicationBaseUrl);
  }
  const available = manifest.schemaVersion === 1 ? { [manifest.threads ? "threaded" : "compatible"]: true } : manifest.variants;
  const selected = effective === "auto" ? shared && available.threaded ? "threaded" : "compatible" : effective;
  if (selected === "threaded" && !shared)
    throw new Error("DOROTIWEB035: Threaded execution requires a secure, cross-origin-isolated page and shared WebAssembly memory. Publish auto or compatible for this host.");
  if (!available[selected])
    throw new Error(`DOROTIWEB036: '${selected}' was not published. Deploy an auto/compatible build for non-isolated hosting.`);
  const variant = manifest.schemaVersion === 2 ? manifest.variants[selected]! : undefined;
  return { requested, selected, reason: legacyMode ? "legacy-runtime-location" : effective !== "auto" ? "explicit-execution-mode" :
    selected === "threaded" ? "shared-memory-available" : !available.threaded ? "compatible-only-deployment" : "shared-memory-unavailable",
    runtimeLocation: selected === "threaded" ? "main" : "worker", runtimeContract: manifest.runtimeContract ?? "mono-10",
    runtimeVersion: manifest.runtimeVersion, buildId: manifest.buildId,
    renderers: variant?.renderers ?? (selected === "threaded" ? ["worker-direct-webgl", "worker-direct-webgpu"] : ["worker-direct-webgl"]),
    applicationBaseUrl, loaderUrl: variant ? sameOriginRuntimeUrl(variant.loaderUrl, applicationBaseUrl) : undefined,
    profileUrl: sameOriginRuntimeUrl(variant?.profileUrl ?? "./doroti.runtime-profile.json", applicationBaseUrl) };
}
export function verifyVariantProfile(value: unknown, selection: ExecutionSelection): void {
  const profile = parseExecutionManifest(value);
  if (profile.schemaVersion !== 1 || profile.threads !== (selection.selected === "threaded") ||
      profile.buildId !== selection.buildId || profile.runtimeContract !== selection.runtimeContract || profile.runtimeVersion !== selection.runtimeVersion)
    throw new Error("DOROTIWEB037: Selected runtime profile does not match the deployment generation/runtime/threads.");
}
export function verifyRuntime(runtime: { runtimeBuildInfo: { wasmEnableThreads: boolean; productVersion: string }; localHeapViewU8(): Uint8Array },
  selection: Pick<ExecutionSelection, "selected" | "runtimeContract" | "runtimeVersion">): { threads: boolean; sharedHeap: boolean; version: string } {
  const info = runtime.runtimeBuildInfo;
  const sharedHeap = typeof SharedArrayBuffer !== "undefined" && runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer;
  if (info.wasmEnableThreads !== (selection.selected === "threaded") || sharedHeap !== info.wasmEnableThreads ||
      !versionMatches(info.productVersion, selection.runtimeContract) || selection.runtimeVersion !== undefined && info.productVersion !== selection.runtimeVersion)
    throw new Error("DOROTIWEB038: Loaded runtime version/threads/heap does not match the selected profile.");
  return { threads: info.wasmEnableThreads, sharedHeap, version: info.productVersion };
}
export function probeExecutionCapabilities(): ExecutionCapabilities {
  let sharedMemory = false;
  if (globalThis.isSecureContext && globalThis.crossOriginIsolated && typeof SharedArrayBuffer !== "undefined") {
    try { sharedMemory = new WebAssembly.Memory({ initial: 1, maximum: 1, shared: true }).buffer instanceof SharedArrayBuffer; }
    catch (error) { console.info("Doroti shared-memory capability unavailable.", error); }
  }
  return { secureContext: globalThis.isSecureContext, isolated: globalThis.crossOriginIsolated, sharedMemory };
}
export async function loadExecutionSelection(requested: ExecutionMode, legacyLocation?: "main" | "worker"): Promise<ExecutionSelection> {
  const base = new URL("./", document.baseURI).href;
  const read = async (url: string): Promise<unknown> => {
    const response = await fetch(url, { cache: "no-store", signal: AbortSignal.timeout(30000) });
    if (!response.ok || response.redirected) throw new Error(`DOROTIWEB039: Runtime profile fetch failed (${response.status}): ${url}`);
    return response.json();
  };
  const manifest = parseExecutionManifest(await read(new URL("doroti.runtime-profile.json", base).href));
  const selection = selectExecution(manifest, requested, probeExecutionCapabilities(), base, legacyLocation);
  if (manifest.schemaVersion === 2) verifyVariantProfile(await read(selection.profileUrl), selection);
  return selection;
}
