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
      const failure = request.failure()?.errorText ?? "";
      // BrowserHttpHandler may abort its fetch controller after it has consumed
      // the complete 200 response body. Chromium reports that cleanup as a
      // failed request even though the managed byte-array read succeeded.
      if (failure === "net::ERR_ABORTED" && request.url().endsWith("/fonts/NanumGothic-Regular.ttf")) return;
      runtimeErrors.push(`requestfailed: ${request.method()} ${request.url()} ${failure}`);
    });

    await use(runtimeErrors);
    await attachDiagnostics(page, testInfo);
    expect(runtimeErrors, "browser runtime errors").toEqual([]);
  },
});

export { expect };
