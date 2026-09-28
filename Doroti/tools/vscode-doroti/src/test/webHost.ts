import * as vscode from 'vscode';
import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';

// The external browser is operated through the browser test surface. File gates
// coordinate inspection without injecting callbacks into the running widget tree.
export async function run(): Promise<void> {
    const evidence = process.env.DOROTI_TEST_EVIDENCE!;
    async function until(predicate: () => unknown | Promise<unknown>, label: string, ms = 180000) {
        const deadline = Date.now() + ms;
        while (Date.now() < deadline) { if (await predicate()) return; await new Promise(resolve => setTimeout(resolve, 200)); }
        throw new Error(`Timed out: ${label}`);
    }
    const exists = async (name: string) => { try { await fs.access(path.join(evidence, name)); return true; } catch { return false; } };
    const extension = vscode.extensions.getExtension('doroti-local.doroti'); assert.ok(extension);
    assert.ok(extension.extensionPath.includes('extensions'));
    const api = await extension.activate();
    await vscode.workspace.getConfiguration('doroti').update('cliPath', process.env.DOROTI_TEST_CLI!, vscode.ConfigurationTarget.Global);
    await vscode.commands.executeCommand('doroti.selectProject');
    await vscode.commands.executeCommand('doroti.selectTarget', 'web');
    const source = vscode.Uri.file(path.join(api.getState().project.root, 'src/HotReloadSample.cs'));
    const doc = await vscode.workspace.openTextDocument(source);
    const editor = await vscode.window.showTextDocument(doc);
    const original = doc.getText();
    try {
        await vscode.commands.executeCommand('doroti.run');
        await until(() => api.getState().runtime?.supported, 'Browser metadata capability', 300000);
        const before = api.getState().runtime;
        await fs.writeFile(path.join(evidence, 'ready.json'), JSON.stringify(before));
        await until(() => exists('continue'), 'Browser counter/input/scroll setup');
        const replace = (value: string) => editor.edit(edit => edit.replace(new vscode.Range(doc.positionAt(0), doc.positionAt(doc.getText().length)), value));
        await replace(original.replace('Before reload', 'Web VSIX Hot Reload passed'));
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => !api.getState().pending && api.getState().runtime?.revision > before.revision, 'Browser frame acknowledgment');
        assert.equal(api.getState().runtime.runtimeId, before.runtimeId);
        assert.equal(api.getState().runtime.status, 'applied');
        await fs.writeFile(path.join(evidence, 'applied.json'), JSON.stringify(api.getState().runtime));
        await until(() => exists('inspected'), 'Browser pixels/state inspection');
        const revision = api.getState().runtime.revision;
        await replace(original.replace('"Before reload"', 'MissingWebReloadValue'));
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => api.getState().problem === 'compile-error', 'Web compile error');
        assert.equal(api.getState().runtime.revision, revision);
        await replace(original.replace('Before reload', 'Web reload recovered'));
        await Promise.all([vscode.commands.executeCommand('doroti.hotReload', true), vscode.commands.executeCommand('doroti.hotReload', true)]);
        await until(() => !api.getState().pending && api.getState().runtime?.revision > revision, 'Web error recovery');
        assert.equal(api.getState().runtime.runtimeId, before.runtimeId);
        assert.equal(api.getState().runtime.status, 'applied');
        await fs.writeFile(path.join(evidence, 'recovered.json'), JSON.stringify(api.getState().runtime));
        await until(() => exists('finish'), 'Final browser inspection');
        await fs.writeFile(process.env.DOROTI_TEST_RESULT!, JSON.stringify({ installedVsix: extension.extensionPath, webMetadataReload: 'passed', correlatedFrameAck: 'passed', compileErrorRetry: 'passed', duplicateRequest: 'passed', before, after: api.getState().runtime }, null, 2));
    } finally {
        await vscode.commands.executeCommand('doroti.stop');
        assert.equal(api.getState().running, false);
        await editor.edit(edit => edit.replace(new vscode.Range(doc.positionAt(0), doc.positionAt(doc.getText().length)), original));
        await doc.save();
    }
}
