"""Evaluate the packaged icon defaults without requiring every platform workload."""
import json
from pathlib import Path
import struct
import subprocess
import tempfile
import zipfile
import sys
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
SDK = ROOT / "Doroti/src/Doroti.Runner.Sdk/Sdk"


def evaluate(folder, target, properties="", items="", default=True):
    project = folder / "icons.proj"
    project.write_text(f'''<Project>
      <PropertyGroup><DorotiTarget>{target}</DorotiTarget>
      <UseMaui>{str(target in ('Android', 'iOS', 'MacCatalyst')).lower()}</UseMaui>
      <DorotiUseDefaultAppIcon>{str(default).lower()}</DorotiUseDefaultAppIcon>{properties}</PropertyGroup>
      <ItemGroup>{items}</ItemGroup>
      <Import Project="{SDK / 'Doroti.AppIcons.targets'}" />
    </Project>''', encoding="utf-8")
    result = subprocess.run(["dotnet", "msbuild", str(project), "-nologo",
        "-getProperty:ApplicationIcon,DorotiMacOSIcon,DorotiLinuxIcon",
        "-getItem:MauiIcon,Content,BundleResource,PartialAppManifest"],
        check=True, capture_output=True, text=True, timeout=1200)
    return json.loads(result.stdout)


scratch_root = ROOT / "temp/testing/app-icons"
scratch_root.mkdir(parents=True, exist_ok=True)
with tempfile.TemporaryDirectory(prefix="contract-", dir=scratch_root) as scratch:
    folder = Path(scratch)
    windows = evaluate(folder, "Windows")
    assert Path(windows["Properties"]["ApplicationIcon"]).is_file()
    assert evaluate(folder, "Windows", "<ApplicationIcon>custom.ico</ApplicationIcon>")["Properties"]["ApplicationIcon"] == "custom.ico"
    for target, scale in [("Android", "0.65"), ("iOS", "0.8"), ("MacCatalyst", "0.8")]:
        icons = evaluate(folder, target)["Items"]["MauiIcon"]
        assert len(icons) == 1 and icons[0]["ForegroundScale"] == scale
        assert icons[0]["Color"] == "#512BD4"
        assert Path(icons[0]["Identity"]).is_file() and Path(icons[0]["ForegroundFile"]).is_file()
        custom = evaluate(folder, target, items='<MauiIcon Include="custom.png" />')["Items"]["MauiIcon"]
        assert len(custom) == 1 and custom[0]["Identity"] == "custom.png"
    mac = evaluate(folder, "macOS")
    assert Path(mac["Properties"]["DorotiMacOSIcon"]).is_file()
    assert mac["Items"]["BundleResource"][0]["LogicalName"] == "appicon.icns"
    assert mac["Items"]["PartialAppManifest"][0]["Overwrite"] == "false"
    linux = evaluate(folder, "Linux")
    assert linux["Items"]["Content"][0]["CopyToPublishDirectory"] == "PreserveNewest"
    assert evaluate(folder, "Linux", "<DorotiLinuxIcon>custom.png</DorotiLinuxIcon>")["Properties"]["DorotiLinuxIcon"] == "custom.png"
    assert evaluate(folder, "macOS", "<DorotiMacOSIcon>custom.icns</DorotiMacOSIcon>")["Properties"]["DorotiMacOSIcon"] == "custom.icns"
    assert evaluate(folder, "Web")["Items"]["Content"][0]["Link"] == "wwwroot/favicon.svg"
    (folder / "wwwroot").mkdir()
    (folder / "wwwroot/favicon.svg").write_text("<svg/>")
    assert not evaluate(folder, "Web")["Items"]["Content"], "Do not shadow a local favicon"
    for target in ["Windows", "Android", "iOS", "MacCatalyst", "macOS", "Linux", "Web"]:
        result = evaluate(folder, target, default=False)
        assert not any(result["Properties"].values()) and not any(result["Items"].values()), target
    local = folder / "Resources/AppIcon"
    local.mkdir(parents=True)
    (local / "appicon.svg").write_text("<svg/>")
    (local / "appiconfg.svg").write_text("<svg/>")
    custom = evaluate(folder, "Android")["Items"]["MauiIcon"]
    assert len(custom) == 1 and custom[0]["Identity"] == "Resources/AppIcon/appicon.svg"

icons = SDK / "Icons"
assert (icons / "favicon.svg").read_bytes() == (ROOT / "Doroti/docs/branding/doroti-app-icon.svg").read_bytes()
background = ET.parse(icons / "appicon.svg").getroot().find("{http://www.w3.org/2000/svg}rect")
assert background.attrib["fill"] == "#512BD4"
for name, size in [("appicon.png", 512), ("appiconfg.png", 1024)]:
    png = (icons / name).read_bytes()
    assert png[:8] == b"\x89PNG\r\n\x1a\n" and struct.unpack_from(">II", png, 16) == (size, size)
ico = (icons / "appicon.ico").read_bytes()
assert struct.unpack_from("<HHH", ico) == (0, 1, 7)
for i, size in enumerate([16, 24, 32, 48, 64, 128, 256]):
    width, height, _, _, planes, bits, length, offset = struct.unpack_from("<BBBBHHII", ico, 6 + 16*i)
    png = ico[offset:offset+length]
    assert (width or 256) == size and (height or 256) == size and planes == 1 and bits == 32
    assert png[:8] == b"\x89PNG\r\n\x1a\n" and struct.unpack_from(">II", png, 16) == (size, size)
icns = (icons / "appicon.icns").read_bytes()
assert icns[:4] == b"icns" and struct.unpack_from(">I", icns, 4)[0] == len(icns)
offset = 8
while offset < len(icns):
    length = struct.unpack_from(">I", icns, offset+4)[0]
    assert length > 8 and offset + length <= len(icns)
    assert icns[offset+8:offset+16] == b"\x89PNG\r\n\x1a\n"
    offset += length
assert offset == len(icns)
if len(sys.argv) > 1:
    with zipfile.ZipFile(sys.argv[1]) as package:
        for source in icons.iterdir():
            assert package.read("Sdk/Icons/" + source.name) == source.read_bytes()
        assert package.read("Sdk/Doroti.AppIcons.targets") == (SDK / "Doroti.AppIcons.targets").read_bytes()
print("PASS: platform icon selection, overrides, opt-out, portable assets and package contents")
