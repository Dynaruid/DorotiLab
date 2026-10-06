import * as vscode from 'vscode';
import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import { performance } from 'node:perf_hooks';
import { Analysis, Transformation, CatalogType } from '../editing/assistClient';
const pause = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));
const p95 = (values: number[]) => [...values].sort((a, b) => a - b)[Math.ceil(values.length * .95) - 1];
export async function run() {
    const extension = vscode.extensions.getExtension('doroti-local.doroti')!;
    assert.ok(extension?.extensionPath.includes('extensions'), 'installed VSIX required');
    const api = await extension.activate();
    if (process.env.DOROTI_TEST_CSHARP) {
        const csharp = vscode.extensions.getExtension('ms-dotnettools.csharp'); assert.ok(csharp);
        await csharp.activate();
    }
    await pause(1800);
    const root = vscode.workspace.workspaceFolders![0].uri.fsPath;
    const doc = await vscode.workspace.openTextDocument(vscode.Uri.file(path.join(root, 'src/EditorProbe.cs')));
    let editor = await vscode.window.showTextDocument(doc, { preview: false });
    await vscode.workspace.getConfiguration('editor').update('snippetSuggestions', 'top', vscode.ConfigurationTarget.Global);
    await vscode.workspace.getConfiguration('editor').update('suggestSelection', 'first', vscode.ConfigurationTarget.Global);
    assert.equal(api.getState().project, undefined, 'editing works before project selection');
    const app = await api.editing.contexts.get(doc); assert.ok(app);
    if (vscode.workspace.isTrusted) {
        for (let attempt = 0; ; attempt++) {
            try { await api.editing.assist.request('analyze', app, doc, 0, 0); break; }
            catch (error) { if (!(error instanceof vscode.CancellationError) || attempt >= 2) throw error; await pause(400); }
        }
        await pause(600);
    }
    async function replace(text: string) {
        editor = await vscode.window.showTextDocument(doc, { preview: false });
        await vscode.commands.executeCommand('leaveSnippet');
        await editor.edit(e => { e.setEndOfLine(text.includes('\r\n') ? vscode.EndOfLine.CRLF : vscode.EndOfLine.LF); e.replace(new vscode.Range(doc.positionAt(0), doc.positionAt(doc.getText().length)), text); });
        editor.selection = new vscode.Selection(doc.positionAt(text.length), doc.positionAt(text.length));
    }
    async function completions(offset = doc.getText().length) {
        return vscode.commands.executeCommand<vscode.CompletionList>('vscode.executeCompletionItemProvider', doc.uri, doc.positionAt(offset));
    }
    async function prefix(value: string, className: string, initial = 'namespace Probe;\n') {
        await replace(initial);
        await vscode.commands.executeCommand('type', { text: value });
        let items = await completions();
        for (let i = 0; i < 4 && !items!.items.some(item => item.detail?.startsWith('Doroti snippet:')); i++) { await pause(400); items = await completions(); }
        if (!items!.items.some(i => i.detail?.startsWith('Doroti snippet:'))) console.log('Prefix probe', JSON.stringify({ text: doc.getText(), position: doc.getText().length, items: items!.items.map(i => ({ label: i.label, detail: i.detail })), analysis: vscode.workspace.isTrusted ? await api.editing.assist.request('analyze', app, doc, doc.getText().length, 0) : 'restricted' }));
        assert.equal(items!.items.filter(i => i.detail?.startsWith('Doroti snippet:') && i.label === value).length, 1, 'prefix appears exactly once');
        await vscode.window.showTextDocument(doc);
        await vscode.commands.executeCommand('workbench.action.focusActiveEditorGroup');
        await vscode.commands.executeCommand('editor.action.triggerSuggest'); await pause(2000);
        await vscode.commands.executeCommand('acceptSelectedSuggestion'); await pause(100);
        if (!doc.getText().includes('class EditorProbe')) console.log('Selected suggestion text', JSON.stringify(doc.getText()));
        assert.ok(doc.getText().includes('${') === false && doc.getText().includes('class EditorProbe'), 'actual suggestion selection expands snippet');
        await vscode.commands.executeCommand('type', { text: className });
        assert.ok(doc.getText().includes('class ' + className));
        if (value.includes('f') || value === 'dstateful') assert.ok(doc.getText().includes(`State<${className}>`) && doc.getText().includes(`new ${className}State()`), 'linked class/State names');
        await vscode.commands.executeCommand('jumpToNextSnippetPlaceholder');
        assert.ok(editor.selection.end.isAfter(editor.selection.start), 'Tab selects build expression');
        await vscode.commands.executeCommand('leaveSnippet');
        if (vscode.workspace.isTrusted) assert.equal(doc.getText().match(/using Doroti.Framework.Widgets;/g)?.length, 1, 'one import');
        else assert.ok(doc.getText().includes('global::Doroti.Framework.Widgets'));
    }
    await prefix('stf', 'EditorCounter');
    await prefix('stl', 'EditorLabel');
    await prefix('dstateful', 'LegacyCounter');
    await prefix('stf', 'BareCounter', '');
    await replace('namespace Probe;\n');
    assert.equal(await vscode.commands.executeCommand('doroti.insertWidgetSnippet', 'StatelessWidget'), true);
    await vscode.commands.executeCommand('type', { text: 'ExplicitWidget' }); await vscode.commands.executeCommand('leaveSnippet');
    assert.ok(doc.getText().includes('class ExplicitWidget'));
    const unrelated = await vscode.workspace.openTextDocument(vscode.Uri.file(path.join(root, 'unrelated/Plain.cs')));
    assert.equal(await api.editing.contexts.get(unrelated), undefined);
    const outside = await vscode.commands.executeCommand<vscode.CompletionList>('vscode.executeCompletionItemProvider', unrelated.uri, new vscode.Position(0, 6));
    assert.ok(!outside?.items.some(i => i.detail?.startsWith('Doroti')));
    if (process.env.DOROTI_TEST_RESTRICTED) {
        assert.equal(vscode.workspace.isTrusted, false);
        await doc.save();
        await fs.writeFile(process.env.DOROTI_TEST_RESULT!, JSON.stringify({ installedVsix: extension.extensionPath, restrictedTemplates: 'PASS', helperExecution: 'disabled', unrelatedProject: 'PASS' }, null, 2));
        return;
    }
    const fixture = path.resolve(__dirname, '../../src/test/fixtures/editing-session.ps1');
    await vscode.workspace.getConfiguration('doroti').update('cliPath', fixture, vscode.ConfigurationTarget.Global);
    await vscode.commands.executeCommand('doroti.selectProject');
    await vscode.commands.executeCommand('doroti.selectTarget', 'editing-fixture');
    await vscode.commands.executeCommand('doroti.run');
    const waitRuntime = async (predicate: () => boolean) => {
        const deadline = Date.now() + 30000;
        while (!predicate()) { if (Date.now() > deadline) throw new Error('Timed out: development contract fixture'); await pause(100); }
    };
    await waitRuntime(() => api.getState().runtime?.supported);
    await replace('namespace Probe;\nclass Missing : StatelessWidget { public override Widget build(BuildContext context) => new Text("Hello"); }');
    const missingOffset = doc.getText().indexOf('StatelessWidget');
    const missingRange = new vscode.Range(doc.positionAt(missingOffset), doc.positionAt(missingOffset + 'StatelessWidget'.length));
    const diagnostics = vscode.languages.createDiagnosticCollection('doroti-editing-test');
    const diagnostic = new vscode.Diagnostic(missingRange, 'Missing type'); diagnostic.code = 'CS0246'; diagnostics.set(doc.uri, [diagnostic]);
    const fixes = await vscode.commands.executeCommand<vscode.CodeAction[]>('vscode.executeCodeActionProvider', doc.uri, missingRange, vscode.CodeActionKind.QuickFix.value);
    const fix = fixes!.find(a => a.title === 'Import Doroti.Framework.Widgets.StatelessWidget'); assert.ok(fix?.command);
    await vscode.commands.executeCommand(fix.command.command, ...fix.command.arguments!);
    assert.equal(doc.getText().match(/using Doroti.Framework.Widgets;/g)?.length, 1); diagnostics.dispose();
    await vscode.commands.executeCommand('undo'); assert.ok(!doc.getText().includes('using Doroti.Framework.Widgets;'));
    const source = 'using Doroti.Framework.Widgets;\r\nnamespace Probe;\r\npublic sealed class Demo : StatelessWidget\r\n{\r\n    public string Title { get; } = "한글";\r\n    public override Widget build(BuildContext context) => new Row(children: [new Text(Title), /*keep*/ new Text("B"), new Text("C")]);\r\n}\r\n';
    await replace(source);
    let offset = source.indexOf('new Text(Title)') + 5;
    async function codeActions(start = offset, length = 0) {
        return vscode.commands.executeCommand<vscode.CodeAction[]>('vscode.executeCodeActionProvider', doc.uri, new vscode.Range(doc.positionAt(start), doc.positionAt(start + length)), vscode.CodeActionKind.Refactor.value);
    }
    let actions = await codeActions(); const padding = actions!.find(a => a.title === 'Wrap with Padding')!; assert.ok(padding?.command);
    await vscode.commands.executeCommand(padding.command.command, ...padding.command.arguments!);
    assert.ok(doc.getText().includes('EdgeInsets.CreateAll(8)') && doc.getText().includes('/*keep*/'));
    assert.ok(doc.isDirty && api.getState().running, 'editing works while development session runs and remains unsaved');
    await vscode.commands.executeCommand('undo'); assert.equal(doc.getText(), source, 'wrapper is one Undo transaction');
    await vscode.commands.executeCommand('redo'); assert.ok(doc.getText().includes('EdgeInsets.CreateAll(8)'));
    await replace(source);
    const first = source.indexOf('new Text(Title)'); const last = source.indexOf('new Text("B")') + 'new Text("B")'.length;
    const listActions = await codeActions(first, last - first); const column = listActions!.find(a => a.title === 'Wrap with Column')!;
    await vscode.commands.executeCommand(column.command!.command, ...column.command!.arguments!);
    assert.ok(doc.getText().includes('Column') && doc.getText().includes('/*keep*/') && doc.getText().includes('"C"'));
    await replace(source);
    const stale = padding.command!.arguments!;
    await vscode.commands.executeCommand('type', { text: '//changed\r\n' });
    const staleSource = doc.getText();
    // Call the shared application path directly so the expected error is observable without a notification.
    await assert.rejects(api.editing.actions.apply(...stale), /Document changed/); assert.equal(doc.getText(), staleSource);
    await replace(source);
    const simple = source.replace('new Row(children: [new Text(Title), /*keep*/ new Text("B"), new Text("C")])', 'new Text(Title)');
    await replace(simple); offset = simple.indexOf('new Text(Title)') + 5;
    const transform = await api.editing.assist.request('transform', app, doc, offset, 0, undefined, 'extract', 'ExtractedLabel') as Transformation;
    assert.ok(transform.preview && transform.edits.length);
    // Exercise the real diff and Cancel/Apply quick-pick path.
    async function preview(id: string, where: number, cancel: boolean, name?: string) {
        const promise = api.editing.actions.apply(doc.uri, doc.version, where, 0, id, name);
        await pause(1400);
        await vscode.commands.executeCommand(cancel ? 'workbench.action.closeQuickOpen' : 'workbench.action.acceptSelectedQuickOpenItem');
        return promise;
    }
    assert.equal(await preview('extract', offset, true, 'ExtractedLabel'), false); assert.equal(doc.getText(), simple);
    assert.equal(await preview('extract', offset, false, 'ExtractedLabel'), true);
    assert.ok(doc.getText().includes('class ExtractedLabel') && doc.getText().includes('new ExtractedLabel(Title)'));
    await vscode.commands.executeCommand('undo'); assert.equal(doc.getText(), simple);
    const classOffset = simple.indexOf('class Demo') + 7;
    assert.equal(await preview('stateful', classOffset, false), true);
    assert.ok(doc.getText().includes('StatefulWidget') && doc.getText().includes('currentWidget.Title'));
    await vscode.commands.executeCommand('undo'); assert.equal(doc.getText(), simple);
    await replace(source);
    const catalog = await api.editing.assist.catalog(app, doc) as CatalogType[];
    const carousel = catalog.find(t => t.name === 'CustomCarousel'); assert.ok(carousel);
    assert.deepEqual(carousel.constructors[0].find(p => p.name === 'effectsBuilder')!.delegateParameters, ['index', 'scrollRatio', 'child']);
    await replace('using Doroti.Framework.Widgets;\nnamespace Probe;\nclass Arguments : StatelessWidget { public override Widget build(BuildContext context) => new Padding( ); }');
    const argumentOffset = doc.getText().indexOf('Padding(') + 8;
    const argumentItems = await completions(argumentOffset);
    assert.ok(argumentItems!.items.some(i => i.detail === 'Doroti argument template' && i.filterText === 'padding'));
    await replace('using Doroti.Framework.Widgets;\nnamespace Probe;\nclass Arguments : StatelessWidget { public override Widget build(BuildContext context) => new Column(children: [] ); }');
    const existingArguments = await completions(doc.getText().indexOf('[]') + 3);
    assert.ok(!existingArguments!.items.some(i => i.detail === 'Doroti argument template' && i.filterText === 'children'));
    await replace('using Doroti.Framework.Widgets;\nnamespace Probe;\nclass Demo : StatefulWidget { public override IState createState() => new DemoState(); } class DemoState : State<Demo> { public override Widget build(BuildContext context) => new SizedBox();\ndinit }');
    const stateItems = await completions(doc.getText().indexOf('dinit') + 5);
    assert.ok(stateItems!.items.some(i => i.detail === 'Doroti snippet: initState'));
    await replace(doc.getText().replace('dinit', 'public override void initState() { base.initState(); }\ndinit'));
    const duplicateStateItems = await completions(doc.getText().lastIndexOf('dinit') + 5);
    assert.ok(!duplicateStateItems!.items.some(i => i.detail === 'Doroti snippet: initState'));
    const completionTimes: number[] = []; const actionTimes: number[] = [];
    await replace('namespace Probe;\nstl'); await completions();
    const warmedStats = await api.editing.assist.request('stats', app, doc, 0, 0);
    for (let i = 0; i < 20; i++) { const before = performance.now(); await completions(); completionTimes.push(performance.now() - before); }
    await replace(source); offset = source.indexOf('new Text(Title)') + 5; await codeActions();
    for (let i = 0; i < 20; i++) { const before = performance.now(); await codeActions(); actionTimes.push(performance.now() - before); }
    const cachedStats = await api.editing.assist.request('stats', app, doc, 0, 0);
    assert.equal(cachedStats.projectLoads, warmedStats.projectLoads, 'cached requests never reload the project');
    await replace('namespace Probe;\n// stl'); assert.ok(!(await completions())!.items.some(i => i.detail?.startsWith('Doroti snippet:')));
    await replace('namespace Probe;\nclass Raw { string Value = """stl"""; }');
    const rawCompletions = await completions(doc.getText().indexOf('stl') + 2); assert.ok(!rawCompletions!.items.some(i => i.detail?.startsWith('Doroti snippet:')));
    await replace(source); await doc.save();
    await vscode.commands.executeCommand('type', { text: '// development contract edit\r\n' });
    const runtimeBefore = api.getState().runtime;
    await vscode.commands.executeCommand('doroti.hotReload', true);
    await waitRuntime(() => api.getState().runtime?.revision > runtimeBefore.revision && !api.getState().pending);
    assert.equal(api.getState().runtime.runtimeId, runtimeBefore.runtimeId, 'fixture acknowledgment preserves runtime identity');
    await replace(source); await doc.save();
    await vscode.commands.executeCommand('doroti.stop'); assert.equal(api.getState().running, false);
    const newUri = vscode.Uri.file(path.join(root, 'src/새위젯.cs'));
    assert.equal(await vscode.commands.executeCommand('doroti.newStatefulWidget', { uri: newUri, name: '새위젯', namespace: 'Probe', key: true }), true);
    const newDoc = await vscode.workspace.openTextDocument(newUri); assert.ok(newDoc.getText().includes('State<새위젯>')); await newDoc.save();
    const existingText = newDoc.getText();
    assert.equal(await vscode.commands.executeCommand('doroti.newStatefulWidget', { uri: newUri, name: '새위젯', namespace: 'Probe', key: true }), false);
    assert.equal(newDoc.getText(), existingText, 'file collision never overwrites');
    const cancelCreation = vscode.commands.executeCommand('doroti.newStatelessWidget');
    await pause(400); await vscode.commands.executeCommand('workbench.action.closeQuickOpen'); assert.equal(await cancelCreation, false);
    await pause(700); // Let source/create watchers settle before reading helper ownership counters.
    await assert.rejects(api.editing.contexts.get(newDoc).then(() => api.editing.assist.request('transform', app, newDoc, 0, 0, { isCancellationRequested: true, onCancellationRequested: () => ({ dispose() {} }) })), /Canceled|Cancelled|canceled|cancelled/);
    const stats = await api.editing.assist.request('stats', app, newDoc, 0, 0);
    const result = { installedVsix: extension.extensionPath, vscode: vscode.version, sdk: '10.0.400', roslyn: '5.9.0', csharp: vscode.extensions.getExtension('ms-dotnettools.csharp')?.packageJSON.version ?? 'absent', prefixSelectionTabLinkedNamesImports: 'PASS', editingDuringFixtureRunAndReloadStop: 'PASS', nativeProductRunReload: 'notVerified', explicitSnippetAndQuickFix: 'PASS', wrapperUndoRedo: 'PASS', listTrivia: 'PASS', staleReject: 'PASS', extractPreviewCancelApplyUndo: 'PASS', statefulPreviewApplyUndo: 'PASS', newUnicodeWidgetCollisionCancel: 'PASS', argumentsLifecycleNoDuplicates: 'PASS', projectSelectionNotRequired: 'PASS', unrelatedProject: 'PASS', rawStringAndComment: 'PASS', carouselCatalog: 'PASS', cancellation: 'PASS', completionP95Ms: p95(completionTimes), actionsP95Ms: p95(actionTimes), repetitions: 20, stats, physicalEditorInput: 'notVerified', macOSLinux: 'notVerified' };
    await fs.writeFile(process.env.DOROTI_TEST_RESULT!, JSON.stringify(result, null, 2));
    console.log(JSON.stringify(result));
    assert.ok(result.completionP95Ms <= 100, 'prepared completion p95 <=100 ms');
    assert.ok(result.actionsP95Ms <= 200, 'cached actions p95 <=200 ms');
    await api.editing.assist.shutdown();
}
