"""Exercise the actual native MSBuild targets using isolated fixture files."""
import json
import os
from pathlib import Path
import subprocess
import sys
from xml.sax.saxutils import escape

repo = Path(__file__).resolve().parents[3]
root = Path(sys.argv[1]).resolve()
root.mkdir(parents=True, exist_ok=False)


def run(project, target, label, success=True):
    result = subprocess.run(["dotnet", "msbuild", str(project), "-nologo", f"-t:{target}"],
                            capture_output=True, text=True, timeout=1200)
    (root / f"{label}.log").write_text(result.stdout + result.stderr, encoding="utf-8")
    assert (result.returncode == 0) == success, result.stdout + result.stderr


def replace_preserving_metadata(path, content):
    stamp = path.stat().st_mtime_ns
    size = path.stat().st_size
    path.write_bytes(content)
    os.utime(path, ns=(stamp, stamp))
    assert path.stat().st_size == size


native = root / "native"
native.mkdir()
library = native / "libdoroti_qt_host.so"
library.write_bytes(b"OLD!")
qt = repo / "Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.Qt.targets"
project = root / "copy.proj"
project.write_text(f'''<Project><PropertyGroup><DorotiHostKind>Qt</DorotiHostKind>
<DorotiBuildQtNative>false</DorotiBuildQtNative><DorotiQtNativeBuildDirectory>{escape(str(native))}</DorotiQtNativeBuildDirectory>
<TargetDir>{escape(str(root / 'out'))}/</TargetDir><PublishDir>{escape(str(root / 'publish'))}/</PublishDir>
</PropertyGroup><Import Project="{escape(str(qt))}" /></Project>''', encoding="utf-8")
run(project, "CopyDorotiQtNative", "qt-initial")
replace_preserving_metadata(library, b"NEW!")
run(project, "CopyDorotiQtNative", "qt-replaced")
for output in [root / "out" / library.name, root / "publish" / library.name]:
    assert output.read_bytes() == b"NEW!", "Stale native library was retained"

# The generator fixture only writes headers; all invalidation/verification is
# performed by the same target imported by the real Windows native project.
generated = root / "generated"
generator = root / "generator.cmd"
ui = root / "ui.winmd"
sdk = root / "sdk.winmd"
ui.write_bytes(b"UI-A")
sdk.write_bytes(b"SDK1")
for name in ["foundation", "graphics"]:
    (root / f"{name}.winmd").write_text(name)
count = root / "calls.txt"
generator.write_text(f'''@echo off
if not exist "{generated / 'winrt'}" mkdir "{generated / 'winrt'}"
copy /y "{ui}" "{generated / 'winrt/Microsoft.UI.Windowing.h'}" >nul
copy /y "{sdk}" "{generated / 'winrt/Secondary.h'}" >nul
echo generated>>"{count}"
rem version A
''', encoding="utf-8")
target = repo / "Doroti/src/Doroti.Host.WindowsAppSdk.Native/Doroti.CppWinRT.targets"
project = root / "headers.proj"
properties = {"GeneratedHeaders": generated, "CppWinRTExe": generator, "MicrosoftUiWinmd": ui,
              "MicrosoftFoundationWinmd": root / "foundation.winmd", "MicrosoftGraphicsWinmd": root / "graphics.winmd",
              "CppWinRTSdkMetadata": sdk}
project.write_text('<Project><PropertyGroup>' + ''.join(f'<{k}>{escape(str(v))}</{k}>' for k, v in properties.items()) +
                   f'</PropertyGroup><Import Project="{escape(str(target))}" /></Project>', encoding="utf-8")


def headers(label, expected_calls, success=True):
    run(project, "GenerateWindowsAppSdkCppWinRT", label, success)
    assert len(count.read_text().splitlines()) == expected_calls


headers("headers-initial", 1)
headers("headers-unchanged", 1)
replace_preserving_metadata(ui, b"UI-B")
headers("headers-metadata-content", 2)
replace_preserving_metadata(generator, generator.read_bytes().replace(b"version A", b"version B"))
headers("headers-tool-content", 3)
replace_preserving_metadata(sdk, b"SDK2")
headers("headers-sdk-content", 4)
(generated / "winrt/Secondary.h").unlink()
headers("headers-missing-secondary", 5)
replace_preserving_metadata(generated / "winrt/Secondary.h", b"BAD!")
headers("headers-tampered-secondary", 6)
generator.unlink()
headers("headers-missing-tool", 6, success=False)
result = {"status": "PASS", "qtSameSizeMtimeBuildAndPublish": True, "windowsHeaderScenarios": 8,
          "scope": "Actual targets with isolated files on Windows; no Linux runtime or physical device acceptance"}
(root / "result.json").write_text(json.dumps(result, indent=2), encoding="utf-8")
print(json.dumps(result, indent=2))
