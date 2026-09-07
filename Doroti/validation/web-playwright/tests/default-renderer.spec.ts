import { test, expect } from "./helpers/fixtures.js";
import { openDoroti } from "./helpers/doroti-diagnostics.js";

for (const selection of ["omitted", "auto", "worker-direct-webg", "document-webgl", "worker-direct-webgl", "worker-canvaskit-webgl", "offscreen-worker", "offscreen-bitmap"] as const) {
  test(`renderer selection: ${selection}`, async ({ page, runtimeErrors }) => {
    test.skip((process.env.DOROTI_WEB_RENDERER_MODE ?? "auto") !== "auto",
      "Default selection is checked without a forced renderer.");
    const query = selection === "omitted" ? "" : `&dorotiRenderer=${selection}`;
    const bundle = await openDoroti(page, query);
    const expected = ["omitted", "auto", "worker-direct-webg"].includes(selection) ? "worker-direct-webgl" : selection;
    expect(bundle.presenter.mode).toBe(expected);
    if (expected === "worker-canvaskit-webgl") {
      expect(bundle.presenter.uiDiagnostics).toBeDefined();
      expect(bundle.presenter.rasterDiagnostics).toBeDefined();
    }
    if (expected === "worker-direct-webgl") {
      expect(bundle.presenter.mainManagedRuntimeCount).toBe(0);
      expect(bundle.presenter.workerManagedRuntimeCount).toBe(1);
      expect(bundle.presenter.visibleContext).toBe("transferred-offscreen-webgl2");
      expect(bundle.presenter.frontRequestId).toBeGreaterThan(0);
      expect(bundle.presenter.fallbackReason).toBeNull();
      expect(bundle.presenter.requestedMode).toBe(["omitted", "auto", "worker-direct-webg"].includes(selection) ? "auto" : selection);
    }
    expect(runtimeErrors).toEqual([]);
  });
}
