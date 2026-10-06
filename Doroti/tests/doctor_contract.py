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
        result=subprocess.run(['dotnet','build',str(ROOT/'Doroti/src/Doroti.Tooling/Doroti.Tooling.csproj'),'-c','Debug','-v','quiet'],cwd=ROOT,capture_output=True,timeout=1200)
        if result.returncode:raise RuntimeError(result.stdout.decode(errors='replace'))
        cls.compiler = cls.tool / 'compiler'; cls.compiler.write_text('local compiler fixture')

    def workspace(self):
        directory = RUN / ('workspace 한글 ' + uuid.uuid4().hex[:8]); directory.mkdir()
        (directory / 'App.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"/>')
        runner = directory / 'web/Web.csproj'; runner.parent.mkdir()
        runner.write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
        platform=dict(provider='web',providerManifest=str(ROOT/'packages/platforms/web/doroti-provider.json'),
            target='Web',targetPackage='Doroti.Target.Web.browser-wasm',runner='web/Web.csproj',backend='BlazorWebAssembly',
            profile='default:browser-wasm',targetFramework='net10.0',runtimeIdentifier='browser-wasm')
        (directory / 'doroti-workspace.json').write_text(json.dumps(dict(schemaVersion='doroti.workspace/v2',applicationProject='App.csproj',platforms={'laboratory-web':platform})))
        (directory / 'global.json').write_text(json.dumps(dict(sdk=dict(version='10.0.400',rollForward='latestPatch',allowPrerelease=False))))
        return directory

    def run_doctor(self, workspace=None, spec=None, scope='target', expected=None, extra=()):
        workspace=workspace or self.workspace()
        directory=RUN / ('report-'+uuid.uuid4().hex[:8]); directory.mkdir()
        specification=directory/'fake.json'; specification.write_text(json.dumps({'compiler':str(self.compiler),**(spec or {})}))
        log=directory/'calls.jsonl'
        environment=os.environ|{'DOROTI_FAKE_SPEC':str(specification),'DOROTI_FAKE_LOG':str(log)}
        command=['dotnet',str(ROOT/'Doroti/src/Doroti.Tooling/bin/Debug/net10.0/Doroti.Tooling.dll'),'doctor',
            '-App',str(workspace),'-Platform','laboratory-web','-DotnetPath',str(self.executable),'-TimeoutSeconds','1','-Scope',scope,*extra]
        result=subprocess.run(command,cwd=ROOT,env=environment,capture_output=True,text=True,timeout=45)
        (directory/'console.log').write_text(result.stdout+result.stderr,encoding='utf-8')
        if extra:
            self.assertNotEqual(result.returncode,0)
            return result.stderr,[]
        report=json.loads(result.stdout)
        checks=report['diagnostics']
        status='Fail' if any(x['status']=='Fail' for x in checks) else 'Partial' if any(x['status']=='Partial' for x in checks) else 'Skipped' if all(x['status']=='Skipped' for x in checks) else 'Pass'
        self.assertEqual(result.returncode,1 if status=='Fail' else 2 if status=='Partial' else 0,checks)
        if expected:self.assertEqual(status,expected,checks)
        calls=[json.loads(line) for line in log.read_text().splitlines()] if log.exists() else []
        self.assertFalse(any('restore' in item['args'] or 'build' in item['args'] for item in calls),calls)
        return checks,calls

    def test_custom_executable_paths_and_scope(self):
        checks,calls=self.run_doctor(scope='managed',expected='Pass')
        self.assertEqual([x['id'] for x in checks],['dotnet-sdk'])
        self.assertEqual(len(calls),1)
        self.assertTrue(calls[0]['cwd'].endswith('web'))

    def test_sdk_feature_band_prerelease_and_rollforward(self):
        for selected,policy,expected in [('10.0.399','latestPatch','Fail'),('10.0.500','latestPatch','Fail'),
                ('10.0.401-preview.1','latestPatch','Fail'),('10.0.401','latestPatch','Pass'),('10.0.500','latestFeature','Pass'),('10.1.100','latestMinor','Pass')]:
            workspace=self.workspace()
            (workspace/'global.json').write_text(json.dumps(dict(sdk=dict(version='10.0.400',rollForward=policy,allowPrerelease=False))))
            self.run_doctor(workspace,dict(version=selected),scope='managed',expected=expected)

    def test_runner_global_json_and_cwd(self):
        workspace=self.workspace()
        (workspace/'web/global.json').write_text(json.dumps(dict(sdk=dict(version='11.0.100-preview.1',rollForward='disable',allowPrerelease=True))))
        checks,calls=self.run_doctor(workspace,dict(selectGlobal='true'),scope='managed',expected='Pass')
        self.assertIn('11.0.100-preview.1',checks[0]['message'])
        self.assertTrue(all(item['cwd']==str(workspace/'web') for item in calls))

    def test_metadata_failure_and_missing_workload_are_distinct(self):
        checks,_=self.run_doctor(spec=dict(evaluation='fail'),expected='Partial')
        self.assertEqual(next(x for x in checks if x['id']=='build-metadata')['status'],'Partial')
        checks,_=self.run_doctor(spec=dict(workloads=''),scope='native',expected='Fail')
        self.assertEqual(next(x for x in checks if x['id']=='workload:wasm-tools')['status'],'Fail')

    def test_timeout_is_partial_and_other_probes_continue(self):
        checks,calls=self.run_doctor(spec=dict(hang='true'),expected='Partial')
        self.assertEqual(next(x for x in checks if x['id']=='dotnet-sdk')['status'],'Partial')
        self.assertTrue(any('msbuild' in item['args'] for item in calls))

    def test_profile_restriction_skips_without_probing(self):
        if sys.platform=='darwin':self.skipTest('This host supports Apple profiles')
        workspace=self.workspace(); path=workspace/'doroti-workspace.json'; model=json.loads(path.read_text())
        model['platforms']['laboratory-web'].update(provider='maui',providerManifest=str(ROOT/'packages/platforms/maui/doroti-provider.json'),
            target='iOS',targetPackage='Doroti.Target.iOS.Maui.iossimulator-arm64',backend='Maui',profile='default:iossimulator-arm64',targetFramework='net10.0-ios',runtimeIdentifier='iossimulator-arm64')
        path.write_text(json.dumps(model))
        _,calls=self.run_doctor(workspace,expected='Skipped')
        self.assertEqual(calls,[])

    def test_invalid_option_is_rejected_before_probe(self):
        error,_=self.run_doctor(extra=('-UnknownProbeOption','true'))
        self.assertIn('unknown-option',error)

if __name__=='__main__': unittest.main()
