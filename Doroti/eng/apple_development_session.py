"""Process-held Apple development ownership and session-matched graceful Stop."""
import hashlib
import json
import os
from pathlib import Path
import re


def stop_requested(directory, session_id):
    try:
        value = json.loads((Path(directory) / 'stop.json').read_text(encoding='utf-8-sig'))
        return isinstance(value, dict) and value.get('sessionId') == session_id
    except (OSError, ValueError):
        return False


class AppleDevelopmentSession:
    def __init__(self, directory, session_id, lease_directory=None):
        if not re.fullmatch(r'[A-Za-z0-9-]{1,80}', session_id):
            raise ValueError('Invalid Apple development session ID')
        self.directory = Path(directory).resolve()
        self.session_id = session_id
        self.lease_directory = Path(lease_directory) if lease_directory else Path.home() / '.doroti/apple-owners'
        self.locks = []

    def claim(self, identity):
        self.lease_directory.mkdir(parents=True, exist_ok=True)
        name = hashlib.sha256(identity.encode('utf-8')).hexdigest() + '.lock'
        stream = (self.lease_directory / name).open('a+b')
        try:
            if os.name == 'nt':
                import msvcrt
                if stream.seek(0, 2) == 0:
                    stream.write(b'\0')
                    stream.flush()
                stream.seek(0)
                msvcrt.locking(stream.fileno(), msvcrt.LK_NBLCK, 1)
            else:
                import fcntl
                fcntl.flock(stream, fcntl.LOCK_EX | fcntl.LOCK_NB)
        except OSError as error:
            stream.close()
            raise RuntimeError('Another Apple development session owns this workspace or device/application') from error
        self.locks.append(stream)

    def claim_device(self, device, bundle_id):
        self.claim('device\0' + device + '\0' + bundle_id)

    def __enter__(self):
        self.claim('session\0' + str(self.directory))
        try:
            self.directory.mkdir(parents=True, exist_ok=True)
        except BaseException:
            self.__exit__()
            raise
        return self

    def __exit__(self, *_):
        for stream in reversed(self.locks):
            if os.name == 'nt':
                import msvcrt
                stream.seek(0)
                msvcrt.locking(stream.fileno(), msvcrt.LK_UNLCK, 1)
            stream.close()
        self.locks.clear()
