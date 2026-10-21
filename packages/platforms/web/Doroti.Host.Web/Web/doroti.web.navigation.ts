/** A Navigator keeps one guarded entry; a Router owns ordinary back/forward entries. */
type NavigationOwner = {
  key: string | null; pop: () => void; pagehide: () => void; pageshow: () => void;
  single: boolean; token: string; route: string; state: string | null; index: number;
  traversal?: () => void; queue: (() => void)[];
  exiting?: boolean;
  resumeSingle?: boolean;
};
const owners = new Map<number, NavigationOwner>();
let dispatch: ((hostId: number, json: string) => void) | undefined;
export function configureNavigation(callback: (hostId: number, json: string) => void): void { dispatch = callback; }

export function captureApplicationNavigation(restorationId: string | null): string {
  let checkpoint: string | null = null;
  try { checkpoint = restorationId ? sessionStorage.getItem(`doroti.restoration.v1:${restorationId}`) : null; } catch { }
  return JSON.stringify({ location: location.pathname + location.search + location.hash, checkpoint, state: routeState() });
}

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
  const owner: NavigationOwner = { key, single: false, token: crypto.randomUUID(), route: currentPath(),
    state: routeState(), index: historyIndex(), queue: [], pop: () => {}, pagehide: () => {}, pageshow: () => {} };
  const pop = (): void => {
    if (owner.exiting) return;
    if (owner.traversal) {
      const complete = owner.traversal;
      owner.traversal = undefined;
      complete();
      while (!owner.exiting && !owner.traversal && owner.queue.length) owner.queue.shift()!();
      if (owner.exiting) owner.queue.length = 0;
      return;
    }
    if (owner.single) {
      const marker = history.state?.dorotiHistory;
      if (marker?.owner === owner.token && marker.kind === "origin") {
        writeSingleEntry(owner, false);
        dispatch?.(hostId, JSON.stringify({ kind: "pop" }));
      } else if (marker?.owner === owner.token && marker.kind === "entry") {
        dispatch?.(hostId, JSON.stringify({ kind: "push", location: owner.route }));
      } else {
        // A manually edited hash adds an entry. Return to our guard before delivering it.
        owner.route = currentPath();
        owner.state = routeState();
        history.go(-1);
      }
      return;
    }
    owner.index = historyIndex();
    dispatch?.(hostId, JSON.stringify({ id: crypto.randomUUID(), location: currentPath(), state: routeState() }));
  };
  const markShutdown = (cleanShutdown: boolean): void => {
    // Framework writes each completed restoration update. Do not depend on async unload work.
    try {
      const raw = key ? sessionStorage.getItem(key) : null;
      if (raw && key) sessionStorage.setItem(key, JSON.stringify({ ...JSON.parse(raw), cleanShutdown }));
    } catch { /* browser storage failures never cancel navigation */ }
  };
  const pagehide = (): void => markShutdown(true);
  // A bfcache return resumes the same runtime without calling open again.
  const pageshow = (): void => {
    markShutdown(false);
    if (owner.exiting) {
      owner.exiting = false;
      if (owner.resumeSingle) selectApplicationHistory(hostId, true);
    }
  };
  Object.assign(owner, { pop, pagehide, pageshow });
  owners.set(hostId, owner);
  window.addEventListener("popstate", pop);
  window.addEventListener("pagehide", pagehide);
  window.addEventListener("pageshow", pageshow);
  return JSON.stringify({ location: location.pathname + location.search + location.hash, checkpoint,
    state: routeState() });
}

function currentPath(): string { return location.pathname + location.search + location.hash; }
function historyIndex(): number {
  const index: unknown = history.state?.doroti?.index;
  return Number.isSafeInteger(index) && (index as number) >= 0 ? index as number : 0;
}
function writeSingleEntry(owner: NavigationOwner, replace: boolean): void {
  const state = { doroti: { state: owner.state }, dorotiHistory: { owner: owner.token, kind: "entry" } };
  if (replace) history.replaceState(state, "", owner.route);
  else history.pushState(state, "", owner.route);
}
function enqueue(owner: NavigationOwner, action: () => void): void {
  if (owner.exiting) return;
  if (owner.traversal) owner.queue.push(action); else action();
}
function leaveSingle(owner: NavigationOwner, complete: () => void): void {
  owner.traversal = () => {
    const original = history.state?.dorotiHistory?.original ?? null;
    history.replaceState(original, "", owner.route);
    owner.single = false;
    owner.index = historyIndex();
    complete();
  };
  history.go(-1);
}

export function selectApplicationHistory(hostId: number, singleEntry: boolean): void {
  const owner = owners.get(hostId);
  if (!owner) return; // Only the document's navigation owner controls browser history.
  enqueue(owner, () => {
    if (owner.single === singleEntry) return;
    if (!singleEntry) { leaveSingle(owner, () => {}); return; }
    owner.single = true;
    owner.route = currentPath();
    owner.state = routeState();
    const marker = history.state?.dorotiHistory;
    if (marker?.kind === "entry" && typeof marker.owner === "string") {
      owner.token = marker.owner; // Reload/bfcache must not add another guard.
      return;
    }
    history.replaceState({ dorotiHistory: { owner: owner.token, kind: "origin", original: history.state } }, "", owner.route);
    writeSingleEntry(owner, false);
  });
}

export function exitApplicationNavigation(hostId: number): void {
  const owner = owners.get(hostId);
  if (!owner) return;
  enqueue(owner, () => {
    owner.resumeSingle = owner.single;
    if (owner.single) leaveSingle(owner, () => { owner.exiting = true; history.go(-1); });
    else { owner.exiting = true; history.go(-owner.index - 1); }
  });
}

export function reportApplicationRoute(hostId: number, route: string, stateJson: string | null, replace: boolean): void {
  const owner = owners.get(hostId);
  if (!owner) throw new Error("Navigation owner is closed.");
  const target = new URL(route, location.href);
  if (target.origin !== location.origin || !["http:", "https:"].includes(target.protocol))
    throw new Error("Router history requires a same-origin HTTP(S) URL.");
  enqueue(owner, () => {
    owner.route = target.pathname + target.search + target.hash;
    owner.state = stateJson;
    if (owner.single) { writeSingleEntry(owner, true); return; }
    if (!replace) owner.index++;
    const state = { ...(replace ? history.state : null), doroti: { state: stateJson, index: owner.index } };
    if (replace) history.replaceState(state, "", target);
    else history.pushState(state, "", target);
  });
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
