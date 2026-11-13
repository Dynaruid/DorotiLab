import assert from "node:assert/strict";
import { spawn } from "node:child_process";
import { createInterface } from "node:readline";
import { mkdtemp, writeFile, rm } from "node:fs/promises";
import { tmpdir } from "node:os";
import { join, resolve } from "node:path";

const [dotnet, serverDll, applicationDll] = process.argv.slice(2);
assert.ok(dotnet && serverDll && applicationDll, "Pass system dotnet.exe, development server DLL and built application DLL.");
const scratch = await mkdtemp(join(tmpdir(), "doroti-header-test-"));
const config = join(scratch, "config.jsonc");
await writeFile(config, JSON.stringify({ server: { headers: [
  { name: "Cross-Origin-Opener-Policy", value: "same-origin" },
  { name: "Cross-Origin-Embedder-Policy", value: "require-corp" },
] } }));
try {
  for (const isolated of [false, true]) {
    const child = spawn(dotnet, [resolve(serverDll), "--applicationpath", resolve(applicationDll), "--web-dev-config", config,
      isolated ? "--apply-cop-headers" : "--no-cop-headers", "--urls", "http://127.0.0.1:0"], { stdio: ["ignore", "pipe", "pipe"] });
    let output = "";
    try {
      const address = await new Promise((resolveAddress, reject) => {
        const timeout = setTimeout(() => reject(new Error("Development server startup timed out: " + output)), 30000);
        child.once("error", error => { clearTimeout(timeout); reject(error); });
        child.once("exit", code => { clearTimeout(timeout); reject(new Error(`Development server exited ${code}: ${output}`)); });
        child.stderr.on("data", data => { output += data; });
        createInterface({ input: child.stdout }).on("line", line => {
          output += line + "\n";
          const match = line.match(/Now listening on:\s+(http:\/\/127\.0\.0\.1:\d+)/);
          if (match) { clearTimeout(timeout); resolveAddress(match[1]); }
        });
      });
      const response = await fetch(address + "/index.html");
      assert.equal(response.status, 200);
      assert.match(await response.text(), /doroti_bootstrap\.js/);
      assert.equal(response.headers.get("Cross-Origin-Opener-Policy"), isolated ? "same-origin" : null);
      assert.equal(response.headers.get("Cross-Origin-Embedder-Policy"), isolated ? "require-corp" : null);
      for (const path of ["/_framework/missing.wasm", "/runtimes/missing/profile"]) {
        const missing = await fetch(address + path);
        assert.equal(missing.status, 404);
        assert.ok(!(await missing.text()).includes("<!doctype html>"));
      }
      console.log(`PASS development server isolation=${isolated}; configured headers and runtime 404 verified.`);
    } finally {
      child.kill();
      await new Promise(resolveExit => child.exitCode !== null ? resolveExit() : child.once("exit", resolveExit));
    }
  }
} finally { await rm(scratch, { recursive: true, force: true }); }
