"""Apple adapter planning/ownership and isolated workspace regressions; no native execution."""
import importlib.util
import json
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
from types import SimpleNamespace
import unittest
import uuid
import zipfile
from unittest.mock import MagicMock, patch
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / 'Doroti/eng'))
from apple_development_session import AppleDevelopmentSession, stop_requested
from apple_development_fixture import create


def load(name):
    spec = importlib.util.spec_from_file_location(name.replace('-', '_'), ROOT / 'Doroti/eng' / (name + '.py'))
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


mac = load('mac-development')
ios = load('ios-development')


class AppleDevelopmentContracts(unittest.TestCase):
    def test_stop_is_session_matched_and_malformed_input_is_ignored(self):
        with tempfile.TemporaryDirectory() as raw:
            directory = Path(raw)
            for payload in ['broken', '[]', '{"sessionId":"old"}']:
                (directory / 'stop.json').write_text(payload)
                self.assertFalse(stop_requested(directory, 'new'))
            (directory / 'stop.json').write_text('{"sessionId":"new"}')
            self.assertTrue(stop_requested(directory, 'new'))

    def test_directory_and_device_leases_exclude_concurrent_owners_and_release(self):
        with tempfile.TemporaryDirectory() as raw:
            root = Path(raw)
            with AppleDevelopmentSession(root / 'one', 'one', root / 'leases') as owner:
                owner.claim_device('phone', 'dev.test')
                with self.assertRaises(RuntimeError):
                    with AppleDevelopmentSession(root / 'one', 'other', root / 'leases'):
                        pass
                with AppleDevelopmentSession(root / 'two', 'two', root / 'leases') as other:
                    with self.assertRaises(RuntimeError):
                        other.claim_device('phone', 'dev.test')
                    other.claim_device('phone', 'dev.other')
            with AppleDevelopmentSession(root / 'one', 'next', root / 'leases') as next_owner:
                next_owner.claim_device('phone', 'dev.test')
            with self.assertRaises(ValueError):
                AppleDevelopmentSession(root / 'one', '../wrong', root / 'leases')

    def mac_arguments(self, root, platform='macos', rid='osx-arm64'):
        return SimpleNamespace(runner=str(root / 'runner/App.csproj'), app_root=str(root),
            working_directory=root / 'runner', platform=platform, rid=rid, framework='net10.0-' + ('macos27.0' if platform == 'macos' else 'maccatalyst27.0'),
            session_directory=root / 'session', session_id='mac-session', dotnet='selected-dotnet')

    def test_mac_adapter_preserves_profile_sdk_directory_and_graceful_stop(self):
        for platform, rid in [('macos', 'osx-arm64'), ('maccatalyst', 'maccatalyst-arm64')]:
            with self.subTest(platform=platform), tempfile.TemporaryDirectory() as raw:
                root = Path(raw); args = self.mac_arguments(root, platform, rid)
                args.session_directory.mkdir()
                (args.session_directory / 'runtime.json').write_text('{"sessionId":"mac-session","supported":true}')
                child = MagicMock(pid=123, returncode=0); child.poll.return_value = None
                with patch.object(mac.shutil, 'which', return_value='selected-dotnet'), \
                     patch.object(mac.subprocess, 'check_output', side_effect=['10.0.401', rid]), \
                     patch.object(mac.subprocess, 'Popen', return_value=child) as start, \
                     patch.object(mac.signal, 'signal'), patch.object(mac.signal, 'SIGKILL', 9, create=True), \
                     patch.object(mac.os, 'killpg', create=True), patch.object(mac, 'stop_requested', return_value=True):
                    self.assertEqual(mac.run(args), 0)
                command = start.call_args.args[0]
                self.assertIn('--property:DorotiMacDevelopment=true', command)
                self.assertIn('--property:DorotiHostTargetFrameworks=' + args.framework, command)
                self.assertIn('--property:DorotiHostRuntimeIdentifier=' + rid, command)
                self.assertIn('--property:EnableAotAnalyzer=false', command)
                self.assertIn('--property:EnableTrimAnalyzer=false', command)
                self.assertNotIn('--runtime', command)
                self.assertFalse(any(value.split(':', 1)[-1].startswith('RuntimeIdentifier=') for value in command))
                self.assertEqual(start.call_args.kwargs['cwd'], args.working_directory)
                self.assertEqual(start.call_args.kwargs['env']['DOROTI_DEV_SESSION_ID'], 'mac-session')
                self.assertEqual(json.loads((args.session_directory / 'runtime.json').read_text())['status'], 'closed')
                child.wait.assert_called()

    def test_mac_adapter_rejects_runner_rid_mismatch_before_start(self):
        with tempfile.TemporaryDirectory() as raw:
            args = self.mac_arguments(Path(raw))
            with patch.object(mac.shutil, 'which', return_value='dotnet'), \
                 patch.object(mac.subprocess, 'check_output', side_effect=['10.0.401', 'osx-x64']), \
                 patch.object(mac.subprocess, 'Popen') as start:
                with self.assertRaisesRegex(ValueError, 'runner RID'):
                    mac.run(args)
                start.assert_not_called()

    def ios_arguments(self, root, physical=False):
        return SimpleNamespace(runner=str(root / 'runner/App.csproj'), app_root=str(root),
            working_directory=root / 'runner', rid='ios-arm64' if physical else 'iossimulator-arm64',
            framework='net11.0-ios' if physical else 'net10.0-ios27.0', sdk_version=None,
            session_directory=str(root / 'session'), session_id='ios-session', device='AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
            dotnet='selected-dotnet', xcrun='selected-xcrun', host=None)

    def test_ios_adapter_preserves_declared_profile_custom_tool_and_owned_cleanup(self):
        for physical in [False, True]:
            with self.subTest(physical=physical), tempfile.TemporaryDirectory() as raw:
                args = self.ios_arguments(Path(raw), physical)
                properties = {'Properties': dict(ApplicationId='dev.test', RuntimeIdentifier=args.rid,
                    DorotiIosDevelopmentRuntimeVersion='11.0.0-rc.2.26504.105')}
                child = MagicMock(pid=123, returncode=0); child.poll.return_value = None
                owner = MagicMock()
                with patch.object(ios.shutil, 'which', return_value='selected-dotnet'), \
                     patch.object(ios.subprocess, 'check_output', side_effect=['11.0.100-rc.2.26504.105' if physical else '10.0.401', json.dumps(properties)]), \
                     patch.object(ios.subprocess, 'Popen', return_value=child) as start, \
                     patch.object(ios.subprocess, 'run') as native_stop, patch.object(ios, 'stop_device_app') as device_stop, \
                     patch.object(ios.signal, 'signal'), patch.object(ios.signal, 'SIGKILL', 9, create=True), \
                     patch.object(ios.os, 'killpg', create=True), patch.object(ios, 'stop_requested', return_value=True):
                    self.assertEqual(ios.run(args, owner), 0)
                command = start.call_args.args[0]
                self.assertIn('--property:DorotiIosDevelopment=true', command)
                self.assertIn('--property:DorotiHostTargetFrameworks=' + args.framework, command)
                self.assertIn('--property:DorotiHostRuntimeIdentifier=' + args.rid, command)
                self.assertIn('--property:EnableAotAnalyzer=false', command)
                self.assertIn('--property:EnableTrimAnalyzer=false', command)
                self.assertIn('--property:DorotiCompilationMode=' + ('CoreClr' if physical else 'Mono'), command)
                self.assertNotIn('--runtime', command)
                self.assertEqual(start.call_args.kwargs['cwd'], args.working_directory)
                owner.claim_device.assert_called_once_with(args.device, 'dev.test')
                if physical:
                    device_stop.assert_called_once_with(args.device, 'dev.test', 'selected-xcrun', expected_pid=None)
                else:
                    self.assertEqual(native_stop.call_args.args[0][:3], ['selected-xcrun', 'simctl', 'terminate'])

    def test_ios_cleanup_does_not_signal_a_replacement_pid(self):
        apps = {'result': {'apps': [{'bundleIdentifier': 'dev.test', 'url': 'file:///app/Test.app/'}]}}
        processes = {'result': {'runningProcesses': [{'executable': 'file:///app/Test.app/Test', 'processIdentifier': 456}]}}
        with patch.object(ios.subprocess, 'check_output', side_effect=[json.dumps(apps), json.dumps(processes)]), \
             patch.object(ios.subprocess, 'run') as signal:
            ios.stop_device_app('device', 'dev.test', expected_pid=123)
        signal.assert_not_called()

    def test_fixture_declares_tfm_rid_and_preserves_external_source_references(self):
        run_root = ROOT / 'temp/testing/apple-development-contract'
        run_root.mkdir(parents=True, exist_ok=True)
        with tempfile.TemporaryDirectory(dir=run_root) as raw:
            for platform, framework, rid in [('ios', 'net10.0-ios27.0', 'iossimulator-arm64'),
                    ('ios', 'net11.0-ios', 'ios-arm64'), ('macos', 'net10.0-macos27.0', 'osx-arm64')]:
                workspace = create(Path(raw) / (platform + rid), platform, framework, rid)
                manifest = json.loads((workspace / 'doroti-workspace.json').read_text())
                entry = manifest['platforms'][platform]
                self.assertEqual((entry['targetFramework'], entry['runtimeIdentifier'], entry['profile']), (framework, rid, 'default:' + rid))
                self.assertTrue((workspace / entry['providerManifest']).is_file())
                runner = workspace / entry['runner']; tree = ET.parse(runner)
                self.assertEqual(tree.find('./PropertyGroup/RuntimeIdentifier').text, rid)
                self.assertEqual(json.loads((runner.parent / 'global.json').read_text())['sdk']['version'].split('.')[0], '11' if framework.startswith('net11.') else '10')
                app = ET.parse(workspace / manifest['applicationProject'])
                references = [node.get('Include') for node in app.iter('ProjectReference')]
                self.assertTrue(all(Path(path).is_file() for path in references), references)
                self.assertTrue((workspace / 'src/MaterialSample/HotReloadSample.cs').is_file())
                self.assertTrue(all((runner.parent / node.get('Project')).is_file() for node in tree.iter('Import') if not node.get('Sdk')))

    def test_real_tool_package_contains_adapter_closure_and_matching_capabilities(self):
        run = ROOT / 'temp/testing/apple-development-package' / uuid.uuid4().hex
        run.mkdir(parents=True)
        success = False
        try:
            result = subprocess.run([sys.executable, str(ROOT / 'Doroti/eng/run-with-timeout.py'), '--timeout', '1200',
                'dotnet', 'pack', str(ROOT / 'packages/platforms/maui/tooling/Doroti.Tool.Maui.csproj'), '-c', 'Release', '--nologo', '-o', str(run)],
                cwd=ROOT, capture_output=True, text=True)
            (run / 'pack.log').write_text(result.stdout + result.stderr, encoding='utf-8')
            self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
            with zipfile.ZipFile(next(run.glob('Doroti.Tool.Maui.*.nupkg'))) as archive:
                for name in ['ios-development.py', 'mac-development.py', 'apple_development_session.py']:
                    self.assertEqual(archive.read('lib/net10.0/apple/' + name), (ROOT / 'Doroti/eng' / name).read_bytes(), name)
                descriptor = json.loads(archive.read('lib/net10.0/doroti-provider.json'))
                for name, profile in descriptor['profiles'].items():
                    if name.startswith(('default:ios', 'default:osx-', 'default:maccatalyst-')):
                        self.assertIn('dev', profile['operations'], name)
                        self.assertTrue(profile['development']['debug'] and profile['development']['usesStopSignal'], name)
                        if name.startswith('default:ios'):
                            self.assertTrue(profile['development']['requiresPreparedAcknowledgment'], name)
            success = True
        finally:
            if success:
                assert run.resolve().is_relative_to((ROOT / 'temp/testing').resolve()) and not run.is_symlink()
                shutil.rmtree(run)
            else:
                print('Package failure evidence retained:', run, file=sys.stderr)


if __name__ == '__main__':
    unittest.main()
