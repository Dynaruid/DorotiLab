import * as fs from 'node:fs';
import * as path from 'node:path';
import { namespaces, importPlan } from '../imports';
import { Change } from './assistClient';

export interface Template { prefix: string | string[]; description: string; body: string[]; package?: string; scope?: 'state' | 'class' | 'expression' | 'member'; member?: string; }
export type Templates = Record<string, Template>;
export function loadTemplates(extensionPath: string): Templates { return JSON.parse(fs.readFileSync(path.join(extensionPath, 'snippets/widgets.json'), 'utf8')); }
export const reserved = new Set(('abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while').split(' '));
export function validateIdentifier(name: string): string | undefined {
    return !/^[\p{L}\p{Nl}_][\p{L}\p{Nl}\p{Nd}\p{Mn}\p{Mc}\p{Pc}\p{Cf}]*$/u.test(name) || reserved.has(name) ? 'Enter a non-keyword C# identifier (letters, digits and underscores).' : undefined;
}
export function defaultName(file: string): string {
    const name = path.basename(file, '.cs'); return validateIdentifier(name) ? 'MyWidget' : name;
}
export function instantiate(template: Template, source: string, name: string, globals: string[], conflicts: string[] = [], qualified = false): { body: string; edits: Change[] } {
    const nl = source.includes('\r\n') ? '\r\n' : '\n';
    let body = template.body.join(nl);
    const edits: Change[] = [];
    const additions = new Map<number, Set<string>>();
    for (const [type, spaces] of Object.entries(namespaces)) {
        if (!new RegExp(`\\b${type}\\b`).test(body)) continue;
        const ns = spaces[0];
        const plan = importPlan(source, type, ns, globals);
        if (qualified || plan.qualified || conflicts.includes(type) || template.scope === 'class' && name === type) body = body.replace(new RegExp(`(?<![\\w.:}])${type}\\b`, 'g'), `global::${ns}.${type}`);
        else if (plan.text) {
            let set = additions.get(plan.offset); if (!set) { set = new Set(); additions.set(plan.offset, set); } set.add(plan.text);
        }
    }
    for (const [start, values] of additions) edits.push({ start, length: 0, text: [...values].join('') });
    return { body: body.replace(/\$\{1:MyWidget\}/g, '${1:' + name + '}'), edits };
}

// Used only for template imports and the trust-free fallback. Semantic completions use Roslyn's referenced API catalog.
Object.assign(namespaces, {
    Padding: ['Doroti.Framework.Widgets'], Expanded: ['Doroti.Framework.Widgets'], Flexible: ['Doroti.Framework.Widgets'],
    Align: ['Doroti.Framework.Widgets'], Positioned: ['Doroti.Framework.Widgets'], Builder: ['Doroti.Framework.Widgets'],
    LayoutBuilder: ['Doroti.Framework.Widgets'], ListView: ['Doroti.Framework.Widgets'], ClipRRect: ['Doroti.Framework.Widgets'],
    SingleChildScrollView: ['Doroti.Framework.Widgets'], MainAxisAlignment: ['Doroti.Framework.Rendering'],
    BoxDecoration: ['Doroti.Framework.Painting'], BorderRadius: ['Doroti.Framework.Painting'], Alignment: ['Doroti.Framework.Painting'],
    ElevatedButton: ['Doroti.Material'], CupertinoButton: ['Doroti.Cupertino']
});

// Lexical fallback is intentionally limited to deciding whether templates can be shown; it never transforms syntax.
export function isCode(source: string, offset: number): boolean {
    let i = 0;
    while (i < offset) {
        if (source.startsWith('//', i)) { const end = source.indexOf('\n', i + 2); if (end < 0 || offset <= end) return false; i = end + 1; continue; }
        if (source.startsWith('/*', i)) { const end = source.indexOf('*/', i + 2); if (end < 0 || offset < end + 2) return false; i = end + 2; continue; }
        if (source[i] === '"' || source[i] === "'") {
            const quote = source[i]; const count = quote === '"' ? /^"+/.exec(source.slice(i))![0].length : 1;
            const raw = count >= 3; const verbatim = source[i - 1] === '@' || source.slice(Math.max(0, i - 2), i) === '@$';
            let end = i + (raw ? count : 1);
            if (raw) { const close = source.indexOf('"'.repeat(count), end); end = close < 0 ? source.length + 1 : close + count; }
            else {
                while (end < source.length) {
                    if (!verbatim && source[end] === '\\') { end += 2; continue; }
                    if (source[end] === quote) { if (verbatim && source[end + 1] === quote) { end += 2; continue; } end++; break; }
                    end++;
                }
            }
            if (offset < end) return false; i = end; continue;
        }
        i++;
    }
    // Preprocessor evaluation requires the helper; do not guess active conditional branches in fallback.
    if (/^\s*#\s*(if|elif|else)\b/m.test(source)) return false;
    return true;
}
