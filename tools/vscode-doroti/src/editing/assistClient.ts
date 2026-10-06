import * as vscode from 'vscode';
import * as path from 'node:path';
import { spawn, ChildProcessWithoutNullStreams } from 'node:child_process';
import { randomUUID } from 'node:crypto';
import { stop } from '../processes';
import { AppContext, within } from './projectContext';

export interface Parameter { name: string; type: string; optional: boolean; default?: string; delegateParameters?: string[]; delegateReturn?: string; }
export interface CatalogType { name: string; namespace: string; fullName: string; kind: string; constructors: Parameter[][]; values: string[]; factories: string[]; documentation: string; }
export interface AssistAction { id: string; title: string; preview: boolean; reason?: string; }
export interface Analysis { semantic: boolean; reason?: string; included: boolean; code: boolean; context: string; parent?: string; stateType?: string; existingMembers: string[]; arguments: Parameter[]; actions: AssistAction[]; globalImports: string[]; conflicts: string[]; }
export interface Change { start: number; length: number; text: string; }
export interface Transformation { edits: Change[]; preview: boolean; reason?: string; }
const schema = 'doroti.editor/v1';
const limit = 8 * 1024 * 1024;

export class AssistClient implements vscode.Disposable {
    private child?: ChildProcessWithoutNullStreams;
    private starting?: Promise<void>;
    private pending = new Map<string, { resolve: (value: any) => void; reject: (error: Error) => void }>();
    private buffer = '';
    private disposed = false;
    private generation = 0;
    private snapshotGeneration = 0;
    private closing?: Promise<void>;
    private catalogs = new Map<string, Promise<CatalogType[]>>();
    private analyses = new Map<string, unknown>();
    constructor(private readonly context: vscode.ExtensionContext, private readonly logs: vscode.OutputChannel) {}
    invalidateCatalog() { this.snapshotGeneration++; this.catalogs.clear(); this.analyses.clear(); }
    invalidate() { this.generation++; this.catalogs.clear(); this.analyses.clear(); if (this.child) this.send({ id: randomUUID(), operation: 'invalidate' }); }
    dispose() { void this.shutdown().catch(error => this.logs.appendLine(`[editing shutdown] ${error}`)); }
    async shutdown() {
        this.disposed = true; this.fail(new Error('Doroti editing helper disposed'));
        const child = this.child;
        this.closing ??= child ? stop(child) : Promise.resolve();
        await this.closing;
    }
    private fail(error: Error) { for (const entry of this.pending.values()) entry.reject(error); this.pending.clear(); this.catalogs.clear(); this.analyses.clear(); }
    private send(value: unknown) {
        const line = JSON.stringify(value);
        if (line.length > limit) throw new Error('Doroti editing request exceeds 8 MiB');
        if (!this.child?.stdin.writable) throw new Error('Doroti editing helper disconnected');
        this.child.stdin.write(line + '\n');
    }
    private async start(app: AppContext) {
        if (this.child) return;
        if (!vscode.workspace.isTrusted || this.disposed) throw new Error('Project analysis requires workspace trust');
        this.starting ??= (async () => {
            const command = vscode.workspace.getConfiguration('doroti').get<string>('dotnetPath', 'dotnet');
            const child = spawn(command, [path.join(this.context.extensionPath, 'helper', 'Doroti.Editor.Assist.dll')], { cwd: path.dirname(app.project), windowsHide: true, detached: process.platform !== 'win32', stdio: 'pipe' });
            this.child = child; this.buffer = '';
            child.stderr.on('data', text => this.logs.append(`[editing] ${text}`));
            child.stdout.setEncoding('utf8'); child.stdout.on('data', (text: string) => {
                this.buffer += text;
                if (this.buffer.length > limit * 2) { void stop(child).catch(e => this.logs.appendLine(String(e))); this.fail(new Error('Oversized editing response')); return; }
                let newline: number;
                while ((newline = this.buffer.indexOf('\n')) >= 0) {
                    const line = this.buffer.slice(0, newline); this.buffer = this.buffer.slice(newline + 1);
                    try {
                        if (line.length > limit) throw new Error('Editing response exceeds 8 MiB');
                        const value = JSON.parse(line);
                        if (value.schemaVersion !== schema) throw new Error('Editing helper schema mismatch');
                        const request = this.pending.get(value.id);
                        this.pending.delete(value.id);
                        if (value.error) request?.reject(new Error(value.error)); else request?.resolve(value.result);
                    } catch (error) { this.logs.appendLine(`[editing] ${error}`); void stop(child).catch(e => this.logs.appendLine(String(e))); this.fail(new Error('Invalid editing protocol')); }
                }
            });
            const ended = (error: Error) => { if (this.child === child) { this.child = undefined; this.starting = undefined; this.fail(error); } };
            child.once('error', ended); child.once('exit', code => ended(new Error(`Editing helper exited (${code}); install .NET 10 SDK or check Doroti logs`)));
            await this.call({ operation: 'hello' });
        })();
        try { await this.starting; } catch (error) { const child = this.child as ChildProcessWithoutNullStreams | undefined; if (child) await stop(child); this.child = undefined; this.starting = undefined; throw error; }
    }
    private async call<T>(value: Record<string, unknown>, token?: vscode.CancellationToken): Promise<T> {
        if (token?.isCancellationRequested) throw new vscode.CancellationError();
        if (this.pending.size >= 64) throw new Error('Editing request queue is full');
        const id = randomUUID();
        let cancel: vscode.Disposable | undefined;
        let timer: NodeJS.Timeout | undefined;
        try {
            return await new Promise<T>((resolve, reject) => {
                this.pending.set(id, { resolve, reject });
                timer = setTimeout(() => { this.pending.delete(id); if (this.child) this.send({ id, operation: 'cancel' }); reject(new Error('Editing analysis timed out; check restored references and Doroti logs')); }, 120000);
                cancel = token?.onCancellationRequested(() => { this.pending.delete(id); if (this.child) this.send({ id, operation: 'cancel' }); reject(new vscode.CancellationError()); });
                try { this.send({ id, ...value }); } catch (error) { this.pending.delete(id); reject(error); }
            });
        } finally { if (timer) clearTimeout(timer); cancel?.dispose(); }
    }
    async request<T>(operation: string, app: AppContext, doc: vscode.TextDocument, offset: number, length: number, token?: vscode.CancellationToken, action?: string, name?: string, attempt = 0): Promise<T> {
        if (token?.isCancellationRequested) throw new vscode.CancellationError();
        const key = `${app.project}|${doc.uri}|${doc.version}|${offset}|${length}`;
        if (operation === 'analyze' && this.analyses.has(key)) return this.analyses.get(key) as T;
        await this.start(app);
        const generation = this.generation; const snapshotGeneration = this.snapshotGeneration; const version = doc.version;
        const live = vscode.workspace.textDocuments.filter(d => d !== doc && d.isDirty && d.uri.scheme === 'file' && d.languageId === 'csharp'
            && (app.linkedFiles.includes(d.uri.fsPath) || app.projects.some(p => within(path.dirname(p), d.uri.fsPath))));
        if (live.length > 31) throw new Error('Close/save some source buffers: Doroti analysis supports 32 simultaneous snapshots.');
        const snapshots = live.map(d => ({ uri: d.uri.toString(), version: d.version, text: d.getText() }));
        snapshots.push({ uri: doc.uri.toString(), version, text: doc.getText() });
        const result = await this.call<T>({ operation, project: app.project, uri: doc.uri.toString(), version, offset, length, snapshots, action, name }, token);
        if (doc.version === version && (generation !== this.generation || snapshotGeneration !== this.snapshotGeneration) && !token?.isCancellationRequested && attempt === 0)
            return this.request(operation, app, doc, offset, length, token, action, name, 1);
        if (doc.version !== version || generation !== this.generation || snapshotGeneration !== this.snapshotGeneration || token?.isCancellationRequested) throw new vscode.CancellationError();
        if (operation === 'analyze') {
            if (this.analyses.size >= 64) this.analyses.delete(this.analyses.keys().next().value!);
            this.analyses.set(key, result);
        }
        return result;
    }
    async catalog(app: AppContext, doc: vscode.TextDocument, token?: vscode.CancellationToken): Promise<CatalogType[]> {
        let value = this.catalogs.get(app.project);
        if (!value) {
            value = this.request<{ types: CatalogType[] }>('catalog', app, doc, 0, 0, token).then(r => r.types ?? []);
            this.catalogs.set(app.project, value);
            value.catch(() => { if (this.catalogs.get(app.project) === value) this.catalogs.delete(app.project); });
        }
        return value;
    }
}
