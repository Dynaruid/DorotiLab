import * as vscode from 'vscode';
import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import type { DorotiSidebar, SidebarState } from '../sidebar';

const pause = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));
async function until(predicate: () => unknown, label: string) {
    const deadline = Date.now() + 30000;
    while (Date.now() < deadline) { if (predicate()) return; await pause(100); }
    throw new Error(`Timed out: ${label}`);
}

export async function run() {
    const extension = vscode.extensions.getExtension('doroti-local.doroti')!;
    assert.ok(extension?.extensionPath.includes('extensions'), 'Exercise installed VSIX');
    const api = await extension.activate();
    const sidebar = api.sidebar as DorotiSidebar;
    const root = vscode.workspace.workspaceFolders![0].uri.fsPath;
    const children = (group: string) => sidebar.getChildren(sidebar.getChildren().find(item => item.id === group)!);
    const item = (id: string, group = 'session') => children(group).find(item => item.id === id)!;
    async function click(id: string, group = 'session', extra?: unknown[]) {
        const command = item(id, group).command;
        assert.ok(command, `${id} is available`);
        return vscode.commands.executeCommand(command.command, ...(extra ?? command.arguments ?? []));
    }
    await vscode.commands.executeCommand('doroti.showSidebar');
    await until(() => sidebar.view.visible, 'Native sidebar visible');
    assert.deepEqual(sidebar.getChildren().map(item => item.id), ['project', 'session', 'widgets']);
    assert.equal(item('stop').command, undefined);
    assert.equal(item('reload').command, undefined);
    assert.equal(item('snippet', 'widgets').command, undefined);
    const doc = await vscode.workspace.openTextDocument(vscode.Uri.file(path.join(root, 'src/EditorProbe.cs')));
    const editor = await vscode.window.showTextDocument(doc);
    await until(() => item('snippet', 'widgets').command, 'Active widget editor');

    if (process.env.DOROTI_TEST_RESTRICTED) {
        assert.equal(vscode.workspace.isTrusted, false);
        for (const id of ['run', 'reload', 'restart', 'stop']) assert.equal(item(id).command, undefined, id);
        for (const id of ['select-project', 'target', 'create-project']) assert.equal(item(id, 'project').command, undefined, id);
        assert.ok(item('trust', 'project').command);
        assert.equal(item('wrap', 'widgets').command, undefined);
        assert.ok(item('stateless', 'widgets').command);
        await click('snippet', 'widgets', ['StatelessWidget']);
        await vscode.commands.executeCommand('leaveSnippet');
        assert.ok(doc.getText().includes('global::Doroti.Framework.Widgets.StatelessWidget'));
        assert.equal(api.editing.assist.child, undefined, 'No helper spawned by restricted sidebar');
        await fs.writeFile(process.env.DOROTI_TEST_RESULT!, JSON.stringify({ version: extension.packageJSON.version, installedVsix: extension.extensionPath, nativeView: 'PASS', restrictedExecution: 'PASS', restrictedSnippet: 'PASS' }, null, 2));
        return;
    }

    assert.equal(vscode.workspace.isTrusted, true);
    await vscode.workspace.getConfiguration('doroti').update('cliPath', path.resolve(__dirname, '../../src/test/fixtures/editing-session.ps1'), vscode.ConfigurationTarget.Global);
    await click('select-project', 'project');
    assert.ok(api.getState().project);
    const targets = sidebar.getChildren(item('target', 'project'));
    assert.equal(targets.length, 1);
    assert.equal(targets[0].label, 'editing-fixture', 'Only provider-declared targets are offered');
    await vscode.commands.executeCommand(targets[0].command!.command, ...targets[0].command!.arguments!);
    assert.equal(api.getState().target, 'editing-fixture');
    assert.equal(item('manifest', 'project').command!.arguments![0].fsPath, path.join(root, 'doroti-workspace.json'));
    let events = 0;
    const changed = sidebar.onDidChangeTreeData(() => events++);
    try {
        const initial: SidebarState = { project: api.getState().project, target: 'editing-fixture', running: false, busy: false, stopping: false, restartRequired: false };
        sidebar.update(initial); events = 0; sidebar.update({ ...initial });
        assert.equal(events, 0, 'Repeated unchanged poll does not rebuild the view');
        sidebar.update({ ...initial, busy: true });
        assert.equal(item('run').command, undefined);
        assert.equal(item('target', 'project').command, undefined);
        assert.ok(item('stop').command, 'Can cancel an operation');
        const runtime = { schemaVersion: 'doroti.dev/v1', sessionId: 'fixture', runtimeId: 'runtime', processId: 1, supported: true, status: 'started', revision: 4 };
        sidebar.update({ ...initial, running: true, runtime, pending: 'request' });
        assert.equal(item('reload').command, undefined);
        assert.equal(item('session-status').label, 'Reloading');
        sidebar.update({ ...initial, running: true, runtime, restartRequired: true });
        assert.equal(item('reload').command, undefined);
        assert.ok(item('restart').command);
        assert.equal(item('session-status').label, 'Restart required');
        sidebar.update({ ...initial, running: true, runtime, problem: 'compile-error' });
        assert.equal(item('session-status').label, 'Compilation failed');
        sidebar.update({ ...initial, running: true, runtime, stopping: true });
        for (const id of ['run', 'reload', 'restart', 'stop']) assert.equal(item(id).command, undefined, id);
        sidebar.update(initial);
        await click('run');
        await until(() => api.getState().runtime?.supported, 'Fixture runtime ready');
        assert.equal(item('run').command, undefined);
        assert.equal(item('select-project', 'project').command, undefined);
        assert.ok(item('reload').command);
        const before = api.getState().runtime;
        await editor.edit(edit => edit.insert(doc.positionAt(doc.getText().length), '// sidebar reload\n'));
        await click('reload', 'session', [true]);
        await until(() => api.getState().runtime?.revision > before.revision && !api.getState().pending, 'Sidebar reload acknowledgment');
        assert.equal(api.getState().runtime.runtimeId, before.runtimeId);
        assert.equal(item('session-status').description, `Revision ${api.getState().runtime.revision}`);
        await vscode.commands.executeCommand('doroti.restart', true);
        await until(() => api.getState().runtime?.supported && api.getState().runtime.runtimeId !== before.runtimeId, 'Restart uses new runtime');
        await click('stop');
        assert.equal(api.getState().running, false);
        assert.equal(item('reload').command, undefined);
        assert.ok(item('run').command);
        assert.ok(events > 0, 'Session changes refresh the sidebar');
        const unrelated = await vscode.workspace.openTextDocument(vscode.Uri.file(path.join(root, 'unrelated/Plain.cs')));
        await vscode.window.showTextDocument(unrelated);
        await until(() => !item('snippet', 'widgets').command, 'Unrelated C# editor disables widgets');
        await vscode.commands.executeCommand('doroti.refreshSidebar');
        await fs.writeFile(process.env.DOROTI_TEST_RESULT!, JSON.stringify({ version: extension.packageJSON.version, vscodeVersion: vscode.version, installedVsix: extension.extensionPath, nativeView: 'PASS', stateGuards: 'PASS', providerTargets: 'PASS', runReloadRestartStopFixture: 'PASS', refreshDeduplication: 'PASS', editorContext: 'PASS', nativeAppReload: 'notVerified' }, null, 2));
    } finally { changed.dispose(); await vscode.commands.executeCommand('doroti.stop'); }
}
