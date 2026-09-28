export const namespaces: Record<string, string[]> = {
    Widget: ['Doroti.Framework.Widgets'], StatelessWidget: ['Doroti.Framework.Widgets'], StatefulWidget: ['Doroti.Framework.Widgets'],
    State: ['Doroti.Framework.Widgets'], IState: ['Doroti.Framework.Widgets'], BuildContext: ['Doroti.Framework.Widgets'],
    Row: ['Doroti.Framework.Widgets'], Column: ['Doroti.Framework.Widgets'], Container: ['Doroti.Framework.Widgets'],
    Text: ['Doroti.Framework.Widgets'], SizedBox: ['Doroti.Framework.Widgets'], Stack: ['Doroti.Framework.Widgets'],
    Center: ['Doroti.Framework.Widgets'], TextEditingController: ['Doroti.Framework.Widgets'], ScrollController: ['Doroti.Framework.Widgets'],
    MaterialApp: ['Doroti.Framework.Material'], Scaffold: ['Doroti.Framework.Material'], TextField: ['Doroti.Framework.Material'],
    CupertinoApp: ['Doroti.Framework.Cupertino'], CupertinoPageScaffold: ['Doroti.Framework.Cupertino'], CupertinoTextField: ['Doroti.Framework.Cupertino'],
    Color: ['Doroti.Ui'], Offset: ['Doroti.Ui'], Size: ['Doroti.Ui'], Key: ['Doroti.Framework.Foundation'],
    EdgeInsets: ['Doroti.Framework.Painting'], CrossAxisAlignment: ['Doroti.Framework.Rendering'],
    TextStyle: ['Doroti.Framework.Painting', 'Doroti.Ui'],
};
export function importPlan(source: string, name: string, ns: string, globalImports: string[] = []): { qualified: boolean; text?: string; offset: number } {
    // Preserve line/offset positions; imports inside comments and strings cannot authorize edits.
    const masked = source.replace(/\/\*[\s\S]*?\*\/|\/\/[^\r\n]*|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"/g, s => s.replace(/[^\r\n]/g, ' '));
    const escape = (s: string) => s.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    if (new RegExp(`\\busing\\s+${escape(name)}\\s*=`).test(masked) ||
        new RegExp(`\\b(class|struct|record|interface|enum)\\s+${escape(name)}\\b`).test(masked))
        return { qualified: true, offset: 0 };
    // Only compilation-unit imports: never insert into an unrelated block namespace.
    const prefix = masked.split(/\b(?:namespace|class|struct|record|interface|enum)\b/)[0];
    const usingPattern = new RegExp(`(?:^|\\n)\\s*(?:global\\s+)?using\\s+(?:global::)?${escape(ns)}\\s*;`);
    if (globalImports.some(value => value.startsWith(name + '='))) return { qualified: true, offset: 0 };
    if (globalImports.includes(ns) || usingPattern.test(prefix)) return { qualified: false, offset: 0 };
    // A nested using may belong to a different block/file namespace. Qualify
    // conservatively instead of adding a duplicate or pretending it is in scope.
    if (usingPattern.test(masked) || /^\s*#\s*(if|else|elif)/m.test(prefix)) return { qualified: true, offset: 0 };
    if ((namespaces[name]?.length ?? 0) > 1 && namespaces[name].some(other => other !== ns &&
        (globalImports.includes(other) || new RegExp(`\\busing\\s+${escape(other)}\\s*;`).test(masked))))
        return { qualified: true, offset: 0 };
    const usings = [...prefix.matchAll(/(?:^|\n)[ \t]*(?:global\s+)?using\s+[^;\r\n]+;[^\S\r\n]*(?:\r?\n)?/g)];
    const offset = usings.length ? usings[usings.length - 1].index! + usings[usings.length - 1][0].length : (source.startsWith('\uFEFF') ? 1 : 0);
    return { qualified: false, text: `using ${ns};${source.includes('\r\n') ? '\r\n' : '\n'}`, offset };
}
