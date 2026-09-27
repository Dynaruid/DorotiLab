from pathlib import Path
import subprocess
font = Path('Doroti/src/Doroti.Skia.Fonts/.doroti/fonts/Roboto-regular.ttf')
original = font.read_bytes()
base = ['dotnet','msbuild','Doroti/src/Doroti.Skia.Fonts/Doroti.Skia.Fonts.csproj','-t:DorotiRestoreNativeFonts','-v:quiet','-nologo']
try:
 font.write_bytes(b'font-cache-corruption-test')
 failed = subprocess.run(base + ['-p:DorotiNativeFontsOffline=true'], capture_output=True, text=True)
 assert failed.returncode != 0 and 'DOROTIFONT001' in failed.stdout + failed.stderr
 print('PASS offline mode rejects corrupted cached font')
 subprocess.run(base, check=True)
 assert font.read_bytes() == original
 stamp = font.stat().st_mtime_ns
 subprocess.run(base + ['-p:DorotiNativeFontsOffline=true'], check=True)
 assert font.stat().st_mtime_ns == stamp
 print('PASS online repair and unchanged offline cache reuse')
finally:
 if font.read_bytes() != original: font.write_bytes(original)
