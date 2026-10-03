import { runTests } from '@vscode/test-electron';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import { spawnSync } from 'node:child_process';
import { randomUUID } from 'node:crypto';

async function main() {
    const extension = path.resolve(__dirname, '../..');
    const repo = path.resolve(extension, '../../..');
    const app = path.resolve(process.argv[2]);
    const evidence = path.resolve(process.argv[3]);
    const profile = path.join(evidence, 'profile'); const extensions = path.join(evidence, 'extensions');
    const harness = path.join(evidence, 'harness');
    await fs.mkdir(harness, { recursive: true });
    await fs.writeFile(path.join(harness, 'package.json'), JSON.stringify({ name: 'doroti-test-harness', publisher: 'local', version: '0.0.0', engines: { vscode: '^1.100.0' } }));
    const code = process.env.DOROTI_TEST_CODE ?? (process.platform === 'darwin'
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
    const install = spawnSync(code, [cli, '--user-data-dir', profile, '--extensions-dir', extensions, '--install-extension', path.join(extension, 'doroti-0.1.0.vsix'), '--force'], { env: environment, encoding: 'utf8', windowsHide: true });
    console.log(install.stdout, install.stderr); if (install.status !== 0) throw new Error('VSIX install failed');
    delete process.env.ELECTRON_RUN_AS_NODE;
    delete process.env.VSCODE_IPC_HOOK_CLI;
    const web = process.argv.includes('--web');
    const ios = process.argv.includes('--ios');
    const android = process.argv.includes('--android');
    const mac = process.argv.includes('--mac');
    const linux = process.argv.includes('--linux');
    await runTests({ vscodeExecutablePath: code, extensionDevelopmentPath: harness, extensionTestsPath: path.join(__dirname, android ? 'androidHost.js' : linux ? 'linuxHost.js' : mac ? 'macHost.js' : ios ? 'iosHost.js' : web ? 'webHost.js' : 'host.js'),
        launchArgs: [app, '--user-data-dir', profile, '--extensions-dir', extensions, '--disable-workspace-trust', '--skip-welcome', '--skip-release-notes', '--disable-updates'],
        extensionTestsEnv: { DOROTI_TEST_CLI: path.join(repo, 'Doroti/eng/doroti.ps1'), DOROTI_RELOAD_PROBE: android || ios && process.env.DOROTI_TEST_IOS_RID === 'ios-arm64' ? `reload-vsix-${randomUUID()}.json` : path.join(evidence, 'state.json'), DOROTI_TEST_RESULT: path.join(evidence, 'result.json'), DOROTI_TEST_EVIDENCE: evidence, ...(android || ios || mac ? { DOROTI_SAMPLE: 'reload' } : {}) } });
}
main().catch(error => { console.error(error); process.exitCode = 1; });
