"""Compile and execute the production IMM editing state with MSVC (no OS IME)."""
from pathlib import Path
import os
import subprocess

ROOT = Path(__file__).resolve().parents[2]
out = ROOT / 'temp/testing/windows-web-text-input/native-contract'
out.mkdir(parents=True, exist_ok=True)
vswhere = Path(os.environ['ProgramFiles(x86)']) / 'Microsoft Visual Studio/Installer/vswhere.exe'
vs = subprocess.check_output([str(vswhere), '-latest', '-products', '*', '-requires',
                              'Microsoft.VisualStudio.Component.VC.Tools.x86.x64', '-property', 'installationPath'], text=True).strip()
vcvars = Path(vs) / 'VC/Auxiliary/Build/vcvars64.bat'
source = ROOT / 'Doroti/tests/native/windows_ime_contract.cpp'
exe = out / 'windows_ime_contract.exe'
command = f'@call "{vcvars}" >nul\n@cl /nologo /std:c++20 /EHsc /W4 /WX /O2 "{source}" /Fe:"{exe}" /Fo:"{out / "contract.obj"}"\n@exit /b %errorlevel%\n'
(out / 'build.cmd').write_text(command, encoding='utf-8')
subprocess.run(['cmd', '/d', '/c', 'build.cmd'], check=True, cwd=out)
subprocess.run([str(exe)], check=True, cwd=out)
