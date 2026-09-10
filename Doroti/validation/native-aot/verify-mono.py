#!/usr/bin/env python3
"""Conservative iOS Mono evidence check; missing native-link evidence is not a pass."""
import argparse
import hashlib
import importlib.util
import json
from pathlib import Path
import plistlib
import re
import subprocess

spec = importlib.util.spec_from_file_location("size_report", Path(__file__).with_name("size-report.py"))
size_report = importlib.util.module_from_spec(spec)
spec.loader.exec_module(size_report)

MONO = re.compile(r"libmonosgen|libmono(?:-2\.0|-component|-ee-interp|-aot|-runtime)|libaot-|mono_aot_module", re.I)
MONO_SYMBOL = re.compile(r"\b_mono_[A-Za-z_0-9]+")


def qualify_bridge_symbols(link_map, runtime_libraries):
    """Attribute legacy Mono-named ABI functions to their actual linked object.

    macios nativeaot-bridge.m implements mono_jit_exec by calling __managed__Main;
    coreclr-bridge.m/runtime.m provide the shared Objective-C bridge. Those are
    NativeAOT glue, not MonoVM. Never exempt an unowned symbol by name alone.
    """
    owners, qualified, rejected = {}, {}, []
    for line in link_map.split("# Dead Stripped Symbols:", 1)[0].splitlines():
        owner = re.match(r"^\[\s*(\d+)\]\s+(.+)$", line)
        if owner:
            owners[owner[1]] = owner[2]
            continue
        entry = re.match(r"^0x[0-9a-fA-F]+\s+0x[0-9a-fA-F]+\s+\[\s*(\d+)\]\s+(_mono_\w+)\s*$", line)
        if not entry:
            continue
        source = owners.get(entry[1], "")
        known = any(source == library + "(" + name + ")"
                    for library in runtime_libraries
                    for name in ("runtime-dotnet-nativeaot.o", "coreclr-bridge-dotnet-nativeaot.o", "nativeaot-bridge-dotnet-nativeaot.o"))
        if known:
            qualified[entry[2]] = source
        else:
            rejected.append(f"Unqualified Mono-named symbol {entry[2]} in {source or 'unknown object'}")
    return qualified, rejected


def macho_uuid(path):
    result = subprocess.run(["xcrun", "dwarfdump", "--uuid", str(path)],
                            capture_output=True, text=True, check=True, timeout=1200)
    values = re.findall(r"UUID: ([0-9A-Fa-f-]+) \(arm64\)", result.stdout)
    return values[0].upper() if len(values) == 1 else None


def check(bundle, publish_result=None, ilc_response=None, link_response=None, link_map=None, native_inputs=None):
    reasons, violations, evidence = [], [], {}
    report = dict(schemaVersion="doroti.mono-evidence/v1", monoAbsent="notVerified",
                  nativeAotPublish="notVerified", functional="notVerified", performance="notVerified",
                  sizeDeltaBytes=None, reasons=reasons, violations=violations, evidence=evidence)
    paths = dict(publishResult=publish_result, ilcResponse=ilc_response, linkMap=link_map)
    paths["nativeInputs" if native_inputs else "linkResponse"] = native_inputs or link_response
    texts = {}
    for name, path in paths.items():
        if path is None or not Path(path).is_file():
            reasons.append(f"Missing {name}")
            continue
        data = Path(path).read_bytes()
        evidence[name] = dict(path=str(Path(path).resolve()), sha256=hashlib.sha256(data).hexdigest())
        texts[name] = data.decode(errors="replace")
    native = {}
    if "nativeInputs" in texts:
        for line in texts["nativeInputs"].splitlines():
            if "=" in line:
                name, value = line.split("=", 1)
                native.setdefault(name, []).append(value)
        for name, expected in dict(RuntimeIdentifier="ios-arm64", PublishAot="true", UseNativeAot="true", UseMonoRuntime="false", MtouchInterpreter="").items():
            if native.get(name) != [expected]:
                reasons.append(f"Native input selection mismatch: {name}")
        if not native.get("RuntimeLibrary") or any("libxamarin-dotnet-nativeaot.a" not in value for value in native["RuntimeLibrary"]):
            reasons.append("Native inputs do not identify the NativeAOT Xamarin platform glue")
        evidence["nativeFiles"] = []
        for name in ("Object", "RuntimeLibrary", "BindingLibrary", "MainLibrary", "NativeReference", "LinkerFlag"):
            for value in native.get(name, []):
                path = Path(value)
                if name == "LinkerFlag" and not path.is_absolute():
                    continue
                if not path.is_file():
                    reasons.append(f"Missing native input: {value}")
                    continue
                with path.open("rb") as stream:
                    sha256 = size_report.digest(stream)
                evidence["nativeFiles"].append(dict(path=str(path), bytes=path.stat().st_size, sha256=sha256))
    bridge_symbols, rejected_symbols = qualify_bridge_symbols(texts.get("linkMap", ""), native.get("RuntimeLibrary", []))
    evidence["nativeAotBridgeSymbols"] = bridge_symbols
    violations.extend(rejected_symbols)
    if bundle is None or not Path(bundle).is_dir():
        reasons.append("Missing final .app bundle")
    else:
        bundle = Path(bundle).resolve()
        inventory = size_report.inventory(bundle)
        report["bundle"] = inventory
        for item in inventory["files"]:
            if MONO.search(item["path"]):
                violations.append(f"Mono payload: {item['path']}")
        plist = bundle / "Info.plist"
        if not plist.is_file():
            reasons.append("Missing Info.plist")
        else:
            info = plistlib.loads(plist.read_bytes())
            executable_name = info.get("CFBundleExecutable", "")
            executable = bundle / executable_name
            report["bundleId"] = info.get("CFBundleIdentifier")
            if not executable_name or Path(executable_name).name != executable_name or not executable.is_file():
                reasons.append("Missing or invalid bundle executable")
            else:
                report["executableSha256"] = hashlib.sha256(executable.read_bytes()).hexdigest()
                for command in (["xcrun", "nm", "-g", str(executable)], ["xcrun", "otool", "-L", str(executable)]):
                    try:
                        result = subprocess.run(command, capture_output=True, text=True, timeout=1200)
                        if result.returncode:
                            reasons.append(f"Cannot inspect executable with {command[1]}")
                        else:
                            evidence[command[1]] = result.stdout
                            violations.extend(MONO.findall(result.stdout))
                            violations.extend(f"Unqualified executable symbol: {symbol}"
                                              for symbol in MONO_SYMBOL.findall(result.stdout) if symbol not in bridge_symbols)
                    except (OSError, subprocess.TimeoutExpired):
                        reasons.append(f"Cannot inspect executable with {command[1]}")
                if native:
                    linked = Path(native.get("NativeExecutable", [""])[0])
                    map_path = re.search(r"^# Path: (.+)$", texts.get("linkMap", ""), re.M)
                    if not linked.is_file() or not map_path or Path(map_path[1]) != linked:
                        reasons.append("Link map does not match the recorded native executable")
                    else:
                        try:
                            final_uuid, linked_uuid = macho_uuid(executable), macho_uuid(linked)
                            evidence["executableUuids"] = dict(bundle=final_uuid, linked=linked_uuid)
                            if not final_uuid or final_uuid != linked_uuid:
                                reasons.append("Linked and bundled Mach-O UUIDs differ")
                        except (OSError, subprocess.SubprocessError):
                            reasons.append("Cannot bind the native link to the final Mach-O UUID")
                elif "linkMap" in texts and str(executable) not in texts["linkMap"]:
                    reasons.append("Link map is not tied to the final executable path")
    for name in ("linkResponse", "linkMap", "nativeInputs"):
        if name in texts:
            violations.extend(f"{name}: {match}" for match in MONO.findall(texts[name]))
    if "publishResult" in texts:
        result = json.loads(texts["publishResult"])
        command = result.get("command", [])
        successful = result.get("exitCode") == 0 and not result.get("timedOut")
        if not successful:
            report["nativeAotPublish"] = "fail"
            reasons.append("Publish failed or timed out")
        elif len(command) < 2 or Path(command[0]).name != "dotnet" or command[1] != "publish":
            reasons.append("Evidence command is not dotnet publish")
        else:
            log = Path(result.get("log", ""))
            if not log.is_file():
                reasons.append("Missing publish execution log")
            else:
                output = log.read_text(errors="replace")
                if not re.search(r'[/\\]ilc["\s].*@', output):
                    reasons.append("No actual ILC command in the execution log")
                elif "ilcResponse" not in evidence or evidence["ilcResponse"]["path"] not in output:
                    reasons.append("ILC response does not match the publish execution")
                else:
                    report["nativeAotPublish"] = "pass"
                if native:
                    if (evidence["nativeInputs"]["path"] not in output or
                            native.get("NativeExecutable", [""])[0] not in output or
                            "LinkNativeCode" not in output or not re.search(r"clang|ld[ .\"]", output)):
                        reasons.append("No matching native linker execution in the diagnostic publish log")
                elif "linkResponse" not in evidence or evidence["linkResponse"]["path"] not in output or not re.search(r"clang|ld[ .\"]", output):
                    reasons.append("No matching native linker command in the execution log")
                if re.search(r"(?:warning|error) IL[23]\d+:", output):
                    reasons.append("Unresolved AOT/trim diagnostics in the publish log")
                evidence["publishLog"] = dict(path=str(log), sha256=hashlib.sha256(log.read_bytes()).hexdigest())
    # A feature flag or a stripped binary with no 'mono' strings is insufficient.
    if "ilcResponse" in texts and not re.search(r"runtime\.nativeaot\.ios-arm64", texts["ilcResponse"], re.I):
        reasons.append("ILC inputs do not identify the iOS NativeAOT runtime pack")
    if "linkResponse" in texts and not re.search(r"native[/\\].*\.o", texts["linkResponse"], re.I):
        reasons.append("Native link inputs do not identify the compiled ILC object")
    if native:
        native_object = native.get("NativeObject", [""])[0]
        if not native_object or native_object not in native.get("Object", []) or f"-o:{native_object}" not in texts.get("ilcResponse", ""):
            reasons.append("ILC output object does not match native link inputs")
    if violations:
        report["monoAbsent"] = "fail"
    elif not reasons and report["nativeAotPublish"] == "pass":
        report["monoAbsent"] = "pass"
    return report


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--bundle", type=Path)
    parser.add_argument("--publish-result", type=Path)
    parser.add_argument("--ilc-response", type=Path)
    parser.add_argument("--link-response", type=Path)
    parser.add_argument("--native-inputs", type=Path, help="SDK LinkNativeCode task inputs collected by native-evidence.targets")
    parser.add_argument("--link-map", type=Path)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    report = check(args.bundle, args.publish_result, args.ilc_response, args.link_response, args.link_map, args.native_inputs)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2, ensure_ascii=False) + "\n")
    print(f"monoAbsent={report['monoAbsent']} nativeAotPublish={report['nativeAotPublish']}")
    raise SystemExit(0 if report["monoAbsent"] == "pass" else 1)


if __name__ == "__main__":
    main()
