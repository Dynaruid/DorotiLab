const fs = require('node:fs');
const path = require('node:path');
const { loadTemplates, instantiate } = require('../dist/editing/catalog');
const templates = loadTemplates(path.resolve(__dirname, '..'));
const output = path.resolve(process.argv[2]);
const declarations = ['namespace TemplateCompilation;'];
let count = 0;
for (const [name, template] of Object.entries(templates)) {
    const className = 'Generated' + (++count);
    let text = instantiate(template, '', className, [], [], true).body.replace('__STATE_TYPE__', 'LifecycleWidget');
    if (template.scope === 'class') text = text.replace(/\$\{1(?::[^}]*)?\}/g, className);
    text = text.replace(/\$\{\d+:([^}]*)\}/g, '$1').replace(/\$\{\d+\}|\$\d+/g, '');
    if (template.scope === 'class') declarations.push(text);
    else if (template.scope === 'state' || name === 'build') declarations.push(`public sealed class ${className} : global::Doroti.Framework.Widgets.State<LifecycleWidget> { ${name === 'setState' ? `private void Change() { ${text} }` : text}\n${name === 'build' ? '' : 'public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context) => new global::Doroti.Framework.Widgets.SizedBox();'} }`);
    else {
        const result = name === 'TextStyle' ? 'global::Doroti.Framework.Painting.TextStyle' : name === 'Decoration' ? 'global::Doroti.Framework.Painting.BoxDecoration' : 'global::Doroti.Framework.Widgets.Widget';
        declarations.push(`public static class ${className} { private static readonly global::Doroti.Framework.Widgets.TextEditingController controller = new(); public static ${result} Create() => ${text}; }`);
    }
}
declarations.push('public sealed class LifecycleWidget : global::Doroti.Framework.Widgets.StatefulWidget { public override global::Doroti.Framework.Widgets.IState createState() => new GeneratedLifecycle(); } public sealed class GeneratedLifecycle : global::Doroti.Framework.Widgets.State<LifecycleWidget> { public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context) => new global::Doroti.Framework.Widgets.SizedBox(); }');
fs.mkdirSync(path.dirname(output), { recursive: true });
fs.writeFileSync(output, declarations.join('\n\n'));
console.log(`Generated ${count} API compilation fixtures: ${output}`);
