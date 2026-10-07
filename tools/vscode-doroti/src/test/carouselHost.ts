import * as vscode from 'vscode';
import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';

const pause = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));
async function until(predicate: () => unknown, label: string) {
    const deadline = Date.now() + 300000;
    while (Date.now() < deadline) { if (predicate()) return; await pause(100); }
    throw new Error(`Timed out: ${label}`);
}

export async function run() {
    const extension = vscode.extensions.getExtension('doroti-local.doroti')!;
    assert.ok(extension?.extensionPath.includes('extensions'), 'Exercise installed VSIX');
    const api = await extension.activate();
    const root = vscode.workspace.workspaceFolders![0].uri.fsPath;
    assert.equal(path.basename(root), 'DorotiCarouselApp');
    await vscode.workspace.getConfiguration('doroti').update('cliPath', process.env.DOROTI_TEST_CLI, vscode.ConfigurationTarget.Global);
    const source = path.join(root, 'src/App.cs');
    const original = await fs.readFile(source, 'utf8');
    const before = 'title: "Doroti Custom Carousel", debugShowCheckedModeBanner: false';
    const changed = original.replace(before, 'title: "Doroti Carousel Watch Probe", debugShowCheckedModeBanner: false');
    assert.notEqual(changed, original, 'Method-body edit exists');
    const doc = await vscode.workspace.openTextDocument(vscode.Uri.file(source));
    try {
        await vscode.commands.executeCommand('doroti.selectProject');
        await vscode.commands.executeCommand('doroti.selectTarget', 'windows');
        await vscode.commands.executeCommand('doroti.run');
        await until(() => api.getState().runtime?.supported, 'Carousel Windows watch ready');
        const initial = { ...api.getState().runtime };
        assert.equal(initial.status, 'ready');
        assert.ok(initial.processId > 0, 'Native application PID is published');
        const edit = new vscode.WorkspaceEdit();
        edit.replace(doc.uri, new vscode.Range(doc.positionAt(0), doc.positionAt(original.length)), changed);
        assert.equal(await vscode.workspace.applyEdit(edit), true);
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => {
            assert.notEqual(api.getState().runtime?.status, 'failed', api.getState().runtime?.error);
            return api.getState().runtime?.revision > initial.revision && !api.getState().pending;
        }, 'Real metadata reload acknowledged');
        const applied = { ...api.getState().runtime };
        assert.equal(applied.status, 'applied');
        assert.equal(applied.runtimeId, initial.runtimeId, 'Reload keeps runtime');
        assert.equal(applied.processId, initial.processId, 'Reload keeps process');
        assert.equal(await fs.readFile(source, 'utf8'), changed, 'Source only contains probe edit');
        const restore = new vscode.WorkspaceEdit();
        restore.replace(doc.uri, new vscode.Range(doc.positionAt(0), doc.positionAt(changed.length)), original);
        assert.equal(await vscode.workspace.applyEdit(restore), true);
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => {
            assert.notEqual(api.getState().runtime?.status, 'failed', api.getState().runtime?.error);
            return api.getState().runtime?.revision > applied.revision && !api.getState().pending;
        }, 'Restored source reloaded');
        assert.equal(api.getState().runtime.status, 'applied');
        await vscode.commands.executeCommand('doroti.stop');
        assert.equal(api.getState().running, false);
        await fs.writeFile(process.env.DOROTI_TEST_RESULT!, JSON.stringify({ version: extension.packageJSON.version, vscode: vscode.version,
            installedVsix: extension.extensionPath, project: root, target: 'windows', provider: 'windowsappsdk',
            watchStartup: 'PASS', metadataReload: 'PASS', sameRuntimeAndProcess: 'PASS', sourceRestored: 'PASS', stop: 'PASS',
            initial, applied, physicalInput: 'notVerified' }, null, 2));
    } finally {
        await vscode.commands.executeCommand('doroti.stop');
        if (await fs.readFile(source, 'utf8') === changed) await fs.writeFile(source, original);
    }
}
