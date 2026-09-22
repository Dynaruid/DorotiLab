"""Local published-product server, with explicit WASM module MIME and isolation."""
import functools
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
import sys

class Handler(SimpleHTTPRequestHandler):
    extensions_map = {**SimpleHTTPRequestHandler.extensions_map,
                      '.js': 'text/javascript', '.mjs': 'text/javascript',
                      '.wasm': 'application/wasm', '.mp4': 'video/mp4'}

    def end_headers(self):
        self.send_header('Cross-Origin-Opener-Policy', 'same-origin')
        self.send_header('Cross-Origin-Embedder-Policy', 'require-corp')
        self.send_header('Cache-Control', 'no-store')
        super().end_headers()

ThreadingHTTPServer(('127.0.0.1', int(sys.argv[2]) if len(sys.argv)>2 else 5089),
                    functools.partial(Handler, directory=sys.argv[1])).serve_forever()
