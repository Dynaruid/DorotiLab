/** Main-DOM ownership shared by product worker composition and adapter validation. */
export const platformViewBatchVersion = 2 as const;
export interface NativeIdentity { readonly owner: string; readonly id: string; readonly generation: string; }
export interface NativeResource { readonly element: HTMLElement; dispose(): void; }
export type NativeFactory = (identity: NativeIdentity, parameters: unknown, signal: AbortSignal) => NativeResource | Promise<NativeResource>;
export interface NativeBounds { readonly left: number; readonly top: number; readonly width: number; readonly height: number; }
export interface NativePlacement {
  readonly identity: NativeIdentity; readonly bounds: NativeBounds; readonly clip?: NativeBounds;
  readonly visible: boolean; readonly order: number;
}
export interface ShieldPlacement { readonly id: string; readonly bounds: NativeBounds; readonly clip?: NativeBounds; readonly order: number; readonly debug: boolean; }
/** Logical CSS pixels. The source is preceding siblings, never this element or its foreground child. */
export interface EffectPlacement {
  readonly id: string; readonly bounds: NativeBounds; readonly clip?: NativeBounds; readonly order: number;
  readonly strength: number; readonly tint: string; readonly saturation: number;
}
export interface NativeBatch {
  readonly version: typeof platformViewBatchVersion; readonly owner: string;
  readonly epoch: number; readonly surfaceGeneration: number; readonly frame: number;
  readonly views: readonly NativePlacement[]; readonly shields: readonly ShieldPlacement[];
  readonly effects?: readonly EffectPlacement[];
}
interface Entry {
  readonly identity: NativeIdentity; readonly abort: AbortController;
  readonly container: HTMLDivElement; ready: Promise<void>;
  resource?: NativeResource; removed: boolean;
  disabled?: boolean;
  focus?: () => void;
}

/** Stable containers: placement never reparents iframe content or recreates its document. */
export class DorotiPlatformViewDomRegistry {
  readonly #factories = new Map<string, NativeFactory>();
  readonly #entries = new Map<string, Entry>();
  readonly #shields = new Map<string, { element: HTMLDivElement; dispose(): void }>();
  readonly #effects = new Map<string, HTMLDivElement>();
  #epoch = 0;
  #surfaceGeneration = 0;
  #frame = -1;
  #closed = false;
  #close?: Promise<void>;
  constructor(readonly root: HTMLElement, readonly owner: string,
    readonly focused: (identity: NativeIdentity) => void,
    readonly ingress: (event: Event) => void) {
    identity({ owner, id: "0", generation: "1" });
  }
  get liveCount(): number { return this.#entries.size; }
  get shieldCount(): number { return this.#shields.size; }
  get effectCount(): number { return this.#effects.size; }
  register(viewType: string, factory: NativeFactory): void {
    this.#requireOpen();
    if (!viewType || this.#factories.has(viewType)) throw new Error("Duplicate/empty native DOM factory.");
    this.#factories.set(viewType, factory);
  }
  create(handle: NativeIdentity, viewType: string, parameters?: unknown): Promise<void> {
    this.#requireOpen(); this.#validateOwner(handle);
    const factory = this.#factories.get(viewType);
    if (!factory) throw new Error(`No DOM factory for '${viewType}'.`);
    if (this.#entries.has(handle.id)) throw new Error("Native DOM id is already live.");
    const container = this.root.ownerDocument.createElement("div");
    container.dataset.dorotiPlatformView = handle.id;
    Object.assign(container.style, { position: "absolute", display: "none", overflow: "hidden", margin: "0", padding: "0" });
    const entry: Entry = { identity: Object.freeze({ ...handle }), abort: new AbortController(), container, ready: Promise.resolve(), removed: false };
    this.#entries.set(handle.id, entry);
    this.root.append(container);
    entry.ready = (async () => {
      let resource: NativeResource | undefined;
      let released = false;
      const release = (): void => { if (resource && !released) { released = true; resource.dispose(); } };
      try {
        resource = await factory(entry.identity, parameters, entry.abort.signal);
        if (resource.element.ownerDocument !== this.root.ownerDocument || resource.element.parentNode)
          throw new Error("DOM factories must return a fresh, unparented element from the owner document.");
        if (entry.removed || this.#closed) { release(); return; }
        entry.resource = resource;
        Object.assign(resource.element.style, { width: "100%", height: "100%", boxSizing: "border-box" });
        container.append(resource.element);
        entry.focus = () => { if (!entry.removed && container.style.display !== "none") this.focused(entry.identity); };
        resource.element.addEventListener("focusin", entry.focus);
      } catch (error) {
        entry.removed = true;
        container.remove();
        if (this.#entries.get(handle.id) === entry) this.#entries.delete(handle.id);
        if (entry.focus && resource) resource.element.removeEventListener("focusin", entry.focus);
        entry.resource = undefined;
        try { release(); } catch (cleanup) { throw new AggregateError([error, cleanup], "DOM creation and cleanup failed."); }
        throw error;
      }
    })();
    return entry.ready;
  }
  async remove(handle: NativeIdentity): Promise<void> {
    this.#validateOwner(handle);
    const entry = this.#entries.get(handle.id);
    if (!entry || entry.identity.generation !== handle.generation) return;
    if (!entry.removed) {
      entry.removed = true;
      entry.abort.abort();
      entry.container.style.pointerEvents = "none";
      entry.container.style.display = "none";
      if (entry.focus && entry.resource) entry.resource.element.removeEventListener("focusin", entry.focus);
    }
    try { await entry.ready; }
    finally {
      // remove() may be called repeatedly while creation is pending.
      const resource = entry.resource; entry.resource = undefined;
      try { resource?.dispose(); }
      finally {
        entry.container.remove();
        if (this.#entries.get(handle.id) === entry) this.#entries.delete(handle.id);
      }
    }
  }
  setEpoch(epoch: number, surfaceGeneration: number): void {
    this.#requireOpen(); sequence(epoch); sequence(surfaceGeneration);
    if (epoch < this.#epoch || surfaceGeneration < this.#surfaceGeneration) throw new Error("Native DOM epoch regressed.");
    if (epoch !== this.#epoch || surfaceGeneration !== this.#surfaceGeneration) this.#frame = -1;
    this.#epoch = epoch; this.#surfaceGeneration = surfaceGeneration;
  }
  /** Validates the whole batch before synchronous style changes; does not certify raster atomicity. */
  commit(batch: NativeBatch, advanceEpoch = false): void {
    this.#requireOpen();
    sequence(batch.epoch); sequence(batch.surfaceGeneration);
    const changedEpoch = batch.epoch !== this.#epoch || batch.surfaceGeneration !== this.#surfaceGeneration;
    if (batch.version !== platformViewBatchVersion || batch.owner !== this.owner ||
      (advanceEpoch ? batch.epoch < this.#epoch || batch.surfaceGeneration < this.#surfaceGeneration : changedEpoch) ||
      !Number.isSafeInteger(batch.frame) || batch.frame < 0 || (!changedEpoch && batch.frame <= this.#frame))
      throw new Error("Stale/version-mismatched native DOM batch.");
    if (batch.views.length > 16 || batch.shields.length > 16) throw new Error("Native DOM overlay limit exceeded.");
    const effects = batch.effects ?? [];
    if (effects.length > 4) throw new Error("Native DOM effect limit exceeded.");
    const effectIds = new Set<string>();
    const ids = new Set<string>(); const shieldIds = new Set<string>(); const orders = new Set<number>();
    for (const effect of effects) {
      validateBounds(effect.bounds); if (effect.clip) validateBounds(effect.clip); validateOrder(effect.order, orders);
      if (!effect.id || effectIds.has(effect.id) || !Number.isFinite(effect.strength) || effect.strength < 0 || effect.strength > 1 ||
          !Number.isFinite(effect.saturation) || effect.saturation < 0 || effect.saturation > 2 ||
          !/^#[0-9a-f]{8}$/i.test(effect.tint)) throw new Error("Invalid native DOM effect intent.");
      const css = this.root.ownerDocument.defaultView?.CSS;
      if (!css?.supports("backdrop-filter", "blur(1px)")) throw new Error("Live CSS backdrop filtering is unavailable.");
      effectIds.add(effect.id);
    }
    for (const placement of batch.views) {
      this.#validateOwner(placement.identity); validateBounds(placement.bounds); if (placement.clip) validateBounds(placement.clip);
      validateOrder(placement.order, orders);
      if (ids.has(placement.identity.id)) throw new Error("Duplicate native DOM placement.");
      ids.add(placement.identity.id);
      const entry = this.#entries.get(placement.identity.id);
      if (!entry || entry.removed || entry.disabled || !entry.resource || entry.identity.generation !== placement.identity.generation)
        throw new Error("Stale or not-ready DOM instance.");
    }
    for (const shield of batch.shields) {
      validateBounds(shield.bounds); if (shield.clip) validateBounds(shield.clip); validateOrder(shield.order, orders);
      if (!shield.id || shieldIds.has(shield.id)) throw new Error("Duplicate/empty input shield id.");
      shieldIds.add(shield.id);
    }
    for (const [id, entry] of this.#entries) if (!ids.has(id)) this.#hide(entry);
    for (const placement of batch.views) {
      const entry = this.#entries.get(placement.identity.id)!;
      apply(entry.container, placement.bounds, placement.clip, placement.order, placement.visible);
      if (entry.container.inert === placement.visible) entry.container.inert = !placement.visible;
      if (!placement.visible) this.#hide(entry);
    }
    for (const [id, shield] of this.#shields) if (!shieldIds.has(id)) { shield.dispose(); this.#shields.delete(id); }
    for (const placement of batch.shields) {
      let shield = this.#shields.get(placement.id);
      if (!shield) {
        const element = this.root.ownerDocument.createElement("div");
        element.dataset.dorotiInputShield = placement.id;
        element.setAttribute("aria-hidden", "true");
        element.style.position = "absolute";
        const events = ["pointerdown", "pointerup", "pointermove", "pointercancel", "wheel", "lostpointercapture"];
        const forward = (event: Event): void => {
          // Stop bubbling so a root-level ingress cannot dispatch this event a second time.
          event.stopPropagation();
          if (event.type === "pointerdown") element.setPointerCapture((event as PointerEvent).pointerId);
          this.ingress(event);
        };
        const preserveFocus = (event: Event): void => event.preventDefault();
        for (const type of events) element.addEventListener(type, forward, { passive: false });
        element.addEventListener("mousedown", preserveFocus);
        this.root.append(element);
        shield = { element, dispose: () => {
          for (const type of events) element.removeEventListener(type, forward);
          element.removeEventListener("mousedown", preserveFocus); element.remove();
        } };
        this.#shields.set(placement.id, shield);
      }
      apply(shield.element, placement.bounds, placement.clip, placement.order, true);
      setStyles(shield.element, { background: placement.debug ? "rgba(255,0,0,.18)" : "transparent" });
    }
    for (const [id, element] of this.#effects) if (!effectIds.has(id)) { element.remove(); this.#effects.delete(id); }
    for (const effect of effects) {
      let element = this.#effects.get(effect.id);
      if (!element) {
        element = this.root.ownerDocument.createElement("div");
        element.dataset.dorotiPlatformEffect = effect.id;
        element.setAttribute("aria-hidden", "true");
        Object.assign(element.style, { position: "absolute", pointerEvents: "none" });
        this.root.append(element); this.#effects.set(effect.id, element);
      }
      apply(element, effect.bounds, effect.clip, effect.order, true);
      setStyles(element, { "backdrop-filter": `blur(${effect.strength * 16}px) saturate(${effect.saturation})`, "background-color": effect.tint });
    }
    this.#epoch = batch.epoch; this.#surfaceGeneration = batch.surfaceGeneration; this.#frame = batch.frame;
  }
  focus(handle: NativeIdentity, focused: boolean): void {
    this.#requireOpen(); this.#validateOwner(handle);
    const entry = this.#entries.get(handle.id);
    if (!entry || entry.removed || entry.identity.generation !== handle.generation || !entry.resource)
      throw new Error("Stale native DOM focus request.");
    if (focused) {
      if (entry.container.style.display === "none" || entry.container.inert) throw new Error("Hidden native DOM view cannot take focus.");
      const active = this.root.ownerDocument.activeElement;
      if (active !== entry.resource.element && !entry.resource.element.contains(active)) entry.resource.element.focus();
    } else entry.resource.element.blur();
  }
  hide(handle: NativeIdentity): void {
    this.#validateOwner(handle);
    const entry = this.#entries.get(handle.id);
    if (entry?.identity.generation === handle.generation) this.#hide(entry);
  }
  disable(handle: NativeIdentity): void {
    this.hide(handle);
    const entry = this.#entries.get(handle.id);
    if (entry?.identity.generation === handle.generation) entry.disabled = true;
  }
  dispose(): Promise<void> {
    if (this.#close) return this.#close;
    this.#closed = true;
    for (const element of this.#effects.values()) element.remove();
    this.#effects.clear();
    for (const shield of this.#shields.values()) shield.dispose();
    this.#shields.clear(); this.#factories.clear();
    this.#close = Promise.allSettled([...this.#entries.values()].map(entry => this.remove(entry.identity))).then(results => {
      const errors = results.filter((result): result is PromiseRejectedResult => result.status === "rejected");
      if (errors.length) throw new AggregateError(errors.map(result => result.reason), "DOM native disposal failed.");
    });
    return this.#close;
  }
  #hide(entry: Entry): void {
    if (entry.container.contains(this.root.ownerDocument.activeElement)) (this.root.ownerDocument.activeElement as HTMLElement)?.blur();
    setStyles(entry.container, { display: "none" });
    if (!entry.container.inert) entry.container.inert = true;
  }
  #requireOpen(): void { if (this.#closed) throw new Error("Native DOM owner is closed."); }
  #validateOwner(handle: NativeIdentity): void { identity(handle); if (handle.owner !== this.owner) throw new Error("Foreign native DOM owner."); }
}
function identity(value: NativeIdentity): void {
  if (![value.owner, value.id, value.generation].every(part => typeof part === "string") ||
    ![value.owner, value.id].every(part => /^(0|[1-9][0-9]*)$/.test(part)) || !/^[1-9][0-9]*$/.test(value.generation))
    throw new Error("Invalid native DOM identity.");
}
function sequence(value: number): void { if (!Number.isSafeInteger(value) || value < 0) throw new Error("Invalid native DOM sequence."); }
function validateOrder(value: number, seen: Set<number>): void { sequence(value); if (seen.has(value)) throw new Error("Duplicate paint order."); seen.add(value); }
function validateBounds(bounds: NativeBounds): void {
  if (![bounds.left, bounds.top, bounds.width, bounds.height].every(Number.isFinite) || bounds.width < 0 || bounds.height < 0)
    throw new Error("Native DOM layout must be finite and nonnegative.");
}
function apply(element: HTMLElement, bounds: NativeBounds, clip: NativeBounds | undefined, order: number, visible: boolean): void {
  const left = clip ? Math.max(bounds.left, clip.left) : bounds.left;
  const top = clip ? Math.max(bounds.top, clip.top) : bounds.top;
  const right = clip ? Math.min(bounds.left + bounds.width, clip.left + clip.width) : bounds.left + bounds.width;
  const bottom = clip ? Math.min(bounds.top + bounds.height, clip.top + clip.height) : bounds.top + bounds.height;
  setStyles(element, { left: "0px", top: "0px", transform: `translate(${bounds.left}px, ${bounds.top}px)`,
    width: `${bounds.width}px`, height: `${bounds.height}px`,
    "z-index": String(order), display: visible && right > left && bottom > top ? "block" : "none",
    "clip-path": `inset(${Math.max(0, top - bounds.top)}px ${Math.max(0, bounds.left + bounds.width - right)}px ${Math.max(0, bounds.top + bounds.height - bottom)}px ${Math.max(0, left - bounds.left)}px)` });
}

// These wrappers belong to the registry. Preserve native elements and avoid
// repeating layout/filter mutations when only the composition frame advances.
const appliedStyles = new WeakMap<HTMLElement, Map<string, string>>();
function setStyles(element: HTMLElement, values: Readonly<Record<string, string>>): void {
  let previous = appliedStyles.get(element);
  if (!previous) { previous = new Map(); appliedStyles.set(element, previous); }
  for (const [name, value] of Object.entries(values)) {
    if (previous.get(name) === value) continue;
    element.style.setProperty(name, value);
    previous.set(name, value);
  }
}
