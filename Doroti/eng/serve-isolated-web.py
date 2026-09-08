"""Serve a local frozen Web build with the isolation headers required by WASM threads.

Usage: python Doroti/eng/serve-isolated-web.py WWWROOT --port 5192
Local development only; production hosting must configure the same headers.
"""
import argparse
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path


class IsolatedHandler(SimpleHTTPRequestHandler):
    extensions_map = {**SimpleHTTPRequestHandler.extensions_map,
                      '.wasm': 'application/wasm', '.mjs': 'text/javascript'}

    def end_headers(self):
        self.send_header('Cross-Origin-Opener-Policy', 'same-origin')
        self.send_header('Cross-Origin-Embedder-Policy', 'require-corp')
        self.send_header('Cross-Origin-Resource-Policy', 'same-origin')
        self.send_header('Cache-Control', 'no-store')
        super().end_headers()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('wwwroot', type=Path)
    parser.add_argument('--port', type=int, default=5192)
    args = parser.parse_args()
    directory = args.wwwroot.resolve(strict=True)
    if not directory.is_dir() or not (directory / 'index.html').is_file():
        parser.error('wwwroot must be a frozen build directory containing index.html')
    handler = partial(IsolatedHandler, directory=str(directory))
    with ThreadingHTTPServer(('127.0.0.1', args.port), handler) as server:
        print(f'Serving isolated Web build at http://127.0.0.1:{args.port} from {directory}', flush=True)
        try:
            server.serve_forever()
        except KeyboardInterrupt:
            pass


if __name__ == '__main__':
    main()
