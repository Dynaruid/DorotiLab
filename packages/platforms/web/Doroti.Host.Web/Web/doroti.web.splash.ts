/** Create the startup overlay outside #app, whose children the engine replaces. */
export function attachDorotiSplash(enabled = true): { complete(): void; fail(): void } {
  if (!enabled) return { complete() {}, fail() {} };

  const splash = document.createElement("div");
  splash.id = "doroti-splash";
  splash.className = "doroti-splash";
  splash.dataset.dorotiSplashState = "loading";

  const icon = document.createElement("img");
  icon.className = "doroti-splash__icon";
  icon.src = new URL("./doroti-app-icon.svg", import.meta.url).href;
  icon.width = icon.height = 112;
  icon.alt = "";
  icon.draggable = false;

  const name = document.createElement("p");
  name.className = "doroti-splash__name";
  name.textContent = "Doroti";

  const progress = document.createElement("div");
  progress.className = "doroti-splash__progress";
  progress.setAttribute("aria-hidden", "true");

  const status = document.createElement("p");
  status.className = "doroti-splash__status";
  status.dataset.dorotiSplashStatus = "";
  status.setAttribute("role", "status");
  status.setAttribute("aria-atomic", "true");
  status.textContent = "Loading…";

  const retry = document.createElement("button");
  retry.className = "doroti-splash__retry";
  retry.dataset.dorotiSplashRetry = "";
  retry.type = "button";
  retry.hidden = true;
  retry.textContent = "Try again";

  splash.append(icon, name, progress, status, retry);
  document.body.append(splash);
  const app = document.getElementById("app");
  app?.setAttribute("aria-busy", "true");
  const reload = (): void => location.reload();
  retry.addEventListener("click", reload);
  let settled = false;

  const fail = (): void => {
    if (settled) return;
    settled = true;
    globalThis.removeEventListener("doroti-first-frame", dismiss);
    globalThis.removeEventListener("doroti-runtime-state", runtimeState);
    app?.removeAttribute("aria-busy");
    splash.dataset.dorotiSplashState = "failed";
    status.setAttribute("role", "alert");
    status.textContent = "Unable to start Doroti. Please try again.";
    retry.hidden = false;
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
    splash.dataset.dorotiSplashState = "ready";
    splash.setAttribute("aria-hidden", "true");
    const remove = (): void => {
      splash.removeEventListener("transitionend", onTransitionEnd);
      retry.removeEventListener("click", reload);
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
      // Engine readiness can precede its first canvas commit. Keep the splash
      // visible until that frame, including when reduced motion skips the fade.
      if (document.getElementById("doroti-surface")?.hasAttribute("data-doroti-front-logical-width")) dismiss();
      else globalThis.addEventListener("doroti-first-frame", dismiss, { once: true });
    },
    fail,
  };
}
