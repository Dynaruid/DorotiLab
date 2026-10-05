import * as vscode from 'vscode';
import * as path from 'node:path';
import * as fs from 'node:fs/promises';
import { randomUUID } from 'node:crypto';
import { ChildProcess } from 'node:child_process';
import { start, stop, completion } from './processes';
import { Project, Runtime, parseProject, validateName, classifyOutput } from './contracts';
import { namespaces, importPlan } from './imports';
import { WebBridge } from './webBridge';

const exclude = '**/{bin,obj,temp,node_modules,reference,.doroti,.git}/**';
interface Session { id: string; directory: string; child: ChildProcess; runtime?: Runtime; pending?: string; timer?: NodeJS.Timeout; deadline?: number; restartRequired?: boolean; stopping?: boolean; problem?: string; bridge?: WebBridge; }
let shutdown: (() => Promise<void>) | undefined;

export function activate(context: vscode.ExtensionContext) {
    const logs = vscode.window.createOutputChannel('Doroti');
    const status = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Left, 30);
    const reload = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Left, 29);
    let project: Project | undefined;
    let target = context.workspaceState.get<string>('target');
    let session: Session | undefined;
    let busy = false;
    let operation: ChildProcess | undefined;
    let lifetime = 0;
    const config = () => vscode.workspace.getConfiguration('doroti');
    const development = () => project?.developmentSupport.find(value => value.platform === target)?.development;

    const trusted = () => { if (!vscode.workspace.isTrusted) throw new Error('Trust this workspace before running Doroti tools.'); };
    const singleRoot = () => { if ((vscode.workspace.workspaceFolders?.length ?? 0) !== 1) throw new Error('Open one local workspace folder. Multi-root is not supported.'); };
    const showError = (e: unknown) => { logs.appendLine(String(e)); logs.show(true); void vscode.window.showErrorMessage(String(e)); };
    function display(message?: string) {
        status.text = `Doroti: ${project ? path.basename(project.root) : 'Select project'}${target ? ` / ${target}` : ''}${message ? ` · ${message}` : ''}`;
        status.command = 'doroti.selectProject'; status.show();
        const supported = !!session?.runtime?.supported && !session.restartRequired;
        reload.text = session?.pending ? '$(sync~spin) Reloading' : '$(debug-restart) Hot Reload';
        reload.tooltip = supported ? 'Save pending C# edits and apply metadata updates' : 'Run a connected provider Debug session.';
        reload.command = supported && !session?.pending ? 'doroti.hotReload' : 'doroti.showLogs';
        if (session) reload.show(); else reload.hide();
        void vscode.commands.executeCommand('setContext', 'doroti.running', !!session || busy);
        void vscode.commands.executeCommand('setContext', 'doroti.reloadSupported', supported);
        void vscode.commands.executeCommand('setContext', 'doroti.reloading', !!session?.pending);
    }
    async function cli(): Promise<string> {
        const configured = config().get<string>('cliPath');
        if (configured) { if (!path.isAbsolute(configured)) throw new Error('doroti.cliPath must be absolute.'); await fs.access(configured); return configured; }
        let directory = vscode.workspace.workspaceFolders?.[0]?.uri.fsPath;
        while (directory) {
            const candidate = path.join(directory, 'Doroti', 'eng', 'doroti.ps1');
            try { await fs.access(candidate); return candidate; } catch { /* search ancestors */ }
            const parent = path.dirname(directory); if (parent === directory) break; directory = parent;
        }
        throw new Error('Set doroti.cliPath to the repository Doroti/eng/doroti.ps1. Install PowerShell 7 and .NET 10 SDK.');
    }
    async function selectProject() {
        trusted(); singleRoot(); if (session) throw new Error('Stop the running app before changing project.');
        const candidates = await vscode.workspace.findFiles('**/doroti-workspace.json', exclude, 100);
        if (!candidates.length) throw new Error('No doroti-workspace.json found. Use Doroti: Create Project.');
        const previous = context.workspaceState.get<string>('manifest');
        const choices = candidates.map(uri => ({ label: vscode.workspace.asRelativePath(uri), uri, picked: uri.fsPath === previous }));
        const choice = choices.length === 1 ? choices[0] : await vscode.window.showQuickPick(choices, { title: 'Select Doroti project' });
        if (!choice) return;
        let output = '';
        operation = start(config().get('powerShellPath', 'pwsh'), ['-NoProfile', '-File', await cli(), 'describe', '-App', path.dirname(choice.uri.fsPath)], path.dirname(choice.uri.fsPath), text => { output += text; logs.append(text); });
        try { await completion(operation); } finally { operation = undefined; }
        project = parseProject(output);
        await context.workspaceState.update('manifest', choice.uri.fsPath);
        if (!project.developmentTargets.includes(target ?? '')) target = undefined;
        display();
    }
    async function selectTarget(requested?: unknown) {
        trusted(); if (session) throw new Error('Stop the running app before changing target.');
        if (!project) await selectProject(); if (!project) return;
        if (!project.developmentTargets.length) throw new Error('This manifest declares no provider development operations on this host.');
        const choice = typeof requested === 'string' ? requested : await vscode.window.showQuickPick(project.developmentTargets, { title: 'Select Doroti target' });
        if (choice && !project.developmentTargets.includes(choice)) throw new Error('Target is not declared by this workspace.');
        if (choice) { target = choice; await context.workspaceState.update('target', target); display(); }
    }
    async function createProject() {
        trusted(); if (session) throw new Error('Stop the running app before creating a project.');
        const name = await vscode.window.showInputBox({ title: 'Doroti project name', value: 'MyDorotiApp', validateInput: validateName });
        if (!name) return;
        const parent = await vscode.window.showOpenDialog({ title: 'Parent folder for the new project', canSelectFolders: true, canSelectFiles: false, canSelectMany: false });
        if (!parent?.length) return;
        const directory = path.join(parent[0].fsPath, name);
        try { await fs.access(directory); throw new Error(`Folder already exists: ${directory}. Choose a new name; existing files are never overwritten.`); }
        catch (error) { if ((error as NodeJS.ErrnoException).code !== 'ENOENT') throw error; }
        let canceled = false;
        try {
            await vscode.window.withProgress({ location: vscode.ProgressLocation.Notification, title: 'Creating Doroti project', cancellable: true }, async (_, token) => {
                operation = start(config().get('dotnetPath', 'dotnet'), ['new', 'doroti-app', '--name', name, '--output', directory], parent[0].fsPath, text => logs.append(text));
                const child = operation;
                const cancel = token.onCancellationRequested(() => { canceled = true; void stop(child).catch(showError); });
                if (token.isCancellationRequested) { canceled = true; void stop(child).catch(showError); }
                try { await completion(child); } finally { cancel.dispose(); operation = undefined; }
            });
        } catch (error) {
            throw new Error(`${canceled ? 'Creation canceled' : 'Creation failed'}. Inspect any partial files at ${directory}. Install .NET 10 SDK and the template with: dotnet new install <repository>/Doroti/templates/Doroti.Templates/content/doroti-app.\n${error}`);
        }
        if (!canceled) await vscode.commands.executeCommand('vscode.openFolder', vscode.Uri.file(directory));
    }
    async function endSession() {
        lifetime++;
        const current = session;
        if (current) {
            current.stopping = true; clearInterval(current.timer); await current.bridge?.close();
            if (development()?.usesStopSignal && current.child.exitCode === null && current.child.signalCode === null) {
                await fs.writeFile(path.join(current.directory, 'stop.json'), JSON.stringify({ sessionId: current.id }));
                const deadline = Date.now() + 30000;
                while (current.child.exitCode === null && current.child.signalCode === null && Date.now() < deadline)
                    await new Promise(resolve => setTimeout(resolve, 100));
            }
            await stop(current.child); if (session === current) session = undefined;
        }
        if (operation) { await stop(operation); operation = undefined; }
        display('Stopped');
    }
    async function poll(current: Session) {
        if (session !== current) return;
        try {
            const runtime = current.bridge ? current.bridge.runtime : JSON.parse(await fs.readFile(path.join(current.directory, 'runtime.json'), 'utf8')) as Runtime;
            if (!runtime) {
                current.runtime = undefined;
                if (current.pending) { current.pending = undefined; logs.appendLine('Hot Reload acknowledgment lost: browser disconnected.'); }
                display('Waiting for browser'); return;
            }
            if (session !== current || runtime.schemaVersion !== 'doroti.dev/v1' || runtime.sessionId !== current.id) return;
            if (current.runtime && current.runtime.runtimeId !== runtime.runtimeId) {
                current.pending = undefined; current.restartRequired = false; current.problem = undefined;
                logs.appendLine('Runtime restarted; widget state was reset.');
            }
            current.runtime = runtime;
            if (current.pending && runtime.requestId === current.pending && ['applied', 'failed'].includes(runtime.status)) {
                current.pending = undefined;
                logs.appendLine(`Hot Reload ${runtime.status}: revision ${runtime.revision}${runtime.error ? `\n${runtime.error}` : ''}`);
                display(runtime.status === 'applied' ? `Reload ${runtime.revision}` : 'Reload failed');
            } else if (current.pending && Date.now() > current.deadline!) {
                current.pending = undefined; logs.appendLine('No metadata update acknowledgment. Check compiler output; unchanged code does not produce a delta.'); display('Reload not applied');
            } else display(current.problem ?? runtime.status);
        } catch (error) { if ((error as NodeJS.ErrnoException).code !== 'ENOENT' && !(error instanceof SyntaxError)) logs.appendLine(String(error)); }
    }
    async function run() {
        trusted(); singleRoot(); if (session) throw new Error('A Doroti session is already running.');
        const generation = lifetime;
        if (!project) await selectProject(); if (!project) return;
        if (!target) await selectTarget(); if (!target) return;
        const script = await cli();
        const id = randomUUID(); const directory = path.join(project.root, '.doroti', 'dev', id);
        await fs.mkdir(directory, { recursive: true });
        if (generation !== lifetime) return;
        const bridge = development()?.transport === 'browser' ? await WebBridge.start(id) : undefined;
        if (generation !== lifetime) { await bridge?.close(); return; }
        let opened = false; let tail = '';
        const args = ['-NoProfile', '-File', script, 'dev', '-App', project.root, '-Platform', target, '-Configuration', 'Debug', '-SessionDirectory', directory, '-SessionId', id];
        args.push('-DotnetPath', config().get<string>('dotnetPath', 'dotnet'));
        const device = config().get<string>('device');
        if (device) args.push('-Device', device);
        const child = start(config().get('powerShellPath', 'pwsh'), args, project.root, text => {
            logs.append(text); tail = (tail + text).slice(-8192);
            if (development()?.launchBrowser && !opened) {
                const url = /Now listening on:\s*(https?:\/\/[^\s]+)[\r\n]/.exec(tail)?.[1];
                if (url) { opened = true; void vscode.env.openExternal(vscode.Uri.parse(bridge ? bridge.browserUrl(url) : url)); }
            }
            const problem = classifyOutput(tail);
            if (session && session.id === id && problem) {
                session.pending = undefined; session.restartRequired = problem === 'restart-required';
                session.problem = problem;
                // dotnet watch does not read redirected stdin for console prompts.
                // Keep the old process/state alive until the explicit Restart command.
                if (session.restartRequired) logs.appendLine('Use Doroti: Restart (resets state) to apply this unsupported edit.');
                tail = ''; display(problem); logs.show(true);
            }
        });
        const current: Session = { id, directory, child, bridge }; session = current;
        current.timer = setInterval(() => void poll(current), 300);
        void completion(child).catch(error => { if (session === current && !current.stopping) showError(error); }).finally(() => {
            clearInterval(current.timer); void current.bridge?.close(); if (session === current) { session = undefined; display('Exited'); }
        });
        logs.show(true); display('Starting Debug');
    }
    async function hotReload(save?: unknown) {
        trusted(); const current = session;
        if (!current?.runtime?.supported || current.restartRequired) throw new Error('Hot Reload unavailable. Run a provider Debug session with a connected runtime, or use Restart (resets state).');
        if (current.pending) return;
        const dirty = vscode.workspace.textDocuments.filter(doc => doc.isDirty && doc.languageId === 'csharp' && project && within(project.root, doc.uri.fsPath));
        if (!dirty.length) { void vscode.window.showInformationMessage('No unsaved C# edits. Saved changes are applied by dotnet watch automatically.'); return; }
        if (save !== true && await vscode.window.showQuickPick(['Save and Hot Reload', 'Cancel'], { title: `Save ${dirty.length} modified C# file(s)?` }) !== 'Save and Hot Reload') return;
        if (session !== current || current.pending) return;
        current.pending = randomUUID(); current.deadline = Date.now() + 45000; display();
        current.problem = undefined;
        try {
            if (current.bridge) await current.bridge.prepare(current.runtime.runtimeId, current.pending);
            else {
                await fs.writeFile(path.join(current.directory, 'request.json'), JSON.stringify({ schemaVersion: 'doroti.dev/v1', sessionId: current.id, runtimeId: current.runtime.runtimeId, requestId: current.pending }));
                if (development()?.requiresPreparedAcknowledgment) {
                    const deadline = Date.now() + 15000;
                    while (true) {
                        if (session !== current || current.stopping) return;
                        let prepared: Runtime | undefined;
                        try { prepared = JSON.parse(await fs.readFile(path.join(current.directory, 'prepared.json'), 'utf8')); }
                        catch (error) { if ((error as NodeJS.ErrnoException).code !== 'ENOENT' && !(error instanceof SyntaxError)) throw error; }
                        if (prepared?.sessionId === current.id && prepared.runtimeId === current.runtime.runtimeId && prepared.requestId === current.pending) break;
                        if (Date.now() > deadline) throw new Error(`${target} runtime did not accept the reload request. Edits have not been saved.`);
                        await new Promise(resolve => setTimeout(resolve, 50));
                    }
                }
            }
            if (session !== current || current.stopping) return;
            for (const doc of dirty) if (!await doc.save()) throw new Error(`Could not save ${doc.fileName}`);
        } catch (error) { current.pending = undefined; display('Reload failed'); throw error; }
    }
    const commands: Record<string, (arg?: unknown) => unknown> = {
        createProject, selectProject, selectTarget, run, hotReload, stop: endSession,
        restart: async (confirmed?: unknown) => { trusted(); if (confirmed === true || await vscode.window.showWarningMessage('Restart resets widget state, input and scroll position.', { modal: true }, 'Restart') === 'Restart') { await endSession(); await run(); } },
        showLogs: () => logs.show(),
    };
    for (const [name, action] of Object.entries(commands)) context.subscriptions.push(vscode.commands.registerCommand(`doroti.${name}`, async (arg?: unknown) => {
        if (busy && !['stop', 'showLogs'].includes(name)) return;
        if (name !== 'stop' && name !== 'showLogs') { busy = true; display(); }
        try { return await action(arg); } catch (error) { showError(error); }
        finally { if (name !== 'stop' && name !== 'showLogs') busy = false; display(); }
    }));
    async function globals(doc: vscode.TextDocument): Promise<string[]> {
        const root = project?.root ?? vscode.workspace.getWorkspaceFolder(doc.uri)?.uri.fsPath;
        if (!root) return [];
        const result: string[] = [];
        for (const uri of await vscode.workspace.findFiles(new vscode.RelativePattern(root, '{Program.cs,src/**/*.cs}'), exclude, 200)) {
            const text = await fs.readFile(uri.fsPath, 'utf8');
            for (const match of text.replace(/\/\*[\s\S]*?\*\/|\/\/[^\r\n]*/g, '').matchAll(/^\s*global\s+using\s+(?:global::)?([\w.]+(?:\s*=\s*(?:global::)?[\w.]+)?)\s*;/gm)) result.push(match[1].replace(/\s/g, ''));
        }
        return result;
    }
    const eligible = async (doc: vscode.TextDocument) => doc.uri.scheme === 'file' && (project ? within(project.root, doc.uri.fsPath) : (await vscode.workspace.findFiles('**/doroti-workspace.json', exclude, 1)).length > 0);
    context.subscriptions.push(vscode.languages.registerCompletionItemProvider('csharp', {
        async provideCompletionItems(doc, position) {
            if (!await eligible(doc)) return [];
            const globalImports = await globals(doc);
            return Object.entries(namespaces).flatMap(([name, spaces]) => spaces.map(ns => {
                const item = new vscode.CompletionItem({ label: name, description: ns }, vscode.CompletionItemKind.Class);
                const plan = importPlan(doc.getText(), name, ns, globalImports);
                item.detail = `Doroti import: ${ns}`; item.insertText = plan.qualified ? `global::${ns}.${name}` : name;
                item.range = doc.getWordRangeAtPosition(position);
                if (plan.text) item.additionalTextEdits = [vscode.TextEdit.insert(doc.positionAt(plan.offset), plan.text)];
                return item;
            }));
        },
    }), vscode.languages.registerCodeActionsProvider('csharp', {
        async provideCodeActions(doc, range, actionContext) {
            if (!await eligible(doc)) return [];
            const fixes: vscode.CodeAction[] = []; const globalImports = await globals(doc);
            for (const diagnostic of actionContext.diagnostics) {
                const code = typeof diagnostic.code === 'object' ? diagnostic.code.value : diagnostic.code;
                if (String(code) !== 'CS0246' && String(code) !== 'CS0103') continue;
                const name = doc.getText(diagnostic.range);
                for (const ns of namespaces[name] ?? []) {
                    const plan = importPlan(doc.getText(), name, ns, globalImports);
                    if (!plan.text && !plan.qualified) continue;
                    const fix = new vscode.CodeAction(`Import ${ns}.${name}`, vscode.CodeActionKind.QuickFix);
                    fix.diagnostics = [diagnostic]; fix.edit = new vscode.WorkspaceEdit();
                    if (plan.qualified) fix.edit.replace(doc.uri, diagnostic.range, `global::${ns}.${name}`);
                    else fix.edit.insert(doc.uri, doc.positionAt(plan.offset), plan.text!);
                    fixes.push(fix);
                }
            }
            return fixes;
        },
    }, { providedCodeActionKinds: [vscode.CodeActionKind.QuickFix] }));
    context.subscriptions.push(logs, status, reload, vscode.workspace.onDidGrantWorkspaceTrust(() => display()));
    shutdown = endSession; display();
    return { getState: () => ({ project, target, running: !!session, runtime: session?.runtime, pending: session?.pending, problem: session?.problem }) };
}
function within(root: string, file: string) { const relative = path.relative(root, file); return relative !== '..' && !relative.startsWith(`..${path.sep}`) && !path.isAbsolute(relative); }
export async function deactivate() { await shutdown?.(); shutdown = undefined; }
