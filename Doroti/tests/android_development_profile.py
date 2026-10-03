"""Evaluate Android development/Release profiles and generate the real Mono startup config.

Run with eng/run-with-timeout.py. No APK deployment or physical device is needed.
"""
import argparse
import json
from pathlib import Path
import subprocess

ROOT = Path(__file__).resolve().parents[2]
RUNNER = ROOT / 'samples/DorotiSampleApp2/android/DorotiSampleApp2.Android.csproj'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('evidence', type=Path)
    args = parser.parse_args()
    evidence = args.evidence.resolve()
    assert evidence.is_relative_to(ROOT / 'Doroti/artifacts')
    evidence.mkdir(parents=True, exist_ok=False)
    properties = ['Configuration', 'UseMonoRuntime', 'RunAOTCompilation', 'PublishTrimmed',
                  'StartupHookSupport', 'Optimize', 'AndroidLinkMode', 'AndroidEnableProfiledAot']

    def evaluate(*extra):
        raw = subprocess.check_output(['dotnet', 'msbuild', str(RUNNER),
            '-getProperty:' + ','.join(properties), *extra], cwd=ROOT, text=True)
        return json.loads(raw)['Properties']

    release = evaluate('-p:Configuration=Release')
    assert all(release[name] == 'true' for name in ('UseMonoRuntime', 'RunAOTCompilation', 'PublishTrimmed', 'AndroidEnableProfiledAot')), release
    assert release['StartupHookSupport'] == 'false', release
    profiles = {}
    for rid in ('android-arm64', 'android-x64'):
        profile = evaluate('-p:Configuration=Debug', '-p:DorotiAndroidDevelopment=true', '-p:RuntimeIdentifier=' + rid)
        assert profile['UseMonoRuntime'] == profile['StartupHookSupport'] == 'true', profile
        assert all(profile[name] == 'false' for name in ('RunAOTCompilation', 'PublishTrimmed', 'Optimize', 'AndroidEnableProfiledAot')), profile
        assert profile['AndroidLinkMode'] == 'None', profile
        profiles[rid] = profile
    build_properties = ['-p:Configuration=Debug', '-p:DorotiAndroidDevelopment=true', '-p:RuntimeIdentifier=android-arm64',
                        '-p:ArtifactsPath=' + str(evidence / 'build')]
    subprocess.run(['dotnet', 'restore', str(RUNNER), *build_properties, '--nologo'], cwd=ROOT, check=True)
    subprocess.run(['dotnet', 'msbuild', str(RUNNER), '-t:PrepareForBuild;GenerateBuildRuntimeConfigurationFiles',
                    *build_properties, '-nologo'], cwd=ROOT, check=True)
    files = list((evidence / 'build').rglob('DorotiSampleApp2.Android.runtimeconfig.json'))
    assert len(files) == 1, files
    config = json.loads(files[0].read_text())['runtimeOptions']['configProperties']
    assert config['STARTUP_HOOKS'] == 'Microsoft.Extensions.DotNetDeltaApplier', config
    assert config['System.StartupHookProvider.IsSupported'] is True, config
    for invalid in ('Configuration=Release', 'Optimize=true', 'PublishTrimmed=true', 'RunAOTCompilation=true', 'UseMonoRuntime=false'):
        # The last global property wins over an earlier property of the same name.
        result = subprocess.run(['dotnet', 'msbuild', str(RUNNER), '-t:ValidateDorotiAndroidDevelopment',
            *build_properties, '-p:' + invalid, '-nologo'], cwd=ROOT, capture_output=True, text=True)
        assert result.returncode != 0 and 'DOROTIANDROIDDEV001' in result.stdout, (invalid, result.stdout, result.stderr)
    (evidence / 'result.json').write_text(json.dumps(dict(release=release, development=profiles,
        startupHook=config['STARTUP_HOOKS'], rejected=['Release', 'optimized', 'trimmed', 'AOT', 'CoreCLR']), indent=2))
    print('PASS arm64/x64 development, unchanged Release, real Mono startup config and invalid-profile rejection', flush=True)


if __name__ == '__main__':
    main()
