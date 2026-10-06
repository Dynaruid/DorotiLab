import * as vscode from 'vscode';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import * as assert from 'node:assert/strict';

export async function run() {
    const config = vscode.workspace.getConfiguration('doroti');
    await config.update('cliPath', process.env.DOROTI_TEST_CLI, vscode.ConfigurationTarget.Global);
    await config.update('dotnetPath', process.env.DOROTI_TEST_DOTNET ?? 'dotnet', vscode.ConfigurationTarget.Global);
    const target = process.env.DOROTI_TEST_MAC_TARGET ?? 'macos';
    assert.ok(['macos', 'maccatalyst'].includes(target));
    await config.update(target === 'macos' ? 'macosTargetFramework' : 'macCatalystTargetFramework',
        process.env.DOROTI_TEST_MAC_TFM ?? (target === 'macos' ? 'net10.0-macos27.0' : 'net10.0-maccatalyst27.0'), vscode.ConfigurationTarget.Global);
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
        return JSON.parse(await fs.readFile(path.join(evidence, 'state.json'), 'utf8'));
    };
    await vscode.commands.executeCommand('doroti.selectProject');
    await vscode.commands.executeCommand('doroti.selectTarget', target);
    const source = vscode.Uri.file(path.join(api.getState().project.root, 'src/MaterialSample/HotReloadSample.cs'));
    const original = await fs.readFile(source.fsPath, 'utf8');
    try {
        await vscode.commands.executeCommand('doroti.run');
        await until(() => !!api.getState().runtime?.supported, 'Mac runtime', 600000);
        await until(async () => { try { return (await read()).scroll === 160; } catch { return false; } }, 'seeded state');
        const before = await read(); const runtime = api.getState().runtime;
        const doc = await vscode.workspace.openTextDocument(source);
        const edit = new vscode.WorkspaceEdit();
        edit.replace(source, new vscode.Range(doc.positionAt(0), doc.positionAt(doc.getText().length)),
            original.replace('"Before reload"', '"Mac VSIX Hot Reload passed"'));
        assert.ok(await vscode.workspace.applyEdit(edit));
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => api.getState().runtime?.revision > runtime.revision && !api.getState().pending, 'frame acknowledgment');
        const after = await read();
        assert.equal(after.message, 'Mac VSIX Hot Reload passed');
        for (const key of ['stateId', 'processId', 'count', 'text', 'scroll']) assert.equal(after[key], before[key], key);
        assert.equal(api.getState().runtime.runtimeId, runtime.runtimeId);
        await fs.writeFile(path.join(evidence, 'result.json'), JSON.stringify({ before, after, runtime: api.getState().runtime }));
        console.log('PASS installed VSIX Mac Run, Hot Reload command, state preservation');
    } finally {
        await vscode.commands.executeCommand('doroti.stop');
        const doc = await vscode.workspace.openTextDocument(source);
        const edit = new vscode.WorkspaceEdit();
        edit.replace(source, new vscode.Range(doc.positionAt(0), doc.positionAt(doc.getText().length)), original);
        await vscode.workspace.applyEdit(edit); await doc.save();
    }
    assert.equal(api.getState().running, false);
    console.log('PASS installed VSIX Mac Stop');
}
