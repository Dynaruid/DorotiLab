"""Exercise the real HTTP status relay without an Apple device."""
import importlib.util
import json
from pathlib import Path
import tempfile
import threading
import unittest
from unittest.mock import patch
import urllib.error
import urllib.request

spec = importlib.util.spec_from_file_location('ios_development', Path(__file__).parents[1] / 'eng/ios-development.py')
bridge = importlib.util.module_from_spec(spec)
spec.loader.exec_module(bridge)


class BridgeTests(unittest.TestCase):
    def test_device_delivery_and_disconnect(self):
        with tempfile.TemporaryDirectory() as directory:
            session = bridge.DeviceSession(directory, 'session', 'device')
            session.bundle_id = 'dev.test'
            runtime = dict(schemaVersion='doroti.dev/v1', sessionId='session', runtimeId='runtime',
                           processId=123, supported=True, status='ready', revision=0)
            request = dict(sessionId='session', runtimeId='runtime', requestId='request')
            bridge.atomic_json(Path(directory) / 'device-runtime.json', runtime)
            bridge.atomic_json(Path(directory) / 'request.json', request)
            alive = json.dumps({'result': {'runningProcesses': [{'processIdentifier': 123}]}})
            with patch.object(session, 'copy', side_effect=lambda direction, *_: direction == 'from'), \
                 patch.object(bridge.subprocess, 'check_output', return_value=alive):
                session.poll()
            self.assertFalse((Path(directory) / 'prepared.json').exists())
            with patch.object(session, 'copy', return_value=True), \
                 patch.object(bridge.subprocess, 'check_output', return_value=alive):
                session.poll()
            self.assertEqual(json.loads((Path(directory) / 'prepared.json').read_text()), request)
            with patch.object(session, 'copy', return_value=True), \
                 patch.object(bridge.subprocess, 'check_output', return_value='{"result":{"runningProcesses":[]}}'):
                session.poll()
            self.assertFalse(json.loads((Path(directory) / 'runtime.json').read_text())['supported'])

    def test_device_cleanup_matches_installed_bundle_container(self):
        apps = {'result': {'apps': [{'bundleIdentifier': 'dev.test', 'url': 'file:///app-id/Test.app/'}]}}
        processes = {'result': {'runningProcesses': [
            {'executable': 'file:///app-id/Test.app/Test', 'processIdentifier': 123},
            {'executable': 'file:///other-id/Test.app/Test', 'processIdentifier': 456}]}}
        with patch.object(bridge.subprocess, 'check_output', side_effect=[json.dumps(apps), json.dumps(processes)]), \
             patch.object(bridge.subprocess, 'run') as signal:
            bridge.stop_device_app('device', 'dev.test')
        self.assertEqual(signal.call_count, 1)
        self.assertIn('123', signal.call_args.args[0])
        self.assertNotIn('456', signal.call_args.args[0])

    def test_request_requires_runtime_ack_and_stale_sessions_are_rejected(self):
        with tempfile.TemporaryDirectory() as directory:
            session = bridge.Session(directory, 'session')
            server = bridge.make_server(session)
            threading.Thread(target=server.serve_forever, daemon=True).start()
            runtime = dict(schemaVersion='doroti.dev/v1', sessionId='session', runtimeId='runtime',
                           supported=True, status='ready', revision=0)

            def send(value=runtime, token=None, prepared=None):
                headers = {'Authorization': 'Bearer ' + (token or session.token)}
                if prepared:
                    headers['X-Doroti-Prepared'] = prepared
                request = urllib.request.Request(f'http://127.0.0.1:{server.server_port}/session',
                    data=json.dumps(value).encode(), headers=headers)
                with urllib.request.urlopen(request, timeout=5) as response:
                    return json.load(response)

            try:
                with self.assertRaises(urllib.error.HTTPError) as error:
                    send(token='wrong')
                self.assertEqual(error.exception.code, 403)
                with self.assertRaises(urllib.error.HTTPError):
                    send({**runtime, 'sessionId': 'other'})
                self.assertEqual(send(), {})
                request = dict(sessionId='session', runtimeId='runtime', requestId='request')
                bridge.atomic_json(Path(directory) / 'request.json', request)
                self.assertEqual(send(), request)
                self.assertFalse((Path(directory) / 'prepared.json').exists())
                send(prepared='request')
                self.assertEqual(json.loads((Path(directory) / 'prepared.json').read_text()), request)
                # A request accepted by an old runtime must never be applied to its replacement.
                self.assertEqual(send({**runtime, 'runtimeId': 'new-runtime'}), {})
                session.last_seen -= 6
                session.expire()
                self.assertFalse(json.loads((Path(directory) / 'runtime.json').read_text())['supported'])
                send()
                self.assertTrue(json.loads((Path(directory) / 'runtime.json').read_text())['supported'])
                session.expire(force=True)
                self.assertEqual(json.loads((Path(directory) / 'runtime.json').read_text())['status'], 'closed')
            finally:
                server.shutdown()
                server.server_close()


if __name__ == '__main__':
    unittest.main()
