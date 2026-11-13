import { createServer } from "node:http";
import { readFile, stat } from "node:fs/promises";
import { resolve, extname, sep } from "node:path";

const root = resolve(process.argv[2] ?? "");
if (!process.argv[2]) throw new Error("Usage: node serve-publish.mjs <published-wwwroot>");
const mime = { ".html": "text/html", ".js": "application/javascript", ".mjs": "application/javascript", ".wasm": "application/wasm",
  ".json": "application/json", ".css": "text/css", ".svg": "image/svg+xml", ".png": "image/png", ".woff2": "font/woff2" };
for (const isolated of [false, true]) {
  const server = createServer(async (request, response) => {
    try {
      const url = new URL(request.url, "http://localhost");
      const subpath = url.pathname.startsWith("/demo/");
      const relative = decodeURIComponent(subpath ? url.pathname.slice(6) : url.pathname.slice(1));
      let file = resolve(root, relative || "index.html");
      if (!file.startsWith(root + sep)) { response.writeHead(403).end(); return; }
      try { if (!(await stat(file)).isFile()) file = resolve(root, "index.html"); }
      catch (error) {
        if (error.code !== "ENOENT") throw error;
        if (extname(relative) || /(?:^|\/)(?:runtimes|_framework|_content)(?:\/|$)/.test(relative)) { response.writeHead(404).end(); return; }
        file = resolve(root, "index.html");
      }
      let body = await readFile(file);
      if (extname(file) === ".html" && subpath) body = Buffer.from(body.toString().replace('<base href="/"', '<base href="/demo/"'));
      const headers = { "Content-Type": mime[extname(file)] ?? "application/octet-stream", "Cache-Control": "no-store" };
      if (isolated) Object.assign(headers, { "Cross-Origin-Opener-Policy": "same-origin", "Cross-Origin-Embedder-Policy": "require-corp" });
      response.writeHead(200, headers).end(body);
    } catch (error) { console.error(error); response.writeHead(500).end(); }
  });
  server.listen(0, "127.0.0.1", () => console.log(JSON.stringify({ isolated, url: `http://127.0.0.1:${server.address().port}` })));
}
