import { test as base, expect } from "@playwright/test";
import { attachDiagnostics } from "./doroti-diagnostics.js";

export const test = base.extend<{ runtimeErrors: string[] }>({
  runtimeErrors: async ({ page }, use, testInfo) => {
    const runtimeErrors: string[] = [];
    page.on("pageerror", (error) => runtimeErrors.push(`pageerror: ${error.message}`));
    page.on("console", (message) => {
      if (message.type() === "error") {
        const location = message.location();
        runtimeErrors.push(`console.error: ${message.text()}${location.url ? ` (${location.url}:${location.lineNumber})` : ""}`);
      }
    });
    page.on("requestfailed", (request) => {
      runtimeErrors.push(`requestfailed: ${request.method()} ${request.url()} ${request.failure()?.errorText ?? ""}`);
    });

    await use(runtimeErrors);
    await attachDiagnostics(page, testInfo);
    expect(runtimeErrors, "browser runtime errors").toEqual([]);
  },
});

export { expect };
