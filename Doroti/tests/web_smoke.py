"""HTTP startup/asset smoke. Browser-visible rendering is verified separately."""
import os
from pathlib import Path
import socket
import subprocess
import sys
import time
import urllib.request

run = Path(sys.argv[1]).resolve()
root = Path(__file__).resolve().parents[2]
if not run.is_relative_to(root / "temp/testing"):
    raise RuntimeError("Expected a test run directory.")
run.mkdir(parents=True, exist_ok=True)
with socket.socket() as sock:
    sock.bind(("127.0.0.1", 0))
    port = sock.getsockname()[1]
url = f"http://127.0.0.1:{port}"
with (run / "server.log").open("w", encoding="utf-8") as log:
    process = subprocess.Popen(["dotnet", "run", "--project", "samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj",
                                "-c", "Debug", "--no-build", "--no-launch-profile"],
                               cwd=root, env={**os.environ, "ASPNETCORE_URLS": url}, stdout=log, stderr=subprocess.STDOUT)
    try:
        deadline = time.monotonic() + 60
        while True:
            if process.poll() is not None:
                raise RuntimeError(f"Web server exited {process.returncode}.")
            try:
                with urllib.request.urlopen(url, timeout=2) as response:
                    assert b"doroti" in response.read().lower()
                break
            except OSError:
                if time.monotonic() >= deadline:
                    raise TimeoutError("Web server failed to become ready.")
                time.sleep(.2)
        with urllib.request.urlopen(url + "/_framework/blazor.webassembly.js", timeout=10) as response:
            assert len(response.read()) > 1000
        print("PASS: Web HTTP startup and runtime bootstrap asset (not a browser rendering assertion)")
    finally:
        if os.name == "nt":
            subprocess.run(["taskkill", "/PID", str(process.pid), "/T", "/F"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        else:
            process.terminate()
        process.wait(timeout=10)
