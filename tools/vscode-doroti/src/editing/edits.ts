import * as vscode from 'vscode';
import { randomUUID } from 'node:crypto';
import { Transformation, Change } from './assistClient';

export function editedText(source: string, changes: Change[]): string {
    let end = source.length;
    for (const edit of [...changes].sort((a, b) => b.start - a.start)) {
        if (edit.start < 0 || edit.start + edit.length > end) throw new Error('Overlapping or out of bounds Doroti edit');
        source = source.slice(0, edit.start) + edit.text + source.slice(edit.start + edit.length); end = edit.start;
    }
    return source;
}
export class EditPreview implements vscode.Disposable, vscode.TextDocumentContentProvider {
    private documents = new Map<string, string>();
    private provider = vscode.workspace.registerTextDocumentContentProvider('doroti-edit', this);
    provideTextDocumentContent(uri: vscode.Uri) { return this.documents.get(uri.toString()) ?? ''; }
    dispose() { this.documents.clear(); this.provider.dispose(); }
    async apply(doc: vscode.TextDocument, version: number, transform: Transformation, preview = transform.preview): Promise<boolean> {
        if (transform.reason) { void vscode.window.showInformationMessage(transform.reason); return false; }
        if (doc.version !== version || doc.isClosed) throw new Error('Document changed; request the Doroti action again.');
        const source = doc.getText();
        const proposed = editedText(source, transform.edits);
        if (!transform.edits.length) return false;
        if (preview) {
            const id = randomUUID();
            const before = vscode.Uri.parse(`doroti-edit:/${id}/before.cs`); const after = vscode.Uri.parse(`doroti-edit:/${id}/after.cs`);
            this.documents.set(before.toString(), source); this.documents.set(after.toString(), proposed);
            try {
                await vscode.commands.executeCommand('vscode.diff', before, after, 'Doroti refactoring preview');
                if (await vscode.window.showQuickPick(['Apply changes', 'Cancel'], { title: 'Apply the previewed Doroti refactoring?' }) !== 'Apply changes') return false;
            } finally { this.documents.delete(before.toString()); this.documents.delete(after.toString()); }
        }
        if (doc.version !== version || doc.isClosed) throw new Error('Document changed during preview; request the action again.');
        const editor = await vscode.window.showTextDocument(doc);
        // One editor transaction gives replacements + imports one Undo unit and leaves the buffer unsaved.
        return editor.edit(builder => {
            for (const change of transform.edits) builder.replace(new vscode.Range(doc.positionAt(change.start), doc.positionAt(change.start + change.length)), change.text);
        }, { undoStopBefore: true, undoStopAfter: true });
    }
}
