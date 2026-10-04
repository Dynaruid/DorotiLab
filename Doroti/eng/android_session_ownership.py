"""Kernel-held device/package lease. A crash releases the lock; stale Stops cannot use a new lease."""
import hashlib
import json
import os
from pathlib import Path
import uuid

class AndroidSessionOwnership:
    def __init__(self, directory, device, package, session_id):
        directory = Path(directory)
        directory.mkdir(parents=True, exist_ok=True)
        key = hashlib.sha256((device + '\0' + package).encode()).hexdigest()
        self.path = directory / (key + '.json')
        self.stream = (directory / (key + '.lock')).open('a+b')
        try:
            self.stream.seek(0)
            if not self.stream.read(1): self.stream.write(b'0'); self.stream.flush()
            self.stream.seek(0)
            if os.name == 'nt':
                import msvcrt
                msvcrt.locking(self.stream.fileno(), msvcrt.LK_NBLCK, 1)
            else:
                import fcntl
                fcntl.flock(self.stream.fileno(), fcntl.LOCK_EX | fcntl.LOCK_NB)
        except OSError:
            self.stream.close()
            raise RuntimeError('This Android device/application already has an active development session.')
        self.token = uuid.uuid4().hex
        self.released = False
        self.path.write_text(json.dumps(dict(token=self.token, sessionId=session_id, device=device,
                                             applicationId=package, hostPid=os.getpid())), encoding='utf-8')

    def owns(self):
        try: return not self.released and json.loads(self.path.read_text(encoding='utf-8')).get('token') == self.token
        except (OSError, ValueError): return False

    def release(self):
        if self.released: return
        self.released = True
        self.stream.close()  # The lock inode stays stable for concurrent contenders.
