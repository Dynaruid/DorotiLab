import * as vscode from 'vscode';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import * as assert from 'node:assert/strict';

export async function run() {
    await vscode.workspace.getConfiguration('doroti').update('cliPath', process.env.DOROTI_TEST_CLI, vscode.ConfigurationTarget.Global);
    const extension = vscode.extensions.getExtension('doroti-local.doroti');
    assert.ok(extension);
    assert.ok(extension.extensionPath.includes('extensions'), 'Use an installed VSIX in the isolated profile');
    const api = await extension.activate();
    const read = async () => JSON.parse(await fs.readFile(process.env.DOROTI_RELOAD_PROBE!, 'utf8'));
    async function until(predicate: () => boolean | Promise<boolean>, label: string, timeout = 120000) {
        const deadline = Date.now() + timeout;
        while (Date.now() < deadline) {
            if (await predicate()) return;
            await new Promise(resolve => setTimeout(resolve, 150));
        }
        throw new Error(`Timed out: ${label}; ${JSON.stringify(api.getState())}`);
    }
    await vscode.commands.executeCommand('doroti.selectProject');
    assert.ok(api.getState().project.developmentTargets.includes('linux'));
    await vscode.commands.executeCommand('doroti.selectTarget', 'linux');
    const source = vscode.Uri.file(path.join(api.getState().project.root, 'src/HotReloadSample.cs'));
    const original = await fs.readFile(source.fsPath, 'utf8');
    async function edit(text: string) {
        const doc = await vscode.workspace.openTextDocument(source);
        const edit = new vscode.WorkspaceEdit();
        edit.replace(source, new vscode.Range(doc.positionAt(0), doc.positionAt(doc.getText().length)), text);
        assert.ok(await vscode.workspace.applyEdit(edit));
        return doc;
    }
    let applicationPid: number | undefined;
    try {
        await vscode.commands.executeCommand('doroti.run');
        await until(() => !!api.getState().runtime?.supported, 'Linux Qt runtime', 600000);
        await until(async () => { try { return (await read()).scroll === 160; } catch { return false; } }, 'seeded state');
        const runtime = api.getState().runtime;
        applicationPid = runtime.processId;
        const before = await read();
        await edit(original.replace('"Before reload"', '"Linux VSIX Hot Reload passed"'));
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => api.getState().runtime?.revision > runtime.revision && !api.getState().pending, 'completed frame acknowledgment');
        const after = await read();
        assert.equal(after.message, 'Linux VSIX Hot Reload passed');
        for (const key of ['stateId', 'processId', 'count', 'text', 'scroll']) assert.equal(after[key], before[key], key);
        assert.equal(api.getState().runtime.runtimeId, runtime.runtimeId);
        const revision = api.getState().runtime.revision;
        await edit(original.replace('"Before reload"', 'MISSING_LINUX_RELOAD_SYMBOL'));
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => api.getState().problem === 'compile-error', 'compiler diagnostics');
        assert.equal(api.getState().runtime.revision, revision);
        await edit(original.replace('"Before reload"', '"Linux VSIX recovery passed"'));
        await Promise.all([vscode.commands.executeCommand('doroti.hotReload', true), vscode.commands.executeCommand('doroti.hotReload', true)]);
        await until(() => api.getState().runtime?.revision > revision && !api.getState().pending, 'compile recovery / concurrent clicks');
        assert.equal((await read()).stateId, before.stateId);
        await edit(original.replace('private int _count;', 'private long _count;'));
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => api.getState().problem === 'restart-required', 'rude edit');
        assert.equal(api.getState().runtime.runtimeId, runtime.runtimeId);
        await vscode.commands.executeCommand('doroti.restart', true);
        await until(() => !!api.getState().runtime?.supported && api.getState().runtime.runtimeId !== runtime.runtimeId, 'explicit Restart', 600000);
        applicationPid = api.getState().runtime.processId;
        await until(async () => { try { return (await read()).stateId !== before.stateId; } catch { return false; } }, 'new widget state');
        await fs.writeFile(process.env.DOROTI_TEST_RESULT!, JSON.stringify({ installedVsix: extension.extensionPath,
            before, after, reload: 'PASS', compileRecovery: 'PASS', concurrentClicks: 'PASS', rudeEdit: 'PASS',
            explicitRestart: 'PASS / new runtime and State', runtime: api.getState().runtime }, null, 2));
        console.log('PASS installed Linux VSIX Run/Hot Reload, state preservation, compile recovery and Restart');
    } finally {
        await vscode.commands.executeCommand('doroti.stop');
        await (await edit(original)).save();
    }
    assert.equal(api.getState().running, false);
    if (applicationPid) await until(() => {
        try { process.kill(applicationPid!, 0); return false; }
        catch (error) { assert.equal((error as NodeJS.ErrnoException).code, 'ESRCH'); return true; }
    }, 'Qt process exit', 30000);
    console.log('PASS installed Linux VSIX Stop; source restored');
}
