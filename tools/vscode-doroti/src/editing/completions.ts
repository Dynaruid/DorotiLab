import * as vscode from 'vscode';
import { ProjectContexts, AppContext } from './projectContext';
import { AssistClient, Analysis, Parameter, CatalogType } from './assistClient';
import { Templates, instantiate, defaultName, isCode, reserved } from './catalog';
import { importPlan, namespaces } from '../imports';

export function parameterSnippet(p: Parameter, source = ''): string {
    const name = reserved.has(p.name) ? '@' + p.name : p.name;
    if (p.delegateParameters) {
        const occupied = new Set(source.match(/[\p{L}\p{Nl}_][\p{L}\p{Nl}\p{Nd}_]*/gu) ?? []);
        const args = p.delegateParameters.map(name => {
            let candidate = name; let suffix = 1;
            while (occupied.has(candidate)) candidate = name + suffix++;
            occupied.add(candidate); return reserved.has(candidate) ? '@' + candidate : candidate;
        }).join(', ');
        const value = p.delegateReturn?.replace(/\?$/, '') === 'global::Doroti.Framework.Widgets.Widget' ? '${1:new global::Doroti.Framework.Widgets.SizedBox()}'
            : p.delegateReturn === 'void' ? '{ $1 }' : '${1:default!}';
        return `${name}: (${args}) => ${value}$0`;
    }
    if (p.name === 'padding') return 'padding: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(${1:8})$0';
    if (p.name === 'child') return 'child: ${1:new global::Doroti.Framework.Widgets.SizedBox()}$0';
    if (p.name === 'children') return 'children: [${1:new global::Doroti.Framework.Widgets.SizedBox()}]$0';
    const value = (p.default ?? 'default!').replace(/[\\$}]/g, '\\$&');
    return `${name}: \${1:${value}}$0`;
}
const hasCSharp = () => !!vscode.extensions.getExtension('ms-dotnettools.csharp')?.isActive;

export class EditingCompletions implements vscode.CompletionItemProvider {
    constructor(private contexts: ProjectContexts, private assist: AssistClient, private templates: Templates, private logs: vscode.OutputChannel) {}
    async provideCompletionItems(doc: vscode.TextDocument, position: vscode.Position, token: vscode.CancellationToken): Promise<vscode.CompletionItem[]> {
        const settings = vscode.workspace.getConfiguration('doroti.editing', doc.uri);
        if (!settings.get('completions', true)) return [];
        const app = await this.contexts.get(doc); if (!app || token.isCancellationRequested) return [];
        const version = doc.version; const source = doc.getText(); const offset = doc.offsetAt(position);
        if (!isCode(source, offset) && (!vscode.workspace.isTrusted || !/^\s*#\s*(if|else|elif)\b/m.test(source))) return [];
        const wordRange = doc.getWordRangeAtPosition(position); const prefix = wordRange ? doc.getText(wordRange).slice(0, offset - doc.offsetAt(wordRange.start)) : '';
        let analysis: Analysis | undefined; let types: CatalogType[] = [];
        if (vscode.workspace.isTrusted) {
            try {
                analysis = await this.assist.request<Analysis>('analyze', app, doc, offset, 0, token);
                if (!analysis.included || !analysis.code) return [];
                if (analysis.semantic && !/^(stl|stf|dstateless|dstateful)/.test(prefix)) types = await this.assist.catalog(app, doc, token);
            } catch (error) { if (error instanceof vscode.CancellationError) return []; this.logs.appendLine(`[editing fallback] ${error}`); }
        }
        if (doc.version !== version || token.isCancellationRequested) return [];
        const globals = analysis?.globalImports ?? [];
        const items: vscode.CompletionItem[] = [];
        if (settings.get('snippets', true)) {
            for (const [name, template] of Object.entries(this.templates)) {
                if (template.package && !app.packages.includes(template.package) && !types.some(t => t.namespace === template.package)) continue;
                if (template.scope === 'class' && analysis?.semantic && analysis.context !== 'declaration') continue;
                if (template.scope === 'member' && !['member', 'state'].includes(analysis?.context ?? '')) continue;
                if (!template.scope && analysis?.semantic && !['body', 'stateBody', 'widget', 'arguments', 'initializer'].includes(analysis.context)) continue;
                if (template.scope === 'state' && analysis?.context !== (name === 'setState' ? 'stateBody' : 'state')) continue;
                if (template.member && analysis?.existingMembers.includes(template.member)) continue;
                if (name.startsWith('Owned ') && analysis?.existingMembers.some(m => ['initState', 'dispose'].includes(m))) continue;
                if (name === 'Owned text controller' && analysis?.existingMembers.includes('_controller') || name === 'Owned scroll controller' && analysis?.existingMembers.includes('_scrollController')) continue;
                if (['Expanded', 'Flexible'].includes(name) && !['Row', 'Column', 'Flex'].includes(analysis?.parent ?? '')) continue;
                const prefixes = Array.isArray(template.prefix) ? template.prefix : [template.prefix];
                for (const alias of prefixes) {
                    if (prefix && !alias.toLowerCase().startsWith(prefix.toLowerCase())) continue;
                    const generated = instantiate(template, source, defaultName(doc.fileName), globals, analysis?.conflicts, !analysis?.semantic);
                    const item = new vscode.CompletionItem(alias, vscode.CompletionItemKind.Snippet);
                    item.detail = `Doroti snippet: ${name}`; item.documentation = template.description;
                    const start = doc.offsetAt(wordRange?.start ?? position);
                    const folded = generated.edits.filter(e => e.start === start).map(e => e.text).join('');
                    item.insertText = new vscode.SnippetString(folded + generated.body.replace('__STATE_TYPE__', analysis?.stateType ?? 'StatefulWidget'));
                    item.range = wordRange ?? new vscode.Range(position, position); item.sortText = '0-' + alias;
                    item.additionalTextEdits = generated.edits.filter(e => e.start !== start).map(e => vscode.TextEdit.insert(doc.positionAt(e.start), e.text));
                    items.push(item);
                }
            }
        }
        for (const parameter of analysis?.arguments ?? []) {
            if (prefix && !parameter.name.startsWith(prefix)) continue;
            const item = new vscode.CompletionItem({ label: `Doroti ${parameter.name}:`, description: parameter.type }, vscode.CompletionItemKind.Snippet);
            item.filterText = parameter.name; item.detail = 'Doroti argument template'; item.range = wordRange;
            item.insertText = new vscode.SnippetString(parameterSnippet(parameter, source)); items.push(item);
        }
        for (const type of types.filter(t => t.kind === 'widget' && t.constructors.length === 1 && !t.fullName.includes('<'))) {
            if (['Expanded', 'Flexible', 'Positioned'].includes(type.name)) continue;
            const alias = 'd' + type.name.toLowerCase();
            if (!prefix || !alias.startsWith(prefix.toLowerCase()) || Object.values(this.templates).some(t => (Array.isArray(t.prefix) ? t.prefix : [t.prefix]).includes(alias))) continue;
            const args = type.constructors[0].filter(p => !p.optional || ['child', 'children', 'builder', 'effectsBuilder'].includes(p.name));
            if (!args.length) continue;
            // Renumber placeholders across the parameter templates, including linked callback names.
            let index = 0;
            const argumentsText = args.map(p => parameterSnippet(p, source).replace(/\$0/g, '').replace(/\$\{1:/g, '${' + (++index) + ':').replace(/\$1\b/g, '$' + index)).join(', ');
            const item = new vscode.CompletionItem(alias, vscode.CompletionItemKind.Snippet);
            item.detail = `Doroti constructor: ${type.fullName}`; item.documentation = type.documentation;
            item.range = wordRange; item.insertText = new vscode.SnippetString(`new ${type.fullName}(${argumentsText})$0`); items.push(item);
        }
        // Basic type/enum IntelliSense belongs to the C# extension when it is running.
        if (!hasCSharp() && settings.get('types', true)) {
            const catalog: (Partial<CatalogType> & { name: string; namespace: string })[] = types.length ? types : Object.entries(namespaces).flatMap(([name, spaces]) => spaces.filter(ns => ns.startsWith('Doroti.Framework.') || ns === 'Doroti.Ui' || app.packages.includes(ns)).map(namespace => ({ name, namespace })));
            for (const type of catalog) {
                if (prefix && !type.name.toLowerCase().startsWith(prefix.toLowerCase())) continue;
                const plan = importPlan(source, type.name, type.namespace, globals);
                const item = new vscode.CompletionItem({ label: type.name, description: type.namespace }, vscode.CompletionItemKind.Class);
                item.detail = `Doroti import: ${type.namespace}`;
                item.range = wordRange;
                item.insertText = !analysis?.semantic || plan.qualified || analysis.conflicts.includes(type.name) ? `global::${type.namespace}.${type.name}` : type.name;
                if (analysis?.semantic && plan.text && !analysis.conflicts.includes(type.name)) {
                    if (plan.offset === doc.offsetAt(wordRange?.start ?? position)) item.insertText = plan.text + item.insertText;
                    else item.additionalTextEdits = [vscode.TextEdit.insert(doc.positionAt(plan.offset), plan.text)];
                }
                items.push(item);
                if (type.values) {
                    for (const value of type.values) {
                        const enumItem = new vscode.CompletionItem(`${type.name}.${value}`, vscode.CompletionItemKind.EnumMember);
                        enumItem.insertText = `global::${type.namespace}.${type.name}.${value}`; enumItem.range = wordRange; items.push(enumItem);
                    }
                    for (const factory of type.factories ?? []) {
                        const match = /\b(Create\w+)\(/.exec(factory); if (!match) continue;
                        const factoryItem = new vscode.CompletionItem(`${type.name}.${match[1]}`, vscode.CompletionItemKind.Method);
                        factoryItem.detail = factory; factoryItem.insertText = new vscode.SnippetString(`global::${type.namespace}.${type.name}.${match[1]}($0)`); factoryItem.range = wordRange; items.push(factoryItem);
                    }
                }
            }
        }
        return items;
    }
}
