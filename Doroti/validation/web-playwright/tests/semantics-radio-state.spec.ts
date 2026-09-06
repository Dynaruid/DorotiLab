import { test, expect } from "./helpers/fixtures.js";
import { openDoroti } from "./helpers/doroti-diagnostics.js";

test("Web semantics keeps radio checked state independent of tile selection", async ({ page }) => {
  await openDoroti(page);
  const states = await page.evaluate(async () => {
    const moduleUrl = "/_content/Doroti.Host.Web/doroti.web.js";
    const host = await import(moduleUrl) as { updateSemantics(hostId: number, json: string): void };
    const hostId = Number(document.querySelector<HTMLElement>("[data-doroti-host-id]")!.dataset.dorotiHostId);
    host.updateSemantics(hostId, JSON.stringify({ generation: 1, nodes: [
      { id: 0, role: "group", rect: [0, 0, 400, 400], children: [1, 2, 3, 4] },
      { id: 1, role: "radio", rect: [0, 0, 100, 40], flags: { checked: "isTrue", selected: false } },
      { id: 2, role: "radio", rect: [0, 40, 100, 80], flags: { checked: "isFalse", selected: true } },
      { id: 3, role: "radio", rect: [0, 80, 100, 120], flags: { selected: true } },
      { id: 4, rect: [0, 120, 100, 160], flags: { mutuallyExclusive: true, checked: "isTrue", selected: false } },
    ] }));
    return [1, 2, 3, 4].map(id => {
      const element = document.getElementById(`doroti-semantics-${hostId}-${id}`)!;
      return [element.getAttribute("role"), element.getAttribute("aria-checked")];
    });
  });
  expect(states).toEqual([["radio", "true"], ["radio", "false"], ["radio", "true"], ["radio", "true"]]);
});
