import * as vscode from 'vscode';
import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
const pause = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));
async function until(predicate: () => unknown | Promise<unknown>, label: string, ms = 180000) {
    const deadline = Date.now() + ms;
    while (Date.now() < deadline) { if (await predicate()) return; await pause(200); }
    throw new Error(`Timed out: ${label}`);
}
export async function run(): Promise<void> {
    const extension = vscode.extensions.getExtension('doroti-local.doroti');
    assert.ok(extension, 'VSIX was not installed into the clean profile');
    assert.ok(extension.extensionPath.includes('extensions'), 'Test must exercise installed VSIX');
    const api = await extension.activate();
    await pause(1200); // Let the clean workbench finish restoring editors before editing.
    await vscode.workspace.getConfiguration('doroti').update('cliPath', process.env.DOROTI_TEST_CLI!, vscode.ConfigurationTarget.Global);
    await vscode.commands.executeCommand('doroti.selectProject');
    assert.ok(api.getState().project, 'CLI manifest selection');
    const app = api.getState().project.root as string;
    const probe = vscode.Uri.file(path.join(app, 'src', 'EditorProbe.cs'));
    await fs.writeFile(probe.fsPath, 'using Doroti.Framework.Widgets;\nnamespace EditorProbe;\n');
    const doc = await vscode.workspace.openTextDocument(probe);
    const editor = await vscode.window.showTextDocument(doc);
    editor.selection = new vscode.Selection(doc.positionAt(doc.getText().length), doc.positionAt(doc.getText().length));
    await vscode.commands.executeCommand('editor.action.insertSnippet', { langId: 'csharp', name: 'StatefulWidget' });
    await vscode.commands.executeCommand('type', { text: 'EditorCounter' });
    assert.ok(doc.getText().includes('State<EditorCounter>') && doc.getText().includes('new EditorCounterState()'), 'linked snippet names');
    await vscode.commands.executeCommand('jumpToNextSnippetPlaceholder');
    await vscode.commands.executeCommand('leaveSnippet');
    await doc.save();
    // Completion uses real language-provider API and edits a file with a file-scoped namespace.
    const completionDoc = await vscode.workspace.openTextDocument({ language: 'csharp', content: 'namespace Probe;\nclass A { Text value; }' });
    // Provider intentionally rejects untitled/non-project documents.
    const outside = await vscode.commands.executeCommand<vscode.CompletionList>('vscode.executeCompletionItemProvider', completionDoc.uri, new vscode.Position(1, 14));
    assert.ok(!outside?.items.some(item => item.detail?.startsWith('Doroti import:')));
    await editor.edit(edit => edit.replace(new vscode.Range(new vscode.Position(0, 0), new vscode.Position(1, 0)), ''));
    const position = doc.positionAt(doc.getText().indexOf('Stat') + 3);
    const items = await vscode.commands.executeCommand<vscode.CompletionList>('vscode.executeCompletionItemProvider', doc.uri, position);
    const completion = items.items.find(item => item.detail === 'Doroti import: Doroti.Framework.Widgets' && (typeof item.label === 'string' ? item.label : item.label.label) === 'StatefulWidget');
    assert.ok(completion?.additionalTextEdits?.length, 'Doroti completion import');
    await editor.edit(edit => { for (const change of completion.additionalTextEdits!) edit.replace(change.range, change.newText); });
    assert.equal(doc.getText().match(/using Doroti.Framework.Widgets;/g)?.length, 1);
    await vscode.commands.executeCommand('undo');
    assert.ok(!doc.getText().includes('using Doroti.Framework.Widgets;'), 'import undo');
    const collection = vscode.languages.createDiagnosticCollection('doroti-test');
    const offset = doc.getText().indexOf('StatefulWidget');
    const range = new vscode.Range(doc.positionAt(offset), doc.positionAt(offset + 'StatefulWidget'.length));
    const diagnostic = new vscode.Diagnostic(range, 'Missing type'); diagnostic.code = 'CS0246'; collection.set(doc.uri, [diagnostic]);
    const fixes = await vscode.commands.executeCommand<vscode.CodeAction[]>('vscode.executeCodeActionProvider', doc.uri, range, vscode.CodeActionKind.QuickFix.value);
    const fix = fixes.find(item => item.title === 'Import Doroti.Framework.Widgets.StatefulWidget');
    assert.ok(fix?.edit, 'Doroti compiler Quick Fix');
    await vscode.workspace.applyEdit(fix.edit); collection.dispose(); await doc.save();
    console.log('PASS: installed VSIX snippet linked names/Tab; completion import; Quick Fix; Undo.');

    await vscode.commands.executeCommand('doroti.selectTarget', 'windows');
    assert.equal(api.getState().target, 'windows');
    await vscode.commands.executeCommand('doroti.run');
    await until(() => api.getState().runtime?.supported, 'Windows metadata capability', 300000);
    const before = api.getState().runtime;
    await vscode.commands.executeCommand('doroti.run'); // Must not start a second session.
    assert.equal(api.getState().runtime.runtimeId, before.runtimeId);
    const source = vscode.Uri.file(path.join(app, 'src', 'HotReloadSample.cs'));
    const sourceDoc = await vscode.workspace.openTextDocument(source);
    const sourceEditor = await vscode.window.showTextDocument(sourceDoc);
    const original = sourceDoc.getText();
    try {
        await sourceEditor.edit(edit => edit.replace(new vscode.Range(sourceDoc.positionAt(0), sourceDoc.positionAt(original.length)), original.replace('Before reload', 'Reload from installed VSIX')));
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => api.getState().runtime?.revision > before.revision && !api.getState().pending, 'Hot Reload button acknowledgment');
        const result = JSON.parse(await fs.readFile(process.env.DOROTI_RELOAD_PROBE!, 'utf8'));
        assert.equal(result.message, 'Reload from installed VSIX');
        assert.equal(result.count, 5); assert.equal(result.text, '한글 유지'); assert.equal(result.scroll, 160);
        assert.equal(api.getState().runtime.runtimeId, before.runtimeId);
        console.log('PASS: installed VSIX Run; duplicate Run; actual Hot Reload command; native state/input/scroll preserved.');
        const revision = api.getState().runtime.revision;
        await sourceEditor.edit(edit => edit.replace(new vscode.Range(sourceDoc.positionAt(0), sourceDoc.positionAt(sourceDoc.getText().length)), original.replace('"Before reload"', 'MissingReloadValue')));
        await vscode.commands.executeCommand('doroti.hotReload', true);
        await until(() => api.getState().problem === 'compile-error', 'Compiler error reporting');
        assert.equal(api.getState().runtime.revision, revision);
        await sourceEditor.edit(edit => edit.replace(new vscode.Range(sourceDoc.positionAt(0), sourceDoc.positionAt(sourceDoc.getText().length)), original.replace('Before reload', 'Recovered reload')));
        await Promise.all([vscode.commands.executeCommand('doroti.hotReload', true), vscode.commands.executeCommand('doroti.hotReload', true)]);
        await until(() => api.getState().runtime?.revision > revision && !api.getState().pending, 'Retry after compiler error');
        assert.equal(api.getState().runtime.runtimeId, before.runtimeId);
        const recovered = JSON.parse(await fs.readFile(process.env.DOROTI_RELOAD_PROBE!, 'utf8'));
        assert.equal(recovered.message, 'Recovered reload'); assert.equal(recovered.stateId, result.stateId);
        console.log('PASS: compiler error does not acknowledge delta; correction retries; concurrent clicks serialize.');
    } finally {
        await vscode.commands.executeCommand('doroti.stop');
        assert.equal(api.getState().running, false);
        await sourceEditor.edit(edit => edit.replace(new vscode.Range(sourceDoc.positionAt(0), sourceDoc.positionAt(sourceDoc.getText().length)), original));
        await sourceDoc.save();
    }
    await fs.writeFile(process.env.DOROTI_TEST_RESULT!, JSON.stringify({ installedVsix: extension.extensionPath, editing: 'passed', windowsMetadataReload: 'passed', compileErrorRetry: 'passed', concurrentReload: 'passed', stop: 'passed', physicalEditorInteraction: 'notVerified', wizardDialogs: 'notVerified' }, null, 2));
}
