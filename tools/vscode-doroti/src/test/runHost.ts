import { runTests, downloadAndUnzipVSCode } from '@vscode/test-electron';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import { spawnSync, spawn } from 'node:child_process';
import { randomUUID } from 'node:crypto';

async function main() {
    const extension = path.resolve(__dirname, '../..');
    const repo = path.resolve(extension, '../..');
    const app = path.resolve(process.argv[2]);
    const evidence = path.resolve(process.argv[3]);
    const profile = path.join(evidence, 'profile'); const extensions = path.join(evidence, 'extensions');
    const harness = path.join(evidence, 'harness');
    await fs.mkdir(harness, { recursive: true });
    await fs.writeFile(path.join(harness, 'package.json'), JSON.stringify({ name: 'doroti-test-harness', publisher: 'local', version: '0.0.0', engines: { vscode: '^1.100.0' } }));
    const requestedVersion = process.argv.find(a => a.startsWith('--version='))?.slice('--version='.length);
    const code = requestedVersion ? await downloadAndUnzipVSCode({ version: requestedVersion, cachePath: path.join(repo, 'temp/testing/vscode-editing/vscode-cache') }) : process.env.DOROTI_TEST_CODE ?? (process.platform === 'darwin'
        ? '/Applications/Visual Studio Code.app/Contents/MacOS/Code'
        : process.platform === 'linux' ? '/usr/share/code/code'
        : path.join(process.env.LOCALAPPDATA!, 'Programs/Microsoft VS Code/Code.exe'));
    let cli = process.platform === 'darwin' ? path.resolve(path.dirname(code), '../Resources/app/out/cli.js') : path.join(path.dirname(code), 'resources/app/out/cli.js');
    try { await fs.access(cli); } catch {
        for (const name of await fs.readdir(path.dirname(code))) {
            const candidate = path.join(path.dirname(code), name, 'resources/app/out/cli.js');
            try { await fs.access(candidate); cli = candidate; break; } catch { /* versioned VS Code layout */ }
        }
    }
    const environment = { ...process.env, ELECTRON_RUN_AS_NODE: '1' };
    const manifest = JSON.parse(await fs.readFile(path.join(extension, 'package.json'), 'utf8'));
    const install = spawnSync(code, [cli, '--user-data-dir', profile, '--extensions-dir', extensions, '--install-extension', path.join(extension, `doroti-${manifest.version}.vsix`), '--force'], { env: environment, encoding: 'utf8', windowsHide: true });
    console.log(install.stdout, install.stderr); if (install.status !== 0) throw new Error('VSIX install failed');
    delete process.env.ELECTRON_RUN_AS_NODE;
    delete process.env.VSCODE_IPC_HOOK_CLI;
    const web = process.argv.includes('--web');
    const ios = process.argv.includes('--ios');
    const android = process.argv.includes('--android');
    const mac = process.argv.includes('--mac');
    const linux = process.argv.includes('--linux');
    const editing = process.argv.includes('--editing');
    const restricted = process.argv.includes('--restricted');
    if (restricted) {
        await fs.mkdir(path.join(profile, 'User'), { recursive: true });
        await fs.writeFile(path.join(profile, 'User/settings.json'), JSON.stringify({ 'security.workspace.trust.startupPrompt': 'never', 'security.workspace.trust.banner': 'never' }));
    }
    if (process.argv.includes('--csharp')) {
        const csharp = spawnSync(code, [cli, '--user-data-dir', profile, '--extensions-dir', extensions, '--install-extension', 'ms-dotnettools.csharp', '--force'], { env: environment, encoding: 'utf8', windowsHide: true });
        console.log(csharp.stdout, csharp.stderr); if (csharp.status !== 0) throw new Error('C# extension install failed');
    }
    const options = { vscodeExecutablePath: code, extensionDevelopmentPath: harness, extensionTestsPath: path.join(__dirname, editing ? 'editingHost.js' : android ? 'androidHost.js' : linux ? 'linuxHost.js' : mac ? 'macHost.js' : ios ? 'iosHost.js' : web ? 'webHost.js' : 'host.js'),
        launchArgs: [app, '--user-data-dir', profile, '--extensions-dir', extensions, ...(restricted ? [] : ['--disable-workspace-trust']), '--skip-welcome', '--skip-release-notes', '--disable-updates'],
        extensionTestsEnv: { DOROTI_TEST_RESTRICTED: restricted ? '1' : '', DOROTI_TEST_CSHARP: process.argv.includes('--csharp') ? '1' : '', DOROTI_TEST_CLI: path.join(repo, 'Doroti/eng/doroti.ps1'), DOROTI_RELOAD_PROBE: android || ios && process.env.DOROTI_TEST_IOS_RID === 'ios-arm64' ? `reload-vsix-${randomUUID()}.json` : path.join(evidence, 'state.json'), DOROTI_TEST_RESULT: path.join(evidence, 'result.json'), DOROTI_TEST_EVIDENCE: evidence, ...(android || ios || mac ? { DOROTI_SAMPLE: 'reload' } : {}) } };
    if (restricted) {
        // test-electron unconditionally adds --disable-workspace-trust. Launch its public test entrypoints directly for this case.
        await new Promise<void>((resolve, reject) => {
            const child = spawn(code, [...options.launchArgs, `--extensionDevelopmentPath=${harness}`, `--extensionTestsPath=${options.extensionTestsPath}`], { env: { ...process.env, ...options.extensionTestsEnv }, stdio: 'inherit', windowsHide: true });
            child.once('error', reject); child.once('exit', code => code === 0 ? resolve() : reject(new Error(`Restricted host exited ${code}`)));
        });
    } else await runTests(options);
}
main().catch(error => { console.error(error); process.exitCode = 1; });
