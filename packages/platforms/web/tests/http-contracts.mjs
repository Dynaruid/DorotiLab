import assert from "node:assert/strict";
import { readdir, access } from "node:fs/promises";
import { resolve } from "node:path";
import { startPublishServers } from "./serve-publish.mjs";
assert.ok(process.argv[2], "Pass a dual publish's wwwroot.");
const root = resolve(process.argv[2]);
const fixture = await startPublishServers(root);
try {
  for (const address of fixture.addresses) {
    const profileResponse = await fetch(`${address.url}/doroti.runtime-profile.json`);
    assert.equal(profileResponse.headers.get("Cache-Control"), "no-store");
    const manifest = await profileResponse.json();
    assert.equal(manifest.schemaVersion, 2);
    for (const route of ["/", "/demo/deep/route"]) {
      const response = await fetch(address.url + route, { headers: { "Accept-Encoding": "gzip" } });
      assert.equal(response.headers.get("Cross-Origin-Embedder-Policy"), address.isolated ? "require-corp" : null);
      const html = await response.text();
      assert.ok(html.includes(`content="${manifest.buildId}"`));
      assert.ok(html.includes(route.startsWith("/demo/") ? '<base href="/demo/"' : '<base href="/"'));
    }
    for (const mode of ["threaded", "compatible"]) {
      const prefix = `runtimes/${mode}/_framework`;
      const files = await readdir(resolve(root, prefix));
      const wasm = files.find(file => /^dotnet\.native.*\.wasm$/.test(file));
      const scripts = [manifest.variants[mode].loaderUrl, ...files.filter(file => /\.mjs$/.test(file)).map(file => `${prefix}/${file}`)];
      await Promise.all(scripts.map(async path => {
        const response = await fetch(`${address.url}/${path}`, { method: "HEAD" });
        assert.equal(response.status, 200);
        assert.equal(response.headers.get("Content-Type"), "application/javascript");
      }));
      for (const [encoding, suffix] of [["gzip", ".gz"], ["br", ".br"]]) {
        await access(resolve(root, prefix, wasm + suffix));
        const response = await fetch(`${address.url}/${prefix}/${wasm}`, { headers: { "Accept-Encoding": encoding } });
        assert.equal(response.headers.get("Content-Type"), "application/wasm");
        assert.equal(response.headers.get("Content-Encoding"), encoding);
        assert.deepEqual([...new Uint8Array(await response.arrayBuffer()).slice(0, 4)], [0, 97, 115, 109]);
      }
    }
    for (const path of ["/runtimes/threaded/_framework/missing.wasm", "/runtimes/missing/profile", "/demo/runtimes/missing/profile", "/demo/_content/missing/module"] ) {
      const response = await fetch(address.url + path);
      assert.equal(response.status, 404);
      assert.ok(!(await response.text()).toLowerCase().includes("<html"));
    }
    console.log(`PASS isolation=${address.isolated}; root/deep links, generation, concurrent modules, gzip/br WASM MIME and runtime 404.`);
  }
} finally { await fixture.close(); }
