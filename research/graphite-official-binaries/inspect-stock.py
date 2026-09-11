"""Read-only package/API inspection; no Skia compilation or GPU context creation.

Run from the repository root with the required external timeout:
python Doroti/validation/run-with-timeout.py python research/graphite-official-binaries/inspect-stock.py
"""
import base64
import ctypes
import hashlib
import json
import os
from pathlib import Path
import subprocess
import urllib.request
import zipfile
from datetime import datetime, timezone

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[1]
VERSION = "4.154.0-preview.1.26454.9"
SKIASHARP = "143a933a753dbfeca1909524b2c06c546c5c3e20"
SKIA = "cc43af052d3d98e605bee4ddc98671dafded1c57"
CACHE = Path(os.environ.get("NUGET_PACKAGES", str(Path.home() / ".nuget/packages")))


def fetch(url):
    with urllib.request.urlopen(url, timeout=60) as response:
        return response.read()


def sha(data):
    return hashlib.sha256(data).hexdigest()


def main():
    report = {
        "schema": "doroti.stock-graphite-research/v1",
        "observedAtUtc": datetime.now(timezone.utc).isoformat(),
        "repositoryHead": subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip(),
        "packageVersion": VERSION,
        "skiaSharpRevision": SKIASHARP,
        "skiaRevision": SKIA,
        "scope": "package integrity, exported API and Windows compiled-backend availability only",
        "gpuContextCreated": False,
        "productQualified": False,
        "sources": [],
        "packages": [],
    }
    index_url = "https://api.nuget.org/v3-flatcontainer/skiasharp/index.json"
    versions = json.loads(fetch(index_url))["versions"]
    report["nugetIndex"] = {"url": index_url, "recentVersions": versions[-8:], "pinnedVersionListed": VERSION in versions}
    paths = {
        "mono/SkiaSharp": [
            "binding/SkiaSharp/Gpu/Graphite/SKGraphiteContext.cs",
            "binding/SkiaSharp/Gpu/Graphite/SKGraphiteVkBackendContext.cs",
            "binding/SkiaSharp/Gpu/Graphite/SKGraphiteBackendTexture.cs",
            "tests/VulkanTests/Visual/GraphiteVulkanRenderer.cs",
            "tests/Tests/SkiaSharp/Visual/Renderers/GraphiteDawnRenderer.cs",
        ],
        "mono/skia": [
            "include/c/sk_graphite.h", "include/c/sk_graphite_vulkan.h",
            "src/c/sk_graphite_vulkan.cpp", "include/gpu/vk/VulkanBackendContext.h",
            "src/gpu/graphite/vk/VulkanSharedContext.cpp",
            "src/gpu/graphite/vk/VulkanCommandBuffer.cpp",
            "src/gpu/graphite/vk/VulkanRenderPass.cpp",
            "src/gpu/graphite/vk/VulkanTexture.cpp",
        ],
    }
    for repo, items in paths.items():
        revision = SKIASHARP if repo.endswith("SkiaSharp") else SKIA
        for path in items:
            url = f"https://raw.githubusercontent.com/{repo}/{revision}/{path}"
            data = fetch(url)
            report["sources"].append({"url": url, "sha256": sha(data), "bytes": len(data)})
    for package, rid, extension in [
        ("skiasharp.nativeassets.win32", "win-x64", "dll"),
        ("skiasharp.nativeassets.android", "android-arm64", "so"),
        ("skiasharp.nativeassets.android", "android-x64", "so"),
    ]:
        directory = CACHE / package / VERSION
        archive = directory / f"{package}.{VERSION}.nupkg"
        entry = f"runtimes/{rid}/native/libSkiaSharp.{extension}"
        asset = directory / entry
        payload = archive.read_bytes()
        archive_sha512 = base64.b64encode(hashlib.sha512(payload).digest()).decode()
        stored_sha512 = (directory / f"{package}.{VERSION}.nupkg.sha512").read_text().strip()
        with zipfile.ZipFile(archive) as zipped:
            original = zipped.read(entry)
        local = asset.read_bytes()
        metadata = json.loads((directory / ".nupkg.metadata").read_text())
        record = {
            "package": package, "rid": rid, "path": str(asset),
            "packageSource": metadata.get("source"),
            "archiveMatchesCachedSha512": archive_sha512 == stored_sha512,
            "installedAssetMatchesArchive": local == original,
            "assetSha256": sha(local), "bytes": len(local),
            "remoteArchiveReDownloaded": False,
        }
        assert record["archiveMatchesCachedSha512"] and record["installedAssetMatchesArchive"]
        report["packages"].append(record)
        if rid == "win-x64":
            # Absolute NuGet asset, in this short-lived process only.
            library = ctypes.CDLL(str(asset))
            symbols = [
                "sk_graphite_backend_is_available", "sk_graphite_context_make_vulkan",
                "sk_graphite_vk_backend_texture_new", "sk_graphite_context_insert_recording",
                "sk_graphite_context_submit", "sk_graphite_context_check_async_work_completion",
                "sk_graphite_context_async_rescale_and_read_pixels_surface",
                "doroti_graphite_interop_version", "doroti_graphite_vk_context_create",
                "doroti_graphite_vk_texture_get_state", "doroti_graphite_vk_texture_set_state",
                "doroti_graphite_vk_insert_recording", "doroti_graphite_vk_context_report_device_lost",
                "doroti_graphite_has_unfinished_gpu_work",
            ]
            record["exports"] = {symbol: hasattr(library, symbol) for symbol in symbols}
            available = library.sk_graphite_backend_is_available
            available.argtypes = [ctypes.c_int]
            available.restype = ctypes.c_bool
            record["compiledBackends"] = {name: bool(available(value)) for name, value in [("Dawn", 0), ("Metal", 1), ("Vulkan", 2)]}
    output = HERE / "evidence.json"
    output.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({"report": str(output), "packages": report["packages"], "sourceCount": len(report["sources"]), "gpuContextCreated": False}, indent=2))


if __name__ == "__main__":
    main()
