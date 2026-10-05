import * as files from "./doroti.web.files.js";
import * as navigation from "./doroti.web.navigation.js";
import * as drop from "./doroti.web.drop.js";
type Payload = Record<string, unknown>;
type Bridge = { postControl(kind: string, payload: Payload): void; requestControl(kind: string, payload: Payload): Promise<string> };
let bridge: Bridge | undefined;
let initialNavigation: string | null = null;
let persistenceFailure: ((id: number, message: string) => void) | undefined;
export function configurePersistenceFailure(callback: (id: number, message: string) => void): void { persistenceFailure = callback; }
export function configureServiceBridge(value: Bridge): void { bridge = value; }
export async function prepareApplicationNavigation(restorationId: string | null): Promise<void> {
  initialNavigation = bridge ? await bridge.requestControl("service-navigation-capture", { restorationId })
    : navigation.captureApplicationNavigation(restorationId);
}
export function openApplicationNavigation(hostId: number, restorationId: string | null): string | null {
  if (!bridge) return navigation.openApplicationNavigation(hostId, restorationId);
  if (initialNavigation === null) throw new Error("Prepare navigation before creating the browser view.");
  bridge.postControl("service-navigation-open", { hostId, restorationId });
  return initialNavigation;
}
export function reportApplicationRoute(hostId: number, route: string, state: string | null, replace: boolean): void {
  if (bridge) { bridge.postControl("service-navigation-report", { hostId, route, state, replace }); return; }
  navigation.reportApplicationRoute(hostId, route, state, replace);
}
export function saveApplicationRestoration(hostId: number, checkpoint: string): string | null {
  if (bridge) {
    void bridge.requestControl("service-navigation-save", { hostId, checkpoint })
      .catch(error => persistenceFailure?.(hostId, String(error)));
    return null;
  }
  return navigation.saveApplicationRestoration(hostId, checkpoint);
}
export function closeApplicationNavigation(hostId: number): void {
  if (bridge) { bridge.postControl("service-navigation-close", { hostId }); return; }
  navigation.closeApplicationNavigation(hostId);
}
export function openFileOwner(hostId: number): boolean {
  if (bridge) { bridge.postControl("service-file-open", { hostId }); return true; }
  return files.openFileOwner(hostId);
}
export function pickBrowserFiles(hostId: number, multiple: boolean, accept: string): Promise<string> {
  return bridge ? bridge.requestControl("service-file-pick", { hostId, multiple, accept }) : files.pickBrowserFiles(hostId, multiple, accept);
}
export function cancelBrowserPicker(hostId: number): void {
  if (bridge) bridge.postControl("service-file-cancel", { hostId }); else files.cancelBrowserPicker(hostId);
}
export function readBrowserFileBase64(hostId: number, token: string, offset: number, count: number): Promise<string> {
  return bridge ? bridge.requestControl("service-file-read", { hostId, token, offset, count }) : files.readBrowserFileBase64(hostId, token, offset, count);
}
export function releaseBrowserFile(hostId: number, token: string): void {
  if (bridge) bridge.postControl("service-file-release", { hostId, token }); else files.releaseBrowserFile(hostId, token);
}
export function closeFileOwner(hostId: number): void {
  if (bridge) bridge.postControl("service-file-close", { hostId }); else files.closeFileOwner(hostId);
}
export function openBrowserDrop(hostId: number, canvasId: string, options: string): void {
  if (bridge) bridge.postControl("service-drop-open", { hostId, canvasId, options }); else drop.openBrowserDrop(hostId, canvasId, options);
}
export function closeBrowserDrop(hostId: number): void {
  if (bridge) bridge.postControl("service-drop-close", { hostId }); else drop.closeBrowserDrop(hostId);
}
export async function handleService(kind: string, p: Payload): Promise<string> {
  const id = Number(p.hostId);
  switch (kind) {
    case "service-navigation-capture": return navigation.captureApplicationNavigation(p.restorationId as string | null);
    case "service-navigation-open": navigation.openApplicationNavigation(id, p.restorationId as string | null); break;
    case "service-navigation-report": navigation.reportApplicationRoute(id, String(p.route), p.state as string | null, Boolean(p.replace)); break;
    case "service-navigation-save": { const error = navigation.saveApplicationRestoration(id, String(p.checkpoint)); if (error) throw new Error(error); break; }
    case "service-navigation-close": navigation.closeApplicationNavigation(id); break;
    case "service-file-open": files.openFileOwner(id); break;
    case "service-file-pick": return files.pickBrowserFiles(id, Boolean(p.multiple), String(p.accept));
    case "service-file-cancel": files.cancelBrowserPicker(id); break;
    case "service-file-read": return files.readBrowserFileBase64(id, String(p.token), Number(p.offset), Number(p.count));
    case "service-file-release": files.releaseBrowserFile(id, String(p.token)); break;
    case "service-file-close": files.closeFileOwner(id); break;
    case "service-drop-open": drop.openBrowserDrop(id, String(p.canvasId), String(p.options)); break;
    case "service-drop-close": drop.closeBrowserDrop(id); break;
    default: throw new Error(`Unknown browser service '${kind}'.`);
  }
  return "";
}
