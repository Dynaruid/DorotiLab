import { defineConfig } from "@playwright/test";

const twentyMinutes = 20 * 60 * 1000;
const rendererMode = process.env.DOROTI_WEB_RENDERER_MODE ?? "auto";
const artifactLabel = process.env.DOROTI_WEB_ARTIFACT_LABEL ?? rendererMode;
const artifactRoot = `./artifacts/${artifactLabel}`;

export default defineConfig({
  testDir: "./tests",
  outputDir: `${artifactRoot}/test-results`,
  fullyParallel: false,
  workers: 1,
  timeout: twentyMinutes,
  expect: { timeout: 20_000 },
  reporter: [
    ["list"],
    ["html", { outputFolder: `${artifactRoot}/report`, open: "never" }],
  ],
  use: {
    baseURL: process.env.DOROTI_WEB_BASE_URL ?? "http://127.0.0.1:5088",
    actionTimeout: 20_000,
    navigationTimeout: 120_000,
    trace: "retain-on-failure",
    video: "retain-on-failure",
    screenshot: "only-on-failure",
  },
  projects: [
    {
      name: "chromium-hardware",
      grepInvert: /@headed|@dpr/,
      use: {
        browserName: "chromium",
        headless: true,
        viewport: { width: 1280, height: 800 },
        launchOptions: {
          args: [
            "--enable-gpu-rasterization",
            "--ignore-gpu-blocklist",
            "--use-angle=default",
          ],
        },
      },
    },
    {
      name: "chromium-dpr2",
      grep: /@dpr/,
      use: {
        browserName: "chromium",
        headless: true,
        viewport: { width: 960, height: 640 },
        deviceScaleFactor: 2,
        launchOptions: {
          args: [
            "--enable-gpu-rasterization",
            "--ignore-gpu-blocklist",
            "--use-angle=default",
          ],
        },
      },
    },
    {
      name: "desktop-chrome-headed",
      grep: /@headed/,
      use: {
        browserName: "chromium",
        channel: "chrome",
        headless: false,
        viewport: null,
        launchOptions: {
          args: ["--enable-gpu-rasterization", "--ignore-gpu-blocklist"],
        },
      },
    },
  ],
});
