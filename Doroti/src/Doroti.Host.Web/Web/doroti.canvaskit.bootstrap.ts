// This file deliberately has no imports or exports. It is emitted as a classic
// Worker script so the upstream CanvasKit UMD artifact can be loaded unchanged
// with importScripts before the Doroti ESM role is imported dynamically.

type CanvasKitBootstrapRole = "ui" | "raster";

interface CanvasKitBootstrapEnvelope {
  readonly protocolVersion: number;
  readonly topologyVersion: number;
  readonly kind: "canvaskit-bootstrap-init";
  readonly role: CanvasKitBootstrapRole;
  readonly sessionId: number;
  readonly canvasKitJsUrl: string;
  readonly canvasKitWasmUrl: string;
  readonly roleModuleUrl: string;
  readonly [name: string]: unknown;
}

interface CanvasKitRoleModule {
  startCanvasKitRole(context: {
    CanvasKitInit: (options?: { locateFile?(file: string): string }) => Promise<unknown>;
    canvasKitWasmUrl: string;
    initEnvelope: CanvasKitBootstrapEnvelope;
  }): Promise<void>;
}

let canvasKitBootstrapStarted = false;

function bootstrapPost(kind: string, payload: Record<string, unknown> = {}): void {
  (globalThis as unknown as { postMessage(message: unknown): void }).postMessage({
    protocolVersion: 2,
    topologyVersion: 1,
    kind,
    ...payload,
  });
}

function requireApprovedAsset(
  value: unknown,
  label: string,
  expectedPath: string,
  allowSdkFingerprint = false,
): string {
  const url = new URL(String(value ?? ""), globalThis.location.href);
  if (url.origin !== globalThis.location.origin || (url.protocol !== "http:" && url.protocol !== "https:"))
    throw new Error(`Doroti CanvasKit ${label} must be a same-origin HTTP(S) URL.`);
  if (url.username || url.password || url.search || url.hash ||
      (url.pathname !== expectedPath &&
       (!allowSdkFingerprint || !isApprovedSdkFingerprint(expectedPath, url.pathname))))
    throw new Error(
      `Doroti CanvasKit ${label} must use the approved logical asset endpoint '${expectedPath}'.`);
  return url.href;
}

function isApprovedSdkFingerprint(logicalPath: string, candidatePath: string): boolean {
  const dot = logicalPath.lastIndexOf(".");
  if (dot < 0) return false;
  const stem = logicalPath.slice(0, dot);
  const extension = logicalPath.slice(dot);
  return candidatePath.startsWith(`${stem}.`) && candidatePath.endsWith(extension) &&
    /^[a-z0-9]{10}$/.test(candidatePath.slice(stem.length + 1, -extension.length));
}

function applicationBasePath(): string {
  const marker = "/_content/Doroti.Host.Web/";
  const index = globalThis.location.pathname.lastIndexOf(marker);
  if (index < 0)
    throw new Error("Doroti CanvasKit bootstrap is not hosted under the Doroti.Host.Web asset root.");
  return globalThis.location.pathname.slice(0, index);
}

globalThis.addEventListener("message", (event: MessageEvent) => {
  const message = event.data as Partial<CanvasKitBootstrapEnvelope> | null;
  if (!message || message.kind !== "canvaskit-bootstrap-init") return;
  if (canvasKitBootstrapStarted) {
    bootstrapPost("fatal", { role: message.role, error: "duplicate CanvasKit bootstrap init" });
    return;
  }
  canvasKitBootstrapStarted = true;
  void (async () => {
    if (message.protocolVersion !== 2 || message.topologyVersion !== 1)
      throw new Error("Doroti CanvasKit bootstrap received an unsupported control/topology version.");
    if (message.role !== "ui" && message.role !== "raster")
      throw new Error(`Doroti CanvasKit bootstrap received unknown role '${String(message.role)}'.`);
    if (!Number.isSafeInteger(message.sessionId) || Number(message.sessionId) <= 0)
      throw new Error("Doroti CanvasKit bootstrap requires a positive session id.");
    const basePath = applicationBasePath();
    const canvasKitBase = `${basePath}/_content/Doroti.Host.Web/canvaskit/0.42.0`;
    const canvasKitJsUrl = requireApprovedAsset(
      message.canvasKitJsUrl, "script", `${canvasKitBase}/canvaskit.js`);
    const canvasKitWasmUrl = requireApprovedAsset(
      message.canvasKitWasmUrl, "WASM", `${canvasKitBase}/canvaskit.wasm`);
    const roleFile = message.role === "ui" ? "doroti.ui.worker.js" : "doroti.canvaskit.worker.js";
    const roleModuleUrl = requireApprovedAsset(
      message.roleModuleUrl,
      "role module",
      `${basePath}/_content/Doroti.Host.Web/${roleFile}`,
      true);
    const started = performance.now();
    (globalThis as unknown as { importScripts(...urls: string[]): void }).importScripts(canvasKitJsUrl);
    const init = (globalThis as unknown as {
      CanvasKitInit?: (options?: { locateFile?(file: string): string }) => Promise<unknown>;
    }).CanvasKitInit;
    if (typeof init !== "function")
      throw new Error("CanvasKitInit was not installed by the pinned classic CanvasKit script.");
    const role = await import(roleModuleUrl) as CanvasKitRoleModule;
    if (typeof role.startCanvasKitRole !== "function")
      throw new Error(`Doroti CanvasKit ${message.role} role module has no startCanvasKitRole export.`);
    bootstrapPost("bootstrap-ready", {
      role: message.role,
      sessionId: message.sessionId,
      scriptLoadMicroseconds: Math.round((performance.now() - started) * 1000),
    });
    await role.startCanvasKitRole({
      CanvasKitInit: init,
      canvasKitWasmUrl,
      initEnvelope: { ...message, canvasKitJsUrl, canvasKitWasmUrl, roleModuleUrl } as CanvasKitBootstrapEnvelope,
    });
  })().catch((error: unknown) => {
    bootstrapPost("fatal", {
      role: message.role,
      sessionId: message.sessionId,
      error: String(error instanceof Error ? error.stack ?? error.message : error),
    });
  });
});
// Preserve worker-side stacks: ErrorEvent.error is not cloned to the main
// Worker error event, which otherwise loses WASM startup failure evidence.
globalThis.addEventListener("error", (event: ErrorEvent) => {
  console.error("CanvasKit worker uncaught error", event.error?.stack ?? event.message);
});
