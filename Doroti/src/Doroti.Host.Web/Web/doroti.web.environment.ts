export interface Insets { left: number; top: number; right: number; bottom: number }
export interface Box { left: number; top: number; right: number; bottom: number }
export interface BrowserDisplayFeature { bounds: Box; type: number; state: number }
export const zeroInsets = (): Insets => ({ left: 0, top: 0, right: 0, bottom: 0 });

// Only edge-spanning occlusions can be represented by Flutter's four insets.
export function edgeOcclusion(view: Box, occlusion: Box): Insets {
  const result = zeroInsets();
  const l = Math.max(view.left, occlusion.left), r = Math.min(view.right, occlusion.right);
  const t = Math.max(view.top, occlusion.top), b = Math.min(view.bottom, occlusion.bottom);
  if (l >= r || t >= b) return result;
  if (l <= view.left && r >= view.right) {
    if (b >= view.bottom) result.bottom = view.bottom - t;
    else if (t <= view.top) result.top = b - view.top;
  } else if (t <= view.top && b >= view.bottom) {
    if (l <= view.left) result.left = r - view.left;
    else if (r >= view.right) result.right = view.right - l;
  }
  return result;
}

export class BrowserViewEnvironment {
  value = { viewPadding: zeroInsets(), viewInsets: zeroInsets(), systemGestureInsets: zeroInsets(),
    displayFeatures: [] as BrowserDisplayFeature[],
    reduceMotion: false, highContrast: false, invertColors: false, environmentGeneration: 0 };
  private readonly probe = document.createElement("div");
  private readonly removers: (() => void)[] = [];
  private readonly observer: ResizeObserver;
  private unfocusedHeight = 0;
  private disposed = false;
  constructor(private readonly root: HTMLElement, private readonly input: HTMLTextAreaElement,
    private readonly changed: () => void) {
    this.probe.style.cssText = "position:fixed;left:0;top:0;visibility:hidden;pointer-events:none;width:0;height:0;" +
      "padding:env(safe-area-inset-top,0px) env(safe-area-inset-right,0px) env(safe-area-inset-bottom,0px) env(safe-area-inset-left,0px)";
    root.append(this.probe);
    const observe = (target: EventTarget, event: string): void => {
      const handler = (): void => this.refresh();
      target.addEventListener(event, handler);
      this.removers.push(() => target.removeEventListener(event, handler));
    };
    observe(window, "resize"); observe(window, "orientationchange");
    observe(document, "visibilitychange"); observe(document, "focusin"); observe(document, "focusout");
    if (window.visualViewport) { observe(window.visualViewport, "resize"); observe(window.visualViewport, "scroll"); }
    const posture = (navigator as Navigator & { devicePosture?: EventTarget }).devicePosture;
    if (posture) observe(posture, "change");
    const keyboard = (navigator as Navigator & { virtualKeyboard?: EventTarget }).virtualKeyboard;
    if (keyboard) observe(keyboard, "geometrychange");
    for (const query of ["(prefers-reduced-motion: reduce)", "(prefers-contrast: more)", "(forced-colors: active)", "(inverted-colors: inverted)"])
      observe(matchMedia(query), "change");
    this.observer = new ResizeObserver(() => this.refresh()); this.observer.observe(root);
    this.refresh(false);
  }
  refresh(notify = true): void {
    if (this.disposed) return;
    const rect = this.root.getBoundingClientRect(), css = getComputedStyle(this.probe);
    const width = document.documentElement.clientWidth, height = document.documentElement.clientHeight;
    const clamp = (n: number, max: number): number => Math.min(max, Math.max(0, n));
    const safeLeft = parseFloat(css.paddingLeft), safeTop = parseFloat(css.paddingTop);
    const safeRight = parseFloat(css.paddingRight), safeBottom = parseFloat(css.paddingBottom);
    const padding = { left: safeLeft > 0 && rect.left >= 0 ? clamp(safeLeft - rect.left, rect.width) : 0,
      top: safeTop > 0 && rect.top >= 0 ? clamp(safeTop - rect.top, rect.height) : 0,
      right: safeRight > 0 && rect.right <= width ? clamp(rect.right - width + safeRight, rect.width) : 0,
      bottom: safeBottom > 0 && rect.bottom <= height ? clamp(rect.bottom - height + safeBottom, rect.height) : 0 };
    const editing = document.activeElement === this.input ||
      (document.activeElement instanceof HTMLElement && this.root.contains(document.activeElement) &&
        (document.activeElement.isContentEditable || /^(INPUT|TEXTAREA)$/.test(document.activeElement.tagName)));
    const viewport = window.visualViewport;
    if (!editing) this.unfocusedHeight = viewport?.height ?? height;
    let insets = zeroInsets();
    const keyboard = (navigator as Navigator & { virtualKeyboard?: { boundingRect: DOMRect } }).virtualKeyboard;
    if (editing && keyboard) insets = edgeOcclusion(rect, keyboard.boundingRect);
    else if (editing && viewport && Math.abs(viewport.scale - 1) < .01 &&
      this.unfocusedHeight - viewport.height > Math.max(100, this.unfocusedHeight * .18) &&
      Math.abs(rect.left) <= 1 && Math.abs(rect.top) <= 1 && Math.abs(rect.width - width) <= 1) {
      // Conservative full-page fallback. Root intersection avoids double avoidance
      // with resize-content; pinch zoom and embedded roots do not use this heuristic.
      insets = edgeOcclusion(rect, { left: 0, right: width,
        top: viewport.offsetTop + viewport.height, bottom: Math.max(height, rect.bottom) });
    }
    const next = { viewPadding: padding, viewInsets: insets, systemGestureInsets: zeroInsets(),
      displayFeatures: this.features(rect),
      reduceMotion: matchMedia("(prefers-reduced-motion: reduce)").matches,
      highContrast: matchMedia("(prefers-contrast: more)").matches || matchMedia("(forced-colors: active)").matches,
      invertColors: matchMedia("(inverted-colors: inverted)").matches,
      environmentGeneration: this.value.environmentGeneration };
    if (JSON.stringify(next) === JSON.stringify(this.value)) return;
    next.environmentGeneration++;
    this.value = next;
    if (notify) this.changed();
  }
  private features(root: DOMRect): BrowserDisplayFeature[] {
    const segments = (window as Window & { viewport?: { segments?: Box[] } }).viewport?.segments ?? [];
    const result: BrowserDisplayFeature[] = [];
    for (let i = 0; i < segments.length; i++) for (let j = i + 1; j < segments.length; j++) {
      const a = segments[i]!, b = segments[j]!;
      let gap: Box | undefined;
      if (a.top === b.top && a.bottom === b.bottom)
        gap = { left: Math.min(a.right, b.right), right: Math.max(a.left, b.left), top: a.top, bottom: a.bottom };
      else if (a.left === b.left && a.right === b.right)
        gap = { top: Math.min(a.bottom, b.bottom), bottom: Math.max(a.top, b.top), left: a.left, right: a.right };
      if (!gap || gap.left > root.right || gap.right < root.left || gap.top > root.bottom || gap.bottom < root.top) continue;
      result.push({ bounds: { left: Math.max(gap.left, root.left) - root.left,
        right: Math.min(gap.right, root.right) - root.left, top: Math.max(gap.top, root.top) - root.top,
        bottom: Math.min(gap.bottom, root.bottom) - root.top },
        type: gap.right > gap.left && gap.bottom > gap.top ? 2 : 1,
        state: (navigator as Navigator & { devicePosture?: { type: string } }).devicePosture?.type === "folded" ? 2 : 1 });
    }
    return result;
  }
  dispose(): void { this.disposed = true; this.observer.disconnect(); for (const remove of this.removers) remove(); this.probe.remove(); }
}
