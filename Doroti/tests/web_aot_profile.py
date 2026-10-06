"""Evaluate Web AOT defaults and explicit opt-outs through real MSBuild imports.

Run through eng/run-with-timeout.py --timeout 1200; no publish or browser is launched.
"""
import argparse
import json
from pathlib import Path
import subprocess
from xml.sax.saxutils import escape

ROOT = Path(__file__).resolve().parents[2]
PROPERTIES = ['Configuration', 'RunAOTCompilation', 'RunAOTCompilationAfterBuild',
              'PublishTrimmed', 'WasmBuildNative', 'WasmEnableThreads', 'NETCoreSdkVersion',
              'DorotiWebInterpretWidgetTree']


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('evidence', type=Path)
    args = parser.parse_args()
    out = args.evidence.resolve()
    assert out.is_relative_to(ROOT / 'temp/testing') and not out.exists()
    out.mkdir(parents=True)

    def evaluate(project, configuration, *extra):
        raw = subprocess.check_output(['dotnet', 'msbuild', str(project), '-nologo',
            '-p:Configuration=' + configuration, '-getProperty:' + ','.join(PROPERTIES), *extra],
            cwd=ROOT, text=True)
        return json.loads(raw)['Properties']

    records = {}
    for app in ('DorotiSampleApp2', 'DorotiTestbedApp'):
        project = ROOT / f'samples/{app}/web/{app}.Web.csproj'
        profiles = {
            'release': evaluate(project, 'Release'),
            'debug': evaluate(project, 'Debug'),
            'releaseOptOut': evaluate(project, 'Release', '-p:RunAOTCompilation=false'),
        }
        assert profiles['release']['RunAOTCompilation'] == 'true', profiles
        assert profiles['debug']['RunAOTCompilation'] == 'false', profiles
        assert profiles['releaseOptOut']['RunAOTCompilation'] == 'false', profiles
        # Ordinary Build/Run remains separate from the SDK publish AOT pipeline.
        assert all(p['RunAOTCompilationAfterBuild'] != 'true' for p in profiles.values()), profiles
        assert all(p['WasmEnableThreads'] == 'true' for p in profiles.values()), profiles
        if app == 'DorotiSampleApp2':
            assert profiles['release']['DorotiWebInterpretWidgetTree'] == 'true', profiles
            opt_out = evaluate(project, 'Release', '-p:DorotiWebInterpretWidgetTree=false')
            assert opt_out['DorotiWebInterpretWidgetTree'] == 'false', opt_out
            profiles['widgetTreeOptOut'] = opt_out
        records[app] = profiles

    # Exercise the provider's packaged build props without the source runner imports.
    target_props = ROOT / 'packages/platforms/web/Doroti.Target.Web.browser-wasm/build/Doroti.Target.Web.browser-wasm.props'
    consumer = out / 'PackageStyle.Web.csproj'
    consumer.write_text('<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">'
        '<PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>'
        f'<Import Project="{escape(str(target_props))}" /></Project>', encoding='utf-8')
    consumer_profiles = {
        'release': evaluate(consumer, 'Release'),
        'debug': evaluate(consumer, 'Debug'),
        'releaseOptOut': evaluate(consumer, 'Release', '-p:RunAOTCompilation=false'),
    }
    assert consumer_profiles['release']['RunAOTCompilation'] == 'true', consumer_profiles
    assert consumer_profiles['debug']['RunAOTCompilation'] == 'false', consumer_profiles
    assert consumer_profiles['releaseOptOut']['RunAOTCompilation'] == 'false', consumer_profiles
    # A local project setting also overrides the imported default.
    consumer.write_text(consumer.read_text().replace('</Project>',
        '<PropertyGroup><RunAOTCompilation>false</RunAOTCompilation></PropertyGroup></Project>'), encoding='utf-8')
    consumer_profiles['projectOptOut'] = evaluate(consumer, 'Release')
    assert consumer_profiles['projectOptOut']['RunAOTCompilation'] == 'false', consumer_profiles
    records['providerBuildProps'] = consumer_profiles

    non_web = out / 'NonWeb.csproj'
    shared_props = ROOT / 'packages/platforms/build/Sdk.props'
    non_web.write_text('<Project Sdk="Microsoft.NET.Sdk">'
        '<PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>'
        f'<Import Project="{escape(str(shared_props))}" /></Project>', encoding='utf-8')
    records['nonWebRelease'] = evaluate(non_web, 'Release')
    assert records['nonWebRelease']['RunAOTCompilation'] != 'true', records
    (out / 'result.json').write_text(json.dumps({'status':'PASS', 'profiles':records,
        'scope':'MSBuild profile evaluation; actual publish/browser validation is separate'}, indent=2), encoding='utf-8')
    print('PASS Web Release AOT, Debug/non-Web separation, command/project opt-outs and provider build props', flush=True)


if __name__ == '__main__':
    main()
