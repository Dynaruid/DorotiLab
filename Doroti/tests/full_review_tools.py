"""Real MSBuild deletion guards, Linux installation failures and candidate receipts."""
import hashlib
import importlib.util
import json
import os
from pathlib import Path
import platform
import subprocess
import sys
import unittest
import uuid
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / 'Doroti/eng'))
from release_receipt import receipt_environment, validate_receipt
spec = importlib.util.spec_from_file_location('linux_installer', ROOT / 'Doroti/eng/install-linux-qt.py')
installer = importlib.util.module_from_spec(spec); spec.loader.exec_module(installer)
RUN = Path(os.environ.get('DOROTI_FULL_REVIEW_TEST_ROOT', str(ROOT / 'temp/testing/full-review/tools'))) / uuid.uuid4().hex
RUN.mkdir(parents=True)
print('Evidence: ' + str(RUN), flush=True)

class ToolTests(unittest.TestCase):
    def test_msbuild_build_and_clean_preserve_unsafe_sentinels(self):
        host = ('win32' if os.name == 'nt' else 'darwin' if sys.platform == 'darwin' else 'linux')
        arch = 'arm64' if platform.machine().lower() in ('arm64', 'aarch64') else 'x64'
        compiler = Path(os.environ.get('NUGET_PACKAGES', str(Path.home() / '.nuget/packages'))) / f'microsoft.typescript.msbuild/7.0.0/tools/runtimes/{host}-{arch}/tsc{ ".exe" if os.name == "nt" else ""}'
        self.assertTrue(compiler.is_file(), 'Restore Microsoft.TypeScript.MSBuild 7.0.0 before this consumer test.')
        directory = RUN / 'MSBuild 한글'; directory.mkdir()
        (directory / 'source').mkdir(); (directory / 'source/main.ts').write_text('export const x = 1;')
        (directory / 'tsconfig.json').write_text(json.dumps({'compilerOptions': {'target': 'ES2022'}, 'include': ['source/*.ts']}))
        targets = ROOT / 'Doroti/src/Doroti.App.Sdk/Sdk/Doroti.TypeScript.targets'
        project = directory / 'consumer.proj'
        project.write_text(f'''<Project><PropertyGroup><BaseIntermediateOutputPath>obj/</BaseIntermediateOutputPath>
<DorotiWebTypeScript>true</DorotiWebTypeScript><TypeScriptMSBuildVersion>7.0.0</TypeScriptMSBuildVersion><DorotiTypeScriptVersion>7.0.0</DorotiTypeScriptVersion>
<DorotiTypeScriptConfig>{directory / 'tsconfig.json'}</DorotiTypeScriptConfig><DorotiTypeScriptSourceRoot>{directory / 'source'}</DorotiTypeScriptSourceRoot>
<DorotiTypeScriptCompilerExecutable>{compiler}</DorotiTypeScriptCompilerExecutable><DorotiTypeScriptExpectedOutput>$(DorotiTypeScriptOutputRoot)/main.js</DorotiTypeScriptExpectedOutput></PropertyGroup>
<Import Project="{targets}"/><Target Name="Build" DependsOnTargets="DorotiCompileTypeScript"/><Target Name="Clean"/></Project>''', encoding='utf-8')
        cases = [('obj', False), ('obj-sibling/generated', False), ('obj/../../outside', False), ('obj/generated', True), (str(directory / 'obj/absolute'), True)]
        if os.name != 'nt': cases.append(('OBJ/generated', False))
        outside = directory / 'external'; outside.mkdir(); (outside / 'sentinel').write_bytes(b'external\x00keep')
        link = directory / 'obj/link'; link.parent.mkdir(exist_ok=True)
        if os.name == 'nt':
            script = directory / 'junction.ps1'; script.write_text('param($LinkPath,$TargetPath)\nNew-Item -ItemType Junction -Path $LinkPath -Target $TargetPath | Out-Null')
            subprocess.run(['pwsh', '-NoProfile', '-File', str(script), str(link), str(outside)], check=True, timeout=30)
        else: link.symlink_to(outside, target_is_directory=True)
        cases.append(('obj/link/generated', False))
        for target in ('Build', 'Clean'):
            for index, (output, expected) in enumerate(cases):
                path = Path(output) if Path(output).is_absolute() else directory / output
                path.mkdir(parents=True, exist_ok=True); sentinel = path / 'sentinel'; sentinel.write_bytes(b'keep\x00\xff')
                result = subprocess.run(['dotnet', 'msbuild', str(project), '-t:' + target, '-nologo', '-p:DorotiTypeScriptOutputRoot=' + output], cwd=directory, capture_output=True, timeout=60)
                (directory / f'{target}-{index}.log').write_bytes(result.stdout + result.stderr)
                self.assertEqual(result.returncode == 0, expected, (output, target, result.stdout[-2000:]))
                if not expected: self.assertEqual(sentinel.read_bytes(), b'keep\x00\xff')
                self.assertEqual((outside / 'sentinel').read_bytes(), b'external\x00keep')

    @unittest.skipUnless(sys.platform.startswith('linux'), 'Requires a real Linux symlink/case filesystem.')
    def test_linux_install_publish_failures_recover_and_preserve_userdata(self):
        for failure_name in ('version', 'run.new', 'application.desktop.new', 'current.new'):
            for after in (False, True):
                directory = RUN / f'install-{failure_name}-{after}'; directory.mkdir()
                candidate = directory / 'candidate'; (candidate / 'publish').mkdir(parents=True)
                executable = candidate / 'publish/app'; executable.write_bytes(b'#!/bin/sh\nexit 0\n'); executable.chmod(0o755)
                def manifest(version):
                    (candidate / 'candidate.json').write_text(json.dumps(dict(version=version, payload={'app': hashlib.sha256(executable.read_bytes()).hexdigest()})))
                install_root = directory / 'installed'
                manifest('1.0.0'); installer.install(candidate, install_root, 'app')
                (install_root / 'userdata').mkdir(); (install_root / 'userdata/sentinel').write_bytes(b'user keep')
                manifest('1.0.1'); real_replace = os.replace; injected = False
                def replace(source, target):
                    nonlocal injected
                    name = Path(source).name
                    matches = name == failure_name or failure_name == 'version' and name == 'payload'
                    if matches and not injected:
                        injected = True
                        if after: real_replace(source, target)
                        raise OSError('injected publish failure')
                    return real_replace(source, target)
                with patch.object(installer.os, 'replace', replace):
                    with self.assertRaises(OSError): installer.install(candidate, install_root, 'app')
                self.assertEqual(os.readlink(install_root / 'current'), 'versions/1.0.0')
                installer.install(candidate, install_root, 'app'); self.assertEqual(os.readlink(install_root / 'current'), 'versions/1.0.1')
                installer.remove(install_root); self.assertEqual((install_root / 'userdata/sentinel').read_bytes(), b'user keep')
        # A foreign temporary symlink is never a recoverable installer artifact.
        install_root = RUN / 'tampered'; installer.owned(install_root)
        (install_root / 'current.new').symlink_to('/tmp')
        with self.assertRaises(ValueError): installer.remove(install_root)

    def test_receipt_rejects_missing_corrupt_stale_and_wrong_candidate(self):
        path = RUN / 'native.json'; environment = receipt_environment({'DOROTI_RELEASE_RECEIPT': 'ambient'}, path, 'run', '1.0.0')
        self.assertEqual(environment['DOROTI_RELEASE_RECEIPT'], str(path.resolve()))
        with self.assertRaises(OSError): validate_receipt(path, 'run', '1.0.0')
        path.write_text('{')
        with self.assertRaises(ValueError): validate_receipt(path, 'run', '1.0.0')
        value = dict(nativeFirstFrame=True, twoWindows=True, secondResizedAndClosed=True, survivorsBeforeMainClose=1, runId='run', version='1.0.0')
        path.write_text(json.dumps(value)); self.assertEqual(validate_receipt(path, 'run', '1.0.0'), value)
        for key, wrong in [('runId', 'stale'), ('version', 'other'), ('survivorsBeforeMainClose', 2), ('nativeFirstFrame', False)]:
            path.write_text(json.dumps({**value, key: wrong}))
            with self.assertRaises(ValueError): validate_receipt(path, 'run', '1.0.0')

if __name__ == '__main__': unittest.main()
