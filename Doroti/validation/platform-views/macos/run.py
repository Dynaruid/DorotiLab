"""Build and execute the app bundle directly so native failures/exit status reach the recorder."""
from pathlib import Path
import subprocess
import sys

root = Path(__file__).resolve().parents[3]
project = Path(__file__).resolve().parent / "AppKitPlatformViews.csproj"
subprocess.run(["dotnet", "build", str(project), "--nologo", "-m:1", "-v:q"], cwd=root, check=True)
executable = project.parent / "bin/Debug/net10.0-macos/osx-arm64/AppKitPlatformViews.app/Contents/MacOS/AppKitPlatformViews"
raise SystemExit(subprocess.run([str(executable)], cwd=root).returncode)
