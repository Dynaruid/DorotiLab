/** The HTML splash stays outside #app, whose children the engine replaces. */
export function attachDorotiSplash(): { complete(): void; fail(): void } {
  const splash = document.getElementById("doroti-splash");
  const app = document.getElementById("app");
  const status = splash?.querySelector<HTMLElement>("[data-doroti-splash-status]");
  const retry = splash?.querySelector<HTMLButtonElement>("[data-doroti-splash-retry]");
  const reload = (): void => location.reload();
  retry?.addEventListener("click", reload);
  let settled = false;

  const fail = (): void => {
    if (settled) return;
    settled = true;
    globalThis.removeEventListener("doroti-first-frame", dismiss);
    globalThis.removeEventListener("doroti-runtime-state", runtimeState);
    app?.removeAttribute("aria-busy");
    if (!splash) return;
    splash.dataset.dorotiSplashState = "failed";
    if (status) {
      status.setAttribute("role", "alert");
      status.textContent = "Unable to start Doroti. Please try again.";
    }
    if (retry) retry.hidden = false;
  };
  const runtimeState = (event: Event): void => {
    if ((event as CustomEvent<{ state: string }>).detail?.state === "failed") fail();
  };

  const dismiss = (): void => {
    if (settled) return;
    settled = true;
    globalThis.removeEventListener("doroti-first-frame", dismiss);
    globalThis.removeEventListener("doroti-runtime-state", runtimeState);
    app?.removeAttribute("aria-busy");
    if (!splash) return;
    splash.dataset.dorotiSplashState = "ready";
    splash.setAttribute("aria-hidden", "true");
    const remove = (): void => {
      splash.removeEventListener("transitionend", onTransitionEnd);
      retry?.removeEventListener("click", reload);
      clearTimeout(timeout);
      splash.remove();
    };
    const onTransitionEnd = (event: TransitionEvent): void => {
      if (event.target === splash && event.propertyName === "opacity") remove();
    };
    // Reduced motion and background tabs may not emit transitionend.
    const timeout = setTimeout(remove, 250);
    if (matchMedia("(prefers-reduced-motion: reduce)").matches) remove();
    else splash.addEventListener("transitionend", onTransitionEnd);
  };
  // Runtime readiness precedes the first frame. A terminal worker failure in
  // that gap must reach the splash even after the bootstrap promise resolves.
  globalThis.addEventListener("doroti-runtime-state", runtimeState);

  return {
    complete() {
      if (settled) return;
      // Engine readiness can precede its first canvas commit. Keep the HTML
      // visible until that frame, including when reduced motion skips the fade.
      if (!splash || document.getElementById("doroti-surface")?.hasAttribute("data-doroti-front-logical-width")) dismiss();
      else globalThis.addEventListener("doroti-first-frame", dismiss, { once: true });
    },
    fail,
  };
}
