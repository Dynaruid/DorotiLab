"""Isolated SDK clean/no-op/edit/remove/rename consumer checks."""
from pathlib import Path
import subprocess
import json
import time
import os

ROOT = Path(__file__).resolve().parents[3]
WORK = ROOT / 'Doroti/artifacts/gpu-effects' / f'sdk-{time.time_ns()}'
WORK.mkdir(parents=True)
(WORK / 'src').mkdir()
(WORK / 'Program.cs').write_text('// platform-neutral library\n', encoding='utf-8')
SDK = ROOT / 'Doroti/src/Doroti.App.Sdk/Sdk'
UI = ROOT / 'Doroti/src/Doroti.Ui/Doroti.Ui.csproj'
TOOL = ROOT / 'tools/Doroti.Wgsl/target/debug/doroti-wgsl.exe'
SOURCE = (ROOT / 'DorotiTestbedApp/assets/effects/swap.wgsl').read_text()
DEFINITION = json.loads((ROOT / 'DorotiTestbedApp/assets/effects/swap.effect.json').read_text())
DEFINITION['requiredBackends'] = ['vulkan-fragment']

def project(name):
    item = f'<DorotiGpuEffect Include="{name}.wgsl" Definition="{name}.effect.json" />' if name else ''
    (WORK / 'Consumer.csproj').write_text(f'''<Project>
<Import Project="{SDK / 'Sdk.props'}" />
<PropertyGroup><DorotiWgslTool>{TOOL}</DorotiWgslTool></PropertyGroup>
<ItemGroup>{item}<ProjectReference Include="{UI}" /></ItemGroup>
<Import Project="{SDK / 'Sdk.targets'}" />
</Project>''', encoding='utf-8')

def build(expected=True):
    result = subprocess.run(['dotnet', 'build', str(WORK / 'Consumer.csproj'), '-v:q'],
                            cwd=WORK, text=True, capture_output=True, encoding='utf-8', errors='replace', timeout=1200)
    with (WORK / 'build.log').open('a', encoding='utf-8') as log:
        log.write(result.stdout + result.stderr)
    assert (result.returncode == 0) == expected, result.stdout + result.stderr
    return result

(WORK / 'swap.wgsl').write_text(SOURCE, encoding='utf-8')
(WORK / 'swap.effect.json').write_text(json.dumps(DEFINITION), encoding='utf-8')
(WORK / 'src/Consumer.cs').write_text('class Consumer { object Value => Doroti.Generated.Effects.SwapChannels; }', encoding='utf-8')
project('swap')
build()
generated = next((WORK / 'obj').rglob('Parameters.g.cs'))
before = generated.stat().st_mtime_ns
build()
assert generated.stat().st_mtime_ns == before, 'No-op build regenerated WGSL'
(WORK / 'swap.wgsl').write_text(SOURCE.replace('.bgra', '.rgba'), encoding='utf-8')
build()
assert generated.stat().st_mtime_ns != before, 'Shader edit did not regenerate code'
(WORK / 'swap.wgsl').rename(WORK / 'renamed.wgsl')
(WORK / 'swap.effect.json').rename(WORK / 'renamed.effect.json')
project('renamed')
build()
assert any('renamed' in str(p) for p in (WORK / 'obj').rglob('Parameters.g.cs'))
project(None)
build(expected=False)  # Referencing the removed effect must fail, even with stale obj files.
(WORK / 'src/Consumer.cs').write_text('class Consumer {}', encoding='utf-8')
build()
project('renamed')
project_file = WORK / 'Consumer.csproj'
project_file.write_text(project_file.read_text().replace('<PropertyGroup>', '<PropertyGroup><OutputType>Exe</OutputType>'), encoding='utf-8')
uniform = 'struct P { a: f32, v: vec3<f32>, m: mat3x3<f32>, values: array<vec4<u32>, 2>, signed: i32 }\n@group(1) @binding(0) var<uniform> p: P;\n'
(WORK / 'renamed.wgsl').write_text(uniform + SOURCE, encoding='utf-8')
(WORK / 'Program.cs').write_text('''using System;
class Program {
 static void Main() {
  var actual = new Doroti.Generated.SwapChannelsParameters {
   p_a = 1.25f, p_v_0 = 2.5f, p_v_2 = -3.5f, p_m_0_0 = 4.5f,
   p_m_1_0 = 5.5f, p_m_2_2 = 7.5f, p_values_0_0 = 23, p_values_1_3 = 0xdeadbeef, p_signed = -42
  }.Snapshot().Bytes;
  var expected = new byte[128];
  BitConverter.GetBytes(1.25f).CopyTo(expected, 0);
  BitConverter.GetBytes(2.5f).CopyTo(expected, 16);
  BitConverter.GetBytes(-3.5f).CopyTo(expected, 24);
  BitConverter.GetBytes(4.5f).CopyTo(expected, 32);
  BitConverter.GetBytes(5.5f).CopyTo(expected, 48);
  BitConverter.GetBytes(7.5f).CopyTo(expected, 72);
  BitConverter.GetBytes((uint)23).CopyTo(expected, 80);
  BitConverter.GetBytes(0xdeadbeefu).CopyTo(expected, 108);
  BitConverter.GetBytes(-42).CopyTo(expected, 112);
  if (!actual.SequenceEqual(expected)) throw new Exception("Generated C# serializer golden failed");
  Console.WriteLine("PASS generated C# 128-byte scalar/vector/matrix/array/padding golden");
 }
}''', encoding='utf-8')
build()
subprocess.run(['dotnet', 'run', '--project', str(project_file), '--no-build'], cwd=WORK, check=True, timeout=1200)
stub = WORK / ('empty-compiler.cmd' if os.name == 'nt' else 'empty-compiler')
stub.write_text('@exit /b 0\n' if os.name == 'nt' else '#!/bin/sh\nexit 0\n', encoding='utf-8')
stub.chmod(0o755)
project_file.write_text(project_file.read_text().replace(str(TOOL), str(stub)), encoding='utf-8')
(WORK / 'renamed.wgsl').write_text(uniform + SOURCE + '\n', encoding='utf-8')
failure = build(expected=False)
assert 'DOROTIWGSL102' in failure.stdout + failure.stderr, 'Empty compiler success was not diagnosed'
print(json.dumps({'status': 'PASS', 'cases': ['clean', 'no-op', 'edit', 'rename', 'remove-rejects-stale-code', 'remove-clean-build', 'generated-csharp-byte-golden', 'empty-compiler-rejects-stale-outputs'], 'output': str(WORK)}))
