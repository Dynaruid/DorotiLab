"""Qt read grants reject FIFOs/devices/directories without blocking or loading contents."""
import ctypes
import os
from pathlib import Path
import sys

root = Path(sys.argv[2]).resolve()
assert root.is_relative_to(Path(__file__).resolve().parents[2] / 'temp/testing')
root.mkdir(parents=True, exist_ok=False)
lib = ctypes.CDLL(str(Path(sys.argv[1]).resolve()))
open_file = lib.doroti_qt_open_read_file_v1
open_file.argtypes = [ctypes.c_char_p]; open_file.restype = ctypes.c_int
small = root / '한글.txt'; small.write_text('한글')
fd = open_file(os.fsencode(small)); assert fd >= 0
try:
    assert os.read(fd, 16).decode() == '한글'
    assert not os.get_inheritable(fd)
finally: os.close(fd)
fifo = root / 'fifo'; os.mkfifo(fifo)
assert open_file(os.fsencode(fifo)) == -22
assert open_file(os.fsencode(root)) == -22
assert open_file(b'/dev/null') == -22
small.chmod(0)
try:
    if os.geteuid() != 0: assert open_file(os.fsencode(small)) == -13
finally: small.chmod(0o600)
print('PASS regular-file grants: Unicode, CLOEXEC, FIFO/device/directory and permission rejection.')
