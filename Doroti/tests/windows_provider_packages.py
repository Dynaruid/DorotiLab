"""Real Windows provider from isolated prebuilt nupkgs; no repository app/test source."""
from pathlib import Path
import subprocess,sys,json,os,uuid,re,zipfile
ROOT=Path(__file__).resolve().parents[2]
run=ROOT/'temp/testing/platform-decoupling/windows-packages'/uuid.uuid4().hex
run.mkdir(parents=True);feed=run/'feed';feed.mkdir();results=[]
def call(name,args,env=None):
    print('Running '+name,flush=True)
    result=subprocess.run([sys.executable,str(ROOT/'Doroti/eng/run-with-timeout.py'),'--timeout','90' if name=='native-consumer' else '1200',*map(str,args)],cwd=ROOT,env=env,text=True,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
    (run/(name+'.log')).write_text(result.stdout,encoding='utf-8')
    if result.returncode:raise RuntimeError(name+': '+result.stdout[-10000:])
    results.append(name);return result.stdout
seen=set()
def visit(project):
    project=project.resolve()
    if project in seen:return
    seen.add(project)
    model=json.loads(call('evaluate-'+project.stem,['dotnet','msbuild',project,'-nologo','-getItem:ProjectReference']))
    for reference in model['Items']['ProjectReference']:visit(Path(reference['FullPath']))
    call('pack-'+project.stem,['dotnet','pack',project,'-c','Release','--no-build','-o',feed])
target=ROOT/'packages/platforms/windowsappsdk/Doroti.Target.Windows.WindowsAppSdk.win-x64/Doroti.Target.Windows.WindowsAppSdk.win-x64.csproj'
call('build-target',['dotnet','build',target,'-c','Release','-p:DorotiBuildNativeFromSource=false'])
visit(target)
visit(ROOT/'Doroti/src/Doroti.Framework.Widgets/Doroti.Framework.Widgets.csproj')
app=run/'consumer';app.mkdir()
for name in ('Directory.Build.props','Directory.Build.targets','Directory.Packages.props'):(app/name).write_text('<Project/>',encoding='utf-8')
(app/'NuGet.Config').write_text(f'<configuration><packageSources><clear/><add key="local" value="{feed}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources><config><add key="globalPackagesFolder" value="{run / "nuget"}"/></config><packageSourceMapping><packageSource key="local"><package pattern="Doroti.*"/></packageSource><packageSource key="nuget"><package pattern="*"/></packageSource></packageSourceMapping></configuration>',encoding='utf-8')
(app/'Consumer.csproj').write_text('''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0-windows10.0.19041.0</TargetFramework><RuntimeIdentifier>win-x64</RuntimeIdentifier><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained><WindowsPackageType>None</WindowsPackageType><SelfContained>true</SelfContained><DorotiTarget>Windows</DorotiTarget></PropertyGroup><ItemGroup><PackageReference Include="Doroti.Target.Windows.WindowsAppSdk.win-x64" Version="0.4.0-beta"/><PackageReference Include="Doroti.Framework.Widgets" Version="0.4.0-beta"/></ItemGroup></Project>''',encoding='utf-8')
(app/'Program.cs').write_text('''using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Framework;
using Doroti.Framework.Widgets;
using Doroti.Desktop;
using Doroti.Host.WindowsAppSdk;
using System.Reflection;
internal static class Program {
  [STAThread] static int Main() {
    Console.WriteLine("consumer.start");
    Doroti.Framework.Foundation.FlutterError.onError = details => Console.Error.WriteLine(details.exceptionThrown);
    return DorotiWindowsAppSdkRunner.Run(() => {
    var descriptor=new DorotiApplicationDescriptor(()=>new DorotiWidgetEntrypoint(views=>new Root(views)),Assembly.GetExecutingAssembly(),Assembly.GetExecutingAssembly(),new DorotiViewConfiguration("Independent provider",new Size(500,400)),new DorotiLaunchContext("Windows","win-x64",[]),[],[]);
    return DesktopApplication.Configure<Startup>(descriptor); });
  }
}
public sealed class Startup:IDorotiDesktopApplicationStartup {
  public void Configure(DesktopApplicationBuilder desktop) {
    var window=desktop.DefaultMainWindow with {Options=new WindowOptions {Title="NuGet primary",Size=new Size(500,400)}};
    desktop.UseMainWindow(window with {OnCreated=async (context,ct)=>{
      await context.Window.WaitUntilReadyToShowAsync(ct);
      var second=await context.Windows.CreateWindowAsync(window with {Options=window.Options with {Title="NuGet survivor"},OnCreated=null},ct);
      await second.WaitUntilReadyToShowAsync(ct);await second.ShowAsync(ct);
      await Task.Delay(1200,ct);await context.Window.CloseAsync(CancellationToken.None);
      await second.SetSizeAsync(new Size(550,450),CancellationToken.None);await Task.Delay(800);
      await second.CloseAsync(CancellationToken.None);Console.WriteLine("consumer.complete");
    }});
  }
}
internal sealed class Root(DorotiApplicationViews views):StatefulWidget {
 public DorotiApplicationViews Views {get;}=views;
 public override IState createState()=>new RootState();
 private sealed class RootState:State<Root> {
  private System.Threading.Timer? timer;private int revision;
  public override void initState(){base.initState();timer=new(_=>{_ = Tick();},null,100,100);}
  private async Task Tick(){var view=widget.Views.Views.FirstOrDefault();if(view is null)return;try {await view.DispatchPlatformEventAsync(()=>{if(mounted)setState(()=>revision++);});}catch(OperationCanceledException){}catch(ObjectDisposedException){}}
  public override Widget build(BuildContext context)=>new DorotiApplicationViewCollection(widget.Views,(_,view)=>{
   Console.WriteLine($"consumer.build app={view.SceneOwner.ApplicationId} view={view.viewId} revision={revision}");
   return new Directionality(textDirection:TextDirection.ltr,child:new ColoredBox(color:new Color(revision%2==0?0xff55aa00u:0xff0055aau),child:new Text($"Revision {revision}",style:new Doroti.Framework.Painting.TextStyle(color:new Color(0xffffffff),fontSize:24))));
  });
  public override void dispose(){timer?.Dispose();timer=null;base.dispose();}
 }
}''',encoding='utf-8')
(app/'manifest.json').write_text(json.dumps(dict(schemaVersion='doroti.application-capabilities/v1',applicationId='Independent.Consumer',targetRid='win-x64',resources=[],plugins=[],platformViews=[])),encoding='utf-8')
p=app/'Consumer.csproj';p.write_text(p.read_text().replace('</Project>', '<ItemGroup><EmbeddedResource Include="manifest.json" LogicalName="Doroti.Application.Manifest"/></ItemGroup></Project>'),encoding='utf-8')
call('publish-consumer',['dotnet','publish',app/'Consumer.csproj','-c','Release','-o',run/'publish'])
assets=json.loads((app/'obj/project.assets.json').read_text());assert all(x['type']=='package' for x in assets['libraries'].values())
assert not any(x.startswith(('Doroti.Material/','Doroti.Cupertino/','MaterialColorUtilities/')) for x in assets['libraries'])
with zipfile.ZipFile(next(feed.glob('Doroti.Target.Windows.WindowsAppSdk.win-x64.*.nupkg'))) as z:
    assert any(x.endswith('doroti_windows_appsdk_host_v1.dll') for x in z.namelist())
    targets=z.read('buildTransitive/Doroti.Target.Windows.WindowsAppSdk.win-x64.targets').decode();assert '<Exec' not in targets and 'BuildDorotiWindowsAppSdkNative' not in targets
output=call('native-consumer',['dotnet',run/'publish/Consumer.dll'],os.environ|{'DOROTI_WINDOWS_APPSDK_DIAGNOSTICS':'1'})
assert 'consumer.complete' in output and not any(x in output for x in ('Unhandled exception','System.InvalidOperationException','FlutterError','DorotiError')),output[-10000:]
builds=[(a,int(v),int(n)) for a,v,n in re.findall(r'consumer.build app=([0-9a-f-]+) view=(\d+) revision=(\d+)',output)]
assert len({a for a,_,_ in builds})==1 and {v for _,v,_ in builds}=={1,2},builds
assert max(n for _,v,n in builds if v==2)>max(n for _,v,n in builds if v==1),builds
assert output.count('complete=true pending=0')==2,output[-5000:]
report=dict(schema='doroti.windows-provider-packages/v1',result='PASS',checks=results,scope='isolated NuGet-only Widgets application, prebuilt native assets, two real Regular windows/one shared root/primary close/newer survivor frame and native/GPU cleanup completion',artifacts=str(run.relative_to(ROOT)),notVerified=['physical IME','slow consumer isolation','modal/native menu','clean machine installation'])
(ROOT/'Doroti/docs/migrations/design-platform/windows-provider-package-verification.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
print('PASS: Windows provider isolated prebuilt packages; '+str(run),flush=True)
