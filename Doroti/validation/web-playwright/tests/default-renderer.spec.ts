import { test, expect } from "./helpers/fixtures.js";
import { openDoroti } from "./helpers/doroti-diagnostics.js";

for (const selection of ["omitted", "auto", "document-webgl"] as const) {
  test(`renderer selection: ${selection}`, async ({ page, runtimeErrors }) => {
    test.skip((process.env.DOROTI_WEB_RENDERER_MODE ?? "auto") !== "auto",
      "Default selection is checked without a forced renderer.");
    const query = selection === "omitted" ? "" : `&dorotiRenderer=${selection}`;
    const bundle = await openDoroti(page, query);
    const expected = selection === "document-webgl" ? selection : "worker-canvaskit-webgl";
    expect(bundle.presenter.mode).toBe(expected);
    if (expected === "worker-canvaskit-webgl") {
      expect(bundle.presenter.uiDiagnostics).toBeDefined();
      expect(bundle.presenter.rasterDiagnostics).toBeDefined();
    }
    expect(runtimeErrors).toEqual([]);
  });
}
