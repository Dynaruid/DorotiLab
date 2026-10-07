import * as vscode from 'vscode';
import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import { execFile } from 'node:child_process';
import { promisify } from 'node:util';
import type { DorotiSidebar } from '../sidebar';

const execute = promisify(execFile);
const pause = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));
async function until(predicate: () => unknown, label: string, timeout = 30000) {
    const deadline = Date.now() + timeout;
    while (Date.now() < deadline) { if (predicate()) return; await pause(100); }
    throw new Error(`Timed out: ${label}`);
}
function alive(pid: number) { try { process.kill(pid, 0); return true; } catch { return false; } }

export async function run() {
    assert.equal(process.platform, 'win32', 'Native window-close regression runs on Windows');
    const extension = vscode.extensions.getExtension('doroti-local.doroti')!;
    assert.ok(extension?.extensionPath.includes('extensions'), 'Exercise installed VSIX');
    const api = await extension.activate();
    const sidebar = api.sidebar as DorotiSidebar;
    const root = vscode.workspace.workspaceFolders![0].uri.fsPath;
    const item = (id: string) => sidebar.getChildren(sidebar.getChildren().find(value => value.id === 'session')!).find(value => value.id === id)!;
    await vscode.workspace.getConfiguration('doroti').update('cliPath', process.env.DOROTI_TEST_CLI, vscode.ConfigurationTarget.Global);
    try {
        await vscode.commands.executeCommand('doroti.selectProject');
        await vscode.commands.executeCommand('doroti.selectTarget', 'windows');
        await vscode.commands.executeCommand('doroti.run');
        const ready = () => { assert.equal(api.getState().running, true, 'Development session is still starting/running'); return api.getState().runtime?.supported; };
        await until(ready, 'Native runtime ready', 300000);
        const initial = { ...api.getState().runtime };
        assert.equal(initial.status, 'ready');
        assert.equal(item('run').command, undefined);
        assert.ok(item('stop').command);
        assert.match(initial.sessionId, /^[a-f0-9-]{36}$/);
        assert.ok(Number.isSafeInteger(initial.processId) && initial.processId > 0);
        // Capture only this test's CLI/watcher/app tree, including the owner PID.
        const tree = await execute('pwsh', ['-NoProfile', '-NonInteractive', '-Command', `
            $exitProcesses = @(Get-CimInstance Win32_Process)
            $exitOwners = @($exitProcesses | Where-Object { $_.Name -eq 'pwsh.exe' -and $_.ProcessId -ne $PID -and $_.CommandLine.Contains('-SessionId ' + '${initial.sessionId}') })
            if ($exitOwners.Count -ne 1) { throw 'Expected one owned session launcher' }
            $exitPids = [System.Collections.Generic.List[int]]::new()
            $exitPids.Add([int]$exitOwners[0].ProcessId)
            for ($exitIndex = 0; $exitIndex -lt $exitPids.Count; $exitIndex++) {
                foreach ($exitProcess in $exitProcesses) {
                    if ($exitProcess.ParentProcessId -eq $exitPids[$exitIndex]) { $exitPids.Add([int]$exitProcess.ProcessId) }
                }
            }
            ConvertTo-Json -InputObject @($exitPids.ToArray()) -Compress
        `], { windowsHide: true });
        const ownedPids = JSON.parse(tree.stdout) as number[];
        assert.ok(ownedPids.includes(initial.processId), 'App belongs to the captured development tree');
        assert.ok(ownedPids.length >= 2, 'Capture watcher and launcher as well as app');
        const startedClosing = Date.now();
        await execute('pwsh', ['-NoProfile', '-NonInteractive', '-Command', `
            $exitApp = Get-Process -Id ${initial.processId} -ErrorAction Stop
            $exitWindowDeadline = [DateTime]::UtcNow.AddSeconds(30)
            do {
                $exitApp.Refresh()
                if ($exitApp.MainWindowHandle -ne [IntPtr]::Zero) { break }
                Start-Sleep -Milliseconds 100
            } while ([DateTime]::UtcNow -lt $exitWindowDeadline -and !$exitApp.HasExited)
            if (!$exitApp.CloseMainWindow()) { throw 'Native window refused the close request' }
        `], { windowsHide: true });
        await until(() => !api.getState().running, 'Window close ends IDE session');
        await until(() => ownedPids.every(pid => !alive(pid)), 'Owned watcher tree terminated');
        const closeMilliseconds = Date.now() - startedClosing;
        const closed = JSON.parse(await fs.readFile(path.join(root, '.doroti/dev', initial.sessionId, 'runtime.json'), 'utf8'));
        assert.equal(closed.status, 'closed', 'Normal view disposal publishes close acknowledgment');
        assert.equal(closed.supported, false);
        assert.equal(item('session-status').label, 'Stopped');
        assert.ok(item('run').command, 'Run is enabled after closing the app');
        for (const id of ['stop', 'reload', 'restart']) assert.equal(item(id).command, undefined, id);

        const run = item('run').command!;
        await vscode.commands.executeCommand(run.command);
        await until(ready, 'Run starts another native app', 300000);
        const relaunched = { ...api.getState().runtime };
        assert.notEqual(relaunched.sessionId, initial.sessionId);
        assert.notEqual(relaunched.runtimeId, initial.runtimeId);
        await vscode.commands.executeCommand('doroti.stop');
        await until(() => !alive(relaunched.processId), 'Explicit Stop still closes the relaunched app');
        assert.equal(api.getState().running, false);
        assert.ok(item('run').command);
        assert.equal(item('stop').command, undefined);
        await fs.writeFile(process.env.DOROTI_TEST_RESULT!, JSON.stringify({ version: extension.packageJSON.version, vscode: vscode.version,
            installedVsix: extension.extensionPath, project: root, target: 'windows', nativeWindowClose: 'PASS',
            watcherTreeCleanup: 'PASS', sidebarButtonsRestored: 'PASS', runAgain: 'PASS', explicitStop: 'PASS',
            closeMilliseconds, ownedPids, initial, closed, relaunched, physicalCloseButton: 'notVerified' }, null, 2));
    } finally { await vscode.commands.executeCommand('doroti.stop'); }
}
