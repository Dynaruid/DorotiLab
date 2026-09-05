import { execFile } from "node:child_process";
import { createHash } from "node:crypto";
import { readFile, writeFile } from "node:fs/promises";
import { resolve } from "node:path";
import { promisify } from "node:util";
import type { Page, Response } from "@playwright/test";

const exec = promisify(execFile);
const hash = (bytes: Uint8Array | string) => createHash("sha256").update(bytes).digest("hex");

export async function sourceManifest(patchPath: string) {
  const cwd = resolve(process.cwd(), "../../..");
  const git = async (...args: string[]) => (await exec("git", args, { cwd, windowsHide: true,
    timeout: 20 * 60 * 1000, maxBuffer: 64 * 1024 * 1024 })).stdout;
  const head = (await git("rev-parse", "HEAD")).trim();
  const patch = await git("diff", "--binary", "HEAD");
  await writeFile(patchPath, patch);
  const untracked = (await git("ls-files", "--others", "--exclude-standard", "-z")).split("\0").filter(Boolean);
  const files = await Promise.all(untracked.map(async path => ({ path, sha256: hash(await readFile(resolve(cwd, path))) })));
  return { head, trackedPatchSha256: hash(patch), untracked: files,
    sourceIdentitySha256: hash(JSON.stringify({ head, patch: hash(patch), untracked: files })) };
}

// Capture only during startup, before the native performance interval. Worker
// requests not exposed by Playwright remain explicitly outside this inventory.
export function observeServedAssets(page: Page) {
  const pending: Promise<unknown>[] = [];
  const listener = (response: Response) => {
    if (!/\.(?:js|wasm|json|bin|ttf|otf|woff2?)(?:\?|$)/i.test(response.url())) return;
    pending.push((async () => {
      try {
        const body = await response.body();
        return { url: response.url(), status: response.status(), bytes: body.length, sha256: hash(body) };
      } catch (error) { return { url: response.url(), status: response.status(), notVerified: String(error) }; }
    })());
  };
  page.context().on("response", listener);
  return async () => {
    page.context().off("response", listener);
    return { scope: "Playwright BrowserContext startup responses; unobserved Worker assets notVerified",
      assets: await Promise.all(pending), browserVersion: page.context().browser()?.version(),
      environment: await page.evaluate(() => ({ userAgent: navigator.userAgent,
        devicePixelRatio, innerWidth, innerHeight, outerWidth, outerHeight,
        crossOriginIsolated, timeOrigin: performance.timeOrigin })) };
  };
}
