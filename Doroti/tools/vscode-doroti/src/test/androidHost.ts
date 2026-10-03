import * as vscode from 'vscode';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import * as assert from 'node:assert/strict';
import { execFile } from 'node:child_process';
import { promisify } from 'node:util';

export async function run() {
    const config = vscode.workspace.getConfiguration('doroti');
    await config.update('cliPath', process.env.DOROTI_TEST_CLI, vscode.ConfigurationTarget.Global);
    await config.update('androidDevice', process.env.DOROTI_TEST_ANDROID_DEVICE, vscode.ConfigurationTarget.Global);
    const extension = vscode.extensions.getExtension('doroti-local.doroti');
    assert.ok(extension);
    const api = await extension.activate();
    const evidence = process.env.DOROTI_TEST_EVIDENCE!;
    async function until(predicate: () => boolean | Promise<boolean>, label: string, timeout = 120000) {
        const deadline = Date.now() + timeout;
        while (Date.now() < deadline) {
            if (await predicate()) return;
            await new Promise(resolve => setTimeout(resolve, 150));
        }
        throw new Error(`Timed out: ${label}; ${JSON.stringify(api.getState())}`);
    }
    const read = async () => {
        const { stdout } = await promisify(execFile)('adb', ['-s', process.env.DOROTI_TEST_ANDROID_DEVICE!,
            'exec-out', 'run-as', 'dev.doroti.testbed', 'cat', 'files/' + process.env.DOROTI_RELOAD_PROBE], { timeout: 10000 });
        return JSON.parse(stdout);
    };
    await vscode.commands.executeCommand('doroti.selectProject');
    await vscode.commands.executeCommand('doroti.selectTarget', 'android');
    const source = vscode.Uri.file(path.join(api.getState().project.root, 'src/MaterialSample/HotReloadSample.cs'));
    const original = await fs.readFile(source.fsPath, 'utf8');
    try {
        await vscode.commands.executeCommand('doroti.run');
        await until(() => !!api.getState().runtime?.supported, 'Android runtime', 600000);
        await until(async () => { try { return (await read()).scroll === 160; } catch { return false; } }, 'seeded state');
        const before = await read(); const runtime = api.getState().runtime;
        const doc = await vscode.workspace.openTextDocument(source);
        const edit = new vscode.WorkspaceEdit();
        edit.replace(source, new vscode.Range(doc.positionAt(0), doc.positionAt(doc.getText().length)),
            original.replace('"Before reload"', '"Android VSIX Hot Reload passed"'));
        assert.ok(await vscode.workspace.applyEdit(edit));
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => api.getState().runtime?.revision > runtime.revision && !api.getState().pending, 'frame acknowledgment');
        const after = await read();
        assert.equal(after.message, 'Android VSIX Hot Reload passed');
        for (const key of ['stateId', 'processId', 'count', 'text', 'scroll']) assert.equal(after[key], before[key], key);
        assert.equal(api.getState().runtime.runtimeId, runtime.runtimeId);
        await fs.writeFile(path.join(evidence, 'result.json'), JSON.stringify({ before, after, runtime: api.getState().runtime }));
        console.log('PASS installed VSIX Android Run, Hot Reload command, state preservation');
        await vscode.commands.executeCommand('doroti.restart', true);
        await until(() => !!api.getState().runtime?.supported && api.getState().runtime.runtimeId !== runtime.runtimeId, 'explicit Restart', 600000);
        await until(async () => { try { return (await read()).scroll === 160; } catch { return false; } }, 'restarted state');
        assert.notEqual((await read()).processId, before.processId);
        console.log('PASS installed VSIX Android Restart creates a new runtime and PID');
    } finally {
        await vscode.commands.executeCommand('doroti.stop');
        const doc = await vscode.workspace.openTextDocument(source);
        const edit = new vscode.WorkspaceEdit();
        edit.replace(source, new vscode.Range(doc.positionAt(0), doc.positionAt(doc.getText().length)), original);
        await vscode.workspace.applyEdit(edit); await doc.save();
    }
    assert.equal(api.getState().running, false);
    const stopped = await promisify(execFile)('adb', ['-s', process.env.DOROTI_TEST_ANDROID_DEVICE!,
        'shell', 'ps', '-A'], { timeout: 10000 });
    assert.ok(!stopped.stdout.includes('dev.doroti.testbed'));
    console.log('PASS installed VSIX Android Stop closes the device app');
}
