// Inject into a frozen Sample2 publish for sustained physical-Safari checks.
// The serving harness supplies /probe-log; this is never a product asset.
(async () => {
  const query = new URLSearchParams(location.search);
  if (!query.has("liveness")) return;
  const run = query.get("run") || "blur-liveness";
  const delay = ms => new Promise(resolve => setTimeout(resolve, ms));
  const label = prefix => [...document.querySelectorAll("#doroti-semantics [aria-label]")]
    .map(element => element.getAttribute("aria-label")).find(value => value.startsWith(prefix));
  const waitLabel = async prefix => {
    for (let i = 0; i < 50; i++) {
      if ([...document.querySelectorAll("#doroti-semantics [aria-label]")]
          .some(element => element.getAttribute("aria-label").includes(prefix))) return;
      await delay(100);
    }
    throw new Error("Application control did not settle within 5s: " + prefix);
  };
  const report = async (event, data = {}) => {
    const row = { run, event, time: Date.now(), ...data };
    try { await fetch("/probe-log", { method: "POST", body: JSON.stringify(row),
      headers: { "Content-Type": "application/json" } }); } catch {}
    console.log("BLUR_LIVENESS", JSON.stringify(row));
  };
  const error = event => void report("error", {
    error: String(event.error || event.reason || event.message),
    stack: event.error?.stack || event.reason?.stack || null,
    source: event.filename || null, line: event.lineno || null, column: event.colno || null,
  });
  const visibility = () => void report("visibility", { state: document.visibilityState });
  const page = event => void report(event.type, { persisted: event.persisted, state: document.visibilityState });
  addEventListener("error", error); addEventListener("unhandledrejection", error);
  document.addEventListener("visibilitychange", visibility);
  addEventListener("pagehide", page); addEventListener("pageshow", page);
  const reloads = +(sessionStorage.getItem(run) || 0);
  sessionStorage.setItem(run, String(reloads + 1));
  let root, capture, heartbeat, mode = "startup", progress = 0, scrollEvents = 0;
  let lastDiagnostic, diagnosticPending = false, stalled = false;
  let nativeFailure;
  let consoleErrors = 0;
  const consoleOriginals = new Map();
  const consoleCaptures = new Map();
  for (const level of ["error", "warn", "log", "info", "debug"]) {
    const original = console[level];
    const capture = (...args) => {
      original.apply(console, args);
      if (args[0] === "BLUR_LIVENESS") return;
      const message = args.map(String).join(" ");
      // Mono's fatal finalizer exception can abort a different pthread without
      // raising an ErrorEvent in this window or incrementing Failed frames.
      if (/FATAL UNHANDLED EXCEPTION|SynchronizationLockException|Assertion.*failed|Aborted\(/i.test(message))
        nativeFailure = message;
      if (++consoleErrors <= 128) void report("runtime-console-error", { level, message: message.slice(0,4000) });
    };
    consoleOriginals.set(level, original);
    consoleCaptures.set(level, capture);
    console[level] = capture;
  }
  // Install before the first await: Emscripten binds its output functions while
  // loading and would otherwise retain the original console methods.
  await report("boot", { reloads, ua: navigator.userAgent, dpr: devicePixelRatio,
    width: innerWidth, height: innerHeight });
  const seconds = +(query.get("seconds") || 180);
  const wideSweep = query.get("sweep") === "wide";
  const startupHeartbeat = setInterval(() => void report("startup-heartbeat", {
    state: document.visibilityState, dataset: { ...document.documentElement.dataset },
  }), 2000);
  try {
    if (reloads) throw new Error("Unexpected page reload");
    for (let i = 0; i < 1200 && document.documentElement.dataset.dorotiBootstrapStage !== "started"; i++) {
      if (document.documentElement.dataset.dorotiBootstrapStage === "failed")
        throw new Error("Renderer startup failed: " + document.documentElement.dataset.dorotiBootstrapError);
      await delay(100);
    }
    await delay(3000);
    clearInterval(startupHeartbeat);
    root = document.querySelector(".doroti-root");
    if (!root) throw new Error("Renderer did not start: " + JSON.stringify({ ...document.documentElement.dataset }));
    capture = root.setPointerCapture;
    root.setPointerCapture = id => { if (id < 99) capture.call(root, id); };
    let pointerId = 99;
    const pointer = (type, x, y, pointerType = "touch") => {
      if (type === "pointerdown") pointerId++;
      return root.dispatchEvent(new PointerEvent(type, {
        pointerId, pointerType, isPrimary: true, clientX: x, clientY: y,
        button: 0, buttons: type === "pointerup" ? 0 : 1, bubbles: true,
      }));
    };
    const tap = async (prefix, bottom = false) => {
      const element = [...document.querySelectorAll("#doroti-semantics [aria-label]")]
        .find(e => e.getAttribute("aria-label").startsWith(prefix)
          && (!bottom || e.getBoundingClientRect().y > innerHeight - 100));
      if (!element) throw new Error("Missing control " + prefix);
      const rect = element.getBoundingClientRect(), x = rect.x + rect.width / 2, y = rect.y + rect.height / 2;
      const pointerType = query.get("tap") || "mouse";
      await report("tap", { prefix, pointerType, x, y, width: innerWidth, height: innerHeight });
      pointer("pointerdown", x, y, pointerType); await delay(70);
      pointer("pointerup", x, y, pointerType); await delay(750);
    };
    const diagnostics = async () => {
      const pending = window.__dorotiFrameCost("diagnostics");
      let timer;
      try {
        return await Promise.race([pending, new Promise((_, reject) => {
          timer = setTimeout(() => reject(new Error("Render worker diagnostic timed out after 5s")), 5000);
        })]);
      } finally { clearTimeout(timer); }
    };
    const sample = async () => {
      if (diagnosticPending) return;
      diagnosticPending = true;
      try {
        if (nativeFailure) throw new Error(nativeFailure);
        const value = await diagnostics();
        const frame = value.managed.frame;
        // Active movement must advance submitted frames, not just DOM labels
        // or main-window rAF. A live main page can hide a frozen render worker.
        if (lastDiagnostic && progress > lastDiagnostic.progress + 30
            && frame.Submitted <= lastDiagnostic.submitted) stalled = true;
        lastDiagnostic = { progress, submitted: frame.Submitted };
        await report("sample", { mode, progress, scrollEvents, label: label("Blur strength:"), diagnostics: value, stalled });
        if (frame.Failed || value.webgl?.failure) stalled = true;
      } catch (failure) { stalled = true; await report("stalled", { mode, progress, error: String(failure) }); }
      finally { diagnosticPending = false; }
    };
    heartbeat = setInterval(() => {
      void report("heartbeat", { mode, progress, state: document.visibilityState,
        stage: document.documentElement.dataset.dorotiBootstrapStage });
      void sample();
    }, 2000);
    await tap("Variable Blur", true);
    await waitLabel("Blur strength:");
    const modes = query.get("modes")?.split(",") || ["Fast adaptive", "Full quality", "Adaptive", "Fixed 1/4", "Dual Kawase"];
    await sample();
    for (mode of modes) {
      await tap(mode);
      const slider = document.querySelector("#doroti-semantics [role=slider]");
      const rect = slider.getBoundingClientRect();
      const left = rect.x + 22, width = rect.width - 44, y = rect.y + rect.height / 2;
      const initial = +(label("Blur strength:").match(/[\d.]+/)[0]) / 32;
      const hint = [...document.querySelectorAll("#doroti-semantics [aria-label]")]
        .find(e => e.getAttribute("aria-label").startsWith("Scroll the list through"));
      const listTop = hint.getBoundingClientRect().bottom + 16, listBottom = innerHeight - 100;
      let sliderDown = true, scrollDown = false, scrollY = listBottom, scrollFrame = 0;
      let sliderX = left + width * initial;
      pointer("pointerdown", left + width * initial, y); await delay(60);
      const start = performance.now();
      while (performance.now() - start < seconds * 1000 / modes.length && !stalled && !nativeFailure) {
        await new Promise(requestAnimationFrame);
        // Keep the filter enabled throughout. The previous zero-ending sweeps
        // gave every round an unfiltered frame and missed sustained overload.
        const phase = (performance.now() - start) / (wideSweep ? 120 : 700);
        // The wide sweep stresses capture-size changes and cache eviction as
        // well as GPU work. Keep the default sweep near maximum for sustained
        // filter load; a low-sigma frame can otherwise mask that overload.
        const fraction = wideSweep
          ? .02 + .97 * (.5 + .5 * Math.sin(phase))
          : .82 + .17 * (.5 + .5 * Math.sin(phase));
        const scrolling = query.has("scroll") && listBottom > listTop + 30
          && performance.now() - start > seconds * 500 / modes.length;
        if (!scrolling) {
          sliderX = left + width * fraction;
          pointer("pointermove", sliderX, y);
        } else {
          // Alternate real slider and list gestures. A second synthetic touch
          // can cancel the slider's gesture and leave sigma at its default.
          if (sliderDown) {
            pointer("pointerup", sliderX, y); sliderDown = false; await delay(500);
          }
          const stroke = scrollFrame % 90, forward = Math.floor(scrollFrame / 90) % 2 === 0;
          if (stroke === 0) {
            scrollY = forward ? listBottom : listTop;
            pointer("pointerdown", innerWidth / 2, scrollY); scrollDown = true;
          }
          scrollY = forward ? listBottom - (listBottom - listTop) * stroke / 89
            : listTop + (listBottom - listTop) * stroke / 89;
          pointer("pointermove", innerWidth / 2, scrollY); scrollEvents++;
          if (stroke === 89) { pointer("pointerup", innerWidth / 2, scrollY); scrollDown = false; }
          scrollFrame++;
        }
        progress++;
      }
      if (scrollDown) pointer("pointerup", innerWidth / 2, scrollY);
      if (sliderDown) pointer("pointerup", sliderX, y);
      await delay(750);
      await sample();
      if (nativeFailure) throw new Error(nativeFailure);
      if (stalled) throw new Error("Rendering stopped during sustained blur");
      // Prove the last stroke settled and ordinary application input remains
      // functional after the GPU load, including an unrelated tab round trip.
      if (!wideSweep && +(label("Blur strength:").match(/[\d.]+/)[0]) < 24)
        throw new Error("The sustained test did not keep the blur active");
      await tap("Components", true);
      await waitLabel("Volume:");
      await tap("Variable Blur", true);
      await waitLabel("Blur strength:");
      await report("mode-complete", { mode, progress, diagnostics: await diagnostics() });
    }
    await report("PASS", { progress, scrollEvents, seconds, wideSweep, reloads, diagnostics: await diagnostics() });
    document.documentElement.dataset.blurLiveness = "PASS";
  } catch (failure) {
    await report("FAIL", { mode, progress, error: String(failure), lastDiagnostic,
      controls: [...document.querySelectorAll("#doroti-semantics [aria-label]")].map(element => ({
        label: element.getAttribute("aria-label"), role: element.getAttribute("role"),
        selected: element.getAttribute("aria-selected"),
        bounds: element.getBoundingClientRect().toJSON(),
      })),
    });
    document.documentElement.dataset.blurLiveness = "FAIL";
  } finally {
    clearInterval(startupHeartbeat);
    clearInterval(heartbeat);
    for (const [level, capture] of consoleCaptures)
      if (console[level] === capture) console[level] = consoleOriginals.get(level);
    if (root && capture) root.setPointerCapture = capture;
    removeEventListener("error", error); removeEventListener("unhandledrejection", error);
    document.removeEventListener("visibilitychange", visibility);
    removeEventListener("pagehide", page); removeEventListener("pageshow", page);
  }
})();
