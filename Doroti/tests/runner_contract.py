"""Keep failure propagation and bounded execution from silently regressing."""
import importlib.util
from pathlib import Path
import sys
import shutil
import subprocess

spec = importlib.util.spec_from_file_location("timeout_runner", Path(__file__).parents[1] / "eng/run-with-timeout.py")
runner = importlib.util.module_from_spec(spec)
spec.loader.exec_module(runner)
assert runner.run([sys.executable, "-c", "raise SystemExit(7)"]) == 7
assert runner.run([sys.executable, "-c", "import time; time.sleep(60)"], timeout=0.2) == 124
assert runner.run([sys.executable, "-c", "pass"]) == 0
print("Runner failure, timeout, success: PASS")
if len(sys.argv) > 1:
    scratch = Path(sys.argv[1]).resolve()
    root = Path(__file__).resolve().parents[2]
    assert scratch.is_relative_to(root / "temp/testing")
    eng = scratch / "Doroti/eng"; eng.mkdir(parents=True)
    shutil.copyfile(root / "Doroti/eng/run-with-timeout.py", eng / "run-with-timeout.py")
    validation = eng / "validate.py"
    validation.write_text("import sys\nprint('fixture suite='+sys.argv[1],flush=True)\nraise SystemExit(7)\n")
    project = scratch / "Doroti/src/Fixture"; project.mkdir(parents=True)
    source = root / "Doroti/src/Doroti.Tooling/RepositoryCommands.cs"
    sdk = root / "Doroti/src/Doroti.Tooling.Extension.Sdk/Doroti.Tooling.Extension.Sdk.csproj"
    (project / 'Fixture.csproj').write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Compile Include="{source}" Link="RepositoryCommands.cs"/><ProjectReference Include="{sdk}"/></ItemGroup></Project>')
    (project / 'Program.cs').write_text('return await Doroti.Tooling.RepositoryCommands.TryRunAsync(args) ?? 77;')
    built=subprocess.run(['dotnet','build',str(project/'Fixture.csproj'),'-o',str(project/'bin')],capture_output=True,text=True,timeout=1200)
    assert built.returncode==0,built.stdout+built.stderr
    executable=project/'bin/Fixture.dll'
    for verb,suite in (("validate","Developer"),("audit","Source"),("release","Release")):
        result=subprocess.run([sys.executable,str(root/'Doroti/eng/run-with-timeout.py'),'--timeout','30','dotnet',str(executable),verb],capture_output=True,text=True,timeout=40)
        assert result.returncode==7 and 'fixture suite='+suite in result.stdout,(verb,result.stdout,result.stderr)
    validation.unlink()
    result=subprocess.run([sys.executable,str(root/'Doroti/eng/run-with-timeout.py'),'--timeout','30','dotnet',str(executable),'validate'],capture_output=True,text=True,timeout=40)
    assert result.returncode==1 and 'missing-validation-runner' in result.stderr,(result.returncode,result.stdout,result.stderr)
    print('Managed CLI validate/audit/release exit propagation, suite selection and missing runner: PASS')
