import { developmentBridge } from "./doroti.web.hot-reload.js";

/** Application options are identical in both runtime transports. */
export function applicationEnvironment(search: string, applicationBaseUrl: string): Record<string, string> {
  const params = new URLSearchParams(search);
  const diagnostics = params.get("dorotiResizeDiagnostics") === "1" ? "1" : "0";
  return {
    DOROTI_WEB_APPLICATION_BASE_URL: applicationBaseUrl,
    DOROTI_DEV_SESSION_ID: developmentBridge(search)?.sessionId ?? "browser",
    DOROTI_TESTBED_MODE: params.get("dorotiTestbedMode") ?? "diagnostics",
    DOROTI_SAMPLE: params.get("dorotiSample") ?? "",
    DOROTI_LAYOUT_PROFILE: params.get("dorotiLayoutProfile") === "1" ? "1" : "0",
    DOROTI_ALLOCATION_PROFILE: params.get("dorotiAllocationProfile") === "1" ? "1" : "0",
    DOROTI_WEB_DIRECT_TRACE: diagnostics,
    DOROTI_STAGE_TRACE: diagnostics,
    DOROTI_WEBVIEW_COUNT: params.get("dorotiWebViewCount") ?? "1",
    DOROTI_WEBVIEW_WORKLOAD: params.get("dorotiWebViewWorkload") ?? "idle",
    DOROTI_SAMPLE_PROGRESS_SCOPE: params.get("dorotiProgressScope") ?? "local",
  };
}
