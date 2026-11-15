import { createServer } from "node:http";
import { readFile, stat } from "node:fs/promises";
import { resolve, extname, sep } from "node:path";
import { fileURLToPath } from "node:url";
import { promisify } from "node:util";
import { gzip, brotliCompress } from "node:zlib";
const compress = { gzip: promisify(gzip), br: promisify(brotliCompress) };
const mime = { ".html": "text/html", ".js": "application/javascript", ".mjs": "application/javascript", ".wasm": "application/wasm",
  ".json": "application/json", ".css": "text/css", ".svg": "image/svg+xml", ".png": "image/png", ".woff2": "font/woff2" };

export async function startPublishServers(directory) {
  const root = resolve(directory);
  const servers = [], addresses = [];
  for (const isolated of [false, true]) {
    const server = createServer(async (request, response) => {
      if (isolated) {
        response.setHeader("Cross-Origin-Opener-Policy", "same-origin");
        response.setHeader("Cross-Origin-Embedder-Policy", "require-corp");
      }
      try {
        const url = new URL(request.url, "http://localhost");
        const subpath = url.pathname.startsWith("/demo/");
        const relative = decodeURIComponent(subpath ? url.pathname.slice(6) : url.pathname.slice(1));
        let file = resolve(root, relative || "index.html");
        if (!file.startsWith(root + sep)) { response.writeHead(403).end(); return; }
        const runtimeAsset = /(?:^|\/)(?:runtimes|_framework|_content)(?:\/|$)/.test(relative);
        try {
          if (!(await stat(file)).isFile()) {
            if (runtimeAsset) { response.writeHead(404).end(); return; }
            file = resolve(root, "index.html");
          }
        } catch (error) {
          if (error.code !== "ENOENT") throw error;
          if (extname(relative) || runtimeAsset) { response.writeHead(404).end(); return; }
          file = resolve(root, "index.html");
        }
        let body = await readFile(file);
        const rewrite = extname(file) === ".html" && subpath;
        if (rewrite) body = Buffer.from(body.toString().replace('<base href="/"', '<base href="/demo/"'));
        const headers = { "Content-Type": mime[extname(file)] ?? "application/octet-stream", "Cache-Control": "no-store", Vary: "Accept-Encoding" };
        if (url.pathname.startsWith("/__csp__/")) headers["Content-Security-Policy"] = "worker-src 'none'";
        const accepted = request.headers["accept-encoding"] ?? "";
        for (const [encoding, suffix] of [["br", ".br"], ["gzip", ".gz"]]) {
          if (!accepted.split(",").some(value => value.trim().split(";")[0] === encoding)) continue;
          try {
            body = rewrite ? await compress[encoding](body) : await readFile(file + suffix);
            headers["Content-Encoding"] = encoding; break;
          } catch (error) { if (error.code !== "ENOENT") throw error; }
        }
        headers["Content-Length"] = body.length;
        response.writeHead(200, headers).end(request.method === "HEAD" ? undefined : body);
      } catch (error) { console.error(error); response.writeHead(500).end(); }
    });
    await new Promise((ready, reject) => { server.once("error", reject); server.listen(0, "127.0.0.1", ready); });
    servers.push(server);
    addresses.push({ isolated, url: `http://127.0.0.1:${server.address().port}` });
  }
  return { addresses, async close() { await Promise.all(servers.map(server => new Promise(done => { server.closeAllConnections(); server.close(done); }))); } };
}
if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  if (!process.argv[2]) throw new Error("Usage: node serve-publish.mjs <published-wwwroot>");
  const fixture = await startPublishServers(process.argv[2]);
  fixture.addresses.forEach(address => console.log(JSON.stringify(address)));
}
