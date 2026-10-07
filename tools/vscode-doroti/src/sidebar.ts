import * as vscode from 'vscode';
import * as path from 'node:path';
import { Project, Runtime } from './contracts';

export interface SidebarState {
    project?: Project;
    target?: string;
    running: boolean;
    busy: boolean;
    stopping: boolean;
    pending?: string;
    runtime?: Runtime;
    restartRequired: boolean;
    problem?: string;
    message?: string;
}

/** Native workbench view. All actions use the existing command/session owners. */
export class DorotiSidebar implements vscode.TreeDataProvider<vscode.TreeItem>, vscode.Disposable {
    private readonly changes = new vscode.EventEmitter<vscode.TreeItem | undefined>();
    readonly onDidChangeTreeData = this.changes.event;
    private readonly subscriptions: vscode.Disposable[] = [];
    private widgetDocument = false;
    private editorGeneration = 0;
    private signature = '';
    private disposed = false;
    readonly view: vscode.TreeView<vscode.TreeItem>;

    constructor(private state: SidebarState, private readonly isWidgetDocument: (doc: vscode.TextDocument) => Promise<boolean>) {
        this.view = vscode.window.createTreeView('doroti.workspace', { treeDataProvider: this, showCollapseAll: true });
        const watcher = vscode.workspace.createFileSystemWatcher('**/{doroti-workspace.json,*.csproj,*.props,*.targets}');
        this.subscriptions.push(this.view, watcher,
            vscode.window.onDidChangeActiveTextEditor(() => void this.refreshEditor()),
            vscode.workspace.onDidGrantWorkspaceTrust(() => this.refresh()),
            vscode.workspace.onDidChangeWorkspaceFolders(() => this.refresh()),
            vscode.workspace.onDidChangeConfiguration(e => { if (e.affectsConfiguration('doroti')) this.refresh(); }),
            watcher.onDidChange(() => this.refresh()), watcher.onDidCreate(() => this.refresh()), watcher.onDidDelete(() => this.refresh()));
        this.refresh();
    }

    update(state: SidebarState) { this.state = state; this.render(); }
    refresh() { this.render(true); void this.refreshEditor(); }
    private async refreshEditor() {
        const generation = ++this.editorGeneration;
        const doc = vscode.window.activeTextEditor?.document;
        // Drop the old document's enabled actions while resolving a new editor.
        this.widgetDocument = false; this.render();
        const eligible = doc ? await this.isWidgetDocument(doc).catch(() => false) : false;
        if (this.disposed || generation !== this.editorGeneration) return;
        this.widgetDocument = eligible; this.render();
    }
    private render(force = false) {
        if (this.disposed) return;
        const signature = JSON.stringify([this.state, this.widgetDocument, vscode.workspace.isTrusted,
            vscode.workspace.workspaceFolders?.map(folder => folder.uri.toString()),
            vscode.workspace.getConfiguration('doroti.editing', vscode.window.activeTextEditor?.document.uri).get('snippets', true),
            vscode.workspace.getConfiguration('doroti.editing', vscode.window.activeTextEditor?.document.uri).get('codeActions', true)]);
        // Runtime polling is frequent; unchanged snapshots must not rebuild the tree.
        if (!force && signature === this.signature) return;
        this.signature = signature;
        this.view.description = this.state.project ? path.basename(this.state.project.root) : undefined;
        this.view.message = !vscode.workspace.isTrusted ? 'Restricted Mode: trust this workspace to run Doroti tools.'
            : (vscode.workspace.workspaceFolders?.length ?? 0) !== 1 ? 'Open one local workspace folder to select and run a project.'
            : !this.state.project ? 'Select a Doroti project, then choose its development target.' : undefined;
        this.changes.fire(undefined);
    }
    getTreeItem(item: vscode.TreeItem) { return item; }
    getChildren(parent?: vscode.TreeItem): vscode.TreeItem[] {
        const s = this.state;
        const trusted = vscode.workspace.isTrusted;
        const oneRoot = vscode.workspace.workspaceFolders?.length === 1 && vscode.workspace.workspaceFolders[0].uri.scheme === 'file';
        const idle = !s.running && !s.busy;
        const executionReason = !trusted ? 'Trust workspace first' : s.stopping ? 'Stopping' : s.busy ? 'Working' : undefined;
        const selectionReason = executionReason ?? (s.running ? 'Stop app first' : !oneRoot ? 'Open one local folder' : undefined);
        if (!parent) return [
            this.group('project', 'Project', 'project', s.project ? path.basename(s.project.root) : 'Not selected'),
            this.group('session', 'Development', 'debug-alt', this.sessionStatus()),
            this.group('widgets', 'Widgets', 'symbol-class'),
        ];
        if (parent.id === 'project') {
            const target = this.action('target', 'Select Target', 'device-desktop', 'doroti.selectTarget', selectionReason ?? (s.project && !s.project.developmentTargets.length ? 'No development targets on this host' : undefined), s.project ? s.target ?? 'Not selected' : 'Select project first');
            if (s.project?.developmentTargets.length) target.collapsibleState = vscode.TreeItemCollapsibleState.Collapsed;
            return [
                this.action('select-project', 'Select Project', 'folder-opened', 'doroti.selectProject', selectionReason, s.project ? path.basename(s.project.root) : 'Not selected', s.project?.root),
                target,
                ...(s.project ? [this.action('manifest', 'Open Workspace Manifest', 'json', 'vscode.open', undefined, undefined, s.project.manifest ?? path.join(s.project.root, 'doroti-workspace.json'), [vscode.Uri.file(s.project.manifest ?? path.join(s.project.root, 'doroti-workspace.json'))])] : []),
                this.action('create-project', 'Create Project…', 'new-folder', 'doroti.createProject', executionReason ?? (!idle ? 'Stop app first' : undefined)),
                ...(!trusted ? [this.action('trust', 'Manage Workspace Trust', 'shield', 'workbench.trust.manage')] : []),
            ];
        }
        if (parent.id === 'target') return (s.project?.developmentTargets ?? []).map(target =>
            this.action(`target:${target}`, target, target === s.target ? 'check' : 'circle-outline', 'doroti.selectTarget', selectionReason,
                target === s.target ? 'Selected' : undefined, `Use provider development target: ${target}`, [target]));
        if (parent.id === 'session') {
            const reloadReason = executionReason ?? (!s.running ? 'Run app first' : s.restartRequired ? 'Restart required'
                : !s.runtime?.supported ? 'Waiting for connected runtime' : s.pending ? 'Reloading' : undefined);
            const runtime = s.runtime;
            return [
                this.action('session-status', this.sessionStatus(), s.problem ? 'warning' : s.running ? 'pulse' : 'circle-outline', undefined, undefined,
                    runtime ? `Revision ${runtime.revision}` : undefined, runtime ? `Runtime: ${runtime.runtimeId}\nStatus: ${runtime.status}${runtime.error ? `\n${runtime.error}` : ''}` : undefined),
                this.action('run', 'Run', 'play', 'doroti.run', selectionReason ?? (s.project && !s.project.developmentTargets.length ? 'No development targets on this host' : undefined), 'Debug'),
                this.action('reload', 'Hot Reload', 'debug-restart', 'doroti.hotReload', reloadReason, 'Preserve state'),
                this.action('restart', 'Restart', 'refresh', 'doroti.restart', executionReason ?? (!s.running ? 'Run app first' : undefined), 'Resets state'),
                this.action('stop', 'Stop', 'debug-stop', 'doroti.stop', s.stopping ? 'Stopping' : !s.running && !s.busy ? 'No active session' : undefined),
                this.action('logs', 'Show Logs', 'output', 'doroti.showLogs'),
            ];
        }
        if (parent.id === 'widgets') {
            const reason = this.widgetDocument ? undefined : 'Open Doroti application C# source';
            const config = vscode.workspace.getConfiguration('doroti.editing', vscode.window.activeTextEditor?.document.uri);
            return [
                this.action('stateless', 'New Stateless Widget…', 'symbol-class', 'doroti.newStatelessWidget', reason),
                this.action('stateful', 'New Stateful Widget…', 'symbol-class', 'doroti.newStatefulWidget', reason),
                this.action('snippet', 'Insert Widget Snippet…', 'symbol-snippet', 'doroti.insertWidgetSnippet', reason ?? (!config.get('snippets', true) ? 'Enable doroti.editing.snippets' : undefined)),
                this.action('wrap', 'Wrap with Widget…', 'surround-with', 'doroti.wrapWidget', reason ?? (!trusted ? 'Trust workspace first' : !config.get('codeActions', true) ? 'Enable doroti.editing.codeActions' : undefined)),
            ];
        }
        return [];
    }
    private sessionStatus() {
        const s = this.state;
        if (s.stopping) return 'Stopping';
        if (s.restartRequired) return 'Restart required';
        if (s.problem === 'compile-error') return 'Compilation failed';
        if (s.pending) return 'Reloading';
        if (s.busy) return 'Working';
        if (!s.running) return 'Stopped';
        if (!s.runtime) return 'Waiting for runtime';
        if (s.runtime.status === 'failed') return 'Reload failed';
        return s.message ?? (s.runtime.supported ? 'Running' : 'Hot Reload unavailable');
    }
    private group(id: string, label: string, icon: string, description?: string) {
        const item = new vscode.TreeItem(label, vscode.TreeItemCollapsibleState.Expanded);
        item.id = id; item.iconPath = new vscode.ThemeIcon(icon); item.description = description;
        return item;
    }
    private action(id: string, label: string, icon: string, command?: string, disabledReason?: string, description?: string, tooltip?: string, args?: unknown[]) {
        const item = new vscode.TreeItem(label);
        item.id = id; item.description = disabledReason ?? description;
        item.tooltip = disabledReason ? `${label}: ${disabledReason}` : tooltip ?? label;
        item.iconPath = new vscode.ThemeIcon(icon, disabledReason ? new vscode.ThemeColor('disabledForeground') : undefined);
        if (command && !disabledReason) item.command = { command, title: label, arguments: args };
        return item;
    }
    dispose() { this.disposed = true; this.editorGeneration++; this.subscriptions.forEach(item => item.dispose()); this.changes.dispose(); }
}
