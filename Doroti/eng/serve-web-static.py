import argparse
import functools
import http.server
import mimetypes


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--directory", required=True)
    parser.add_argument("--port", type=int, required=True)
    args = parser.parse_args()
    mimetypes.add_type("text/javascript", ".mjs")
    mimetypes.add_type("application/javascript", ".js")
    mimetypes.add_type("application/wasm", ".wasm")
    handler = functools.partial(http.server.SimpleHTTPRequestHandler, directory=args.directory)
    server = http.server.ThreadingHTTPServer(("127.0.0.1", args.port), handler)
    server.serve_forever()


if __name__ == "__main__":
    main()
