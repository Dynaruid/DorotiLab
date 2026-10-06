import * as vscode from 'vscode';
import * as path from 'node:path';
import * as fs from 'node:fs/promises';

const exclude = '**/{bin,obj,temp,node_modules,reference,.doroti,.git}/**';
export interface AppContext { root: string; project: string; projects: string[]; packages: string[]; globals: string[]; linkedFiles: string[]; }
export function within(root: string, file: string): boolean {
    const relative = path.relative(root, file);
    return relative !== '..' && !relative.startsWith(`..${path.sep}`) && !path.isAbsolute(relative);
}
const attributes = (text: string, tag: string) => [...text.matchAll(new RegExp(`<${tag}\\b[^>]*Include\\s*=\\s*["']([^"']+)["']`, 'g'))].map(m => m[1]);

export class ProjectContexts implements vscode.Disposable {
    private inventory?: Promise<AppContext[]>;
    private globals = new Map<string, Promise<string[]>>();
    private owners = new Map<string, Promise<AppContext | undefined>>();
    private watchers: vscode.Disposable[] = [];
    constructor(private readonly changed: () => void) {
        const watcher = vscode.workspace.createFileSystemWatcher('**/{doroti-workspace.json,*.csproj,*.props,*.targets,project.assets.json,global.json}');
        this.watchers.push(watcher, watcher.onDidChange(() => this.invalidate()), watcher.onDidCreate(() => this.invalidate()), watcher.onDidDelete(() => this.invalidate()),
            vscode.workspace.onDidChangeWorkspaceFolders(() => this.invalidate()),
            vscode.workspace.onDidChangeTextDocument(e => { if (e.document.languageId === 'csharp') this.globals.clear(); }),
            vscode.workspace.onDidCloseTextDocument(doc => { this.globals.clear(); this.owners.delete(doc.uri.toString()); }),
            vscode.workspace.onDidCreateFiles(() => this.invalidate()), vscode.workspace.onDidDeleteFiles(() => this.invalidate()));
    }
    invalidate() { this.inventory = undefined; this.globals.clear(); this.owners.clear(); this.changed(); }
    dispose() { this.watchers.forEach(w => w.dispose()); }
    async get(doc: vscode.TextDocument): Promise<AppContext | undefined> {
        if (doc.uri.scheme !== 'file' || doc.languageId !== 'csharp') return;
        const key = doc.uri.toString();
        let owner = this.owners.get(key);
        if (!owner) { owner = this.findOwner(doc); if (this.owners.size >= 256) this.owners.delete(this.owners.keys().next().value!); this.owners.set(key, owner); }
        return owner;
    }
    private async findOwner(doc: vscode.TextDocument): Promise<AppContext | undefined> {
        this.inventory ??= this.load();
        const apps = await this.inventory;
        // A sibling runner/test C# project inside a manifest root is not application source.
        const owners = apps.filter(app => app.linkedFiles.includes(doc.uri.fsPath) || app.projects.some(p => within(path.dirname(p), doc.uri.fsPath)));
        for (const app of owners.sort((a, b) => b.root.length - a.root.length)) {
            if (app.linkedFiles.includes(doc.uri.fsPath)) return app;
            let dir = path.dirname(doc.uri.fsPath);
            while (within(app.root, dir)) {
                const entries = await fs.readdir(dir).catch(() => []);
                const projects = entries.filter(e => e.endsWith('.csproj')).map(e => path.join(dir, e));
                if (projects.length) {
                    if (projects.some(p => app.projects.includes(p))) return app;
                    break;
                }
                const parent = path.dirname(dir); if (parent === dir) break; dir = parent;
            }
            // Linked Compile items can be outside the app root; Roslyn will verify membership.
            if (!within(app.root, doc.uri.fsPath) && app.projects.some(p => within(path.dirname(p), doc.uri.fsPath))) return app;
        }
        return undefined;
    }
    private async load(): Promise<AppContext[]> {
        const result: AppContext[] = [];
        for (const uri of await vscode.workspace.findFiles('**/doroti-workspace.json', exclude, 256)) {
            try {
                const manifest = JSON.parse(await fs.readFile(uri.fsPath, 'utf8'));
                if (manifest.schemaVersion !== 'doroti.workspace/v2' || typeof manifest.applicationProject !== 'string') continue;
                const root = path.dirname(uri.fsPath);
                const project = path.resolve(root, manifest.applicationProject);
                if (!within(root, project) || !project.endsWith('.csproj')) continue;
                const projects: string[] = []; const packages = new Set<string>(); const linkedFiles: string[] = [];
                async function visit(file: string) {
                    if (projects.includes(file) || projects.length >= 128) return;
                    const text = await fs.readFile(file, 'utf8'); projects.push(file);
                    for (const p of attributes(text, 'PackageReference')) packages.add(p);
                    for (const p of attributes(text, 'Reference')) packages.add(p);
                    for (const p of attributes(text, 'Compile')) {
                        if (!/[$*?;]/.test(p) && p.endsWith('.cs')) linkedFiles.push(path.resolve(path.dirname(file), p.replace(/\\/g, path.sep)));
                    }
                    for (const p of attributes(text, 'ProjectReference')) {
                        if (p.includes('$(')) continue;
                        const reference = path.resolve(path.dirname(file), p.replace(/\\/g, path.sep));
                        packages.add(path.basename(reference, '.csproj'));
                        await visit(reference).catch(() => {});
                    }
                }
                await visit(project);
                // Doroti.App.Sdk supplies the core framework even if references are imported.
                packages.add('Doroti.Framework.Widgets');
                result.push({ root, project, projects, packages: [...packages], globals: [], linkedFiles });
            } catch { /* A malformed manifest is not an eligible project. */ }
        }
        return result;
    }
    async globalImports(app: AppContext): Promise<string[]> {
        let cached = this.globals.get(app.project);
        if (!cached) {
            cached = (async () => {
                const imports = new Set<string>();
                const uris = await vscode.workspace.findFiles(new vscode.RelativePattern(path.dirname(app.project), '**/*.cs'), exclude, 2000);
                for (const uri of uris) {
                    const live = vscode.workspace.textDocuments.find(d => d.uri.toString() === uri.toString());
                    const text = live?.getText() ?? await fs.readFile(uri.fsPath, 'utf8');
                    for (const m of text.matchAll(/^\s*global\s+using\s+(?:global::)?([\w.]+(?:\s*=\s*(?:global::)?[\w.]+)?)\s*;/gm)) imports.add(m[1].replace(/\s/g, ''));
                }
                for (const file of app.projects) {
                    const text = await fs.readFile(file, 'utf8').catch(() => '');
                    for (const value of attributes(text, 'Using')) imports.add(value);
                }
                return [...imports];
            })();
            this.globals.set(app.project, cached);
        }
        return cached;
    }
}
