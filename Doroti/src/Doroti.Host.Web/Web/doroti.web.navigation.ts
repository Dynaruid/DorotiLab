/** Per-tab state. History changes are fed to the existing framework Router. */
type NavigationOwner = { key: string | null; pop: () => void; pagehide: () => void; pageshow: () => void };
const owners = new Map<number, NavigationOwner>();
let dispatch: ((hostId: number, json: string) => void) | undefined;
export function configureNavigation(callback: (hostId: number, json: string) => void): void { dispatch = callback; }

function routeState(): string | null {
  // Other scripts and earlier app versions may own existing history entries.
  const state: unknown = history.state?.doroti?.state;
  if (typeof state !== "string") return null;
  try { JSON.parse(state); return state; } catch { return null; }
}

export function openApplicationNavigation(hostId: number, restorationId: string | null): string | null {
  if (typeof window === "undefined") return null; // worker-owned framework is separately qualified
  if (owners.has(hostId)) throw new Error("Navigation already attached.");
  if (owners.size) throw new Error("A document history has one Router owner.");
  const key = restorationId ? `doroti.restoration.v1:${restorationId}` : null;
  let checkpoint: string | null = null;
  try { checkpoint = key ? sessionStorage.getItem(key) : null; } catch { /* storage can be denied */ }
  const pop = (): void => dispatch?.(hostId, JSON.stringify({
    id: crypto.randomUUID(), location: location.pathname + location.search + location.hash,
    state: routeState(),
  }));
  const markShutdown = (cleanShutdown: boolean): void => {
    // Framework writes each completed restoration update. Do not depend on async unload work.
    try {
      const raw = key ? sessionStorage.getItem(key) : null;
      if (raw && key) sessionStorage.setItem(key, JSON.stringify({ ...JSON.parse(raw), cleanShutdown }));
    } catch { /* browser storage failures never cancel navigation */ }
  };
  const pagehide = (): void => markShutdown(true);
  // A bfcache return resumes the same runtime without calling open again.
  const pageshow = (): void => markShutdown(false);
  owners.set(hostId, { key, pop, pagehide, pageshow });
  window.addEventListener("popstate", pop);
  window.addEventListener("pagehide", pagehide);
  window.addEventListener("pageshow", pageshow);
  return JSON.stringify({ location: location.pathname + location.search + location.hash, checkpoint,
    state: routeState() });
}

export function reportApplicationRoute(hostId: number, route: string, stateJson: string | null, replace: boolean): void {
  if (!owners.has(hostId)) throw new Error("Navigation owner is closed.");
  const target = new URL(route, location.href);
  if (target.origin !== location.origin || !["http:", "https:"].includes(target.protocol))
    throw new Error("Router history requires a same-origin HTTP(S) URL.");
  const state = { ...(replace ? history.state : null), doroti: { state: stateJson } };
  if (replace) history.replaceState(state, "", target);
  else history.pushState(state, "", target);
}

export function saveApplicationRestoration(hostId: number, checkpoint: string): string | null {
  const owner = owners.get(hostId);
  if (!owner) return "Navigation owner is closed.";
  try { if (owner.key) sessionStorage.setItem(owner.key, checkpoint); return null; }
  catch (error) { return String(error); }
}

export function closeApplicationNavigation(hostId: number): void {
  const owner = owners.get(hostId);
  if (!owner) return;
  window.removeEventListener("popstate", owner.pop);
  window.removeEventListener("pagehide", owner.pagehide);
  window.removeEventListener("pageshow", owner.pageshow);
  owners.delete(hostId);
}
