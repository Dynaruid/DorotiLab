"""Short semantics-located input probe for the testbed gallery after lifecycle.py."""
import argparse
import json
from pathlib import Path
import re
import subprocess
import time
import xml.etree.ElementTree as ET


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--serial", required=True)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument("--material-only", action="store_true")
    parser.add_argument("--template-only", action="store_true")
    args = parser.parse_args()
    args.output.mkdir(parents=True, exist_ok=True)
    commands = []

    def adb(*parts):
        command = ["adb", "-s", args.serial, *map(str, parts)]
        result = subprocess.run(command, capture_output=True, timeout=1200, check=True)
        commands.append(command)
        (args.output / "commands.json").write_text(json.dumps(commands, indent=2), encoding="utf-8")
        return result.stdout

    def capture(label):
        remote = "/sdcard/doroti-input-" + str(time.time_ns()) + ".xml"
        adb("shell", "uiautomator", "dump", remote)
        xml = adb("shell", "cat", remote)
        adb("shell", "rm", remote)
        (args.output / (label + ".xml")).write_bytes(xml)
        (args.output / (label + ".png")).write_bytes(adb("exec-out", "screencap", "-p"))
        return ET.fromstring(xml)

    def tap(tree, label):
        node = next(n for n in tree.iter("node") if label in (n.get("text"), n.get("content-desc")))
        left, top, right, bottom = map(int, re.findall(r"\d+", node.get("bounds")))
        adb("shell", "input", "tap", (left + right) // 2, (top + bottom) // 2)
        time.sleep(.6)

    since = adb("shell", "date", "+%s.%N").decode().strip()
    tree = capture("before")
    if args.template_only:
        tap(tree, "Increment")
        tree = capture("increment")
        assert any(n.get("text") == "Custom SkSL count: 1" for n in tree.iter("node")), "Template counter did not repaint"
        (args.output / "result.json").write_text(json.dumps(dict(status="PASS-automated-template-input",
            device=args.serial, count=1, physicalInput=False), indent=2), encoding="utf-8")
        return
    if args.material_only:
        tap(tree, "Color\nTab 2 of 4")
        tree = capture("color")
        assert any("Primary" in n.get("text", "") for n in tree.iter("node")), "Color content not shown"
        tap(tree, "Components\nTab 1 of 4")
        tree = capture("components")
        adb("shell", "input", "swipe", "520", "1750", "520", "850", "400")
        time.sleep(1)
        after = capture("scrolled")
        assert ET.tostring(tree) != ET.tostring(after), "Scroll did not change visible content"
        (args.output / "result.json").write_text(json.dumps(dict(status="PASS-automated-material-input",
            device=args.serial, colorTab=True, componentsTab=True, scroll=True, physicalInput=False), indent=2), encoding="utf-8")
        return
    tap(tree, "G6 Material button")
    tree = capture("button")
    assert any(n.get("content-desc") == "G6 Material button 1" for n in tree.iter("node"))
    tap(tree, "Text field")
    adb("shell", "input", "text", "work0")
    time.sleep(1)
    tree = capture("text-keyboard")
    assert any("work0" in n.get("text", "") for n in tree.iter("node")), "Text did not reach the field"
    adb("shell", "input", "keyevent", "KEYCODE_BACK")
    time.sleep(1)
    tree = capture("text-committed")
    tap(tree, "Open Material sample")
    time.sleep(2)
    tree = capture("material")
    texts = [n.get("text") or n.get("content-desc") for n in tree.iter("node")]
    assert any(value and "Components" in value for value in texts), "Material screen not shown"
    pid = adb("shell", "pidof", "dev.doroti.testbed").decode().strip()
    log = adb("logcat", "-d", "-T", since, "--pid=" + pid)
    (args.output / "process.log").write_bytes(log)
    assert not re.search(rb"FATAL EXCEPTION|Fatal signal| E DorotiGraphite", log)
    (args.output / "result.json").write_text(json.dumps(dict(status="PASS-automated-input",
        device=args.serial, buttonCount=1, text="work0", materialOpened=True,
        koreanComposition="notVerified", physicalInput=False), indent=2), encoding="utf-8")


if __name__ == "__main__":
    main()
