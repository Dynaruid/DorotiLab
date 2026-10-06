import * as vscode from 'vscode';
import * as path from 'node:path';
import { Templates, instantiate, defaultName, validateIdentifier } from './catalog';
import { ProjectContexts } from './projectContext';
import { AssistClient, Analysis } from './assistClient';

export interface NewWidgetOptions { uri: vscode.Uri; name: string; namespace?: string; key?: boolean; }
export class WidgetTemplates {
    constructor(private templates: Templates, private contexts: ProjectContexts, private assist: AssistClient) {}
    async insert(name?: string) {
        const editor = vscode.window.activeTextEditor; if (!editor) return false;
        const doc = editor.document; const app = await this.contexts.get(doc); if (!app) return false;
        if (!vscode.workspace.getConfiguration('doroti.editing', doc.uri).get('snippets', true)) return false;
        const version = doc.version;
        let analysis: Analysis | undefined;
        if (vscode.workspace.isTrusted) {
            try { analysis = await this.assist.request<Analysis>('analyze', app, doc, doc.offsetAt(editor.selection.start), 0); } catch { /* trust-free template fallback */ }
        }
        const available = Object.entries(this.templates).filter(([label, t]) => (!t.package || app.packages.includes(t.package))
            && (t.scope !== 'state' || analysis?.context === (t.prefix === 'dsetstate' ? 'stateBody' : 'state')) && (!t.member || !analysis?.existingMembers.includes(t.member))
            && (!label.startsWith('Owned ') || !analysis?.existingMembers.some(m => ['initState', 'dispose'].includes(m)))
            && (!['Expanded', 'Flexible'].includes(label) || analysis?.actions.some(a => a.id === 'wrap:' + label)));
        if (!name) name = (await vscode.window.showQuickPick(available.map(([label, t]) => ({ label, description: (Array.isArray(t.prefix) ? t.prefix : [t.prefix]).join(', ') })), { title: 'Insert Doroti Widget Snippet' }))?.label;
        if (!name || !available.some(([key]) => key === name)) return false;
        if (doc.version !== version) throw new Error('Document changed; request the template again.');
        const result = instantiate(this.templates[name], doc.getText(), defaultName(doc.fileName), analysis?.globalImports ?? [], analysis?.conflicts, !analysis?.semantic);
        if (result.edits.length) {
            await editor.edit(builder => { for (const edit of result.edits) builder.insert(doc.positionAt(edit.start), edit.text); }, { undoStopBefore: true, undoStopAfter: false });
        }
        return editor.insertSnippet(new vscode.SnippetString(result.body.replace('__STATE_TYPE__', analysis?.stateType ?? 'StatefulWidget')), undefined,
            { undoStopBefore: result.edits.length === 0, undoStopAfter: true });
    }
    async create(stateful: boolean, options?: NewWidgetOptions) {
        if (!options) {
            const editor = vscode.window.activeTextEditor;
            const name = await vscode.window.showInputBox({ title: stateful ? 'New Stateful Widget' : 'New Stateless Widget', value: 'MyWidget', validateInput: validateIdentifier });
            if (!name) return false;
            const namespaceDefault = editor?.document.getText().match(/\bnamespace\s+([\p{L}\p{Nl}_][\p{L}\p{Nl}\p{Nd}_.]*)/u)?.[1] ?? '';
            const namespace = await vscode.window.showInputBox({ title: 'Namespace (empty for global namespace)', value: namespaceDefault,
                validateInput: value => value && value.split('.').some(p => validateIdentifier(p)) ? 'Use valid C# namespace segments.' : undefined });
            if (namespace === undefined) return false;
            const uri = await vscode.window.showSaveDialog({ title: 'Create widget file', defaultUri: vscode.Uri.file(path.join(editor ? path.dirname(editor.document.fileName) : vscode.workspace.workspaceFolders?.[0]?.uri.fsPath ?? '.', name + '.cs')), filters: { 'C#': ['cs'] } });
            if (!uri) return false;
            const key = await vscode.window.showQuickPick(['Include key constructor', 'Minimal template'], { title: 'Widget constructor' });
            if (!key) return false;
            options = { uri, name, namespace, key: key === 'Include key constructor' };
        }
        if (validateIdentifier(options.name) || options.namespace && options.namespace.split('.').some(p => validateIdentifier(p))) throw new Error('Invalid C# class name or namespace');
        if (options.uri.scheme !== 'file' || !options.uri.fsPath.endsWith('.cs')) throw new Error('Choose a local .cs file');
        try { await vscode.workspace.fs.stat(options.uri); throw new Error('File already exists; choose a new file name.'); }
        catch (error) { if (!(error instanceof vscode.FileSystemError) || error.code !== 'FileNotFound') throw error; }
        const active = vscode.window.activeTextEditor?.document;
        if (active && vscode.workspace.isTrusted) {
            const app = await this.contexts.get(active);
            if (app) {
                let types;
                try { types = await this.assist.catalog(app, active); } catch { /* A new-file template remains available when references are unavailable. */ }
                if (types?.some(t => t.name === options!.name && t.namespace === (options!.namespace ?? '')))
                    throw new Error('Class name already exists in this namespace; choose another name.');
            }
        }
        const template = this.templates[(stateful ? 'StatefulWidget' : 'StatelessWidget') + (options.key ? ' with key' : '')];
        // A new file may not have evaluated references yet. Fully qualified core types remain safe in Restricted Mode.
        let body = instantiate(template, '', options.name, [], [], true).body;
        body = body.replace(/\$\{1:([^}]+)\}/g, options.name).replace(/\$\{1\}/g, options.name).replace(/\$\{0:([^}]+)\}/g, '$1').replace(/\$0/g, '');
        const nl = vscode.workspace.getConfiguration('files', options.uri).get<string>('eol') === '\r\n' ? '\r\n' : '\n';
        body = (options.namespace ? `namespace ${options.namespace};\n\n` : '') + body + '\n'; body = body.replace(/\r?\n/g, nl);
        const edit = new vscode.WorkspaceEdit(); edit.createFile(options.uri, { overwrite: false, ignoreIfExists: false }); edit.insert(options.uri, new vscode.Position(0, 0), body);
        if (!await vscode.workspace.applyEdit(edit)) return false;
        this.contexts.invalidate();
        await vscode.window.showTextDocument(await vscode.workspace.openTextDocument(options.uri));
        return true;
    }
}
