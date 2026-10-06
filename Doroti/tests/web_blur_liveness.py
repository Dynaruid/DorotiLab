"""Serve a frozen Sample2 publish with the sustained blur probe injected.

Use a trusted HTTPS tunnel for physical Safari. No source or product binaries
are modified. --force-fence-timeout replaces only the frame queue's fence query
in the served JavaScript to exercise recovery with real GPU work still running.
"""
import argparse
import json
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import urlsplit


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", required=True, type=Path, help="Published Sample2 wwwroot")
    parser.add_argument("--out", required=True, type=Path, help="Evidence directory")
    parser.add_argument("--port", type=int, default=8765)
    parser.add_argument("--force-fence-timeout", action="store_true")
    args = parser.parse_args()
    root = args.root.resolve(strict=True)
    args.out.mkdir(parents=True, exist_ok=True)
    log = args.out / "liveness.jsonl"
    probe = Path(__file__).with_suffix(".js").read_bytes()
    index = (root / "index.html").read_text().replace(
        "</body>", '<script src="/__blur_liveness.js"></script></body>'
    ).encode()
    queue_path = "/_content/Doroti.Host.Web/doroti.web.gl-frames.js"
    queue_file = root / queue_path.lstrip("/")
    # Baselines published before frame admission was added have no queue asset.
    queue = queue_file.read_text() if queue_file.is_file() else ""
    if args.force_fence_timeout:
        query = "this.gl.clientWaitSync(submission.fence, 0, 0)"
        if queue.count(query) != 1:
            parser.error("Published frame queue query did not match; refusing unrelated injection")
        queue = queue.replace(query, "this.gl.TIMEOUT_EXPIRED")
    queue = queue.encode() if queue else None

    class Handler(SimpleHTTPRequestHandler):
        extensions_map = SimpleHTTPRequestHandler.extensions_map | {
            ".mjs": "text/javascript", ".wasm": "application/wasm"
        }

        def end_headers(self):
            self.send_header("Cross-Origin-Opener-Policy", "same-origin")
            self.send_header("Cross-Origin-Embedder-Policy", "require-corp")
            self.send_header("Cache-Control", "no-store")
            super().end_headers()

        def log_message(self, *args):
            pass

        def translate_path(self, path):
            target = root.joinpath(urlsplit(path).path.lstrip("/")).resolve()
            return str(target if target.is_relative_to(root) else root / "__invalid_path")

        def do_GET(self):
            path = urlsplit(self.path).path
            data, mime = (index, "text/html") if path in ("/", "/index.html") else (
                (probe, "text/javascript") if path == "/__blur_liveness.js" else (
                    (queue, "text/javascript") if path == queue_path else (None, None)))
            if data is None:
                return super().do_GET()
            self.send_response(200)
            self.send_header("Content-Type", mime)
            self.send_header("Content-Length", str(len(data)))
            self.end_headers()
            self.wfile.write(data)

        def do_POST(self):
            if urlsplit(self.path).path != "/probe-log":
                return self.send_error(404)
            row = json.loads(self.rfile.read(int(self.headers.get("Content-Length", "0"))))
            row["forcedFenceTimeout"] = args.force_fence_timeout
            with log.open("a") as stream:
                stream.write(json.dumps(row, ensure_ascii=False) + "\n")
            if row.get("event") not in ("sample", "heartbeat"):
                print(row.get("run"), row.get("event"), row.get("mode", ""), row.get("error", ""), flush=True)
            self.send_response(204)
            self.end_headers()

    print(f"http://localhost:{args.port}/?liveness=1&dorotiFrameCost=1&run=blur-check&seconds=180&scroll=1", flush=True)
    print(f"Evidence: {log}; forced fence timeout: {args.force_fence_timeout}", flush=True)
    ThreadingHTTPServer(("0.0.0.0", args.port), Handler).serve_forever()


if __name__ == "__main__":
    main()
