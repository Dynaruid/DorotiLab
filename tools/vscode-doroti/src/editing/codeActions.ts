import * as vscode from 'vscode';
import { ProjectContexts } from './projectContext';
import { AssistClient, Analysis, CatalogType, Transformation } from './assistClient';
import { importPlan } from '../imports';
import { EditPreview } from './edits';
import { validateIdentifier } from './catalog';

export class EditingActions implements vscode.CodeActionProvider {
    constructor(private contexts: ProjectContexts, private assist: AssistClient, private preview: EditPreview, private logs: vscode.OutputChannel) {}
    async provideCodeActions(doc: vscode.TextDocument, range: vscode.Range, context: vscode.CodeActionContext, token: vscode.CancellationToken) {
        if (!vscode.workspace.getConfiguration('doroti.editing', doc.uri).get('codeActions', true) || !vscode.workspace.isTrusted) return [];
        const app = await this.contexts.get(doc); if (!app) return [];
        const offset = doc.offsetAt(range.start); const length = doc.offsetAt(range.end) - offset;
        const version = doc.version;
        try {
            const analysis = await this.assist.request<Analysis>('analyze', app, doc, offset, length, token);
            if (!analysis.included || !analysis.code || doc.version !== version) return [];
            const actions: vscode.CodeAction[] = [];
            for (const info of analysis.actions) {
                const kind = info.id === 'extract' ? vscode.CodeActionKind.RefactorExtract : vscode.CodeActionKind.RefactorRewrite;
                if (context.only && !context.only.contains(kind)) continue;
                const action = new vscode.CodeAction(info.title, kind);
                if (info.reason) action.disabled = { reason: info.reason };
                action.command = { command: 'doroti.applyWidgetAction', title: info.title, arguments: [doc.uri, version, offset, length, info.id] };
                actions.push(action);
            }
            if (!context.only || context.only.contains(vscode.CodeActionKind.QuickFix)) {
                const types = await this.assist.catalog(app, doc, token);
                for (const diagnostic of context.diagnostics) {
                    const code = typeof diagnostic.code === 'object' ? diagnostic.code.value : diagnostic.code;
                    if (!['CS0246', 'CS0103'].includes(String(code))) continue;
                    const name = doc.getText(diagnostic.range);
                    for (const type of types.filter(t => t.name === name)) {
                        const plan = importPlan(doc.getText(), name, type.namespace, analysis.globalImports);
                        const qualified = plan.qualified || analysis.conflicts.includes(name);
                        if (!plan.text && !qualified) continue;
                        const fix = new vscode.CodeAction(`Import ${type.namespace}.${name}`, vscode.CodeActionKind.QuickFix);
                        fix.diagnostics = [diagnostic]; fix.command = { command: 'doroti.applyWidgetImport', title: fix.title,
                            arguments: [doc.uri, version, qualified ? [{ start: doc.offsetAt(diagnostic.range.start), length: name.length, text: type.fullName }] : [{ start: plan.offset, length: 0, text: plan.text }]] };
                        actions.push(fix);
                    }
                }
            }
            return actions;
        } catch (error) { if (!(error instanceof vscode.CancellationError)) this.logs.appendLine(`[editing] ${error}`); return []; }
    }
    async apply(uri: vscode.Uri, version: number, offset: number, length: number, id: string, name?: string, skipPreview = false) {
        const doc = await vscode.workspace.openTextDocument(uri);
        if (!vscode.workspace.getConfiguration('doroti.editing', uri).get('codeActions', true)) return false;
        if (doc.version !== version) throw new Error('Document changed; request the Doroti action again.');
        const app = await this.contexts.get(doc); if (!app || !vscode.workspace.isTrusted) return false;
        if (id === 'extract' && !name) {
            name = await vscode.window.showInputBox({ title: 'Extracted widget class name', value: 'ExtractedWidget', validateInput: validateIdentifier });
            if (!name) return false;
        }
        const transform = await this.assist.request<Transformation>('transform', app, doc, offset, length, undefined, id, name);
        const result = await this.preview.apply(doc, version, transform, !skipPreview && transform.preview);
        if (result && ['extract', 'stateful'].includes(id)) void vscode.window.showInformationMessage('Class/type changes may require Doroti: Restart (resets state).');
        return result;
    }
    async wrap() {
        const editor = vscode.window.activeTextEditor; if (!editor) return;
        if (!vscode.workspace.getConfiguration('doroti.editing', editor.document.uri).get('codeActions', true)) return;
        const doc = editor.document; const app = await this.contexts.get(doc); if (!app || !vscode.workspace.isTrusted) return;
        const version = doc.version; const offset = doc.offsetAt(editor.selection.start); const length = doc.offsetAt(editor.selection.end) - offset;
        const analysis = await this.assist.request<Analysis>('analyze', app, doc, offset, length);
        const options = analysis.actions.filter(a => a.id.startsWith('wrap:') && !a.reason).map(a => ({ label: a.title, id: a.id }));
        if (!options.length) { void vscode.window.showInformationMessage(analysis.reason ?? 'Place the cursor on a resolved Doroti Widget expression or select complete children items.'); return; }
        const choice = await vscode.window.showQuickPick(options, { title: 'Wrap with Widget' });
        if (choice) return this.apply(doc.uri, version, offset, length, choice.id);
    }
}
