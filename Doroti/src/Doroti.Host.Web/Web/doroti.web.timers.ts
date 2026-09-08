let dispatch: ((id: number, generation: number) => void) | undefined;
const timers = new Map<number, { owner: number; handle: number }>();

export function initializeBrowserTimers(callback: (id: number, generation: number) => void): void {
  if (dispatch) throw new Error("Doroti browser timer dispatcher is already initialized.");
  dispatch = callback;
}

export function scheduleBrowserTimer(id: number, owner: number, generation: number, milliseconds: number): void {
  if (!dispatch) throw new Error("Doroti browser timer dispatcher is not initialized.");
  if (!Number.isFinite(milliseconds) || milliseconds < 0) throw new Error("Invalid browser timer delay.");
  cancelBrowserTimer(id);
  const handle = globalThis.setTimeout(() => {
    if (timers.get(id)?.handle !== handle) return;
    timers.delete(id);
    dispatch!(id, generation);
  }, Math.min(0x7fffffff, Math.ceil(milliseconds)));
  timers.set(id, { owner, handle });
}

export function cancelBrowserTimer(id: number): void {
  const timer = timers.get(id);
  if (!timer) return;
  globalThis.clearTimeout(timer.handle);
  timers.delete(id);
}

export function cancelBrowserTimerOwner(owner: number): void {
  for (const [id, timer] of timers) if (timer.owner === owner) cancelBrowserTimer(id);
}

export function captureBrowserTimers() { return { pending: timers.size }; }
