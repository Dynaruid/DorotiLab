"""Read host listener identity so a stale session cannot remove a reused ADB port."""
import ctypes
import os
from pathlib import Path
import subprocess
import sys

def listener_identity(port):
    try:
        if os.name == 'nt':
            output = subprocess.check_output(['netstat', '-ano', '-p', 'tcp'], text=True, timeout=5)
            pids = {int(fields[-1]) for line in output.splitlines() if len(fields := line.split()) >= 5
                    and fields[1].endswith(':' + str(port)) and fields[-2] == 'LISTENING'}
            if len(pids) != 1: return None
            pid = pids.pop()
            from ctypes import wintypes
            kernel = ctypes.WinDLL('kernel32', use_last_error=True)
            kernel.OpenProcess.argtypes=[wintypes.DWORD,wintypes.BOOL,wintypes.DWORD]; kernel.OpenProcess.restype=wintypes.HANDLE
            kernel.GetProcessTimes.argtypes=[wintypes.HANDLE,ctypes.POINTER(wintypes.FILETIME),ctypes.POINTER(wintypes.FILETIME),ctypes.POINTER(wintypes.FILETIME),ctypes.POINTER(wintypes.FILETIME)]
            kernel.CloseHandle.argtypes=[wintypes.HANDLE]
            handle=kernel.OpenProcess(0x1000,False,pid)
            if not handle: return None
            try:
                times=[wintypes.FILETIME() for _ in range(4)]
                if not kernel.GetProcessTimes(handle, *(ctypes.byref(time) for time in times)): return None
                return (pid, (times[0].dwHighDateTime << 32) | times[0].dwLowDateTime)
            finally: kernel.CloseHandle(handle)
        if sys.platform == 'darwin':
            output = subprocess.check_output(['lsof', '-nP', '-t', '-iTCP:' + str(port), '-sTCP:LISTEN'], text=True, timeout=5)
            pids = {int(line) for line in output.splitlines() if line.isdecimal()}
            if len(pids) != 1: return None
            pid = pids.pop()
            started = subprocess.check_output(['ps', '-p', str(pid), '-o', 'lstart='], text=True, timeout=5).strip()
            return (pid, started) if started else None
        inodes=set()
        for name in ('tcp','tcp6'):
            for line in Path('/proc/net/' + name).read_text().splitlines()[1:]:
                fields=line.split()
                if fields[3]=='0A' and int(fields[1].rsplit(':',1)[1],16)==port: inodes.add(fields[9])
        for directory in Path('/proc').iterdir():
            if not directory.name.isdecimal(): continue
            try:
                if any(os.readlink(fd) in {'socket:['+inode+']' for inode in inodes} for fd in (directory/'fd').iterdir()):
                    return (int(directory.name), (directory/'stat').read_text().rsplit(')',1)[1].split()[19])
            except (OSError, ValueError): continue
    except (OSError, ValueError, subprocess.SubprocessError): pass
    return None
