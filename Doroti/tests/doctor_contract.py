"""Execute the real doctor CLI with disposable workspaces and a local fake tool executable."""
import json
import os
from pathlib import Path
import subprocess
import sys
import unittest
import uuid
import shutil

ROOT = Path(__file__).resolve().parents[2]
RUN = ROOT / 'temp/testing/full-review/doctor-contract' / uuid.uuid4().hex
RUN.mkdir(parents=True)

class DoctorTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.tool = RUN / '도구 with space'; cls.tool.mkdir()
        (cls.tool / 'Fake.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings></PropertyGroup></Project>')
        (cls.tool / 'Program.cs').write_text('''using System.Text.Json;
var spec = JsonDocument.Parse(File.ReadAllText(Environment.GetEnvironmentVariable("DOROTI_FAKE_SPEC")!)).RootElement;
string Get(string name, string fallback) => spec.TryGetProperty(name, out var value) ? value.GetString()! : fallback;
File.AppendAllText(Environment.GetEnvironmentVariable("DOROTI_FAKE_LOG")!, JsonSerializer.Serialize(new {cwd=Environment.CurrentDirectory,args})+"\\n");
if (args.Contains("--version")) {
 if (Get("hang", "") == "true") Thread.Sleep(30000);
 var version = Get("version", "10.0.400");
 if (Get("selectGlobal", "") == "true") for (var d=new DirectoryInfo(Environment.CurrentDirectory); d is not null; d=d.Parent) {
  var path=Path.Combine(d.FullName,"global.json"); if (File.Exists(path)) {version=JsonDocument.Parse(File.ReadAllText(path)).RootElement.GetProperty("sdk").GetProperty("version").GetString()!; break;}
 }
 Console.WriteLine(version); return;
}
if (args.Contains("--info")) { Console.Error.Write(new string('e',100000)); Console.WriteLine("SDK Base Path: fake SDK / architecture x64"); return; }
if (args.Contains("--list-sdks")) { Console.WriteLine("10.0.400 [fake SDK]"); return; }
if (args.Contains("workload")) { Console.WriteLine(Get("workloads", "wasm-tools 10.0.400 SDK 10.0.400\\nios 10.0.400 SDK\\nmaui-windows 10.0.400 SDK\\nandroid 10.0.400 SDK")); return; }
if (args.Contains("msbuild")) {
 if (Get("evaluation", "") == "fail") { Console.Error.WriteLine("unrestored import"); Environment.Exit(1); }
 var project=args[Array.IndexOf(args,"msbuild")+1];
 var text=File.ReadAllText(project);
 var props=new Dictionary<string,string>{{"TargetFramework","net10.0"},{"RuntimeIdentifier","browser-wasm"},{"WasmBuildNative","true"},{"WasmEnableThreads","true"},
 {"NETCoreSdkVersion","10.0.400"},{"TypeScriptMSBuildVersion","7.0.0"},{"DorotiTypeScriptVersion","7.0.0"},{"DorotiTypeScriptCompilerExecutable",Get("compiler", "")},
 {"DorotiHostKind",text.Contains("<DorotiHostKind>Maui</DorotiHostKind>")?"Maui":"BlazorWebAssembly"}};
 if (spec.TryGetProperty("properties",out var overrides)) foreach(var item in overrides.EnumerateObject()) props[item.Name]=item.Value.GetString()!;
 Console.WriteLine(JsonSerializer.Serialize(new {Properties=props, Items=new {ProjectReference=Array.Empty<object>(),PackageReference=Array.Empty<object>(),DorotiGpuEffect=Array.Empty<object>()}})); return;
}
Console.WriteLine("fake tool");''', encoding='utf-8')
        result = subprocess.run(['dotnet', 'build', str(cls.tool / 'Fake.csproj'), '-nologo', '-o', str(cls.tool / 'bin')], cwd=ROOT, capture_output=True, timeout=120)
        if result.returncode: raise RuntimeError(result.stdout.decode(errors='replace'))
        cls.executable = cls.tool / ('bin/Fake.exe' if os.name == 'nt' else 'bin/Fake')
        cls.compiler = cls.tool / 'compiler'; cls.compiler.write_text('local compiler fixture')

    def workspace(self):
        directory = RUN / ('workspace 한글 ' + uuid.uuid4().hex[:8]); directory.mkdir()
        paths={'web':'web/Web.csproj','ios':'ios/iOS.csproj','windows':'windows/Default.csproj','android':'android/Android.csproj','linux':'linux/Linux.csproj'}
        (directory / 'App.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"/>')
        for relative in [*paths.values(),'windows/maui/Maui.csproj','android/binding/Native.csproj']:
            path=directory / relative; path.parent.mkdir(parents=True,exist_ok=True)
            path.write_text('<Project><PropertyGroup><DorotiHostKind>' + ('Maui' if 'Maui.csproj' in relative else 'test') + '</DorotiHostKind></PropertyGroup>' +
                ('<ItemGroup><DorotiNativeBindingProject Include="binding/Native.csproj"/></ItemGroup>' if relative==paths['android'] else '') + '</Project>')
        (directory / 'android/native').mkdir()
        (directory / 'doroti-workspace.json').write_text(json.dumps(dict(schemaVersion='doroti.workspace/v1',applicationProject='App.csproj',platforms=paths)))
        (directory / 'global.json').write_text(json.dumps(dict(sdk=dict(version='10.0.400',rollForward='latestPatch',allowPrerelease=False))))
        return directory

    def run_doctor(self, workspace=None, spec=None, arguments=(), expected=None, native=False):
        directory=RUN / ('report-'+uuid.uuid4().hex[:8]); directory.mkdir()
        fixture={ 'compiler':str(self.compiler), **(spec or {}) }
        specification=directory / 'fake.json'; specification.write_text(json.dumps(fixture))
        log=directory / 'calls.jsonl'
        environment={**os.environ, 'DOROTI_FAKE_SPEC':str(specification),'DOROTI_FAKE_LOG':str(log)}
        command=['pwsh','-NoProfile','-File',str(ROOT / 'Doroti/eng/doroti.ps1'),*(['native','doctor'] if native else ['doctor']),
            '-DotnetPath',str(self.executable),'-DoctorReportDirectory',str(directory),'-DoctorProbeTimeoutSeconds','1']
        if workspace is not None:
            command+=['-App',str(workspace)]
            if '-Platform' not in arguments: command+=['-Platform','web']
        command+=list(arguments)
        result=subprocess.run(command,cwd=ROOT,env=environment,capture_output=True,timeout=45)
        (directory / 'console.log').write_bytes(result.stdout+result.stderr)
        report=json.loads((directory / 'doctor.json').read_text(encoding='utf-8'))
        self.assertEqual(report['schemaVersion'],'doroti.doctor/v4')
        self.assertEqual(result.returncode==0,report['success'])
        self.assertIn('**'+report['status']+'**',(directory / 'doctor.md').read_text(encoding='utf-8'))
        if expected: self.assertEqual(report['status'],expected,report['checks'])
        calls=[json.loads(line) for line in log.read_text().splitlines()] if log.exists() else []
        return report,calls

    def test_generic_web_custom_dotnet_needs_no_node_or_rust_and_drains_both_pipes(self):
        report,calls=self.run_doctor(self.workspace(),expected='PASS')
        self.assertFalse(any(check['id'] in ('node','cargo','rust') for check in report['checks']))
        self.assertEqual(report['context']['targets'][0]['dotnetPath'],str(self.executable))
        self.assertFalse(any('restore' in call['args'] or 'build' in call['args'] for call in calls))
        info=next(check for check in report['checks'] if check['id']=='web.sdk-info')
        self.assertIn('[truncated]',info['evidence'])

    def test_sdk_feature_band_prerelease_and_rollforward(self):
        for selected,policy,expected in [('10.0.399','latestPatch','FAIL'),('10.0.500','latestPatch','FAIL'),
                ('10.0.401-preview.1','latestPatch','FAIL'),('10.0.401','latestPatch','PASS'),('10.0.500','latestFeature','PASS'),('10.1.100','latestMinor','PASS')]:
            workspace=self.workspace(); (workspace / 'global.json').write_text(json.dumps(dict(sdk=dict(version='10.0.400',rollForward=policy,allowPrerelease=False))))
            self.run_doctor(workspace,dict(version=selected),expected=expected)

    def test_workload_missing_and_unrestored_import_are_distinct(self):
        self.run_doctor(self.workspace(),dict(workloads=''),expected='FAIL')
        report,_=self.run_doctor(self.workspace(),dict(evaluation='fail'),expected='PARTIAL')
        check=next(check for check in report['checks'] if check['id']=='web.project-evaluation')
        self.assertEqual(check['reason'],'nonzeroExit')

    def test_ios_uses_runner_global_json_without_forcing_testbed_sdk(self):
        workspace=self.workspace(); (workspace / 'ios/global.json').write_text(json.dumps(dict(sdk=dict(version='11.0.100-preview.1',rollForward='disable',allowPrerelease=True))))
        report,calls=self.run_doctor(workspace,dict(selectGlobal='true'),arguments=('-Platform','ios'),expected='PARTIAL' if sys.platform!='darwin' else None)
        target=report['context']['targets'][0]
        self.assertEqual(target['cwd'],str(workspace / 'ios')); self.assertEqual(target['sdk'],'11.0.100-preview.1')
        self.assertTrue(all(call['cwd']==str(workspace / 'ios') for call in calls))

    def test_maui_backend_selects_actual_runner(self):
        report,_=self.run_doctor(self.workspace(),arguments=('-Platform','windows','-WindowsBackend','Maui'),expected='PASS' if os.name=='nt' else 'PARTIAL')
        self.assertTrue(report['context']['targets'][0]['runner'].endswith('Maui.csproj'))
        self.assertFalse(any(check['id']=='windows.vswhere' for check in report['checks']))

    def test_timeout_keeps_report_and_continues_independent_probes(self):
        report,calls=self.run_doctor(spec=dict(hang='true'),expected='FAIL')
        self.assertEqual(next(check for check in report['checks'] if check['id']=='common.sdk')['reason'],'timeout')
        self.assertTrue(any('--info' in call['args'] for call in calls))

    def test_cancellation_has_terminal_report_without_starting_unneeded_tools(self):
        cancel=RUN / 'cancel.txt'; cancel.write_text('cancel')
        report,calls=self.run_doctor(arguments=('-DoctorCancellationFile',str(cancel)),expected='FAIL')
        self.assertEqual(calls,[])
        self.assertEqual(next(check for check in report['checks'] if check['id']=='common.sdk')['reason'],'canceled')

    def test_invalid_workspace_native_and_all_never_report_full_pass(self):
        self.run_doctor(RUN / 'missing',expected='FAIL')
        report,_=self.run_doctor(self.workspace(),arguments=('-Platform','all'),expected=None)
        self.assertNotEqual(report['status'],'PASS')
        report,_=self.run_doctor(self.workspace(),arguments=('-Platform','web','-Rid','linux-x64'),expected='FAIL')
        self.assertTrue(any('RID' in str(check['actual']) for check in report['checks']))

    def test_native_android_reports_missing_wrapper_without_downloading(self):
        report,calls=self.run_doctor(self.workspace(),spec=dict(properties={'DorotiHostKind':'Android'}),
            arguments=('-Platform','android'),expected='FAIL',native=True)
        self.assertTrue(report['context']['nativeBinding'])
        self.assertEqual(next(check for check in report['checks'] if check['id']=='android.gradle-wrapper')['status'],'FAIL')
        self.assertFalse(any('restore' in call['args'] or 'build' in call['args'] for call in calls))

    def test_android_build_does_not_require_a_connected_device(self):
        report,_=self.run_doctor(self.workspace(),spec=dict(properties={'DorotiHostKind':'Android'}),
            arguments=('-Platform','android','-DoctorProfile','build','-Device','absent-device'))
        self.assertEqual(report['context']['device'],'absent-device')
        self.assertFalse(any(check['id']=='android.selected-device' for check in report['checks']))

    def test_python_resolution_python3_only_python_only_and_missing(self):
        pwsh=shutil.which('pwsh')
        helper=str(ROOT / 'Doroti/eng/python-tools.ps1').replace("'","''")
        for name in ('python3','python',None):
            directory=RUN / ('interpreter '+(name or 'missing')); directory.mkdir()
            if name:
                executable=directory / (name+('.exe' if os.name=='nt' else ''))
                shutil.copy2(self.executable,executable)
            command=['pwsh','-NoProfile','-Command',f". '{helper}'; try {{ Resolve-DorotiPython }} catch {{ exit 7 }}"]
            command[0]=pwsh
            result=subprocess.run(command,cwd=ROOT,env={**os.environ,'PATH':str(directory)},capture_output=True,timeout=20)
            self.assertEqual(result.returncode,0 if name else 7,result.stderr)
            if name: self.assertEqual(Path(result.stdout.decode().strip()),executable)

if __name__=='__main__': unittest.main()
