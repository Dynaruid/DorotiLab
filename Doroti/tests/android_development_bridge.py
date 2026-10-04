"""Android session isolation, pre-save delivery, disconnection and port ownership."""
import importlib.util
import json
from pathlib import Path
import subprocess
import tempfile
import sys
import unittest
from unittest.mock import Mock, patch
sys.path.insert(0, str(Path(__file__).parents[1] / 'eng'))
from android_session_ownership import AndroidSessionOwnership

spec = importlib.util.spec_from_file_location('android_development', Path(__file__).parents[1] / 'eng/android-development.py')
development = importlib.util.module_from_spec(spec)
spec.loader.exec_module(development)


class BridgeTests(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        self.directory = Path(self.temporary.name)
        self.session = development.DeviceSession(self.directory, 'session', 'adb', 'phone', 'dev.doroti.testbed')
        self.runtime = dict(schemaVersion='doroti.dev/v1', sessionId='session', runtimeId='runtime',
                            processId=123, supported=True, status='ready', revision=0)
        self.request = dict(schemaVersion='doroti.dev/v1', sessionId='session', runtimeId='runtime', requestId='request')
        self.session.shell = Mock(side_effect=[subprocess.CompletedProcess([], 0, json.dumps(self.runtime).encode()),
            subprocess.CompletedProcess([], 0, b'123'), subprocess.CompletedProcess([], 0, b'')])

    def test_prepared_only_after_atomic_device_delivery(self):
        development.atomic_json(self.directory / 'request.json', self.request)
        self.session.poll()
        self.assertEqual(json.loads((self.directory / 'prepared.json').read_text()), self.request)
        delivery = self.session.shell.call_args_list[2]
        self.assertEqual(json.loads(delivery.kwargs['input']), self.request)
        self.assertIn('mv', delivery.args[1])
        self.assertIn('files/Doroti.Dev/session/request.json', delivery.args[1])

    def test_stale_runtime_request_is_never_delivered(self):
        development.atomic_json(self.directory / 'request.json', {**self.request, 'runtimeId': 'old-runtime'})
        self.session.poll()
        self.assertEqual(self.session.shell.call_count, 2)
        self.assertFalse((self.directory / 'prepared.json').exists())

    def test_failed_delivery_disables_reload_without_acknowledgment(self):
        development.atomic_json(self.directory / 'request.json', self.request)
        self.session.shell.side_effect = [subprocess.CompletedProcess([], 0, json.dumps(self.runtime).encode()),
            subprocess.CompletedProcess([], 0, b'123'), subprocess.CompletedProcess([], 1, b'')]
        self.session.poll()
        self.assertFalse(json.loads((self.directory / 'runtime.json').read_text())['supported'])
        self.assertFalse((self.directory / 'prepared.json').exists())

    def test_malformed_request_does_not_end_running_session(self):
        development.atomic_json(self.directory / 'request.json', [])
        self.session.poll()
        self.assertTrue(json.loads((self.directory / 'runtime.json').read_text())['supported'])
        self.assertEqual(self.session.shell.call_count, 2)

    def test_dead_device_process_disables_stale_status(self):
        self.session.runtime = self.runtime
        self.session.shell.side_effect = [subprocess.CompletedProcess([], 0, json.dumps(self.runtime).encode()),
                                         subprocess.CompletedProcess([], 0, b'456')]
        self.session.poll()
        self.assertFalse(json.loads((self.directory / 'runtime.json').read_text())['supported'])

    def test_stop_removes_only_owned_port(self):
        self.session.ownership = AndroidSessionOwnership(self.directory / 'leases', 'phone', self.session.package, 'session')
        self.session.shell = Mock(return_value=subprocess.CompletedProcess([], 0, b''))
        self.session.port = '12345'; self.session.port_identity = (1, 100)
        (self.directory / 'android-hot-reload-port.txt').write_text('12345')
        with patch.object(development, 'listener_identity', return_value=None): self.session.stop()
        self.assertEqual(self.session.shell.call_args_list[0].args, ('reverse', '--remove', 'tcp:12345'))
        self.assertEqual(self.session.shell.call_count, 1)  # No connected runtime identity: no package kill.

    def test_stale_stop_does_not_remove_reused_host_port(self):
        self.session.ownership = AndroidSessionOwnership(self.directory / 'leases', 'phone', self.session.package, 'session')
        self.session.port='12345'; self.session.port_identity=(1,100)
        (self.directory / 'android-hot-reload-port.txt').write_text('12345')
        self.session.shell = Mock()
        with patch.object(development, 'listener_identity', return_value=(1,200)): self.session.stop()
        self.session.shell.assert_not_called()

    def test_duplicate_device_package_is_rejected_until_owner_releases(self):
        leases = self.directory / 'leases'
        first = AndroidSessionOwnership(leases, 'phone', self.session.package, 'a')
        self.addCleanup(first.release)
        with self.assertRaises(RuntimeError): AndroidSessionOwnership(leases, 'phone', self.session.package, 'b')
        other = AndroidSessionOwnership(leases, 'other-phone', self.session.package, 'b'); other.release()
        other = AndroidSessionOwnership(leases, 'phone', 'dev.doroti.other', 'b'); other.release()
        first.release()
        second = AndroidSessionOwnership(leases, 'phone', self.session.package, 'b'); self.addCleanup(second.release)
        self.session.ownership = first
        self.session.shell = Mock()
        self.session.stop(); self.session.shell.assert_not_called()
        self.assertTrue(second.owns())

    def test_stop_requires_runtime_id_pid_and_process_start_identity(self):
        for change in ('none', 'runtime', 'pid', 'start'):
            lease = AndroidSessionOwnership(self.directory / 'leases', 'phone', self.session.package, 'session')
            self.session.ownership = lease; self.session.runtime = self.runtime; self.session.process_start = b'100'
            current = {**self.runtime, 'runtimeId': 'replacement' if change == 'runtime' else 'runtime'}
            stat = b'123 (app) ' + b' '.join([b'0'] * 19 + [b'200' if change == 'start' else b'100'])
            self.session.shell = Mock(side_effect=[subprocess.CompletedProcess([], 0, json.dumps(current).encode()),
                subprocess.CompletedProcess([], 0, b'456' if change == 'pid' else b'123'),
                subprocess.CompletedProcess([], 0, stat), subprocess.CompletedProcess([], 0, b'')])
            self.session.stop()
            killed = any(call.args == ('shell', 'am', 'force-stop', self.session.package) for call in self.session.shell.call_args_list)
            self.assertEqual(killed, change == 'none')

    def test_ambiguous_or_unauthorized_device_requires_explicit_selection(self):
        with patch.object(subprocess, 'check_output', return_value='List of devices attached\na\tdevice\nb\tdevice\nc\tunauthorized\n'):
            with self.assertRaises(ValueError):
                development.select_device('adb', None)
            with self.assertRaises(ValueError):
                development.select_device('adb', 'c')
            self.assertEqual(development.select_device('adb', 'b'), 'b')


if __name__ == '__main__':
    unittest.main()
