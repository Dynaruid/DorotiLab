import * as vscode from 'vscode';
import { ProjectContexts } from './projectContext';
import { AssistClient, Analysis } from './assistClient';
import { loadTemplates } from './catalog';
import { EditingCompletions } from './completions';
import { EditingActions } from './codeActions';
import { EditPreview } from './edits';
import { WidgetTemplates, NewWidgetOptions } from './newWidget';

export function registerEditing(context: vscode.ExtensionContext, logs: vscode.OutputChannel) {
    const assist = new AssistClient(context, logs);
    const contexts = new ProjectContexts(() => assist.invalidate());
    const templates = loadTemplates(context.extensionPath);
    const preview = new EditPreview();
    const actions = new EditingActions(contexts, assist, preview, logs);
    const snippets = new WidgetTemplates(templates, contexts, assist);
    const sources = vscode.workspace.createFileSystemWatcher('**/*.cs');
    const sourceChanged = (uri: vscode.Uri) => { if (!/(^|[\\/])(bin|obj|temp|node_modules|reference|\.doroti|\.git)([\\/]|$)/i.test(vscode.workspace.asRelativePath(uri))) assist.invalidate(); };
    const safe = (fn: (...args: any[]) => any) => async (...args: any[]) => { try { return await fn(...args); } catch (error) { logs.appendLine(`[editing] ${error}`); void vscode.window.showErrorMessage(String(error)); return false; } };
    const updateContext = async () => {
        const doc = vscode.window.activeTextEditor?.document;
        await vscode.commands.executeCommand('setContext', 'doroti.isWidgetDocument', !!doc && !!await contexts.get(doc));
    };
    context.subscriptions.push(assist, contexts, preview, sources, sources.onDidChange(sourceChanged), sources.onDidCreate(sourceChanged), sources.onDidDelete(sourceChanged),
        vscode.languages.registerCompletionItemProvider({ language: 'csharp', scheme: 'file' }, new EditingCompletions(contexts, assist, templates, logs), '(', ',', ':'),
        vscode.languages.registerCodeActionsProvider({ language: 'csharp', scheme: 'file' }, actions, { providedCodeActionKinds: [vscode.CodeActionKind.QuickFix, vscode.CodeActionKind.RefactorRewrite, vscode.CodeActionKind.RefactorExtract] }),
        vscode.commands.registerCommand('doroti.insertWidgetSnippet', safe((name?: string) => snippets.insert(name))),
        vscode.commands.registerCommand('doroti.newStatelessWidget', safe((options?: NewWidgetOptions) => snippets.create(false, options))),
        vscode.commands.registerCommand('doroti.newStatefulWidget', safe((options?: NewWidgetOptions) => snippets.create(true, options))),
        vscode.commands.registerCommand('doroti.wrapWidget', safe(() => actions.wrap())),
        vscode.commands.registerCommand('doroti.applyWidgetAction', safe((...args: Parameters<EditingActions['apply']>) => actions.apply(...args))),
        vscode.commands.registerCommand('doroti.applyWidgetImport', safe(async (uri, version, edits) => {
            const doc = await vscode.workspace.openTextDocument(uri);
            if (!vscode.workspace.isTrusted || !await contexts.get(doc) || !vscode.workspace.getConfiguration('doroti.editing', uri).get('codeActions', true)) return false;
            return preview.apply(doc, version, { edits, preview: false });
        })),
        vscode.window.onDidChangeActiveTextEditor(() => void updateContext()),
        vscode.workspace.onDidGrantWorkspaceTrust(() => { contexts.invalidate(); void updateContext(); }),
        vscode.workspace.onDidChangeTextDocument(e => { if (e.document.languageId === 'csharp' && e.contentChanges.length) assist.invalidateCatalog(); }),
        vscode.workspace.onDidCloseTextDocument(doc => { if (doc.languageId === 'csharp') assist.invalidateCatalog(); }),
        vscode.languages.registerHoverProvider({ language: 'csharp', scheme: 'file' }, {
            async provideHover(doc, position, token) {
                if (!vscode.workspace.isTrusted || !vscode.workspace.getConfiguration('doroti.editing', doc.uri).get('hints', true)) return;
                const app = await contexts.get(doc); if (!app) return;
                const word = doc.getText(doc.getWordRangeAtPosition(position));
                if (!['Expanded', 'Flexible', 'Positioned', 'Container', 'LayoutBuilder'].includes(word)) return;
                try {
                    const analysis = await assist.request<Analysis>('analyze', app, doc, doc.offsetAt(position), 0, token);
                    if (!analysis.semantic || !analysis.actions.length) return;
                    const hint = word === 'Container' ? 'Use color or decoration; do not specify both.' : word === 'LayoutBuilder' ? 'builder receives (BuildContext, BoxConstraints) and returns Widget.' : word === 'Positioned' ? 'Place directly in a Stack children list.' : 'Place directly in a Row, Column or Flex children list.';
                    return new vscode.Hover(new vscode.MarkdownString(`Doroti: ${hint}\n\n[Editing guide](https://github.com/Dynaruid/DorotiLab/tree/main/tools/vscode-doroti)`));
                } catch { return; }
            }
        }));
    void updateContext();
    return { contexts, assist, actions, preview };
}
