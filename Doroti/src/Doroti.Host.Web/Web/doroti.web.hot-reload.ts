interface ReloadExports {
  ReadStatusAsync(): Promise<string>;
  PrepareAsync(runtimeId: string, requestId: string): Promise<boolean>;
}
export interface ReloadRuntime {
  getAssemblyExports(name: string): Promise<{ Doroti: { Host: { Web: { BrowserHotReload: ReloadExports } } } }>;
}

export function developmentBridge(search: string): { url: string; sessionId: string } | undefined {
  const params = new URLSearchParams(search);
  const value = params.get("dorotiDevBridge");
  const sessionId = params.get("dorotiDevSession");
  if (!value || !sessionId || !/^[a-f0-9-]{32,36}$/i.test(sessionId)) return;
  try {
    const url = new URL(value);
    if (url.protocol !== "http:" || url.hostname !== "127.0.0.1" || !url.port ||
        url.username || url.password || url.search || url.hash || !/^\/[a-f0-9]{48}$/.test(url.pathname)) return;
    return { url: url.href, sessionId };
  } catch { return; }
}

/** Status/request correlation only. The installed .NET SDK agent applies deltas. */
export async function startBrowserHotReload(runtime: ReloadRuntime): Promise<void> {
  if (!document.querySelector("script[src*='aspnetcore-browser-refresh']")) return;
  const methods = (await runtime.getAssemblyExports("Doroti.Host.Web.dll")).Doroti.Host.Web.BrowserHotReload;
  const bridge = developmentBridge(location.search);
  let active = true;
  let timer: ReturnType<typeof setTimeout> | undefined;
  let prepared: string | undefined;
  addEventListener("pagehide", () => { active = false; clearTimeout(timer); }, { once: true });
  async function poll(): Promise<void> {
    try {
      const json = await methods.ReadStatusAsync();
      const state = JSON.parse(json) as { runtimeId?: string; supported?: boolean };
      document.documentElement.dataset.dorotiHotReload = json;
      if (bridge && state.runtimeId) {
        const options = { signal: AbortSignal.timeout(3000) };
        const response = await fetch(`${bridge.url}/poll`, options);
        if (!response.ok) throw new Error(`Development bridge returned ${response.status}.`);
        const { request } = await response.json() as { request?: { requestId: string; runtimeId: string } };
        if (request && request.requestId !== prepared && state.supported &&
            await methods.PrepareAsync(request.runtimeId, request.requestId)) {
          const ack = await fetch(`${bridge.url}/prepared`, { ...options, method: "POST",
            headers: { "Content-Type": "application/json" }, body: JSON.stringify(request) });
          if (!ack.ok) throw new Error("Development request expired.");
          prepared = request.requestId;
        }
        const sent = await fetch(`${bridge.url}/status`, { ...options, method: "POST",
          headers: { "Content-Type": "application/json" }, body: json });
        if (!sent.ok) throw new Error("Development status rejected.");
      }
    } catch (error) {
      // A stopped editor/session must not keep sending requests or report success.
      active = false;
      document.documentElement.dataset.dorotiHotReloadBridge = "disconnected";
      console.warn("Doroti development status disconnected.", error);
    }
    if (active) timer = setTimeout(() => void poll(), 250);
  }
  void poll();
}
